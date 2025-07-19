using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  当前桌子信息管理
/// </summary>
public class WZQTableManager {
    public static ushort FristHandSeat = 0;//黑子座位号
    public static ulong MaxPayMoney = 20000000;//最大支付金币
    public static ushort TotalTime = 10 * 60;//每局总时间/秒
    public static ushort[] LeftTime = new ushort[2];//双方剩余时间[单位/秒]
    public static ushort mCurRoleSeat;//当前正在下棋的玩家座位号
    public static ushort mRegretCount;//当局悔棋总数

    public static ushort mCurTable = ushort.MaxValue;
    public static RoleInfo mOtherRole;//对方玩家
    public static void InitData(ushort table) {//根据桌子信息初始化
        WZQTableManager.mCurTable = table;
        List<RoleInfo> list = RoleManager.GetRoleByTable(table);
        if(RoleManager.Self.ChairSeat == 0){//获取对方玩家信息
            WZQTableManager.mOtherRole = RoleManager.FindRoleByTable(table, 1);
        } else {
            WZQTableManager.mOtherRole = RoleManager.FindRoleByTable(table, 0);
        }
    }

    public static void SetCurRoleSeat(ushort cur_user) {//设置当前执子用户位置
        WZQTableManager.mCurRoleSeat = cur_user;
        EventManager.Notifiy(GameEvent.CurPlaySeatChange, WZQTableManager.mCurRoleSeat);
    }

    public static void HandleUserEnter(SC_UserEnter sc_user_enter) {
        if (RoleManager.Self.UserID == sc_user_enter.UserInfoHead.UserID) {
            WZQTableManager.InitData(RoleManager.Self.TableID);
        } else if (sc_user_enter.UserInfoHead.TableID == WZQTableManager.mCurTable) {
            WZQTableManager.mOtherRole = RoleManager.FindRole(sc_user_enter.UserInfoHead.UserID);
        }
    }
    public static void HandleUserState(SC_GR_UserStatus sc_user_state) {
        if (RoleManager.Self.UserID == sc_user_state.userID) {
            //Debug.LogError(LitJson.JsonMapper.ToJson(sc_user_state));
            if (RoleManager.Self.TableID != WZQTableManager.mCurTable) {
                WZQTableManager.InitData(RoleManager.Self.TableID);
            }
        } else if (mOtherRole != null && WZQTableManager.mOtherRole.UserID == sc_user_state.userID) {
            if (sc_user_state.UserStats.TableID != WZQTableManager.mCurTable) {
                if (RoleManager.Self.ChairSeat == 0) {//获取对方玩家信息
                    WZQTableManager.mOtherRole = RoleManager.FindRoleByTable(RoleManager.Self.TableID, 1);
                } else {
                    WZQTableManager.mOtherRole = RoleManager.FindRoleByTable(RoleManager.Self.TableID, 0);
                }
            }
        } else if (sc_user_state.UserStats.TableID == WZQTableManager.mCurTable) {
            WZQTableManager.mOtherRole = RoleManager.FindRole(sc_user_state.userID);
        }
    }
}
