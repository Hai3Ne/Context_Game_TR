using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 签到管理
/// </summary>
public class SignManager {
    public static bool IsSign;//是否可签到
    public static bool __init_tick;//初始化标记
    public static void InitData(CMD_GP_SignInfo sign_info) {
        int cur_week = sign_info.CurWeekDay;
        if (cur_week == byte.MaxValue) {
            SignManager.IsSign = false;
            return;
        }
        if (cur_week == 0) {
            cur_week = 7;
        }
        SignManager.IsSign = sign_info.SignInfo[cur_week - 1].Signed != 1;
        if (IsSign && __init_tick == false) {
            __init_tick = true;
            MainEntrace.Instance.ShowLoginTick();
        }
    }
}
