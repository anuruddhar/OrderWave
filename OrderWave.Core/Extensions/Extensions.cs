
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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
#endregion	  


namespace OrderWave.Core.Extensions {
    public static class Extensions {

        //public static T Clone<T>(T obj) where T : class {
        //    var serializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null);
        //    using (var ms = new System.IO.MemoryStream()) {
        //        serializer.WriteObject(ms, obj);
        //        ms.Position = 0;
        //        return (T)serializer.ReadObject(ms);
        //    }
        //}

        public static string ToStringValue (this Enum value) {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static bool HasColumn (this IDataRecord dr, string columnName) {
            for (int i = 0; i < dr.FieldCount; i++) {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        #region DateTime

        public static bool IsValid (this DateTime date) {
            return date > DateTime.MinValue;
        }

        public static DateTime ToDefaultDateFormat (this DateTime dt) {
            //if (dt == null || dt == DateTime.MinValue)
            //{
            //    return DateTime.Now();
            //}

            IFormatProvider culture = new CultureInfo("en-GB", true);
            return Convert.ToDateTime(dt.Day + "/" +
                                                    dt.Month + "/" +
                                                    dt.Year, culture);


            //return dt.ToString(System.Configuration.ConfigurationManager.AppSettings["DATE_FORMAT"]);
            //return dt.ToLongDateString();
        }

        public static DateTime ToDefaultDate (this DateTime dt) {
            if (dt == null || dt == DateTime.MinValue) {
                dt = DateTime.UtcNow;
            }
            return dt;
        }

        #endregion


        public static decimal GetMedian(this IEnumerable<int> source) {
            // Create a copy of the input, and sort the copy
            int[] temp = source.ToArray();
            Array.Sort(temp);

            int count = temp.Length;
            if (count == 0) {
                //throw new InvalidOperationException("Empty collection");
                return 0;
            } else if (count % 2 == 0) {
                // count is even, average two middle elements
                int a = temp[count / 2 - 1];
                int b = temp[count / 2];
                return ( a + b ) / 2m;
            } else {
                // count is odd, return the middle element
                return temp[count / 2];
            }
        }


        #region List Extensions
        public static bool IsEmpty<T>(this IEnumerable<T> list) {
            if (list == null) {
                return true;
            }
            if (list is ICollection<T>) {
                return ( (ICollection<T>)list ).Count == 0;
            }
            return !list.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> list) {
            if (list == null) {
                return false;
            }
            if (list is ICollection<T>) {
                if (( (ICollection<T>)list ).Count > 0) {
                    return true;
                }
            }

            return false;

        }
        #endregion


    }
}
