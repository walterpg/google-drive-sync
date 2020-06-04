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
using KeePass.Plugins;
using KeePassLib.Security;
using System;
using System.Globalization;

namespace GoogleDriveSync
{
	class PluginConfig
	{
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
		bool m_isDirty;

		PluginConfig()
		{
			m_autoSync = AutoSyncMode.DISABLED;
			m_enabledCmds = SyncCommands.All;
			m_defaultFolder = null;
			m_defaultFolderColor = null;
			m_defaultDriveScope = null;
			m_defaultClientId = string.Empty;
			m_defaultClientSecret = GdsDefs.PsEmptyEx;
			m_isDirty = false;
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
				if (m_enabledCmds != value)
				{
					m_enabledCmds = value;
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

		public string DriveScope
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

		public string ClientId
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

		public ProtectedString ClientSecret
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

		public void UpdateConfig(IPluginHost host)
		{
			if (!m_isDirty)
			{
				return;
			}
			host.SetConfig(GdsDefs.ConfigEnabledCmds,
				((int)m_enabledCmds).ToString(NumberFormatInfo.InvariantInfo));
			host.SetConfig(GdsDefs.ConfigAutoSync, m_autoSync.ToString());
			host.SetConfig(GdsDefs.ConfigDefaultAppFolder, m_defaultFolder);
			host.SetConfig(GdsDefs.ConfigDefaultAppFolderColor,
				m_defaultFolderColor == null ? null :
				GoogleColor.SerializeToString(m_defaultFolderColor));
			host.SetConfig(GdsDefs.ConfigDriveScope, m_defaultDriveScope);
			host.SetConfig(GdsDefs.ConfigDefaultClientId, m_defaultClientId);
			host.SetConfig(GdsDefs.ConfigDefaultClientSecret,
				m_defaultClientSecret == null ? string.Empty :
				m_defaultClientSecret.ReadString());
		}

		public static void InitAppConfig(IPluginHost host)
		{
			PluginConfig update = new PluginConfig();
			string cmds = host.GetConfig(GdsDefs.ConfigEnabledCmds,
				((int)SyncCommands.All).ToString(
					NumberFormatInfo.InvariantInfo));
			int cmdsAsInt;
			if (!int.TryParse(cmds, NumberStyles.Integer,
				NumberFormatInfo.InvariantInfo, out cmdsAsInt))
			{
				cmdsAsInt = (int)SyncCommands.All;
			}
			update.EnabledCommands = (SyncCommands)cmdsAsInt;

			string syncOption = host.GetConfig(GdsDefs.ConfigAutoSync,
											AutoSyncMode.DISABLED.ToString());
			AutoSyncMode mode;
			if (!Enum.TryParse(syncOption, out mode))
			{
				// Support obsolete Sync on Save confg.
				if (host.GetConfig(GdsDefs.ConfigAutoSync, false))
				{
					mode = AutoSyncMode.SAVE;
				}
				else
				{
					mode = AutoSyncMode.DISABLED;
				}
			}
			update.AutoSync = mode;

			update.Folder = host.GetConfig(GdsDefs.ConfigDefaultAppFolder,
												string.Empty);
			string encodedColor = host.GetConfig(
				GdsDefs.ConfigDefaultAppFolderColor);
			update.FolderColor = !string.IsNullOrEmpty(encodedColor) ?
				GoogleColor.DeserializeFromString(encodedColor) : null;

			update.DriveScope = host.GetConfig(GdsDefs.ConfigDriveScope,
									DriveService.Scope.Drive);

			// Default is no OAuth 2.0 credentials.
			update.ClientId = host.GetConfig(GdsDefs.ConfigDefaultClientId,
												string.Empty);
			string secretVal = host.GetConfig(GdsDefs.ConfigDefaultClientSecret,
												string.Empty);
			update.ClientSecret = new ProtectedString(true, secretVal);

			update.m_isDirty = false;
			Default = update;
		}
	}
}
