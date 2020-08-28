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

using KeePass.DataExchange;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Interfaces;
using KeePassLib.Security;
using KeePassLib.Serialization;
using System.IO;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    internal class PluginEntryFactory : FileFormatProvider
    {
        public static PluginEntryFactory Create(string title,
            string driveScope, string clientId, ProtectedString clientSecret,
            string folder, bool useLegacyCreds)
        {
            return new PluginEntryFactory(title, driveScope, clientId,
                                            clientSecret, folder,
                                            useLegacyCreds);
        }

        public static PwEntry Import(IPluginHost host,
            PluginEntryFactory creator)
        {
            Form fParent = host.MainWindow;
            IUIOperations uiOps = host.MainWindow;
            IOConnectionInfo[] vCx = new IOConnectionInfo[] { };
            bool? success = ImportUtil.Import(host.Database, creator,
                vCx, false, uiOps, false, fParent);
            if (success.HasValue && success.Value &&
                host.MainWindow.GetSelectedGroup()
                    == host.Database.RootGroup)
            {
                host.MainWindow.UpdateUI(false, null, false, null,
                    true, host.Database.RootGroup, true);
            }
            return creator.Entry;
        }

        PluginEntryFactory(string title, string driveScope, string clientId,
            ProtectedString clientSecret, string folder, bool useLegacyCreds)
        {
            Entry = new PwEntry(true, true);
            ProtectedStringDictionary strings = Entry.Strings;
            strings.Set(PwDefs.TitleField,
                new ProtectedString(false, title));
            strings.Set(PwDefs.NotesField,
                new ProtectedString(false, 
                    Resources.GetFormat("Msg_NewEntryNotesFmt",
                                        GdsDefs.ProductName)));
            strings.Set(PwDefs.UrlField,
                new ProtectedString(false, GdsDefs.AccountSearchString));

            StringDictionaryEx data = Entry.CustomData;
            if (useLegacyCreds)
            {
                data.Set(GdsDefs.EntryDriveScope, driveScope);
                data.Set(GdsDefs.EntryClientId, clientId);
                data.Set(GdsDefs.EntryClientSecret, clientSecret.ReadString());
            }
            data.Set(GdsDefs.EntryActiveAppFolder, folder);
            data.Set(GdsDefs.EntryActiveAccount, GdsDefs.EntryActiveAccountTrue);
            data.Set(GdsDefs.EntryUseLegacyCreds, useLegacyCreds ?
                GdsDefs.ConfigTrue : GdsDefs.ConfigFalse);
            data.Set(GdsDefs.EntryVersion, 
                SyncConfiguration.CurrentVersion.ToString(2));

            Entry.IconId = PwIcon.WorldComputer;
        }

        public PwEntry Entry { get; private set; }

        public override void Import(PwDatabase pwStorage,
            Stream sInput, IStatusLogger slLogger)
        {
            pwStorage.RootGroup.AddEntry(Entry, true);
        }

        public override bool ImportAppendsToRootGroupOnly
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsImport
        {
            get
            {
                return true;
            }
        }

        public override bool SupportsExport
        {
            get
            {
                return false;
            }
        }

        public override string FormatName
        {
            get
            {
                return GetType().FullName + '-' +
                    SyncConfiguration.CurrentVersion.ToString(2);
            }
        }
    }
}