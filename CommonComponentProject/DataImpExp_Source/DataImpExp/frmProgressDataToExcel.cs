namespace DataImpExp
{
    using Excel;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    public class frmProgressDataToExcel : Form
    {
        private IEDataSourceType _DataSourceType = IEDataSourceType.dstDataTable;
        private string _FileName = "";
        private DataGridView _GrdData = null;
        private string _RptTitle = "";
        private System.Data.DataTable _TbData = null;
        private Excel.Application appExcel = null;
        private IContainer components = null;
        private DateTime dBegin;
        private DateTime dEnd;
        private SaveFileDialog dlgSave;
        private System.Windows.Forms.Label lblState;
        private int nCols = 0;
        private int nCurrC = 0;
        private int nCurrR = 0;
        private int nRows = 0;
        private ProgressBar prgMain;
        private Excel.Range range = null;
        private System.Windows.Forms.Timer trmMain;
        private Workbook wkBook = null;
        private Worksheet wkSheet = null;

        public frmProgressDataToExcel()
        {
            this.InitializeComponent();
        }

        private void DataGridViewToExcel()
        {
            bool flag = false;
            if (this._GrdData != null)
            {
                Exception exception2;
                this.dBegin = DateTime.Now;
                if (this.appExcel == null)
                {
                    this.appExcel = new ApplicationClass();
                }
                this.wkBook = this.appExcel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                this.wkSheet = (Worksheet) this.wkBook.Worksheets[1];
                this.appExcel.Visible = false;
                this.dEnd = DateTime.Now;
                this.nCols = this._GrdData.ColumnCount;
                this.nRows = this._GrdData.Rows.Count;
                this.nCurrR = 1;
                this.nCurrC = 1;
                if (this._RptTitle != "")
                {
                    this.SetControlText(this.lblState, "正在处理报表头......");
                    this.wkSheet.Cells[this.nCurrR, this.nCurrC] = this._RptTitle;
                    this.range = this.wkSheet.get_Range(this.wkSheet.Cells[this.nCurrR, 1], this.wkSheet.Cells[this.nCurrR, this.nCols]);
                    this.range.MergeCells = true;
                    this.range.Font.Bold = true;
                    this.range.Font.Size = 0x12;
                    this.range.Font.Name = "宋体";
                    this.range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    this.range.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    this.range.EntireRow.AutoFit();
                    this.nRows++;
                    this.nCurrR++;
                }
                this.nRows++;
                this.SetProgressMaxMin(this.prgMain, this.nRows, 0);
                this.SetProgressValue(this.prgMain, this.nCurrR);
                this.SetControlText(this.lblState, "正在处理列头......");
                this.nCurrC = 1;
                while (this.nCurrC <= this.nCols)
                {
                    this.wkSheet.Cells[this.nCurrR, this.nCurrC] = this._GrdData.Columns[this.nCurrC - 1].HeaderText;
                    this.nCurrC++;
                }
                this.nCurrR++;
                try
                {
                    foreach (DataGridViewRow row in (IEnumerable) this._GrdData.Rows)
                    {
                        this.nCurrC = 0;
                        while (this.nCurrC < this.nCols)
                        {
                            Exception exception;
                            string str3;
                            if (row.Cells[this.nCurrC].Value != null)
                            {
                                try
                                {
                                    string str2 = row.Cells[this.nCurrC].Value.ToString().Trim();
                                    double a;
                                    if (str2.StartsWith("=") || double.TryParse(str2,out a))
                                    {
                                        str2 = "'" + str2;
                                    }
                                    this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1] = str2;
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    str3 = this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1].ToString();
                                    string str4 = row.Cells[this.nCurrC].Value.ToString();
                                    MessageBox.Show(string.Format("在：nCurrC=[{3}]   nCurrR=[{4}]==[{0}]=:=[{1}]=:=[{2}]  nCols=[{5}]", new object[] { str3, exception.Message, str4, this.nCurrC, this.nCurrR, this.nCols }));
                                }
                            }
                            else
                            {
                                try
                                {
                                    this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1] = "";
                                }
                                catch (Exception exception3)
                                {
                                    exception = exception3;
                                    str3 = this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1].ToString();
                                }
                            }
                            this.nCurrC++;
                        }
                        this.SetControlText(this.lblState, "正在处理数据......(" + this.nCurrR.ToString() + "/" + this.nRows.ToString() + ")");
                        this.SetProgressValue(this.prgMain, this.nCurrR);
                        this.nCurrR++;
                    }
                }
                catch (Exception exception4)
                {
                    exception2 = exception4;
                    MessageBox.Show(exception2.Source + "\r\n" + exception2.Message + exception2.StackTrace + "\r\n" + exception2.TargetSite.ToString());
                }
                this.range = null;
                this.range = this.wkSheet.get_Range(this.wkSheet.Cells[1, 1], this.wkSheet.Cells[this.nRows, this.nCols]);
                this.range.Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                this.range.Borders.Weight = XlBorderWeight.xlThin;
                this.range.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
                if (this._FileName.Trim() == "")
                {
                    this._FileName = Path.Combine(System.Windows.Forms.Application.StartupPath, "datagrid.xls");
                }
                if (this._FileName.ToLower().IndexOf(".xls") <= 0)
                {
                    this._FileName = this._FileName + ".xls";
                }
                if (File.Exists(this._FileName))
                {
                    try
                    {
                        File.Delete(this._FileName);
                    }
                    catch (Exception exception5)
                    {
                        exception2 = exception5;
                        MessageBox.Show(exception2.Message);
                    }
                }
                try
                {
                    this.wkBook.SaveAs(this._FileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                    flag = true;
                }
                catch (Exception exception6)
                {
                    exception2 = exception6;
                    flag = false;
                    MessageBox.Show(exception2.Message);
                }
                try
                {
                    this.appExcel.Workbooks.Close();
                    this.appExcel.Quit();
                    DataIE.KillProgress((IntPtr) this.appExcel.Hwnd);
                    this.appExcel = null;
                }
                catch (Exception exception7)
                {
                    exception2 = exception7;
                    flag = false;
                    MessageBox.Show(exception2.Message);
                }
                if (flag)
                {
                    MessageBox.Show("已经成功导出：" + this._FileName);
                }
                base.Close();
            }
        }

        private void DataTableToExcel()
        {
            if (this._TbData != null)
            {
                this.dBegin = DateTime.Now;
                if (this.appExcel == null)
                {
                    this.appExcel = new ApplicationClass();
                }
                this.wkBook = this.appExcel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                this.wkSheet = (Worksheet) this.wkBook.Worksheets[1];
                this.appExcel.Visible = false;
                this.dEnd = DateTime.Now;
                this.nCols = this._TbData.Columns.Count;
                this.nRows = this._TbData.Rows.Count;
                this.nCurrR = 1;
                this.nCurrC = 1;
                if (this._RptTitle != "")
                {
                    this.SetControlText(this.lblState, "正在处理表头......");
                    this.wkSheet.Cells[this.nCurrR, this.nCurrC] = this._RptTitle;
                    this.range = this.wkSheet.get_Range(this.wkSheet.Cells[this.nCurrR, 1], this.wkSheet.Cells[this.nCurrR, this.nCols]);
                    this.range.MergeCells = true;
                    this.range.Font.Bold = true;
                    this.range.Font.Size = 0x12;
                    this.range.Font.Name = "宋体";
                    this.range.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                    this.range.VerticalAlignment = XlVAlign.xlVAlignCenter;
                    this.range.EntireRow.AutoFit();
                    this.nRows++;
                    this.nCurrR++;
                }
                this.nRows++;
                this.SetProgressMaxMin(this.prgMain, this.nRows, 0);
                this.SetProgressValue(this.prgMain, this.nCurrR);
                this.SetControlText(this.lblState, "正在处理列头......");
                this.nCurrC = 1;
                while (this.nCurrC <= this.nCols)
                {
                    this.wkSheet.Cells[this.nCurrR, this.nCurrC] = this._TbData.Columns[this.nCurrC - 1].ColumnName;
                    this.nCurrC++;
                }
                this.nCurrR++;
                this.SetProgressValue(this.prgMain, this.nCurrR);
                foreach (DataRow row in this._TbData.Rows)
                {
                    this.nCurrC = 0;
                    while (this.nCurrC < this.nCols)
                    {
                        if (row[this.nCurrC] != null)
                        {
                            string str = row[this.nCurrC].ToString().Trim();
                            double a;
                            if (str.StartsWith("=") || double.TryParse(str, out a))
                            {
                                str = "'" + str;
                            }
                            this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1] = str;
                        }
                        else
                        {
                            this.wkSheet.Cells[this.nCurrR, this.nCurrC + 1] = "";
                        }
                        this.nCurrC++;
                    }
                    this.SetControlText(this.lblState, "正在处理数据......(" + this.nCurrR.ToString() + "/" + this.nRows.ToString() + ")");
                    this.SetProgressValue(this.prgMain, this.nCurrR);
                    this.nCurrR++;
                }
                this.range = null;
                this.range = this.wkSheet.get_Range(this.wkSheet.Cells[1, 1], this.wkSheet.Cells[this.nRows, this.nCols]);
                this.range.Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                this.range.Borders.Weight = XlBorderWeight.xlThin;
                this.range.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlMedium;
                this.range.Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
                if (this._FileName.Trim() == "")
                {
                    this._FileName = Path.Combine(System.Windows.Forms.Application.StartupPath, this._TbData.TableName + ".xls");
                }
                if (this._FileName.ToLower().IndexOf(".xls") <= 0)
                {
                    this._FileName = this._FileName + ".xls";
                }
                this.wkBook.SaveAs(this._FileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                this.appExcel.Workbooks.Close();
                this.appExcel.Quit();
                DataIE.KillProgress((IntPtr) this.appExcel.Hwnd);
                this.appExcel = null;
                MessageBox.Show("已经成功导出：" + this._FileName);
                base.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoData()
        {
            if ((this._FileName.Trim() == "") && (this.dlgSave.ShowDialog() == DialogResult.OK))
            {
                this._FileName = this.dlgSave.FileName;
            }
            new Thread(new ThreadStart(this.DoExportExcel)).Start();
        }

        private void DoExportExcel()
        {
            switch (this._DataSourceType)
            {
                case IEDataSourceType.dstDataTable:
                    this.DataTableToExcel();
                    break;

                case IEDataSourceType.dstDataGridView:
                    this.DataGridViewToExcel();
                    break;
            }
        }

        private void frmProgressDataToExcel_Load(object sender, EventArgs e)
        {
            this.trmMain.Enabled = true;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.prgMain = new ProgressBar();
            this.lblState = new System.Windows.Forms.Label();
            this.dlgSave = new SaveFileDialog();
            this.trmMain = new System.Windows.Forms.Timer(this.components);
            base.SuspendLayout();
            this.prgMain.Location = new System.Drawing.Point(8, 0x27);
            this.prgMain.Name = "prgMain";
            this.prgMain.Size = new Size(0x261, 0x17);
            this.prgMain.TabIndex = 0;
            this.lblState.ForeColor = Color.Blue;
            this.lblState.Location = new System.Drawing.Point(8, 9);
            this.lblState.Name = "lblState";
            this.lblState.Size = new Size(0x25d, 0x17);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "稍候...";
            this.dlgSave.DefaultExt = "xls";
            this.dlgSave.Filter = "(*.xls)|*.xls";
            this.dlgSave.Title = "导出Excel...";
            this.trmMain.Interval = 200;
            this.trmMain.Tick += new EventHandler(this.trmMain_Tick);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x270, 0x4f);
            base.Controls.Add(this.lblState);
            base.Controls.Add(this.prgMain);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "frmProgressDataToExcel";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "导出EXCEL";
            base.Load += new EventHandler(this.frmProgressDataToExcel_Load);
            base.ResumeLayout(false);
        }

        private void SetControlText(Control ctrl, string sText)
        {
            if (ctrl.InvokeRequired)
            {
                ChangeControlText method = new ChangeControlText(this.SetControlText);
                base.Invoke(method, new object[] { ctrl, sText });
            }
            else
            {
                ctrl.Text = sText;
                base.Update();
            }
        }

        private void SetProgressMaxMin(ProgressBar prg, int nMax, int nMin)
        {
            if (prg.InvokeRequired)
            {
                SetProgresMaxMinValue method = new SetProgresMaxMinValue(this.SetProgressMaxMin);
                base.Invoke(method, new object[] { prg, nMax, nMin });
            }
            else
            {
                prg.Maximum = nMax;
                prg.Minimum = nMin;
                base.Update();
            }
        }

        private void SetProgressValue(ProgressBar prg, int nValue)
        {
            if (prg.InvokeRequired)
            {
                ChangleProgressValue method = new ChangleProgressValue(this.SetProgressValue);
                base.Invoke(method, new object[] { prg, nValue });
            }
            else
            {
                prg.Value = nValue;
                base.Update();
            }
        }

        private void ThreadCloseForm()
        {
            if (base.InvokeRequired)
            {
                ThrdCloseForm method = new ThrdCloseForm(this.ThreadCloseForm);
                base.Invoke(method, null);
            }
            else
            {
                base.Close();
            }
        }

        private void trmMain_Tick(object sender, EventArgs e)
        {
            this.trmMain.Enabled = false;
            this.DoExportExcel();
        }

        public IEDataSourceType DataSourceType
        {
            get
            {
                return this._DataSourceType;
            }
            set
            {
                this._DataSourceType = value;
            }
        }

        public string FileName
        {
            get
            {
                return this._FileName;
            }
            set
            {
                this._FileName = value;
            }
        }

        public DataGridView GrdData
        {
            get
            {
                return this._GrdData;
            }
            set
            {
                this._GrdData = value;
            }
        }

        public string RptTitle
        {
            get
            {
                return this._RptTitle;
            }
            set
            {
                this._RptTitle = value;
            }
        }

        public System.Data.DataTable TbData
        {
            get
            {
                return this._TbData;
            }
            set
            {
                this._TbData = value;
            }
        }

        public delegate void ChangeControlText(Control ctrl, string sText);

        public delegate void ChangleProgressValue(ProgressBar prg, int nValue);

        public delegate void SetProgresMaxMinValue(ProgressBar prg, int nMax, int nMin);

        public delegate void ThrdCloseForm();
    }
}

