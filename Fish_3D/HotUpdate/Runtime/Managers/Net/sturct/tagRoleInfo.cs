using UnityEngine;
using System.Collections;
using System;

public class tagRoleInfo
{
	[TypeInfo(0)]
	public UInt32 dwUserID;
	[TypeInfo(1, NetDataConstValues.NickNameLength + 1)]
	public string NickName;
	[TypeInfo(2)]
	public UInt16 wLevel;//玩家点击
	[TypeInfo(3)]
	public UInt32 dwExp;//玩家经验
	[TypeInfo(4)]
	public UInt32 dwFaceID;//玩家头像ID
	[TypeInfo(5)]
	public bool bGender; //玩家性别
	[TypeInfo(6)]
	public UInt32 dwGlobeNum;
	[TypeInfo(7)]
	public UInt32 dwMedalNum;
	[TypeInfo(8)]
	public UInt32 dwCurrencyNum;  //钻石
	[TypeInfo(9)]
	public UInt32 dwAchievementPoint;
	[TypeInfo(10)]
	public Byte TitleID;
	[TypeInfo(11)]
	public UInt32 dwProduction;//当天获得的金币数量
	[TypeInfo(12)]
	public UInt32 dwGameTime;//当天的游戏时间
	[TypeInfo(13)]
	public Byte SendGiffSum;//发送赠送的次数 当天
	[TypeInfo(14)]
	public Byte AcceptGiffSum;//接收赠送的次数 当天
	[TypeInfo(15)]
	public int256 TaskStates;
	[TypeInfo(16)]
	public int256 AchievementStates;
	[TypeInfo(17)]
	public int256 ActionStates;
	[TypeInfo(18)]
	public UInt16 OnlineMin;
	[TypeInfo(19)]
	public UInt32 OnlineRewardStates;
	[TypeInfo(20, NetDataConstValues.MAX_CHARM_ITEMSUM)]
	public UInt32[] CharmArray;
	[TypeInfo(21)]
	public bool IsCanResetAccount;
	[TypeInfo(22)]
	public UInt32 AchievementPointIndex;
	[TypeInfo(23)]
	public UInt32 ClientIP;
	[TypeInfo(24, NetDataConstValues.MAX_ADDRESS_LENGTH_IP+1)]
	public string IPAddress;
	[TypeInfo(25)]
	public UInt32 CheckData;
	[TypeInfo(26)]
	public bool IsShowIPAddress;//默认值为true
	[TypeInfo(27)]
	public UInt32 ExChangeStates;
	[TypeInfo(28)]
	public UInt32 TotalRechargeSum;
	[TypeInfo(29)]
	public bool bIsFirstPayGlobel;
	[TypeInfo(30)]
	public bool bIsFirstPayCurrcey;
	[TypeInfo(31)]
	public UInt32 LotteryScore;
	[TypeInfo(32)]
	public Byte LotteryFishSum;
	//Vip数据
	[TypeInfo(33)]
	public Byte VipLevel;//不涉及数据库的
	//月卡数据
	[TypeInfo(34)]
	public Byte MonthCardID;
	[TypeInfo(35)]
	public Int64 MonthCardEndTime;
	[TypeInfo(36)]
	public Int64 GetMonthCardRewardTime;
	//倍率
	[TypeInfo(37)]
	public int256 RateValue;//开启的倍率的数值
	[TypeInfo(38)]
	public byte CashSum;
	[TypeInfo(39)]
	public Byte benefitCount;//
	[TypeInfo(40)]
	public UInt32 benefitTime;//
	[TypeInfo(41)]
	public UInt32 TotalUseMedal;//
	[TypeInfo(42)]
	public UInt32 ParticularStates;//
	[TypeInfo(43)]
	public UInt32 GameID;//
	[TypeInfo(44)]
	public bool bShareStates;//
	[TypeInfo(45)]
	public UInt32 TotalCashSum;//
}
