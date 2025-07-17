using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_setting_new : UILayer {
    public UISlider mSliderMusic;
    public UISlider mSliderSound;
    public GameObject mBtnShakeOn;
    public GameObject mBtnShakeOff;
    public GameObject mBtnWindowsOn;
    public GameObject mBtnWindowsOff;

    public void InitData() {
        this.SetShakeOP(GameConfig.OP_Shake);
#if PC_BUILD || BUILD_PC_DEV_TEST
        GameConfig.OP_Fullscreen = Screen.fullScreen;
        GameConfig.SaveData();
        this.SetWindows(GameConfig.OP_Fullscreen);
#endif
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
#if PC_BUILD || BUILD_PC_DEV_TEST
    private void SetWindows(bool isFullscreen)
    {
        this.mBtnWindowsOn.SetActive(isFullscreen);
        this.mBtnWindowsOff.SetActive(!isFullscreen);
        if (Screen.fullScreen != isFullscreen)
        {
            StartCoroutine(ChangeScreenModeCoroutine(isFullscreen));
        }
        GameConfig.OP_Fullscreen = isFullscreen;
        GameConfig.SaveData();
        
        Debug.Log($"Screen mode changed to: {(isFullscreen ? "Fullscreen" : "Windowed")}");
    }
    
    [Obsolete("Obsolete")]
    private IEnumerator ChangeScreenModeCoroutine(bool isFullscreen)
    {
        if (isFullscreen)
        {
            UnityEngine.Resolution[] resolutions = Screen.resolutions;
            UnityEngine.Resolution nativeRes = resolutions[^1];
            
            Screen.SetResolution(nativeRes.width, nativeRes.height, true, nativeRes.refreshRate);
        }
        else
        {
            Screen.SetResolution(1280, 720, false);
        }
        yield return new WaitForEndOfFrame();
        QualitySettings.SetQualityLevel(QualitySettings.GetQualityLevel(), true);
        RefreshCameraSettings();
        
        yield return new WaitForEndOfFrame();
    }
    
    [Obsolete("Obsolete")]
    private void RefreshCameraSettings()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam != null)
            {
                cam.enabled = false;
                cam.enabled = true;
            }
        }
    }
#endif
    
    
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
                
#if PC_BUILD || BUILD_PC_DEV_TEST
            case "btn_windows_off":
                this.SetWindows(true);
                break;
            case "btn_windows_on":
                this.SetWindows(false);
                break;
#endif
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
#if PC_BUILD || BUILD_PC_DEV_TEST
            case "btn_windows_on":
                this.mBtnWindowsOn = tf.gameObject;
                break;
            case "btn_windows_off":
                this.mBtnWindowsOff = tf.gameObject;
                break;
#endif
        }
    }
}
