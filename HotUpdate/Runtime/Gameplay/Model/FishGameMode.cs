using UnityEngine;
using System;
using System.Collections.Generic;

public class FishGameMode : ISceneMgr
{
    bool                 SeatInversion;  //是否反转
    uint                 m_BackgroundIndex;
    public bool                 m_MouseValid = true;        //判断鼠标是否处于有效区
    public byte                 m_DelayCount = 1;           //按纽点击延时
	static float                DelayTime = 0.01f;//10毫秒的网络延迟

	public static EnumGameMode currentModel = EnumGameMode.Mode_Time;

	public static bool IsTrigle3dTapcallback = false;

	static List<Func<bool>> tap3dSceneCallbacks = new List<Func<bool>>();
	public static void AddListTap3Dscene(Func<bool> cb)
	{
		if (!tap3dSceneCallbacks.Contains(cb))
			tap3dSceneCallbacks.Add (cb);
	}
	public static void RemoveListTap3Dscene(Func<bool> cb)
	{
		if (tap3dSceneCallbacks.Contains (cb))
			tap3dSceneCallbacks.Remove (cb);
	}
	public static bool OnTap3Dscene()
	{
		if (IsTrigle3dTapcallback)
			return false;
		bool isblock3d = false;
		tap3dSceneCallbacks.ForEach (x => isblock3d = isblock3d || x.Invoke ());
		IsTrigle3dTapcallback = true;
		GInput.IsBlock3D = isblock3d;
		return isblock3d;
	}

    public static bool IsTap3DScene
    {
        get 
		{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR
				UICamera.Raycast (GInput.mousePosition);
				bool is3dtap =  UICamera.hoveredObject == SceneObjMgr.Instance.UIRoot && (!DllTest.WaitExecuteResult);
				if (is3dtap && !IsTrigle3dTapcallback)
				{
					return !OnTap3Dscene();
				}
				return is3dtap;
			#else
				UICamera.Raycast (GInput.mousePosition);
				bool is3dtap = UICamera.hoveredObject == SceneObjMgr.Instance.UIRoot;
				if (is3dtap && GInput.GetMouseButton (0) && !IsTrigle3dTapcallback)
				{
					return !OnTap3Dscene();
				}
				return is3dtap;
			#endif
		}
    }
	private Dictionary<uint, TablePlayerInfo> mTablePlayerIDMap = new Dictionary<uint, TablePlayerInfo>();//桌子上的玩家处理 一定是先获得玩家桌子数据 
	private Dictionary<Byte, TablePlayerInfo> mTablePlayerSeatMap = new Dictionary<Byte, TablePlayerInfo>();

	MsgNotificator mNotify;
	public MsgNotificator Notify { get { return mNotify; }}
	public void Init()
	{
		mNotify = new MsgNotificator ();
		LauncherPositionSetting.Init ();
		lastTimeTick = GlobalTimer.m_Watch.Elapsed;
		RoleInfoModel.Instance.RegisterGlobalMsg (SysEventType.UserScoreChange, HandleUserSocreChange);
	}

	public void OnPlayerJoin(JoinRoomInfo joinroomInfo)
	{
		TablePlayerInfo selfPlayInfo = new TablePlayerInfo ();
		selfPlayInfo.CopyFrom (RoleInfoModel.Instance.Self);
		selfPlayInfo.TableID = joinroomInfo.TableID;
		selfPlayInfo.SeatID = joinroomInfo.Seat;
		selfPlayInfo.LcrEngery = joinroomInfo.LcrEngery;
		selfPlayInfo.RateValue = joinroomInfo.RateValue;
		selfPlayInfo.ServerLauncherTypeID = joinroomInfo.ServerLauncherTypeID;
		OnPlayerJoin (selfPlayInfo);
	}

	void HandleUserSocreChange(object data){
		PlayerInfo playerInfo = data as PlayerInfo;
		if (playerInfo != null) {
            TablePlayerInfo info;
            if(mTablePlayerIDMap.TryGetValue(playerInfo.UserID,out info)){
                info.SetUserBaseGlobeNum(playerInfo.GoldNum);
				Notify.Notifiy (FishingMsgType.Msg_GoldNumChange, playerInfo.UserID);
			} else {
                if (LogMgr.ShowLog) {
                    LogMgr.LogWarning("User【" + playerInfo.UserID + "】Not Exist in current tablbe. " + playerInfo.TableID);
                }
			}
		} else {
            if (LogMgr.ShowLog) {
                LogMgr.LogWarning("playerInfo is null");
            }
		}
	}

	public void OnPlayerJoin(TablePlayerInfo tablePlayer)
	{
		tablePlayer.curEngery = tablePlayer.LcrEngery;
		mTablePlayerIDMap.TryAdd (tablePlayer.UserID, tablePlayer);
		mTablePlayerSeatMap.TryAdd (tablePlayer.SeatID, tablePlayer);
	}

	public void OnPlayerLeave(Byte seat)
	{
		TablePlayerInfo tablUsr = null;
		if (mTablePlayerSeatMap.TryGetValue (seat, out tablUsr)) {
			mTablePlayerIDMap.Remove (tablUsr.UserID);
			mTablePlayerSeatMap.Remove (seat);
		}
	}

	TimeSpan lastTimeTick;
	public void Update (float delta)
	{
		TimeSpan ts = GlobalTimer.m_Watch.Elapsed.Subtract(lastTimeTick);
		lastTimeTick = GlobalTimer.m_Watch.Elapsed;
		if (SceneLogic.Instance.RoomType == EnumRoomType.Normal_TimeLimit)
		{
			long tickSec = ts.Ticks;
			foreach (var tabP in mTablePlayerSeatMap) {
				if (tabP.Value.curEngery > 0) {
					tabP.Value.curEngery -= tickSec;
				}
			}
		}
	}

	//判断炮台是否可用
	public bool IsCanUseLauncher(uint launcherType, uint launcherLevel)
	{
		return true;
	}

    //服务器位置到客户端位置转换
    public static float NetDelayTime   {  get { return DelayTime; }}

    public short AngleInversion(short angle)
    {
        return Inversion ?(short) -angle : (short)angle;    
    }

    public byte ServerToClientSeat(byte seat)
    {
		return Inversion ? LauncherPositionSetting.SeatMapping[seat] : seat;
    }
    
    public byte ClientToServerSeat(byte seat)
    {
		return Inversion ? LauncherPositionSetting.SeatMapping[seat] : seat;
    }

    public byte LauncherPrefabIndx(byte seat, bool bMyself = false)
    {
		return bMyself ?  (byte)0 : LauncherPositionSetting.IndxMapping[seat];
    }

	public void CheckLauncherValid(uint serverLauncherType, out uint clientLauncherType, out uint level,  out bool valid)
    {
		clientLauncherType = serverLauncherType & 0xFFFFFF;
		level = (serverLauncherType >> 24) & 0x7f;
		valid = (serverLauncherType >> 31) != 0;
    }

	public bool IsLauncherValid(uint lrCfgID, uint lrCfgLv)
	{
		return true;
	}

	public void FetchLauncherTypeLevel(uint serverLauncherID, out uint launcherCfgId, out uint launcherLevel)
	{
		launcherCfgId = serverLauncherID & 0xFFFFFF;
		launcherLevel = (serverLauncherID >> 24) & 0x7f;
	}


    public bool Inversion { get {return SeatInversion;}	set{SeatInversion = value;}}

	Byte mSelfSeat = 0xFF;
	public Byte SelfServerSeat{ get{ return mSelfSeat;} set{ mSelfSeat = value; }}
	public Byte SelfClientSeat{ get{ return ServerToClientSeat(mSelfSeat);}}


    public void GetBulletPosAndDir(byte clientSeat, short angle, out  Vector3 dir, out Vector3 pos)
    {
		LauncherPositionSetting.GetBulletPosAndDir (clientSeat, angle, out dir, out pos);
    }

	public Vector3 GetBulletPos(byte clientSeat, short angle) {
		Vector3 dir, pos;
		LauncherPositionSetting.GetBulletPosAndDir (clientSeat, angle, out dir, out pos);
		return pos;
	}

	public Vector3 GetBulletDir(byte clientSeat, short angle) {
		Vector3 dir, pos;
		LauncherPositionSetting.GetBulletPosAndDir (clientSeat, angle, out dir, out pos);
		return dir;
	}


    public void BuuletIDToSeat(ushort bulletId, out byte clientSeat, out byte id)
    {
        id = (byte)bulletId;
        clientSeat = ServerToClientSeat((byte)(bulletId >> 8));
    }

    //返回炮台0和炮台1的起始位置
    public Vector3 GetLauncherScrStartPos(byte clientIdx)
    {
        if (clientIdx == 0)
			return LauncherPositionSetting.LauncherScrStartPos1;
        else
			return LauncherPositionSetting.LauncherScrStartPos2;
    }

    public Vector3 GetLauncherViewStartPos(byte clientIdx)
    {
        if (clientIdx == 0)
			return LauncherPositionSetting.LauncherViewStartPos1;
        else
			return LauncherPositionSetting.LauncherViewStartPos2;
    }

    public uint BackgroundIndex {get{return m_BackgroundIndex;} set{m_BackgroundIndex = value;}}

    public int GetFishGold(uint fishCfgID, uint rateValue)
    {
		return (int)(rateValue * FishConfig.Instance.FishConf.TryGet(fishCfgID).Multiple);
    }

    public float ComputeZScaling(Fish fish, float scaling)
    {
        return Mathf.Max(1.0f, (fish.Position.z / 1000)) * scaling;
    }

	public bool CheckGoldEnought(byte seat, uint rateValue, LauncherVo launchervo)
	{
		return GetPlayerGlobelBySeat (seat) >= (long)CalLauncherConsume (rateValue, launchervo);
	}

	public uint CalLauncherConsume(uint rateValue, LauncherVo launchervo)
	{
		return rateValue * (uint)(launchervo.Level+launchervo.HiddenLevel);
	}

	public bool IsCanAutoFire()
	{
		return true;
	}

    public TablePlayerInfo GetTabPlayerByCSeat(byte clientSeat) {
        byte serverSeat = ClientToServerSeat(clientSeat);
        TablePlayerInfo pRole = null;
        if (mTablePlayerSeatMap.TryGetValue(serverSeat, out pRole))
            return pRole;
        return null;
    }
    public TablePlayerInfo GetTabPlayerBySSeat(byte serverSeat) {
        TablePlayerInfo pRole = null;
        if (mTablePlayerSeatMap.TryGetValue(serverSeat, out pRole))
            return pRole;
        return null;
    }
    public TablePlayerInfo GetPlayerByUserID(uint user_id) {
        TablePlayerInfo pRole;
        if (mTablePlayerIDMap.TryGetValue(user_id, out pRole)) {
            return pRole;
        } else {
            return null;
        }
    }

	public long GetPlayerGlobelBySeat(byte clientSeat)
	{
		byte serverSeat = ClientToServerSeat(clientSeat);
		TablePlayerInfo pRole = null;
		mTablePlayerSeatMap.TryGetValue(serverSeat, out pRole);
		if (pRole == null)
			return 0;
        if (pRole.GlobeNum + pRole.BaseGlobeNum >= 0) {
            return pRole.GlobeNum + pRole.BaseGlobeNum;
        } else {
            LogMgr.LogError("pRole.GlobeNum:" + pRole.GlobeNum + "  pRole.BaseGlobeNum:" + pRole.BaseGlobeNum);
            return 0;
        }
	}


	public byte GetTableRolerSeat (uint userID)
	{
		byte clientSeat = ServerToClientSeat (mTablePlayerIDMap.TryGet (userID).SeatID);
		return clientSeat;
	}

	public void OnAddUserGlobelByCatchedData(byte clientSeat, long GlobelSum)
	{
		byte serverSeat = ClientToServerSeat(clientSeat);
		uint roleUserID = 0;
		TablePlayerInfo tabPlayer = mTablePlayerSeatMap.TryGet(serverSeat);
		if (tabPlayer == null)
			return;

		tabPlayer.GlobeNum += GlobelSum;
		roleUserID = tabPlayer.UserID;
		Notify.Notifiy (FishingMsgType.Msg_GoldNumChange, roleUserID);

        if (GlobelSum > 0) {
            ScenePlayer player = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
            if (player != null && player.Launcher.mLbGold != null) {
                //if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                //    AnimBig.Begin(player.Launcher.mLbGold, 0f, 0.2f, 1.2f,new Color32(255,222,39,255),Color.red);
                //} else {

                float time = 0.2f;
                float rate = GlobelSum * 1f / SceneLogic.Instance.RoomVo.RoomMultiple;
                if (rate < 50) {
                    rate = 1.1f;
                    time = 0.2f;
                } else if (rate < 800) {
                    rate = Mathf.Sqrt((rate - 50) / (800 - 50)) * 0.1f + 1.1f;
                    time = 0.25f;
                } else if (rate < 1600) {
                    rate = Mathf.Sqrt((rate - 800) / (1600 - 800)) * 0.1f + 1.2f;
                    time = 0.3f;
                } else {
                    rate = 1.3f;
                    time = 0.4f;
                }

                AnimBig.Begin(player.Launcher.mLbGold, 0f, time, rate);
                //}
            }
        }
	}

	public void HandeCatchFishData(CatchedData cd)  //捕获鱼的结果
	{
		if (cd == null)
			return;
		
		uint Exp = 0;
		int GoldNum = 0;
		if (cd.FishList.Count > 0) 
		{
			for (int i = 0; i < cd.FishList.Count; ++i)
			{
				FishVo fishvo = FishConfig.Instance.FishConf.TryGet (cd.FishList [i].FishCfgID);
				Exp += fishvo.Exp;
				GoldNum += cd.GetFishGoldNum (cd.FishList [i].FishObj.FishID);
			}
		}
		OnAddUserGlobelByCatchedData (cd.ClientSeat, GoldNum);
	}

	public float ComputeEffectScaling(Fish fish, float scaling)
	{
		return fish.Scaling * Mathf.Max(1.0f, (fish.Position.z / 1000)) * scaling;
	}

	public bool CheckRateValueAvaible(uint rateValue)
	{
		return Array.Exists (GetAvaibleRateValuList (), x=>x == rateValue);
	}

	public uint[] GetAvaibleRateValuList()
	{
		if (SceneLogic.Instance.RoomType == EnumRoomType.Normal_TimeLimit)
		{
			uint roomCfgId = SceneLogic.Instance.GetRoomCfgID ();
			TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet (roomCfgId);
			return roomVo.Multiple;
		}
		else if (SceneLogic.Instance.RoomType == EnumRoomType.Normal_BulletLimit) 
		{
			uint roomCfgId = SceneLogic.Instance.GetRoomCfgID ();
			EngeryRoomVo roomVo = FishConfig.Instance.EngeryRoomConf.TryGet (roomCfgId);
			return roomVo.Multiples;
		}
		return new uint[0];
	}

	uint roomMulti = 0;
	public uint RoomMulti
	{
		get 
		{
			if (roomMulti == 0) {
				uint currentRoomID = SceneLogic.Instance.GetRoomCfgID ();
				roomMulti = FishConfig.Instance.TimeRoomConf.TryGet (currentRoomID).RoomMultiple;
			}
			return roomMulti;
		}
	}

	public bool CheckIsBankrup (LauncherVo vo)
	{
		if (SceneLogic.Instance.RoomType == EnumRoomType.Normal_TimeLimit) 
		{
			return false;
		}
		else if (SceneLogic.Instance.RoomType == EnumRoomType.Normal_BulletLimit) 
		{
			uint roomCfgId = SceneLogic.Instance.GetRoomCfgID ();
			EngeryRoomVo roomVo = FishConfig.Instance.EngeryRoomConf.TryGet (roomCfgId);
			uint minRate = GameUtils.Min(roomVo.Multiples);
			return CheckGoldEnought (SelfClientSeat, minRate, vo);
		}

		return false;
	}

	public long CalSelfSellPrice(){
		long price = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.EnergyPoolLogic.CurEnergy;
		TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID());
		uint[] skillItemList = roomVo.Items;
		uint[]  heroItemIDList = roomVo.Heroes;
		for (int i = 0; i < skillItemList.Length; i++) {
			ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet(skillItemList[i]);
			int count = RoleItemModel.Instance.getItemCount(skillItemList[i]);
			price += itemVo.SalePrice * count;
		}
		for (int i = 0; i < heroItemIDList.Length; i++) {
			ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet(heroItemIDList[i]);
			int count = RoleItemModel.Instance.getItemCount(heroItemIDList[i]);
			price += itemVo.SalePrice * count;
		}
		//特殊处理  防止精度缺失
		int rate = Mathf.FloorToInt(FishConfig.Instance.GameSettingConf.QuickSell * 10000);
		price = rate * price / 10000;
		return price;
	}

	public void Shutdown()
	{
		RoleInfoModel.Instance.UnRegisterGlobalMsg (SysEventType.UserScoreChange, HandleUserSocreChange);
		IsTrigle3dTapcallback = false;
		mNotify.Dispose ();
		tap3dSceneCallbacks.Clear();
		mTablePlayerSeatMap.Clear ();
		mTablePlayerIDMap.Clear ();
	}

}
