using System;

//桌子
public class tagRemoteRoleInfo
{
	[TypeInfo(0)]
	public Byte SeatID;
	[TypeInfo(1)]
	public UInt32 dwUserID;
	[TypeInfo(2, NetDataConstValues.NickNameLength + 1)]
	public string NickName;
	[TypeInfo(3)]
	public UInt16 wLevel;//玩家点击
	//[TypeInfo(4)]
	//public UInt32 dwExp;//玩家经验
	[TypeInfo(4)]
	public UInt32 dwFaceID;//玩家头像ID
	[TypeInfo(5)]
	public bool bGender; //玩家性别
	[TypeInfo(6)]
	public UInt32 dwGlobeNum; //玩家金币
	[TypeInfo(7, NetDataConstValues.MAX_CHARM_ITEMSUM)]
	public UInt32[] CharmArray;
	[TypeInfo(8)]
	public UInt32 dwAchievementPoint;
	[TypeInfo(9)]
	public Byte TitleID;
	[TypeInfo(10, NetDataConstValues.MAX_ADDRESS_LENGTH_IP+1)]
	public string	IPAddress;
	[TypeInfo(11)]
	public Byte VipLevel;
	[TypeInfo(12)]
	public bool IsInMonthCard;
	[TypeInfo(13)]
	public UInt32 GameID;
}