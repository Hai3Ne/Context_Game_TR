using System;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using UnityEngine;


namespace SEZSJ
{
    //登陆阶段 消息类型（只是订阅解包类型，非具体消息处理）.

	public class registerMsgType_login
	{
		public static void init()
		{
			//NetMgr.netMgr.registerMsgType((int)NetMsgLoginDef.LoginSystem,(int)NetMsgLoginDef.enLoginSystem.sUserLogin,typeof(MsgData_sUserLogin)); 
		}

	}
    //角色阶段 消息类型（只是订阅解包类型，非具体消息处理）..

	public class registerMsgType_actor
	{
		public static void init()
		{
            //NetMgr.netMgr.registerMsgType((int)NetMsgLoginDef.LoginSystem, (int)NetMsgLoginDef.enLoginSystem.sActorList, typeof(MsgData_sActorList)); 
		}
		
	}

    
  //  public delegate void RegisterMsgTypeCall(LuaTable tb);

    //游戏阶段 消息类型（只是订阅解包类型，非具体消息处理）..

    public class registerMsgType_game
	{
		public static void init()
		{
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_HEART_BEAT, typeof(MsgData_sHeartBeat));


            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_LOGIN, typeof(MsgData_sLogin));
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_ROLEINFO, typeof(MsgData_sRoleInfo));
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_CREATEROLE, typeof(MsgData_sCreateRole));
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_ENTERGAME, typeof(MsgData_sEnterGame)); 
           

            //重连
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_RECONNECT, typeof(MsgData_sReconnect));
            NetMgr.netMgr.registerMsgType((Int16)NetMsgDef.S_COOKIE_UPDATE, typeof(MsgData_sCookieUpdate));
        } 
	}
}

