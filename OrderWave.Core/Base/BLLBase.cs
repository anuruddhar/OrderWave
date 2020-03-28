﻿#region Modification Log
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

namespace OrderWave.Core.Base {
    public class BLLBase<T>  where T : new() {
        private T _Data = default(T);
        public T Data {
            get {
                if (_Data == null) {
                    _Data = new T();
                }
                return _Data;
            }
            set { _Data = value; }
        }
    }
}