using cfg.Game;
using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class ExchangePanel : PanelBase
    {
        private string prefabName = "ExchangeItem";
        List<Transform> tempList = new List<Transform>();
        ScrollRect scrollRect;
        private List<List<cfg.Game.Shop_Config>> roomList = new List<List<cfg.Game.Shop_Config>>();

        public List<cfg.Game.ItemExchange> ItemsType9 = new List<cfg.Game.ItemExchange>();
        public List<cfg.Game.ItemExchange> ItemsType15 = new List<cfg.Game.ItemExchange>();
        public List<cfg.Game.ItemExchange> Items = new List<cfg.Game.ItemExchange>();
        List<Transform> Selects = new List<Transform>();
        List<Transform> UnSelects = new List<Transform>();
        public ItemExchange exchaneData;
        int type = 9;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);


            ItemsType9 = MainUIModel.Instance.getExChangeConfigByType(1);
            ItemsType15 = MainUIModel.Instance.getExChangeConfigByType(2);
            type = 9;
            Items = ItemsType9;
            m_VGridScroll_DoublePackList.InitGridView(ItemsType9.Count, OnGetItemByRowColumn);
            scrollRect = m_VGridScroll_DoublePackList.GetComponent<ScrollRect>();

            Selects.Add(m_Tog_Gift.transform.GetChild(1));
            Selects.Add(m_Tog_Card.transform.GetChild(1));
 

            UnSelects.Add(m_Tog_Gift.transform.GetChild(0));
            UnSelects.Add(m_Tog_Card.transform.GetChild(0));
  

        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int arg4)
        {
            if (itemIndex < 0 || itemIndex >= Items.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<ExchangeItem>();
            script.SetUpItem(Items[itemIndex]);
            return item;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            reloadUI();
            RegisterListener();
            SelectIndexPos();
     
        }

        public void reloadUI()
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsCashBlind, out bool IsCashBlind);
            if (MainUIModel.Instance.pixData != null && MainUIModel.Instance.pixData.AccountNum != "" && IsCashBlind && HotStart.ins.m_isShow)
            {
                m_Tog_Card.gameObject.SetActive(true);
                m_Tog_Gift.gameObject.SetActive(true);
            }
            else
            {
                m_Tog_Card.gameObject.SetActive(false);
                m_Tog_Gift.gameObject.SetActive(false);
            }
        }

        private async void SelectIndexPos()
        {
            var roomType = MainUIModel.Instance.RoomData != null ? MainUIModel.Instance.RoomData.nRoomType : 0;
            m_VGridScroll_DoublePackList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            if (m_Tog_Gift.isOn)
            {
                Refresh();
                PlayAni();
            }
                
            m_Tog_Gift.isOn = true;
            for (int i = 0; i < Selects.Count; i++)
            {
                Selects[i].gameObject.SetActive(i == 0);
            }

            for (int i = 0; i < UnSelects.Count; i++)
            {
                UnSelects[i].gameObject.SetActive(i != 0);
            }
        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Tog_Gift.onValueChanged.AddListener((isOn)=> 
            {
                OnClickTog(isOn,9);
            });
            m_Tog_Card.onValueChanged.AddListener((isOn)=> 
            {
                OnClickTog(isOn, 15);
            });
            Message.AddListener(MessageName.REFRESH_EXCHANGE, onDuihuanSucess);
            Message.AddListener(MessageName.REDEEBIND_RESULT, reloadUI);
            m_Btn_TipsClose.onClick.AddListener(closeTips);
            m_Btn_duihuan.onClick.AddListener(sendDuihuan);
        }

        public void showRectTips(ItemExchange item)
        {
            exchaneData = item;
            m_Rect_Tips.gameObject.SetActive(true);
            if(item.Type == 9)
            {
                m_Img_Icon.transform.localScale = new Vector3(1,1,1);
                m_Img_gold.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:ddfl_icon_{item.Itemid}");
                m_Img_gold.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:ddfl_icon_{item.Itemid}");
                m_Txt_lab1.text = ToolUtil.AbbreviateNumberf0(item.Target);
                m_Txt_ExchangeNum.text = ToolUtil.AbbreviateNumberf0(item.Target);
                m_Txt_Tips.text = "星点兑换金币没有库存限制\n当前兑换金币数量：" + ToolUtil.AbbreviateNumberf0(item.Target)+ "";
            }
            else
            {
                m_Img_Icon.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                m_Img_gold.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:zfb");
                m_Img_gold.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:zfb");
                m_Txt_lab1.text = (double)item.Target/100f + "";
                m_Txt_ExchangeNum.text = (double)item.Target / 100f + "";
                m_Txt_Tips.text = "星点兑换权益卡没有库存限制\n当前兑换权益卡数量：" + (double)item.Target / 100f + ""+ ""; ;
            }
            m_Img_Icon.SetNativeSize();
            m_Img_gold.SetNativeSize();
            m_Txt_lab2.text = (double)item.Original / 100f + "";
            m_Btn_duihuan.interactable = true;


        }


        public void onDuihuanSucess()
        {
            closeTips();
            ToolUtil.FloattingText("兑换成功", MainPanelMgr.Instance.GetDialog("ExchangePanel").transform);
            Refresh();
        }
        public void sendDuihuan()
        {
            ToolUtil.DelayResponse(m_Btn_duihuan,1);
            MainUICtrl.Instance.CS_DIAMOND_EXCHANGE_REQ(exchaneData.Type, exchaneData.Itemid);
        }

        public void closeTips()
        {
            m_Rect_Tips.gameObject.SetActive(false);
        }

        public void Refresh()
        {
            ItemsType9 = MainUIModel.Instance.getExChangeConfigByType(1);
            ItemsType15 = MainUIModel.Instance.getExChangeConfigByType(2);
            Items = type == 9 ? ItemsType9 : ItemsType15;

            m_VGridScroll_DoublePackList.SetListItemCount(Items.Count);
            m_VGridScroll_DoublePackList.RefreshAllShownItem();
        }


        public void OnClickTog(bool isOn,int itemtype)
        {
            if(itemtype == 9)
            {
                Selects[0].gameObject.SetActive(isOn);
                UnSelects[0].gameObject.SetActive(!isOn);
            }
            else
            {
                Selects[1].gameObject.SetActive(isOn);
                UnSelects[1].gameObject.SetActive(!isOn);
            }

            if (isOn)
            {
                type = itemtype;
                Items = type == 9 ? ItemsType9 : ItemsType15;
                m_VGridScroll_DoublePackList.SetListItemCount(Items.Count);
                m_VGridScroll_DoublePackList.RefreshAllShownItem();
                PlayAni();
            }
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Tog_Gift.onValueChanged.RemoveAllListeners();
            m_Tog_Card.onValueChanged.RemoveAllListeners();
            Message.RemoveListener(MessageName.REFRESH_EXCHANGE, onDuihuanSucess);
            Message.RemoveListener(MessageName.REDEEBIND_RESULT, reloadUI);
            m_Btn_TipsClose.onClick.RemoveListener(closeTips);
            m_Btn_duihuan.onClick.RemoveListener(sendDuihuan);
        }

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("ExchangePanel");
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);

        }
        private List<TaskItemData> tewen = new List<TaskItemData>();
        private async void PlayAni()
        {
            if (tewen.Count > 0)
            {
                foreach (var item in tewen)
                {
                    item.canvasGroup.DOKill();
                    item.rect.DOKill();
                    item.canvasGroup.alpha = 1;
                }
                tewen.Clear();
            }
            m_VGridScroll_DoublePackList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            scrollRect.enabled = false;
            scrollRect.verticalNormalizedPosition = 1;
            Transform content = m_VGridScroll_DoublePackList.transform.GetChild(0).GetChild(0);
            float aniTimes = 0.4f;
            tempList.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            for (int i = 0; i < m_VGridScroll_DoublePackList.ItemTotalCount; i++)
            {
                var item = m_VGridScroll_DoublePackList.GetShownItemByItemIndex(i);
                if (item != null)
                {
                    if (i < 20)
                    {

                        tempList.Add(item.transform);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }


            for (int i = 0; i < tempList.Count; i++)
            {
                int index = i;
                tempList[index].gameObject.SetActive(true);
                var rect = tempList[index].GetComponent<RectTransform>();
                var posY = rect.anchoredPosition.y;

                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 200);
                CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                var pos1 = new Vector2(rect.anchoredPosition.x, posY);
                var tween1 = rect.DOAnchorPos(pos1, aniTimes).SetDelay(0.15f * i); ;
                canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
                {
                    canvasGroup.alpha = 1;
                }).SetEase(Ease.Linear).SetDelay(0.15f * i);

                TaskItemData data = new TaskItemData();

                data.pos = pos1;
                data.rect = rect;
                data.canvasGroup = canvasGroup;
                tewen.Add(data);
                //await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.15f));
            }
            CoreEntry.gTimeMgr.RemoveTimer(251);
            CoreEntry.gTimeMgr.AddTimer(tempList.Count * 0.15f+0.5f, false, () => {
                if (scrollRect != null)
                    scrollRect.enabled = true;
                CoreEntry.gTimeMgr.RemoveTimer(251);

                  //  Handle();
                if (!GuideModel.Instance.bReachCondition(6))
                {
                    if (GuideModel.Instance.bReachCondition(8))
                        Handle();
                }
            }, 251);
        }

        private async void Handle()
        {
            GuideModel.Instance.SetFinish(8);
            scrollRect.verticalNormalizedPosition = 0;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.031f));
            for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
            {
                ExchangeItem temp = scrollRect.content.GetChild(i).GetComponent<ExchangeItem>();
                if (temp.ItemData != null)
                {
                    if (temp.gameObject.activeSelf)
                    {
                        if (temp.ItemData.Original == 30)
                        {
                            MainUICtrl.Instance.OpenGuidePanel(temp.transform, () =>
                            {
                                temp.OnClickBtn();
                            }, 8);
                            break;
                        }
                    }
                }


            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            if (tewen.Count > 0)
            {
                foreach (var item in tewen)
                {
                    item.canvasGroup.DOKill();
                    item.rect.DOKill();
                    item.canvasGroup.alpha = 1;
                }
                tewen.Clear();
            }
            scrollRect.enabled = true;
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}
