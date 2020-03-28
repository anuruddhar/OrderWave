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
using OrderWave.Core.Interfaces;
using System;
#endregion

namespace OrderWave.Core.Base {
    public abstract class SearchBase : IQueryStringConverter {


        [QueryString("UserId")]
        public string UserId { get; set; } = string.Empty;

        [QueryString("PageNo")]
        public int PageNo { get; set; } = 1;

        [QueryString("ItemsPerPage")]
        public int ItemsPerPage { get; set; } = 20;

        [QueryString("SearchString")]
        public string SearchString { get; set; } = string.Empty;

        [QueryString("FromDate")]
        public DateTime FromDate { get; set; } = DateTime.MinValue;

        [QueryString("ToDate")]
        public DateTime ToDate { get; set; } = DateTime.MinValue;

        public virtual string ToQueryString() {
            return $"UserId={UserId}&PageNo={PageNo}&ItemsPerPage={ItemsPerPage}&SearchString={SearchString}&FromDate={FromDate}&ToDate={ToDate}";
        }

        //public string ToQueryString() {
        //    return QueryStringMapper<SearchBase>.ToQueryString(this);
        //}
    }
}
