using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_wzqmain_Room : UIItem {
    public UILabel mLbGold;//进入房间要求

    private UI_wzqmain ui;
    private tagGameServer server;
    public void InitData(UI_wzqmain ui, tagGameServer server) {
        this.ui = ui;
        this.server = server;
        if (server.MinEnterScore > 0) {
            if (server.MinEnterScore % 10000 == 0) {
                this.mLbGold.text = string.Format("{0}万乐豆以上", server.MinEnterScore / 10000);
            } else {
                this.mLbGold.text = string.Format("{0}乐豆以上", server.MinEnterScore);
            }
        } else {
            this.mLbGold.text = "免费进入";
        }
    }
    public override void OnButtonClick(GameObject obj) {
        if (obj == this.gameObject) {
            this.ui.EnterGame(this.server);
            AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
        }
    }
}
