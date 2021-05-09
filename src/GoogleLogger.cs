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
                return Log.Default != null &&
                    Log.Default.IsEnabled(LogEventLevel.Debug);
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
