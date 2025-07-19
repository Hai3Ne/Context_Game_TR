using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏状态
/// </summary>
public enum GameState {
    GAME_STATUS_FREE = 0,//空闲状态
    GAME_STATUS_PLAY = 100,//游戏状态
    GAME_STATUS_WAIT = 200,//等待状态

    SG_GAME_STATE_PLAY = 100,//下注状态
    SH_GAME_STATE_GAME_END = 101,//结束状态
    SH_GAME_STATE_MOVECARD_END = 102,//转盘结束

    /// <summary>
    /// 准备开始的那5秒状态
    /// </summary>
    FQZS_GAME_STATE_FREE = 0,

    /// <summary>
    /// 飞禽走兽押注状态(有20秒的时间)
    /// </summary>
    FQZS_GAME_STATE_PLAY = 100,

    /// <summary>
    /// 飞禽走兽开始进行转盘的状态
    /// </summary>
    FQZS_GAME_STATE_END = 101,

    /// <summary>
    /// 飞禽走兽结算界面打开状态
    /// </summary>
    FQZS_GAME_STATE_RESULT = 102,
}
