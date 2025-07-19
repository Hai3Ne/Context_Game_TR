using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class RankPanel : PanelBase
    {
        private string prefabName = "RankItem";
        private string preffabName1 = "RankItemBig";
       [SerializeField] private GameObject targetObject;
        private bool isLastRank;
        ScrollRect scrollRect;
        private bool bFirst = true;
        List<Transform> tempList = new List<Transform>();
        List<Vector2> posList = new List<Vector2>();
        public List<GameObject> rankBtnItems = new List<GameObject>();

        List<Transform> Selects = new List<Transform>();
        List<Transform> UnSelects = new List<Transform>();

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);


            Selects.Add(m_Tog_Page1.transform.GetChild(2));
            Selects.Add(m_Tog_Page2.transform.GetChild(2));
            Selects.Add(m_Tog_Page3.transform.GetChild(2));

            UnSelects.Add(m_Tog_Page1.transform.GetChild(1));
            UnSelects.Add(m_Tog_Page2.transform.GetChild(1));
            UnSelects.Add(m_Tog_Page3.transform.GetChild(1));

            scrollRect = m_VGridScroll_RankList.GetComponent<ScrollRect>();
            m_VGridScroll_RankList.InitGridView(0, OnGetItemByRowColumn);
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
            RankModel.Instance.rankItemDic.Clear();
            RankModel.Instance.lastRankItemDic.Clear();
            m_VGridScroll_RankList.gameObject.SetActive(false);
            RankModel.Instance.rankType = 1;
            SelectIndexPos();
            //m_Txt_lastBtnTxt.text = "上期榜单";
            m_Btn_LastRank.transform.Find("lastRank").gameObject.SetActive(true);
            m_Btn_LastRank.transform.Find("nowRank").gameObject.SetActive(false);
            //m_Btn_LastRank.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:btn_previous_list");
            StartCoroutine(ToolUtil.GetHeadImage(Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_szHeadUrl).Replace("\0", null), m_Img_head));
            var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            if (!Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName).Equals(""))
            {

                m_Txt_MyName.text = Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName);
            }
            else
            {
                m_Txt_MyName.text = $"U{uid.Substring(uid.Length - 4, 4)}";
            }
        }


        private async void SelectIndexPos()
        {
           
            m_VGridScroll_RankList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            isLastRank = false;
            sendRankData(1);
            m_Tog_Page1.SetIsOnWithoutNotify(true);
            for (int i = 0; i < Selects.Count; i++)
            {
                Selects[i].gameObject.SetActive(i == 0);
            }

            for (int i = 0; i < UnSelects.Count; i++)
            {
                UnSelects[i].gameObject.SetActive(i != 0);
            }


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
            m_VGridScroll_RankList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            scrollRect.enabled = false;
            scrollRect.verticalNormalizedPosition = 1;
            Transform content = m_VGridScroll_RankList.transform.GetChild(0).GetChild(0);
            float aniTimes = 0.4f;
            tempList.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            for (int i = 0; i < m_VGridScroll_RankList.ItemTotalCount; i++)
            {
                var item = m_VGridScroll_RankList.GetShownItemByItemIndex(i);
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
                //if (posList.Count != 0&& !bFirst)
                //{
                //    rect.anchoredPosition = new Vector2(posList[i].x, posList[i].y-100);
                //}
                //else
                //{
                //    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 100);
                //}
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 100);
                CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                var pos1 = new Vector2(rect.anchoredPosition.x, posY);
                //posList.Add(pos1);
                var tween1 = rect.DOAnchorPos(pos1, aniTimes).SetDelay(0.15f * i); ;
                canvasGroup.DOFade(1, 0.8f).OnComplete(() =>
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
            m_VGridScroll_RankList.gameObject.SetActive(true);
            bFirst = false;
            CoreEntry.gTimeMgr.RemoveTimer(250);
            CoreEntry.gTimeMgr.AddTimer(tempList.Count * 0.15f, false, () => {
                if (scrollRect != null)
                    scrollRect.enabled = true;
                CoreEntry.gTimeMgr.RemoveTimer(250);
            },250);
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
            RankModel.Instance.rankItemDic.Clear();
            RankModel.Instance.lastRankItemDic.Clear();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_LastRank.onClick.AddListener(OnLastBtn);
            Message.AddListener<int>(MessageName.RELOAD_RANK_UI, ReloadRankUI);
            Message.AddListener<int>(MessageName.RELOAD_LASTRANK_UI, ReloadLastRankUI);
            m_Tog_Page1.onValueChanged.AddListener(Toggle1Changed);
            m_Tog_Page2.onValueChanged.AddListener(Toggle2Changed);
            m_Tog_Page3.onValueChanged.AddListener(Toggle3Changed);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_LastRank.onClick.RemoveListener(OnLastBtn);
            Message.RemoveListener<int>(MessageName.RELOAD_RANK_UI, ReloadRankUI);
            Message.RemoveListener<int>(MessageName.RELOAD_LASTRANK_UI, ReloadLastRankUI);
            RankModel.Instance.rankItemDic.Clear();
            RankModel.Instance.lastRankItemDic.Clear();
            m_Tog_Page1.onValueChanged.RemoveListener(Toggle1Changed);
            m_Tog_Page2.onValueChanged.RemoveListener(Toggle2Changed);
            m_Tog_Page3.onValueChanged.RemoveListener(Toggle3Changed);
        }
        #endregion


        private void Toggle1Changed(bool isOn)
        {
            Selects[0].gameObject.SetActive(isOn);
            UnSelects[0].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }

                sendRankData(1);
            }
  
        }

        private void Toggle2Changed(bool isOn)
        {
            Selects[1].gameObject.SetActive(isOn);
            UnSelects[1].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }
                sendRankData(2);

            }
  
        }

        private void Toggle3Changed(bool isOn)
        {
            Selects[2].gameObject.SetActive(isOn);
            UnSelects[2].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }
                sendRankData(3);
            }
  
        }


        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            var data = isLastRank ? RankModel.Instance.lastRankItemDic : RankModel.Instance.rankItemDic;
            if (data == null) return null;
            if (itemIndex < 0 /*|| itemIndex >= data[RankModel.Instance.rankType].Count*/)
            {
                return null;
            }
            //Debug.LogError($"itemIndex:{itemIndex}  rankitemCountDic:${RankModel.Instance.RankItemCount}  RankModel.Instance.rankType:${RankModel.Instance.rankType}");
            LoopGridViewItem item ;
            /*if (itemIndex == 0||itemIndex==1||itemIndex==2)
            {

                if (itemIndex == 1 || itemIndex == 2)
                {
                    
                    return null;
                }
                item = loopView.NewListViewItem(preffabName1);
                var script = item.GetComponent<RankItemBig>();
                script.SetUpItem(isLastRank);
            }
            else
            {*/
                item = loopView.NewListViewItem(prefabName);
                var script = item.GetComponent<RankItemChild>();
                if (data[RankModel.Instance.rankType].Count == 0) 
                {
                    script.SetUpItem(null, RankModel.Instance.rankType, itemIndex + 1);
                }
                else
                {
                    Debug.LogError($"itemIndex:{itemIndex}");
                    script.SetUpItem(data[RankModel.Instance.rankType].Count > itemIndex ? data[RankModel.Instance.rankType][itemIndex] : null, RankModel.Instance.rankType, itemIndex + 1);
                    //script.SetUpItem(data[RankModel.Instance.rankType][itemIndex], RankModel.Instance.rankType, itemIndex + 1);
                }
                
            //}

            return item;
        }

        private void UpdataRankList()
        {
            var myRank = RankModel.Instance.FindMyRankData(RankModel.Instance.rankType);
            m_Txt_MyRankTxt.text = myRank == 0 ? "未上榜" : $"{myRank}";
            m_Txt_MyConsume.text = myRank == 0 ? ToolUtil.AbbreviateNumberf02(RankModel.Instance.myScoreDict[RankModel.Instance.rankType]) : ToolUtil.AbbreviateNumberf02(RankModel.Instance.rankItemDic[RankModel.Instance.rankType][myRank -1].n64Total);// .ToString();
            m_VGridScroll_RankList.MovePanelToItemByIndex(0);
            m_VGridScroll_RankList.SetListItemCount(RankModel.Instance.RankItemCount);
            m_VGridScroll_RankList.RefreshAllShownItem();


            PlayAni();
        }

        private void UpdataLastRankList()
        {
            var myRank = RankModel.Instance.FindLastMyRankData(RankModel.Instance.rankType);
            m_Txt_MyRankTxt.text = myRank == 0 ? "未上榜" : $"{myRank}";

            m_Txt_MyConsume.text = myRank == 0 ? "0":ToolUtil.AbbreviateNumber(RankModel.Instance.lastRankItemDic[RankModel.Instance.rankType][myRank - 1].n64Total);// .ToString();
            m_VGridScroll_RankList.MovePanelToItemByIndex(0);
            m_VGridScroll_RankList.SetListItemCount(RankModel.Instance.RankItemCount);
            m_VGridScroll_RankList.RefreshAllShownItem();
            PlayAni();
        }

        private void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("RankPanel");
        }

     

        public void sendRankData(int rankType)
        {
            if (isLastRank)
            {
                if (!RankModel.Instance.lastRankItemDic.ContainsKey(rankType))
                {
                    RankCtrl.Instance.SendLastRankInfo(rankType);
                }
                else
                {
                    RankModel.Instance.rankType = rankType;
                    UpdataLastRankList();
                }
            }
            else
            {
                if (!RankModel.Instance.rankItemDic.ContainsKey(rankType))
                {
          
                    RankCtrl.Instance.SendRankInfo(rankType);
                }
                else
                {
                    RankModel.Instance.rankType = rankType;
                    UpdataRankList();
                }
            }

        }
        public void ReloadLastRankUI(int type)
        {
            for (int i = 0; i < rankBtnItems.Count; i++)
            {
                var btn = rankBtnItems[i];
                var cmp = btn.GetComponent<RankBtnItem>();
                if (cmp != null)
                {
                    cmp.setFreeState();
                }
            }
            UpdataLastRankList();
        }

        public void ReloadRankUI(int type) 
        {
            for (int i = 0; i < rankBtnItems.Count; i++)
            {
                var btn = rankBtnItems[i];
                var cmp = btn.GetComponent<RankBtnItem>();
                if(cmp != null)
                {
                    cmp.setFreeState();
                }
            }
            UpdataRankList();
        }

        public void OnLastBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            isLastRank = !isLastRank;
            if (isLastRank)
            {
                //m_Txt_lastBtnTxt.text = "回到当前";
                //m_Btn_LastRank.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:btn_back_to_current");
                sendRankData(RankModel.Instance.rankType);
              
        
            }
            else
            {
                //m_Txt_lastBtnTxt.text = "上期榜单";
                //m_Btn_LastRank.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite($"Rank:btn_previous_list");
                sendRankData(RankModel.Instance.rankType);
            }
            m_Btn_LastRank.transform.Find("lastRank").gameObject.SetActive(!isLastRank);
            m_Btn_LastRank.transform.Find("nowRank").gameObject.SetActive(isLastRank);
        }


    }
}
