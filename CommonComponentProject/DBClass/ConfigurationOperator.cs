using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;

namespace DBClass
{
    /// <summary> 
    /// 说明：本类主要负责对程序配置文件(.config)进行修改的类， 
    /// 可以对网站和应用程序的配置文件进行修改 
    /// </summary> 
    public class ConfigurationOperator
    {
        private Configuration config;
        private string configPath;

        /// <summary> 
        /// 对应的配置文件 
        /// </summary> 
        public Configuration Configuration
        {
            get { return config; }
            set { config = value; }
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="configType">.config文件的类型，只能是网站配置文件或者应用程序配置文件</param> 
        public ConfigurationOperator()
        {
            this.configPath = Application.ExecutablePath;
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(configPath);
        }

        /// <summary> 
        /// 构造函数 
        /// </summary> 
        /// <param name="path">.config文件的位置</param> 
        /// <param name="type">.config文件的类型，只能是网站配置文件或者应用程序配置文件</param> 
        public ConfigurationOperator(string configPath)
        {
            this.configPath = configPath;
            config = System.Configuration.ConfigurationManager.OpenExeConfiguration(configPath);
        }

        /// <summary> 
        /// 添加应用程序配置节点，如果已经存在此节点，则会修改该节点的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="value">节点值</param> 
        public void EditAppSetting(string key, string value)
        {
            AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings");
            if (appSetting.Settings[key] == null)//如果不存在此节点，则添加 
            {
                appSetting.Settings.Add(key, value);
            }
            else//如果存在此节点，则修改 
            {
                appSetting.Settings[key].Value = value;
            }
        }

        /// <summary> 
        /// 添加数据库连接字符串节点，如果已经存在此节点，则会修改该节点的值 
        /// </summary> 
        /// <param name="key">节点名称</param> 
        /// <param name="value">节点值</param> 
        public void EditConnectionString(string key, string connectionString, string providerName)
        {
            ConnectionStringsSection connectionSetting = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (connectionSetting.ConnectionStrings[key] == null)//如果不存在此节点，则添加 
            {
                ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings(key, connectionString, providerName);
                config.ConnectionStrings.ConnectionStrings.Add(connectionStringSettings);
            }
            else//如果存在此节点，则修改 
            {
                connectionSetting.ConnectionStrings[key].ConnectionString = connectionString;
                connectionSetting.ConnectionStrings[key].ProviderName = providerName;
            }
        }

        /// <summary> 
        /// 保存所作的修改 
        /// </summary> 
        public void Save()
        {
            config.Save();
        }
    }
}
