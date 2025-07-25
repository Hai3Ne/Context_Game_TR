using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class LuckyCatPanel : PanelBase
    {
        //是否已领取招财猫奖励
        public bool IsClaimed
        {
            get
            {
                return MainUIModel.Instance.luckyCatData.CatClaimTime != 0 && ToolUtil.getServerTime() <= MainUIModel.Instance.luckyCatData.CatClaimTime + 86400;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            SetUpPanel();
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                m_Rect_tips.gameObject.SetActive(true);
            }
            this.m_Trans_lcon.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            m_Img_leftRS.gameObject.SetActive(MainUIModel.Instance.bNormalGame);
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
            m_Btn_GLeftReceber.onClick.AddListener(OnCanNotBuyBtn);
            Message.AddListener(MessageName.REFRESH_ALMS_PANEL, RefreshPanel);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_leftInfo.onClick.RemoveListener(OnLeftInfoBtn);
            m_Btn_Cancelar.onClick.RemoveListener(OnCloseLeftInfo);
            m_Btn_LeftInfoClose.onClick.RemoveListener(OnCloseLeftInfo);
            m_Btn_leftReceber.onClick.RemoveListener(OnLeftReceberBtn);
            m_Btn_GLeftReceber.onClick.RemoveListener(OnCanNotBuyBtn);
            Message.RemoveListener(MessageName.REFRESH_ALMS_PANEL, RefreshPanel);
        }

        protected override void Update()
        {
            base.Update();
            if (IsClaimed)
                m_TxtM_timeUp.text = TimeUtil.DateDiffByDay("Próximo Bônus em:", TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(MainUIModel.Instance.luckyCatData.CatClaimTime).AddDays(1));
        }

        #endregion

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.CloseLuckyCatPanel();
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                MainUICtrl.Instance.OpenAlmsPanel();
            }
        }

        public void OnLeftInfoBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_LeftInfo.gameObject.SetActive(true);
        }
        public void OnCloseLeftInfo()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_LeftInfo.gameObject.SetActive(false);
        }

        public void OnCanNotBuyBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            ToolUtil.FloattingText("O bônus de hoje foi reivindicado,\n  volte novamente após o término da     \n      contagem regressiva para reivindicá-lo", this.transform);
        }

        public void OnLeftReceberBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
            var isBuyedFirst = isState;
            if (isBuyedFirst)
            {
                StartCoroutine(ToolUtil.DelayResponse(m_Btn_leftReceber, 1f));
                MainUICtrl.Instance.SendGetZccat();
            }
            else
            {
                MainUICtrl.Instance.OpenSmallTipsPanel();
            }
        }


        public void SetUpPanel()
        {

            m_Btn_GLeftReceber.gameObject.SetActive(IsClaimed);
            m_Btn_leftReceber.gameObject.SetActive(!IsClaimed);
            m_Img_TimeUpBg.gameObject.SetActive(IsClaimed);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
            m_TxtM_luckyCatTxt.gameObject.SetActive(!isState);
            Debug.Log($"{(float)MainUIModel.Instance.Golds / 10000}");
            if (MainUIModel.Instance.palyerData.m_i4CatExp == 0)
            {
                m_Txt_leftCoinNum.text = ToolUtil.ShowF2Num(10000);

            }
            else
            {
                long num = MainUIModel.Instance.palyerData.m_i4CatExp;
                if (num >= ConfigCtrl.Instance.Tables.TbConst_Config.GetOrDefault(2).Val2)
                {
                    num = ConfigCtrl.Instance.Tables.TbConst_Config.GetOrDefault(2).Val2;
                }
                m_Txt_leftCoinNum.text = ToolUtil.ShowF2Num(num);

            }

        }

        public void RefreshPanel()
        {
            SetUpPanel();
        }
    }
}
