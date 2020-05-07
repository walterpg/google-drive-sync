/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * Google Drive Sync for KeePass Password Safe
 * Copyright(C) 2020       Walter Goodwin
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
**/

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using KeePass;
using KeePass.DataExchange;
using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Delegates;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using File = System.IO.File;
using GDriveFile = Google.Apis.Drive.v3.Data.File;

namespace GoogleDriveSync
{
	[Flags]
	public enum AutoSyncMode
	{
		DISABLED = 0,
		SAVE = 1,
		OPEN = 2
	}

	/// <summary>
	/// main plugin class
	/// </summary>
	public sealed class GoogleDriveSyncExt : Plugin, IDriveServiceProvider
	{
		private IPluginHost m_host = null;

		private AutoSyncMode m_autoSync = AutoSyncMode.DISABLED;

		private string m_defaultFolder = null;
		private GoogleColor m_defaultFolderColor = null;
		private string m_defaultDriveScope = null;
		private string m_defaultClientId = string.Empty;
		private ProtectedString m_defaultClientSecret = GdsDefs.PsEmptyEx;

		private ToolStripSeparator m_tsSeparator = null;
		private ToolStripMenuItem m_tsmiPopup = null;
		private ToolStripMenuItem m_tsmiSync = null;
		private ToolStripMenuItem m_tsmiUpload = null;
		private ToolStripMenuItem m_tsmiDownload = null;
		private ToolStripMenuItem m_tsmiConfigure = null;

		GDriveFile.ContentHintsData m_contentInfo;

		// For UI status updates.
		volatile ResumableUpload<GDriveFile, GDriveFile> m_currentUpload = null;
		volatile GDriveFile m_currentDownload = null;

		private enum SyncCommand
		{
			DOWNLOAD = 1,
			SYNC = 2,
			UPLOAD = 3
		}

		/// <summary>
		/// URL of a version information file
		/// </summary>
		public override string UpdateUrl
		{
			get
			{
				return GdsDefs.UpdateUrl;
			}
		}

		/// <summary>
		/// The <c>Initialize</c> function is called by KeePass when
		/// you should initialize your plugin (create menu items, etc.).
		/// </summary>
		/// <param name="host">Plugin host interface. By using this
		/// interface, you can access the KeePass main window and the
		/// currently opened database.</param>
		/// <returns>You must return <c>true</c> in order to signal
		/// successful initialization. If you return <c>false</c>,
		/// KeePass unloads your plugin (without calling the
		/// <c>Terminate</c> function of your plugin).</returns>
		public override bool Initialize(IPluginHost host)
		{
			if (host == null)
			{
				return false;
			}

			UpdateCheckEx.SetFileSigKey(UpdateUrl, Images.PubKey);

			m_host = host;
			string syncOption = m_host.GetConfig(GdsDefs.ConfigAutoSync,
											AutoSyncMode.DISABLED.ToString());
			if (!Enum.TryParse(syncOption, out m_autoSync))
			{
				// Support obsolete Sync on Save confg.
				if (m_host.GetConfig(GdsDefs.ConfigAutoSync, false))
				{
					m_autoSync = AutoSyncMode.SAVE;
				}
				else
				{
					m_autoSync = AutoSyncMode.DISABLED;
				}
			}

			// The default setting is to use the My Drive root folder.
			m_defaultFolder = m_host.GetConfig(GdsDefs.ConfigDefaultAppFolder,
									GdsDefs.AppDefaultFolderName);
			string encodedColor = m_host.GetConfig(GdsDefs.ConfigDefaultAppFolderColor);
			m_defaultFolderColor = !string.IsNullOrEmpty(encodedColor) ?
				GoogleColor.DeserializeFromString(encodedColor) : null;

			// The default setting is to use the less restrictive API scope.
			m_defaultDriveScope = m_host.GetConfig(GdsDefs.ConfigDriveScope,
									DriveService.Scope.DriveFile);

			// Default is no OAuth 2.0 credentials.
			m_defaultClientId = m_host.GetConfig(GdsDefs.ConfigDefaultClientId,
												string.Empty);
			string secretVal = m_host.GetConfig(GdsDefs.ConfigDefaultClientSecret,
												string.Empty);
			m_defaultClientSecret = new ProtectedString(true, secretVal);

			// Get a reference to the 'Tools' menu item container
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;

			m_contentInfo = null;

			// Add a separator at the bottom
			m_tsSeparator = new ToolStripSeparator();
			tsMenu.Add(m_tsSeparator);

			// Add the popup menu item
			m_tsmiPopup = new ToolStripMenuItem
			{
				Text = GdsDefs.ProductName,
				Image = Resources.GetBitmap("google_signin_light")
			};
			tsMenu.Add(m_tsmiPopup);

			m_tsmiSync = new ToolStripMenuItem
			{
				Name = "Sync",
				Tag = SyncCommand.SYNC,
				Text = Resources.GetString("MenuLabel_Sync"),
				Image = Resources.GetBitmap("round_sync_black_18dp")
			};
			m_tsmiSync.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiSync);

			m_tsmiUpload = new ToolStripMenuItem
			{
				Name = "Upload",
				Tag = SyncCommand.UPLOAD,
				Text = Resources.GetString("MenuLabel_Upload"),
				Image = Resources.GetBitmap("round_cloud_upload_black_18dp")
			};
			m_tsmiUpload.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiUpload);

			m_tsmiDownload = new ToolStripMenuItem
			{
				Name = "Download",
				Tag = SyncCommand.DOWNLOAD,
				Text = Resources.GetString("MenuLabel_Download"),
				Image = Resources.GetBitmap("round_cloud_download_black_18dp")
			};
			m_tsmiDownload.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiDownload);

			m_tsmiConfigure = new ToolStripMenuItem
			{
				Name = "CONFIG",
				Text = Resources.GetString("MenuLabel_Config"),
				Image = Resources.GetBitmap("round_settings_black_18dp")
			};
			m_tsmiConfigure.Click += OnConfigure;
			m_tsmiPopup.DropDownItems.Add(m_tsmiConfigure);

			// We want a notification when the user tried to save the
			// current database or opened a new one.
			m_host.MainWindow.FileSaved += OnFileSaved;
			m_host.MainWindow.FileOpened += OnFileOpened;

			return true; // Initialization successful
		}

		/// <summary>
		/// The <c>Terminate</c> function is called by KeePass when
		/// you should free all resources, close open files/streams,
		/// etc. It is also recommended that you remove all your
		/// plugin menu items from the KeePass menu.
		/// </summary>
		public override void Terminate()
		{
			// Remove all of our menu items
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;
			tsMenu.Remove(m_tsmiPopup);
			tsMenu.Remove(m_tsSeparator);
			tsMenu.Remove(m_tsmiSync);
			tsMenu.Remove(m_tsmiUpload);
			tsMenu.Remove(m_tsmiDownload);
			tsMenu.Remove(m_tsmiConfigure);

			m_tsmiPopup.Dispose();
			m_tsSeparator.Dispose();
			m_tsmiSync.Dispose();
			m_tsmiUpload.Dispose();
			m_tsmiDownload.Dispose();
			m_tsmiConfigure.Dispose();

			m_tsmiPopup = null;
			m_tsSeparator = null;
			m_tsmiSync = null;
			m_tsmiUpload = null;
			m_tsmiDownload = null;
			m_tsmiConfigure = null;

			m_host.MainWindow.FileSaved -= OnFileSaved;
			m_host.MainWindow.FileOpened -= OnFileOpened;
		}

		// File thumbnail image.
		private GDriveFile.ContentHintsData ContentHints
		{
			get
			{
				if (m_contentInfo != null)
				{
					return m_contentInfo;
				}

				// GDrive requires this be sent with every upload/update
				// for binary files, so build it here once.
				using (MemoryStream stream = Resources.GetImageStream(
														"keepass_thumbnail",
														ImageFormat.Png))
				using (BinaryReader reader = new BinaryReader(stream))
				{
					m_contentInfo = new GDriveFile.ContentHintsData()
					{
						Thumbnail = new GDriveFile.ContentHintsData.ThumbnailData()
						{
							MimeType = "image/png",
							Image = reader.ReadBytes((int)stream.Length)
										.ToUrlSafeBase64()
						}
					};
				}
				return m_contentInfo;
			}
		}

		private async Task SyncOnOpenOrSaveCmd()
		{
			if (Keys.Shift == (Control.ModifierKeys & Keys.Shift))
			{
				ShowMessage(Resources.GetString("Msg_AutoSyncIgnore"), true);
			}
			else if (LoadConfiguration() == null)
			{
				ShowMessage(Resources.GetString("Msg_NoAutoSyncConfig"), true);
			}
			else
			{
				await ConfigAndSyncWithGoogle(SyncCommand.SYNC, true);
			}
		}

		/// <summary>
		/// Event handler to implement auto sync on save
		/// </summary>
		private async void OnFileSaved(object sender, FileSavedEventArgs e)
		{
			if (e.Success && 
				AutoSyncMode.SAVE == (m_autoSync & AutoSyncMode.SAVE))
			{
				await SyncOnOpenOrSaveCmd();
			}
		}

		/// <summary>
		/// Event handler to implement auto sync on open
		/// </summary>
		private async void OnFileOpened(object sender, FileOpenedEventArgs e)
		{
			if (AutoSyncMode.OPEN == (m_autoSync & AutoSyncMode.OPEN))
			{
				await SyncOnOpenOrSaveCmd();
			}
		}

		/// <summary>
		/// Event handler for sync menu entries
		/// </summary>
		private async void OnSyncWithGoogle(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			SyncCommand syncCommand = (SyncCommand)item.Tag;
			await ConfigAndSyncWithGoogle(syncCommand, false);
		}

		/// <summary>
		/// Event handler for configuration menu entry
		/// </summary>
		private void OnConfigure(object sender, EventArgs e)
		{
			if (!m_host.Database.IsOpen)
			{
				ShowMessage(Resources.GetString("Msg_FirstOpenDb"));
			}
			else
			{
				EntryConfiguration config = AskForConfiguration();
				if (config != null)
				{
					SaveConfiguration(config);
				}
			}
		}

		void NotifyTokenError(TokenResponseException ex)
		{
			string msg;
			switch (ex.Error.Error)
			{
				case "access_denied":
					msg = Resources.GetString("Err_TokenAccessDenied");
					break;
				case "invalid_request":
					msg = Resources.GetString("Err_TokenInvalidReq");
					break;
				case "invalid_client":
					msg = Resources.GetString("Err_TokenInvalidClient");
					break;
				case "invalid_grant":
					msg = Resources.GetString("Err_TokenInvalidGrant");
					break;
				case "unauthorized_client":
					msg = Resources.GetString("Err_TokenUnauthClient");
					break;
				case "unsupported_grant_type":
					msg = Resources.GetString("Err_TokenUnsupportedGrant");
					break;
				case "invalid_scope":
					msg = Resources.GetString("Err_TokenInvalidScope");
					break;
				case "user_cancelled":
					msg = Resources.GetFormat("Msg_UserCancelledOpFmt", GdsDefs.ProductName);
					ShowMessage(msg, true);
					return;
				default:
					msg = ex.Message;
					break;
			}
			ShowMessage(msg);
		}

		private void SaveDatabase()
		{
			if (!m_host.Database.Modified)
			{
				return;
			}

			MainForm window = m_host.MainWindow;
			if (window.InvokeRequired)
			{
				window.Invoke(new MethodInvoker(() => SaveDatabase()));
				return;
			}
			ShowWarningsLogger logger = window.CreateShowWarningsLogger();
			string status = Resources.GetString("Msg_SavingDatabase");
			try
			{
				logger.StartLogging(status, true);
				m_host.Database.Save(logger);
			}
			finally
			{
				status = Resources.GetString("Msg_DatabaseSaved");
				logger.SetText(status, LogStatusType.Info);
				logger.EndLogging();
			}
		}

		/// <summary>
		/// Authenticate and authorize the drive service and invoke a user
		/// function. Obtain and update authorization details from/to the
		/// current configuration.
		/// </summary>
		public async Task<string> ConfigAndUseDriveService(
						Func<DriveService, string, Task<string>> use)
		{
			if (!m_host.Database.IsOpen)
			{
				return Resources.GetString("Msg_FirstOpenDb");
			}
			else if (!m_host.Database.IOConnectionInfo.IsLocalFile())
			{
				return Resources.GetString("Msg_LocalDbOnly");
			}

			// Ensure the configuration with auth data.
			EntryConfiguration config = GetConfiguration();
			if (config == null)
			{
				return Resources.GetFormat("Msg_ProductAbortedFmt",
											GdsDefs.ProductName);
			}

			// Save reference to current token.
			ProtectedString RefreshToken = config.RefreshToken;

			// Invoke service user.
			string status = await UseDriveService(config, use);

			// Update the configuration if necessary.
			if (status != "ERROR" &&
				(RefreshToken == null ||
				 !RefreshToken.OrdinalEquals(config.RefreshToken, true)))
			{
				// An access token was granted.
				// If there is no saved token, or the saved token is
				// different than the granted token, save it to the
				// database entry.
				string status2 = Resources.GetString("Msg_SaveUserAuth");
				ShowMessage(status2, true);


				// Traditionally, the plugin's indicator for "use default 
				// clientId" is empty strings for clientId & secret.  Maintain
				// that compatibility point.
				if (GdsDefs.DefaultClientId.ReadString() == config.ClientId &&
					 GdsDefs.DefaultClientSecret
						.OrdinalEquals(config.ClientSecret, true))
				{
					config.ClientId = string.Empty;
					config.ClientSecret = null;
				}

				config.CommitChangesIfAny();
				SaveConfiguration(config);
			}
			return status;
		}

		public async Task<string> UseDriveService(SyncConfiguration authData,
						Func<DriveService, string, Task<string>> use)
		{
			string status;
			try
			{
				Tuple<UserCredential, ProtectedString> credAndToken;
				credAndToken = await Task.Run(() => GetAuthorization(m_host, authData));
				if (credAndToken.Item2 != null)
				{
					// Save the new refresh token.
					authData.RefreshToken = credAndToken.Item2;
				}

				// Create the drive service and call the caller.
				DriveService service = new DriveService(new BaseClientService.Initializer()
				{
					HttpClientInitializer = credAndToken.Item1,
					ApplicationName = GdsDefs.ProductName
				});
				status = await use(service, authData.ActiveFolder);
			}
			catch (TokenResponseException ex)
			{
				status = "ERROR";
				NotifyTokenError(ex);
			}
			catch (AggregateException ex)
			{
				status = "ERROR";
				foreach (Exception inner in ex.InnerExceptions)
				{
					TokenResponseException tokenExc = inner as TokenResponseException;
					if (tokenExc != null)
					{
						NotifyTokenError(tokenExc);
					}
					else
					{
						ShowMessage(inner.Message);
					}
				}
			}
			catch (Exception ex)
			{
				status = "ERROR";
				ShowMessage(ex.Message);
			}
			return status;
		}

		/// <summary>
		/// Configure if necessary, then execute synchronization of the current
		/// database with Google Drive. Create a new file if it does not already
		/// exist.
		/// </summary>
		private async Task ConfigAndSyncWithGoogle(SyncCommand syncCommand,
			bool autoSync)
		{
			// Suspend these events temporarily in case the configuration
			// needs to be saved.
			m_host.MainWindow.FileSaved -= OnFileSaved;
			m_host.MainWindow.FileOpened -= OnFileOpened;
			m_host.MainWindow.Enabled = false;

			try
			{
				await ConfigAndSyncUnsafe(syncCommand, autoSync);
			}
			finally
			{
				m_host.MainWindow.Enabled = true;
				m_host.MainWindow.FileSaved += OnFileSaved;
				m_host.MainWindow.FileOpened += OnFileOpened;
			}
		}

		private async Task ConfigAndSyncUnsafe(SyncCommand sync, bool autoSync)
		{
			string status = Resources.GetString("Msg_PleaseWaitEllipsis");
			ShowMessage(status, true);

			status = await ConfigAndUseDriveService(async (service, targetFolder) =>
			{
				string filePath = m_host.Database.IOConnectionInfo.Path;
				string contentType = "application/x-keepass2";
				status = null;

				GDriveFile folder = await GetFolder(service, targetFolder);
				GDriveFile file = await GetFile(service, folder, filePath);
				if (file == null)
				{
					if (sync == SyncCommand.DOWNLOAD)
					{
						status = Resources.GetString("Msg_GDriveFileNotFound");
					}
					else // upload
					{
						if (!autoSync)
						{
							SaveDatabase();
						}
						status = await UploadFile(service, folder,
							Resources.GetString("Descr_KpDbFile"),
							string.Empty, contentType, filePath,
							ContentHints);
					}
				}
				else
				{
					if (sync == SyncCommand.UPLOAD)
					{
						if (!autoSync)
						{
							SaveDatabase();
						}
						status = Resources.GetFormat("Msg_ReplaceFileFmt", file.Name);
						ShowMessage(status, true);
						status = await UpdateFile(service, file,
							filePath, contentType, ContentHints);
					}
					else
					{
						string fileCopy = await DownloadCopy(service, file, filePath);
						if (!string.IsNullOrEmpty(fileCopy))
						{
							if (sync == SyncCommand.DOWNLOAD)
							{
								status = await ReplaceDatabase(filePath, fileCopy);
							}
							else
							{
								string syncStatus = SyncFromThenDeleteFile(fileCopy);
								status = Resources.GetString("Msg_UploadingSync");
								ShowMessage(status, true);
								status = String.Format("{0} {1}", syncStatus,
									await UpdateFile(service, file, filePath,
													contentType, ContentHints));
							}
						}
						else
						{
							status = Resources.GetString("Msg_FileNotDownloaded");
						}
					}
				}
				return status;
			});

			m_host.MainWindow.UpdateUI(false, null, true, null, true, null, false);
			if (!string.IsNullOrEmpty(status) &&
				status != "ERROR")
			{
				ShowMessage(status, true);
			}
		}

		/// <summary>
		/// Download the Drive file and save to a file adjacent to database but
		/// with a unique name.
		/// </summary>
		/// <param name="service">The Google Drive service</param>
		/// <param name="file">The Google Drive File instance</param>
		/// <param name="filePath">The local file name and path to download to (timestamp will be appended)</param>
		/// <returns>File's path if successful, null or empty otherwise.</returns>
		private async Task<string> DownloadCopy(DriveService service,
			GDriveFile file, string filePath)
		{
			if (file == null || String.IsNullOrEmpty(file.Id) ||
				string.IsNullOrEmpty(filePath))
			{
				return null;
			}

			string status = Resources.GetFormat("Msg_DownloadFileFmt", file.Name);
			ShowMessage(status, true);

			FilesResource.GetRequest request = service.Files.Get(file.Id);
			request.MediaDownloader.ProgressChanged += DownloadProgressChanged;
			request.MediaDownloader.ChunkSize = GdsDefs.DefaultDotNetFileBufferSize;
			string downloadFilePath = Path.GetTempFileName();
			using (FileStream fileStream = new FileStream(downloadFilePath, 
					FileMode.Create, FileAccess.Write, FileShare.None,
					GdsDefs.DefaultDotNetFileBufferSize, true))
			{
				try
				{
					m_currentDownload = file;
					await request.DownloadAsync(fileStream);
				}
				finally
				{
					m_currentDownload = null;
				}
			}
			return downloadFilePath;
		}

		private void DownloadProgressChanged(IDownloadProgress obj)
		{
			MainForm window = m_host.MainWindow;
			if (window.InvokeRequired)
			{
				window.BeginInvoke(new MethodInvoker(() =>
				{
					DownloadProgressChanged(obj);
				}));
				return;
			}
			ToolStripProgressBar progBar = window.MainProgressBar;
			switch (obj.Status)
			{
				case DownloadStatus.Downloading:
					GDriveFile dnload = m_currentDownload;
					progBar.Minimum = 0;
					progBar.Maximum = dnload == null || !dnload.Size.HasValue ? int.MaxValue :
						(int)Math.Min((long)int.MaxValue, dnload.Size.Value);
					progBar.Value = (int)Math.Min((long)progBar.Maximum, obj.BytesDownloaded);
					progBar.Visible = true;
					break;
				case DownloadStatus.Completed:
					progBar.Value = progBar.Maximum;
					Thread.Sleep(10);
					progBar.Visible = false;
					break;
				case DownloadStatus.Failed:
					progBar.Visible = false;
					break;
			}
		}

		private async Task<GDriveFile> GetFolder(DriveService service, string folderName)
		{
			if (string.IsNullOrEmpty(folderName))
			{
				return new GDriveFile()
				{
					Id = "root" // alias for root folder in v3
				};
			}

			string status;
			status = Resources.GetFormat("Msg_RetrievingFolderFmt",
													folderName);
			ShowMessage(status, true);

			// Only look for root-level folders.
			FilesResource.ListRequest req = service.Files.List();
			req.Q = "mimeType='" + GdsDefs.FolderMimeType + "' and name='" +
					folderName.Replace("'", "\\'") + 
					"' and 'root' in parents and trashed=false";
			FileList appFolders = await req.ExecuteAsync();
			if (appFolders.Files.Count == 1)
			{
				return appFolders.Files[0];
			}
			if (appFolders.Files.Count > 1)
			{
				status = Resources.GetFormat("Exc_MultipleFoldersFmt",
									appFolders.Files[0].Name);
				throw new PlgxException(status);
			}

			// Create the app folder within our scope.
			GDriveFile folderMetadata = new GDriveFile()
			{
				Name = folderName,
				MimeType = GdsDefs.FolderMimeType
			};
			if (m_defaultFolderColor != null)
			{
				folderMetadata.FolderColorRgb = m_defaultFolderColor.HtmlHexString;
			}
			FilesResource.CreateRequest folderCreate;
			folderCreate = service.Files.Create(folderMetadata);
			folderCreate.Fields = "id";
			return await folderCreate.ExecuteAsync();
		}

		/// <summary>
		/// Get File from Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="folder">Folder containing file (may be null).</param>
		/// <param name="filepath">Full path of the current database file</param>
		/// <returns>Return Google File</returns>
		private async Task<GDriveFile> GetFile(DriveService service,
			GDriveFile folder, string filepath)
		{
			string filename = Path.GetFileName(filepath);
			string status = Resources.GetFormat("Msg_RetrievingGDriveFileFmt", filename);
			ShowMessage(status, true);

			FilesResource.ListRequest req = service.Files.List();
			req.Q = "name='" + filename.Replace("'", "\\'") + "' and '" + 
				folder.Id + "' in parents and trashed=false";
			req.Fields = "files(id,name,size)";
			FileList filesObj = await req.ExecuteAsync();

			IEnumerable<GDriveFile> files = filesObj.Files;
			if (!files.Any())
			{
				return null;
			}
			else if (files.Count() > 1)
			{
				status = Resources.GetFormat("Exc_MultipleGDriveFilesFmt", filename);
				throw new PlgxException(status);
			}
			return files.First();
		}

		/// <summary>
		/// Sync given File with currently open Database file
		/// </summary>
		/// <param name="tempFilePath">Full path of database file to sync with</param>
		/// <returns>Return status of the update</returns>
		private string SyncFromThenDeleteFile(string tempFilePath)
		{
			string status = Resources.GetString("Msg_Synchronizing");
			ShowMessage(status, true);

			IOConnectionInfo connection = IOConnectionInfo.FromPath(tempFilePath);

			Form fParent = m_host.MainWindow;
			IUIOperations uiOps = m_host.MainWindow;
			bool? success = ImportUtil.Synchronize(m_host.Database, uiOps,
												connection, true, fParent);

			// Delete the file.
			Task.Run(() =>
			{
				File.Delete(tempFilePath);
			});

			if (!success.HasValue)
			{
				throw new PlgxException(Resources.GetString("Exc_NoImportPermission"));
			}
			if (!success.Value)
			{
				throw new PlgxException(Resources.GetString("Exc_SyncFailureOther"));
			}
			return Resources.GetString("Msg_LocalSyncComplete");
		}

		/// <summary>
		/// Replace contents of the Google Drive File with currently open Database file
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="file">File from Google Drive</param>
		/// <param name="filePath">Full path of the current database file</param>
		/// <param name="contentType">Content type of the Database file</param>
		/// <returns>Return status of the update</returns>
		private async Task<string> UpdateFile(DriveService service,
			GDriveFile file, string filePath, string contentType,
			GDriveFile.ContentHintsData thumbnailImage)
		{
			GDriveFile temp = new GDriveFile()
			{
				// "Thumbnails are invalidated each time the content of the 
				// file changes..."
				ContentHints = thumbnailImage
			};
			GDriveFile updatedFile = await UploadFileImpl(
				s => service.Files.Update(temp, file.Id, s, contentType),
				filePath);

			return Resources.GetFormat("Msg_FileUpdatedFmt", updatedFile.Name, updatedFile.Id);
		}

		/// <summary>
		/// Upload a new file to Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="description">File description</param>
		/// <param name="fileName">File name</param>
		/// <param name="contentType">File content type</param>
		/// <param name="filepath">Full path of the current database file</param>
		/// <returns>Return status of the upload</returns>
		private async Task<string> UploadFile(DriveService service, GDriveFile folder,
			string description, string fileName, string contentType, string filePath,
			GDriveFile.ContentHintsData thumbnailImage)
		{
			GDriveFile temp = new GDriveFile()
			{
				Description = description,
				MimeType = contentType,
				Name = string.IsNullOrEmpty(fileName) ?
					Path.GetFileName(filePath) : fileName,
				Parents = new List<string>(new[] { folder.Id }),
				ContentHints = thumbnailImage
			};

			string message = Resources.GetFormat("Msg_UploadingFileFmt", temp.Name);
			ShowMessage(message, true);

			GDriveFile file = await UploadFileImpl(
				s => service.Files.Create(temp, s, contentType),
				filePath);

			return Resources.GetFormat("Msg_FileUploadedFmt", file.Name, file.Id);
		}

		private async Task<GDriveFile> UploadFileImpl(
			Func<Stream, ResumableUpload<GDriveFile, GDriveFile>> uploadFactory,
			string filePath)
		{
			using (FileStream stream = new FileStream(filePath,
					FileMode.Open, FileAccess.Read,
					FileShare.Read, GdsDefs.DefaultDotNetFileBufferSize, true))
			{
				m_currentUpload = uploadFactory(stream);
				m_currentUpload.ChunkSize = Math.Max(GdsDefs.DefaultDotNetFileBufferSize,
					FilesResource.CreateMediaUpload.MinimumChunkSize);
				m_currentUpload.ProgressChanged += UploadProgressChanged;
				IUploadProgress progress = await m_currentUpload.UploadAsync();
				ResumableUpload<GDriveFile, GDriveFile> request = m_currentUpload;
				m_currentUpload = null;
				if (progress.Exception != null)
				{
					throw progress.Exception;
				}
				return request.ResponseBody;
			}
		}

		private void UploadProgressChanged(IUploadProgress obj)
		{
			MainForm window = m_host.MainWindow;
			if (window.InvokeRequired)
			{
				window.BeginInvoke(new MethodInvoker(() =>
				{
					UploadProgressChanged(obj);
				}));
				return;
			}
			ToolStripProgressBar progBar = window.MainProgressBar;
			switch (obj.Status)
			{
				case UploadStatus.Starting:
				case UploadStatus.Uploading:
					ResumableUpload<GDriveFile, GDriveFile> upload = m_currentUpload;
					progBar.Minimum = 0;
					progBar.Maximum = upload == null ? int.MaxValue :
						(int) Math.Min((long)int.MaxValue, upload.ContentStream.Length);
					progBar.Value = (int) Math.Min((long)progBar.Maximum, obj.BytesSent);
					progBar.Visible = true;
					break;
				case UploadStatus.Completed:
					progBar.Value = progBar.Maximum;
					Thread.Sleep(10);
					progBar.Visible = false;
					break;
				case UploadStatus.Failed:
					progBar.Visible = false;
					break;
			}
		}

		/// <summary>
		/// Replace the current database file with a new file and open it
		/// </summary>
		/// <param name="downloadFilePath">Full path of the new database file</param>
		/// <returns>Status of the replacement</returns>
		private async Task<string> ReplaceDatabase(string currentFilePath, string downloadFilePath)
		{
			string tempFilePath = currentFilePath + GdsDefs.GsyncBackupExt;

			string status = Resources.GetString("Msg_TempClose");
			ShowMessage(status, true);

			KeePassLib.Keys.CompositeKey pwKey = m_host.Database.MasterKey;
			m_host.Database.Close();

			status = Resources.GetString("Msg_ReplacingFile");
			ShowMessage(status, true);

			string returnStatus = await Task.Run(() =>
			{
				try
				{
					File.Move(currentFilePath, tempFilePath);
					File.Move(downloadFilePath, currentFilePath);
					File.Delete(tempFilePath);
					return Resources.GetFormat("Msg_ReplaceDbStatusFmt",
								Path.GetFileName(currentFilePath));
				}
				catch
				{
					return Resources.GetFormat("Msg_ReplaceDbFailedFmt",
								Path.GetFileName(downloadFilePath));
				}
			});

			status = Resources.GetString("Msg_ReopenDatabase");
			ShowMessage(status, true);

			try
			{
				// Re-open the database with the prior key.
				m_host.Database.Open(IOConnectionInfo.FromPath(currentFilePath),
										pwKey, new NullStatusLogger());
			}
			catch (KeePassLib.Keys.InvalidCompositeKeyException)
			{
				// The key is different in the downloaded file, so allow user
				// to open it via dialog.
				m_host.MainWindow.OpenDatabase(
					IOConnectionInfo.FromPath(currentFilePath), null, true);
			}

			return returnStatus;
		}

		/// <summary>
		/// Get Access Token from Google OAuth 2.0 API
		/// </summary>
		/// <returns>The Sign-in credentials (access token) and the refresh token</returns>
		private static async Task<Tuple<UserCredential, ProtectedString>> 
			GetAuthorization(IPluginHost host, SyncConfiguration config)
		{
			// Set up the Installed App OAuth 2.0 Flow for Google APIs with a
			// custom code receiver that uses the system browser to 
			// authenticate the Google user and/or authorize the use of the
			// API by this program.
			GoogleAuthorizationCodeFlow.Initializer init;
			init = new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets()
				{
					ClientId = config.ClientId,
					ClientSecret = config.ClientSecret.ReadString().Trim()
				},
				Scopes = new[] { config.DriveScope }
			};
			GoogleAuthorizationCodeFlow codeFlow = new GoogleAuthorizationCodeFlow(init);
			NativeCodeReceiver codeReceiver = new NativeCodeReceiver(host, config);
			AuthorizationCodeInstalledApp authCode;
			authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);
			UserCredential credential = null;

			string status;
			if (config.RefreshToken != null && !config.RefreshToken.IsEmpty)
			{
				// Try using an existing Refresh Token to get a new Access Token
				status = Resources.GetString("Msg_RefreshTokenAuth");
				host.ShowMessage(status, true);

				try
				{
					TokenResponse token;
					string refreshString = config.RefreshToken.ReadString();
					token = await authCode.Flow.RefreshTokenAsync("user",
														refreshString,
														CancellationToken.None);
					credential = new UserCredential(codeFlow, "user", token);
				}
				catch (TokenResponseException ex)
				{
					switch (ex.Error.Error)
					{
						case "invalid_grant":
							// Refresh Token is invalid. Get user authorization
							// below.
							credential = null;
							break;
						default:
							throw;
					}
				}
			}

			ProtectedString newToken = null;
			if (credential == null ||
				string.IsNullOrEmpty(credential.Token.AccessToken))
			{
				// There is no saved authorization, so get the user to
				// authorize the access to Drive.

				status = Resources.GetString("Msg_UserAuth");
				host.ShowMessage(status, true);

				credential = await authCode.AuthorizeAsync("user",
												CancellationToken.None);

				if (credential != null &&
					!string.IsNullOrEmpty(credential.Token.RefreshToken) &&
					(config.RefreshToken == null || 
					 credential.Token.RefreshToken != config.RefreshToken.ReadString()))
				{
					newToken = new ProtectedString(true, credential.Token.RefreshToken);
				}
			}
			return Tuple.Create(credential, newToken);
		}

		/// <summary>
		/// Find active configured Google Accounts, avoiding the recycle bin.
		/// (Should only return one account.)
		/// </summary>
		private IEnumerable<EntryConfiguration> FindActiveAccounts()
		{
			if (!m_host.Database.IsOpen)
			{
				return Enumerable.Empty<EntryConfiguration>();
			}

			List<EntryConfiguration> accounts = new List<EntryConfiguration>();
			PwUuid recycler = m_host.Database.RecycleBinUuid;
			m_host.Database.RootGroup.TraverseTree(TraversalMethod.PreOrder,
				null,
				e =>
				{
					if (e.ParentGroup.Uuid.Equals(recycler))
					{
						// Ignore anything in the recycle bin.
						return true;
					}
					EntryConfiguration entry = new EntryConfiguration(e);
					if (entry.ActiveAccount.GetValueOrDefault(false))
					{
						accounts.Add(entry);
					}
					return true;
				});
			return accounts;
		}

		// Tile over the main forms when StartPosition == Manual.
		internal static DialogResult ShowModalDialogAndDestroy(Form f)
		{
			if (Program.MainForm.InvokeRequired ||
				f.InvokeRequired != Program.MainForm.InvokeRequired)
			{
				Debug.Fail("must create form and call this method on the UI thread");
				return DialogResult.Abort;
			}

			f.Load += (o, e) =>
			{
				if (f.StartPosition != FormStartPosition.Manual)
				{
					return;
				}
				Form main = f.Owner;
				int cOtherForms = main.OwnedForms
								.Except(new[] { f })
								.Count();
				int offset = cOtherForms * 38;
				Point p = new Point(
					main.Left+main.Width/2 -f.Width/2 +offset,
					main.Top +main.Height/2-f.Height/2+offset);
				f.Location = p;
			};

			DialogResult dr = f.ShowDialog(Program.MainForm);
			UIUtil.DestroyForm(f);
			return dr;
		}

		async Task<IEnumerable<Color>> GetColors(SyncConfiguration authData)
		{
			SyncConfiguration originalAuthData = null;
			if (string.IsNullOrEmpty(authData.ClientId) &&
				GdsDefs.PsEmptyEx.OrdinalEquals(authData.ClientSecret, true))
			{
				originalAuthData = authData;
				authData = new TransientConfiguration(authData)
				{
					ClientId = GdsDefs.DefaultClientId.ReadString(),
					ClientSecret = GdsDefs.DefaultClientSecret
				};
			}

			int[] palette = new int[0];

			string status;
			status = await UseDriveService(authData, async (service, appfolder) =>
			{
				AboutResource aboutResource = new AboutResource(service);
				AboutResource.GetRequest req = aboutResource.Get();
				req.Fields = "folderColorPalette";
				About rsc = await req.ExecuteAsync();
				palette = rsc.FolderColorPalette.Cast<string>()
								.Select(s => s.Substring(1))
								.Select(s => int.Parse(s, System.Globalization.NumberStyles.HexNumber))
								.ToArray();
				return Resources.GetString("Msg_FolderColorsRetrieved");
			});

			ShowMessage(status, true);

			if (originalAuthData != null &&
				(authData.RefreshToken == null || originalAuthData.RefreshToken == null ||
				 !authData.RefreshToken.OrdinalEquals(originalAuthData.RefreshToken, true)))
			{
				// Copy new refresh token for caller.
				originalAuthData.RefreshToken = authData.RefreshToken;
			}

			return palette.Select(i => Color.FromArgb((int)(0xFF000000|(uint)i)));
		}

		/// <summary>
		/// Show the configuration form
		/// </summary>
		private EntryConfiguration AskForConfiguration()
		{
			if (!m_host.Database.IsOpen)
			{
				return null;
			}

			// Plugin entries, this and legacy, have the google accounts
			// url fragment in their URL field (and apparently, legacy entries
			// might also have this in the title field).  Get a list of all
			// such entries to populate the accounts drop-down in the dialog.
			// Later, respect the very obsolete PwUuid-based configuration
			// option used in very old clients.
			List<EntryConfiguration> acctList = new List<EntryConfiguration>();
			PwGroup root = m_host.Database.RootGroup;
			PwUuid recyclerID = m_host.Database.RecycleBinUuid;
			root.TraverseTree(TraversalMethod.PreOrder, null, e =>
			{
				if (e.ParentGroup.Uuid.Equals(recyclerID))
				{
					return true;
				}
				if (-1 != e.Strings.ReadSafe(PwDefs.UrlField)
								.IndexOf(GdsDefs.AccountSearchString,
									StringComparison.OrdinalIgnoreCase))
				{
					acctList.Add(new EntryConfiguration(e));
				}
				else if(-1 != e.Strings.ReadSafe(PwDefs.TitleField)
								.IndexOf(GdsDefs.AccountSearchString,
									StringComparison.OrdinalIgnoreCase))
				{
					acctList.Add(new EntryConfiguration(e));
				}
				return true;
			});

			// Create a "presentation" object for dialog data binding.
			ConfigurationFormData options;
			options = new ConfigurationFormData(acctList, GetColors)
			{
				AutoSync = m_autoSync,
				DefaultApiScope = m_defaultDriveScope,
				DefaultClientId = m_defaultClientId,
				DefaultClientSecret = m_defaultClientSecret,
				DefaultUseLegacyClientId = 
					SyncConfiguration.IsDefaultOauthCredential(
													m_defaultClientId,
													m_defaultClientSecret),
				DefaultAppFolder = m_defaultFolder,
				DefaultAppFolderColor = m_defaultFolderColor
			};
			ConfigurationForm optionsForm = new ConfigurationForm(options)
			{
				DatabaseFilePath = m_host.Database.IOConnectionInfo.Path
			};
			using (options)
			using (optionsForm)
			{
				// Find an "active account".
				IEnumerable<EntryConfiguration> activeAccounts;
				activeAccounts = FindActiveAccounts();
				string strUuid = null;
				EntryConfiguration entryConfig;
				if (activeAccounts.Any())
				{
					entryConfig = activeAccounts.First();
				}
				else
				{
					try
					{
						// Attempt to use long-deprecated UUID config.
						strUuid = m_host.GetConfig(GdsDefs.ConfigUUID);
						if (!string.IsNullOrEmpty(strUuid))
						{
							PwEntry entry = m_host.Database.RootGroup.FindEntry(
													strUuid.ToPwUuid(), true);
							entryConfig = new EntryConfiguration(entry);
						}
						else
						{
							entryConfig = null;
						}
					}
					catch (ArgumentException)
					{
						entryConfig = null;
					}
				}

				// If all of the above did not find an account, offer to create
				// a new entry for the plugin config.
				PluginEntryFactory entryFactory = null;
				if (entryConfig == null)
				{
					if (MessageBox.Show(
							Resources.GetFormat("Msg_NoAcct",
								GdsDefs.AccountSearchString),
							GdsDefs.ProductName,
							MessageBoxButtons.YesNo,
							MessageBoxIcon.Warning) != DialogResult.Yes)
					{
						return null;
					}
					entryFactory = GetEntryFactory();
					entryConfig = new EntryConfiguration(entryFactory.Entry);
				}

				// Look for the active entry in account list, and if not found,
				// add it to the front.
				if (!acctList.Any(es => es.Entry == entryConfig.Entry))
				{
					acctList.Insert(0, entryConfig);
				}

				// Show the configuration dialog.
				if (DialogResult.OK != ShowModalDialogAndDestroy(optionsForm))
				{
					return null;
				}

				// Update global options.
				m_defaultFolder = options.DefaultAppFolder;
				m_defaultFolderColor = options.DefaultAppFolderColor;
				m_defaultDriveScope = options.DefaultApiScope;
				m_defaultClientId = options.DefaultClientId;
				m_defaultClientSecret = options.DefaultClientSecret;
				m_autoSync = options.AutoSync;

				// Update the chosen config entry.
				entryConfig = options.SelectedAccountShadow;
				entryConfig.CommitChangesIfAny();
				if (entryFactory != null &&
					entryConfig.Entry == entryFactory.Entry)
				{
					// An entry was created above, but it isn't in the
					// database yet...put it there now.
					PluginEntryFactory.Import(m_host, entryFactory);
				}
				return entryConfig;
			}
		}

		private PluginEntryFactory GetEntryFactory()
		{
			// Search KeePass for pre-existing entry titles.
			string searchString = GdsDefs.ProductName;
			EntryHandler checkEntry = delegate(PwEntry e)
			{
				return -1 == e.Strings.ReadSafe(PwDefs.TitleField)
							.IndexOf(searchString,
								StringComparison.OrdinalIgnoreCase);
			};
			int cDup = 2;
			PwGroup root = m_host.Database.RootGroup;
			while (!root.TraverseTree(TraversalMethod.PreOrder,
							null, checkEntry))
			{
				searchString = string.Format("{0}-{1}", GdsDefs.ProductName, cDup++);
			}

			// Return entry creator with unused title.
			return PluginEntryFactory.Create(searchString,
												m_defaultDriveScope,
												m_defaultClientId,
												m_defaultClientSecret,
												m_defaultFolder);
		}

		/// <summary>
		/// Load the current configuration
		/// </summary>
		private EntryConfiguration LoadConfiguration()
		{
			EntryConfiguration entryConfig = null;

			if (!m_host.Database.IsOpen)
			{
				return entryConfig;
			}

			// Find the active account.
			IEnumerable<EntryConfiguration> accounts = FindActiveAccounts();
			if (accounts.Any())
			{
				entryConfig = accounts.First();
			}
			else
			{
				try
				{
					// Attempt to use long-obsolete UUID config.
					string strUuid = m_host.GetConfig(GdsDefs.ConfigUUID);
					if (!string.IsNullOrEmpty(strUuid))
					{
						PwEntry entry = m_host.Database.RootGroup.FindEntry(
												strUuid.ToPwUuid(), true);
						entryConfig = new EntryConfiguration(entry);
					}
				}
				catch (ArgumentException) 
				{
					// Bad config, etc.
					entryConfig = null;
				}
			}

			if (entryConfig != null && entryConfig.IsMissingOauthCredentials)
			{
				entryConfig.ClientId = GdsDefs.DefaultClientId.ReadString();
				entryConfig.ClientSecret = GdsDefs.DefaultClientSecret;
			}

			return entryConfig;
		}

		/// <summary>
		/// Load saved configuration or if missing query user for it.
		/// </summary>
		private EntryConfiguration GetConfiguration()
		{
			EntryConfiguration config = LoadConfiguration();
			if (config == null)
			{
				config = AskForConfiguration();
				if (config != null)
				{
					SaveConfiguration(config);

					// Use default OAuth 2.0 credentials if missing.
					if (config.IsMissingOauthCredentials)
					{
						config.ClientId = GdsDefs.DefaultClientId.ReadString();
						config.ClientSecret = GdsDefs.DefaultClientSecret;
					}
				}
			}
			return config;
		}

		/// <summary>
		/// Save the current configuration as "active".
		/// </summary>
		private bool SaveConfiguration(EntryConfiguration entryConfig)
		{
			// Save global config to app config.
			m_host.SetConfig(GdsDefs.ConfigAutoSync, m_autoSync.ToString());
			m_host.SetConfig(GdsDefs.ConfigDefaultAppFolder, m_defaultFolder);
			m_host.SetConfig(GdsDefs.ConfigDefaultAppFolderColor, 
				m_defaultFolderColor == null ? null :
				GoogleColor.SerializeToString(m_defaultFolderColor));
			m_host.SetConfig(GdsDefs.ConfigDriveScope, m_defaultDriveScope);
			m_host.SetConfig(GdsDefs.ConfigDefaultClientId, m_defaultClientId);
			m_host.SetConfig(GdsDefs.ConfigDefaultClientSecret,
				m_defaultClientSecret == null ? string.Empty :
				m_defaultClientSecret.ReadString());

			if (!m_host.Database.IsOpen || entryConfig == null)
			{
				return false;
			}

			// Disable all currently active accounts but the selected (if any)
			IEnumerable<EntryConfiguration> accounts = FindActiveAccounts();
			foreach (EntryConfiguration entry in accounts)
			{
				if (!object.ReferenceEquals(entry.Entry, entryConfig.Entry) && 
					entry.ActiveAccount.GetValueOrDefault(false))
				{
					entry.ActiveAccount = null;
					entry.CommitChangesIfAny();
					m_host.Database.Modified = entry.ChangesCommitted;
				}
			}
			entryConfig.ActiveAccount = true;
			entryConfig.CommitChangesIfAny();

			m_host.Database.Modified = entryConfig.ChangesCommitted ||
											m_host.Database.Modified;

			SaveDatabase();

			if (m_host.GetConfig(GdsDefs.ConfigUUID) != null)
			{
				// Remove deprecated uuiid setting now that the new-style
				// setting ares saved to the database.
				m_host.SetConfig(GdsDefs.ConfigUUID, null);
			}

			return true;
		}

		private void ShowMessage(string msg, bool isStatusMessage = false)
		{
			m_host.ShowMessage(msg, isStatusMessage);
		}
	}

	public class NativeCodeReceiver : ICodeReceiver
	{
		private readonly SyncConfiguration m_entry;
		private readonly IPluginHost m_host;
		private string m_code, m_state, m_redirectUri;
		HttpListener m_listener;

		public string RedirectUri
		{
			get
			{
				if (m_redirectUri == null)
				{
					try
					{
						m_redirectUri = string.Format("{0}:{1}/",
									GoogleAuthConsts.LocalhostRedirectUri,
									GetRandomUnusedPort());

						// Create and start the listener now so less likely
						// to lose the port.
						m_listener = new HttpListener();
						m_listener.Prefixes.Add(m_redirectUri);
						m_listener.Start();
						Debug.WriteLine(string.Format("Listening for '{0}'...",
														RedirectUri));
					}
					catch (Exception e)
					{
						using (m_listener) { }
						m_code = e.ToString();
						m_listener = null;
					}
				}
				return m_redirectUri;
			}
		}

		public NativeCodeReceiver(IPluginHost host, SyncConfiguration entry)
		{
			m_entry = entry;
			m_host = host;
			m_listener = null;
			m_state = null;
			m_code = "access_denied";
		}

		public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(
			AuthorizationCodeRequestUrl url,
			CancellationToken taskCancellationToken)
		{
			// No matter what else, dispose the HTTP listener in this call.
			return await Task.Run(() =>
			{
				using (m_listener)
				{
					if (m_listener == null)
					{
						return new AuthorizationCodeResponseUrl()
						{
							Error = m_code
						};
					}

					string email = m_entry.LoginHint;
					if (url is GoogleAuthorizationCodeRequestUrl &&
						!string.IsNullOrEmpty(email))
					{
						((GoogleAuthorizationCodeRequestUrl)url).LoginHint = email;
					}

					// Invoke the browser-based sign-in flow, and handle response.
					if (SignIn(url))
					{
						return new AuthorizationCodeResponseUrl()
						{
							Code = m_code,
							State = m_state
						};
					}
					else
					{
						return new AuthorizationCodeResponseUrl()
						{
							Error = m_code
						};
					}
				}
			});
		}

		// ref http://stackoverflow.com/a/3978040
		public static int GetRandomUnusedPort()
		{
			TcpListener portGrabber;
			portGrabber = new TcpListener(IPAddress.Loopback, 0);
			portGrabber.Start();
			int port = ((IPEndPoint)portGrabber.LocalEndpoint).Port;
			portGrabber.Stop();
			return port;
		}

		/// <summary>
		/// Returns URI-safe RNG data with a given input length.
		/// </summary>
		/// <param name="length">Number of bytes to encode.
		/// </param>
		/// <returns></returns>
		public static string RandomDataBase64url(uint length)
		{
			byte[] bytes;
			bytes = CryptoRandom.Instance.GetRandomBytes(length);
			return bytes.ToUrlSafeBase64();
		}

		HttpListenerContext DisplayFormAndAwaitAuth()
		{
			HttpListenerContext context = null;
			if (m_listener == null)
			{
				return context;
			}

			// Create and display the form on the KeePass UI thread.
			if (m_host.MainWindow.InvokeRequired)
			{
				m_host.MainWindow.Invoke(new MethodInvoker(() =>
				{
					context = DisplayFormAndAwaitAuth();
				}));
			}
			else
			{
				using (AuthWaitOrCancel form = new AuthWaitOrCancel(m_host,
													m_entry as EntryConfiguration))
				{
					Task t = new Task(async () =>
					{
						try
						{
							// Wait for user to ponder scary warning about
							// using unverified apps, hopefully resulting in
							// OAuth authentication/authorization response via
							// the redirect.
							context = await m_listener.GetContextAsync();
						}
						catch (ObjectDisposedException)
						{
							// This can happen when the user cancels and the
							// listener is disposed.
							context = null;
						}

						// Close the waiter dialog if not already gone.
						if (!form.IsDisposed)
						{
							form.Invoke(new MethodInvoker(form.Close));
						}

						// Bring main KeePass window back.  
						Form window = m_host.MainWindow;
						window.BeginInvoke(new MethodInvoker(window.Activate));
					});
					form.Shown += (o, e) => t.Start();
					GoogleDriveSyncExt.ShowModalDialogAndDestroy(form);
				}
			}
			return context;
		}

		/// <summary>
		/// Authenticate user and authorize this application with browser-based
		/// sign-on.
		/// </summary>
		/// <returns><code>true</code> or <code>false</code> based on authentication
		/// result.</returns>
		private bool SignIn(AuthorizationCodeRequestUrl url)
		{
			// Generate OAuth request state value and set redirect.
			url.RedirectUri = RedirectUri;
			url.State = RandomDataBase64url(32);

			// Open browser via Windows URL doc handler.
			Process.Start(url.Build().ToString());

			// Show a dialog to direct user's attention to the browser, and
			// to offer a way to get out of this if the browser is somehow
			// munged or the user is distracted/discouraged.
			HttpListenerContext context = DisplayFormAndAwaitAuth();
			if (context == null)
			{
				m_code = "user_cancelled";
				return false;
			}

			// Send acknowledgement response to the browser user.
			string responseString;
			responseString = Resources.GetString("Html_AuthResponseString");
			byte[] buffer = Encoding.UTF8.GetBytes(responseString);
			HttpListenerResponse response = context.Response;
			response.KeepAlive = false;
			response.ContentLength64 = buffer.Length;
			using (Stream responseOutput = response.OutputStream)
			{
				// Don't use non-blocking here so that m_listener doesn't
				// dispose before we push out the response.
				responseOutput.Write(buffer, 0, buffer.Length);
				Debug.WriteLine("Browser acknowledged.");
			}

			// Check for error response.
			string error = context.Request.QueryString.Get("error");
			if (error != null)
			{
				Debug.WriteLine(string.Format("OAuth response error: {0}.", error));
				return false;
			}

			// Response must have a code and state.
			m_code = context.Request.QueryString.Get("code");
			m_state = context.Request.QueryString.Get("state");
			if (m_code == null || m_state == null)
			{
				Debug.WriteLine(string.Format("Unexpected response: {0}",
					context.Request.QueryString));
				return false;
			}

			// Compare response state to expected value validating that the request url
			// sent above resulted in this authorization response.
			if (m_state != url.State)
			{
				Debug.WriteLine(string.Format("Response with unexpected state ({0}).",
					m_state));
				return false;
			}

			Debug.WriteLine(string.Format("Authorization code: {0}.", m_code));
			return true;
		}
	}
}
