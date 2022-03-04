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
using KeePassLib.Serialization;
using KeePassLib.Utility;
using System;
using System.Collections.Generic;
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


        public static bool TraverseTreePostedNull(PwEntry e)
        {
            if (e == null)
            {
                Log.Debug("TraverseTree posted null entry; ignoring.");
                return true;
            }
            return false;
        }

        public static PluginEntryFactory GetEntryFactory(this PwDatabase db)
        {
            // Search KeePass for pre-existing entry titles.
            string searchString = GdsDefs.ProductName;
            KeePassLib.Delegates.EntryHandler checkEntry = e =>
            {
                if (TraverseTreePostedNull(e))
                {
                    return true;
                }
                return -1 == e.Strings.ReadSafe(PwDefs.TitleField)
                            .IndexOf(searchString,
                                StringComparison.OrdinalIgnoreCase);
            };
            int cDup = 2;
            PwGroup root = db.RootGroup;
            while (!root.TraverseTree(TraversalMethod.PreOrder,
                            null, checkEntry))
            {
                searchString = string.Format("{0}-{1}",
                                    GdsDefs.ProductName, cDup++);
            }

            // Return entry creator with unused title.
            return PluginEntryFactory.CreateDefault(searchString);
        }

        public static List<EntryConfiguration> GetLegacyAccounts(
            this PwDatabase db)
        {
            // Plugin entries, this and legacy, have the google accounts
            // url fragment in their URL field (and apparently, legacy entries
            // might also have this in the title field).  Get a list of all
            // such entries to populate the accounts drop-down in the dialog.
            // Later, respect the very obsolete PwUuid-based configuration
            // option used in very old clients.
            List<EntryConfiguration> acctList = new List<EntryConfiguration>();
            PwGroup root = db.RootGroup;
            PwUuid recyclerID = db.RecycleBinUuid;
            root.TraverseTree(TraversalMethod.PreOrder, null, e =>
            {
                if (TraverseTreePostedNull(e) ||
                    e.ParentGroup == null ||
                    e.ParentGroup.Uuid == null ||
                    e.ParentGroup.Uuid.Equals(recyclerID))
                {
                    return true;
                }
                if (-1 != e.Strings.ReadSafe(PwDefs.UrlField)
                                .IndexOf(GdsDefs.AccountSearchString,
                                    StringComparison.OrdinalIgnoreCase))
                {
                    acctList.Add(new EntryConfiguration(e));
                }
                else if (-1 != e.Strings.ReadSafe(PwDefs.TitleField)
                                .IndexOf(GdsDefs.AccountSearchString,
                                    StringComparison.OrdinalIgnoreCase))
                {
                    acctList.Add(new EntryConfiguration(e));
                }
                return true;
            });
            return acctList;
        }

        static bool InsertActiveEntryHandler(PwDatabase db,
            List<EntryConfiguration> accts, PwEntry e, bool ignoreRecycleBin)
        {
            if (TraverseTreePostedNull(e))
            {
                return true;
            }
            bool isRecyled = e.ParentGroup == null ||
                 e.ParentGroup.Uuid == null ||
                 e.ParentGroup.Uuid.Equals(db.RecycleBinUuid);
            if (ignoreRecycleBin && isRecyled)
            {
                return true;
            }
            EntryConfiguration entry = new EntryConfiguration(e);
            if (entry.ActiveAccount.GetValueOrDefault(false))
            {
                // There should only be one active account.  Any extras
                // have been introduced externally (e.g., Duplicate
                // Entry command, merge, etc.). Insert-sort to return
                // active accounts in the order of decreasing
                // modification time span.
                int i = accts.Select(c => c.Entry)
                    .TakeWhile(f =>
                    {
                        return (f.LastModificationTime ==
                            e.LastModificationTime &&
                            f.CreationTime < e.CreationTime) ||
                                f.LastModificationTime
                                    < e.LastModificationTime;
                    })
                    .Count();
                accts.Insert(i, entry);
                Log.Debug("Found {2}entry candidate '{0}'; current rank {1}.",
                    entry.Entry.Strings.ReadSafe(PwDefs.TitleField), i,
                    isRecyled ? "recycled " : string.Empty);
            }
            return true;
        }

        public static IList<EntryConfiguration> GetActiveAccounts(
            this PwDatabase db, bool ignoreRecycleBin = true)
        {
            List<EntryConfiguration> accounts = new List<EntryConfiguration>();
            if (db.IsOpen)
            {
                db.RootGroup.TraverseTree(TraversalMethod.PreOrder,
                    null, e => InsertActiveEntryHandler(db, accounts, e,
                                    ignoreRecycleBin));
            }
            return accounts;
        }

        /// <summary>
        /// Load the current configuration and adds it to the database
        /// context.  Return true if loaded, false otherwise.
        /// </summary>
        public static bool TryGetDbConfig(this IPluginHost host,
            PwDatabase db, out EntryConfiguration configuration)
        {
            configuration = null;
            if (!db.IsOpen)
            {
                return false;
            }

            // Find the active account.
            IEnumerable<EntryConfiguration> accounts
                = db.GetActiveAccounts(ignoreRecycleBin: true);
            if (accounts.Any())
            {
                configuration = accounts.First();
            }
            else
            {
                try
                {
                    // Attempt to use long-obsolete UUID config.
                    string strUuid = host.GetConfig(GdsDefs.ConfigUUID);
                    if (!string.IsNullOrEmpty(strUuid))
                    {
                        PwEntry entry = db.RootGroup.FindEntry(
                                                strUuid.ToPwUuid(), true);
                        if (entry != null)
                        {
                            configuration = new EntryConfiguration(entry);
                        }
                    }
                }
                catch (SystemException)
                {
                    // Bad config, etc.
                }
            }

            return configuration != null;
        }

        public static string GetDisplayNameAndPath(this PwDatabase db)
        {
            if (db == null)
            {
                return "'(null)'";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append('\'');
            sb.Append(db.Name);
            sb.Append('\'');
            IOConnectionInfo pathInfo = db.IOConnectionInfo;
            if (pathInfo != null &&
                !string.IsNullOrEmpty(pathInfo.Path))
            {
                sb.Append(' ');
                sb.Append('(');
                sb.Append(pathInfo.Path);
                sb.Append(')');
            }
            return sb.ToString();
        }

        // Get or create a persistent ID for the database.
        public static Guid GetKpSyncUuid(this PwDatabase db)
        {
            if (db == null || !db.IsOpen)
            {
                return Guid.Empty;
            }


            string stringVal = db.CustomData.Get(PluginDbStateKeys.UuidKey);
            if (stringVal == null)
            {
                stringVal = string.Empty;
            }
            Guid retVal;
            if (!Guid.TryParseExact(stringVal, PluginDbStateKeys.UuidFmt,
                out retVal))
            {
                retVal = Guid.NewGuid();
                db.CustomData.Set(PluginDbStateKeys.UuidKey,
                    retVal.ToString(PluginDbStateKeys.UuidFmt));
            }
            return retVal;
        }

        public static bool ClearDbState(PwDatabase db)
        {
            Log.Diag("Begin removing all plugin state keys from {0}.",
                db.GetDisplayNameAndPath());

            int cRemoved = 0;
            foreach (string key in PluginDbStateKeys.All)
            {
                if (db.CustomData.Remove(key))
                {
                    cRemoved++;
                    db.MarkDirty();
                }
            }

            Log.Diag("State key removal from {0} complete, {1} keys removed.",
                db.GetDisplayNameAndPath(), cRemoved);
            return cRemoved > 0;
        }

        public static FileEventFlags GetClosingEvent(this PwDatabase db,
            bool bErase)
        {
            return GetFileEventFlags(db, PluginDbStateKeys.ClosingEventKey, bErase);
        }

        public static FileEventFlags GetDeferredAutoSync(this PwDatabase db,
            bool bErase)
        {
            return GetFileEventFlags(db, PluginDbStateKeys.DeferredAutoSyncKey, bErase);
        }

        static FileEventFlags? GetPropAsFileEventFlags(
            StringDictionaryEx props, string key)
        {
            if (!props.Exists(key))
            {
                return null;
            }
            string stringVal = props.Get(key);
            if (string.IsNullOrEmpty(stringVal))
            {
                return null;
            }
            FileEventFlags retVal;
            if (!Enum.TryParse(stringVal, out retVal))
            {
                return null;
            }
            return retVal;
        }

        static FileEventFlags GetFileEventFlags(PwDatabase db,
            string key, bool bErase)
        {
            if (db == null || !db.IsOpen || db.CustomData == null)
            {
                return FileEventFlags.None;
            }
            StringDictionaryEx props = db.CustomData;
            FileEventFlags? retVal = GetPropAsFileEventFlags(props, key);
            if (!retVal.HasValue)
            {
                retVal = FileEventFlags.None;
                bErase = true;
            }
            retVal &= FileEventFlags.Exiting | FileEventFlags.Locking;
            if (bErase)
            {
                SetFileEventFlags(props, key, FileEventFlags.None);
            }
            return retVal.Value;
        }

        static void SetFileEventFlags(StringDictionaryEx props,
            string key, FileEventFlags flag)
        {
            // 'None' is special: it is equivalent to no key,
            // or, on retrieval, an invalid value.
            // Don't set 'None' if the key doesn't exist.
            if (flag == FileEventFlags.None && !props.Exists(key))
            {
                return;
            }
            // Setting a value marks the database "dirty", even when the
            // value isn't changed. Avoid when the value is already set.
            FileEventFlags? existingValue
                = GetPropAsFileEventFlags(props, key);
            if (existingValue.HasValue &&
                existingValue.Value == flag)
            {
                return;
            }
            props.Set(key, flag.ToString("G"));
        }

        public static void SetClosingEvent(this PwDatabase db,
            FileEventFlags f)
        {
            SetFileEventFlags(db, PluginDbStateKeys.ClosingEventKey, f);
        }

        public static void SetDeferrredAutoSync(this PwDatabase db,
            FileEventFlags f)
        {
            SetFileEventFlags(db, PluginDbStateKeys.DeferredAutoSyncKey, f);
        }

        public static void SetFileEventFlags(PwDatabase db,
            string key, FileEventFlags reason)
        {
            if (db == null || !db.IsOpen || db.CustomData == null)
            {
                return;
            }

            // Record locking, exit, and clearing events.
            reason &= FileEventFlags.Exiting | FileEventFlags.Locking;

            SetFileEventFlags(db.CustomData, key, reason);
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
            Log.Info(msg);
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
