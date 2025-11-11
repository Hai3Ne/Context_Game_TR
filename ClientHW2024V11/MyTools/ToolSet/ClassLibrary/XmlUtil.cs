using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XuXiang.ClassLibrary
{
    /// <summary>
    /// 提供XML公共操作。
    /// </summary>
    public static class XmlUtil
    {
        /// <summary>
        /// 获取XML节点属性的字符串值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        /// <param name="name">属性名称。</param>
        /// <param name="defaultvalue">属性不存在时的默认值。</param>
        /// <returns>属性字符串值，若该属性不存在则返回默认值。</returns>
        public static String GetAttribute(XmlNode xmlNode, String name, string defaultvalue = "")
        {
            XmlAttribute xmlAttr = xmlNode.Attributes[name];
            return xmlAttr == null ? String.Empty : xmlAttr.InnerText;
        }

        /// <summary>
        /// 获取XML节点属性值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        /// <param name="name">属性名称。</param>
        /// <param name="defaultvalue">属性不存在时的默认值。</param>
        /// <returns>属性值，若该属性不存在则返回默认值。</returns>
        public static bool GetAttributeBool(XmlNode xmlNode, String name, bool defaultvalue = false)
        {
            bool ret = defaultvalue;
            XmlAttribute xmlAttr = xmlNode.Attributes[name];
            if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.InnerText))
            {
                string s = xmlAttr.InnerText.ToLower();
                ret = s.CompareTo("0") != 0 && s.CompareTo("false") != 0;       //不是0或false就为真
            }
            return ret;
        }

        /// <summary>
        /// 获取XML节点属性值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        /// <param name="name">属性名称。</param>
        /// <param name="defaultvalue">属性不存在时的默认值。</param>
        /// <returns>属性值，若该属性不存在则返回默认值。</returns>
        public static int GetAttributeInt(XmlNode xmlNode, String name, int defaultvalue = 0)
        {
            int ret = defaultvalue;
            XmlAttribute xmlAttr = xmlNode.Attributes[name];
            if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.InnerText))
            {
                int.TryParse(xmlAttr.InnerText, out ret);
            }
            return ret;
        }

        /// <summary>
        /// 获取XML节点属性值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        /// <param name="name">属性名称。</param>
        /// <param name="defaultvalue">属性不存在时的默认值。</param>
        /// <returns>属性值，若该属性不存在则返回默认值。</returns>
        public static float GetAttributeFloat(XmlNode xmlNode, String name, float defaultvalue = 0)
        {
            float ret = defaultvalue;
            XmlAttribute xmlAttr = xmlNode.Attributes[name];
            if (xmlAttr != null && !string.IsNullOrEmpty(xmlAttr.InnerText))
            {
                float.TryParse(xmlAttr.InnerText, out ret);
            }
            return ret;
        }

        /// <summary>
        /// 设置XML节点属性值。
        /// </summary>
        /// <param name="node">XML节点。</param>
        /// <param name="key">属性名称。</param>
        /// <param name="value">属性值。</param>
        public static void SetAttribute(XmlNode node, string key, string value)
        {
            XmlAttribute attr = node.Attributes[key];
            if (attr == null)
            {
                XmlDocument doc = node.OwnerDocument;
                if (doc != null)
                {
                    node.Attributes.Append(doc.CreateAttribute(key)).InnerText = value;
                }
                else
                {
                    Console.WriteLine("The XmlNode(name:{0}).OwnerDocument is null.", node.Name);
                }
            }
            else
            {
                attr.InnerText = value;
            }
        }
    }
}
