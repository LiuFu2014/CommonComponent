﻿using FastReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FastReportHelper
{
    /// <summary>
    /// 未完善，这里记录基本用法
    /// </summary>
    public class FastReportHelper
    {
        static Report report = new Report();
        public static void ReportShow(string filePath)
        {
            report.Load(filePath);
            report.Prepare();
            //显示完整查看窗体
            report.Show();
        }

        public  static void ReportPrint(string filePath)
        {
            report.Load(filePath);
            //是否弹出弹框
            report.PrintSettings.ShowDialog = false;
            report.Print();
        }
        
    }
}
