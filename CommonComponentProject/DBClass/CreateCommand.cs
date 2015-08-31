using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace DBClass
{
    public class CreateCommand
    {

        /// <summary>
        /// 根据控件生成Insert的Commond对象，返回新增语句的主键
        /// </summary>
        /// <param name="FormControl">需要组成SQL语句的控件</param>
        /// <param name="Table">表名</param>
        /// <param name="TableId">更新的ID</param>
        /// <param name="db">需要生成Commond的DBHelper</param>
        /// <returns>更新数据的键值</returns>
        public static void GetInsertCommond(Control FormControl, string Table, string TableId, DBClass.DBHelper db)
        {
            string KeyValue = string.Empty;
            StringBuilder FieldString = new StringBuilder();
            StringBuilder ValueString = new StringBuilder();

            foreach (Control ControlItem in FormControl.Controls)
            {
                if (ControlItem.GetType() == typeof(TextBox) || ControlItem.GetType() == typeof(RichTextBox))
                {
                    if (!string.IsNullOrEmpty(ControlItem.Text))
                    {
                        if (ControlItem.Name == TableId)
                        {
                            KeyValue = ControlItem.Text;
                        }
                        FieldString.AppendFormat("{0},", ControlItem.Name);
                        ValueString.AppendFormat("@{0},", ControlItem.Name);
                        db.AddInParameter(string.Format("@{0}", ControlItem.Name), DbType.AnsiString, ControlItem.Text);
                    }
                }
            }
            string SQLString = String.Format("Insert into {0} ({1}) values ({2})",
                                                Table,
                                                FieldString.ToString(0, FieldString.Length - 1),
                                                ValueString.ToString(0, ValueString.Length - 1));
            db.SetSqlStringCommond(SQLString);
        }
        
        /// <summary>
        /// 根据控件返回Update的Commond对象
        /// </summary>
        /// <param name="FormControl">需要组成SQL语句的控件</param>
        /// <param name="Table">表名</param>
        /// <param name="TableId">更新的ID</param>
        /// <param name="db">需要生成Commond的DBHelper</param>
        /// <returns>更新数据的键值</returns>
        public static void GetUpdateCommond(Control FormControl, string Table, string TableId, DBClass.DBHelper db)
        {
            string Where = string.Empty;
            string KeyValue = string.Empty;
            StringBuilder FieldString = new StringBuilder();
            for (int i = 0; i < FormControl.Controls.Count; i++)
            {
                if (FormControl.Controls[i].GetType() == typeof(TextBox) || FormControl.Controls[i].GetType() == typeof(RichTextBox))
                {
                    if (!string.IsNullOrEmpty(FormControl.Controls[i].Text))
                    {
                        if (FormControl.Controls[i].Name == TableId)
                        {
                            KeyValue = FormControl.Controls[i].Text;
                            Where = String.Format("{0}=@{0}", FormControl.Controls[i].Name);
                        }
                        else
                        {
                            FieldString.AppendFormat("{0}=@{0},", FormControl.Controls[i].Name);
                        }
                    }
                }
            }
            string SQLString = String.Format("Update {0} set {1}  Where {2}", Table, FieldString.ToString(0, FieldString.Length - 1), Where);
            db.SetSqlStringCommond(SQLString);
            for (int i = 0; i < FormControl.Controls.Count; i++)
            {
                if (FormControl.Controls[i].GetType() == typeof(TextBox) || FormControl.Controls[i].GetType() == typeof(RichTextBox))
                {
                    if (!string.IsNullOrEmpty(FormControl.Controls[i].Text))
                    {
                        if (FormControl.Controls[i].Name == TableId)
                        {
                            db.AddInParameter(string.Format("@{0}", FormControl.Controls[i].Name), DbType.AnsiString, FormControl.Controls[i].Text);
                        }
                        else
                        {
                            db.AddInParameter(string.Format("@{0}", FormControl.Controls[i].Name), DbType.AnsiString, FormControl.Controls[i].Text);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="WorkStation">主机名</param>
        /// <param name="ProgramId">程序模块名</param>
        /// <param name="OptType">操作类型</param>
        /// <param name="KeyValue">操作键值</param>
        /// <param name="db">需要生成Commond的DBHelper</param>
        public static void GetSysLogCommond(string UserId, string WorkStation, string ProgramId, string OptType, string KeyValue, DBClass.DBHelper db)
        {
            db.SetSqlStringCommond("Insert Into Wms_SysLog (UserId, WorkStation, ProgramId, OptTime, OptType, KeyValue) Values (@UserId, @WorkStation, @ProgramId, @OptTime, @OptType, @KeyValue)");
            db.AddInParameter("@UserId", DbType.AnsiString, UserId);
            db.AddInParameter("@WorkStation", DbType.AnsiString, WorkStation);
            db.AddInParameter("@ProgramId", DbType.AnsiString, ProgramId);
            db.AddInParameter("@OptTime", DbType.AnsiString, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            db.AddInParameter("@OptType", DbType.AnsiString, OptType);
            db.AddInParameter("@KeyValue", DbType.AnsiString, KeyValue);
        }








    }
}
