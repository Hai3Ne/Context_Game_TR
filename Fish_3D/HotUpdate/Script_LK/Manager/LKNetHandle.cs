using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKNetHandle {
    public static void Handle(NetCmdPack pack) {
        NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;
        switch (cmdType) {
            case NetCmdType.SUB_GR_LOGON_FINISH://登录操作完成
                if (LKGameManager.mInitLoadFinish) {
                    if (RoleManager.Self.ChairSeat == ushort.MaxValue && RoleManager.Self.TableID == ushort.MaxValue) {
                        NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown {
                            TableID = ushort.MaxValue,
                            ChairID = ushort.MaxValue,
                            Password = string.Empty,
                        });
                    }
                } else {
                    UI.ExitAllUI();
                    UI.EnterUI<UI_LK_loading>(GameEnum.Fish_LK).InitData();
                }
                break;
            case NetCmdType.SUB_GF_GAME_SCENE_LKPY://ID="101" name="CMD_S_GameStatus_lkpy"游戏状态
                LKGameManager.OnGameStatus(pack.ToObj<CMD_S_GameStatus_lkpy>());
                break;
            case NetCmdType.SUB_S_GAME_CONFIG_LKPY://ID="100"name="CMD_S_GameConfig_lkpy"游戏配置
                LKGameManager.OnGameConfig(pack.ToObj<CMD_S_GameConfig_lkpy>());
                break;
            case NetCmdType.SUB_S_FISH_TRACE_LKPY://ID="101"name="CMD_S_FishTrace_lkpy"鱼的轨迹(单条鱼)
                LKFishManager.OnFishTrace(pack.ToObj<CMD_S_FishTrace_lkpy>());
                break;
            //case NetCmdType.SUB_S_USER_FIRE_LKPY://ID="102"name="CMD_S_UserFire_lkpy"玩家发炮
            case NetCmdType.SUB_S_EXCHANGE_FISHSCORE_LKPY://ID="103"name="CMD_S_ExchangeFishScore_lkpy"鱼币金币兑换
                LKRoleManager.OnExchangeFishScore(pack.ToObj<CMD_S_ExchangeFishScore_lkpy>());
                break;
            //case NetCmdType.SUB_S_BULLET_ION_TIMEOUT_LKPY://ID="104"name="CMD_S_BulletIonTimeout_lkpy"能量炮结束
            case NetCmdType.SUB_S_LOCK_TIMEOUT_LKPY://ID="105"name="CMD_S_LockTimeout_lkpy"屏幕锁定结束,打爆定屏炸弹后会锁屏收到后解除屏幕锁定
                LKFishManager.OnUnlockTimeOut();
                break;
            case NetCmdType.SUB_S_CATCH_SWEEP_FISH_LKPY://ID="106"name="CMD_S_CatchSweepFish_lkpy"捕获炸弹鱼
                LKFishManager.OnCatchSweepFish(pack.ToObj<CMD_S_CatchSweepFish_lkpy>());
                break;
            case NetCmdType.SUB_S_CATCH_SWEEP_FISH_RESULT_LKPY://ID="107"name="CMD_S_CatchSweepFishResult_lkpy"炸弹鱼结果
                LKFishManager.OnCatchSweepFishResult(pack.GetCatchSweepFishResult());
                break;
            case NetCmdType.SUB_S_HIT_FISH_LK_LKPY://ID="108"name="CMD_S_HitFishLK_lkpy"李逵武松倍数增加
                LKFishManager.OnHitFishLK(pack.ToObj<CMD_S_HitFishLK_lkpy>());
                break;
            case NetCmdType.SUB_S_SWITCH_SCENE_LKPY://ID="109"name="CMD_S_SwitchScene_lkpy"切换场景
                LKFishManager.OnSwitchScene(pack.GetSwitchScene_klpy());
                break;
            case NetCmdType.SUB_S_CATCH_FISH_LKPY://ID="111"name="CMD_S_CatchFish_lkpy"捕获鱼类
                LKFishManager.OnCatchFish(pack.ToObj<CMD_S_CatchFish_lkpy>());
                break;
            case NetCmdType.SUB_S_SCENE_END_LKPY://ID="112"name="CMD_S_SceneEnd_lkpy"场景结束
                LKFishManager.OnSceneEnd();
                break;
            case NetCmdType.SUB_S_RADIATION_LKPY://ID="113"name="CMD_S_Radiation_lkpy"辐射鱼群
                LKFishManager.OnRadiation(pack.ToObj<CMD_S_Radiation_lkpy>());
                break;
            //case NetCmdType.SUB_S_HEART_BEAT_LKPY://ID="114"name="CMD_S_HeartBeat_lkpy"时钟同步
            case NetCmdType.SUB_S_CLIENT_CFG_LKPY://ID="115"name="CMD_S_ClientCfg_lkpy"客户端操作
                LKRoleManager.OnClientCfg(pack.ToObj<CMD_S_ClientCfg_lkpy>());
                break;
            case NetCmdType.SUB_S_CATCH_MASKKING_LKPY://ID="116"name="CMD_S_CatchMaskKing_lkpy"捕获蒙面鱼王
                {
                    CMD_S_CatchMaskKing_lkpy cmd = pack.ToObj<CMD_S_CatchMaskKing_lkpy>();
                    LKFishManager.OnCatchMeskKing(cmd.fish_id, cmd.fish_kind, cmd.fish_multi);
                    break;
                }
            case NetCmdType.SUB_S_FISH_TRACE2_LKPY://ID="117"name="CMD_S_FishTrace2_lkpy"鱼的轨迹（读文本,鱼群）
                LKFishManager.OnFishTrace2(pack.ToObj<CMD_S_FishTrace2_lkpy>());
                break;
            case NetCmdType.SUB_S_FIRE_KEY_LKPY://ID="118"name="CMD_S_FireKey_lkpy"玩家发炮密钥
                LKGameManager.OnFireKey(pack.ToObj<CMD_S_FireKey_lkpy>());
                break;
            //case NetCmdType.SUB_S_LOTTERY_QUALIFICATION_LKPY://ID="119"name="CMD_S_LotteryQualification_lkpy"中奖资格
            //case NetCmdType.SUB_S_DRAW_LOTTERY_LKPY://ID="120"name="CMD_S_DrawLottery_lkpy"中奖结果
            case NetCmdType.SUB_S_CANNON_LEVEL_LKPY://ID="121"name="CMD_S_CannonLevel_lkpy"炮台等级
                LKRoleManager.OnCannonLevel(pack.ToObj<CMD_S_CannonLevel_lkpy>());
                break;

            ////case NetCmdType.SUB_GF_GAME_SCENE_FREE://空闲状态下同步数据
            //case NetCmdType.SUB_GF_GAME_SCENE_PLAY://游戏状态下同步数据
            //    WZQGameManager.HandleGameScene(pack);
            //    break;

            case NetCmdType.SUB_S_HEART_BEAT_LKPY:
                LKGameManager.OnHeartBeat(pack.ToObj<CMD_S_HeartBeat_lkpy>());
                break;
        }
    }
}
