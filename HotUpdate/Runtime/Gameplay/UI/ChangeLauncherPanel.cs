using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeLauncherPanel
{
    private GameObject mParentInfo,lcritemPrefab;
	Item_Launcher selectedItemGo = null;
    private UISprite mSprFrame;
    List<Item_Launcher> lcrIconItemList = new List<Item_Launcher>();
	Transform mListCon;

    public void Init()
	{
		if (mParentInfo == null)
        {
            //Kubility.KAssetBundleManger.Instance.LoadGameObject (ResPath.UIPath + "ChangeLauncherUI", SceneObjMgr.Instance.UIPanelTransform.gameObject, delegate(SmallAbStruct obj) {
            //	mParentInfo = obj.MainGameObject;
            //	Initview ();
            //});

            mParentInfo = ResManager.LoadAndCreate(GameEnum.Fish_3D, ResPath.NewUIPath + "ChangeLauncherUI", SceneObjMgr.Instance.UIPanelTransform);
            Initview();
        }
        else
        {
			Initview ();
		}
	}

	void Initview(){
        lcritemPrefab = mParentInfo.transform.Find("ItemPrefab").gameObject;
        mListCon = mParentInfo.transform.Find("Grid");
        mSprFrame = mParentInfo.transform.Find("spr_frame").GetComponent<UISprite>();
		lcritemPrefab.SetActive (false);
		mParentInfo.SetActive (false);

        WndManager.Instance.SetPanelSort(this.mParentInfo);
        //this.mParentInfo.GetComponent<UIPanel> ().depth = 5;
	}

	bool mIsAutoHide = false;
    private float _hide_time;
	public void Update(float delta)
	{
        if (this.mShowLCR && mIsAutoHide) {
            if (Time.realtimeSinceStartup > _hide_time) {
                this.HideLCR();
            }
        }
	}

	public void Shutdown()
	{
		selectedItemGo = null;
		lcrIconItemList.Clear ();
		if (mParentInfo != null)
			GameObject.Destroy (mParentInfo.gameObject);
		mParentInfo = null;
	}

    private const float mAnimTime = 0.3f;
    public bool mShowLCR = false;
    public void ShowLCR(bool autoHide = true) {
		RefreshDatas ();
        this.mShowLCR = true;
		mIsAutoHide = autoHide;
        _hide_time = Time.realtimeSinceStartup + 5;
        this.mParentInfo.SetActive(true);
        this.mParentInfo.transform.localScale = Vector3.zero;
        TweenScale.Begin(this.mParentInfo, mAnimTime, Vector3.one);
        this.mParentInfo.transform.localPosition = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.LauncherUIPos;
        TweenPosition.Begin(this.mParentInfo, mAnimTime, SceneLogic.Instance.PlayerMgr.MySelf.Launcher.LauncherUIPos + Vector3.up * 318f);
    }

    public void HideLCR(float time = mAnimTime) {
        mIsAutoHide = true;
        this.mShowLCR = false;
        if (time > 0) {
			TweenScale.Begin(this.mParentInfo, time, Vector3.zero);
        } else {
            this.mParentInfo.transform.localScale = Vector3.zero;
        }
        TweenPosition.Begin(this.mParentInfo, time, SceneLogic.Instance.PlayerMgr.MySelf.Launcher.LauncherUIPos);
    }

    void HandleLcrItemClick(Item_Launcher go) {
        if (SceneLogic.Instance.IsLOOKGuster) {
            return;
        }
		if (selectedItemGo != go) {
            SetSelected(go);
            byte level = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.Vo.Level;
            FishNetAPI.Instance.ChangeLauncher(go.mCfgID, level);
            SceneLogic.Instance.PlayerMgr.MySelf.Launcher.IsSettingLCR = true;
            _hide_time = Time.realtimeSinceStartup + 5;
            this.HideLCR();
		}
	}

    void RefreshDatas() {
        uint selectedLcrCfgID = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.Vo.LrCfgID;
        uint[] lcr_ids = SceneLogic.Instance.RoomVo.Launchers;// RoleInfoModel.Instance.GetAvaibleLauncherList();
        int i;
        Item_Launcher lcrItem = null;
        for (i = 0; i < lcr_ids.Length; i++) {
            if (i >= lcrIconItemList.Count) {
                lcrItem = UIItem.CreateItem<Item_Launcher>(lcritemPrefab, this.mListCon);
                lcrItem.gameObject.SetActive(true);
                lcrItem.SetCall(HandleLcrItemClick);
                lcrIconItemList.Add(lcrItem);
            } else {
                lcrItem = lcrIconItemList[i];
                lcrItem.gameObject.SetActive(true);
            }
            lcrItem.InitData(lcr_ids[i]);
            if (lcr_ids[i] == selectedLcrCfgID) {
                this.SetSelected(lcrItem);
                lcrItem.SetSelect(true);
            } else {
                lcrItem.SetSelect(false);
            }
        }
        while (i < lcrIconItemList.Count) {
            GameObject.Destroy(lcrIconItemList[i].gameObject);
            lcrIconItemList.RemoveAt(i);
        }

        this.mSprFrame.width = lcrIconItemList.Count * 200 + 100;
        mListCon.GetComponent<UIGrid>().Reposition();
    }

	void SetSelected(Item_Launcher item) {
        if (selectedItemGo != null) {
            selectedItemGo.SetSelect(false);
        }
        selectedItemGo = item;
        if (selectedItemGo != null) {
            selectedItemGo.SetSelect(true);
        }
	}
}
