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

using KeePass.UI;
using System;
using System.Windows.Forms;

namespace KeePassSyncForDrive
{
    public partial class SavedAuthWarning : Form
    {
        public SavedAuthWarning()
        {
            InitializeComponent();

            Text = GdsDefs.ProductName;
            m_lblMessage.Text = Resources.GetString(m_lblMessage.Text);
            m_cbDontShowAgain.Text = Resources.GetString(m_cbDontShowAgain.Text);
            m_btnCancel.Text = Resources.GetString(m_btnCancel.Text);
            m_btnOK.Text = Resources.GetString(m_btnOK.Text);

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("gdsync"),
                Resources.GetFormat("Title_SavedAuthWarning",
                                    GdsDefs.ProductName),
                string.Format("{0} {1}", GdsDefs.ProductName,
                                GdsDefs.Version));
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (m_cbDontShowAgain.Checked)
            {
                PluginConfig.Default.WarnOnSavedAuthToken = false;
            }
            base.OnFormClosing(e);
        }
    }
}
