using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class LoginPanel : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected async override void OnEnable()
        {
            base.OnEnable();
            m_Drop_selec.gameObject.SetActive(SdkCtrl.Instance.getChannle() == 9999);
            m_Drop_selec.ClearOptions();
            var list = new List<string>();
            list.Add("无");
            for (int i = 1; i <= 10; i++)
            {
                list.Add("账号" + i);
            }
            m_Drop_selec.AddOptions(list);
            m_Drop_selec.onValueChanged.AddListener(changeAccount);
            LoginCtrl.Instance.initLogin();
            Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 0);
            RegisterListener();
     
            m_Btn_Comece.interactable = true;
            Message.AddListener(MessageName.CLOSE_LOGINBTN, CloseComeceBtn);
            Message.AddListener(MessageName.OPEN_LOGINBTN,OpenComeceBtn);

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            var isShow = false;
            if (PlayerPrefs.HasKey("ISCHECKYINSI"))
            {
                isShow = PlayerPrefs.GetInt("ISCHECKYINSI") == 1;
            }
            m_Tog_Btn.SetIsOnWithoutNotify(false);
            if (isShow)
            {
                SdkCtrl.Instance.perMission();
            }

          
            //else
            //{
            //    MainPanelMgr.Instance.ShowDialog("PrivacyTipPanel");
            //}


        }
        public void changeAccount(int value)
        {
            if (value <= 0) return;
            var code = ToolUtil.GetRandomCode(12);
            var hash = ToolUtil.HMACSHA1(code, LoginCtrl.pwdKey);
            LoginCtrl.Instance.loginType = 0; // Guest login
            LoginCtrl.Instance.SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", 1 + "", "", "", "", "HWCNDY2_" + SystemInfo.deviceUniqueIdentifier + value);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            m_Btn_Comece.interactable = true;
            m_Btn_Service.onClick.RemoveListener(OnServiceBtn);
            m_Drop_selec.onValueChanged.RemoveListener(changeAccount);
            m_Btn_User.onClick.RemoveListener(OnUsrBtn);
            m_Btn_Yinsi.onClick.RemoveListener(OnYinsiBtn);
            m_Btn_Comece.onClick.RemoveListener(OnComeceBtn);
            m_Btn_Comece1.onClick.RemoveListener(OnComece1Btn);
            m_Btn_Comece2.onClick.RemoveListener(OnComece2Btn);
            
            m_Tog_Btn.onValueChanged.RemoveListener(onValueChange);
            Message.RemoveListener<string>(MessageName.WX_LOGIN_CALLBACK, OnWxCallBack);
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Service.onClick.AddListener(OnServiceBtn);
       
            m_Btn_User.onClick.AddListener(OnUsrBtn);
            m_Btn_Yinsi.onClick.AddListener(OnYinsiBtn);
            m_Btn_Comece.onClick.AddListener(OnComeceBtn);
            m_Btn_Comece1.onClick.AddListener(OnComece1Btn);
            m_Btn_Comece2.onClick.AddListener(OnComece2Btn);
            m_Tog_Btn.onValueChanged.AddListener(onValueChange);
            Message.AddListener<string>(MessageName.WX_LOGIN_CALLBACK, OnWxCallBack);
        }

        private void OnWxCallBack(string code)
        {
            if (!HotStart.ins.m_isShow)
            {
                HotStart.ins.CloseView();
                UICtrl.Instance.CloseLoading();
                MainPanelMgr.Instance.ShowPanel("MainUIPanel");
                MainPanelMgr.Instance.GetPanel("MainUIPanel").Canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
                return;
            }
            LoginCtrl.Instance.ClickLogin(code);
        }

        public void UnRegisterListener()
        {
            m_Btn_Service.onClick.RemoveListener(OnServiceBtn);
      
            m_Btn_User.onClick.RemoveListener(OnUsrBtn);
            m_Btn_Yinsi.onClick.RemoveListener(OnYinsiBtn);
            m_Btn_Comece.onClick.RemoveListener(OnComeceBtn);
            m_Tog_Btn.onValueChanged.RemoveListener(onValueChange);
            Message.RemoveListener<string>(MessageName.WX_LOGIN_CALLBACK, OnWxCallBack);
        }
        #endregion
        
        public void OnServiceBtn() 
        {
        
            CoreEntry.gAudioMgr.PlayUISound(46);
            var str = string.Format("http://{0}/index/index/home?theme=05202d&visiter_id=&visiter_name=&avatar=&business_id=1&groupid=0", HotStart.ins.m_customer);
            Application.OpenURL(str);
            // MainUICtrl.Instance.OpenServicePanel();
        }
        public void OnLoginBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            //if (!MainUIModel.Instance.isBindPhone)
            //{
            //    MainUICtrl.Instance.OpenPhoneBindPanel();
            //}
            if (!m_Tog_Btn.isOn)
            {
                ToolUtil.FloattingText("请选择同意用户服务协议及隐私政策!", transform);
                return;
            }

            if (!SdkCtrl.Instance.isCanLogin())
            {
                return;
            }

            if (!SdkCtrl.Instance.isPermission())
            {
                SdkCtrl.Instance.perMission();
                return;
            }

            LoginCtrl.Instance.OpenLoginDaContaPanel();
        }

        public void OnAccountBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            LoginCtrl.Instance.OpenCadastrPanel();
        }

        public void OnComece1Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!m_Tog_Btn.isOn)
            {
                ToolUtil.FloattingText("请选择同意用户服务协议及隐私政策!", transform);
                return;
            }
            if (!SdkCtrl.Instance.isCanLogin())
            {
                return;
            }
            if (!SdkCtrl.Instance.isPermission())
            {
                SdkCtrl.Instance.perMission();
                return;
            }
            if (!HotStart.ins.m_isShow)
            {
                HotStart.ins.CloseView();
                UICtrl.Instance.CloseLoading();
                MainPanelMgr.Instance.ShowPanel("MainUIPanel");
                MainPanelMgr.Instance.GetPanel("MainUIPanel").Canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
            }
            else {
                var code = ToolUtil.GetRandomCode(12);
                var hash = ToolUtil.HMACSHA1(code, LoginCtrl.pwdKey);
                LoginCtrl.Instance.loginType = 0; // Guest login
                LoginCtrl.Instance.SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", LoginCtrl.Instance.channelId + "", "", "", "", "HWCNDY37_" + SystemInfo.deviceUniqueIdentifier);
                // LoginCtrl.Instance.SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", LoginCtrl.Instance.channelId + "", "", "", "", "HWCNDY34_4490105bca9a7f4f8696a6a6d17dc26a1e9997f6f");

            }


        }
        public void OnComece2Btn()
        {
            OnLoginBtn();
        }

        public void OnComeceBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!m_Tog_Btn.isOn)
            {
                ToolUtil.FloattingText("请选择同意用户服务协议及隐私政策!",transform);
                return;
            }
            if (!SdkCtrl.Instance.isCanLogin())
            {
                return;
            }
            if (!SdkCtrl.Instance.isPermission())
            {
                SdkCtrl.Instance.perMission();
                return;
            }
            SdkCtrl.Instance.WxLogin();
            // var code = ToolUtil.GetRandomCode(12);
            // var hash = ToolUtil.HMACSHA1(code, LoginCtrl.pwdKey);
            // LoginCtrl.Instance. SendReqcLogin("", "", "", 101, 0, 0, code, hash, "", "3.0.1", LoginCtrl.Instance.channelId + "", "", "", "", SystemInfo.deviceUniqueIdentifier);

        }
        public void onValueChange(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (isOn)
            {
                SdkCtrl.Instance.perMission();
                PlayerPrefs.SetInt("ISCHECKYINSI", 1);
            }
            else
            {
                PlayerPrefs.SetInt("ISCHECKYINSI", 0);
            }
        }

        public void OnUsrBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Application.OpenURL(HotStart.ins.m_backstage + "/user.html");
           // MainPanelMgr.Instance.ShowDialog("PrivacyPanel", true, 0);
        }
        public void OnYinsiBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Application.OpenURL(HotStart.ins.m_backstage + "/yinsi.html");
            //   MainPanelMgr.Instance.ShowDialog("PrivacyPanel",true,1);

        }

        void CloseComeceBtn() 
        {
            m_Btn_Comece.interactable = false;
        }

        void OpenComeceBtn() 
        {
            m_Btn_Comece.interactable = true;
        }
    }
}
