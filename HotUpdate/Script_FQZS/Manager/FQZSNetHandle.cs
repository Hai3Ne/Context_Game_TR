using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FQZSNetHandle
{
    public static void Handle(NetCmdPack pack)
    {
        NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;

        switch (cmdType)
        {
            case NetCmdType.SUB_GR_LOGON_SUCCESS://登录成功打开飞禽走兽界面
                UI.EnterUI<UI_FQZS_Main>(GameEnum.FQZS);
                break;
            case NetCmdType.SUB_GR_LOGON_FINISH://登录操作完成,进行坐下操作
                if (RoleManager.Self.ChairSeat == ushort.MaxValue && RoleManager.Self.TableID == ushort.MaxValue)
                {
                    NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown
                    {
                        TableID = ushort.MaxValue,
                        ChairID = ushort.MaxValue,
                        Password = string.Empty,
                    });
                }
                break;
            case NetCmdType.SUB_S_GAME_START_FQZS://游戏开始
                FQZSGameManager.HandleGameStart(pack.ToObj<CMD_S_GameStart_fqzs>());
                break;
            case NetCmdType.SUB_S_GAME_END_FQZS://游戏结束
                FQZSGameManager.HandleGameEnd(pack.ToObj<CMD_S_GameEnd_fqzs>());
                break;
            case NetCmdType.SUB_S_PLACE_JETTON_FQZS://用户下注
                FQZSGameManager.HandlePlaceJetton(pack.ToObj<CMD_S_PlaceJetton_fqzs>());
                break;
            case NetCmdType.SUB_S_CHANGE_BANKER://切换了擂主
                FQZSGameManager.HandleChangeLeiZhu(pack.ToObj<CMD_S_ChangeBanker>());
                break;
            case NetCmdType.SUB_S_AGAIN_JETTON_FQZS:
                FQZSGameManager.HandleAgainJetton(pack.ToObj<CMD_S_Again_Jetton_fqzs>());
                break;
        }
    }
}
