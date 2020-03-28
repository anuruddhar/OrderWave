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
#endregion




namespace OrderWave.Core.Extensions {
    public static class NullExtensions {

        public static object ToNullDateTime(this DateTime value) {
            if (value == DateTime.MinValue) {
                return DBNull.Value;
            } else {
                return value;
            }
        }

        public static object ToNullInterger(this Int32 value) {
            if (value == int.MinValue || value == -1) {
                return DBNull.Value;
            } else {
                return value;
            }
        }

        public static object ToNullString(this String value) {
            if (string.IsNullOrEmpty(value) || value == "" || value == String.Empty || value == null || value == "-1" || value == "null") {
                return DBNull.Value;
            } else {
                return value;
            }
        }


        public static object ToNullBinary(this byte[] value) {
            if (value == null) {
                return DBNull.Value;
            } else {
                return value;
            }
        }

    }
}
