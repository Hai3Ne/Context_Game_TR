using System;
using UnityEngine;
using System.Collections.Generic;

public class HeroListPanel
{
    List<HeroItemRender> heroListRenders = new List<HeroItemRender>();
    private Transform mHeroListContent;
    private GameObject mBtnShowList;
    private GameObject mObjItemListCollider;//道具技能碰撞区域
    private GameObject item_hero_prefab;
	uint[] heroItemIDList = null;
	bool mIsAutoHide = false;
    KeyCode[] hero_arr = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6 };
    private UIGrid mGridHeroList;
    private bool mIsShowNullHero = false;//是否显示不拥有的英雄图标
	public void Init(Transform listCon, GameObject itemPrefab,GameObject btn_show_hero,GameObject collider)
	{
        this.mHeroListContent = listCon;
        this.mBtnShowList = btn_show_hero;
        this.mObjItemListCollider = collider;
        this.item_hero_prefab = itemPrefab;

		TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet (SceneLogic.Instance.GetRoomCfgID());
        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_NotItem) {
            heroItemIDList = new uint[0];
        } else {
            heroItemIDList = roomVo.Heroes;
        }
		int itemCnt = heroItemIDList.Length;
		heroListRenders.Clear ();

        itemPrefab.SetActive(false);
        this.mGridHeroList = listCon.GetComponent<UIGrid>();
		for (int i = 0; i < itemCnt; i++) {
            HeroItemRender heroRender = GameUtils.CreateGo(itemPrefab, listCon).GetComponent<HeroItemRender>();
            heroListRenders.Add(heroRender);
			UIEventListener.Get (heroRender.gameObject).onClick = HandleClickItem;
        }
        this.mGridHeroList.Reposition();
        UIEventListener.Get(this.mBtnShowList).onClick = OnButtonClick;

		RoleItemModel.Instance.RegisterGlobalMsg (SysEventType.ItemInfoChange, HandleItemChange);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.SceneHeroLeave, HandleHeroOnLeave);
        for (int i = 0; i < heroListRenders.Count; i++) {
            heroListRenders[i].maskObj.gameObject.SetActive(false);
        }

		UpdateData ();

        this.HideHeroList();
        //this.ShowHeroList(true);
        //this.mIsShowList = false;
        ////this.mHeroListContent.localScale = new Vector3(0, 1);
        //this.mHeroListContent.localPosition = Vector3.zero;
        //mIsAutoHide = true;
        ////this.mListHideTime = Time.realtimeSinceStartup + 5;
	}

    public void SetShowNullHero(bool is_show) {
        this.mIsShowNullHero = is_show;
        this.RefershBtnShow();
    }
	public void ShowHeroList(bool autoHide = true) {
        this.mIsShowList = true;
		this.mIsAutoHide = autoHide;
        this.mListHideTime = Time.realtimeSinceStartup + 5;
        //TweenScale.Begin(this.mHeroListContent.gameObject, SceneGameUIMgr.anim_time, new Vector3(1, 1));

        int show_count = 0;
        foreach (var item in heroListRenders) {
            if (item.gameObject.activeSelf) {
                show_count = 0;
            }
        }
        this.mHeroListContent.localPosition = Vector3.zero + new Vector3(show_count * 118, 0);
        TweenPosition.Begin(this.mHeroListContent.gameObject, SceneGameUIMgr.anim_time, Vector3.zero);
        this.RefershBtnShow();
    }
    public void HideHeroList() {
        this.mIsShowList = false;
		this.mIsAutoHide = true;
        //TweenScale.Begin(this.mHeroListContent.gameObject, SceneGameUIMgr.anim_time, new Vector3(0, 1));
        TweenPosition.Begin(this.mHeroListContent.gameObject, SceneGameUIMgr.anim_time, new Vector3(800, 0));
        this.RefershBtnShow();
    }

    private bool mIsShowList = false;
    public float mListHideTime;//英雄道具技能菜单隐藏时间
    private void OnButtonClick(GameObject obj) {
        if (this.mBtnShowList == obj) {
            if (this.mIsShowList) {
                this.HideHeroList();
            } else {
                this.ShowHeroList();
            }
        } else if (this.mMaxHero != null && this.mMaxHero.gameObject == obj) {
            if (this.mIsShowList) {
                this.HideHeroList();
            } else {
                this.ShowHeroList();
            }
        }
    }

    public void RefershPos(float x) {//刷新英雄位置
        this.mHeroListContent.transform.localPosition = new Vector3(x, 192);
    }

	public void OnClickItem(int idx)
	{
        if (heroListRenders.Count > 0) {
            HandleClickItem(heroListRenders[idx].gameObject);
        }
	}

	void HandleItemChange(object args)
	{
		UpdateData ();
	}

	void HandleHeroOnLeave(object args){
		ResetHeroCD ();
	}

	public bool GetIconPos(uint itemCfgID, out Vector3 uiWorldPos)
	{
		uiWorldPos = Vector3.zero;
		HeroItemRender itemR = heroListRenders.Find (x => x.heroItemCfgId == itemCfgID);
		if (itemR != null) {
			uiWorldPos = itemR.transform.position;
			return true;
		}
		return false;
	}

    private HeroItemRender mMaxHero;//最大数量的英雄图标
    private int max_hero_count = -1;//最大英雄数量
    private void ReferMaxHeroInfo(uint hero_id,int index) {
        if (index < 0) {
            this.RefershBtnShow();
            return;
        }
        if (this.mMaxHero == null) {
            this.mMaxHero = GameUtils.CreateGo(this.item_hero_prefab, this.mBtnShowList.transform.parent).GetComponent<HeroItemRender>();
            UIEventListener.Get(this.mMaxHero.gameObject).onClick = OnButtonClick;
            this.mMaxHero.transform.localPosition = this.mBtnShowList.transform.localPosition;
            this.mMaxHero.maskObj.type = UIBasicSprite.Type.Filled;
            this.mMaxHero.maskObj.gameObject.SetActive(false);
        }
        this.SetHeroInfo(this.mMaxHero, hero_id, index);
        this.RefershBtnShow();
    }

    private HeroItemRender mCurHero;
    void ResetHeroCD() {//重置英雄道具CD
        for (int i = 0; i < heroListRenders.Count; i++) {
            heroListRenders[i].maskObj.gameObject.SetActive(false);
        }
        if (SceneLogic.Instance.HeroMgr.currentHero != null) {
            SceneLogic.Instance.HeroMgr.currentHero.CurrLifeTicks = 0;
        }
        if (this.mCurHero != null) {
            GameObject.Destroy(this.mCurHero.gameObject);
            this.mCurHero = null;
            this.RefershBtnShow();
        }
    }

    private void RefershBtnShow() {//更新显示英雄按钮的显示隐藏
        if (this.mCurHero != null) {
            this.mBtnShowList.gameObject.SetActive(false);
            if (this.mMaxHero != null) {
                this.mMaxHero.gameObject.SetActive(false);
            }
        } else if (this.mMaxHero == null) {
            if (heroListRenders.Count > 0 && (max_hero_count > 0 || this.mIsShowNullHero)) {
                this.mBtnShowList.SetActive(true);
                //this.mBtnShowList.SetActive(true);
            } else {
                this.mBtnShowList.SetActive(false);
            }
        } else {
            if ((max_hero_count > 0 || this.mIsShowNullHero) && this.mIsShowList == false) {
                this.mMaxHero.transform.localPosition = this.mBtnShowList.transform.localPosition;
                this.mBtnShowList.SetActive(false);
                this.mMaxHero.gameObject.SetActive(true);
            } else {
                //if (heroListRenders.Count > 0) {
                //    this.mBtnShowList.SetActive(true);
                //} else {
                if (heroListRenders.Count > 0 && (max_hero_count > 0 || this.mIsShowNullHero)) {
                    this.mBtnShowList.SetActive(true);
                } else {
                    this.mBtnShowList.SetActive(false);
                }
                //}
                this.mMaxHero.gameObject.SetActive(false);
            }
        }
    }

	public void Update(float delta) {
        if (mIsAutoHide && this.mIsShowList && UICamera.currentCamera != null) {
            RaycastHit[] hits = Physics.RaycastAll(UICamera.currentCamera.ScreenPointToRay(GInput.mousePosition), float.MaxValue, UICamera.currentCamera.cullingMask);
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].collider.gameObject == this.mObjItemListCollider) {
                    this.mListHideTime = Time.realtimeSinceStartup + 5;
                }
            }
            if (this.mIsShowList && this.mListHideTime < Time.realtimeSinceStartup) {
                this.HideHeroList();
            }
        }

		if (SceneLogic.Instance.HeroMgr.currentHero != null) 
		{
			Hero selfHero = SceneLogic.Instance.HeroMgr.currentHero;
            if (selfHero != null && selfHero.CurrLifeTicks > 0) {

                float progress = selfHero.CurrLifeTicks * 1.0f / selfHero.TotalLifeTicks;
                string sec = Mathf.FloorToInt(Utility.TickToFloat((uint)selfHero.CurrLifeTicks)).ToString();

                for (int i = 0; i < heroListRenders.Count; i++) {
                    if (heroListRenders[i].heroCfgID == selfHero.mHeroVo.CfgID) {//只显示当前召唤道具CD
                        heroListRenders[i].LifeLabel.text = sec;
                        if (this.mCurHero == null) {
                            this.mCurHero = GameUtils.CreateGo(this.item_hero_prefab, this.mBtnShowList.transform.parent).GetComponent<HeroItemRender>();
                            this.mCurHero.transform.localPosition = this.mBtnShowList.transform.localPosition;
                            this.SetHeroInfo(this.mCurHero, heroListRenders[i].heroItemCfgId, i);
                            this.mCurHero.maskObj.type = UIBasicSprite.Type.Filled;
                            this.mCurHero.maskObj.gameObject.SetActive(true);

                            this.RefershBtnShow();
                        }
                        this.mCurHero.LifeLabel.text = sec;
                        this.mCurHero.maskObj.fillAmount = progress;
                    } else {
                        heroListRenders[i].LifeLabel.text = string.Empty;
                    }
                    if (heroListRenders[i].maskObj.gameObject.activeSelf == false) {
                        heroListRenders[i].maskObj.gameObject.SetActive(true);
                        heroListRenders[i].maskObj.type = UIBasicSprite.Type.Filled;
                    }
                    heroListRenders[i].maskObj.fillAmount = progress;
                }
            } else {
                this.ResetHeroCD();
            }
        }
        ////英雄快捷键
        //for (int i = 0; i < hero_arr.Length; i++) {
        //    if (GInput.GetKeyDown(hero_arr[i])) {
        //        this.OnClickItem(i);
        //    }
        //}

        if (GameConfig.OP_AutoHero && SceneLogic.Instance.PlayerMgr.AutoShot) {
            ItemManager.AutoUseHero(SceneLogic.Instance.PlayerMgr.MySelf);
        }
	}

    private int SetHeroInfo(HeroItemRender item, uint item_id,int index) {
        ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet(item_id);
        uint heroCfgId = RoleItemModel.Instance.ApplyForData(item_id);
        int heroCnt = RoleItemModel.Instance.getItemCount(item_id);
        item.gameObject.SetActive(true);
        item.iconObj.spriteName = itemVo.ItemIcon.ToString();
        item.heroItemCfgId = item_id;
        item.heroCfgID = heroCfgId;
        item.countLabel.text = string.Format("x{0}", heroCnt);
        item.frameSp.spriteName = ConstValue.HeroFrameSpList[itemVo.Star];
        //item.mLbKey.text = (index + 1).ToString();


        UI_Ticks.AddTickListener(item.gameObject, StringTable.GetString(itemVo.ItemDec), SpriteAlignment.BottomLeft,itemVo.SalePrice);
        return heroCnt;
    }

    public void UpdateData() {
		if (heroListRenders == null)
			return;

        int max_count = 0;
        int __tmp;
        uint max_hero = 0;
        int max_index = -1;
        //uint roomCfgID = SceneLogic.Instance.GetRoomCfgID();
        for (int i = 0; i < heroListRenders.Count; i++) {
            if (i < heroItemIDList.Length) {
                __tmp = this.SetHeroInfo(heroListRenders[i], heroItemIDList[i], i);
                if (__tmp > max_count) {
                    max_count = __tmp;
                    max_hero = heroItemIDList[i];
                    max_index = i;
                }
                if (this.mCurHero != null && this.mCurHero.heroItemCfgId == heroItemIDList[i]) {
                    this.SetHeroInfo(this.mCurHero, heroItemIDList[i], i);
                }
                if (__tmp == 0 && this.mIsShowNullHero == false) {
                    heroListRenders[i].gameObject.SetActive(false);
                } else {
                    heroListRenders[i].gameObject.SetActive(true);
                }
            } else {
                heroListRenders[i].gameObject.SetActive(false);
            }
        }
        this.mGridHeroList.Reposition();

        max_hero_count = max_count;
        this.ReferMaxHeroInfo(max_hero, max_index);
        this.RefershBtnShow();
    }

	void HandleClickItem(GameObject button)
	{
		HeroItemRender itemR = button.GetComponent<HeroItemRender> ();
		uint heroItemCfgID = itemR.heroItemCfgId;

		if (SceneLogic.Instance.HeroMgr.currentHero != null)
			return;
		
		SceneLogic.Instance.HeroMgr.LaunchHero(heroItemCfgID);
	}

	public void Shutdown()
	{
		WndManager.Instance.CloseUI (EnumUI.QuickBuyUI);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.SceneHeroLeave, HandleHeroOnLeave);
		RoleItemModel.Instance.UnRegisterGlobalMsg (SysEventType.ItemInfoChange, HandleItemChange);
		heroListRenders = null;
        heroItemIDList = null;
	}
}