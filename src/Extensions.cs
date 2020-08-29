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

using KeePass.Forms;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassLib.Utility;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    static class Extensions
    {
        /// <summary>
        /// Returns URL-safe Base64-encoded data.  See RFC 4648 Section 5.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ToUrlSafeBase64(this byte[] buffer)
        {
            string unsafeBase64 = Convert.ToBase64String(buffer);
            return unsafeBase64.Aggregate(
                new StringBuilder(unsafeBase64.Length),
                (sb, c) =>
                {
                    switch (c)
                    {
                        case '+':
                            sb.Append('-');
                            break;
                        case '/':
                            sb.Append('_');
                            break;
                        case '=':
                            // Strip pad char if any.
                            break;
                        default:
                            sb.Append(c);
                            break;
                    }
                    return sb;
                },
                sb => sb.ToString());
        }

        public static PwUuid ToPwUuid(this string uuidStr)
        {
            byte[] uuidBytes = MemUtil.HexStringToByteArray(uuidStr);
            return new PwUuid(uuidBytes);
        }

        public static string GetConfig(this IPluginHost host,
            string key, string defaultValue = null)
        {
            return host.CustomConfig.GetString(key, defaultValue);
        }

        public static bool GetConfig(this IPluginHost host,
            string key, bool defaultValue)
        {
            return host.CustomConfig.GetBool(key, defaultValue);
        }

        public static void SetConfig(this IPluginHost host,
            string key, string value)
        {
            host.CustomConfig.SetString(key, value);
        }

        /// <summary>
        /// This is ProtectedString.Equals but with StringComparison.Ordinal
        /// for non-encrypted strings.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="ps"></param>
        /// <param name="bCheckProtEqual"></param>
        /// <returns></returns>
        public static bool OrdinalEquals(this ProtectedString This,
            ProtectedString ps, bool bCheckProtEqual)
        {
            if (ps == null || This == null)
            {
                throw new ArgumentNullException();
            }
            if (ReferenceEquals(This, ps))
            {
                return true;
            }

            bool bPA = This.IsProtected, bPB = ps.IsProtected;
            if (bCheckProtEqual && (bPA != bPB))
            {
                return false;
            }
            if (!bPA && !bPB)
            {
                return This.ReadString().Equals(ps.ReadString(),
                                            StringComparison.Ordinal);
            }

            byte[] pbA = This.ReadUtf8(), pbB = null;
            bool bEq;
            try
            {
                pbB = ps.ReadUtf8();
                bEq = MemUtil.ArraysEqual(pbA, pbB);
            }
            finally
            {
                if (bPA)
                {
                    MemUtil.ZeroByteArray(pbA);
                }
                if (bPB && (pbB != null))
                {
                    MemUtil.ZeroByteArray(pbB);
                }
            }

            return bEq;
        }

        public static bool IsNullOrEmpty(this ProtectedString ps)
        {
            return ps == null || ps.IsEmpty;
        }

        public static bool ModIfNeeded(this PwEntry entry,
            string key, ProtectedString value)
        {
            ProtectedStringDictionary strings = entry.Strings;
            if (value == null)
            {
                return strings.Remove(key);
            }
            else if (!strings.Exists(key) ||
                 !strings.Get(key).OrdinalEquals(value, true))
            {
                strings.Set(key, value);
                return true;
            }
            return false;
        }

        public static bool ModIfNeeded(this PwEntry entry,
            string key, string value)
        {
            ProtectedString ps = value == null ?
                null : new ProtectedString(false, value);
            return ModIfNeeded(entry, key, ps);
        }

        public static bool RemoveIfPresent(this PwEntry entry,
            string key)
        {
            return ModIfNeeded(entry, key, null as ProtectedString);
        }

        // UI

        /// <summary>
        /// Show message as KeePass parented alert or in its status bar
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isStatusMessage"></param>
        public static void ShowMessage(this IPluginHost host, string msg,
            bool isStatusMessage = false)
        {
            MainForm window = host.MainWindow;
            if (window.InvokeRequired)
            {
                window.BeginInvoke(new MethodInvoker(() =>
                {
                    ShowMessage(host, msg, isStatusMessage);
                }));
                return;
            }
            if (isStatusMessage)
            {
                window.SetStatusEx(GdsDefs.ProductName + ": " + msg);
            }
            else
            {
                using(new CenterWin32Dlg(window))
                {
                    MessageBox.Show(window, msg, GdsDefs.ProductName);
                }
            }
        }
    }
}
