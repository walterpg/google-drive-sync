using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoogleSyncPlugin
{
    public partial class Form1 : Form
    {
        private string authCode;
        private string m_user;
        private string m_password;
        public Form1(string user, string password)
        {
            InitializeComponent();
            authCode = "access_denied";
            m_user = user;
            m_password = password;
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        public WebBrowser Browser
        {
            get { return this.webBrowser1; }
        }

        public string AuthCode
        {
            get { return authCode; }
        }
        void webBrowser1_DocumentTitleChanged(object sender, System.EventArgs e)
        {
            string title = webBrowser1.DocumentTitle;
            this.Text = title;
            if (title.Contains("code=") || title.Contains("error="))
            {
                int indexStart = title.IndexOf("=") + 1;
                authCode = title.Substring(indexStart);
                this.Close();
            }
        }
        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath.Equals("/ServiceLogin"))
            {
                webBrowser1.Document.GetElementById("Email").SetAttribute("value", m_user);
                webBrowser1.Document.GetElementById("Passwd").SetAttribute("value", m_password);
                webBrowser1.Document.Forms[0].InvokeMember("submit");
            }
            else if (e.Url.AbsolutePath.Equals("/o/oauth2/auth"))
            {
                // TODO: allow access button is enabled through javscript code post document complete event
                // at this moment, i'm unable to automate this part
                // When hitting "Allow access" too fast, iOS gives "Javascript is disabled" error messsage
                // http://code.google.com/p/gtm-oauth2/issues/detail?id=11
            }
        }
    }
}
