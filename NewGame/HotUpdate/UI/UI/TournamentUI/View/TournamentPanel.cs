using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace HotUpdate
{
    public partial class TournamentPanel : PanelBase
    {
        private string prefabName = "TournamentItem";
        private bool isLastTournament = false;
        private bool isInit = false;
        ScrollRect scrollRect;
        [SerializeField] private List<TourmarmentProess> tourmarmentProesses = new List<TourmarmentProess>();
        ScrollRect scroll;
   
        private int tourType = 4;

        List<Transform> tempList = new List<Transform>();
        private List<Toggle> toggleList = new List<Toggle>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            scrollRect = m_VGridScroll_TournamentList.GetComponent<ScrollRect>();
            TournamentModel.Instance.tourLevel =4;
            scroll = m_VGridScroll_TournamentList.GetComponent<ScrollRect>();
            m_VGridScroll_TournamentList.InitGridView(0, OnGetItemByRowColumn);
            toggleList.Add(m_Tog_Options1);
            toggleList.Add(m_Tog_Options2);
            toggleList.Add(m_Tog_Options3);
            toggleList.Add(m_Tog_Options4);
            toggleList.Add(m_Tog_Options5);
            toggleList.Add(m_Tog_Options6);
            toggleList.Add(m_Tog_Options7);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
       
            isLastTournament = false;
            SetUpProess();
            m_Txt_Num.text = "0/0";
            m_Txt_Num.text = "未上榜";
            m_Txt_MyHeart.text = "0";
            TournamentModel.Instance.tournamentData.Clear();
            TournamentModel.Instance.lastTournamentData.Clear();
            TournamentModel.Instance.tourLevel = 4;
            if (m_Tog_Options1.isOn != true)
            {
                StartCoroutine(changeOptions());

            }
            else
            {
                sendTournamentData(1);
                m_Tog_Options1.isOn = true;
            }

            RankModel.Instance.lastRankItemDic.Clear();
            SetLastPanel();


            //TournamentCtrl.Instance.SendLastTournamentnfo();

        }

        private IEnumerator changeOptions()
        {
            yield return new WaitForSecondsRealtime(0.02f);
            for (int i = 0; i < toggleList.Count; i++)
            {
                if(i == 0)
                {
                    toggleList[i].SetIsOnWithoutNotify(true);
                    toggleList[i].interactable = false;
                    toggleList[i].isOn = true;
                }
                else
                {
                    toggleList[i].SetIsOnWithoutNotify(false);

                    toggleList[i].interactable = true;
                }
            }
            sendTournamentData(1);
            

        }

        public void sendTournamentData(int levelid)
        {
            setTog1Color();
            if (isLastTournament)
            {
                if (!TournamentModel.Instance.lastTournamentData.ContainsKey(levelid + tourType*10))
                {
                    TournamentCtrl.Instance.SendLastTournamentnfo(levelid, tourType);
                }
                else
                {
                    TournamentModel.Instance.tourLevel = levelid;
                    UpdataLastRankList();
                }
            }
            else
            {
                if (!TournamentModel.Instance.tournamentData.ContainsKey(levelid + this.tourType * 10))
                {
                    TournamentCtrl.Instance.SendTournamentInfo(levelid, this.tourType);
                }
                else
                {
                    TournamentModel.Instance.tourLevel = levelid;
                    UpdataRankList();
                }
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            UnRegisterListener();
            SetUpAnimat(m_Img_ProessBg1.transform, m_Trans_Effect1.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg2.transform, m_Trans_Effect2.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg3.transform, m_Trans_Effect3.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg4.transform, m_Trans_Effect4.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg5.transform, m_Trans_Effect5.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg6.transform, m_Trans_Effect6.gameObject, 0f);
            SetUpAnimat(m_Img_ProessBg7.transform, m_Trans_Effect7.gameObject, 0f);
            TournamentModel.Instance.tournamentData.Clear();
            TournamentModel.Instance.lastTournamentData.Clear();

        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
            if (m_Txt_CountTime != null)
                m_Txt_CountTime.text = TimeUtil.DateDiffByDay("", TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(TournamentModel.Instance.nextTime));
        }

        public void SetUpPanel() 
        {
            m_Txt_MyHeart.text = $"{TournamentModel.Instance.MyScores}";

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Tog_Options1.onValueChanged.AddListener(Toggle1Changed);
            m_Tog_Options2.onValueChanged.AddListener(Toggle2Changed);
            m_Tog_Options3.onValueChanged.AddListener(Toggle3Changed);
            m_Tog_Options4.onValueChanged.AddListener(Toggle4Changed);
            m_Tog_Options5.onValueChanged.AddListener(Toggle5Changed);
            m_Tog_Options6.onValueChanged.AddListener(Toggle6Changed);
            m_Tog_Options7.onValueChanged.AddListener(Toggle7Changed);
            m_Tog_Select.onValueChanged.AddListener(ToggleSelecChanged);
            m_Btn_rule.onClick.AddListener(OnRuleBtn);
 
            Message.AddListener<int>(MessageName.RELOAD_TOURNAMENTCTRL_UI, ReloadUI);
            Message.AddListener<int>(MessageName.RELOAD_TOURNAMENTCTRLLAST_UI, ReloadLastUI);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Tog_Options1.onValueChanged.RemoveListener(Toggle1Changed);
            m_Tog_Options2.onValueChanged.RemoveListener(Toggle2Changed);
            m_Tog_Options3.onValueChanged.RemoveListener(Toggle3Changed);
            m_Tog_Options4.onValueChanged.RemoveListener(Toggle4Changed);
            m_Tog_Options5.onValueChanged.RemoveListener(Toggle5Changed);
            m_Tog_Options6.onValueChanged.RemoveListener(Toggle6Changed);
            m_Tog_Options7.onValueChanged.RemoveListener(Toggle7Changed);
            m_Tog_Select.onValueChanged.RemoveListener(ToggleSelecChanged);
            m_Btn_rule.onClick.RemoveListener(OnRuleBtn);
     
            Message.RemoveListener<int>(MessageName.RELOAD_TOURNAMENTCTRL_UI, ReloadUI);
            Message.RemoveListener<int>(MessageName.RELOAD_TOURNAMENTCTRLLAST_UI, ReloadLastUI);
            //TournamentModel.Instance.tournamentMidData.Clear();
            //TournamentModel.Instance.tournamentData.Clear();
        }
        #endregion

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            var data = isLastTournament ? TournamentModel.Instance.lastTournamentData : TournamentModel.Instance.tournamentData;
            SCommArenaData itemData = null;
            if (itemIndex < data[TournamentModel.Instance.tourLevel + tourType*10].Count)
            {
                itemData = data[TournamentModel.Instance.tourLevel + tourType * 10][itemIndex];
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<TournamentItem>();
            script.SetUpItem(itemData);
            return item;
        }

        private void UpdataRankList()
        {
            
            var config = TournamentModel.Instance.FindGameArenaConfig(tourType,TournamentModel.Instance.tourLevel);
            m_Txt_Num.text = $"{TournamentModel.Instance.tournamentData[TournamentModel.Instance.tourLevel + tourType * 10].Count}/{config.Count}";
            m_VGridScroll_TournamentList.SetListItemCount(config.Count);
            var tournament = TournamentModel.Instance.tournamentData[TournamentModel.Instance.tourLevel + tourType * 10].Find(x => x.n64Charguid==MainUIModel.Instance.palyerData.m_i8roleID);
            m_Txt_MyRank.text = tournament!=null? $"已上榜" : "未上榜";
            m_VGridScroll_TournamentList.RefreshAllShownItem();
            m_VGridScroll_TournamentList.MovePanelToItemByIndex(0);
           
            PlayAni();
        }

        private void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("TournamentPanel");
        }

        public void ReloadUI(int level)
        {
            TournamentModel.Instance.tourLevel = level;
            UpdataRankList();
            scroll.verticalNormalizedPosition = 1;
            //CreatBtn();
            SetUpPanel();
            //UpdataRankList();
            SetUpAnimat(m_Img_ProessBg1.transform,m_Trans_Effect1.gameObject,1f);
            SetUpAnimat(m_Img_ProessBg2.transform, m_Trans_Effect2.gameObject, 1f);
            SetUpAnimat(m_Img_ProessBg3.transform, m_Trans_Effect3.gameObject, 1f);
            SetUpAnimat(m_Img_ProessBg4.transform, m_Trans_Effect4.gameObject, 1f);
            SetUpAnimat(m_Img_ProessBg5.transform, m_Trans_Effect5.gameObject, 1f);
            SetUpAnimat(m_Img_ProessBg6.transform, m_Trans_Effect6.gameObject, 1f);
            SetUpAnimat(m_Img_ProessBg7.transform, m_Trans_Effect7.gameObject, 1f);

        }

        private void setTog1Color() 
        {
            m_Txt_PageTxt1.color = !m_Tog_Options1.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt1.color = !m_Tog_Options1.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt2.color = !m_Tog_Options2.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt2.color = !m_Tog_Options2.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt3.color = !m_Tog_Options3.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt3.color = !m_Tog_Options3.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt4.color = !m_Tog_Options4.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt4.color = !m_Tog_Options4.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt5.color = !m_Tog_Options5.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt5.color = !m_Tog_Options5.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt6.color = !m_Tog_Options6.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt6.color = !m_Tog_Options6.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageTxt7.color = !m_Tog_Options7.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
            m_Txt_PageRewardTxt7.color = !m_Tog_Options7.isOn ? new Color32(195, 101, 9, 255) : new Color32(48, 63, 106, 255);
        }

        public void Toggle1Changed(bool isOn) 
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
             
                sendTournamentData(1);
                ShowProessAnimat(m_Img_ProessBg1.transform);
                m_Tog_Options1.interactable = false;
                Debug.LogError("tttttttttttt");
            }
            else
            {
                m_Tog_Options1.interactable = true;
                Debug.LogError("ffffffffffffff");
            }
        }

        public void Toggle2Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
         
                sendTournamentData(2);
                ShowProessAnimat(m_Img_ProessBg2.transform);
                m_Tog_Options2.interactable = false;
            }
            else
            {
       
                m_Tog_Options2.interactable = true;
            }
        }
        public void Toggle3Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);

                sendTournamentData(3);
                ShowProessAnimat(m_Img_ProessBg3.transform);
                m_Tog_Options3.interactable = false;
            }
            else
            {
  
                m_Tog_Options3.interactable = true;
            }
        }
        public void Toggle4Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
          
                sendTournamentData(4);
                ShowProessAnimat(m_Img_ProessBg4.transform);
                m_Tog_Options4.interactable = false;
            }
            else
            {
     
                m_Tog_Options4.interactable = true;
            }
        }
        public void Toggle5Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);

                sendTournamentData(5);
                ShowProessAnimat(m_Img_ProessBg5.transform);
                m_Tog_Options5.interactable = false;
            }
            else
            {
      
                m_Tog_Options5.interactable = true;
            }
        }
        public void Toggle6Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
      
                sendTournamentData(6);
                ShowProessAnimat(m_Img_ProessBg6.transform);
                m_Tog_Options6.interactable = false;
            }
            else
            {
   
                m_Tog_Options6.interactable = true;
            }
        }
        public void Toggle7Changed(bool isOn)
        {
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
               
                sendTournamentData(7);
                ShowProessAnimat(m_Img_ProessBg7.transform);
                m_Tog_Options7.interactable = false;
            }
            else
            {
           
                m_Tog_Options7.interactable = true;
            }
        }

        public void ToggleSelecChanged(bool isOn)
        {
            OnLastBtn();
        }

        public void SetUpAnimat(Transform target,GameObject targetEffect,float targetNum) 
        {
            Tweener tween = null;
            tween = target.GetComponent<Image>().DOFillAmount(targetNum, 0.5f).OnComplete(
                delegate 
                {
                    targetEffect.SetActive(targetNum==1);
                });
        }

        public void ShowProessAnimat(Transform target) 
        {
            Tweener tween = null;
            tween = target.transform.DOShakePosition(1f,2,7,90);
        }

        public void SetUpProess() 
        {
            var config = ConfigCtrl.Instance.Tables.TbGameArena.DataList;
            for (int i = 0; i < config.Count; i++)
            {
                if(config[i].Type == 4)
                {
                    tourmarmentProesses[config[i].Levelid - 1].SetUpItem(config[i]);
                }
            }        
        }

        public void OnLastBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            isLastTournament = !isLastTournament;

            SetLastPanel();


            sendTournamentData(TournamentModel.Instance.tourLevel);
        }

        public void SetLastPanel()
        {
            if (isLastTournament)
            {
                m_Tog_Select.transform.GetChild(1).gameObject.SetActive(true);
                m_Tog_Select.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                m_Tog_Select.transform.GetChild(1).gameObject.SetActive(false);
                m_Tog_Select.transform.GetChild(2).gameObject.SetActive(true);
            }
        }

        private void UpdataLastRankList()
        {

            var config = TournamentModel.Instance.FindGameArenaConfig(tourType, TournamentModel.Instance.tourLevel);
            m_Txt_Num.text = $"{TournamentModel.Instance.lastTournamentData[TournamentModel.Instance.tourLevel + tourType * 10].Count}/{config.Count}";
            m_VGridScroll_TournamentList.SetListItemCount(config.Count);
            var tournament = TournamentModel.Instance.lastTournamentData[TournamentModel.Instance.tourLevel + tourType * 10].Find(x => x.n64Charguid == MainUIModel.Instance.palyerData.m_i8roleID);
            m_Txt_MyRank.text = tournament != null ? $"已上榜" : "未上榜";
            m_VGridScroll_TournamentList.RefreshAllShownItem();

            PlayAni();
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

            scrollRect.gameObject.SetActive(false);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            scrollRect.gameObject.SetActive(true);
            scrollRect.enabled = false;
            scrollRect.verticalNormalizedPosition = 1;
            Transform content = m_VGridScroll_TournamentList.transform.GetChild(0).GetChild(0);
            float aniTimes = 0.3f;
            tempList.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            for (int i = 0; i < m_VGridScroll_TournamentList.ItemTotalCount; i++)
            {
                var item = m_VGridScroll_TournamentList.GetShownItemByItemIndex(i);
                if (item != null)
                {
                    if (i <30)
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
            m_VGridScroll_TournamentList.gameObject.SetActive(true);
            //bFirst = false;
            CoreEntry.gTimeMgr.RemoveTimer(250);
            CoreEntry.gTimeMgr.AddTimer(tempList.Count * 0.15f, false, () => {
                if (scrollRect != null)
                    scrollRect.enabled = true;
                CoreEntry.gTimeMgr.RemoveTimer(250);
            }, 250);
        }
        private void ReloadLastUI(int level) 
        {

            TournamentModel.Instance.tourLevel = level;
            UpdataLastRankList();


        }

        public void OnRuleBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            CommonPanel panel = MainPanelMgr.Instance.ShowDialog("CommonPanel") as CommonPanel;
            panel.SetContent("规则说明", "1.所有游戏房间的伤害值均参与排行统计\n2.活动期间，每日0点12点进行奖励结算\n3.每个档位上榜都有名额限制，先到先得\n4.根据达成的伤害目标的时间进行排名，奖励层层叠加，越早完成，奖励越高\n5.奖励通过邮件发送");

        }

        public void OnRuleColseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("CommonPanel");

        }



    }
}

