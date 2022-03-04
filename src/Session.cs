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
using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Security;

namespace KPSyncForDrive
{
    static class Session
    {
        static readonly Dictionary<Guid, ProtectedString> s_sessionAuthTokens
            = new Dictionary<Guid, ProtectedString>();

        public static void ClearSessionTokens()
        {
            Log.Diag("Clearing all {0} session tokens.",
                s_sessionAuthTokens.Count);
            s_sessionAuthTokens.Clear();
        }

        public static bool TryGetSessionToken(this PwDatabase db,
            out ProtectedString authToken)
        {
            return s_sessionAuthTokens.TryGetValue(db.GetKpSyncUuid(),
                out authToken);
        }

        public static void MarkDirty(this PwDatabase db)
        {
            db.Modified = db.Modified || true;
            db.SettingsChanged = DateTime.UtcNow;
        }

        public static bool RemoveSessionToken(this PwDatabase db)
        {
            Log.Diag("Removing session token for {0}, GUID={1}.",
                db.GetDisplayNameAndPath(), db.GetKpSyncUuid());
            return s_sessionAuthTokens.Remove(db.GetKpSyncUuid());
        }

        public static void SetSessionToken(this PwDatabase db,
            ProtectedString authToken)
        {
            Log.Diag("New session token for {0}, GUID={1}.",
                db.GetDisplayNameAndPath(), db.GetKpSyncUuid());
            s_sessionAuthTokens[db.GetKpSyncUuid()] = authToken;
        }

    }
}
