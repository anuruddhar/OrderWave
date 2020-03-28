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
namespace OrderWave.Core.Attributes {
    public sealed class JavascriptEnumAttribute : Attribute {
        public string[] Groups { get; set; }

        public JavascriptEnumAttribute (params string[] groups) {
            Groups = groups;
        }
    }
}
