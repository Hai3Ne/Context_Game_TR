using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProtocolBuilder
{
    /// <summary>
    /// 协议生成者。
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// 加载文件。
        /// </summary>
        /// <param name="file">文件路径。</param>
        /// <returns>是否加载成功。</returns>
        public bool Load(string file)
        {
            m_dicClassInfos.Clear();

            bool ok = true;
            try
            {
                //打开XML文件
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                //配置
                XmlNode root = doc.SelectSingleNode("lib");
                ok = ParseDefine(root);
            }
            catch (Exception e)
            {
                ok = false;
                Console.WriteLine(e.Message);
            }

            return ok;
        }

        /// <summary>
        /// 获取类名。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ClassInfo GetClassInfo(String name)
        {
            ClassInfo info;
            if (m_dicClassInfos.TryGetValue(name, out info))
            {
                return info;
            }
            return null;
        }

        /// <summary>
        /// 解析类型定义。
        /// </summary>
        /// <param name="node">类型定义节点。</param>
        private bool ParseDefine(XmlNode node)
        {
            if (node == null)
            {
                return false;
            }

            //逐个类加载
            foreach (XmlNode tmp in node.ChildNodes)
            {
                if (tmp.Name.CompareTo("struct") == 0)
                {
                    ClassInfo info = new ClassInfo();
                    info.AssignFromXmlNode(tmp);
                    if (!m_dicClassInfos.ContainsKey(info.Name))
                    {
                        m_dicClassInfos.Add(info.Name, info);
                    }
                    else
                    {
                        Console.WriteLine("类型({0})重复定义", info.Name);
                    }
                }
            }

            //检查字段
            foreach (var kvp in m_dicClassInfos)
            {
                foreach (FieldInfo info in kvp.Value.FieldInfos)
                {
                    string tl = info.Type.ToLower();
                    if (!ClassInfo.DefaultValue.ContainsKey(tl) && GetClassInfo(info.Type) == null)
                    {
                        Console.WriteLine("类型({0})未定义", info.Type);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 生成定义文件。
        /// </summary>
        /// <param name="file">输出文件。</param>
        public void BuildDefineScriptCS(string file)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//NetMsgData.cs");
            sb.AppendLine("//协议工具自动生成");
            sb.AppendLine();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("namespace SEZSJ");
            sb.AppendLine("{");

            //数据类型
            int n = m_dicClassInfos.Count;
            foreach (var kvp in m_dicClassInfos)
            {
                kvp.Value.BuildDefineCS(sb);
                if (--n > 0)
                {
                    //还有类要导出则继续换行
                    sb.AppendLine();
                }
            }
            sb.AppendLine("}");
            SaveFile(sb, file);
        }

        /// <summary>
        /// 生成定义文件。
        /// </summary>
        /// <param name="file">输出文件。</param>
        public void BuildDefineScriptLua(string file)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("-- @file     : NetMsgData.lua");
            sb.AppendLine("-- @brief    : 网络协议消息体定义");
            sb.AppendLine("-- @author   : 协议工具自动生成");
            sb.AppendLine();
            sb.AppendLine();

            //数据类型
            int n = m_dicClassInfos.Count;
            foreach (var kvp in m_dicClassInfos)
            {
                kvp.Value.BuildDefineLua(sb);
                if (--n > 0)
                {
                    //还有类要导出则继续换行
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }
            SaveFile(sb, file);
        }

        /// <summary>
        /// 保存文件。
        /// </summary>
        /// <param name="sb">文件内容。</param>
        /// <param name="file">文件路径。</param>
        private void SaveFile(StringBuilder sb, string file)
        {
            FileStream fstream = new FileStream(file, FileMode.Create);
            StreamWriter sw = new StreamWriter(fstream);
            sw.WriteLine(sb.ToString());
            sw.Flush();
            fstream.Flush();
            sw.Close();
            sw.Dispose();
            sw = null;
            fstream.Dispose();
            fstream = null;
            GC.Collect();
            Console.WriteLine("生成定义文件成功:\n{0}", file);
        }

        /// <summary>
        /// 类定义信息。
        /// </summary>
        private Dictionary<string, ClassInfo> m_dicClassInfos = new Dictionary<string, ClassInfo>();
    }
}
