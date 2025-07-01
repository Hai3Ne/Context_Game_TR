using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_LeiZhu_Item : UIItem {
    public UILabel mLbName;
    public UILabel mLbGold;

    [HideInInspector]
    public RoleInfo mRole;

    public void InitData(RoleInfo role)
    {
        mRole = role;
        this.mLbName.text = GameUtils.SubStringByWidth(this.mLbName, role.NickName, 150);
        this.mLbGold.text = role.GoldNum.ToString();
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "_item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "_item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
        }
    }
}
