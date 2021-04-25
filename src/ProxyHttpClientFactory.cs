/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * KPSync for Google Drive
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

using Google.Apis.Http;
using KeePass.App.Configuration;
using KeePassLib;
using KeePassLib.Utility;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace KPSyncForDrive
{
    class TestWebProxy : IWebProxy
    {
        readonly WebProxy m_inner;

        public TestWebProxy(string server)
        {
            m_inner = new WebProxy(server);
        }

        public TestWebProxy(string server, int port)
        {
            m_inner = new WebProxy(server, port);
        }

        public ICredentials Credentials
        {
            get
            {
                return m_inner.Credentials;
            }
            set
            {
                m_inner.Credentials = value;
            }
        }

        public Uri GetProxy(Uri destination)
        {
            Debug.WriteLine(string.Format("Proxying '{0}'.",
                destination.AbsoluteUri));
            return m_inner.GetProxy(destination);
        }

        public bool IsBypassed(Uri host)
        {
            bool retVal = m_inner.IsBypassed(host);
            if (retVal)
            {
                Debug.WriteLine(string.Format("BYPASSING PROXY for '{0}'.",
                                                host.AbsoluteUri));
            }
            return retVal;
        }
    }

    class ProxyHttpClientFactory : HttpClientFactory
    {
        IWebProxy m_proxy;

        public ProxyHttpClientFactory()
        {
            AceIntegration config = KeePass.Program.Config.Integration;
            Server = config.ProxyAddress;
            Port = config.ProxyPort;
            ProxyType = config.ProxyType;
            Password = config.ProxyPassword;
            User = config.ProxyUserName;
            if (config.ProxyAuthType == ProxyAuthType.Auto)
            {
                AuthType = HasCredentials ?
                    ProxyAuthType.Manual : ProxyAuthType.Default;
            }
            m_proxy = null;
        }

        string Server { get; set; }

        string Port { get; set; }

        ProxyServerType ProxyType { get; set; }

        ProxyAuthType AuthType { get; set; }

        string Password { get; set; }

        string User { get; set; }

        bool HasCredentials
        {
            get
            {
                return !string.IsNullOrEmpty(User) ||
                    !string.IsNullOrEmpty(Password);
            }
        }

        protected override HttpClientHandler CreateClientHandler()
        {
            HttpClientHandler retVal = base.CreateClientHandler();
            IWebProxy proxy;
            if (m_proxy == null &&
                TryGetWebProxy(out proxy))
            {
                m_proxy = proxy;
            }
            retVal.Proxy = m_proxy;
            return retVal;
        }

        // The logic below this comment is "borrowed" from 
        // KeePass.Lib.Serialization.IOConnection.

        bool TryGetWebProxy(out IWebProxy proxy)
        {
            if (TryGetWebProxyServer(out proxy))
            {
                AssignCredentials(proxy);
                return true;
            }
            return false;
        }

        void AssignCredentials(IWebProxy proxy)
        {
            if (proxy == null)
            {
                return;
            }

            try
            {
                if (AuthType == ProxyAuthType.None)
                {
                    proxy.Credentials = null;
                }
                else if (AuthType == ProxyAuthType.Default)
                {
                    proxy.Credentials = CredentialCache.DefaultCredentials;
                }
                else if (AuthType == ProxyAuthType.Manual && HasCredentials)
                {
                    proxy.Credentials
                        = new NetworkCredential(User, Password);
                }
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }
        }

        bool TryGetWebProxyServer(out IWebProxy proxy)
        {
            proxy = null;

            if (ProxyType == ProxyServerType.None)
            {
                return true; // Use null proxy
            }

            if (ProxyType == ProxyServerType.Manual)
            {
                try
                {
                    if (string.IsNullOrEmpty(Server))
                    {
                        // First try default (from config), then system
                        proxy = WebRequest.DefaultWebProxy;
                        if (proxy == null)
                        {
                            proxy = WebRequest.GetSystemWebProxy();
                        }
                    }
                    else if (!string.IsNullOrEmpty(Port))
                    {
                        ushort portno;
                        if (!ushort.TryParse(Port, out portno))
                        {
                            throw new Exception(
                                "Unexpected port number value.");
                        }
                        proxy = new WebProxy(Server, portno);
                    }
                    else
                    {
                        proxy = new WebProxy(Server);
                    }

                    return proxy != null;
                }
                catch (Exception e)
                {
                    StringBuilder strInfo = new StringBuilder(Server);
                    if (!string.IsNullOrEmpty(Port))
                    {
                        strInfo.Append(':');
                        strInfo.Append(Port);
                    }
                    MessageService.ShowWarning(strInfo, e);
                }

                return false; // Use default
            }

            Debug.Assert(ProxyType == ProxyServerType.System);
            try
            {
                // First try system, then default (from config)
                proxy = WebRequest.GetSystemWebProxy();
                if (proxy == null)
                {
                    proxy = WebRequest.DefaultWebProxy;
                }

                return proxy != null;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error fetching proxy config.");
            }
            return false;
        }
    }
}
