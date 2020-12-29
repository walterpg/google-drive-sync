/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * KeePass Sync for Google Drive
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

namespace KeePassSyncForDrive
{
	[Flags]
	public enum AutoSyncMode
	{
		DISABLED = 0,
		SAVE = 1,
		OPEN = 2
	}

	[Flags]
    public enum SyncCommands : int
	{
		DOWNLOAD = 1,
		SYNC = 2,
		UPLOAD = 4,

		All = 7,
		None = 0
	}

	/// <summary>
	/// main plugin class
	/// </summary>
	public sealed class KeePassSyncForDriveExt : Plugin, IDriveServiceProvider
	{
		private IPluginHost m_host = null;

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
		volatile bool m_ignoreDatabaseEvents;

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

			PluginConfig appDefaults = PluginConfig.InitDefault(host);

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
				Image = Images.DriveIcon.ToBitmap()
			};
			tsMenu.Add(m_tsmiPopup);

			m_tsmiSync = new ToolStripMenuItem
			{
				Name = "Sync",
				Tag = SyncCommands.SYNC,
				Text = Resources.GetString("MenuLabel_Sync"),
				Image = Resources.GetBitmap("round_sync_black_18dp"),
				Enabled = appDefaults.IsCmdEnabled(SyncCommands.SYNC)
			};
			m_tsmiSync.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiSync);

			m_tsmiUpload = new ToolStripMenuItem
			{
				Name = "Upload",
				Tag = SyncCommands.UPLOAD,
				Text = Resources.GetString("MenuLabel_Upload"),
				Image = Resources.GetBitmap("round_cloud_upload_black_18dp"),
				Enabled = appDefaults.IsCmdEnabled(SyncCommands.UPLOAD)
			};
			m_tsmiUpload.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiUpload);

			m_tsmiDownload = new ToolStripMenuItem
			{
				Name = "Download",
				Tag = SyncCommands.DOWNLOAD,
				Text = Resources.GetString("MenuLabel_Download"),
				Image = Resources.GetBitmap("round_cloud_download_black_18dp"),
				Enabled = appDefaults.IsCmdEnabled(SyncCommands.DOWNLOAD)
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
			m_ignoreDatabaseEvents = false;

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
				m_host.ShowStatusMessage(
					Resources.GetString("Msg_AutoSyncIgnore"));
			}
			else if (LoadConfiguration() == null)
			{
				m_host.ShowStatusMessage(
					Resources.GetString("Msg_NoAutoSyncConfig"));
			}
			else
			{
				await ConfigAndSyncWithGoogle(SyncCommands.SYNC, true);
			}
		}

		/// <summary>
		/// Event handler to implement auto sync on save
		/// </summary>
		private async void OnFileSaved(object sender, FileSavedEventArgs e)
		{
			if (!m_ignoreDatabaseEvents && e.Success &&
				PluginConfig.Default.IsAutoSync(AutoSyncMode.SAVE))
			{
				await SyncOnOpenOrSaveCmd();
			}
		}

		/// <summary>
		/// Event handler to implement auto sync on open
		/// </summary>
		private async void OnFileOpened(object sender, FileOpenedEventArgs e)
		{
			if (!m_ignoreDatabaseEvents &&
				PluginConfig.Default.IsAutoSync(AutoSyncMode.OPEN))
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
			SyncCommands syncCommand = (SyncCommands)item.Tag;
			await ConfigAndSyncWithGoogle(syncCommand, false);
		}

		/// <summary>
		/// Event handler for configuration menu entry
		/// </summary>
		private void OnConfigure(object sender, EventArgs e)
		{
			if (!m_host.Database.IsOpen)
			{
				m_host.ShowDlgMessage(
					Resources.GetString("Msg_FirstOpenDb"));
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
					m_host.ShowStatusMessage(msg);
					return;
				default:
					msg = ex.Message;
					break;
			}
			m_host.ShowDlgMessage(msg);
		}

		internal static void SaveDatabase(IPluginHost host, object sender,
			string saveMessage = null)
		{
			if (!host.Database.Modified)
			{
				return;
			}

			MainForm window = host.MainWindow;
			if (window.InvokeRequired)
			{
				window.Invoke(new MethodInvoker(
					() => SaveDatabase(host, sender, saveMessage)));
				return;
			}

			string status = saveMessage;
			if (string.IsNullOrEmpty(saveMessage))
			{
				status = Resources.GetString("Msg_SavingDatabase");
			}

			try
			{
				host.MainWindow.SaveDatabase(null, sender);
			}
			finally
			{
				status = Resources.GetString("Msg_DatabaseSaved");
			}
		}

		/// <summary>
		/// Authenticate and authorize the drive service and invoke a user
		/// function. Obtain and update authorization details from/to the
		/// current configuration.
		/// </summary>
		public async Task<string> ConfigAndUseDriveService(
						Func<DriveService, SyncConfiguration, Task<string>> use)
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
				m_host.ShowStatusMessage(status2);


				// Traditionally, the plugin's indicator for "use default 
				// clientId" is empty strings for clientId & secret.  Maintain
				// that compatibility point.
				if (config.UseLegacyCreds &&
					GdsDefs.LegacyClientId.ReadString() == config.ClientId &&
					 GdsDefs.LegacyClientSecret
						.OrdinalEquals(config.ClientSecret, true))
				{
					config.ClientId = string.Empty;
					config.ClientSecret = null;
				}
			}

			config.CommitChangesIfAny();
			SaveConfiguration(config);

			return status;
		}

		public async Task<string> UseDriveService(SyncConfiguration authData,
						Func<DriveService, SyncConfiguration, Task<string>> use)
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
					ApplicationName = GdsDefs.ProductName,
					HttpClientFactory = new ProxyHttpClientFactory()
				});
				status = await use(service, authData);
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
						HandleUseDriveServiceException(inner);
					}
				}
			}
			catch (Exception ex)
			{
				status = HandleUseDriveServiceException(ex);
			}
			return status;
		}

		string HandleUseDriveServiceException(Exception e)
        {
			SharedFileException sfe = e as SharedFileException;
			if (null != sfe)
            {
				m_host.ShowDlgMessage(sfe.Message, new Uri(sfe.Link));
				return sfe.Message;
            }

			m_host.ShowDlgMessage(e.Message);
			return "ERROR";
        }

		/// <summary>
		/// Configure if necessary, then execute synchronization of the current
		/// database with Google Drive. Create a new file if it does not already
		/// exist.
		/// </summary>
		private async Task ConfigAndSyncWithGoogle(SyncCommands syncCommand,
			bool autoSync)
		{
			// Suspend these events temporarily in case the configuration
			// needs to be saved.
			m_ignoreDatabaseEvents = true;
			try
			{
				await ConfigAndSyncUnsafe(syncCommand, autoSync);
			}
			finally
			{
				m_ignoreDatabaseEvents = false;
			}
		}

		private async Task ConfigAndSyncUnsafe(SyncCommands sync, bool autoSync)
		{
			string status = Resources.GetString("Msg_PleaseWaitEllipsis");
			m_host.ShowStatusMessage(status);

			status = await ConfigAndUseDriveService(async (service, config) =>
			{
				string filePath = m_host.Database.IOConnectionInfo.Path;
				string fileName = Path.GetFileName(filePath);
				string contentType = "application/x-keepass2";
				status = null;

				List<Folder> folders = await GetFolders(service, config.ActiveFolder);
				GDriveFile file = await GetFile(service, folders, fileName, 
											config);
				if (file == null)
				{
					if (sync == SyncCommands.DOWNLOAD)
					{
						return Resources.GetString("Msg_GDriveFileNotFound");
					}
					else // upload
					{
						if (!autoSync)
						{
							SaveDatabase(m_host, this);
						}
						GDriveFile folder = null;
						if (folders != null && folders.Any())
                        {
							if (folders.Count > 1)
                            {
								status = Resources.GetFormat("Exc_MultipleFoldersFmt",
									fileName, folders.First().ToString());
								throw new PluginException(status);
                            }
							folder = folders.First().DriveEntry;
                        }
						status = await UploadFile(service, folder,
							Resources.GetString("Descr_KpDbFile"),
							string.Empty, contentType, filePath,
							ContentHints);
					}
				}
				else
				{
					if (sync == SyncCommands.UPLOAD)
					{
						if (!autoSync)
						{
							SaveDatabase(m_host, this);
						}
						status = Resources.GetFormat("Msg_ReplaceFileFmt", file.Name);
						m_host.ShowStatusMessage(status);
						status = await UpdateFile(service, file,
							filePath, contentType, ContentHints);
					}
					else
					{
						string fileCopy = await DownloadCopy(service, file, filePath);
						if (!string.IsNullOrEmpty(fileCopy))
						{
							if (sync == SyncCommands.DOWNLOAD)
							{
								status = await ReplaceDatabase(filePath, fileCopy);
							}
							else
							{
								string syncStatus = SyncFromThenDeleteFile(fileCopy);
								status = Resources.GetString("Msg_UploadingSync");
								m_host.ShowStatusMessage(status);
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
			if (!string.IsNullOrEmpty(status))// &&
				//status != "ERROR")
			{
				m_host.ShowStatusMessage(status);
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
			m_host.ShowStatusMessage(status);

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
					IDownloadProgress progress 
						= await request.DownloadAsync(fileStream);
					if (progress.Status != DownloadStatus.Completed)
                    {
						status = string.Format("Download not completed: {0}",
							progress.Exception == null ? "Unknown error." :
							progress.Exception.Message);
						m_host.ShowStatusMessage(status);
						if (progress.Exception != null)
                        {
							status = string.Format("Exception occurred " +
								"downloading Drive file '{0}' to temporary " +
								"file '{1}'.", file.Name, downloadFilePath);
							throw new PluginException(status,
										progress.Exception);
                        }
                    }
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

		private async Task<List<Folder>> GetFolders(DriveService service, string folderPath)
		{
			if (string.IsNullOrWhiteSpace(folderPath))
			{
				return null;
			}
			folderPath = folderPath.TrimLeadingAndTrailingSeparators();
			if (folderPath.Length == 0)
            {
				return new List<Folder>()
				{
					Folder.RootFolder
				};
			}
			
			FolderName links = FolderName.GetFolderNameLinks(folderPath);
			List<Folder> folders = await links.ResolveLeafFolders(service,
				status => m_host.ShowStatusMessage(status));
			if (folders.Any())
			{
				return folders;
			}

			FolderName endLink = links.FirstFolderNameWithoutFolders();

			Folder createRoot = null;
			if (endLink.ParentFolderName != null)
			{
				createRoot = endLink.ParentFolderName.Folders.First();
				if (endLink.ParentFolderName.Folders.Count > 1)
				{
					string status = Resources.GetFormat(
									"Exc_MultipleFoldersFmt",
									createRoot.ToString());
					throw new PluginException(status);
				}
			}

			// Create the app folder within our scope.
			Folder newLeaf = await endLink.CreateNewFolderPathFrom(
								createRoot, service,
								status => m_host.ShowStatusMessage(status));
			return new List<Folder>(new[] { newLeaf });
		}

		/// <summary>
		/// Get File from Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="folders">Folders which may contain the file (may be
		/// null).</param>
		/// <param name="filename">Full name (not path) of the current database
		/// file</param>
		/// <returns>Return Google File</returns>
		private async Task<GDriveFile> GetFile(DriveService service,
			List<Folder> folders, string fileName, SyncConfiguration config)
		{
			string status = Resources.GetFormat("Msg_RetrievingGDriveFileFmt", fileName);
			m_host.ShowStatusMessage(status);

			FilesResource.ListRequest req = service.Files.List();
			StringBuilder sb = new StringBuilder()
				.Append("name='")
				.Append(fileName.QueryGdriveObjectName())
				.Append("' and ");
			if (folders != null && folders.Any())
            {
				// Folders are only relevant when targeted.
				// https://github.com/walterpg/google-drive-sync/issues/14#issuecomment-696170908
				folders.Aggregate(sb, (cb, f) =>
				{
					if (f != folders.First())
                    {
						cb.Append("or ");
                    }
					else
                    {
						cb.Append('(');
                    }
					return cb.Append('\'')
						.Append(f.DriveEntry.Id)
						.Append("' in parents ");
				})
				.Append(") and ");
			}
			sb.Append("trashed=false");
			req.Q = sb.ToString();
			req.Fields = "files(id,name,mimeType,shortcutDetails/targetId,shared)";

			FileList filesObj = await req.ExecuteAsync();

			IEnumerable<GDriveFile> files = filesObj.Files;
			if (!files.Any())
            {
				return null;
            }
			else if (files.Count() > 1 && 
				!files.All(f => f.Id == files.First().Id))
			{
				// Duplicate ids: files or shortcuts.
				string targetFolder = string.Empty;
				string fmtName;
				if (folders == null)
				{
					fmtName = "Exc_MultipleGDriveFilesFmt_NoFolder";
				}
				else if (folders.Count > 1)
				{
					targetFolder = folders.First().ToString();
					fmtName = "Exc_MultipleFilesAndFolderFmt";
				}
				else
                {
					fmtName = "Exc_MultipleGDriveFilesFmt";
				}
				status = Resources.GetFormat(fmtName, fileName, targetFolder);
				throw new PluginException(status);
			}
			GDriveFile file = await ResolveShortcut(service, files.First());
			Debug.Assert(file == null || file.Shared.HasValue,
				"req.Fields 'shared property' not observed");
			if (file != null && file.Shared.HasValue && file.Shared.Value &&
				(!config.IsUsingPersonalOauthCreds ||
					DialogResult.Cancel == 
						SharedFileWarning.ShowIfNeeded(m_host, fileName,
														config)))
            {
				// The file is a shared file or it's shared-ness cannot
				// be determined.
				throw !config.IsUsingPersonalOauthCreds ?
					new SharedFileException() :
					new PluginException(Resources.GetString("Exc_SharedFile"));
			}
			return file;
		}

		/// <summary>
		/// If the passed object is a Drive shortcut, resolve the file, if
		/// any, that it is linked to.  Otherwise return the passed object.
		/// </summary>
		/// <param name="service">The authorized Drive service.</param>
		/// <param name="driveObject">The potential shortcut. Must include
		/// a valid "name" property.</param>
		/// <returns>A Drive file object, or null.</returns>
		async Task<GDriveFile> ResolveShortcut(DriveService service,
			GDriveFile driveObject)
        {
			string shortcutName = null;
			bool bFileFromShortcut = false;
			string status;
			while (driveObject != null &&
				GdsDefs.MimeTypeShortcut.Equals(driveObject.MimeType,
							StringComparison.Ordinal))
			{
				if (shortcutName == null)
                {
					shortcutName = driveObject.Name;
                }

				// Resolve a shortcut link to a file.
				if (driveObject.ShortcutDetails == null ||
					string.IsNullOrEmpty(driveObject.ShortcutDetails.TargetId))
				{
					status = Resources.GetFormat(
						"Exc_ShortcutTargetIdNotFound", shortcutName);
					throw new PluginException(status);
				}

				status = Resources.GetFormat("Msg_RetrievingGDriveShortcutFmt",
					shortcutName, driveObject.ShortcutDetails.TargetId);
				m_host.ShowStatusMessage(status);

				FilesResource.GetRequest listReq
					= service.Files.Get(driveObject.ShortcutDetails.TargetId);
				listReq.Fields = "id,name,trashed,mimeType," +
					"shortcutDetails/targetId,shared";
				try
				{
					driveObject = await listReq.ExecuteAsync();
					bFileFromShortcut = true;
				}
				catch (Google.GoogleApiException gexc)
				{
					if (gexc.Error.Code != 404)
					{
						throw;
					}
					status = Resources.GetFormat(
						"Exc_ShortcutNoLink", shortcutName);
					throw new PluginException(status);
				}
			}
			if (driveObject != null && bFileFromShortcut &&
				driveObject.Trashed.HasValue && driveObject.Trashed.Value)
			{
				status = Resources.GetFormat("Exc_ShortcutTargetTrashed",
					driveObject.Name, shortcutName);
				throw new PluginException(status);
			}
			return driveObject;
		}

		/// <summary>
		/// Sync given File with currently open Database file
		/// </summary>
		/// <param name="tempFilePath">Full path of database file to sync
		/// with</param>
		/// <returns>Return status of the update</returns>
		private string SyncFromThenDeleteFile(string tempFilePath)
		{
			string status = null;
			Form fParent = m_host.MainWindow;
			if (fParent.InvokeRequired)
            {
				fParent.Invoke(new MethodInvoker(() =>
				{
					status = SyncFromThenDeleteFile(tempFilePath);
				}));
				return status;
            }

			status = Resources.GetString("Msg_Synchronizing");
			m_host.ShowStatusMessage(status);

			IOConnectionInfo connection =
				IOConnectionInfo.FromPath(tempFilePath);

			IUIOperations uiOps = m_host.MainWindow;
			bool? success;
			using (new MruFreezer(m_host))
			{
				success = ImportUtil.Synchronize(m_host.Database, uiOps,
												connection, true, fParent);
			}

			// Delete the file.
			Task.Run(() =>
			{
				File.Delete(tempFilePath);
			});

			if (!success.HasValue)
			{
				throw new PluginException(
					Resources.GetString("Exc_NoImportPermission"));
			}
			if (!success.Value)
			{
				throw new PluginException(
					Resources.GetString("Exc_SyncFailureOther"));
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
		/// <param name="folder">The folder (if any) to contain the new file.</param>
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
				ContentHints = thumbnailImage
			};
			if (folder != null)
            {
				temp.Parents = new List<string>()
				{
					folder.Id
				};
            }

			string message = Resources.GetFormat("Msg_UploadingFileFmt", temp.Name);
			m_host.ShowStatusMessage(message);

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
			m_host.ShowStatusMessage(status);

			KeePassLib.Keys.CompositeKey pwKey = m_host.Database.MasterKey;
			m_host.Database.Close();

			status = Resources.GetString("Msg_ReplacingFile");
			m_host.ShowStatusMessage(status);

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
			m_host.ShowStatusMessage(status);

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
				KpOpenDatabase(currentFilePath);
			}

			return returnStatus;
		}

		void KpOpenDatabase(string file)
        {
			MainForm winform = m_host.MainWindow;
			if (winform.InvokeRequired)
			{
				winform.Invoke(new MethodInvoker(() =>
				{
					KpOpenDatabase(file);
				}));
				return;
			}
			m_host.MainWindow.OpenDatabase(
				IOConnectionInfo.FromPath(file), null, true);
		}

		/// <summary>
		/// Get Access Token from Google OAuth 2.0 API
		/// </summary>
		/// <returns>The Sign-in credentials (access token) and the refresh token</returns>
		private static async Task<Tuple<UserCredential, ProtectedString>> 
			GetAuthorization(IPluginHost host, SyncConfiguration config)
		{
			string clientId;
			ProtectedString secret;
			if (!config.UseLegacyCreds)
			{
				// Built-in credentials.
				clientId = GdsDefs.ClientId.ReadString().Trim();
				secret = GdsDefs.ClientSecret;
			}
			else if (config.IsEmptyOauthCredentials)
			{
				// Legacy built-in credentials.
				clientId = GdsDefs.LegacyClientId.ReadString().Trim();
				secret = GdsDefs.LegacyClientSecret;
			}
			else
			{
				// Personal credentials.
				clientId = config.ClientId;
				secret = config.ClientSecret;
			}

			// Scope choice only available with legacy creds.
			string scope;
			if (config.UseLegacyCreds &&
				!string.IsNullOrEmpty(config.LegacyDriveScope))
			{
				scope = config.LegacyDriveScope;
			}
			else
			{
				scope = DriveService.Scope.Drive;
			}

			// Set up the Installed App OAuth 2.0 Flow for Google APIs with a
			// custom code receiver that uses the system browser to 
			// authenticate the Google user and/or authorize the use of the
			// API by this program.
			GoogleAuthorizationCodeFlow.Initializer init;
			init = new GoogleAuthorizationCodeFlow.Initializer
			{
				ClientSecrets = new ClientSecrets()
				{
					ClientId = clientId,
					ClientSecret = secret.ReadString().Trim()
				},
				Scopes = new[] { scope },
				HttpClientFactory = new ProxyHttpClientFactory()
			};
			GoogleAuthorizationCodeFlow codeFlow = new GoogleAuthorizationCodeFlow(init);
			NativeCodeReceiver codeReceiver = new NativeCodeReceiver(host, config);
			AuthorizationCodeInstalledApp authCode;
			authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);
			UserCredential credential = null;

			string status;
			if (!config.RefreshToken.IsNullOrEmpty())
			{
				// Try using an existing Refresh Token to get a new Access Token
				status = Resources.GetString("Msg_RefreshTokenAuth");
				host.ShowStatusMessage(status);

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
						case "unauthorized_client":
						case "invalid_grant":
							// Refresh Token is invalid. Get user authorization
							// below and dump the token.
							credential = null;
							config.RefreshToken = null;
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
				host.ShowStatusMessage(status);

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
		private IList<EntryConfiguration> FindActiveAccounts()
		{
			if (!m_host.Database.IsOpen)
			{
				return Enumerable.Empty<EntryConfiguration>().ToList();
			}

			List<EntryConfiguration> accounts = new List<EntryConfiguration>();

			m_host.Database.RootGroup.TraverseTree(TraversalMethod.PreOrder,
				null, e => InsertActiveEntryHandler(accounts, e));

			return accounts;
		}

		private bool InsertActiveEntryHandler(List<EntryConfiguration> accts,
			PwEntry e)
        {
			PwUuid recycler = m_host.Database.RecycleBinUuid;
			if (e.ParentGroup.Uuid.Equals(recycler))
			{
				// Ignore anything in the recycle bin.
				return true;
			}
			EntryConfiguration entry = new EntryConfiguration(e);
			if (entry.ActiveAccount.GetValueOrDefault(false))
			{
				// There should only be one active account.  Any extras
				// have been introduced externally (e.g., Duplicate
				// Entry command, merge, etc.). Insert-sort to return
				// active accounts in the order of decreasing
				// modification time span.
				int i = accts.Select(c => c.Entry)
					.TakeWhile(f =>
					{
						return (f.LastModificationTime ==
							e.LastModificationTime &&
							f.CreationTime < e.CreationTime) ||
								f.LastModificationTime
									< e.LastModificationTime;
					})
					.Count();
				accts.Insert(i, entry);
			}
			return true;
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
			SyncConfiguration originalAuthData = authData;
			if (!authData.UseLegacyCreds)
			{
				authData = new TransientConfiguration(authData)
				{
					ClientId = GdsDefs.ClientId.ReadString(),
					ClientSecret = GdsDefs.ClientSecret
				};

			}
			else if (authData.IsEmptyOauthCredentials)
			{
				authData = new TransientConfiguration(authData)
				{
					ClientId = GdsDefs.LegacyClientId.ReadString(),
					ClientSecret = GdsDefs.LegacyClientSecret
				};
			}
			else
			{
				originalAuthData = null;
			}

			int[] palette = new int[0];

			string status;
			status = await UseDriveService(authData, async (service, config) =>
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

			m_host.ShowStatusMessage(status);

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
			options = new ConfigurationFormData(acctList, GetColors);
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

				// If all of the above did not find an "active" account, offer some
				// alternatives to configure one.
				PluginEntryFactory entryFactory = null;
				if (entryConfig == null)
				{
					if (acctList.Any())
					{
						// Eligible entries available, ask to use one.
						if (MessageBox.Show(
								Resources.GetString("Msg_NoActiveAccount"),
								GdsDefs.ProductName,
								MessageBoxButtons.OKCancel,
								MessageBoxIcon.Asterisk) != DialogResult.OK)
						{
							return null;
						}
					}
					else
					{
						// No eligible entries found, ask if one may be created.
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
				}

				// Look for the active entry in account list, and if not found,
				// add it to the front.
				if (entryConfig != null &&
					!acctList.Any(es => es.Entry == entryConfig.Entry))
				{
					acctList.Insert(0, entryConfig);
				}

				// Migrate older settings if needed.
				if (entryConfig != null &&
					!OfferEntryMigrationIfNeeded(entryConfig))
				{
					return null;
				}

				// Show the configuration dialog.
				if (DialogResult.OK != ShowModalDialogAndDestroy(optionsForm))
				{
					return null;
				}

				// Update commands.
				m_tsmiSync.Enabled = options.CmdSyncEnabled;
				m_tsmiUpload.Enabled = options.CmdUploadEnabled;
				m_tsmiDownload.Enabled = options.CmdDownloadEnabled;

				// Update the chosen config entry.
				entryConfig = options.SelectedAccountShadow;
				if (entryConfig.IsStaleRefreshToken)
				{
					// When user changes credentials, the refresh token must be
					// reset.
					entryConfig.RefreshToken = GdsDefs.PsEmptyEx;
				}
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

		// Return false if the offer was postponed via Cancel.
		bool OfferEntryMigrationIfNeeded(EntryConfiguration entryConfig)
		{
			if (entryConfig.Version < Version.Parse(SyncConfiguration.Ver1))
			{
				DialogResult result
					= ShowModalDialogAndDestroy(new AppCredsUpgrade());
				if (DialogResult.Cancel == result)
				{
					return false;
				}

				entryConfig.UseLegacyCreds = DialogResult.No == result;
				if (DialogResult.Yes == result)
				{
					entryConfig.RefreshToken = null;
				}
			}
			return true;
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
				searchString = string.Format("{0}-{1}",
									GdsDefs.ProductName, cDup++);
			}

			// Return entry creator with unused title.
			PluginConfig appConfig = PluginConfig.Default;
			return PluginEntryFactory.Create(searchString,
											appConfig.LegacyDriveScope,
											appConfig.PersonalClientId,
											appConfig.PersonalClientSecret,
											appConfig.Folder,
											appConfig.UseLegacyAppCredentials);
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

			return entryConfig;
		}

		/// <summary>
		/// Load saved configuration or if missing query user for it.
		/// </summary>
		private EntryConfiguration GetConfiguration()
		{
			EntryConfiguration config = LoadConfiguration();
			if (config != null)
			{
				// Migrate older settings if necessary.
				if (!OfferEntryMigrationIfNeeded(config))
				{
					return null;
				}
			}
			else
			{
				config = AskForConfiguration();
				if (config != null)
				{
					SaveConfiguration(config);

					// Seed the effective credentials.
					if (!config.UseLegacyCreds)
					{
						config.ClientId = GdsDefs.ClientId.ReadString();
						config.ClientSecret = GdsDefs.ClientSecret;
					}
					else if (config.IsEmptyOauthCredentials)
					{
						// Use legacy OAuth 2.0 credentials if personal creds
						// are missing.
						config.ClientId = GdsDefs.LegacyClientId.ReadString();
						config.ClientSecret = GdsDefs.LegacyClientSecret;
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
			PluginConfig.Default.UpdateConfig(m_host);

			if (!m_host.Database.IsOpen || entryConfig == null)
			{
				return false;
			}

			IEnumerable<EntryConfiguration> accounts = FindActiveAccounts();
			EntryConfiguration currentConfig = accounts.FirstOrDefault(e =>
				entryConfig.Entry.Uuid.Equals(e.Entry.Uuid));
			if (accounts.Any() && currentConfig != null)
			{
				// entryConfig will replaces currentConfig, so take it
				// out of list of accounts to "disable" below.
				accounts = accounts.Except(new[] { currentConfig });
			}

			// Disable any previously active accounts.
			foreach (EntryConfiguration entry in accounts.Where(e =>
						e != entryConfig))
			{
				entry.ActiveAccount = null;
				entry.SharedWarning = SharedFileWarning.Option.AlwaysShow;
				entry.CommitChangesIfAny();
				m_host.Database.Modified = entry.ChangesCommitted;
			}

			entryConfig.ActiveAccount = true;
			entryConfig.CommitChangesIfAny();

			m_host.Database.Modified = entryConfig.ChangesCommitted ||
											m_host.Database.Modified;

			SaveDatabase(m_host, Resources.GetString("Msg_SavingConfig"));
			entryConfig.Reset();

			if (m_host.GetConfig(GdsDefs.ConfigUUID) != null)
			{
				// Remove deprecated uuid setting now that the new-style
				// setting are saved to the database.
				m_host.SetConfig(GdsDefs.ConfigUUID, null);
			}

			return true;
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
					KeePassSyncForDriveExt.ShowModalDialogAndDestroy(form);
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
			responseString = Resources.GetFormat("Html_AuthResponseString",
											GdsDefs.UrlGoogleDrive);
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
