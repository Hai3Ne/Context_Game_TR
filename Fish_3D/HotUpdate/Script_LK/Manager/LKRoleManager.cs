using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKRoleManager {
    public static ushort mTable;
    public static LKRole[] mRoles = new LKRole[LKGameConfig.MAXSEAT];

    public static void InitData(ushort table_id, long[] fish_golds, bool[] is_vips, long[] exchange_gold) {
        LKRoleManager.mTable = table_id;
        List<RoleInfo> list = RoleManager.GetRoleByTable(table_id);
        for (int i = 0; i < mRoles.Length; i++) {
            mRoles[i] = null;
        }

        foreach (var item in list) {
            if (item.UserStatus == EnumUserStats.US_LOOKON) {
                continue;
            }
            mRoles[item.ChairSeat] = new LKRole {
                RoleInfo = item,
                CannonMul = LKGameManager.mMinBulletMul,
                FishGold = fish_golds[item.ChairSeat],
                IsVipCannon = is_vips[item.ChairSeat],
                ChangeGold = -exchange_gold[item.ChairSeat] * LKGameManager.mRatioUserGold / LKGameManager.mRatioFishGold,
            };
        }
    }

    public static LKRole GetRole(ushort seat) {
        return mRoles[seat];
    }
    public static long GetBaseGold(ushort seat) {
        LKRole role = mRoles[seat];
        if (role == null) {
            return 0;
        } else {
            return role.GetBaseGold();
        }
    }
    public static long GetFishGold(ushort seat) {//获取玩家鱼币
        if (mRoles[seat] == null) {
            return 0;
        } else {
            return mRoles[seat].FishGold;
        }
    }
    public static void AddFishGold(ushort seat, long gold) {//设置玩家鱼币
        if (mRoles[seat] != null) {
            mRoles[seat].AddFishGold(gold);
        }
    }

    public static void OnUserLevelTable(RoleInfo info) {
        if (info == RoleManager.Self) {
            mTable = ushort.MaxValue;
            for (int i = 0; i < mRoles.Length; i++) {
                mRoles[i] = null;
            }
        }else if (info.TableID == mTable) {
            mRoles[info.ChairSeat] = null;
        }
    }
    public static void OnUserEnterTable(RoleInfo info) {
        if (info == RoleManager.Self) {
            long[] golds = new long[LKGameConfig.MAXSEAT];
            bool[] vips = new bool[LKGameConfig.MAXSEAT];
            for (int i = 0; i < LKGameConfig.MAXSEAT; i++) {
                golds[i] = 0;
                vips[i] = false;
            }
            LKRoleManager.InitData(info.TableID, golds, vips, golds);
        } else if(info.TableID == LKRoleManager.mTable) {
            mRoles[info.ChairSeat] = new LKRole {
                RoleInfo = info,
                CannonMul = LKGameManager.mMinBulletMul,
                FishGold = 0,
                IsVipCannon = false,
                ChangeGold = 0,
            };
        }
    }

    public static void OnExchangeFishScore(CMD_S_ExchangeFishScore_lkpy cmd) {//鱼币金币兑换
        if (mRoles[cmd.chair_id] != null) {
            mRoles[cmd.chair_id].FishGold += cmd.swap_fish_score;
            EventManager.Notifiy(GameEvent.UserInfoChange, mRoles[cmd.chair_id].RoleInfo);
        } else {
            LogMgr.LogError("找不到当前位置的用户:"+cmd.chair_id);
        }
    }

    public static void OnClientCfg(CMD_S_ClientCfg_lkpy cmd) {//用户切换炮台操作
        if (mRoles[cmd.chair_id] != null) {
            if (cmd.cfg_type == 1) {//会员跑切换
                mRoles[cmd.chair_id].IsVipCannon = cmd.cfg == 1;
                AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.CannonSwitch);
            } else {
                mRoles[cmd.chair_id].CannonMul = cmd.cfg;
            }
        }
    }
    public static void OnCannonLevel(CMD_S_CannonLevel_lkpy cmd) {//翅膀
        if (mRoles[cmd.chair_id] != null) {
            //mRoles[cmd.chair_id].CannonMul = 
        }
    }
}
