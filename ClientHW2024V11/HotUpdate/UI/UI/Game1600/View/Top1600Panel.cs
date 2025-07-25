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
    public partial class Top1600Panel : PanelBase
    {
        private UIRoom1600 SlotCpt;
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
        public Game1600_BigWin m_Gold_EffectNew;

        public Trans_AutoPanel1600 autoPanel;

        public int freeRemainTimes = 5;
        public GameTips1600 gameTips;

     
     
        public List<Text> linesTextList = new List<Text>();
        public List<Transform> lineTranList = new List<Transform>();
        /// <summary>
        /// 游戏提示类型
        /// </summary>
        private int gameTipIndex = 1;

        Vector3 Rate10Pos;


        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "img_Head_7");
            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/Prefabs/Game1600/FirstRes/Game1600_BigWin");
            m_Gold_EffectNew = go1.GetComponent<Game1600_BigWin>();
            m_Gold_EffectNew.gameObject.SetActive(false);

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
            Rate10Pos = m_Txt_Rate10.GetComponent<RectTransform>().anchoredPosition;

            GameObject auto = CommonTools.AddSubChild(transform.gameObject, "UI/Prefabs/Game1600/FirstRes/Trans_AutoPanel1600");
            autoPanel = auto.GetComponent<Trans_AutoPanel1600>();
            autoPanel.gameObject.SetActive(false);
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1600;
            m_Img_Head.transform.parent.gameObject.SetActive(!SlotCpt.bDanJi);
            m_Btn_Leave.transform.gameObject.SetActive(!SlotCpt.bDanJi);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet1600, BetGameRet1600);// 1600房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1600, RefreshJACKPOT1600);

            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);

            RegisterListener();

            SetRollBtnRorate(false);
            gameTipIndex = 0;
            PlayGameTipsLoop();
            ShowAllLines(false);
            ShowAllLineText(false);
        }

        public void RegisterListener()
        {
            m_Btn_Leave.onClick.AddListener(OnClickBtnLeave);
            m_Btn_AddGold.onClick.AddListener(ClickGoldAdd);
            m_Btn_Rank1.onClick.AddListener(ClickBtnRank);
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
            m_Btn_Rank1.onClick.RemoveListener(ClickBtnRank);
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

        public void InitData()
        {
            m_Txt_Score.text = ToolUtil.GetCurrencySymbol()+":" + "0";
            autoPanel.SetScore(0);
            Game1600Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            m_Txt_Gold.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(Game1600Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold);
            InitGame();
        }

        public void OpenGameTips(int ele,int pos)
        {
            if(gameTips == null)
            {
                gameTips =CommonTools.AddSubChild(m_Trans_GameTips.gameObject, "UI/Prefabs/Game1600/FirstRes/GameTips1600").GetComponent<GameTips1600>();
                gameTips.transform.localScale = new Vector3(1, 1, 1);
                gameTips.transform.localPosition = new Vector3(0, 0, 0);
                gameTips.gameObject.SetActive(false);
            }
            gameTips.OpenTips(ele,pos);
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game1600Model.Instance.arrayAward.Count; i++)
            {
                if (Game1600Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1600Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList,1600);
        }



        public void ClickBtnHelp()
        {
            //MainPanelMgr.Instance.ShowDialog("UIRoom1600Help");
        }

        public void ClickBtnCloseEffect()
        {
            long tempWinGold = winGold;
            if (Game1600Model.Instance.bAllLine == 1)
                tempWinGold = tempWinGold * 10;
            if(Game1600Model.Instance.nFreeGame == 0)
                m_Txt_Score.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(tempWinGold);
            autoPanel.SetScore(tempWinGold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            m_Txt_Gold.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(Game1600Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () => {SlotCpt.continueSpin();}, 100000);

            if(Game1600Model.Instance.gameStates == 3 && Game1600Model.Instance.toSpin.WinGold > 0)
            {
                Game1600Model.Instance.gameStates = 0;
            }
        }

        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin && Game1600Model.Instance.n64FreeGold > 0)
                return;
            m_Txt_Score.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(score);// (score/16000f).ToString();
            autoPanel.SetScore(score);
        }

        public async void InitGame()
        {
            BetList = Game1600Model.Instance.GearList;// --.Gear1;
            betMax = BetList[BetList.Count - 1];
            betMin = BetList[0];
            Bet = BetList[0];
            if (Game1600Model.Instance.nBet > 0)
                Bet = Game1600Model.Instance.nBet;
            else
                betID = 0;
            OnBetChange(Bet);
            m_Btn_Min.interactable = Bet != betMin;
            m_Btn_Add.interactable = Bet != betMax;
            SlotCpt.setState(slotState.Idle);
            if (Game1600Model.Instance.nBet > 0)
            {
                SlotCpt.SetFreeData();
      
                
                StartCoroutine(SlotCpt.DoGoSlotScale(1));
                SetWinGoldValue(Game1600Model.Instance.n64FreeGold);                
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
                CoreEntry.gAudioMgr.PlayUIMusic(276);
                SlotCpt.continueSpin();
            }
            SetTopRank();
        }

        protected void SetTopRank()
        {
           if(Game1600Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1600Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                m_TxtM_ID.text = CommonTools.BytesToString(Game1600Model.Instance.arrayAward[0].szName);
                m_TxtM_Money.text = "R$ " + ToolUtil.ShowF2Num(Game1600Model.Instance.arrayAward[0].n64Gold);
                string imgurl = "img_Head_" + Game1600Model.Instance.arrayAward[0].nIconID;
                m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
            }
        }

        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1600, BetGameRet1600);// -- 1600房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1600, RefreshJACKPOT1600);// -- 1600房间下注结果返回
            UnRegisterListener();
        }


        public void BeginSlot(int num)
        {
            SlotCpt.beginSpin(num, false);
        }



        public void OnClickBtnLeave()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            if (!SlotCpt.bDanJi)
            {
                CoreEntry.gTimeMgr.Reset();
                NetLogicGame.Instance.Send_CS_HUMAN_LEAVE_GAME_REQ();
            }
        }

        public bool GetTogTurboIsOn()
        {
            return m_Tog_Turbo.isOn;
        }

        public void OnClickBtnMin()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
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
            CoreEntry.gAudioMgr.PlayUISound(285);
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
                m_Txt_Gold.color = new Color32(93,167,184,255);
                m_Txt_SingleGold.color = new Color32(93, 167, 184, 255);
                m_Txt_Score.color = new Color32(93, 167, 184, 255);
            }
            else
            {
                m_Txt_Gold.color = new Color32(255, 255, 255, 255);
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

        List<int> zhongjiangSoundIndex = new List<int> { 264,273,274};
        int soundIndex = 0;

        public void OnSlotWinGold()
        {
            StartCoroutine(ShowGameTips(3));
            winGold = Game1600Model.Instance.toSpin.WinGold;
            if (Game1600Model.Instance.n64FreeGold > 0)
                winGold = Game1600Model.Instance.n64FreeGold;
            long tempWinGold = winGold;
            if (Game1600Model.Instance.bAllLine == 1)
                tempWinGold = winGold ;
            RectTransform rect = m_Txt_WinGold.GetComponent<RectTransform>();

            if ((Game1600Model.Instance.toSpin.rate < 5 && Game1600Model.Instance.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                if (Game1600Model.Instance.lastFreeGameIndex == 0)
                {
                    CoreEntry.gAudioMgr.PlayUISound(283);
                    CoreEntry.gAudioMgr.PlayUISound(zhongjiangSoundIndex[soundIndex], transform.gameObject);
                    soundIndex++;
                    if (soundIndex >= 3)
                        soundIndex = 0;
                  
                }
                m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold - Game1600Model.Instance.n64NumberPowerGold);
                if (Game1600Model.Instance.bAllLine == 1 && Game1600Model.Instance.nFreeGame == 0)
                    m_Txt_Score.text = "R$:" + ToolUtil.ShowF2Num(tempWinGold);
                else
                {
                    if (rect.sizeDelta.x > 200)
                        rect.anchoredPosition = new Vector2(-(rect.sizeDelta.x - 200) / 2, -2.1f);
                    else
                        rect.anchoredPosition = new Vector2(0, -2.1f);
                    if (Game1600Model.Instance.nFreeGame == 0)
                        m_Txt_Score.text = "R$:" + ToolUtil.ShowF2Num(tempWinGold);

                    autoPanel.SetScore(winGold);
                    autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold);
                }

            }
            else
            {
                if (Game1600Model.Instance.toSpin.rate >= 5 && Game1600Model.Instance.toSpin.rate <= 20)
                {
                    CoreEntry.gAudioMgr.PlayUISound(280);
                    CoreEntry.gAudioMgr.PlayUISound(272, transform.GetChild(3).gameObject);
                    CoreEntry.gTimeMgr.AddTimer(0.35f, false, () => CoreEntry.gAudioMgr.PlayUISound(268, transform.GetChild(2).gameObject), 14);
                    ToolUtil.RollText(0, tempWinGold, m_Txt_WinGold, 2f, () =>
                    {
                        PlayAni(m_Txt_WinGold.transform);
                        CoreEntry.gAudioMgr.StopSound(transform.gameObject);
                        ClickBtnCloseEffect();
                    });
                    string temp = ToolUtil.ShowF2Num(tempWinGold);
                    if (temp.Length > 6)
                        rect.anchoredPosition = new Vector2(-(temp.Length - 6) * 36 / 2 - 10, -2.1f);
                    else
                        rect.anchoredPosition = new Vector2(0, -2.1f);
                }
                else
                {
                    m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold);
                    m_Gold_EffectNew.setData(playGoldType, Game1600Model.Instance.toSpin.WinGold, () =>
                    {
                        CoreEntry.gAudioMgr.PlayUIMusic(265);
                        SlotCpt.continueSpin();
                    }, SlotCpt.autoSpinNum != 0);
                }
         
            }
        }

        public async void SetWinGoldBg(int rate, bool bDelay = false)
        {
            if (bDelay)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            if (rate <0)
            {
                m_Trans_RedGoldBg.gameObject.SetActive(true);
                m_Trans_GreenGoldBg.gameObject.SetActive(false);
                m_Trans_PinkGoldBg.gameObject.SetActive(false);
                //if(rate >=0)
                //    CoreEntry.gAudioMgr.PlayUISound(217, SlotCpt.GetSoundObj1());
            }
            else if(rate <20)
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
                //CoreEntry.gAudioMgr.PlayUISound(219, SlotCpt.GetSoundObj1());
            }
        }

        public void BigWinAni(Action callBack = null)
        {
            m_Trans_Effect.gameObject.SetActive(true);
            gold = 0;
            m_Gold_EffectNew.setData(playGoldType, Game1600Model.Instance.n64FreeGold, () =>
            {
                m_Txt_WinGold.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                m_Txt_WinGold.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                m_Dragon_GetGoldEffect.gameObject.SetActive(true);
                CoreEntry.gAudioMgr.PlayUISound(268, gameObject);
                PlayAni(m_Trans_Win.transform);
                CommonTools.PlayArmatureAni(m_Dragon_GetGoldEffect.transform, "Sprite", 1, () => PlayBigWinAniCallBack(callBack));               
            }, SlotCpt.autoSpinNum != 0);  
        }

        private void PlayBigWinAniCallBack(Action callBack = null)
        {
            CoreEntry.gAudioMgr.PlayUIMusic(265);
            m_Dragon_GetGoldEffect.gameObject.SetActive(false);
            SlotCpt.ResetScale();
            SlotCpt.ReturnNormal();
  
            SetScoreText(Game1600Model.Instance.n64FreeGold);
            DOTween.To(() => 1f, (value) =>
            {
                m_Trans_Free.GetComponent<CanvasGroup>().alpha = value;
            }, 0, 0.9f).OnComplete(() =>
            {
                m_Trans_Free.GetComponent<CanvasGroup>().alpha = 1;
                m_Trans_Free.gameObject.SetActive(false);
            }).SetEase(Ease.Linear);
            callBack?.Invoke();
            ClickBtnCloseEffect();
        } 

        public void OnBetChange(int bet)
        {
            Bet = bet;
            for(int i = 0;i < BetList.Count;i++ )
            {
                if (BetList[i] == bet)
                {
                    int temps22 = BetList[i];
                    Game1600Model.Instance.nBet1 = temps22;
                    betID = i;
                }
            }
            m_Txt_SingleGold.text = ToolUtil.GetCurrencySymbol()+ ":"+ ToolUtil.ShowF2Num2(bet*10);
            autoPanel.SetSingleGoldText(bet);
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
        }



        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gAudioMgr.PlayUISound(71);
        }

        public void BetGameRet1600()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }





        public void RefreshJACKPOT1600()
        {
            SetTopRank();
        }

        public void ReloadGame()
        {
            m_Txt_Gold.text = "R$:"+ ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Golds);
            autoPanel.SetGoldText(MainUIModel.Instance.palyerData.m_i8Golds);
            if (SlotCpt.gameStatus == 1)
            {
               // SlotCpt.continueSpin();
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
            }
        }

        public void UpdateGold(long gold)
        {
            Game1600Model.Instance.toSpin.n64Gold = gold;
            m_Txt_Gold.text = "R$:"+ ToolUtil.ShowF2Num(gold);
            autoPanel.SetGoldText(gold);
            MainUIModel.Instance.Golds = gold;
        }
 
        public void PlayGameTipsLoop()
        {
            CoreEntry.gTimeMgr.RemoveTimer(312);
            CoreEntry.gTimeMgr.AddTimer(6, true, () =>
            {
                SetWinGoldBg(-1);
                PlayGameTips(gameTipIndex);
                if (gameTipIndex == 0)
                {
                    m_Trans_Respins.localPosition = Vector3.zero;
                    m_Trans_Respins.DOLocalMoveX(-820, 6.5f).SetDelay(0.2f).SetEase(Ease.Linear).SetAutoKill();
                }
                gameTipIndex++;
                if (gameTipIndex >= 3)
                    gameTipIndex = 0;
            }, 312);
        }

        public IEnumerator ShowGameTips(int type,bool bSpecial = false,Action callBack = null)
        {
            if (Game1600Model.Instance.bHasElement(8))
                yield return new WaitForSeconds(1.2f);

            yield return new WaitForSeconds(bSpecial == false? 0.35f:2.5f);
            CoreEntry.gTimeMgr.RemoveTimer(312);
            PlayAni(m_Trans_Win.transform);
            PlayGameTips(3);
            SetWinGoldBg((int)Game1600Model.Instance.toSpin.rate);
            callBack?.Invoke();
            //if (Game1600Model.Instance.toSpin.rate < 2)
            //{
            //    CoreEntry.gAudioMgr.PlayUISound(283, transform.GetChild(3).gameObject);
            //}
            //else 
            //{
            //    CoreEntry.gAudioMgr.PlayUISound(272, transform.GetChild(3).gameObject);
            //}
        }

        public void ContiunePlayGameTipsLoop()
        {
            m_Txt_WinGold.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            m_Txt_WinGold.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            CoreEntry.gTimeMgr.RemoveTimer(312);
            SetWinGoldBg(-1);
            PlayGameTips(gameTipIndex);
            if(gameTipIndex == 0)
            {
                m_Trans_Respins.localPosition = Vector3.zero;
                m_Trans_Respins.DOLocalMoveX(-820, 6.5f).SetDelay(0.25f).SetEase(Ease.Linear).SetAutoKill();
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

        public void SetFreeTimes(bool bShow,int times = 0,bool bShowFreeContent = true)
        {
            m_Trans_Free.gameObject.SetActive(bShow);
            m_Txt_FreeTimes.text = times.ToString();
            m_Trans_Content.gameObject.SetActive(bShowFreeContent);
            m_Trans_0.gameObject.SetActive(times<=0);
            m_Trans_Not0.gameObject.SetActive(times > 0);
            m_Trans_Content.gameObject.SetActive(times > 0);
        }
        public void ShowFreeAni(bool bShow)
        {
            m_Trans_GetFreeAni.gameObject.SetActive(bShow);
        }

        public void DoFreeAni(Callback callBack = null,bool bFree = true,Callback callBack2 = null)
        {
            m_Trans_FreeRabbit.anchoredPosition = new Vector2(0, 0);

            m_Trans_FreeCount.gameObject.SetActive(false);
            m_Dragon_Rabbit1.gameObject.SetActive(true);
            m_Dragon_Rabbit2.gameObject.SetActive(true);
            m_Dragon_Rabbit3.gameObject.SetActive(true);
            if (!bFree)
                return;
            Sequence seq = DOTween.Sequence();
            m_Trans_FreeCount.gameObject.SetActive(true);
            m_Trans_FreeCount.localScale = new Vector3(0.9f, 0.9f, 1);

            m_Trans_FreeRabbit.DOLocalMoveY(304, 0.4f).SetDelay(0.3f);
            Tweener t1 = m_Trans_FreeCount.DOScale(Vector3.one, 0.3f);
            seq.Append(t1);
            seq.AppendInterval(1);
            seq.OnComplete(() =>
            {
                dealy(callBack, callBack2);
            });
        }

        private async void dealy(Callback callBack,Callback callBack2 = null)
        {
            bPlayRabbitAni(false);
            m_Trans_FreeRabbit.DOLocalMoveY(0, 0.35f).SetDelay(0.3f);
            callBack2?.Invoke();
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.4f));
            m_Trans_FreeCount.gameObject.SetActive(false);
            callBack?.Invoke();
        }

        public void bPlayRabbitAni(bool bPlay)
        {
            if(bPlay)
            {
                m_Dragon_Rabbit1.gameObject.SetActive(true);
                m_Dragon_Rabbit2.gameObject.SetActive(true);
                m_Dragon_Rabbit3.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_Rabbit1.transform,"newAnimation",1);
                CommonTools.PlayArmatureAni(m_Dragon_Rabbit2.transform, "newAnimation", 1);
                CommonTools.PlayArmatureAni(m_Dragon_Rabbit3.transform, "newAnimation", 1);
            }
            else
            {
                m_Dragon_Rabbit1.gameObject.SetActive(false);
                m_Dragon_Rabbit2.gameObject.SetActive(false);
                m_Dragon_Rabbit3.gameObject.SetActive(false);
            }    
        }

        public Transform GetTrans_Free2()
        {
            return m_Trans_Free2;
        }

        public Transform GetTrans_Free()
        {
            return m_Trans_Free;
        }


        public Text GetTextGold()
        {
            return m_Txt_Gold;
        }

        public virtual void ClickGoldAdd()
        {
            //CoreEntry.gAudioMgr.PlayUISound(198);
            //if (!SlotCpt.bDanJi)
            //    MainUICtrl.Instance.OpenShopPanel();
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
                m_Dragon_Begin.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_Begin.transform,"Sprite",1,()=> m_Dragon_Begin.gameObject.SetActive(false));
                CoreEntry.gAudioMgr.PlayUISound(266, SlotCpt.GetSoundObj1());
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
            {
                m_Trans_Rorate.transform.localEulerAngles = Vector3.zero;        
            }

            m_Trans_Normal.gameObject.SetActive(!bPause);
            m_Trans_Grey.gameObject.SetActive(bPause);
        }

        private void ClickAuto()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            autoPanel.OpenPanel((num)=> 
            {
                SlotCpt.autoSpinNum = num;
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(160000);
                    BeginSlot(num);
                }
            });

        }

        private void ClickBtnCloseAutoSpin()
        {
            if (Game1600Model.Instance.gameStates == 1 || (Game1600Model.Instance.gameStates == 3))
                return;

            CoreEntry.gAudioMgr.PlayUISound(285);
            m_Trans_GoAutoSpinNum.gameObject.SetActive(false);

            CoreEntry.gAudioMgr.PlayUISound(1);
            SlotCpt.autoSpinNum = 0;
        }

        private void ClickBtnMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            m_Trans_MenuePanel.gameObject.SetActive(true);
        }
        private void ClickBtnCloseMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            m_Trans_MenuePanel.gameObject.SetActive(false);
        }

        private void ClickBtnPayTable()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            MainPanelMgr.Instance.ShowDialog("UIRoom1600Help");
        }

        private void ClickBtnRule()
        {
            CoreEntry.gAudioMgr.PlayUISound(285);
            MainPanelMgr.Instance.ShowDialog("UIRoom1600Help", true, 2);
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

        private void ClickTrbro(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(285);

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
                        _bet = BetList[i - 1];
                }
                break;
            }
            return _bet;
        }

        public void ShowOneLine(int index, bool bShow)
        {
            if (SlotCpt.slotColumns[0].isRolling && bShow)
                return;
            SlotCpt.linesList0[index - 1].gameObject.SetActive(bShow);
            SlotCpt.linesList1[index - 1].gameObject.SetActive(bShow);
            SlotCpt.linesList2[index - 1].gameObject.SetActive(bShow);
        }

        public void ShowAllLines(bool bShow = false)
        {
     
            if (SlotCpt == null)
                return;
            if (SlotCpt.slotColumns.Count > 0 && SlotCpt.slotColumns[0].isRolling && bShow)
                return;
            for (int i = 0; i < SlotCpt.linesList0.Count; i++)
            {
                lineTranList[i].gameObject.SetActive(bShow);
                SlotCpt.linesList0[i].gameObject.SetActive(bShow);
                SlotCpt.linesList1[i].gameObject.SetActive(bShow);
                SlotCpt.linesList2[i].gameObject.SetActive(bShow);
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

        public Transform GetMoveGoldPos()
        {
            return m_Trans_WinGoldBg;
        }

        public Transform GetmDragon_AddGoldEffect()
        {
            return m_Dragon_AddGoldEffect.transform;
        }


        public void SetWinGoldValue(long value)
        {
            m_Txt_WinGold.text = ToolUtil.ShowF2Num(value);
        }

        public void SetScoreText( long value)
        {
            m_Txt_Score.text = "R$:" + ToolUtil.ShowF2Num(value);
        }

        public Image GetImg_LeftLight()
        {
            return m_Img_LeftLight;
        }
        public Image GetImg_RightLight()
        {
            return m_Img_RightLight;
        }
    }
}
