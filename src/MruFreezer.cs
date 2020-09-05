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

using KeePass.Plugins;
using KeePass.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KeePassSyncForDrive
{
    class MruListAsList : List<KeyValuePair<string, object>> { }

    static class MruListExtensions
    {
        public static IEnumerable<KeyValuePair<string, object>>
            AsEnumerable(this MruList mrulist)
        {
            for (uint i = 0; i<mrulist.ItemCount; i++)
            {
                yield return mrulist.GetItem(i);
            }
        }
    }

    class MruFreezer : IDisposable
    {
        readonly MruListAsList m_list;
        IPluginHost m_host;

        public MruFreezer(IPluginHost host)
        {
            m_list = new MruListAsList();
            m_list.AddRange(host.MainWindow.FileMruList.AsEnumerable());
            m_host = host;
        }

        public void Cancel()
        {
            m_host = null;
        }

        public void Dispose()
        {
            if (m_host != null)
            {
                MruList hostList = m_host.MainWindow.FileMruList;
                hostList.Clear();

                // The list is actually a stack.
                foreach (KeyValuePair<string, object> item in 
                    m_list.Cast<KeyValuePair<string, object>>().Reverse())
                {
                    hostList.AddItem(item.Key, item.Value);
                }
                m_host = null;
            }
        }
    }
}
