using UnityEngine;
using System.Collections;

public class Item_GetAward : MonoBehaviour {
    public UISprite mSprIcon;
    public UILabel mLbName;
    //public UILabel mLbCount;
    public UISprite mSprFrame;

    public void InitData(ItemsVo item,uint count) {
        this.mSprIcon.spriteName = item.ItemIcon;
        if (item.ItemType == (byte)EnumItemType.Hero) {
            this.mSprFrame.spriteName = ConstValue.HeroFrameSpList[item.Star];
        } else {
            this.mSprFrame.spriteName = ConstValue.ItemFrameSpList[item.Star];
        }
       // this.mSprIcon.MakePixelPerfect();
       // this.mSprFrame.MakePixelPerfect();

		this.mLbName.text = StringTable.GetString (item.ItemName) + (count > 0 ? " x " + count : "");
        //if (count > 1) {
        //    this.mLbCount.gameObject.SetActive(true);
        //    this.mLbCount.text = count.ToString();
        //} else {
        //    this.mLbCount.gameObject.SetActive(false);
        //}
    }
}
