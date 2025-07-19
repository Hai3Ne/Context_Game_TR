using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class CadastrPanel : PanelBase
    {
        private long downTime = 0;
        private long downTime1 = 0;
        const string pwdKey = "pud4tIdkyRQ8zl9O";
        private int showType = 0;

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            showType = (int)param;
            m_Input_phone.text = "";
            m_Input_pwd.text = "";
            m_Input_yzm.text = "";
            m_Img_title1.gameObject.SetActive(showType == 1);
            m_Img_title2.gameObject.SetActive(showType == 0);
            RegisterListener();
            m_Btn_submit.interactable = true;
            m_Img_eyeH.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Standard);
            m_Img_eyeS.gameObject.SetActive(m_Input_pwd.inputType == InputField.InputType.Password);
            if(showType == 0)
            {
                if (downTime > ToolUtil.getServerTime())
                {
                    TimeDown1();
                }
                else
                {
                    m_Txt_time.text = "发送验证码";
                    m_Btn_Verificar.interactable = true;
                }
            }
            else
            {
                if (downTime1 > ToolUtil.getServerTime())
                {
                    TimeDown1();
                }
                else
                {
                    m_Txt_time.text = "发送验证码";
                    m_Btn_Verificar.interactable = true;
                }
            }

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
            m_Btn_Verificar.onClick.AddListener(OnVerificarBtn);
            m_Btn_submit.onClick.AddListener(OnSubmitBtn);
            m_Btn_Eye.onClick.AddListener(OnEyeBtn);

            Message.AddListener(MessageName.REFRESH_CADASTR_PANEL, TimeDown) ;
            Message.AddListener(MessageName.REFRESH_CADASTR_SUCESS, OnSucess);
            m_Btn_submit.interactable = true;
            m_Btn_Verificar.interactable = true;
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Verificar.onClick.RemoveListener(OnVerificarBtn);
            m_Btn_submit.onClick.RemoveListener(OnSubmitBtn);
            m_Btn_Eye.onClick.RemoveListener(OnEyeBtn);
            Message.RemoveListener(MessageName.REFRESH_CADASTR_PANEL, TimeDown);
            Message.RemoveListener(MessageName.REFRESH_CADASTR_SUCESS, OnSucess);
        }
        #endregion
        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            LoginCtrl.Instance.CloseCadastrPanel();
        }

        public void OnSucess()
        {
            LoginCtrl.Instance.CloseCadastrPanel();
        }

        public void SetUpPanel() 
        {
            
        }
        /// <summary>
        /// 注册账号短信验证码
        /// </summary>
        public void OnVerificarBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var phone = $"{m_Input_phone.text}";
            var pwd = m_Input_pwd.text;
            var code = m_Input_yzm.text;
            if (m_Input_phone.text.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("您输入的电话号码不正确", this.gameObject.transform);
                return;
            }
            else if (m_Input_phone.text.Length < 11 || m_Input_phone.text.Length > 11)
            {
                //手机号位数不对
                ToolUtil.FloattingText("抱歉，不支持此数字的手机号码", this.gameObject.transform);
                return;
            }
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
            if(showType == 0)
            {
                LoginCtrl.Instance.SendChangePwdSms(phone);
            }
            else if (showType == 1)
            {
                MainUICtrl.Instance.SendVerificationSMS(phone);
            
            }
          //  LoginCtrl.Instance.SendRegisterAccountSms(phone);
        }

        public void OnSubmitBtn() 
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_submit, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            var phone = $"{m_Input_phone.text}";
            var pwd = m_Input_pwd.text;
            var code = m_Input_yzm.text;
            var encryption = ToolUtil.GetRandomCode(12);
            var hash = ToolUtil.HMACSHA1(code, pwdKey);
            if (phone.Equals(""))
            {
                //手机号码未填写
                ToolUtil.FloattingText("您输入的电话号码不正确", this.gameObject.transform);
                return;
            }
            else if (pwd.Equals("") || pwd.Length < 8 || pwd.Length > 16)
            {
                //密码未填写或密码格式不对
                ToolUtil.FloattingText("密码由8到16位数字和字母组成", this.gameObject.transform);
                return;
            }
            else if (phone.Length < 11)
            {
                //手机号位数不对
                ToolUtil.FloattingText("抱歉，不支持此数字的手机号码", this.gameObject.transform);
                return;
            }
            else if (code.Equals(""))
            {
                //没有填写验证码
                ToolUtil.FloattingText("您输入的验证码不正确", this.gameObject.transform);
                return;
            }
            else if (ToolUtil.CheckPwd(m_Input_pwd.text))
            {
                ToolUtil.FloattingText("密码不能有特殊字符", this.gameObject.transform);
                return;
            }
            if (showType == 0)
            {
                MainUIModel.Instance.Phonemima = pwd;
                LoginCtrl.Instance.SendChangePWD(phone, code, pwd);
            }
            else if (showType == 1)
            {
                MainUIModel.Instance.Phonemima = pwd;
                MainUICtrl.Instance.SendBindPhone(phone, code, pwd);
            }

           // LoginCtrl.Instance.SendRegisterAccount(phone, code, pwd,  "", "", 101, 0, 0, encryption, hash, "", "3.0.1", LoginCtrl.Instance.channelId + "");
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
        public void TimeDown()
        {

            if(showType == 0)
            {
                downTime = ToolUtil.getServerTime() + 120;
            }
            else
            {
                downTime1 = ToolUtil.getServerTime() + 120;
            }
            
            m_Btn_Verificar.interactable = false;
            StartCoroutine(TimeUtil.TimeCountDown(120, m_Txt_time, delegate
            {
                m_Txt_time.text = "发送验证码";
                m_Btn_Verificar.interactable = true;
            }));
        }


        public void TimeDown1()
        {
            var time = showType == 0 ? downTime : downTime1;
            m_Btn_Verificar.interactable = false;
            StartCoroutine(TimeUtil.TimeCountDown(time - ToolUtil.getServerTime(), m_Txt_time, delegate
            {
                m_Txt_time.text = "发送验证码";
                m_Btn_Verificar.interactable = true;
            }));
        }
    }
}
