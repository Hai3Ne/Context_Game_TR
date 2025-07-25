using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class ShopPackPanel : PanelBase
    {
        private string prefabName = "ShopPackItem";
        private bool bFirst = true;
        List<Transform> tempList = new List<Transform>();
        ScrollRect scrollRect;
        private List<List<cfg.Game.Shop_Config>> roomList = new List<List<cfg.Game.Shop_Config>>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            //var rect = m_VGridScroll_DoublePackList.GetComponent<RectTransform>();
            //var posX = rect.anchoredPosition.x;
            //var rect1 = gameObject.GetComponent<RectTransform>();
            //var width = rect1.sizeDelta.x - (rect1.sizeDelta.x / 2 + posX);
            //rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
            getShopData();


            var goodsList = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.FindAll(x => x.BuyType == 2);
            m_VGridScroll_DoublePackList.InitGridView(goodsList.Count, OnGetItemByRowColumn);
            scrollRect = m_VGridScroll_DoublePackList.GetComponent<ScrollRect>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            //getShopData();
            //m_VGridScroll_DoublePackList.SetListItemCount(roomList.Count);
            //m_VGridScroll_DoublePackList.RefreshAllShownItem();
            PlayAni();
            if (HotStart.ins.m_isShow)
                SetRedDotData();

        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
        }
        private async void PlayAni()
        {
            scrollRect.verticalNormalizedPosition = 1;
            scrollRect.enabled = false;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.021f));
            Transform content = m_VGridScroll_DoublePackList.transform.GetChild(0).GetChild(0);
            float aniTimes = 0.4f;
            tempList.Clear();
            for (int i = 0; i < content.childCount; i++)
            {
                if (content.GetChild(i).gameObject.activeSelf)
                    tempList.Add(content.GetChild(i));
            }
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            tempList.Sort((a, b) =>
            {
                if (a.position.y == b.position.y)
                    return a.position.x.CompareTo(b.position.x);
                return -a.position.y.CompareTo(b.position.y);
            });
            for (int i = 0; i < content.childCount; i++)
                content.GetChild(i).gameObject.SetActive(false);

            for (int i = 0; i < tempList.Count; i++)
            {
                int index = i;
                tempList[index].gameObject.SetActive(true);
                tempList[index].GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, 200);
                CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                tempList[index].GetComponent<RectTransform>().DOAnchorPos(tempList[index].GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 200), aniTimes);
                DOTween.To(() => 0f, (value) =>
                {
                    CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                    canvasGroup.alpha = value;
                }, 0.8f, aniTimes).OnComplete(() =>
                {
                    CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 1;
                }).SetEase(Ease.Linear);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.15f));
            }
            bFirst = false;
            scrollRect.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            scrollRect.enabled = true;
            scrollRect.verticalNormalizedPosition = 1;
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds));

            if (num < 5000 && MainUIModel.Instance.CurrnetAlmsCount < MainUIModel.Instance.AlmsCount)
            {
                MainUICtrl.Instance.OpenAlmsPanel();
            }
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
        }
        #endregion

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("ShopPackPanel");
            if (HotStart.ins.m_isShow)
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);

        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            var goodsList = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.FindAll(x => x.BuyType == 2 && x.BuyId != 100);
            if (itemIndex < 0 || itemIndex >= goodsList.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<ShopPackItem>();
            script.SetUpItem(goodsList[itemIndex]);
            return item;
        }

        public void SetRedDotData()
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(ToolUtil.getServerTime() + 86400).ToLocalTime();
            DateTime zeroDateTime = dateTime.Date;
            long zeroTimestamp = zeroDateTime.ToUnixTimeSeconds();
            if (PlayerPrefs.HasKey($"ShopRedDot:+{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                PlayerPrefs.SetString($"ShopRedDot:+{MainUIModel.Instance.palyerData.m_i8roleID}", $"{zeroTimestamp}");
            }
            else
            {
                var time = PlayerPrefs.HasKey($"ShopRedDot:+{MainUIModel.Instance.palyerData.m_i8roleID}") ? long.Parse(PlayerPrefs.GetString($"ShopRedDot:+{MainUIModel.Instance.palyerData.m_i8roleID}")) : 0;
                var isShow = ToolUtil.getServerTime() >= time;
                if (isShow)
                {
                    PlayerPrefs.SetString($"ShopRedDot:+{MainUIModel.Instance.palyerData.m_i8roleID}", $"{zeroTimestamp}");
                }

            }
        }

        public void getShopData()
        {
            var dataList = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.FindAll(x => x.BuyType == 2 && x.BuyId != 100);
            var index = 0;
            while (dataList.Count > index)
            {
                var data = dataList[index];
                var list1 = new List<cfg.Game.Shop_Config>();
                list1.Add(data);

                index++;
                if (index > dataList.Count - 1)
                {
                    list1.Add(null);
                }
                else
                {
                    list1.Add(dataList[index]);
                }
                roomList.Add(list1);
                index++;
            }
        }
    }
}
