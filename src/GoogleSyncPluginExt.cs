/*
  KeePass Password Safe - The Open-Source Password Manager
  Copyright (C) 2003-2009 Dominik Reichl <dominik.reichl@t-online.de>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Forms;

using KeePass.Plugins;
using KeePass.Forms;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Security;

using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using System.Threading;
using System.Net;
using Google.Apis.Requests;
using KeePass.DataExchange;
using KeePassLib.Serialization;
using Google.Apis.Authentication;

namespace GoogleSyncPlugin
{
	/// <summary>
	/// main plugin class
	/// </summary>
	public sealed class GoogleSyncPluginExt : Plugin
	{
		private IPluginHost m_host = null;

		private ToolStripSeparator m_tsSeparator = null;
		private ToolStripMenuItem m_tsmiPopup = null;
		private ToolStripMenuItem m_tsmiSync = null;
		private ToolStripMenuItem m_tsmiUpload = null;
		private ToolStripMenuItem m_tsmiDownload = null;
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
			Debug.Assert(host != null);
			if(host == null) return false;
			m_host = host;

			bool isConfigFound = !String.IsNullOrEmpty(m_host.CustomConfig.GetString("GoogleSyncClientID"))
							&& !String.IsNullOrEmpty(m_host.CustomConfig.GetString("GoogleSyncClientSecret"))
							&& !String.IsNullOrEmpty(m_host.CustomConfig.GetString("GoogleSyncKeePassUID"));

			if (!isConfigFound)
			{
				MessageBox.Show("Initilization Failed: Required configuration entries were not found. Please exit KeePass and update configuration file.\n" +
				 "Required Configuration entries: GoogleSyncClientID, GoogleSyncClientSecret, GoogleSyncKeePassUID\n" +
				 "Optional Configuration entires: EnableAutoSync, GoogleSyncShowAuthenticationForm", "Google Sync Plugin");
				return false;
			}

			// Get a reference to the 'Tools' menu item container
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;

			// Add a separator at the bottom
			m_tsSeparator = new ToolStripSeparator();
			tsMenu.Add(m_tsSeparator);

			// Add the popup menu item
			m_tsmiPopup = new ToolStripMenuItem();
			m_tsmiPopup.Text = "GoogleSyncPlugin";
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

			// We want a notification when the user tried to save the
			// current database
			m_host.MainWindow.FileSaved += OnFileSaved;

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

			// Important! Remove event handlers!
			m_host.MainWindow.FileSaved -= OnFileSaved;
		}

		private void OnFileSaved(object sender, FileSavedEventArgs e)
		{
			//MessageBox.Show("Notification received: the user has tried to save the current database to:\r\n" +
			//    m_host.Database.IOConnectionInfo.Path + "\r\n\r\nResult:\r\n" +
			//    (e.Success ? "Success" : "Failed"), "Plugin",
			//    MessageBoxButtons.OK, MessageBoxIcon.Information);
			if (m_host.CustomConfig.GetBool("EnableAutoSync", false))
				syncWithGoogle(SyncCommand.SYNC);
		}

		private void OnSyncWithGoogle(object sender, EventArgs e)
		{
			ToolStripItem item = (ToolStripItem)sender;
			SyncCommand syncCommand = (SyncCommand)Enum.Parse(typeof(SyncCommand), item.Name);
			syncWithGoogle(syncCommand);
		}

		/// <summary>
		/// Sync the current database with Google Drive. Create a new file if it does not already exists
		/// </summary>
		private void syncWithGoogle(SyncCommand syncCommand)
		{
			if (!m_host.Database.IsOpen && syncCommand != SyncCommand.DOWNLOAD)
			{
				MessageBox.Show("You first need to open a database!", "Google Sync Plugin");
				return;
			}
			string status = "Google sync started, please wait ...";
			try
			{
				m_host.MainWindow.FileSaved -= OnFileSaved;
				m_host.MainWindow.SetStatusEx(status);
				m_host.MainWindow.Enabled = false;

				String CLIENT_ID = m_host.CustomConfig.GetString("GoogleSyncClientID");
				String CLIENT_SECRET = m_host.CustomConfig.GetString("GoogleSyncClientSecret");

				string filePath = m_host.Database.IOConnectionInfo.Path;
				string contentType = "*/*";

				// Register the authenticator and create the service
				var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
				var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
				var service = new DriveService(auth);

				File file = getFile(service, filePath);
				if (file == null)
				{
					if (syncCommand == SyncCommand.DOWNLOAD)
					{
						status = "File was not found. Please upload or sync with Google Drive first.";
					}
					else // upload or sync
						status = uploadFile(service, "Keepass Password Safe Database", string.Empty, contentType, contentType, filePath);
				}
				else
				{
					if (syncCommand == SyncCommand.UPLOAD)
						status = updateFile(service, file, filePath, contentType);
					else
					{
						string downloadFilePath = downloadFile(service.Authenticator, file, filePath);
						if (!string.IsNullOrEmpty(downloadFilePath))
						{
							if (syncCommand == SyncCommand.DOWNLOAD)
								status = "File downloaded: " + downloadFilePath;
							else // sync
								status = string.Format("{0} {1}", syncFile(downloadFilePath),
									updateFile(service, file, filePath, contentType));
						}
						else
							status = "File could not be downloaded";
					}
				}
			}
			catch (Exception ex)
			{
				status = string.Empty;
				MessageBox.Show(ex.Message);
			}
			finally
			{
				m_host.MainWindow.UpdateUI(false, null, true, m_host.Database.RootGroup, true, null, false);
				m_host.MainWindow.SetStatusEx("Google sync complete. " + status);
				m_host.MainWindow.Enabled = true;
				m_host.MainWindow.FileSaved += OnFileSaved;
			}
		}

		/// <summary>
		/// Download a file and return a string with its content.
		/// </summary>
		/// <param name="authenticator">
		/// Authenticator responsible for creating authorized web requests.
		/// </param>
		/// <param name="file">Drive File instance.</param>
		/// <returns>File's content if successful, null otherwise.</returns>
		public string downloadFile(IAuthenticator authenticator, File file, String filePath)
		{
			if (!String.IsNullOrEmpty(file.DownloadUrl))
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
					new Uri(file.DownloadUrl));
				authenticator.ApplyAuthenticationToRequest(request);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				if (response.StatusCode == HttpStatusCode.OK)
				{
					String downloadFilePath = System.IO.Path.GetDirectoryName(filePath) + "\\"
						+ System.IO.Path.GetFileNameWithoutExtension(filePath) + DateTime.Now.ToString("_yyyy-MM-dd-hh-mm-ss-tt") + System.IO.Path.GetExtension(filePath);
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
					MessageBox.Show(
						"An error occurred downloading file: " + response.StatusDescription);
					return null;
				}
			}
			else
			{
				// The file doesn't have any content stored on Drive.
				return null;
			}
		}


		/// <summary>
		/// Get File from Google Drive
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="filepath">Full path of the current database file</param>
		/// <returns>Return Google File</returns>
		private File getFile(DriveService service, String filepath)
		{
			string filename = System.IO.Path.GetFileName(filepath);
			FileList files = service.Files.List().Fetch();
			foreach (File file in files.Items)
			{
				if (file.Title.Equals(filename, StringComparison.OrdinalIgnoreCase))
				{
					return file;
				}
			}
			return null;
		}

		/// <summary>
		/// Sync Google Drive File with currently open Database file
		/// </summary>
		/// <param name="downloadFilePath">Full path of database file to sync with</param>
		/// <returns>Return status of the update</returns>
		private string syncFile(String downloadFilePath)
		{
			IOConnectionInfo connection = IOConnectionInfo.FromPath(downloadFilePath);
			ImportUtil.Synchronize(m_host.Database, m_host.MainWindow, connection, true, m_host.MainWindow);

			System.IO.File.Delete(downloadFilePath);

			return "Local file synchronzied.";
		}

		/// <summary>
		/// Replace contents of the Google Drive File with currently open Database file
		/// </summary>
		/// <param name="service">DriveService</param>
		/// <param name="file">File from Google Drive</param>
		/// <param name="filePath">Full path of the current database file</param>
		/// <param name="contentType">Content type of the Database file</param>
		/// <returns>Return status of the update</returns>
		private string updateFile(DriveService service, File file, String filePath, String contentType)
		{
			byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
			System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

			FilesResource.UpdateMediaUpload request = service.Files.Update(file, file.Id, stream, contentType);
			request.Upload();

			System.IO.File.SetLastWriteTime(filePath, DateTime.Now);

			return string.Format("File on Google Drive updated. Id:{0}, Name:{1}", file.Id, file.Title);
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
		private string uploadFile(DriveService service, String description, String title, String mimeType, String contentType, String filepath)
		{
			File temp = new File();
			if (string.IsNullOrEmpty(title))
				temp.Title = System.IO.Path.GetFileName(filepath);
			else
				temp.Title = title;
			temp.Description = description;
			temp.MimeType = mimeType;

			byte[] byteArray = System.IO.File.ReadAllBytes(filepath);
			System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

			FilesResource.InsertMediaUpload request = service.Files.Insert(temp, stream, contentType);
			request.Upload();

			File file = request.ResponseBody;
			return string.Format("File uploaded. Id:{0}, Name:{1}", file.Id, file.Title);
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

			string refreshToken = LoadRefreshToken();
			if (!String.IsNullOrEmpty(refreshToken))
			{
				state.RefreshToken = refreshToken;
				if (arg.RefreshToken(state))
				{
					return state;
				}
			}

			Uri authUri = arg.RequestUserAuthorization(state);

			string struid = m_host.CustomConfig.GetString("GoogleSyncKeePassUID");
			bool showAuthenticationForm = m_host.CustomConfig.GetBool("GoogleSyncShowAuthenticationForm", false);
			PwUuid pid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(struid));
			PwEntry pentry = m_host.Database.RootGroup.FindEntry(pid, true);
			if (pentry == null)
				MessageBox.Show("Entry for google account not found. Invalid UID: " + struid);
			GoogleAuthenticateForm form1 = new GoogleAuthenticateForm(pentry.Strings.Get(PwDefs.UserNameField).ReadString(),
				pentry.Strings.Get(PwDefs.PasswordField).ReadString(), showAuthenticationForm);
			form1.Browser.Navigate(authUri);
			if (!showAuthenticationForm)
			{
				while (!form1.Visible)
				{
					Application.DoEvents();
				}
			}
			form1.Visible = false;
			form1.ShowDialog();

			var result = arg.ProcessUserAuthorization(form1.AuthCode, state);

			StoreRefreshToken(state);

			return result;
		}

		private static byte[] aditionalEntropy = { 1, 2, 3, 4, 5 };
		private static string LoadRefreshToken()
		{
			if (string.IsNullOrEmpty(Properties.Settings.Default.RefreshToken))
				return null;
			return Encoding.Unicode.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Properties.Settings.Default.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
		}
		private static void StoreRefreshToken(IAuthorizationState state)
		{
			Properties.Settings.Default.RefreshToken = Convert.ToBase64String(ProtectedData.Protect(Encoding.Unicode.GetBytes(state.RefreshToken), aditionalEntropy, DataProtectionScope.CurrentUser));
			Properties.Settings.Default.Save();
		}
	}
}
