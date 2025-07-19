using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class WishingElfPanel : PanelBase
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
            if (IsClaimed && m_Txt_TimeUp != null)
            {
                m_Txt_TimeUp.gameObject.SetActive(true);
                m_Txt_TimeUp.text = TimeUtil.DateDiffByDay("", TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(MainUIModel.Instance.luckyCatData.CatClaimTime).AddDays(1));
            }
            else
            {
                m_Txt_TimeUp.gameObject.SetActive(false);
            }
        }

        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Buy.onClick.AddListener(OnBuyBtn);
            m_Btn_Rule.onClick.AddListener(ClickRule);
            Message.AddListener(MessageName.REFRESH_WISHINGELF_PANEL, SetUpPanel);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Buy.onClick.RemoveListener(OnBuyBtn);
            m_Btn_Rule.onClick.RemoveListener(ClickRule);
            Message.RemoveListener(MessageName.REFRESH_WISHINGELF_PANEL, SetUpPanel);
        }

        #endregion

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("WishingElfPanel");
        }

        public void OnBuyBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (IsClaimed)
            {
                ToolUtil.FloattingText("领取时间未到，请稍候再来", this.transform);
                return;
            }
            bool isState;
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out isState);
            if (isState)
            {
                StartCoroutine(ToolUtil.DelayResponse(m_Btn_Buy, 1f));
                MainUICtrl.Instance.SendGetZccat();
            }
            else
            {
                MainUICtrl.Instance.OpenSmallTipsPanel();
            }
        }


        private void ClickRule()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            CommonPanel panel = MainPanelMgr.Instance.ShowDialog("CommonPanel") as CommonPanel;
            panel.SetContent("规则说明", "1.任意充值即可激活愿望精灵\n2.每24小时可许愿一次\n3.每累计充值10元可升级一次，每级可多获得400金币，最高可升至30010级(即可获得12004000金币）");
        }

        public void SetUpPanel()
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
   
       
            //m_TxtM_Tips.text = $"本次许愿需要消耗钻石：{catLev}";
      
            long num = MainUIModel.Instance.palyerData.m_i4CatExp;
            if (num >= ConfigCtrl.Instance.Tables.TbConst_Config.GetOrDefault(2).Val2)
            {
                num = ConfigCtrl.Instance.Tables.TbConst_Config.GetOrDefault(2).Val2;
            }
            else if (num==0)
            {
                num = ConfigCtrl.Instance.Tables.TbConst_Config.GetOrDefault(2).Fval;
            }

            m_Txt_GetGoldNum.text =$"{num}";
            m_Txt_NeedVIP.text = $"{num/400}";
        }
    }

}
