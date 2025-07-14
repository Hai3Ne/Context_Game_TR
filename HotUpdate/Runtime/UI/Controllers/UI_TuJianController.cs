using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_TuJianController : IUIControllerImp {
    UI_TuJianRef mUIRef;

    private int def_menu = 0;//默认显示菜单项
    public override void Init(object data) {
        base.Init(data);
        if (data != null) {
            int.TryParse(data.ToString(), out def_menu);
        }
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_TuJian", SceneObjMgr.Instance.UIPanelTransform,
			delegate(GameObject obj) {
				uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
				TweenShow();
                mUIRef = uiRefGo.GetComponent<UI_TuJianRef>();

                UIEventListener.Get(this.mUIRef.mObjBtnClose).onClick = OnButtonClick;
                for (int i = 0; i < this.mUIRef.mMenus.Length; i++) {
                    UIEventListener.Get(this.mUIRef.mMenus[i]).onClick = OnButtonClick;
                }

                this.mUIRef.mItemLauncher.SetActive(false);
                this.mUIRef.mItemFishTeShu.SetActive(false);
                this.mUIRef.mItemFishPuTong.SetActive(false);

                this.SetMenu(this.def_menu);
            }
        );
        base.Show();
    }


    public List<Item_TuJian_Fish> mItemPuTongList = new List<Item_TuJian_Fish>();//普通鱼列表
    public List<Item_TuJian_Fish> mItemTeShuList = new List<Item_TuJian_Fish>();//特殊鱼列表
    public List<Item_TuJian_Launcher> mItemLauncherList = new List<Item_TuJian_Launcher>();//图鉴列表
    public void InitData_FishpuTong() {
        for (int i = 0; i < mItemPuTongList.Count; i++) {
            this.mItemPuTongList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemTeShuList.Count; i++) {
            this.mItemTeShuList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemLauncherList.Count; i++) {
            this.mItemLauncherList[i].gameObject.SetActive(false);
        }

        var enumerator = FishConfig.Instance.mFishBookConf.Values.GetEnumerator();
        Item_TuJian_Fish item;
        int index = 0;
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Type == 1) {//type: 1.普通鱼   2.特殊鱼
                if (this.mItemPuTongList.Count > index) {
                    item = this.mItemPuTongList[index];
                } else {
                    GameObject obj = GameObject.Instantiate(this.mUIRef.mItemFishPuTong, this.mUIRef.mGrid.transform) as GameObject;
                    obj.transform.localScale = Vector3.one;
                    item = obj.AddComponent<Item_TuJian_Fish>();
                    this.mItemPuTongList.Add(item);
                }
                item.gameObject.SetActive(true);
                item.InitData(enumerator.Current);

                index++;
            }
        }
        this.mUIRef.mGrid.cellWidth = 240;
        this.mUIRef.mGrid.cellHeight = 290;
        this.mUIRef.mGrid.maxPerLine = 7;
        this.mUIRef.mGrid.Reposition();

        TimeManager.DelayExec(0, () => {
            this.mUIRef.mScrollViewInfo.panel.ResetAndUpdateAnchors();
            this.mUIRef.mScrollViewInfo.ResetPosition();
        });
    }
    public void InitData_FishTeShu() {
        for (int i = 0; i < mItemPuTongList.Count; i++) {
            this.mItemPuTongList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemTeShuList.Count; i++) {
            this.mItemTeShuList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemLauncherList.Count; i++) {
            this.mItemLauncherList[i].gameObject.SetActive(false);
        }

        var enumerator = FishConfig.Instance.mFishBookConf.Values.GetEnumerator();
        Item_TuJian_Fish item;
        int index = 0;
        while (enumerator.MoveNext()) {
            if (enumerator.Current.Type == 2) {//type: 1.普通鱼   2.特殊鱼
                if (this.mItemTeShuList.Count > index) {
                    item = this.mItemTeShuList[index];
                } else {
                    GameObject obj = GameObject.Instantiate(this.mUIRef.mItemFishTeShu, this.mUIRef.mGrid.transform) as GameObject;
                    obj.transform.localScale = Vector3.one;
                    item = obj.AddComponent<Item_TuJian_Fish>();
                    this.mItemTeShuList.Add(item);
                }
                item.gameObject.SetActive(true);
                item.InitData(enumerator.Current);

                index++;
            }
        }
        this.mUIRef.mGrid.cellWidth = 240;
        this.mUIRef.mGrid.cellHeight = 290;
        this.mUIRef.mGrid.maxPerLine = 7;
        this.mUIRef.mGrid.Reposition();

        TimeManager.DelayExec(0, () => {
            this.mUIRef.mScrollViewInfo.panel.ResetAndUpdateAnchors();
            this.mUIRef.mScrollViewInfo.ResetPosition();
        });
    }
    public void InitData_Launcher() {//
        for (int i = 0; i < mItemPuTongList.Count; i++) {
            this.mItemPuTongList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemTeShuList.Count; i++) {
            this.mItemTeShuList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemLauncherList.Count; i++) {
            this.mItemLauncherList[i].gameObject.SetActive(false);
        }

        var enumerator = FishConfig.Instance.mLauncherBookConf.Values.GetEnumerator();
        Item_TuJian_Launcher item;
        int count = 0;
        while (enumerator.MoveNext()) {
            if (this.mItemLauncherList.Count > count) {
                item = this.mItemLauncherList[count];
            } else {
                GameObject obj = GameObject.Instantiate(this.mUIRef.mItemLauncher, this.mUIRef.transform) as GameObject;
                obj.transform.localScale = Vector3.one;
                item = obj.AddComponent<Item_TuJian_Launcher>();
                this.mItemLauncherList.Add(item);
            }
            item.gameObject.SetActive(true);
            item.InitData(enumerator.Current);

            count++;
        }

        int cell_width = 1200 / (count - 1);
        for (int i = 0; i < count; i++) {
            this.mItemLauncherList[i].transform.localPosition = new Vector3((i + 0.5f - count / 2f) * cell_width, 120);
        }

    }

    public void SetMenu(int index) {//0:特殊鱼  1:普通鱼  2:炮台
        for (int i = 0; i < this.mUIRef.mMenus.Length; i++) {
            this.mUIRef.mMenus[i].SetActive(i != index);
        }
        for (int i = 0; i < this.mUIRef.mMenuSelects.Length; i++) {
            this.mUIRef.mMenuSelects[i].SetActive(i == index);
        }

        for (int i = 0; i < mItemPuTongList.Count; i++) {
            this.mItemPuTongList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemTeShuList.Count; i++) {
            this.mItemTeShuList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < mItemLauncherList.Count; i++) {
            this.mItemLauncherList[i].gameObject.SetActive(false);
        }
        switch (index) {
            case 0:
                this.InitData_FishTeShu();
                break;
            case 1:
                this.InitData_FishpuTong();
                break;
            case 2:
                this.InitData_Launcher();
                break;
        }
    }


    public override void Close() {
        this.mItemPuTongList.Clear();
        this.mItemTeShuList.Clear();
        this.mItemLauncherList.Clear();
        WndManager.Instance.CloseUI(EnumUI.UI_FishInfo);
        WndManager.Instance.CloseUI(EnumUI.UI_LauncherInfo);
        base.Close();
    }

    public void OnButtonClick(GameObject obj) {
        if (this.mUIRef.mObjBtnClose == obj) {
            this.Close();
        } else {
            for (int i = 0; i < this.mUIRef.mMenus.Length; i++) {
                if (this.mUIRef.mMenus[i] == obj) {
                    this.SetMenu(i);
                }
            }
        }
    }
}
