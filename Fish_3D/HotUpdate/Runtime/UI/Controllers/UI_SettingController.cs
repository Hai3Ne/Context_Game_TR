using UnityEngine;
using System.Collections;

public class UI_SettingController : IUIControllerImp {
    public GameObject mBtnClose;
    public UISlider mSliderMusic;//音乐
    public UISlider mSliderSound;//音效
    public GameObject mBtnEffOn;//特效
    public GameObject mBtnEffOff;
    public GameObject mBtnShakeOn;//震动
    public GameObject mBtnShakeOff;
    public GameObject mBtnAutoBuyOn;//自动购买且使用
    public GameObject mBtnAutoBuyOff;
    public GameObject mBtnChangeUser;//切换帐号
    public GameObject mBtnSave;//保存继续

    public override void Init(object data) {
        base.Init(data);
    }

	long timerID;
    public override void Show() {
		WndManager.LoadUIGameObject("UI_Setting",
			SceneObjMgr.Instance.UIPanelTransform,
			delegate(GameObject obj) {
				uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
                GameUtils.Traversal(obj.transform.Find("scale"), this.OnNodeAsset);
				if (TweenShow(delegate(){
					TimeManager.ClearIntervalID(timerID);
				})){
					timerID = TimeManager.StartTimerInterval(delegate(){
						this.mSliderMusic.ForceUpdate();
						this.mSliderSound.ForceUpdate();
					});
				}

                UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnMusicOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnMusicOff).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnSoundOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnSoundOff).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnShakeOn).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnShakeOff).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnEffOn).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnEffOff).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnLoginOutOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnLoginOutOff).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnAutoBuyOn).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnAutoBuyOff).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoLauncherOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoLauncherOff).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoItemOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoItemOff).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoHeroOn).onClick = this.OnButtonClick;
                //UIEventListener.Get(this.mBtnAutoHeroOff).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnChangeUser).onClick = this.OnButtonClick;
                UIEventListener.Get(this.mBtnSave).onClick = this.OnButtonClick;
                
                TimeManager.DelayExec(0, () => {
                    this.mSliderMusic.value = AudioManager.MusicVolume;
                    this.mSliderSound.value = AudioManager.AudioVolume;
                    EventDelegate.Add(this.mSliderMusic.onChange, onMusicChange);
                    EventDelegate.Add(this.mSliderSound.onChange, onAudioChange);
                });

                this.SetShakeOP(GameConfig.OP_Shake);
                //this.SetEffOP(GameConfig.OP_Eff);
                //this.SetLoginOutOP(GameConfig.OP_AutoLoginBank);
                this.SetAutoBuyOP(GameConfig.OP_QuickBuy);
                //this.SetAutoLauncherOP(GameConfig.OP_AutoLauncher);
                //this.SetAutoItemOP(GameConfig.OP_AutoSkill);
                //this.SetAutoHeroOP(GameConfig.OP_AutoHero);

                if (SceneLogic.Instance.IsGameOver == false) {//游戏界面中隐藏切换帐号功能
                    this.mBtnChangeUser.SetActive(false);
                    Vector3 pos = this.mBtnSave.transform.localPosition;
                    pos.x = 0;
                    this.mBtnSave.transform.localPosition = pos;
                }
            }
        );
        base.Show();
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
    //private void SetEffOP(bool open) {//特效开关
    //    this.mBtnEffOn.SetActive(open);
    //    this.mBtnEffOff.SetActive(open == false);
    //    GameConfig.OP_Eff = open;
    //    GameConfig.SaveData();
    //}
    //private void SetLoginOutOP(bool open) {//保险箱自动注销开关
    //    this.mBtnLoginOutOn.SetActive(open);
    //    GameConfig.OP_AutoLoginBank = open;
    //    this.mBtnLoginOutOff.SetActive(open == false);
    //}
    private void SetAutoBuyOP(bool open) {//自动购买开关
        this.mBtnAutoBuyOn.SetActive(open);
        this.mBtnAutoBuyOff.SetActive(open == false);
        GameConfig.OP_QuickBuy = open;
        GameConfig.SaveData();
    }
    //private void SetAutoLauncherOP(bool open) {//自动释放炮台技能开关
    //    this.mBtnAutoLauncherOn.SetActive(open);
    //    this.mBtnAutoLauncherOff.SetActive(open == false);
    //    GameConfig.OP_AutoLauncher = open;
    //}
    //private void SetAutoItemOP(bool open) {//自动使用道具技能开关
    //    this.mBtnAutoItemOn.SetActive(open);
    //    this.mBtnAutoItemOff.SetActive(open == false);
    //    GameConfig.OP_AutoSkill = open;
    //}
    //private void SetAutoHeroOP(bool open) {//自动召唤英雄开关
    //    this.mBtnAutoHeroOn.SetActive(open);
    //    this.mBtnAutoHeroOff.SetActive(open == false);
    //    GameConfig.OP_AutoHero = open;
    //}
    
    public void OnButtonClick(GameObject obj) {
        if (this.mBtnClose == obj) {//关闭按钮
            this.Close();
        //} else if (this.mBtnMusicOn == obj) {//音乐
        //    this.SetMusicOP(false);
        //} else if (this.mBtnMusicOff == obj) {
        //    this.SetMusicOP(true);
        //} else if (this.mBtnSoundOn == obj) {//音效
        //    this.SetSoundOP(false);
        //} else if (this.mBtnSoundOff == obj) {
        //    this.SetSoundOP(true);
        } else if (this.mBtnShakeOn == obj) {//震动
            this.SetShakeOP(false);
        } else if (this.mBtnShakeOff == obj) {
            this.SetShakeOP(true);
        //} else if (this.mBtnEffOn == obj) {//特效
        //    this.SetEffOP(false);
        //} else if (this.mBtnEffOff == obj) {
        //    this.SetEffOP(true);
        //} else if (this.mBtnLoginOutOn == obj) {//保险箱自动登出
        //    this.SetLoginOutOP(false);
        //} else if (this.mBtnLoginOutOff == obj) {
        //    this.SetLoginOutOP(true);
        } else if (this.mBtnAutoBuyOn == obj) {//自动购买且使用
            this.SetAutoBuyOP(false);
        } else if (this.mBtnAutoBuyOff == obj) {
            this.SetAutoBuyOP(true);
        //} else if (this.mBtnAutoLauncherOn == obj) {//自动释放炮台技能
        //    this.SetAutoLauncherOP(false);
        //} else if (this.mBtnAutoLauncherOff == obj) {
        //    this.SetAutoLauncherOP(true);
        //} else if (this.mBtnAutoItemOn == obj) {//自动使用道具技能
        //    this.SetAutoItemOP(false);
        //} else if (this.mBtnAutoItemOff == obj) {
        //    this.SetAutoItemOP(true);
        //} else if (this.mBtnAutoHeroOn == obj) {//自动召唤英雄
        //    this.SetAutoHeroOP(false);
        //} else if (this.mBtnAutoHeroOff == obj) {
        //    this.SetAutoHeroOP(true);
        //} else if (this.mBtnChangeUser == obj) {//更换帐号        
        //    UI.ExitAllUI();
        //    //MainEntrace.Instance.CallLuaMethod("SwitchAccount");
        //    ShopManager.mIsShowFrist = false;
        //    //删除当前登录数据
        //    PlayerPrefs.DeleteKey("wx_openid");
        //    PlayerPrefs.DeleteKey("wx_access_token");
        //    PlayerPrefs.DeleteKey("wx_refresh_token");
        //    MainEntrace.Instance.ClearTick();
        //    VersionManager.CheckVersion((is_update) => {
        //        if (is_update) {
        //            VersionManager.ShowVersionTick();
        //        }
        //    });
        //    MainEntrace.Instance.OpenLogin();
        } else if (this.mBtnSave == obj) {//保存继续
            this.Close();
        }
    }
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "btn_close":
                this.mBtnClose = tf.gameObject;
                break;
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
            case "btn_eff_off":
                this.mBtnEffOff = tf.gameObject;
                break;
            case "btn_eff_on":
                this.mBtnEffOn = tf.gameObject;
                break;
            case "btn_auto_buy_off":
                this.mBtnAutoBuyOff = tf.gameObject;
                break;
            case "btn_auto_buy_on":
                this.mBtnAutoBuyOn = tf.gameObject;
                break;
            case "btn_change_user":
                this.mBtnChangeUser = tf.gameObject;
                break;
            case "btn_save":
                this.mBtnSave = tf.gameObject;
                break;
        }
    }
}
