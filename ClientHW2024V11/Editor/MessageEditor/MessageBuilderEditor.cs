
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SEZSJ
{
    public class MessageBuilderEditor 
    {
        struct Message
        {
            public int nCode;
            public string ClassName;
        }
       
        static string PacketBuilderPath = Application.dataPath + "/HotUpdate/Socket/Net/Handles/PacketBuilder.cs";
        //------------------------------------------------------
        static bool IsGoogleIMessage(Type type)
        {
            if (!type.IsClass) return false;

            bool bMsg = false;
            if(type.BaseType == typeof(MsgData))
            {
               var s =  type.GetMethods(BindingFlags.Public| BindingFlags.Instance | BindingFlags.DeclaredOnly);
                for (int i = 0; i < s.Length; i++)
                {
                    var mem = s[i];
                    if(mem.Name == "unpack")
                    {
                        bMsg = true;
                        break;
                    }
                }
                
            }
            Type[] interfaces = type.GetInterfaces();
            
            if (interfaces == null) return false;
            for(int i = 0; i < interfaces.Length; ++i)
            {
                if (interfaces[i] == typeof(MsgData))
                {
                    bMsg = true;
                    break;
                }
            }

            if (bMsg /*type.IsSubclassOf(typeof(Google.Protobuf.IMessage))*/)
            {
                if (type.FullName.Contains("+Types")) return false;
                if (type.FullName.Contains("SEZSJ") && (type.FullName.Contains("Request") || type.FullName.Contains("Response"))) return true;
            }
            return false;
        }


        //------------------------------------------------------
        [MenuItem("Tools/消息协议")]
        public static void DoBuilder()
        {
            List<MethodInfo> vMethods = new List<MethodInfo>();
            HashSet<NetMsgDef> vMids = new HashSet<NetMsgDef>();
            foreach (NetMsgDef o in Enum.GetValues(typeof(NetMsgDef)))
            {
                vMids.Add(o);
            }
            List<Message> vMessageClass = new List<Message>();
            Assembly assembly = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.GetName().Name == "HotUpdate")
                {
                    assembly = ass;
                    Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; ++i)
                    {
                        Type tp = types[i];
                        if (tp.IsDefined(typeof(NetUnPackAttribute)))
                        {
                            MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                            for (int m = 0; m < meths.Length; ++m)
                            {
                                if (meths[m].IsDefined(typeof(NetUnPackResponseAttribute), false))
                                {
                                    vMethods.Add(meths[m]);
                                }
                            }
                        }
                    }

               }
            }

            if(vMethods.Count > 0)
            {
                vMethods.Sort((x, y) => ((NetUnPackResponseAttribute)x.GetCustomAttribute(typeof(NetUnPackResponseAttribute))).mid.CompareTo(((NetUnPackResponseAttribute)y.GetCustomAttribute(typeof(NetUnPackResponseAttribute))).mid));
            }
            System.DateTime nowTime = System.DateTime.Now;
            string date_time = string.Format("{0}:{1}:{2}   {3}:{4}", nowTime.Day, nowTime.Month, nowTime.Year, nowTime.Hour, nowTime.Minute);

            string code = "";
            code += "/********************************************************************\n";
            code += "生成日期:	" + date_time + "\n";
            code += "作    者:	" + "自动生成" + "\n";
            code += "描    述:\n";
            code += "*********************************************************************/\n";

            code += "using System;\n";
            code += "using UnityEngine;\n";
            code += "namespace SEZSJ\n";
            code += "{\n";
            code += "\tpublic static class PacketBuilder\n";
            code += "\t{\n";
            code += "\t\tpublic static MsgData newBuilder(int code, NetReadBuffer pDatas)\n";
            code += "\t\t{\n";
            code += "\t\t\tMsgData msgdata = null;\n";
            code += "\t\t\tswitch(code)\n";
            code += "\t\t\t{\n";
            for (int i = 0; i < vMethods.Count; ++i)
            {
                var attri = (NetUnPackResponseAttribute)vMethods[i].GetCustomAttribute(typeof(NetUnPackResponseAttribute));
                code += "\t\t\t\tcase " + attri.mid + ":\n";
                code += "\t\t\t\t{\n";
                // code += "\t\t\t\tGoogle.Protobuf.CodedInputStream pStream = new Google.Protobuf.CodedInputStream(pDatas, 0, nLenth);\n";
                // code += "\t\t\t\t" + vMessageClass[i].ClassName + " pMsg = new " + vMessageClass[i].ClassName + "();\n";
                // code += "\t\t\t\tpMsg.MergeFrom(pStream);\n";

                
                
                code += "\t\t\t\t\tmsgdata = Activator.CreateInstance(typeof(" + vMethods[i].DeclaringType.Name + ")) as MsgData;\n";
                code += "\t\t\t\t\tmsgdata.unpack(pDatas);\n";
                code += "\t\t\t\t\tbreak;\n";
                code += "\t\t\t\t}\n";
            }
            code += "\t\t\t\tdefault:\n";
            code += "\t\t\t\t\tDebug.LogWarning(\"not find msgid \" + code);\n";
            code += "\t\t\t\tbreak;\n";
            code += "\t\t\t}\n";
            code += "\t\t\treturn msgdata;\n";
            code += "\t\t}\n";
            code += "\t}\n";
            code += "}\n";

            if (File.Exists(PacketBuilderPath))
            {
                File.Delete(PacketBuilderPath);
            }
            FileStream fs = new FileStream(PacketBuilderPath, FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8);
            writer.Write(code);
            writer.Close();

            MessageHandlerEditor.DoBuilder();

            AssetDatabase.Refresh();
        }
    }

}

