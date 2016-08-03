using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System.Data;
using System.Data.Common;

namespace SQLHelper
{
    public class OraHelper
    {

        public static string ConnectionString;

        static OraHelper()
        {
            //ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            ConnectionString = "";//SunEast.WMS.ServiceCore.AppCommon.Connstr;
        }
        #region "General data access methods"

        #region "ExecuteNonQuery"
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteNonQuery(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(TransactionManager transactionManager, string storedProcedureName, params object[] parameterValues)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteNonQuery(transactionManager.TransactionObject, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="command">The command wrapper.</param>
        public static void ExecuteNonQuery(DbCommand command)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            database.ExecuteNonQuery(command);

        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="command">The command wrapper.</param>
        public void ExecuteNonQuery(TransactionManager transactionManager, DbCommand command)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            database.ExecuteNonQuery(command, transactionManager.TransactionObject);
        }


        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteNonQuery(commandType, commandText);
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(TransactionManager transactionManager, CommandType commandType, string commandText)
        {
            Database database = transactionManager.Database;
            return database.ExecuteNonQuery(transactionManager.TransactionObject, commandType, commandText);
        }
        #endregion

        #region "ExecuteDataReader"
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteReader(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(TransactionManager transactionManager, string storedProcedureName, params object[] parameterValues)
        {
            Database database = transactionManager.Database;
            return database.ExecuteReader(transactionManager.TransactionObject, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(DbCommand command)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteReader(command);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(TransactionManager transactionManager, DbCommand command)
        {
            Database database = transactionManager.Database;
            return database.ExecuteReader(command, transactionManager.TransactionObject);
        }


        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public static IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteReader(commandType, commandText);
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(TransactionManager transactionManager, CommandType commandType, string commandText)
        {
            Database database = transactionManager.Database;
            return database.ExecuteReader(transactionManager.TransactionObject, commandType, commandText);
        }
        #endregion

        #region "ExecuteDataSet"
        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteDataSet(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        //public  DataSet ExecuteDataSet(TransactionManager transactionManager, string storedProcedureName, params object[] parameterValues)
        //{
        //    Database database = transactionManager.Database;
        //    return database.ExecuteDataSet(transactionManager.TransactionObject, storedProcedureName, parameterValues);
        //}

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(DbCommand command)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteDataSet(command);
        }

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(TransactionManager transactionManager, DbCommand command)
        {
            Database database = transactionManager.Database;
            return database.ExecuteDataSet(command, transactionManager.TransactionObject);
        }


        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteDataSet(commandType, commandText);
        }
        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(TransactionManager transactionManager, CommandType commandType, string commandText)
        {
            Database database = transactionManager.Database;
            return database.ExecuteDataSet(transactionManager.TransactionObject, commandType, commandText);
        }
        #endregion

        #region "ExecuteScalar"
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public static object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteScalar(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public object ExecuteScalar(TransactionManager transactionManager, string storedProcedureName, params object[] parameterValues)
        {
            Database database = transactionManager.Database;
            return database.ExecuteScalar(transactionManager.TransactionObject, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public static object ExecuteScalar(DbCommand command)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteScalar(command);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="command">The command wrapper.</param>
        /// <returns></returns>
        public object ExecuteScalar(TransactionManager transactionManager, DbCommand command)
        {
            Database database = transactionManager.Database;
            return database.ExecuteScalar(command, transactionManager.TransactionObject);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public static object ExecuteScalar(CommandType commandType, string commandText)
        {
            OracleDatabase database = new OracleDatabase(ConnectionString);
            return database.ExecuteScalar(commandType, commandText);
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public object ExecuteScalar(TransactionManager transactionManager, CommandType commandType, string commandText)
        {
            Database database = transactionManager.Database;
            return database.ExecuteScalar(transactionManager.TransactionObject, commandType, commandText);
        }
        #endregion

        #endregion
    }
}
