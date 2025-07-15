using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SC : UILayer {
    public UISprite mSprMoney;
    public UIGrid mGridAward;
    public Item_SC[] mItems = new Item_SC[5];
    public GameObject mDialog_pay;

    public PayItem msg_pay_info;
    public void InitData(PayItem item_info) {
        this.msg_pay_info = item_info;

        if (this.mSprMoney != null && item_info.Price == 6) {
            this.mSprMoney.spriteName = "6-min";
        }

        CMD_GP_FirstChargeAward award = ShopManager.msg_cs_award;
        List<uint> item_id_list = new List<uint>(award.vecItemID);
        List<uint> item_count_list = new List<uint>(award.vecItemNum);
        if (ShopManager.ShouChongGold > 0) {
            item_id_list.Insert(0, 2001u);
            item_count_list.Insert(0, ShopManager.ShouChongGold);
        }
        if (award != null) {
            for (int i = 0; i < this.mItems.Length; i++) {
                if (i == 0) {
                    if (award.MemberOrder > 0 && award.MemberDays > 0) {
                        this.mItems[i].gameObject.SetActive(true);
                        this.mItems[i].InitData_VIP(award.MemberOrder, award.MemberDays);
                    } else {
                        this.mItems[i].gameObject.SetActive(false);
                    }
                } else {
                    if (item_id_list.Count > i - 1 && item_count_list.Count > i - 1) {
                        this.mItems[i].gameObject.SetActive(true);
                        this.mItems[i].InitData(item_id_list[i - 1], item_count_list[i - 1]);
                    } else {
                        this.mItems[i].gameObject.SetActive(false);
                    }
                }
            }  
        }
        this.mGridAward.Reposition();
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() {
        //MainEntrace.Instance.EnterNextTick();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
            case "btn_buynow":
                mDialog_pay.SetActive(true);
                break;
            case "btn_wechat":
                Pay(PlatormEnum.IOS_WX);
                break;
            case "btn_alipay":
                Pay(PlatormEnum.IOS_AliPay);
                break;
            case "btn_pay_close":
                mDialog_pay.SetActive(false);
                break;
        }
    }

    private void Pay(PlatormEnum payType)
    {
        MainEntrace.Instance.ShowLoad("支付中...", 5);

        SDKMgr.Instance.StartPay((errr_code) =>
        {
            //0.成功  -1 主动取消
            TimeManager.AddCallThread(() =>
            {
                MainEntrace.Instance.HideLoad();
                OnPayResult(errr_code);
                LogMgr.LogError("id:" + this.msg_pay_info.szProductID + "  pay_result_code:" + errr_code.ToString());
            });
        }, this.msg_pay_info, HallHandle.Accounts, payType);
    }

    private void OnPayResult(int code)
    {
        if (code == 0)
        {
            MainEntrace.Instance.ShowLoad("支付确认中...", 5);
            // PayManager.AddFinishOrder(PayManager.mPreOrder);
            this.Close();
            //首充成功之后删除首充标记
            ShopManager.SetFristTick(false);
        }
        else if (code == -1)
        {
            LogMgr.Log("用户取消支付");
        }
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "spr_money":
                this.mSprMoney = tf.GetComponent<UISprite>();
                break;
            case "grid_award":
                this.mGridAward = tf.GetComponent<UIGrid>();
                break;
            case "jiangli_01":
                this.mItems[0] = this.BindItem<Item_SC>(tf.gameObject);
                break;
            case "jiangli_02":
                this.mItems[1] = this.BindItem<Item_SC>(tf.gameObject);
                break;
            case "jiangli_03":
                this.mItems[2] = this.BindItem<Item_SC>(tf.gameObject);
                break;
            case "jiangli_04":
                this.mItems[3] = this.BindItem<Item_SC>(tf.gameObject);
                break;
            case "jiangli_05":
                this.mItems[4] = this.BindItem<Item_SC>(tf.gameObject);
                break;
            case "dialog_pay":
                mDialog_pay = tf.gameObject;
                break;
        }
    }
}
