using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_BetList_Item : UIItem {
    public UISprite mSprGold;
    public UILabel mLbGold;
    public UISprite mSprSelectTick;

    public Item_ShenHua_BetList mParent;
    public long mSelectGold;
    public bool mIsEnable;
    public void InitData(Item_ShenHua_BetList parent, long gold) {
        this.mParent = parent;
        this.mSelectGold = gold;
        if (this.mSelectGold % 10000 == 0) {
            this.mLbGold.text = string.Format("{0}万", this.mSelectGold / 10000);
        } else if (this.mSelectGold % 1000 == 0) {
            this.mLbGold.text = string.Format("{0}千", this.mSelectGold / 1000);
        } else {
            this.mLbGold.text = this.mSelectGold.ToString();
        }
        this.mSprGold.spriteName = SHGameConfig.GetGoldStr(this.mSelectGold);
        this.mSprGold.MakePixelPerfect();
        this.SetEnable(false);
    }

    public void SetEnable(bool is_enable) {
        this.mIsEnable = is_enable;
        if (is_enable == false) {
            this.SetSelect(false);
        }
        this.mSprGold.IsGray = is_enable == false;
        this.mLbGold.IsGray = is_enable == false;
    }

    public void SetSelect(bool is_select) {
        this.mSprSelectTick.gameObject.SetActive(is_select);
    }

    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_select":
                if (this.mIsEnable == true) {
                    this.mParent.SelectItem(this);
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_icon":
                this.mSprGold = tf.GetComponent<UISprite>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_spr_select_tick":
                this.mSprSelectTick = tf.GetComponent<UISprite>();
                break;
        }
    }
}
