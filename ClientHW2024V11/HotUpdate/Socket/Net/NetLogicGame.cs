
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;

using System.Text;

namespace SEZSJ
{

    

    public class NetLogicGame
    {

        static NetLogicGame mInstance;

        public static NetLogicGame Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new NetLogicGame();
                }

                return mInstance;
            }
        }

        public NetLogicGame()
        {
        }

        public bool IsCrossLinking = false;
       
        public static void str2Bytes(string strInput, byte[] output)
        {
            if (strInput.Length <= output.Length)
                System.Text.Encoding.UTF8.GetBytes(strInput).CopyTo(output, 0);
            else
                LogMgr.LogError("协议参数超过长度");
        }

        /// <summary>
        /// 返回创建角色结果
        /// </summary>
        /// <param name="msg"></param>
        public void OnCreateRole(MsgData msg)
        {
            Debug.Log("返回创建角色结果");
            
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_CREATE_ROLE, EventParameter.Get(msg));
        }

        /// <summary>
        /// 返回角色信息
        /// </summary>
        /// <param name="msg"></param>
        public void OnRoleInfo(MsgData msg)
        {
            Debug.Log("--------------------------------------------------");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ROLE_INFO, EventParameter.Get(msg));
        }

 
        /// <summary>
        /// 请求登录账号
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="platform"></param>
        /// <param name="game_name"></param>
        /// <param name="i4server_id"></param>
        /// <param name="i4time"></param>
        /// <param name="i4is_adult"></param>
        /// <param name="exts"></param>
        /// <param name="sign"></param>
        /// <param name="mac"></param>
        /// <param name="version"></param>
        /// <param name="pf"></param>
        public void SendReqcLogin(string accountID, // 玩家ID
            string platform, // 平台
            string game_name, // 游戏名
            uint i4server_id, // 区服ID
            uint i4time, // 时间
            uint i4is_adult, // 防沉迷标记
            string exts, // 扩展信息
            string sign, // 签名
            string mac, // 物理地址
            string version, // 协议版本
            string pf,// 渠道
            string strTxOpenId,
            string strTxOpenKey,
            string strTxPfKey)
        {
            MsgData_cLogin msgdata = new MsgData_cLogin();
            str2Bytes(accountID, msgdata.Account);
            str2Bytes(platform, msgdata.Platform);
            str2Bytes(game_name, msgdata.GameName);
            str2Bytes(exts, msgdata.Exts);
            str2Bytes(sign, msgdata.Sign);
            str2Bytes(mac, msgdata.Mac);
            str2Bytes(version, msgdata.Version);
            str2Bytes(pf, msgdata.Channel);
            str2Bytes(strTxOpenId, msgdata.TxOpenId);
            str2Bytes(strTxOpenKey, msgdata.TxOpenKey);
            str2Bytes(strTxPfKey, msgdata.TxPfKey);

            msgdata.ServerID = i4server_id;
            msgdata.ClientTime = i4time;
            msgdata.IsAdult = i4is_adult;

            NetMgr.netMgr.send(NetMsgDef.C_LOGIN, msgdata);
        }

        /// <summary>
        /// 请求进入游戏
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="ip"></param>
        /// <param name="mac"></param>
        /// <param name="openKey"></param>
        /// <param name="channel"></param>
        /// <param name="exts"></param>
        /// <param name="serverID"></param>
        /// <param name="loginType"></param>
        /// <param name="activityID"></param>
        /// <param name="pid"></param>
        public void SendReqEnterGame(string accountID, string ip, string mac, string openKey, string channel, string exts, int serverID, int loginType, int activityID, int pid, long rid, long AccountGUID)
        {
            LogMgr.Log("SendReqEnterGame: accountID : " + accountID);
            MsgData_cEnterGame data = new MsgData_cEnterGame();
            // data.AccountGUID = 0;
            data.AccountGUID = AccountGUID;
            //NetLogicGame.str2Bytes(ip, data.IP);
            //NetLogicGame.str2Bytes(mac, data.MAC);
            //NetLogicGame.str2Bytes(openKey, data.OpenKey);
            //NetLogicGame.str2Bytes(channel, data.Channel);
            //NetLogicGame.str2Bytes(exts, data.Exts);
           // NetLogicGame.str2Bytes(I2.Loc.LocalizationManager.CurrentLanguage, data.Language);

          //  string TxOpenId = "E5460BF460300BBA6A533C81C179E4CE";
            //string TxOpenKey = "C593CBD29841BE2E7D0BF3BB7B25C5FB";
            //string TxPfKey = "39941BC1CD79D7C4996AE96DF5A66B58";

           // string TxOpenId = "A9C8291868098A90EAA7021AEACD37F5";//正式QQ
           // string TxOpenKey = "30F9BDC114EA9DC16485F938A90262D9";
            //string TxPfKey = "30F9BDC114EA9DC16485F938A90262D9";

           // NetLogicGame.str2Bytes(TxOpenId, data.TxOpenId);
            //NetLogicGame.str2Bytes(TxOpenKey, data.TxOpenKey);
            //NetLogicGame.str2Bytes(TxPfKey, data.TxPfKey);

#if UNITY_ANDROID
            data.LoginType = 1;
#elif UNITY_EDITOR
            data.LoginType =1;
#elif UNITY_IOS
            data.LoginType = 2;
#endif
            // data.ServerID = serverID;
            //data.LoginType = loginType;
            // data.ActivityID = activityID;
            //data.PID = pid;
            data.SelectedRole = rid;
            //  data.AccountGUID = Account.Instance.AccountGuid;
            Debug.LogError(data.Channel);
            //data.Channel = null;
            NetMgr.netMgr.send(NetMsgDef.C_ENTERGAME, data);
            Debug.Log("-----------------------------");
            Debug.Log(data);
        }

        public void SendReqGetRechargeorder(
            int ItemID,int ItemNum,int serverID,string pid,string serverName
           )
        {
   /*         LogMgr.Log("ItemID:" + ItemID + " ItemNum:" + ItemNum + " serverID:" + serverID + " pid:" + pid + "  serverName: " + serverName);
            MsgData_cGetRechargeorder msgdata = new MsgData_cGetRechargeorder();
            msgdata.ItemID = ItemID;
            msgdata.ItemNum = ItemNum;
            msgdata.ServerID = serverID;
            msgdata.PidSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(pid).Length;
            msgdata.ServerNameSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(serverName).Length;
            str2Bytes(pid, msgdata.Pid);
            str2Bytes(serverName, msgdata.ServerName);
             
            NetMgr.netMgr.send(NetMsgDef.C_GETRECHARGEORDER, msgdata); */
        }

        /// <summary>
        /// DYB创建支付订单申请
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="ItemNum"></param>
        /// <param name="serverID"></param>
        /// <param name="pid"></param>
        /// <param name="serverName"></param>
        public void SendReqGetRechargeorder_DYB(
           int ItemID, int ItemNum, int serverID, string pid, string serverName
          )
        {
     /*       LogMgr.Log("ItemID:" + ItemID + " ItemNum:" + ItemNum + " serverID:" + serverID + " pid:" + pid + "  serverName: " + serverName);
            MsgData_cGetRechargeorder_DYB msgdata = new MsgData_cGetRechargeorder_DYB();
            msgdata.ItemID = ItemID;
            msgdata.ItemNum = ItemNum;
            msgdata.ServerID = serverID;
            msgdata.PidSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(pid).Length;
            msgdata.ServerNameSize = (sbyte)System.Text.Encoding.UTF8.GetBytes(serverName).Length;
            str2Bytes(pid, msgdata.Pid);
            str2Bytes(serverName, msgdata.ServerName);
            Debug.Log("发送第一拨订单申请");
            NetMgr.netMgr.send(NetMsgDef.C_CREATECHARGEORDER_DYB, msgdata);*/
        }

        /// <summary>
        /// 登录返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnLogin(MsgData msg)
        {
           MsgData_sLogin data = msg as MsgData_sLogin;
            Debug.LogError(" 登录返回 OnLogin ======= ");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_LOGIN_MSG, EventParameter.Get(msg));
            Debug.Log("------------------------------");
            Debug.Log(EventParameter.Get(msg));
        }

        /// <summary>
        /// 进入游戏返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnEnterGame(MsgData msg)
        {
            Debug.LogError("-----------OnEnterGame----------");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_ENTER_GAME, EventParameter.Get(msg));
        }

    


        /// <summary>
        /// 服务器通知对象死亡
        /// </summary>
        /// <param name="msg"></param>
        public void OnObjectDead(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_OBJ_DEAD, EventParameter.Get(msg));
        }



        public void SendReqCreateRole(string name,int job,int icon,long accountGUID)
        {
            MsgData_cCreateRole data = new MsgData_cCreateRole();
            str2Bytes(name, data.RoleName);
            str2Bytes("", data.Channel);
            str2Bytes("", data.Exts);
            data.AccountGUID = accountGUID;
            data.Job = job;
            data.Icon = icon;
            //data.AccountGUID = Account.Instance.AccountGuid;
            data.CurrentRoleID = 0;
            str2Bytes("", data.szIp);
            //str2Bytes("29E8FDD59BFA4ABE9CEC4F22986BFB29", data.TxOpenId);//
            //str2Bytes("BAF86E0590655215CDCF69177489885F", data.TxOpenKey);
           // str2Bytes("", data.TxPfKey);
            NetMgr.netMgr.send(NetMsgDef.C_CREATEROLE, data);
        }

        /// <summary>
        /// 请求离开游戏，回到角色选择界面
        /// </summary>
        public void SendReqLeaveGame()
        { 
            NetMgr.netMgr.send(NetMsgDef.C_LEAVE_GAME, new MsgData_cLeaveGame());
        }

        /// <summary>
        /// 返回离开游戏消息
        /// </summary>
        /// <param name="msg"></param>
        public void OnLeaveGame(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_SC_LEAVE_GAME, EventParameter.Get(msg));
        }

        // 连接结果回调，如果成功，则表示已连接并鉴权通过.
        public void onConnectedGame(bool isSuccessfull, IPEndPoint remote)
        {
            if (isSuccessfull == true)
            {
                NetMgr.netMgr.bindMsgHandler(NetMsgDef.S_LOGIN, OnLogin);
                NetMgr.netMgr.bindMsgHandler(NetMsgDef.S_ROLEINFO, OnRoleInfo);
                NetMgr.netMgr.bindMsgHandler(NetMsgDef.S_CREATEROLE, OnCreateRole);
                NetMgr.netMgr.bindMsgHandler(NetMsgDef.S_ENTERGAME, OnEnterGame);

                NetMgr.netMgr.bindMsgHandler(NetMsgDef.S_LEAVE_GAME, OnLeaveGame);

                NetMgr.netMgr.Reconnect.RegisterNetMsg();

                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECTINFO, EventParameter.Get((int)NetMgr.ConnectInfo.connectedlogin_Successfull));

       
            }
            else
            {
                LogMgr.UnityError("connect game server failed!");
            }
        }

    

        
        // 连接关闭
        public void onConnectedCloseGame()
        {
            LogMgr.UnityWarning("connection close! please reconnect!");
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECT_CLOSE, null);
        }

        public void onConnectedError(int errorCode)
        {
           // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECT_CLOSE, null);

        }

        // 连接游戏网关
        public void connectGame(string host, short port)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_CONNECTINFO, EventParameter.Get((int)NetMgr.ConnectInfo.connectgame));
        
            Debug.Log("connectGame:" + host + ":" + port);
            LogMgr.UnityLog("connectGame...");
             
            NetMgr.netMgr.connect((int)NetMgr.Page.game, host, (int)(port));
            NetMgr.netMgr.bindConnect(onConnectedGame);
            NetMgr.netMgr.Reconnect.BindConnect();
            NetMgr.netMgr.bindConnectClose(onConnectedCloseGame);
            NetMgr.netMgr.bindConnectError(onConnectedError);

        }

        //移除回调
        public void RemoveConnectCB()
        {
            NetMgr.netMgr.removeConnectCB(onConnectedGame);
            NetMgr.netMgr.removeConnectCloseCB(onConnectedCloseGame);
            NetMgr.netMgr.removeConnectErrorCB(onConnectedError);
        }
        //添加回调
        public void AddConnectCB()
        {
            NetMgr.netMgr.addConnectCB(onConnectedGame);
            NetMgr.netMgr.addConnectCloseCB(onConnectedCloseGame);
            NetMgr.netMgr.addConnectErrorCB(onConnectedError);
        }
        //取得当前设置的Host
        public string GetNowConnectHost()
        {
            return NetMgr.netMgr.getSettedHost();
        }
        //取得当前设置的IP
        public int GetNowConnectPort()
        {
            return NetMgr.netMgr.getSettedPort();
        }
        //跨服连接
        public void ConnectCross(string szHost, short port)
        {
            LogMgr.UnityLog("//跨服连接...");

            IsCrossLinking = true;

            RemoveConnectCB();
            NetMgr.netMgr.forceClose();
            NetMgr.netMgr.connect((int)NetMgr.Page.game, szHost, (int)(port));
        }
        //跨服战结束后, 本服重新连接
        public void ConnectBackByCrossEnd(string szHost, short port)
        {
            LogMgr.UnityLog("//跨服战结束后, 本服重新连接");

            NetMgr.netMgr.forceClose();
            NetMgr.netMgr.connect((int)NetMgr.Page.game, szHost, (int)(port));

            //IsCrossLinking = false;
        }





    



        /// <summary>
        /// 任务列表返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnC_QueryQuest(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_TASK_Initiaze, EventParameter.Get(msg));
        }


        /// <summary>
        /// 玩家进入游戏房间请求
        /// </summary>
        public void Send_ENTER_GAME_REQ(int nGameType,int nRoomType)
        {
           CS_HUMAN_ENTER_GAME_REQ data = new CS_HUMAN_ENTER_GAME_REQ();
            data.i4GameType = nGameType;
            data.i4RoomType = nRoomType;

            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_ENTER_GAME_REQ, data);
        }

        /// <summary>
        /// 玩家进入游戏房间请求返回
        /// </summary>
        /// <param name="msg"></param>
        public void OnEnterRoomRet(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_EnterRoomRet500, EventParameter.Get(msg));
        }

        /// <summary>
        /// 请求离开房间500
        /// </summary>
        public void Send_CS_HUMAN_LEAVE_GAME_REQ()
        {
            CS_HUMAN_LEAVE_GAME_REQ data = new CS_HUMAN_LEAVE_GAME_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_LEAVE_GAME_REQ, data);
        }

        /// <summary>
        /// 请求离开房间500
        /// </summary>
        public void OnLeaveGameRet500(MsgData msg)
        {
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_LeaveRoomRet500, EventParameter.Get(msg));
        }

        /// <summary>
        /// 请求500房间下注
        /// </summary>
        /// <param name="BET"></param>
        public void Send_CS_GAME1_BET_REQ(int bet)
        {
            CS_GAME1_BET_REQ data = new CS_GAME1_BET_REQ();
            data.nBet = bet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME1_BET_REQ, data);
        }

        /// <summary>
        /// 请求500房间下注结果返回
        /// </summary>
        /// <param name="BET"></param>
        public void OnBetGameResult500(MsgData msg)
        {
           // LuaEnv luaEnv = LuaMgr.Instance.GetLuaEnv();

      

            SC_GAME1_BET_RET temp = new SC_GAME1_BET_RET();
            //temp.arrayLogo = new List<byte> { 1, 2, 3, 5, 4, 6, 7, 8, 9, 5, 4, 2, 1, 5, 2 };
            msg = temp;
            //LuaTable luaTable = luaEnv.NewTable();
            //for (int i = 0; i < temp.arrayLogo.Length; i++)
            //{
            //    //lua table 下标从1开始的
            //    luaTable.Set(i + 1, temp.arrayLogo[i]);
            //}
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, EventParameter.Get(msg));
        }

    }

}

