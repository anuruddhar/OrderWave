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
using OrderWave.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
#endregion

namespace OrderWave.Core.Parser {

    /// <summary>
    /// Contains the ORM Framework functions and Methods for mapping between business objects and DB schema
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ValueMapper<T> where T: new() {

        /// <summary>
        /// Convert a IDataReader to a generic object
        /// </summary>
        /// <param name="_reader"></param>
        /// <returns></returns>
        public static T DataReaderToGenericObj(IDataReader _reader) {
            T genObj = new T();
            var propsInT = GetObjProperties(typeof(T));
            if (_reader.Read()) {
                populateGenericObj(_reader, propsInT, genObj);
            }
            return genObj;
        }
        /// <summary>
        /// Convert a IDataReader to a generic list
        /// </summary>
        /// <param name="_reader"></param>
        /// <returns></returns>
        public static List<T> DataReaderToGenericList(IDataReader _reader) {
            List<T> listOfT = new List<T>();

            var propsInT = GetObjProperties(typeof(T));
            while (_reader.Read()) {
                T newT = new T();
                populateGenericObj(_reader, propsInT, newT);

                listOfT.Add(newT);
            }
            return listOfT;
        }

        /// <summary>
        /// Set Generic Object vaues
        /// </summary>
        /// <param name="_reader"></param>
        /// <param name="propsInT"></param>
        /// <param name="genObj"></param>
        private static void populateGenericObj(IDataReader _reader, PropertyInfo[] propsInT, Object genObj) {
            foreach (var prop in propsInT) {
                string columnName = ( (ColumnAttribute)prop.GetCustomAttribute(typeof(ColumnAttribute)) ).DisplayName;
                if (_reader.HasColumn(columnName)) {
                    if (!DBNull.Value.Equals(_reader[columnName])) {
                        prop.SetValue(genObj, _reader[columnName]);
                    } else {
                        prop.SetValue(genObj, null);
                    }
                }
            }
        }

        /// <summary>
        /// Create the Class property - DBColumn attribute array for given type. 
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
        /// Create the Class property - DBColumn attribute array for given type. 
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetFilteredPropertiesByAttribute(Type _type) {
            PropertyInfo[] listOfProperties;
            List<PropertyInfo> filteredlistOfProperties = new List<PropertyInfo>();
            listOfProperties = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); //Get all Public n NonPublic for Instance(GET;SET;)

            foreach (var property in listOfProperties) {
                var attributeSet = property.GetCustomAttributes(false);
                var columnMap = attributeSet.FirstOrDefault(t => t.GetType() == typeof(ColumnAttribute));
                if (columnMap != null) {
                    filteredlistOfProperties.Add(property);
                }
            }
            return filteredlistOfProperties.ToArray();
        }

    }
}
