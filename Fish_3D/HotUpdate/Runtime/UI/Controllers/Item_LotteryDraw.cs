using UnityEngine;
using System.Collections;

public class Item_LotteryDraw : MonoBehaviour {
    public GameObject mObjChoose;
    public UILabel mLbChooseNum;
    public UISprite mSprItemIcon;
    public UISprite mSprItemFrame;
    public UILabel mLbItemName;
    public GameObject mObjSelectTick;//指针划过提示

    public UI_LotteryDrawController.ItemData data;

    public void InitData() {
        GameUtils.Traversal(this.transform, this.OnNodeAsset);
        this.mObjChoose.SetActive(false);
        this.mLbChooseNum.text = string.Empty;
    }
    public void SetItem(UI_LotteryDrawController.ItemData data) {
        ItemsVo itmVo = FishConfig.Instance.Itemconf.TryGet(data.itemCfgId);
        if (itmVo == null) {
            LogMgr.LogError("itemCfgId :" + data.itemCfgId);
            return;
        }
        this.mSprItemIcon.spriteName = itmVo.ItemIcon;
        if (itmVo.ItemType == (byte)EnumItemType.Hero) {
            this.mSprItemFrame.spriteName = ConstValue.HeroFrameSpList[itmVo.Star];
        } else {
            this.mSprItemFrame.spriteName = ConstValue.ItemFrameSpList[itmVo.Star];
        }
        this.mLbItemName.text = StringTable.GetString(itmVo.ItemName) + "x" + GameUtils.FormatGoldNum((int)data.count);
        this.data = data;
    }
    public void SetShowTick(bool is_show) {
        if (this.mObjSelectTick.activeSelf != is_show) {
            this.mObjSelectTick.SetActive(is_show);
        }
    }
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_icon":
                this.mSprItemIcon = tf.GetComponent<UISprite>();
                break;
            case "item_spr_frame":
                this.mSprItemFrame = tf.GetComponent<UISprite>();
                break;
            case "item_lb_name":
                this.mLbItemName = tf.GetComponent<UILabel>();
                break;
            case "item_choose":
                this.mObjChoose = tf.gameObject;
                break;
            case "item_choose_num":
                this.mLbChooseNum = tf.GetComponent<UILabel>();
                break;
            case "item_spr_select":
                this.mObjSelectTick = tf.gameObject;
                this.mObjSelectTick.SetActive(false);
                break;
        }
    }
}
