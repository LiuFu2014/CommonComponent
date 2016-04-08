using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCAutomation;


namespace OPCHelper
{
    public class OPCHelp
    {
        string PCIP;
        public OPCServer s7 = new OPCServer();
        public OPCGroup s7Group;

        /// <summary>
        /// PC站IP地址
        /// </summary>
        /// <param name="OPCIP"></param>
        public OPCHelp(string OPCIP)
        {
            PCIP = OPCIP;
        }

        /// <summary>
        /// 打开OPC连接
        /// </summary>
        /// <returns></returns>
        public bool OpenConn() 
        {
            try
            {
                s7.Connect("OPC.SimaticNET", PCIP);
                if (s7.ServerState == (int)OPCServerState.OPCRunning)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public bool CloseConn()
        {
            try
            {
                s7.Disconnect();
                if (s7.ServerState == (int)OPCServerState.OPCDisconnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取OPCserver当前状态，TRUE：正在运行，false：连接关闭
        /// </summary>
        /// <returns></returns>
        public bool GetConnStatus()
        {
            return s7.ServerState ==(int) OPCServerState.OPCRunning ? true : false;
        }

        /// <summary>
        /// 创建组
        /// </summary>
        public bool CreateGroup(string name)
        {
            try
            {
                s7Group = s7.OPCGroups.Add(name);
                SetGroupProperty();
            }
            catch (Exception)
            {
                //MessageBox.Show("创建组出现错误：" + err.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置组属性
        /// </summary>
        private void SetGroupProperty()
        {
            s7.OPCGroups.DefaultGroupIsActive = Convert.ToBoolean(true);
            s7.OPCGroups.DefaultGroupDeadband = Convert.ToInt32(0);
            s7Group.UpdateRate = Convert.ToInt32(250);
            s7Group.IsActive = Convert.ToBoolean(true);
            s7Group.IsSubscribed = Convert.ToBoolean(false);
        }

        /// <summary>
        /// 添加项目地址
        /// </summary>
        public int AddAddr(string Addr, int ClientHandle)
        {
            int returnvalue = 0;
            if (!string.IsNullOrEmpty(Addr))
            {
                try
                {
                    OPCItem tempItem = s7Group.OPCItems.AddItem(Addr, ClientHandle);

                    //ServerHandle,这是一个重要的东西，个人理解为组中的项的索引，读取程序根据这个索引找到db块并进行读写操作。
                    //程序中应对ServerHandle与实际的变量地址已经这个变量地址所代表的意义进行一个映射。
                    returnvalue = tempItem.ServerHandle;
                }
                catch (Exception err)
                {

                }
            }
            return returnvalue;
        }

        /// <summary>
        /// 读取数据,成功返回值，失败返回null
        /// </summary>
        public Array ReadData(int[] handle)
        {
            try
            {
                int count = handle.Length;
                int[] temp = new int[count + 1];
                temp[0] = 0;
                for (int i = 1; i < temp.Length; i++)
                {
                    temp[i] = handle[i - 1];
                }
                Array serverHandles = (Array)temp;
                Array values;
                Array Errors;
                object Qualities;
                object TimeStamps;
                //OPCAutomation.OPCDataSource.OPCCache;
                s7Group.SyncRead(1, count, ref serverHandles, out values, out Errors, out Qualities, out TimeStamps);
                //对于读取，不会报异常，如果对应的errors的值非0，则这个值读取失败；在这里可以做这个处理，只要有读取失败的，就整个读取失败。
                for (int i = 0; i < Errors.Length; i++)
                {
                    if (Errors.GetValue(i).ToString()!="0")
                    {
                        return null;
                    }
                }
                return values;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        /// <summary>
        /// 写入数据,handle,索引值数组,value对应值数组
        /// </summary>
        public bool WriteData(int[] handle, int[] value)
        {
            try
            {
                int[] temp = new int[handle.Length + 1];
                temp[0] = 0;
                object[] temp1 = new object[handle.Length + 1];
                temp1[0] = "";
                for (int i = 1; i < temp.Length; i++)
                {
                    temp[i] = handle[i - 1];
                    temp1[i] = value[i - 1];
                }
                Array serverHandles = (Array)temp;
                Array values = (Array)temp1;
                Array Errors;
                //OPCAutomation.OPCDataSource.OPCCache;
                s7Group.SyncWrite(handle.Length, ref serverHandles, ref values, out Errors);
                //写入方法不会抛出异常，errors对应项返回0表示写入成功，返回非0表示写入失败；但是如果多值写入，是否会出现部分写入成功，部分写入失败的结果呢？
                //如果单单检测到写入失败的值，就返回false，则可能会造成流程上的错误。
                for (int i = 0; i < Errors.Length; i++)
                {
                    if (Errors.GetValue(i).ToString()!="0")
                    {
                        //表示当前handle指代地址写入错误
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #region 操作相关


        ///// <summary>
        ///// 刷新PLC地址(所有,存储到数据库)
        ///// </summary>
        //private bool UpdateAllPLCAddr()
        //{
        //    //地址初始且OPC运行
        //    if (list_PLCAddr != null || S7.GetConnStatus())
        //    {
        //        Array temp_Array = S7.ReadData(serverHandle);
        //        if (temp_Array == null)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            try
        //            {
        //                HKDBEntities db = new HKDBEntities();
        //                var temp = db.T_PLCAddress.ToList();
        //                for (int i = 0; i < list_PLCAddr.Count; i++)
        //                {
        //                    //db.Attach(list_PLCAddr[i]);
        //                    list_PLCAddr[i].Data = temp_Array.GetValue(i + 1).ToString();
        //                    temp[i].Data = list_PLCAddr[i].Data;
        //                }
        //                db.SaveChanges();
        //                db.Dispose();
        //                return true;
        //            }
        //            catch (Exception)
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 打开PLC连接
        ///// </summary>
        ///// <returns></returns>
        //private bool OpenPLCConn()
        //{
        //    S7 = new OPCHelp(PLCIP);
        //    if (S7.OpenConn())
        //    {
        //        if (S7.CreateGroup("HK"))
        //        {
        //            serverHandle = new int[list_PLCAddr.Count];
        //            for (int i = 0; i < list_PLCAddr.Count; i++)
        //            {
        //                serverHandle[i] = S7.AddAddr("S7:[S7 connection_1]," + list_PLCAddr[i].Address, 1);
        //                list_PLCAddr[i].ServerHandle = serverHandle[i];
        //            }
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 关闭PLC连接
        ///// </summary>
        ///// <returns></returns>
        //private bool ClosePLCConn()
        //{
        //    return S7.CloseConn() ? true : false;
        //}

        ///// <summary>
        ///// 根据指定的PLC地址写入制定的值
        ///// </summary>
        //private bool WritePLCDataByAddr(int[] handles, int[] values)
        //{
        //    if (S7.GetConnStatus())
        //    {
        //        return S7.WriteData(handles, values) ? true : false;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// 单值写入
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //private bool WriteSingleData(int id, int value)
        //{
        //    int[] handle = new int[1];
        //    handle[0] = (int)list_PLCAddr.First(p => p.ID == id).ServerHandle;
        //    int[] values = new int[1];
        //    values[0] = value;
        //    if (WritePLCDataByAddr(handle, values))
        //    {
        //        //..
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        #endregion
    }
}
