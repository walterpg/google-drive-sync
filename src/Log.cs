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

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using KeePassLib;
using Microsoft.Win32;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace KPSyncForDrive
{
    static class Log
    {
        static Logger s_logger = null;

        public static ILogger Default
        {
            get
            {
                return s_logger == null ? Logger.None : s_logger;
            }
        }

        public static void Configure()
        {
            // temprorary; replace with UI model.
            NameValueCollection appSettings
                = ConfigurationManager.AppSettings;
            string strLevel = appSettings["KpSyncLogLevel"];
            if (string.IsNullOrEmpty(strLevel))
            {
                strLevel = Enum.GetName(typeof(LogEventLevel),
                    LogEventLevel.Debug);
            }
            LogEventLevel level;
            if (!Enum.TryParse(strLevel, out level))
            {
                uint intLevel;
                if (uint.TryParse(strLevel, out intLevel))
                {
                    intLevel = Math.Min(intLevel, (uint)LogEventLevel.Fatal);
                    level = (LogEventLevel)intLevel;
                }
                else
                {
                    level = LogEventLevel.Debug;
                }
            }
            string strFilePath = appSettings["KpSyncLogFile"];
            if (string.IsNullOrEmpty(strFilePath) ||
                Path.GetInvalidPathChars()
                    .Any(c => strFilePath.Contains(c)) ||
                !Path.IsPathRooted(strFilePath))
            {
                return;
            }

            try
            {
                LoggerConfiguration logConfig = new LoggerConfiguration();
                logConfig = logConfig.WriteTo.File(strFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 20);
                s_logger = logConfig.MinimumLevel.Is(level)
                    .CreateLogger();
                GoogleLogger.Register();

                Assembly asm = Assembly.GetExecutingAssembly();
                Info("{0} v{1} loaded by {2} v{3}.",
                    GdsDefs.ProductName, asm.GetName().Version,
                    PwDefs.ShortProductName, PwDefs.VersionString);
                TargetFrameworkAttribute attr
                    = asm.GetCustomAttribute<TargetFrameworkAttribute>();
                string tgtFw = attr == null ? "(unknown)" :
                    attr.FrameworkDisplayName;
                Info("Target={0}, Current CLR=v{1}",
                    tgtFw, Environment.Version);
                string releaseId = TryGetRegValueAsString(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ReleaseId", "<unknown>");
                string edition = TryGetRegValueAsString(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ProductName", "Windows ??");
                Info("OS Product: {0}, {1}", edition, releaseId);
                OperatingSystem os = Environment.OSVersion;
                Info("Platform ID: {0:G}, {1}, SP('{2}')",
                    os.Platform, os.VersionString, os.ServicePack);
                Info("Installed .NET Framework: {0}", GetNetFrameworkVer());
            }
            catch (Exception e)
            {
                Error(e, "Logging setup exception.");
            }
        }

        static string Get45orLaterVersion(int releaseNum)
        {
            if (releaseNum >= 528040)
                return "4.8 or later";
            if (releaseNum >= 461808)
                return "4.7.2";
            if (releaseNum >= 461308)
                return "4.7.1";
            if (releaseNum >= 460798)
                return "4.7";
            if (releaseNum >= 394802)
                return "4.6.2";
            if (releaseNum >= 394254)
                return "4.6.1";
            if (releaseNum >= 393295)
                return "4.6";
            if (releaseNum >= 379893)
                return "4.5.2";
            if (releaseNum >= 378675)
                return "4.5.1";
            if (releaseNum >= 378389)
                return "4.5";
            return string.Format("(Unknown release number {0})", releaseNum);
        }

        static string GetNetFrameworkVer()
        {
            const string subkey
                = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            try
            {
                using (var ndpKey = RegistryKey.OpenBaseKey(
                    RegistryHive.LocalMachine, RegistryView.Registry32)
                    .OpenSubKey(subkey))
                {
                    object keyVal = null;
                    if (ndpKey != null && null != (keyVal = ndpKey.GetValue("Release")) &&
                        keyVal is int)
                    {
                        return Get45orLaterVersion((int)keyVal);
                    }
                }
            }
            catch (SystemException e)
            {
                Error(e, "Retrieving .NET Framework v4.5 or later info.");
            }
            return "(Release info not found)";
        }

        static string TryGetRegValueAsString(string key, string value, string defaultVal)
        {
            try
            {
                return Registry.GetValue(key, value, defaultVal).ToString();
            }
            catch (SystemException e)
            {
                Error(e, @"Retrieving '{0}\{1}' registry value.",
                    key, value);
                return defaultVal;
            }
        }

        public static void Shutdown()
        {
            using (s_logger)
            {
                s_logger = null;
            }
        }

        public static void Diag(string format, params object[] args)
        {
            Default.Verbose(format, args);
        }

        public static void Debug(string format, params object[] args)
        {
            Default.Debug(format, args);
        }

        public static void Debug(Exception e, string format, params object[] args)
        {
            Default.Debug(e, format, args);
        }

        public static void Warning(string format, params object[] args)
        {
            Default.Warning(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            Default.Error(format, args);
        }

        public static void Error(Exception e, string format,
            params object[] args)
        {
            Default.Error(e, format, args);
        }

        public static void Info(string format, params object[] args)
        {
            Default.Information(format, args);
        }
    }
}
