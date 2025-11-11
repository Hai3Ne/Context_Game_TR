using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XuXiang.ClassLibrary;

namespace ExcelExporter
{
    /// <summary>
    /// 字段信息。
    /// </summary>
    public class FieldInfo
    {
        public static FieldInfo CreateFiledInfo(int col, string vname, string type, string target, string note, ExportTarget et)
        {
            //变量名 类型 目标都得有
            vname = vname.Trim();
            type = type.Trim().ToLower();
            target = target.Trim().ToLower();
            if (string.IsNullOrEmpty(vname) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(type))
            {
                return null;
            }

            //导出目标判断，客户端得有C，服务端得有S
            if (!((et == ExportTarget.Client && target.IndexOf('c') != -1) || (et == ExportTarget.Server && target.IndexOf('s') != -1)))
            {
                return null;
            }

            //数据类型要正确
            string typestr = type;
            DataExporter de = null;
            bool isarray = false;
            int number = 0;
            int left = type.IndexOf('[');
            int right = type.IndexOf(']');
            if (left != -1)
            {
                typestr = type.Substring(0, left);
                if (right != -1)
                {
                    string numsub = type.Substring(left + 1, right - left - 1);
                    int.TryParse(numsub, out number);
                    isarray = true;
                }
            }
            de = DataExporter.GetDataExporter(typestr);
            if (de == null)
            {
                return null;
            }

            //创建字段
            FieldInfo info = new FieldInfo();
            info.Col = col;
            info.VName = vname;
            info.target = target;
            info.Type = typestr;
            info.IsArray = isarray;
            info.ArrayNumber = number;
            info.Note = note;
            info.m_deExporter = de;
            return info;
        }

        /// <summary>
        /// 所在的列。
        /// </summary>
        public int Col;

        /// <summary>
        /// 变量名称，用于生成代码加载用。
        /// </summary>
        public string VName;

        /// <summary>
        /// 目标
        /// </summary>
        public string target;
        public bool isClient
        {
            get
            {
                return target.Contains("c");
            }
        }
        public bool isServer
        {
            get
            {
                return target.Contains("s");
            }
        }
        /// <summary>
        /// 是否是多语言
        /// </summary>
        public bool IsLanguage
        {
            get
            {
                return target.Contains("l");
            }
        }
        /// <summary>
        /// 是否自动格式化
        /// </summary>
        public bool IsAutoParam
        {
            get
            {
                return target.Contains("p");
            }
        }

        /// <summary>
        /// 数据类型。
        /// </summary>
        public string Type;

        /// <summary>
        /// 是否为数组。
        /// </summary>
        public bool IsArray;

        /// <summary>
        /// 数组长度，0表示变长数组。
        /// </summary>
        public int ArrayNumber;

        /// <summary>
        /// 描述。
        /// </summary>
        public string Note;

        /// <summary>
        /// 数据导出者。
        /// </summary>
        private DataExporter m_deExporter;

        /// <summary>
        /// 导出lua数据。
        /// </summary>
        public string ExprotLua(string data)
        {
            string ret = string.Empty;
            if (IsArray)
            {
                //数组的要先写入数量，在拆分逐个导出
                List<string> fdata = DataUtil.Split(data, ",", true);
                List<string> values = new List<string>();
                int n = ArrayNumber > 0 ? ArrayNumber : fdata.Count;
                for (int i = 0; i < n; ++i)
                {
                    values.Add(m_deExporter.ExprotLua(i < fdata.Count ? fdata[i] : string.Empty));
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                for (int i=0; i<values.Count; ++i)
                {
                    if (i != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(values[i]);
                }
                sb.Append("}");
                ret = sb.ToString();
            }
            else
            {
                ret = m_deExporter.ExprotLua(data);
            }

            return ret;
        }

        /// <summary>
        /// 获取CPP的定义。
        /// </summary>
        /// <returns>CPP的定义。</returns>
        public string GetCPPDefine()
        {
            string type = m_deExporter.TypeNameCPP;
            string postfix = IsArray ? string.Format("[{0}]", ArrayNumber) : string.Empty;
            string note = string.IsNullOrEmpty(Note) ? "null" : Note;
            string define = string.Format("{0} {1}{2};\t//{3};", type, VName, postfix, note);
            return define;
        }

        /// <summary>
        /// 获取CPP的读取代码。
        /// </summary>
        /// <returns>CPP读取代码。</returns>
        public string GetCPPRead()
        {
            string read = m_deExporter.GetReadCPP(IsArray);
            string code = IsArray ? string.Format("v.{0}[i] = reader.{1}(key,\"{0}\",i);", VName, read) : string.Format("v.{0} = reader.{1}(key,\"{0}\");", VName, read);
            return code;
        }
    }
}
