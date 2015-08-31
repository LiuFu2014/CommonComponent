using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace DBClass
{
    public partial class SetDataBase : Form
    {
        public SetDataBase()
        {
            InitializeComponent();
        }

        private void SetDataBase_Load(object sender, EventArgs e)
        {
            dbProviderCbx.SelectedIndex = 0;
            //DatabaseCbx.SelectedIndex = 0;              
        }

        private void dbProviderName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dbProviderCbx.SelectedIndex == 0)
            {
                //获取本地网络的所有服务器
                System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
                DataTable Dt = instance.GetDataSources();
                Dt.Columns["ServerName"].ReadOnly = false;
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    if (Dt.Rows[i]["InstanceName"] != DBNull.Value && Dt.Rows[i]["InstanceName"].ToString() != "")
                    {
                        Dt.Rows[i]["ServerName"] = Dt.Rows[i]["ServerName"].ToString() + "\\" + Dt.Rows[i]["InstanceName"].ToString();
                    }
                }
                ServerCbx.DataSource = Dt;
                ServerCbx.DisplayMember = "ServerName";
                ServerCbx.ValueMember = "ServerName";
            }
            else
            {
                ServerCbx.DataSource = null;
            }
        }

        private void DatabaseCbx_DropDown(object sender, EventArgs e)
        {
            if (ServerCbx.Text != "" && UID.Text != "" && UID.Text != "")
            {
                String ConnectionString = String.Format("Server={0};Database={1};UID={2};PWD={3}", ServerCbx.Text, "master", UID.Text, PWD.Text);
                DataTable Dt = new DataTable();
                try
                {
                    SqlDataAdapter Da = new SqlDataAdapter("sp_helpdb", ConnectionString);
                    Da.Fill(Dt);
                    DatabaseCbx.DataSource = Dt;
                    DatabaseCbx.DisplayMember = "name";
                    DatabaseCbx.ValueMember = "name";
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dbProviderName = "System.Data.SqlClient";
            string dbConnectionString = String.Format("Server={0};Database={1};UID={2};PWD={3};", ServerCbx.Text, DatabaseCbx.Text, UID.Text, PWD.Text);
            //dbProviderName = DESEncrypt.Encrypt(dbProviderName);
            //dbConnectionString = DESEncrypt.Encrypt(dbConnectionString);

            ConfigurationOperator Config = new ConfigurationOperator();
            Config.EditConnectionString("Wms", dbConnectionString, dbProviderName);
            Config.Save();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            if (!this.Modal)
                Application.Exit();
        }


    }
}






