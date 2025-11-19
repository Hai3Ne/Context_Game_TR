using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HotUpdate
{
    public partial class BenefitPanelPanel : PanelBase
    {
        private long cachedGolds; // Cache gold value from UPDATE_GOLD message

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            m_Btn_Get.interactable = true;
            SetUpPanel();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            //m_Btn_leftInfo.onClick.AddListener(OnLeftInfoBtn);
            //m_Btn_Cancelar.onClick.AddListener(OnCloseLeftInfo);
            //m_Btn_LeftInfoClose.onClick.AddListener(OnCloseLeftInfo);
            m_Btn_Get .onClick.AddListener(OnLeftReceberBtn);
            m_Btn_NoGet .onClick.AddListener(OnGLeftReceberBtn);
            Message.AddListener(MessageName.REFRESH_ALMS_PANEL, RefreshPanel);
            Message.AddListener(MessageName.UPDATE_GOLD, OnGoldUpdated);
            // Initialize cached gold value
            cachedGolds = MainUIModel.Instance.Golds;
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            //m_Btn_leftInfo.onClick.RemoveListener(OnLeftInfoBtn);
            //m_Btn_Cancelar.onClick.RemoveListener(OnCloseLeftInfo);
            //m_Btn_LeftInfoClose.onClick.RemoveListener(OnCloseLeftInfo);
            m_Btn_Get.onClick.RemoveListener(OnLeftReceberBtn);
            m_Btn_NoGet.onClick.RemoveListener(OnGLeftReceberBtn);
            Message.RemoveListener(MessageName.REFRESH_ALMS_PANEL, RefreshPanel);
            Message.RemoveListener(MessageName.UPDATE_GOLD, OnGoldUpdated);
        }

        private void OnGoldUpdated(params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                cachedGolds = (long)args[0];
            }
        }

        public void OnGLeftReceberBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            ToolUtil.FloattingText("今天的救助金已用完，请明天再来", this.transform);
        }


        public void OnLeftReceberBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            // Use cached gold value instead of reading directly from MainUIModel
            var num = float.Parse(ToolUtil.ShowF2Num(cachedGolds), new CultureInfo("en"));
            if (num >= 5000)
            {
                ToolUtil.FloattingText("当您的余额低于 5000 时可用", this.transform);
                return;
            }
            if ((ToolUtil.getServerTime() <= MainUIModel.Instance.almsData.JJJClaimTime + 86400)
                && MainUIModel.Instance.CurrnetAlmsCount >= MainUIModel.Instance.AlmsCount)
            {
                ToolUtil.FloattingText("今天的救助金已用完，请明天再来", this.transform);
                return;
            }
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Get, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendGetJJJ();
        }

        public void OnCloseBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("BenefitPanel");
            //if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            //{
            //    MainUICtrl.Instance.OpenFirstChargePanel();
            //}
        }

        public void SetUpPanel()
        {
            //Debug.LogError(">>>>>>>>>>>"+ MainUIModel.Instance.CurrnetAlmsCount+"===="+ MainUIModel.Instance.AlmsCount+"===="+ (ToolUtil.getServerTime() <= MainUIModel.Instance.almsData.JJJClaimTime + 86400));
            var showGReceber = MainUIModel.Instance.CurrnetAlmsCount >= MainUIModel.Instance.AlmsCount && ToolUtil.getServerTime() <= MainUIModel.Instance.almsData.JJJClaimTime + 86400;
            m_Btn_NoGet.gameObject.SetActive(showGReceber);
            var showReceber = MainUIModel.Instance.CurrnetAlmsCount < MainUIModel.Instance.AlmsCount || ToolUtil.getServerTime() >= MainUIModel.Instance.almsData.JJJClaimTime + 86400;
            m_Btn_Get.gameObject.SetActive(showReceber);
            m_Txt_GoldNum.text = $"{MainUIModel.Instance.AlmsCondition}";
        }

        public void RefreshPanel()
        {
      
            SetUpPanel();
            MainPanelMgr.Instance.Close("BenefitPanel");
        }
    }
}
