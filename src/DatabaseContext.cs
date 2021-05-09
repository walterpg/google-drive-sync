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
using KeePassLib;
using KeePassLib.Serialization;

namespace KPSyncForDrive
{
    public class DatabaseContext
    {
        PwDatabase m_db;
        Guid m_uuid;
        IOConnectionInfo m_pathEtc;

        internal DatabaseContext(PwDatabase db)
        {
            m_db = db;
            m_uuid = db.GetUuid();
            m_pathEtc = db.IOConnectionInfo.CloneDeep();
            LoadedConfig = null;
        }

        public PwDatabase Database
        {
            get
            {
                return m_db;
            }
        }

        public IOConnectionInfo PathEtc
        {
            get
            {
                return m_pathEtc;
            }
        }

        public Guid Uuid
        {
            get
            {
                return m_uuid;
            }
        }

        public EntryConfiguration LoadedConfig
        {
            get; set;
        }
    }
}
