using System;
/// <summary>
/// // 桌子上的玩家信息
/// </summary>
public class TablePlayerInfo
{
	public ushort TableID;
	public byte SeatID;
	public uint UserID;
	public string NickName;
	public int VipLv;//玩家会员等级
	public uint FaceID;//玩家头像ID
	public bool Gender; //玩家性别
	public long GlobeNum; //玩家金币

	long mBaseGlobeNum = 0;
	public long BaseGlobeNum{ get { return mBaseGlobeNum;} }
	public long LcrEngery;
	public long curEngery;
	public uint ServerLauncherTypeID, RateValue;
	public TablePlayerInfo(){}

	public TablePlayerInfo(PlayerJoinTableInfo pInfo)
	{
        this.CopyFrom(pInfo.playerInfo);
        this.GlobeNum = pInfo.ChangeGold;
		this.LcrEngery = pInfo.lcrEngery;
		this.ServerLauncherTypeID = pInfo.ServerLauncherTypeID;
		this.RateValue = pInfo.RateValue;
		this.curEngery = LcrEngery;
	}

	public void SetUserBaseGlobeNum(long gold){
		this.mBaseGlobeNum = gold;
        if (gold < 0) {
            LogMgr.LogError("初始金额错误 gold:" + gold);
        }
	}

	public void CopyFrom(PlayerInfo pInfo)
	{
		this.TableID = pInfo.TableID;
		this.SeatID = pInfo.ChairSeat;
		this.UserID = pInfo.UserID;
		this.NickName = pInfo.NickName;
		this.VipLv = pInfo.VipLv;
		this.FaceID = pInfo.FaceID;
		this.Gender = pInfo.Gender != 0;
		this.mBaseGlobeNum = pInfo.GoldNum;
		this.GlobeNum = 0;
	}
}