/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright © 2012-2016  DesignsInnovate
 * Copyright © 2014-2016  Paul Voegler
 * 
 * KeePass Sync for Google Drive
 * Copyright © 2020-2021 Walter Goodwin
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

using KeePass.Plugins;
using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePassSyncForDrive
{
    public partial class SharedFileError : Form
    {
        internal static DialogResult ShowIfNeeded(IPluginHost host,
            string fileName, SyncConfiguration config)
        {
            if (config.DontSaveAuthToken ||
                config.IsUsingPersonalOauthCreds)
            {
                return DialogResult.None;
            }

            SharedFileError dlg = new SharedFileError();
            dlg.TargetFile = fileName;
            KeePassSyncForDriveExt.ShowModalDialogAndDestroy(dlg);
            return DialogResult.OK;
        }

        public SharedFileError()
        {
            InitializeComponent();

            Text = GdsDefs.ProductName;
            Resources.GetControlText(lblMessage);
            Resources.GetControlText(btnOK);
            Resources.GetControlText(lnkHelp);
            Resources.GetControlText(lnkPersonalOauth);
            Resources.GetControlText(lnkSessionAuthTokens);

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("gdsync"),
                Resources.GetString("Title_SharedFileError"),
                string.Format("{0} {1}", GdsDefs.ProductName,
                                GdsDefs.Version));
        }

        string TargetFile { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            lblSubTitle.Text
                = Resources.GetFormat("SubTitle_SharedFileError",
                                        TargetFile);
            base.OnLoad(e);
        }

        private void lnkHelp_LinkClicked(object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GdsDefs.UrlSharedFileHelp);
        }

        private void lnkPersonalOauth_LinkClicked(object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GdsDefs.UrlPersonalAppCreds);
        }

        private void lnkSessionAuthTokens_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GdsDefs.UrlTokenHandling);
        }
    }
}
