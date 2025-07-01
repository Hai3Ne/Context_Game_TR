using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Battle_Item : UIItem {
    public UISprite mSprIcon;
    public UISprite mSprMask;
    public UISprite mSprFrame;
    public UILabel mLbCD;
    public UILabel mLbCount;
    public UISprite mSprGold;
    public UILabel mLbUserCount;//可使用次数

    public ItemListPanel ui;
    public uint mItemCfgID;
    public ItemsVo mItemVo;
    public int mUserCount;//可使用次数
    public float mUserTime;//可用倒计时

    //状态相关
    public bool isCDStats;
    public bool isUsing;
    public SkillVo skillVo;
    public float time;

    public ItemCachInfo mItemInfo;
    private BulletBufferData mBuffData;
    private float buff_time;//buff剩余时间

    public void InitData(ItemListPanel panel, uint item_cfg_id) {
        this.ui = panel;
        this.mItemCfgID = item_cfg_id;
        this.mItemVo = FishConfig.Instance.Itemconf.TryGet(this.mItemCfgID);

        this.mSprIcon.spriteName = this.mItemVo.ItemIcon;
        this.mSprFrame.spriteName = ConstValue.ItemFrameSpList[this.mItemVo.Star];

        this.mSprMask.gameObject.SetActive(false);
        this.mLbCD.text = string.Empty;
        this.RefershInfo();

        if (this.mItemVo.GainMax > 0) {
            this.mLbUserCount.gameObject.SetActive(true);
        } else {
            this.mLbUserCount.gameObject.SetActive(false);
        }

        UI_Ticks.AddTickListener(this.gameObject, StringTable.GetString(this.mItemVo.ItemDec), SpriteAlignment.BottomRight, this.mItemVo.SalePrice);
    }

    public void RefershInfo() {
        this.mItemInfo = RoleItemModel.Instance.GetItemInfo(this.mItemCfgID);
        //if (this.mItemInfo.Count == 0 && this.mItemVo.SaleNum > 0) {//数量为0，且可购买状态，显示购买金币
        //    if (this.mItemVo.SalePrice >= 10000) {
        //        this.mLbCount.text = (this.mItemVo.SalePrice / 10000f).ToString("0.#");
        //    } else {
        //        this.mLbCount.text = this.mItemVo.SalePrice.ToString();
        //    }
        //    this.mLbCount.transform.localPosition = new Vector3(26, -18);
        //    this.mSprGold.gameObject.SetActive(true);
        //} else {
            this.mLbCount.text = string.Format("x{0}", this.mItemInfo.Count);
            this.mLbCount.transform.localPosition = new Vector3(38, -18);
            this.mSprGold.gameObject.SetActive(false);
        //}
        if (this.mItemVo.GainMax > 0) {
            this.mLbUserCount.text = this.mItemInfo.ItemUseTime.ToString();
        }
    }
    public void SetCD(float cd) {//设置道具CD
        if (this.skillVo == null) {
            uint skillID = (uint)this.mItemVo.Value0;
            this.skillVo = FishConfig.Instance.SkillConf.TryGet(skillID);
        }
        this.isCDStats = true;
        this.time = cd;
    }
    public void UserItemResult(bool is_succ) {
        if (is_succ) {
            this.isUsing = false;
            this.isCDStats = true;
            this.time = this.skillVo.CD;
            this.mBuffData = null;
        } else {
            this.isUsing = false;
            this.isCDStats = false;
        }
    }

    private float _progress = 0;
    private float _cd;
    public void Update() {
        this._progress = 0;
        this._cd = 0;
        if (this.mItemInfo != null && this.mItemInfo.ItemUseTime < this.mItemVo.GainMax) {
            this.mItemInfo = RoleItemModel.Instance.GetItemInfo(this.mItemCfgID);
            this.mItemInfo.ItemIncTick -= Time.deltaTime;
            if (this.mItemInfo.ItemIncTick < 0) {
                this.mItemInfo.ItemUseTime++;
                if (this.mItemInfo.ItemUseTime < this.mItemVo.GainMax) {
                    this.mItemInfo.ItemIncTick += this.mItemVo.GainCD;
                } else {
                    this.mItemInfo.ItemIncTick = this.mItemVo.GainCD;
                }
                this.RefershInfo();
            }
            if (this.mItemInfo.ItemUseTime > 0) {//有可用次数时，不显示倒计时
                this._progress = 0;
                this._cd = 0;
            } else {
                this._cd = this.mItemInfo.ItemIncTick;
                this._progress = this._cd / this.mItemVo.GainCD;
            }
        }
        if (this.isUsing == false && this.isCDStats == true) {
            this.time -= Time.deltaTime;

            float _p_ = 0;
            if (this.skillVo.BBuffID > 0 && this.mBuffData == null) {
                this.mBuffData = SceneLogic.Instance.BulletMgr.FindBBufferByID(SceneLogic.Instance.PlayerMgr.MyClientSeat, this.skillVo.BBuffID);
            }
            if (this.mBuffData != null) {
                this.buff_time = this.mBuffData.duration - (Time.realtimeSinceStartup - this.mBuffData.startTime);
                _p_ = this.buff_time / this.mBuffData.duration;
                if (this.mBuffData.mBbufferVo.BulletNum > 0) {
                    _p_ = Mathf.Min(_p_, this.mBuffData.bulletNum * 1f / this.mBuffData.mBbufferVo.BulletNum);
                }
            }
            _p_ = Mathf.Max(_p_, this.time / this.skillVo.CD);
            this._cd = Mathf.Max(this.buff_time, this.time);
            if (_p_ <= 0) {
                this.time = this.skillVo.CD;
                this.isCDStats = false;
            }
            this._progress = Mathf.Max(this._progress, _p_);
            this._cd = Mathf.Max(this._cd, Mathf.Max(this.buff_time, this.time));
        }
        if (this._progress > 0) {
            if (this.mSprMask.gameObject.activeSelf == false) {
                this.mSprMask.gameObject.SetActive(true);
                this.mLbCD.gameObject.SetActive(true);
                this.mSprMask.type = UIBasicSprite.Type.Filled;
            }

            this.mSprMask.fillAmount = this._progress;
            this.mLbCD.text = Mathf.FloorToInt(this._cd).ToString();
        } else if (this.mSprMask.gameObject.activeSelf) {
            this.mSprMask.gameObject.SetActive(false);
            this.mLbCD.text = string.Empty;
        }
    }

    public override void OnButtonClick(GameObject obj) {
        if (this.gameObject == obj) {
            this.ui.UserItem(this, GameConfig.OP_QuickBuy);
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_icon":
                this.mSprIcon = tf.GetComponent<UISprite>();
                break;
            case "item_spr_mask":
                this.mSprMask = tf.GetComponent<UISprite>();
                break;
            case "item_spr_frame":
                this.mSprFrame = tf.GetComponent<UISprite>();
                break;
            case "item_lb_cd":
                this.mLbCD = tf.GetComponent<UILabel>();
                break;
            case "item_lb_count":
                this.mLbCount = tf.GetComponent<UILabel>();
                break;
            case "item_lb_use_count":
                this.mLbUserCount = tf.GetComponent<UILabel>();
                break;
            case "item_spr_gold":
                this.mSprGold = tf.GetComponent<UISprite>();
                break;
        }
    }
}
