using DG.Tweening;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace HotUpdate
{
    public partial class PersonDataPanel : PanelBase
    {

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

           
            m_Rect_out.gameObject.SetActive(false);
            RegisterListener();
            SetUpPanel();
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
            m_Btn_AtlasChoose.onClick.AddListener(OnAtlasChooseBtn);
            m_Btn_Copy.onClick.AddListener(OnCopyBtn);
      
            m_Btn_Pen.onClick.AddListener(OnPenBtn);
   
            m_Btn_LoginOut.onClick.AddListener(OnLoginOutBtn);
   
            m_Btn_Sure.onClick.AddListener(OnSureBtn);
            m_Btn_Quxiao.onClick.AddListener(OnQuxiaoBtn);
            Message.AddListener(MessageName.REFRESH_PERSONDATA_PANEL, SetUpPanel);
         
            m_Btn_Customer.onClick.AddListener(ClickCustomer);
            m_Tog_Sound.onValueChanged.AddListener(OnMusicBtn);
            m_Tog_Effect.onValueChanged.AddListener(OnEfeitoBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_AtlasChoose.onClick.RemoveListener(OnAtlasChooseBtn);
            m_Btn_Copy.onClick.RemoveListener(OnCopyBtn);
            m_Btn_LoginOut.onClick.RemoveListener(OnLoginOutBtn);
            m_Btn_Pen.onClick.RemoveListener(OnPenBtn);

            m_Btn_Sure.onClick.RemoveListener(OnSureBtn);
            m_Btn_Quxiao.onClick.RemoveListener(OnQuxiaoBtn);
            Message.RemoveListener(MessageName.REFRESH_PERSONDATA_PANEL, SetUpPanel);
     
            m_Btn_Customer.onClick.RemoveListener(ClickCustomer);
            m_Tog_Sound.onValueChanged.RemoveListener(OnMusicBtn);
            m_Tog_Effect.onValueChanged.RemoveListener(OnEfeitoBtn);
        }
        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            PersonDataCtrl.Instance.ClosePersonDataPanel();
        }


        private void ClickRecharge()
        {
            Close();

        }

        private void ClickSetting()
        {

        }

        private void ClickCustomer()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var str = string.Format("http://{0}/index/index/home?theme=05202d&visiter_id=&visiter_name=&avatar=&business_id=1&groupid=0&roleid={1}", HotStart.ins.m_customer, MainUIModel.Instance.palyerData.m_i8roleID);
            Application.OpenURL(str);
        }

        public void OnAtlasChooseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            PersonDataCtrl.Instance.OpenPersonAtlasPanel();
        }


        public void SetUpPanel() 
        {
            m_Input_nameInput.text = "";
            m_Input_nameInput.text = "";
            m_Rect_Panel.gameObject.SetActive(true);

            StartCoroutine(ToolUtil.GetHeadImage(Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_szHeadUrl).Replace("\0", null), m_Img_head));
            //StartCoroutine(GetHeadImage("https://thirdwx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTKic1Wd7e5iacNb9JxUNQGjrS4sJKS05mPAySaTBmibibJKomB7gYWbbFujFeFUDtVnN4ekorX4ms6pYA/132"));
            // StartCoroutine(GetHeadImage(Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_HeadUrl).Replace("\0", null)));
            m_Input_nameInput.text = Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName);
            m_Txt_UID.text = $"{MainUIModel.Instance.palyerData.m_i8roleID}";

        }

        public void OnCopyBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            GUIUtility.systemCopyBuffer = m_Txt_UID.text;
            ToolUtil.FloattingText("复制成功", this.gameObject.transform);
        }

        public void OnSendBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!MainUIModel.Instance.isBindPhone)
            {
                MainUICtrl.Instance.OpenPhoneBindPanel();
                return;
            }
        }

        public void OnBindBtn()
        {
            MainPanelMgr.Instance.ShowDialog("CadastrPanel",true,1);
        }
        public void OnLoginOutBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_out.gameObject.SetActive(true);

        }

        public void OnSureBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            PersonDataCtrl.Instance.ClosePersonDataPanel();
            MainUIModel.Instance.isOpenGuid = false;
            MainUIModel.Instance.CashOutCount = 3;
            MainUIModel.Instance.pixData = null;
            MainUIModel.Instance.cashOutTotalDay = 0;
            SdkCtrl.Instance.SendEvent(ApplyType.pagePause);
            UICtrl.Instance.OpenView("LoginPanel");
        }

        public void OnQuxiaoBtn()
        {
            m_Rect_out.gameObject.SetActive(false);
        }
        public void OnPenBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!MainUIModel.Instance.isBindPhone)
            {
                ToolUtil.FloattingText("Por favor, vincule seu telefone primeiro",this.gameObject.transform);
            }
            else
            {
                MainUICtrl.Instance.OpenRenamePanel();
                //m_Input_nameInput.interactable = true;
            }
        }

        public void OnSongBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
   

        }


        public void OnMusicBtn(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (CoreEntry.gAudioMgr.musicMute)
                CoreEntry.gAudioMgr.SetAudioMusicPrefs(0);
            else
                CoreEntry.gAudioMgr.SetAudioMusicPrefs(1);
            SetMusicPanel();
        }

        public void OnEfeitoBtn(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (CoreEntry.gAudioMgr.soundMute)
                CoreEntry.gAudioMgr.SetAudioSoundPrefs(0);
            else
                CoreEntry.gAudioMgr.SetAudioSoundPrefs(1);
            SetSoundPanel();
        }

        public void SetMusicPanel()
        {
            if(!CoreEntry.gAudioMgr.musicMute)
            {
                m_Tog_Sound.transform.GetChild(1).gameObject.SetActive(true);
                m_Tog_Sound.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                m_Tog_Sound.transform.GetChild(1).gameObject.SetActive(false);
                m_Tog_Sound.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        public void SetSoundPanel()
        {
            if (!CoreEntry.gAudioMgr.soundMute)
            {
                m_Tog_Effect.transform.GetChild(1).gameObject.SetActive(true);
                m_Tog_Effect.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                m_Tog_Effect.transform.GetChild(1).gameObject.SetActive(false);
                m_Tog_Effect.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }
}