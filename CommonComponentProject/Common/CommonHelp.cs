using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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

        /// <summary>
        /// 方法，只允许数字类型输入 (拷贝的春哥的代码，这里都是局部变量，对于多线程都是独立的，加锁是多余)
        /// </summary>
        /// <param name="KeyChar">键码</param>
        /// <param name="t">数据类型</param>
        /// <param name="Text">文本</param>
        /// <returns>真假值</returns>
        public static bool MaskNumber(Char KeyChar, Type t, string Text, bool blnDot, bool blnLine)//forbitDot---是否允许圆点输入
        {
            #region

            if (KeyChar == 8) return false;//(e.KeyChar == (Char)8)//Backspace_8 和 Enter_13 键

            lock (typeof(CommonHelp))
            {
                //浮点型
                if (t == typeof(decimal) || t == typeof(Single) || t == typeof(Double))
                {
                    int intIndex = Text.IndexOf(".");

                    if (intIndex < 0 && blnDot) //allowDot=true允许圆点号-----//点号(intIndex=-1)
                    {
                        if (Char.IsDigit(KeyChar) || KeyChar == '.') return false;
                    }
                    else
                    {
                        if (Char.IsDigit(KeyChar)) return false;
                    }

                    intIndex = Text.IndexOf("-");

                    if (intIndex < 0 && blnLine)
                    {
                        if (Char.IsDigit(KeyChar) || KeyChar == '-') return false;/* || KeyChar == '-'*/
                    }
                    else
                    {
                        if (Char.IsDigit(KeyChar)) return false;
                    }
                    return true;
                }

                //整型
                if (t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64) || t == typeof(UInt16) || t == typeof(UInt32) || t == typeof(UInt64) || t == typeof(Byte) || t == typeof(SByte))
                {
                    if (Char.IsDigit(KeyChar)) return false;
                    return true;
                }

                //日期型
                if (t == typeof(System.DateTime))
                {
                    int intIndex = Text.IndexOf("-");
                    int intIndex1 = Text.IndexOf("/");

                    if (intIndex < 0 && intIndex1 < 0)
                    {
                        if (Char.IsDigit(KeyChar) || KeyChar == '-' || KeyChar == '/') return false;
                    }
                    else
                    {
                        if (intIndex > 0)
                        {
                            if (ContainCharNumber(Text, "-") == 1)
                            {
                                if (Char.IsDigit(KeyChar) || KeyChar == '-') return false;
                            }
                            else
                            {
                                if (Char.IsDigit(KeyChar)) return false;
                            }
                            return true;
                        }
                        if (intIndex1 > 0)
                        {
                            if (ContainCharNumber(Text, "/") == 1)
                            {
                                if (Char.IsDigit(KeyChar) || KeyChar == '/') return false;
                            }
                            else
                            {
                                if (Char.IsDigit(KeyChar)) return false;
                            }
                            return true;
                        }
                        if (Char.IsDigit(KeyChar)) return false;
                    }
                    return true;
                }

                return false;
            }

            #endregion
        }

        /// <summary>
        /// 方法，字符串中包含字符的数量 (拷贝的春哥的代码，这里都是局部变量，对于多线程都是独立的，加锁是多余)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="Char"></param>
        /// <returns>返回包含字符Char的数量</returns>
        private static int ContainCharNumber(string str, string Char)
        {
            #region

            lock (typeof(CommonHelp))
            {
                if (str.Length < 1) return 0;

                int j = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str.Substring(i, 1) == Char)
                    {
                        j++;
                    }
                }

                return j;
            }

            #endregion
        }

        /// <summary>
        /// 截取图像的矩形区域
        /// </summary>
        /// <param name="source">源图像对应picturebox1</param>
        /// <param name="rect">矩形区域，如上初始化的rect</param>
        /// <returns>矩形区域的图像</returns>
        public static Image AcquireRectangleImage(Image source, Rectangle rect)
        {
            if (source == null || rect.IsEmpty) return null;
            Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            //Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, source.PixelFormat);

            using (Graphics grSmall = Graphics.FromImage(bmSmall))
            {
                grSmall.DrawImage(source,
                                  new System.Drawing.Rectangle(0, 0, bmSmall.Width, bmSmall.Height),
                                  rect,
                                  GraphicsUnit.Pixel);
                grSmall.Dispose();
            }
            return bmSmall;
        }

        //// <summary>
        /// 获取图片编码信息
        /// </summary>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="quality">质量压缩比</param>
        /// <param name="multiple">收缩倍数</param>
        /// <param name="outputFile">输出文件名</param>
        /// <returns>成功返回true,失败则返回false</returns>
        public static bool getThumImage(String sourceFile, long quality, int multiple, String outputFile)
        {
            try
            {
                long imageQuality = quality;
                Bitmap sourceImage = new Bitmap(sourceFile);
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                float xWidth = sourceImage.Width;
                float yWidth = sourceImage.Height;
                Bitmap newImage = new Bitmap((int)(xWidth / multiple), (int)(yWidth / multiple));
                Graphics g = Graphics.FromImage(newImage);

                g.DrawImage(sourceImage, 0, 0, xWidth / multiple, yWidth / multiple);
                g.Dispose();
                newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //// <summary>
        /// 图片压缩函数
        /// </summary>
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="quality">质量压缩比</param>
        /// <param name="ouputFile">输出文件名,请用 .jpg 后缀 </param>
        /// <returns>成功返回true，失败则返回false</returns>
        public static bool imageCompress(String sourceFile, long quality, String outputFile)
        {
            try
            {
                long imageQuality = quality;
                Bitmap sourceImage = new Bitmap(sourceFile);
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                sourceImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);
                return true;

            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 图片切割函数
        /// </summary>
        /// <param name="sourceFile">原始图片文件</param>
        /// <param name="xNum">在Ｘ轴上的切割数量</param>
        /// <param name="yNum">在Ｙ轴上的切割数量</param>
        /// <param name="quality">质量压缩比</param>
        /// <param name="outputFile">输出文件名，不带后缀</param>
        /// <returns>成功返回true，失败则返回false</returns>
        public static bool imageCut(String sourceFile, int xNum, int yNum, long quality, String outputFile)
        {
            try
            {
                long imageQuality = quality;
                Bitmap sourceImage = new Bitmap(sourceFile);
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                float xWidth = sourceImage.Width / xNum;
                float yWidth = sourceImage.Height / yNum;
                String outputImage = "";

                for (int countY = 0; countY < yNum; countY++)
                    for (int countX = 0; countX < xNum; countX++)
                    {

                        RectangleF cloneRect = new RectangleF(countX * xWidth, countY * yWidth, xWidth, yWidth);
                        Bitmap newImage = sourceImage.Clone(cloneRect, PixelFormat.Format24bppRgb);
                        outputImage = outputFile + countX + countY + ".jpg";
                        newImage.Save(outputImage, myImageCodecInfo, myEncoderParameters);

                    }
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
