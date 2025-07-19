using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_QianDao : UIItem {
    public UILabel mLbWeek;
    public UISprite mSprSelect;
    public UISprite mSprGold;
    public UILabel mLbGold;
    public GameObject mObjGetTick;//已经签到提示
    public GameObject mVipBlue;//蓝钻双倍提示
    public GameObject mVipYellow;//黄钻双倍提示
    public GameObject mVipRed;//红钻双倍提示
    public GameObject mBtnSign;
    public UISprite mSprSign;
    public UILabel mLbSign;

    public bool mIsCur;//是否今天

    public void InitData(ulong gold, byte vip,bool is_cur) {
        this.mLbGold.text = string.Format("乐豆{0}", gold);
        this.mVipBlue.SetActive(false);
        this.mVipYellow.SetActive(false);
        this.mVipRed.SetActive(false);
        switch (vip) {
            case 1://蓝钻
                this.mVipBlue.SetActive(true);
                break;
            case 2://黄钻
                this.mVipYellow.SetActive(true);
                break;
            case 3://红钻
            case 4://红钻
                this.mVipRed.SetActive(true);
                break;
            //case 5://至尊
            //case 6://至尊
            //default:
            //    //this.mVipRed.SetActive(true);
            //    break;
        }
        this.SetCur(is_cur);
    }
    public void SetCur(bool is_cur) {//是否是今天
        this.mIsCur = is_cur;
        this.mSprSelect.gameObject.SetActive(is_cur);
    }
    public void SetShowGetTick(bool is_show) {//设置已领取提示
        this.mObjGetTick.SetActive(is_show);
        if (this.mIsCur) {
            this.mSprSign.GetComponent<BoxCollider>().enabled = is_show == false;
            this.mBtnSign.SetActive(true);
            this.mSprSign.IsGray = is_show;
            this.mLbSign.IsGray = is_show;
            if (is_show) {
                this.mLbSign.text = "已签到";
            } else {
                this.mLbSign.text = "签到";
            }
        } else {
            this.mBtnSign.SetActive(false);
        }
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_lingqu"://领取
                if (UserManager.IsBingWX) {
                    if (this.mSprGold.IsGray == false) {
                        HttpServer.Instance.Send<CMD_GP_WeekSign>(NetCmdType.SUB_GP_WEEKSIGN, new CMD_GP_WeekSign {
                            UserID = HallHandle.UserID,
                            UnionID = HallHandle.WXUnionID,
                        });
                    }
                } else
                {
                    UI.EnterUI<UI_BindNotice>(GameEnum.All).InitData(false);
                    //UI.EnterUI<UI_BindNotice>(ui => ui.InitData(false));
                }
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_get":
                this.mObjGetTick = tf.gameObject;
                break;
            case "item_spr_select":
                this.mSprSelect = tf.GetComponent<UISprite>();
                break;
            case "item_lb_week":
                this.mLbWeek = tf.GetComponent<UILabel>();
                break;
            case "item_spr_gold":
                this.mSprGold = tf.GetComponent<UISprite>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "vip_yellow":
                this.mVipYellow = tf.gameObject;
                break;
            case "vip_blue":
                this.mVipBlue = tf.gameObject;
                break;
            case "vip_red":
                this.mVipRed = tf.gameObject;
                break;
            case "btn_lingqu":
                this.mBtnSign = tf.gameObject;
                this.mSprSign = tf.GetComponent<UISprite>();
                break;
            case "lb_lingqu":
                this.mLbSign = tf.GetComponent<UILabel>();
                break;
        }
    }
}
