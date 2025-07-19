using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class PhoneBindPanel : PanelBase
    {
        private long downTime = 0;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            m_Btn_Submit.interactable = true;
            if (downTime > ToolUtil.getServerTime())
            {
                TimeDown1();
            }
            else
            {
                m_Txt_Verificar.text = "Verificar";
                m_Btn_Verificar.interactable = true;
            }
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                m_Rect_tips.gameObject.SetActive(true);
            }
            m_Trans_Icon.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            m_TxtM_Gold.text = m_TxtM_Gold.text.Replace("R$",ToolUtil.GetCurrencySymbol());
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
            m_Btn_Eye.onClick.AddListener(OnEyeBtn);
            m_Btn_Verificar.onClick.AddListener(OnVerificarBtn);
            m_Btn_Submit.onClick.AddListener(OnSubmitBtn);
            Message.AddListener(MessageName.PHONEBINDTIMEDOWN, TimeDown);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Eye.onClick.RemoveListener(OnEyeBtn);
            m_Btn_Verificar.onClick.RemoveListener(OnVerificarBtn);
            m_Btn_Submit.onClick.RemoveListener(OnSubmitBtn);
            Message.RemoveListener(MessageName.PHONEBINDTIMEDOWN, TimeDown);
        }
        #endregion
        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            PhoneBindCtrl.Instance.ClosePhoneBindPanel();
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                MainUICtrl.Instance.OpenWithDrawPanel();
            }
        }
        public void OnEyeBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Input_PWD.inputType!= InputField.InputType.Password)
            {
                m_Input_PWD.inputType = InputField.InputType.Password;
                m_Btn_Eye.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Login:buxianshi@2x");
            }
            else
            {
                m_Input_PWD.inputType = InputField.InputType.Standard;
                m_Btn_Eye.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Login:xianshi@2x");
            }
            m_Btn_Eye.GetComponent<Image>().SetNativeSize();
            m_Input_PWD.ForceLabelUpdate();
        }

        /// <summary>
        /// 短信验证
        /// </summary>
        public void OnVerificarBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Txt_inputPhone.text.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("O número de celular que você digitou está incorreto", this.gameObject.transform);
                return;
            }
            else if (m_Txt_inputPhone.text.Length < 11 || m_Txt_inputPhone.text.Length > 11)
            {
                //手机号位数不对
                ToolUtil.FloattingText("Desculpe, números de celular com este número de dígitos não são suportados", this.gameObject.transform);
                return;
            }
            else if (m_Input_PWD.text.Equals("") || m_Input_PWD.text.Length < 8 || m_Input_PWD.text.Length > 16)
            {
                //密码未填写或密码格式不对
                ToolUtil.FloattingText("A senha consiste em 8 a 16 dígitos e letras", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_PWD.text))
            {
                ToolUtil.FloattingText("A senha não pode ter caracteres especiais", this.gameObject.transform);
                return;
            }

            var phone = $"55{m_Txt_inputPhone.text}";
            MainUICtrl.Instance.SendVerificationSMS(phone);
        }
        /// <summary>
        /// 绑定手机提交
        /// </summary>
        public void OnSubmitBtn() 
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Submit, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Txt_inputPhone.text.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("O número de celular que você digitou está incorreto",this.gameObject.transform);
                return;
            }
            else if (m_Txt_inputPhone.text.Length < 11 || m_Txt_inputPhone.text.Length > 11)
            {
                //手机号位数不对
                ToolUtil.FloattingText("Desculpe, números de celular com este número de dígitos não são suportados", this.gameObject.transform);
                return;
            }
            else if (m_Input_PWD.text.Equals("")|| m_Input_PWD.text.Length<8|| m_Input_PWD.text.Length>16)
            {
                //密码未填写或密码格式不对 
                ToolUtil.FloattingText("A senha consiste em 8 a 16 dígitos e letras", this.gameObject.transform);
                return;
            }
            else if (m_Input_Code.text.Equals(""))
            {
                //没有填写验证码
                ToolUtil.FloattingText("O código que você digitou está incorreto", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_PWD.text))
            {
                ToolUtil.FloattingText("A senha não pode ter caracteres especiais", this.gameObject.transform);
                return;
            }
            var phone = $"55{m_Txt_inputPhone.text}";
            var code = m_Input_Code.text;
            var pwd = m_Input_PWD.text;

            MainUIModel.Instance.Phonemima = pwd;
            //MainUIModel.Instance.PhonePwd = pwd;
            MainUICtrl.Instance.SendBindPhone(phone,code, pwd);
        }

        public void TimeDown() 
        {
            downTime = ToolUtil.getServerTime() + 120;
            m_Btn_Verificar.interactable = false;
            StartCoroutine(TimeUtil.TimeCountDown(120, m_Txt_Verificar, delegate
            {
                m_Txt_Verificar.text = "Verificar";
                m_Btn_Verificar.interactable = true;
            }));
        }

        public void TimeDown1()
        {
            m_Btn_Verificar.interactable = false;
            StartCoroutine(TimeUtil.TimeCountDown(downTime - ToolUtil.getServerTime(), m_Txt_Verificar, delegate
            {
                m_Txt_Verificar.text = "Verificar";
                m_Btn_Verificar.interactable = true;
            }));
        }
    }
}
