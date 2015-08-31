using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace Common
{
    /// <summary>
    /// 使用说明：
    /// 1.传入一个dgv作为参数，但不必给他赋值
    /// 2.使用自带函数刷新数据
    /// 3.使用DgvDisplayUtil配置你需要显示的列
    /// </summary>
    public class DgvDisplayUtil
    {
        const string dgv_chk_tag = "F6FC5CEF7E5B48bbB04834B22DC248C3";
        private DataGridView _Dgv;
        private ContextMenuStrip ContextMenuStripSet = null;
        private object Cellstr = null;
        private bool _IsCheckBox = false;
        private BindingSource dataSource = new BindingSource();//数据源
        public  DataTable dtSource = null;

    


        private bool changesource = false;
        private List<HeadColFilter> HeadColList = new List<HeadColFilter> ();

        public void AddDgvTitle(string ColName, string HeaderText, int Width)
        {
            if (_Dgv.Columns.Contains(ColName))
            {
                return;
            }
            DataGridViewColumn colitem = new DataGridViewTextBoxColumn();
            colitem.Name = colitem.DataPropertyName = ColName;
            colitem.Width = Width;
            colitem.HeaderText = HeaderText;
            colitem.DisplayIndex = _Dgv.Columns.Count;
            colitem.ReadOnly = true;
            colitem.Frozen = false;
            _Dgv.Columns.Add(colitem);
            HeadColFilter var = HeadColList.Find(a => a.ColName == ColName);

            if (var==null)
            {
                HeadColList.Add(new HeadColFilter() {ColName=ColName,FilterStr="" });
                dgvFilterColHeadCell dgvheadercell = new dgvFilterColHeadCell(colitem.HeaderCell, ColFilterType.list.ToString());
                dgvheadercell.AutomaticSortingEnabled = true;
                dgvheadercell.eventFilterEx += new dgvFilterColHeadCell.delegateFilterEx(dgvheadercell_eventFilterEx);
                colitem.HeaderCell = dgvheadercell;
            }
        }

        private void dgvheadercell_eventFilterEx(string Filterstr, string ColName)
        {
            try
            {
                HeadColFilter var = HeadColList.Find(a => a.ColName == ColName);

                if (var != null)
                {
                    string Filters = string.Empty;
                    var.FilterStr = Filterstr;
                    bool ta = true;
                    foreach (HeadColFilter str in HeadColList)
                    {
                        if (!string.IsNullOrEmpty(str.FilterStr))
                        {
                            if (ta)
                            {
                                Filters = str.FilterStr;
                                ta = false;
                            }
                            else
                            {

                                Filters = Filters + " AND ( " + str.FilterStr + " )";
                            }

                        }
                    }
                    dataSource.Filter = Filters;
                }
            }
            catch
            {

                dataSource.Filter = "";
            }
        }


        public void AddDateTimeDgvTitle(string ColName, string HeaderText, int Width)
        {
            if (_Dgv.Columns.Contains(ColName))
            {
                return;
            }
            DataGridViewColumn colitem = new DataGridViewTextBoxColumn();
            colitem.Name = colitem.DataPropertyName = ColName;
            colitem.Width = Width;
            colitem.HeaderText = HeaderText;
            colitem.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss FFF";
            colitem.DisplayIndex = _Dgv.Columns.Count;
            colitem.ReadOnly = true;
            colitem.Frozen = false;
            _Dgv.Columns.Add(colitem);
        }

        public void AddDateDgvTitle(string ColName, string HeaderText, int Width)
        {
            if (_Dgv.Columns.Contains(ColName))
            {
                return;
            }
            DataGridViewColumn colitem = new DataGridViewTextBoxColumn();
            colitem.Name = colitem.DataPropertyName = ColName;
            colitem.Width = Width;
            colitem.HeaderText = HeaderText;
            colitem.DefaultCellStyle.Format = "yyyy-MM-dd";
            colitem.DisplayIndex = _Dgv.Columns.Count;
            colitem.ReadOnly = true;
            colitem.Frozen = false;
            _Dgv.Columns.Add(colitem);
        }



        public DgvDisplayUtil(DataGridView Dgv)
        {
            if (Dgv == null)
            {
                throw new Exception("DataGridView为空无法进行操作");
            }
            _Dgv = Dgv;
            InitContextMenuStripSet();
            InitDgv();
        }

        public bool IsCheckbox
        {
            get
            {
                return _IsCheckBox;
            }
            set
            {
                if (_Dgv.Columns.Contains(dgv_chk_tag))
                {
                    _Dgv.Columns[dgv_chk_tag].Visible = value;
                    if (value)
                    {
                        _Dgv.Columns[dgv_chk_tag].Width = 25;
                    }
                    else
                    {
                        _Dgv.Columns[dgv_chk_tag].Width = 10;

                    }
                    _IsCheckBox = value;
                }
                else
                {
                    _IsCheckBox = false;
                }
            }

        }

        public void RefreshDgvData(DataTable dt)
        {
            dtSource = dt;
            if (dt == null)
            {
                dt = new DataTable();
            }
            if (!dt.Columns.Contains(dgv_chk_tag))
            {
                dt.Columns.Add(dgv_chk_tag, typeof(Boolean));
            }
            dataSource.DataSource = dt;
            changesource = true;
            _Dgv.DataSource = dataSource;
            changesource = false;
        }

        private void InitDgv()
        {
            _Dgv.AllowUserToAddRows = false;//禁止用户添加行;
            _Dgv.AllowUserToDeleteRows = false;//禁止用户删除行;
            _Dgv.AllowUserToOrderColumns = false;//允许用户拖动列顺序;
            _Dgv.EnableHeadersVisualStyles = false;//显示格式;
            _Dgv.AutoGenerateColumns = false;
            _Dgv.MultiSelect = false;
            _Dgv.RowHeadersWidth = 15;
            _Dgv.BorderStyle = BorderStyle.Fixed3D;
            _Dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            _Dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _Dgv.CellMouseDown += new DataGridViewCellMouseEventHandler(dgv_CellMouseDown);
            _Dgv.CurrentCellDirtyStateChanged += new EventHandler(dgvSelectAll_CurrentCellDirtyStateChanged);
            _Dgv.DataSourceChanged += new EventHandler(dgv_DataSourceChanged);
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;

            _Dgv.DefaultCellStyle = dataGridViewCellStyle;
            _Dgv.Columns.Clear();
            {
                DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
                col.Name = col.DataPropertyName = dgv_chk_tag;
                col.Width = 10;
                col.HeaderText = "";
                col.DisplayIndex = 0;
                col.ReadOnly = false;
                col.Frozen = true;
                col.Visible = _IsCheckBox;
                col.SortMode = DataGridViewColumnSortMode.Automatic;
                _Dgv.Columns.Add(col);
                HeadColFilter var = HeadColList.Find(a => a.ColName == dgv_chk_tag);
                if (var == null)
                {
                    HeadColList.Add(new HeadColFilter() { ColName = dgv_chk_tag, FilterStr = "" });
                    dgvFilterColHeadCell dgvheadercell = new dgvFilterColHeadCell(col.HeaderCell, ColFilterType.Check.ToString());
                    dgvheadercell.AutomaticSortingEnabled = true;
                    dgvheadercell.eventFilterEx += new dgvFilterColHeadCell.delegateFilterEx(dgvheadercell_eventFilterEx);
                    col.HeaderCell = dgvheadercell;

                }
            }
        }

        private void dgv_DataSourceChanged(object sender, EventArgs e)
        {
            if (!changesource)
                throw new Exception("请使用DgvUtil.DataSource替代DataGridView.DataSource");
        }

        private void dgvSelectAll_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _Dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        public int GetCheckedRowsQty()
        {
            int i = 0;
            try
            {
                if (_IsCheckBox && _Dgv.Columns.Contains(dgv_chk_tag))
                {
                    foreach (DataGridViewRow dgr in _Dgv.Rows)
                    {
                        if (dgr.IsNewRow)
                        {
                            continue;
                        }
                        object obj = dgr.Cells[dgv_chk_tag].Value;
                        if (obj == null || !Boolean.Parse(obj.ToString()))
                        {
                            continue;
                        }
                        i++;
                    }
                }
            }
            catch
            {
            }
            return i;

        }

        public List<DataRow> GetCheckedRows()
        {
            List<DataRow> itemlist = new List<DataRow>();
            try
            {
                if (_IsCheckBox)
                {
                    if (_Dgv.Columns.Contains(dgv_chk_tag))
                    {
                        if (dtSource != null && dtSource.Rows.Count > 0)
                        {

                            itemlist = (from DataRow dr in dtSource.Rows
                                        where (!string.IsNullOrEmpty(dr[dgv_chk_tag].ToString()) && Boolean.Parse(dr[dgv_chk_tag].ToString()))
                                        select dr).ToList();
                        }
                    }
                }
                else
                {
                    if (_Dgv.SelectedRows.Count > 0)
                    {
                        object drv = _Dgv.SelectedRows[0].DataBoundItem;
                        if (drv is DataRowView)
                        {
                            itemlist.Add(((DataRowView)drv).Row);
                        }
                    }
                }
            }
            catch
            {

            }
            return itemlist;
        }

        private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStripSet.Items[2].Visible = _IsCheckBox;
                ContextMenuStripSet.Items[3].Visible = _IsCheckBox;
                if (_IsCheckBox)
                {
                    int a = GetCheckedRowsQty();
                    if (a == 0)
                    {
                        ContextMenuStripSet.Items[2].Enabled = true;
                        ContextMenuStripSet.Items[3].Enabled = false;
                    }
                    else if (a == _Dgv.Rows.Count)
                    {
                        ContextMenuStripSet.Items[2].Enabled = false;
                        ContextMenuStripSet.Items[3].Enabled = true;
                    }
                    else
                    {
                        ContextMenuStripSet.Items[2].Enabled = true;
                        ContextMenuStripSet.Items[3].Enabled = true;
                        ContextMenuStripSet.Items[2].Visible = true;
                        ContextMenuStripSet.Items[3].Visible = true;
                    }
                }

                if (string.IsNullOrEmpty(dataSource.Filter))
                {
                    ContextMenuStripSet.Items[4].Visible = false;

                }
                else
                {
                    ContextMenuStripSet.Items[4].Visible = true;
                }


                ContextMenuStripSet.Items[0].Enabled = false;
                ContextMenuStripSet.Items[1].Enabled = false;
                _Dgv.ClearSelection();
                if (e.RowIndex >= 0)
                {
                    _Dgv.Rows[e.RowIndex].Selected = true;
                    ContextMenuStripSet.Items[1].Enabled = true;
                    if (e.ColumnIndex >= 0)
                    {
                        ContextMenuStripSet.Items[0].Enabled = true;
                        Cellstr = _Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                }

                this.ContextMenuStripSet.Show(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            }
        }

        #region ContextMenuStrip
        private void InitContextMenuStripSet()
        {
            ContextMenuStripSet = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItemCCell = new ToolStripMenuItem();
            toolStripMenuItemCCell.Text = "复制单元格(&C)";
            toolStripMenuItemCCell.Click += new EventHandler(toolStripMenuItemCCell_Click);
            ContextMenuStripSet.Items.Add(toolStripMenuItemCCell);

            ToolStripMenuItem toolStripMenuItemCLine = new ToolStripMenuItem();
            toolStripMenuItemCLine.Text = "复制选定行(&L)";
            toolStripMenuItemCLine.Click += new EventHandler(toolStripMenuItemCLine_Click);
            ContextMenuStripSet.Items.Add(toolStripMenuItemCLine);

            ToolStripMenuItem toolStripMenuItemALL = new ToolStripMenuItem();
            toolStripMenuItemALL.Text = "选择所有行(&A)";
            toolStripMenuItemALL.Click += new EventHandler(toolStripMenuItemALL_Click);
            toolStripMenuItemALL.Visible = _IsCheckBox;
            ContextMenuStripSet.Items.Add(toolStripMenuItemALL);

            ToolStripMenuItem toolStripMenuItemUnALL = new ToolStripMenuItem();
            toolStripMenuItemUnALL.Text = "取消选择行(&U)";
            toolStripMenuItemUnALL.Click += new EventHandler(toolStripMenuItemUnALL_Click);
            toolStripMenuItemUnALL.Visible = _IsCheckBox;
            ContextMenuStripSet.Items.Add(toolStripMenuItemUnALL);

            ToolStripMenuItem toolStripMenuItemUnFilter = new ToolStripMenuItem();
            toolStripMenuItemUnFilter.Text = "清空筛选(&F)";
            toolStripMenuItemUnFilter.Click += new EventHandler(toolStripMenuItemUnFilter_Click);
            ContextMenuStripSet.Items.Add(toolStripMenuItemUnFilter);

        }

        private void toolStripMenuItemUnFilter_Click(object sender, EventArgs e)
        {
            dataSource.Filter = "";
            int i = HeadColList.Count;
            for (int a = 0; a < i; a++)
            {
                HeadColList[a].FilterStr = "";
            }
        }

        private void toolStripMenuItemALL_Click(object sender, EventArgs e)
        {
            if (_IsCheckBox && _Dgv.Columns.Contains(dgv_chk_tag))
            {
                foreach (DataGridViewRow dr in _Dgv.Rows)
                {
                    if (dr.IsNewRow)
                    {
                        continue;
                    }
                    dr.Cells[dgv_chk_tag].Value = true;
                }
            }
        }

        private void toolStripMenuItemUnALL_Click(object sender, EventArgs e)
        {
            if (_IsCheckBox && _Dgv.Columns.Contains(dgv_chk_tag))
            {
                foreach (DataGridViewRow dr in _Dgv.Rows)
                {
                    if (dr.IsNewRow)
                    {
                        continue;
                    }
                    dr.Cells[dgv_chk_tag].Value = false;
                }
            }
        }

        private void toolStripMenuItemCLine_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(_Dgv.GetClipboardContent());
        }

        private void toolStripMenuItemCCell_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(Cellstr);
        }
        #endregion

    }


    public enum ColFilterType
    {
        Check, list
    }
    public class HeadColFilter
    {
        public string ColName;
        public string FilterStr;

    }
}
