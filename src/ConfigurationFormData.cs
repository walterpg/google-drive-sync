/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright(C) 2012-2016  DesignsInnovate
 * Copyright(C) 2014-2016  Paul Voegler
 * 
 * Google Drive Sync for KeePass Password Safe
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

using Google.Apis.Drive.v3;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    public class ConfigurationFormData : IDisposable
    {
        string m_defaultAppFolder;
        string m_defaultClientId;
        ProtectedString m_defaultClientSecret;
        IEnumerable<Color> m_colors;
        readonly Func<EntryConfiguration, Task<IEnumerable<Color>>> m_colorProvider;

        public ConfigurationFormData(IList<EntryConfiguration> entries,
            Func<EntryConfiguration, Task<IEnumerable<Color>>> colorProvider)
        {
            Entries = entries;
            EntryBindingSource = new BindingSource
            {
                DataSource = Entries
            };

            m_colors = null;
            m_colorProvider = colorProvider;
            DefaultAppFolderColor = null;
            DefaultUseLegacyClientId = true;
            DefaultClientId = string.Empty;
            DefaultClientSecret = GdsDefs.PsEmptyEx;
        }

        protected void Dispose(bool bIsDisposing)
        {
            if (bIsDisposing)
            {
                EntryBindingSource.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public EntryConfiguration SelectedAccountShadow
        {
            get
            {
                return EntryBindingSource.IsBindingSuspended ?
                    null : EntryBindingSource.Current as EntryConfiguration;
            }
        }

        public BindingSource EntryBindingSource { get; private set; }

        public IList<EntryConfiguration> Entries { get; private set; }

        public bool SyncOnOpen { get; set; }

        public bool SyncOnSave { get; set; }

        public AutoSyncMode AutoSync
        {
            get
            {
                AutoSyncMode mode = AutoSyncMode.DISABLED;
                if (SyncOnOpen)
                {
                    mode |= AutoSyncMode.OPEN;
                }
                if (SyncOnSave)
                {
                    mode |= AutoSyncMode.SAVE;
                }
                return mode;
            }
            set
            {
                SyncOnOpen = (value & AutoSyncMode.OPEN) == AutoSyncMode.OPEN;
                SyncOnSave = (value & AutoSyncMode.SAVE) == AutoSyncMode.SAVE;
            }
        }


        public string DefaultAppFolder 
        {
            get
            {
                return m_defaultAppFolder.Trim();
            }
            set
            {
                m_defaultAppFolder = value ?? string.Empty;
            }
        }

        public GoogleColor DefaultAppFolderColor { get; set; }

        public bool DefaultIsRestrictedDriveScope
        {
            get
            {
                return DefaultApiScope == DriveService.Scope.DriveFile;
            }
            set
            {
                DefaultApiScope = value ?
                    DriveService.Scope.DriveFile : DriveService.Scope.Drive;
            }
        }

        public string DefaultApiScope { get; set; }

        public string DefaultClientId
        {
            get
            {
                return DefaultUseLegacyClientId ?
                    string.Empty : m_defaultClientId;
            }
            set
            {
                m_defaultClientId = value;
            }
        }

        public ProtectedString DefaultClientSecret
        {
            get
            {
                return DefaultUseLegacyClientId ?
                    GdsDefs.PsEmptyEx : m_defaultClientSecret;
            }
            set
            {
                m_defaultClientSecret = value == null ?
                    GdsDefs.PsEmptyEx : value;
            }
        }

        public bool DefaultUseLegacyClientId { get; set; }

        public async Task<IEnumerable<Color>> GetColors()
        {
            if (m_colors == null)
            {
                EntryConfiguration current;
                current = EntryBindingSource.Current as EntryConfiguration;
                IEnumerable<Color> result = await m_colorProvider(current);
                if (!result.Any())
                {
                    return result;
                }
                m_colors = result;
            }
            return m_colors;
        }
    }
}
