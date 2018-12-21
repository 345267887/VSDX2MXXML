using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace mxGraph
{
    public static class Common
    {
        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        /// <summary>
        /// 转角度
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return degrees;

        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="codeName">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string EncodeBase64(Encoding encode, string source)
        {
            byte[] bytes = encode.GetBytes(source);

            string encodeStr = string.Empty;
            try
            {
                encodeStr = Convert.ToBase64String(bytes);
            }
            catch
            {
                encodeStr = source;
            }
            return encodeStr;
        }

        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(Encoding.UTF8, source);
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(Encoding encode, string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string DecodeBase64(string result)
        {
            return DecodeBase64(Encoding.UTF8, result);
        }


        public static String quote(String s)
        {
            int slashEIndex = s.IndexOf("\\E");
            if (slashEIndex == -1)
                return "\\Q" + s + "\\E";
            StringBuilder sb = new StringBuilder(s.Length * 2);
            sb.Append("\\Q");
            slashEIndex = 0;
            int current = 0;
            while ((slashEIndex = s.IndexOf("\\E", current)) != -1)
            {
                sb.Append(s.Substring(current, slashEIndex));
                current = slashEIndex + 2;
                sb.Append("\\E\\\\E\\Q");
            }
            sb.Append(s.Substring(current, s.Length));
            sb.Append("\\E");
            return sb.ToString();
        }

        public static int charCount(int codePoint)
        {
            int Min_Supplementary_Code_Poing = 0x010000;
            return codePoint >= Min_Supplementary_Code_Poing ? 2 : 1;
        }

        public static Color RgbToColor(int color)
        {
            return Color.FromArgb(color & 0x0000ff, (color & 0x00ff00) >> 8, (color & 0xff0000) >> 16);
        }

        public static void AddPoint(this Rectangle rectangle, Point point)
        {
            int newx = point.X;
            int newy = point.Y;

            if ((rectangle.Width | rectangle.Height) < 0)
            {
                rectangle.X = newx;
                rectangle.Y = newy;

                rectangle.Width = rectangle.Height = 0;

                return;
            }

            int x1 = rectangle.X;
            int y1 = rectangle.Y;
            long x2 = rectangle.Width;
            long y2 = rectangle.Height;

            x2 += x1;
            y2 += y1;

            if (x1 > newx)
            {
                x1 = newx;
            }

            if (y1 > newy)
            {
                y1 = newy;
            }

            if (x2 < newx)
            {
                x2 = newx;
            }

            if (y2 < newy)
            {
                y2 = newy;
            }

            x2 -= x1;
            y2 -= y1;

            if (x2 > int.MaxValue) x2 = int.MaxValue;
            if (y2 > int.MaxValue) y2 = int.MaxValue;

            {
                rectangle.X = x1;
                rectangle.Y = y1;
                rectangle.Width = (int)x2;
                rectangle.Height = (int)y2;

            }

        }


        public static sbyte[] ConvertSbytes(this byte[] bytes)
        {
            sbyte[] mySByte = new sbyte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 127)
                    mySByte[i] = (sbyte)(bytes[i] - 256);
                else
                    mySByte[i] = (sbyte)bytes[i];
            }

            return mySByte;
        }

        public static byte[] ConvertBytes(this sbyte[] sbytes)
        {
            byte[] bytes = new byte[sbytes.Length];
            for (int i = 0; i < sbytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(sbytes[i]);
            }

            return bytes;
        }

        public static char[] ConvertChars(this sbyte[] sbytes)
        {

            char[] chars = new char[sbytes.Length];
            for (int i = 0; i < sbytes.Length; i++)
            {
                chars[i] = Convert.ToChar(sbytes[i]);
            }

            return chars;
        }
    }

    public class SwingConstants
    {
        /// <summary>
        /// 框方向常量用于指定框的底部
        /// </summary>
        public static int BOTTOM = 3;
        /// <summary>
        /// 一个地区的中心位置
        /// </summary>
        public static int CENTER = 0;
        /// <summary>
        /// 指南针向东（右）。
        /// </summary>
        public static int EAST = 3;
        /// <summary>
        /// 水平方向。
        /// </summary>
        public static int HORIZONTAL = 0;
        /// <summary>
        /// 标识用于从左到右和从右到左语言的文本的前沿。
        /// </summary>
        public static int LEADING = 10;
        /// <summary>
        /// 框方向常量用于指定框的左侧。
        /// </summary>
        public static int LEFT = 2;
        /// <summary>
        /// 标识序列中的下一个方向。
        /// </summary>
        public static int NEXT = 12;
        /// <summary>
        /// 指南针方向北（向上）。
        /// </summary>
        public static int NORTH = 1;
        /// <summary>
        /// 指南针方向东北（右上）。
        /// </summary>
        public static int NORTH_EAST = 2;
        /// <summary>
        /// 指南针方向西北（左上）。
        /// </summary>
        public static int NORTH_WEST = 8;
        /// <summary>
        /// 标识序列中的上一个方向。
        /// </summary>
        public static int PREVIOUS = 13;
        /// <summary>
        /// 框方向常量用于指定框的右侧。
        /// </summary>
        public static int RIGHT = 4;
        /// <summary>
        /// 指南针向南（向下）。
        /// </summary>
        public static int SOUTH = 5;
        /// <summary>
        /// 指南针方向东南（右下）。
        /// </summary>
        public static int SOUTH_EAST = 4;
        /// <summary>
        /// 指南针方向西南（左下）。
        /// </summary>
        public static int SOUTH_WEST = 6;
        /// <summary>
        /// 框方向常量用于指定框的顶部。
        /// </summary>
        public static int TOP = 1;
        /// <summary>
        /// 标识文本的后端，以便使用从左到右和从右到左的语言。
        /// </summary>
        public static int TRAILING = 11;
        /// <summary>
        /// 垂直方向。
        /// </summary>
        public static int VERTICAL = 1;
        /// <summary>
        /// 指南针向西（左）。
        /// </summary>
        public static int WEST = 7;

    }
}
