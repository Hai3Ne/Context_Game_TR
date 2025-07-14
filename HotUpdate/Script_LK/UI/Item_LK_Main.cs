using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_LK_Main : UILayer {
    private static string[] imgs = { "icon_xsf-min", "icon_cjf-min", "icon_hjw-min", "icon_byf-min", "icon_fcg-min", "icon_zsh-min" };
    public UISprite mSprRoom;
    public UILabel mLbGoldTick;//进入条件
    public UILabel mLbName;//房间名称

    public tagGameServer msg_server;

    public void InitData(int index, tagGameServer server) {
        this.msg_server = server;

        if (server.MinEnterMember > 0)
        {
            this.mSprRoom.spriteName = "icon_zsh-min";
            mLbName.text = "钻石会";
        }
        else if (server.MinTableScore >= 7500000)
        {
            this.mSprRoom.spriteName = "icon_zsh-min";
            mLbName.text = "钻石会";
        }
        else if (server.MinTableScore >= 2000000)
        {
            this.mSprRoom.spriteName = "icon_fcg-min";
            mLbName.text = "翡翠阁";
        }
        else if (server.MinTableScore >= 200000)
        {
            this.mSprRoom.spriteName = "icon_byf-min";
            mLbName.text = "白玉坊";
        }
        else if (server.MinTableScore >= 50000)
        {
            this.mSprRoom.spriteName = "icon_hjw-min";
            mLbName.text = "黄金屋";
        }
        else if (server.MinTableScore >= 5000)
        {
            this.mSprRoom.spriteName = "icon_cjf-min";
            mLbName.text = "初级房";
        }
        else
        {
            this.mSprRoom.spriteName = "icon_xsf-min";
            mLbName.text = "新手房";
        }
        this.mSprRoom.MakePixelPerfect();

        //if (server.ServerName.Length > 6) {
        //    this.mLbName.text = server.ServerName.Replace("李逵劈鱼", "");
        //} else {
        //    this.mLbName.text = server.ServerName;
        //}
        if (server.MinTableScore > 20000) {
            this.mLbGoldTick.text = string.Format("{0}万乐豆以上", server.MinTableScore / 10000);
        } else if (server.MinTableScore > 0) {
            this.mLbGoldTick.text = string.Format("{0}乐豆以上", server.MinEnterScore);
        } else if (server.MinEnterMember > 0) {
            this.mLbGoldTick.text = string.Format("{0}以上会员进入", ConstValue.VIPName[server.MinEnterScore]);
        } else {
            this.mLbGoldTick.text = "免费进入";
        }
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj)
    {
        if (obj == this.gameObject)
        {
            GameManager.EnterGame(GameEnum.Fish_LK, this.msg_server);
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_room":
                this.mSprRoom = tf.GetComponent<UISprite>();
                break;
            case "item_lb_tj":
                this.mLbGoldTick = tf.GetComponent<UILabel>();
                break;
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
        }
    }
}
