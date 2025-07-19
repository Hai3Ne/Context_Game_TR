using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_UserList_Item : UIItem {
    public UISprite mSprVip;
    public UILabel mLbName;
    public UILabel mLbGold;
    public UILabel mLbLv;//级别

    public RoleInfo mRoleInfo;
    public void InitData(RoleInfo role) {
        this.mRoleInfo = role;

        this.mSprVip.spriteName = SHGameConfig.VIPIcons[role.MemberOrder];
        this.mLbName.text = role.NickName;
        this.mLbGold.text = role.GoldNum.ToString();
        this.mLbLv.text = SHGameConfig.GetLvStr(role.GoldNum);
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_vip":
                this.mSprVip = tf.GetComponent<UISprite>();
                break;
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_lv":
                this.mLbLv = tf.GetComponent<UILabel>();
                break;
        }
    }
}