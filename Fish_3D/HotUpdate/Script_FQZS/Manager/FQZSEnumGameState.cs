using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 飞禽走兽状态
/// </summary>
public enum FQZSEnumGameState
{
    /// <summary>
    /// 空状态
    /// </summary>
    None,
    /// <summary>
    /// 准备开始的状态(5秒倒计时)
    /// </summary>
    Free,
    /// <summary>
    /// 押注的状态
    /// </summary>
    Bet,
    /// <summary>
    /// 开始进行转盘的状态(这个状态有20秒的时间)
    /// </summary>
    End,
    /// <summary>
    /// 打开结算界面状态
    /// </summary>
    Reslut,
}
