using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHNetHandle {
    public static void Handle(NetCmdPack pack) {
        NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;
        switch (cmdType) {
            case NetCmdType.SUB_GR_LOGON_SUCCESS://登录成功
                UI.EnterUI<UI_Shenhua>(GameEnum.SH);
                break;
            case NetCmdType.SUB_GR_LOGON_FINISH://登录操作完成
                if (RoleManager.Self.ChairSeat == ushort.MaxValue && RoleManager.Self.TableID == ushort.MaxValue) {
                    NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown {
                        TableID = ushort.MaxValue,
                        ChairID = ushort.MaxValue,
                        Password = string.Empty,
                    });
                }
                break;

            case NetCmdType.SUB_S_GAME_FREE_SSZP://游戏进入空闲状态
                SHGameManager.HandleGameFree(pack.ToObj<CMD_S_GameFree_sszp>());
                break;
            case NetCmdType.SUB_S_GAME_START_SSZP://游戏开始
                SHGameManager.HandleGameStart(pack.ToObj<CMD_S_GameStart_sszp>());
                break;
            case NetCmdType.SUB_S_GAME_END_SSZP://游戏结束
                SHGameManager.HandleGameEnd(pack.ToObj<CMD_S_GameEnd_sszp>());
                break;
            case NetCmdType.SUB_S_PLACE_JETTON_SSZP://用户下注
                SHGameManager.HandlePlaceJetton(pack.ToObj<CMD_S_PlaceJetton_sszp>());
                break;
            //case NetCmdType.SUB_S_APPLY_BANKER_SSZP://<Pack name="CMD_S_ApplyBanker_sszp" ID="103" desc="申请庄家">
            case NetCmdType.SUB_S_CHANGE_BANKER_SSZP://<Pack name="CMD_S_ChangeBanker_sszp" ID="104" desc="切换庄家">
                SHGameManager.HandleChangeLeiZhu(pack.ToObj<CMD_S_ChangeBanker_sszp>());
                break;
            //case NetCmdType.SUB_S_SEND_RECORD_SSZP://<Pack name="CMD_S_GameRecord_sszp" ID="106" desc="游戏记录">
            case NetCmdType.SUB_S_PLACE_JETTON_FAIL_SSZP://<Pack name="CMD_S_PlaceJettonFail_sszp" ID="107" desc="下注失败">
                SystemMessageMgr.Instance.ShowMessageBox("该区域竞猜额已达上限");
                break;
            //case NetCmdType.SUB_S_CANCEL_BANKER_SSZP://<Pack name="CMD_S_CancelBanker_sszp" ID="108" desc="取消上庄申请">
            case NetCmdType.SUB_S_PLACE_JETTON_BROAD_SSZP://<Pack name="CMD_S_PlaceJettonBroad_sszp" ID="113" desc="广播下注">
                SHGameManager.HandlePlaceJettonBroad(pack.ToObj<CMD_S_PlaceJettonBroad_sszp>());
                break;
        }
    }
}
