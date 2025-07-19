using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_LK_Battle_Gold : UIItem {
    public UILabel mLbGold;
    public UISprite mSprPostBG1;//绿色
    public UISprite mSprPostBG2;//红色
    public UISprite mSprPost;
    public UISpriteAnimation mAnimPost;//金币上顶动画

    public int mGold;
    public int mLayer;
    private Item_LK_Battle item_battle;
    private int mHeight;

    public void InitData(Item_LK_Battle item,bool is_red, int gold, int layer) {
        this.item_battle = item;
        this.mGold = gold;
        this.mLayer = layer;
        this.mLbGold.text = gold.ToString();
        this.mSprPostBG1.gameObject.SetActive(is_red == false);
        this.mSprPostBG2.gameObject.SetActive(is_red == true);
        this.mAnimPost.gameObject.SetActive(true);
        this.mLbGold.gameObject.SetActive(false);
        this.mHeight = layer * 6;
        this.mSprPost.height = 0;
        this.mLbGold.alpha = 1;
        this.mSprPost.alpha = 1;
        this._time = 0;
    }

    private float _time;
    public void Update() {
        this._time += Time.deltaTime;

        if (this.mSprPost.height < this.mHeight) {
            int height = (int)(this.mSprPost.height + 300 * Time.deltaTime);
            if (height >= this.mHeight) {
                this.mSprPost.height = this.mHeight;
                this.mAnimPost.gameObject.SetActive(false);
                this.mLbGold.gameObject.SetActive(true);
            } else {
                this.mSprPost.height = height;
            }
        }

        if (this._time > 3) {
            float alpha = (3.5f - this._time) * 2;
            if (alpha > 0) {
                this.mLbGold.alpha = alpha;
                this.mSprPost.alpha = alpha;
            } else {
                this.mLbGold.alpha = 0;
                this.mSprPost.alpha = 0;
                this.item_battle.RemvoeGoldPost(this);
            }
        }
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_post":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_spr_post_1":
                this.mSprPostBG1 = tf.GetComponent<UISprite>();
                break;
            case "item_spr_post_2":
                this.mSprPostBG2 = tf.GetComponent<UISprite>();
                break;
            case "item_spr_post":
                this.mSprPost = tf.GetComponent<UISprite>();
                break;
            case "item_anim_post":
                this.mAnimPost = tf.GetComponent<UISpriteAnimation>();
                break;
        }
    }
}
