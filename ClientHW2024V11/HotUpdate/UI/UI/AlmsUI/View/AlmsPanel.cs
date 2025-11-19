using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class AlmsPanel : PanelBase
    {
        private long cachedGolds; // Cache gold value from UPDATE_GOLD message

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }
        protected override void Start()
        {
            base.Start();
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Trans_CoinNum);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            m_Btn_leftReceber.interactable = true;
            SetUpPanel();
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
               m_Rect_tips.gameObject.SetActive(true);
            }
            m_Img_leftRS.gameObject.SetActive(MainUIModel.Instance.bNormalGame);
            m_Img_GoldIcon.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            m_Trans_CoinNum.GetComponent<HorizontalLayoutGroup>().spacing =  MainUIModel.Instance.bNormalGame ? 15 : -110;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Trans_CoinNum);
            m_TxtM_LeftInfoContent.text = m_TxtM_LeftInfoContent.text.Replace("R$",ToolUtil.GetCurrencySymbol());
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
            m_Btn_leftInfo.onClick.AddListener(OnLeftInfoBtn);
            m_Btn_Cancelar.onClick.AddListener(OnCloseLeftInfo);
            m_Btn_LeftInfoClose.onClick.AddListener(OnCloseLeftInfo);
            m_Btn_leftReceber.onClick.AddListener(OnLeftReceberBtn);
            m_Btn_GLeftReceber.onClick.AddListener(OnGLeftReceberBtn);
            Message.AddListener(MessageName.REFRESH_ALMS_PANEL, RefreshPanel);
            Message.AddListener(MessageName.UPDATE_GOLD, OnGoldUpdated);
            // Initialize cached gold value
            cachedGolds = MainUIModel.Instance.Golds;
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_leftInfo.onClick.RemoveListener(OnLeftInfoBtn);
            m_Btn_Cancelar.onClick.RemoveListener(OnCloseLeftInfo);
            m_Btn_LeftInfoClose.onClick.RemoveListener(OnCloseLeftInfo);
            m_Btn_leftReceber.onClick.RemoveListener(OnLeftReceberBtn);
            m_Btn_GLeftReceber.onClick.RemoveListener(OnGLeftReceberBtn);
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

        //protected override void Update()
        //{
        //    base.Update();
        //    if (IsClaimed)
        //        m_TxtM_timeUp.text = TimeUtil.DateDiffByDay("Próximo Bônus em:", TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(MainUIModel.Instance.almsData.CatClaimTime).AddDays(1));
        //}

        #endregion

        public void OnCloseBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            AlmsCtrl.Instance.CloseAlmsPanel();
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                MainUICtrl.Instance.OpenFirstChargePanel();
            }
        }

        public void OnLeftInfoBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_LeftInfo.gameObject.SetActive(true);
        }


        public void OnCloseLeftInfo()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_LeftInfo.gameObject.SetActive(false);
        }

        public void OnGLeftReceberBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            ToolUtil.FloattingText("O fundo de ajuda de hoje se esgotou, por favor, volte amanhã", this.transform);
        }


        public void OnLeftReceberBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            // Use cached gold value instead of reading directly from MainUIModel
            var num = float.Parse(ToolUtil.ShowF2Num(cachedGolds), new CultureInfo("en"));
            if (num >= 2)
            {
                ToolUtil.FloattingText("Disponível quando seu saldo for inferior a R$ 2", this.transform);
                return;
            }
            if ((ToolUtil.getServerTime() <= MainUIModel.Instance.almsData.JJJClaimTime + 86400)
                && MainUIModel.Instance.CurrnetAlmsCount >= MainUIModel.Instance.AlmsCount)
            {
                ToolUtil.FloattingText("O fundo de ajuda de hoje se esgotou, por favor, volte amanhã", this.transform);
                return;
            }
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_leftReceber, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendGetJJJ();
        }


        public void SetUpPanel()
        {
            var showGReceber = MainUIModel.Instance.CurrnetAlmsCount >= MainUIModel.Instance.AlmsCount && ToolUtil.getServerTime() <= MainUIModel.Instance.almsData.JJJClaimTime + 86400;
            m_Btn_GLeftReceber.gameObject.SetActive(showGReceber); 
            var showReceber = MainUIModel.Instance.CurrnetAlmsCount < MainUIModel.Instance.AlmsCount || ToolUtil.getServerTime() >= MainUIModel.Instance.almsData.JJJClaimTime + 86400;
            m_Btn_leftReceber.gameObject.SetActive(showReceber);
            m_Txt_leftCoinNum.text =$"{MainUIModel.Instance.AlmsCondition}";
        }

        public void RefreshPanel()
        {
            SetUpPanel();
        }
    }
}
