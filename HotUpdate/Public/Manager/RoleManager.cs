using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleManager {
    public static RoleInfo Self;//自己信息
    public static Dictionary<uint, RoleInfo> dic_role = new Dictionary<uint, RoleInfo>();//当前游戏用户列表

    public static void Clear() {//清除用户信息
        Self = null;
        dic_role.Clear();
    }
    public static void HandleUserEnter(SC_UserEnter sc_user_enter) {//用户进入
        RoleInfo pInfo = null;
        if (LogMgr.ShowLog) {
            LogMgr.Log("UserEnter: UserID:[" + sc_user_enter.UserInfoHead.UserID + "], TableID:[" + sc_user_enter.UserInfoHead.TableID + "], ChairId:[" + sc_user_enter.UserInfoHead.ChairID + "]");
        }
        if (dic_role.TryGetValue(sc_user_enter.UserInfoHead.UserID, out pInfo) == false) {
            pInfo = new RoleInfo();
            dic_role.Add(sc_user_enter.UserInfoHead.UserID, pInfo);
        }
        pInfo.GameID = sc_user_enter.UserInfoHead.GameID;
        pInfo.UserID = sc_user_enter.UserInfoHead.UserID;
        pInfo.TableID = sc_user_enter.UserInfoHead.TableID;
        pInfo.ChairSeat = sc_user_enter.UserInfoHead.ChairID;
        pInfo.SetGold(sc_user_enter.UserInfoHead.Score);
        pInfo.Gender = sc_user_enter.UserInfoHead.Gender;
        pInfo.FaceID = sc_user_enter.UserInfoHead.FaceID;
        pInfo.Grade = sc_user_enter.UserInfoHead.Grade;
        pInfo.MemberOrder = (int)sc_user_enter.UserInfoHead.MemberOrder;
        pInfo.NickName = sc_user_enter.UserInfoExt != null ? sc_user_enter.UserInfoExt.UseNickName : pInfo.UserID.ToString();
        pInfo.UserStatus = (EnumUserStats)sc_user_enter.UserInfoHead.UserStatus;

        if (RoleManager.Self.UserID == sc_user_enter.UserInfoHead.UserID) {
            RoleManager.Clear();//自己进入的时候清除旧数据
            RoleManager.Self = pInfo;
            dic_role.Add(pInfo.UserID, pInfo);
            if (pInfo.UserStatus != EnumUserStats.US_FREE) {//当前用户刚进入时，只要不是站立状态，就发送GameOption
                if (GameManager.CurGameEnum != GameEnum.Fish_LK) {//李逵劈鱼逻辑特殊处理
                    RoleManager.SendGameOption();
                }
            }
        }
        EventManager.Notifiy(GameEvent.UserEneter, pInfo);
    }
    public static void HandleUserState(SC_GR_UserStatus user_state) {//用户状态更新
        RoleInfo pInfo = null;
        if (dic_role.TryGetValue(user_state.userID, out pInfo) == false) {
            LogMgr.LogError("没有当前用户数据");
            return;
        }
        //Debug.LogError(LitJson.JsonMapper.ToJson(user_state));
        if (pInfo.TableID != ushort.MaxValue && pInfo.TableID != user_state.UserStats.TableID) {//用户离开当前桌
            EventManager.Notifiy(GameEvent.UserLeaveTable, pInfo);
        }
        if (user_state.UserStats.TableID != ushort.MaxValue && pInfo.TableID != user_state.UserStats.TableID) {//用户进入当前桌
            pInfo.TableID = user_state.UserStats.TableID;
            pInfo.ChairSeat = user_state.UserStats.ChairID;
            EventManager.Notifiy(GameEvent.UserEnterTable, pInfo);
        } else {
            pInfo.TableID = user_state.UserStats.TableID;
            pInfo.ChairSeat = user_state.UserStats.ChairID;
        }
        //pInfo.UserStatus = (EnumUserStats)user_state.UserStats.UserStatus;
        RoleManager.SetUserState(pInfo, (EnumUserStats)user_state.UserStats.UserStatus);
        EventManager.Notifiy(GameEvent.UserStateChange, pInfo);
    }
    public static void HandleUserScore(SC_GR_UserScore gr_user_score) {//用户分数更新
        RoleInfo pInfo = null;
        if (dic_role.TryGetValue(gr_user_score.UserID, out pInfo) == false) {
            return;
        }
        pInfo.SetGold(gr_user_score.UserScroe.Scroe);
        EventManager.Notifiy(GameEvent.UserInfoChange, pInfo);

        if (RoleManager.Self == pInfo && (HallHandle.ServerType & 0x0001) > 0) {
            HallHandle.AsynScore(gr_user_score.UserScroe.Scroe, gr_user_score.UserScroe.Insure);
        }
    }
    public static void HandleUserIndividual(CMD_GP_UserIndividual cmd) {//用户信息修改
        RoleInfo pInfo = null;
        if (dic_role.TryGetValue(cmd.dwUserID, out pInfo) == false) {
            return;
        }
        pInfo.NickName = cmd.userIndividExt.NickName;
        EventManager.Notifiy(GameEvent.UserInfoChange, pInfo);
    }

    public static void SendGameOption() {
        CS_GF_GameOption req = new CS_GF_GameOption();
        req.SetCmdType(NetCmdType.SUB_GF_GAME_OPTION);
        NetClient.Send<CS_GF_GameOption>(req);
    }
    private static void SetUserState(RoleInfo pInfo, EnumUserStats state) {//用户状态变化
        if (pInfo.UserStatus != state) {
            //只有从站立状态坐下才会发送GameOption
            if (pInfo == Self) {
                if (pInfo == Self && pInfo.UserStatus == EnumUserStats.US_FREE && (state == EnumUserStats.US_SIT || state == EnumUserStats.US_PLAYING)) {
                    RoleManager.SendGameOption();
                }
            }
            EventManager.Notifiy(GameEvent.UserStateChange, pInfo, state);
            pInfo.UserStatus = state;
        }
    }

    public static RoleInfo FindRole(uint user_id) {//根据用户ID获取用户信息
        RoleInfo role;
        if (dic_role.TryGetValue(user_id, out role) == false) {
            return null;
        } else {
            return role;
        }
    }
    public static RoleInfo FindRole(string nick_name) {//根据昵称获取用户信息
        foreach (var item in dic_role.Values) {
            if (item.NickName == nick_name) {
                return item;
            }
        }
        return null;
    }
    public static RoleInfo FindRoleByTable(ushort table,ushort seat) {//根据位置获取用户
        foreach (var item in dic_role.Values) {
            if (item.TableID == table && item.ChairSeat == seat) {
                return item;
            }
        }
        return null;
    }
    public static List<RoleInfo> GetRoleByTable(ushort table) {//根据桌子获取玩家列表
        List<RoleInfo> list = new List<RoleInfo>();
        foreach (var item in dic_role.Values) {
            if (item.TableID == table) {
                list.Add(item);
            }
        }
        return list;
    }
}
