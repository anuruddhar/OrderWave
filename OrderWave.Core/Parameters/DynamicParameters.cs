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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
#endregion


namespace OrderWave.Core.Parameters {
    public class DynamicParameters {
        public DynamicParameters() {
            Dictionary = new Dictionary<string, SqlParameter>();
            Parameters = new List<SqlParameter>();
        }

        public List<SqlParameter> Parameters { get; set; }
        public Dictionary<string, SqlParameter> Dictionary { get; set; }

        public T Get<T>(string name) {
            var firstOrDefault = Parameters.FirstOrDefault(x => x.ParameterName == name);
            if (firstOrDefault != null && firstOrDefault.Value != System.DBNull.Value)
                return ( (T)firstOrDefault.Value != null ? (T)firstOrDefault.Value : default(T) );

            return default(T);
        }

        public List<SqlParameter> GetOutputParameters() {
            return
                Parameters.Where(
                    x => x.Direction == ParameterDirection.Output || x.Direction == ParameterDirection.InputOutput)
                    .ToList();
        }

        public void Add(string name, DbType dbType, ParameterDirection direction, object value) {
            var parameter = new SqlParameter {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                //Value = value == null ? 0 : value
                Value = value
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }

        public void Add(string name, SqlDbType dbType, ParameterDirection direction, object value) {
            var parameter = new SqlParameter {
                SqlDbType = dbType,
                ParameterName = name,
                Direction = direction,
                //Value = value == null ? 0 : value
                Value = value
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }

        public void Add(string name, DbType dbType, ParameterDirection direction, int size, object value) {
            var parameter = new SqlParameter {
                DbType = dbType,
                ParameterName = name,
                Direction = direction,
                //Value = value == null ? 0 : value
                Value = value,
                Size = size
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }


        public void Add(string name, SqlDbType dbType, ParameterDirection direction, int size, object value) {
            var parameter = new SqlParameter {
                SqlDbType = dbType,
                ParameterName = name,
                Direction = direction,
                //Value = value == null ? 0 : value
                Value = value,
                Size = size
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }


        public void Add(string name, DbType dbType, object value) {
            var parameter = new SqlParameter {
                DbType = dbType,
                ParameterName = name,
                Direction = ParameterDirection.Input,
                //Value = value == null ? 0 : value
                Value = value
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }
        // overLaoded method for Table value parameter pass 
        public void Add(string name, SqlDbType dbType, object value) {
            var parameter = new SqlParameter {
                SqlDbType = dbType,
                ParameterName = name,
                Direction = ParameterDirection.Input,
                Value = value
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }
        public void Add(string name, object value) {
            var parameter = new SqlParameter {
                ParameterName = name,
                Direction = ParameterDirection.Input,
                //Value = value == null ? 0 : value
                Value = value
            };

            Parameters.Add(parameter);
            Dictionary.Add(name, parameter);
        }
    }
}
