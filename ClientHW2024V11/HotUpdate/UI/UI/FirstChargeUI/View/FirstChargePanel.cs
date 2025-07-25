using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace HotUpdate
{
    public partial class FirstChargePanel : PanelBase
    {
        private ShopItemData shopItemData;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
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
            if (MainUIModel.Instance.bOpenAlmsPanel)
            {
                MainUICtrl.Instance.OpenShopPanel();

                //var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));                
                //if (num < 5000 && MainUIModel.Instance.CurrnetAlmsCount < MainUIModel.Instance.AlmsCount)
                //{
                //    MainUICtrl.Instance.OpenAlmsPanel();
                //}
            }
            MainUIModel.Instance.bOpenAlmsPanel = false;
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Buy.onClick.AddListener(OnBuyBtn);
            Message.AddListener(MessageName.REFRESH_FIRSTCHARGE_PANEL,SetUpPanel);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Buy.onClick.RemoveListener(OnBuyBtn);
            Message.RemoveListener(MessageName.REFRESH_FIRSTCHARGE_PANEL, SetUpPanel);

        }

        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            FirstChargeCtrl.Instance.CloseFirstChargePanel();
        }

        public void OnBuyBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            //MainUICtrl.Instance.SendBuyShopItem(100);
            ConfirmBuyPanel confirmBuy = MainPanelMgr.Instance.ShowDialog("ConfirmBuyPanel") as ConfirmBuyPanel;
            var config = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.Find(x=>x.BuyId==100);
            confirmBuy.SetUpPanel(config);
        }

        public void SetUpPanel() 
        {
            if (!HotStart.ins.m_isShow)
            {
                var config = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.Find(x => x.BuyId == 100);
                m_Txt_Num1.text = $"{ToolUtil.AbbreviateNumberf0(config.Diamond * 100)}";
                m_Txt_Money.text = $"{ToolUtil.ShowF2Num2Shop(config.BuyNew)}元购买";
                m_Btn_canNotBuy.gameObject.SetActive(false);
                m_Btn_Buy.gameObject.SetActive(true);
            }
            else
            {
                shopItemData = MainUIModel.Instance.shopdata.shopItemDatas.Find(x => x.shopId == 100);
                m_Txt_Num1.text = $"{ToolUtil.AbbreviateNumberf0(shopItemData.diamond * 100)}金币";
                // m_Txt_Num2.text = $"钻石X{shopItemData.GiveDiamond}";
            //    m_Txt_CoinNum.text = $"{ToolUtil.AbbreviateNumberf0(shopItemData.diamond * 100)}";
                m_Txt_Money.text = $"{ToolUtil.ShowF2Num2Shop(shopItemData.realityPirce)}元购买";
                m_Txt_canNotMoney.text = $"{ToolUtil.ShowF2Num2Shop(shopItemData.realityPirce)}元购买";
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
                m_Btn_canNotBuy.gameObject.SetActive(isState);
                m_Btn_Buy.gameObject.SetActive(!isState);
            }

        }
    }
}
