using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Shop : UILayer
{
    public UILabel mLbGold;
    public UIGrid mGrid;
    public GameObject mItemInfo;
    public GameObject dialog_rule;
    public GameObject dialog_pay;
    private PayItem CurPayItem;

    public List<Item_Shop> mItemList = new List<Item_Shop>();
    public void InitData()
    {
        List<PayItem> list = ShopManager.GetPayList();
        Item_Shop item;
        for (int i = 0; i < list.Count; i++)
        {
            item = this.AddItem<Item_Shop>(this.mItemInfo, this.mGrid.transform);
            item.InitData(list[i], OpenPayWindow);
        }
        this.mGrid.Reposition();
        this.UpdateUserInfo();
        this.SetShowRule(false);
    }

    private void OpenPayWindow(PayItem pay_item)
    {
        CurPayItem = pay_item;
        dialog_pay.SetActive(true);
    }

    public void UpdateUserInfo()
    {
        this.mLbGold.text = HallHandle.UserGold.ToString();
    }
    public void SetShowRule(bool is_show)
    {
        this.dialog_rule.SetActive(is_show);
    }

    private void OnEventHandle(GameEvent event_type, object obj) {
        switch (event_type) {
            case GameEvent.Hall_UserInfoChange://用户信息变更
                this.UpdateUserInfo();
                break;
        }
    }
    public override void OnNodeLoad() {
        EventManager.RegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);

        this.InitData();
    }
    public override void OnEnter() { }
    public override void OnExit() {
        EventManager.UnRegisterEvent(GameEvent.Hall_UserInfoChange, this.OnEventHandle);
    }
    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_close":
                this.Close();
                break;
            case "item_btn_close":
                this.SetShowRule(false);
                break;
            case "btn_rule":
                this.SetShowRule(true);
                break;
            case "btn_pay_close":
                dialog_pay.SetActive(false);
                break;
            case "btn_wechat":
                StartPay(PlatormEnum.IOS_WX);
                break;
            case "btn_alipay":
                StartPay(PlatormEnum.IOS_AliPay);
                break;
        }
    }

    private void StartPay(PlatormEnum payType)
    {
        if (CurPayItem == null)
            return;

        MainEntrace.Instance.ShowLoad("支付中...", 5);
        SDKMgr.Instance.StartPay((errr_code) =>
        {
            TimeManager.AddCallThread(() =>
            {
                MainEntrace.Instance.HideLoad();
                LogMgr.LogError("pay_result_code:" + errr_code.ToString());
                OnPayResult(errr_code);
            });
        }, CurPayItem, HallHandle.Accounts, payType);
    }

    private void OnPayResult(int code)
    {
        if (code == 0)
        {
            MainEntrace.Instance.ShowLoad("支付确认中...", 5);
            PayManager.AddFinishOrder(PayManager.mPreOrder);
        }
        else if (code == -1)
        {
            LogMgr.Log("用户取消支付");
        }
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "grid_list":
                this.mGrid = tf.GetComponent<UIGrid>();
                break;
            case "item_info":
                this.mItemInfo = tf.gameObject;
                this.mItemInfo.SetActive(false);
                break;
            case "dialog_rule":
                this.dialog_rule = tf.gameObject;
                break;
            case "dialog_pay":
                dialog_pay = tf.gameObject;
                break;
        }
    }
}
