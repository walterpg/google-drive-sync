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

using KeePassLib.Security;
using System;
using System.Reflection;

namespace GoogleDriveSync
{
	static partial class GdsDefs
	{
		static string s_productName;
		static string s_productVersion;
		static ProtectedString s_emptyEx;
		static ProtectedString s_legacyClientId;
		static ProtectedString s_legacyClientSecret;
		static ProtectedString s_currentClientId;
		static ProtectedString s_currentClientSecret;

		static ProtectedString GetData(byte[] data, byte[] pad)
		{
			XorredBuffer buf = null;
			try
			{
				buf = new XorredBuffer(data, pad);
				return new ProtectedString(true, buf);
			}
			finally
			{
				// $$REVIEW
				// XorredBuffer isn't IDisposable until sometime after KeePass
				// v2.35, so fool the compiler and dispose if necessary.
				object unknown = buf;
				using (IDisposable disposable = unknown as IDisposable) { }
			}
		}

		public static string ProductName
		{
			get
			{
				if (s_productName == null)
				{
					Assembly assembly = Assembly.GetExecutingAssembly();
					object[] attrs = assembly.GetCustomAttributes(
						typeof(AssemblyTitleAttribute), false);
					AssemblyTitleAttribute assemblyTitle;
					assemblyTitle = attrs[0] as AssemblyTitleAttribute;
					s_productName = assemblyTitle.Title;
				}
				return s_productName;
			}
		}

		public static string Version
		{
			get
			{
				if (s_productVersion == null)
				{
					Assembly assembly = Assembly.GetExecutingAssembly();
					object[] attrs = assembly.GetCustomAttributes(
						typeof(AssemblyInformationalVersionAttribute), false);
					AssemblyInformationalVersionAttribute asmVer;
					asmVer = attrs[0] as AssemblyInformationalVersionAttribute;
					s_productVersion = asmVer.InformationalVersion;
				}
				return s_productVersion;
			}
		}

		// $$REVIEW
		// ProtectedString.EmptyEx is not available until after the current target
		// release (2.35).
		public static ProtectedString PsEmptyEx
		{
			get
			{
				if (s_emptyEx == null)
				{
					s_emptyEx = new ProtectedString(true, new byte[0]);
				}
				return s_emptyEx;
			}
		}

		public static string UpdateUrl
		{
			get
			{
				return string.Format(UrlUpdateFormat, GitHubProjectName);
			}
		}

		public static ProtectedString ClientSecret
		{
			get
			{
				if (s_currentClientSecret == null)
				{
					s_currentClientSecret = GetData(
						s_clientSecretBytes,
						s_clientSecretPad);
				}
				return s_currentClientSecret;
			}
		}

		public static ProtectedString ClientId
		{
			get
			{
				if (s_currentClientId == null)
				{
					s_currentClientId = GetData(
						s_clientIdBytes,
						s_clientIdPad);
				}
				return s_currentClientId;
			}
		}

		public static ProtectedString LegacyClientSecret
		{
			get
			{
				if (s_legacyClientSecret == null)
				{
					s_legacyClientSecret = GetData(
						s_legacyClientSecretBytes,
						s_legacyClientSecretPad);
				}
				return s_legacyClientSecret;
			}
		}

		public static ProtectedString LegacyClientId
		{
			get
			{
				if (s_legacyClientId == null)
				{
					s_legacyClientId = GetData(
						s_legacyClientIdBytes,
						s_legacyClientIdPad);
				}
				return s_legacyClientId;
			}
		}

		public const string GitHubProjectName = "google-drive-sync";
		public const string ConfigAutoSync = "GoogleSync.AutoSync";
		public const string ConfigEnabledCmds = "GoogleSync.EnabledCmds";
		public const string ConfigUUID = "GoogleSync.AccountUUID";
		public const string ConfigDefaultAppFolder = "GoogleSync.DefaultAppFolder";
		public const string ConfigDefaultAppFolderColor = "GoogleSync.DefaultAppFolderColor";
		public const string ConfigDriveScope = "GoogleSync.DriveApiScope";
		public const string ConfigDefaultClientId = "GoogleSync.DefaultClientId";
		public const string ConfigDefaultClientSecret = "GoogleSync.DefaultClientSecret";
		public const string ConfigUseLegacyCreds = "GoogleSync.UseLegacyAppCreds";
		public const string ConfigVersion = "GoogleSync.ConfigVersion";
		public const string ConfigTrue = "TRUE";
		public const string ConfigFalse = "FALSE";
		public const string EntryClientId = "GoogleSync.ClientID";
		public const string EntryClientSecret = "GoogleSync.ClientSecret";
		public const string EntryRefreshToken = "GoogleSync.RefreshToken";
		public const string EntryActiveAccount = "GoogleSync.ActiveAccount";
		public const string EntryActiveAccountTrue = EntryActiveAccount + "." + ConfigTrue;
		public const string EntryActiveAccountFalse = EntryActiveAccount + "." + ConfigFalse;
		public const string EntryActiveAppFolder = "GoogleSync.ActiveAppFolder";
		public const string EntryDriveScope = ConfigDriveScope;
		public const string EntryUseLegacyCreds = ConfigUseLegacyCreds;
		public const string EntryVersion = ConfigVersion;
		public const string AccountSearchString = "accounts.google.com";
		public const string UrlDomainRoot = "kpsync.org";
		public const string UrlDomain = "www." + UrlDomainRoot;
		public const string UrlHome = "https://" + UrlDomain;
		public const string UrlLegacyHome = "https://sourceforge.net/p/kp-googlesync";
		public const string UrlHelp = UrlHome;
		public const string UrlGoogleDev = "https://console.developers.google.com/start";
		public const string UrlUpdateFormat = "https://raw.githubusercontent.com/walterpg/"+ GitHubProjectName + "/master/kpsync_version_4.txt";
		public const string UrlSignInHelp = "https://developers.google.com/identity/sign-in/web/troubleshooting";
		public const string UrlUpgradeV1 = UrlHome + "/install/upgrade1";
		public const string GsyncBackupExt = ".gsyncbak";
		public const string AppDefaultFolderName = "KeePass Google Sync";
		public const string AppFolderColor = "#4986e7"; // "Rainy Sky"
		public const string FolderMimeType = "application/vnd.google-apps.folder";
		public const int DefaultDotNetFileBufferSize = 4096;
	}

}
