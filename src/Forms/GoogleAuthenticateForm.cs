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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KeePassLib.Security;


namespace GoogleSyncPlugin
{
	public partial class GoogleAuthenticateForm : Form
	{
		private Uri m_uri = null;
		private string m_email = string.Empty;
		private ProtectedString m_passwd = null;
		private bool m_success = false;
		private string m_code = "access_denied";

		public GoogleAuthenticateForm(Uri uri, string email, ProtectedString password)
		{
			InitializeComponent();

			m_uri = uri;
			if (!String.IsNullOrEmpty(email))
				m_uri = new Uri(m_uri.OriginalString + "&login_hint=" + Uri.EscapeDataString(email));
			m_email = email;
			m_passwd = password;

			this.Visible = false;
		}

		public bool Success
		{
			get { return m_success; }
		}

		public string Code
		{
			get { return m_code; }
		}

		private void GoogleAuthenticateForm_Load(object sender, EventArgs e)
		{
			webBrowser1.Navigate(m_uri);
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			string title = webBrowser1.DocumentTitle;

			if (e.Url.AbsolutePath.Equals("/ServiceLogin"))
			{
				HtmlElement elEmail = webBrowser1.Document.GetElementById("Email");
				if (elEmail != null && String.IsNullOrEmpty(elEmail.GetAttribute("value")) && m_email != null && !String.IsNullOrEmpty(m_email))
					elEmail.SetAttribute("value", m_email);

				HtmlElement elPasswd = webBrowser1.Document.GetElementById("Passwd");
				if (elPasswd != null && String.IsNullOrEmpty(elPasswd.GetAttribute("value")) && m_passwd != null && !m_passwd.IsEmpty)
					elPasswd.SetAttribute("value", m_passwd.ReadString());
			}
			else if (title.Contains("code=") || title.Contains("error="))
			{
				int indexStart = title.IndexOf("=") + 1;
				m_code = title.Substring(indexStart);
				if (title.Contains("code="))
					m_success = true;
				this.Close();
			}
		}
	}
}
