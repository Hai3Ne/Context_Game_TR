using System;
using System.Collections.Generic;
public class JoinRoomInfo
{
	public byte Result;
	public uint RoomID;
	public ushort TableID = 0xFFFF;
	public byte Seat = 0x0F;
	public uint ServerLauncherTypeID;
	public uint BackgroundImage;
	public uint RateValue;
	public long LcrEngery;
	public HashSet<ushort> PlayerSeatList = new HashSet<ushort>();
	public EnumRoomType roomType;
	public tagHeroCache[] HeroData;
	public List<tagBuffCache[]> buffCacheList;
	public ushort LastBulletID;
    public float mXiuYuQiEndTime;//休渔期结束时间
	public bool IsLookOn = false;
	public PlayerJoinTableInfo[] playerJoinArray;
}


public class PlayerJoinTableInfo
{
	public PlayerInfo playerInfo;
    public byte Seat;
    public long ChangeGold;//更改金币
	public long lcrEngery;
	public uint ServerLauncherTypeID, RateValue;
}