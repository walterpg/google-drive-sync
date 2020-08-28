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

using Google.Apis.Drive.v3;
using KeePassLib.Security;
using KeePassSyncForDrive;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleDriveSync
{
    class ConfigurationFormData : IDisposable
    {
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
            DefaultUseKpgs3ClientId = 
                SyncConfiguration.IsEmpty(
                    PluginConfig.Default.PersonalClientId,
                    PluginConfig.Default.PersonalClientSecret);
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

        public bool CmdSyncEnabled
        {
            get
            {
                return PluginConfig.Default.IsCmdEnabled(SyncCommands.SYNC);
            }
            set
            {
                PluginConfig.Default.EnableCmd(SyncCommands.SYNC, value);
            }
        }

        public bool CmdUploadEnabled
        {
            get
            {
                return PluginConfig.Default.IsCmdEnabled(SyncCommands.UPLOAD);
            }
            set
            {
                PluginConfig.Default.EnableCmd(SyncCommands.UPLOAD, value);
            }
        }

        public bool CmdDownloadEnabled
        {
            get
            {
                return PluginConfig.Default.IsCmdEnabled(SyncCommands.DOWNLOAD);
            }
            set
            {
                PluginConfig.Default.EnableCmd(SyncCommands.DOWNLOAD, value);
            }
        }

        public bool SyncOnOpen 
        {
            get
            {
                return PluginConfig.Default.IsAutoSync(AutoSyncMode.OPEN);
            }
            set
            {
                PluginConfig.Default.EnableAutoSync(AutoSyncMode.OPEN, value);
            }
        }

        public bool SyncOnSave 
        {
            get
            {
                return PluginConfig.Default.IsAutoSync(AutoSyncMode.SAVE);
            }
            set
            {
                PluginConfig.Default.EnableAutoSync(AutoSyncMode.SAVE, value);
            }
        }

        public string DefaultAppFolder 
        {
            get
            {
                return PluginConfig.Default.Folder.Trim();
            }
            set
            {
                PluginConfig.Default.Folder = value ?? string.Empty;
            }
        }

        public GoogleColor DefaultAppFolderColor
        {
            get
            {
                return PluginConfig.Default.FolderColor;
            }
            set
            {
                PluginConfig.Default.FolderColor = value;
            }
        }

        public bool DefaultIsLegacyRestrictedDriveScope
        {
            get
            {
                return PluginConfig.Default.LegacyDriveScope ==
                    DriveService.Scope.Drive;
            }
            set
            {
                PluginConfig.Default.LegacyDriveScope = value ?
                    DriveService.Scope.Drive: DriveService.Scope.DriveFile;
            }
        }

        public string DefaultLegacyClientId
        {
            get
            {
                return DefaultUseKpgs3ClientId ?
                    string.Empty : PluginConfig.Default.PersonalClientId;
            }
            set
            {
                PluginConfig.Default.PersonalClientId = value;
            }
        }

        public ProtectedString DefaultLegacyClientSecret
        {
            get
            {
                return DefaultUseKpgs3ClientId ?
                    GdsDefs.PsEmptyEx : PluginConfig.Default.PersonalClientSecret;
            }
            set
            {
                PluginConfig.Default.PersonalClientSecret = value == null ?
                    GdsDefs.PsEmptyEx : value;
            }
        }

        public bool DefaultUseKpgs3ClientId { get; set; }

        public bool DefaultUseLegacyCredentials
        {
            get
            {
                return PluginConfig.Default.UseLegacyAppCredentials;
            }
            set
            {
                PluginConfig.Default.UseLegacyAppCredentials = value;
            }
        }

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
