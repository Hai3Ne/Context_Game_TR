using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 大厅消息监听
/// </summary>
/// 
public class PerfsAccount
{
    public string uid;
    public string pwd;
}

public class HallHandle {
    public static string WXUnionID;//微信UnionID
    public static string Accounts;//用户帐号
    public static ushort FaceID;//用户头像
    public static uint GameID;//游戏ID
    public static uint UserID;//用户ID
    public static byte Gender;//用户性别
    public static SystemTime LogonTime;//登录时间
    public static long UserGold;//用户金币
    public static long UserInsure;//用户保险箱金币
    public static string NickName;//用户昵称
    public static byte MemberOrder;//会员等级
    public static bool IsWXLogin;//是否微信登录
    public static string MobilePhone;//手机号
    public static string PassPortID;//身份证号
    public static string Compellation;//真实名称
    public static ushort ServerType;//服务器类型
    public static string LoginPassword = "111111";//登录密码，由Lua传递过来，不是实时
    public static string LogonCode {
        get {
            SystemTime dt = HallHandle.LogonTime;
            string dt_str = string.Format("{0}-{1}-{2},{3}:{4}:{5}.{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Milliseconds);
            return GameUtils.CalMD5(dt_str);
        }
    }
    public static Dictionary<ushort, List<tagGameServer>> dic_server_list = new Dictionary<ushort, List<tagGameServer>>();
    public static void Handle(NetCmdPack pack) {
        NetCmdType cmdtype = (NetCmdType)pack.cmdTypeId;
        switch (cmdtype) {
            case NetCmdType.SUB_GP_SHUTDOWN://网络断开通知
                HttpServer.Instance.Disconnect();
                break;
            case NetCmdType.SUB_GP_USER_FACE_INFO://更改头像
                HallHandle.FaceID = (pack.ToObj<CMD_GP_UserFaceInfo>()).FaceID;
                break;
            case NetCmdType.SUB_GP_USER_INSURE_SUCCESS://取钱成功//CMD_GP_UserInsureSuccess
                CMD_GP_UserInsureSuccess gp_user_in = (pack.ToObj<CMD_GP_UserInsureSuccess>());
                HallHandle.UserGold = gp_user_in.UserScore;
                HallHandle.UserInsure = gp_user_in.UserInsure;
                EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                SystemMessageMgr.Instance.DialogShow(gp_user_in.DescribeString, null);
                break;
            case NetCmdType.SUB_GR_USER_INSURE_SUCCESS://取钱成功//CMD_GR_S_UserInsureSuccess
                CMD_GR_S_UserInsureSuccess gr_user_in = (pack.ToObj<CMD_GR_S_UserInsureSuccess>());
                HallHandle.UserGold = gr_user_in.UserScore;
                HallHandle.UserInsure = gr_user_in.UserInsure;
                EventManager.Notifiy(GameEvent.Hall_UserInfoChange,null);
                SystemMessageMgr.Instance.DialogShow(gr_user_in.DescribeString, null);
                break;
            case NetCmdType.SUB_GP_USER_INSURE_FAILURE://取钱失败
                {
                    CMD_GP_UserInsureFailure cmd = pack.ToObj<CMD_GP_UserInsureFailure>();
                    //WndManager.Instance.ShowDialog(cmd.DescribeString, null);
                    SystemMessageMgr.Instance.ShowMessageBox(cmd.DescribeString, 1);
                    break;
                }
            case NetCmdType.SUB_GR_USER_INSURE_FAILURE://取钱失败
                {
                    CMD_GR_S_UserInsureFailure cmd = pack.ToObj<CMD_GR_S_UserInsureFailure>();
                    SystemMessageMgr.Instance.ShowMessageBox(cmd.DescribeString, 1);
                    break;
                }
            case NetCmdType.SUB_GP_OPERATE_SUCCESS://操作成功
                {
                    //WndManager.Instance.ShowDialog(pack.ToObj<CMD_GP_OperateSuccess>().DescribeString, null);
                    WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay(pack.ToObj<CMD_GP_OperateSuccess>().DescribeString), true);
                    break;
                }
            case NetCmdType.SUB_GP_OPERATE_FAILURE://操作失败
                {
                    //WndManager.Instance.ShowDialog(pack.ToObj<CMD_GP_OperateFailure>().DescribeString, null);
                    WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay(pack.ToObj<CMD_GP_OperateFailure>().DescribeString), true);
                    break;
                }
            case NetCmdType.SUB_GP_USER_INSURE_INFO://保险箱资料
                CMD_GP_UserInsureInfo gp_bank = pack.ToObj<CMD_GP_UserInsureInfo>();
                HallHandle.UserGold = gp_bank.UserScore;
                HallHandle.UserInsure = gp_bank.UserInsure;
                EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                break;
            case NetCmdType.SUB_GR_USER_INSURE_INFO://保险箱资料
                CMD_GR_S_UserInsureInfo gr_bank = pack.ToObj<CMD_GR_S_UserInsureInfo>();
                HallHandle.UserGold = gr_bank.UserScore;
                HallHandle.UserInsure = gr_bank.UserInsure;
                EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                break;
            case NetCmdType.SUB_GP_LOGON_SUCCESS://登录成功，
                CMD_GP_LogonSuccess gp_logon = (pack.ToObj<CMD_GP_LogonSuccess>());
                HallHandle.Accounts = gp_logon.Accounts;
                HallHandle.FaceID = gp_logon.FaceID;
                HallHandle.GameID = gp_logon.GameID;
                HallHandle.UserID = gp_logon.UserID;
                HallHandle.Gender = gp_logon.Gender;
                HallHandle.LogonTime = gp_logon.LogonTime;
                HallHandle.UserGold = gp_logon.UserScore;
                HallHandle.UserInsure = gp_logon.UserInsure;
                HallHandle.NickName = gp_logon.NickName;
                HallHandle.PassPortID = gp_logon.PassPortID;
                HallHandle.Compellation = gp_logon.Compellation;
                HallHandle.MobilePhone = string.Empty;
                if (gp_logon.LoginExInfo != null && gp_logon.LoginExInfo.MemberInfo != null) {
                    HallHandle.MemberOrder = gp_logon.LoginExInfo.MemberInfo.cbMemberOrder;
                } else {
                    HallHandle.MemberOrder = 0;
                }
                HallHandle.IsWXLogin = (gp_logon.dwStatus & 1 << 1) > 0;
                UserManager.IsBingWX = HallHandle.IsWXLogin;
                if (HallHandle.IsWXLogin == false) {
                    HallHandle.WXUnionID = string.Empty;
                }
                PayManager.ClearOrder();//重新登录时，清除之前的订单记录
                dic_server_list.Clear();
 
                EventManager.Notifiy(GameEvent.Hall_UserInfoChange,null);
                ApplePayManager.InitData();
                break;
            case NetCmdType.SUB_GP_Wechat_Bind_RES: //微信绑定信息
                {
                    CMD_GP_WechatBindRes cmd = pack.ToObj<CMD_GP_WechatBindRes>();
                    UserManager.IsBingWX = cmd.cbBind == 1;//是否绑定了微信
                    if (UserManager.IsBingWX == false && cmd.cbBindSwitch == 1 && HallHandle.UserGold + HallHandle.UserInsure >= 5000000) {
                        //UI.EnterUI<UI_BindNotice>(ui => {
                        //    ui.InitData(true);
                        //    UI.mUIList.Remove(ui);
                        //});

                    /*    UI_BindNotice ui_bind = UI.EnterUI<UI_BindNotice>(GameEnum.All);
                        ui_bind.InitData(true);
                        UI.mUIList.Remove(ui_bind);*/
                    }
                    break;
                }
            case NetCmdType.SUB_GP_LOGON_FAILURE://登录失败
                {
                    CMD_GP_LogonFailure cmd = pack.ToObj<CMD_GP_LogonFailure>();
                    SystemMessageMgr.Instance.DialogShow(cmd.DescribeString, null);
		            MainEntrace.Instance.HideLoad();
                    break;
                }
            case NetCmdType.SUB_GR_PROPERTY_EFFECT://用户赠送会员效果
                CMD_GR_S_PropertyEffect item_effect = (pack.ToObj<CMD_GR_S_PropertyEffect>());
                if (item_effect.wUserID == HallHandle.UserID) {
                    HallHandle.MemberOrder = item_effect.cbMemberOrder;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                }
                break;
            case NetCmdType.SUB_GP_USER_INDIVIDUAL://用户个人资料
                CMD_GP_UserIndividual gp_user_individual = (pack.ToObj<CMD_GP_UserIndividual>());
                if (gp_user_individual.userIndividExt != null) {
                    if (string.IsNullOrEmpty(gp_user_individual.userIndividExt.NickName) == false) {
                        HallHandle.NickName = gp_user_individual.userIndividExt.NickName;
                    }
                    if (string.IsNullOrEmpty(gp_user_individual.userIndividExt.MobilePhone) == false) {
                        HallHandle.MobilePhone = gp_user_individual.userIndividExt.MobilePhone;
                    }
                    if (string.IsNullOrEmpty(gp_user_individual.userIndividExt.Compellation) == false) {
                        HallHandle.Compellation = gp_user_individual.userIndividExt.Compellation;
                    }
                    if (string.IsNullOrEmpty(gp_user_individual.userIndividExt.PassPortID) == false) {
                        HallHandle.PassPortID = gp_user_individual.userIndividExt.PassPortID;
                    }
                }
                break;
            case NetCmdType.SUB_GP_LIST_SERVER://服务器列表
                CMD_GP_ServerList gp_server_list = (pack.ToObj < CMD_GP_ServerList>());
                //dic_server_list.Clear();
                List<tagGameServer> server_list;
                foreach (var item in gp_server_list.ServerList) {
                    if (dic_server_list.TryGetValue(item.KindID, out server_list) == false) {
                        server_list = new List<tagGameServer>();
                        dic_server_list.Add(item.KindID, server_list);
                    }
                    server_list.Add(item);
                }
                tagGameServer _t;
                foreach (var _list in dic_server_list.Values) {
                    for (int i = 0, count = _list.Count; i < count; i++) {
                        for (int j = i+1; j < count; j++) {
                            if (_list[i].SortID > _list[j].SortID) {
                                _t = _list[i];
                                _list[i] = _list[j];
                                _list[j] = _t;
                            }
                        }
                    }
                }
                break;
            case NetCmdType.SUB_GP_PAY_INFO://商城支付列表
                ShopManager.InitData(pack.ToObj<CMD_GP_PayInfo>());
                break;
            case NetCmdType.SUB_GP_FIRSTCHARGEAWARD://首充奖励
                ShopManager.SetFristBuyAward(pack.ToObj<CMD_GP_FirstChargeAward>());
                break;
            case NetCmdType.SUB_GP_AWARD_FIRSTCHARGE_RET://首充奖励验证结果
                var sss = pack.ToObj<CMD_GP_AwardFirstCharge_Ret>();
                if (sss.ResultInfo.ErrorCode == 0) {
                    SceneLogic.Instance.ShowShouChongAward();
                } else {
                    LogMgr.LogError(sss.ResultInfo.ErrorString);
                }
                break;
            case NetCmdType.SUB_GP_CAN_FIRSTCHARGE_RET://首充标识
                ShopManager.SetFristTick(pack.ToObj<CMD_GP_CanFirstChargeRet>().ResultCode == 0);
                break;
            case NetCmdType.SUB_GP_WEEKSIGNINFO://签到列表
                SignManager.InitData(pack.ToObj<CMD_GP_SignInfo>());
                break;
            case NetCmdType.SUB_GP_WEEKSIGN_RET://签到结果
                CMD_GP_Sign_Ret result = pack.ToObj<CMD_GP_Sign_Ret>();
                if (result.ResultInfo.ErrorCode == 0) {
                    List<KeyValuePair<ItemsVo, uint>> item_list = new List<KeyValuePair<ItemsVo, uint>>();
                    ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(2001u);
                    item_list.Add(new KeyValuePair<ItemsVo, uint>(vo, (uint)(result.UserScore - HallHandle.UserGold)));
                    UI_GetAwardController.ParamInfo pi = new UI_GetAwardController.ParamInfo {
                        //tipInfos = StringTable.GetString("Tip_37"),
                        db_item_list = item_list,
                    };
                    if (result.Twofold == 1) {//会员翻倍
                        pi.tipInfos = StringTable.GetString("Tip_38");
                    } else {
                        pi.tipInfos = StringTable.GetString("Tip_37");
                    }
                    WndManager.Instance.ShowUI(EnumUI.UI_GetAward, pi);

                    HallHandle.UserGold = result.UserScore;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange,null);

                }
                break;
            case NetCmdType.SUB_GP_DELETE_ORDER://IAP支付订单移除
                ApplePayManager.RemoveOrder(pack.ToObj<CMD_GP_DeleteOrder>().TransactionID);
                break;
            case NetCmdType.SUB_GP_SHARE_RET: {//每日首次分享奖励
                    {
                        CMD_GP_Share_Ret cmd = pack.ToObj<CMD_GP_Share_Ret>();
                        if (cmd.ResultInfo.ErrorCode == 0) {

                            List<KeyValuePair<ItemsVo, uint>> item_list = new List<KeyValuePair<ItemsVo, uint>>();
                            ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(2001u);
                            item_list.Add(new KeyValuePair<ItemsVo, uint>(vo, (uint)(cmd.Score - HallHandle.UserGold)));
                            UI_GetAwardController.ParamInfo pi = new UI_GetAwardController.ParamInfo {
                                //tipInfos = StringTable.GetString("Tip_37"),
                                db_item_list = item_list,
                            };
                            pi.tipInfos = StringTable.GetString("Tip_43");
                            WndManager.Instance.ShowUI(EnumUI.UI_GetAward, pi);

                            HallHandle.UserGold = cmd.Score;
                            EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                        }
                    }
                    break;
                }
            //default://不处理的消息直接过滤
            //    return;
            //default:
            //    Debug.LogError(cmdtype.ToString());
            //    break;
        }
        NetEventManager.Notifiy(cmdtype, pack);
    }
    public static void AsynScore(long score, long insure) {//游戏结束同步金币数量
        if (score >= 0) {
            HallHandle.UserGold = score;
        }
        if (insure >= 0) {
            HallHandle.UserInsure = insure;
        }
        EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
    }

    public static List<tagGameServer> GetServerList(ushort kind) {//根据Kind获取服务器列表
        List<tagGameServer> server_list;
        if (dic_server_list.TryGetValue(kind, out server_list) == false) {
            return new List<tagGameServer>();
        } else {
            return server_list;
        }
    }
    public static tagGameServer GetServerBySortID(int sort_id) {//根据sort_id获取对应服务器
        List<tagGameServer> server_list;
        if (dic_server_list.TryGetValue(GameConfig.KindID, out server_list)) {
            foreach (var server in server_list) {
                if (server.SortID == sort_id) {
                    return server;
                }
            }
        }
        return null;
    }
    public static List<tagGameServer> GetServerList(int coin_mode, int game_mode) {
        List<tagGameServer> list = HallHandle.GetServerList(GameConfig.KindID);
        if (coin_mode == -1) {
            return list;
        }

        int min_sort = coin_mode * 100 + game_mode * 10;
        int max_sort = min_sort + 10;
        int sort;
        List<tagGameServer> max_list = new List<tagGameServer>();
        tagGameServer[] servers = new tagGameServer[4];
        foreach (var item in list) {
            sort = item.SortID % 10000;
            if ((sort >= 500) || (sort >= min_sort && sort < max_sort)) {//大于500的  几个房间通用  防止本地调试找不到房间
                if (sort > min_sort + servers.Length || sort <= min_sort || servers[sort - min_sort - 1] != null) {
                    max_list.Add(item);
                } else if (sort - min_sort - 1 < servers.Length) {
                    servers[sort - min_sort - 1] = item;
                }
            }
        }

        //房间排序规则  1，2，3，4分别代表四个等级房间  其他见缝插针
        foreach (var item in max_list) {
            for (int j = 0; j < servers.Length; j++) {
                if (servers[j] == null) {
                    servers[j] = item;
                    break;
                }
            }
        }

        return new List<tagGameServer>(servers);
    }

    public static bool CheckPerfect(bool is_tick) {//检查个人信息是否完善
        return true;
        //if (HallHandle.IsPerfect()) {
        //    return true;
        //} else if (is_tick) {
        //    WndManager.Instance.ShowDialog("当前操作需要完善个人信息，是否前往？", (code) => {
        //        if (code == 1) {//确认
        //            UI.EnterUI<UI_userinfo_new>(ui => ui.InitData(3));
        //        }
        //    });
        //}
        //return false;
    }
    public static bool IsPerfect(){//用户信息是否完善
        if(HallHandle.IsWXLogin){//只有微信登录状态下才需要进行帐号完善
            return string.IsNullOrEmpty(HallHandle.PassPortID) == false;
        }else{
            return true;
        }
    }

    public static void SavePerfsAccounts(string account, string pwd) {//保存用户登录记录
        string str = PlayerPrefs.GetString("PerfsAccounts");

        if (string.IsNullOrEmpty(str))
        {
            PlayerPrefs.SetString("PerfsAccounts", account + "_" + pwd);
        }
        else
        {
            string[] PerfsArray = str.Split('#');

            bool isHaveAccount = false;

            string tmpStr = string.Empty;

            for (int i = 0; i < PerfsArray.Length; i++)
            {
                if (string.IsNullOrEmpty(PerfsArray[i]))
                    continue;
                string[] item = PerfsArray[i].Split('_');

                if (i > 0)
                {
                    tmpStr += string.Format("#{0}", item[0]);
                }
                else
                {
                    tmpStr += item[0];
                }

                if (item[0].Equals(account))
                {
                    item[1] = pwd;
                    isHaveAccount = true;
                }

                tmpStr += string.Format("_{0}", item[1]);
            }

            if (!isHaveAccount)
            {
                string newStr = account + "_" + pwd + "#" + str;
                PlayerPrefs.SetString("PerfsAccounts", newStr);
            }
            else
            {
                PlayerPrefs.SetString("PerfsAccounts", tmpStr);
            }
        }
    }
    public static List<PerfsAccount> ReadPerfsAccounts() {//读取登录记录
        List<PerfsAccount> list = new List<PerfsAccount>();
        string str = PlayerPrefs.GetString("PerfsAccounts");

        if (string.IsNullOrEmpty(str))
            return list;

        string[] PerfsArray = str.Split('#');

        for (int i = 0; i < PerfsArray.Length; i++) {
            if (string.IsNullOrEmpty(PerfsArray[i]))
                continue;
            string[] item = PerfsArray[i].Split('_');

            PerfsAccount account = new PerfsAccount();
            account.uid = item[0];
            account.pwd = item[1];
            list.Add(account);
        }
        return list;
    }

    #region 网络消息处理
    public static void ModifyLogonPassword(string ScrPassword, string newPassword) {//修改登录密码
        string MachineID = GameUtils.GetMachineID().ToUpper();
        CMD_GP_ModifyLogonPass cmd = new CMD_GP_ModifyLogonPass();
        cmd.UserID = HallHandle.UserID;
        cmd.Type = 2;//2.验证登陆码+用户密码
        cmd.DesPassword = GameUtils.CalMD5(newPassword);
        cmd.ScrPassword = GameUtils.CalMD5(ScrPassword);
        cmd.MachineID = MachineID;
        cmd.CheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);
        cmd.LogonCode = HallHandle.LogonCode;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_MODIFY_LOGON_PASS, cmd);
    }
    
    public static void ModifyInsurancePassword(string ScrPassword,string newPassword ){//修改保险箱密码
        string MachineID = GameUtils.GetMachineID().ToUpper();
        CMD_GP_ModifyInsurePass cmd = new CMD_GP_ModifyInsurePass();
	    cmd.UserID = HallHandle.UserID;
	    cmd.DesPassword = GameUtils.CalMD5(newPassword) ;
	    cmd.ScrPassword = GameUtils.CalMD5(ScrPassword);
	    cmd.MachineID = MachineID;
	    cmd.CheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);
        cmd.LogonCode =HallHandle.LogonCode;
        //cmd.Type = 0;
	    HttpServer.Instance.Send(NetCmdType.SUB_GP_MODIFY_INSURE_PASS, cmd);
    }

    public static void SendPerfectInfo(string username, string password, string name, string phone, string code, string nickname, string idcard) {
        string MachineID = GameUtils.GetMachineID().ToUpper();
        CMD_GP_PerfectInfo cmd = new CMD_GP_PerfectInfo();
        cmd.dwUserID = HallHandle.UserID;
        cmd.cbGender = HallHandle.Gender;
        cmd.szAccounts = username;
        cmd.szNickName = nickname;
        cmd.szPassPortID = idcard;
        cmd.szCompellation = name;
        cmd.szPhone = phone;
        cmd.szMachineID = MachineID;
        cmd.szLogonPass = GameUtils.CalMD5(password);
        cmd.szCheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);

        HttpServer.Instance.Send(NetCmdType.SUB_GP_PERFECT_INFO, cmd);
    }
    
    public static void SendVerifyPhoneCode(string phone,string verifycode ){
        CMD_GP_PhoneVerifyCodeReq cmd = new CMD_GP_PhoneVerifyCodeReq();
	    cmd.Phone = phone;
	    cmd.VerifyCode = verifycode;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_VERIFY_CODE, cmd);
    }
    
    public static void SendVerifyAccounts(string Accounts ){//帐号验证
        CMD_GP_VerifyAccounts cmd = new CMD_GP_VerifyAccounts();
	    cmd.Accounts = Accounts;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_VERIFY_ACCOUNTS, cmd);
    }
    public static void SendVerifyNickName(string NickName ){//昵称验证
        CMD_GP_VerifyNickName cmd = new CMD_GP_VerifyNickName();
	    cmd.NickName = NickName;

        HttpServer.Instance.Send(NetCmdType.SUB_GP_VERIFY_NICKNAME, cmd);
    }
    public static void QueryIndividualInfo(){//查询用户信息
        string MachineID = GameUtils.GetMachineID().ToUpper();
        CMD_GP_QueryIndividual cmd = new CMD_GP_QueryIndividual();

	    cmd.dwUserID = HallHandle.UserID;
	    if(HallHandle.IsWXLogin){
		    cmd.cbType = 0;//微信登录
	    }else{
		    cmd.cbType = 2;//正常登录
		    cmd.szPassword = GameUtils.CalMD5(HallHandle.LoginPassword);//--用户密码
	    }

	    cmd.szLogonCode = HallHandle.LogonCode;
	    cmd.szMachineID = MachineID;
	    cmd.szCheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);//校验参数

        HttpServer.Instance.Send(NetCmdType.SUB_GP_QUERY_INDIVIDUAL, cmd);
    }

    public static void ChangePlayerHeadIcon(ushort newFaceId) {//更改用户头像
        string MachineID = GameUtils.GetMachineID().ToUpper();
        CMD_GP_SystemFaceInfo cmd = new CMD_GP_SystemFaceInfo();
        cmd.FaceID = newFaceId;//--头像标识
        cmd.UserID = HallHandle.UserID;//用户 I D

        if (HallHandle.IsWXLogin) {
            cmd.cbType = 0;//微信登录
        } else {
            cmd.cbType = 2;//正常登录
            cmd.Password = GameUtils.CalMD5(HallHandle.LoginPassword);//--用户密码
        }

        cmd.MachineID = MachineID;//机器序列
        cmd.LogonCode = HallHandle.LogonCode;
        cmd.CheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);//校验参数

        HttpServer.Instance.Send(NetCmdType.SUB_GP_SYSTEM_FACE_INFO, cmd);
    }

    #endregion
}
