using System;
using System.Collections;
using System.Collections.Generic;

public enum EnumGameMode
{
	Mode_Energy = 0,// 正常能量模式
	Mode_Time = 1,	// 时段模式
    Mode_NotItem = 2,  //经典模式（无道具模式）
    //Mode_Score = 3, //积分模式
    Mode_PK = 3,    //PK模式
}
public enum EnumCoinMode {
    Score = 0,//积分模式
    Gold = 1,//金币模式
}
public enum RoomError
{
	TE_Sucess = 1,//可以进入
	TE_MinGlobel = 5,//金币太少
	TE_IsNotExists = 4
}