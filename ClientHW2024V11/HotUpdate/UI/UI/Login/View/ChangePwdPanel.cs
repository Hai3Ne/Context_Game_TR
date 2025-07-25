using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class ChangePwdPanel : PanelBase
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

        protected override void Update()
        {
            base.Update();
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Eye.onClick.AddListener(OnEyeBtn);
            m_Btn_Verificar.onClick.AddListener(OnVerificarBtn);
            m_Btn_submit.onClick.AddListener(OnSubmitBtn);
            Message.AddListener(MessageName.REFRESH_CHANGEPWD_PANEL, TimeDown);
            m_Btn_Verificar.interactable = true;
            m_Btn_submit.interactable = true;
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Eye.onClick.RemoveListener(OnEyeBtn);
            m_Btn_Verificar.onClick.RemoveListener(OnVerificarBtn);
            m_Btn_submit.onClick.RemoveListener(OnSubmitBtn);
            Message.AddListener(MessageName.REFRESH_CHANGEPWD_PANEL, TimeDown);
        }
        #endregion
        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            LoginCtrl.Instance.CloseChangePwdPanel();
        }

        public void OnVerificarBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var phone = $"55{m_Input_phone.text}";
            var pwd = $"{m_Input_pwd.text}";
            var code = $"{m_Input_code.text}";
            if (m_Input_phone.text.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("O número de celular que você digitou está incorreto", this.gameObject.transform);
                return;
            }
            else if (pwd.Equals("") || pwd.Length < 8 || pwd.Length > 16)
            {
                //密码未填写或密码格式不对
                ToolUtil.FloattingText("A senha consiste em 8 a 16 dígitos e letras", this.gameObject.transform);
                return;
            }
            else if (m_Input_phone.text.Length < 11 || m_Input_phone.text.Length > 11)
            {
                //手机号位数不对
                ToolUtil.FloattingText("Desculpe, números de celular com este número de dígitos não são suportados", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_pwd.text))
            {
                ToolUtil.FloattingText("A senha não pode ter caracteres especiais", this.gameObject.transform);
                return;
            }

            LoginCtrl.Instance.SendChangePwdSms(phone);

        }
        public void OnEyeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Input_pwd.inputType != InputField.InputType.Password)
            {
                m_Input_pwd.inputType = InputField.InputType.Password;
                m_Btn_Eye.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Common:DL_btn_YanJing_2");
            }
            else
            {
                m_Input_pwd.inputType = InputField.InputType.Standard;
                m_Btn_Eye.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Common:DL_btn_YanJing_1");
            }
            m_Input_pwd.ForceLabelUpdate();
        }

        public void OnSubmitBtn()
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_submit, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            var phone = $"55{m_Input_phone.text}";
            var pwd =$"{m_Input_pwd.text}" ;
            var code = $"{m_Input_code.text}";
            if (phone.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("O número de celular que você digitou está incorreto", this.gameObject.transform);
                return;
            }
            else if (pwd.Equals("") || pwd.Length < 8 || pwd.Length > 16)
            {
                //密码未填写或密码格式不对
                ToolUtil.FloattingText("A senha consiste em 8 a 16 dígitos e letras", this.gameObject.transform);
                return;
            }
            else if (phone.Length < 11 )
            {
                //手机号位数不对
                ToolUtil.FloattingText("Desculpe, números de celular com este número de dígitos não são suportados", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_pwd.text))
            {
                ToolUtil.FloattingText("A senha não pode ter caracteres especiais", this.gameObject.transform);
                return;
            }
            else if (code.Equals(""))
            {
                //没有填写验证码
                ToolUtil.FloattingText("O código que você digitou está incorreto", this.gameObject.transform);
                return;
            }
            MainUIModel.Instance.Phone = phone;
            MainUIModel.Instance.PhonePwd = pwd;
            LoginCtrl.Instance.SendChangePWD(phone,code,pwd);
        }

        public void TimeDown()
        {
            m_Btn_Verificar.interactable = false;
            StartCoroutine(TimeUtil.TimeCountDown(120, m_Txt_time, delegate
            {
                m_Txt_time.text = "Verificar";
                m_Btn_Verificar.interactable = true;
            }));
        }




    }
}
