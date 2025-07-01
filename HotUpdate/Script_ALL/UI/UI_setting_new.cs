using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_setting_new : UILayer {
    public UISlider mSliderMusic;
    public UISlider mSliderSound;
    public GameObject mBtnShakeOn;
    public GameObject mBtnShakeOff;

    public void InitData() {
        this.SetShakeOP(GameConfig.OP_Shake);
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
    public override void OnNodeLoad() {
        this.InitData();
    }
    public override void OnEnter() {
        this.mSliderMusic.value = AudioManager.MusicVolume;
        this.mSliderSound.value = AudioManager.AudioVolume;
        EventDelegate.Add(this.mSliderMusic.onChange, onMusicChange);
        EventDelegate.Add(this.mSliderSound.onChange, onAudioChange);
    }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "bg":
            case "btn_close":
                this.Close();
                break;
            case "btn_shake_on":
                this.SetShakeOP(false);
                break;
            case "btn_shake_off":
                this.SetShakeOP(true);
                break;
            case "btn_switch"://切换账号
                UserManager.GoLogin();
                break;
            case "btn_ok":
                this.Close();
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
        }
    }
}
