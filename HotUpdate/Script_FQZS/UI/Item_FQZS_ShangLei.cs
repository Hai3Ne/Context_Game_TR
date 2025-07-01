using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ShangLei : UIItem
{
    /// <summary>
    /// 当前申请擂主的人数
    /// </summary>
    public UILabel mApplyNum;

    /// <summary>
    /// 前面还有多少个擂主
    /// </summary>
    public UILabel mHavebefroeApply;

    private float _app_time = 0;

    /// <summary>
    /// 上擂玩家的预设列表
    /// </summary>
    public List<GameObject> mLeiZhuPrefabList = new List<GameObject>();

    private List<RoleInfo> mShangLeiInfoList = new List<RoleInfo>();

    /// <summary>
    /// 上擂文本
    /// </summary>
    public UILabel mLbShanglei;

    private UISprite mShangLeiBg;

    /// <summary>
    /// 上擂列表
    /// </summary>
    private GameObject mShangLeiList;

    /// <summary>
    /// 上擂玩家滑动列表
    /// </summary>
    private UIScrollView mShangLeiView;

    /// <summary>
    /// 上擂玩家的预设
    /// </summary>
    private GameObject mShangLeiItem;

    private void Awake()
    {
        mLeiZhuPrefabList.Clear();
        mShangLeiInfoList.Clear();
    }

    /// <summary>
    /// 申请擂主服务器返回
    /// </summary>
    /// <param name="applyNum"></param>
    /// <param name="leizhuSeat"></param>
    public void ApplyLeiZhuCallBack(int applyNum, ushort applySeat)
    {
        mApplyNum.text = applyNum.ToString();

        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, applySeat);

        if (leizhu == null)
        {
            LogMgr.LogError("庄家位置信息错误 : " + applySeat);
            return;
        }
        
       if(!mShangLeiInfoList.Contains(leizhu))
            mShangLeiInfoList.Add(leizhu);

        if (applySeat == RoleManager.Self.ChairSeat)
        {
            mLbShanglei.text = "我要下擂";
        }

        AddShangLeiView(leizhu);

        StartCoroutine(RefreshItemPos());
    }

    /// <summary>
    /// 移除一个擂主
    /// </summary>
    /// <param name="userId"></param>
    private void RemoveLeiZhu(RoleInfo role)
    {
        for (int i = 0; i < mShangLeiInfoList.Count; i++)
        {
            if (mShangLeiInfoList[i] == role)
            {
                mShangLeiInfoList.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            if (mLeiZhuPrefabList[i].GetComponent<Item_FQZS_ApplyShangLei_Item>().userInfo == role)
            {
                Destroy(mLeiZhuPrefabList[i]);
                mLeiZhuPrefabList.RemoveAt(i);
                break;
            }
        }

        StartCoroutine(RefreshItemPos());
    }

    /// <summary>
    /// 当从保险箱中取钱的时候刷新自己的金币数量
    /// </summary>
    public void RefreshSelfGoldInScorllView(long goldNum)
    {
        for (int i = 0; i < mShangLeiView.transform.childCount; i++)
        {
            if (mShangLeiView.transform.GetChild(i).GetComponent<Item_FQZS_ApplyShangLei_Item>().userInfo.UserID == RoleManager.Self.UserID)
            {
                mShangLeiView.transform.GetChild(i).GetComponent<Item_FQZS_ApplyShangLei_Item>().mLeDou.text = goldNum.ToString();
            }
        }
    }

    private void RemoveLeiZhu(string nickName)
    {
        for (int i = 0; i < mShangLeiInfoList.Count; i++)
        {
            if (mShangLeiInfoList[i].NickName.Equals(nickName))
            {
                mShangLeiInfoList.RemoveAt(i);
            }
        }

        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            if (mLeiZhuPrefabList[i].GetComponent<Item_FQZS_ApplyShangLei_Item>().userInfo.NickName.Equals(nickName))
            {
                Destroy(mLeiZhuPrefabList[i]);
                mLeiZhuPrefabList.RemoveAt(i);
                break;
            }
        }

        StartCoroutine(RefreshItemPos());
    }

    IEnumerator RefreshItemPos()
    {
        yield return new WaitForSeconds(0.2f);

        float myPosY = 0;

        bool checkMe = false;

        for (int i = 0; i < mShangLeiView.transform.childCount; i++)
        {
            Transform trans = mShangLeiView.transform.GetChild(i);
            trans.localPosition = new Vector3(0, -i * 30);

            Item_FQZS_ApplyShangLei_Item curItem = trans.GetComponent<Item_FQZS_ApplyShangLei_Item>();
            if (curItem.userInfo == null)
                continue;

            if (curItem.userInfo.UserID == RoleManager.Self.UserID)
            {
                myPosY = trans.localPosition.y;
                checkMe = true;
            }
        }

        if (checkMe)
        {
            mHavebefroeApply.text =  (Mathf.Abs(myPosY) / 30).ToString();
        }
        else
        {
            mHavebefroeApply.text = "--";
        }
    }

    /// <summary>
    /// 擂主被更换服务器返回
    /// </summary>
    public void ChangeLeiZhuCallBack(int applyNum, ushort leizhuSeat)
    {
        mApplyNum.text = applyNum.ToString();
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, leizhuSeat);

        if(leizhu!=null)
            RemoveLeiZhu(leizhu);

        ChangeWYSLBtnState(false);
    }

    /// <summary>
    /// 取消申请擂主服务器返回
    /// </summary>
    public void CancelApplyLeiZhu(int applyNum,string cancelName)
    {
        mApplyNum.text = applyNum.ToString();
        RemoveLeiZhu(cancelName);

        if (cancelName.Equals(RoleManager.Self.NickName))
        {
            mLbShanglei.text = "我要上擂";
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
    }

    public void InitData(List<RoleInfo> leiZhuList)
    {
        if (leiZhuList != null)
        {
            mShangLeiInfoList.AddRange(leiZhuList.ToArray());
        }

        ChangeWYSLBtnState(false);

        for (int i = 0; i < mShangLeiInfoList.Count; i++)
        {
            if(!IsHave(mShangLeiInfoList[i].UserID))
                AddShangLeiView(mShangLeiInfoList[i]);
        }

        for (int i = 0; i < mLeiZhuPrefabList.Count; i++)
        {
            mLeiZhuPrefabList[i].transform.localPosition = new Vector3(0, -i * 30);
        }
    }

    private bool IsHave(uint userId)
    {
        for (int i = 0; i <mLeiZhuPrefabList.Count; i++)
        {
            if (mLeiZhuPrefabList[i].GetComponent<Item_FQZS_ApplyShangLei_Item>().userInfo.UserID == userId)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 刷新上擂滑动列表
    /// </summary>
    private void AddShangLeiView(RoleInfo role)
    {
        GameObject shangLeiItem = Instantiate(mShangLeiItem, mShangLeiView.transform);
        shangLeiItem.GetComponent<Item_FQZS_ApplyShangLei_Item>().InitData(role);
        shangLeiItem.gameObject.SetActive(true);
        mLeiZhuPrefabList.Add(shangLeiItem);
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_sl":
                ApplyLeiZhu();
                break;
            case "btn_lb":
                ShowShangLeiList();
                break;
        }
    }

    public void ChangeWYSLBtnState(bool isActive)
    {
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, FQZSGameManager.LeiZhuSeat);

        if (leizhu == null)
        {
            if (mLbShanglei.IsGray)
            {
                mLbShanglei.IsGray = false;
                mLbShanglei.parent.GetComponent<UISprite>().IsGray = false;
                mLbShanglei.parent.GetComponent<BoxCollider>().enabled = true;
                mLbShanglei.text = "我要上擂";
            }

            return;
        }


        if (leizhu.UserID == RoleManager.Self.UserID)
        {
            if (!isActive)
            {
                mLbShanglei.IsGray = true;
                mLbShanglei.parent.GetComponent<UISprite>().IsGray = true;
                mLbShanglei.parent.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                mLbShanglei.IsGray = false;
                mLbShanglei.parent.GetComponent<UISprite>().IsGray = false;
                mLbShanglei.parent.GetComponent<BoxCollider>().enabled = true;
                mLbShanglei.text = "我要下擂";
            }
        }
        else
        {
            if (mLbShanglei.IsGray)
            {
                mLbShanglei.IsGray = false;
                mLbShanglei.parent.GetComponent<UISprite>().IsGray = false;
                mLbShanglei.parent.GetComponent<BoxCollider>().enabled = true;
                mLbShanglei.text = "我要上擂";
            }
        }
    }

    /// <summary>
    /// 申请上擂
    /// </summary>
    private void ApplyLeiZhu()
    {
        if (_app_time > 0)
        {
            //上擂下擂需要CD
            SystemMessageMgr.Instance.ShowMessageBox("当前操作过于频繁");
            return;
        }

        _app_time = 3;

        if (mLbShanglei.text.Equals("我要上擂"))
        {
            if (FQZSGameManager.ApplyBankerCondition < RoleManager.Self.GoldNum && FQZSGameManager.LeiZhuSeat !=RoleManager.Self.ChairSeat)
            {
                CMD_C_ApplyBanker_fqzs req = new CMD_C_ApplyBanker_fqzs();
                NetClient.Send(NetCmdType.SUB_C_APPLY_BANKER_FQZS, req);
            }
            else
            {
                SystemMessageMgr.Instance.ShowMessageBox("上擂金额不足,无法上擂");
            }
        }
        else
        {
            RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, FQZSGameManager.LeiZhuSeat);
            if (leizhu != null && leizhu.UserID == RoleManager.Self.UserID)
            {
                mLbShanglei.text = "我要上擂";
            }
            CMD_C_CancelBanker_fqzs req = new CMD_C_CancelBanker_fqzs();
            NetClient.Send(NetCmdType.SUB_C_CANCEL_BANKER_FQZS, req);
        }
    }

    /// <summary>
    /// 显示上擂列表
    /// </summary>
    private void ShowShangLeiList()
    {
        mShangLeiList.SetActive(!mShangLeiList.activeSelf);
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "dqsq":
                mApplyNum = tf.GetComponent<UILabel>();
                break;
            case "qmhy":
                mHavebefroeApply = tf.GetComponent<UILabel>();
                break;
            case "lb_shanglei":
                mLbShanglei = tf.GetComponent<UILabel>();
                break;
            case "btn_sl":
                mShangLeiBg = tf.GetComponent<UISprite>();
                break;
            case "list_shanglei":
                mShangLeiList = tf.gameObject;
                break;
            case "scrollview_shanglei":
                mShangLeiView = tf.GetComponent<UIScrollView>();
                break;
            case "lb_user":
                BindItem<Item_FQZS_ApplyShangLei_Item>(tf.gameObject);
                mShangLeiItem = tf.gameObject;
                mShangLeiItem.SetActive(false);
                break;
        }
    }
}
