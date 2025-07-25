using DG.Tweening;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class ExchangeItem : MonoBehaviour
    {
        [SerializeField] private Text title;
        [SerializeField] private Text goldTxt;
        [SerializeField] private Text goldTxt2;
        [SerializeField] private GameObject tag;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] public Text btnTxt;
        [SerializeField] private Text canNotBuyTxt;
        [SerializeField] private Button canNotBuyBtn;
        public cfg.Game.ItemExchange ItemData;
        private Transform m_Trans_Dramond;
        private Text m_TextM_DramondNum;
        private void Awake()
        {
            m_Trans_Dramond = transform.Find("Trans_Dramond");
            m_TextM_DramondNum = m_Trans_Dramond.Find("TextM_DramondNum").GetComponent<Text>();
        }
        public void OnEnable()
        {
            button.onClick.AddListener(OnClickBtn);
            canNotBuyBtn.onClick.AddListener(OnClickNotBuyBtn);
            transform.DOScale(Vector3.one, 0.1f);
        }

       

        public void OnDisable()
        {
            button.onClick.RemoveListener(OnClickBtn);
            canNotBuyBtn.onClick.RemoveListener(OnClickNotBuyBtn);
            //transform.localScale = Vector3.zero;
        }

        public void SetUpItem(cfg.Game.ItemExchange data)
        {
            ItemData = data;
            title.text = string.Format("每日兑换{0}次",data.Day);
            if (data.Day == 0)
                title.text = "可兑换";
            btnTxt.text = ((double)data.Original / 100f) + "";
            canNotBuyTxt.text = ((double)data.Original / 100f) + "";
       
            goldTxt.text = data.Type == 9 ? $"{ToolUtil.AbbreviateNumberf0(data.Target)}金币" : ((double)data.Target/100f) + "元";
            goldTxt.gameObject.SetActive(data.Type == 9);
            goldTxt2.gameObject.SetActive(data.Type != 9);
            goldTxt2.text = data.Type == 9 ? $"{ToolUtil.AbbreviateNumberf0(data.Target)}金币" : ((double)data.Target / 100f)+"元";
            if (data.Type == 9)
            {
                icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:ddfl_icon_{data.Itemid}");
            }
            else
            {
                icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:zfb");
            }
            
            icon.SetNativeSize();
            button.gameObject.SetActive(true);
            canNotBuyBtn.gameObject.SetActive(false);

        }

        public void OnClickBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsCashBlind, out bool IsCashBlind);
        
            if (ItemData.Type == 9)
            {
                if (MainUIModel.Instance.palyerData.m_i8Diamonds >= ItemData.Original)
                {
                    var cmp = MainPanelMgr.Instance.GetDialog("ExchangePanel").GetComponent<ExchangePanel>();
                    cmp.showRectTips(ItemData);
              
                }
                else
                {
                    ToolUtil.FloattingText("星点不足", MainPanelMgr.Instance.GetDialog("ExchangePanel").transform);
                }
         
            }
            else
            {
                if (MainUIModel.Instance.pixData != null && MainUIModel.Instance.pixData.AccountNum != "" && IsCashBlind)
                {
                    if (MainUIModel.Instance.palyerData.m_i8Diamonds >= ItemData.Original)
                    {
                        if (ItemData.Page == 2)
                        {
                            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
                            if (!isState)
                            {
                                ToolUtil.FloattingText("充值即可领取", MainPanelMgr.Instance.GetDialog("ExchangePanel").transform);
                                return;
                            }
                        }
                        var cmp = MainPanelMgr.Instance.GetDialog("ExchangePanel").GetComponent<ExchangePanel>();
                        cmp.showRectTips(ItemData);
                    }
                    else
                    {
                        ToolUtil.FloattingText("星点不足", MainPanelMgr.Instance.GetDialog("ExchangePanel").transform);
                    }

                }
                else
                {
                    MainPanelMgr.Instance.ShowDialog("RedeemBind");
                }
            }

           
        }

        public void OnClickNotBuyBtn()
        {
            //if (shopData.BuyType == 4 || shopData.BuyType == 5 || shopData.BuyType == 6 || shopData.BuyType == 7)
            //{
            //    ToolUtil.FloattingText("只能购买一次哦，请购买其他商品", MainPanelMgr.Instance.GetPanel("NoviceGiftPanel").transform);
            //}
           var  config = ConfigCtrl.Instance.Tables.TbItemExchange.DataList.Find(x => x.Type == ItemData.Type && x.Itemid == ItemData.Itemid);

           
        }

        /// <summary>
        /// 是否购买新手礼包
        /// true已购买
        /// false 未购买
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsNovicePack(cfg.Game.Shop_Config data)
        {
            bool canBuy = false;
            switch (data.BuyType)
            {
                case 4:
                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool E_First1);
                    canBuy = E_First1;
                    break;
                case 5:
                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool E_First2);
                    canBuy = E_First2;
                    break;
                    /*            case 6:
                                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First3, out bool E_First3);
                                    canBuy = E_First3;
                                    break;
                                case 7:
                                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First4, out bool E_First4);
                                    canBuy = E_First4;
                                    break;*/
            }
            return canBuy;
        }
    }
}
