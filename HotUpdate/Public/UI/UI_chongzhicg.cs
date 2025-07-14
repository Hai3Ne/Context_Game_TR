using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_chongzhicg : UILayer {
    public Item_chongzhicg item_vip;
    public Item_chongzhicg item_gold;
    public UILabel mLbPwdTick;

    public void InitData(PayItem pay) {
        if (pay.MemberOrder > 0 && pay.MemberDays > 0) {
            this.item_vip.InitData_VIP(pay.MemberOrder, pay.MemberDays);
        } else {
            this.item_vip.gameObject.SetActive(false);
            Vector3 pos = this.item_gold.transform.localPosition;
            pos.x = 0;
            this.item_gold.transform.localPosition = pos;
        }
        if (pay.Score > 0) {
            this.item_gold.InitData(2001u, pay.Score);
        } else {
            this.item_gold.gameObject.SetActive(false);
            Vector3 pos = this.item_vip.transform.localPosition;
            pos.x = 0;
            this.item_vip.transform.localPosition = pos;
        }
    }
    public override void OnNodeLoad() {
        if (string.IsNullOrEmpty(HallHandle.PassPortID) == false) {//已完善信息
            this.mLbPwdTick.text = StringTable.GetString("Tip_41");
        } else {
            this.mLbPwdTick.text = StringTable.GetString("Tip_40");
        }
    }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_zdl":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_vip":
                this.item_vip = this.BindItem<Item_chongzhicg>(tf.gameObject);
                break;
            case "item_gold":
                this.item_gold = this.BindItem<Item_chongzhicg>(tf.gameObject);
                break;
            case "lb_pwd_tick":
                this.mLbPwdTick = tf.GetComponent<UILabel>();
                break;
        }
    }
}
