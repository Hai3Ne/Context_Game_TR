using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_Setting : UILayer {
    public UISlider mSliderMusic;
    public UISlider mSliderSound;
    public GameObject mBtnShakeOn;
    public GameObject mBtnShakeOff;
    public GameObject mBtnVIPOn;
    public GameObject mBtnVIPOff;
    public GameObject[] mBtnChanges = new GameObject[LKGameConfig.MAXSEAT];//换座按钮

    public void InitData() {
        this.SetShakeOP(GameConfig.OP_Shake);
        this.SetVIPOP(LKRoleManager.mRoles[RoleManager.Self.ChairSeat].IsVipCannon,false);

        for (int i = 0; i < LKGameConfig.MAXSEAT; i++) {
            this.mBtnChanges[i].SetActive(UI_LK_Battle.ui.mItems[i].mRoleInfo == null);
        }
    }
    private void onMusicChange() {//音乐
        AudioManager.MusicVolume = this.mSliderMusic.value;
    }
    private void onAudioChange() {//音效
        AudioManager.AudioVolume = this.mSliderSound.value;
    }
    private void SetShakeOP(bool open) {//震动开关
        this.mBtnShakeOn.SetActive(open);
        this.mBtnShakeOff.SetActive(open == false);
        GameConfig.OP_Shake = open;
        GameConfig.SaveData();
    }
    private void SetVIPOP(bool open,bool is_send) {//会员炮开关
        this.mBtnVIPOn.SetActive(open);
        this.mBtnVIPOff.SetActive(open == false);
        //切换会员炮请求
        if (is_send) {
            NetClient.Send(NetCmdType.SUB_C_CLIENT_CFG_LKPY, new CMD_C_ClientCfg_lkpy {
                cfg_type = 1,
                cfg = open ? 1 : 0,
            });
        }
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() {
        this.mSliderMusic.value = AudioManager.MusicVolume;
        this.mSliderSound.value = AudioManager.AudioVolume;
        EventDelegate.Add(this.mSliderMusic.onChange, onMusicChange);
        EventDelegate.Add(this.mSliderSound.onChange, onAudioChange);
    }
    public override void OnExit() { }
    public void ChangeTable(ushort table, ushort seat) {
        LKGameManager.mIsChangeTable = true;
        UI_LK_Battle.ui.SetAutoFire(false);
        UI_LK_Battle.ui.SetAutoLock(false);
        NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown {
            TableID = table,
            ChairID = seat,
            Password = string.Empty,
        });
        this.Close();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
            case "bg":
                this.Close();
                break;
            case "btn_shake_on":
                this.SetShakeOP(false);
                break;
            case "btn_shake_off":
                this.SetShakeOP(true);
                break;
            case "btn_vip_on":
                this.SetVIPOP(false,true);
                break;
            case "btn_vip_off":
                if (RoleManager.Self.MemberOrder > 0) {
                    this.SetVIPOP(true, true);
                } else {
                    SystemMessageMgr.Instance.ShowMessageBox("需要蓝钻以上会员开启");
                }
                break;
            case "btn_change_table"://快速换桌
                this.ChangeTable(ushort.MaxValue, ushort.MaxValue);
                break;
            case "btn_yes"://继续游戏
                this.Close();
                break;
            case "btn_kefu"://客服界面
                UI.EnterUI<UI_LK_KeFu>(GameEnum.Fish_LK);
                break;
            case "btn_tujian"://图鉴
                UI.EnterUI<UI_LK_TuJian>(GameEnum.Fish_LK);
                break;
            case "btn_change_0":
                this.ChangeTable(RoleManager.Self.TableID, 0);
                break;
            case "btn_change_1":
                this.ChangeTable(RoleManager.Self.TableID, 1);
                break;
            case "btn_change_2":
                this.ChangeTable(RoleManager.Self.TableID, 2);
                break;
            case "btn_change_3":
                this.ChangeTable(RoleManager.Self.TableID, 3);
                break;
            case "btn_change_4":
                this.ChangeTable(RoleManager.Self.TableID, 4);
                break;
            case "btn_change_5":
                this.ChangeTable(RoleManager.Self.TableID, 5);
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "slider_music":
                this.mSliderMusic = tf.GetComponent<UISlider>();
                break;
            case "slider_sound":
                this.mSliderSound = tf.GetComponent<UISlider>();
                break;
            case "btn_shake_on":
                this.mBtnShakeOn = tf.gameObject;
                break;
            case "btn_shake_off":
                this.mBtnShakeOff = tf.gameObject;
                break;
            case "btn_vip_on":
                this.mBtnVIPOn = tf.gameObject;
                break;
            case "btn_vip_off":
                this.mBtnVIPOff = tf.gameObject;
                break;
            case "btn_change_0":
                this.mBtnChanges[0] = tf.gameObject;
                break;
            case "btn_change_1":
                this.mBtnChanges[1] = tf.gameObject;
                break;
            case "btn_change_2":
                this.mBtnChanges[2] = tf.gameObject;
                break;
            case "btn_change_3":
                this.mBtnChanges[3] = tf.gameObject;
                break;
            case "btn_change_4":
                this.mBtnChanges[4] = tf.gameObject;
                break;
            case "btn_change_5":
                this.mBtnChanges[5] = tf.gameObject;
                break;
        }
    }
}
