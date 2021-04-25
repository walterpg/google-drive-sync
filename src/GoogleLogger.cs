using System;
using System.Threading;
using Serilog.Events;
using IGoogleLogger = Google.Apis.Logging.ILogger;

namespace KPSyncForDrive
{
    class GoogleLogger : IGoogleLogger
    {
        static int s_cApiLoggers = 0;

        internal static void Register()
        {
            try
            {
                GoogleLogger glogger = new GoogleLogger();
                Google.ApplicationContext.RegisterLogger(glogger);
                Log.Debug("Google logging registered.");
            }
            catch (InvalidOperationException e)
            {
                Log.Warning("Google API logging not available:",
                    e.Message);
            }
        }

        readonly int m_id;

        GoogleLogger()
        {
            m_id = Interlocked.Increment(ref s_cApiLoggers);
        }

        public bool IsDebugEnabled 
        {
            get
            {
                return Log.Default.IsEnabled(LogEventLevel.Debug);
            }
        }

        public void Debug(string message, params object[] formatArgs)
        {
            Log.Debug(PrepMsg(message, formatArgs), m_id);
        }

        public void Error(Exception exception, string message,
            params object[] formatArgs)
        {
            Log.Error(PrepMsg(message, formatArgs), m_id, exception);
        }

        public void Error(string message, params object[] formatArgs)
        {
            Log.Error(PrepMsg(message, formatArgs), m_id);
        }

        public IGoogleLogger ForType(Type type)
        {
            GoogleLogger retVal = new GoogleLogger();
            Log.Debug("GAPI{0}={1}.", retVal.m_id,
                type.FullName);
            return retVal;
        }

        public IGoogleLogger ForType<T>()
        {
            return ForType(typeof(T));
        }

        public void Info(string message, params object[] formatArgs)
        {
            Log.Info(PrepMsg(message, formatArgs), m_id);
        }

        public void Warning(string message, params object[] formatArgs)
        {
            Log.Warning(PrepMsg(message, formatArgs), m_id);
        }

        string PrepMsg(string message, object[] args)
        {
            return "GAPI{0}: " + 
                string.Format(message, args);
        }
    }
}
