using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace Common
{
    public class dgvFilterColHeadCell : DataGridViewColumnHeaderCell
    {
        #region Constructors
        public dgvFilterColHeadCell(DataGridViewColumnHeaderCell oldHeaderCell, string _FilterType)
        {
            this.ContextMenuStrip = oldHeaderCell.ContextMenuStrip;
            this.ErrorText = oldHeaderCell.ErrorText;
            this.Tag = oldHeaderCell.Tag;
            this.ToolTipText = oldHeaderCell.ToolTipText;
            this.Value = oldHeaderCell.Value;
            this.ValueType = oldHeaderCell.ValueType;
            FilterType = _FilterType;

            if (oldHeaderCell.HasStyle)
            {
                this.Style = oldHeaderCell.Style;
            }
            ColumnDataName = oldHeaderCell.DataGridView.Columns[oldHeaderCell.ColumnIndex].DataPropertyName;
            ColumnName = oldHeaderCell.DataGridView.Columns[oldHeaderCell.ColumnIndex].Name;
            dgvFilterColHeadCell filterCell =
                oldHeaderCell as dgvFilterColHeadCell;
            if (filterCell != null)
            {
                this.FilteringEnabled = filterCell.FilteringEnabled;
                this.AutomaticSortingEnabled = filterCell.AutomaticSortingEnabled;
                this.DropDownListBoxMaxLines = filterCell.DropDownListBoxMaxLines;
                this.currentDropDownButtonPaddingOffset =
                    filterCell.currentDropDownButtonPaddingOffset;
                this.ucFilterTop = filterCell.ucFilterTop;
                this.eventFilterEx = filterCell.eventFilterEx;
            }
            if (_FilterType == ColFilterType.Check.ToString())
            {
                ucFilterTop = new UCCheckSelect();
            }
            else
            {
                ucFilterTop = new UCListSelect();
            }
            this.ucFilterTop.ColDataName = ColumnDataName;

        }


        public dgvFilterColHeadCell()
        {

        }

        public override object Clone()
        {
            return new dgvFilterColHeadCell(this, FilterType);
        }

        #endregion

        #region Fields

        private Rectangle dropDownButtonBoundsValue = Rectangle.Empty;

        private Int32 currentDropDownButtonPaddingOffset;

        private String FilterStr = String.Empty;

        private Boolean dropDownListBoxShowing = false;

        private Boolean lostFocusOnDropDownButtonClick = false;

        private String ColumnDataName = string.Empty;

        private String ColumnName = string.Empty;

        private String FilterType = string.Empty;

        private Boolean datasourcechange = true;
        
        private UCFilterTop ucFilterTop = null;


        #endregion


        #region Properties

        #region 是否启用筛选
        private Boolean filteringEnabledValue = true;
        [DefaultValue(true)]
        public Boolean FilteringEnabled
        {
            get
            {
                if (this.DataGridView == null ||
                    this.DataGridView.DataSource == null)
                {
                    return filteringEnabledValue;
                }

                BindingSource data = this.DataGridView.DataSource as BindingSource;
                Debug.Assert(data != null);
                return filteringEnabledValue && data.SupportsFiltering;
            }
            set
            {
                if (!value)
                {
                    AdjustPadding(0);
                    InvalidateDropDownButtonBounds();
                }

                filteringEnabledValue = value;
            }
        }
        #endregion


        #region 是否自动排序
        private Boolean automaticSortingEnabledValue = true;
        [DefaultValue(true)]
        public Boolean AutomaticSortingEnabled
        {
            get
            {
                return automaticSortingEnabledValue;
            }
            set
            {
                automaticSortingEnabledValue = value;
                if (OwningColumn != null)
                {
                    if (value)
                    {
                        OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
                    }
                    else
                    {
                        OwningColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
            }
        }
        #endregion

        #region 下拉列表最大显示行
        [DefaultValue(1000)]
        private Int32 dropDownListBoxMaxLinesValue = 1000;
        public Int32 DropDownListBoxMaxLines
        {
            get { return dropDownListBoxMaxLinesValue; }
            set { dropDownListBoxMaxLinesValue = value; }
        }
        #endregion

        #endregion


        #region protected
        protected override void OnDataGridViewChanged()
        {
            if (this.DataGridView == null)
            {
                return;
            }

            if (OwningColumn != null)
            {
                if (OwningColumn is DataGridViewImageColumn ||
                (OwningColumn is DataGridViewButtonColumn &&
                ((DataGridViewButtonColumn)OwningColumn).UseColumnTextForButtonValue) ||
                (OwningColumn is DataGridViewLinkColumn &&
                ((DataGridViewLinkColumn)OwningColumn).UseColumnTextForLinkValue))
                {
                    AutomaticSortingEnabled = false;
                    FilteringEnabled = false;
                }
                if (OwningColumn.SortMode == DataGridViewColumnSortMode.Automatic)
                {
                    OwningColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
            }

            VerifyDataSource();

            HandleDataGridViewEvents();

            SetDropDownButtonBounds();

            base.OnDataGridViewChanged();
        }
        private void VerifyDataSource()
        {
            // Continue only if there is a DataGridView and its DataSource has been set.
            if (this.DataGridView == null || this.DataGridView.DataSource == null)
            {
                return;
            }

            // Throw an exception if the data source is not a BindingSource. 
            BindingSource data = this.DataGridView.DataSource as BindingSource;
            if (data == null)
            {
                throw new NotSupportedException(
                    "The DataSource property of the containing DataGridView control " +
                    "must be set to a BindingSource.");
            }
        }

        private void HandleDataGridViewEvents()
        {
            this.DataGridView.Scroll += new ScrollEventHandler(DataGridView_Scroll);
            this.DataGridView.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnDisplayIndexChanged);
            this.DataGridView.ColumnWidthChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnWidthChanged);
            this.DataGridView.ColumnHeadersHeightChanged += new EventHandler(DataGridView_ColumnHeadersHeightChanged);
            this.DataGridView.SizeChanged += new EventHandler(DataGridView_SizeChanged);
            this.DataGridView.DataSourceChanged += new EventHandler(DataGridView_DataSourceChanged);
            this.DataGridView.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGridView_DataBindingComplete);
            this.DataGridView.ColumnSortModeChanged += new DataGridViewColumnEventHandler(DataGridView_ColumnSortModeChanged);

        }

        private void SetDropDownButtonBounds()
        {

            Rectangle cellBounds =
                this.DataGridView.GetCellDisplayRectangle(
                this.ColumnIndex, -1, false);

            Int32 buttonEdgeLength = this.InheritedStyle.Font.Height + 5;


            Rectangle borderRect = BorderWidths(
                this.DataGridView.AdjustColumnHeaderBorderStyle(
                this.DataGridView.AdvancedColumnHeadersBorderStyle,
                new DataGridViewAdvancedBorderStyle(), false, false));
            Int32 borderAndPaddingHeight = 2 +
                borderRect.Top + borderRect.Height +
                this.InheritedStyle.Padding.Vertical;
            Boolean visualStylesEnabled =
                Application.RenderWithVisualStyles &&
                this.DataGridView.EnableHeadersVisualStyles;
            if (visualStylesEnabled)
            {
                borderAndPaddingHeight += 3;
            }

            // Constrain the button edge length to the height of the 
            // column headers minus the border and padding height. 
            if (buttonEdgeLength >
                this.DataGridView.ColumnHeadersHeight -
                borderAndPaddingHeight)
            {
                buttonEdgeLength =
                    this.DataGridView.ColumnHeadersHeight -
                    borderAndPaddingHeight;
            }

            // Constrain the button edge length to the
            // width of the cell minus three.
            if (buttonEdgeLength > cellBounds.Width - 3)
            {
                buttonEdgeLength = cellBounds.Width - 3;
            }

            // Calculate the location of the drop-down button, with adjustments
            // based on whether visual styles are enabled. 
            Int32 topOffset = visualStylesEnabled ? 4 : 1;
            Int32 top = cellBounds.Bottom - buttonEdgeLength - topOffset;
            Int32 leftOffset = visualStylesEnabled ? 3 : 1;
            Int32 left = 0;
            if (this.DataGridView.RightToLeft == RightToLeft.No)
            {
                left = cellBounds.Right - buttonEdgeLength - leftOffset;
            }
            else
            {
                left = cellBounds.Left + leftOffset;
            }

            // Set the dropDownButtonBoundsValue value using the calculated 
            // values, and adjust the cell padding accordingly.  
            dropDownButtonBoundsValue = new Rectangle(left, top,
                buttonEdgeLength, buttonEdgeLength);
            AdjustPadding(buttonEdgeLength + leftOffset);
        }

        protected Rectangle DropDownButtonBounds
        {
            get
            {
                if (!FilteringEnabled)
                {
                    return Rectangle.Empty;
                }
                if (dropDownButtonBoundsValue == Rectangle.Empty)
                {
                    SetDropDownButtonBounds();
                }
                return dropDownButtonBoundsValue;
            }
        }



        private bool CheckFiltered()
        {
            BindingSource source = this.DataGridView.DataSource as BindingSource;
            if (source == null || String.IsNullOrEmpty(source.Filter) )
            {
                return false;
            }
            return String.IsNullOrEmpty(FilterStr);
        }

        protected override void Paint(
          Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
          int rowIndex, DataGridViewElementStates cellState,
          object value, object formattedValue, string errorText,
          DataGridViewCellStyle cellStyle,
          DataGridViewAdvancedBorderStyle advancedBorderStyle,
          DataGridViewPaintParts paintParts)
        {
            // Use the base method to paint the default appearance. 
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                cellState, value, formattedValue,
                errorText, cellStyle, advancedBorderStyle, paintParts);

            // Continue only if filtering is enabled and ContentBackground is 
            // part of the paint request. 
            if (!FilteringEnabled ||
                (paintParts & DataGridViewPaintParts.ContentBackground) == 0)
            {
                return;
            }

            // Retrieve the current button bounds. 
            Rectangle buttonBounds = DropDownButtonBounds;

            // Continue only if the buttonBounds is big enough to draw.
            if (buttonBounds.Width < 1 || buttonBounds.Height < 1) return;

            // Paint the button manually or using visual styles if visual styles 
            // are enabled, using the correct state depending on whether the 
            // filter list is showing and whether there is a filter in effect 
            // for the current column. 
            if (Application.RenderWithVisualStyles)
            {
                ComboBoxState state = ComboBoxState.Normal;
                if (dropDownListBoxShowing)
                {
                    state = ComboBoxState.Pressed;

                }
                else if (CheckFiltered())
                {
                    state = ComboBoxState.Hot;

                }
                ComboBoxRenderer.DrawDropDownButton(
                    graphics, buttonBounds, state);
            }
            else
            {
                // Determine the pressed state in order to paint the button 
                // correctly and to offset the down arrow. 
                Int32 pressedOffset = 0;
                PushButtonState state = PushButtonState.Normal;
                if (dropDownListBoxShowing)
                {
                    state = PushButtonState.Pressed;
                    pressedOffset = 1;
                }
                ButtonRenderer.DrawButton(graphics, buttonBounds, state);

                // If there is a filter in effect for the column, paint the 
                // down arrow as an unfilled triangle. If there is no filter 
                // in effect, paint the down arrow as a filled triangle.
                if (CheckFiltered())
                {
                    graphics.DrawPolygon(SystemPens.ControlText, new Point[] {
                        new Point(
                            buttonBounds.Width / 2 + 
                                buttonBounds.Left - 1 + pressedOffset, 
                            buttonBounds.Height * 3 / 4 + 
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width / 4 + 
                                buttonBounds.Left + pressedOffset,
                            buttonBounds.Height / 2 + 
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width * 3 / 4 + 
                                buttonBounds.Left - 1 + pressedOffset,
                            buttonBounds.Height / 2 + 
                                buttonBounds.Top - 1 + pressedOffset)
                    });
                }
                else
                {
                    graphics.FillPolygon(SystemBrushes.ControlText, new Point[] {
                        new Point(
                            buttonBounds.Width / 2 + 
                                buttonBounds.Left - 1 + pressedOffset, 
                            buttonBounds.Height * 3 / 4 + 
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width / 4 + 
                                buttonBounds.Left + pressedOffset,
                            buttonBounds.Height / 2 + 
                                buttonBounds.Top - 1 + pressedOffset),
                        new Point(
                            buttonBounds.Width * 3 / 4 + 
                                buttonBounds.Left - 1 + pressedOffset,
                            buttonBounds.Height / 2 + 
                                buttonBounds.Top - 1 + pressedOffset)
                    });
                }
            }

        }


        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            Debug.Assert(this.DataGridView != null, "DataGridView is null");

            if (lostFocusOnDropDownButtonClick)
            {
                lostFocusOnDropDownButtonClick = false;
                return;
            }

            Rectangle cellBounds = this.DataGridView
                .GetCellDisplayRectangle(e.ColumnIndex, -1, false);

            if (this.OwningColumn.Resizable == DataGridViewTriState.True &&
                ((this.DataGridView.RightToLeft == RightToLeft.No &&
                cellBounds.Width - e.X < 6) || e.X < 6))
            {
                return;
            }
            Int32 scrollingOffset = 0;
            if (this.DataGridView.RightToLeft == RightToLeft.No &&
                this.DataGridView.FirstDisplayedScrollingColumnIndex ==
                this.ColumnIndex)
            {
                scrollingOffset =
                    this.DataGridView.FirstDisplayedScrollingColumnHiddenWidth;
            }



            if (FilteringEnabled &&
                DropDownButtonBounds.Contains(
                e.X + cellBounds.Left - scrollingOffset, e.Y + cellBounds.Top))
            {

                if (this.DataGridView.IsCurrentCellInEditMode)
                {
                    // Commit and end the cell edit.  
                    this.DataGridView.EndEdit();

                    // Commit any change to the underlying data source. 
                    BindingSource source =
                        this.DataGridView.DataSource as BindingSource;
                    if (source != null)
                    {
                        source.EndEdit();
                    }
                }
                ShowDropDownList();

            }
            else if (AutomaticSortingEnabled &&
                this.DataGridView.SelectionMode !=
                DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                SortByColumn();
            }
            base.OnMouseDown(e);

        }

        #endregion

        #region private

        private void SortByColumn()
        {
            Debug.Assert(this.DataGridView != null && OwningColumn != null, "DataGridView or OwningColumn is null");

            // Continue only if the data source supports sorting. 
            IBindingList sortList = this.DataGridView.DataSource as IBindingList;
            if (sortList == null ||
                !sortList.SupportsSorting ||
                !AutomaticSortingEnabled)
            {
                return;
            }

            // Determine the sort direction and sort by the owning column. 
            ListSortDirection direction = ListSortDirection.Ascending;
            if (this.DataGridView.SortedColumn == OwningColumn &&
                this.DataGridView.SortOrder == SortOrder.Ascending)
            {
                direction = ListSortDirection.Descending;
            }
            this.DataGridView.Sort(OwningColumn, direction);
        }

        private void AdjustPadding(Int32 newDropDownButtonPaddingOffset)
        {
            // Determine the difference between the new and current 
            // padding adjustment.
            Int32 widthChange = newDropDownButtonPaddingOffset -
                currentDropDownButtonPaddingOffset;

            // If the padding needs to change, store the new value and 
            // make the change.
            if (widthChange != 0)
            {
                // Store the offset for the drop-down button separately from 
                // the padding in case the client needs additional padding.
                currentDropDownButtonPaddingOffset =
                    newDropDownButtonPaddingOffset;

                // Create a new Padding using the adjustment amount, then add it
                // to the cell's existing Style.Padding property value. 
                Padding dropDownPadding = new Padding(0, 0, widthChange, 0);
                this.Style.Padding = Padding.Add(
                    this.InheritedStyle.Padding, dropDownPadding);
            }
        }

        private void InvalidateDropDownButtonBounds()
        {
            if (!dropDownButtonBoundsValue.IsEmpty)
            {
                dropDownButtonBoundsValue = Rectangle.Empty;
            }
        }




        #region DataGridView events

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                ResetDropDown();
            }
        }
        private void DataGridView_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetDropDown();
        }

        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            ResetDropDown();
        }

        private void DataGridView_ColumnHeadersHeightChanged(object sender, EventArgs e)
        {
            ResetDropDown();
        }

        private void DataGridView_SizeChanged(object sender, EventArgs e)
        {
            ResetDropDown();
        }


        private void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                ResetDropDown();
                ResetFilter();
            }
        }

  
        private void DataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            datasourcechange = true;
            VerifyDataSource();
            ResetDropDown();
            ResetFilter();
        }


        private void ResetDropDown()
        {
            InvalidateDropDownButtonBounds();
            if (dropDownListBoxShowing)
            {
                HideDropDownList();
            }
        }

        /// <summary>
        /// Resets the cached filter values if the filter has been removed.
        /// </summary>
        private void ResetFilter()
        {
            if (this.DataGridView == null) return;
            BindingSource source = this.DataGridView.DataSource as BindingSource;
            if (source == null || String.IsNullOrEmpty(source.Filter) )
            {
                FilterStr = string.Empty;
            }
        }

        private void DataGridView_ColumnSortModeChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column == OwningColumn &&
                e.Column.SortMode == DataGridViewColumnSortMode.Automatic)
            {
                throw new InvalidOperationException(
                    "A SortMode value of Automatic is incompatible with " +
                    "the DataGridViewAutoFilterColumnHeaderCell type. " +
                    "Use the AutomaticSortingEnabled property instead.");
            }
        }

        #endregion DataGridView events



        #endregion

        private void SetDropDownListBoxBounds()
        {



            Int32 dropDownListBoxHeight = ucFilterTop.Height;
            Int32 dropDownListBoxWidth = ucFilterTop.Width;
            Int32 dropDownListBoxLeft = 0;

            if (this.DataGridView.RightToLeft == RightToLeft.No)
            {
                dropDownListBoxLeft = DropDownButtonBounds.Right -
                    dropDownListBoxWidth + 1;
            }
            else
            {
                dropDownListBoxLeft = DropDownButtonBounds.Left - 1;
            }


            Int32 clientLeft = 1;
            Int32 clientRight = this.DataGridView.ClientRectangle.Right;
            if (this.DataGridView.DisplayedRowCount(false) <
                this.DataGridView.RowCount)
            {
                if (this.DataGridView.RightToLeft == RightToLeft.Yes)
                {
                    clientLeft += SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    clientRight -= SystemInformation.VerticalScrollBarWidth;
                }
            }

            // Adjust the dropDownListBox location and/or width if it would
            // otherwise overlap the left or right edge of the DataGridView.
            if (dropDownListBoxLeft < clientLeft)
            {
                dropDownListBoxLeft = clientLeft;
            }
            Int32 dropDownListBoxRight =
                dropDownListBoxLeft + dropDownListBoxWidth + 1;
            if (dropDownListBoxRight > clientRight)
            {
                if (dropDownListBoxLeft == clientLeft)
                {
                    dropDownListBoxWidth -=
                        dropDownListBoxRight - clientRight;
                }
                else
                {
                    dropDownListBoxLeft -=
                        dropDownListBoxRight - clientRight;
                    if (dropDownListBoxLeft < clientLeft)
                    {
                        dropDownListBoxWidth -= clientLeft - dropDownListBoxLeft;
                        dropDownListBoxLeft = clientLeft;
                    }
                }
            }

            // Set the ListBox.Bounds property using the calculated values. 
            ucFilterTop.Bounds = new Rectangle(dropDownListBoxLeft,
                DropDownButtonBounds.Bottom + 5, // top of drop-down list box
                dropDownListBoxWidth, dropDownListBoxHeight);
        }

        public void ShowDropDownList()
        {
            if (this.DataGridView == null)
            {
                return;
            }
            if (this.DataGridView.CurrentRow != null &&
                this.DataGridView.CurrentRow.IsNewRow)
            {
                this.DataGridView.CurrentCell = null;
            }
            if (ColFilterType.list.ToString() == FilterType)
            {
                if (datasourcechange)
                {
                    datasourcechange = false;
                    BindingSource bs = this.DataGridView.DataSource as BindingSource;
                    if (bs == null)
                    {
                        return;
                    }
                    DataTable dt = bs.DataSource as DataTable;
                    if (dt == null || dt.Columns.Count == 0 || !dt.Columns.Contains(ColumnDataName))
                    {
                        return;
                    }
                    var stringList1 = from DataRow item in dt.Rows
                                      where !string.IsNullOrEmpty(item[ColumnDataName].ToString())
                                     select   item[ColumnDataName].ToString();
                    var q1 = stringList1.Distinct();
                    string[] pp = q1.ToArray();
                    this.ucFilterTop.Setlistdatasource(pp);

                }
            }
            SetDropDownListBoxBounds();
            this.DataGridView.Controls.Add(ucFilterTop);
            this.ucFilterTop.Visible = true;
            dropDownListBoxShowing = true;
            this.ucFilterTop.eventLostFocus += new UCCheckSelect.delegateLostFocus(SelectLostFocus);
            ucFilterTop.SetFocusEx(FilterStr);
            this.DataGridView.InvalidateCell(this);
        }
        private void SelectLostFocus(bool IsFilter, string Filters)
        {
            if (DropDownButtonBounds.Contains(
               this.DataGridView.PointToClient(new Point(
             System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y))))
            {
                lostFocusOnDropDownButtonClick = true;
            }

            HideDropDownList();
            if (IsFilter)
            {
                FilterExExcute(Filters, ColumnName);
            }
        }

        public void HideDropDownList()
        {
            if (this.DataGridView == null)
            {
                return;
            }
             
            this.ucFilterTop .Visible = false;
            dropDownListBoxShowing = false;
            this.ucFilterTop.eventLostFocus -= new UCCheckSelect.delegateLostFocus(SelectLostFocus);
            this.DataGridView.Controls.Remove(ucFilterTop);
            this.DataGridView.InvalidateCell(this);
        }


        public delegate void delegateFilterEx( string Filterstr, string ColName);
        public event delegateFilterEx eventFilterEx;
        void FilterExExcute( string Filterstr, string ColName)
        {
            if (eventFilterEx != null)
            {
                eventFilterEx(Filterstr, ColName);
            }
        }


    }
}
