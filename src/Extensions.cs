/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright © 2012-2016  DesignsInnovate
 * Copyright © 2014-2016  Paul Voegler
 * 
 * KPSync for Google Drive
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

namespace KPSyncForDrive
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

        // Get or create a persistent ID for the database.
        public static Guid GetUuid(this PwDatabase db)
        {
            if (db == null || !db.IsOpen)
            {
                return Guid.Empty;
            }

            const string Key = "KeePassSyncForDrive.Extensions.GetDatabaseUuid";
            const string guidFmt = "D";

            string stringVal = db.CustomData.Get(Key);
            if (stringVal == null)
            {
                stringVal = "";
            }
            Guid retVal;
            if (!Guid.TryParseExact(stringVal, guidFmt, out retVal))
            {
                retVal = Guid.NewGuid();
                db.CustomData.Set(Key, retVal.ToString(guidFmt));
            }
            return retVal;
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
            if (This == null)
            {
                throw new ArgumentNullException();
            }
            if (ps == null)
            {
                return false;
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
        /// Show a message in the KeePass status bar.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isStatusMessage"></param>
        public static void ShowStatusMessage(this IPluginHost host, string msg)
        {
            MainForm window = host.MainWindow;
            if (window.InvokeRequired)
            {
                window.BeginInvoke(new MethodInvoker(() =>
                {
                    ShowStatusMessage(host, msg);
                }));
                return;
            }
            window.SetStatusEx(GdsDefs.ProductName + ": " + msg);
        }

        /// <summary>
        /// Show message as KeePass parented alert dialog with optional
        /// help link.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="link"></param>
        public static void ShowDlgMessage(this IPluginHost host, string msg,
            Uri link = null)
        {
            MainForm window = host.MainWindow;
            if (window.InvokeRequired)
            {
                window.BeginInvoke(new MethodInvoker(() =>
                {
                    ShowDlgMessage(host, msg, link);
                }));
                return;
            }
            using(new CenterWin32Dlg(window))
            {
                if (link == null)
                {
                    MessageBox.Show(window, msg, GdsDefs.ProductName);
                }
                else if (DialogResult.Yes ==
                        MessageBox.Show(window, msg + Environment.NewLine +
                            Environment.NewLine +
                            Resources.GetString("Msg_MsgBoxLinkPrompt"),
                            GdsDefs.ProductName,
                            MessageBoxButtons.YesNo, MessageBoxIcon.None,
                            MessageBoxDefaultButton.Button2))
                {
                    System.Diagnostics.Process.Start(link.AbsoluteUri);
                }
            }
        }
    }
}
