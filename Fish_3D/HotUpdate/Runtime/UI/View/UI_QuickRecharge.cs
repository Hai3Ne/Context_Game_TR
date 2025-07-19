using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuickRecharge : UILayer {
    public UILabel mLbBlue;
    public UILabel mLbRed;
    public UILabel mLbYellow;
    public UILabel mLbGold;
    public UILabel mLbBuy;
    public UILabel mLbTicks;

    private PayItem msg_pay;
    private VoidCall mCallCancel;
    public void InitData(int type, PayItem item, VoidCall call) {//1.乐豆不足  2.VIP等级不足
        this.msg_pay = item;
        this.mCallCancel = call;
        this.mLbGold.text = this.msg_pay.Score.ToString();
        this.mLbBuy.text = string.Format("购买(¥{0:0.##})",this.msg_pay.Price);

        if (type == 1) {
            this.mLbTicks.text = "乐豆不足,充点小钱娱乐一下!";
        } else {
            this.mLbTicks.text = "会员等级不足,充点小钱娱乐一下!";
        }

        this.mLbBlue.text = string.Empty;
        this.mLbRed.text = string.Empty;
        this.mLbYellow.text = string.Empty;

        if (this.msg_pay.MemberOrder > 0) {
            switch (this.msg_pay.MemberOrder) {
                case 1://蓝钻
                    this.mLbBlue.text = string.Format("{0}天{1}会员", this.msg_pay.MemberDays, ConstValue.VIPName[this.msg_pay.MemberOrder]);
                    break;
                case 2://黄钻
                    this.mLbYellow.text = string.Format("{0}天{1}会员", this.msg_pay.MemberDays, ConstValue.VIPName[this.msg_pay.MemberOrder]);
                    break;
                default://红钻
                    this.mLbRed.text = string.Format("{0}天{1}会员", this.msg_pay.MemberDays, ConstValue.VIPName[this.msg_pay.MemberOrder]);
                    break;
            }
        }
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_buy":
                if (HallHandle.CheckPerfect(false)) {
                    MainEntrace.Instance.ShowLoad("支付中...", 5);
                    SDKMgr.Instance.StartPay((errr_code) => {
                        TimeManager.AddCallThread(() => {
                            MainEntrace.Instance.HideLoad();
                            LogMgr.LogError("pay_result_code:" + errr_code.ToString());
                            if (errr_code == 0) {
                                MainEntrace.Instance.ShowLoad("支付确认中...", 5);
                                // PayManager.AddFinishOrder(PayManager.mPreOrder);
                                this.Close();
                            }
                        });
                    }, this.msg_pay, HallHandle.Accounts,PlatormEnum.IOS_WX);
                } else {
                    SystemMessageMgr.Instance.ShowMessageBox("请返回大厅完善个人信息");
                }
                break;
            case "btn_close":
                this.Close();
                if (this.mCallCancel != null) {
                    this.mCallCancel();
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_blue":
                this.mLbBlue = tf.GetComponent<UILabel>();
                break;
            case "lb_red":
                this.mLbRed = tf.GetComponent<UILabel>();
                break;
            case "lb_yellow":
                this.mLbYellow = tf.GetComponent<UILabel>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "lb_buy":
                this.mLbBuy = tf.GetComponent<UILabel>();
                break;
            case "lb_ticks":
                this.mLbTicks = tf.GetComponent<UILabel>();
                break;
        }
    }
}
