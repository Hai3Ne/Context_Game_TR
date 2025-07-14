using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Item_wzqmain_Dialog : UIItem {
    public UILabel[] mLbPwd = new UILabel[6];//6位密码

    public string mPwdTick = "输入房间密码";//
    public StringBuilder mInputPwd = new StringBuilder();
    public bool IsShow {
        get {
            return this.gameObject.activeSelf;
        }
    }
    public void Show() {
        this.gameObject.SetActive(true);
        this.Clear();
    }
    public void Hide() {
        this.gameObject.SetActive(false);
    }
    public void Clear() {
        this.mInputPwd.Remove(0, this.mInputPwd.Length);
        for (int i = 0; i < this.mLbPwd.Length; i++) {
            this.mLbPwd[i].text = mPwdTick[i].ToString();
        }
    }

    public void RemoveInput(int len) {
        if (this.mInputPwd.Length > len) {
            this.mInputPwd.Remove(this.mInputPwd.Length - len, len);
        } else {
            this.mInputPwd.Remove(0, this.mInputPwd.Length);
        }
        if (this.mInputPwd.Length > 0) {
            for (int i = this.mInputPwd.Length; i < this.mLbPwd.Length; i++) {
                this.mLbPwd[i].text = string.Empty;
            }
        } else {
            for (int i = this.mInputPwd.Length; i < this.mLbPwd.Length; i++) {
                this.mLbPwd[i].text = mPwdTick[i].ToString();
            }
        }
    }
    public void AddInput(int num) {
        if (this.mInputPwd.Length >= this.mLbPwd.Length) {
            return;
        }
        this.mInputPwd.Append(num.ToString());
        this.mLbPwd[this.mInputPwd.Length - 1].text = num.ToString();
        for (int i = this.mInputPwd.Length; i < this.mLbPwd.Length; i++) {
            this.mLbPwd[i].text = string.Empty;
        }

        if (this.mInputPwd.Length >= this.mLbPwd.Length) {//密码输入完毕后自动发送进入房间请求
            NetClient.Send(NetCmdType.SUB_GR_USER_JOIN_TABLE, new CMD_GR_UserJoinTableReq {
                szRandTableID = this.mInputPwd.ToString(),
            });
        }
    }

    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_close":
                this.Hide();
                break;
            case "item_btn_1":
                this.AddInput(1);
                break;
            case "item_btn_2":
                this.AddInput(2);
                break;
            case "item_btn_3":
                this.AddInput(3);
                break;
            case "item_btn_4":
                this.AddInput(4);
                break;
            case "item_btn_5":
                this.AddInput(5);
                break;
            case "item_btn_6":
                this.AddInput(6);
                break;
            case "item_btn_7":
                this.AddInput(7);
                break;
            case "item_btn_8":
                this.AddInput(8);
                break;
            case "item_btn_9":
                this.AddInput(9);
                break;
            case "item_btn_0":
                this.AddInput(0);
                break;
            case "item_btn_del":
                this.RemoveInput(1);
                break;
            case "item_btn_clear":
                this.Clear();
                break;
            default:
                return;
        }

        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_table_pwd_1":
                this.mLbPwd[0] = tf.GetComponent<UILabel>();
                break;
            case "item_lb_table_pwd_2":
                this.mLbPwd[1] = tf.GetComponent<UILabel>();
                break;
            case "item_lb_table_pwd_3":
                this.mLbPwd[2] = tf.GetComponent<UILabel>();
                break;
            case "item_lb_table_pwd_4":
                this.mLbPwd[3] = tf.GetComponent<UILabel>();
                break;
            case "item_lb_table_pwd_5":
                this.mLbPwd[4] = tf.GetComponent<UILabel>();
                break;
            case "item_lb_table_pwd_6":
                this.mLbPwd[5] = tf.GetComponent<UILabel>();
                break;
        }
    }
}
