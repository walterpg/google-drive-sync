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

using KeePass.Plugins;
using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePassSyncForDrive
{
    public partial class SharedFileWarning : Form
    {
        public enum Option
        {
            SyncDontShow,
            DontSyncDontShow,
            AlwaysShow
        }

        internal static Option OptionFromString(string strVal,
            Option defaultVal = Option.AlwaysShow)
        {
            Option result;
            return Enum.TryParse(strVal, true, out result) ?
                result : defaultVal;
        }

        internal static string StringFromOption(Option opt)
        {
            return Enum.GetName(typeof(Option), opt);
        }

        internal static DialogResult ShowIfNeeded(IPluginHost host,
            string fileName, SyncConfiguration config)
        {
            switch (config.SharedWarning)
            {
                case Option.DontSyncDontShow:
                    return DialogResult.Cancel;
                case Option.SyncDontShow:
                    return DialogResult.OK;
            }

            SharedFileWarning dlg = new SharedFileWarning();
            dlg.TargetFile = fileName;
            DialogResult dr = 
                KeePassSyncForDriveExt.ShowModalDialogAndDestroy(dlg);
            if (dlg.cbDontShowAgain.Checked)
            {
                config.SharedWarning = dr == DialogResult.OK ?
                    Option.SyncDontShow : Option.DontSyncDontShow;
            }

            // Save the option selected to the database before the sync
            // op proceeds.
            EntryConfiguration entryConfig = config as EntryConfiguration;
            if (entryConfig != null)
            {
                entryConfig.CommitChangesIfAny();
                if (entryConfig.ChangesCommitted)
                {
                    host.Database.Modified = true;
                    KeePassSyncForDriveExt.SaveDatabase(host,
                        Resources.GetString("Msg_SavingSharedFileChoice"));
                    entryConfig.Reset();
                }
            }

            return dr;
        }

        public SharedFileWarning()
        {
            InitializeComponent();

            Text = GdsDefs.ProductName;
            Resources.GetControlText(lblMessage);
            Resources.GetControlText(btnCancel);
            Resources.GetControlText(btnOK);
            Resources.GetControlText(lnkHelp);
            Resources.GetControlText(lnkPersonalOauth);
            Resources.GetControlText(cbDontShowAgain);

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("gdsync"),
                Resources.GetString("Title_SharedFileWarning"),
                string.Format("{0} {1}", GdsDefs.ProductName,
                                GdsDefs.Version));
        }

        string TargetFile { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            lblSubTitle.Text
                = Resources.GetFormat("SubTitle_SharedFileWarning",
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
    }
}
