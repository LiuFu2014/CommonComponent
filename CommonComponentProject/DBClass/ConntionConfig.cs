using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Win32;


namespace DBClass
{
    /// <summary>
    /// SQL数据库服务器配置检测类
    /// </summary>
    public class ConntionConfig
    {

        private static string dbProviderName;
        private static string dbConnectionString; 
       
        /// <summary>
        /// 检查配置信息
        /// </summary>
        /// <returns>完整有效返回true,无效则启动配置界面</returns>
        public static bool CheckConntionConfig()
        {
            if (CheckedConnection())
            {
                return true;
            }
            else
            {
               return CheckedConfig();
            }
        }

        /// <summary>
        /// 验证配置信息的数据库连接
        /// </summary>
        /// <returns>数据库是否连接成功的bool型</returns>
        private static bool CheckedConnection()
        {
            Configuration Config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            if (Config.ConnectionStrings.ConnectionStrings["Wms"] != null)
            {
                //dbProviderName = DESEncrypt.Decrypt(dbProviderName);
                //dbConnectionString = DESEncrypt.Decrypt(dbConnectionString);
                dbProviderName = Config.ConnectionStrings.ConnectionStrings["Wms"].ProviderName;
                dbConnectionString = Config.ConnectionStrings.ConnectionStrings["Wms"].ConnectionString; 
                return TestConntion();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 测试与服务器数据库是否成功连接
        /// </summary>
        /// <returns>数据库是否连接成功的bool型</returns>
        public static bool TestConntion()
        {
            bool ConnectionSuccess = false;
            if (String.IsNullOrEmpty(dbProviderName) || String.IsNullOrEmpty(dbConnectionString))
            {
                return ConnectionSuccess;
            }
            else
            {
                DBHelper DB = new DBHelper();
                try
                {
                    DB.CloseCon();
                    DB.OpenCon();
                    ConnectionSuccess = true;
                    return ConnectionSuccess;
                }
                catch
                {
                    return ConnectionSuccess;
                }
                finally
                {
                    DB.CloseCon();
                }
            }
        }


        /// <summary>
        /// 验证配置信息
        /// </summary>
        private static bool CheckedConfig()
        {
            MessageBox.Show("数据库服务器无法连接，请重新配置。", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //SetDataBase SetDB = new SetDataBase();
            //SetDB.ShowDialog();

            //if (MessageBox.Show("是否现在进入系统？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            //    return CheckConntionConfig();
            //}
            //else
            //{
                return false;
            //}
        }

        /// <summary>
        /// 获取数据库连接的驱动类型
        /// </summary>
        public static string DBProviderName
        {
            get
            {
                return dbProviderName;
            }
        }

        /// <summary>
        /// 获取数据库连接的字符串
        /// </summary>
        public static string DBConnectionString
        {
            get
            {
                return dbConnectionString;
            }
        }





    }
}
