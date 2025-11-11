using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelExporter
{
    /// <summary>
    /// 用于对数据进行导出。
    /// </summary>
    public abstract class DataExporter
    {
        /// <summary>
        /// 获取数据导出对象。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <returns></returns>
        public static DataExporter GetDataExporter(string type)
        {
            string tl = type.ToLower();
            if (tl.CompareTo("boolean") == 0)
            {
                return _cacheDataExporterBool;
            }
            else if (tl.CompareTo("int") == 0)
            {
                return _cacheDataExporterInt;
            }
            else if (tl.CompareTo("float") == 0)
            {
                return _cacheDataExporterFloat;
            }
            else if (tl.CompareTo("double") == 0)
            {
                return _cacheDataExporterDouble;
            }
            else if (tl.CompareTo("string") == 0)
            {
                return _cacheDataExporterString;
            }

            MainForm.CurForm.Log("未知数据类型:{0}", type);
            return null;
        }

        /// <summary>
        /// 布尔值导出者。
        /// </summary>
        private static DataExporter _cacheDataExporterBool = new DataExporterBool();

        /// <summary>
        /// 整数导出者。
        /// </summary>
        private static DataExporter _cacheDataExporterInt = new DataExporterInt();

        /// <summary>
        /// 浮点数导出者。
        /// </summary>
        private static DataExporter _cacheDataExporterFloat = new DataExporterFloat();

        /// <summary>
        /// 双精度浮点数导出者。
        /// </summary>
        private static DataExporter _cacheDataExporterDouble = new DataExporterDouble();

        /// <summary>
        /// 字符串导出者。
        /// </summary>
        private static DataExporter _cacheDataExporterString = new DataExporterString();

        /// <summary>
        /// 导出lua数据。
        /// </summary>
        public abstract string ExprotLua(string data);

        /// <summary>
        /// CPP的数据类型名称。
        /// </summary>
        public abstract string TypeNameCPP { get; }

        /// <summary>
        /// CPP的读取代码。
        /// </summary>
        /// <param name="array">是否数组读取。</param>
        /// <returns>读取代码。</returns>
        public abstract string GetReadCPP(bool array);
    }

    /// <summary>
    /// 布尔值导出。
    /// </summary>
    public class DataExporterBool : DataExporter
    {
        public override string TypeNameCPP
        {
            get { return "bool"; }
        }

        public override string ExprotLua(string data)
        {
            bool isfalse = string.IsNullOrEmpty(data) || data.CompareTo("0") == 0|| data.ToLower().CompareTo("false") == 0;
            return isfalse ? "false" : "true";
        }

        public override string GetReadCPP(bool array)
        {
            if (array)
            {
                MainForm.CurForm.Log("没有提供读取Bool数组的函数");
            }
            
            return "ReadBoolean";
        }
    }

    /// <summary>
    /// 整数导出。
    /// </summary>
    public class DataExporterInt : DataExporter
    {
        public override string TypeNameCPP
        {
            get { return "int"; }
        }

        public override string ExprotLua(string data)
        {
            int n = 0;
            if (!string.IsNullOrEmpty(data))
            {
                if (!int.TryParse(data, out n))
                {
                    MainForm.CurForm.Log("无法将{0}转换成int", data);
                }
            }
            
            return string.IsNullOrEmpty(data) ? "0" : data;
        }

        public override string GetReadCPP(bool array)
        {
            return array ? "ReadArrayInteger" : "ReadInteger";
        }
    }

    /// <summary>
    /// 浮点数导出。
    /// </summary>
    public class DataExporterFloat : DataExporter
    {
        public override string TypeNameCPP
        {
            get { return "float"; }
        }

        public override string ExprotLua(string data)
        {
            return string.IsNullOrEmpty(data) ? "0" : data;
        }

        public override string GetReadCPP(bool array)
        {
            return array ? "ReadArrayDouble" : "ReadDouble";
        }
    }

    /// <summary>
    /// 双精度浮点数导出。
    /// </summary>
    public class DataExporterDouble : DataExporter
    {
        public override string TypeNameCPP
        {
            get { return "double"; }
        }

        public override string ExprotLua(string data)
        {
            return string.IsNullOrEmpty(data) ? "0" : data;
        }

        public override string GetReadCPP(bool array)
        {
            return array ? "ReadArrayDouble" : "ReadDouble";
        }
    }

    /// <summary>
    /// 字符串导出。
    /// </summary>
    public class DataExporterString : DataExporter
    {
        public override string TypeNameCPP
        {
            get { return "std::string"; }
        }

        public override string ExprotLua(string data)
        {
            if(data.StartsWith("[===["))
            {
                return string.Format("{0}", data);
            }else
            {
                return string.Format("\"{0}\"", data.Replace("\"", "\\\""));
            }
        }

        public override string GetReadCPP(bool array)
        {
            if (array)
            {
                MainForm.CurForm.Log("没有提供读取String数组的函数");
            }

            return "ReadString";
        }
    }
}
