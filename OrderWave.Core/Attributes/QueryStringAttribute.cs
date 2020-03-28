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
    public class QueryStringAttribute : Attribute {
        public string Name { get; set; } = string.Empty;

        public QueryStringAttribute(string _name) {
            this.Name = _name;
        }

    }
}
