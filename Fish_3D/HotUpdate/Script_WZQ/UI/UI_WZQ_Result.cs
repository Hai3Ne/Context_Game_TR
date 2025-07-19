using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WZQ_Result : UILayer {
    public UILabel mLbName1;
    public UITexture mTexturePlayer1;
    public UILabel mLbGold1;
    public UILabel mLbName2;
    public UITexture mTexturePlayer2;
    public UILabel mLbGold2;
    public UILabel mLbDownCount;//隐藏倒计时
    public GameObject mObjWin;//胜利标识
    public GameObject mObjLose;//失败标识
    public GameObject mObjDraw;//平局
    private float mDownCount;//关闭倒计时
    public void InitData(CMD_S_GameEnd game_end, RoleInfo[] roles) {
        this.mObjWin.SetActive(false);
        this.mObjLose.SetActive(false);
        this.mObjDraw.SetActive(false);
        if (RoleManager.Self.ChairSeat == game_end.WinUser) {
            this.mObjWin.SetActive(true);
        } else if (game_end.WinUser == ushort.MaxValue) {
            this.mObjDraw.SetActive(true);
        } else {
            this.mObjLose.SetActive(true);
        }

        mLbName1.text = GameUtils.SubStringByWidth(mLbName1, roles[0].NickName, 240);
        this.mTexturePlayer1.uvRect = GameUtils.FaceUVRect(roles[0].FaceID);
        if (game_end.UserScore[0] >= 0) {
            mLbGold1.text = string.Format("+{0}乐豆", game_end.UserScore[0]);
        } else {
            mLbGold1.text = string.Format("{0}乐豆", game_end.UserScore[0]);
        }

        mLbName2.text = GameUtils.SubStringByWidth(mLbName2, roles[1].NickName, 240);
        this.mTexturePlayer2.uvRect = GameUtils.FaceUVRect(roles[1].FaceID);
        if (game_end.UserScore[1] >= 0) {
            mLbGold2.text = string.Format("+{0}乐豆", game_end.UserScore[1]);
        } else {
            mLbGold2.text = string.Format("{0}乐豆", game_end.UserScore[1]);
        }
        this.mDownCount = 4.99f;
        this.SetDownCount((int)this.mDownCount);
    }
    public void SetDownCount(int time) {
        this.mLbDownCount.text = time.ToString();
    }

    public void Update() {
        if (this.mDownCount > Time.deltaTime) {
            this.mDownCount -= Time.deltaTime;
            this.SetDownCount((int)this.mDownCount);
        } else {
            this.Close();
        }
    }
    
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok":
                this.Close();
                break;
            default:
                return;
        }
        AudioManager.PlayAudio(GameEnum.WZQ, WZQGameConfig.Audio_ClickBtn);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_name_1":
                this.mLbName1 = tf.GetComponent<UILabel>();
                break;
            case "texture_player_1":
                this.mTexturePlayer1 = tf.GetComponent<UITexture>();
                break;
            case "lb_gold_1":
                this.mLbGold1 = tf.GetComponent<UILabel>();
                break;
            case "lb_name_2":
                this.mLbName2 = tf.GetComponent<UILabel>();
                break;
            case "texture_player_2":
                this.mTexturePlayer2 = tf.GetComponent<UITexture>();
                break;
            case "lb_gold_2":
                this.mLbGold2 = tf.GetComponent<UILabel>();
                break;
            case "lb_downcount":
                this.mLbDownCount = tf.GetComponent<UILabel>();
                break;
            case "item_spr_win":
                this.mObjWin = tf.gameObject;
                break;
            case "item_spr_lose":
                this.mObjLose = tf.gameObject;
                break;
            case "item_spr_draw":
                this.mObjDraw = tf.gameObject;
                break;
        }
    }
}
