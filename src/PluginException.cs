/**
 * Google Sync Plugin for KeePass Password Safe
 * Copyright © 2012-2016  DesignsInnovate
 * Copyright © 2014-2016  Paul Voegler
 * 
 * KeePass Sync for Google Drive
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

namespace KeePassSyncForDrive
{
    class PluginException : Exception
    {
        public PluginException(string message,
            Exception inner)
            : base(message, inner)
        {
        }

        public PluginException(string message)
            : this(message, null)
        {
        }
    }

    class PluginStatusException : PluginException
    {
        public PluginStatusException(string message)
            : base(message)
        {
        }
    }
}
