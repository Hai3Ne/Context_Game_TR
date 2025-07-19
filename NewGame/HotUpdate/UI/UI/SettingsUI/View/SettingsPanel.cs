using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class SettingsPanel : PanelBase
    {


        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable() 
        {
            base.OnEnable();
            RegisterListener();

            SetMusicPanel();
            SetSoundPanel();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Music.onClick.AddListener(OnMusicBtn);
            m_Btn_Efeito.onClick.AddListener(OnEfeitoBtn);
            m_Btn_Save.onClick.AddListener(OnSaveBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Music.onClick.RemoveListener(OnMusicBtn);
            m_Btn_Efeito.onClick.RemoveListener(OnEfeitoBtn);
            m_Btn_Save.onClick.RemoveListener(OnSaveBtn);
        }

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.CloseSettingsPanel();
        }

        public void OnMusicBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (CoreEntry.gAudioMgr.musicMute)
            {
                CoreEntry.gAudioMgr.SetAudioMusicPrefs(0);
            }
            else
            {
                CoreEntry.gAudioMgr.SetAudioMusicPrefs(1);
            }
            SetMusicPanel();
        }

        public void OnEfeitoBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (CoreEntry.gAudioMgr.soundMute)
            {
                CoreEntry.gAudioMgr.SetAudioSoundPrefs(0);
            }
            else
            {
                CoreEntry.gAudioMgr.SetAudioSoundPrefs(1);
            }
            SetSoundPanel();
        }

        public void SetMusicPanel() 
        {
            var musicPic = CoreEntry.gAudioMgr.musicMute ? "SettingsPanel:toggle_setting_on@2x" : "SettingsPanel:toggle_setting_off@2x";
            m_Btn_Music.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite(musicPic);
        }

        public void SetSoundPanel()
        {
            var soundPic = CoreEntry.gAudioMgr.soundMute ? "SettingsPanel:toggle_setting_on@2x" : "SettingsPanel:toggle_setting_off@2x";
            m_Btn_Efeito.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite(soundPic);
        }

        public void OnSaveBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
            tips.SetTipsPanel("Dicas", "Você quer sair?","Okay", delegate
            {
                MainUIModel.Instance.isOpenGuid = false;
                MainUIModel.Instance.CashOutCount = 3;
                MainUIModel.Instance.pixData = null;
                MainUIModel.Instance.cashOutTotalDay = 0;
                MainUICtrl.Instance.CloseSettingsPanel();
                UICtrl.Instance.OpenView("LoginPanel");
            },true);
        }
        #endregion
    }
}
