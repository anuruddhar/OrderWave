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


using System.Data.SqlClient;

namespace OrderWave.Core.Base {
    public class DALBase  {

        public static string ConnectionString = string.Empty;

        public static string WorkstationId = string.Empty;

        public static int ConnectionTimeOut = 30;

        private static string FullConnectionString {
            get {
                SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
                sqlConnectionStringBuilder.WorkstationID = WorkstationId;
                sqlConnectionStringBuilder.ConnectTimeout = ConnectionTimeOut;
                return sqlConnectionStringBuilder.ToString();
            }
        }

        private DataBase _Database = null;
        public DataBase Database {
            get {
                return _Database;
            }
        }

        public DALBase() {
            if (_Database == null) {
                _Database = new DataBase(FullConnectionString);
            }
        }



    }
}
