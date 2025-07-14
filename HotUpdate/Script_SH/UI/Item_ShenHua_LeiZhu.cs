using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_LeiZhu : UIItem {
    private int ItemHeight = 28;//列表项高度

    public UILabel mLbName;
    public UILabel mLbGold;
    public UILabel mLbResult;
    public UILabel mLbCount;//连续守擂次数
    public UISprite mSprShangLei;
    public GameObject mBtnXiaLei;
    public UISprite mSprShowList;
    public UIScrollView mPlayerScrollView;
    public UIScrollView mLeiZhuScrollView;
    public Item_ShenHua_Online_Item mItemPlayer;
    public Item_ShenHua_LeiZhu_Item mItemLeiZhu;
    public UILabel mLbWYSL;
    public GameObject mlbNoLeizhu;

    /// <summary>
    /// 在线按钮灰化
    /// </summary>
    public GameObject mBtnZaiXian0;

    /// <summary>
    /// 在线按钮
    /// </summary>
    public GameObject mBtnZaiXian;

    /// <summary>
    /// 上擂列表按钮
    /// </summary>
    public GameObject mBtnShangLei;

    /// <summary>
    /// 上擂列表按钮灰化
    /// </summary>
    public GameObject mBtnShangLei0;

    /// <summary>
    /// 在线玩家或者上擂玩家列表
    /// </summary>
    public GameObject mPlayerLstUI;

    /// <summary>
    /// 申请上擂的玩家列表
    /// </summary>
    public GameObject mLeiZhuLstUI;

    /// <summary>
    /// 在线玩家的预设列表
    /// </summary>
    public List<GameObject> mOnLinePrefabList = new List<GameObject>();

    /// <summary>
    /// 上擂玩家的预设列表
    /// </summary>
    public List<GameObject> mLeiZhuPrefabList = new List<GameObject>();

    /// <summary>
    /// 申请上擂玩家列表
    /// </summary>
    public List<RoleInfo> mLeiZhuList = new List<RoleInfo>();

    /// <summary>
    /// 在线玩家数量
    /// </summary>
    private UILabel mOnlinePlayerNum;

    /// <summary>
    /// 在线玩家灰化的显示
    /// </summary>
    private UILabel mOnlinePlayerNum_Gray;

    public List<Item_ShenHua_LeiZhu_Item> mItemList = new List<Item_ShenHua_LeiZhu_Item>();
    public bool mIsApply;//是否已经申请上擂
    public bool mIsShowLeiZhuList;//是否显示上擂列表

    public void InitData(RoleInfo leizhu,List<RoleInfo> list,long result,int count)
    {
        if (list != null)
        {
            mLeiZhuList.AddRange(list);
        }
        SetLeiZhu(leizhu, result, count);

        ShowOnLinePlayer();
    }

    /// <summary>
    /// 添加在线玩家
    /// </summary>
    /// <param name="role"></param>
    public void AddOnLineRole(RoleInfo role)
    {
        for (int i = 0; i < mOnLinePrefabList.Count; i++)
        {
            if (mOnLinePrefabList[i].GetComponent<Item_ShenHua_Online_Item>().mRole.UserID == role.UserID)
                return;
        }

        GameObject onLineItem = Instantiate(mItemPlayer.gameObject, mPlayerScrollView.transform);
        onLineItem.GetComponent<Item_ShenHua_Online_Item>().InitData(role);
        onLineItem.SetActive(true);
        mOnLinePrefabList.Add(onLineItem);

        for (int i = 0; i < mOnLinePrefabList.Count; i++)
        {
            mOnLinePrefabList[i].transform.localPosition = new Vector3(0, -i * 26);
        }

        mOnlinePlayerNum.text = string.Format("([c][ffffff] {0} [-][/c])", mOnLinePrefabList.Count);

        mOnlinePlayerNum_Gray.text = string.Format("([c][ffffff] {0} [-][/c])", mOnLinePrefabList.Count);

        RefreshOnlineScrollView(mOnLinePrefabList.Count);
    }

    private void RefreshOnlineScrollView(int onLineNum)
    {
        if (onLineNum < 19)
        {
            mPlayerScrollView.enabled = false;
            mPlayerScrollView.verticalScrollBar.alpha = 0;
        }
        else
        {
            mPlayerScrollView.enabled = true;
            mPlayerScrollView.verticalScrollBar.alpha = 1;
        }
    }

    private void RefreshLeiZhuScrollView(int LeiZhuNum)
    {
        if (LeiZhuNum < 19)
        {
            mLeiZhuScrollView.enabled = false;
            mLeiZhuScrollView.verticalScrollBar.alpha = 0;
        }
        else
        {
            mLeiZhuScrollView.enabled = true;
            mLeiZhuScrollView.verticalScrollBar.alpha = 1;
        }
    }

    /// <summary>
    ///在上线或者
    /// </summary>
    public void RefrshSelfGoldInScorllView(long gold)
    {
        for (int i = 0; i < mOnLinePrefabList.Count; i++)
        {
            if (mOnLinePrefabList[i].GetComponent<Item_ShenHua_Online_Item>().mRole == RoleManager.Self)
            {
                mOnLinePrefabList[i].GetComponent<Item_ShenHua_Online_Item>().mLbGold.text = gold.ToString();
            }
        }
        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            if (mLeiZhuPrefabList[i].GetComponent<Item_ShenHua_LeiZhu_Item>().mRole == RoleManager.Self)
            {
                mLeiZhuPrefabList[i].GetComponent<Item_ShenHua_LeiZhu_Item>().mLbGold.text = gold.ToString();
            }
        }
    }

    /// <summary>
    /// 将离开的玩家从列表中移除
    /// </summary>
    /// <param name="role"></param>
    public void RemoveOnLineRole(RoleInfo role)
    {
        for (int i = 0; i < mOnLinePrefabList.Count; i++)
        {
            if (role == mOnLinePrefabList[i].GetComponent<Item_ShenHua_Online_Item>().mRole)
            {
                Destroy(mOnLinePrefabList[i]);
                mOnLinePrefabList.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < mOnLinePrefabList.Count; i++)
        {
            mOnLinePrefabList[i].transform.localPosition = new Vector3(0, -i * 26);
        }

        mOnlinePlayerNum.text = string.Format("([c][ffffff] {0} [-][/c])", mOnLinePrefabList.Count);

        mOnlinePlayerNum_Gray.text = string.Format("([c][ffffff] {0} [-][/c])", mOnLinePrefabList.Count);
        RefreshOnlineScrollView(mOnLinePrefabList.Count);
    }

    /// <summary>
    /// 添加上擂
    /// </summary>
    /// <param name="leizhu"></param>
    public void AddLeiZhu(RoleInfo leizhu)
    {
        GameObject onLineItem = Instantiate(mItemLeiZhu.gameObject, mLeiZhuScrollView.transform);
        onLineItem.GetComponent<Item_ShenHua_LeiZhu_Item>().InitData(leizhu);
        onLineItem.SetActive(true);
        mLeiZhuPrefabList.Add(onLineItem);

        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            mLeiZhuPrefabList[i].transform.localPosition = new Vector3(0, -i * 26);
        }

        if (!IsInLeiZhuApply(leizhu))
            mLeiZhuList.Add(leizhu);

        if (leizhu.UserID == RoleManager.Self.UserID)
        {
            mLbWYSL.text = "我要下擂";
        }

        RefreshLeiZhuScrollView(mLeiZhuPrefabList.Count);
    }

    /// <summary>
    /// 该玩家是否已经在申请擂主的列表里
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    private bool IsInLeiZhuApply(RoleInfo role)
    {
        for (int i = 0; i < mLeiZhuList.Count; i++)
        {
            if (mLeiZhuList[i] == role)
                return true;
        }

        return false;
    }

    private void RemoveLeiZhuFromLeiZhuList(RoleInfo role)
    {
        for (int i = 0; i < mLeiZhuList.Count; i++)
        {
            if (mLeiZhuList[i] == role)
            {
                mLeiZhuList.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 移除下擂
    /// </summary>
    /// <param name="leizhu"></param>
    public void RemoveLeiZhu(RoleInfo leizhu)
    {
        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            if (mLeiZhuPrefabList[i].GetComponent<Item_ShenHua_LeiZhu_Item>().mRole == leizhu)
            {
                Destroy(mLeiZhuPrefabList[i]);
                mLeiZhuPrefabList.RemoveAt(i);
                break;
            }
        }

        RemoveLeiZhuFromLeiZhuList(leizhu);

        if (leizhu.UserID == RoleManager.Self.UserID)
        {
            mLbWYSL.text = "我要上擂";
        }

        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            mLeiZhuPrefabList[i].transform.localPosition = new Vector3(0, -i * 26);
        }

        RefreshLeiZhuScrollView(mLeiZhuPrefabList.Count);
    }

    /// <summary>
    /// 是否要激活我要上擂的按钮
    /// </summary>
    /// <param name="isActive"></param>
    public void ChangeWYSLBtnState(bool isActive)
    {
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, SHGameManager.LeiZhuSeat);

        if (leizhu == null)
        {
            if (mLbWYSL.IsGray)
            {
                mLbWYSL.IsGray = false;
                mLbWYSL.parent.GetComponent<UISprite>().IsGray = false;
                mLbWYSL.parent.GetComponent<BoxCollider>().enabled = true;
                mLbWYSL.text = "我要上擂";
            }

            return;
        }

        if (leizhu.UserID == RoleManager.Self.UserID)
        {
            if (!isActive)
            {
                mLbWYSL.IsGray = true;
                mLbWYSL.parent.GetComponent<UISprite>().IsGray = true;
                mLbWYSL.parent.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                mLbWYSL.IsGray = false;
                mLbWYSL.parent.GetComponent<UISprite>().IsGray = false;
                mLbWYSL.parent.GetComponent<BoxCollider>().enabled = true;
            }
            mLbWYSL.text = "我要下擂";
        }
        else
        {
            if (mLbWYSL.IsGray)
            {
                mLbWYSL.IsGray = false;
                mLbWYSL.parent.GetComponent<UISprite>().IsGray = false;
                mLbWYSL.parent.GetComponent<BoxCollider>().enabled = true;
                mLbWYSL.text = "我要上擂";
            }
        }
    }

    public void SetLeiZhu(RoleInfo leizhu, long result,int count)
    {
        if (leizhu != null)
        {
            mLbName.text = leizhu.NickName;
            mLbGold.text = Tools.longToStr(SHGameManager.LeiZhuGold, 3);
            RemoveLeiZhu(leizhu);

            ChangeWYSLBtnState(false);
        }
        else
        {
            if (SHGameManager.IsSysLeiZhu)
            {
                mLbName.text = "系统守擂";
                mLbGold.text = "1,000,000,000";
            }
            else
            {
                mLbName.text = "擂主离场";
                mLbGold.text = "0";
            }
        }
        mLbResult.text = Tools.longToStr(result, 3);
        mLbCount.text = Mathf.Min(99, count).ToString();
    }

    /// <summary>
    /// 刷新擂主信息
    /// </summary>
    public void RefershLeiZhuInfo()
    {
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, SHGameManager.LeiZhuSeat);
        SetLeiZhu(leizhu, SHGameManager.LeiZhuTotalResult, SHGameManager.LeiZhuTimes);
    }


    public void RefershApplyState()
    {
        //刷新是否可以上擂
        bool is_apply = RoleManager.Self.GoldNum >= SHGameManager.ApplyMinGold && (this.mLeiZhuList.Contains(RoleManager.Self) == false) && SHGameManager.LeiZhuSeat != RoleManager.Self.ChairSeat;
        this.mIsApply = is_apply;
    }

    public void SetShowLeiZhuList(bool is_show) {
        this.mIsShowLeiZhuList = is_show;
        if (is_show) {
            this.mSprShowList.flip = UIBasicSprite.Flip.Vertically;

            this.mLeiZhuScrollView.transform.parent.gameObject.SetActive(true);
            for (int i = 0; i < this.mItemList.Count; i++) {
                this.mItemList[i].gameObject.SetActive(false);
                this.mItemList[i].transform.localPosition = Vector2.zero;
            }

            Item_ShenHua_LeiZhu_Item item;
            for (int i = 0; i < this.mLeiZhuList.Count; i++) {
                if (this.mItemList.Count > i) {
                    item = this.mItemList[i];
                    item.gameObject.SetActive(true);
                } else {
                    item = this.AddItem<Item_ShenHua_LeiZhu_Item>(this.mItemPlayer.gameObject, this.mLeiZhuScrollView.transform);
                    this.mItemList.Add(item);
                }
                item.InitData(this.mLeiZhuList[i]);
                item.transform.localPosition = new Vector3(0, -ItemHeight * i);
            }
            this.mLeiZhuScrollView.ResetPosition();
        } else {
            this.mSprShowList.flip = UIBasicSprite.Flip.Nothing;
            this.mLeiZhuScrollView.transform.parent.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if (_app_time > 0)
        {
            if (_app_time > Time.deltaTime)
            {
                _app_time -= Time.deltaTime;
            }
            else
            {
                _app_time = 0;
            }
        }

        if (mLeiZhuPrefabList.Count == 0)
        {
            if (!mlbNoLeizhu.activeSelf)
                mlbNoLeizhu.SetActive(true);
        }
        else
        {
            if (mlbNoLeizhu.activeSelf)
                mlbNoLeizhu.SetActive(false);
        }
    }

    private float _app_time = 0;
    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_wysl"://上擂
                if (_app_time > 0)
                {
                    //上擂下擂需要CD
                    SystemMessageMgr.Instance.ShowMessageBox("当前操作过于频繁");
                    return;
                }
                _app_time = 3;

                if (mLbWYSL.text.Equals("我要上擂"))
                {
                    RefershApplyState();
                    if (mIsApply)
                    {
                        NetClient.Send(NetCmdType.SUB_C_APPLY_BANKER_SSZP, new CMD_C_ApplyBanker_sszp());
                    }
                }
                else
                {
                    RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, SHGameManager.LeiZhuSeat);
                    if (leizhu.UserID == RoleManager.Self.UserID)
                    {
                        mLbWYSL.text = "我要上擂";
                    }
                    NetClient.Send(NetCmdType.SUB_C_CANCEL_BANKER_SSZP, new CMD_C_CancelBanker_sszp());
                }
                break;
            case "item_btn_show_list":
                break;
            case "list_btn_zaixian0":
                ShowOnLinePlayer();
                break;
            case "list_btn_zaixian":
                ShowOnLinePlayer();
                break;
            case "list_btn_shanglei":
                ShowLeiZhuLst();
                break;
            case "list_btn_shanglei0":
                ShowLeiZhuLst();
                break;
        }
    }

    /// <summary>
    /// 显示在线玩家
    /// </summary>
    private void ShowOnLinePlayer()
    {
        mBtnShangLei.SetActive(false);
        mBtnShangLei0.SetActive(true);
        mBtnZaiXian.SetActive(true);
        mBtnZaiXian0.SetActive(false);
        mPlayerLstUI.SetActive(true);
        mLeiZhuLstUI.SetActive(false);
    }

    /// <summary>
    /// 显示擂主列表
    /// </summary>
    private void ShowLeiZhuLst()
    {
        mBtnZaiXian.SetActive(false);
        mBtnShangLei0.SetActive(false);
        mBtnShangLei.SetActive(true);
        mBtnZaiXian0.SetActive(true);
        mPlayerLstUI.SetActive(false);
        mLeiZhuLstUI.SetActive(true);
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_result":
                this.mLbResult = tf.GetComponent<UILabel>();
                break;
            case "item_lb_count":
                this.mLbCount = tf.GetComponent<UILabel>();
                break;
            case "item_btn_xialei":
                this.mBtnXiaLei = tf.gameObject;
                break;
            case "item_spr_show_list":
                this.mSprShowList = tf.GetComponent<UISprite>();
                break;
            case "scrollview_player":
                this.mPlayerScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "scrollview_leizhu":
                mLeiZhuScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "item_player":
                mItemPlayer = BindItem<Item_ShenHua_Online_Item>(tf.gameObject);
                mItemPlayer.gameObject.SetActive(false);
                break;
            case "list_btn_zaixian0":
                mBtnZaiXian0 = tf.gameObject;
                break;
            case "list_btn_zaixian":
                mBtnZaiXian = tf.gameObject;
                break;
            case "list_btn_shanglei":
                mBtnShangLei = tf.gameObject;
                break;
            case "list_btn_shanglei0":
                mBtnShangLei0 = tf.gameObject;
                break;
            case "item_player_list":
                mPlayerLstUI = tf.gameObject;
                break;
            case "item_leizhu_list":
                mLeiZhuLstUI = tf.gameObject;
                break;
            case "item_leizhu_item":
                mItemLeiZhu = BindItem<Item_ShenHua_LeiZhu_Item>(tf.gameObject);
                mItemLeiZhu.gameObject.SetActive(false);
                break;
            case "wysl":
                mLbWYSL = tf.GetComponent<UILabel>();
                break;
            case "lb_online_num":
                mOnlinePlayerNum = tf.GetComponent<UILabel>();
                break;
            case "lbl_onlinenum_gray":
                mOnlinePlayerNum_Gray = tf.GetComponent<UILabel>();
                break;
            case "lb_no_leizhu":
                mlbNoLeizhu = tf.gameObject;
                break;
        }
    }
}
