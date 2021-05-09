using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Util.Store;

namespace KPSyncForDrive
{
    /// <summary>
    /// Placeholder for a secure store.
    /// </summary>
    class DataStore : IDataStore
    {
        static IDataStore s_default = null;

        public static IDataStore Default 
        { 
            get 
            {
                if (s_default == null)
                {
                    s_default = new DataStore();
                }
                return s_default;
            }
        }

        ConcurrentDictionary<string, object> m_store;

        DataStore()
        {
            m_store = new ConcurrentDictionary<string, object>();
        }

        public async Task ClearAsync()
        {
            await Task.Run(() => m_store.Clear());
        }

        public async Task DeleteAsync<T>(string key)
        {
            object val;
            await Task.Run(() => m_store.TryRemove(key, out val));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run<T>(() =>
            {
                object val;
                if (m_store.TryGetValue(key, out val) &&
                    val is T)
                {
                    return (T)val;
                }
                return default(T);
            });
        }

        public async Task StoreAsync<T>(string key, T value)
        {
            await Task.Run(() =>
            {
                m_store.AddOrUpdate(key, value, (k, o) => o);
            });
        }
    }
}
