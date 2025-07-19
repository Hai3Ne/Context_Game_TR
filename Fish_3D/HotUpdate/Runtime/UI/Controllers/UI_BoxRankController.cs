using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_BoxRankController : IUIControllerImp {
    UI_BoxRankRef mUIRef;
    SC_GR_WorldBossRank resp_rank_info;//排行榜信息

    private List<Item_BoxRank> mRankList = new List<Item_BoxRank>();

    public override EnumPanelPRI PRI {
        get { return EnumPanelPRI.MIDDLE; }
    }
    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }

    public override void Init(object data) {
        base.Init(data);
        resp_rank_info = data as SC_GR_WorldBossRank;
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_BoxRank", SceneObjMgr.Instance.UIPanelTransform,
			delegate(GameObject obj) {
				uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
                mUIRef = uiRefGo.GetComponent<UI_BoxRankRef>();

                UIEventListener.Get(this.mUIRef.mBtnShowRule).onClick = OnButtonClick;
                UIEventListener.Get(this.mUIRef.mBtnClose).onClick = OnButtonClick;
                UIEventListener.Get(this.mUIRef.mMenuGet).onClick = OnButtonClick;
                UIEventListener.Get(this.mUIRef.mMenuUse).onClick = OnButtonClick;
                UIEventListener.Get(this.mUIRef.mBtnHideRule).onClick = OnButtonClick;
                //UIEventListener.Get(this.mUIRef.mBtnRuleOK).onClick = OnButtonClick;
                this.SetMenu(0);
                this.InitData();
                //
                this.SetRuleInfo(StringTable.GetString("WorldChest1"));
                this.ShowRuleInfo(false);
                this.mUIRef.mItemRank.SetActive(false);
                this.mUIRef.mItemRule.SetActive(false);

                mUIRef.mSprFrame.transform.localPosition = SceneLogic.Instance.WorldBossMgr.mViewer.transform.localPosition;
                mUIRef.mSprFrame.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                this.mUIRef.mSprBG.alpha = 0.01f;
                TweenPosition.Begin(this.mUIRef.mSprFrame.gameObject, 0.3f, Vector3.zero);
                TweenScale.Begin(this.mUIRef.mSprFrame.gameObject, 0.3f, Vector3.one);
                TweenAlpha.Begin(this.mUIRef.mSprBG.gameObject, 0.3f, 1);

                UI3DModelManager.AddUIPanel(uiRefGo);
            }
        );
        base.Show();
    }
    private void InitData() {
        string str = StringTable.GetString(string.Format("ActiveTime{0}", SceneLogic.Instance.GetRoomCfgID()), "");
        if (string.IsNullOrEmpty(str)) {
            this.mUIRef.mLbActionTime.gameObject.SetActive(false);
        } else {
            this.mUIRef.mLbActionTime.gameObject.SetActive(true);
            this.mUIRef.mLbActionTime.text = str;
        }

        if (this.resp_rank_info.MyGainIndex == 0xffff) {//排名等于0xffff表示未上榜
            this.mUIRef.mLbGetRank.text = "未上榜";
        } else {
            this.mUIRef.mLbGetRank.text = (this.resp_rank_info.MyGainIndex+1).ToString();
        }
        this.mUIRef.mLbGetScore.text = this.resp_rank_info.MyGain.ToString();
        if (this.resp_rank_info.MyCostIndex == 0xffff) {//排名等于0xffff表示未上榜
            this.mUIRef.mLbUseRank.text = "未上榜";
        } else {
            this.mUIRef.mLbUseRank.text = (this.resp_rank_info.MyCostIndex+1).ToString();
        }
        this.mUIRef.mLbUserScore.text = this.resp_rank_info.MyCost.ToString();
        if (this.resp_rank_info.Jackpot > 0) {
            this.mUIRef.mLbJackpot.gameObject.SetActive(true);
            this.mUIRef.mLbJackpot.text = this.resp_rank_info.Jackpot.ToString();
        } else {
            this.mUIRef.mLbJackpot.gameObject.SetActive(false);
        }
    }

    private void SetRuleInfo(string info) {//设置规则信息
        string[] lines = info.Split('\n');
        GameObject item;
        int y = 200;
        for (int i = 0; i < lines.Length; i++) {
            item = GameUtils.CreateGo(this.mUIRef.mItemRule, this.mUIRef.mScrollViewRule.transform);
            item.transform.localPosition = new Vector3(0, y);
            UILabel lb = item.GetComponentInChildren<UILabel>();
            lb.text = string.Format("{0}  {1}", i + 1, lines[i]);
            y -= lb.height + 26;
        }

        this.mUIRef.mObjRankInfo.transform.localPosition = new Vector3(0, y);
        y -= 140;

        List<WorldBossGrantVo> list = FishConfig.Instance.mWorldBossGrantConf;
        List<GrantInfo> list_rant = new List<GrantInfo>();
        bool is_add;
        for (int i = 0; i < list.Count; i++) {
            is_add = false;
            for (int j = 0; j < list_rant.Count; j++) {
                if (list[i].Grant == list_rant[j].Grant && list[i].Condition == list_rant[j].Condition) {
                    is_add = true;
                    list_rant[j].AddRank(list[i].CostRank);
                    break;
                }
            }
            if (is_add == false) {
                GrantInfo rant = new GrantInfo();
                rant.Grant = list[i].Grant;
                rant.Condition = list[i].Condition;
                rant.AddRank(list[i].CostRank);
                list_rant.Add(rant);
            }
        }
        
        uint RoomMultiple = FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID()).RoomMultiple;
        this.mUIRef.mItemGrantInfo.SetActive(false);
        for (int i = 0; i < list_rant.Count; i++) {
            item = GameUtils.CreateGo(this.mUIRef.mItemGrantInfo, this.mUIRef.mScrollViewRule.transform);
            item.transform.localPosition = new Vector3(0, y);
            Item_GrantInfoRef grant = item.GetComponent<Item_GrantInfoRef>();
            if (list_rant[i].min == list_rant[i].max) {
                grant.mLbRank.text = string.Format("第{0}名", list_rant[i].min+1);
            } else {
                grant.mLbRank.text = string.Format("第{0}~{1}名", list_rant[i].min+1, list_rant[i].max+1);
            }
            grant.mLbGold.text = (list_rant[i].Grant * RoomMultiple).ToString();
            grant.mLbCost.text = (list_rant[i].Condition * RoomMultiple).ToString();
            y -= 60;
        }
        this.mUIRef.mLbBottomInfo.text = StringTable.GetString("WorldChest2");
        this.mUIRef.mLbBottomInfo.transform.localPosition = new Vector3(0, y);
    }
    private class GrantInfo {
        public List<uint> mRankList = new List<uint>();
        public uint Grant;
        public uint Condition;

        public uint min = 0;
        public uint max = 0;

        public void AddRank(uint rank) {
            if (mRankList.Count == 0) {
                min = rank;
                max = rank;
            } else {
                if (min > rank) {
                    min = rank;
                }
                if (max < rank) {
                    max = rank;
                }
            }
            mRankList.Add(rank);
        }
    }

    public void SetMenu(int index) {//0:收益  1:消耗
        this.mUIRef.mMenuGet.SetActive(true);
        this.mUIRef.mMenuUse.SetActive(true);
        this.mUIRef.mMenuGetActivity.SetActive(false);
        this.mUIRef.mMenuUserActivity.SetActive(false);
        tagUserWBData[] ranks;
        switch (index) {
            case 0://收益
                this.mUIRef.mMenuGet.SetActive(false);
                this.mUIRef.mMenuGetActivity.SetActive(true);
                ranks = this.resp_rank_info.GainList;
                break;
            case 1://消耗
            default:
                this.mUIRef.mMenuUse.SetActive(false);
                this.mUIRef.mMenuUserActivity.SetActive(true);
                ranks = this.resp_rank_info.CostList;
                break;
        }
        if (ranks == null) {
            ranks = new tagUserWBData[0];
        }

        for (int i = 0; i < mRankList.Count; i++) {
            mRankList[i].gameObject.SetActive(false);
        }
        Item_BoxRank item;
        for (int i = 0; i < 20; i++) {
            if (mRankList.Count > i) {
                item = mRankList[i];
                item.gameObject.SetActive(true);
            } else { 
                item = GameUtils.CreateGo(this.mUIRef.mItemRank, this.mUIRef.mScrollViewRank.transform).GetComponent<Item_BoxRank>();
                mRankList.Add(item);
            }
            if (ranks.Length > i) {
                item.InitData(i+1, ranks[i]);
            } else {
                item.InitData(i + 1);
            }
            item.transform.localPosition = new Vector3(0, -140 * i, 0);
        }
        TimeManager.DelayExec(0, this.mUIRef.mScrollViewRank.ResetPosition);
    }


    public override void Close() {
        if (mRankList.Count > 0) {
            mRankList.Clear();
            TweenPosition.Begin(this.mUIRef.mSprFrame.gameObject, 0.3f, SceneLogic.Instance.WorldBossMgr.mViewer.transform.localPosition);
            TweenScale.Begin(this.mUIRef.mSprFrame.gameObject, 0.3f, Vector3.zero);
            TweenAlpha.Begin(this.mUIRef.mSprBG.gameObject, 0.3f, 0);
            GameObject.Destroy(uiRefGo, 0.3f);
            uiRefGo = null;
            base.Close();
        }
    }

    public void ShowRuleInfo(bool is_show) {
        if (is_show) {
            TimeManager.DelayExec(0, this.mUIRef.mScrollViewRule.ResetPosition);
            this.mUIRef.mRuleInfo.SetActive(true);
        } else {
            this.mUIRef.mRuleInfo.SetActive(false);
        }
    }

    public void OnButtonClick(GameObject obj) {
        if (this.mUIRef.mBtnShowRule == obj) {//显示规则详情
            this.ShowRuleInfo(true);
        } else if (this.mUIRef.mBtnClose == obj) {//关闭按钮
            this.Close();
        } else if (this.mUIRef.mMenuGet == obj) {//显示收益排名
            this.SetMenu(0);
        } else if (this.mUIRef.mMenuUse == obj) {//显示消耗排名
            this.SetMenu(1);
        } else if (this.mUIRef.mBtnHideRule == obj) {//隐藏规则详情
            this.ShowRuleInfo(false);
        //} else if (this.mUIRef.mBtnRuleOK == obj) {//规则确认按钮
        //    this.ShowRuleInfo(false);
        }
    }
}
