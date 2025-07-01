using UnityEngine;
using System.Collections;

public class Item_TuJian_Fish : MonoBehaviour {
    public UILabel mLbName;//名称
    public UILabel mLbVal;//价值
    public UILabel mLbRandomVal;//随机倍率提示
    public UISprite mSprFish;//图标
    public UILabel mLbStar;//难度星级

    private FishBookVo mVo;
    public void Awake() {
        this.mLbName = this.transform.Find("item_lb_name").GetComponent<UILabel>();
        this.mLbVal = this.transform.Find("item_lb_val").GetComponent<UILabel>();
        this.mSprFish = this.transform.Find("item_spr_fish").GetComponent<UISprite>();
        this.mLbRandomVal = this.transform.Find("item_lb_random").GetComponent<UILabel>();
        this.mLbStar = this.transform.Find("item_lb_star").GetComponent<UILabel>();

        UIEventListener.Get(this.transform.Find("item_btn_frame").gameObject).onClick = this.OnButtonClick;
    }

    public void InitData(FishBookVo vo) {
        this.mVo = vo;

        this.mLbName.text = StringTable.GetString(this.mVo.PicName);
        if (this.mVo.Multiple == 0) {//随机倍率
            this.mLbVal.text = string.Empty;
            this.mLbRandomVal.text = StringTable.GetString("Tip_30");// "随机倍率";
        } else if (this.mVo.MultipleMax == 0) {//固定倍率
            this.mLbVal.text = this.mVo.Multiple.ToString();
            this.mLbRandomVal.text = string.Empty;
        } else {
            this.mLbVal.text = string.Format("{0}~{1}", this.mVo.Multiple, this.mVo.MultipleMax);
            this.mLbRandomVal.text = string.Empty;
        }
        this.mSprFish.spriteName = this.mVo.ResName.ToString();
        this.mSprFish.MakePixelPerfect();
        this.SetDifficulty(this.mVo.Difficulty);
    }
    private void SetDifficulty(uint difficulty) {
        switch (difficulty) {
            case 1:
                this.mLbStar.text = "★";
                break;
            case 2:
                this.mLbStar.text = "★★";
                break;
            case 3:
                this.mLbStar.text = "★★★";
                break;
            case 4:
                this.mLbStar.text = "★★★★";
                break;
            case 5:
                this.mLbStar.text = "★★★★★";
                break;
            default:
                this.mLbStar.text = string.Empty;
                break;
        }
    }
    public void OnButtonClick(GameObject obj) {
        //打开鱼详情界面
        if (WndManager.Instance.isActive(EnumUI.UI_FishInfo) == false) {
            WndManager.Instance.ShowUI(EnumUI.UI_FishInfo, this.mVo);
        }
    }
}
