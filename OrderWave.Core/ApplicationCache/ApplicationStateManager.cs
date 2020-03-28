#region Modification Log
/*------------------------------------------------------------------------------------------------------------------------------------------------- 
   System      -   OrderWave
   Module      -   Core

Modification History:
==================================================================================================================================================
Date              Version      		Modify by              					Description
--------------------------------------------------------------------------------------------------------------------------------------------------
31/05/2015         	  1.0           Anuruddha   					        Initial Version
--------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion

#region Namespace
using System;
using System.Runtime.Caching;
#endregion	 

namespace OrderWave.Core.ApplicationCache {
    public static class ApplicationStateManager {

        private static int _daysToKeepInCache = 1;




        /// <summary>
        /// Add object to In-Memory cache
        /// </summary>
        /// <param name="objectTOCache"></param>
        /// <param name="key"></param>
        public static void AddItemToInMemoryCache(object objectTOCache, string key) {
            try {
                ObjectCache _cacheReference = MemoryCache.Default;
                if (_cacheReference.Contains(key)) {
                    RemoveItemFromInMemoryCache(key);
                }
                _cacheReference.Add(key, objectTOCache, DateTime.Now.AddDays(_daysToKeepInCache));
            } catch {
                throw;
            }
        }




        /// <summary>
        /// Get object from In-Memory cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetItemFromInMemoryCache<T>(string key) where T : class {
            try {
                ObjectCache _cacheReference = MemoryCache.Default;
                return (T)_cacheReference[key];
            } catch {
                // handle exception
                return null;
            }
        }




        /// <summary>
        /// Remove an object from the In-Memory cache for the corresponding key
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveItemFromInMemoryCache(string key) {
            try {
                ObjectCache _cacheReference = MemoryCache.Default;
                _cacheReference.Remove(key);
            } catch {
                throw;
            }
        }

    }
}
