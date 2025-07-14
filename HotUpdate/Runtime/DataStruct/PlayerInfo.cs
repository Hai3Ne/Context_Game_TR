using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo
{
	public uint     UserID;    //玩家ID
	public string   NickName;
	public uint     FaceID;     //头像CRC
    public long     GoldNum;    //金币数量
	public byte 	Gender;

	public ushort TableID = 0xFFFF;
	public byte ChairSeat = 0xF;

	public byte PreUserStatus = (byte)EnumUserStats.US_NULL;
	public byte UserStatus = (byte)EnumUserStats.US_FREE;
    public long Grade;

    public int VipLv;
}