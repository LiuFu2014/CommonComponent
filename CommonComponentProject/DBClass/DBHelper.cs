using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;

namespace DBClass
{
    public class DBHelper
    {
        private string dbProviderName;
        private string dbConnectionString;
        private DbConnection connection;
        private DbCommand dbCommand;
        public string ErrorMessage = null;

        public DBHelper()
        {
            dbProviderName = ConntionConfig.DBProviderName;
            dbConnectionString = ConntionConfig.DBConnectionString;
            this.connection = CreateConnection(dbConnectionString);
            this.dbCommand = this.connection.CreateCommand();
        }

        public DBHelper(string connectionString)
        {
            this.connection = CreateConnection(connectionString);
            this.dbCommand = this.connection.CreateCommand();
        }

        /// <summary>
        ///  生成DbConnection对象
        /// </summary>
        /// <returns>DbConnection对象</returns>
        private DbConnection CreateConnection()
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = dbConnectionString;
            return dbconn;
        }

        /// <summary>
        /// 生成DbConnection对象
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <returns>DbConnection对象</returns>
        private DbConnection CreateConnection(string connectionString)
        {
            DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
            DbConnection dbconn = dbfactory.CreateConnection();
            dbconn.ConnectionString = connectionString;
            return dbconn;
        }

        /// <summary>
        /// 生成DbCommand
        /// </summary>
        /// <param name="storedProcedure">存储过程语句</param>
        public void SetStoredProcCommond(string storedProcedure)
        {
            dbCommand.Parameters.Clear();
            dbCommand.CommandText = storedProcedure;
            dbCommand.CommandType = CommandType.StoredProcedure;
        }

        /// <summary>
        /// 生成DbCommand
        /// </summary>
        /// <param name="sqlQuery">SQL语句</param>
        public void SetSqlStringCommond(string sqlQuery)
        {
            dbCommand.Parameters.Clear();
            dbCommand.CommandText = sqlQuery;
            dbCommand.CommandType = CommandType.Text;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            OpenCon();
            dbCommand.Transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// 提交事物
        /// </summary>
        public void CommitTransaction()
        {
            if (dbCommand.Transaction != null)
            {
                try
                {
                    if (ErrorMessage == null)
                    {
                        dbCommand.Transaction.Commit();
                    }
                    else
                    {
                        dbCommand.Transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    dbCommand.Transaction.Rollback();
                    ErrorMessage = ex.Message;
                }
            }
            CloseCon();
        }


        #region 增加参数
        /// <summary>
        /// 新增参数列表
        /// </summary>
        /// <param name="dbParameterCollection">要增加的参数列表</param>
        public void AddParameterCollection(DbParameterCollection dbParameterCollection)
        {
            foreach (DbParameter dbParameter in dbParameterCollection)
            {
                dbCommand.Parameters.Add(dbParameter);
            }
        }
        /// <summary>
        /// 新增参数列表
        /// </summary>
        /// <param name="dbParameterCollection">要增加的参数列表</param>
        public void AddParameterCollection(List<DbParameter> DbParameterList)
        {
            foreach (DbParameter dbParameter in DbParameterList)
            {
                dbCommand.Parameters.Add(dbParameter);
            }
        }
        /// <summary>
        /// 新增输出参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="size">参数长度</param>
        public void AddOutParameter(string parameterName, DbType dbType, int size)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Size = size;
            dbParameter.Direction = ParameterDirection.Output;
            dbCommand.Parameters.Add(dbParameter);
        }
        /// <summary>
        /// 新增输入参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="value">参数值</param>
        public void AddInParameter(string parameterName, DbType dbType, object value)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            dbCommand.Parameters.Add(dbParameter);
        }
        /// <summary>
        /// 新增返回参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="dbType">参数类型</param>
        public void AddReturnParameter(string parameterName, DbType dbType)
        {
            DbParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = parameterName;
            dbParameter.Direction = ParameterDirection.ReturnValue;
            dbCommand.Parameters.Add(dbParameter);
        }
        /// <summary>
        /// 获取数据库的指定参数
        /// </summary>
        public DbParameter GetParameter(string parameterName)
        {
            return dbCommand.Parameters[parameterName];
        }
        /// <summary>
        /// 获取数据库的指定参数
        /// </summary>
        public void ClearParameter()
        {
            dbCommand.Parameters.Clear();
        }
        /// <summary>
        /// 生成数据库的参数
        /// </summary>
        public static System.Data.Common.DbParameter GetParameter(string parameterName, DbType dbType, object value)
        {
             DbParameter dbParameter = System.Data.Common.DbProviderFactories.GetFactory(ConntionConfig.DBProviderName).CreateParameter();
             dbParameter.DbType = dbType;
             dbParameter.ParameterName = parameterName;
             dbParameter.Value = value;
             dbParameter.Direction = ParameterDirection.Input;
             return dbParameter;
        }
        #endregion


        /// <summary>
        ///  执行dbCommand中的SQL语句，执行完之后返回DataSet对象
        /// </summary>
        /// <returns>返回DataSet对象</returns>
        public DataSet ExecuteDataSet()
        {
            DataSet ds = new DataSet();
            if (ErrorMessage == null)
            {
                try
                {
                    DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
                    DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
                    dbDataAdapter.SelectCommand = dbCommand;
                    dbDataAdapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return ds;
        }

        /// <summary>
        ///  执行dbCommand中的SQL语句，执行完之后返回DataTable对象
        /// </summary>
        /// <returns>返回DataTable对象</returns>
        public DataTable ExecuteDataTable()
        {
            DataTable dataTable = new DataTable();
            if (ErrorMessage == null)
            {
                try
                {
                    DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
                    DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
                    dbDataAdapter.SelectCommand = dbCommand;
                    dbDataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 用DataTable对象更新数据库
        /// 1.DataTable的列可以多于数据库的列，不能少于数据库的列 
        /// 2.不能用一个DataTable更新两个表的时候，需要调用两次GetChanges()方法获取两个DataTable来更新两个表
        /// 3.一个DataTable更新之后，需要用AcceptChanges来消除状态，然后再提交更新，重复更新会爆并发性错误
        /// </summary>
        /// <param name="ClientDataTable">用来跟更新的DataTable</param>
        /// <param name="SQLString">用来匹配DataTable格式的查询Sql语句</param>
        /// <returns>影响的行数</returns>
        public int SubmitDataTable(DataTable ClientDataTable, string SQLString)
        {
            int RowsCount = 0;
            if (ClientDataTable != null && ErrorMessage == null)
            {
                try
                {
                    SetSqlStringCommond(SQLString);
                    DbProviderFactory dbfactory = DbProviderFactories.GetFactory(dbProviderName);
                    DbDataAdapter dbDataAdapter = dbfactory.CreateDataAdapter();
                    dbDataAdapter.SelectCommand = dbCommand;
                    DbCommandBuilder DCB = dbfactory.CreateCommandBuilder();
                    DCB.DataAdapter = dbDataAdapter;
                    RowsCount = dbDataAdapter.Update(ClientDataTable);
                    return RowsCount;
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        ///  执行dbCommand中的SQL语句，执行完之后返回DbDataReader类型的数据集
        /// </summary>
        /// <returns>返回SQL语句查询的结果</returns>
        public DbDataReader ExecuteReader()
        {
            DbDataReader reader = null;
            if (ErrorMessage == null)
            {
                try
                {
                    OpenCon();
                    reader = dbCommand.ExecuteReader();
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return reader;
        }

        /// <summary>
        ///  执行dbCommand中的SQL查询语句
        /// </summary>
        /// <returns>返回查询到得第一行第一列</returns>
        public Object ExecuteScalar()
        {
            Object ReturnValue = null;
            if (ErrorMessage == null)
            {
                try
                {
                    OpenCon();
                    ReturnValue = dbCommand.ExecuteScalar();
                    CloseCon();
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return ReturnValue;
        }

        /// <summary>
        ///  执行dbCommand中的SQL查询语句
        /// </summary>
        /// <returns>返回查询到得第一行第一列</returns>
        public Object ExecuteScalar_NotClose()
        {
            Object ReturnValue = null;
            if (ErrorMessage == null)
            {
                try
                {
                    OpenCon();
                    ReturnValue = dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return ReturnValue;
        }

        /// <summary>
        ///  执行dbCommand中的SQL语句，执行完之后自动关闭数据库连接
        /// </summary>
        /// <returns>执行影响的行数</returns>
        public int ExecuteNonQuery()
        {
            int ret = 0;
            if (ErrorMessage == null)
            {
                try
                {
                    OpenCon();
                    ret = dbCommand.ExecuteNonQuery();
                    CloseCon();
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return ret;
        }

        /// <summary>
        ///  执行dbCommand中的SQL语句，执行之后不关闭数据库可以继续执行
        /// </summary>
        /// <returns>执行影响的行数</returns>
        public int ExecuteNonQuery_NotClose()
        {
            int ret = 0;
            if (ErrorMessage == null)
            {
                try
                {
                    OpenCon();
                    ret = dbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ShowException(ex);
                }
            }
            return ret;
        }


        /// <summary>
        /// 打开Con连接
        /// </summary>
        public void OpenCon()
        {
            if (connection.State == ConnectionState.Closed)   
            {
                connection.Open();   //打开数据库的连接
            }
        }

        /// <summary>
        /// 关闭Con连接
        /// </summary>        
        public void CloseCon()
        {
            if (connection.State == ConnectionState.Open)   
            {
                connection.Close();   //关闭数据库的连接
                //connection.Dispose();   //释放My_con变量的所有空间
            }
        }
        /// <summary>
        ///  处理数据执行的异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        private void ShowException(Exception ex)
        {
            if (dbCommand.Transaction != null)
            {
                dbCommand.Transaction.Rollback();
            }
            CloseCon();
            ErrorMessage = ex.Message;
            System.Windows.Forms.MessageBox.Show(ErrorMessage);
        }
 
    }
}