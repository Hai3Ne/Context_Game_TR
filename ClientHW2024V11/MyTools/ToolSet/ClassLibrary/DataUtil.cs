using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XuXiang.ClassLibrary
{
    public class DataUtil
    {
        /// <summary>
        /// 从字符串中分析点。
        /// </summary>
        /// <param name="value">点字符串。</param>
        /// <returns>点。</returns>
        public static Point ParsePoint(String value)
        {
            String[] stra = value.Split(',');
            Int32 x = Int32.Parse(stra[0]);
            Int32 y = Int32.Parse(stra[1]);
            return new Point(x, y);
        }

        /// <summary>
        /// 从字符串中分析矩形。
        /// </summary>
        /// <param name="value">矩形字符串。</param>
        /// <returns>矩形。</returns>
        public static Rect ParseRect(String value)
        {
            String[] stra = value.Split(',');
            Int32 x = Int32.Parse(stra[0]);
            Int32 y = Int32.Parse(stra[1]);
            Int32 w = Int32.Parse(stra[2]);
            Int32 h = Int32.Parse(stra[3]);
            return new Rect(x, y, w, h);
        }

        /// <summary>
        /// 从字符串中分析矩形颜色。
        /// </summary>
        /// <param name="value">矩形字符串。</param>
        /// <returns>颜色。</returns>
        public static Color ParseColor(String value)
        {
            String[] stra = value.Split(',');
            Byte a = Byte.Parse(stra[0]);
            Byte r = Byte.Parse(stra[1]);
            Byte g = Byte.Parse(stra[2]);
            Byte b = Byte.Parse(stra[3]);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// 试着分析字符数组某部分是否为颜色值
        /// </summary>
        /// <param name="buf">字符缓冲区。</param>
        /// <param name="pos">开始分析的位置。</param>
        /// <param name="color">输出参数。如果是颜色值，则保存颜色值，否则不改变，不能为null。</param>
        /// <returns>是否为颜色值。</returns>
        public static Boolean TryParseColor(char[] buf, Int32 pos, out Color color)
        {
            //判断剩余长度
            color = Color.Transparent;
            if (buf.Length < pos + 8)
            {
                return false;
            }

            //生成32位颜色值
            Int32 value = 0;
            for (Int32 i = 0; i < 8; ++i)
            {
                Int32 b = 0;
                switch (buf[pos + i])
                {
                    case '0':
                        b = 0;
                        break;
                    case '1':
                        b = 1;
                        break;
                    case '2':
                        b = 2;
                        break;
                    case '3':
                        b = 3;
                        break;
                    case '4':
                        b = 4;
                        break;
                    case '5':
                        b = 5;
                        break;
                    case '6':
                        b = 6;
                        break;
                    case '7':
                        b = 7;
                        break;
                    case '8':
                        b = 8;
                        break;
                    case '9':
                        b = 9;
                        break;
                    case 'A':
                    case 'a':
                        b = 10;
                        break;
                    case 'B':
                    case 'b':
                        b = 11;
                        break;
                    case 'C':
                    case 'c':
                        b = 12;
                        break;
                    case 'D':
                    case 'd':
                        b = 13;
                        break;
                    case 'E':
                    case 'e':
                        b = 14;
                        break;
                    case 'F':
                    case 'f':
                        b = 15;
                        break;
                    default:
                        return false;
                }
                value = (value << 4) | b;
            }

            //保存颜色值
            Int32 alpha = (Int32)((value & 0xFF000000) >> 24);
            Int32 red = (Int32)((value & 0xFF0000) >> 16);
            Int32 green = (Int32)((value & 0xFF00) >> 8);
            Int32 blue = (Int32)(value & 0xFF);
            color = Color.FromArgb(alpha, red, green, blue);

            return true;
        }

        /// <summary>
        /// 获取颜色的字符串值。
        /// </summary>
        /// <param name="c">颜色值。</param>
        /// <returns>字符串值，4个数字用逗号分开。</returns>
        public static String ToStringValue(Color c)
        {
            return String.Format("{0},{1},{2},{3}", c.A, c.R, c.G, c.B);
        }

        /// <summary>
        /// 获取点的字符串值。
        /// </summary>
        /// <param name="rt">点值。</param>
        /// <returns>字符串值，2个数字用逗号分开。</returns>
        public static String ToStringValue(Point p)
        {
            return String.Format("{0},{1}", p.X, p.Y);
        }

        /// <summary>
        /// 获取矩形的字符串值。
        /// </summary>
        /// <param name="rt">矩形值。</param>
        /// <returns>字符串值，4个数字用逗号分开。</returns>
        public static String ToStringValue(Rect rt)
        {
            return String.Format("{0},{1},{2},{3}", rt.X, rt.Y, rt.Width, rt.Height);
        }

        /// <summary>
        /// 获取布尔值对应的字节。
        /// </summary>
        /// <param name="b">要获取的布尔值。</param>
        /// <returns>字节值，true为0，false为0。</returns>
        public static Byte GetBooleanByte(Boolean b)
        {
            return (Byte)(b ? 1 : 0);
        }

        /// <summary>
        /// 获取16位整数的字节流。
        /// </summary>
        /// <param name="i">要获取的16位整数。</param>
        /// <returns>两字节的字节数组。</returns>
        public static Byte[] GetInt16Bytes(Int16 i)
        {
            Byte[] bytes = new Byte[2];
            UInt16 ui = (UInt16)i;
            bytes[1] = (Byte)(ui & 0xFF);                    //低位
            bytes[0] = (Byte)((ui >> 8) & 0xFF);             //高位
            return bytes;
        }

        /// <summary>
        /// 获取32位整数的字节流。
        /// </summary>
        /// <param name="i">要获取的32位整数。</param>
        /// <returns>四字节的字节数组。</returns>
        public static Byte[] GetInt32Bytes(Int32 i)
        {
            Byte[] bytes = new Byte[4];
            UInt32 ui = (UInt32)i;
            bytes[3] = (Byte)(i & 0xFF);                        //最低位
            bytes[2] = (Byte)((i >> 8) & 0xFF);                 //次低位
            bytes[1] = (Byte)((i >> 16) & 0xFF);                 //次高位
            bytes[0] = (Byte)((i >> 24) & 0xFF);                 //最高位
            return bytes;
        }

        /// <summary>
        /// 获取字符串的字节流。
        /// </summary>
        /// <param name="str">要获取的字符串。</param>
        /// <returns>生成的字节数组。</returns>
        public static Byte[] GetStringBytes(String str)
        {
            Byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(str);
            Byte[] len = GetInt32Bytes(data.Length);
            Byte[] ret = new Byte[data.Length + len.Length];
            Int32 pos = 0;
            foreach (Byte b in len)
            {
                ret[pos++] = b;
            }
            foreach (Byte b in data)
            {
                ret[pos++] = b;
            }
            return ret;
        }

        /// <summary>
        /// 将字节数组全部写入到数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="bs">要写入的字节数组。</param>
        public static void WriteBytes(Stream s, Byte[] bs)
        {
            s.Write(bs, 0, bs.Length);
        }

        /// <summary>
        /// 将布尔值写入的数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的布尔值。</param>
        public static void WriteBoolean(Stream s, Boolean b)
        {
            s.WriteByte(DataUtil.GetBooleanByte(b));
        }

        /// <summary>
        /// 将16位整数写入的数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的整数值。</param>
        public static void WriteInt16(Stream s, Int16 i)
        {
            DataUtil.WriteBytes(s, DataUtil.GetInt16Bytes(i));
        }

        /// <summary>
        /// 将32位整数写入的数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的整数值。</param>
        public static void WriteInt32(Stream s, Int32 i)
        {
            DataUtil.WriteBytes(s, DataUtil.GetInt32Bytes(i));
        }

        /// <summary>
        /// 将浮点数写入的数据流中，乘以1000按Int32写入。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的浮点数。</param>
        public static void WriteSingle(Stream s, Single d)
        {
            DataUtil.WriteBytes(s, DataUtil.GetInt32Bytes((Int32)(d * 1000)));
        }

        /// <summary>
        /// 将浮点数写入的数据流中，乘以1000按Int32写入。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的浮点数。</param>
        public static void WriteDouble(Stream s, Double d)
        {
            DataUtil.WriteBytes(s, DataUtil.GetInt32Bytes((Int32)(d * 1000)));
        }

        /// <summary>
        /// 将字符串写入的数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="i">要写入的字符串值。</param>
        public static void WriteString(Stream s, String str)
        {
            DataUtil.WriteBytes(s, DataUtil.GetStringBytes(str));
        }

        /// <summary>
        /// 将点写入到数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="rt">要写入的点值。</param>
        public static void WritePoint(Stream s, Point pt)
        {
            DataUtil.WriteSingle(s, pt.X);
            DataUtil.WriteSingle(s, pt.Y);
        }

        /// <summary>
        /// 将矩形写入到数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="rt">要写入的矩形值。</param>
        public static void WriteRect(Stream s, Rect rt)
        {
            DataUtil.WriteSingle(s, rt.X);
            DataUtil.WriteSingle(s, rt.Y);
            DataUtil.WriteSingle(s, rt.Width);
            DataUtil.WriteSingle(s, rt.Height);
        }

        /// <summary>
        /// 将颜色写入到数据流中。
        /// </summary>
        /// <param name="s">要写入的数据流。</param>
        /// <param name="c">要写入的颜色值。</param>
        public static void WriteColor(Stream s, Color c)
        {
            DataUtil.WriteSingle(s, c.A / 255.0f);
            DataUtil.WriteSingle(s, c.R / 255.0f);
            DataUtil.WriteSingle(s, c.G / 255.0f);
            DataUtil.WriteSingle(s, c.B / 255.0f);
        }

        /// <summary>
        /// 拆分文本。
        /// </summary>
        /// <param name="text">要拆分的文本。</param>
        /// <param name="s">分隔串。</param>
        /// <param name="ignoreempty">分隔结果是否忽略空串。</param>
        /// <returns>字符串列表。</returns>
        public static List<string> Split(string text, string s = " ", bool ignoreempty = true)
        {
            List<string> rets = new List<string>();
            string[] strs = text.Split(s.ToCharArray());
            if (ignoreempty)
            {
                for (int i = 0; i < strs.Length; ++i)
                {
                    string tmp = strs[i].Trim();
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        rets.Add(tmp);
                    }
                }
            }
            else
            {
                rets.AddRange(strs);
            }
            
            return rets;
        }
    }
}
