using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SEZSJ
{
    public class MessageHandlerEditor
    {
        struct Message
        {
            public int nCode;
            public string ClassName;
        }
        static string PacketBuilderPath = Application.dataPath + "/HotUpdate/Socket/Net/Handles/NetHandlerRegister.cs";
        //------------------------------------------------------
        public static void DoBuilder()
        {
            HashSet<MethodInfo> vMethods = new HashSet<MethodInfo>();
            Assembly assembly = null;
            foreach (var ass in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                assembly = ass;
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type tp = types[i];
                    if (tp.IsDefined( typeof(NetHandlerAttribute) ))
                    {
                        MethodInfo[] meths = types[i].GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        for (int m = 0; m < meths.Length; ++m)
                        {
                            if (meths[m].IsDefined(typeof(NetResponseAttribute), false))
                            {
                                vMethods.Add(meths[m]);
                            }
                        }
                    }
                }
            }

            System.DateTime nowTime = System.DateTime.Now;
            string date_time = string.Format("{0}:{1}:{2}   {3}:{4}", nowTime.Day, nowTime.Month, nowTime.Year, nowTime.Hour, nowTime.Minute);

            string code = "";
            code += "/********************************************************************\n";
            code += "作    者:	" + "自动生成" + "\n";
            code += "描    述:\n";
            code += "*********************************************************************/\n";

            code += "namespace SEZSJ\n";
            code += "{\n";
            code += "\tpublic static class NetHandlerRegister\n";
            code += "\t{\n";
            code += "\t\tpublic static void Init()\n";
            code += "\t\t{\n";


            HashSet<NetMsgDef> vSets = new HashSet<NetMsgDef>();
            foreach(var method in vMethods)
            {
                NetResponseAttribute attr = (NetResponseAttribute)method.GetCustomAttribute(typeof(NetResponseAttribute));
                if (attr == null) continue;

                if (vSets.Contains(attr.mid))
                {
                    Debug.LogWarning("具有两个相同的消息码,请检查:[mid=" + attr.mid + "]" + method.DeclaringType.FullName + "::" + method.Name);
                    continue;
                }

                vSets.Add(attr.mid);
                code += "\t\t\tNetHandler.Instance.Register((int)NetMsgDef." + attr.mid +"," + method.DeclaringType.FullName + ".Instance." + method.Name + ");\r\n";
            }
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
        }
    }

}

