using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_chongzhicg : UIItem {
    public UISprite mSprIcon;
    public UILabel mLbCount;
    public void InitData(uint item_id, long num) {
        ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(item_id);
        this.mSprIcon.spriteName = vo.ItemIcon;
        if (vo.ItemType == (byte)EnumItemType.Hero) {
            this.GetComponent<UISprite>().spriteName = ConstValue.HeroFrameSpList[vo.Star];
        } else {
            this.GetComponent<UISprite>().spriteName = ConstValue.ItemFrameSpList[vo.Star];
        }
        if (num < 10000) {
            this.mLbCount.text = string.Format("乐豆{0}", num);
        } else {
            this.mLbCount.text = string.Format("乐豆{0}万", num / 10000);
        }
    }
    public void InitData_VIP(byte vip, int day) {
        this.gameObject.SetActive(true);
        switch (vip) {
            case 1://蓝钻
                this.mSprIcon.spriteName = "blue";
                break;
            case 2://黄钻
                this.mSprIcon.spriteName = "yellow";
                break;
            case 3://红钻
            case 4://红钻
                this.mSprIcon.spriteName = "red";
                break;
            case 5://至尊
            case 6://至尊
            default:
                this.mSprIcon.spriteName = "red";
                break;
        }
        this.mLbCount.text = string.Format("{0}会员{1}天", ConstValue.VIPName[vip],day);
    }
    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_icon":
                this.mSprIcon = tf.GetComponent<UISprite>();
                break;
            case "item_lb_count":
                this.mLbCount = tf.GetComponent<UILabel>();
                break;
        }
    }
}
