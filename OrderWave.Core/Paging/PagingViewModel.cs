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
using System.Collections.Generic;
#endregion

namespace OrderWave.Core.Paging {
    public class PagingViewModel<T> {
        public int TotalRows { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> RowData { get; set; }
    }
}
