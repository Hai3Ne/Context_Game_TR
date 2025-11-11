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
    /// 字段信息。
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// 变量名称，用于生成代码加载用。
        /// </summary>
        public string Name;

        /// <summary>
        /// 数据类型。
        /// </summary>
        public string Type;

        /// <summary>
        /// 数组长度变量。
        /// </summary>
        public string Count;

        /// <summary>
        /// 数组长度。
        /// </summary>
        public int CountNumber;

        /// <summary>
        /// 固定长度。
        /// </summary>
        public int Fixed;

        /// <summary>
        /// 注释。
        /// </summary>
        public string Note;

        /// <summary>
        /// 是否为数组。
        /// </summary>
        public bool IsArray
        {
            get { return CountNumber > 0 || !string.IsNullOrEmpty(Count); }
        }

        /// <summary>
        /// 从XML节点中赋值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        public void AssignFromXmlNode(XmlNode node)
        {
            Name = XmlUtil.GetAttribute(node, "name");
            Type = XmlUtil.GetAttribute(node, "type");
            Count = XmlUtil.GetAttribute(node, "count");
            Note = XmlUtil.GetAttribute(node, "desc");
            Fixed = XmlUtil.GetAttributeInt(node, "fixed");
            int.TryParse(Count, out CountNumber);
        }
    }
}
