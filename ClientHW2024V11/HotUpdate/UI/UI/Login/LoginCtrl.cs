
using I2.Loc.SimpleJSON;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace HotUpdate
{
    [NetHandler]
    public class LoginCtrl : Singleton<LoginCtrl>
    {
        private string ip = "18.162.135.99"; // main"16.162.236.155";// test"43.199.166.178";//"18.163.221.99"; //"192.168.0.161"; //"18.163.221.99";//  "192.168.0.123";//    "116.62.137.36";////"54.94.240.56";//177.71.192.232"192.168.0.123";//"116.62.137.36";//"116.62.137.36";//"116.62.137.36";//
        private short port = 8200;

        public bool isEnterGame = false;
        private bool isLogin = false;
        /// <summary>
        /// 游戏类型
        /// </summary>
        public int GameType = 0;
        /// <summary>
        /// 房间类型
        /// </summary> 
        public int RoomType = 0;
        /// <summary>
        /// 是否在游戏中
        /// </summary>
        public bool InGame = false;

        public bool isConnet = false;

        public bool isShowAb = false;
        public const string pwdKey = "pud4tIdkyRQ8zl9O";
        public bool isBindAccount;
        public bool isPhoneBind;
        public bool isConnted = false;
        public int loginType = 0;
        private long roleId = 0;
        private long Guid = 0;
        private bool isShowLogin = false;
        private List<string> updateFileArr = new List<string>();
        public int channelId = 1;
        public string payLimit = "0";
        public LoginCtrl()
        {

        }
        public void init()
        {
            isShowAb = false;
            channelId = SdkCtrl.Instance.getChannle();
            SdkCtrl.Instance.checkUSB();
            Debug.Log("---------------------checkAb-----------------------" + isShowAb);
            RegisterListener();

        }

        public void initLogin()
        {
            if (!isEnterGame)
            {
                isLogin = true;
                disconnect();
                CoreEntry.gTimeMgr.AddTimer(1.0f, false, ReConnect, 632156);
                //  StartConnectServer();
            }
            else
            {

                WinUpdate.isCanOpen = true;
                isLogin = true;
                disconnect();
                CoreEntry.gTimeMgr.AddTimer(1.0f, false, ReConnect, 632156);
                //StartConnectServer();
            }

        }

        public void RegisterListener()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, OpenView);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CONNECTINFO, ConnectInfo);
            //CoreEntry.gEventMgr.AddListener(GameEvent.GE_EndPause, EndPause);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_CONNECT_CLOSE, EndPause);
            CoreEntry.gEventMgr.AddListener(GameEvent.Ge_InGame_Connect, EndPause);
            Message.AddListener(MessageName.NET_INIT, StartConnectServer);

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_Focus_Off, OffFocus);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_Focus_On, OnFocus);
        }

        public void UnRegisterListener()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, OpenView);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CONNECTINFO, ConnectInfo);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CONNECT_CLOSE, EndPause);
            Message.RemoveListener(MessageName.NET_INIT, StartConnectServer);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.Ge_InGame_Connect, EndPause);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_Focus_Off, OffFocus);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_Focus_On, OnFocus);
        }

        /// <summary>
        /// 后台切出去
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OffFocus(GameEvent ge, EventParameter parameter)
        {

        }

        /// <summary>
        /// 后台切回来
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        private void OnFocus(GameEvent ge, EventParameter parameter)
        {
            if (!HotStart.ins.m_isShow)
            {
                return;
            }
            if (SdkCtrl.Instance.checkUSB())
            {

                disconnect();
                return;
            }

            var time = parameter.floatParameter;
            if (time > 60f)
            {
                EndPause(ge, null);
            }
        }

        private void EndPause(GameEvent ge, EventParameter parameter)
        {
            if (isShowAb) return;
            UICtrl.Instance.ShowLoading(true);
            disconnect();
            isConnet = true;
            CoreEntry.gTimeMgr.AddTimer(2.0f, false, ReConnect, 632156);


        }

        public void disconnect()
        {
            NetMgr.netMgr.disconnect();
            isConnted = false;

        }


        public GameObject VersionPanel = null;
        public bool isUpdate = false;
        public void StartConnectServer()
        {
            isUpdate = Application.version != "1.0.0";
            if (isUpdate && VersionPanel == null)
            {

                var obj = CoreEntry.gResLoader.Load("UI/Prefabs/Version/FirstRes/VersionPanel");
                var obj1 = GameObject.Instantiate(obj);
                obj1.transform.SetParent(MainPanelMgr.Instance.uUIRootObj.transform);
                var canvas = obj1.GetComponent<Canvas>();
                canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                VersionPanel = obj;

            }
            if (isShowAb)
            {
                isEnterGame = true;
                OpenView(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, null);
            }
            else
            {
                if (!HotStart.ins.m_isShow)
                {
#if UNITY_EDITOR
                    // UICtrl.Instance.OpenView("LoginPanel");
                    HotStart.ins.CloseView();
                    UICtrl.Instance.CloseLoading();
                    MainPanelMgr.Instance.ShowPanel("MainUIPanel");
                    MainPanelMgr.Instance.GetPanel("MainUIPanel").Canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
#else
// if (PlayerPrefs.HasKey("WxLoginToken")){
//                     HotStart.ins.CloseView();
//                     UICtrl.Instance.CloseLoading();
//                     MainPanelMgr.Instance.ShowPanel("MainUIPanel");
//                     MainPanelMgr.Instance.GetPanel("MainUIPanel").Canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
// }else{
UICtrl.Instance.OpenView("LoginPanel");
// }
                    
#endif
                    return;
                }
                UICtrl.Instance.ShowLoading(isConnet);  
                if ( !GameConst.isEditor)
                {
                    var list = HotStart.ins.m_port.Split('|');
                    int randomNum = UnityEngine.Random.Range(0, list.Length);
                    port = short.Parse(list[randomNum]);
                    ip = HotStart.ins.m_ip;

                }
                // NetLogicGame.Instance.connectGame(ip,port);
                NetLogicGame.Instance.connectGame("192.168.0.161",8200);
            }
        }

        private void ReConnect()
        {
            CoreEntry.gTimeMgr.RemoveTimer(632156);
            StartConnectServer();
            Message.Broadcast(MessageName.OPEN_LOGINBTN);
        }

        private void ConnectInfo(GameEvent ge, EventParameter parameter)
        {
            if (parameter.intParameter == 8)
            {
                isConnted = true;
                if (isLogin)
                {
                    if (isConnet)
                    {
                        isConnet = false;
                        Message.Broadcast(MessageName.GAME_RECONNET);
                        UICtrl.Instance.CloseLoading();
                    }
                    else
                    {
                        UICtrl.Instance.CloseLoading();
                    }
                }
                else
                {
                    if (SdkCtrl.Instance.getChannle() == 9999)
                    {
                        UICtrl.Instance.OpenView("LoginPanel");
                        return;
                    }
#if !UNITY_EDITOR

                    if (PlayerPrefs.HasKey("WxLoginToken"))
                    {
                        var token = PlayerPrefs.GetString("WxLoginToken");
                        var code = ToolUtil.GetRandomCode(12);
                        var hash = ToolUtil.HMACSHA1(code, pwdKey);
                        SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", channelId + "", "", "", "", token);
                    }
                    else
                    {
                        UICtrl.Instance.OpenView("LoginPanel");
                    }
#else
                    UICtrl.Instance.OpenView("LoginPanel");
                    return;
                    var code = ToolUtil.GetRandomCode(12);
                    var hash = ToolUtil.HMACSHA1(code, pwdKey);
                    SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", channelId + "", "", "", "", SystemInfo.deviceUniqueIdentifier);
#endif

                }

            }
        }

        public void ClickLogin(string wxcode = "")
        {
            var code = ToolUtil.GetRandomCode(12);
            var hash = ToolUtil.HMACSHA1(code, pwdKey);
            SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", channelId + "", "", "", "", wxcode);

        }

        private void OpenView(GameEvent ge, EventParameter parameter)
        {
            Debug.Log("进入游戏1" + isEnterGame + "|" + WinUpdate.isCanOpen + "|" + isShowLogin);
            WinUpdate.isCanOpen = true;
            if (WinUpdate.isCanOpen && isEnterGame)
            {
                HotStart.ins.CloseView();
                if (isShowLogin)
                {
                    JumpToLoginPanel();
                }
                else
                {
                    if (isShowAb)
                    {
                        UICtrl.Instance.OpenView("UIRoom500", true);
                    }
                    else
                    {
                        UICtrl.Instance.OpenView("MainUIPanel");
                        MainPanelMgr.Instance.GetPanel("MainUIPanel").Canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                    }
                    isLogin = false;
                    WinUpdate.isCanOpen = false;
                }


            }
        }

#region 请求

        /// <summary>
        /// 登陸請求
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
        /// <param name="strTxOpenId"></param>
        /// <param name="strTxOpenKey"></param>
        /// <param name="strTxPfKey"></param>
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
            string strTxPfKey,
            string wxCode)
        {
            MsgData_cLogin msgdata = new MsgData_cLogin();
            string deviceId = wxCode;
            
#if UNITY_EDITOR
            deviceId = "HWCNDY34_4" + SystemInfo.deviceUniqueIdentifier;
#endif
      

            accountID = deviceId;
            Debug.LogError(accountID);
            ToolUtil.str2Bytes(accountID, msgdata.Account);
            ToolUtil.str2Bytes(platform, msgdata.Platform);
            ToolUtil.str2Bytes(game_name, msgdata.GameName);
            ToolUtil.str2Bytes(exts, msgdata.Exts);
            ToolUtil.str2Bytes(sign, msgdata.Sign);
            ToolUtil.str2Bytes(mac, msgdata.Mac);
            ToolUtil.str2Bytes(version, msgdata.Version);
            ToolUtil.str2Bytes(pf, msgdata.Channel);
            ToolUtil.str2Bytes(strTxOpenId, msgdata.TxOpenId);
            ToolUtil.str2Bytes(strTxOpenKey, msgdata.TxOpenKey);
            ToolUtil.str2Bytes(strTxPfKey, msgdata.TxPfKey);
            msgdata.ServerID = i4server_id;
            msgdata.ClientTime = i4time;
            msgdata.IsAdult = i4is_adult;
            loginType = 0;
            NetMgr.netMgr.send(NetMsgDef.C_LOGIN, msgdata); 
        }

        /// <summary>
        /// 创角
        /// </summary>
        /// <param name="name"></param>
        /// <param name="job"></param>
        /// <param name="icon"></param>
        /// <param name="accountGUID"></param>
        public IEnumerator SendReqCreateRole(string name, int job, int icon, long accountGUID)
        {
/*#if UNITY_ANDROID && !UNITY_EDITOR
            yield return new WaitWhile(()=> !GameBegin.ins.isSdkInit);
#endif*/
            var str = "0123456789qwertyuiopasdfghjklzxcvbnm";
            var str1 = "";
            for (int i = 0; i < 7; i++)
            {
                str1 += str[UnityEngine.Random.Range(0, str.Length)];
            }
            name = str1;
            MsgData_cCreateRole data = new MsgData_cCreateRole();
            System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
          //  BindShareID();
            ToolUtil.str2Bytes(name + startTime.Ticks, data.RoleName);
     
            ToolUtil.str2Bytes(shareId, data.Channel);
            ToolUtil.str2Bytes("", data.Exts);
            data.AccountGUID = accountGUID;
            data.Job = job;
            data.Icon = icon;

            data.CurrentRoleID = 0;
            ToolUtil.str2Bytes("", data.szIp);

            NetMgr.netMgr.send(NetMsgDef.C_CREATEROLE, data);
            yield break;
        }
        string shareId = "";
        
        /// <summary>
        /// 請求進入游戏
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
        /// <param name="rid"></param>
        /// <param name="AccountGUID"></param>
        public void SendReqEnterGame(string accountID, string ip, string mac, string openKey, string channel, string exts, int serverID, int loginType, int activityID, int pid, long rid, long AccountGUID)
        {
            LogMgr.Log("SendReqEnterGame: accountID : " + accountID);
            MsgData_cEnterGame data = new MsgData_cEnterGame();
            data.AccountGUID = AccountGUID;
            ToolUtil.str2Bytes("English", data.Language);
#if UNITY_ANDROID
            data.LoginType = 1;
#elif UNITY_EDITOR
            data.LoginType = 1;
#elif UNITY_IOS
            data.LoginType = 2;
#endif
            // data.ServerID = serverID;
            //data.LoginType = loginType;
            // data.ActivityID = activityID;
            //data.PID = pid;

            data.SelectedRole = rid;
            ToolUtil.str2Bytes(channelId + "", data.Channel);
            //  data.AccountGUID = Account.Instance.AccountGuid;
            //data.Channel = null;
            NetMgr.netMgr.send(NetMsgDef.C_ENTERGAME, data);
        }

        public void SendPhoneLogin(
            string phone,//手机号
            string pwd,//密码
            string platform, // 平台
            string game_name, // 游戏名
            int i4server_id, // 区服ID
            int i4time, // 时间
            int i4is_adult, // 防沉迷标记
            string exts, // 扩展信息
            string sign, // 签名
            string mac, // 物理地址
            string version, // 协议版本
            string pf,// 渠道
            string strTxOpenId,
            string strTxOpenKey,
            string strTxPfKey)
        {
            CL_CONN_SRV_PHONE msgdata = new CL_CONN_SRV_PHONE();
            ToolUtil.str2Bytes(phone, msgdata.m_szPhone);
            var encryptPwd = Encoding.Default.GetBytes(pwd);
            PacketUtil.Encrypt(encryptPwd, encryptPwd.Length);
            encryptPwd.CopyTo(msgdata.m_szPswd, 0);
            ToolUtil.str2Bytes(platform, msgdata.Platform);
            ToolUtil.str2Bytes(game_name, msgdata.GameName);
            ToolUtil.str2Bytes(exts, msgdata.Exts);
            ToolUtil.str2Bytes(sign, msgdata.Sign);
            ToolUtil.str2Bytes(mac, msgdata.Mac);
            ToolUtil.str2Bytes(version, msgdata.Version);
            ToolUtil.str2Bytes(pf, msgdata.Channel);
            ToolUtil.str2Bytes(strTxOpenId, msgdata.TxOpenId);
            ToolUtil.str2Bytes(strTxOpenKey, msgdata.TxOpenKey);
            ToolUtil.str2Bytes(strTxPfKey, msgdata.TxPfKey);

            msgdata.ServerID = i4server_id;
            msgdata.ClientTime = i4time;
            msgdata.IsAdult = i4is_adult;
            isPhoneBind = true;
            loginType = 1;
            NetMgr.netMgr.send(NetMsgDef.CL_CONN_SRV_PHONE, msgdata);
        }

        /// <summary>
        /// 请求修改密码
        /// </summary>  
        /// <param name="code"></param>l
        /// <param name="pwd"></param>
        public void SendChangePWD(string phone, string code, string pwd)
        {
            CL_ChangePwdForAccountReq data = new CL_ChangePwdForAccountReq();
            ToolUtil.str2Bytes(phone, data.m_szPhone);
            ToolUtil.str2Bytes(code, data.m_szCode);
            var encryptPwd = Encoding.Default.GetBytes(pwd);
            PacketUtil.Encrypt(encryptPwd, encryptPwd.Length);
            encryptPwd.CopyTo(data.m_szPswd, 0);
            NetMgr.netMgr.send(NetMsgDef.CL_ChangePwdForAccountReq, data);
        }
        /// <summary>
        /// 请求修改账号密码短信验证
        /// </summary>
        public void SendChangePwdSms(string phone)
        {
            CL_ChangePwdForAccountSmsgReq data = new CL_ChangePwdForAccountSmsgReq();
            ToolUtil.str2Bytes(phone, data.m_szPhone);
            NetMgr.netMgr.send(NetMsgDef.CL_ChangePwdForAccountSmsgReq, data);
        }

        /// <summary>
        /// 注册账号请求
        /// </summary>
        public void SendRegisterAccount(
            string phone,//手机
            string code,//验证码
            string pwd, //密码
            string platform, // 平台
            string game_name, // 游戏名
            int i4server_id, // 区服ID
            int i4time, // 时间
            int i4is_adult, // 防沉迷标记
            string exts, // 扩展信息
            string sign, // 签名
            string mac, // 物理地址
            string version, // 协议版本
            string pf// 渠道
            )
        {
            CL_RegisterAccountReq data = new CL_RegisterAccountReq();
            ToolUtil.str2Bytes(phone, data.m_szPhone);
            ToolUtil.str2Bytes(code, data.m_szCode);
            var encryptPwd = Encoding.Default.GetBytes(pwd);
            PacketUtil.Encrypt(encryptPwd, encryptPwd.Length);
            encryptPwd.CopyTo(data.m_szPswd, 0);
            ToolUtil.str2Bytes(platform, data.m_platform);
            ToolUtil.str2Bytes(game_name, data.m_game_name);
            ToolUtil.str2Bytes(exts, data.m_exts);
            ToolUtil.str2Bytes(sign, data.m_sign);
            ToolUtil.str2Bytes(mac, data.m_mac);
            ToolUtil.str2Bytes(version, data.m_version);
            ToolUtil.str2Bytes(pf, data.m_pf);
            NetMgr.netMgr.send(NetMsgDef.CL_RegisterAccountReq, data);

        }
        /// <summary>
        /// 请求注册账号短信验证码
        /// </summary>
        public void SendRegisterAccountSms(string phone)
        {
            CL_RegisterAccountSmsgReq data = new CL_RegisterAccountSmsgReq();
            ToolUtil.str2Bytes(phone, data.m_szPhone);
            NetMgr.netMgr.send(NetMsgDef.CL_RegisterAccountSmsgReq, data);
        }

#endregion

#region 接收
        [NetResponse(NetMsgDef.S_LOGIN)]
        public void LoginRecevied(MsgData msgData)
        {
            MsgData_sLogin data = (MsgData_sLogin)msgData;
            LoginModel.Instance.loginData = data;
            if (data.ResultCode == -1)
            {

                Guid = data.AccountGUID;
                Debug.LogError("没有任何角色 进入角色创建");
                UICtrl.Instance.StartCoroutine(SendReqCreateRole("C718974587ABCDEFG", 1, 3, data.AccountGUID));
            }
            else if (data.ResultCode == 0)
            {
                Guid = data.AccountGUID;
                Debug.Log($"<color=#ffff00>成功</color>");
                if (MainPanelMgr.Instance.IsShow("LoginDaContaPanel"))
                {
                    CloseLoginDaContaPanel();
                }
            }
            else if (data.ResultCode == -2)
            {
                Debug.Log($"<color=#ffff00>时间戳错误</color>");
                //JumpToLoginPanel();
            }
            else if (data.ResultCode == -3)
            {
                Debug.Log($"<color=#ffff00>签名不匹配</color>");
                //JumpToLoginPanel();
            }
            else if (data.ResultCode == -4)
            {
                Debug.Log($"<color=#ffff00>封停</color>");
                ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
            }
            else if (data.ResultCode == -5)
            {
                Debug.Log($"<color=#ffff00>协议不一致</color>");
                //JumpToLoginPanel();
            }
            else if (data.ResultCode == -6)
            {
                Debug.Log($"<color=#ffff00>MAC封禁</color>");
                //JumpToLoginPanel();
            }
            else if (data.ResultCode == -7)
            {
                Debug.Log($"<color=#ffff00>区服错误</color>");
                ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
            }
            else if (data.ResultCode == -8)
            {
                Debug.Log($"<color=#ffff00>手机号码错误</color>");
                if (MainPanelMgr.Instance.IsShow("LoginDaContaPanel"))
                {
                    ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                }
                else
                {
                    JumpToLoginPanel();

                }
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
            }
            else if (data.ResultCode == -9)
            {
                Debug.Log($"<color=#ffff00>手机密码错误</color>");
                if (MainPanelMgr.Instance.IsShow("LoginPanel"))
                {
                    ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                }
                else
                {
                    JumpToLoginPanel();

                }
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
            }
            else if (data.ResultCode == -10)
            {
                Debug.Log($"<color=#ffff00>未找到手机号对应账号</color>");
                if (MainPanelMgr.Instance.IsShow("LoginDaContaPanel"))
                {
                    ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginDaContaPanel").transform);
                }
                else
                {
                    if (MainPanelMgr.Instance.IsShow("LoginPanel"))
                    {
                        ToolUtil.FloattingText("您的帐户或密码错误", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                    }
                    JumpToLoginPanel();

                }
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
            }
            else if (data.ResultCode == -11)
            {
                Debug.Log($"<color=#ffff00>登录校验失败</color>");
                JumpToLoginPanel();
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
            }
            else if (data.ResultCode == -12)
            {

                Debug.Log($"<color=#ffff00>账号已绑定手机号</color>");
                Message.Broadcast(MessageName.OPEN_LOGINBTN);
                if (MainPanelMgr.Instance.IsShow("LoginPanel"))
                {
                    ToolUtil.FloattingText("您已绑定手机，请使用手机号码登录", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                    JumpToLoginPanel();

                }
                else
                {
                    JumpToLoginPanel();

                }
            }
            else if (data.ResultCode == -13)
            {
                if (isConnet && isEnterGame)
                {
                    if (roleId != 0 && Guid != 0)
                    {
                        CW_ACCOUNT_RELOGIN_REQ dataReq = new CW_ACCOUNT_RELOGIN_REQ();
                        dataReq.n64AccountGUID = Guid;
                        dataReq.n64RoleGUID = roleId;
                        NetMgr.netMgr.send(NetMsgDef.C_RELOGINGAME, dataReq);
                        CoreEntry.gTimeMgr.AddTimer(3.0f, false, ReConnect, 632156);
                    }
                    else
                    {
                        CoreEntry.gTimeMgr.AddTimer(10.0f, false, ReConnect, 632156);
                    }
                }
                else
                {
                    Debug.Log($"<color=#ffff00>账号已在线</color>");
                    Message.Broadcast(MessageName.OPEN_LOGINBTN);
                    JumpToLoginPanel();
                    ToolUtil.FloattingText("账号已在线", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                }

            }
        }

        [NetResponse(NetMsgDef.S_CREATEROLE)]
        public void CreateRoleRecevied(MsgData msgData)
        {

            SdkCtrl.Instance.SendEvent(ApplyType.Login);
            MsgData_sCreateRole data = (MsgData_sCreateRole)msgData;
            if (data.Result == 0)
            {
                roleId = data.ID;
                var curID = data.ID;
                SendReqEnterGame("", "", "", "", "", "", 0, 0, 1, 0, curID, LoginModel.Instance.loginData.AccountGUID);
            }
            else
            {

            }

        }

        [NetResponse(NetMsgDef.S_ROLEINFO)]
        public void RoleInfoRecevied(MsgData msgData)
        {
            MsgData_sRoleInfo data = (MsgData_sRoleInfo)msgData;

            var id = data.Roles[0].ID;
            roleId = id;
            SendReqEnterGame("", "", "", "", "", "", 0, 0, 1, 0, id, LoginModel.Instance.loginData.AccountGUID);
        }

        [NetResponse(NetMsgDef.S_ENTERGAME)]
        public void EnterGameRecevied(MsgData msgData)
        {
            if (!PlayerPrefs.HasKey("ADCALLBACK"))
            {
                SdkCtrl.Instance.setAdCallBack("channel_id", channelId + "");
                SdkCtrl.Instance.setAdCallBack("account", SystemInfo.deviceUniqueIdentifier);
                SdkCtrl.Instance.setAdCallBack("roleid", MainUIModel.Instance.palyerData.m_i8roleID + "");
                SdkCtrl.Instance.setAdCallBack("account_id", LoginModel.Instance.loginData.AccountGUID + "");
                PlayerPrefs.SetInt("ADCALLBACK", 1);
            }

           
            if (isConnet)
            {
                if (MainUIModel.Instance.RoomData == null)
                {
                    LoginCtrl.Instance.isConnet = false;
                    Message.Broadcast(MessageName.GAME_RECONNET);
                    UICtrl.Instance.CloseLoading();
                }
                else
                {
                    MainUICtrl.Instance.SendEnterGameRoom(MainUIModel.Instance.RoomData.nGameType, MainUIModel.Instance.RoomData.nRoomType);
                }
            }
            else
            {
                Debug.Log("进入游戏" + isEnterGame + "|" + isLogin);
                if (!isEnterGame || isLogin)
                {
                    isEnterGame = true;
                    SdkCtrl.Instance.SendEvent(ApplyType.PageResume);
                    UICtrl.Instance.CloseLoading();
                    OpenView(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, null);



                }
            }
        }


        /// <summary>
        /// 请求修改密码返回
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.LC_ChangePwdForAccountRet)]
        public void OnChangePWDResult(MsgData msgData)
        {
            LC_ChangePwdForAccountRet data = msgData as LC_ChangePwdForAccountRet;
            if (data.m_nResult == 0)
            {
                Debug.Log($"<color=#ffff00>修改密码成功</color>");
                MainUIModel.Instance.PhonePwd = MainUIModel.Instance.Phonemima;
                PlayerPrefs.SetString("LOGIN_PHONE", $"{MainUIModel.Instance.Phone}");
                PlayerPrefs.SetString("LOGIN_PHONEPWD", $"{MainUIModel.Instance.PhonePwd}");
                PlayerPrefs.Save();
                ToolUtil.FloattingText("修改密码成功", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform, delegate { Message.Broadcast(MessageName.REFRESH_CADASTR_SUCESS); });
                
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>修改密码失败</color>");
                ToolUtil.FloattingText("修改密码失败", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>请重新发启验证码(断线了)</color>");
                ToolUtil.FloattingText("请重新发启验证码", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>两次手机号码不相同</color>");
                ToolUtil.FloattingText("两次手机号码不相同", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>手机号码长度不对</color>");
                ToolUtil.FloattingText("手机号码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>验证码不正确</color>");
                ToolUtil.FloattingText("验证码不正确", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 6)
            {
                Debug.Log($"<color=#ffff00>验证码超时</color>");
                ToolUtil.FloattingText("验证码超时", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 7)
            {
                Debug.Log($"<color=#ffff00>错误的密码格式</color>");
                ToolUtil.FloattingText("错误的密码格式", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 8)
            {
                Debug.Log($"<color=#ffff00>密码长度不对</color>");
                ToolUtil.FloattingText("密码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
        }
        /// <summary>
        /// 求修修改账号密码短信返回
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.LC_ChangePwdForAccountSmsgRet)]
        public void OnChangePWDSmsResult(MsgData msgData)
        {
            LC_ChangePwdForAccountSmsgRet data = msgData as LC_ChangePwdForAccountSmsgRet;
            Debug.Log($"<color=#ffff00>修改密码短信验证返回：{Encoding.Default.GetString(data.m_szPhone)}</color>");
            if (data.m_nResult == 0)
            {
                Debug.Log($"<color=#ffff00>成功</color>");
                ToolUtil.FloattingText("发送成功", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
                Message.Broadcast(MessageName.REFRESH_CADASTR_PANEL);
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>失败</color>");
                ToolUtil.FloattingText("未知错误", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>号码格式错误</color>");
                ToolUtil.FloattingText("号码格式错误", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>手机号码长度不对</color>");
                ToolUtil.FloattingText("手机号码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>请求短信验证码频繁</color>");
                ToolUtil.FloattingText("请求短信验证码频繁", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>此手机未绑定过账号</color>");
                ToolUtil.FloattingText("此手机未绑定过账号", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
        }
        /// <summary>
        /// 请求注册账号短信验证码回复
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.LC_RegisterAccountSmsgRet)]
        public void OnRegisterAccountSmsResult(MsgData msgData)
        {
            LC_RegisterAccountSmsgRet data = msgData as LC_RegisterAccountSmsgRet;
            Debug.Log($"<color=#ffff00>手机号码：{Encoding.Default.GetString(data.m_szPhone)}</color>");
            if (data.m_nResult == 0)
            {
                Debug.Log($"<color=#ffff00>0成功</color>");
                ToolUtil.FloattingText("发送成功", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
                Message.Broadcast(MessageName.REFRESH_CADASTR_PANEL);
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>1失败</color>");
                ToolUtil.FloattingText("发送失败", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>2号码格式错误</color>");
                ToolUtil.FloattingText("号码格式错误", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>3手机号码长度不对</color>");
                ToolUtil.FloattingText("手机号码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>4请求短信验证码频繁</color>");
                ToolUtil.FloattingText("请求短信验证码频繁", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>5此手机已绑定过账号</color>");
                ToolUtil.FloattingText("此手机已绑定过账号", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
        }
        /// <summary>
        /// 请求注册账号返回
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.LC_RegisterAccountRet)]
        public void OnRegisterAccountResult(MsgData msgData)
        {
            LC_RegisterAccountRet data = msgData as LC_RegisterAccountRet;
            if (data.m_nResult == 0)
            {
                //disconnect();
                //CoreEntry.gTimeMgr.AddTimer(1.0f, false, ReConnect, 632156);
                Debug.Log($"<color=#ffff00>成功</color>");
                CloseCadastrPanel();
                ToolUtil.FloattingText("您的帐户已成功注册，请登录", MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                OpenLoginDaContaPanel();
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>失败</color>");

            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>请重新发启验证码(断线了)</color>");
            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>两次手机号码不相同</color>");
                ToolUtil.FloattingText("两次手机号码不相同", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>手机号码长度不对</color>");

            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>验证码不正确</color>");
                ToolUtil.FloattingText("验证码不正确", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 6)
            {
                Debug.Log($"<color=#ffff00>验证码超时</color>");

            }
            else if (data.m_nResult == 7)
            {
                Debug.Log($"<color=#ffff00>错误的密码格式</color>");

            }
            else if (data.m_nResult == 8)
            {
                Debug.Log($"<color=#ffff00>密码长度不对</color>");

            }
        }
#endregion


        public void OpenLoginDaContaPanel()
        {
            MainPanelMgr.Instance.ShowDialog("LoginDaContaPanel");
        }

        public void CloseLoginDaContaPanel()
        {
            Message.Broadcast(MessageName.OPEN_LOGINBTN);
            MainPanelMgr.Instance.Close("LoginDaContaPanel");
        }

        public void OpenChangePwdPanel()
        {
            MainPanelMgr.Instance.ShowDialog("CadastrPanel",true,0);
        }

        public void CloseChangePwdPanel()
        {
            MainPanelMgr.Instance.Close("ChangePwdPanel");
        }

        public void OpenCadastrPanel()
        {
            MainPanelMgr.Instance.ShowDialog("CadastrPanel");
        }
        public void CloseCadastrPanel()
        {
            MainPanelMgr.Instance.Close("CadastrPanel");
        }

        private void JumpToLoginPanel()
        {
            if (isEnterGame)
            {
                if (MainUIModel.Instance.RoomData != null)
                {
                    return;
                }
                isBindAccount = true;
                UICtrl.Instance.OpenView("LoginPanel");
                UICtrl.Instance.CloseLoading();
                isShowLogin = false;
            }
            else
            {
                isEnterGame = true;
                isShowLogin = true;
                UICtrl.Instance.CloseLoading();
                OpenView(GameEvent.GE_BEGIN_LOADSCENE_LOGIN, null);
            }


        }

    }
}