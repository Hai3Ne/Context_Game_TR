using UnityEngine;
using  System;
using  System.Collections;
using System.Collections.Generic;

public interface ISceneMgr : IRunUpdate
{
	void Init ();
	void Shutdown();
}

public class SceneLogic : SingleTon<SceneLogic>,IRunUpdate,ICmdHandler
{
	public static bool isIn3Dscene = false;
	SceneGameUIMgr    m_LogicUI;
	SceneFishMgr      m_FishMgr;
	SceneBulletMgr    m_BulletMgr;
	SceneEffectMgr    m_EffectMgr;
	ScenePlayerMgr    m_PlayerMgr;
	SceneSkillMgr     m_SkillMgr;
	FishGameMode 	  m_fishModel;
	SceneCollisionMgr m_CollisonMgr;
    SceneHeroMgr      m_HeroMgr;
	SceneItemDropMgr  m_ItemDropMgr;
	SceneFishBubbleMgr m_bubbleMgr;
	SceneWorldBossMgr  mWorldBossMgr;

	ISceneMgr[] sceneMgrlist = new ISceneMgr[0];

	EnumRoomType 	  mRoomType = EnumRoomType.Normal_TimeLimit;
	uint              m_RoomCfgId;
    bool              m_bClearScene;
    float             m_fClearTime = 0;
    bool              m_bRefreshScene = false;
	bool 			  m_isInitCompleted = false;
	bool mIsLOOKGuster = false;
    public bool mIsXiuYuQi;//当前是否处于休渔期
	short mSceneMgrLength;
	bool isInited = false;
	public static bool iShowOnekey = true;
	public bool Init(JoinRoomInfo roominfo)
    {
		if (isInited)
			return false;
		m_isInitCompleted = false;
		mIsGameOver = false;
        MainEntrace.Instance.StartCoroutine(MainInitProcedure(roominfo));
        isInited = true;
        SceneSeaEffect.instance.StartPalyAnim();
		return true;
    }

	public bool IsInitCompleted{ set { m_isInitCompleted = value;} get { return m_isInitCompleted;}}

	public bool IsLOOKGuster {
		get { return mIsLOOKGuster;}
	}


	IEnumerator MainInitProcedure(JoinRoomInfo obj)
    {
        //异步加载
		m_RoomCfgId = obj.RoomID;
		mRoomType = obj.roomType;
		mIsLOOKGuster = obj.IsLookOn;
		FishNetAPI.Instance.Enabled = !mIsLOOKGuster;

		m_LogicUI = new SceneGameUIMgr();
		m_FishMgr = new SceneFishMgr();
		m_BulletMgr = new SceneBulletMgr();
		m_PlayerMgr = new ScenePlayerMgr();
		m_SkillMgr = new SceneSkillMgr();
		m_EffectMgr = new SceneEffectMgr();
		m_fishModel = new FishGameMode();
		m_CollisonMgr = new SceneCollisionMgr ();
		m_HeroMgr = new SceneHeroMgr();
		m_ItemDropMgr = new SceneItemDropMgr ();
		m_bubbleMgr = new SceneFishBubbleMgr ();
		mWorldBossMgr = new SceneWorldBossMgr ();

		sceneMgrlist = new ISceneMgr[] {
			m_fishModel,
			m_FishMgr,
			m_BulletMgr,
			m_PlayerMgr,
			m_SkillMgr,
			m_EffectMgr,
			m_CollisonMgr,
			m_LogicUI,
			m_HeroMgr,
			m_ItemDropMgr,
			m_bubbleMgr,
			mWorldBossMgr,
			new SceneObjSortMgr ()
		};
		mSceneMgrLength = (short)sceneMgrlist.Length;
		Array.ForEach (sceneMgrlist, x => x.Init ());
		ResetScene(obj, true);
		GlobalUpdate.RegisterUpdate (this);
		m_isInitCompleted = true;
		Notifiy (SysEventType.FishRoomReady);
		foreach (var joinInfo in obj.playerJoinArray) {
			SceneLogic.Instance.OnPlayerJoinTable (joinInfo);
        }
        BulletMgr.InitBuffer(obj.buffCacheList);
		isIn3Dscene = true;
		FishNetAPI.Instance.RegisterHandler (SceneLogic.Instance);

        m_LogicUI.ShowXiuYuQi(obj.mXiuYuQiEndTime);//显示休渔期动画特效
        m_LogicUI.RefershItemListPos();

        TimeManager.DelayExec(1, () => {
            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/Ef_table_1", (ab_data, game_obj) => {
                GameObject obj_table = GameUtils.CreateGo(game_obj, SceneObjMgr.Instance.UIPanelTransform);
                obj_table.AddComponent<ResCount>().ab_info = ab_data;
                Animator anim = obj_table.GetComponent<Animator>();
                GameObject.Destroy(obj_table, anim.GetCurrentAnimatorStateInfo(0).length);
                UILabel label = obj_table.GetComponentInChildren<UILabel>();
                label.text = (obj.TableID + 1).ToString("00");
            }, GameEnum.Fish_3D);
        });
		GuideMgr.Instance.GlobalInit ();
        OtherManager.StartGold = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.PlayerMgr.MyClientSeat);
        OtherManager.StartTime = Time.realtimeSinceStartup;

        MtaManager.AddGameEnter(OtherManager.StartGold);
        MtaManager.BeginGameInfo();
		yield return null;
    }

    public void ResetScene(JoinRoomInfo jrd, bool bFirst = false)
    {
        ResetPlayerData(jrd, bFirst);
        if(bFirst == false)
            RefreshScene();
    }

    public void Shutdown() {
        RoleItemModel.Instance.Clear();
		GlobalUpdate.UnRegisterUpdate (this);
		if (delayCorne != null)
			MonoDelegate.Instance.StopCoroutine(delayCorne);
		delayCorne = null;
        WndManager.Instance.CloseAllUI();
		Array.ForEach (sceneMgrlist, x => x.Shutdown ());
		if (m_Background != null) {
			GameObject.Destroy (m_Background.gameObject);
			m_Background = null;
		}
		if (oldBground != null && oldBground.gameObject != null) {
			GameObject.Destroy (oldBground.gameObject);
			oldBground = null;
		}

		isInited = false;
        GlobalEffectMgr.Instance.Clear();
		FishNetAPI.Instance.UnRegisterHandler (this);
		m_isInitCompleted = false;
		
		isIn3Dscene = false;
		mIsGameOver = true;

        TimeManager.ClearAction();
        AutoDestroy[] ass = GameObject.FindObjectsOfType<AutoDestroy>();
		GuideMgr.Instance.Shutdown ();
        for (int i = 0; i < ass.Length; i++) {
            GameObject.Destroy(ass[i].gameObject);
        }
        EffManager.ClearAll();
        MtaManager.EndGameInfo();
    }

	public bool bClearScene
    {
        get { return m_bClearScene; }
    }

	public SceneEffectMgr 	EffectMgr	{ get { return m_EffectMgr; }}
	public SceneBulletMgr 	BulletMgr	{ get { return m_BulletMgr; }}
	public FishGameMode 	FModel	{	get { return m_fishModel; }}
	public SceneFishMgr 	FishMgr {   get { return m_FishMgr; }}
	public ScenePlayerMgr 	PlayerMgr { get { return m_PlayerMgr; }}
	public SceneGameUIMgr 	LogicUI { get { return m_LogicUI; }}
	public SceneSkillMgr 	SkillMgr {get { return m_SkillMgr; } }
    public SceneHeroMgr 	HeroMgr  {get { return m_HeroMgr; }}
	public SceneCollisionMgr CollisonMgr {get { return m_CollisonMgr; }}
	public SceneItemDropMgr ItemDropMgr { get { return m_ItemDropMgr;}}
	public SceneWorldBossMgr WorldBossMgr {get { return mWorldBossMgr;}}

	public virtual uint GetRoomCfgID()
	{
		return m_RoomCfgId;
	}
    private TimeRoomVo vo_room;
    public TimeRoomVo RoomVo {//获取当前房间配置
        get {
            if (vo_room == null || vo_room.CfgID != m_RoomCfgId) {
                vo_room = FishConfig.Instance.TimeRoomConf.TryGet(m_RoomCfgId);
            }
            return vo_room;
        }
    }

	public EnumRoomType RoomType{ get { return mRoomType;}}

    public void SetClearScene()
    {
        m_LogicUI.ShowClearSceneMsg();
        m_bClearScene = true;
        m_fClearTime = 0;
    }

    public float exit_time = 180;//退出游戏倒计时
    private int pre_exit_time = -1;//
    private UI_NotOperateTickRef ui_not_operater_tick;//未操作提示
    private bool __is_load_ab;
    public void ResetExitTime() {//重置退出时间
        exit_time = 180;
    }
    public void Update(float delta)
    {
        if(m_bClearScene)
        {
            m_fClearTime += delta;
            if (m_fClearTime >= ConstValue.CLEAR_TIME)
                m_bClearScene = false;
        }

        //初始化已经完成了
		if (m_isInitCompleted == false)
            return;

        for (short idx = 0; idx < mSceneMgrLength; idx++) {
            sceneMgrlist[idx].Update(delta);
		}
        
#if UNITY_EDITOR
        return;//编辑模式判断超时不操作逻辑
#endif
        //一定时间不操作则踢出游戏
        if (this.IsLOOKGuster == false && GuideMgr.Instance.isGuiding == false) {
            exit_time -= delta;
            if (exit_time < 0) {
                BackToHall();
                if (ui_not_operater_tick != null) {
                    __is_load_ab = false;
                    GameObject.Destroy(ui_not_operater_tick.gameObject);
                    ui_not_operater_tick = null;
                }
            }else if (exit_time < 11) {
                if (ui_not_operater_tick == null) {
                    if (__is_load_ab == false) {
                        __is_load_ab = true;
                        ResManager.LoadAsyn<GameObject>(ResPath.NewUIPath + "UI_NotOperateTick", (ab_data, obj) => {
                            if (__is_load_ab) {
                                __is_load_ab = false;
                                obj = GameUtils.CreateGo(obj, SceneObjMgr.Instance.UIPanelTransform);
                                obj.AddComponent<ResCount>().ab_info = ab_data;
                                ui_not_operater_tick = obj.GetComponent<UI_NotOperateTickRef>();
                                pre_exit_time = (int)exit_time;
                                ui_not_operater_tick.mLbTime.text = pre_exit_time.ToString("00");
                            }
                        }, GameEnum.Fish_3D);
                    }
                }else if (pre_exit_time != (int)exit_time) {
                    pre_exit_time = (int)exit_time;
                    ui_not_operater_tick.mLbTime.text = pre_exit_time.ToString("00");
                }
            } else if (ui_not_operater_tick != null) {
                __is_load_ab = false;
                GameObject.Destroy(ui_not_operater_tick.gameObject);
                ui_not_operater_tick = null;
            }
        }
    }

    public void LateUpdate(float delta)
    {
		if (m_isInitCompleted == false)
            return;
        //FioshMgr,BulletMgr,必须在网络命令之后进行更新。
        
    }
    //命令处理
    //======================================================================
    private List<uint> boss_bmg = null;
    private int pre_boss_bgm_index = -1;
    private ushort pre_boss_id = 0;
    public void PlayBossBGM(ushort boss_id) {
        if (boss_bmg == null) {
            boss_bmg = new List<uint>();
            boss_bmg.Add(FishConfig.Instance.AudioConf.BossBgm);
            boss_bmg.Add(FishConfig.Instance.AudioConf.BossBgm2);
            boss_bmg.Add(FishConfig.Instance.AudioConf.BossBgm3);
            boss_bmg.Add(FishConfig.Instance.AudioConf.BossBgm4);
            boss_bmg.Add(FishConfig.Instance.AudioConf.BossBgm5);
        }
        if (pre_boss_id != boss_id || pre_boss_bgm_index < 0) {
            pre_boss_id = boss_id;
            int index = Utility.Range(0, boss_bmg.Count - 1);
            if (index >= pre_boss_bgm_index) {
                pre_boss_bgm_index = index + 1;
            } else {
                pre_boss_bgm_index = index;
            }
        }
        TimeManager.DelayExec(1, () => {
            GlobalAudioMgr.Instance.PlayBGMusic(boss_bmg[pre_boss_bgm_index]);
        });
    }
    private List<uint> bgm_list = null;
    private int pre_bgm_index = -1;
//    private uint pre_bg_index = uint.MaxValue;
    public void PlayBGM(uint bg_index,bool is_reset) {//播放背景音乐
        if (bgm_list == null) {
            bgm_list = new List<uint>();
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm1);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm2);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm3);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm4);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm5);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm6);
            bgm_list.Add(FishConfig.Instance.AudioConf.GameBgm7);
        }
        if (is_reset || pre_bgm_index < 0) {
            int index = Utility.Range(0, bgm_list.Count-1);
            if (index >= pre_bgm_index) {
                pre_bgm_index = index + 1;
            } else {
                pre_bgm_index = index;
            }
        }
        TimeManager.DelayExec(1, () => {
            GlobalAudioMgr.Instance.PlayBGMusic(bgm_list[pre_bgm_index]);
        });
    }
	IEnumerator delayCorne = null;
	MapParallax m_Background = null, oldBground = null;

	void LoadGameBG(uint pBackImageIdx)
	{
		m_BackImageIdx = pBackImageIdx;
        this.PlayBGM(m_BackImageIdx,true);
		bool isFade = true;

        GameObject bgObj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_3D,ResPath.NewSceneBackground + m_BackImageIdx);
        bgObj = GameUtils.ResumeShader(bgObj);
        if (IsGameOver) return;

        GameObject bgGo = null;
        if (m_Background == null)
        {
            bgGo = GameObject.Instantiate(bgObj);
            m_Background = bgGo.GetComponent<MapParallax>();
            return;
        }
        else if (!isFade)
        {
            bgGo = GameObject.Instantiate(bgObj);
            GameObject.Destroy(m_Background.gameObject);
            m_Background = bgGo.GetComponent<MapParallax>();
        }
        else
        {
            FishMgr.BackupFishList();

            var bolang = GameObject.Instantiate(FishResManager.Instance.BoLangObj);
            AutoDestroy.Begin(bolang, 4f);
            float time = Time.realtimeSinceStartup + 1;
            delayCorne = MonoDelegate.Instance.Coroutine_Delay(1f, delegate
            {
                delayCorne = null;
                GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.WaterWave);
                bgGo = GameObject.Instantiate(bgObj);
                oldBground = m_Background;
                m_Background = bgGo.GetComponent<MapParallax>();
                MapParallax.BgFade(oldBground, m_Background, 15f, Time.realtimeSinceStartup - time);
                FishMgr.ClearScene();
            });
        }


  //      Kubility.KAssetBundleManger.Instance.LoadGameObject(ResPath.SceneBackground+m_BackImageIdx, delegate(SmallAbStruct obj) 
  //      {
		//	GameObject bgObj = obj.MainObject as GameObject;
		//	bgObj = GameUtils.ResumeShader(bgObj);
		//	if (IsGameOver) return;

		//	GameObject bgGo = null;
		//	if (m_Background == null)
		//	{
		//		bgGo = GameObject.Instantiate(bgObj);
		//		m_Background = bgGo.GetComponent<MapParallax>();
		//		return;
		//	}
		//	else if (!isFade)
		//	{
		//		bgGo = GameObject.Instantiate(bgObj);
		//		GameObject.Destroy(m_Background.gameObject);
		//		m_Background = bgGo.GetComponent<MapParallax>();
		//	}
		//	else
		//	{
		//		FishMgr.BackupFishList();

  //              var bolang = GameObject.Instantiate(FishResManager.Instance.BoLangObj);
  //              AutoDestroy.Begin(bolang, 4f);
  //              float time = Time.realtimeSinceStartup + 1;
		//		delayCorne = MonoDelegate.Instance.Coroutine_Delay(1f, delegate 
  //              {
		//			delayCorne = null;
		//			GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.WaterWave);
		//			bgGo = GameObject.Instantiate(bgObj);
		//			oldBground = m_Background;
		//			m_Background = bgGo.GetComponent<MapParallax>();
  //                  MapParallax.BgFade(oldBground, m_Background, 15f, Time.realtimeSinceStartup - time);
		//			FishMgr.ClearScene();
		//		});
		//	}
		//});	

        //切换场景时，隐藏BOSS血条
        SceneLogic.Instance.LogicUI.BossLifeUI.Hide();
	}

	uint m_BackImageIdx = 0;
	public uint BackgroundIdx
	{
		get { return m_BackImageIdx;}
	}

	public void SwitchGameBackground(uint pBackImageIdx)
	{
		LoadGameBG (pBackImageIdx);
	}

	public void ResetPlayerData(JoinRoomInfo jrd, bool bFirst)
    {

        byte serverSeat = jrd.Seat;
        FModel.Inversion = serverSeat > 1;
        m_PlayerMgr.MyClientSeat = FModel.ServerToClientSeat(serverSeat);
        m_PlayerMgr.ClearAllPlayer();
        uint serverLauncherType = jrd.ServerLauncherTypeID;
		LoadGameBG (jrd.BackgroundImage);
		BulletMgr.bulletID = jrd.LastBulletID;
		HeroMgr.InitHeroData (jrd.HeroData);

		FModel.OnPlayerJoin(jrd);
		FModel.BackgroundIndex = jrd.BackgroundImage;
        //加入自己
        bool launcherValid;
		uint clientLauncherType, launcherLevel;

		FModel.CheckLauncherValid(
            serverLauncherType,
            out clientLauncherType,
			out launcherLevel,
            out launcherValid);
        //获取自己的消息

		FModel.SelfServerSeat = serverSeat;
		TablePlayerInfo pPlayer = FModel.GetTabPlayerByCSeat (m_PlayerMgr.MyClientSeat);
        m_PlayerMgr.PlayerJoin(pPlayer,
            m_PlayerMgr.MyClientSeat,
            jrd.RateValue,
            clientLauncherType,
			launcherLevel,
            launcherValid);
        //BulletMgr.InitBuffer (jrd.buffCacheList);		
    }

    public void RefreshScene(bool bSendResetCmd = false)
    {
        m_PlayerMgr.ClearPlayer();
        m_FishMgr.ClearAllFish();
        m_BulletMgr.ClearAllBullet();
        m_EffectMgr.Clear();
        m_bRefreshScene = true;
        if (bSendResetCmd)
			FishNetAPI.Instance.ResetSceneInfo();
    }

	bool mIsGameOver = true;
	public bool IsGameOver
	{
		get { return mIsGameOver;}
	}
	public bool IsInited
	{
		get { return isInited;}
	}

    //返回大厅
    public void BackToHall() {
        if (RoleInfoModel.Instance.CoinMode != EnumCoinMode.Score && RoleInfoModel.Instance.GameMode != EnumGameMode.Mode_PK) {
            TablePlayerInfo player = SceneLogic.Instance.FModel.GetPlayerByUserID(RoleInfoModel.Instance.Self.UserID);
            if (player != null) {
                HallHandle.AsynScore(player.GlobeNum + player.BaseGlobeNum, -1);
            }
        }
        if (this.m_LogicUI != null && this.m_LogicUI.mbattleUIRef != null) {
            this.m_LogicUI.mbattleUIRef.gameObject.SetActive(false);
        }

        mIsGameOver = true;
		FishNetAPI.Instance.LeaveTable (true);
		FishNetAPI.Instance.UnRegisterHandler (SceneLogic.Instance);
		//FishNetAPI.Instance.Disconnect ();
        Shutdown();

        SceneObjMgr.Instance.ResetTopCameraParam();

        GameSceneManager.BackToHall(GameEnum.Fish_3D);
    }

	//========================================== 
	public bool CanProcessCmd()
	{
		return m_isInitCompleted;
	}


    //处理网络命令
    public bool Handle(NetCmdPack pack)
	{
		if (mIsGameOver)
			return false;
		if (m_bRefreshScene) {
			//刷新场景中，过期的命令不处理
            if ((int)pack.cmdTypeId >> 16 == (int)TcpMainCMD.CMD_MAIN_GAME)
				return true;
		}

		byte clientSeat;
		uint skillCfgId;
		NetCmdType cmdType = (NetCmdType)pack.cmdTypeId;
		switch (cmdType) {

		case NetCmdType.SUB_S_CLOCK:
			{
			}
			break;

		case NetCmdType.SUB_S_FISH: // 出鱼
            m_FishMgr.LaunchFish(pack.ToObj < SC_GR_Fish>(),pack.tickSpan);
			break;
		
		case NetCmdType.SUB_S_FISH2:
			{
                m_FishMgr.LaunchFish2(pack.ToObj<SC_GR_Fish2>());
			}
			break;
        case NetCmdType.SUB_S_CIRCLEFISH://特殊路径出鱼
            m_FishMgr.LaunchFish(pack.ToObj<SC_GR_CircleFish>(), pack.tickSpan);
            break;

		case NetCmdType.SUB_S_BEFOREMAPEND:
			{
				m_FishMgr.BeforeMapEnd ();
			}
			break;

		case NetCmdType.SUB_S_OPEN_PARADE: 
			{
                m_FishMgr.StartOpeningParade(pack.ToObj<SC_GR_OpeningParade>());
			}
			break;
		case NetCmdType.SUB_S_OPEN_PARADE_FISH: 
			m_FishMgr.StartOpeningParadeFish (pack.ToObj<SC_GR_OpeningParadeFish>());
			break;
//		case NetCmdType.SUB_S_SYNC_FISH: // 出鱼
//			m_FishMgr.LaunchFishByAsycScene(pack);
//              break;

//		case NetCmdType.SUB_S_SYNC_BULLET:
//			m_PlayerMgr.LaunchSyncBullet (pack);
//			break;

		case NetCmdType.SUB_S_ONBULLET:
            m_PlayerMgr.OnPlayerLaunchBullet(pack.ToObj<SC_GR_Bullet>());
			break;


		case NetCmdType.SUB_S_SET_LAUNCHER:
            m_PlayerMgr.OnChangeLauncher(pack.ToObj<SC_GR_SetLauncher>());
			break;
	
		case NetCmdType.SUB_S_SET_LR_MULTI:
            m_PlayerMgr.OnChangeRateValue(pack.ToObj<SC_GR_SetLrMulti>());
			break;

		case NetCmdType.SUB_S_BULLET_CATCH:
            m_PlayerMgr.OnEngeryUpdate(pack.ToObj<SC_GR_BulletCatch>());
            m_SkillMgr.FishCatched(pack.ToObj<SC_GR_BulletCatch>(), pack.tickSpan);
			break;

        case NetCmdType.SUB_S_LAUNCHER_SKILL: {
                SC_GR_LauncherSkill cmd = pack.ToObj<SC_GR_LauncherSkill>();
                clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);
                if (SceneLogic.Instance.mIsXiuYuQi == false || clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {//休渔期屏蔽其他玩家技能动画
                    skillCfgId = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat).Launcher.Vo.SkillID;
                    m_PlayerMgr.OnPlayerXPSkill(pack);
                    m_SkillMgr.OnSkillCaster(cmd.ChairID, cmd.Handler, cmd.CacheID, skillCfgId, 0, new Vector3(cmd.xPos, cmd.yPos, 0f), 0xFFFF, 0xFF, cmd.Angle);

                    if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                        MtaManager.AddLauncherSkillEvent(skillCfgId);
                    }
                }
            }
            break;

		case NetCmdType.SUB_S_LR_SKILL_CATCH:
			{
                SC_GR_LrSkillCatch cmd = pack.ToObj<SC_GR_LrSkillCatch>();
				clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);
				skillCfgId = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat).Launcher.Vo.SkillID;
				m_SkillMgr.SkillFishCatched (clientSeat, skillCfgId, cmd.EffectUnite, cmd.ArrCollID, cmd.ArrCatchID, cmd.ArrGold, cmd.ArrAwardID);
			}
			break;

		case NetCmdType.SUB_S_ITEM_SKILL:
			{
                SC_GR_ItemSkill cmd = pack.ToObj<SC_GR_ItemSkill>();
				clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);
				skillCfgId = FishConfig.GetSkillCfgIDByItem (cmd.ItemCfgID);
				if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
					if (cmd.ErrorCode == 0) {
						m_LogicUI.OnUsedItemSucc (cmd.ItemCfgID);
					} else {
						m_LogicUI.OnUserdItemFail (cmd.ItemCfgID, cmd.ErrorCode);
						return true;
					}
				}
                if (SceneLogic.Instance.mIsXiuYuQi == false || clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {//休渔期屏蔽其他玩家技能动画
					m_SkillMgr.OnSkillCaster(cmd.ChairID, cmd.Handler, cmd.CacheID, skillCfgId, cmd.ItemCfgID, new Vector3(cmd.xPos, cmd.yPos), cmd.FishID, cmd.FishPartID, cmd.Angle);
                }
			}
			break;

		case NetCmdType.SUB_S_ITEM_SKILL_CATCH:
			{
				SC_GR_ItemSkillCatch cmd = pack.ToObj<SC_GR_ItemSkillCatch>();
				clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);
				skillCfgId = FishConfig.GetSkillCfgIDByItem (cmd.ItemCfgID);
				m_SkillMgr.SkillFishCatched (clientSeat, skillCfgId, cmd.EffectUnite, cmd.ArrCollID, cmd.ArrCatchID, cmd.ArrGold, cmd.ArrAwardID);
			}
			break;

		case NetCmdType.SUB_S_HERO_CALL:
			{
                m_HeroMgr.OnHeroCallSucc(pack.ToObj<SC_GR_HeroCall>());
			}
			break;
		case NetCmdType.SUB_S_HERO_SYNC:
			{
                m_HeroMgr.OnHeroSync(pack.ToObj<SC_GR_HeroSync>());
			}
			break;
		case NetCmdType.SUB_S_HERO_CATCH:
			{
                m_HeroMgr.OnSyncHeroCatch(pack.ToObj<SC_GR_HeroCatch>(), pack.tickSpan);
			}
			break;
		case NetCmdType.SUB_S_HERO_BULLET:
			{
                m_HeroMgr.OnHeroLaunchBullet(pack.ToObj<SC_GR_HeroBullet>(), pack.tickSpan);
			}
			break;
		case NetCmdType.SUB_S_HERO_LEAVE:
			{
                m_HeroMgr.OnHeroLeave(pack.ToObj<SC_GR_HeroLeave>());
			}
			break;

		case NetCmdType.SUB_S_LAUNCHBOSS:
			{
                m_FishMgr.LaunchBoss(pack.ToObj<SC_GR_LaunchBoss>());
			}
			break;
		case NetCmdType.SUB_S_BOSSLEAVE:
			{
                m_FishMgr.BossLeave(pack.ToObj<SC_GR_BossLeave>());
			}
			break;

		case NetCmdType.SUB_S_SYNCBOSS:
			{
				SC_GR_SyncBoss bossBlood = pack.ToObj<SC_GR_SyncBoss>();
				FishMgr.SetBossBloodIndex (bossBlood);
				if ((bossBlood.BossState & (uint)0x01) == 1)
					SkillMgr.BossDizzy (bossBlood.BossID);
			}
			break;

		case NetCmdType.SUB_S_SPECFISH:
            SkillMgr.SpecailCaster(pack.ToObj<SC_GR_SpecFish>());
			break;

		case NetCmdType.SUB_S_SPECFISHCATCH:
            SkillMgr.SpecailCatch(pack.ToObj<SC_GR_SpecFishCatch>());
			break;
		case NetCmdType.SUB_S_BUFFSYNC:
            BulletMgr.UpdateBuffer(pack.ToObj<SC_GR_BuffSync>());
			break;
		case NetCmdType.SUB_S_COMBO_SYNC:
            PlayerMgr.SyncCombo(pack.ToObj<SC_GR_ComboSync>());
			break;
		case NetCmdType.SUB_S_QUICKSELL:
            PlayerMgr.QuickSell(pack.ToObj<SC_GR_QuickSell>());
			break;
		case NetCmdType.SUB_S_ONBRANCHBULLET:
            PlayerMgr.OnPlayerLaunchBranchBullet(pack.ToObj<SC_GR_BranchBullet>());
			break;
		case NetCmdType.SUB_S_WORLDBOSS_COMING:
            mWorldBossMgr.OnWorldBossTimeCountDown(pack.ToObj<SC_GR_WorldBossComing>());
			break;
        case NetCmdType.SUB_S_WORLDBOSS_JACKPOT:
            mWorldBossMgr.OnWorldBossJackpot(pack.ToObj<SC_GR_WorldBossJackpot>());
			break;
        case NetCmdType.SUB_S_WORLDBOSS_SYNC:
            mWorldBossMgr.OnSyncWorldBossInfo(pack.ToObj<SC_GR_WorldBossSync>());
			break;
        case NetCmdType.SUB_S_WORLDBOSS_CATCH:
            mWorldBossMgr.OnWorldBossCatch(pack.ToObj<SC_GR_WorldBossCatch>());
			break;
		case NetCmdType.SUB_S_WORLDBOSS_RANK:
            PlayerMgr.ShowWorldBossRank(pack.ToObj<SC_GR_WorldBossRank>());
			break;
        case NetCmdType.SUB_S_WORLDBOSS_GRANT:
            PlayerMgr.WorldBossGrant(pack.ToObj<SC_GR_WorldBossGrant>());
            break;
        case NetCmdType.SUB_S_WORLDBOSS_COSTRANK:
            mWorldBossMgr.OnWorldBossCostRank(pack.ToObj<SC_GR_WorldBossCostRank>());
            break;
		case NetCmdType.SUB_S_PBOX_MULTI_CHANGE:
            ScenePirateBoxMgr.OnPirateRateChange(pack.ToObj<SC_GR_PBoxMultiChange>());
			break;
		case NetCmdType.SUB_S_OPEN_PBOX:
            ScenePirateBoxMgr.OnOpenPirateBox(pack.ToObj<SC_GR_OpenPBox>());
			break;

        case NetCmdType.SUB_S_PBOX_CATCH:
            ScenePirateBoxMgr.OnPirateBoxCatch(pack.ToObj<SC_GR_PBoxCatch>());
			break;
        case NetCmdType.SUB_S_LOTTERY_TICKET://奖卷累计信息
            if (Item_Battle_Lottery.item_lottery != null) {
                Item_Battle_Lottery.item_lottery.OnLotteryTickUpdate(pack.ToObj<SC_GR_LotteryTicket>());
            }
			break;
        case NetCmdType.SUB_S_INCOME_SOURCE://收入来源
            SC_GR_IncomeSrc income_src = pack.ToObj<SC_GR_IncomeSrc>();
//#if UNITY_EDITOR
//            Debug.LogError("SC_GR_IncomeSrc : " + LitJson.JsonMapper.ToJson(income_src));
//#endif
            if (income_src.SrcCfgID == 10402) {//闪电鱼延迟显示
                TimeManager.DelayExec(2, () => {
                    AnimGoldCount.ShowAwardCount(income_src);
                });
            } else if (income_src.SrcCfgID == 13004) {//足球收益延迟显示
                EffectMove em;
                if (EffectMove.dic_pre_eff.TryGetValue(FModel.ServerToClientSeat((byte)income_src.ChairID), out em)) {
                    if (em.resp_income_src != null) {
                        em.resp_income_src.InCome += income_src.InCome;
                    } else {
                        em.resp_income_src = income_src;
                    }
                }else{
                    LogMgr.LogError("找不到对应的足球鱼");
                    TimeManager.DelayExec(3, () => {
                        AnimGoldCount.ShowAwardCount(income_src);
                    });
                }
            } else {
                AnimGoldCount.ShowAwardCount(income_src);
            }
            break;
        case NetCmdType.SUB_S_CATCHPANDORA://潘多拉被捕获
            ScenePandoraMgr.OnCatchPandora(pack.ToObj<SC_GR_CatchPandora>());
            break;
        //case  NetCmdType.SUB_S_SKILLREFRESH://重置技能，客户端重新触发
        //    SC_GR_SkillRefresh gr_skill_refresh = pack.ToObj<SC_GR_SkillRefresh>();
        //    if (gr_skill_refresh.ChairID == RoleInfoModel.Instance.Self.ChairSeat || gr_skill_refresh.Handler == RoleInfoModel.Instance.Self.ChairSeat) {//需要自己处理的
        //        byte client_seat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)gr_skill_refresh.ChairID);
        //        //float _time;
        //        //if (dic_pre_skill.TryGetValue(client_seat, out _time) == false || _time + 0.5f < Time.realtimeSinceStartup) {
        //            SceneLogic.Instance.PlayerMgr.GetPlayer(client_seat).Launcher.ResetSkillAndUse();
        //            //dic_pre_skill[client_seat] = Time.realtimeSinceStartup;
        //        //}
        //    }
        //    break;
        case NetCmdType.SUB_GR_USER_FIRSTCHARGEAWARD_RESULT://首充奖励验证结果
            var sss = pack.ToObj<CMD_GR_S_FirstChargeAward_Ret>();
            if (sss.ResultInfo.ErrorCode == 0) {
                CMD_GP_FirstChargeAward award = ShopManager.msg_cs_award;
                if (award != null) {
                    for (int i = 0; i < award.vecItemID.Length; i++) {
                        RoleItemModel.Instance.UpdateItem(award.vecItemID[i], (int)award.vecItemNum[i]);
                    }
                }

                SceneLogic.Instance.ShowShouChongAward();
            } else {
                LogMgr.LogError(sss.ResultInfo.ErrorString);
            }
            break;
        case NetCmdType.SUB_S_SUBITEMCD://道具CD更新
            var sub_item = pack.ToObj<SC_GR_SubItemCD>();
            SceneLogic.Instance.LogicUI.ItemListUI.SetItemCD(sub_item.ItemCfgID, sub_item.ItemCD);
            break;
        case NetCmdType.SUB_GR_NEEDSHARE://剩余救济金领取次数  次数大于0  则需要进行分享
            {
                CMD_GR_S_NeedShare cmd = pack.ToObj<CMD_GR_S_NeedShare>();
                //Debug.LogError("cmd.SafeTime:" + cmd.SafeTime);
                if (cmd.SafeTime > 0 && UserManager.IsBingWX) {
                    //UI.EnterUI<UI_Share>(ui => ui.InitData(true));
                    UI.EnterUI<UI_Share>(GameEnum.All).InitData(true);
                        //} else {//没有救济金领取的时候  显示乐豆不足弹框
                        //    UI.EnterUI<UI_poor>(ui => {
                        //        ui.InitData(1);//乐豆不足
                        //    });
                    }
            }
            break;
        case NetCmdType.SUB_GP_USER_INSURE_INFO://保险箱资料
            CMD_GP_UserInsureInfo gp_bank = pack.ToObj<CMD_GP_UserInsureInfo>();
            HallHandle.UserGold = gp_bank.UserScore;
            HallHandle.UserInsure = gp_bank.UserInsure;
            EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
            break;
        case NetCmdType.SUB_GR_USER_INSURE_INFO://保险箱资料
            CMD_GR_S_UserInsureInfo gr_bank = pack.ToObj<CMD_GR_S_UserInsureInfo>();
            HallHandle.UserGold = gr_bank.UserScore;
            HallHandle.UserInsure = gr_bank.UserInsure;
            EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
            break;
        case NetCmdType.SUB_GP_USER_INSURE_FAILURE://取钱失败
                {
                CMD_GP_UserInsureFailure cmd = pack.ToObj<CMD_GP_UserInsureFailure>();
                //WndManager.Instance.ShowDialog(cmd.DescribeString, null);
                SystemMessageMgr.Instance.ShowMessageBox(cmd.DescribeString, 1);
                break;
            }
        case NetCmdType.SUB_GR_USER_INSURE_FAILURE://取钱失败
                {
                    CMD_GR_S_UserInsureFailure cmd = pack.ToObj<CMD_GR_S_UserInsureFailure>();
                    SystemMessageMgr.Instance.ShowMessageBox(cmd.DescribeString, 1);
                    break;
                }
		default:
            return NetEventManager.Notifiy(cmdType, pack);
        }
        NetEventManager.Notifiy(cmdType, pack);
		return true;
	}

	public void StateChanged(NetState state)
	{
		if (state == NetState.NET_DISCONNECT || state == NetState.NET_ERROR)
		{
			FishNetAPI.Instance.ReconnectServer ();
			//SystemMessageMgr.Instance.DialogShow("Network Disconnect..");
		}
	}


	public void OnPlayerJoinTable(PlayerJoinTableInfo playerJoin)
	{
        bool launcherValid;
        uint launcherType, launcherlevel;
        FModel.CheckLauncherValid(playerJoin.ServerLauncherTypeID, out launcherType, out launcherlevel, out launcherValid);
        if (FModel.SelfServerSeat == playerJoin.Seat) {
            launcherValid = SceneLogic.Instance.FModel.IsLauncherValid(launcherType, launcherlevel);
            if (m_PlayerMgr.MySelf.Launcher.Vo.LrCfgID != launcherType || m_PlayerMgr.MySelf.Launcher.Vo.Level != launcherlevel) {
                m_PlayerMgr.MySelf.Launcher.ChangeLauncher(launcherType, launcherlevel, launcherValid);
            }
            m_PlayerMgr.MySelf.Launcher.ChangeRate(playerJoin.RateValue, launcherValid);
            m_PlayerMgr.MySelf.RateValue = playerJoin.RateValue;

            m_PlayerMgr.MySelf.Player.GlobeNum = playerJoin.ChangeGold;
            SceneLogic.Instance.FModel.Notify.Notifiy(FishingMsgType.Msg_GoldNumChange, m_PlayerMgr.MySelf.Player.UserID);

            GameConfig.SetLauncherInfo(this.RoomVo, launcherType, (byte)launcherlevel, playerJoin.RateValue);
        } else {
            TablePlayerInfo tablePlayer = new TablePlayerInfo(playerJoin);
            FModel.OnPlayerJoin(tablePlayer);
            byte clientSeat = FModel.ServerToClientSeat(playerJoin.Seat);
            m_PlayerMgr.PlayerJoin(tablePlayer, clientSeat, playerJoin.RateValue, launcherType, launcherlevel, launcherValid);
        }
	}

	public void OnPlayerLeaveTable(byte playerSeat)
	{
		byte clientSeat = FModel.ServerToClientSeat(playerSeat);
		FModel.OnPlayerLeave (playerSeat);
		m_PlayerMgr.PlayerLeave(clientSeat);
		m_HeroMgr.OnHeroLeave (clientSeat);
	}
    public void ShowShouChongAward() {//显示首充奖励
        CMD_GP_FirstChargeAward award = ShopManager.msg_cs_award;
        if (award != null) {
            List<uint> item_id_list = new List<uint>(award.vecItemID);
            List<uint> item_count_list = new List<uint>(award.vecItemNum);
            if (ShopManager.ShouChongGold > 0) {
                item_id_list.Insert(0, 2001u);
                item_count_list.Insert(0, ShopManager.ShouChongGold);
            }
            List<KeyValuePair<ItemsVo, uint>> item_list = new List<KeyValuePair<ItemsVo, uint>>();
            for (int i = 0; i < item_id_list.Count; i++) {
                var kv = new KeyValuePair<ItemsVo, uint>(FishConfig.Instance.Itemconf.TryGet(item_id_list[i]), item_count_list[i]);
                item_list.Add(kv);
            }
            UI_GetAwardController.ParamInfo pi = new UI_GetAwardController.ParamInfo {
                tipInfos = StringTable.GetString("Tip_34"),
                db_item_list = item_list,
            };
            WndManager.Instance.ShowUI(EnumUI.UI_GetAward, pi);
        } else {
            LogMgr.LogError("首充奖励为null");
        }
    }
}
