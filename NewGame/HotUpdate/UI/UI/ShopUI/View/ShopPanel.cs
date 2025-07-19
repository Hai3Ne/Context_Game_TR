using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace HotUpdate
{
    public partial class ShopPanel : PanelBase
    {
        private List<ShopItemData> shopItemList = new List<ShopItemData>();
        private string prefabName = "ShopItem";
        private bool isBuyFirstCharge 
        {
            get 
            {
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool isFirst1);
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool isFirst2);
                var isBuyedFirst = isState && isFirst1 && isFirst2;
                return isBuyedFirst;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            //var rect = m_VGridScroll_HeadList.GetComponent<RectTransform>();
            //var posX = rect.anchoredPosition.x;
            //var rect1 = gameObject.GetComponent<RectTransform>();
            //var width = rect1.sizeDelta.x - (rect1.sizeDelta.x  + posX);
            //rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
            shopItemList = MainUIModel.Instance.shopdata.shopItemDatas.FindAll(x => x.shopId != 100&&x.shopId!=301 && x.shopId != 302);
            m_VGridScroll_HeadList.InitGridView(shopItemList.Count, OnGetItemByRowColumn);
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

        }

        protected override void Update()
        {
            base.Update();
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            Message.AddListener(MessageName.REFRESH_SHOP_PANEL, RefresShopList);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            Message.RemoveListener(MessageName.REFRESH_SHOP_PANEL, RefresShopList);
        } 
        #endregion
        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            ShopCtrl.Instance.CloseShopPanel();
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));
      
            if (num < 5000 && MainUIModel.Instance.RoomData != null && MainUIModel.Instance.CurrnetAlmsCount < MainUIModel.Instance.AlmsCount)
            {
                MainUICtrl.Instance.OpenAlmsPanel();
            }
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex >=shopItemList.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<ShopItem>();
            script.SetUpItem(shopItemList[itemIndex]);
            return item;
        }

        public void RefresShopList()
        {
            shopItemList = MainUIModel.Instance.shopdata.shopItemDatas.FindAll(x => x.shopId != 100 && x.shopId != 301 && x.shopId != 302);
            m_VGridScroll_HeadList.SetListItemCount(shopItemList.Count, true);
            m_VGridScroll_HeadList.RefreshAllShownItem();
        }
    }
}
