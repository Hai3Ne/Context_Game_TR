using UnityEngine;
using System.Collections;

/// <summary>
/// 游戏枚举
/// </summary>
public enum GameEnum
{
    None = 0,
    /// <summary>
    /// 大厅
    /// </summary>
    All = 1,
    /// <summary>
    /// 3D捕鱼
    /// </summary>
    Fish_3D = 2,
    /// <summary>
    /// 李逵劈鱼
    /// </summary>
    Fish_LK = 3,
    /// <summary>
    /// 五子棋
    /// </summary>
    WZQ = 4,
    /// <summary>
    /// 神兽
    /// </summary>
    SH = 5,
    /// <summary>
    /// 飞禽走兽
    /// </summary>
    FQZS = 6,
}

public enum LoadType
{
    DownLoad,
    LoadConfig,
}
