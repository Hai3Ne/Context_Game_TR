using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using uGUI;
using SEZSJ;

namespace HotUpdate
{
    public partial class Top1500Panel : PanelBase
    {
        private UIRoom1500 SlotCpt;
        public int betID = 0;//--挡位

        protected float timeBeginSpin = 0;
        protected slotState m_curSlotState;
        protected bool m_bIsCanClick = true;
        public List<int> BetList = new List<int>();
        public int Bet = 0;// -- 挡位
        public int BetID = 0;
        protected int betMax = 0;
        public int betMin = 0;

        protected long gold = 0;

        public long winGold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;

        //public GoldEffectNew m_Gold_EffectNew;
        public Game1500_BigWin m_Gold_EffectNew;

        public Trans_AutoPanel1500 autoPanel;

        public int freeRemainTimes = 5;
        public GameTips1500 gameTips;
        public List<Transform> elementList = new List<Transform>();
        public List<Transform> elementBgList = new List<Transform>();
        public List<Transform> linesList = new List<Transform>();
        public List<Text> linesTextList = new List<Text>();
        public List<Transform> lineTranList = new List<Transform>();
        /// <summary>
        /// 游戏提示类型
        /// </summary>
        private int gameTipIndex = 0;

        FrameAnimation1500 beginsSpinEffect;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "img_Head_7");
            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/Prefabs/Game1500/FirstRes/Game1500_BigWin");
            m_Gold_EffectNew = go1.GetComponent<Game1500_BigWin>();
            m_Gold_EffectNew.gameObject.SetActive(false);
            Transform ElementEffect = transform.Find("Trans_ElementEffect");
            for (int i = 0; i < ElementEffect.childCount; i++)
                elementList.Add(ElementEffect.Find("Cell"+i));
            Transform ElementEffectBg = transform.Find("Trans_ElementEffectBg");
            for (int i = 0; i < ElementEffectBg.childCount; i++)
                elementBgList.Add(ElementEffectBg.Find("Cell" + i));
            Transform Lines = transform.Find("Trans_Lines");
            for (int i = 0; i < Lines.childCount; i++)
                linesList.Add(Lines.Find("line" + i));
            Transform LinesText = transform.Find("LinesText");
            for (int i = 0; i < LinesText.childCount; i++)
            {
                linesTextList.Add(LinesText.transform.Find("Trans_TextGold" + i+"/TxtLineGold" + i).GetComponent<Text>());
            }
    
            lineTranList.Add(m_Trans_TextGold0);
            lineTranList.Add(m_Trans_TextGold1);
            lineTranList.Add(m_Trans_TextGold2);
            lineTranList.Add(m_Trans_TextGold3);
            lineTranList.Add(m_Trans_TextGold4);
            lineTranList.Add(m_Trans_TextGold5);
            lineTranList.Add(m_Trans_TextGold6);
            lineTranList.Add(m_Trans_TextGold7);
            lineTranList.Add(m_Trans_TextGold8);
            lineTranList.Add(m_Trans_TextGold9);

            GameObject auto = CommonTools.AddSubChild(transform.gameObject, "UI/Prefabs/Game1500/FirstRes/Trans_AutoPanel");
            autoPanel = auto.GetComponent<Trans_AutoPanel1500>();
            autoPanel.gameObject.SetActive(false);
            beginsSpinEffect = m_Trans_BeginSpinEffect.GetComponent<FrameAnimation1500>();
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1500;
            //m_Img_Head.transform.parent.gameObject.SetActive(!SlotCpt.bDanJi);
            //m_Btn_Leave.transform.gameObject.SetActive(!SlotCpt.bDanJi);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet1500, BetGameRet1500);// 1500房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1500, RefreshJACKPOT1500);

            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);

            RegisterListener();

            SetRollBtnRorate(false);
            gameTipIndex = 1;
            PlayGameTipsLoop();
            ShowAllLines(false);
            ShowAllLineText(false);
            if (gameTips != null)
                gameTips.gameObject.SetActive(false);
            if (m_Trans_MenuePanel.gameObject.activeSelf)
                m_Trans_MenuePanel.gameObject.SetActive(false);
        }

        public void RegisterListener()
        {
            m_Btn_Leave.onClick.AddListener(OnClickBtnLeave);
            m_Btn_AddGold.onClick.AddListener(ClickGoldAdd);
            //m_Btn_Rank.onClick.AddListener(ClickBtnRank);
            m_Btn_Help.onClick.AddListener(ClickBtnHelp);
            m_Btn_Min.onClick.AddListener(OnClickBtnMin);
            m_Btn_Add.onClick.AddListener(OnClickBtnAdd);
            m_Btn_Auto.onClick.AddListener(ClickAuto);
            m_Btn_BeginSpin.onClick.AddListener(onPointerUp);

            m_Tog_Turbo.onValueChanged.AddListener((isOn)=> { ClickTrbro(isOn); });
            m_Btn_CloseAutoSpin.onClick.AddListener(ClickBtnCloseAutoSpin);

            m_Btn_Menue.onClick.AddListener(ClickBtnMenue);
            m_Btn_CloseMenue.onClick.AddListener(ClickBtnCloseMenue);
            m_Btn_PayTable.onClick.AddListener(ClickBtnPayTable);
            m_Btn_Rule.onClick.AddListener(ClickBtnRule);
            m_Btn_Rank.onClick.AddListener(OnRankBtn);
            m_Btn_Tour.onClick.AddListener(OnTourBtn);
            m_Btn_TourRank.onClick.AddListener(OnTourRankBtn);
        }

        public void UnRegisterListener()
        {
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Leave.onClick.RemoveListener(OnClickBtnLeave);
            m_Btn_AddGold.onClick.RemoveListener(ClickGoldAdd);
           // m_Btn_Rank.onClick.RemoveListener(ClickBtnRank);
            m_Btn_Help.onClick.RemoveListener(ClickBtnHelp);
            m_Btn_Min.onClick.RemoveListener(OnClickBtnMin);
            m_Btn_Add.onClick.RemoveListener(OnClickBtnAdd);
            m_Btn_Auto.onClick.RemoveListener(ClickAuto);
            m_Btn_BeginSpin.onClick.RemoveListener(onPointerUp);

            m_Tog_Turbo.onValueChanged.RemoveListener((isOn) => { ClickTrbro(isOn); });
            m_Btn_CloseAutoSpin.onClick.RemoveListener(ClickBtnCloseAutoSpin);

            m_Btn_Menue.onClick.RemoveListener(ClickBtnMenue);
            m_Btn_CloseMenue.onClick.RemoveListener(ClickBtnCloseMenue);
            m_Btn_PayTable.onClick.RemoveListener(ClickBtnPayTable);
            m_Btn_Rule.onClick.RemoveListener(ClickBtnRule);
            m_Btn_Rank.onClick.RemoveListener(OnRankBtn);
            m_Btn_Tour.onClick.RemoveListener(OnTourBtn);
            m_Btn_TourRank.onClick.RemoveListener(OnTourRankBtn);
        }

        private void OnRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenRankPanel();
        }

        private void OnTourBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTournamentPanel();
        }

        public void OnTourRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTourRankPanel();
        }

        public void InitData()
        {
            m_Txt_Score.text =   "" + "0";
            autoPanel.SetScore(0);
            Game1500Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            m_Txt_Gold.text =  ToolUtil.AbbreviateNumber(Game1500Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold);
            InitGame();
        }

        public void OpenGameTips(int ele,int pos)
        {
            if(gameTips == null)
            {
                gameTips =CommonTools.AddSubChild(m_Trans_GameTips.gameObject, "UI/Prefabs/Game1500/FirstRes/GameTips1500").GetComponent<GameTips1500>();
                //gameTips = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/GameTips1500").GetComponent<GameTips1500>();

                //gameTips.transform.SetParent(m_Trans_GameTips);
                gameTips.transform.localScale = new Vector3(1, 1, 1);
                gameTips.transform.localPosition = new Vector3(0, 0, 0);
                gameTips.gameObject.SetActive(false);
            }
            //if (gameTips.gameObject.activeSelf)
            //    return;
            gameTips.OpenTips(ele,pos);
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game1500Model.Instance.arrayAward.Count; i++)
            {
                if (Game1500Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1500Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList,1500);
        }



        public void ClickBtnHelp()
        {
            //MainPanelMgr.Instance.ShowDialog("UIRoom1500Help");
        }

        public void ClickBtnCloseEffect()
        {
            long tempWinGold = winGold;
            if (Game1500Model.Instance.ucAllSame == 1)
            {
                tempWinGold = tempWinGold * 10;
                m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold);
                PlayAni2(m_Txt_WinGold.transform);
                CoreEntry.gAudioMgr.PlayUISound(258);
            }
         
            m_Txt_Score.text = "" + ToolUtil.ShowF2Num(tempWinGold);
  
            autoPanel.SetScore(tempWinGold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            m_Txt_Gold.text =  ToolUtil.AbbreviateNumber(Game1500Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold);

            SlotCpt.ContinueGame();
           // CoreEntry.gTimeMgr.AddTimer(1f, false, () => {SlotCpt.continueSpin();}, 100000);

            if(Game1500Model.Instance.bFreeGameFinished())
            {
                Game1500Model.Instance.gameStates = 0;
            }
       
        }

        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin && Game1500Model.Instance.toSpin.n64FreeGold > 0)
                return;
            m_Txt_Score.text = "" + ToolUtil.ShowF2Num(score);// (score/15000f).ToString();
            autoPanel.SetScore(score);
        }

        public async void InitGame()
        {
            BetList = Game1500Model.Instance.GearList;// --.Gear1;
            betMax = BetList[BetList.Count - 1];
            betMin = BetList[0];
            Bet = BetList[0];
            if (Game1500Model.Instance.nBet > 0)
                Bet = Game1500Model.Instance.nBet;
            else
                betID = 0;
            OnBetChange(Bet);
            m_Btn_Min.interactable = Bet != betMin;
            m_Btn_Add.interactable = Bet != betMax;
            SlotCpt.setState(slotState.Idle);
            if (Game1500Model.Instance.nBet > 0)
            {
                SlotCpt.SetRollBg(1);
                StartCoroutine(SlotCpt.DoGoSlotScale(1));
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
                //CoreEntry.gAudioMgr.PlayUIMusic(205);
                SlotCpt.continueSpin();
            }
            SetTopRank();
        }


        protected void SetTopRank()
        {
           if(Game1500Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1500Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                m_TxtM_ID.text = CommonTools.BytesToString(Game1500Model.Instance.arrayAward[0].szName);
                m_TxtM_Money.text = ToolUtil.GetCurrencySymbol()+" " + ToolUtil.ShowF2Num(Game1500Model.Instance.arrayAward[0].n64Gold);// (Game1500Model.Instance.arrayAward[0].n64Gold / 15000).ToString("f2");

                string imgurl = "img_Head_" + Game1500Model.Instance.arrayAward[0].nIconID;
                m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
            }
        }

 


        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1500, BetGameRet1500);// -- 1500房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1500, RefreshJACKPOT1500);// -- 1500房间下注结果返回
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            UnRegisterListener();
        }


        public void BeginSlot(int num)
        {
            SlotCpt.beginSpin(num, false);
           // showAutoSpin(false);
        }



        public void OnClickBtnLeave()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            CoreEntry.gTimeMgr.Reset();

            UICtrl.Instance.CloseAllView();


            UICtrl.Instance.OpenView("MainUIPanel");
            /*        if (MainUIModel.Instance.RoomData.nGameType == 12)
                    {
                        UICtrl.Instance.OpenView("MainUIPanel");
                    }
                    else
                    {
                        UICtrl.Instance.OpenView("RoomPanel");
                    }*/
            MainUIModel.Instance.RoomData = null;
            Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 1);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_exit, null);
            if (!SlotCpt.bDanJi)
            {
                CoreEntry.gTimeMgr.Reset();
                // MainPanelMgr.Instance.ReturnPrePanel();
                NetLogicGame.Instance.Send_CS_HUMAN_LEAVE_GAME_REQ();
            }
        }

        public bool GetTogTurboIsOn()
        {
            return m_Tog_Turbo.isOn;
        }

        public void OnClickBtnMin()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            if (SlotCpt.isFreeSpin)
                return;
            int betId = nextBet(false);
            OnBetChange(betId);
            PlayAni(m_Txt_SingleGold.transform);
            m_Btn_Add.interactable = true;
            if (Bet == betMin)
                m_Btn_Min.interactable = false;
        }

        public void OnClickBtnAdd()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            if (SlotCpt.isFreeSpin)
                return;
            int betId = nextBet(true);
            OnBetChange(betId);
            PlayAni(m_Txt_SingleGold.transform);
            m_Btn_Min.interactable = true;
            if (Bet == betMax)
                m_Btn_Add.interactable = false;
        }

        private void PlayAni(Transform trans)
        {
            m_Txt_SingleGold.transform.DOKill();
            Sequence seq = DOTween.Sequence();
            
            Tweener t1 = trans.DOScale(new Vector3(1.25f, 1.25f, 1), 0.25f);
            Tweener t2 = trans.DOScale(Vector3.one, 0.25f);
            seq.Append(t1);
            seq.Append(t2);
            seq.SetAutoKill();
            seq.Play();
        }

        private void PlayAni2(Transform trans)
        {
            m_Txt_SingleGold.transform.DOKill();
            Sequence seq = DOTween.Sequence();

            Tweener t1 = trans.DOScale(new Vector3(1.45f, 1.45f, 1), 0.25f);
            Tweener t2 = trans.DOScale(Vector3.one*1.3f, 0.25f);
            seq.Append(t1);
            seq.Append(t2);
            seq.SetAutoKill();
            seq.Play();
        }

        public void OnClickBtnMax()
        {
            if (SlotCpt.isFreeSpin)
                return;
            OnBetChange(betMax);
            m_Btn_Min.interactable = true;
            if (Bet == betMax)
                m_Btn_Add.interactable = false;
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gTimeMgr.RemoveTimer(250);
            slotState state = SlotCpt.StateSlot;
            m_curSlotState = state;
            
            if(state == slotState.Idle)
            {
                m_Txt_Gold.color = new Color32(93,167,184,255);// string.Format("<color=#5DA7B8>{0}</color>",);
                m_Txt_SingleGold.color = new Color32(93, 167, 184, 255);
                m_Txt_Score.color = new Color32(93, 167, 184, 255);
                //m_TxtM_Score.co
            }
            else
            {
                m_Txt_Gold.color = new Color32(255, 255, 255, 255);// string.Format("<color=#5DA7B8>{0}</color>",);
                m_Txt_SingleGold.color = new Color32(255, 255, 255, 255); 
                m_Txt_Score.color = new Color32(255, 255, 255, 255);
            }

            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    SetBtnBySlotState(false);
                    return;
                }
                m_Btn_BeginSpin.gameObject.SetActive(true);
                SetBtnBeginSpinInteractable(true);
                SetBtnBySlotState(true);
                return;
            }
            else if (state == slotState.SpinBegin)
            {
                m_Btn_BeginSpin.gameObject.SetActive(true);
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);   
                return;
            }
            else if (state == slotState.SpinSuccess)
            {
                CoreEntry.gTimeMgr.RemoveTimer(10);
                //-- 等待停止
                CoreEntry.gTimeMgr.AddTimer(0.4f, false, () =>
                {
                    if (m_curSlotState == slotState.SpinSuccess)
                    {
                        SetBtnBySlotState(false);
                        CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                        {
                            if (m_curSlotState == slotState.SpinSuccess)
                            {
                                m_Btn_BeginSpin.gameObject.SetActive(true);
                                SetBtnBeginSpinInteractable(false);
                                SetBtnBySlotState(false);
                            }
                        }, 12);

                    }
                }, 10);

            }
            else if (state == slotState.SpinStop || state == slotState.SpinEnd)
            {
                CoreEntry.gTimeMgr.RemoveTimer(12);
                m_Btn_BeginSpin.gameObject.SetActive(true);
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);
            }
            else { }
        }

        public async void OnSlotWinGold()
        {
            StartCoroutine(ShowGameTips(3));
            winGold = Game1500Model.Instance.toSpin.WinGold;
            if (Game1500Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game1500Model.Instance.toSpin.n64FreeGold;
            long tempWinGold = winGold;
            if (Game1500Model.Instance.ucAllSame == 1)
                tempWinGold = winGold ;
            m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold);
            RectTransform rect = m_Txt_WinGold.GetComponent<RectTransform>();
            if ((Game1500Model.Instance.toSpin.rate < 5 && Game1500Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                if (Game1500Model.Instance.ucAllSame == 1)
                    m_Txt_Score.text = "" + ToolUtil.ShowF2Num(tempWinGold);
                else
                {
                    m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold);
                    if (rect.sizeDelta.x > 200)
                        rect.anchoredPosition = new Vector2(-(rect.sizeDelta.x - 200) / 2, -2.1f);
                    else
                        rect.anchoredPosition = new Vector2(0, -2.1f);

                    m_Txt_Score.text = "" + ToolUtil.ShowF2Num(tempWinGold);
                    autoPanel.SetScore(winGold);
                    autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold);
                }
            }
            else if(Game1500Model.Instance.toSpin.rate > 20 && !Game1500Model.Instance.bInFreeGame())
                m_Gold_EffectNew.setData(tempWinGold, () => { ClickBtnCloseEffect(); }, SlotCpt.autoSpinNum != 0);
            else
            {

                m_Txt_WinGold.text = "0";
                if (Game1500Model.Instance.bHasElement(7))
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.2f));
                m_Txt_WinGold.DOKill();

                ToolUtil.RollText2(0, tempWinGold, m_Txt_WinGold, 1f, () =>
                {
                    PlayAni2(m_Txt_WinGold.transform);
                    CoreEntry.gAudioMgr.StopSound(SlotCpt.GetSoundObj());
                    if (Game1500Model.Instance.ucAllSame == 0)
                        ClickBtnCloseEffect();
                });
                string temp = ToolUtil.ShowF2Num(tempWinGold);
                if (temp.Length > 6)
                    rect.anchoredPosition = new Vector2(-(temp.Length - 6) * 36 / 2 - 10, -2.1f);
                else
                    rect.anchoredPosition = new Vector2(0, -2.1f);
            }
        }

        private async void SetWinGoldBg(int rate,bool bDelay = false)
        {
            if (bDelay)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            if (rate < 0)
            {
                m_Trans_RedGoldBg.gameObject.SetActive(true);
                m_Trans_GreenGoldBg.gameObject.SetActive(false);
                m_Trans_PinkGoldBg.gameObject.SetActive(false);
                //if (rate >= 0)
                //    CoreEntry.gAudioMgr.PlayUISound(217, SlotCpt.GetSoundObj1());
            }
            else if(rate < 5)
            {
                m_Trans_RedGoldBg.gameObject.SetActive(false);
                m_Trans_GreenGoldBg.gameObject.SetActive(true);
                m_Trans_PinkGoldBg.gameObject.SetActive(false);
                //CoreEntry.gAudioMgr.PlayUISound(218, SlotCpt.GetSoundObj1());
            }
            else
            {
                m_Trans_RedGoldBg.gameObject.SetActive(false);
                m_Trans_GreenGoldBg.gameObject.SetActive(false);
                m_Trans_PinkGoldBg.gameObject.SetActive(true);
               // CoreEntry.gAudioMgr.PlayUISound(219, SlotCpt.GetSoundObj1());
            }
        }

        public void BigWinAni(long SpecialNum = 0, Action callBack = null)
        {
            m_Trans_Effect.gameObject.SetActive(true);
            gold = 0;
            if (SpecialNum > 0)
                gold = SpecialNum;
            else
                gold = Game1500Model.Instance.toSpin.WinGold;
            if (Game1500Model.Instance.ucAllSame == 1)
                gold = gold * 10;


            playGoldType = 0;
            if (Game1500Model.Instance.toSpin.n64RSPowerGold > 0)
            {
                gold = Game1500Model.Instance.toSpin.n64RSPowerGold;
                playGoldType = 4;
            }
            else
            {
                if (Game1500Model.Instance.toSpin.rate > 2 && Game1500Model.Instance.toSpin.rate <= 4)
                    playGoldType = 1;
                else if (Game1500Model.Instance.toSpin.rate > 4 && Game1500Model.Instance.toSpin.rate <= 12)
                    playGoldType = 2;
                else
                    playGoldType = 3;
            }
            m_Gold_EffectNew.setData(gold, () => { callBack?.Invoke(); }, SlotCpt.autoSpinNum != 0);
        } 

        public void OnBetChange(int bet)
        {
            Bet = bet;
            for(int i = 0;i < BetList.Count;i++ )
            {
                if (BetList[i] == bet)
                {
                    int temps22 = BetList[i];
                    Game1500Model.Instance.nBet1 = temps22;
                    betID = i;
                }
            }
            m_Txt_SingleGold.text = "" + ToolUtil.ShowF2Num2(bet*10);
            autoPanel.SetSingleGoldText(bet);
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
        }

        public void SetFreeTimes()
        {
            //m_Trans_GoFreeTimes.gameObject.SetActive(true);
            //m_Txt_Times.text = Game1500Model.Instance.toSpin.FreeTimes + "/" + Game1500Model.Instance.toSpin.nModelGame;
        }

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gAudioMgr.PlayUISound(71);
        }

        public void BetGameRet1500()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void RefreshJACKPOT1500()
        {
            SetTopRank();
        }

        public void ReloadGame()
        {
            m_Txt_Gold.text =  ToolUtil.AbbreviateNumber(MainUIModel.Instance.palyerData.m_i8Golds);
            autoPanel.SetGoldText(MainUIModel.Instance.palyerData.m_i8Golds);
            if (SlotCpt.gameStatus == 1)
            {
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
            }
        }

        public void UpdateGold(long gold)
        {
            Game1500Model.Instance.toSpin.n64Gold = gold;
            m_Txt_Gold.text =  ToolUtil.AbbreviateNumber(gold);
            autoPanel.SetGoldText(gold);
            MainUIModel.Instance.Golds = gold;
        }
 
        public void PlayGameTipsLoop()
        {
            CoreEntry.gTimeMgr.RemoveTimer(312);
            CoreEntry.gTimeMgr.AddTimer(7, true, () =>
            {
                SetWinGoldBg(-1);
                PlayGameTips(gameTipIndex);
                if (gameTipIndex == 2)
                {
                    m_Trans_Multiplier.localPosition = Vector3.zero;
                    m_Trans_Multiplier.DOLocalMoveX(-1000, 5.5f).SetDelay(1f).SetEase(Ease.Linear).SetAutoKill();
                }
                else if (gameTipIndex == 0)
                {
                    m_Trans_Respins.localPosition = Vector3.zero;
                    m_Trans_Respins.DOLocalMoveX(-1100, 5.5f).SetDelay(1f).SetEase(Ease.Linear).SetAutoKill();
                }
                gameTipIndex++;
                if (gameTipIndex >= 3)
                    gameTipIndex = 0;
            }, 312);
        }

        List<int> awardSound = new List<int> { 244,254,261};
        int sounIndex = 0;


        List<int> awardSoundBig = new List<int> { 251, 252, 263 };
        int sounIndexBig = 0;
        public IEnumerator ShowGameTips(int type, bool bSpecial = false, Action callBack = null)
        {
            if (Game1500Model.Instance.bHasElement(7))
                yield return new WaitForSeconds(1.2f);
            if (Game1500Model.Instance.toSpin.WinGold > 0)
            {
                if (Game1500Model.Instance.toSpin.rate < 5)
                    CoreEntry.gAudioMgr.PlayUISound(258, gameObject);
                else
                    CoreEntry.gAudioMgr.PlayUISound(260, gameObject);
            }
            yield return new WaitForSeconds(bSpecial == false ? 0.35f : 2.5f);
            CoreEntry.gTimeMgr.RemoveTimer(312);
            PlayAni(m_Trans_Win.transform);

            m_Dragon_GetGoldEffect.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_GetGoldEffect.transform,"Sprite",1,() => m_Dragon_GetGoldEffect.gameObject.SetActive(false));
            PlayGameTips(type);
            if(Game1500Model.Instance.toSpin.rate <5 && !Game1500Model.Instance.bInFreeGame())
            {
                CoreEntry.gAudioMgr.PlayUISound(awardSound[sounIndex],transform.GetChild(3).gameObject);
                sounIndex++;
                if (sounIndex >= 3)
                    sounIndex = 0;
                SlotCpt.PlayTigerAni(2);
            }
            else if(Game1500Model.Instance.toSpin.rate < 20 &&!Game1500Model.Instance.bInFreeGame())
            {
                CoreEntry.gAudioMgr.PlayUISound(awardSoundBig[sounIndexBig], transform.GetChild(3).gameObject);
                sounIndexBig++;
                if (sounIndexBig >= 3)
                    sounIndexBig = 0;
                SlotCpt.PlayTigerAni(10);
            }

            SetWinGoldBg((int)Game1500Model.Instance.toSpin.rate);
            m_Dragon_GetGoldEffectBg.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_GetGoldEffectBg.transform,"Sprite",1,()=> m_Dragon_GetGoldEffectBg.gameObject.SetActive(false));
     
            callBack?.Invoke();
        }

        public void ContiunePlayGameTipsLoop()
        {
            CoreEntry.gTimeMgr.RemoveTimer(312);
            SetWinGoldBg(-1);
            PlayGameTips(gameTipIndex);
            if (gameTipIndex == 2)
            {
                m_Trans_Multiplier.localPosition = Vector3.zero;
                m_Trans_Multiplier.DOLocalMoveX(-1100, 5.5f).SetDelay(1f).SetEase(Ease.Linear).SetAutoKill();
            }
            else if(gameTipIndex == 0)
            {
                m_Trans_Respins.localPosition = Vector3.zero;
                m_Trans_Respins.DOLocalMoveX(-1000, 5.5f).SetDelay(1f).SetEase(Ease.Linear).SetAutoKill();
            }
            gameTipIndex++;
            if (gameTipIndex >= 3)
                gameTipIndex = 0;
            PlayGameTipsLoop();
        }

        public void PlayGameTips(int type)
        {
            m_Trans_Respins.gameObject.SetActive(type == 0);
            m_Trans_WinUpTo2500.gameObject.SetActive(type == 1);
            m_Trans_Multiplier.gameObject.SetActive(type == 2);
            m_Trans_Win.gameObject.SetActive(type == 3);
            m_Trans_FortuneTigerFeature.gameObject.SetActive(type == 4);
            m_Trans_Wild.gameObject.SetActive(type == 5);
        }

        public Transform GetTrans_Win()
        {
            return m_Trans_Win;
        }

        public void OnSlotSpinNum(int num)
        {
            if (num <= 0)
            {
                m_Trans_GoAutoSpinNum.gameObject.SetActive(false);
            }
            else
            {
                m_Trans_GoAutoSpinNum.gameObject.SetActive(true);
                m_Txt_AutoSpinNum.text = num + "";
            }
        }

        public Text GetTextGold()
        {
            return m_Txt_Gold;
        }

        public Transform GetTransWin()
        {
            return m_Trans_Win;
        }

        public Transform GetTrans_Mask()
        {
            return m_Trans_Mask;
        }

        public Transform GetTrans_SpecialMask()
        {
            return m_Trans_SpecialMask;
        }


        public Transform GetTrans_LineNum()
        {
            return m_Trans_LineNum;
        }


        public Transform GetTransElementEffect()
        {
            return m_Trans_ElementEffect;
        }

        public Transform GetTransElementBgEffect()
        {
            return m_Trans_ElementEffectBg;
        }


        public virtual void ClickGoldAdd()
        {
            //CoreEntry.gAudioMgr.PlayUISound(198);
            //if (!SlotCpt.bDanJi)
            //    MainUICtrl.Instance.OpenShopPanel();
        }

        public Transform GetTrans_Lines()
        {
            return m_Trans_Lines;
        }

        public virtual void onPointDown()
        {
            if (m_Btn_BeginSpin.interactable == false)
                return;
            timeBeginSpin = UnityEngine.Time.realtimeSinceStartup;
        }

        public virtual void onPointerUp()
        {
            if (m_Btn_BeginSpin.interactable == false)
                return;
            if(slotState.Idle == SlotCpt.StateSlot)
            {
                m_Dragon_BeginSpin.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_BeginSpin.transform,"Sprite",1,()=> m_Dragon_BeginSpin.gameObject.SetActive(false));

                beginsSpinEffect.gameObject.SetActive(true);
                beginsSpinEffect.play(true,()=> { beginsSpinEffect.gameObject.SetActive(false); });
                CoreEntry.gAudioMgr.PlayUISound(248, SlotCpt.GetSoundObj1());
                CoreEntry.gTimeMgr.RemoveTimer(10);
                BeginSlot(0);
                timeBeginSpin = 0;
            }
        }


        public void SetRollBtnRorate(bool bFast = false,bool bPause = false)
        {
            m_Trans_Rorate.transform.DOKill();
            if (!bPause)
            {
                if(bFast)
                    m_Trans_Rorate.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                else
                    m_Trans_Rorate.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 2f, RotateMode.LocalAxisAdd).SetDelay(0.55f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            }  
            else
                m_Trans_Rorate.transform.localEulerAngles = Vector3.zero;  
            m_Trans_Normal.gameObject.SetActive(!bPause);
            m_Trans_Grey.gameObject.SetActive(bPause);
        }

        private void ClickAuto()
        {
            CoreEntry.gAudioMgr.PlayUISound(262);
            autoPanel.OpenPanel((num)=> 
            {
                SlotCpt.autoSpinNum = num;
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(150000);
                    BeginSlot(num);
                }
            });
        }

        private void ClickBtnCloseAutoSpin()
        {
            if (Game1500Model.Instance.bInFreeGame())
                return;
            CoreEntry.gAudioMgr.PlayUISound(262);
            m_Trans_GoAutoSpinNum.gameObject.SetActive(false);
            CoreEntry.gAudioMgr.PlayUISound(1);
            SlotCpt.autoSpinNum = 0;
        }

        private void ClickBtnMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(262);
            m_Trans_MenuePanel.gameObject.SetActive(true);
        }
        private void ClickBtnCloseMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(262);
            m_Trans_MenuePanel.gameObject.SetActive(false);
        }

        private void ClickBtnPayTable()
        {
            CoreEntry.gAudioMgr.PlayUISound(262);
            MainPanelMgr.Instance.ShowDialog("UIRoom1500Help");
        }

        private void ClickBtnRule()
        {
            CoreEntry.gAudioMgr.PlayUISound(262);
            MainPanelMgr.Instance.ShowDialog("UIRoom1500Help", true, 2);
        }

        private void ClickTrbro(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(262);

            if (SlotCpt.StateSlot == slotState.Idle)
                m_Tog_Turbo.transform.GetChild(0).gameObject.SetActive(!isOn);
        }

        public virtual void SetBtnBeginSpinInteractable(bool bValue)
        {
            m_Btn_BeginSpin.interactable = bValue;
            m_Btn_BeginSpin.gameObject.transform.Find("Light").gameObject.SetActive(bValue);
            m_Btn_BeginSpin.gameObject.transform.Find("Dark").gameObject.SetActive(false == bValue);
        }

        public void SetBtnBySlotState(bool isenabled)
        {
            m_bIsCanClick = isenabled;
            m_Btn_Leave.interactable = isenabled;
            m_Btn_Add.interactable = isenabled;
            m_Btn_Min.interactable = isenabled;
            m_Btn_Auto.interactable = isenabled;
            m_Btn_Menue.interactable = isenabled;
            if (Bet == betMax)
                m_Btn_Add.interactable = false;
            if (Bet == betMin)
                m_Btn_Min.interactable = false;
        }

        public int nextBet(bool plus)
        {
            int _bet = Bet;
            int max = betMax;
            for (int i = 0; i < BetList.Count; i++)
            {
                if (BetList[i] != Bet)
                    continue;
                if (plus)
                {
                    if (Bet >= max || i == BetList.Count - 1)
                        _bet = betMin;
                    else
                        _bet = BetList[i + 1];
                }
                else
                {
                    if (Bet <= betMin || i == 0)
                    {
                        if (max >= betMin)
                            _bet = max;
                        else
                            _bet = betMin;
                    }
                    else
                    {
                        _bet = BetList[i - 1];
                    }
                }
                break;
            }
            return _bet;
        }

        public void SetAniParents()
        {
            for (int i = 0; i < SlotCpt.slotColumns.Count; i++)
            {
                for (int j = 0; j < SlotCpt.slotColumns[i].lstCells.Count - 1; j++)
                {
                    UISlotCell cell = SlotCpt.slotColumns[i].lstCells[j];
                    if (cell.TfSpine.childCount > 0)
                    {
                        Transform parent = elementList[i * 3 + 2 - j];
                        cell.TfSpine.GetChild(0).SetParent(parent);
                    }
                }
            }
        }


        public void SpecialGameEffect()
        {
            for(int i = 0;i < Game1500Model.Instance.slotResult.Count;i++)
            {
                if(Game1500Model.Instance.slotResult[i] != 0)
                {
                    int col = 0;
                    int row = i % 3;
                    if (i >= 3 && i < 7)
                    {
                        col = 1;
                        row = (i - 3) % 4;
                    }                     
                    else if (i >= 7 && i < 10)
                    {
                        col = 2;
                        row = (i - 7) % 3;
                    }   

                    UIRoom1500SlotCell cell = SlotCpt.slotColumns[col].lstCells[row] as UIRoom1500SlotCell;
                    if (elementList[i].childCount <= 0)
                    {
                        GameObject elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/SpecialElement");
                        elementGo.transform.SetParent(elementList[i], true);
                        elementGo.gameObject.SetActive(true);
                        elementGo.transform.localScale = new Vector3(1, 1, 1);
                        elementGo.transform.localPosition = new Vector3(0, 0, 0);
                        if(Game1500Model.Instance.slotResult[i] <=4)
                            elementGo.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1500_2:" + "Tb" + Game1500Model.Instance.slotResult[i]);
                        else
                            elementGo.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1500_3:" + "Tb" + Game1500Model.Instance.slotResult[i]);
                        elementGo.transform.DOScale(new Vector3(1.15f, 1.15f, 1.2f), 0.35f);
                        cell.ImgElement.gameObject.SetActive(false);
                    }
             
                }
            }

            List<int> allLines = Game1500Model.Instance.GetAllLines();
            if (allLines.Count > 0)
            {
                for (int i = 0; i < allLines.Count; i++)
                    ShowOneLine(allLines[i], true);
            }
        }


        public void ShowOneLine(int index, bool bShow)
        {
            linesList[index - 1].gameObject.SetActive(bShow);
            linesList[index - 1].gameObject.SetActive(bShow);
        }

        public void ShowAllLines(bool bShow = false)
        {
            for (int i = 0; i < linesList.Count; i++)
            {
                lineTranList[i].gameObject.SetActive(bShow);
                linesList[i].gameObject.SetActive(bShow);
            }   
        }     

        public void ShowAllLineText(bool bShow = false)
        {
            for (int i = 0; i < linesTextList.Count; i++)
            {
                lineTranList[i].gameObject.SetActive(bShow);
                linesTextList[i].gameObject.SetActive(bShow);
            }      
        }

        public void SetLineText(int index,long value)
        {
            lineTranList[index - 1].gameObject.SetActive(true);
            linesTextList[index - 1].gameObject.SetActive(true);
            linesTextList[index - 1].text = ToolUtil.ShowF2Num(value);
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

        public async void PlayBigAwardEffect()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f));
            CanvasGroup canvasGroup = m_Trans_BigAwardEffect.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.65f;
            DOTween.To(() => 0.65f, (value) =>
            {
                canvasGroup.alpha = value;
            }, 1, 1.5f).OnComplete(() =>
            {
                canvasGroup.alpha = 1;
            }).SetEase(Ease.Linear);
            m_Trans_BigAwardEffect.localScale = Vector3.one;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f));
            //OnSlotWinGold();
            SlotCpt.PlayTexSpecialRateAni2(() =>
            {
                BigWinAni(0, () =>
                {
         ;
          
                    SlotCpt.ResetScale();
                    m_Trans_BigAwardEffect.GetChild(0).gameObject.SetActive(false);
                    SlotCpt.GetTransRodar().gameObject.SetActive(false);
                    SlotCpt.GetSpecialRate().gameObject.SetActive(true);
                    SlotCpt.GetSpecialRate().GetComponent<RectTransform>().anchoredPosition = new Vector3(-2, 25, 0);
                    Game1500Model.Instance.gameStates = 0;
                    ClickBtnCloseEffect();
                });
            });          
        }


       public void MoveGoldBg(bool bAppear,float times)
        {
            if(bAppear)
            {
                m_Trans_Left.DOAnchorPos(new Vector2(126, 65), times);
                m_Trans_Right.DOAnchorPos(new Vector2(-116, 53), times);
            }
            else
            {
                m_Trans_Left.DOAnchorPos(new Vector2(37, -54), times);
                m_Trans_Right.DOAnchorPos(new Vector2(-38, -54), times);
            }
        }
    }
}
