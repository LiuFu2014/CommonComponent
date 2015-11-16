using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    class CommonHelp
    {
        /// <summary>
        /// 比较两个结构相同的datatable内容是否相等
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static bool IsDatatableEquals(DataTable dt, DataTable dt2)
        {
            if (dt == null || dt2 == null)
            {
                return false;
            }
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j].ToString() != dt2.Rows[i][j].ToString())
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 比较两个结构相同的List内容是否相等
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static bool IsListEquals<T>(List<T> l1, List<T> l2)
        {
            if (l1 == null || l2 == null)
            {
                return false;

            }
            try
            {
                if (l1.Count != l2.Count)
                {
                    return false;
                }
                for (int i = 0; i < l1.Count; i++)
                {
                    if (l1[i].ToString() != l2[i].ToString())
                    {
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
    }
}
