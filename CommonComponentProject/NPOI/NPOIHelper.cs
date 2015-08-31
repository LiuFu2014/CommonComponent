using System;
using System.Data;
using System.Web;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.Windows.Forms;

namespace Common
{
    public static class NpoiHelper
    {
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="Dt">要导出的DataTable</param>
        /// <param name="ExelName">Excel名称</param>
        public static void ExportForm(DataTable Dt, string ExelName)
        {
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = ExelName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;

            if (saveFileName.IndexOf(":") < 0) return; //被点了取消

            using (MemoryStream ms = Export(Dt, ExelName))
            {
                using (var fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataGridView导出到Excel文件
        /// </summary>
        /// <param name="ExportGridView">要导出的DataGridView</param>
        /// <param name="ExelName">要保存的名称</param>
        public static void ExportForm(DataGridView ExportGridView, string ExelName)
        {
            DataTable dtSource = (DataTable)((BindingSource)ExportGridView.DataSource).DataSource;
            if (dtSource == null) return;

            DataTable Dt = dtSource.Copy();
            for (int i = 0; i < Dt.Columns.Count; i++)
            {
                if (ExportGridView.Columns[Dt.Columns[i].ColumnName] != null)
                    Dt.Columns[i].ColumnName = ExportGridView.Columns[Dt.Columns[i].ColumnName].HeaderText;  
            }
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xls";
            saveDialog.Filter = "Excel文件|*.xls";
            saveDialog.FileName = ExelName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;

            if (saveFileName.IndexOf(":") < 0) return; //被点了取消

            using (MemoryStream ms = Export(Dt, ExelName))
            {
                using (var fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            var dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            var format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            dateStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            dateStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            dateStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            dateStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            dateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            //单元格样式
            var cellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;


            //取得列宽
            var arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
            }
            for (var i = 0; i < dtSource.Rows.Count; i++)
            {
                for (var j = 0; j < dtSource.Columns.Count; j++)
                {
                    var intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            var rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        var headerRow = (HSSFRow)sheet.CreateRow(0);
                        headerRow.HeightInPoints = 20;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        var headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            
                        var font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 16;
                        font.Boldweight = 600;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                   
                        // headerRow.Dispose();
                 
                    }
                    #endregion


                    #region 列头及样式
                    {
                        var headerRow = (HSSFRow)sheet.CreateRow(1);
                        var headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        var font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }

                        //headerRow.Dispose();
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion


                #region 填充内容
                var dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
               
                foreach (DataColumn column in dtSource.Columns)
                {

                    var newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);
                    newCell.CellStyle = cellStyle;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示


                            break;
                        case "System.Boolean"://布尔型
                            var boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            var intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }
            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;

                //sheet
                //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet
                return ms;
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(Export(dtSource, strHeaderText).GetBuffer());
            curContext.Response.End();
        }


        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable Import(string strFileName)
        {
            var dt = new DataTable();
            HSSFWorkbook hssfworkbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }
            var sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            var rows = sheet.GetRowEnumerator();

            var headerRow = (HSSFRow)sheet.GetRow(1);
            var cellCount = headerRow.LastCellNum;

            for (var j = 0; j < cellCount; j++)
            {
                var cell = (HSSFCell)headerRow.GetCell(j);
                if(cell!=null)
                {
                    dt.Columns.Add(cell.ToString());
                }
            }

            for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = (HSSFRow)sheet.GetRow(i);
                var dataRow = dt.NewRow();

                for (var j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                dt.Rows.Add(dataRow);
            }
            return dt;
        }
    }


}
