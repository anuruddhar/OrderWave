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
using OrderWave.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
#endregion

namespace OrderWave.Core.Extensions {
    public static class DataTableExtensions {
        private static readonly Dictionary<Type, IList<PropertyInfo>> TypeDictionary =
            new Dictionary<Type, IList<PropertyInfo>>();

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new() {
            var item = new T();

            //var search = "searchingItem";            

            //This is for the search base classes helper

            foreach (var property in properties) {
                var att = property.GetCustomAttributes(true);
                ColumnAttribute p = null;

                if (att.Length > 0) {
                    p = att[0] as ColumnAttribute;
                    if (p != null) {
                        if (( row.Table.Columns.Contains(p.FieldName) ) && ( row[p.FieldName] != DBNull.Value )) {
                            property.SetValue(item, row[p.FieldName], null);
                        }
                    }
                }
            }
            return item;
        }

        public static IList<PropertyInfo> GetPropertiesForType<T>() {
            var type = typeof(T);
            if (!TypeDictionary.ContainsKey(typeof(T))) {
                TypeDictionary.Add(type, type.GetProperties().ToList());
            }
            return TypeDictionary[type];
        }

        public static IList<PropertyInfo> GetPropertiesForType<T>(Type modeltype) {

            if (!TypeDictionary.ContainsKey(modeltype)) {
                TypeDictionary.Add(modeltype, modeltype.GetProperties().ToList());
            }
            return TypeDictionary[modeltype];
        }

        private static string GetSearchItems() {
            return string.Empty;
        }

        public static bool IsValid(this DataSet ds, bool checkFirstRowCount = true) {
            // return (ds != null && ds.Tables.Count > 0 && (checkFirstRowCount == false || ds.Tables[0].Rows.Count > 0));
            return ( ds != null && ds.Tables.Count > 0 );
        }

        public static IList<T> ToList<T>(this DataTable table) where T : new() {
            var properties = GetPropertiesForType<T>();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows) {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        public static T ToItem<T>(this DataRow dataRow) where T : new() {
            var properties = GetPropertiesForType<T>();
            T result = new T();
            result = CreateItemFromRow<T>(dataRow, properties);
            return result;
        }

        public static IList<T> ToList<T>(this IDataReader reader) where T : new() {
            var properties = GetPropertiesForType<T>();
            IList<T> result = new List<T>();

            while (reader.Read()) {
                var item = CreateItemFromReaderRow<T>(reader, properties);
                result.Add(item);
            }

            // Reader we will close in service layer as we can read data more than one time from Reader.
            //reader.Close();

            //foreach (var row in table.Rows)
            //{
            //    var item = CreateItemFromRow<T>((DataRow)row, properties);
            //    result.Add(item);
            //}

            return result;
        }

        private static T CreateItemFromReaderRow<T>(IDataReader reader, IList<PropertyInfo> properties) where T : new() {
            var item = new T();

            var readerColumns = reader.GetSchemaTable()
                .Rows.Cast<DataRow>()
                .Select(row => row["ColumnName"].ToString().ToUpper() as string)
                .ToList();


            foreach (var property in properties) {
                var att = property.GetCustomAttributes(true);
                System.Diagnostics.Debug.WriteLine(property.Name);
                ColumnAttribute p = null;

                if (att.Length > 0) {
                    p = att[0] as ColumnAttribute;
                    if (p != null) {

                        //Need this to ensure that when the column is not returned from DB
                        if (readerColumns.Contains(p.FieldName.ToUpper())) {

                            if (reader[p.FieldName] != DBNull.Value) {
                                property.SetValue(item, reader[p.FieldName], null);
                            }
                        } //Manjuke - Ludmal to Review

                    }
                }
            }
            return item;
        }

        /// <summary>
        /// Convert a List<T> into DataTable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iList"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> iList) {
            DataTable dataTable = new DataTable();
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
            var columnName = "";
            for (int i = 0; i < propertyDescriptorCollection.Count; i++) {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);
                columnName = "";
                foreach (var attribute in propertyDescriptor.Attributes) {
                    if (typeof(ColumnAttribute).IsEquivalentTo(attribute.GetType())) {
                        var att = (ColumnAttribute)attribute;
                        columnName = att.FieldName;
                    }
                }
                dataTable.Columns.Add(string.IsNullOrEmpty(columnName) ? propertyDescriptor.Name : columnName, type);
            }

            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList) {
                for (int i = 0; i < values.Length; i++) {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}
