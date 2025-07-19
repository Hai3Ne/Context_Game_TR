using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//网络交互常用接口
public class NetManager {
    public static void LoginGame(GameEnum game_enum,bool is_wx, uint userID, string username, string usrPwd, string dt_str) {
        uint ver_code;//客户端版本号
        ushort kind_id;//类型索引
        uint game_id;//游戏ID
        switch (game_enum) {
            case GameEnum.WZQ://五子棋
                MainEntrace.Instance.HideLoad();
                ver_code = WZQGameConfig.VersionCode;
                kind_id = WZQGameConfig.KindID;
                game_id = WZQGameConfig.ClientGameID;
                break;
            case GameEnum.Fish_LK://李逵劈鱼
                ver_code = LKGameConfig.VersionCode;
            //    ver_code = 105709568;
                kind_id = LKGameConfig.KindID;
                game_id = LKGameConfig.ClientGameID;
                break;
            case GameEnum.SH://神话（神兽）
                ver_code = SHGameConfig.VersionCode;
                kind_id = SHGameConfig.KindID;
                game_id = SHGameConfig.ClientGameID;
                break;
            case GameEnum.FQZS://飞禽走兽
                ver_code = FQZSGameConfig.VersionCode;
                kind_id = FQZSGameConfig.KindID;
                game_id = FQZSGameConfig.ClientGameID;
                break;
            case GameEnum.Fish_3D://3D捕鱼
            default:
                MainEntrace.Instance.HideLoad();
                ver_code = GameConfig.VersionCode;
                kind_id = GameConfig.KindID;
                game_id = GameConfig.ClientGameID;
                break;
        }

        if (is_wx) {
            NetManager.LoginGameSrvByOtherLogin(userID, username, dt_str, ver_code, kind_id, game_id);
        } else {
            NetManager.LoginGameSrv(userID, username, GameUtils.CalMD5(usrPwd), dt_str, ver_code, kind_id, game_id);
        }
    }
    public static void LoginGameSrv(uint userID, string username, string usrPwd, string dt_str, uint ver_code,ushort kind_id,uint game_id) {
        CS_LoginUserID logonncb = new CS_LoginUserID();
        logonncb.SetCmdType(NetCmdType.SUB_GR_LOGON_USERID);
        logonncb.PlazaVersion = GameConfig.PlazaVersion;// 100728833;
        logonncb.FrameVersion = GameConfig.FrameVersion;// 100663297;
        //logonncb.ProcessVersion = 101122051;
        
        logonncb.ProcessVersion = ver_code;//GameUtils.ConvertToVersion(6, 13, 0, 3);// 101187587;
        logonncb.szLogonCode = GameUtils.CalMD5(dt_str);
        logonncb.ClientAddr = GameConfig.ClientAddr;// 16777343;
        logonncb.UserID = userID;
        logonncb.szAccounts = username;
        logonncb.szPassword = usrPwd;
        logonncb.szMachineID = GameUtils.GetMachineID();// "27B686E02DE8700D870169BA41F8051C";
        logonncb.PayPlatformID = GameConfig.PayPlatformID;// 3;
        logonncb.wKindID = kind_id;// 5000;
        logonncb.szCheckParam = ZJEncrypt.MapEncrypt(logonncb.szMachineID, 33);
        logonncb.ClientGameID = game_id;//5000;
        NetClient.Send<CS_LoginUserID>(logonncb);
        // 103415808
    }
    public static void LoginGameSrvByOtherLogin(uint userID, string username, string dt_str, uint ver_code, ushort kind_id, uint game_id) {//第三方平台登录
        CMD_GR_LogonOtherPlatform logonncb = new CMD_GR_LogonOtherPlatform();
        logonncb.SetCmdType(NetCmdType.SUB_GR_LOGON_OTHERPLATFORM);
        logonncb.szLogonCode = GameUtils.CalMD5(dt_str);
        logonncb.dwPlazaVersion = GameConfig.PlazaVersion;//100728833;
        logonncb.dwFrameVersion = GameConfig.FrameVersion;//100663297;
        logonncb.dwProcessVersion = ver_code;
        logonncb.dwClientAddr = GameConfig.ClientAddr;// 16777343;
        logonncb.dwUserID = userID;
        logonncb.szAccounts = username;
        logonncb.szPassword = "1C282BEAF240B6CA6366C7E634B871BB";//微信默认密码
        logonncb.szMachineID = GameUtils.GetMachineID();
        logonncb.szCheckParam = ZJEncrypt.MapEncrypt(logonncb.szMachineID, 33);
        logonncb.wKindID = kind_id;// 5000;
        logonncb.nPayPlatformID = GameConfig.PayPlatformID;// 3;
        logonncb.dwClientGameID = game_id;//5000;

        NetClient.Send<CMD_GR_LogonOtherPlatform>(logonncb);
    }

}
