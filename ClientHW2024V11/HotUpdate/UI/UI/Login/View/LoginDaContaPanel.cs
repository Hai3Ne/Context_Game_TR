using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class LoginDaContaPanel : PanelBase
    {
        const string pwdKey = "pud4tIdkyRQ8zl9O";
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Img_eyeH.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Standard);
            m_Img_eyeS.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Password);
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
            m_Btn_Esqueceu.onClick.AddListener(OnEsqueceuBtn);

            m_Btn_Login.onClick.AddListener(OnLoginBtn);
            m_Btn_Eye.onClick.AddListener(OnEyeBtn);
            m_Btn_Login.interactable = true;
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Esqueceu.onClick.RemoveListener(OnEsqueceuBtn);

            m_Btn_Login.onClick.RemoveListener(OnLoginBtn);
            m_Btn_Eye.onClick.RemoveListener(OnEyeBtn);

        }
        #endregion
        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            LoginCtrl.Instance.CloseLoginDaContaPanel();
        }

        public void OnEsqueceuBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            LoginCtrl.Instance.OpenChangePwdPanel();
        }


        public void OnLoginBtn() 
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Login, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);

            var phone =$"{m_Input_phone.text}";
            var code = ToolUtil.GetRandomCode(12);
            var hash = ToolUtil.HMACSHA1(code, pwdKey); 
            var pwd = m_Input_pwd.text;
            if (m_Input_phone.text.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("您输入的电话号码不正确", this.gameObject.transform);
                return;
            }
            // else if (m_Input_phone.text.Length < 11 || m_Input_phone.text.Length > 11)
            // {
            //     //手机号位数不对
            //     ToolUtil.FloattingText("抱歉，不支持此数字的手机号码", this.gameObject.transform);
            //     return;
            // }
            else if (m_Input_pwd.text.Equals("") || m_Input_pwd.text.Length < 8 || m_Input_pwd.text.Length > 16)
            {
                //密码未填写或密码格式不对 
                ToolUtil.FloattingText("密码由8到16位数字和字母组成", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_pwd.text))
            {
                ToolUtil.FloattingText("密码不能有特殊字符", this.gameObject.transform);
                return;
            }
            MainUIModel.Instance.Phone = phone;
            MainUIModel.Instance.PhonePwd = pwd;
            LoginCtrl.Instance.SendPhoneLogin(phone, pwd,"", "", 101, 0, 0, code, hash, "", "3.0.1", "", "", "", "");
        }

        public void OnEyeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Input_pwd.inputType != InputField.InputType.Password)
            {
                m_Input_pwd.inputType = InputField.InputType.Password;
            }
            else
            {
                m_Input_pwd.inputType = InputField.InputType.Standard;

            }
            m_Img_eyeH.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Standard);
            m_Img_eyeS.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Password);
            m_Input_pwd.ForceLabelUpdate();
        }
    }
}
