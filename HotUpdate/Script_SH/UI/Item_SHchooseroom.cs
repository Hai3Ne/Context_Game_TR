using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_SHchooseroom : UIItem {
    private string[] imgs = { "shadow_Dragon-min", "sahdow_Basaltic-min", "shadow_Suzaku-min", "shadow_Tiger-min" };
    public UISprite mSprImg;
    public UILabel mLbName;
    public UILabel mLbGold;
    public UILabel mLbGoldTick;

    public tagGameServer msg_server;

    public void InitData(int index, tagGameServer server) {
        this.msg_server = server;
        this.mSprImg.spriteName = imgs[index % imgs.Length];
        //if (server.ServerName.Length > 3)
        //{
        //    this.mLbName.text = server.ServerName.Replace("神话", "");
        //}
        //else
        //{
        //    this.mLbName.text = server.ServerName;
        //}
        if (server.MinEnterScore > 0)
        {
            this.mLbGold.text = server.MinEnterScore.ToString();

            if (server.MinEnterScore >= 100000 && server.MinEnterScore < 500000)
                mLbName.text = "初级房";
            else if (server.MinEnterScore >= 500000 && server.MinEnterScore <= 1000000)
                mLbName.text = "中级房";
            else
                mLbName.text = "高级房";
        }
        else
        {
            mLbName.text = "新手房";
            this.mLbGold.text = string.Empty;
            this.mLbGoldTick.text = "免费进入";
            this.mLbGoldTick.transform.localPosition = (this.mLbGoldTick.transform.localPosition + this.mLbGold.transform.localPosition) * 0.5f;
        }
    }

    public override void OnButtonClick(GameObject obj) {
        if (obj == this.gameObject) {
            GameManager.EnterGame(GameEnum.SH, this.msg_server);
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_img":
                this.mSprImg = tf.GetComponent<UISprite>();
                break;
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold_tick":
                this.mLbGoldTick = tf.GetComponent<UILabel>();
                break;
        }
    }
}
