using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神话状态
/// </summary>
public enum SHEnumGameState {
    /// <summary>
    /// 等待擂主上擂
    /// </summary>
    Wait,
    /// <summary>
    /// 开始押注
    /// </summary>
    Bet,
    /// <summary>
    /// 转盘转动以及结算结果
    /// </summary>
    Result,
}
