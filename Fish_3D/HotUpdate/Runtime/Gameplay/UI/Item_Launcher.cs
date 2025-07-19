using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Launcher : UIItem {
    public UISprite mSprIcon;
    public GameObject mObjSelect;
    public GameObject mObjLock;
    public UILabel mLbVIP;
    public UISprite mSprVIP;

    public LauncherVo mVo;
    public uint mCfgID;
    public VoidCall<Item_Launcher> mCall;

    public void InitData(uint cfg_id) {
        this.mCfgID = cfg_id;
        this.mVo = FishConfig.Instance.GetLauncherByConfig(cfg_id);
        this.mSprIcon.spriteName = this.mVo.Icon.ToString();

        if (this.mVo.VIPLevel > RoleInfoModel.Instance.Self.VipLv) {//未解锁
            this.mObjLock.gameObject.SetActive(true);
            this.mLbVIP.text = string.Format("{0}解锁",ConstValue.VIPName[this.mVo.VIPLevel]);
            this.mSprVIP.spriteName = string.Format("vip_{0}", this.mVo.VIPLevel);
        } else {
            this.mObjLock.gameObject.SetActive(false);
        }
    }

    public void SetSelect(bool is_select) {
        this.mObjSelect.SetActive(is_select);
    }
    public void SetCall(VoidCall<Item_Launcher> call) {
        this.mCall = call;
    }

    public override void OnButtonClick(GameObject obj) {
        if (this.gameObject == obj) {
            if (this.mVo.VIPLevel > RoleInfoModel.Instance.Self.VipLv) {//未解锁


                PayItem item = ShopManager.GetPayByVIP(this.mVo.VIPLevel);
                if (item != null)
                {
                    //UI.EnterUI<UI_QuickRecharge>(ui => 
                    //{
                    //    ui.InitData(2, item, null);//会员等级不足
                    //});

                    UI.EnterUI<UI_QuickRecharge>(GameEnum.All).InitData(2, item, null);
                }
                else
                {
                    SystemMessageMgr.Instance.ShowMessageBox(string.Format("需要{0}会员才可使用", ConstValue.VIPName[this.mVo.VIPLevel]));
                }

                //UI.EnterUI<UI_poor>(ui => {
                //    ui.InitData(2);//VIP等级不足
                //});
                //SystemMessageMgr.Instance.ShowMessageBox(string.Format("需要{0}会员才可使用", ConstValue.VIPName[this.mVo.VIPLevel]));
            } else {
                if (this.mCall != null) {
                    this.mCall(this);
                }
            }
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "spr_icon":
                this.mSprIcon = tf.GetComponent<UISprite>();
                break;
            case "spr_select":
                this.mObjSelect = tf.gameObject;
                break;
            case "locked":
                this.mObjLock = tf.gameObject;
                break;
            case "lb_vip":
                this.mLbVIP = tf.GetComponent<UILabel>();
                break;
            case "spr_vip":
                this.mSprVIP = tf.GetComponent<UISprite>();
                break;
        }
    }
}
