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
		private string m_authCode;
		private string m_user;
		private ProtectedString m_password;

		public GoogleAuthenticateForm(string user, ProtectedString password)
		{
			InitializeComponent();
			m_authCode = string.Empty;
			m_user = user;
			m_password = password;
			webBrowser1.ScriptErrorsSuppressed = true;
			Visible = false;
		}

		public WebBrowser Browser
		{
			get { return this.webBrowser1; }
		}

		public string AuthCode
		{
			get { return m_authCode; }
		}

		void webBrowser1_DocumentTitleChanged(object sender, System.EventArgs e)
		{
			string title = webBrowser1.DocumentTitle;
			this.Text = title;
			if (title.Contains("code="))
			{
				int indexStart = title.IndexOf("=") + 1;
				m_authCode = title.Substring(indexStart);
				this.Close();
			}
		}

		void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (e.Url.AbsolutePath.Equals("/ServiceLogin"))
			{
				webBrowser1.Document.GetElementById("Email").SetAttribute("value", m_user);
				webBrowser1.Document.GetElementById("Passwd").SetAttribute("value", m_password.ReadString());
				webBrowser1.Document.GetElementById("signIn").Focus();
			}
		}
	}
}
