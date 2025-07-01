using System;
using System.Collections.Generic;
using UnityEngine;

public class LCRReduce
{
	public float reduceRate;
	public float duration;
    public int mAddCatchRange = 0;//捕获范围附加
    public int mAddCatchCount = 0;//捕获个数附加
	public LCRReduce(float rate, float dur){
		this.reduceRate = rate;
		this.duration = dur;
	}
    public LCRReduce(float rate, float dur,int range,int count) {
        this.reduceRate = rate;
        this.duration = dur;
        this.mAddCatchRange = range;
        this.mAddCatchCount = count;
    }
}

public class Launcher
{
	bool m_bMyself;
	Vector2 m_Direction;
    //Vector3 m_LauncherPos;
	LauncherVo m_LauncherVo;
	byte m_CSeat;
	uint m_LauncherType, m_LauncherLevel, m_RateValue;
	float m_LauncherInterval = 0f, lcrInterRate = 1f;
    private float mAddCatchRange;//附加捕获范围
    private int mAddCatchCount;//附加捕获个数
	public float m_LauncherTime = 0;
	bool m_RateValid = false, m_LaunchValid = true, m_bLauncherXPSkill = false, m_IsBackrupt = false, m_IsTriggerSkill = false;

    private GameObject mObj;
	Transform m_TransformHandle,mLauncherBaseCon;
	Vector2 m_GunPivot;
	float m_Angle;

	EnergyPool m_EnergyPoolLogic = new EnergyPool();
	GunBarrel    m_GunBarrel = new GunBarrel();
	RateListPanel	m_RateSelPanel = new RateListPanel();
	public TableUserInfoPanel m_UserInfoPanel = new TableUserInfoPanel();

	public UILabel mLbGold;
    UILabel rateLabel, engeryLabel;
	Transform goldIconTrans;
	GameObject m_BankruptcyObj = null;
	Vector3 mGoldWorldPos;

	uint[] lcrLevels;
	float mGunFireTime = 0;
	long mLcrEngery;

    public float AddCatchRange {//是否附加攻击范围
        get {
            return this.mAddCatchRange;
        }
    }
    public float CatchRange {//炮台当前子弹捕获范围
        get {
            return this.Vo.Range + this.mAddCatchRange;
        }
    }
    public int CatchCount {//炮台当前子弹捕获个数
        get {
            return this.Vo.MaxCatch + this.mAddCatchCount;
        }
    }

	public Launcher(uint launcherType, uint launcherLevel, bool valid, byte seat, uint rateValue, long pLcrEngery)
	{
		mIsShutDown = false;
		m_CSeat = seat;
		m_LauncherType = launcherType;
		m_LauncherLevel = launcherLevel;
		m_RateValue = rateValue;
		m_LaunchValid = valid;
		mLcrEngery = pLcrEngery;
		m_RateValid = true;
		uint mergerID = (uint)(launcherLevel << 24 | launcherType);
		m_LauncherVo = FishConfig.Instance.LauncherConf.TryGet(mergerID);

		m_bMyself = SceneLogic.Instance.PlayerMgr.MyClientSeat == m_CSeat;
		m_LauncherInterval = m_LauncherVo.Interval;
		lcrInterRate = 1f;
		m_LauncherTime = 0;

        int i = 0;
        List<byte> list = FishConfig.GetLauncherLvListByVip(launcherType, RoleInfoModel.Instance.Self.VipLv);
        lcrLevels = new uint[list.Count];
        for (i = 0; i < list.Count; i++) {
            lcrLevels[i] = list[i];
        }

        ResetGunAngleRange();
	}

	public void Init() {
		byte indx = SceneLogic.Instance.FModel.LauncherPrefabIndx(m_CSeat, m_bMyself);
        this.mObj = GameUtils.CreateGo(FishResManager.Instance.LauncherObject.TryGet(indx), SceneObjMgr.Instance.UIPanelTransform);
        m_TransformHandle = this.mObj.transform;

		InitComponets();
		if (m_bMyself) {
            m_EnergyPoolLogic.InitEnergy(m_LauncherVo.SkillID, mLcrEngery, m_RateValue);
            this.UpdateEneryFullStatusEffect();
		}
        rateLabel.text = string.Format("x{0}倍", m_RateValue);
		m_GunBarrel.onFireFrame = HandleOnFire;

        if (SceneLogic.Instance.mIsXiuYuQi && m_bMyself == false) {
            this.mObj.SetActive(false);
        } else {
            if (SceneGameUIMgr.ui_hide_finish_time > 0) {
                this.mObj.SetActive(false);
                TimeManager.DelayExec(SceneGameUIMgr.ui_hide_finish_time - Time.realtimeSinceStartup, () => {
                    if (SceneLogic.Instance.mIsXiuYuQi == false || m_bMyself) {
                        this.mObj.SetActive(true);
                        this.Show();
                    }
                });
            }
        }

        if (m_bMyself) {
            MtaManager.BeginLauncherEvent(m_LauncherVo);
        }
	}

    public void SetBtnEnable(bool is_enable) {//设置炮台相关设置按钮是否禁用
        SceneLogic.Instance.LogicUI.SetLCRBtnEnable(is_enable);

        if (this.selfSeatUI != null) {
            this.selfSeatUI.btnAdd.GetComponent<BoxCollider>().enabled = is_enable;
            this.selfSeatUI.btnSub.GetComponent<BoxCollider>().enabled = is_enable;
            this.selfSeatUI.rateBtn.GetComponent<BoxCollider>().enabled = is_enable;

            this.selfSeatUI.btnAdd.GetComponent<UISprite>().IsGray = is_enable == false;
            this.selfSeatUI.btnSub.GetComponent<UISprite>().IsGray = is_enable == false;
            this.selfSeatUI.rateBtn.GetComponent<UISprite>().IsGray = is_enable == false;
        }
    }

    public List<GameObject> mEffBreanchList = new List<GameObject>();
    public void ShowBranchEff(int num) {//显示分叉炮特效  0表示隐藏
        while (mEffBreanchList.Count > num) {
            GameObject.Destroy(mEffBreanchList[0]);
            mEffBreanchList.RemoveAt(0);
        }
        GameObject eff;
        while (mEffBreanchList.Count < num) {
            eff = GameUtils.CreateGo(FishResManager.Instance.mEffBranch, SceneObjMgr.Instance.UIPanelTransform);
            eff.transform.SetParent(this.m_GunBarrel.BaseTrans);
            eff.transform.localPosition = Vector3.zero;
            //eff.transform.position = this.SeatTransform.position;
            mEffBreanchList.Add(eff);
        }

        this.UpdateFunAngleRange(num + 1);

        this.RefershBranchAngle(this.m_Angle);
    }

    public void UpdateFunAngleRange(int branchNum) {
        if (branchNum == 3) {
            this.SetGunAngleRange(ConstValue.LaunchRotRangeMin + 30f, ConstValue.LaunchRotRangeMax - 30f);
        } else if (branchNum == 5) {
            this.SetGunAngleRange(ConstValue.LaunchRotRangeMin + 50f, ConstValue.LaunchRotRangeMax - 50f);
        } else {
            this.SetGunAngleRange(ConstValue.LaunchRotRangeMin, ConstValue.LaunchRotRangeMax);
        }
    }

    public void RefershBranchAngle(float angle) {//更新分叉炮特效角度
        float[] angles = SceneLogic.Instance.BulletMgr.GetBreanchAngles(this.mEffBreanchList.Count+1, angle);
        for (int i = 1; i < angles.Length; i++) {
            this.mEffBreanchList[i-1].transform.localEulerAngles = new Vector3(0, 0, angles[i]);
        }
    }

	// 是否激活能量炮
	bool mIsRatingLCRActive = false;
	public void SetRateLCRActive(bool isActive)
	{
		mIsRatingLCRActive = isActive;
		SetBtnEnable (!mIsRatingLCRActive);
		SetHalo (mIsRatingLCRActive);
        this.m_UserInfoPanel.MoveUserInfo(isActive);
        if (isActive == false) {
            if (m_RateSelPanel.isActive) {
                m_RateSelPanel.Hide();
            }
            SceneLogic.Instance.LogicUI.HideLCRList();	
        }

	}
    public void ShowUserInfo(bool is_show) {
        this.m_UserInfoPanel.ShowInfo(is_show);
    }

	public void OperatLaunch(int idx)
	{
		if (mIsRatingLCRActive) {
            return;//能量炮状态不能进行炮台相关操作
        }
		int i = 0, j = 0;
		uint[] uintAry = null;
		switch (idx) {
		case 0:
			if (!IsSettingLCR) {
				uintAry = SceneLogic.Instance.FModel.GetAvaibleRateValuList ();
				i = Array.IndexOf (uintAry, m_RateValue);
				j = Mathf.Min (i + 1, uintAry.Length - 1);
				if (i != j)
					HandleSelectRate (uintAry [j]);
			}
			break;
		case 1:
			if (!IsSettingLCR) {
				uintAry = SceneLogic.Instance.FModel.GetAvaibleRateValuList ();
				i = Array.IndexOf (uintAry, m_RateValue);
				j = Mathf.Max (i - 1, 0);
				if (i != j)
					HandleSelectRate (uintAry [j]);
			}
			break;
		case 2:
            OnSubButtonHandle(selfSeatUI.btnSub);
			break;
		case 3:
            OnAddButtonHandle(selfSeatUI.btnAdd);
			break;
		}
	}

	public Transform SeatTransform
	{
		get { return m_TransformHandle;}
	}
    public GunBarrel GunBarrel {
        get {
            return this.m_GunBarrel;
        }
    }

    //public void OnGoldArrive(int addGoldNum)
    //{
    //    goldIconTrans.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    //    TweenScale.Begin(goldIconTrans.gameObject, 0.5f, Vector3.one);
    //}

	public bool IsSettingLCR = false;
	//===========================================  事件处理 =====================================
	void OnAddButtonHandle(GameObject button)
	{
		GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.SwitchBulletMultiple);
		int curChangeLvIndex = Array.IndexOf (lcrLevels, m_LauncherLevel);
		curChangeLvIndex = curChangeLvIndex >= lcrLevels.Length - 1 ? 0 : curChangeLvIndex + 1;
		FishNetAPI.Instance.ChangeLauncher (m_LauncherType, (byte)lcrLevels[curChangeLvIndex]);
		IsSettingLCR = true;

	}

	void OnSubButtonHandle(GameObject button)
	{
		GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.SwitchBulletMultiple);
		int curChangeLvIndex = Array.IndexOf (lcrLevels, m_LauncherLevel);
		curChangeLvIndex = curChangeLvIndex <= 0 ? lcrLevels.Length-1 : curChangeLvIndex - 1;
        FishNetAPI.Instance.ChangeLauncher(m_LauncherType, (byte)lcrLevels[curChangeLvIndex]);
        IsSettingLCR = true;
	}

	public float GunFireTick
	{
		get { return mGunFireTime;}
	}
    public EnergyPool EnergyPoolLogic
	{
        get { return m_EnergyPoolLogic; }
	}
	void OnClickAutoLaunch(GameObject go)
	{

	}

	void HandleSelectRate(uint rateValue)
	{
		FishNetAPI.Instance.ChangeRateValue (rateValue);
		IsSettingLCR = true;
	}

	void OnRatioButtonMessage(GameObject button)
	{
		if (m_RateSelPanel.isActive)
			m_RateSelPanel.Hide ();
		else
			m_RateSelPanel.Show (m_RateValue);
	}

	void OnShowChangeLCR(GameObject button)
	{
		SceneLogic.Instance.LogicUI.ShowLCRList (true);	
	}

	public void HandleScaterSkill(GameObject button)
	{
		m_IsTriggerSkill = true;
	}

	public void ShowRateList(bool isshow){
		if (isshow)
			m_RateSelPanel.Show (m_RateValue);
		else
			m_RateSelPanel.Hide ();
	}

	//======================================================================================
	SelfSeatUIRef selfSeatUI;
	void InitComponets()
	{
		if (m_bMyself)
		{
			selfSeatUI = m_TransformHandle.GetComponent<SelfSeatUIRef>();
			UIEventListener.Get(selfSeatUI.btnAdd).onClick = OnAddButtonHandle;
			UIEventListener.Get(selfSeatUI.btnSub).onClick = OnSubButtonHandle;
			UIEventListener.Get(selfSeatUI.rateBtn).onClick = OnRatioButtonMessage;
			UIEventListener.Get(selfSeatUI.launcherCon.gameObject).onClick = OnShowChangeLCR;
			m_RateSelPanel.OnSelected += HandleSelectRate;
			mLbGold = selfSeatUI.goldLabel;
			rateLabel = selfSeatUI.rateLabel;
			goldIconTrans = selfSeatUI.goldIconTrans;
			mLauncherBaseCon = selfSeatUI.launcherCon;
            m_RateSelPanel.Init(selfSeatUI.ratePanel);
            UIEventListener.Get(selfSeatUI.skillIcon.gameObject).onClick = HandleScaterSkill;
            UI_Ticks.AddTickListener(selfSeatUI.skillIcon.gameObject, UI_Ticks.ShowLuncherSkill);

            if (m_CSeat == 0) {
                m_UserInfoPanel.Init(selfSeatUI.left_user_info, m_CSeat);
				selfSeatUI.right_user_info.panelObj.SetActive(false);
            } else {
                m_UserInfoPanel.Init(selfSeatUI.right_user_info, m_CSeat);
				selfSeatUI.left_user_info.panelObj.SetActive(false);
            }
			m_EnergyPoolLogic.Init (this, selfSeatUI.skillIcon, selfSeatUI.engeryProgress, selfSeatUI.engeryLabel);

            //当前炮台位置表示特效
            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_PlayerPosition", (ab_data, obj) => {
                obj = GameObject.Instantiate<GameObject>(obj);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                obj.SetActive(true);
                obj.transform.SetParent(m_TransformHandle);
                obj.transform.localScale = Vector3.one * 0.01851852f;
                obj.transform.localPosition = mLauncherBaseCon.localPosition;
                GameObject.Destroy(obj, 50);
            }, GameEnum.Fish_3D);
		}
		else
		{
			SeatUIRef seatUI = m_TransformHandle.GetComponent<SeatUIRef>();
			mLbGold = seatUI.GoldLabel;
			rateLabel = seatUI.RateLabel;
			goldIconTrans = seatUI.goldIconTrans;
			mLauncherBaseCon = seatUI.cannonTrans;
			m_UserInfoPanel.Init (seatUI.usrInfoUI, m_CSeat);

            UISprite spr = goldIconTrans.GetComponent<UISprite>();
            spr.IsGray = true;
		}
        UIEventListener.Get(goldIconTrans.parent.gameObject).onClick = this.OnButtonClick;
		UpdateUserGold(m_CSeat);
		CreatGunBarrel(m_LauncherVo);
		UpdateRootPos();

        float _x;
        if (m_bMyself) {
            _x = 450;
        } else {
            _x = 550;
        }
        switch (m_CSeat) {
            case 0:
                m_TransformHandle.localPosition = new Vector3(-_x * Resolution.AdaptAspect, -540);
                break;
            case 1:
                m_TransformHandle.localPosition = new Vector3(_x * Resolution.AdaptAspect, -540);
                break;
            case 2:
                m_TransformHandle.localPosition = new Vector3(_x * Resolution.AdaptAspect, 540);
                break;
            case 3:
                m_TransformHandle.localPosition = new Vector3(-_x * Resolution.AdaptAspect, 540);
                break;
            default:
                m_TransformHandle.localPosition = Vector3.zero;
                break;
        }

        //m_TransformHandle.position = m_LauncherPos;

        //Vector3 lcrScreenpos = LauncherPositionSetting.GetLcrCenterPos (m_CSeat);
        //lcrScreenpos.z = ConstValue.NEAR_Z+0.1f;
        //lcrScreenpos = SceneObjMgr.Instance.MainCam.WorldToScreenPoint (lcrScreenpos);
        //lcrScreenpos.z = 0f;
        //Vector3 wp = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint (lcrScreenpos);
        //Vector3 posOffset = m_TransformHandle.InverseTransformPoint(wp) - mLauncherBaseCon.localPosition;
        //m_TransformHandle.localPosition += posOffset;
        //m_LauncherPos = m_TransformHandle.position;

        //WndManager.Instance.SetPanelSort(m_TransformHandle.gameObject);
	}

	void CreatGunBarrel(LauncherVo launcherVo)
	{
        GameObject gunGo;
        if (m_bMyself) {
            gunGo = GameUtils.CreateGo(FishResManager.Instance.GunBarrelObjList[launcherVo.SourceSelf], mLauncherBaseCon);
        } else {
            gunGo = GameUtils.CreateGo(FishResManager.Instance.GunBarrelObjList[launcherVo.Source], mLauncherBaseCon);
        }
		gunGo.transform.localPosition = Vector3.zero;
		gunGo.transform.localRotation = Quaternion.identity;
		m_GunBarrel.Init(gunGo, launcherVo);
		m_GunBarrel.Reset(m_CSeat, launcherVo.Interval);
		mGunFireTime = m_GunBarrel.GunFireTick;
	}

    public void ResetSkillAndUse() {//重置技能且使用
        if (m_bMyself) {
            m_EnergyPoolLogic.IsCDState = false;
            short angle = Utility.FloatToShort(m_Angle);
            angle = SceneLogic.Instance.FModel.AngleInversion(angle);
            SkillVo skvo = FishConfig.Instance.SkillConf.TryGet(m_LauncherVo.SkillID);
            SceneLogic.Instance.SkillMgr.SkillPrepareForAim(skvo, delegate {
                FishNetAPI.Instance.SkillCaster(GInput.mousePosition, angle);
            });
            m_bLauncherXPSkill = true;
        } else {
            short angle = Utility.FloatToShort(m_Angle);
            angle = SceneLogic.Instance.FModel.AngleInversion(angle);
            FishNetAPI.Instance.SkillCaster(GInput.mousePosition, angle);
        }
    }

    public List<LCRReduce> lcrReduceData = new List<LCRReduce>();
    private LCRReduce __lcr;
    private float _tick_time = 0;
	public void Update(float delta)
	{
		if (SceneLogic.Instance.IsGameOver)
			return;

        if (lcrReduceData.Count > 0) {
            this.mAddCatchRange = 0;
            this.mAddCatchCount = 0;
            float buff_spd = 1;//加速buff
            float debuff_spd = 1;//减速buff
            for (int i = lcrReduceData.Count-1; i >= 0; i--) {
                __lcr = lcrReduceData[i];
                if (__lcr.duration > delta) {
                    __lcr.duration -= delta;
                    if (__lcr.reduceRate > 1) {//减速
                        debuff_spd = Mathf.Max(lcrInterRate, __lcr.reduceRate);
                    } else {//加速
                        buff_spd = Mathf.Min(lcrInterRate, __lcr.reduceRate);
                    }
                    this.mAddCatchRange = Mathf.Max(this.mAddCatchRange, __lcr.mAddCatchRange);
                    this.mAddCatchCount = Mathf.Max(this.mAddCatchCount, __lcr.mAddCatchCount);
                } else {
                    lcrReduceData.RemoveAt(i);
                }
            }
            lcrInterRate = buff_spd * debuff_spd;
        } else {
            lcrInterRate = 1f;
            this.mAddCatchRange = 0;
            this.mAddCatchCount = 0;
        }
		m_GunBarrel.Update(delta);

        UpdateRootPos();

        if (SceneLogic.Instance.mIsXiuYuQi) {
            UpdateLaunchAngle();    //更新炮台角度
			return;
		}

        //处于大招CD状态
        if (m_bMyself && m_EnergyPoolLogic.IsCDState) {
            m_EnergyPoolLogic.PlayCD(delta);
            //UpdateLaunchAngle();    //更新炮台角度
            //return;
        }

		var player = SceneLogic.Instance.PlayerMgr.GetPlayer (m_CSeat);
		ushort lockfishID = player != null ? player.LockedFishID : (ushort)0;
		if (lockfishID != 0)
		{
			Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(lockfishID);
			if(fish != null)
			{
				if (player.LockFishBodyIdx != 0xFF) {					
					Vector3 lockPos;
					fish.GetBodyPartScreenPos (player.LockFishBodyIdx, out lockPos);
					UpdateLaunchByLockFish(lockPos);
				} else {
					UpdateLaunchByLockFish(fish.ScreenPos);
				}
			}
		}
		else
		{
			UpdateLaunchAngle();    //更新炮台角度
		}

		if (m_BankruptcyObj != null)
		{
			m_IsBackrupt = SceneLogic.Instance.FModel.CheckIsBankrup (Vo);
			m_BankruptcyObj.SetActive(m_IsBackrupt);
		}

		m_LauncherTime += delta;
		if (CheckXPSkill() && !SceneLogic.Instance.bClearScene)//向服务器发送大招请求
		{
			short angle = Utility.FloatToShort(m_Angle);
			angle = SceneLogic.Instance.FModel.AngleInversion(angle);
			SkillVo skvo = FishConfig.Instance.SkillConf.TryGet(m_LauncherVo.SkillID);
			SceneLogic.Instance.SkillMgr.SkillPrepareForAim (skvo, delegate {
				FishNetAPI.Instance.SkillCaster (GInput.mousePosition, angle);
			});
			m_bLauncherXPSkill = true;
		}
		m_IsTriggerSkill = false;

		if (CheckLaunch())
		{
			float lcrinterval = m_LauncherInterval * lcrInterRate;
			if (m_LauncherTime >= lcrinterval)
			{
				m_LauncherTime = 0;
				if (!SceneLogic.Instance.FModel.CheckGoldEnought(m_CSeat, m_RateValue,  m_LauncherVo))//检测是否钱够
				{
                    if (_tick_time < Time.realtimeSinceStartup) {
                        _tick_time = Time.realtimeSinceStartup + 5;
                        SceneLogic.Instance.LogicUI.SetAutoLaunch(false);
                        if (RoleInfoModel.Instance.CoinMode == EnumCoinMode.Score) {
                            SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_33"));
                        } else {
                            PayItem item;
                            if (SceneLogic.Instance.RoomVo.QuickCharge > 0) {
                                item = ShopManager.GetPayByIndex(SceneLogic.Instance.RoomVo.QuickCharge - 1);
                            }else{
                                item = null;
                            }
                            if(item != null)
                            {
                                //UI.EnterUI<UI_QuickRecharge>(ui => 
                                //{
                                //    ui.InitData(1, item, () =>
                                //    {
                                      
                                //    });
                                //});

                                UI.EnterUI<UI_QuickRecharge>(GameEnum.All).InitData(1, item, () =>
                                {
                                    //乐豆不足
                                    NetServices.Instance.Send(NetCmdType.SUB_C_SAFE, new CS_GR_Safe());//检查救济金领取
                                });

                            } else {
                                SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_5"));
                            }
                        }
                    }
					return;
				}

				short angle = Utility.FloatToShort(m_Angle);
				angle = SceneLogic.Instance.FModel.AngleInversion(angle);
                SceneLogic.Instance.BulletMgr.LaunchBullet(angle);
				
			}
		}
	}


	public short LcrAngle
	{
		get { return Utility.FloatToShort(m_Angle); }
	}

	//根据鱼的屏幕坐标更新炮台位置。
	void UpdateLaunchByLockFish(Vector3 screePos)
	{
		bool vaild = m_LaunchValid & m_RateValid;
		if (!vaild)// || m_bLauncherXPSkill)
			return;
		Vector2 UpDir = new Vector2(0, m_CSeat > 1 ? -1 : 1);
		Vector2 pos = new Vector2(screePos.x, screePos.y);
		m_Direction = (pos - m_GunPivot).normalized;
		float dot = Vector2.Dot(UpDir, m_Direction);
		Vector3 cos = Vector3.Cross (new Vector3 (UpDir.x, UpDir.y, 0f), new Vector3 (m_Direction.x, m_Direction.y, 0f));
		m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f)) * Mathf.Rad2Deg;
		if (cos.z < 0)
			m_Angle = -m_Angle;
		m_Angle = Mathf.Clamp(m_Angle, mMinAngle, mMaxAngle);
        m_GunBarrel.UpdateAngle(m_Angle);
        this.RefershBranchAngle(m_Angle);
	}
    //根据鱼的屏幕坐标获取炮台角度
    public float GetLaunchByLockFishAngle(Vector3 screePos) {
        Vector2 UpDir = new Vector2(0, m_CSeat > 1 ? -1 : 1);
        Vector2 pos = new Vector2(screePos.x, screePos.y);
        Vector2 dire = (pos - m_GunPivot).normalized;
        float dot = Vector2.Dot(UpDir, dire);
        Vector3 cos = Vector3.Cross(new Vector3(UpDir.x, UpDir.y, 0f), new Vector3(dire.x, dire.y, 0f));
        float angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f)) * Mathf.Rad2Deg;
        if (cos.z < 0)
            angle = -angle;
        return Mathf.Clamp(angle, mMinAngle, mMaxAngle);
    }
	public void SetGunAngleRange(float minAngle, float maxAngle){
		mMinAngle = minAngle;
		mMaxAngle = maxAngle;
	}

	public void ResetGunAngleRange(){
		mMinAngle = ConstValue.LaunchRotRangeMin;
		mMaxAngle = ConstValue.LaunchRotRangeMax;
	}

	float mMinAngle = -85f, mMaxAngle = 85f;
	void UpdateLaunchAngle()
	{
        if (SceneLogic.Instance.IsLOOKGuster) {
            return;
        }
        if (FishGameMode.IsTap3DScene == false) {
            return;
        }

		bool vaild = m_LaunchValid & m_RateValid;

        if (!m_bMyself || !vaild || m_IsBackrupt){// || m_bLauncherXPSkill) { //等待选中目标时间炮台继续更新角度
            return;
        }
		
		Vector3 position = new Vector3(GInput.mousePosition.x, GInput.mousePosition.y, ConstValue.NEAR_Z);
	
		Vector2 UpDir = new Vector2(0, m_CSeat > 1 ? -1 : 1);
		Vector2 pos = new Vector2(position.x, position.y);
		m_Direction = (pos - m_GunPivot);

		if (m_CSeat <= 1 && m_Direction.y < 0.1f)
			m_Direction.y = 0.1f;
		else if (m_CSeat > 1 && m_Direction.y > -0.1f)
			m_Direction.y = 0.1f;

		if (m_Direction.x < 0.1f && m_Direction.x > 0.0f)
			m_Direction.x = 0.1f;
		else if (m_Direction.x > -0.1f && m_Direction.x < 0.0f)
			m_Direction.x = -0.1f;

		m_Direction.Normalize();
		float dot = Vector2.Dot(UpDir, m_Direction);
		m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f)) * Mathf.Rad2Deg;
		//第一三象和第二四象角度相反
		if (m_Direction.x >= 0)
			m_Angle = -m_Angle;
		m_Angle = Mathf.Clamp(m_Angle, mMinAngle, mMaxAngle);
		m_GunBarrel.UpdateAngle(m_Angle);
        this.RefershBranchAngle(m_Angle);
	}

	GameObject skillFullEffGo = null;
	public void UpdateEnergy(long energy)
	{
		if (m_bMyself && m_EnergyPoolLogic != null)
		{
			if (m_EnergyPoolLogic.UpdateEnergyPool (energy)) {
				SceneLogic.Instance.Notifiy (SysEventType.EngeryFirstFull);
			}
			UpdateEneryFullStatusEffect ();
		}
	}

	public void UpdateEneryFullStatusEffect()
	{
		if (!m_bMyself)
			return;
		
		if (m_EnergyPoolLogic.IsEngeryFull) {
			if (skillFullEffGo == null) {
				skillFullEffGo = GameUtils.CreateGo (FishResManager.Instance.Eff_EngeryFull);
                skillFullEffGo.transform.SetParent(this.selfSeatUI.skillIcon.transform);
				skillFullEffGo.transform.localScale = Vector3.one;
                skillFullEffGo.transform.localPosition = Vector3.zero;
                GameUtils.PlayPS(skillFullEffGo);
			}
			skillFullEffGo.SetActive (true);
		} else {
			if (skillFullEffGo != null) {
				GameObject.Destroy (skillFullEffGo);
				skillFullEffGo = null;
			}
		}
	}

	//非自己炮台角度更新
	public void UpdatOtherAngle()
	{
		Vector2 UpDir = new Vector2(0, m_CSeat> 1 ? -1 : 1);
		float dot = 0.0f;
		dot = Vector2.Dot(UpDir, m_Direction);
		m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f)) * Mathf.Rad2Deg;
		if (m_Direction.x >= 0)
			m_Angle = -m_Angle;
		m_Angle = Mathf.Clamp(m_Angle, mMinAngle, mMaxAngle);
		if (SceneLogic.Instance.IsLOOKGuster == false && m_bMyself)
			return;
		m_Angle = m_CSeat > 1 ? -m_Angle : m_Angle;
        m_GunBarrel.UpdateAngle(m_Angle);
        this.RefershBranchAngle(m_Angle);
	}

    public Vector3 GetGoldAnimPos() {//获取金币数字弹出动画位置
        return mLbGold.transform.TransformPoint(new Vector3(mLbGold.width / 2, 0));
    }

	public void UpdateUserGold(byte clientSeat)
	{
		mLbGold.text = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(clientSeat).ToString();
	}

	void UpdateRootPos()
	{
        //Vector3 pos;
        ////Vector3 scrPos = new Vector3(Screen.width, Screen.height, 0);
        ////Vector3 worldPos = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(scrPos);

        ////float x = 0.214f;//坐标偏差值
        //float x = 0.2625f;//坐标临时调整
        //switch (m_CSeat) {
        //    case 0:
        //        pos = new Vector3(x, 0);
        //        break;
        //    case 1:
        //        pos = new Vector3(1 - x, 0);
        //        break;
        //    case 2:
        //        pos = new Vector3(1 - x, 1);
        //        break;
        //    case 3:
        //        pos = new Vector3(x, 1);
        //        break;
        //    default:
        //        pos = Vector3.zero;
        //        break;
        //}
        //m_LauncherPos = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(pos);
		m_GunPivot = m_GunBarrel.GunPivot;
		mGoldWorldPos = goldIconTrans.position;
	}

	public Matrix4x4 localToWorldMatx
	{
		get {return m_TransformHandle.localToWorldMatrix;}
	}

	public Matrix4x4 worldTolocalMatx
	{
		get {return m_TransformHandle.worldToLocalMatrix;}
	}

	bool CheckLaunch()
	{
		if (!m_bMyself || IsSettingLCR)// || m_bLauncherXPSkill)      // 炮为锁状态
			return false;

        if (SceneLogic.Instance.PlayerMgr.AutoShot || Input.GetKey(KeyCode.Space)) {
            return true;
        }
        //if (SceneLogic.Instance.PlayerMgr.AutoShot == false && SceneLogic.Instance.PlayerMgr.MySelf.LockedFishID > 0 && SceneLogic.Instance.PlayerMgr.mIsTouchValid) { //锁定鱼之后 自动发炮
        //    return true;
        //}
		bool isBlock = GInput.IsBlock3D || WndManager.Instance.HasTopWnd;
		bool bcanFire = GInput.GetMouseButton(0) && !isBlock;
        if (bcanFire && FishGameMode.IsTap3DScene && SceneLogic.Instance.PlayerMgr.mIsTouchValid) {
			return true;
		}
		return false;
	}

	GlobalEffectData mPrepareEffData = null;
	bool CheckXPSkill() {        
		if (!m_bMyself)
			return false;
		
		if (SceneLogic.Instance.SkillMgr.IsCastingSkill)
			return false;
		
		if (m_EnergyPoolLogic.IsCDState)
			return false;
		
		if (m_bLauncherXPSkill || WndManager.Instance.HasTopWnd)
			return false;
		
		bool Vaild = m_LaunchValid & m_RateValid;
		if (!Vaild)
			return false;
		
		if (!m_EnergyPoolLogic.IsEngeryFull)
			return false;

        if (!m_IsTriggerSkill) {
            //无道具模式不使用自动炮台技能释放
            if (GameConfig.OP_AutoLauncher && RoleInfoModel.Instance.GameMode != EnumGameMode.Mode_NotItem && SceneLogic.Instance.PlayerMgr.AutoShot) {//炮台技能自动释放逻辑判定
                return ItemManager.IsUseLauncher(SceneLogic.Instance.PlayerMgr.MySelf, FishConfig.Instance.mAutoUseConf.TryGet(this.m_LauncherVo.AutoType));
            } else {
                return false;
            }
        } else {
            return true;
        }
	}


	void SetHalo(bool isEnable)
	{
		m_GunBarrel.SetHalo (isEnable);
	}

	public void ChangeLauncher(uint laucherType, uint launcherlevel, bool lrcValid) {
        if (m_bMyself && m_LauncherVo != null) {
            MtaManager.EndLauncherEvent(m_LauncherVo);
        }
		m_LauncherType = laucherType;
		m_LauncherLevel = launcherlevel;

		m_Angle = m_GunBarrel.GetAngle();
		m_GunBarrel.ShutDown();
		SceneLogic.Instance.EffectMgr.PlayChangeLauncherEft(LauncherPos);
		m_LauncherVo = FishConfig.Instance.LauncherConf.TryGet((uint)(m_LauncherLevel<<24 | m_LauncherType));
		m_LauncherInterval = m_LauncherVo.Interval;
		CreatGunBarrel(m_LauncherVo);
        m_GunBarrel.UpdateAngle(m_Angle);
        this.RefershBranchAngle(m_Angle);
		m_LaunchValid = lrcValid;
		m_RateValid = SceneLogic.Instance.FModel.CheckRateValueAvaible (m_RateValue);
        if (m_bMyself) {
            m_EnergyPoolLogic.InitEnergy(m_LauncherVo.SkillID, m_EnergyPoolLogic.CurEnergy, m_RateValue);
            this.UpdateEneryFullStatusEffect();

            MtaManager.BeginLauncherEvent(m_LauncherVo);
        }
		IsSettingLCR = false;
	}

	public Vector3 GunDirection
	{
		get { return m_GunBarrel.BaseTrans.up;}
	}

	public void ChangeRate(uint rateValue, bool bvaild)
	{
		m_RateValue = rateValue;
		SceneLogic.Instance.EffectMgr.PlayChangeLauncherEft(LauncherPos);
		rateLabel.text = string.Format("x{0}倍", m_RateValue);
        m_RateValid = bvaild;
        m_Angle = m_GunBarrel.GetAngle();
        m_GunBarrel.UpdateAngle(m_Angle);
        this.RefershBranchAngle(m_Angle);
		IsSettingLCR = false;
        if (m_bMyself) {
			if(m_EnergyPoolLogic.OnLcrMultiChange(m_RateValue))
				SceneLogic.Instance.Notifiy (SysEventType.EngeryFirstFull);
            this.UpdateEneryFullStatusEffect();
        }

	}

	void HandleOnFire()
	{
		SceneLogic.Instance.EffectMgr.PlayGunBarrelFire(m_GunBarrel.FireTrans, Vo.FireEffID);
	}

	public void OnLauncherBullet(float delaySecs)
	{
        if (this.mObj.activeSelf) {
            m_GunBarrel.PlayAnimation(delaySecs);
        }

		m_LauncherTime = 0;
	}

	public void OnPlayerXPSkill(byte clientSeat, short degree)
	{
        if (m_bMyself)
        {
            m_bLauncherXPSkill = false;
            ClearXPSkillEft();
			m_EnergyPoolLogic.IsCDState = true;
            m_EnergyPoolLogic.ConsumeSkEngery();
			if (mPrepareEffData != null)
				GlobalEffectMgr.Instance.ClearEff (mPrepareEffData);
			mPrepareEffData = null;
			UpdateEneryFullStatusEffect ();
        }
	}

	void ClearXPSkillEft()
	{
		
	}

	public void UpdatePlayerInfo(uint userID)
	{
		
	}

    public void Hide() {
        Vector3 pos = m_TransformHandle.localPosition;
        if (m_CSeat == 0 || m_CSeat == 1) {
            pos.y -= 220;
        } else {
            pos.y += 220;
        }
        TweenPosition.Begin(this.mObj, SceneGameUIMgr.anim_time, pos, false);
    }
    public void Show() {
        Vector3 target = m_TransformHandle.localPosition;
        Vector3 pos = target;
        if (m_CSeat == 0 || m_CSeat == 1) {
            pos.y -= 220;
        } else {
            pos.y += 220;
        }
        m_TransformHandle.localPosition = pos;
        TweenPosition.Begin(this.mObj, SceneGameUIMgr.anim_time, target, false);
    }

	bool mIsShutDown = false;
	public void Shutdown()
	{
		mIsShutDown = true;
		m_GunBarrel.ShutDown();
		if (m_TransformHandle != null)
		{
            this.Hide();
            GameObject.Destroy(this.mObj, SceneGameUIMgr.anim_time);
			m_TransformHandle = null;
            this.mObj = null;
        }
        if (m_bMyself && m_LauncherVo != null) {
            MtaManager.EndLauncherEvent(m_LauncherVo);
        }

	}

	public bool IsShutdown { get { return mIsShutDown;}}

	public bool Myself
	{
		get { return m_bMyself; }
		set { m_bMyself = value; }
	}


	public Vector3 skillPos{
		get { 
			return this.selfSeatUI.skillIcon.transform.position;
		}
	}
	public LauncherVo Vo
	{
		get { return m_LauncherVo;  }	
	}

	public Vector2 Direction
	{
		get { return m_Direction; }
		set { m_Direction = value; }
	}
	public Vector3 LauncherPos
	{
        get { return m_TransformHandle.position; }
	}


	public Vector3 CanonBaseLocalPos { get { return mLauncherBaseCon.localPosition;} }
	public Vector3 CanonBaseWorldPos { get { return mLauncherBaseCon.position;} }

	public Vector3 LauncherUIPos { get { return mLauncherBaseCon.localPosition + m_TransformHandle.localPosition;}}
	public Vector3 GoldWorldPos
	{
		get { return mGoldWorldPos;}
	}

    public void OnButtonClick(GameObject obj) {
        if (goldIconTrans.parent.gameObject == obj) {//点击金币图标
            this.m_UserInfoPanel.ShowInfo(this.m_UserInfoPanel.mIsShow == false);
        }
    }
}