using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Data.OleDb;
using Excel2 = Microsoft.Office.Interop.Excel;
using System.Reflection;//Missing(Excel)-->SQLDataToExcel

namespace DataImpExp
{
    /// <summary>
    /// 使用office的com接口调用
    /// </summary>
    class ExportData
    {
        static StreamWriter writer = null;

        /// <summary>
        /// 导入记事本
        /// </summary>
        /// <param name="lv">ListView</param>
        public static void SaveToNotePad(ListView lv)
        {
            #region

            FileStream outputfile = null; // 写文件

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.DefaultExt = "xls";
            saveDlg.Filter = "文本文件(*.txt)|*.txt|文本文件(*.ini)|*.ini|所有文件(*.*)|*.*";
            saveDlg.Title = "文件另存为";

            try
            {
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    outputfile = new FileStream(saveDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    writer = new StreamWriter(outputfile);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    for (int i = 0; i < lv.Items.Count; i++)
                    {
                        DoWrite(lv.Columns[0].Text + ":" + lv.Items[i].Text);
                        for (int j = 1; j < lv.Columns.Count; j++)
                        {
                            DoWrite(lv.Columns[j].Text + ":" + lv.Items[i].SubItems[j].Text);
                        }
                        DoWrite("--------");
                    }
                    writer.Close();
                    MessageBox.Show("已将数据成功导入记事本：" + saveDlg.FileName + " 中！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                outputfile = null;
                writer = null;
                saveDlg.Dispose();
            }

            #endregion
        }

        /// <summary>
        /// 导入记事本
        /// </summary>
        /// <param name="dv">DataGridView</param>
        public static void SaveToNotePad(DataGridView dv)
        {
            #region

            //if (File.Exists(filepath))
            //{
            //    Response.Write("<script>(confirm('文件存在！是不是要替换？');</script>");
            //}

            FileStream outputfile = null; // 写文件

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.DefaultExt = "xls";
            saveDlg.Filter = "文本文件(*.txt)|*.txt|文本文件(*.ini)|*.ini|所有文件(*.*)|*.*";
            saveDlg.Title = "文件另存为";

            try
            {
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveDlg.FileName)) File.Delete(saveDlg.FileName);

                    outputfile = new FileStream(saveDlg.FileName, FileMode.Create, FileAccess.Write);//OpenOrCreate
                    writer = new StreamWriter(outputfile);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    for (int i = 0; i < dv.RowCount; i++)
                    {
                        for (int j = 0; j < dv.Columns.Count; j++)
                        {
                            DoWrite(dv.Columns[j].Name.ToString() + ":" + dv.Rows[i].Cells[j].Value.ToString());
                        }
                        DoWrite("--------");
                    }
                    writer.Close();
                    MessageBox.Show("已将数据成功导入记事本：" + saveDlg.FileName + " 中！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                outputfile = null;
                writer = null;
                saveDlg.Dispose();
            }

            #endregion
        }

        /// <summary>
        /// 11.5,Writed by Fufu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool SaveToTxt(string text)
        {
            FileStream outputfile = null; // 写文件

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.DefaultExt = "xls";
            saveDlg.Filter = "文本文件(*.txt)|*.txt|文本文件(*.ini)|*.ini|所有文件(*.*)|*.*";
            saveDlg.Title = "文件另存为";

            try
            {
                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(saveDlg.FileName)) File.Delete(saveDlg.FileName);

                    outputfile = new FileStream(saveDlg.FileName, FileMode.Create, FileAccess.Write);//OpenOrCreate
                    writer = new StreamWriter(outputfile);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    writer.Write(text);

                    writer.Close();
                    //MessageBox.Show("已将数据成功导入记事本：" + saveDlg.FileName + " 中！");
                    return true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                outputfile = null;
                writer = null;
                saveDlg.Dispose();
                return false;
            }

            return false;
        }

        public static void DoWrite(String line)
        {
            line += "\r\n";         // 加上换行符

            byte[] binfo = new System.Text.UTF8Encoding(true).GetBytes(line);
            char[] info = new System.Text.UTF8Encoding(true).GetChars(binfo);

            writer.Write(info, 0, info.Length);//writer.WriteLine(line);
            writer.Flush();
        }


        /// <summary>
        ///   以操作Excel控件的方式将DataGridView数据导出到Excel   add   by   sunny   2007/1/18 
        /// </summary > 
        /// <param   name= "GridView " >DataGridView对象 </param > 
        /// <param   name= "strExcelFile " >Excel文件名 </param > 
        /// <param   name= "strError " >out参数，返回出错信息 </param > 
        /// <returns > 
        ///         -1   出错 
        ///         0   成功 
        /// </returns > 
        public static int DataGridViewToExcel(System.Windows.Forms.DataGridView GridView)
        {
            #region

            int nRet = 0;

            SaveFileDialog saveDlg1 = new SaveFileDialog();
            saveDlg1.DefaultExt = "xls";
            saveDlg1.Filter = "Microsoft Office Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            saveDlg1.Title = "文件另存为";//saveDlg1.FileName
                                     //saveDlg1.InitialDirectory = Directory.GetCurrentDirectory();   

            //if (saveDlg1.ShowDialog() == DialogResult.Cancel) return nRet;
            if (saveDlg1.ShowDialog() == DialogResult.Cancel) return -1;


            Excel.Application xlApp = new Excel.Application();
            Excel.Workbooks workbooks = xlApp.Workbooks;
            Excel.Workbook workbook = workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 

            try
            {
                //~~ 
                //   写字段名 
                for (int i = 0; i < GridView.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = GridView.Columns[i].HeaderText.ToString();
                }

                //   写记录 
                for (int i = 0; i < GridView.Rows.Count; i++)
                {
                    for (int j = 0; j < GridView.Columns.Count; j++)
                    {
                        //Fu_我的很多dgv里面都是复杂类，无法用这条语句导出
                        //worksheet.Cells[i + 2, j + 1] = GridView.Rows[i].Cells[j].Value.ToString();
                        //Fu_11.5_重写，获取格式化后的值
                        //worksheet.Cells[i + 2, j + 1] = GridView.Rows[i].Cells[j].EditedFormattedValue == null ? "" : GridView.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        string value = GridView.Rows[i].Cells[j].EditedFormattedValue == null ? "" : GridView.Rows[i].Cells[j].EditedFormattedValue.ToString();
                        if (value.Length > 11)
                        {
                            value = "'" + value;
                        }
                        worksheet.Cells[i + 2, j + 1] = value;
                    }
                }

                worksheet.Columns.EntireColumn.AutoFit();//自动适应每列的宽度   add   by   sunny.li 
                                                         //下面这句会报错
                                                         //Excel.Range rg = worksheet.get_Range(worksheet.Cells[1, 1], worksheet.Cells[1, GridView.Columns.Count]);
                                                         //Fu_csdn上的写法 ， 膜拜大神...
                Excel.Range rg = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, GridView.Columns.Count]];
                rg.Font.Bold = true;
                workbook.Saved = true;
                workbook.SaveCopyAs(saveDlg1.FileName);

                //   关掉内存中的进程 

                // xlApp.Quit();

                nRet = 0;
            }
            catch (Exception ex)
            {
                //strError = ex.ToString();
                //MessageBox.Show(ex.ToString());
                nRet = -1;
            }

            return nRet;
            #endregion
        }


        /// <summary>
        /// 读取Excel文件，将内容存储在DataSet中
        /// </summary>
        /// <param name="opnFileName">带路径的Excel文件名</param>
        /// <returns>DataSet</returns>
        public static DataSet ExcelToDataSet(string TableName)
        {
            #region

            string strExcel = "";

            OpenFileDialog openDlg1 = new OpenFileDialog();
            openDlg1.DefaultExt = "xls";
            openDlg1.Filter = "Microsoft Office Excel文件(*.xls)|*.xls|所有文件(*.*)|*.*";
            openDlg1.Title = "打开";
            //saveDlg1.InitialDirectory = Directory.GetCurrentDirectory();   

            if (openDlg1.ShowDialog() == DialogResult.Cancel) return null;

            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + openDlg1.FileName + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            OleDbConnection conn = new OleDbConnection(strConn);
            OleDbDataAdapter da = null;
            DataSet ds = new DataSet();

            strExcel = "select * from [sheet1$]";
            //strExcel = string.Format("select * from [{0}$]", "sheet1");


            try
            {
                da = new OleDbDataAdapter(strExcel, strConn);
                da.Fill(ds, TableName);

                //用bcp导入数据
                //using (System.Data.SqlClient.SqlBulkCopy bcp = new System.Data.SqlClient.SqlBulkCopy(Login.Cn()))
                //{
                //    //bcp.SqlRowsCopied += new System.Data.SqlClient.SqlRowsCopiedEventHandler(bcp_SqlRowsCopied);
                //    bcp.BatchSize = 100;//每次传输的行数
                //    bcp.NotifyAfter = 100;//进度提示的行数
                //    bcp.DestinationTableName = TableName;//目标表
                //    bcp.WriteToServer(ds.Tables[0]);
                //}

                return ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入出错：" + ex, "错误信息");
                return ds;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            #endregion
        }

        ////进度显示
        //void bcp_SqlRowsCopied(object sender, System.Data.SqlClient.SqlRowsCopiedEventArgs e)
        //{
        //    this.Text = e.RowsCopied.ToString();
        //    this.Update();
        //}

        /// <summary> 
        /// 直接导出Sql表数据到excel --用excel的QueryTable进行批量数据的导入代码
        /// </summary> 
        /// <param name="connectionString">连接字符串</param> 
        /// <param name="sql">查询语句</param> 
        /// <param name="fileName">文件名</param> 
        /// <param name="sheetName">表名</param> 
        /// <returns>真假值</returns>
        //public static bool SQLDataToExcel(string connectionString, string sql, string fileName, string sheetName)
        //{
        //    #region
        //    //SQLDataToExcel("Provider=SQLOLEDB.1;sever=localhost;uid=sa;password=123;database=autowarehouse;",
        //    //"select * from tblmaterialno", @"c:\testOle.xls", "tblmaterialno"); 

        //    Excel.Application app = new Excel.ApplicationClass();
        //    Excel.Workbook wb = (Excel.WorkbookClass)app.Workbooks.Add(Missing.Value);
        //    Excel.Worksheet ws = wb.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value) as Excel.Worksheet;
        //    ws.Name = sheetName;

        //    try
        //    {
        //        Excel.QueryTable qt = ws.QueryTables.Add("OLEDB;" + connectionString,
        //            ws.get_Range("A1", Missing.Value), sql);
        //        qt.Refresh(false);//是否异步查询 
        //    }
        //    catch (Exception ex)
        //    {
        //        string str = ex.Message;
        //    }
        //    finally
        //    {
        //        wb.Saved = true;
        //        wb.SaveCopyAs(fileName);//保存 
        //        app.Quit();//关闭进程 
        //    }
        //    return true;
        //    #endregion
        //}
    }
}
