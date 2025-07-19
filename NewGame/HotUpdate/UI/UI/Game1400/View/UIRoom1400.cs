using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class UIRoom1400 : PanelBase
    {
        /// <summary>
        /// 下注区域按钮
        /// </summary>
        private List<Button> chipBtnList = new List<Button>();
        /// <summary>
        /// 下注区域
        /// </summary>
        private List<Transform> betAreaList = new List<Transform>();
        /// <summary>
        /// 下注数值
        /// </summary>
        private List<Text> betTextList = new List<Text>();
        /// <summary>
        /// 自己下注筹码值
        /// </summary>
        private List<Text> betMyTextList = new List<Text>();

        private List<Transform> elementList = new List<Transform>();
        private List<UIChip1400> chipList = new List<UIChip1400>();
        private List<Transform> trendList = new List<Transform>();
        private Transform currentTrend;

        private ResultPanel1400 resultPanel;
        private TrendPanel trendPanel;
        private OnlinePanel1400 onlinePanel;
        /// <summary>
        /// 自己的下注筹码
        /// </summary>
        private List<KeyValuePair<int, GameObject>> myBetChips = new List<KeyValuePair<int, GameObject>>();

        bool bCanBet = true;//是否可以下注

        private bool bPauseFinished = false;
        Sequence seq;

        Coroutine coroutine;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            InitData();
            var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            m_Txt_UID.text = $"玩家{uid.Substring(uid.Length - 4, 4)}";
            var config = ConfigCtrl.Instance.Tables.TbHead_Config.Get(MainUIModel.Instance.palyerData.m_i4icon);
            m_Img_Head.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + config.Icon);
        }

        public float focusOffTime = -1;
        public float focusOnTime = -1;
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                focusOffTime = Time.realtimeSinceStartup;
                OnFocus_Off();
                //CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Focus_Off, EventParameter.Get(focusOffTime - focusOnTime));
            }
            else
            {
                focusOnTime = Time.realtimeSinceStartup;
                if (focusOffTime > 0)
                {
                    OnFocus(focusOnTime - focusOffTime);
                   // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_Focus_On, EventParameter.Get(focusOnTime - focusOffTime));
                }
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Game1400Model.Instance.currentGold = MainUIModel.Instance.Golds;
            UpdateMyGold();
            Game1400Model.Instance.InitConfig();
            Init();
            RegisterListener();
            InitRoomInfo();
            CoreEntry.gAudioMgr.PlayUIMusic(181);
            bCanBet = true;
            Game1400Model.Instance.selectChipType = 0;
        }

        protected override void Update()
        {
            Game1400Model.Instance.syncTime();
        }

        private void InitData()
        {
            chipBtnList.Clear();
            chipBtnList.Add(m_Btn_1);
            chipBtnList.Add(m_Btn_2);
            chipBtnList.Add(m_Btn_3);
            chipBtnList.Add(m_Btn_4);
            chipBtnList.Add(m_Btn_5);
            chipBtnList.Add(m_Btn_6);
            chipBtnList.Add(m_Btn_7);
            chipBtnList.Add(m_Btn_8);

            betAreaList.Clear();
            betAreaList.Add(m_Trans_1);
            betAreaList.Add(m_Trans_2);
            betAreaList.Add(m_Trans_3);
            betAreaList.Add(m_Trans_4);
            betAreaList.Add(m_Trans_5);
            betAreaList.Add(m_Trans_6);
            betAreaList.Add(m_Trans_7);
            betAreaList.Add(m_Trans_8);

            betTextList.Clear();
            betTextList.Add(m_Txt_1);
            betTextList.Add(m_Txt_2);
            betTextList.Add(m_Txt_3);
            betTextList.Add(m_Txt_4);
            betTextList.Add(m_Txt_5);
            betTextList.Add(m_Txt_6);
            betTextList.Add(m_Txt_7);
            betTextList.Add(m_Txt_8);

            betMyTextList.Clear();
            betMyTextList.Add(m_Txt_1my);
            betMyTextList.Add(m_Txt_2my);
            betMyTextList.Add(m_Txt_3my);
            betMyTextList.Add(m_Txt_4my);
            betMyTextList.Add(m_Txt_5my);
            betMyTextList.Add(m_Txt_6my);
            betMyTextList.Add(m_Txt_7my);
            betMyTextList.Add(m_Txt_8my);
        }

        private void InitRoomInfo()
        {
            if (Game1400Model.Instance.arrayAreaBet == null || Game1400Model.Instance.arrayHistory == null)
                return;
            SetRemainTimes(Game1400Model.Instance.betTimes- (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer));
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                ResetRollAni();
            }
            ReSetBet();
            RecycleChips();
            InitBetAreaChips();
            InitTrendData();
            SetOnline();
        }

        /// <summary>
        /// 自己下注
        /// </summary>
        private void OnBetGameRet1400()
        {
            for(int i = 0;i < Game1400Model.Instance.chipValues.Count;i++)
            {
                //if(Game1400Model.Instance.chipValues[i] == Game1400Model.Instance.nBet_my)
                //{
                //    FlyChip(Game1400Model.Instance.ucArea_my, i, true,()=> 
                //    {
                //        betMyTextList[i].text = Game1400Model.Instance.n64AllBet_my + "";
                //        betTextList[i].text = Game1400Model.Instance.arrayAreaBet[i] + Game1400Model.Instance.nBet_my+"";
                //    });
                //    break;
                //}
            }

            UpdateMyGold();
        }

        /// <summary>
        ///广播下注
        /// </summary>
        private async void OnBroadCastBetGameRet1400()
        {
            if (bPauseFinished)
                return;
            List<SGame12RoomBetInfo>  tempBet =   Game1400Model.Instance.totalBroadCast[0];
            Game1400Model.Instance.totalBroadCast.RemoveAt(0);
            for (int k = 0; k < tempBet.Count; k++)
            {
                SGame12RoomBetInfo temp2 = tempBet[k];
                Game1400Model.Instance.ucArea = temp2.ucArea;
                Game1400Model.Instance.nBet = temp2.nBet;
                Game1400Model.Instance.n64AllBet = temp2.n64AllBet;
                Game1400Model.Instance.arrayAreaBet[temp2.ucArea] = Game1400Model.Instance.n64AllBet;                

                for (int i = 0; i < Game1400Model.Instance.chipValues.Count; i++)
                {
                    if (Game1400Model.Instance.chipValues[i] == Game1400Model.Instance.nBet)
                    {
                        int tempIndex = Game1400Model.Instance.ucArea;
                        long tempGold = Game1400Model.Instance.myBet[tempIndex];
                        long tempTotalGold = Game1400Model.Instance.n64AllBet;
                        int chipType = i;
                        FlyChip(tempIndex, chipType, true, () =>
                        {
                            string temp = tempGold > 0 ? ToolUtil.AbbreviateNumberF0(tempGold) + "/" : "0/";
                            betTextList[tempIndex].text = temp + ToolUtil.AbbreviateNumberF0(tempTotalGold);
                        });
                        break;
                    }
                }
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.2f / tempBet.Count));
            }          
        }

        private void OnBroadCastAddPlayer1400()
        {
            SetOnline();
        }

        private void OnBroadCastDelPlayer1400()
        {
            SetOnline();
        }

        private void Init()
        {
            elementList.Clear();
            for (int i = 1; i <= 24; i++)
            {
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/Tb1");
                go.transform.SetParent(m_Trans_ElementParent, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                string aniName = i.ToString();
                if (i < 10)
                    aniName = "0" + i;
                aniName += "a";
                ToolUtil.PlayAnimation(go.transform, aniName, true);
                elementList.Add(go.transform);
            }
            chipList.Clear();
            for (int i = 0; i < 6; i++)
            {
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/ChipCell");
                go.transform.SetParent(m_Trans_BetChips, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                UIChip1400 chip = go.GetComponent<UIChip1400>();
                chip.SetChipValue(i);
                chipList.Add(chip);
        
            }
            if(trendPanel != null && trendPanel.gameObject.activeSelf)
            {
                trendPanel.gameObject.SetActive(false);
            }
        }

        public void RegisterListener()
        {
            m_Btn_Leave.onClick.AddListener(ClickBtnLeave);
            m_Btn_Online.onClick.AddListener(ClickBtnOnline);
            m_Btn_Trend.onClick.AddListener(ClickBtnTrend);
            m_Btn_Repetir.onClick.AddListener(ClickBtnRepetir);
            m_Btn_AddGold.onClick.AddListener(ClickAddGoldBtn);

            for (int i = 0;i < chipBtnList.Count;i++)
            {
                int index = i;
                chipBtnList[index].onClick.AddListener(() => { ClickBetArea(index); });
            }

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GE_RoomInfo1400, InitRoomInfo);//房间信息
            Message.AddListener(MessageName.GE_BetGameRet1400, OnBetGameRet1400);//自己下注回复
            Message.AddListener(MessageName.GE_BroadCastBetGameRet1400, OnBroadCastBetGameRet1400);//广播下注
            Message.AddListener(MessageName.GE_BroadCastAddPlayer1400, OnBroadCastAddPlayer1400);//玩家进入
            Message.AddListener(MessageName.GE_BROADCAST_DEL_PLAYER1400, OnBroadCastDelPlayer1400);//玩家离开
            Message.AddListener(MessageName.GE_NoticeGameStart1400, NoticeGameStart1400);// 游戏开始通知
            Message.AddListener(MessageName.GE_NoticeBetEnd1400, NoticeBetEnd);//开奖通知
        }

      

        public void UnRegisterListener()
        {
            m_Btn_Leave.onClick.RemoveListener(ClickBtnLeave);
            m_Btn_Online.onClick.RemoveListener(ClickBtnOnline);
            m_Btn_Trend.onClick.RemoveListener(ClickBtnTrend);
            m_Btn_Repetir.onClick.RemoveListener(ClickBtnRepetir);
            m_Btn_AddGold.onClick.RemoveListener(ClickAddGoldBtn);
            for (int i = 0; i < chipBtnList.Count; i++)
            {
                int index = i;
                chipBtnList[index].onClick.RemoveListener(() => { ClickBetArea(index); });
            }
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.RemoveListener(MessageName.GE_RoomInfo1400, InitRoomInfo);//房间信息
            Message.RemoveListener(MessageName.GE_BetGameRet1400, OnBetGameRet1400);//自己下注回复
            Message.RemoveListener(MessageName.GE_BroadCastBetGameRet1400, OnBroadCastBetGameRet1400);//广播下注
            Message.RemoveListener(MessageName.GE_BroadCastAddPlayer1400, OnBroadCastAddPlayer1400);//玩家进入
            Message.RemoveListener(MessageName.GE_BROADCAST_DEL_PLAYER1400, OnBroadCastDelPlayer1400);//玩家离开
            Message.RemoveListener(MessageName.GE_NoticeGameStart1400, NoticeGameStart1400);// 游戏开始通知
            Message.RemoveListener(MessageName.GE_NoticeBetEnd1400, NoticeBetEnd);//开奖通知
        }

        private void ClickBtnLeave()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if(Game1400Model.Instance.CanLeave())
            {
                CoreEntry.gTimeMgr.Reset();
                // MainPanelMgr.Instance.ReturnPrePanel();
                NetLogicGame.Instance.Send_CS_HUMAN_LEAVE_GAME_REQ();
            }
            else
            {
                ToolUtil.FloattingText("正在游戏中", transform);
                return;
            }
     
        }
        private void ClickBtnOnline()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            OpenOnlinePanel();
        }
        private void ClickBtnTrend()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            OpenTrendPanel();
        }

        private void ClickBtnRepetir()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
        }

        private void ClickAddGoldBtn()
        {
            MainUICtrl.Instance.OpenShopPanel();
            CoreEntry.gAudioMgr.PlayUISound(1);
        }


        private async void ClickBetArea(int index)
        {
            if(!bCanBet)
                return;
            if (Game1400Model.Instance.selectChipType < 0)
            {
                ToolUtil.FloattingText("请选择投注金币", transform);
                return;
            }
            if (Game1400Model.Instance.chipValues[Game1400Model.Instance.selectChipType] > Game1400Model.Instance.currentGold)
            {
                bool bCanBet = false;
                if(Game1400Model.Instance.selectChipType > 0)
                {
                    for(int i = Game1400Model.Instance.selectChipType - 1; i >= 0 ; i-- )
                    {
                        if(Game1400Model.Instance.currentGold >=Game1400Model.Instance.chipValues[i] )
                        {
                            bCanBet = true;
                            Game1400Model.Instance.selectChipType = i;
                            Message.Broadcast(MessageName.GE_ClickChips1400, i);
                            break;
                        }
                    }
                }
                if(!bCanBet)
                {
                    ShowNotEnoughMoneyTips();
                    return;
                }                 
            }

            if (! Game1400Model.Instance.CanBet(Game1400Model.Instance.chipValues[Game1400Model.Instance.selectChipType], index))
            {

                ToolUtil.FloattingText("下注量已达到极限", transform);
                return;
            }
          
            if (Game1400Model.Instance.ucModelType == 2)
                return;
       
            bCanBet = false;
            Game1400Ctrl.Instance.ReqSendGAME12_BET_REQ(Game1400Model.Instance.chipValues[Game1400Model.Instance.selectChipType], index);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.3f));
            bCanBet = true;
        }
        public void ShowNotEnoughMoneyTips()
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
            //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool isFirst1);
            //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool isFirst2);
            var isBuyedFirst = isState;// && isFirst1 && isFirst2;
            if (isBuyedFirst)
            {
                SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
                tips.SetTipsPanel("温馨提示", "我感到抱歉！ 您的余额不足，请充值", "充值", delegate
                {
                    MainUICtrl.Instance.OpenShopPanel();
                });
            }
            else
            {
                MainUIModel.Instance.bOpenAlmsPanel = true;
                MainUICtrl.Instance.OpenFirstChargePanel();
            }
        }

        private void FlyChip(int betArea,int chipType,bool bSelf = false,Callback callBack = null)
        {
            CoreEntry.gAudioMgr.PlayUISound(179);
            int rand = UnityEngine.Random.Range(0, 3);
            int pos = rand * 8 + betArea;
            Vector3 elementPos = m_Trans_ElementPos.GetChild(pos).position;
            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/betChip");
            go.transform.position = elementPos;

            string aniName = pos + 1 < 10 ? "0" + (pos + 1) + "b" : (pos + 1) + "b";
            ToolUtil.PlayAnimation(elementList[pos], aniName, false);
            string flyChipName = string.Format("0{0}_2b", chipType + 1);
            ToolUtil.PlayAnimation(go.transform, flyChipName);
            Transform tempBetArea = betAreaList[betArea];
            Game1400Model.Instance.FlyChip(go.transform, tempBetArea, true,callBack);

            if(bSelf)
            {
                myBetChips.Add(new KeyValuePair<int, GameObject> (pos,go));
            }
        }

        public void SetBetAreaValue(int area,long value)
        {
            betTextList[area].text = value == 0?"": ToolUtil.AbbreviateNumberF0(value);
        }

        public void InitBetAreaChips()
        {
            for (int i = 0; i < Game1400Model.Instance.arrayAreaBet.Length; i++)
            {
                SetBetAreaValue(i, Game1400Model.Instance.arrayAreaBet[i]);
     
                if (Game1400Model.Instance.arrayAreaBet[i] >0)
                {
                    long tempGold = Game1400Model.Instance.arrayAreaBet[i];
                    for (int j = 5; j >= 0; j--)
                    {
                        int chipCount = (int)(tempGold / Game1400Model.Instance.chipValues[j]);
           
                        tempGold -= chipCount * Game1400Model.Instance.chipValues[j];
                        for (int k = 0;k < chipCount;k++)
                        {
                            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/betChip");
                            string flyChipName = string.Format("0{0}_2b", j + 1);
                            ToolUtil.PlayAnimation(go.transform, flyChipName);
                            Transform tempParent = betAreaList[i];
                            Game1400Model.Instance.FlyChip(go.transform, tempParent, false);
                        }              
                    }
                }             
            }
        }

        public void InitTrendData()
        {
            currentTrend = null;
            trendList.Clear();
            for(int i = m_Trans_TrendContent.childCount - 1; i >= 0;i--)
                CoreEntry.gGameObjPoolMgr.Destroy(m_Trans_TrendContent.GetChild(i).gameObject);
            for (int i = 0; i < Game1400Model.Instance.arrayHistory.Count; i++)
            {
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/TopTrendCell");
                go.transform.SetParent(m_Trans_TrendContent, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.GetComponent<Image>().enabled = false;
                trendList.Add(go.transform);
   
                go.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1400:" + "img_pcbmsjb_f" + (Game1400Model.Instance.arrayHistory[i]).ToString().PadLeft(2, '0')) ; 
                if (i == (Game1400Model.Instance.arrayHistory.Count - 1))
                    SetCurrentTrendCell(go.transform);
            }
        }
        public void AddOneTrenCell()
        {
            Transform cell;
            if(trendList.Count <20)
            {
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1400/FirstRes/TopTrendCell");
                go.transform.SetParent(m_Trans_TrendContent, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                trendList.Add(go.transform);     
                cell = go.transform;
            }
            else
            {
                cell = trendList[0];
                trendList.RemoveAt(0);
                trendList.Add(cell);
            }

            int element = Game1400Model.Instance.winPos % 8;
            if (element == 0)
                element = 8;
            cell.transform.GetChild(0).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1400:" + "img_pcbmsjb_f" + element.ToString().PadLeft(2, '0'));
            cell.SetAsLastSibling();
            SetCurrentTrendCell(cell);
        }
        private async void SetCurrentTrendCell(Transform trans)
        {
            if(currentTrend != null)
                currentTrend.GetComponent<Image>().enabled = false;
            trans.GetComponent<Image>().enabled = true;
            currentTrend = trans;
            await System.Threading.Tasks.Task.Delay(10);
             m_SRect_Trend.horizontalNormalizedPosition = 1;
        }

        public void OpenResultPanel()
        {
            if(resultPanel == null)
            {
                GameObject go = CommonTools.AddSubChild(gameObject,"UI/Prefabs/Game1400/FirstRes/ResultPanel1400");
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                resultPanel = go.GetComponent<ResultPanel1400>();
            }
            resultPanel.transform.SetAsLastSibling();
            resultPanel.Open();
        }
       
        public void OpenTrendPanel()
        {
            if (trendPanel == null)
            {
                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1400/FirstRes/TrendPanel");
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                trendPanel = go.GetComponent<TrendPanel>();
            }
            trendPanel.Open();            
        }

        public void OpenOnlinePanel()
        {
            if (onlinePanel == null)
            {
                GameObject go = CommonTools.AddSubChild(gameObject,"UI/Prefabs/Game1400/FirstRes/OnlinePanel1400");
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                onlinePanel = go.GetComponent<OnlinePanel1400>();
            }
            onlinePanel.Open();
        }


        private IEnumerator StartRoll(int startIndex = 1)
        {       
            Game1400Model.Instance.winPos = Game1400Model.Instance.ucShowArea%8;
            int targetIndex = Game1400Model.Instance.winPos;
            int elementIndex = 10;
            int totalCount = 3 * 24 + targetIndex+ (UnityEngine.Random.Range(0, 4)*8);
            for (int i = startIndex; i <= totalCount; i++)
            {
                elementIndex = i % 24;
                if (elementIndex == 0)
                    elementIndex = 24;
                string aniName = elementIndex.ToString().PadLeft(2, '0') + "b";
                string aniName2 = elementIndex.ToString().PadLeft(2, '0') + "a";
                string aniName3 = elementIndex.ToString().PadLeft(2, '0') + "c";
                int tempIndex = elementIndex - 1;
                int index = i;
                ToolUtil.PlayAnimation(elementList[tempIndex], aniName, false, () =>
                {
                    if (index == totalCount)
                    {
                        ToolUtil.PlayAnimation(elementList[tempIndex], aniName3, true);
                        StartEnd(elementList[tempIndex], aniName);
                    }
                    else
                        ToolUtil.PlayAnimation(elementList[tempIndex], aniName2, false);
                });


                if (i == totalCount)
                    CoreEntry.gAudioMgr.PlayUISound(185);
                else
                    CoreEntry.gAudioMgr.PlayUISound(184);

                if (totalCount - i <= 5)
                    yield return new WaitForSeconds(0.6f - 0.1f * (totalCount - i));
                else
                {
                    float times = 0.1f;
                    if (i <5)
                        times = 0.5f - 0.1f * i;
                    else
                    {
                        times = 0.1f - Game1400Model.Instance.AddSpeend * (i - 4);
                        if (times <= 0.001f)
                            times = 0.05f;
                    }
                    yield return new WaitForSeconds(times);
                }
            }
        }


        /// <summary>
        /// 到达第几格
        /// </summary>
        /// <param name="times"></param>
        public int GetIndexByTimes(float times = 2.8f)
        {        
            int totalCount = 3 * 24 + Game1400Model.Instance.winPos;
            int index = 0;
            if (times <= 0.5f)
                index = 1;
            else if (times <= 0.9f)
                index = 2;
            else if (times <= 1.2f)
                index = 3;
            else if (times <= 1.4f)
                index = 4;
            else if(times <= 1.5f)
                index = 5;
            else if(times <= 1.6f)
            {
                index = (int)((times - 1.5f) / Game1400Model.Instance.AddSpeend)+5;
            }
            else
            {
                int temp = (int)((times - 1.6f) / 0.05f);
                if(temp + 6 <= totalCount - 5)
                {
                    index = 11 + temp;
                }
                else
                {
                    float remainTimes = times - 1.6f - 0.05f * (totalCount - 5- 16);
                    if (remainTimes <= 0.1f)
                        index = totalCount - 5;
                    else if (remainTimes < 0.3f)
                        index = totalCount - 4;
                    else if (remainTimes <= 0.6f)
                        index = totalCount - 3;
                    else if (remainTimes <= 1)
                        index = totalCount - 2;
                    else if (remainTimes <= 1.5f)
                        index = totalCount - 1;
                    else
                        index = totalCount;
                }
            }
            return index;
        }

        private float totalTimes()
        {
            int totalCount = 3 * 24 + Game1400Model.Instance.winPos;
            float tempTimes = 3.1f;
            tempTimes += (totalCount - 21) * 0.05f;
            return tempTimes;
        }

        private void ResetRollAni()//经过了多少秒
        {
            for(int i = 1;i <= elementList.Count;i++)
            {
                int temp = i;
                string aniName2 = temp.ToString().PadLeft(2, '0') + "a";
                ToolUtil.PlayAnimation(elementList[i -1], aniName2, false);
            }
        }



        private async void StartEnd(Transform trans,string aniName)
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            if (MainUIModel.Instance.RoomData == null)
                return;
            if (MainUIModel.Instance.RoomData.nGameType != 12)
                return;
            ToolUtil.PlayAnimation(trans, aniName, false);
            AddOneTrenCell();
            RecycleChips();
            OpenResultPanel();
            UpdateMyGold();
            ReSetBet();
            SetRemainTimes(Game1400Model.Instance.betTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer));
        }


        private void NoticeGameStart1400()
        {
            Game1400Model.Instance.InitData();
            if (bPauseFinished)
                return;
            bCanBet = true;
      
            ReSetBet();
            RecycleChips();

            if (resultPanel != null && resultPanel.gameObject.activeSelf)
                resultPanel.gameObject.SetActive(false);
            m_Trans_RemainTimes.gameObject.SetActive(false);
            ToolUtil.PlayAnimation(m_Spine_Apostar.transform,"a1",false,()=> 
            {
                m_Spine_Apostar.gameObject.SetActive(false);
                SetRemainTimes(Game1400Model.Instance.betTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer));
            });
        }

        /// <summary>
        /// 下注结束
        /// </summary>
        private void NoticeBetEnd()
        {
            Game1400Model.Instance.InitData();
            if (bPauseFinished)
                return;
            CoreEntry.gAudioMgr.PlayUISound(186);
            m_Trans_RemainTimes.gameObject.SetActive(false);

            ToolUtil.PlayAnimation(m_Spine_Apostar.transform, "a2", false, () =>
            {
                m_Spine_Apostar.gameObject.SetActive(false);
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    ResetRollAni();
                }
                SetRemainTimes(Game1400Model.Instance.openAwardTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer),false);
                coroutine = StartCoroutine(StartRoll(1));
            });
        }

        private void UpdateMyGold()
        {
            UpdateGold(Game1400Model.Instance.currentGold);
        }

        private void SetRemainTimes(long times,bool bShow = true)
        {
            CancelInvoke("SetTimesInfo");
            m_Trans_RemainTimes.gameObject.SetActive(bShow);
            Game1400Model.Instance.remainTimes = times;
            SetTimesInfo();
            InvokeRepeating("SetTimesInfo", 1, 1);
        }

        private void SetTimesInfo()
        {
            if (Game1400Model.Instance.remainTimes <= 0)
            {
                m_Trans_RemainTimes.gameObject.SetActive(false);
                CancelInvoke("SetTimesInfo");
                return;
            }
            if (MainUIModel.Instance.RoomData == null)
                return;
            if (MainUIModel.Instance.RoomData.nGameType != 12)
                return;

            if (Game1400Model.Instance.remainTimes <= 3)
                CoreEntry.gAudioMgr.PlayUISound(180);
            m_TxtM_RemainTimes.text = Game1400Model.Instance.remainTimes.ToString();
            Game1400Model.Instance.remainTimes--;
            m_TxtM_RemainTimes.transform.localPosition = new Vector3(0, 34f, 0);
            m_TxtM_RemainTimes.transform.localScale = new Vector3(0.1f, 0.1f, 1);
            seq?.Kill();
            seq = null;
            seq = DOTween.Sequence();
            Tweener t1 = m_TxtM_RemainTimes.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.5f).SetEase(Ease.OutCubic);
            seq.Append(t1);
            Tweener t2 = m_TxtM_RemainTimes.transform.DOLocalMoveY(-12.6f, 0.5f).SetEase(Ease.OutCubic);
            seq.Join(t2);

            Tweener t3 = m_TxtM_RemainTimes.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.45f);
            seq.Append(t3);
            Tweener t4 = m_TxtM_RemainTimes.transform.DOLocalMoveY(-4f, 0.45f).SetEase(Ease.Linear);
            seq.Join(t4);
            seq.Play();
        }

        private void ReSetBet()
        {
            for(int i = 0;i < betTextList.Count;i++)
            {
                betTextList[i].text = "";
                betMyTextList[i].text = "";
            }
            for(int i = 0;i < Game1400Model.Instance.myBet.Length;i++)
            {
                Game1400Model.Instance.myBet[i] = 0;
            }
        }

        private void RecycleChips()
        {
            for(int i = 0;i < betAreaList.Count;i++)
            {
                for(int j = betAreaList[i].childCount -1; j >= 0;j--)
                    CoreEntry.gGameObjPoolMgr.Destroy(betAreaList[i].GetChild(j).gameObject);
            }
        }

        private int GetRemainTimes()
        {
            int times = int.Parse(m_TxtM_RemainTimes.text);
            return times;
        }

        public void UpdateGold(long gold)
        {
            Game1400Model.Instance.currentGold = gold;
            m_Txt_Gold.text = ToolUtil.AbbreviateNumber(gold);
            MainUIModel.Instance.Golds = gold;
        }

        private async void OnFocus(float time)
        {
            if(time < Game1400Model.Instance.remainTimesLeave)///状态未发生改变
            {
                SetRemainTimes((int)(Game1400Model.Instance.remainTimesLeave - time));
                if (Game1400Model.Instance.gameState == 1)//下注
                {             
                }
                else
                {               
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                        ResetRollAni();
                    }
                    m_Trans_RemainTimes.gameObject.SetActive(Game1400Model.Instance.openAwardTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer) > totalTimes());
                    coroutine = StartCoroutine(StartRoll(GetIndexByTimes(ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer)));          
                }
            }
            else
            {
                bPauseFinished = true;
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.35f));
                GameState();
            }
        }
        private void OnFocus_Off()
        {
            //Debug.LogError("=============" + Debug.LogError());

            Game1400Model.Instance.gameState = Game1400Model.Instance.ucModelType;
            Game1400Model.Instance.remainTimesLeave = (int)(Game1400Model.Instance.betTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer));
        }

        public void SetOnline()
        {
            m_TxtM_Online.text = Game1400Model.Instance.arrayPlayer.Count+"";
        }

        private void GameState()
        {
            Game1400Model.Instance.totalBroadCast.Clear();
            ResetRollAni();
            ReSetBet();
            RecycleChips();
            InitBetAreaChips();
            InitTrendData();
            UpdateMyGold();
            long tempTimes = Game1400Model.Instance.betTimes - (ToolUtil.getServerTime() - Game1400Model.Instance.timeSyncServer);
            SetRemainTimes((int)tempTimes);
            if (Game1400Model.Instance.ucModelType == 1)//  1下注模式    2开奖模式
            {              
            }
            else
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    ResetRollAni();
                }
                coroutine = StartCoroutine(StartRoll(GetIndexByTimes(tempTimes)));

                m_Trans_RemainTimes.gameObject.SetActive(tempTimes > totalTimes());
            }
            bPauseFinished = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            CoreEntry.gAudioMgr.StopMusic(181);
        }
    }

}


  