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
using OrderWave.Core.ApplicationCache;
using OrderWave.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#endregion

namespace OrderWave.Core.Parser {
    public static class QueryStringMapper<T> where T : class, new() {

        public static string ToQueryString(T obj) {
            StringBuilder result = new StringBuilder();
            var propsInT = GetObjProperties(typeof(T));
            foreach (var prop in propsInT) {
                string columnName = ((QueryStringAttribute)prop.GetCustomAttribute(typeof(QueryStringAttribute))).Name;
                result.Append($"{columnName}={prop.GetValue(obj)}&");
            }
            return result.ToString();
        }

        /// <summary>
        /// Create the Class property - QueryString attribute array for given type. 
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetObjProperties(Type _type) {
            string objKey = typeof(T).FullName;

            PropertyInfo[] listOfProperties;
            var cacheData = ApplicationStateManager.GetItemFromInMemoryCache<PropertyInfo[]>(objKey);

            if (cacheData == null) {
                listOfProperties = GetFilteredPropertiesByAttribute(_type);
                ApplicationStateManager.AddItemToInMemoryCache(listOfProperties, objKey);
            } else {
                listOfProperties = cacheData;
            }

            return listOfProperties;
        }

        /// <summary>
        /// Create the Class property - QueryString attribute array for given type. 
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetFilteredPropertiesByAttribute(Type _type) {
            PropertyInfo[] listOfProperties;
            List<PropertyInfo> filteredlistOfProperties = new List<PropertyInfo>();
            listOfProperties = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); //Get all Public n NonPublic for Instance(GET;SET;)

            foreach (var property in listOfProperties) {
                var attributeSet = property.GetCustomAttributes(false);
                var columnMap = attributeSet.FirstOrDefault(t => t.GetType() == typeof(QueryStringAttribute));
                if (columnMap != null) {
                    filteredlistOfProperties.Add(property);
                }
            }
            return filteredlistOfProperties.ToArray();
        }

    }
}
