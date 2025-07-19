using UnityEngine;
using System.Collections.Generic;

//场景中的玩家集合类，玩家扩展信息和炮台相关
public class ScenePlayer
{
	public TablePlayerInfo  Player = new TablePlayerInfo();
	public Launcher       Launcher;
    public byte             ClientSeat;  //反转之后的客户端位置
    public uint             RateValue;
	public ushort 			LockedFishID;
	public byte			LockFishBodyIdx = 0xFF;
}

public class ScenePlayerMgr : ISceneMgr
{
    ScenePlayer[]   m_PlayerList = new ScenePlayer[ConstValue.PLAYER_MAX_NUM];
	List<ushort> mLockedFishIDList = new List<ushort> ();

	byte            m_MyClientSeat;  //自己的座位
    ushort          m_LockedFishID = 0;//锁定的鱼ID，0未锁定
	byte 			m_lockfishPartIndex = 0xFF;
    bool            m_bAutoShot = false;
    bool            m_bAutoLocked = false;//自动锁定
//    uint            m_LockInterval = 0;
	public SceneComboEftMgr	mComboEft = new SceneComboEftMgr();

    public static Dictionary<int, int> mDicGoldChange = new Dictionary<int, int>();//客户端金钱提前变化《保存客户端与服务器的金币差值》

    public void Init()
    {
		m_MyClientSeat = 0xFF;
		m_LockedFishID = 0;
		m_lockfishPartIndex = 0xFF;
		m_bAutoShot = false;
		m_bAutoLocked = false;
		mComboEft.Init ();
		MsgNotificator notify = SceneLogic.Instance.FModel.Notify;
		notify.RegisterGlobalMsg (FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
		notify.RegisterGlobalMsg (FishingMsgType.Msg_TableRoleInfoChange, OnTablePlayerGoldNumChange);
		RoleItemModel.Instance.RegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);

        ScenePlayerMgr.mDicGoldChange.Clear();
    }

	//=======================  事件处理 =================
	void OnPlayGoldNumChange(object o)
	{
		uint playerId = RoleInfoModel.Instance.Self.UserID;
		if (o != null)			
			playerId = (uint)o;
		RefreshPlayerGold (playerId);
		RefreshMatchGoldBg (playerId);
	}

	void OnTablePlayerGoldNumChange(object o)
	{
		uint userID = (uint)o;
		RefreshPlayerData (userID);
	}

	void RefreshPlayerGold(uint UserID)
	{
		byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat (UserID);
		if (m_PlayerList[clientSeat] == null)
			return;
		m_PlayerList[clientSeat].Launcher.UpdateUserGold(clientSeat);
	}

	void RefreshMatchGoldBg(uint UserID)
	{
		byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat (UserID);
		if (m_PlayerList[clientSeat] == null)
			return;
	}

	void RefreshPlayerData(uint UserID)
	{
		if (UserID == RoleInfoModel.Instance.Self.UserID)
			return;	
		byte clientSeat = SceneLogic.Instance.FModel.GetTableRolerSeat (UserID);
		if (m_PlayerList[clientSeat] == null)
			return;
		if (m_PlayerList[clientSeat].Launcher == null)
			return;
		m_PlayerList[clientSeat].Launcher.UpdatePlayerInfo(UserID);
	}

	//=======================  事件处理 =================


    public bool AutoShotOrLocked
    {
        get
        {
            return m_bAutoShot | (m_LockedFishID != 0);
        }
    }

    public bool AutoShot
    {
        get
        {
            return m_bAutoShot;
        }
    }
    public void SetAutoShot(bool bAutoShot)
    {
        m_bAutoShot = bAutoShot;
    }

    public bool AutoLocked
    {
		get {
			return m_bAutoLocked;
		}

		set {
			m_bAutoLocked = value;
            if (m_bAutoLocked == false) {
                m_LockedFishID = 0;
                m_lockfishPartIndex = 0xFF;
            }
		}
    }

    public byte MyClientSeat
    {
        get { return m_MyClientSeat; }
        set { m_MyClientSeat = value; }
    }
    public ScenePlayer MySelf
    {
        get
        {
            return m_PlayerList[m_MyClientSeat];
        }
    }

	public ScenePlayer[] PlayerList
	{
		get { return m_PlayerList;}
	}
    public ScenePlayer GetPlayer(byte clientSeat)
    {
        if (clientSeat >= m_PlayerList.Length)
            return null;
        return m_PlayerList[clientSeat];
    }

	public bool PlayerJoin(TablePlayerInfo player, byte clientSeat, uint rateValue, uint launcherType, uint launcherLevel, bool valid)
    {
        if (m_PlayerList[clientSeat] != null)
        {
            LogMgr.Log("Seat palyer already exists:" + clientSeat.ToString());
            return false;
        }
        ScenePlayer sp = new ScenePlayer();
        m_PlayerList[clientSeat] = sp;
        sp.Player = player;
        sp.ClientSeat = clientSeat;
        sp.RateValue = rateValue;
		sp.Launcher = new Launcher(launcherType, launcherLevel, valid, clientSeat, rateValue, player.LcrEngery);
        sp.Launcher.Init();
        sp.Launcher.ShowUserInfo(SceneLogic.iShowOnekey);
        //获取金币
        sp.Launcher.UpdateUserGold(clientSeat);
        return true;
    }

    public void PlayerLeave(byte clientSeat)
    {
        if (m_PlayerList[clientSeat] == null)
        {
            LogMgr.Log("Seat not found player:" + clientSeat.ToString());
            return;
        }
       
        m_PlayerList[clientSeat].Launcher.Shutdown();
        m_PlayerList[clientSeat] = null;
		SceneLogic.Instance.BulletMgr.PlayerLeave(clientSeat);
        mComboEft.ClearAll(clientSeat);
        SceneLogic.Instance.HeroMgr.RemoveHero(clientSeat);
    }

	public bool CheckCanUnLocked(ushort lockFishID, byte clientseat)
	{
		Vector3 startScrPoint = SceneLogic.Instance.FModel.GetLauncherScrStartPos(clientseat);
		Vector3 startViewPoint = SceneLogic.Instance.FModel.GetLauncherViewStartPos(clientseat);
		Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(lockFishID);
		return (fish == null  
			|| fish.IsDelay 
			|| fish.Catched 
			|| GameUtils.IsInScreen (fish.ScreenPos) == false 
			|| GameUtils.CheckLauncherAngle (fish.ScreenPos, fish.ViewPos, startScrPoint, startViewPoint) == false);
	}

	bool SelectLockFish()
	{
        if (SceneLogic.Instance.IsLOOKGuster) {
            return false;
        }

        int fishLayer = LayerMask.NameToLayer("FishLayer");
        int fishPartLayer = LayerMask.NameToLayer("FishPartLayer");
        Ray ray = Camera.main.ScreenPointToRay(GInput.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, (1 << fishLayer) | (1 << fishPartLayer));

        List<ushort> fish_list = new List<ushort>();
        List<byte> part_list = new List<byte>();
        FishRootRef fish_ref;
        byte body_part;
        for (int i = 0; i < hits.Length; i++) {
            fish_ref = hits[i].collider.gameObject.GetComponentInParent<FishRootRef>();
            if (fish_ref == null) {
                continue;
            }
            body_part = 0xFF;
            if (hits[i].collider.gameObject.layer == fishPartLayer) {
                Collider[] colliders = hits[i].collider.gameObject.GetComponents<Collider>();
                for (byte k = 0; k < colliders.Length; k++) {
                    if (colliders[k] == hits[i].collider) {
                        body_part = k;
                        break;
                    }
                }
                int _index = fish_list.IndexOf(fish_ref.fishID);
                if (_index >= 0) {
                    if (part_list[_index] == 0xFF) {
                        part_list[_index] = body_part;
                    }
                    continue;
                }
            } else {
                if (fish_list.Contains(fish_ref.fishID)) {
                    continue;
                }
            }
            fish_list.Add(fish_ref.fishID);
            part_list.Add(body_part);
        }

        if (fish_list.Count == 0) {
            return false;
        }
        int offset;
        if (m_LockedFishID > 0) {
            offset = fish_list.IndexOf(m_LockedFishID) + 1;
        } else {
            offset = 0;
        }

        int index;
        for (int i = 0, count = fish_list.Count; i < count; i++) {
            index = (i + offset) % count;
            Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(fish_list[index]);
            if (fish != null && !fish.IsDelay && fish.vo.IsLock) {
                m_LockedFishID = fish_list[index];
                m_lockfishPartIndex = part_list[index];
                return true;
            }
        }

		return false;
	}

	public void SwitchLockTarget()
	{
		Vector3 startScrPoint = SceneLogic.Instance.FModel.GetLauncherScrStartPos(MyClientSeat);
		Vector3 startViewPoint = SceneLogic.Instance.FModel.GetLauncherViewStartPos(MyClientSeat);
        Fish fish  = SceneLogic.Instance.FishMgr.GetFishBySortGold(startScrPoint, startViewPoint, mLockedFishIDList);
		if (fish == null && mLockedFishIDList.Count > 0) {
			mLockedFishIDList.Clear ();
			fish  = SceneLogic.Instance.FishMgr.GetFishBySortGold(startScrPoint, startViewPoint, mLockedFishIDList);
		}

        if (fish != null) {
			mLockedFishIDList.Add (fish.FishID);
            m_LockedFishID = fish.FishID;
            m_lockfishPartIndex = 0xFF;
        } else {
            m_LockedFishID = 0;
            m_lockfishPartIndex = 0xFF;
        }
	}

    public bool mIsTouchValid = true;//当前点击是否有效
    public bool mIsTouch = false;//当前是否点击
    public void Update(float delta)
    {
        if (GInput.GetMouseButtonDown(0)) {
            mIsTouch = true;
            if (mIsTouchValid == false) {
                mIsTouchValid = true;
            }
        } else if (GInput.GetMouseButtonUp(0)) {
            mIsTouch = false;
        }
        if (SceneLogic.Instance.IsLOOKGuster == false) {
            if (m_LockedFishID != 0) {
                //if (mIsTouch) {
                //    m_LockedFishID = 0;
                //    m_lockfishPartIndex = 0xFF;
                //} else {
                //    if (CheckCanUnLocked(m_LockedFishID, MyClientSeat)) {
                //        mLockedFishIDList.Remove(m_LockedFishID);
                //        m_LockedFishID = 0;
                //        m_lockfishPartIndex = 0xFF;
                //    }
                //}
                if (GInput.GetMouseButtonDown(0) && FishGameMode.IsTap3DScene) {
                    if (!SelectLockFish() && AutoLocked == false) {
                        //mIsTouchValid = false;
                        m_LockedFishID = 0;
                        m_lockfishPartIndex = 0xFF;
                        //SceneLogic.Instance.LogicUI.SetAutoLock(false);
                    }
                }

                if (CheckCanUnLocked(m_LockedFishID, MyClientSeat)) {
                    mLockedFishIDList.Remove(m_LockedFishID);
                    m_LockedFishID = 0;
                    m_lockfishPartIndex = 0xFF;
                }
            } else if (AutoLocked && mIsTouch == false) {//鼠标点击中 暂时取消自动锁定效果
                this.SwitchLockTarget();
            } else if (GInput.GetMouseButtonDown(0) && FishGameMode.IsTap3DScene) {
                SelectLockFish();
            }
        }
        if (MySelf.LockedFishID != m_LockedFishID) {//锁定鱼变化时，临时取消射击CD
            MySelf.Launcher.m_LauncherTime = 0;
        }
		ushort lastLockedfishId = 0;
        for (int i = 0; i < ConstValue.PLAYER_MAX_NUM; ++i)
        {
            if (m_PlayerList[i] == null)
                continue;
            ScenePlayer sp = m_PlayerList[i];
			if (sp.LockedFishID != 0 && sp.ClientSeat != MySelf.ClientSeat)
			{
				lastLockedfishId = sp.LockedFishID;
				if (CheckCanUnLocked (sp.LockedFishID, sp.ClientSeat)) 
				{
					sp.LockedFishID = 0;
					sp.LockFishBodyIdx = 0xFF;
				}
			}
            sp.Launcher.Update(delta);
			mComboEft.Update(delta);
			EffectProgressMgr.Update (delta);
        }

		lastLockedfishId = MySelf.LockedFishID;
		MySelf.LockedFishID = m_LockedFishID;
        MySelf.LockFishBodyIdx = m_lockfishPartIndex;
    }

    public ushort LockedFishID
    {
        get
        {
            return m_LockedFishID;
        }
    }

	public byte LockfishPartIndex
	{
		get { return m_lockfishPartIndex;}
	}


    public void OnChangeLauncher(SC_GR_SetLauncher ncc)
    {
		GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.SwitchLauncher, false, true);
        bool launcherValid = true;
		uint lrCfgID = ncc.LrCfgID;
		uint lrCfgLevel = (uint)ncc.LrcLevel;
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)ncc.ChairID);
		ScenePlayer sp = GetPlayer (clientSeat);
		if (sp == null)
            return;
		
		launcherValid = SceneLogic.Instance.FModel.IsLauncherValid(ncc.LrCfgID, ncc.LrcLevel);
		if (sp.Launcher.Vo.LrCfgID != lrCfgID || sp.Launcher.Vo.Level != lrCfgLevel)
			sp.Launcher.ChangeLauncher(lrCfgID, lrCfgLevel, launcherValid);
		if (clientSeat == MyClientSeat) {
            GameConfig.SetLauncherInfo(SceneLogic.Instance.RoomVo, lrCfgID, (byte)lrCfgLevel, 0);
		}
    }

    public void OnChangeRateValue(SC_GR_SetLrMulti ncc)
	{
		uint rateValue = ncc.Multi;
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)ncc.ChairID);
		ScenePlayer sp = GetPlayer (clientSeat);
		if (sp == null)
			return;
		sp.RateValue = rateValue;
		sp.Launcher.ChangeRate (rateValue, true);
		if (clientSeat == m_MyClientSeat) {
            mComboEft.CloseCombo();
            GameConfig.SetLauncherInfo(SceneLogic.Instance.RoomVo, 0, 0, rateValue);
		}
	}

    public void OnEngeryUpdate(SC_GR_BulletCatch cmd) {
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);
		ScenePlayer sp = GetPlayer(clientSeat);

		if (sp != null)
			sp.Launcher.UpdateEnergy (cmd.Energy);
	}

	public void OnPlayerLaunchBranchBullet(SC_GR_BranchBullet cmd, bool isNet = true){
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat (cmd.SeatID);
		ScenePlayer sp = GetPlayer(clientSeat);
		if (sp == null)
			return;
		if (SceneLogic.Instance.IsLOOKGuster == false && isNet && clientSeat == MyClientSeat) {
            SceneLogic.Instance.BulletMgr.mSendBulletCount--;
            ScenePlayerMgr.mDicGoldChange.Remove(cmd.ArrBulletID[0]);
			return;
        }
        if (SceneLogic.Instance.mIsXiuYuQi) {//休渔期不发射子弹
            return;
        }
        this.launch_BranchBullet(cmd, clientSeat, sp, isNet);
	}

    public void OnPlayerLaunchBullet(SC_GR_Bullet cmd, bool isNet = true)
    {
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat (cmd.SeatID);
        ScenePlayer sp = GetPlayer(clientSeat);
        if (sp == null)
            return;
		if (SceneLogic.Instance.IsLOOKGuster == false && isNet && clientSeat == MyClientSeat) {
            SceneLogic.Instance.BulletMgr.mSendBulletCount--;
            ScenePlayerMgr.mDicGoldChange.Remove(cmd.BulletID);
			return;
        }
        if (SceneLogic.Instance.mIsXiuYuQi) {//休渔期不发射子弹
            return;
        }

        this.launch_bullet(cmd, clientSeat, sp, isNet);
    }

    private Fish refersh_bullet_lock(ushort lock_fish, ScenePlayer sp) {//更新子弹锁定鱼的信息
        if (lock_fish == 0xffff) {
            Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(sp.LockedFishID);
            if (fish == null || fish.IsDelay || fish.Catched || fish.IsInView() == false) {
                //return null;
            } else {
                return fish;
            }

            Vector3 startScrPoint = SceneLogic.Instance.FModel.GetLauncherScrStartPos(MyClientSeat);
            Vector3 startViewPoint = SceneLogic.Instance.FModel.GetLauncherViewStartPos(MyClientSeat);
            List<Fish> list = SceneLogic.Instance.FishMgr.GetLockFishList(startScrPoint, startViewPoint, null);
            if (list.Count > 0) {//机器人锁定鱼改为完全随机锁定
                Random.State _state = Random.state;
                Random.InitState((int)sp.Player.UserID);
                fish = list[Random.Range(0, list.Count)];
                Random.state = _state;
                return fish;
            } else {
                return null;
            }
            //return SceneLogic.Instance.FishMgr.GetFishBySortGold(startScrPoint, startViewPoint, null);
        } else if (lock_fish > 0) {
            return SceneLogic.Instance.FishMgr.FindFishByID(lock_fish);
        } else {
            return null;
        }
    }

    private void launch_BranchBullet(SC_GR_BranchBullet cmd, byte clientSeat, ScenePlayer sp, bool isNet) {
        uint nowTicks = UTool.GetTickCount();
        float delaySecs;
        if (nowTicks > cmd.Tick) {
            delaySecs = - (nowTicks - cmd.Tick) * 0.001f;
        } else {
            delaySecs = + (cmd.Tick - nowTicks) * 0.001f;
        }
		short degree = 0;//
		int ReboundCount = 0;
		BulletEffectVo lcrEffvo = null;
		if (FishConfig.CheckLcrEffType(sp.Launcher.Vo, EnumLauncherEffectType.REBOUND, out lcrEffvo)) {
			ReboundCount = lcrEffvo.Value0;
        }

        if (cmd.LockFishID == 0xffff) {
            Fish fish = this.refersh_bullet_lock(cmd.LockFishID, sp);
            if (fish != null) {//如果有锁定的鱼，则优先使用锁定的鱼进行角度判定
                Vector3 lockPos;
                if (cmd.FishPartID != 0xFF) {
                    if (fish.GetBodyPartScreenPos(cmd.FishPartID, out lockPos) == false) {
                        lockPos = fish.ScreenPos;
                    }
                } else {
                    lockPos = fish.ScreenPos;
                }
                degree = Utility.FloatToShort(SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat).Launcher.GetLaunchByLockFishAngle(lockPos));
                sp.LockedFishID = fish.FishID;
            } else {
                degree = SceneLogic.Instance.FModel.AngleInversion(cmd.ArrDegree[0]);
                sp.LockedFishID = 0;
            }
        } else {
            degree = SceneLogic.Instance.FModel.AngleInversion(cmd.ArrDegree[0]);
            sp.LockedFishID = cmd.LockFishID;
        }
		sp.LockFishBodyIdx = cmd.FishPartID;

		if (SceneLogic.Instance.IsLOOKGuster && clientSeat == MyClientSeat) {
			this.m_LockedFishID = sp.LockedFishID;
			this.m_lockfishPartIndex = sp.LockFishBodyIdx;
		}

		short degreeOffset = 0;
		for (int i = 0; i < cmd.ArrBulletID.Length; i++) {
			if (i > 0)
				degreeOffset = SceneLogic.Instance.FModel.AngleInversion(cmd.ArrDegree[i]);
            SceneLogic.Instance.BulletMgr.OnLaunchBullet(clientSeat, cmd.Handler, cmd.ArrBulletID[i], sp.Launcher, sp.RateValue, degree, delaySecs,
                ReboundCount, sp.LockedFishID, sp.LockFishBodyIdx, cmd.ArrBuffID, degreeOffset, i == 0);
        }
        sp.Launcher.OnLauncherBullet(Mathf.Max(0, -delaySecs));
		uint gold = SceneLogic.Instance.FModel.CalLauncherConsume(sp.RateValue, sp.Launcher.Vo);
        ConsumeGold(clientSeat, (int)gold, sp, cmd.ArrBulletID[0], isNet);     
	}


    private void launch_bullet(SC_GR_Bullet cmd, byte clientSeat, ScenePlayer sp, bool isNet) {
		if (cmd == null)
			return;
		if (sp.Launcher == null || sp.Launcher.IsShutdown)
			return;
        uint nowTicks = UTool.GetTickCount();
        float delaySecs;
        if (nowTicks > cmd.Tick) {
            delaySecs = - (nowTicks - cmd.Tick) * 0.001f;
        } else {
            delaySecs = + (cmd.Tick - nowTicks) * 0.001f;
        }
        int ReboundCount = 0;
        BulletEffectVo lcrEffvo = null;
        if (FishConfig.CheckLcrEffType(sp.Launcher.Vo, EnumLauncherEffectType.REBOUND, out lcrEffvo)) {
            ReboundCount = lcrEffvo.Value0;
        }

        short degree;
        if (cmd.LockFishID == 0xffff) {
            //Debug.LogError("cmd.LockFishID:" + cmd.LockFishID+"  seat:"+sp.ClientSeat);
            Fish fish = this.refersh_bullet_lock(cmd.LockFishID, sp);
            if (fish != null) {//如果有锁定的鱼，则优先使用锁定的鱼进行角度判定
                Vector3 lockPos;
                if (cmd.FishPartID != 0xFF) {
                    if (fish.GetBodyPartScreenPos(cmd.FishPartID, out lockPos) == false) {
                        lockPos = fish.ScreenPos;
                    }
                } else {
                    lockPos = fish.ScreenPos;
                }
                degree = Utility.FloatToShort(SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat).Launcher.GetLaunchByLockFishAngle(lockPos));
                sp.LockedFishID = fish.FishID;
            } else {
                degree = SceneLogic.Instance.FModel.AngleInversion(cmd.Degree);
                sp.LockedFishID = 0;
            }
        } else {
            degree = SceneLogic.Instance.FModel.AngleInversion(cmd.Degree);
            sp.LockedFishID = cmd.LockFishID;
        }
        sp.LockFishBodyIdx = cmd.FishPartID;

        if (SceneLogic.Instance.IsLOOKGuster && clientSeat == MyClientSeat) {
            this.m_LockedFishID = sp.LockedFishID;
            this.m_lockfishPartIndex = sp.LockFishBodyIdx;
        }

        SceneLogic.Instance.BulletMgr.OnLaunchBullet(clientSeat, cmd.Handler, cmd.BulletID, sp.Launcher, sp.RateValue, degree, delaySecs,
            ReboundCount, sp.LockedFishID, sp.LockFishBodyIdx, cmd.ArrBuffID);

        sp.Launcher.OnLauncherBullet(Mathf.Max(0, -delaySecs));
		if (SceneLogic.Instance.BulletMgr.BufferMgr.CheckHasFreeLCR (cmd.ArrBuffID)) {
			
		} else {
			uint gold = SceneLogic.Instance.FModel.CalLauncherConsume (sp.RateValue, sp.Launcher.Vo);
            ConsumeGold(clientSeat, (int)gold, sp, cmd.BulletID, isNet);     
		}
    }

	public void OnPlayerXPSkill(NetCmdPack pack)
	{
        SC_GR_LauncherSkill skillCaster = pack.ToObj<SC_GR_LauncherSkill>();
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)skillCaster.ChairID);
		//skillCaster.Angle
		//byte clientSeat, short degree
		ScenePlayer sp = GetPlayer(clientSeat);
		if (sp != null) {
			sp.Launcher.OnPlayerXPSkill (clientSeat, skillCaster.Angle);
		}
	}

	/*
    public void LaunchSyncBullet(NetCmdPack pack)
    {
        SC_GR_SyncBullet cmd = (SC_GR_SyncBullet)pack.cmd;
        float elapsedTime = (UTool.GetTickCount() - pack.tick) * 0.001f + FishGameMode.NetDelayTime;
        for(int i = 0; i < cmd.Bullets.Length; ++i)
        {
            SyncBulletData data = cmd.Bullets[i];
			byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)data.ChairID);
            ScenePlayer sp = GetPlayer(clientSeat);
            if (sp == null) return;
			short degree = SceneLogic.Instance.FModel.AngleInversion(data.Degree);
			uint launcherType, launcherLevel;
			SceneLogic.Instance.FModel.FetchLauncherTypeLevel (data.BulletTypeID, out launcherType, out launcherLevel);
			SceneLogic.Instance.BulletMgr.OnLaunchBullet (clientSeat, data.BulletID, launcherType, launcherLevel, data.RateIdx, degree, data.Time * 0.001f + elapsedTime, data.ReboundCount, data.LockFishID, 0);
        }
    }
  //*/
    public void ShowOtherUserLocked(byte clientSeat)
    {
		if (SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat) != null)
        {
			//SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat).Launcher.ShowOtherUserLocked();
        }
    }
    public void ClearPlayer()
    {
        for (int i = 0; i < m_PlayerList.Length; ++i)
        {
            if (m_PlayerList[i] == null || m_PlayerList[i].ClientSeat == MyClientSeat)
                continue;

            m_PlayerList[i].Launcher.Shutdown();
            m_PlayerList[i] = null;
        }
    }
    public void ClearAllPlayer()
    {
        for (int i = 0; i < m_PlayerList.Length; ++i)
        {
            if (m_PlayerList[i] == null)
                continue;

            m_PlayerList[i].Launcher.Shutdown();
            m_PlayerList[i] = null;
        }
    }
    public void Shutdown()
    {
        for(int i = 0; i < m_PlayerList.Length; ++i)
        {
            if(m_PlayerList[i] != null)
            {
                m_PlayerList[i].Launcher.Shutdown();
                m_PlayerList[i] = null;
            }            
        }
		mLockedFishIDList.Clear ();
		mComboEft.ShutDown ();
		RoleInfoModel.Instance.UnRegisterGlobalMsg(FishingMsgType.Msg_GoldNumChange, OnPlayGoldNumChange);
		EffectProgressMgr.Shutdown ();
    }

    void ConsumeGold(byte clientSeat, int gold, ScenePlayer sp,ushort bullet_id, bool isNet)//发射子弹扣除金币数
    {
        if(m_PlayerList[clientSeat] == null)
        {
			LogMgr.Log("client Player is null.");
            return;
        }
        if (isNet == false) {
            ScenePlayerMgr.mDicGoldChange.Add(bullet_id, -gold);
        }
        gold += gold * RoleInfoModel.Instance.RoomDeduct * 10000;//扣除金币时，直接扣除房间抽水
		SceneLogic.Instance.FModel.OnAddUserGlobelByCatchedData(clientSeat, -gold);
    }

    public void ChangeLauncherAngle(Vector3 dir, byte seat)
    {
        m_PlayerList[seat].Launcher.Direction = dir;
        m_PlayerList[seat].Launcher.UpdatOtherAngle();
    }

	public void SyncCombo(SC_GR_ComboSync combo)
    {
		mComboEft.ShowCombo(combo);
    }

    public void QuickSell(SC_GR_QuickSell sell) {
		if (sell.ErrorID != 0) {
			//错误处理
		} else {
			long price = SceneLogic.Instance.FModel.CalSelfSellPrice ();
			byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)sell.ChairID);
			if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
				if (sell.GetScore != price) {
					LogMgr.LogError ("error  server GetScore:" + sell.GetScore + " client GetScore:" + price);
				}
				SceneLogic.Instance.PlayerMgr.MySelf.Launcher.UpdateEnergy(0);//清空炮台能量
				RoleItemModel.Instance.ClearAllItem();//清空道具
				SystemMessageMgr.Instance.ShowMessageBox(string.Format(StringTable.GetString("Tip_7"), price));

                MtaManager.AddAllSellEvent(sell.GetScore);
			}
			price = sell.GetScore;
			SceneLogic.Instance.FModel.OnAddUserGlobelByCatchedData(clientSeat, price);
		}
	}
    public void ShowWorldBossRank(SC_GR_WorldBossRank cmd) {//显示全服宝箱排名信息
        WndManager.Instance.CloseUI(EnumUI.UI_BoxRank);
        WndManager.Instance.ShowUI(EnumUI.UI_BoxRank, cmd);
    }

    private void OpenWorldBossGrant(SC_GR_WorldBossGrant grant) {
        IUIController ui;
        UI_BoxRankAwardController bra = WndManager.Instance.GetController<UI_BoxRankAwardController>();
        if (bra.resp_grant != null) {
            bra.ReveiveGold();
        }
        if (bra.IsActive) {
            bra.Close();
        }
        do {
            ui = WndManager.Instance.GetCurActive();
            if (ui != null) {
                ui.Close();
            } else {
                break;
            }
        } while (true);
        WndManager.Instance.ShowUI(EnumUI.UI_BoxRankAward, grant);
    }
    public float WorldBossDieTime = 0;//世界BOSS被打死时间，只有本人打死才会记录
    public void WorldBossGrant(SC_GR_WorldBossGrant grant) {//全服宝箱消耗榜排名奖励发放
        if (grant.SeatID == SceneLogic.Instance.FModel.SelfServerSeat) {
            if (ScenePlayerMgr.mDicGoldChange.ContainsKey(-1)) {//-1 表示全服宝箱消耗榜奖励
                ScenePlayerMgr.mDicGoldChange[-1] += (int)grant.Grant;
            } else {
                ScenePlayerMgr.mDicGoldChange.Add(-1, (int)grant.Grant);
            }
            if (Time.realtimeSinceStartup > this.WorldBossDieTime + 3) {
                this.OpenWorldBossGrant(grant);
            } else {
                TimeManager.DelayExec(this.WorldBossDieTime + 3 - Time.realtimeSinceStartup, () => {//本人打死BOSS  奖励延迟3秒显示
                    this.OpenWorldBossGrant(grant);
                });
            }

            MtaManager.AddWorldBoxAward(grant.Grant, grant.Rank, 3);//消耗榜奖励
        } else {
            SceneLogic.Instance.FModel.OnAddUserGlobelByCatchedData(SceneLogic.Instance.FModel.ServerToClientSeat(grant.SeatID), grant.Grant);
        }
    }
    public void SyncPlayingScore(SC_GR_SyncPlayingScore end) {//游戏结束，同步金币数量
        TablePlayerInfo tabPlayer = SceneLogic.Instance.FModel.GetTabPlayerBySSeat((byte)end.ChairID);
        if (tabPlayer != null) {
            tabPlayer.SetUserBaseGlobeNum(end.UserScore);
            tabPlayer.GlobeNum = 0;
            if (ScenePlayerMgr.mDicGoldChange.Count > 0) {
                var gold = ScenePlayerMgr.mDicGoldChange.Values.GetEnumerator();
                while (gold.MoveNext()) {
                    tabPlayer.GlobeNum += gold.Current;
                }
                //Debug.LogError("tabPlayer.GlobeNum:" + tabPlayer.GlobeNum);
            }
            SceneLogic.Instance.FModel.Notify.Notifiy(FishingMsgType.Msg_GoldNumChange, tabPlayer.UserID);
        }
    }
}