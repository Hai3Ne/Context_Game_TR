using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameEvent {
    //大厅相关
    Hall_UserInfoChange,//用户信息变更
    Hall_SCUpdate,//首充状态更新
    Hall_Share,//分享状态更新

    //游戏中用户管理
    UserInfoChange,//用户信息变更
    UserEneter,//用户进入
    UserStateChange,//用户状态变更
    UserLeaveTable,//用户离开桌子
    UserEnterTable,//用户进入桌子

    //五子棋相关
    CurPlaySeatChange,//当前操作玩家变更  用来通知该谁下子
    BegStatus,//请求状态1:等待求和,2:等待悔棋
}
