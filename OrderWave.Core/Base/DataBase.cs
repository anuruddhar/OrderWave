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
using OrderWave.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace OrderWave.Core.Base {
    public static class DataBase {

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


        // public string ConnectionString { get; set; }
        public static SqlTransaction Transaction { get; set; }

        private static int iCommandTimeout = 60;
        public static int CommandTimeout {
            get {
                return Convert.ToInt32(ConfigurationManager.AppSettings.AllKeys.Contains("ConnectionTimeOut") ? Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOut"]) : iCommandTimeout);
            }
            set {
                iCommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings.AllKeys.Contains("ConnectionTimeOut") ? Convert.ToInt32(ConfigurationManager.AppSettings["ConnectionTimeOut"]) : iCommandTimeout);
            }
        }


        #region ExecuteDataSet

        public static async Task<DataSet> ExecuteDataSetAsync(string storedProcedure,
                                                        List<SqlParameter> parameters,
                                                        SqlTransaction transaction = null,
                                                        int commandTimeout = 60) {
            var con = GetConnection();
            try {
                using (var command = new SqlCommand(storedProcedure, con)) {

                    if (transaction != null) {
                        command.Transaction = transaction;
                    }

                    foreach (var sqlParameter in parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeout;
                    var da = new SqlDataAdapter(command);

                    con.Open();
                    var ds = new DataSet();
                    //da.Fill(ds);
                    await Task.Run(() => da.Fill(ds));
                    //await Task.FromResult(da.Fill(ds));
                    return ds;
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        public static async Task<DataSet> ExecuteDataSetAsync(string storedProcedure,
                                                        DynamicParameters parameters,
                                                        SqlTransaction transaction = null,
                                                        int commandTimeout = 60) {
            var con = GetConnection();
            try {
                using (var command = new SqlCommand(storedProcedure, con)) {

                    if (transaction != null) {
                        command.Transaction = transaction;
                    }

                    foreach (var sqlParameter in parameters.Parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeout;

                    var da = new SqlDataAdapter(command);

                    con.Open();
                    var ds = new DataSet();
                    //da.Fill(ds);
                    await Task.Run(() => da.Fill(ds));
                    //await Task.FromResult(da.Fill(ds));

                    if (parameters != null) {
                        var outputParameters = parameters.GetOutputParameters();
                        foreach (var outputParameter in outputParameters) {
                            parameters.Dictionary[outputParameter.ParameterName].Value =
                                command.Parameters[outputParameter.ParameterName].Value;
                        }
                    }
                    return ds;

                }

            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        #endregion

        #region ExecuteNonQuery

        public static async Task<Tuple<T, Dictionary<string, object>>> ExecuteNonQueryAsync<T>(string storedProcedure,
                                    List<SqlParameter> parameters,
                                    SqlTransaction transaction = null,
                                    SqlConnection connection = null,
                                    int commandTimeout = 60) {
            return await ExecuteNonQueryAsync<T>(storedProcedure, parameters, null, transaction, connection, commandTimeout);
        }

        public static async Task<T> ExecuteNonQueryAsync<T>(string storedProcedure,
                                                    DynamicParameters parameters = null,
                                                    SqlTransaction transaction = null,
                                                    int commandTimeout = 60) {
            var con = GetConnection();
            var command = con.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedure;
            command.CommandTimeout = commandTimeout;

            if (transaction != null)
                command.Transaction = transaction;

            if (parameters != null) {
                foreach (var sqlParameter in parameters.Parameters) {
                    command.Parameters.Add(sqlParameter);
                }
            }
            con.Open();

            try {
                object retval = await command.ExecuteNonQueryAsync();

                if (parameters != null) {
                    var outputParameters = parameters.GetOutputParameters();
                    foreach (var outputParameter in outputParameters) {
                        parameters.Dictionary[outputParameter.ParameterName].Value =
                            command.Parameters[outputParameter.ParameterName].Value;
                    }
                }

                command.Dispose();
                command = null;
                return (T)retval;
            } catch (Exception ex) {
                con.Close();
                con = null;
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }


        public static async Task<Tuple<T, Dictionary<string, object>>> ExecuteNonQueryAsync<T>(string storedProcedure,
                            List<SqlParameter> parameters,
                            List<string> outputParameterNames = null,
                            SqlTransaction transaction = null, 
                            SqlConnection connection = null,
                            int commandTimeout = 60) {
            Dictionary<string, object> outputParameters;
            var con = connection ?? GetConnection();
            try {
                using (var command = new SqlCommand(storedProcedure, con)) {
                    if (transaction != null) {
                        command.Transaction = transaction;
                    }

                    foreach (var sqlParameter in parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeout;
                    // Execute the command & return the results
                    if (connection == null) {
                        con.Open();
                    }
                    object retval = await command.ExecuteNonQueryAsync();

                    if (outputParameterNames != null && outputParameterNames.Any()) {
                        outputParameters = outputParameterNames.ToDictionary(outputParameter => outputParameter,
                            outputParameter => command.Parameters[outputParameter].Value);
                    } else {
                        outputParameters = new Dictionary<string, object>();
                    }
                    return new Tuple<T, Dictionary<string, object>>((T)retval, outputParameters);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        /*
        public async Task<Tuple<T, Dictionary<string, object>>> ExecuteNonQueryAsync<T>(string storedProcedure,
                                    DynamicParameters parameters,
                                    List<string> outputParameterNames = null,
                                    SqlTransaction transaction = null, 
                                    SqlConnection connection = null,
                                    int commandTimeout = 60) {
            Dictionary<string, object> outputParameters;
            var con = connection ?? GetConnection();
            try {
                using (var command = new SqlCommand(storedProcedure, con)) {
                    if (transaction != null) {
                        command.Transaction = transaction;
                    }

                    foreach (var sqlParameter in parameters.Parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = commandTimeout;
                    // Execute the command & return the results
                    if (connection == null) {
                        con.Open();
                    }
                    object retval = await command.ExecuteNonQueryAsync();

                    if (outputParameterNames != null && outputParameterNames.Any()) {
                        outputParameters = outputParameterNames.ToDictionary(outputParameter => outputParameter,
                            outputParameter => command.Parameters[outputParameter].Value);
                    } else {
                        outputParameters = new Dictionary<string, object>();
                    }
                    return new Tuple<T, Dictionary<string, object>>((T)retval, outputParameters);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }
        */
        #endregion

        #region ExecuteScalar


        public static async Task<Tuple<T, Dictionary<string, object>>> ExecuteScalarAsync<T>(string storedProcedure,
                                                                                      List<SqlParameter> parameters,
                                                                                      SqlTransaction transaction = null,
                                                                                      SqlConnection connection = null) {
            return await ExecuteScalarAsync<T>(storedProcedure, parameters, null, transaction, connection);
        }

        public static async Task<Tuple<T, Dictionary<string, object>>> ExecuteScalarAsync<T>(string storedProcedure,
                                                                                 List<SqlParameter> parameters,
                                                                                 List<string> outputParameterNames = null,
                                                                                 SqlTransaction transaction = null,
                                                                                 SqlConnection connection = null) {

            var con = connection ?? GetConnection();
            Dictionary<string, object> outputParameters;

            try {
                using (var command = new SqlCommand(storedProcedure, con)) {

                    if (transaction != null) {
                        command.Transaction = transaction;
                    }


                    foreach (var sqlParameter in parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    // Execute the command & return the results
                    if (connection == null) {
                        con.Open();
                    }
                    var retval = await command.ExecuteScalarAsync();

                    if (outputParameterNames != null && outputParameterNames.Any()) {
                        outputParameters = outputParameterNames.ToDictionary(outputParameter => outputParameter,
                            outputParameter => command.Parameters[outputParameter].Value);
                    } else {
                        outputParameters = new Dictionary<string, object>();
                    }
                    return new Tuple<T, Dictionary<string, object>>((T)retval, outputParameters);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }


        public static async Task<Tuple<T, Dictionary<string, object>>> ExecuteScalarAsync<T>(string storedProcedure,
                                                                                    DynamicParameters parameters,
                                                                                    List<string> outputParameterNames = null,
                                                                                    SqlTransaction transaction = null,
                                                                                    SqlConnection connection = null) {
            var con = connection ?? GetConnection();
            Dictionary<string, object> outputParameters;

            try {
                using (var command = new SqlCommand(storedProcedure, con)) {

                    if (transaction != null) {
                        command.Transaction = transaction;
                    }


                    foreach (var sqlParameter in parameters.Parameters) {
                        command.Parameters.Add(sqlParameter);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    // Execute the command & return the results
                    if (connection == null) {
                        con.Open();
                    }
                    var retval = await command.ExecuteScalarAsync();
                    if(retval == null) { retval = 0; }

                    if (outputParameterNames != null && outputParameterNames.Any()) {
                        outputParameters = outputParameterNames.ToDictionary(outputParameter => outputParameter, outputParameter => command.Parameters[outputParameter].Value);
                    } else {
                        outputParameters = new Dictionary<string, object>();
                    }
                    return new Tuple<T, Dictionary<string, object>>((T)retval, outputParameters);
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (transaction == null) {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        #endregion

        #region ExecuteReader

        public static async Task<IDataReader> ExecuteDataReaderAsync(string storedProcedure,
                                                                DynamicParameters parameters = null,
                                                                int commandTimeout = 60) {
            var connection = GetConnection();
            var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcedure;

            if (parameters != null) {
                foreach (var sqlParameter in parameters.Parameters) {
                    command.Parameters.Add(sqlParameter);
                }
            }
            connection.Open();
            try {
                command.CommandTimeout = commandTimeout;
                IDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                if (parameters != null) {
                    var outputParameters = parameters.GetOutputParameters();
                    foreach (var outputParameter in outputParameters) {
                        parameters.Dictionary[outputParameter.ParameterName].Value =
                            command.Parameters[outputParameter.ParameterName].Value;
                    }
                }

                command.Dispose();
                command = null;
                return reader;
            } catch (Exception ex) {
                connection.Close();
                connection = null;
                throw ex;
            }
        }

        #endregion

        /// <summary>
        ///     Return the object name with Schema.
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static string WithSchema(string objectName, string schemaname = "SSO") {
            return string.Format("[{0}].[{1}]", schemaname, objectName);
        }

        public static SqlConnection GetConnection() {
            return new SqlConnection(FullConnectionString);
        }
    }
}
