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

namespace OrderWave.Core.Extensions {
    public static class StringExtensions {
        public static string ToCamelCase (this string s) {
            return s.Substring(0, 1).ToLower() + s.Substring(1);
        }

        public static string ToEmptyString (this string value) {
            if (string.IsNullOrEmpty(value)) {
                value = string.Empty;
                return value;
            }
            return value;
        }

    }
}
