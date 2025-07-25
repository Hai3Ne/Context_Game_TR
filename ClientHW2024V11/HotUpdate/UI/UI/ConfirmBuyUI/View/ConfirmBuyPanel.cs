
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using SEZSJ;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class ConfirmBuyPanel : PanelBase
    {
        private cfg.Game.Shop_Config goodsData;
        private VipItemData vipData;
        private int payID;//2,微信支付 ,1.支付宝支付
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
            SdkCtrl.Instance.isClick = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
        }
        #region 事件绑定
        public void RegisterListener()
        {
 
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Buy.onClick.AddListener(OnBuyBtn);
            m_Tog_Off.onValueChanged.AddListener(Toggle1Changed);
            m_Tog_On.onValueChanged.AddListener(Toggle2Changed);
        }

        public void UnRegisterListener()
        {
      
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Buy.onClick.RemoveListener(OnBuyBtn);
            m_Tog_Off.onValueChanged.RemoveListener(Toggle1Changed);
            m_Tog_On.onValueChanged.RemoveListener(Toggle2Changed);
        }

        #endregion


        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("ConfirmBuyPanel");
        }
        public void OnBuyBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!HotStart.ins.m_isShow) return;
            //if (HotStart.ins.m_isShow) return;
            if (goodsData==null&&vipData==null)
            {
                return;
            }
            MainUICtrl.Instance.SendBuyShopItem(goodsData.BuyId, payID);
       
        }


        public void SetUpPanel(cfg.Game.Shop_Config data) 
        {
            goodsData = data;
            //m_Img_Icon.sprite = $"";
            m_Txt_IconNum.text = ToolUtil.AbbreviateNumberf0(data.Diamond * 100);// $"{}";
            if (data.BuyType==8)
            {
                m_Txt_PackName.text = $"月卡礼包";
            }
            else
            {
                m_Txt_PackName.text = $"金币礼包";  

            }
            m_Txt_SellNum.text = $"{data.BuyNew/100}元";
            var payLimit = int.Parse(LoginCtrl.Instance.payLimit);
            var isShowAlPay = data.BuyNew / 100 >= payLimit;
            m_Tog_Off.gameObject.SetActive(isShowAlPay);
            if (isShowAlPay)
            {
                m_Tog_Off.isOn = true;
                m_Tog_On.isOn = false;
                payID = 1;
            }
            else
            {
               
                m_Tog_On.isOn = true;
                payID = 2;

            }

         
        }
        public void SetUpPanel(VipItemData data)
        {
            goodsData = null;
            vipData = data;
            //m_Img_Icon.sprite = $"";
            m_Txt_IconNum.text = $"{data.ShopData.diamond*100}";
            m_Txt_PackName.text = $"Vip{data.VipLv}礼包";
            m_Txt_SellNum.text = $"{data.ShopData.realityPirce / 100}元";
            var payLimit = int.Parse(LoginCtrl.Instance.payLimit);
            var isShowAlPay = data.ShopData.realityPirce / 100 >= payLimit;
            m_Tog_Off.gameObject.SetActive(isShowAlPay);
            m_Tog_On.isOn = true;
            payID = 2;
        }

        private void Toggle1Changed(bool isOn)
        {

            if (isOn)
            {
                //m_Tog_On.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_choose_gray@2x");
                CoreEntry.gAudioMgr.PlayUISound(46);
                payID = 1;
                //m_Img_alibar.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_pay");
                //m_Img_alicon.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:icon_pick_Alipay");
                //m_Txt_aliLabel.color = new Color(255f, 255f, 255f, 255f);


            }
            else
            {
                //m_Img_alibar.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_pay_1");
                //m_Img_alicon.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:icon_Alipay");
                //m_Txt_aliLabel.color = new Color(167 / 255f, 146 / 255f, 128 / 255f, 255f);

                //m_Tog_On.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_choose_green@2x");
            }
        }

        private void Toggle2Changed(bool isOn)
        {
            if (isOn)
            {
                //m_Tog_Off.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_choose_gray@2x");
                CoreEntry.gAudioMgr.PlayUISound(46);
                payID = 2;
                //m_Img_wxbar.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_pay");
                //m_Img_wxicon.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:icon_pick_wx");
                //m_Txt_wxLabel.color = new Color(255f, 255f, 255f, 255f);
            }
            else
            {
                //m_Img_wxbar.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_pay_1");
                //m_Img_wxicon.sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:icon_wx");
                //m_Txt_wxLabel.color = new Color(167 / 255f, 146 / 255f, 128 / 255f, 255f);
                //m_Tog_Off.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"ConfirmBuyPanel:btn_choose_green@2x");
            }
        }


    }
}
