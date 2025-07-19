using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GuideStepData {
	public const byte EventTipByte = 3;
	public const int TipStartId = 100;
    public GuideEventType EventType;
	public int stepID;
	public string msgContent;
	public Vector3 dialogPosition,fingerPos;
	public Vector4 holeRect = Vector4.zero;
	public float ArrowRange;
	public int   ArrowDir;
	public bool IsShowNPC = false;
	public Vector2 textMaxSize = Vector2.zero;

}

[System.Serializable]
public class GuideTipData{
	public float ArrowRange;
	public int   ArrowDir;
	public Vector2 textMaxSize = Vector2.zero;
}
public enum GuideEventType {
    [Header("空")]
    None,
    [Header("显示炮台列表")]
    ShowLCTList,
    [Header("隐藏炮台列表")]
    HideLCTList,
    [Header("显示炮台倍率列表")]
    ShowRateList,
    [Header("隐藏炮台倍率列表")]
    HideRateList,
    [Header("显示英雄列表")]
    ShowHeroList,
    [Header("隐藏英雄列表")]
    HideHeroList,
    [Header("显示快捷购买")]
    ShowQuickBuy,
    [Header("隐藏快捷购买")]
    HideQuickBuy,
    [Header("显示退出界面")]
    ShowExit,
    [Header("隐藏退出界面")]
    HideExit,
    [Header("显示设置界面")]
    ShowSetting,
    [Header("隐藏设置界面")]
    HideSetting,
    [Header("显示图鉴-特殊鱼")]
    ShowTuJian_TeShu,
    [Header("显示图鉴-普通鱼")]
    ShowTuJian_PuTong,
    [Header("显示图鉴-炮台")]
    ShowTuJian_PaoTai,
    [Header("隐藏图鉴界面")]
    HideTuJian,
    [Header("显示帮助按钮")]
    ShowHelp,
    [Header("隐藏帮助按钮")]
    HideHelp,
    [Header("显示菜单组")]
    ShowMenuList,
    [Header("隐藏菜单组")]
    HideMenuList,
}
public class GuideMgr : SingleTon<GuideMgr> {

	public bool isNeedGuide = true;

	const int MaxConditionGuideLength = 4;
	UI_GuideTips mGuideTips = null;
	List<GuideStepData> mGuideStepList = new List<GuideStepData>();
	List<GuideStepData> mGuideTipDataList;
	// Use this for initialization

	public void GlobalInit(){
        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_Energy) {//正常房间模式才显示引导功能
            isNeedGuide = LocalSaver.GetData("IsNeedGuide", 0) == 0;
        } else {
            isNeedGuide = false;
        }
        //if (isNeedGuide)
        //    PlayerPrefs.DeleteAll ();
		CondtionGuideIDList = GameUtils.SplitIntNumberString<int>(LocalSaver.GetData ("CondtionGuideSteps",""));
		Start ();
	}

	void Start () {
        //MonoDelegate.Instance.StartCoroutine (startGuide ());

        if (PlayerPrefs.GetInt("version_help_tick", 0) != GameConfig.ClientVersionCode) {
            PlayerPrefs.SetInt("version_help_tick", (int)GameConfig.ClientVersionCode);
            PlayerPrefs.Save();

            WndManager.Instance.ShowUI(EnumUI.UI_Help);
        }
	}

    public void Reset() {
        isNeedGuide = true;
        MonoDelegate.Instance.StartCoroutine(startGuide());
    }
	
	public bool OnAppQuit(){
		if (isGuiding) {
			if (confirSkipConfrmUI != null) {
				WndManager.Instance.CloseUI (EnumUI.PromptMsgUI);
				confirSkipConfrmUI = null;
			} else {
				ShowConfirmPanel ();
			}
			return false;
		}
		return true;
	}

	public bool isGuiding
	{
		get { return mIsGuiding; }
	}

	public void ResetClickEvent()
	{
		UICamera.ChkGameObjCanClick = FilterClick;
	}

	bool mIsGuiding = false;
	GuideUIRef mGuideRef;
	int stepIdx = 0;
	int stepMax = 24;
	bool isCommGuideOver = false;
	List<int> CondtionGuideIDList = new List<int>();
	IEnumerator startGuide(){
        mIsGuiding = false;
        SceneLogic.Instance.LogicUI.HeroListUI.SetShowNullHero(true);
		while (SceneLogic.Instance.IsInitCompleted == false)
			yield return null;
		
		if (isNeedGuide == false)
        {
            if (CondtionGuideIDList.Count >= MaxConditionGuideLength)
            {
                SceneLogic.Instance.LogicUI.HeroListUI.SetShowNullHero(false);
				yield break;	
			}
            else
            {
                if (mGuideTips == null)
                {
                    //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.UIPath + "UI_GuideTips");
                    //ResManager.LoadAsset<GameObject>(data, (prefab) => 
                    //{
                    //    GameObject guideObj = GameUtils.CreateGo(prefab, SceneObjMgr.Instance.UIPanelTransform);
                    //    guideObj.AddComponent<ResCount>().ab_data = data;
                    //    mGuideTips = guideObj.GetComponent<UI_GuideTips>();
                    //});

                    GameObject guideObj = ResManager.LoadAndCreate(GameEnum.Fish_3D, ResPath.NewUIPath + "UI_GuideTips", SceneObjMgr.Instance.UIPanelTransform);
                    mGuideTips = guideObj.GetComponent<UI_GuideTips>();
                }

				while (mGuideTips == null)
					yield return null;
				mGuideTipDataList = FishConfig.Instance.mGuideStepsConf.TryGet(GuideStepData.EventTipByte);
				mGuideTips.gameObject.SetActive (false);
				isCommGuideOver = true;
				RegisterConditionGuides ();
				Vector3 wwp = Vector3.zero;
				if (SceneLogic.Instance.WorldBossMgr.GetActionBtnPos (ref wwp))
				{
					OnEventWorldBossStart(wwp);
                }
                SceneLogic.Instance.LogicUI.HeroListUI.SetShowNullHero(false);
				yield break;
			}
		}
		mIsGuiding = true;
        if (mGuideRef == null)
        {
            //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.UIPath + "UI_Guide");
            //ResManager.LoadAsset<GameObject>(data, (prefab) =>
            //{
            //    GameObject guideObj = GameUtils.CreateGo(prefab, SceneObjMgr.Instance.UIPanelTransform);
            //    guideObj.AddComponent<ResCount>().ab_data = data;
            //    mGuideRef = guideObj.GetComponent<GuideUIRef>();
            //});

            GameObject guideObj = ResManager.LoadAndCreate(GameEnum.Fish_3D, ResPath.NewUIPath + "UI_Guide", SceneObjMgr.Instance.UIPanelTransform);
            mGuideRef = guideObj.GetComponent<GuideUIRef>();
        }
		while (mGuideRef == null)
			yield return null;

        if (mGuideTips == null)
        {
            //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.UIPath + "UI_GuideTips");
            //ResManager.LoadAsset<GameObject>(data, (prefab) =>
            //{
            //    GameObject guideObj = GameUtils.CreateGo(prefab, SceneObjMgr.Instance.UIPanelTransform);
            //    guideObj.AddComponent<ResCount>().ab_data = data;
            //    mGuideTips = guideObj.GetComponent<UI_GuideTips>();
            //});
            GameObject guideObj = ResManager.LoadAndCreate(GameEnum.Fish_3D, ResPath.NewUIPath + "UI_GuideTips", SceneObjMgr.Instance.UIPanelTransform);
            mGuideTips = guideObj.GetComponent<UI_GuideTips>();
        }

        while (mGuideTips == null)
            yield return null;
		mGuideTips.gameObject.SetActive (false);
		SceneObjMgr.Instance.SceneTopCamera.gameObject.SetActive (false);
		isCommGuideOver = false;
		mGuideStepList = FishConfig.Instance.mGuideStepsConf.TryGet(SceneLogic.Instance.FModel.SelfClientSeat);
		mGuideTipDataList = FishConfig.Instance.mGuideStepsConf.TryGet(GuideStepData.EventTipByte);
		RegisterConditionGuides ();
		ResetClickEvent ();
		UIEventListener.Get(mGuideRef.skipBtn).onClick = onSkipBtnHandle;
		UIEventListener.Get(mGuideRef.bgMask.gameObject).onClick = StepNextHandle;
		int lastStepIdx = -1;
		GInput.Enabled = false;
		confirSkipConfrmUI = null;
		stepMax = mGuideStepList.Count;
		while (stepIdx < stepMax) {
			if (Input.GetKeyUp(KeyCode.Escape)) {//退出游戏
				if (confirSkipConfrmUI != null) {
					WndManager.Instance.CloseUI (EnumUI.PromptMsgUI);
					confirSkipConfrmUI = null;
				} else {
					ShowConfirmPanel ();
				}
			}
				
			if (lastStepIdx == stepIdx) {
				yield return 0f;
				continue;
			}
			lastStepIdx = stepIdx;
            this.ExecEventType(mGuideStepList[stepIdx].EventType);
			if (mGuideStepList [stepIdx].textMaxSize != Vector2.zero) {
				mGuideRef.dialogText.width = (int)Mathf.Max(mGuideStepList [stepIdx].textMaxSize.x, 400f);
				mGuideRef.dialogText.height = (int)Mathf.Max(mGuideStepList [stepIdx].textMaxSize.y, 90f);
			}
			mGuideRef.npc.SetActive (mGuideStepList [stepIdx].IsShowNPC);
			mGuideRef.titleLabel.gameObject.SetActive (mGuideStepList [stepIdx].IsShowNPC);
			mGuideRef.ShowMsg (StringTable.GetString (string.Format("GuideTips{0}", stepIdx)));
			mGuideRef.setFingerPos (mGuideStepList [stepIdx].fingerPos);
			mGuideRef.setHoleRect (mGuideStepList [stepIdx].holeRect);
			mGuideRef.arrowDirect = mGuideStepList [stepIdx].ArrowDir;
			mGuideRef.range = mGuideStepList [stepIdx].ArrowRange;
			mGuideRef.setDialogPosition (mGuideStepList [stepIdx].dialogPosition);
			yield return 0f;
		}
		SceneObjMgr.Instance.SceneTopCamera.gameObject.SetActive (true);
		GameObject.Destroy (mGuideRef.gameObject);
		mGuideRef = null;
		UICamera.ChkGameObjCanClick = null;
		GInput.Enabled = true;
		isCommGuideOver = true;
		mIsGuiding = false;
		LocalSaver.SetData ("IsNeedGuide", 1);
        LocalSaver.Save();

		Vector3 wp = Vector3.zero;
		if (SceneLogic.Instance.WorldBossMgr.GetActionBtnPos (ref wp))
		{
			OnEventWorldBossStart(wp);
        }
        SceneLogic.Instance.LogicUI.HeroListUI.SetShowNullHero(false);
	}

    private void ExecEventType(GuideEventType event_type) {
        switch (event_type) {
            case GuideEventType.ShowLCTList://显示炮台列表
                SceneLogic.Instance.LogicUI.ShowLCRList(false);
                break;
            case GuideEventType.HideLCTList://隐藏炮台列表
                SceneLogic.Instance.LogicUI.HideLCRList();
                break;
            case GuideEventType.ShowRateList://显示炮台倍率列表
                SceneLogic.Instance.PlayerMgr.MySelf.Launcher.ShowRateList(true);
                break;
            case GuideEventType.HideRateList://隐藏炮台倍率列表
                SceneLogic.Instance.PlayerMgr.MySelf.Launcher.ShowRateList(false);
                break;
            case GuideEventType.ShowHeroList://显示英雄列表
                SceneLogic.Instance.LogicUI.HeroListUI.ShowHeroList(false);
                break;
            case GuideEventType.HideHeroList://隐藏英雄列表
                SceneLogic.Instance.LogicUI.HeroListUI.HideHeroList();
                break;
            case GuideEventType.ShowQuickBuy://显示快捷购买
                WndManager.Instance.ShowUI(EnumUI.QuickBuyUI, SceneLogic.Instance.RoomVo.Items[0]);
                break;
            case GuideEventType.HideQuickBuy://隐藏快捷购买
                WndManager.Instance.CloseUI(EnumUI.QuickBuyUI);
                mGuideRef.selectFrameSp.gameObject.SetActive(false);
                break;
            case GuideEventType.ShowExit://显示退出界面
                this.ShowBtnMenu();
                WndManager.Instance.ShowUI(EnumUI.UI_ExitGame);
                break;
            case GuideEventType.HideExit://隐藏退出界面
                WndManager.Instance.CloseUI(EnumUI.UI_ExitGame);
                break;
            case GuideEventType.ShowSetting://显示设置界面
                WndManager.Instance.ShowUI(EnumUI.UI_Setting);
                break;
            case GuideEventType.HideSetting://隐藏设置界面
                WndManager.Instance.CloseUI(EnumUI.UI_Setting);
                break;
            case GuideEventType.ShowTuJian_TeShu://显示图鉴-特殊鱼
                if (WndManager.Instance.isActive(EnumUI.UI_TuJian) == false)
                    WndManager.Instance.ShowUI(EnumUI.UI_TuJian, 0);
                WndManager.Instance.GetController<UI_TuJianController>().SetMenu(0);
                break;
            case GuideEventType.ShowTuJian_PuTong://显示图鉴-普通鱼
                if (WndManager.Instance.isActive(EnumUI.UI_TuJian) == false)
                    WndManager.Instance.ShowUI(EnumUI.UI_TuJian, 1);
                WndManager.Instance.GetController<UI_TuJianController>().SetMenu(1);
                break;
            case GuideEventType.ShowTuJian_PaoTai://显示图鉴-炮台
                if (WndManager.Instance.isActive(EnumUI.UI_TuJian) == false)
                    WndManager.Instance.ShowUI(EnumUI.UI_TuJian, 2);
                WndManager.Instance.GetController<UI_TuJianController>().SetMenu(2);
                break;
            case GuideEventType.HideTuJian://隐藏图鉴界面
                WndManager.Instance.CloseUI(EnumUI.UI_TuJian);
                break;
            case GuideEventType.ShowHelp://显示帮助
                this.ShowBtnMenu();
                WndManager.Instance.ShowUI(EnumUI.UI_Help);
                break;
            case GuideEventType.HideHelp://隐藏帮助
                WndManager.Instance.CloseUI(EnumUI.UI_Help);
                break;
            case GuideEventType.ShowMenuList://显示菜单组
                this.ShowBtnMenu();
                break;
            case GuideEventType.HideMenuList://隐藏菜单组")]
                SceneLogic.Instance.LogicUI.SetShowBtnMenu(false);
                break;
        }
    }

    private void ShowBtnMenu() {//显示按钮列表
        SceneLogic.Instance.LogicUI.SetShowBtnMenu(true);
        SceneLogic.Instance.LogicUI._hide_btn_panel_time = float.MaxValue;
    }

	void StepNextHandle(GameObject go){
		stepIdx++;
	}

	PromptSysMessageController confirSkipConfrmUI = null;
	void ShowConfirmPanel(){
		confirSkipConfrmUI = (PromptSysMessageController)WndManager.Instance.ShowUI (EnumUI.PromptMsgUI, StringTable.GetString("confirmSkipGuide"), true);
		confirSkipConfrmUI.setPanelSortOrder (20);
		confirSkipConfrmUI.onConfirmCb = delegate() {
			stepIdx = mGuideStepList.Count;	
		};

		confirSkipConfrmUI.onCancelCb = delegate() {
			confirSkipConfrmUI = null;
		};
	}

	void onSkipBtnHandle(GameObject go){
		ShowConfirmPanel ();
	}

	public bool FilterClick(GameObject go){
		if (mGuideRef != null) {
			bool isGuideBtns = go == mGuideRef.bgMask.gameObject || go == mGuideRef.skipBtn;
			if (confirSkipConfrmUI != null && confirSkipConfrmUI.IsActive) {
				if (isGuideBtns)
					return false;
				return go.GetComponentInParent<UIPanel> ().sortingOrder > mGuideRef.GetComponent<UIPanel> ().sortingOrder;
			} else {
				return isGuideBtns;
			}
		}
		return false;
	}


	void RegisterConditionGuides(){
		if (!CondtionGuideIDList.Contains(0))
			SceneLogic.Instance.RegisterGlobalMsg (SysEventType.ItemDroped, OnEventFirstGetItem);
		if (!CondtionGuideIDList.Contains(1))
			SceneLogic.Instance.RegisterGlobalMsg (SysEventType.EngeryFirstFull, OnEventSkillEngeryFull);
		if (!CondtionGuideIDList.Contains(2))
			SceneLogic.Instance.RegisterGlobalMsg (SysEventType.QuickByKeyActive, OnEventQuickByKeyOpen);
		if (!CondtionGuideIDList.Contains(3))
			SceneLogic.Instance.RegisterGlobalMsg (SysEventType.WorldBossActStart, OnEventWorldBossStart);
	}

	void UnRegisterConditionGuides(){
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.ItemDroped, OnEventFirstGetItem);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.EngeryFirstFull, OnEventSkillEngeryFull);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.QuickByKeyActive, OnEventQuickByKeyOpen);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.WorldBossActStart, OnEventWorldBossStart);
	}

	void SaveCondtionGuideIDS (){
		string str = GameUtils.Join2String<int> (CondtionGuideIDList);
		LocalSaver.SetData ("CondtionGuideSteps", str);
	}

	void OnEventFirstGetItem(object data){
		if (isCommGuideOver == false)
			return;		
		Vector3 wp = SceneLogic.Instance.LogicUI.GetItemIconWorldPos ((uint)data);
		if (wp != Vector3.zero) {
			setGuideTipsLayoutParams (0);
			string strkey = string.Format("GuideTips{0}", GuideStepData.TipStartId+0);
			mGuideTips.Init (StringTable.GetString (strkey), wp, Vector3.up * 100f);
			CondtionGuideIDList.TryAdd (0);
			SaveCondtionGuideIDS ();
			SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.ItemDroped, OnEventFirstGetItem);
		}
	}

	void OnEventSkillEngeryFull(object data){
		if (isCommGuideOver == false)
			return;
		if (CondtionGuideIDList.Contains (1))
			return;	
		setGuideTipsLayoutParams (1);
		string strkey = string.Format("GuideTips{0}", GuideStepData.TipStartId+1);
		Vector3 wp =  SceneLogic.Instance.PlayerMgr.MySelf.Launcher.skillPos;
		mGuideTips.Init (StringTable.GetString (strkey), wp, Vector3.up*120f);

        if (SceneLogic.Instance.PlayerMgr.MyClientSeat == 0 || SceneLogic.Instance.PlayerMgr.MyClientSeat == 3) {
            mGuideTips.mFlipH = false;
        }else{
            mGuideTips.mFlipH = true;
        }

		CondtionGuideIDList.TryAdd (1);
		SaveCondtionGuideIDS ();
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.EngeryFirstFull, OnEventSkillEngeryFull);
	}

	void OnEventQuickByKeyOpen(object data){
		if (isCommGuideOver == false)
			return;
		if (CondtionGuideIDList.Contains (2))
			return;		
		setGuideTipsLayoutParams (2);
		string strkey = string.Format("GuideTips{0}", GuideStepData.TipStartId+2);
		Vector3 wp = (Vector3)data;
		mGuideTips.Init (StringTable.GetString (strkey), wp, Vector3.right*50f);
		CondtionGuideIDList.TryAdd (2);
		SaveCondtionGuideIDS ();
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.QuickByKeyActive, OnEventQuickByKeyOpen);
	}

    void OnEventWorldBossStart(object data) {
		if (isCommGuideOver == false)
			return;
		if (CondtionGuideIDList.Contains (3))
			return;
		setGuideTipsLayoutParams (3);
		string strkey = string.Format("GuideTips{0}", GuideStepData.TipStartId+3);
		Vector3 wp = (Vector3)data;
		mGuideTips.Init (StringTable.GetString (strkey), wp, Vector3.right*80f);
		CondtionGuideIDList.TryAdd (3);
		SaveCondtionGuideIDS ();
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.WorldBossActStart, OnEventWorldBossStart);
	}

	void setGuideTipsLayoutParams(int step){
		int tipIndex = step;
		mGuideTips.arrowDirect = mGuideTipDataList [tipIndex].ArrowDir;
		mGuideTips.range = mGuideTipDataList [tipIndex].ArrowRange;
		mGuideTips.dialogText.width = (int)mGuideTipDataList [tipIndex].textMaxSize.x;
		mGuideTips.dialogText.height = (int)mGuideTipDataList [tipIndex].textMaxSize.y;
	}

	public void Shutdown(){
		UnRegisterConditionGuides ();
		if (mGuideTips != null) {
			GameObject.Destroy (mGuideTips.gameObject);
			mGuideTips = null;
		}

		if (mGuideRef != null) {
			GameObject.Destroy (mGuideRef.gameObject);
			mGuideRef = null;
		}
	}
}
