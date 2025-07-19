using UnityEngine;
using System.Collections.Generic;
using Kubility;

public class SceneGameUIMgr : ISceneMgr
{
	
	public BattleUIRef mbattleUIRef;
	HeroListPanel heroListUI = new HeroListPanel();
	ItemListPanel skillListUI = new ItemListPanel();
	BossLifePanel bossLifeUI = new BossLifePanel();
	ChangeLauncherPanel changeLcrUI = new ChangeLauncherPanel();
	FishLockedController fishLockCtrl = new FishLockedController();
    EffXiuYuQi eff_xiuyuqi;//休渔期动画

    public Transform BattleUI {
        get {
            if (this.mbattleUIRef != null) {
                return this.mbattleUIRef.transform;
            } else {
                return SceneObjMgr.Instance.UIPanelTransform;
            }
        }
    }

	public void Init()
	{
		isLCRListEnabled = true;
		WndManager.LoadUIGameObject ("BattleUI", SceneObjMgr.Instance.UIPanelTransform, delegate(GameObject obj) {
			obj = GameUtils.ResumeShader(obj);
			mbattleUIRef = obj.GetComponent<BattleUIRef>();
            heroListUI.Init(mbattleUIRef.hero_list_content, mbattleUIRef.heroItemPrefab, mbattleUIRef.mBtnShowHeroList, mbattleUIRef.mObjItemListCollider);
            skillListUI.Init(mbattleUIRef.item_list_trans, mbattleUIRef.skillItemPrefab);
            bossLifeUI.Init(mbattleUIRef.bossTrans, mbattleUIRef.bossIcon, mbattleUIRef.bossHpBar, mbattleUIRef.mSprBossName, mbattleUIRef.mLbBOSSHPCount);
            changeLcrUI.Init();
            fishLockCtrl.Init();
            LotteryVo vo = FishConfig.Instance.mLotteryConf.TryGet(RoleInfoModel.Instance.RoomCfgID);
            if (vo != null && vo.IsOpen){// && RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_Energy) {//只有正常能量模式才开启奖劵信息
                mbattleUIRef.mBtnLottery.SetActive(true);
				mbattleUIRef.mBtnLottery.AddComponent<Item_Battle_Lottery>().InitData(vo);
            } else {
                mbattleUIRef.mBtnLottery.SetActive(false);
            }
            UIEventListener.Get(mbattleUIRef.mBtnSetting).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnAutoLaunch).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnAutoLock).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnTuJian).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnHelp).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnInsurance).onClick = this.OnButtonClick;
            //UIEventListener.Get(mbattleUIRef.mBtnSale).onClick = this.OnButtonClick;
            UIEventListener.Get(mbattleUIRef.mBtnBox).onClick = this.OnButtonClick;
			UIEventListener.Get(mbattleUIRef.mBtnExitGame).onClick = this.OnButtonClick;

            if (MainEntrace.IsWidthScreen) {//宽屏适配
                Transform left_parent = mbattleUIRef.mLeftParent;
                for (int i = 0; i < left_parent.childCount; i++) {
                    left_parent.GetChild(i).localPosition += new Vector3(70, 0, 0);
                }
                Transform right_parent = mbattleUIRef.mRightParent;
                for (int i = 0; i < right_parent.childCount; i++) {
                    right_parent.GetChild(i).localPosition += new Vector3(-70, 0, 0);
                }
            }

//			WndManager.Instance.Push(mbattleUIRef.gameObject);

            //mbattleUIRef.mBtnReset.gameObject.SetActive(false);
            //this.LoadBtnPos();

            this.RefershItemListPos();
			this.SetShowAllBtn(SceneLogic.iShowOnekey, false);
            this.SetAutoLock(false);
            this.layout();

            this.mbattleUIRef.gameObject.SetActive(false);
			TimeManager.DelayExec(anim_time, () => {
                this.mbattleUIRef.gameObject.SetActive(true);
                this.ShowUI();
            });
            SceneLogic.Instance.WorldBossMgr.SetView(this.mbattleUIRef.mBtnBox,this.mbattleUIRef.mEffBoxWork);

            UIEventListener.Get(mbattleUIRef.mBtnShowBtnMenu).onClick = (btn) => {
                this.SetShowBtnMenu(this.mIsShowBtnMenu == false);
            };
            this.SetShowBtnMenu(false);

		});
        //isLayout = false;
	}

    //bool isLayout = false;
	void layout(){
        if (RoleInfoModel.Instance.Self.ChairSeat == 1 || RoleInfoModel.Instance.Self.ChairSeat == 2) {
        //if (isLayout == false && SceneLogic.Instance.PlayerMgr.MyClientSeat > 0) {
			mbattleUIRef.mBtnAutoLaunch.transform.localPosition += Vector3.left * 358f;
			mbattleUIRef.mBtnAutoLock.transform.localPosition += Vector3.left * 358f;
		}
        //isLayout = true;
	}

    public float _hide_btn_panel_time = 0;
    private bool mIsShowBtnMenu = false;
    public void SetShowBtnMenu(bool is_show) {
        this.mIsShowBtnMenu = is_show;
        if (this.mIsShowBtnMenu == true) {
            TweenPosition.Begin(this.mbattleUIRef.mObjBtnParent, SceneGameUIMgr.anim_time, Vector3.zero);
            this.mbattleUIRef.mSprShowBtnMenu.flip = UIBasicSprite.Flip.Nothing;
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
        } else {
            TweenPosition.Begin(this.mbattleUIRef.mObjBtnParent, SceneGameUIMgr.anim_time, new Vector3(-150, 0));
            this.mbattleUIRef.mSprShowBtnMenu.flip = UIBasicSprite.Flip.Horizontally;
        }
    }

	/*
    private float _reset_hide_time;
    private void btn_drag_end() {//拖拽结束
        _reset_hide_time = Time.realtimeSinceStartup + 3;
        mbattleUIRef.mBtnReset.SetActive(true);
        this.SaveBtnPos();
    }
    private Dictionary<Transform, Vector3> dic_pos = new Dictionary<Transform, Vector3>();//按钮最初状态
    private void ResetBtnPos() {//还原按钮到最初状态
        foreach (var item in dic_pos) {
            item.Key.localPosition = item.Value;
            item.Key.localScale = Vector3.one;
        }
        this.SaveBtnPos();
    }
    private void LoadBtnPos() {//加载按钮位置
        DragDropItemAnim[] items = mbattleUIRef.GetComponentsInChildren<DragDropItemAnim>();
        Transform tran;
        Vector3 pos;
        for (int i = 0; i < items.Length; i++) {
            //items[i].restriction = DragDropItemAnim.Restriction.PressAndHold;
            //items[i].pressAndHoldDelay = 0.2f;
            tran = items[i].transform;
            pos = tran.localPosition;
            dic_pos.Add(tran, pos);
            pos.x = LocalSaver.GetData(string.Format("{0}_x", tran.name), pos.x);
            pos.y = LocalSaver.GetData(string.Format("{0}_y", tran.name), pos.y);
            pos.z = LocalSaver.GetData(string.Format("{0}_z", tran.name), pos.z);
            tran.localPosition = pos;
            items[i].event_drag_end = this.btn_drag_end;
        }
    }
    public void SaveBtnPos() {
        DragDropItemAnim[] items = mbattleUIRef.GetComponentsInChildren<DragDropItemAnim>();
        Transform tran;
        Vector3 pos;
        for (int i = 0; i < items.Length; i++) {
            tran = items[i].transform;
            pos = tran.localPosition;
            LocalSaver.SetData(string.Format("{0}_x", tran.name), pos.x);
            LocalSaver.SetData(string.Format("{0}_y", tran.name), pos.y);
            LocalSaver.SetData(string.Format("{0}_z", tran.name), pos.z);
        }
        LocalSaver.Save();
    }
	//*/


	public void ShowLCRList(bool autoHide){
		if (isLCRListEnabled == false)
			return;
        if (changeLcrUI.mShowLCR) {
            changeLcrUI.HideLCR();
        }else{
            if (SceneLogic.Instance.RoomVo.Launchers.Length > 1) {
                changeLcrUI.ShowLCR(autoHide);
            }
        }
	}

	public void HideLCRList(){
		changeLcrUI.HideLCR ();
	}

	bool isLCRListEnabled = true;
    public void SetLCRBtnEnable(bool is_enable) {//是否换炮按钮是否禁用
        changeLcrUI.HideLCR(0);
		isLCRListEnabled = is_enable;
		if (isLCRListEnabled == false)
			changeLcrUI.HideLCR ();	
    }

    public void RefershItemListPos() {//刷新技能道具位置
/*        if (SceneLogic.Instance.FModel.SelfClientSeat == 0) {
            mbattleUIRef.item_list_trans.localPosition = new Vector3(68, 0);
            mbattleUIRef.mBtnShowHeroList.transform.localPosition = new Vector3(250, 0);
            mbattleUIRef.item_list_trans.GetChild(0).localScale = new Vector3(1, 1, 1);

            this.heroListUI.RefershPos(210);
        } else {
            mbattleUIRef.item_list_trans.localPosition = new Vector3(-68, 0);
            mbattleUIRef.mBtnShowHeroList.transform.localPosition = new Vector3(-250, 0);
            mbattleUIRef.item_list_trans.GetChild(0).localScale = new Vector3(-1, 1, 1);

            this.heroListUI.RefershPos(-210);
        }

        TweenPosition tp = mbattleUIRef.item_list_trans.GetComponent<TweenPosition>();
        if (tp != null) {
            tp.from.x = mbattleUIRef.item_list_trans.localPosition.x;
            tp.to.x = mbattleUIRef.item_list_trans.localPosition.x;
        }
        */
    }

    public ItemListPanel ItemListUI {
        get { return skillListUI; }
    }
    public HeroListPanel HeroListUI {
        get { return heroListUI; }
    }

    public BossLifePanel BossLifeUI {
        get { return bossLifeUI; }
    }

    public void ShowXiuYuQi(float end_time) {//显示休渔期特效  time：显示倒计时
        if (SceneLogic.Instance.IsLOOKGuster == false && end_time > Time.realtimeSinceStartup) {
            if (SceneLogic.Instance.mIsXiuYuQi == false) {
                ScenePlayer sp;
                for (byte i = 0; i < ConstValue.SEAT_MAX; i++) {
                    if (i != SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                        sp = SceneLogic.Instance.PlayerMgr.GetPlayer(i);
                        if (sp != null) {
                            sp.Launcher.SeatTransform.gameObject.SetActive(false);
                        }
                    }
                }
                SceneLogic.Instance.HeroMgr.SetOtherHeroShow(false);
            }
            SceneLogic.Instance.mIsXiuYuQi = true;

            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/anim_xiuyuqi", (ab_data, obj) => {
                obj = GameUtils.CreateGo(obj, SceneObjMgr.Instance.UIPanelTransform);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                eff_xiuyuqi = obj.AddComponent<EffXiuYuQi>();
                eff_xiuyuqi.InitData(end_time);
            }, GameEnum.Fish_3D);
        }
    }
    public void HideXiuYuQi() {//隐藏休渔期特效
        if (SceneLogic.Instance.mIsXiuYuQi) {
            ScenePlayer sp;
            for (byte i = 0; i < ConstValue.SEAT_MAX; i++) {
                if (i != SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                    sp = SceneLogic.Instance.PlayerMgr.GetPlayer(i);
                    if (sp != null) {
                        sp.Launcher.SeatTransform.gameObject.SetActive(true);
                        sp.Launcher.Show();
                    }
                }
            }
            SceneLogic.Instance.HeroMgr.SetOtherHeroShow(true);
        }
        SceneLogic.Instance.mIsXiuYuQi = false;
        if (eff_xiuyuqi != null) {
            eff_xiuyuqi.Hide();
            eff_xiuyuqi = null;
        }
    }

    private GameObject _eff_auto_launch = null;//自动锁定标识特效
    public void SetAutoLaunch(bool is_auto) {//设置自动发射开关
        if (SceneLogic.Instance.IsLOOKGuster) {
            return;
        }
        SceneLogic.Instance.PlayerMgr.SetAutoShot(is_auto);
        if (is_auto) {
            if (this._eff_auto_launch == null) {
                this._eff_auto_launch = GameUtils.CreateGo(FishResManager.Instance.mEffAutoBtn, this.mbattleUIRef.mBtnAutoLaunch.transform);
                this._eff_auto_launch.transform.localPosition = new Vector3(-2, 6, 0);
            }
            this._eff_auto_launch.SetActive(true && SceneLogic.iShowOnekey);
        } else {
            if (this._eff_auto_launch != null) {
                this._eff_auto_launch.SetActive(false);
            }
        }
    }
    private GameObject _eff_auto_lock = null;//自动锁定标识特效
    public void SetAutoLock(bool is_auto) {//设置自动锁定
        if (SceneLogic.Instance.IsLOOKGuster) {
            return;
        }
        SceneLogic.Instance.PlayerMgr.AutoLocked = is_auto;
        if (is_auto) {
            if (this._eff_auto_lock == null) {
                this._eff_auto_lock = GameUtils.CreateGo(FishResManager.Instance.mEffAutoBtn, this.mbattleUIRef.mBtnAutoLock.transform);
                this._eff_auto_lock.transform.localPosition = new Vector3(-2, 6, 0);
            }
            this._eff_auto_lock.SetActive(true && SceneLogic.iShowOnekey);
        } else {
            if (this._eff_auto_lock != null) {
                this._eff_auto_lock.SetActive(false);
            }
        }
    }

    private void OnButtonClick(GameObject obj) {
        GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.BtnClick,type:GameEnum.All);
        if (this.mbattleUIRef.mBtnSetting == obj) {//设置界面
            if (WndManager.Instance.isActive(EnumUI.UI_Setting) == false) {
                WndManager.Instance.ShowUI(EnumUI.UI_Setting);
            } else {
                WndManager.Instance.CloseUI(EnumUI.UI_Setting);
            }
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
        } else if (this.mbattleUIRef.mBtnTuJian == obj) {//图鉴
            if (WndManager.Instance.isActive(EnumUI.UI_TuJian) == false) {
                WndManager.Instance.ShowUI(EnumUI.UI_TuJian);
            } else {
                WndManager.Instance.CloseUI(EnumUI.UI_TuJian);
            }
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
        } else if (this.mbattleUIRef.mBtnAutoLaunch == obj) {//自动发射
            this.SetAutoLaunch(SceneLogic.Instance.PlayerMgr.AutoShot == false);
        } else if (this.mbattleUIRef.mBtnAutoLock == obj) {//自动锁定
            this.SetAutoLock(SceneLogic.Instance.PlayerMgr.AutoLocked == false);
        } else if (this.mbattleUIRef.mBtnHelp == obj) { // 帮助界面
            if (WndManager.Instance.isActive(EnumUI.UI_Help) == false) {
                WndManager.Instance.ShowUI(EnumUI.UI_Help);
            } else {
                WndManager.Instance.CloseUI(EnumUI.UI_Help);
            }
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
		} else if (this.mbattleUIRef.mBtnExitGame == obj) {
            ShowExitGameUI();
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
        } else if (this.mbattleUIRef.mBtnBox == obj) {//全服宝箱排名
            FishNetAPI.Instance.SendWorldBOSSRank();

        } else if (this.mbattleUIRef.mBtnInsurance == obj) {//保险箱
            if (RoleInfoModel.Instance.CoinMode == EnumCoinMode.Score) {//积分模式下保险箱功能不可用
                SystemMessageMgr.Instance.DialogShow("Tip_22", null);
            } else {
                //UI.EnterUI<UI_safebox_new>(ui => {
                //    ui.InitData();
                //});

                UI.EnterUI<UI_safebox_new>(GameEnum.All).InitData();
            }
            _hide_btn_panel_time = Time.realtimeSinceStartup + 5;
        }
    }

    public static float ui_hide_finish_time;//界面隐藏完成时间
    public const float anim_time = 0.3f;//隐藏动画时间
    public void HideUI() {//隐藏界面
        ui_hide_finish_time = Time.realtimeSinceStartup + anim_time;
        TweenPosition.Begin(this.mbattleUIRef.bossTrans.gameObject, anim_time, new Vector3(0, 80), false);
        this.heroListUI.HideHeroList();

        this.mbattleUIRef.item_list_trans.gameObject.SetActive(true);
        Vector3 pos = this.mbattleUIRef.item_list_trans.localPosition;
        pos.x = 84f;
        TweenPosition tp = this.mbattleUIRef.item_list_trans.gameObject.GetComponent<TweenPosition>();
        if (tp != null && tp.enabled == true) {
            pos.y = tp.to.y;
        }
        TweenPosition.Begin(this.mbattleUIRef.item_list_trans.gameObject, anim_time, pos, false);
    }

    public void ShowUI() {//显示界面
        SceneGameUIMgr.ui_hide_finish_time = 0;
        this.mbattleUIRef.bossTrans.localPosition = new Vector3(0, 80);
        TweenPosition.Begin(this.mbattleUIRef.bossTrans.gameObject, anim_time, new Vector3(0, -80), false);

        Vector3 pos = this.mbattleUIRef.item_list_trans.localPosition;
        pos.x = 84f;
        this.mbattleUIRef.item_list_trans.localPosition = pos;

        TweenPosition tp = this.mbattleUIRef.item_list_trans.gameObject.GetComponent<TweenPosition>();
        if (tp != null && tp.enabled == true) {
            pos.y = tp.to.y;
        }
        pos.x = -84f;
        TweenPosition.Begin(this.mbattleUIRef.item_list_trans.gameObject, anim_time, pos, false);
    }

	public void SetShowAllBtn(bool is_show, bool isTween = true) {
		SceneLogic.iShowOnekey = is_show;
        List<GameObject> list = new List<GameObject>();
        list.Add(this.mbattleUIRef.mBtnAutoLaunch);
        list.Add(this.mbattleUIRef.mBtnAutoLock);
        list.Add(this.mbattleUIRef.mBtnSetting);
        list.Add(this.mbattleUIRef.mBtnTuJian);
        //list.Add(this.mbattleUIRef.item_list_trans.gameObject);

        if (is_show) {
            for (int i = 0; i < list.Count; i++) {
				var w = list [i].GetComponent<UIWidget> ();
				if (w.alpha < 1f){
                	w.alpha = 0f;
				}
				if (isTween)
					TweenAlpha.Begin (list [i], anim_time, 1);
				else
					w.alpha = 1f;
            }
            if (_eff_auto_launch != null) {
                _eff_auto_launch.SetActive(SceneLogic.Instance.PlayerMgr.AutoShot && SceneLogic.iShowOnekey);
            }
            if (_eff_auto_lock != null) {
                _eff_auto_lock.SetActive(SceneLogic.Instance.PlayerMgr.AutoLocked && SceneLogic.iShowOnekey);
            }
        } else {
            for (int i = 0; i < list.Count; i++) {
				var w = list [i].GetComponent<UIWidget> ();
				if (w.alpha > 0f) {
					w.alpha = 1f;
				}
				if (isTween)
					TweenAlpha.Begin (list [i], anim_time, 0f);
				else
					w.alpha = 0f;
            }
            if (_eff_auto_launch != null) {
                _eff_auto_launch.SetActive(false);
            }
            if (_eff_auto_lock != null) {
                _eff_auto_lock.SetActive(false);
            }
        }
        ScenePlayer[] players = SceneLogic.Instance.PlayerMgr.PlayerList;
        for (int i = 0; i < players.Length; i++) {
            if (players[i] != null) {
                players[i].Launcher.ShowUserInfo(is_show);
            }
        }
    }

	public void Shutdown()
	{
        if (mbattleUIRef != null) {
            this.HideUI();
            GameObject.Destroy(mbattleUIRef.gameObject, anim_time);
        }
		mbattleUIRef = null;

		heroListUI.Shutdown ();
		skillListUI.Shutdown ();
		bossLifeUI.Shutdown ();
		changeLcrUI.Shutdown ();
		fishLockCtrl.Shutdown ();
		if (eff_xiuyuqi!=null)
			eff_xiuyuqi.Shutdown ();
		eff_xiuyuqi = null;
	}

	public void ShowClearSceneMsg()
	{
		if (SceneLogic.Instance != null)
			return;
//		SystemMessageMgr.Instance.ShowSystemTips(StringTable.GetString("clearmsg"), ConstValue.CLEAR_TIME, false);
	}

    public void Update(float delta) {

        //layout ();
        //if (GInput.GetKeyDown(KeyCode.F7)) {
        //    this.SetAutoLaunch(SceneLogic.Instance.PlayerMgr.AutoShot == false);
        //}

        //if (GInput.GetKey(KeyCode.S)) {
        //    this.SetAutoLock(true);
        //}

        //if (GInput.GetKeyUp (KeyCode.S)) {
        //    SceneLogic.Instance.PlayerMgr.SwitchLockTarget ();
        //}

        //if (GInput.GetKeyUp(KeyCode.Q)) {
        //    this.SetAutoLock(false);
        //}

        //if (GInput.GetKeyUp(KeyCode.F9)) {
        //    this.SetShowAllBtn(SceneLogic.iShowOnekey == false);
        //}

        //if (GInput.GetKeyUp(KeyCode.F4)) {//设置界面
        //    if (WndManager.Instance.isActive(EnumUI.UI_Setting) == false) {
        //        WndManager.Instance.ShowUI(EnumUI.UI_Setting);
        //    } else {
        //        WndManager.Instance.CloseUI(EnumUI.UI_Setting);
        //    }
        //}
        //if (GInput.GetKeyUp(KeyCode.F8)) {//帮助界面
        //    if (WndManager.Instance.isActive(EnumUI.UI_TuJian) == false) {
        //        WndManager.Instance.ShowUI(EnumUI.UI_TuJian);
        //    } else {
        //        WndManager.Instance.CloseUI(EnumUI.UI_TuJian);
        //    }
        //}
		if (GInput.GetKeyUp(KeyCode.Escape)) {//退出游戏
            if (SceneLogic.Instance.IsInitCompleted == true) {
                IUIController ui = WndManager.Instance.GetCurActive();
                if (ui != null) {
                    ui.Close();
                } else {
                    ShowExitGameUI();
                }
            }
        }
        //if (GInput.GetKeyDown(KeyCode.F)) {//炮台技能释放
        //    SceneLogic.Instance.PlayerMgr.MySelf.Launcher.HandleScaterSkill(null);
        //}
        
        //foreach (var k in ArrowAry) {
        //    if (GInput.GetKeyDown(k)) {
        //        int i = System.Array.IndexOf (ArrowAry, k);
        //        SceneLogic.Instance.PlayerMgr.MySelf.Launcher.OperatLaunch (i);
        //    }
        //}

		skillListUI.Update (delta);
		heroListUI.Update (delta);
		bossLifeUI.Update (delta);
		fishLockCtrl.Update (delta);
        changeLcrUI.Update(delta);

//        if (mbattleUIRef.mBtnReset.activeSelf && _reset_hide_time < Time.realtimeSinceStartup) {
//          mbattleUIRef.mBtnReset.SetActive(false);
        //        }


        _head_time += delta;
        if (_head_time > 10) {//每隔10秒发送一次心跳包
            _head_time = 0;
            //FishNetAPI.Instance.isOpenClockSync = true;
            FishNetAPI.Instance.SendClockSync();
        }

        if (this.mIsShowBtnMenu == true && Time.realtimeSinceStartup > this._hide_btn_panel_time) {
            this.SetShowBtnMenu(false);
        }

        //if (changeLcrUI.mShowLCR && _check_lcr_time < Time.realtimeSinceStartup && GInput.GetMouseButtonUp(0)) {
        //    changeLcrUI.HideLCR();
        //}
    }
    private float _head_time;//心跳包时间

	public void OnUsedItemSucc(uint itemCfgId)
	{
		skillListUI.OnUseSkillItem(itemCfgId);
	}
    public void OnUserdItemFail(uint item_id, byte error_code) {
        skillListUI.OnUseItemFail(item_id, error_code);
    }

	public Vector3 GetItemIconWorldPos(uint itemID)
	{
		Vector3 uiWorldPos = Vector3.zero;
		if (skillListUI.GetIconPos (itemID, out uiWorldPos)) {
			return uiWorldPos;
		}

		if (heroListUI.GetIconPos (itemID, out uiWorldPos)) {
            return this.mbattleUIRef.mBtnShowHeroList.transform.position;
            //return uiWorldPos;
		}			
		return Vector3.zero;
	}

	void ShowExitGameUI(){
		if (SceneLogic.Instance.IsLOOKGuster == false) {
            WndManager.Instance.ShowUI(EnumUI.UI_ExitGame);
		}
	}
}