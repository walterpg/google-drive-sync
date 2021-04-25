using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using Microsoft.Win32;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace KPSyncForDrive
{
    static class Log
    {
        static ILogger s_logger = Logger.None;

        public static ILogger Default
        {
            get
            {
                return s_logger;
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
                OperatingSystem os = Environment.OSVersion;
                Info("Platform ID: {0:G}, {1}, SP('{2}')",
                    os.Platform, os.VersionString, os.ServicePack);
                string releaseId = Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion",
                    "ReleaseId", "").ToString();

            }
            catch (Exception e)
            {
                Error(e, "Logging setup exception.");
            }
        }

        public static void Shutdown()
        {
            // Hmm...serilog...
            ILogger other = Serilog.Log.Logger;
            try
            {
                Serilog.Log.Logger = s_logger;
                s_logger = Logger.None;
                Serilog.Log.CloseAndFlush();
            }
            finally
            {
                Serilog.Log.Logger = other;
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
