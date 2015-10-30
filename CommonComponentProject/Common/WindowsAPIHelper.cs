using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [StructLayout(LayoutKind.Sequential)]
    class SystemTime
    {
        public ushort year;
        public ushort month;
        public ushort dayofweek;
        public ushort day;
        public ushort hour;
        public ushort minute;
        public ushort second;
        public ushort milliseconds;
    }

    /// <summary>
    /// 用于调用WindowsAPI帮助类，持续不断的更新中...
    /// </summary>
    class WindowsAPIHelper
    {
        #region 函数声明

        [DllImport("Kernel32.dll")]
        private static extern Boolean SetSystemTime([In, Out] SystemTime st);

        [DllImport("shell32.DLL", EntryPoint = "ExtractAssociatedIcon")]
        private static extern int ExtractAssociatedIconA(int hInst, string lpIconPath, ref int lpiIcon); //声明函数  

        System.IntPtr thisHandle;

        //获取注册表中的启动位置  
        RegistryKey RKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        #endregion

        #region 函数体

        /// <summary>
        /// 设置系统时间
        /// </summary>
        /// <param name="newdatetime">新时间</param>
        /// <returns></returns>
        public static bool SetSysTime(DateTime newdatetime)
        {
            SystemTime st = new SystemTime();
            st.year = Convert.ToUInt16(newdatetime.Year);
            st.month = Convert.ToUInt16(newdatetime.Month);
            st.day = Convert.ToUInt16(newdatetime.Day);
            st.dayofweek = Convert.ToUInt16(newdatetime.DayOfWeek);
            st.hour = Convert.ToUInt16(newdatetime.Hour - TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime(2001, 09, 01)).Hours);
            st.minute = Convert.ToUInt16(newdatetime.Minute);
            st.second = Convert.ToUInt16(newdatetime.Second);
            st.milliseconds = Convert.ToUInt16(newdatetime.Millisecond);
            return SetSystemTime(st);
        }

        /// <summary>
        /// filePath是要获取文件路径，返回ico格式文件  
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public System.Drawing.Icon GetIco(string filePath)
        {
            int RefInt = 0;
            thisHandle = new IntPtr(ExtractAssociatedIconA(0, filePath, ref RefInt));
            return System.Drawing.Icon.FromHandle(thisHandle);
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <returns></returns>
        public string GetComputorInformation()
        {

            StringBuilder mStringBuilder = new StringBuilder();
            DriveInfo[] myAllDrivers = DriveInfo.GetDrives();
            try
            {
                foreach (DriveInfo myDrive in myAllDrivers)
                {
                    if (myDrive.IsReady)
                    {
                        mStringBuilder.Append("磁盘驱动器盘符：");
                        mStringBuilder.AppendLine(myDrive.Name);
                        mStringBuilder.Append("磁盘卷标：");
                        mStringBuilder.AppendLine(myDrive.VolumeLabel);
                        mStringBuilder.Append("磁盘类型：");
                        mStringBuilder.AppendLine(myDrive.DriveType.ToString());
                        mStringBuilder.Append("磁盘格式：");
                        mStringBuilder.AppendLine(myDrive.DriveFormat);
                        mStringBuilder.Append("磁盘大小：");
                        decimal resultmyDrive = Math.Round((decimal)myDrive.TotalSize / 1024 / 1024 / 1024, 2);
                        mStringBuilder.AppendLine(resultmyDrive + "GB");
                        mStringBuilder.Append("剩余空间：");
                        decimal resultAvailableFreeSpace = Math.Round((decimal)myDrive.AvailableFreeSpace / 1024 / 1024 / 1024, 2);
                        mStringBuilder.AppendLine(resultAvailableFreeSpace + "GB");
                        mStringBuilder.Append("总剩余空间（含磁盘配额）：");
                        decimal resultTotalFreeSpace = Math.Round((decimal)myDrive.TotalFreeSpace / 1024 / 1024 / 1024, 2);
                        mStringBuilder.AppendLine(resultTotalFreeSpace + "GB");
                        mStringBuilder.AppendLine("-------------------------------------");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return mStringBuilder.ToString();
        }

        ///<summary>
        /// 设置开机启动  
        ///</summary>
        ///<param name="path"/>
        public void StartRunApp(string path)
        {
            string strnewName = path.Substring(path.LastIndexOf("\\") + 1);//要写入注册表的键值名称  
            if (!File.Exists(path))//判断指定的文件是否存在  
                return;
            if (RKey == null)
            {
                RKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            }
            RKey.SetValue(strnewName, path);//通过修改注册表，使程序在开机时自动运行  
        }

        ///<summary>
        /// 取消开机启动  
        ///</summary>
        ///<param name="path"/>
        public void ForbitStartRun(string path)
        {
            string strnewName = path.Substring(path.LastIndexOf("\\") + 1);//要写入注册表的键值名称  
            RKey.DeleteValue(strnewName, false);//通过修改注册表，取消程序在开机时自动运行  
        } 

        #endregion

    }
}
