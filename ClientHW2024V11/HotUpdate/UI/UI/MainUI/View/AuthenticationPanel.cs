using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public partial class AuthenticationPanel : PanelBase
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

        #region 事件绑定
        public void RegisterListener()
        {
      
            m_Btn_JumpToPhone.onClick.AddListener(OpenPhoneBindPanel);
            // m_Btn_Close.onClick.AddListener(OnCloseBtn);
            Message.AddListener(MessageName.REFRESH_AUTHENTICATION_PANEL, OnCloseBtn);
        }

        public void UnRegisterListener()
        {
    
            m_Btn_JumpToPhone.onClick.RemoveListener(OpenPhoneBindPanel);
            // m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            Message.RemoveListener(MessageName.REFRESH_AUTHENTICATION_PANEL, OnCloseBtn);
        }

        public void OnCloseBtn()
        {
            MainUICtrl.Instance.CloseAuthenticationPanel();
            Message.Broadcast(MessageName.REALName);
        }

        public void OpenPhoneBindPanel()
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_RealNameAuth, out bool isRealNameAuth);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsAdult, out bool isAdult);

            if(isRealNameAuth && !isAdult)
            {
                ToolUtil.FloattingText("未成年", this.transform);
                return;
            }

            if (!isRealNameAuth && isAdult)
            {
                ToolUtil.FloattingText("已经认证过了", this.transform);
                return;
            }
            if (!ToolUtil.ValidateIDCardNumber(m_Input_code.text) || m_Input_code.text.Length != 18)
            {
                ToolUtil.FloattingText("请输入正确的身份证号码", this.transform);
                return;
            }
            if (m_Input_code.text == "")
            {
                ToolUtil.FloattingText("请输入身份证号码", this.transform);
                return;
            }
            else if (m_Input_name.text == "")
            {
                ToolUtil.FloattingText("请输入姓名", this.transform);
                return;
            }
            //SdkCtrl.Instance.sendAu(MainUIModel.Instance.palyerData.m_i8roleID + "", m_TMPInput_name.text, m_TMPInput_code.text);
            MainUICtrl.Instance.SendCW_HUMAN_REAL_NAME_AUTHENTICATION_REQ(m_Input_name.text, m_Input_code.text);
        }
        #endregion
    }
}
