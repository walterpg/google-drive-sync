/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright (C) 2014  DesignsInnovate
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;


namespace GoogleSyncPlugin
{
	public partial class ConfigurationForm : Form
	{
		private PwObjectList<PwEntry> m_accounts = null;
		private int m_accidx = -1;
		private AutoSyncMode m_autoSync = AutoSyncMode.DISABLED;
		private string m_uuid = string.Empty;
		private string m_clientId = string.Empty;
		private string m_clientSecret = string.Empty;

		public ConfigurationForm(PwObjectList<PwEntry> accounts, int idx, AutoSyncMode autoSync)
		{
			InitializeComponent();

			m_accounts = accounts;
			m_accidx = idx >= 0 ? idx : -1;
			m_autoSync = autoSync;

			Visible = false;
		}

		public string Uuid
		{
			get { return m_uuid; }
		}

		public string ClientId
		{
			get { return m_clientId; }
		}

		public string ClientSecrect
		{
			get { return m_clientSecret; }
		}

		public AutoSyncMode AutoSync
		{
			get { return m_autoSync; }
		}

		private void GoogleOAuthCredentialsForm_Load(object sender, EventArgs e)
		{
			lblTitle.Text = Defs.ProductName() + " Configuration";
			lblVersion.Text = Defs.VersionString();

			cbAccount.Items.Add("Custom KeePass UUID");
			foreach (PwEntry entry in m_accounts)
			{
				cbAccount.Items.Add(entry.Strings.GetSafe(PwDefs.UserNameField).ReadString() + " - " + entry.Strings.GetSafe(PwDefs.TitleField).ReadString());
			}

			// preselect first account found when not configured
			//if (m_accidx < 0 && m_accounts.UCount > 0)
			//	m_accidx = 0;

			cbAccount.SelectedIndex = m_accidx + 1;
			if (m_accidx >= 0)
			{
				PwEntry entry = m_accounts.GetAt((uint)cbAccount.SelectedIndex - 1);
				txtUuid.Text = entry.Uuid.ToHexString();
				ProtectedString pstr = entry.Strings.Get(Defs.ConfigClientId);
				if (pstr != null)
					txtClientId.Text = pstr.ReadString();
				pstr = entry.Strings.Get(Defs.ConfigClientSecret);
				if (pstr != null)
					txtClientSecret.Text = pstr.ReadString();
			}
			txtUuid.Enabled = m_accidx < 0;

			chkOAuth.Checked = !String.IsNullOrEmpty(txtClientId.Text) || !String.IsNullOrEmpty(txtClientSecret.Text);
			txtClientId.Enabled = chkOAuth.Checked;
			txtClientSecret.Enabled = chkOAuth.Checked;

			cbAutoSync.SelectedIndex = (int)m_autoSync;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			string strUuid = txtUuid.Text.Trim().ToUpper();

			if (String.IsNullOrEmpty(strUuid))
			{
				DialogResult dlgr = MessageBox.Show("Remove Google Account association from KeePass config?", Defs.ProductName(), MessageBoxButtons.YesNoCancel);
				if (DialogResult.Yes != dlgr)
					DialogResult = DialogResult.None;
				return;
			}

			if (!Regex.IsMatch(strUuid, "^[0-9A-F]{32}$"))
			{
				MessageBox.Show("The entered UUID is not valid.", Defs.ProductName());
				DialogResult = DialogResult.None;
				return;
			}

			if (chkOAuth.Checked && (String.IsNullOrEmpty(txtClientId.Text.Trim()) || String.IsNullOrEmpty(txtClientSecret.Text.Trim())))
			{
				MessageBox.Show("Please enter a valid custom Google OAuth 2.0 Client ID and Client Secrect for " + Defs.ProductName() + " or use default values.", Defs.ProductName());
				DialogResult = DialogResult.None;
				return;
			}

			m_uuid = strUuid;
			if (chkOAuth.Checked)
			{
				m_clientId = txtClientId.Text.Trim();
				m_clientSecret = txtClientSecret.Text.Trim();
			}
			m_autoSync = (AutoSyncMode)cbAutoSync.SelectedIndex;
		}

		private void cbAccount_SelectedIndexChanged(object sender, EventArgs e)
		{
			txtUuid.Text = string.Empty;
			txtClientId.Text = string.Empty;
			txtClientSecret.Text = string.Empty;

			if (cbAccount.SelectedIndex >= 1)
			{
				txtUuid.Enabled = false;
				PwEntry entry = m_accounts.GetAt((uint)cbAccount.SelectedIndex - 1);
				txtUuid.Text = entry.Uuid.ToHexString();
				ProtectedString pstr = entry.Strings.Get(Defs.ConfigClientId);
				if (pstr != null)
					txtClientId.Text = pstr.ReadString();
				pstr = entry.Strings.Get(Defs.ConfigClientSecret);
				if (pstr != null)
					txtClientSecret.Text = pstr.ReadString();
			}
			else
			{
				txtUuid.Enabled = true;
			}

			chkOAuth.Checked = !String.IsNullOrEmpty(txtClientId.Text) || !String.IsNullOrEmpty(txtClientSecret.Text);
			txtClientId.Enabled = chkOAuth.Checked;
			txtClientSecret.Enabled = chkOAuth.Checked;
		}

		private void chkOAuth_CheckedChanged(object sender, EventArgs e)
		{
			txtClientId.Enabled = chkOAuth.Checked;
			txtClientSecret.Enabled = chkOAuth.Checked;
		}

		private void lnkHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(Defs.URLHome);
		}

		private void lnkHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(Defs.URLHelp);
		}

		private void lnkGoogle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(Defs.URLGoogleDev);
		}

		private void lblVersion_DoubleClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Defs.URLHome);
		}
	}
}
