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
		private ToolStripMenuItem m_tsmiAddEntries = null;

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

			// Get a reference to the 'Tools' menu item container
			ToolStripItemCollection tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;

			// Add a separator at the bottom
			m_tsSeparator = new ToolStripSeparator();
			tsMenu.Add(m_tsSeparator);

			// Add the popup menu item
			m_tsmiPopup = new ToolStripMenuItem();
			m_tsmiPopup.Text = "Sync";
			tsMenu.Add(m_tsmiPopup);

            m_tsmiAddEntries = new ToolStripMenuItem();
            m_tsmiAddEntries.Text = "Sync with Google";
            m_tsmiAddEntries.Click += OnSyncWithGoogle;
            m_tsmiPopup.DropDownItems.Add(m_tsmiAddEntries);

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
			tsMenu.Remove(m_tsmiAddEntries);

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
                syncWithGoogle();
        }

        private void OnSyncWithGoogle(object sender, EventArgs e)
        {
            syncWithGoogle();
        }

        private void syncWithGoogle()
        {
            if (!m_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database!", "Google Sync Plugin");
                return;
            }
            string status = "Google sync started,  please wait ...";
            try
            {
                m_host.MainWindow.SetStatusEx(status);
                m_host.MainWindow.Enabled = false;

                String CLIENT_ID = m_host.CustomConfig.GetString("GoogleSyncClientID");
                String CLIENT_SECRET = m_host.CustomConfig.GetString("GoogleSyncClientSecret");


                // Register the authenticator and create the service
                var provider = new NativeApplicationClient(GoogleAuthenticationServer.Description, CLIENT_ID, CLIENT_SECRET);
                var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
                var service = new DriveService(auth);

                string filepath = m_host.Database.IOConnectionInfo.Path;
                File file = getFile(service, filepath);
                if (file == null)
                {
                    status = uploadFile(service, "Keepass Password Safe Database", string.Empty, "*/*", "*/*", filepath);
                }
                else
                {
                    status = updateFile(service, file, filepath, "*/*");
                }
            }
            catch (Exception ex)
            {
                status = string.Empty;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                m_host.MainWindow.Enabled = true;
                m_host.MainWindow.SetStatusEx("Google sync complete. " + status);
            }
        }

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

        private string updateFile(DriveService service, File file, String filepath, String contentType)
        {
            // see if file on server needs updating (is it newer than current database?)
            DateTime dtFileModified;
            DateTime dtDatabaseModified;

            DateTime.TryParse(file.ModifiedDate, out dtFileModified);
            dtDatabaseModified = System.IO.File.GetLastWriteTime(m_host.Database.IOConnectionInfo.Path);

            int result = DateTime.Compare(dtFileModified, dtDatabaseModified);

            if (result >= 0) // same or is later than
            {
                return "File on server is newer. Aborting update.";
            }

            string filename = System.IO.Path.GetFileName(filepath);

            byte[] byteArray = System.IO.File.ReadAllBytes(filepath);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            FilesResource.UpdateMediaUpload request = service.Files.Update(file, file.Id, stream, contentType);
            request.Upload();

            return string.Format("File updated. Id:{0}, Name:{1}", file.Id, file.Title);
        }

        private string uploadFile(DriveService service, String description, String title, String mimeType, String contentType, String filepath)
        {
            string filename = System.IO.Path.GetFileName(filepath);
            File temp = new File();
            if (string.IsNullOrEmpty(title))
                temp.Title = filename;
            else
                temp.Title = title;
            temp.Description = description;
            temp.MimeType = mimeType;

            byte[] byteArray = System.IO.File.ReadAllBytes(filename);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

            FilesResource.InsertMediaUpload request = service.Files.Insert(temp, stream, contentType);
            request.Upload();

            File file = request.ResponseBody;
            return string.Format("File uploaded. Id:{0}, Name:{1}", file.Id, file.Title);
        }

        private IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(new[] { DriveService.Scopes.Drive.GetStringValue() });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            string struid = m_host.CustomConfig.GetString("GoogleSyncKeepassUID");
            PwUuid pid = new PwUuid(KeePassLib.Utility.MemUtil.HexStringToByteArray(struid));
            PwEntry pentry = m_host.Database.RootGroup.FindEntry(pid, true);
            if (pentry == null)
                MessageBox.Show("Entry for google account not found. Invalid UID: " + struid);
            GoogleAuthenticateForm form1 = new GoogleAuthenticateForm(pentry.Strings.Get(PwDefs.UserNameField).ReadString(), 
                pentry.Strings.Get(PwDefs.PasswordField).ReadString());
            form1.Browser.Navigate(authUri);
            form1.ShowDialog();
            return arg.ProcessUserAuthorization(form1.AuthCode, state);
        }
    }
}
