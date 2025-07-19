using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Shop : UIItem {
    public GameObject mObjTuiJian;
    public UISprite mSprIcon;//金币图标
    public UILabel mLbMoney;//支付金额
    public UILabel mLbGold;//乐豆数量
    public UILabel mLbVIP;//会员天数提示

    public PayItem msg_pay;
    private Action<PayItem> OpenPayWindow;
    public void InitData(PayItem item,Action<PayItem> onOpenPayWin)
    {
        OpenPayWindow = onOpenPayWin;
        this.msg_pay = item;
        this.mObjTuiJian.SetActive(false);
        //不同等级的乐豆，显示图片不同
        if (item.Score < 200000) {
            this.mSprIcon.spriteName = "gold1";
        } else if (item.Score < 1000000) {
            this.mSprIcon.spriteName = "gold2";
        } else if (item.Score < 5000000) {
            this.mSprIcon.spriteName = "gold3";
        } else {
            this.mSprIcon.spriteName = "gold4";
        }
        this.mSprIcon.MakePixelPerfect();
        this.mLbMoney.text = string.Format("{0:0.##}",this.msg_pay.Price);
        this.mLbGold.text = string.Format("{0}", this.msg_pay.Score);

        if (this.msg_pay.MemberOrder > 0) {
            this.mLbVIP.text = string.Format("{0}天{1}会员", this.msg_pay.MemberDays, ConstValue.VIPName[this.msg_pay.MemberOrder]);
            switch (this.msg_pay.MemberOrder) {
                case 1://蓝钻
                    this.mLbVIP.gradientTop = new Color32(216,234,255,255);
                    this.mLbVIP.gradientBottom = new Color32(132,189,255,255);
                    this.mLbVIP.effectColor = new Color32(35,39,130,255);
                    break;
                case 2://黄钻
                    this.mLbVIP.gradientTop = new Color32(255, 247, 216, 255);
                    this.mLbVIP.gradientBottom = new Color32(255, 213, 100, 255);
                    this.mLbVIP.effectColor = new Color32(123, 88, 3, 255);
                    break;
                default://红钻
                    this.mLbVIP.gradientTop = new Color32(255,196,178,255);
                    this.mLbVIP.gradientBottom = new Color32(255,96,96,255);
                    this.mLbVIP.effectColor = new Color32(98,0,0,255);
                    break;
            }
        } else {
            this.mLbVIP.text = string.Empty;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            //打开支付窗口选择支付方式
            if (OpenPayWindow != null)
                OpenPayWindow(msg_pay);
        }
    }

    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_tuijian":
                this.mObjTuiJian = tf.gameObject;
                break;
            case "item_spr_icon":
                this.mSprIcon = tf.GetComponent<UISprite>();
                break;
            case "item_lb_monry":
                this.mLbMoney = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_vip":
                this.mLbVIP = tf.GetComponent<UILabel>();
                break;
        }
    }
}
