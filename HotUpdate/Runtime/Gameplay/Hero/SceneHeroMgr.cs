using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroSyncData 
{
    public uint       HeroCfgID;      //英雄(雇佣兵)CfgID
    public ushort       Speed;          //移动速度
	public ushort 		RotSpeed;		// 转向速度
	public Vector3 		startPos;
	public Vector3 		targetPos;
    public byte         Anim;           //动画状态
	public byte 		subclip;
	public ushort FishID;
	public uint TickCount;
}

public class SceneHeroMgr : ISceneMgr 
{
    //<clientSeat座位编号， <英雄cfgID, Hero对象> 保存其他玩家英雄对象
    Dictionary<byte, HeroClient>                                            m_PlayerHeroDictionary = new Dictionary<byte, HeroClient>();//每个玩家的英雄池，目前需求确定一个玩家只能同时召唤一个英雄
    //<clientSeat座位编号，英雄数据包列表>保存从网络接受的数据包，更新一个就从容器中扔掉，保持最新
    Dictionary<byte, List<HeroSyncData>>                                    m_PlayerHeroData = new Dictionary<byte,List<HeroSyncData>>();//保存后台同步过来的数据，在update中更新
    Hero                                                                    m_MyPlayerHero;             //本地玩家英雄

    public void Init() 
    {
		float xRange = FishConfig.Instance.GameSettingConf.HeroXRange;
		float yRange = FishConfig.Instance.GameSettingConf.HeroYRange;
		HeroUtilty.Init (xRange, yRange); //0.4f, 0.4f
        m_PlayerHeroDictionary.Clear();
        m_PlayerHeroData.Clear();
        m_MyPlayerHero = null;
    }

	public void InitHeroData(tagHeroCache[] heroCaches){
		byte myClientSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
		Vector3 pos, dir;
		for (int i = 0; i < heroCaches.Length; i++) {

			if (heroCaches [i].HeroCfgID == 0)
				continue;

            tagHeroCache tag = heroCaches[i];

            pos = new Vector3(tag.X, tag.Y, tag.Z);
            dir = new Vector3(tag.D1, tag.D2, tag.D3);
            HeroVo vo = FishConfig.Instance.HeroConf.TryGet(tag.HeroCfgID);
            //uint[] heroIDs = FishConfig.GetLoadResIDList(FishResType.HeroObjMap);
            //GameObjDictLoadItem heroloadDict = new GameObjDictLoadItem(FishResType.HeroObjMap, ResPath.HeroPath + "{0}", heroIDs, HeroObjMap);
            //loadDictList.Add(heroloadDict);
            ResManager.LoadAsyn<GameObject>(ResPath.NewHeroPath + vo.SourceID, (ab_data, prefab) => {
                GameObject obj = GameUtils.CreateGo(prefab);
                obj.transform.localScale = prefab.transform.localScale;
                obj.AddComponent<ResCount>().ab_info = ab_data;
                byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)i);
                if (clientSeat == myClientSeat && SceneLogic.Instance.IsLOOKGuster == false) {
                    m_MyPlayerHero = new Hero();
                    m_MyPlayerHero.Init(vo, myClientSeat, obj);
                    m_MyPlayerHero.InitHeroData(tag.AttRemain, tag.StartTick, tag.Anim, pos, dir);
                } else {
                    HeroClient heroClient = new HeroClient();
                    heroClient.Init(vo, clientSeat, obj);
                    m_PlayerHeroDictionary[clientSeat] = heroClient;
                    m_PlayerHeroData[clientSeat] = new List<HeroSyncData>();
                    heroClient.InitHeroData(tag.Anim, pos, dir);
                }
            }, GameEnum.Fish_3D);
		}
	}

	public Hero currentHero
	{
		get { return m_MyPlayerHero;}
	}
    public void SetOtherHeroShow(bool is_show) {//设置其他英雄显示或者隐藏
        var enumerator = m_PlayerHeroDictionary.Values.GetEnumerator();
        while (enumerator.MoveNext()) {
            enumerator.Current.HeroObj.IsVisible = is_show;
        }
    }

	List<ISortZObj> AllHeroTrans = new List<ISortZObj>();
	public List<ISortZObj> GetFishHeroObjs()
	{
		AllHeroTrans.Clear ();
		if (m_MyPlayerHero != null)
			AllHeroTrans.Add(m_MyPlayerHero.HeroObj);

        var enumerator = m_PlayerHeroDictionary.Values.GetEnumerator();
        while (enumerator.MoveNext()) {
            if (enumerator.Current != null && enumerator.Current.HeroObj != null)
                AllHeroTrans.Add(enumerator.Current.HeroObj);
        }
		return AllHeroTrans;		
	}

    public void Update(float delta)
    {
        if (null != m_MyPlayerHero)
        {
			if (!m_MyPlayerHero.Update (delta)) {
				m_MyPlayerHero.Dispose ();
				m_MyPlayerHero = null;
			}
        }

		byte[] seats = new byte[m_PlayerHeroDictionary.Count];
		m_PlayerHeroDictionary.Keys.CopyTo (seats, 0);

		foreach (var cSeat in seats) 
        {
            //存在玩家数据,更新该玩家数据包
			if (m_PlayerHeroData.ContainsKey(cSeat) && null != m_PlayerHeroDictionary[cSeat]) 
            {
				List<HeroSyncData> dataList = m_PlayerHeroData[cSeat];
                if (dataList.Count > 0) 
                {
                    //每更新一个包就从列表中移出
                    HeroSyncData data = dataList[0];
					m_PlayerHeroDictionary[cSeat].UpdateHeroState(data);
                    dataList.RemoveAt(0);
                }
            }

            //更新逻辑帧
			if (!m_PlayerHeroDictionary [cSeat].Update (delta)) {
				m_PlayerHeroDictionary [cSeat].Dispose ();
				m_PlayerHeroDictionary.Remove (cSeat);
				m_PlayerHeroData.Remove (cSeat);
			}
        }
    }

    public void RemoveHero(byte client_seat) {
        if (m_PlayerHeroDictionary.ContainsKey(client_seat)) {
            HeroClient clientHero = m_PlayerHeroDictionary.TryGet(client_seat);
            m_PlayerHeroDictionary.Remove(client_seat);
            m_PlayerHeroData.Remove(client_seat);
            clientHero.Dispose();
        }
    }
    public void Shutdown() 
	{
		if (m_MyPlayerHero != null) {
			m_MyPlayerHero.Dispose ();
			m_MyPlayerHero = null;
		}
		byte[] seats = new byte[m_PlayerHeroDictionary.Keys.Count];
		m_PlayerHeroDictionary.Keys.CopyTo (seats, 0);
		foreach (var clientSeat in seats) 
		{
			HeroClient clientHero = m_PlayerHeroDictionary.TryGet (clientSeat);
			m_PlayerHeroDictionary.Remove (clientSeat);
			m_PlayerHeroData.Remove (clientSeat);
			clientHero.Dispose ();
		}
		m_PlayerHeroDictionary.Clear ();
		m_PlayerHeroData.Clear ();
		AllHeroTrans.Clear ();
	}

    private bool is_request_call;//当前是否请求召唤英雄
    //客户端主动出英雄
	public bool LaunchHero(uint heroItemCfgID) {
        if (SceneLogic.Instance.IsLOOKGuster) {//旁观者模式屏蔽道具使用
            return false;
        }
        if (SceneLogic.Instance.mIsXiuYuQi) {//休渔期不能使用道具
            return false;
        }
        if (this.is_request_call) {
            return false;
        }

        if (RoleItemModel.Instance.getItemCount(heroItemCfgID) <= 0) {
            if (SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.PlayerMgr.MyClientSeat) < SceneLogic.Instance.RoomVo.CostScore) {
                //金币少于某些值，禁止购买
                string gold_str;
                if (SceneLogic.Instance.RoomVo.CostScore > 10000) {
                    gold_str = string.Format("{0}万", SceneLogic.Instance.RoomVo.CostScore / 10000);
                } else {
                    gold_str = SceneLogic.Instance.RoomVo.CostScore.ToString();
                }
                SystemMessageMgr.Instance.ShowMessageBox(string.Format(StringTable.GetString("Tip_42"), gold_str));
                return false;
            }
            ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(heroItemCfgID);
            if (vo.SaleNum > 0) {
                if (!WndManager.Instance.isActive(EnumUI.QuickBuyUI)) {
                    WndManager.Instance.ShowUI(EnumUI.QuickBuyUI, heroItemCfgID);
                }
            } else {
                //最大购买数量为0，不开放购买
                SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_1"));
            }
            return false;
        }

        if (m_MyPlayerHero == null)
        {
            //SceneLogic.Instance.LogicUI.HeroListUI.mListHideTime = Time.realtimeSinceStartup + 5;
            SceneLogic.Instance.LogicUI.HeroListUI.HideHeroList();
			FishNetAPI.Instance.LaunchHero (heroItemCfgID);
            is_request_call = true;
            return true;
        }
        return false;
    }

    public void OnHeroCallSucc(SC_GR_HeroCall cmd) {
        byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);
        byte myClientSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
        HeroVo vo = FishConfig.Instance.HeroConf.TryGet(cmd.HeroCfgID);

        ResManager.LoadAsyn<GameObject>(ResPath.NewHeroPath + vo.SourceID, (ab_data, prefab) => {
            GameObject obj = GameUtils.CreateGo(prefab);
            obj.transform.localScale = prefab.transform.localScale;
            obj.AddComponent<ResCount>().ab_info = ab_data;
            if (clientSeat == myClientSeat && SceneLogic.Instance.IsLOOKGuster == false) {
                is_request_call = false;
                if (m_MyPlayerHero != null)
                    m_MyPlayerHero.Dispose();
                m_MyPlayerHero = null;
                m_MyPlayerHero = new Hero();
                m_MyPlayerHero.Init(vo, myClientSeat, obj);
                m_MyPlayerHero.HeroObj.IsVisible = false;

                ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_HeroComein", (hc_ab_data, hc_prefab) => {
                    GameObject effGo = GameUtils.CreateGo(hc_prefab);
                    effGo.AddComponent<ResCount>().ab_info = ab_data;
                    effGo.transform.position = m_MyPlayerHero.HeroObj.Position;
                    effGo.transform.localScale = m_MyPlayerHero.HeroObj.Transform.localScale;
                    AutoDestroy.Begin(effGo, -1f, delegate() {
                        m_MyPlayerHero.HeroObj.IsVisible = true;
                    });
                }, GameEnum.Fish_3D);
            } else {
                HeroClient heroClient = new HeroClient();
                heroClient.Init(vo, clientSeat, obj);
                m_PlayerHeroDictionary[clientSeat] = heroClient;
                m_PlayerHeroData[clientSeat] = new List<HeroSyncData>();
                heroClient.HeroObj.IsVisible = false;

                if (SceneLogic.Instance.mIsXiuYuQi == false) {
                    ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_HeroComein", (hc_ab_data, hc_prefab) => {
                        GameObject effGo = GameUtils.CreateGo(hc_prefab);
                        effGo.AddComponent<ResCount>().ab_info = ab_data;
                        effGo.transform.position = heroClient.HeroObj.Position;
                        effGo.transform.localScale = heroClient.HeroObj.Transform.localScale;
                        AutoDestroy.Begin(effGo, -1f, delegate() {
                            heroClient.HeroObj.IsVisible = true;
                        });
                    }, GameEnum.Fish_3D);
                }
            }
        }, GameEnum.Fish_3D);

        if (SceneLogic.Instance.mIsXiuYuQi == false || clientSeat == myClientSeat) {
            ResManager.LoadAsyn<GameObject>(ResPath.NewHeroEffectPath + vo.EnterPrefab, (ab_data, prefab) => {
                //GameObject prefab = FishResManager.Instance.HeroEnterSceneMap.TryGet(vo.EnterPrefab);
                GameObject obj = GameUtils.CreateGo(prefab, SceneObjMgr.Instance.UIPanelTransform);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                obj.SetActive(true);
                Animator anim = obj.GetComponent<Animator>();
                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);

                GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.HeroBirth);
            }, GameEnum.Fish_3D);
        }
    }

    public void OnHeroSync(SC_GR_HeroSync cmd) 
    {
        byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);
		uint heroCfgID = cmd.HeroCfgID;

        //LogMgr.Log("SceneHeroMgr OnLaunchHero clientSeat:" + clientSeat + " heroCfgID:" + heroCfgID);

        if (SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat) == null) return;

        if (!m_PlayerHeroDictionary.ContainsKey(clientSeat)) //如果不存在,则新增该玩家英雄
        {
			return;
        }
        HeroSyncData heroSyncData = new HeroSyncData();
        heroSyncData.HeroCfgID = cmd.HeroCfgID;
		heroSyncData.targetPos = new Vector3(cmd.D1,cmd.D2,cmd.D3);
        heroSyncData.Speed = cmd.Speed;
		heroSyncData.RotSpeed = 1;
		heroSyncData.FishID = cmd.FishID;
		heroSyncData.TickCount = cmd.TickCount;
		heroSyncData.startPos = new Vector3(cmd.X,cmd.Y,cmd.Z);
		heroSyncData.Anim = (byte)cmd.Anim;
		heroSyncData.subclip = (byte)(cmd.Anim>>8);
		m_PlayerHeroData[clientSeat].Add(heroSyncData);
    }
    
    //同步英雄近身攻击捕获鱼
    public void OnSyncHeroCatch(SC_GR_HeroCatch cmd, uint tickSpan)
    {
        byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);

        bool bNotDelay = tickSpan < ConstValue.FISH_OVER_TIME;
        if (bNotDelay == false)
        {
            return;
        }

        CatchedData cd = new CatchedData();
		cd.CatchType = EnumCatchedType.CATCHED_HERO;
        cd.ClientSeat = clientSeat;
        cd.FishList = new List<CatchFishData>();
		cd.SubType = cmd.ActionID;
		cd.RateValue = SceneLogic.Instance.RoomVo.RoomMultiple;

        ushort[] colliFishIDList = cmd.ArrCollID != null ? cmd.ArrCollID : new ushort[0];
        ushort[] catchFishIDList = cmd.ArrCatchID != null ? cmd.ArrCatchID : new ushort[0];
		int[]    catchScoreIDList = cmd.ArrGold != null ? cmd.ArrGold : new int[0];
		uint[] 	awardItemIDList = cmd.ArrAwardID != null ? cmd.ArrAwardID : new uint[0];

		SceneLogic.Instance.SkillMgr.HandleCatchedResult(cd, 0, colliFishIDList, catchFishIDList, catchScoreIDList, awardItemIDList);        

		HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet (cmd.ActionID);
		if (actVo.BulletCfgID > 0) 
		{
			SceneLogic.Instance.BulletMgr.RemoveBullet (clientSeat, cmd.BulletID);
		}
    }

    //同步远程英雄发射子弹
    public void OnHeroLaunchBullet(SC_GR_HeroBullet cmd, uint tickSpan) 
    {
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID); 
        ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
        if (sp == null)
            return;
        if (SceneLogic.Instance.mIsXiuYuQi && clientSeat != SceneLogic.Instance.PlayerMgr.MyClientSeat) {
            return;
        }
		uint herocfgID = 0;
		Vector3 startPos;
		BaseHeroObject3D hero3DObj = null;
		if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat && SceneLogic.Instance.IsLOOKGuster == false) {
			startPos = currentHero.HeroObj.LcrPos;
			herocfgID = currentHero.HeroCfgID;
			hero3DObj = currentHero.HeroObj;
		} else {
			HeroClient clientHero = m_PlayerHeroDictionary.TryGet (clientSeat);
			startPos = clientHero.HeroObj.LcrPos;
			herocfgID = clientHero.HeroCfgID;
			hero3DObj = clientHero.HeroObj;
		}

        float elapsedTime = tickSpan * 0.001f + FishGameMode.NetDelayTime;
		Vector3 bulletDir;
		HeroFirePosition[] fireposAry = hero3DObj.FirePositionList;
		if (fireposAry == null || fireposAry.Length == 0) {
			bulletDir = new Vector3 (cmd.X, cmd.Y, cmd.Z);        
            //ushort bulletId = cmd.BulletID;

			HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet (cmd.ActionID);
			uint rateValue = FishConfig.Instance.TimeRoomConf.TryGet (SceneLogic.Instance.GetRoomCfgID ()).RoomMultiple;
			SceneLogic.Instance.BulletMgr.OnLaunchHeroBullet (clientSeat, herocfgID, cmd.BulletID, actVo, rateValue, elapsedTime, cmd.LockFishID, startPos, bulletDir,  bulletDir);
		} else {
			Vector3 targetPos = new Vector3 (cmd.X, cmd.Y, cmd.Z);        
			HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet (cmd.ActionID);
			int i = hero3DObj.UseFireSlotIdx ();
			if (i != -1) {
				bool isDirSlerp = fireposAry [i].isDir;
				startPos = fireposAry [i].transform.position;
				bulletDir = fireposAry [i].isDir ? fireposAry [i].transform.forward : (targetPos - fireposAry [i].transform.position).normalized;
				if (FishResManager.Instance.LauncherFireEff.ContainsKey (actVo.FireEffID)) {
					GameObject fireObj = GameUtils.CreateGo (FishResManager.Instance.LauncherFireEff.TryGet (actVo.FireEffID));
					fireObj.transform.SetParent (fireposAry [i].transform);
					fireObj.transform.localPosition = Vector3.zero;
					AutoDestroy.Begin (fireObj);
				}
				
				uint rateValue = FishConfig.Instance.TimeRoomConf.TryGet (SceneLogic.Instance.GetRoomCfgID ()).RoomMultiple;
				SceneLogic.Instance.BulletMgr.OnLaunchHeroBullet (clientSeat, herocfgID, cmd.BulletID, actVo, rateValue, elapsedTime, cmd.LockFishID, startPos, targetPos, bulletDir, isDirSlerp);
			} else {
				LogMgr.LogError (i);
			}
		}
    }

	public void OnHeroLeave(byte clientSeat){
		if (m_PlayerHeroDictionary.ContainsKey (clientSeat)) 
		{
			HeroClient clientHero = m_PlayerHeroDictionary.TryGet (clientSeat);
			clientHero.LeaveScene ();
		}
	}

    public void OnHeroLeave(SC_GR_HeroLeave cmd)
	{
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);	
		if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat && SceneLogic.Instance.IsLOOKGuster == false) 
		{
			SceneLogic.Instance.Notifiy (SysEventType.SceneHeroLeave);
			if (m_MyPlayerHero != null) {
				m_MyPlayerHero.LeaveScene ();
			}
		} else {
            this.OnHeroLeave(clientSeat);
		}
	}

    //角度到方向的变换
    public static Vector2 CalculateDegreeToDir(short angle)
    {
        Vector2 ret = Vector2.zero;

        short inversionAngle = SceneLogic.Instance.FModel.AngleInversion(angle);

        //LogMgr.Log("SceneHeroMgr CalculateDegreeToDir angle:" + angle + " inversionAngle:" + inversionAngle);

		float angleFloatVal = Utility.ShortToFloat180(inversionAngle);
        float radian = angleFloatVal / 180f * Mathf.PI;
        //float cosValue = Mathf.Cos(radian);

        float dirX = Mathf.Tan(radian);
        float dirY = 1;
        if (Mathf.Abs(angleFloatVal) > 90)
        {
            dirX *= -1;
            dirY *= -1;
        }
        ret = new Vector2(dirX, dirY).normalized;
		return ret;
    }
    
    public static short CalculateDegreeToShortValue(Vector3 direct)
    {
        short ret = 0;
		float floatAngle = CalculateDegreeToFloatValue(direct);
		ret = Utility.FloatToShort180(floatAngle);
        return ret;
    }

    public static float CalculateDegreeToFloatValue(Vector3 targetPos, Vector3 centerPos) 
    {
		return CalculateDegreeToFloatValue (targetPos - centerPos);
    }

	public static float CalculateDegreeToFloatValue(Vector3 direct)
	{
		Vector2 UpDir = new Vector2(0, 1);
		Vector2 normalizedDir = direct.normalized;
		float dot = Vector2.Dot(UpDir, normalizedDir);
		float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
		if (normalizedDir.x < 0f)
		{
			angle *= -1;
		}

		if (direct.z < 0) {
			angle = 180f - angle ;
		}
		return angle;
	}
}