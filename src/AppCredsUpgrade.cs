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

using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePassSyncForDrive
{
    public partial class AppCredsUpgrade : Form
    {
        public AppCredsUpgrade()
        {
            InitializeComponent();

            Text = GdsDefs.ProductName;
            rbNewBuiltIn.Text = Resources.GetString(rbNewBuiltIn.Text);
            rbNoCredsUpgrade.Text = Resources.GetString(rbNoCredsUpgrade.Text);
            lblMessage.Text = Resources.GetString(lblMessage.Text);
            btnCancel.Text = Resources.GetString(btnCancel.Text);
            btnOK.Text = Resources.GetString(btnOK.Text);
            lnkPrivacy.Text = Resources.GetString(lnkPrivacy.Text);
            lnkMoreInfo.Text = Resources.GetFormat(lnkMoreInfo.Text,
                                    GdsDefs.UrlDomainRoot);

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("gdsync"),
                Resources.GetFormat("Title_AppCredsUpgradeMain",
                                    GdsDefs.ProductName),
                string.Format("{0} {1}", GdsDefs.ProductName,
                                GdsDefs.Version));
        }

        private void lnkMoreInfo_LinkClicked(object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GdsDefs.UrlUpgradeV1);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = rbNewBuiltIn.Checked ?
                DialogResult.Yes : DialogResult.No;
        }

        private void lnkPrivacy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(GdsDefs.UrlPrivacy);
        }
    }
}
