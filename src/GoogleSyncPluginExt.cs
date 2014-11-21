/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright (C) 2012-2014  DesignsInnovate
 * Copyright (C) 2014  Paul Voegler
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

using System;
using System.Windows.Forms;
using System.Net;

using KeePass;
using KeePass.UI;
using KeePass.Plugins;
using KeePass.Forms;
using KeePass.DataExchange;

using KeePassLib;
using KeePassLib.Interfaces;
using KeePassLib.Serialization;
using KeePassLib.Security;
using KeePassLib.Collections;

using DotNetOpenAuth.OAuth2;

using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using Google.Apis.Authentication;


namespace GoogleSyncPlugin
{
	public static class Defs
	{
		public const string ProductName = "Google Sync Plugin";
		public const string VersionString = "2.1";
		public const string ConfigAutoSync = "GoogleSync.AutoSync";
		public const string ConfigUUID = "GoogleSync.AccountUUID";
		public const string ConfigClientId = "GoogleSync.ClientID";
		public const string ConfigClientSecret = "GoogleSync.ClientSecret";
		public const string ConfigRefreshToken = "GoogleSync.RefreshToken";
		public const string URLHome = "http://sourceforge.net/p/kp-googlesync";
		public const string URLHelp = "http://sourceforge.net/p/kp-googlesync/support";
		public const string URLGoogleDev = "https://console.developers.google.com/project";
	}

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
	public sealed class GoogleSyncPluginExt : Plugin
	{
		private IPluginHost m_host = null;

		private AutoSyncMode m_autoSync = AutoSyncMode.DISABLED;

		private PwEntry m_entry = null;
		private string m_clientId = string.Empty;
		private ProtectedString m_clientSecret = null;
		private ProtectedString m_refreshToken = null;

		private ToolStripSeparator m_tsSeparator = null;
		private ToolStripMenuItem m_tsmiPopup = null;
		private ToolStripMenuItem m_tsmiSync = null;
		private ToolStripMenuItem m_tsmiUpload = null;
		private ToolStripMenuItem m_tsmiDownload = null;
		private ToolStripMenuItem m_tsmiConfigure = null;
		private enum SyncCommand
		{
			DOWNLOAD = 1,
			SYNC = 2,
			UPLOAD = 3
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
			if(host == null) return false;
			m_host = host;

			try
			{
				m_autoSync = (AutoSyncMode)Enum.Parse(typeof(AutoSyncMode), m_host.CustomConfig.GetString(Defs.ConfigAutoSync, AutoSyncMode.DISABLED.ToString()), true);
			}
			catch (Exception)
			{
				// support old boolean value (Sync on Save)
				if (m_host.CustomConfig.GetBool(Defs.ConfigAutoSync, false))
					m_autoSync = AutoSyncMode.SAVE;
			}

			// Get a reference to the 'Tools' menu item container
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;

			// Add a separator at the bottom
			m_tsSeparator = new ToolStripSeparator();
			tsMenu.Add(m_tsSeparator);

			// Add the popup menu item
			m_tsmiPopup = new ToolStripMenuItem();
			m_tsmiPopup.Text = Defs.ProductName;
			tsMenu.Add(m_tsmiPopup);

			m_tsmiSync = new ToolStripMenuItem();
			m_tsmiSync.Name = SyncCommand.SYNC.ToString();
			m_tsmiSync.Text = "Sync with Google Drive";
			m_tsmiSync.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiSync);

			m_tsmiUpload = new ToolStripMenuItem();
			m_tsmiUpload.Name = SyncCommand.UPLOAD.ToString();
			m_tsmiUpload.Text = "Upload to Google Drive";
			m_tsmiUpload.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiUpload);

			m_tsmiDownload = new ToolStripMenuItem();
			m_tsmiDownload.Name = SyncCommand.DOWNLOAD.ToString();
			m_tsmiDownload.Text = "Download from Google Drive";
			m_tsmiDownload.Click += OnSyncWithGoogle;
			m_tsmiPopup.DropDownItems.Add(m_tsmiDownload);

			m_tsmiConfigure = new ToolStripMenuItem();
			m_tsmiConfigure.Name = "CONFIG";
			m_tsmiConfigure.Text = "Configuration...";
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
			tsMenu.Remove(m_tsSeparator);
			tsMenu.Remove(m_tsmiSync);
			tsMenu.Remove(m_tsmiUpload);
			tsMenu.Remove(m_tsmiDownload);
			tsMenu.Remove(m_tsmiConfigure);

			// Important! Remove event handlers!
			m_host.MainWindow.FileSaved -= OnFileSaved;
			m_host.MainWindow.FileOpened -= OnFileOpened;
		}

		/// <summary>
		/// Event handler to implement auto sync on save
		/// </summary>
		private void OnFileSaved(object sender, FileSavedEventArgs e)
		{
			if (e.Success && AutoSyncMode.SAVE == (m_autoSync & AutoSyncMode.SAVE))
			{
				if (LoadConfiguration())
					syncWithGoogle(SyncCommand.SYNC);
				else
					m_host.MainWindow.SetStatusEx(Defs.ProductName + ": No configuration found. Auto Sync ignored.");
			}
		}

		/// <summary>
		/// Event handler to implement auto sync on open
		/// </summary>
		private void OnFileOpened(object sender, FileOpenedEventArgs e)
		{
			if (AutoSyncMode.OPEN == (m_autoSync & AutoSyncMode.OPEN))
			{
				if (LoadConfiguration())
					syncWithGoogle(SyncCommand.SYNC);
				else
					m_host.MainWindow.SetStatusEx(Defs.ProductName + ": No configuration found. Auto Sync ignored.");
			}
		}

		/// <summary>
		/// Event handler for sync menu entries
		/// </summary>
		private void OnSyncWithGoogle(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			SyncCommand syncCommand = (SyncCommand)Enum.Parse(typeof(SyncCommand), item.Name);
			syncWithGoogle(syncCommand);
		}

		/// <summary>
		/// Event handler for configuration menu entry
		/// </summary>
		private void OnConfigure(object sender, EventArgs e)
		{
			if (!m_host.Database.IsOpen)
			{
				MessageBox.Show("You first need to open a database.", Defs.ProductName);
				return;
			}

			if (AskForConfiguration())
				SaveConfiguration();
		}

		/// <summary>
		/// Sync the current database with Google Drive. Create a new file if it does not already exists
		/// </summary>
		private void syncWithGoogle(SyncCommand syncCommand)
		{
			if (!m_host.Database.IsOpen)
			{
				MessageBox.Show("You first need to open a database.", Defs.ProductName);
				return;
			}
			else if (!m_host.Database.IOConnectionInfo.IsLocalFile())
			{
				MessageBox.Show("Only databases stored locally or on a network share are supported.\n" +
					"Save your database locally or on a network share and try again.", Defs.ProductName);
				return;
			}

			string status = Defs.ProductName + " started. Please wait ...";
			try
			{
				m_host.MainWindow.FileSaved -= OnFileSaved; // disable to not trigger when saving ourselves
				m_host.MainWindow.FileOpened -= OnFileOpened; // disable to not trigger when opening ourselves
				m_host.MainWindow.SetStatusEx(status);
				m_host.MainWindow.Enabled = false;

				// abort when user cancelled or didn't provide config
				if (!GetConfiguration())
					throw new PlgxException(Defs.ProductName + " aborted!");

				string filePath = m_host.Database.IOConnectionInfo.Path;
				string contentType = "application/x-keepass2";

				// Register the authenticator and create the service
				var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, m_clientId, m_clientSecret.ReadString());
				var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
				var service = new DriveService(auth);

				File file = getFile(service, filePath);
				if (file == null)
				{
					if (syncCommand == SyncCommand.DOWNLOAD)
					{
						status = "File name not found on Google Drive. Please upload or sync with Google Drive first.";
					}
					else // upload
						status = uploadFile(service, "KeePass Password Safe Database", string.Empty, contentType, contentType, filePath);
				}
				else
				{
					if (syncCommand == SyncCommand.UPLOAD)
						status = updateFile(service, file, filePath, contentType);
					else
					{
						string downloadFilePath = downloadFile(service.Authenticator, file, filePath);
						if (!String.IsNullOrEmpty(downloadFilePath))
						{
							if (syncCommand == SyncCommand.DOWNLOAD)
								status = replaceDatabase(filePath, downloadFilePath);
							else // sync
								status = String.Format("{0} {1}",
									syncFile(downloadFilePath),
									updateFile(service, file, filePath, contentType));
						}
						else
							status = "File could not be downloaded.";
					}
				}
			}
			catch (Exception ex)
			{
				status = "ERROR";
				MessageBox.Show(ex.Message, Defs.ProductName);
			}
			finally
			{
				m_host.MainWindow.UpdateUI(false, null, true, m_host.Database.RootGroup, true, null, false);
				m_host.MainWindow.SetStatusEx(Defs.ProductName + ": " + status);
				m_host.MainWindow.Enabled = true;
				m_host.MainWindow.FileSaved += OnFileSaved;
				m_host.MainWindow.FileOpened += OnFileOpened;
			}
		}

		/// <summary>
		/// Download a file and return a string with its content.
		/// </summary>
		/// <param name="authenticator">Authenticator responsible for creating authorized web requests.</param>
		/// <param name="file">The Google Drive File instance</param>
		/// <param name="filePath">The local file name and path to download to (timestamp will be appended)</param>
		/// <returns>File's path if successful, null or empty otherwise.</returns>
		private string downloadFile(IAuthenticator authenticator, File file, string filePath)
		{
			if (file == null || String.IsNullOrEmpty(file.DownloadUrl) || String.IsNullOrEmpty(filePath))
				return null;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
				new Uri(file.DownloadUrl));
			authenticator.ApplyAuthenticationToRequest(request);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				string downloadFilePath = System.IO.Path.GetDirectoryName(filePath) + "\\"
					+ System.IO.Path.GetFileNameWithoutExtension(filePath)
					+ DateTime.Now.ToString("_yyyyMMddHHmmss")
					+ System.IO.Path.GetExtension(filePath);

				using (System.IO.Stream stream = response.GetResponseStream())
				{
					byte[] buffer = new byte[1024];
					int bytesRead;
					using (System.IO.FileStream fileStream = System.IO.File.Create(downloadFilePath))
					{
						do
						{
							bytesRead = stream.Read(buffer, 0, buffer.Length);
							fileStream.Write(buffer, 0, bytesRead);
						} while (bytesRead > 0);
					}
				}
				return downloadFilePath;
			}
			else
			{
				MessageBox.Show("File download failed: " + response.StatusDescription, Defs.ProductName);
				return null;
			}
		}

		/// <summary>
		/// Get File from Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="filepath">Full path of the current database file</param>
		/// <returns>Return Google File</returns>
		private File getFile(DriveService service, string filepath)
		{
			string filename = System.IO.Path.GetFileName(filepath);
			FilesResource.ListRequest req = service.Files.List();
			req.Q = "title='" + filename.Replace("'", "\\'") + "' and trashed=false";
			FileList files = req.Fetch();
			if (files.Items.Count < 1)
				return null;
			else if (files.Items.Count == 1)
				return files.Items[0];

			throw new PlgxException("More than one file name '" + filename + "' found on Google Drive. Please make sure the file name is unique across all folders.");
		}

		/// <summary>
		/// Sync Google Drive File with currently open Database file
		/// </summary>
		/// <param name="downloadFilePath">Full path of database file to sync with</param>
		/// <returns>Return status of the update</returns>
		private string syncFile(string downloadFilePath)
		{
			IOConnectionInfo connection = IOConnectionInfo.FromPath(downloadFilePath);
			ImportUtil.Synchronize(m_host.Database, m_host.MainWindow, connection, true, m_host.MainWindow);

			System.IO.File.Delete(downloadFilePath);

			return "Local file synchronized.";
		}

		/// <summary>
		/// Replace contents of the Google Drive File with currently open Database file
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="file">File from Google Drive</param>
		/// <param name="filePath">Full path of the current database file</param>
		/// <param name="contentType">Content type of the Database file</param>
		/// <returns>Return status of the update</returns>
		private string updateFile(DriveService service, File file, string filePath, string contentType)
		{
			byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
			System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

			FilesResource.UpdateMediaUpload request = service.Files.Update(file, file.Id, stream, contentType);
			request.Upload();

			System.IO.File.SetLastWriteTime(filePath, DateTime.Now);

			return string.Format("File on Google Drive updated. Name: {0}, ID: {1}", file.Title, file.Id);
		}

		/// <summary>
		/// Upload a new file to Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="description">File description</param>
		/// <param name="title">File title</param>
		/// <param name="mimeType">File MIME ype</param>
		/// <param name="contentType">File conrent type</param>
		/// <param name="filepath">Full path of the current database file</param>
		/// <returns>Return status of the upload</returns>
		private string uploadFile(DriveService service, string description, string title, string mimeType, string contentType, string filePath)
		{
			File temp = new File();
			if (string.IsNullOrEmpty(title))
				temp.Title = System.IO.Path.GetFileName(filePath);
			else
				temp.Title = title;
			temp.Description = description;
			temp.MimeType = mimeType;

			byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
			System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

			FilesResource.InsertMediaUpload request = service.Files.Insert(temp, stream, contentType);
			request.Upload();

			File file = request.ResponseBody;
			return string.Format("File uploaded to Google Drive. Name: {0}, ID: {1}", file.Title, file.Id);
		}

		/// <summary>
		/// Replace the current database file with a new file and open it
		/// </summary>
		/// <param name="downloadFilePath">Full path of the new database file</param>
		/// <returns>Status of the replacement</returns>
		private string replaceDatabase(string currentFilePath, string downloadFilePath)
		{
			string status = string.Empty;
			string tempFilePath = currentFilePath + ".gsyncbak";

			var pwKey = m_host.Database.MasterKey;
			m_host.Database.Close();

			try
			{
				System.IO.File.Move(currentFilePath, tempFilePath);
				System.IO.File.Move(downloadFilePath, currentFilePath);
				System.IO.File.Delete(tempFilePath);
				status = "File downloaded replacing current database '" + System.IO.Path.GetFileName(currentFilePath) + "'";
			}
			catch (Exception)
			{
				status = "Replacing current database failed. File downloaded as '" + System.IO.Path.GetFileName(downloadFilePath) + "'";
			}
			finally
			{
				try
				{
					// try to open with current MasterKey ...
					m_host.Database.Open(IOConnectionInfo.FromPath(currentFilePath), pwKey, new NullStatusLogger());
				}
				catch (KeePassLib.Keys.InvalidCompositeKeyException)
				{
					// ... MasterKey is different, let user enter the MasterKey
					m_host.MainWindow.OpenDatabase(IOConnectionInfo.FromPath(currentFilePath), null, true);
				}
			}

			return status;
		}

		/// <summary>
		/// Get Authorization token from Google
		/// </summary>
		/// <param name="arg"></param>
		/// <returns></returns>
		private IAuthorizationState GetAuthorization(NativeApplicationClient arg)
		{
			// Get the auth URL:
			IAuthorizationState state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
			state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);

			if (m_refreshToken != null && !m_refreshToken.IsEmpty)
			{
				state.RefreshToken = m_refreshToken.ReadString();
				try
				{
					if (arg.RefreshToken(state))
						return state;
				}
				catch (DotNetOpenAuth.Messaging.ProtocolException ex)
				{
					// fetch http status code
					HttpStatusCode status = 0;
					if (ex.InnerException is WebException && ((WebException)ex.InnerException).Response is HttpWebResponse)
						status =((HttpWebResponse)((WebException)ex.InnerException).Response).StatusCode;

					// refresh token invalid (because user revoked access)?
					if (status == HttpStatusCode.BadRequest
						|| status == HttpStatusCode.Unauthorized
						|| status == HttpStatusCode.Forbidden)
					{
						// invalidate token and let the user authorize again below
						m_refreshToken = null;
						state.RefreshToken = String.Empty;
					}
					else
					{
						throw ex; // sth. else went wrong
					}
				}
			}

			Uri authUri = arg.RequestUserAuthorization(state);
			GoogleAuthenticateForm form1;
			if (m_entry != null)
				form1 = new GoogleAuthenticateForm(authUri, m_entry.Strings.Get(PwDefs.UserNameField).ReadString(), m_entry.Strings.Get(PwDefs.PasswordField));
			else
				form1 = new GoogleAuthenticateForm(authUri, string.Empty, null);

			UIUtil.ShowDialogAndDestroy(form1);
			if (!form1.Success)
				throw new PlgxException("Authorization failed: " + form1.Code);

			state = arg.ProcessUserAuthorization(form1.Code, state);

			// save the refresh token if new or different
			if (!String.IsNullOrEmpty(state.RefreshToken) && (m_refreshToken == null || state.RefreshToken != m_refreshToken.ReadString()))
			{
				m_refreshToken = new ProtectedString(true, state.RefreshToken);
				SaveConfiguration();
			}

			return state;
		}

		/// <summary>
		/// Show the configuration form
		/// </summary>
		private bool AskForConfiguration()
		{
			// find google accounts
			SearchParameters sp = new SearchParameters();
			sp.SearchInUrls = true;
			sp.SearchString = "accounts.google.com";
			PwObjectList<PwEntry> accounts = new PwObjectList<PwEntry>();
			m_host.Database.RootGroup.SearchEntries(sp, accounts);

			// find the configured entry
			PwEntry entry = null;
			string strUuid = m_host.CustomConfig.GetString(Defs.ConfigUUID);
			try
			{
				entry = m_host.Database.RootGroup.FindEntry(new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(strUuid)), true);
			}
			catch (ArgumentException) { }

			// find configured entry in account list
			int idx = -1;
			if (entry != null)
			{
				idx = accounts.IndexOf(entry);
				// add configured entry to account list if not already present
				if (idx < 0)
				{
					accounts.Insert(0, entry);
					idx = 0;
				}
			}

			ConfigurationForm form1 = new ConfigurationForm(accounts, idx, m_autoSync);
			if (DialogResult.OK != UIUtil.ShowDialogAndDestroy(form1))
				return false;

			m_entry = null;
			strUuid = form1.Uuid;
			try
			{
				m_entry = m_host.Database.RootGroup.FindEntry(new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(strUuid)), true);
			}
			catch (ArgumentException) { }

			if (m_entry == null && !String.IsNullOrEmpty(strUuid))
			{
				MessageBox.Show("Password entry with UUID '" + strUuid + "' not found.", Defs.ProductName);
				return false;
			}

			m_clientId = form1.ClientId;
			m_clientSecret = new ProtectedString(true, form1.ClientSecrect);
			m_autoSync = form1.AutoSync;

			return true;
		}

		/// <summary>
		/// Load the current configuration
		/// </summary>
		private bool LoadConfiguration()
		{
			m_entry = null;
			m_clientId = string.Empty;
			m_clientSecret = null;
			m_refreshToken = null;

			if (!m_host.Database.IsOpen)
				return false;

			// find configured password entry in db
			string strUuid = m_host.CustomConfig.GetString(Defs.ConfigUUID);
			try
			{
				m_entry = m_host.Database.RootGroup.FindEntry(new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(strUuid)), true);
			}
			catch (ArgumentException) {}

			if (m_entry == null)
				return false;

			// read OAuth 2.0 credentials
			ProtectedString pstr = m_entry.Strings.Get(Defs.ConfigClientId);
			if (pstr != null)
				m_clientId = pstr.ReadString();
			m_clientSecret = m_entry.Strings.Get(Defs.ConfigClientSecret);
			m_refreshToken = m_entry.Strings.Get(Defs.ConfigRefreshToken);

			// something missing?
			if (m_entry == null || String.IsNullOrEmpty(m_clientId) || m_clientSecret == null || m_clientSecret.IsEmpty)
				return false;

			return true;
		}

		/// <summary>
		/// Load the current configuration or ask for configuration if missing
		/// </summary>
		private bool GetConfiguration()
		{
			// configuration not complete?
			if (!LoadConfiguration())
			{
				if (AskForConfiguration())
					SaveConfiguration();
				else
					return false; // user cancelled or error
			}

			// only return true if in fact nothing is missing. AskForConfiguration can also be true when deleting config (empty UUID).
			return  m_entry != null && !String.IsNullOrEmpty(m_clientId) && m_clientSecret != null && !m_clientSecret.IsEmpty;
		}

		/// <summary>
		/// Save the current configuration
		/// </summary>
		private bool SaveConfiguration()
		{
			if (m_entry == null)
			{
				m_autoSync = AutoSyncMode.DISABLED;
				m_host.CustomConfig.SetString(Defs.ConfigAutoSync, m_autoSync.ToString());
				m_host.CustomConfig.SetString(Defs.ConfigUUID, string.Empty);
				return true;
			}

			if (!m_host.Database.IsOpen)
				return false;

			m_host.CustomConfig.SetString(Defs.ConfigAutoSync, m_autoSync.ToString());
			m_host.CustomConfig.SetString(Defs.ConfigUUID, m_entry.Uuid.ToHexString());

			if (!String.IsNullOrEmpty(m_clientId))
				m_entry.Strings.Set(Defs.ConfigClientId, new ProtectedString(false, m_clientId));
			if (m_clientSecret != null)
				m_entry.Strings.Set(Defs.ConfigClientSecret, m_clientSecret);
			if (m_refreshToken != null)
				m_entry.Strings.Set(Defs.ConfigRefreshToken, m_refreshToken);

			m_host.Database.Save(new NullStatusLogger());

			return true;
		}
	}
}
