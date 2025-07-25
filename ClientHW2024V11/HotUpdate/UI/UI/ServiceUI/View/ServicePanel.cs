using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class ServicePanel : PanelBase
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
            m_Btn_Submeter.gameObject.SetActive(!MainPanelMgr.Instance.IsShow("LoginPanel"));
            m_Btn_CanNotSubmeter.gameObject.SetActive(MainPanelMgr.Instance.IsShow("LoginPanel"));
            m_Btn_Submeter.interactable = true;
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
            m_Btn_Server.onClick.AddListener(JumpToLink);
            m_Btn_Submeter.onClick.AddListener(SendMessage);
            Message.AddListener(MessageName.REFRESH_SERVICE_PANEL,UpdataPanel);
            m_Btn_Return.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Server.onClick.RemoveListener(JumpToLink);
            m_Btn_Submeter.onClick.RemoveListener(SendMessage);
            Message.RemoveListener(MessageName.REFRESH_SERVICE_PANEL, UpdataPanel);
            m_Btn_Return.onClick.RemoveListener(OnCloseBtn);
        }
        #endregion
        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            ServiceCtrl.Instance.CloseServicePanel();
        }

        public void JumpToLink() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            string link = "https://t.me/Slotswin2023";
            Application.OpenURL(link);
        }

        public void SendMessage() 
        {
            //m_Btn_Submeter.interactable = false;
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Submeter, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (string.IsNullOrEmpty(m_Txt_content.text))
            {
                ToolUtil.FloattingText("O conteúdo não pode ficar em branco!", this.gameObject.transform);
                return;
            }
            if (MainUIModel.Instance.MessageCount >= 3)
            {
                ToolUtil.FloattingText("Você atingiu o limite máximo de mensagens hoje, tente novamente amanhã", this.gameObject.transform);
                return;
            }
            MainUICtrl.Instance.SendOnLineMessage(1, m_Txt_content.text);
            ToolUtil.FloattingText("Mensagem bem sucedida!", this.gameObject.transform);
        }

        public void UpdataPanel() 
        {
            m_Input_content.text = "";
            ServiceCtrl.Instance.CloseServicePanel();
        }
    }
}
