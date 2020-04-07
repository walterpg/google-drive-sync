/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
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

using KeePass.Forms;
using KeePass.Plugins;
using KeePass.UI;
using KeePass.Util;
using System;
using System.Windows.Forms;

namespace GoogleSyncPlugin
{
    public partial class AuthWaitOrCancel : Form
    {
        readonly EntryConfiguration m_entryConfig;
        readonly IPluginHost m_host;

        public AuthWaitOrCancel(IPluginHost host, EntryConfiguration entry)
        {
            m_entryConfig = entry;
            m_host = host;

            InitializeComponent();

            lblMessage.Text = Resources.GetString(lblMessage.Text);
            lnkHelp.Text = Resources.GetString(lnkHelp.Text);
            btnCancel.Text = Resources.GetString(btnCancel.Text);
            lblSubTitle.Text = Resources.GetString(lblSubTitle.Text);
            btnCopyUser.Text = Resources.GetString(btnCopyUser.Text);
            btnCopyPassword.Text = Resources.GetString(btnCopyPassword.Text);
            Text = Defs.ProductName;

            btnCopyUser.Enabled = false;
            btnCopyPassword.Enabled = false;
            if (m_entryConfig != null)
            {
                btnCopyUser.Enabled = !string.IsNullOrEmpty(m_entryConfig.User);
                btnCopyPassword.Enabled = m_entryConfig.Password != null &&
                                    !m_entryConfig.Password.IsEmpty;
            }

            BannerFactory.CreateBannerEx(this, m_bannerImage,
                Resources.GetBitmap("btn_google_signin_dark_pressed_web"),
                Resources.GetFormat("Title_AuthDialogMain",
                                    Defs.ProductName),
                string.Format("{0} {1}", Defs.ProductName, Defs.Version));
        }

        private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Defs.UrlSignInHelp);
        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {
            MainForm main = m_host.MainWindow;
            if (main.InvokeRequired)
            {
                EventHandler handler = new EventHandler(btnCopyPassword_Click);
                main.BeginInvoke(handler, sender, e);
                return;
            }
            ClipboardUtil.CopyAndMinimize(m_entryConfig.Password.ReadString(),
                                        true, null, m_entryConfig.Entry, m_host.Database);
            main.StartClipboardCountdown();
        }

        private void btnCopyUser_Click(object sender, EventArgs e)
        {
            MainForm main = m_host.MainWindow;
            if (main.InvokeRequired)
            {
                EventHandler handler = new EventHandler(btnCopyUser_Click);
                main.BeginInvoke(handler, sender, e);
                return;
            }
            ClipboardUtil.CopyAndMinimize(m_entryConfig.User, true, null,
                                        m_entryConfig.Entry, m_host.Database);
            main.StartClipboardCountdown();
        }
    }
}
