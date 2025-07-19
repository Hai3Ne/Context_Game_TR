using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{

    public partial class PrivacyTipPanel : PanelBase
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Refuse.onClick.AddListener(ClickResuse);
            m_Btn_Agree.onClick.AddListener(ClickAgree);
            m_Btn_Refuse.onClick.AddListener(ClickResuse);
            m_Btn_Privacy.onClick.AddListener(ClickPrivacy);
            m_Btn_User.onClick.AddListener(ClickUser);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Refuse.onClick.RemoveListener(ClickResuse);
            m_Btn_Agree.onClick.RemoveListener(ClickAgree);
            m_Btn_Privacy.onClick.RemoveListener(ClickPrivacy);
            m_Btn_User.onClick.RemoveListener(ClickUser);
        }

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("PrivacyTipPanel");
        }

        private void ClickResuse()
        {
            OnCloseBtn();
        }

        private void ClickAgree()
        {
            OnCloseBtn();
        }

        private void ClickPrivacy()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.ShowDialog("PrivacyPanel", true, 1);
        }

        private void ClickUser()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.ShowDialog("PrivacyPanel", true, 0);
        }
    }
}
