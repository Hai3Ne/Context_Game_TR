using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetHandle {
    public static void Handle(NetCmdPack pack) {
        NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;
        switch (cmdType) {
            case NetCmdType.SUB_GR_CONFIG_SERVER://房间配置
                HallHandle.ServerType = pack.ToObj<SC_ConfigServer>().ServerType;
                break;
            //case NetCmdType.SUB_GR_LOGON_SUCCESS://登录成功
            case NetCmdType.SUB_GR_LOGON_FAILURE://登录失败
                NetClient.CloseConnect();//登录失败，直接中断链接
                HandleLoginFail(pack.ToObj<SC_LoginFail>());
                break;
            //case NetCmdType.SUB_GR_CONFIG_SERVER://房间配置
            //case NetCmdType.SUB_GR_CONFIG_COLUMN:
            //case NetCmdType.SUB_GR_CONFIG_PROPERTY:
            //case NetCmdType.SUB_GR_TABLE_INFO:
            //case NetCmdType.SUB_GR_LOGON_FINISH://登录操作完成
            case NetCmdType.SUB_GR_UPDATE_NOTIFY://版本升级通知
                NetClient.CloseConnect();//进入失败，直接中断链接
                ;                MainEntrace.Instance.HideLoad();
                SystemMessageMgr.Instance.DialogShow("您的客户端版本不是最新的,请联系官方客服。", null);
                break;
            //case NetCmdType.SUB_S_GAME_CONFIG:
            case NetCmdType.SUB_GR_USER_ENTER://用户进入
                RoleManager.HandleUserEnter(pack.ToObj<SC_UserEnter>());
                break;
            case NetCmdType.SUB_GR_USER_STATUS://用户当前状态更新
                RoleManager.HandleUserState(pack.ToObj<SC_GR_UserStatus>());
                break;
            case NetCmdType.SUB_GR_USER_SCORE://用户分数
                RoleManager.HandleUserScore(pack.ToObj<SC_GR_UserScore>());
                break;
            case NetCmdType.SUB_GF_GAME_STATE://游戏状态通知
                GameManager.SetGameState((GameState)pack.ToObj<SC_GF_GameStatus>().GameStatus);
                break;

            case NetCmdType.SUB_CM_SYSTEM_MESSAGE://房间内消息
                SC_GR_CM_SystemMessage cmSysmsg = pack.ToObj<SC_GR_CM_SystemMessage>();
                cmSysmsg.Message = Tools.MessageColor(cmSysmsg.Message);
                ushort msgtype = cmSysmsg.Type;
                if (((msgtype & ((ushort)SysMessageType.SMT_CLOSE_ROOM | (ushort)SysMessageType.SMT_CLOSE_LINK)) != 0))
                {
                    NetClient.CloseConnect();//被踢出房间，直接中断链接
                }
                if ((msgtype & (ushort)(SysMessageType.SMT_TABLE_ROLL | SysMessageType.SMT_PROMPT | SysMessageType.SMT_CHAT)) != 0)
                {
                    WndManager.Instance.GetController<ScrollingMessageUIController>().PushRollMsg(cmSysmsg.Message);
                }
                if ((msgtype & (ushort)SysMessageType.SMT_EJECT) != 0)
                {
                    SystemMessageMgr.Instance.HandlePormptMsg(new SystMsg(cmSysmsg.Type, cmSysmsg.Message));
                }
                break;
            case NetCmdType.SUB_GF_SYSTEM_MESSAGE://桌子内消息
                SC_GR_GF_SystemMessage sysMsg = pack.ToObj<SC_GR_GF_SystemMessage>();
                sysMsg.Message = Tools.MessageColor(sysMsg.Message);
                if (((sysMsg.Type & ((ushort)SysMessageType.SMT_CLOSE_ROOM | (ushort)SysMessageType.SMT_CLOSE_LINK)) != 0)) {
                    NetClient.CloseConnect();//被踢出房间，直接中断链接
                }
                if ((sysMsg.Type & (ushort)SysMessageType.SMT_EJECT) != 0)
                {
                     SystemMessageMgr.Instance.ShowMessageBox(sysMsg.Message);
                }
                break;
            case NetCmdType.SUB_GR_REQUEST_FAILURE://请求失败
                MainEntrace.Instance.HideLoad();
                if (GameManager.CurGameEnum == GameEnum.Fish_LK) {
                    SystemMessageMgr.Instance.DialogShow(pack.ToObj<SC_GR_RequestFailure>().DescribeString, () => {
                        GameSceneManager.BackToHall(GameEnum.Fish_LK);
                    });
                } else {
                    SystemMessageMgr.Instance.DialogShow(pack.ToObj<SC_GR_RequestFailure>().DescribeString, null);
                }
                break;
            case NetCmdType.SUB_GP_USER_INDIVIDUAL://用户个人资料修改
                RoleManager.HandleUserIndividual(pack.ToObj<CMD_GP_UserIndividual>());
                break;
            case NetCmdType.SUB_GP_USER_INSURE_INFO://保险箱资料
                {
                    CMD_GP_UserInsureInfo gp_bank = pack.ToObj<CMD_GP_UserInsureInfo>();
                    HallHandle.UserGold = gp_bank.UserScore;
                    HallHandle.UserInsure = gp_bank.UserInsure;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                }
                break;
            case NetCmdType.SUB_GR_USER_INSURE_INFO://保险箱资料
                {
                    CMD_GR_S_UserInsureInfo gr_bank = pack.ToObj<CMD_GR_S_UserInsureInfo>();
                    HallHandle.UserGold = gr_bank.UserScore;
                    HallHandle.UserInsure = gr_bank.UserInsure;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                }
                break;
            case NetCmdType.SUB_GP_USER_INSURE_SUCCESS://取钱成功//CMD_GP_UserInsureSuccess
                {
                    CMD_GP_UserInsureSuccess gp_user_in = (pack.ToObj<CMD_GP_UserInsureSuccess>());
                    HallHandle.UserGold = gp_user_in.UserScore;
                    HallHandle.UserInsure = gp_user_in.UserInsure;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                }
                break;
            case NetCmdType.SUB_GR_USER_INSURE_SUCCESS://取钱成功//CMD_GR_S_UserInsureSuccess
                {
                    CMD_GR_S_UserInsureSuccess gr_user_in = (pack.ToObj<CMD_GR_S_UserInsureSuccess>());
                    HallHandle.UserGold = gr_user_in.UserScore;
                    HallHandle.UserInsure = gr_user_in.UserInsure;
                    EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                }
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
            default:
                if (LogMgr.ShowLog) {
                    LogMgr.Log(string.Format("not resp <-- cmdType:{0}   main:{1}   sub:{2}", cmdType, ((pack.cmdTypeId & int.MaxValue) >> 16), (pack.cmdTypeId & ushort.MaxValue)));
                }
                break;
        }

        //LogMgr.LogError(string.Format("main:{0}   sub:{1}", ((pack.cmdTypeId & int.MaxValue) >> 16), (pack.cmdTypeId & ushort.MaxValue)));
        switch (GameManager.CurGameEnum) {
            case GameEnum.SH://神兽转盘
                SHNetHandle.Handle(pack);
                break;
            case GameEnum.WZQ://五子棋
                WZQNetHandle.Handle(pack);
                break;
            case GameEnum.Fish_LK://李逵劈鱼
                LKNetHandle.Handle(pack);
                break;
            case GameEnum.FQZS://飞禽走兽
                FQZSNetHandle.Handle(pack);
                break;
        }

        NetEventManager.Notifiy(cmdType, pack);
    }
    private static void HandleLoginFail(SC_LoginFail loginFail) {
        MainEntrace.Instance.HideLoad();
        SystemMessageMgr.Instance.DialogShow(loginFail.DescribeString, () => {
            GameManager.SetGameEnum(GameEnum.None);
        });
        LogMgr.Log("SC_LoginFail " + LitJson.JsonMapper.ToJson(loginFail));
        GameManager.__next_enter_time = 0;//登录失败后刷新下次登录时间
    }
}
