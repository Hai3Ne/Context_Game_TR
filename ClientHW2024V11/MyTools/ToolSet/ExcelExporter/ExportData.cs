using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExcelExporter
{
    /// <summary>
    /// 需要导出的数据。
    /// </summary>
    public class ExportData
    {
        /// <summary>
        /// 数据起始行。
        /// </summary>
        public const int DATA_START_ROW = 5;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public ExportData(string name, object[,] data, string fileName)
        {
            m_Name = name;
            m_ObjectData = data;
            m_FileName = fileName;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ExportData(string name, string[,] data)
        {
            m_Name = name;
            StringData = data;
        }

        /// <summary>
        /// 检测数据合法性。
        /// </summary>
        /// <returns>数据是否正确。</returns>
        private bool CheckData()
        {
            //有必要的话进行数据转换
            if (StringData == null)
            {
                if (m_ObjectData == null)
                {
                    return false;
                }
                StringData = ObjectToString();
            }

            //初步检查
            int c = StringData.GetLength(1);
            if (c <= 0)
            {
                MainForm.CurForm.Log("要导出的数据表至少要有1列 sheet:({0})", m_Name);
                return false;
            }

            int r = StringData.GetLength(0);
            if (r < DATA_START_ROW)
            {
                MainForm.CurForm.Log("要导出的数据表至少要有{0}行() sheet:({1})", DATA_START_ROW, m_Name);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开始导出。
        /// </summary>
        public void StartExprot()
        {
            if (CheckData())
            {
                MainForm.CurForm.Log($"==== 开始导出【{m_FileName}】{m_Name}"); 
                //客户端服务器一起导出
                ExportLuaEx(ExportTarget.Client, StringData, m_Name);
                ExportLua(ExportTarget.Server, StringData, m_Name);
            }            
        }

        /// <summary>
        /// 将对象转成字符串。
        /// </summary>
        /// <returns>字符串二维数组。</returns>
        private string[,] ObjectToString()
        {
            int row = m_ObjectData.GetLength(0);
            int col = m_ObjectData.GetLength(1);
            string[,] data = new string[row, col];
            for (int i=0; i<row; ++i)
            {
                for (int j=0; j<col; ++j)
                {
                    object obj = m_ObjectData[i, j];  //Excel转换出来的数据是1开始
                    data[i, j] = obj == null ? string.Empty : obj.ToString();
                }
            }
            return data;
        }

        /// <summary>
        /// 获取服务器导出文件名。
        /// </summary>
        /// <param name="name">表格名。</param>
        /// <returns>文件名。</returns>
        public static string GetServerLuaName(string name)
        {
            if (name.Length <= 3)
            {
                return name;
            }

            string fc = name.Substring(2, 1).ToUpper();
            return fc + name.Remove(0, 3) + "Config";
        }

        /// <summary>
        /// 导出数据表。
        /// </summary>
        /// <param name="et">导出目标。</param>
        /// <param name="data">数据表内容。</param>
        /// <param name="sb">要写入到的数据。</param>
        private static void ExportLua(ExportTarget et, string[,] data, string name)
        {
            //读取表头
            int c = data.GetLength(1);
            int r = data.GetLength(0);
            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < c; ++i)
            {
                string vname = data[0, i];
                string type = data[1, i];
                string target = data[2, i];
                string note = data[4, i];
                FieldInfo info = FieldInfo.CreateFiledInfo(i, vname, type, target, note, et);
                if (info != null)
                {
                    fields.Add(info);
                }
            }
            if (fields.Count <= 0)
            {
                MainForm.CurForm.Log("没有需要导出的字段 sheet:({0}) target:{1}", name, et);
                return;
            }

            //开始生成导出数据
            StringBuilder sb = new StringBuilder();
            string fname = et == ExportTarget.Client ? name : GetServerLuaName(name);
            string define = et == ExportTarget.Client ? string.Format("_G.{0}={{", fname) : string.Format("{0}={{", fname); //表定义
            sb.AppendLine(define);

            //逐行导出数据
            for (int i = DATA_START_ROW; i < r; ++i)
            {
                string idobj = data[i, 0];
                string idstr = idobj == null ? string.Empty : idobj.Trim();
                if (string.IsNullOrEmpty(idstr))
                {
                    break;
                }

                //int id = int.Parse(idstr);
                var id = idstr;
                if (et == ExportTarget.Client)
                {
                    sb.AppendLine(string.Format("\t[{0}]={{", id));
                }
                else
                {
                    sb.AppendLine(string.Format("\t[\'{0}\']={{", id));
                }
                for (int j = 0; j < fields.Count; ++j)
                {
                    FieldInfo info = fields[j];
                    string valueobj = data[i, info.Col];
                    string valuestr = valueobj == null ? string.Empty : valueobj.Trim();
                    sb.AppendLine(string.Format("\t\t[\'{0}\'] = {1},", info.VName, info.ExprotLua(valuestr)));
                }
                sb.AppendLine(string.Format("\t}},"));
            }

            sb.AppendLine(string.Format("}}"));
            if (et == ExportTarget.Client)
            {
                sb.AppendLine(string.Format("return _G.{0}", fname));           //客户端加个return，方便required
            }
            SaveLuaFile(et, sb, name);
        }

        /// <summary>
        /// 导出数据表(优化版)。
        /// </summary>
        /// <param name="et">导出目标。</param>
        /// <param name="data">数据表内容。</param>
        /// <param name="sb">要写入到的数据。</param>
        private static void ExportLuaEx(ExportTarget et, string[,] data, string name)
        {
            //读取表头
            int c = data.GetLength(1);
            int r = data.GetLength(0);
            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < c; ++i)
            {
                string vname = data[0, i];
                string type = data[1, i];
                string target = data[2, i];
                string note = data[4, i];
                FieldInfo info = FieldInfo.CreateFiledInfo(i, vname, type, target, note, et);
                if (info != null)
                {
                    fields.Add(info);
                }
            }
            if (fields.Count <= 0)
            {
                MainForm.CurForm.Log("没有需要导出的字段 sheet:({0}) target:{1}", name, et);
                return;
            }
            if(!((fields[0].isClient && et == ExportTarget.Client)
                || ((fields[0].isServer && et == ExportTarget.Server))
                ))
            {
                MainForm.CurForm.Log("######不导出 " + (et == ExportTarget.Client?"客户端 ":"服务器 ") + name);
                return;
            }

            //开始生成导出数据
            StringBuilder sb = new StringBuilder();
            string fname = et == ExportTarget.Client ? name : GetServerLuaName(name);
            string define = et == ExportTarget.Client ? string.Format("_G.{0}={{", fname) : string.Format("{0}={{", fname); //表定义
            sb.AppendLine(define);

            //逐行导出数据
            for (int i = DATA_START_ROW; i < r; ++i)
            {
                string idobj = data[i, 0];
                string idstr = idobj == null ? string.Empty : idobj.Trim();
                if (string.IsNullOrEmpty(idstr))
                {
                    break;
                }

                //int id = int.Parse(idstr);
                if (et == ExportTarget.Client)
                {
                    if (fields[0].Type.Equals("String", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.Append(string.Format("\t['{0}']={{", idstr));
                    }
                    else
                    {
                        sb.Append(string.Format("\t[{0}]={{", idstr));
                    }
                }
                else
                {
                    sb.Append(string.Format("\t[\'{0}\']={{", idstr));
                }
                for (int j = 0; j < fields.Count; ++j)
                {
                    FieldInfo info = fields[j];
                    string valueobj = data[i, info.Col];
                    string valuestr = valueobj == null ? string.Empty : valueobj.Trim();
                    if (j > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(info.ExprotLua(valuestr));
                }
                sb.AppendLine(string.Format("}},"));
            }
            sb.AppendLine(string.Format("}}"));

            //导出Keys和原表设置代码
            sb.AppendLine();
            sb.Append("local keys = {");
            for (int i=0; i< fields.Count; ++i)
            {
                FieldInfo info = fields[i];
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(string.Format("\"{0}\"", info.VName));
            }
            sb.AppendLine("}");
            //多语言
            sb.Append("local language_keys = {");
            var jjj = 0;
            for (int i = 0; i < fields.Count; ++i)
            {
                FieldInfo info = fields[i];
                if (!info.IsLanguage) continue;
                if (jjj > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(string.Format("\"{0}\"", info.VName));
                jjj++;
            }
            sb.AppendLine("}");

            //自动参数
            sb.Append("local param_keys = {");
            var kkk = 0;
            for (int i = 0; i < fields.Count; ++i)
            {
                FieldInfo info = fields[i];
                if (!info.IsAutoParam) continue;
                if (kkk > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(string.Format("\"{0}\"", info.VName));
                kkk++;
            }
            sb.AppendLine("}");

            sb.AppendLine("local mt = Common.GetKeyMetaTable(keys, language_keys, param_keys)");
            sb.AppendLine(string.Format("Common.SetConfigMetaTable(_G.{0}, mt)", fname));
            sb.AppendLine();

            if (et == ExportTarget.Client)
            {
                sb.AppendLine(string.Format("return _G.{0}", fname));           //客户端加个return，方便required
            }
            SaveLuaFile(et, sb, name);
        }

        /// <summary>
        /// 将字符串保存到文件。
        /// </summary>
        /// <param name="et">导出目标类型。</param>
        /// <param name="sb">字符串。</param>
        /// <param name="name">表格名称。</param>
        private static void SaveLuaFile(ExportTarget et, StringBuilder sb, string name)
        {
            string fname = et == ExportTarget.Client ? name : GetServerLuaName(name);
            string exportfile = ConfigArchive.Instance.GetExportFolderLua(et, fname) + "\\" + fname;
            string exname = et == ExportTarget.Client ? ".lua.txt" : ".lua";
            FileStream fs = new FileStream(exportfile + exname, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(sb.ToString());
            writer.Flush();
            fs.Flush();
            writer.Dispose();
            writer = null;
            fs.Dispose();
            fs = null;
        }
        
        /// <summary>
        /// 开始生成CPP。
        /// </summary>
        public void StartBuildCPP()
        {
            if (CheckData())
            {
                BuildCPP(StringData, m_Name);
            }
        }

        /// <summary>
        /// 生成CPP。
        /// </summary>
        /// <param name="data">数据表内容。</param>
        /// <param name="sb">要写入到的数据。</param>
        private static void BuildCPP(string[,] data, string name)
        {
            //读取表头
            int c = data.GetLength(1);
            int r = data.GetLength(0);
            List<FieldInfo> fields = new List<FieldInfo>();
            for (int i = 0; i < c; ++i)
            {
                string vname = data[0, i];
                string type = data[1, i];
                string target = data[2, i];
                string note = data[4, i];
                FieldInfo info = FieldInfo.CreateFiledInfo(i, vname, type, target, note, ExportTarget.Server);
                if (info != null)
                {
                    fields.Add(info);
                }
            }
            if (fields.Count <= 0)
            {
                MainForm.CurForm.Log("没有需要生成CPP的字段 sheet:({0})", name);
                return;
            }
            BuildHeaderFile(fields, name);
            BuildCPPFile(fields, name);
        }

        /// <summary>
        /// 生成头文件。
        /// </summary>
        /// <param name="fields">字段列表。</param>
        /// <param name="name">配置名称。</param>
        private static void BuildHeaderFile(List<FieldInfo> fields, string name)
        {
            string defname = name.Substring(2).ToLower();       //宏定义用的名称
            string define = string.Format("_mars_{0}_config_", defname);         //宏定义

            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("\n");
            sb.AppendFormat("#ifndef {0}\n", define);
            sb.AppendFormat("#define {0}\n", define);
            sb.Append("\n");
            sb.AppendFormat("#include <string>\n");
            sb.AppendFormat("#include <unordered_map>\n");
            sb.Append("\n");
            sb.AppendFormat("#include <vector>\n");
            sb.AppendFormat("using namespace std;\n");
            sb.AppendFormat("namespace mars\n");
            sb.AppendFormat("{{\n");

            //生成定义结构体
            string cname = GetServerLuaName(name);              //类名称
            string sname = "S" + cname;                         //结构体名称
            sb.AppendFormat("\tstruct {0}\n", sname);
            sb.AppendFormat("\t{{\n");
            for (int i = 0; i < fields.Count; ++i)
            {
                FieldInfo info = fields[i];
                sb.AppendFormat("\t\t{0}\n", info.GetCPPDefine());
            }
            sb.AppendFormat("\t}};\n");
            sb.Append("\n");

            //生成类
            sb.AppendFormat("\tclass {0}\n", cname);
            sb.AppendFormat("\t{{\n");
            sb.AppendFormat("\tpublic:\n");
            sb.AppendFormat("\t\tint Load(const char* path,const char* name);\n");
            sb.AppendFormat("\t\tint Size();\n");
            sb.AppendFormat("\t\t{0}& Get(int i);\n", sname);
            sb.AppendFormat("\tprivate:\n");
            sb.AppendFormat("\t\ttypedef std::vector<{0}> CollectionConfigsT;\n", sname);
            sb.AppendFormat("\t\tCollectionConfigsT m_vConfigs;\n");
            sb.AppendFormat("\t}};\n");


            sb.AppendFormat("}}\n");
            sb.Append("\n");
            sb.AppendFormat("#endif");
            SaveCPPFile(sb, name, true);
        }

        /// <summary>
        /// 生成CPP文件。
        /// </summary>
        /// <param name="fields">字段列表。</param>
        /// <param name="name">配置名称。</param>
        private static void BuildCPPFile(List<FieldInfo> fields, string name)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            sb.Append("\n");

            //#include部分
            string cname = GetServerLuaName(name);              //类名称
            string sname = "S" + cname;                         //结构体名称
            sb.AppendFormat("#include \"{0}.h\"\n", cname);
            sb.AppendFormat("#include \"LuaReader.h\"\n");
            sb.AppendFormat("#include <cassert>\n");
            sb.Append("\n");

            sb.Append("namespace mars\n");
            sb.AppendFormat("{{\n");

            sb.AppendFormat("\tint {0}::Load(const char* path,const char* name)\n", cname);
            sb.AppendFormat("\t{{\n");

            sb.AppendFormat("\t\tmars_utils::LuaReader reader;\n");
            sb.AppendFormat("\t\treader.LoadFile(path,name);\n");
            sb.AppendFormat("\n");

            sb.AppendFormat("\t\tstd::vector<std::string>& vTableKeys = reader.GetTablekeys();\n");
            sb.AppendFormat("\t\tfor (size_t i = 0; i < vTableKeys.size(); ++i)\n");
            sb.AppendFormat("\t\t{{\n");

            sb.AppendFormat("\t\t\t{0} v;\n", sname);            
            sb.AppendFormat("\n");

            sb.AppendFormat("\t\t\tconst char* key = vTableKeys[i].c_str();\n");
            for (int i = 0; i < fields.Count; ++i)
            {
                FieldInfo info = fields[i];
                if (info.IsArray)
                {
                    sb.AppendFormat("\t\t\tfor (int i = 0; i < {0}; ++i)\n", info.ArrayNumber);
                    sb.AppendFormat("\t\t\t{{\n");
                    sb.AppendFormat("\t\t\t\t{0}\n", info.GetCPPRead());
                    sb.AppendFormat("\t\t\t}}\n");
                }
                else
                {
                    sb.AppendFormat("\t\t\t{0}\n", info.GetCPPRead());
                }
            }

            sb.AppendFormat("\n");
            sb.AppendFormat("\t\t\tm_vConfigs.push_back(v);\n");

            sb.AppendFormat("\t\t}}\n");
            sb.AppendFormat("\t\treturn 0;\n");
            sb.AppendFormat("\t}}\n");
            sb.Append("\n");

            sb.AppendFormat("\tint {0}::Size()\n", cname);
            sb.AppendFormat("\t{{\n");
            sb.AppendFormat("\t\treturn m_vConfigs.size();\n");
            sb.AppendFormat("\t}}\n");
            sb.Append("\n");

            sb.AppendFormat("\t{1}& {0}::Get(int i)\n", cname, sname);
            sb.AppendFormat("\t{{\n");
            sb.AppendFormat("\t\tif (i >= 0 && i < (int)m_vConfigs.size())\n");
            sb.AppendFormat("\t\t{{\n");
            sb.AppendFormat("\t\t\treturn m_vConfigs[i];\n");
            sb.AppendFormat("\t\t}}\n");
            sb.AppendFormat("\t\tstatic {0} tmp;\n", sname);
            sb.AppendFormat("\t\treturn tmp;\n");
            sb.AppendFormat("\t}}\n");

            sb.AppendFormat("}}\n");
            SaveCPPFile(sb, name, false);
        }

        /// <summary>
        /// 将字符串保存到文件。
        /// </summary>
        /// <param name="sb">字符串。</param>
        /// <param name="name">表格名称。</param>
        /// <param name="head">是否头文件。</param>
        private static void SaveCPPFile(StringBuilder sb, string name, bool head)
        {
            string fname = GetServerLuaName(name);
            string exportfile = ConfigArchive.Instance.ExportFolderServerCPP + "\\" + fname;
            string exname = head ? ".h" : ".cpp";
            FileStream fs = new FileStream(exportfile + exname, FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(sb.ToString());
            writer.Flush();
            fs.Flush();
            writer.Dispose();
            writer = null;
            fs.Dispose();
            fs = null;
        }

        /// <summary>
        /// 表格名称。
        /// </summary>
        private string m_Name;
        private string m_FileName;

        /// <summary>
        /// Native加载的对象数据。
        /// </summary>
        private object[,] m_ObjectData;

        /// <summary>
        /// NPOI加载的字符串数据。
        /// </summary>
        private string[,] StringData;
    }
}
