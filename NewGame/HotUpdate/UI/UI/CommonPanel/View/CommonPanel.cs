using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class CommonPanel : PanelBase
    {
        private Callback callBack;
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

        public void SetContent(string title,string content, Callback callBack = null)
        {
            m_Txt_Title.text = title;
            this.callBack = callBack;
            m_Txt_Content.text = content;
        }

        public void RegisterListener()
        {
            m_Btn_Colse.onClick.AddListener(ClosePanel);
            m_Btn_know.onClick.AddListener(ClickCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Colse.onClick.RemoveListener(ClosePanel);
            m_Btn_know.onClick.RemoveListener(ClickCloseBtn);
        }

        private void ClosePanel()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Close();
        }

        private void ClickCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Close();
            callBack?.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }
    }
}
