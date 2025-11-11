using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProtocolBuilder
{
    /// <summary>
    /// 类定义信息。
    /// </summary>
    public class ClassInfo
    {
        public static Dictionary<string, string> DefaultValue = new Dictionary<string, string>();

        public static Dictionary<string, string> DefaultWrite = new Dictionary<string, string>();

        public static Dictionary<string, string> DefaultRead = new Dictionary<string, string>();

        public static Dictionary<string, string> TypeToCS = new Dictionary<string, string>();


        static ClassInfo()
        {
            //默认值
            DefaultValue.Add("byte", "0");
            DefaultValue.Add("int8", "0");
            DefaultValue.Add("uint8", "0");
            DefaultValue.Add("int16", "0");
            DefaultValue.Add("uint16", "0");
            DefaultValue.Add("int32", "0");
            DefaultValue.Add("uint32", "0");
            DefaultValue.Add("int64", "0");
            DefaultValue.Add("uint64", "0");
            DefaultValue.Add("float", "0");
            DefaultValue.Add("double", "0");
            DefaultValue.Add("string", "\"\"");
            DefaultValue.Add("buffer", "\"\"");

            //写入函数
            DefaultWrite.Add("int8", "WriteInt8");
            DefaultWrite.Add("uint8", "WriteUInt8");
            DefaultWrite.Add("int16", "WriteInt16");
            DefaultWrite.Add("uint16", "WriteUInt16");
            DefaultWrite.Add("int32", "WriteInt32");
            DefaultWrite.Add("uint32", "WriteUInt32");
            DefaultWrite.Add("int64", "WriteInt64");
            DefaultWrite.Add("uint64", "WriteUInt64");
            DefaultWrite.Add("float", "WriteFloat");
            DefaultWrite.Add("double", "WriteDouble");
            DefaultWrite.Add("buffer", "WriteBytes");

            //读取函数
            DefaultRead.Add("int8", "ReadInt8");
            DefaultRead.Add("uint8", "ReadUInt8");
            DefaultRead.Add("int16", "ReadInt16");
            DefaultRead.Add("uint16", "ReadUInt16");
            DefaultRead.Add("int32", "ReadInt32");
            DefaultRead.Add("uint32", "ReadUInt32");
            DefaultRead.Add("int64", "ReadInt64");
            DefaultRead.Add("uint64", "ReadUInt64");
            DefaultRead.Add("float", "ReadFloat");
            DefaultRead.Add("double", "ReadDouble");
            DefaultRead.Add("buffer", "ReadBytes");

            //CS类型映射
            TypeToCS.Add("byte", "byte");
            TypeToCS.Add("int8", "sbyte");
            TypeToCS.Add("uint8", "byte");
            TypeToCS.Add("int16", "short");
            TypeToCS.Add("uint16", "ushort");
            TypeToCS.Add("int32", "int");
            TypeToCS.Add("uint32", "uint");
            TypeToCS.Add("int64", "long");
            TypeToCS.Add("uint64", "ulong");
            TypeToCS.Add("buffer", "byte");
        }

        /// <summary>
        /// 获取CS的类型名称。
        /// </summary>
        /// <param name="type">定义类型。</param>
        /// <returns>CS的类型名称</returns>
        public static string GetCSName(string type)
        {
            string name;
            if (TypeToCS.TryGetValue(type.ToLower(), out name))
            {
                return name;
            }
            return type;
        }

        /// <summary>
        /// 所属Builder。
        /// </summary>
        public Builder Belong;

        /// <summary>
        /// 类名称。
        /// </summary>
        public string Name;

        /// <summary>
        /// 类描述。
        /// </summary>
        public string Note;

        public int msgId;

        /// <summary>
        /// 上行，标记为1；下行标记为2；如果上下行都用到，标记为3。
        /// </summary>
        public int Dir;

        /// <summary>
        /// 字段列表。
        /// </summary>
        public List<FieldInfo> FieldInfos = new List<FieldInfo>();

        /// <summary>
        /// 从XML节点中赋值。
        /// </summary>
        /// <param name="xmlNode">XML节点。</param>
        public void AssignFromXmlNode(XmlNode node)
        {
            Name = XmlUtil.GetAttribute(node, "name");
            Note = XmlUtil.GetAttribute(node, "desc");
            msgId = XmlUtil.GetAttributeInt(node, "msgId");
            Dir = XmlUtil.GetAttributeInt(node, "dir");
            FieldInfos.Clear();
            foreach (XmlNode tmp in node.ChildNodes)
            {
                FieldInfo info = new FieldInfo();
                info.AssignFromXmlNode(tmp);
                FieldInfos.Add(info);
            }
        }

        /// <summary>
        /// 生成定义。
        /// </summary>
        /// <param name="sb">定义内容保存对象。</param>
        public void BuildDefineCS(StringBuilder sb)
        {
            string pre = Dir == 1 ? "C->S" : (Dir == 2 ? "S->C" : (Dir == 3 ? "C<->S" : string.Empty));
            sb.AppendLine(string.Format("\t// {0} {1}", pre, Note));
            if(Dir == 2 && msgId > 0)
            {
                sb.AppendLine("\t[NetUnPack]");
            }
            
            sb.AppendLine(string.Format("\tpublic class {0} : MsgData", Name));
            sb.AppendLine("\t{");
            
            //字段
            for (int i = 0; i < FieldInfos.Count; ++i)
            {
                FieldInfo info = FieldInfos[i];
                string tl = GetCSName(info.Type);
                if (info.IsArray)
                {
                    if (info.CountNumber > 0)
                    {
                        //定长的用数组
                        sb.AppendLine(string.Format("\t\tpublic {0}[] {1} = new {0}[{2}];\t// {3}", tl, info.Name, info.CountNumber, info.Note));
                    }
                    else
                    {
                        //不定长的用List
                        sb.AppendLine(string.Format("\t\tpublic List<{0}> {1} = new List<{0}>();\t// {2}", tl, info.Name, info.Note));
                    }                    
                }
                else
                {
                    sb.AppendLine(string.Format("\t\tpublic {0} {1};\t// {2}", tl, info.Name, info.Note));
                }
            }
            sb.AppendLine();

            ////重置
            //sb.AppendLine("\t\tpublic override void reset()");
            //sb.AppendLine("\t\t{");
            //for (int i = 0; i < FieldInfos.Count; ++i)
            //{
            //    //初始化字段的值
            //    FieldInfo info = FieldInfos[i];
            //    string tl = GetCSName(info.Type);
            //    if (info.IsArray)
            //    {
            //    }
            //    else
            //    {

            //    }
            //}
            //sb.AppendLine("\t\t}");
            //sb.AppendLine();

            //序列化
            if (Dir == 1 || Dir == 3)
            {
                sb.AppendLine("\t\tpublic override void pack(NetWriteBuffer buffer)");
                sb.AppendLine("\t\t{");
                
                for (int i = 0; i < FieldInfos.Count; ++i)
                {
                    FieldInfo info = FieldInfos[i];
                    if (DefaultValue.ContainsKey(info.Type))
                    {
                        //基础数据类型
                        string writefun = DefaultWrite[info.Type];
                        if (info.IsArray && info.Type.CompareTo("byte") != 0 && info.Type.CompareTo("buffer") != 0)
                        {
                            sb.AppendLine(string.Format("\t\t\tfor (int i = 0; i < {0}; i++)", info.Count));
                            sb.AppendLine("\t\t\t{");
                            sb.AppendLine(string.Format("\t\t\t\tbuffer.{0}({1}[i]);", writefun, info.Name));
                            sb.AppendLine("\t\t\t}");
                        }
                        else
                        {
                            sb.AppendLine(string.Format("\t\t\tbuffer.{0}({1});", writefun, info.Name));
                        }                        
                    }
                    else
                    {
                        //自定义数据类型
                        if (info.IsArray)
                        {
                            sb.AppendLine(string.Format("\t\t\tfor (int i = 0; i < {0}; i++)", info.Count));
                            sb.AppendLine("\t\t\t{");
                            sb.AppendLine(string.Format("\t\t\t\t{0} __item = {1}[i];", info.Type, info.Name));
                            sb.AppendLine(string.Format("\t\t\t\t__item.pack(buffer);"));
                            sb.AppendLine("\t\t\t}");
                        }
                        else
                        {
                            sb.AppendLine(string.Format("\t\t\t{0}.pack(buffer);", info.Name));
                        }
                    }
                }
                
                sb.AppendLine("\t\t}");
            }

            //反序列化
            if (Dir == 2 || Dir == 3)
            {
                
                if (msgId > 0)
                {
                    sb.AppendLine("\t\t[NetUnPackResponse(" + msgId + ")]");
                }

                var str = "";
                sb.AppendLine("\t\tpublic override void unpack(NetReadBuffer buffer)");
                sb.AppendLine("\t\t{");

                for (int i = 0; i < FieldInfos.Count; ++i)
                {
                    FieldInfo info = FieldInfos[i];
                    if (DefaultValue.ContainsKey(info.Type))
                    {
                        //基础数据类型
                        string readfun = DefaultRead[info.Type];
                        if (info.IsArray && info.Type.CompareTo("byte") != 0 && info.Type.CompareTo("buffer") != 0)
                        {
                            string tl = GetCSName(info.Type);
                            if (info.CountNumber <= 0)
                            {
                                sb.AppendLine(string.Format("\t\t\t{0} = new List<{1}>();", info.Name, tl));
                            }                                
                            sb.AppendLine(string.Format("\t\t\tfor (int i = 0; i < {0}; i++)", info.Count));
                            sb.AppendLine("\t\t\t{");
                            sb.AppendLine(string.Format("\t\t\t\t{0} __item = buffer.{1}();", tl, readfun));
                           
                            if (info.CountNumber <= 0)
                            {
                                sb.AppendLine(string.Format("\t\t\t\t{0}.Add(__item);", info.Name));
                                str += "\"" + info.Name + "\"[i]=__item";
                            }
                            else
                            {
                                sb.AppendLine(string.Format("\t\t\t\t{0}[i]=__item;", info.Name));
                                str += "\"" + info.Name + "\"[i]=__item";
                            }
                            sb.AppendLine("\t\t\t}");
                        }
                        else
                        {
                            if (info.IsArray)
                            {
                                if (info.CountNumber > 0)
                                {
                                    sb.AppendLine(string.Format("\t\t\tbuffer.{0}({1});", readfun, info.Name));
                                    

                                }
                                else
                                {
                                    sb.AppendLine(string.Format("\t\t\tbuffer.{0}({1}, (int){2});", readfun, info.Name, info.Count));
                                }
                            }
                            else
                            {
                                sb.AppendLine(string.Format("\t\t\t{1} = buffer.{0}();", readfun, info.Name));
                                str += "\"" + info.Name + "\"=info.Name";
                            }
                            
                        }
                    }
                    else
                    {
                        //自定义数据类型
                        if (info.IsArray)
                        {
                            if (info.CountNumber <= 0)
                            {
                                sb.AppendLine(string.Format("\t\t\t{0} = new List<{1}>();", info.Name, info.Type));
                            }                            
                            sb.AppendLine(string.Format("\t\t\tfor (int i = 0; i < {0}; i++)", info.Count));
                            sb.AppendLine("\t\t\t{");
                            sb.AppendLine(string.Format("\t\t\t\t{0} __item = new {0}();", info.Type));
                            sb.AppendLine(string.Format("\t\t\t\t__item.unpack(buffer);"));
                            if (info.CountNumber <= 0)
                            {
                                sb.AppendLine(string.Format("\t\t\t\t{0}.Add(__item);", info.Name));                                
                            }
                            else
                            {
                                sb.AppendLine(string.Format("\t\t\t\t{0}[i]=__item;", info.Name));
                            }                                
                            sb.AppendLine("\t\t\t}");
                        }
                        else
                        {
                            sb.AppendLine(string.Format("\t\t\t{0}.unpack(buffer);", info.Name));
                        }
                    }
                }

                sb.AppendLine("\t\t}");
            }

            sb.AppendLine("\t}");
        }

        /// <summary>
        /// 生成定义。
        /// </summary>
        /// <param name="sb">定义内容保存对象。</param>
        public void BuildDefineLua(StringBuilder sb)
        {
            //类型基础
            string pre = Dir == 1 ? "C->S" : (Dir == 2 ? "S->C" : (Dir == 3 ? "C<->S" : string.Empty));
            sb.AppendLine(string.Format("--{0} {1}", pre, Note));
            sb.AppendLine(string.Format("{0} = {{}}", Name));
            sb.AppendLine(string.Format("{0}.__index = {0}", Name));

            //字段
            List<FieldInfo> specialfield = new List<FieldInfo>();
            for (int i = 0; i < FieldInfos.Count; ++i)
            {
                FieldInfo info = FieldInfos[i];
                string tl = info.Type.ToLower();
                sb.AppendLine(string.Format("{0}.{1} = {2} --{3}", Name, info.Name, GetDefaultValueLua(info), info.Note));
                if (!DefaultValue.ContainsKey(tl) || (!string.IsNullOrEmpty(info.Count) && tl.CompareTo("buffer") != 0))
                {
                    specialfield.Add(info);
                }
            }
            
            //构造函数，特殊字段初始化
            sb.AppendLine();
            sb.AppendLine(string.Format("function {0}:New()", Name));
            sb.AppendLine("\tlocal o = {}");
            sb.AppendLine("\tsetmetatable(o, self)");
            if (specialfield.Count > 0)
            {
                sb.AppendLine();
                for (int i = 0; i < specialfield.Count; ++i)
                {
                    FieldInfo info = specialfield[i];
                    if (string.IsNullOrEmpty(info.Count))
                    {
                        sb.AppendLine(string.Format("\to.{0} = {1}:New()", info.Name, info.Type));
                    }
                    else
                    {
                        //数组
                        sb.AppendLine(string.Format("\to.{0} = {{}}", info.Name));
                        if (info.CountNumber > 0)
                        {
                            //定长直接补充数量
                            sb.AppendLine(string.Format("\tfor i=1,{0} do  --定长数组补齐", info.CountNumber));
                            sb.AppendLine(string.Format("\t\ttable.insert(o.{0}, {1})", info.Name, GetDefaultValueLua(info, true)));
                            sb.AppendLine(("\tend"));
                        }
                    }
                }
                sb.AppendLine();
            }            
            sb.AppendLine("\treturn o");
            sb.AppendLine("end");

            //序列化
            if (Dir == 1 || Dir == 3)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("function {0}:Pack(buffer)", Name));
                for (int i = 0; i < FieldInfos.Count; ++i)
                {
                    FieldInfo info = FieldInfos[i];
                    string tl = info.Type.ToLower();
                    string fun;
                    if (string.IsNullOrEmpty(info.Count))
                    {
                        if (DefaultWrite.TryGetValue(tl, out fun))
                        {
                            //基础数据
                            sb.AppendLine(string.Format("\tbuffer:{0}(self.{1})", fun, info.Name));
                        }
                        else if (tl.CompareTo("string") == 0)
                        {
                            //字符串
                            sb.AppendLine(string.Format("\tbuffer:WriteString(self.{0}, {1})", info.Name, info.Fixed));
                        }
                        else
                        {
                            //自定义类型
                            sb.AppendLine(string.Format("\tself.{0}:Pack(buffer)", info.Name));
                        }
                    }
                    else
                    {
                        //写入数组
                        string n = info.CountNumber > 0 ? info.CountNumber.ToString() : string.Format("self.{0}", info.Count);
                        sb.AppendLine(string.Format("\tfor i=1,{0} do", n));
                        if (DefaultWrite.TryGetValue(tl, out fun))
                        {
                            //基础数据
                            sb.AppendLine(string.Format("\t\tbuffer:{0}(self.{1}[i])", fun, info.Name));
                        }
                        else if (tl.CompareTo("string") == 0)
                        {
                            //字符串
                            sb.AppendLine(string.Format("\t\tbuffer:WriteString(self.{0}[i], {1})", info.Name, info.Fixed));
                        }
                        else
                        {
                            //自定义类型
                            sb.AppendLine(string.Format("\t\tself.{0}[i]:Pack(buffer)", info.Name));
                        }
                        sb.AppendLine(("\tend"));
                    }

                }
                sb.AppendLine("end");
            }

            //反序列化
            if (Dir == 2 || Dir == 3)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("function {0}:Unpack(buffer)", Name));
                for (int i = 0; i < FieldInfos.Count; ++i)
                {
                    FieldInfo info = FieldInfos[i];
                    string tl = info.Type.ToLower();
                    string fun;
                    if (string.IsNullOrEmpty(info.Count))
                    {
                        if (DefaultRead.TryGetValue(tl, out fun))
                        {
                            //基础数据
                            sb.AppendLine(string.Format("\tself.{1} = buffer:{0}()", fun, info.Name));
                        }
                        else if (tl.CompareTo("string") == 0)
                        {
                            //字符串
                            sb.AppendLine(string.Format("\tself.{0} = buffer:ReadString({1})", info.Name, info.Fixed));
                        }
                        else
                        {
                            //自定义类型
                            sb.AppendLine(string.Format("\tself.{0}:Unpack(buffer)", info.Name));
                        }
                    }
                    else
                    {
                        //读取数组
                        string n = info.CountNumber > 0 ? info.CountNumber.ToString() : string.Format("self.{0}", info.Count);
                        sb.AppendLine(string.Format("\tfor i=1,{0} do", n));
                        if (DefaultRead.TryGetValue(tl, out fun))
                        {
                            //基础数据
                            sb.AppendLine(string.Format("\t\ttable.insert(self.{1}, buffer:{0}())", fun, info.Name));
                        }
                        else if (tl.CompareTo("string") == 0)
                        {
                            //字符串
                            sb.AppendLine(string.Format("\t\ttable.insert(self.{0}, buffer:ReadString({1}))", info.Name, info.Fixed));
                        }
                        else
                        {
                            //自定义类型
                            if (info.CountNumber > 0)
                            {
                                //定长数据不用创建新对象
                                sb.AppendLine(string.Format("\t\tself.{0}[i]:Unpack(buffer)", info.Name));
                            }
                            else
                            {
                                sb.AppendLine(string.Format("\t\tlocal tmp = {0}:New()", info.Type));
                                sb.AppendLine(string.Format("\t\ttmp:Unpack(buffer)"));
                                sb.AppendLine(string.Format("\t\ttable.insert(self.{0}, tmp)", info.Name));
                            }                            
                        }
                        sb.AppendLine(("\tend"));
                    }
                }
                sb.AppendLine("end");
            }
        }

        /// <summary>
        /// 获取lua默认值。
        /// </summary>
        /// <param name="info">字段信息。</param>
        /// <param name="usenew">是否使用new创建新值,否则为nil。</param>
        /// <returns>默认值。</returns>
        public static string GetDefaultValueLua(FieldInfo info, bool usenew=false)
        {
            //处理数组，buffer不是数组，为定长字符串
            string tl = info.Type.ToLower();
            if (!usenew)
            {
                //new新值的化则忽略数组
                if (!string.IsNullOrEmpty(info.Count))
                {
                    //数组定义为空
                    return "nil";
                }
            }            

            //基础类型
            string ret;
            if (DefaultValue.TryGetValue(tl, out ret))
            {
                return ret;
            }

            //自定义类型
            return usenew ? string.Format("{0}:New()", info.Type) : "nil";
        }

        /// <summary>
        /// 获取数据读取代码。
        /// </summary>
        /// <param name="vname">变量名称。</param>
        /// <param name="type">数据类型。</param>
        /// <returns>读取代码。</returns>
        public string GetReadCode(string vname, string type)
        {
            string tl = type.ToLower();
            if (tl.CompareTo("bool") == 0)
            {
                return string.Format("{0} = dr.ReadBoolean();", vname);
            }
            else if (tl.CompareTo("int") == 0)
            {
                return string.Format("{0} = dr.ReadInt32();", vname);
            }
            else if (tl.CompareTo("float") == 0)
            {
                return string.Format("{0} = dr.ReadFloat();", vname);
            }
            else if (tl.CompareTo("string") == 0)
            {
                return string.Format("{0} = dr.ReadString();", vname);
            }

            ClassInfo cinfo = Belong.GetClassInfo(type);
            if (cinfo != null)
            {
                return string.Format("{0}.ReadData(dr);", vname);
            }

            return string.Empty;
        }

        /// <summary>
        /// 生成数组读取代码。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="retract"></param>
        /// <param name="vname"></param>
        /// <param name="type"></param>
        public void BuildArrayReadCode(StringBuilder sb, string retract, string vname, string type)
        {
            string tl = type.ToLower();
            if (tl.CompareTo("bool") == 0)
            {
                sb.AppendLine(string.Format("{0}{1}.Add(dr.ReadBoolean());", retract, vname));
            }
            else if (tl.CompareTo("int") == 0)
            {
                sb.AppendLine(string.Format("{0}{1}.Add(dr.ReadInt32());", retract, vname));
            }
            else if (tl.CompareTo("float") == 0)
            {
                sb.AppendLine(string.Format("{0}{1}.Add(dr.ReadFloat());", retract, vname));
            }
            else if (tl.CompareTo("string") == 0)
            {
                sb.AppendLine(string.Format("{0}{1}.Add(dr.ReadString());", retract, vname));
            }
            else
            {
                ClassInfo cinfo = Belong.GetClassInfo(type);
                if (cinfo != null)
                {
                    sb.AppendLine(string.Format("{0}{1} temp = new {1}();", retract, type));
                    sb.AppendLine(string.Format("{0}temp.ReadData(dr);", retract));
                    sb.AppendLine(string.Format("{0}{1}.Add(temp);", retract, vname));
                    return;
                }
            }
        }
    }
}
