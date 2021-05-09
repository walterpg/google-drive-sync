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

using Google.Apis.Drive.v3;
using KeePass.Plugins;
using KeePassLib.Security;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace KPSyncForDrive
{
    class PluginConfig
    {
        // Ver0
        //    clientID/secret present => custom client id
        //    clientID/secret missing => legacy client id
        //
        // Ver1.0
        //    if Use legacy flag missing or false
        //     => new app client id
        //    else
        //     clientID/secret present => custom client id
        //     clientID/secret missing => legacy client id
        //
        // Ver1.1
        //   Add DontSaveAuthToken config.
        //
        //    Don't prompt for migration. Since Ver0 was not persisted,
        //    there is no very simple way to know if this is a new install or
        //    migration.  This was new functionality in Ver0, which was
        //    pre-release software anyway....

        public const string CurrentVer = "1.1";
        const string ConfigPluginKey = "Plugin.KeePassSyncForDrive";

        class ProtectedStringConverter : JsonConverter<ProtectedString>
        {
            public override ProtectedString ReadJson(JsonReader reader,
                Type objectType, ProtectedString existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                string stringVal = reader.Value as string;
                return !string.IsNullOrEmpty(stringVal) ?
                    new ProtectedString(true, stringVal) : null;
            }

            public override void WriteJson(JsonWriter writer,
                ProtectedString value, JsonSerializer serializer)
            {
                if (value == null || value == GdsDefs.PsEmptyEx)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.WriteValue(value.ReadString());
                }
            }
        }

        class ColorConverter : JsonConverter<GoogleColor>
        {
            public override GoogleColor ReadJson(JsonReader reader,
                Type objectType, GoogleColor existingValue,
                bool hasExistingValue, JsonSerializer serializer)
            {
                string colorStr = reader.Value as string;
                if (string.IsNullOrEmpty(colorStr))
                {
                    return null;
                }
                int iName = colorStr.IndexOf(',');
                int argb;
                if (iName == -1 || iName+1 == colorStr.Length ||
                    !int.TryParse(colorStr.Substring(0,iName), out argb))
                {
                    return null;
                }
                System.Drawing.Color color
                    = System.Drawing.Color.FromArgb(argb);
                string colorName = colorStr.Substring(iName + 1);
                return new GoogleColor(color, colorName);
            }

            public override void WriteJson(JsonWriter writer,
                GoogleColor value, JsonSerializer serializer)
            {
                Debug.Assert(value != null);
                StringBuilder sb = new StringBuilder();
                sb.Append(value.Color.ToArgb());
                sb.Append(',');
                sb.Append(value.Name);
                writer.WriteValue(sb.ToString());
            }
        }

        static PluginConfig()
        {
            Default = new PluginConfig();
        }

        public static PluginConfig Default
        {
            get; private set;
        }

        AutoSyncMode m_autoSync;
        SyncCommands m_enabledCmds;
        string m_defaultFolder;
        GoogleColor m_defaultFolderColor;
        string m_defaultDriveScope;
        string m_defaultClientId;
        ProtectedString m_defaultClientSecret;
        bool m_useLegacyCreds;
        bool m_dontSaveAuthToken;
        bool m_warnSavedAuthToken;
        bool m_isDirty;
        string m_ver;
        bool m_autoResumeSave;

        PluginConfig()
        {
            m_autoSync = AutoSyncMode.DISABLED;
            m_enabledCmds = SyncCommands.All;
            m_defaultFolder = null;
            m_defaultFolderColor = null;
            m_defaultDriveScope = null;
            m_defaultClientId = string.Empty;
            m_defaultClientSecret = GdsDefs.PsEmptyEx;
            m_useLegacyCreds = false;
            m_dontSaveAuthToken = false;
            m_warnSavedAuthToken = false;
            m_isDirty = true;
            m_ver = null;
            m_autoResumeSave = false;
        }

        PluginConfig(PluginConfig c)
        {
            m_autoSync = c.m_autoSync;
            m_enabledCmds = c.m_enabledCmds;
            m_defaultFolder = c.m_defaultFolder;
            m_defaultFolderColor = c.m_defaultFolderColor;
            m_defaultDriveScope = c.m_defaultDriveScope;
            m_defaultClientId = c.m_defaultClientId;
            m_defaultClientSecret = c.m_defaultClientSecret;
            m_useLegacyCreds = c.m_useLegacyCreds;
            m_dontSaveAuthToken = c.m_dontSaveAuthToken;
            m_warnSavedAuthToken = c.m_warnSavedAuthToken;
            m_isDirty = c.m_isDirty;
            m_ver = c.m_ver;
            m_autoResumeSave = c.m_autoResumeSave;
        }

        public static PluginConfig GetCopyOfDefault()
        {
            return new PluginConfig(Default);
        }

        public static void UpdateDefault(PluginConfig c)
        {
            Default = c;
        }

        public bool IsCmdEnabled(SyncCommands cmd)
        {
            return (EnabledCommands & cmd) == cmd;
        }

        public void EnableCmd(SyncCommands cmd, bool enabled)
        {
            EnabledCommands &= ~cmd;
            if (enabled)
            {
                EnabledCommands |= cmd;
            }
        }

        public bool IsAutoSync(AutoSyncMode mode)
        {
            return IsCmdEnabled(SyncCommands.SYNC) &&
                (AutoSync & mode) == mode;
        }

        public void EnableAutoSync(AutoSyncMode mode, bool enabled)
        {
            AutoSync &= ~mode;
            if (enabled)
            {
                AutoSync |= mode;
            }
        }

        public AutoSyncMode AutoSync
        {
            get { return m_autoSync; }
            set
            {
                if (m_autoSync != value)
                {
                    m_autoSync = value;
                    m_isDirty = true;
                }
            }
        }

        public SyncCommands EnabledCommands
        {
            get { return m_enabledCmds; }
            set
            {
                SyncCommands normalized = value & SyncCommands.All;
                if (m_enabledCmds != normalized)
                {
                    m_enabledCmds = normalized;
                    m_isDirty = true;
                }
            }
        }

        public string Folder
        {
            get { return m_defaultFolder; }
            set
            {
                if (!string.Equals(m_defaultFolder, value,
                        StringComparison.Ordinal))
                {
                    m_defaultFolder = value;
                    m_isDirty = true;
                }
            }
        }

        [JsonConverter(typeof(ColorConverter))]
        public GoogleColor FolderColor
        {
            get { return m_defaultFolderColor; }
            set
            {
                if (value == null)
                {
                    if (m_defaultFolderColor == null)
                    {
                        return;
                    }
                    m_defaultFolderColor = null;
                    m_isDirty = true;
                }
                else
                {
                    string curVal = m_defaultFolderColor == null ?
                        string.Empty : m_defaultFolderColor.HtmlHexString;
                    m_isDirty = !value.HtmlHexString.Equals(curVal,
                        StringComparison.Ordinal);
                }
                m_defaultFolderColor = value;
            }
        }

        public string LegacyDriveScope
        {
            get { return m_defaultDriveScope; }
            set
            {
                if (!string.Equals(m_defaultDriveScope, value,
                        StringComparison.Ordinal))
                {
                    m_defaultDriveScope = value;
                    m_isDirty = true;
                }
            }
        }

        public string PersonalClientId
        {
            get { return m_defaultClientId; }
            set
            {
                if (!string.Equals(m_defaultClientId, value,
                        StringComparison.Ordinal))
                {
                    m_defaultClientId = value;
                    m_isDirty = true;
                }
            }
        }

        [JsonConverter(typeof(ProtectedStringConverter))]
        public ProtectedString PersonalClientSecret
        {
            get { return m_defaultClientSecret; }
            set
            {
                if (value == null)
                {
                    if (m_defaultClientSecret == null)
                    {
                        return;
                    }
                    m_defaultClientSecret = null;
                    m_isDirty = true;
                }
                else
                {
                    m_isDirty = !value.OrdinalEquals(m_defaultClientSecret,
                                                    true);
                }
                m_defaultClientSecret = value;
            }
        }

        public bool UseLegacyAppCredentials
        {
            get { return m_useLegacyCreds; }
            set
            {
                if (m_useLegacyCreds != value)
                {
                    m_isDirty = true;
                    m_useLegacyCreds = value;
                }
            }
        }

        public string ConfigVersion
        {
            get
            {
                return m_ver;
            }
            set
            {
                if (m_ver != value)
                {
                    m_ver = value;
                    m_isDirty = true;
                }
            }
        }

        public bool DontSaveAuthToken
        {
            get
            {
                return m_dontSaveAuthToken;
            }
            set
            {
                if (m_dontSaveAuthToken != value)
                {
                    m_dontSaveAuthToken = value;
                    m_isDirty = true;
                }
            }
        }

        public bool WarnOnSavedAuthToken
        {
            get
            {
                return m_warnSavedAuthToken;
            }
            set
            {
                if (m_warnSavedAuthToken != value)
                {
                    m_warnSavedAuthToken = value;
                    m_isDirty = true;
                }
            }
        }

        public bool AutoResumeSaveSync
        {
            get
            {
                return m_autoResumeSave;
            }
            set
            {
                if (m_autoResumeSave != value)
                {
                    m_autoResumeSave = value;
                    m_isDirty = true;
                }
            }
        }

        public void UpdateConfig(IPluginHost host)
        {
            Version currentVer, configVer;
            currentVer = new Version(CurrentVer);
            configVer = new Version(ConfigVersion);

            if (!m_isDirty && configVer >= currentVer)
            {
                return;
            }

            // New properties take default values. No mods to existing
            // properties required so far.
            ConfigVersion = CurrentVer;

            JsonSerializerSettings serSettings = new JsonSerializerSettings()
            {
                //Formatting = Formatting.Indented
            };
            JsonSerializer ser = JsonSerializer.CreateDefault(serSettings);
            StringWriter sw = new StringWriter();
            using (sw)
            {
                ser.Serialize(sw, this);
                host.SetConfig(ConfigPluginKey, sw.ToString());
            }
            Debug.Assert(!host.MainWindow.InvokeRequired);
            host.MainWindow.SaveConfig();

            m_isDirty = false;
        }

        public static PluginConfig InitDefault(IPluginHost host)
        {
            PluginConfig update;

            string json = host.GetConfig(ConfigPluginKey);
            if (string.IsNullOrEmpty(json) || 
                InitFromJson(json, out update) == null)
            {
                update = InitLegacyDefault(host);

                // Force update to new config.
                update.m_isDirty = true;
            }

            update.UpdateConfig(host);
            Default = update;
            return Default;
        }

        static PluginConfig InitFromJson(string json, out PluginConfig update)
        {
            update = null;
            JsonSerializerSettings serSettings = new JsonSerializerSettings()
            {
            };
            JsonSerializer ser = JsonSerializer.CreateDefault(serSettings);
            StringReader sr = new StringReader(json);
            using (sr)
            {
                update = ser.Deserialize(sr, typeof(PluginConfig))
                                as PluginConfig;
                if (update != null)
                {
                    update.m_isDirty = false;
                }
            }
            return update;
        }

        static PluginConfig InitLegacyDefault(IPluginHost host)
        {
            const string ConfigAutoSyncKey = "GoogleSync.AutoSync";
            const string ConfigEnabledCmdsKey = "GoogleSync.EnabledCmds";
            const string ConfigDefaultAppFolderKey = "GoogleSync.DefaultAppFolder";
            const string ConfigDriveScopeKey = "GoogleSync.DriveApiScope";
            const string ConfigDefaultClientIdKey = "GoogleSync.DefaultClientId";
            const string ConfigDefaultClientSecretKey = "GoogleSync.DefaultClientSecret";
            const string ConfigVersionKey = "GoogleSync.ConfigVersion";
            const string Ver0 = "0.0"; // virtual version

            PluginConfig update = new PluginConfig();

            string verString = host.GetConfig(ConfigVersionKey, "invalid");
            Version configVersion;
            if (!Version.TryParse(verString, out configVersion))
            {
                if (null != host.GetConfig(GdsDefs.ConfigUUID) ||
                    null != host.GetConfig(ConfigAutoSyncKey))
                {
                    // The alpha and early betas introduced ConfigVersionKey.
                    // The legacy plugin did not have this config. The
                    // "virtual" Ver0 is reserved to signify the legacy plugin.
                    // The only way to infer an upgrade from legacy plugin
                    // is to look for the global config keys it saved after
                    // a successful configuration.
                    configVersion = new Version(Ver0);
                }
                else
                {
                    configVersion = new Version(CurrentVer);
                }
            }
            update.ConfigVersion = configVersion.ToString(2);

            string cmds = host.GetConfig(ConfigEnabledCmdsKey,
                ((int)SyncCommands.All).ToString(
                    NumberFormatInfo.InvariantInfo));
            int cmdsAsInt;
            if (!int.TryParse(cmds, NumberStyles.Integer,
                NumberFormatInfo.InvariantInfo, out cmdsAsInt))
            {
                cmdsAsInt = (int)SyncCommands.All;
            }
            update.EnabledCommands = (SyncCommands)cmdsAsInt;

            string syncOption = host.GetConfig(ConfigAutoSyncKey,
                                        AutoSyncMode.DISABLED.ToString());
            AutoSyncMode mode;
            if (!Enum.TryParse(syncOption, out mode))
            {
                // Support obsolete Sync on Save confg.
                if (host.GetConfig(ConfigAutoSyncKey, false))
                {
                    mode = AutoSyncMode.SAVE;
                }
                else
                {
                    mode = AutoSyncMode.DISABLED;
                }
            }
            update.AutoSync = mode;

            update.Folder = host.GetConfig(ConfigDefaultAppFolderKey,
                                            string.Empty);
            update.FolderColor = null;

            update.LegacyDriveScope
                = host.GetConfig(ConfigDriveScopeKey,
                                    DriveService.Scope.Drive);

            // Default is no OAuth 2.0 credentials.
            update.PersonalClientId
                = host.GetConfig(ConfigDefaultClientIdKey, string.Empty);
            string secretVal
                = host.GetConfig(ConfigDefaultClientSecretKey, string.Empty);
            update.PersonalClientSecret
                = string.IsNullOrEmpty(update.PersonalClientId) ?
                    null : new ProtectedString(true, secretVal);

            // If the user wants to enable legacy creds, there is nothing
            // stopping her.  This is not a compatibility point.
            update.UseLegacyAppCredentials
                = host.GetConfig(SyncConfiguration.EntryUseLegacyCredsKey,
                    false);

            return update;
        }
    }
}
