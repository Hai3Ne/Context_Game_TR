using System;
using System.Collections.Generic;

//====================================================
//           网络命令ID和结构体映射
//====================================================

public class NetTypeInfo
{
	public NetCmdType   CmdType;
	public Type         StructType;
	public ushort       StructSize;
	public int          TypeHashCode;
}
public  class NetTypeCreater<T> where T : NetCmdBase, new()
{
	public static NetTypeInfo GetTypeInfo(NetCmdType type)
	{
		ushort size = (ushort)TypeSize<T>.SIZE;
		NetTypeInfo nti = new NetTypeInfo();
		nti.CmdType     = type;
		nti.StructType  = TypeSize<T>.RUN_TYPE;
		nti.StructSize  = size;
		nti.TypeHashCode = TypeSize<T>.HASH;
		return nti;
	}
}

public partial class NetCmdMapping
{
    public static Dictionary<NetCmdType, NetTypeInfo> ms_TypeMapList = new Dictionary<NetCmdType,NetTypeInfo>();
    static public int GetMapCount()
    {
        return ms_TypeMapList.Count;
    }
    static public bool IsRegisterCmd(NetCmdType cmdType)
    {
        return ms_TypeMapList.ContainsKey(cmdType);
    }
    static public NetTypeInfo GetTypeInfo(NetCmdType cmdType)
    {
        NetTypeInfo info ;
        if(ms_TypeMapList.TryGetValue(cmdType, out info))
            return info;
        else
            return null;
    }
    static public bool InitCmdTypeInfo<T>(NetCmdType cmdType) where T : NetCmdBase, new()
    {
        TypeMapping.RegisterClassFromType<T>();
        if (ms_TypeMapList.ContainsKey(cmdType)) {
            //Debuger.LogError("重复注册:" + cmdType.ToString() + " .. " + ms_TypeMapList[cmdType].StructType.ToString());
            return false;
        }
        NetTypeInfo nti = NetTypeCreater<T>.GetTypeInfo(cmdType);
        ms_TypeMapList.Add(cmdType, nti);
        return true;
    }

    static public void GlobalInit()
    {
        bool bRet = TypeMapping.GlobalInit();
		TypeMapping.RegisterClassFromType<NetCmdBase>();
		TypeMapping.RegisterClassFromType<tagUserInfoHead> ();
		TypeMapping.RegisterClassFromType<tagUserinfoExt> ();
		TypeMapping.RegisterClassFromType<tagUserStatus> ();
		NetCmdRegister.RegisterCmd ();
    }

   
}
 
 
 
 







