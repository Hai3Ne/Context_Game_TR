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
    public partial class Top1200Panel : PanelBase
    {
        private UIRoom1200 SlotCpt;
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
        public Game1200_BigWin m_Gold_EffectNew;

        public Trans_AutoPanel autoPanel;

        public int freeRemainTimes = 5;
        public GameTips1200 gameTips;
        public BetOptionsPanel1200 betPanel;
        public List<Transform> elementList = new List<Transform>();
        public List<Transform> elementBgList = new List<Transform>();
        public List<Transform> linesList = new List<Transform>();
        public List<Text> linesTextList = new List<Text>();
        public List<Transform> lineTranList = new List<Transform>();
        /// <summary>
        /// 游戏提示类型
        /// </summary>
        private int gameTipIndex = 0;

        Vector3 Rate10Pos;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "img_Head_7");

            //GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_EffectNew");
            //m_Gold_EffectNew = go1.GetComponent<GoldEffectNew>();
            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/Prefabs/Game1200/FirstRes/Game1200_BigWin");
            m_Gold_EffectNew = go1.GetComponent<Game1200_BigWin>();
            m_Gold_EffectNew.gameObject.SetActive(false);
            Transform ElementEffect = transform.Find("Trans_ElementEffect");
            for (int i = 0; i < ElementEffect.childCount; i++)
                elementList.Add(ElementEffect.Find("Cell"+i));
            Transform ElementEffectBg = transform.Find("ElementEffectBg");
            for (int i = 0; i < ElementEffectBg.childCount; i++)
                elementBgList.Add(ElementEffectBg.Find("Cell" + i));
            Transform Lines = transform.Find("Trans_Lines");
            for (int i = 1; i <= Lines.childCount; i++)
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
            Rate10Pos = m_Txt_Rate10.GetComponent<RectTransform>().anchoredPosition;

            GameObject auto = CommonTools.AddSubChild(transform.gameObject, "UI/Prefabs/Game1200/FirstRes/Trans_AutoPanel");
            autoPanel = auto.GetComponent<Trans_AutoPanel>();
            autoPanel.gameObject.SetActive(false);
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1200;
            m_Img_Head.transform.parent.gameObject.SetActive(!SlotCpt.bDanJi);
            m_Btn_Leave.transform.gameObject.SetActive(!SlotCpt.bDanJi);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet1200, BetGameRet1200);// 1200房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1200, RefreshJACKPOT1200);

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
            m_Btn_Rank.onClick.AddListener(ClickBtnRank);
            m_Btn_Help.onClick.AddListener(ClickBtnHelp);
            m_Btn_Min.onClick.AddListener(OnClickBtnMin);
            m_Btn_Add.onClick.AddListener(OnClickBtnAdd);
            m_Tog_Auto.onValueChanged.AddListener((isOn)=> { ClickAuto(isOn);  });
            m_Btn_Auto.onClick.AddListener(ClickAuto);
            m_Btn_BeginSpin.onClick.AddListener(onPointerUp);
            m_Btn_Bet.onClick.AddListener(ClickBetPanel);

            m_Tog_Turbo.onValueChanged.AddListener((isOn)=> { ClickTrbro(isOn); });
            m_Btn_CloseAutoSpin.onClick.AddListener(ClickBtnCloseAutoSpin);

            m_Btn_Menue.onClick.AddListener(ClickBtnMenue);
            m_Btn_CloseMenue.onClick.AddListener(ClickBtnCloseMenue);
            m_Btn_PayTable.onClick.AddListener(ClickBtnPayTable);
            m_Btn_Rule.onClick.AddListener(ClickBtnRule);
        }

        public void UnRegisterListener()
        {
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Leave.onClick.RemoveListener(OnClickBtnLeave);
            m_Btn_AddGold.onClick.RemoveListener(ClickGoldAdd);
            m_Btn_Rank.onClick.RemoveListener(ClickBtnRank);
            m_Btn_Help.onClick.RemoveListener(ClickBtnHelp);
            m_Btn_Min.onClick.RemoveListener(OnClickBtnMin);
            m_Btn_Add.onClick.RemoveListener(OnClickBtnAdd);
            m_Tog_Auto.onValueChanged.RemoveListener((isOn) => { ClickAuto(isOn); });
            m_Btn_Auto.onClick.RemoveListener(ClickAuto);
            m_Btn_BeginSpin.onClick.RemoveListener(onPointerUp);
            m_Btn_Bet.onClick.RemoveListener(ClickBetPanel);

            m_Tog_Turbo.onValueChanged.RemoveListener((isOn) => { ClickTrbro(isOn); });
            m_Btn_CloseAutoSpin.onClick.RemoveListener(ClickBtnCloseAutoSpin);

            m_Btn_Menue.onClick.RemoveListener(ClickBtnMenue);
            m_Btn_CloseMenue.onClick.RemoveListener(ClickBtnCloseMenue);
            m_Btn_PayTable.onClick.RemoveListener(ClickBtnPayTable);
            m_Btn_Rule.onClick.RemoveListener(ClickBtnRule);
        }

        public void InitData()
        {
            m_TxtM_Score.text = ToolUtil.GetCurrencySymbol()+ ":" + "0";
            autoPanel.SetScore(0);
            Game1200Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            m_TxtM_Gold.text = ToolUtil.GetCurrencySymbol() + ToolUtil.ShowF2Num(Game1200Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold);
            InitGame();
        }

        public void OpenGameTips(int ele,int pos)
        {
            if(gameTips == null)
            {
                gameTips =CommonTools.AddSubChild(m_Trans_GameTips.gameObject, "UI/Prefabs/Game1200/FirstRes/GameTips1200").GetComponent<GameTips1200>();
                //gameTips = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/GameTips1200").GetComponent<GameTips1200>();

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
            for (int i = 0; i < Game1200Model.Instance.arrayAward.Count; i++)
            {
                if (Game1200Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1200Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList,1200);
        }



        public void ClickBtnHelp()
        {
            //MainPanelMgr.Instance.ShowDialog("UIRoom1200Help");
        }

        public void ClickBtnCloseEffect()
        {
            long tempWinGold = winGold;
            if (Game1200Model.Instance.bAllLine == 1)
                tempWinGold = tempWinGold * 10;
            m_TxtM_Score.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(tempWinGold);
            autoPanel.SetScore(tempWinGold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            m_TxtM_Gold.text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(Game1200Model.Instance.toSpin.n64Gold);
            autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () => {SlotCpt.continueSpin();}, 100000);

            if(Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold > 0)
            {
                Game1200Model.Instance.gameStates = 0;
                CoreEntry.gAudioMgr.PlayUIMusic(206);
            }
       
        }

        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin && Game1200Model.Instance.toSpin.n64FreeGold > 0)
                return;
            m_TxtM_Score.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(score);// (score/12000f).ToString();
            autoPanel.SetScore(score);
        }

        public async void InitGame()
        {
            BetList = Game1200Model.Instance.GearList;// --.Gear1;
            betMax = BetList[BetList.Count - 1];
            betMin = BetList[0];
            Bet = BetList[0];
            if (Game1200Model.Instance.nBet > 0)
                Bet = Game1200Model.Instance.nBet;
            else
                betID = 0;
            OnBetChange(Bet);
            m_Btn_Min.interactable = Bet != betMin;
            m_Btn_Add.interactable = Bet != betMax;
            SlotCpt.setState(slotState.Idle);
            if (Game1200Model.Instance.nBet > 0)
            {
                StartCoroutine(SlotCpt.DoGoSlotScale(1));
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
                CoreEntry.gAudioMgr.PlayUIMusic(205);
                SpecialGameEffect();
                SlotCpt.continueSpin();
            }
            SetTopRank();
        }


        protected void SetTopRank()
        {
           if(Game1200Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1200Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                m_TxtM_ID.text = CommonTools.BytesToString(Game1200Model.Instance.arrayAward[0].szName);
                m_TxtM_Money.text = ToolUtil.GetCurrencySymbol()+" " + ToolUtil.ShowF2Num(Game1200Model.Instance.arrayAward[0].n64Gold);// (Game1200Model.Instance.arrayAward[0].n64Gold / 12000).ToString("f2");

                string imgurl = "img_Head_" + Game1200Model.Instance.arrayAward[0].nIconID;
                m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
            }
        }

 


        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1200, BetGameRet1200);// -- 1200房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1200, RefreshJACKPOT1200);// -- 1200房间下注结果返回

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
            PlayAni(m_TxtM_SingleGold.transform);
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
            PlayAni(m_TxtM_SingleGold.transform);
            m_Btn_Min.interactable = true;
            if (Bet == betMax)
                m_Btn_Add.interactable = false;

            //SlotCpt.playTigerAni(3);

            //PlayBigAwardEffect(1);
            //PlayAppearRedPapgerAni(1);
            //UIRoom1200SlotCell cell = SlotCpt.slotColumns[2].lstCells[1] as UIRoom1200SlotCell;
            //cell.onSpinFinish();
        }

        private void PlayAni(Transform trans)
        {
            m_TxtM_SingleGold.transform.DOKill();
            Sequence seq = DOTween.Sequence();
            
            Tweener t1 = trans.DOScale(new Vector3(1.25f, 1.25f, 1), 0.25f);
            Tweener t2 = trans.DOScale(Vector3.one, 0.25f);
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
                m_TxtM_Gold.color = new Color32(93,167,184,255);// string.Format("<color=#5DA7B8>{0}</color>",);
                m_TxtM_SingleGold.color = new Color32(93, 167, 184, 255);
                m_TxtM_Score.color = new Color32(93, 167, 184, 255);
                //m_TxtM_Score.co
            }
            else
            {
                m_TxtM_Gold.color = new Color32(255, 255, 255, 255);// string.Format("<color=#5DA7B8>{0}</color>",);
                m_TxtM_SingleGold.color = new Color32(255, 255, 255, 255); 
                m_TxtM_Score.color = new Color32(255, 255, 255, 255);
            }

            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    //  BtnBeginSpin.gameObject.SetActive(false);
                    // BtnEndSpin.gameObject.SetActive(true);
                    //SetBtnEndSpinInteractable(false);
                    SetBtnBySlotState(false);
                    return;
                }
                m_Btn_BeginSpin.gameObject.SetActive(true);
               // BtnEndSpin.gameObject.SetActive(false);
                SetBtnBeginSpinInteractable(true);
                SetBtnBySlotState(true);
               // SlotCpt.BtnRollBar.interactable = true;
                return;
            }
            else if (state == slotState.SpinBegin)
            {
                //if (SlotCpt.IsAutoSpin())
                //{
                //    //显示 停止灰色
                //   // BtnBeginSpin.gameObject.SetActive(false);
                //  //  BtnEndSpin.gameObject.SetActive(true);
                //    SetBtnEndSpinInteractable(false);
                //    SetBtnBySlotState(false);
                //    return;
                //}
                m_Btn_BeginSpin.gameObject.SetActive(true);
                //BtnEndSpin.gameObject.SetActive(false);
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);                              //一些功能按钮不让点击
                //showAutoSpin(false);
                return;
            }
            else if (state == slotState.SpinSuccess)
            {
                //if (SlotCpt.IsAutoSpin())
                //{
                //    // --显示 停止 亮色
                //  //  BtnBeginSpin.gameObject.SetActive(false);
                //   // BtnEndSpin.gameObject.SetActive(true);
                //    SetBtnEndSpinInteractable(true);
                //    SetBtnBySlotState(false);
                //    return;
                //}


                CoreEntry.gTimeMgr.RemoveTimer(10);
                //-- 等待停止
                CoreEntry.gTimeMgr.AddTimer(0.4f, false, () =>
                {
                    if (m_curSlotState == slotState.SpinSuccess)
                    {
                        // -- stop按钮亮起并且可以点击
                        // BtnBeginSpin.gameObject.SetActive(false);
                        // BtnEndSpin.gameObject.SetActive(true);
                        //SetBtnEndSpinInteractable(true);
                        SetBtnBySlotState(false);
                        CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                        {
                            if (m_curSlotState == slotState.SpinSuccess)
                            {
                                //--stop按钮亮起并且可以点击
                                m_Btn_BeginSpin.gameObject.SetActive(true);
                               // BtnEndSpin.gameObject.SetActive(false);
                                SetBtnBeginSpinInteractable(false);
                                SetBtnBySlotState(false);// -- 一些功能按钮不让点击
                               // showAutoSpin(false);
                            }
                        }, 12);

                    }
                }, 10);

            }
            else if (state == slotState.SpinStop || state == slotState.SpinEnd)
            {
                CoreEntry.gTimeMgr.RemoveTimer(12);
                //if (SlotCpt.IsAutoSpin())
                //{
                //    //--显示 停止 灰色
                //   // BtnBeginSpin.gameObject.SetActive(false);
                //   // BtnEndSpin.gameObject.SetActive(true);
                //    SetBtnEndSpinInteractable(false);
                //    SetBtnBySlotState(false);
                //    return;
                //}



                m_Btn_BeginSpin.gameObject.SetActive(true);
               // BtnEndSpin.gameObject.SetActive(false);
                //-- SetBtnBeginSpinInteractable(false)
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);
            }
            else { }
        }

        public void OnSlotWinGold()
        {
            StartCoroutine(ShowGameTips(3));
            winGold = Game1200Model.Instance.toSpin.WinGold;
            if (Game1200Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game1200Model.Instance.toSpin.n64FreeGold;
            long tempWinGold = winGold;
            if (Game1200Model.Instance.bAllLine == 1)
                tempWinGold = winGold ;
            RectTransform rect = m_Txt_WinGold.GetComponent<RectTransform>();
            if ((Game1200Model.Instance.toSpin.rate <= 2 && Game1200Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                if(Game1200Model.Instance.bAllLine ==1)
                {
                    m_TxtM_Score.text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(tempWinGold);// (gold/12000f).ToString("f2");
                }
                else
                {
                    m_Txt_WinGold.text = ToolUtil.ShowF2Num(tempWinGold);
        
                    if(rect.sizeDelta.x > 200)
                        rect.anchoredPosition = new Vector2(-(rect.sizeDelta.x - 200)/2, 2.5f);
                    else
                        rect.anchoredPosition = new Vector2(0,2.5f);

                    m_TxtM_Score.text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(tempWinGold);

                    autoPanel.SetScore(winGold);
                    autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold);
                }         
            }
            else
            {

                CoreEntry.gAudioMgr.PlayUISound(210, SlotCpt.GetSoundObj());
                ToolUtil.RollText(0, tempWinGold, m_Txt_WinGold, 2f, () =>
                    {
                        PlayAni(m_Txt_WinGold.transform);
                        CoreEntry.gAudioMgr.StopSound(SlotCpt.GetSoundObj());
                        if (Game1200Model.Instance.bAllLine == 0)
                            ClickBtnCloseEffect();
                    },()=> 
                    {
                    });
                string temp = ToolUtil.ShowF2Num(tempWinGold);
                if(temp.Length > 6)
                    rect.anchoredPosition = new Vector2(-(temp.Length - 6)*36/2 -10, 2.5f);
                else
                    rect.anchoredPosition = new Vector2(0, 2.5f);
            }
        }

        private void SetTxt_EffectPos()
        {
            //if (goldLength != m_Txt_Effect.text.Length)
            //{
            //    goldLength = m_Txt_Effect.text.Length;
            //    RectTransform rect = m_Txt_Effect.GetComponent<RectTransform>();
            //    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            //    rect.anchoredPosition = new Vector2(-rect.sizeDelta.x / 2 * 0.5f, -66);
            //}

            //float tempGold = float.Parse(m_Txt_Effect.text);
            //tempGold = tempGold * 10000;

            //if (tempGold > tempGold3 && !bPlay3)
            //{
            //    bPlay3 = true;
            //    ShowBg(3);
            //}
            //else if (tempGold > tempGold2 && !bPlay2)
            //{
            //    bPlay2 = true;
            //    ShowBg(2);
            //}
        }

        private async void SetWinGoldBg(int rate,bool bDelay = false)
        {
            //UIRoom1200 tempSlotCpt = slotCpt as UIRoom1200;
            //CoreEntry.gAudioMgr.PlayUISound(207, SlotCpt.GetSoundObj());
            if (bDelay)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            if (rate <=4)
            {
                m_Trans_RedGoldBg.gameObject.SetActive(true);
                m_Trans_GreenGoldBg.gameObject.SetActive(false);
                m_Trans_PinkGoldBg.gameObject.SetActive(false);
                if(rate >=0)
                    CoreEntry.gAudioMgr.PlayUISound(217, SlotCpt.GetSoundObj1());
            }
            else if(rate <20)
            {
                m_Trans_RedGoldBg.gameObject.SetActive(false);
                m_Trans_GreenGoldBg.gameObject.SetActive(true);
                m_Trans_PinkGoldBg.gameObject.SetActive(false);
                CoreEntry.gAudioMgr.PlayUISound(218, SlotCpt.GetSoundObj1());
            }
            else
            {
                m_Trans_RedGoldBg.gameObject.SetActive(false);
                m_Trans_GreenGoldBg.gameObject.SetActive(false);
                m_Trans_PinkGoldBg.gameObject.SetActive(true);
                CoreEntry.gAudioMgr.PlayUISound(219, SlotCpt.GetSoundObj1());
            }
        }

        public void BigWinAni(long SpecialNum = 0,Action callBack = null)
        {
            //Debug.LogError("______"+ Game1200Model.Instance.toSpin.rate);
            //if(Game1200Model.Instance.toSpin.rate > 2 || SpecialNum > 0)
            //{
                m_Trans_Effect.gameObject.SetActive(true);
                gold = 0;
                if (SpecialNum > 0)
                    gold = SpecialNum;
                else
                    gold = Game1200Model.Instance.toSpin.WinGold;
                if (Game1200Model.Instance.bAllLine == 1)
                    gold = gold * 10;


                playGoldType = 0;
                if (Game1200Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game1200Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {
                    if (Game1200Model.Instance.toSpin.rate > 2 && Game1200Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game1200Model.Instance.toSpin.rate > 4 && Game1200Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else
                        playGoldType = 3;
                }
                m_Gold_EffectNew.setData(playGoldType, gold, () =>{ callBack?.Invoke(); ClickBtnCloseEffect();}, SlotCpt.autoSpinNum != 0);
            //}
        }

 

        public void OnBetChange(int bet)
        {
            Bet = bet;
            for(int i = 0;i < BetList.Count;i++ )
            {
                if (BetList[i] == bet)
                {
                    int temps22 = BetList[i];
                    Game1200Model.Instance.nBet1 = temps22;
                    betID = i;
                }
            }
            m_TxtM_SingleGold.text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num2(bet*5);
            autoPanel.SetSingleGoldText(bet);
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
        }

        public void SetFreeTimes()
        {
            //m_Trans_GoFreeTimes.gameObject.SetActive(true);
            //m_Txt_Times.text = Game1200Model.Instance.toSpin.FreeTimes + "/" + Game1200Model.Instance.toSpin.nModelGame;
        }

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gAudioMgr.PlayUISound(71);
        }

        public void BetGameRet1200()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }





        public void RefreshJACKPOT1200()
        {
            SetTopRank();
        }

        public void ReloadGame()
        {
            m_TxtM_Gold.text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Golds);
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
            Game1200Model.Instance.toSpin.n64Gold = gold;
            m_TxtM_Gold.text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(gold);
            autoPanel.SetGoldText(gold);
            MainUIModel.Instance.Golds = gold;
        }
 
        public void PlayGameTipsLoop()
        {
            CoreEntry.gTimeMgr.RemoveTimer(312);
            CoreEntry.gTimeMgr.AddTimer(5, true, () =>
            {
                SetWinGoldBg(-1);
                PlayGameTips(gameTipIndex);
                if (gameTipIndex == 2)
                {
                    m_Trans_Multiplier.localPosition = Vector3.zero;
                    m_Trans_Multiplier.DOLocalMoveX(-750, 4.5f).SetDelay(0.3f).SetEase(Ease.Linear).SetAutoKill();
                }
                gameTipIndex++;
                if (gameTipIndex >= 3)
                    gameTipIndex = 0;
            }, 312);
        }

        public IEnumerator ShowGameTips(int type,bool bSpecial = false,Action callBack = null)
        {
            if (Game1200Model.Instance.bHasElement(7))
                yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(bSpecial == false? 0.35f:2.5f);
            CoreEntry.gTimeMgr.RemoveTimer(312);

            //if ((Game1200Model.Instance.toSpin.rate <= 2 && Game1200Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            //{
                PlayAni(m_Trans_Win.transform);
                PlayAni(m_Trans_WinGoldBg.transform);
            //}
            //else
            //{

            //}
                
            m_Dragon_GetGoldEffect.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_GetGoldEffect.transform, "newAnimation", 1, () =>
            {
                m_Dragon_GetGoldEffect.gameObject.SetActive(false);
            });
            PlayGameTips(type);
            CoreEntry.gAudioMgr.PlayUISound(210 + 1);
            SetWinGoldBg((int)Game1200Model.Instance.toSpin.rate);
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
                m_Trans_Multiplier.DOLocalMoveX(-750, 4.5f).SetDelay(0.3f).SetEase(Ease.Linear).SetAutoKill();
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

        public TextMeshProUGUI GetTextGold()
        {
            return m_TxtM_Gold;
        }

        public Transform GetTransWin()
        {
            return m_Trans_Win;
        }

        public Transform GetTrans_Mask()
        {
            return m_Trans_Mask;
        }

        public Transform GetTransElementEffect()
        {
            return m_Trans_ElementEffect;
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
                CoreEntry.gAudioMgr.PlayUISound(220, SlotCpt.GetSoundObj1());
                CoreEntry.gTimeMgr.RemoveTimer(10);
                BeginSlot(m_Tog_Auto.isOn ? -1 : 0);
                timeBeginSpin = 0;
                m_Dragon_ClickBtn.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_ClickBtn.transform, "anniudianjiguangxiao", 1, () => { m_Dragon_ClickBtn.gameObject.SetActive(false); });
                SlotCpt.PlayWaiBianKuangAni();
            }
        }

        private void ClickBetPanel()
        {
            if (betPanel == null)
            {
                betPanel = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/BetOptionsPanel").GetComponent<BetOptionsPanel1200>();

                betPanel.transform.SetParent(transform);
                betPanel.transform.localScale = new Vector3(1, 1, 1);
                betPanel.transform.localPosition = new Vector3(0, 0, 0);
                betPanel.gameObject.SetActive(false);
                if (betPanel.transform is RectTransform rectTrs)
                {
                    rectTrs.anchoredPosition3D = Vector3.zero;
                    if (rectTrs != null)
                    {
                        rectTrs.offsetMin = Vector2.zero;
                        rectTrs.offsetMax = Vector2.zero;
                    }
                }
            }
            betPanel.Open();
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


        private void ClickAuto(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
    
            SlotCpt.autoSpinNum = 20;
            if (SlotCpt.StateSlot == slotState.Idle)
            {
                CoreEntry.gTimeMgr.RemoveTimer(120000);
                BeginSlot(20);
            }
        }

        private void ClickAuto()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            autoPanel.OpenPanel((num)=> 
            {
                SlotCpt.autoSpinNum = num;
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(120000);
                    BeginSlot(num);
                }
            });

        }

        private void ClickBtnCloseAutoSpin()
        {
            if (Game1200Model.Instance.gameStates == 1 || (Game1200Model.Instance.gameStates == 3))
                return;

            CoreEntry.gAudioMgr.PlayUISound(198);
            m_Trans_GoAutoSpinNum.gameObject.SetActive(false);

            CoreEntry.gAudioMgr.PlayUISound(1);
            SlotCpt.autoSpinNum = 0;
        }

        private void ClickBtnMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            m_Trans_MenuePanel.gameObject.SetActive(true);
        }
        private void ClickBtnCloseMenue()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            m_Trans_MenuePanel.gameObject.SetActive(false);
        }

        private void ClickBtnPayTable()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            MainPanelMgr.Instance.ShowDialog("UIRoom1200Help");
        }

        private void ClickBtnRule()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            MainPanelMgr.Instance.ShowDialog("UIRoom1200Help", true, 2);
        }

        private void ClickTrbro(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(198);

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
            m_Tog_Auto.interactable = isenabled;
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

        public void DestoryEffect()
        {
            //for (int i = 0; i < elementList.Count; i++)
            //{
            //    if (elementList[i].childCount > 0)
            //        CoreEntry.gGameObjPoolMgr.Destroy(elementList[i].GetChild(0).gameObject);
            //}
            //for (int i = 0; i < elementBgList.Count; i++)
            //{
            //    if (elementBgList[i].childCount > 0)
            //    {
            //        CoreEntry.gGameObjPoolMgr.Destroy(elementBgList[i].GetChild(0).gameObject);
            //        int col = i / 3;
            //        int row = i % 3;
            //        SlotCpt.slotColumns[col].lstCells[2 - row].ImgElement.gameObject.SetActive(true);
            //    }              
            //}

        

            ShowAllLines(false);
        }

        public void SpecialGameEffect()
        {
            for(int i = 0;i < Game1200Model.Instance.slotResult.Count;i++)
            {
                if(Game1200Model.Instance.slotResult[i] != 0)
                {
                    int col = i / 3;
                    int row =2 - i % 3;

                    UIRoom1200SlotCell cell = SlotCpt.slotColumns[col].lstCells[row] as UIRoom1200SlotCell;
                    if (elementList[i].childCount <= 0)
                    {
                        GameObject elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/SpecialElement");
                        elementGo.transform.SetParent(elementList[i], true);
                        elementGo.gameObject.SetActive(true);
                        elementGo.transform.localScale = new Vector3(1, 1, 1);
                        elementGo.transform.localPosition = new Vector3(0, 0, 0);
                        elementGo.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1200:" + "Tb" + Game1200Model.Instance.slotResult[i]);
                        elementGo.transform.DOScale(new Vector3(1.15f, 1.15f, 1.2f), 0.35f);
                        cell.ImgElement.gameObject.SetActive(false);
                    }
             
                }
            }

            List<int> allLines = Game1200Model.Instance.GetAllLines();
            if (allLines.Count > 0)
            {
                for (int i = 0; i < allLines.Count; i++)
                    ShowOneLine(allLines[i], true);
            }
        }


        public async void PlayRate10Ani(Action callBack = null)
        {
            m_Txt_Rate10.gameObject.SetActive(true);
            m_Txt_Rate10.GetComponent<RectTransform>().anchoredPosition = Rate10Pos;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
            Game1200Model.Instance.MovePos(m_Txt_Rate10.transform, m_Trans_Win.transform, () =>
             {
                 callBack?.Invoke();
                 //m_Txt_Rate10.gameObject.SetActive(false);

             }, 1f,false);
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

        public void PlayAppearRedPapgerAni(int ele = 1)
        {
            m_Dragon_RedPapger.transform.gameObject.SetActive(true);
            m_Dragon_RedPapger.GetComponent<RectTransform>().anchoredPosition = new Vector2(-115, 537);
            m_Dragon_RedPapger.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 438), 0.6f).SetEase(Ease.Linear);

            // m_Dragon_RedPapger.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,1079);
            CommonTools.PlayArmatureAni(m_Dragon_RedPapger.transform, "newAnimation", 1, () =>
               {
                   m_Dragon_RedPapger.gameObject.SetActive(false);
               });


            CoreEntry.gTimeMgr.AddTimer(2.65f,false,()=> 
            {
                SlotCpt.playTigerAni(5);
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Tb" + ele);
                go.transform.SetParent(m_Trans_SpecialElement, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(0.68f, 0.68f, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                string aniName = "newAnimation";
                if (ele == 2 || ele == 7)
                    aniName = "idle";
                CommonTools.PlayArmatureAni(go.transform, aniName, 0, () => { });

                m_Trans_SpecialElement.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 1210);
                Sequence seq = DOTween.Sequence();
                Tweener t1 = m_Trans_SpecialElement.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 660), 0.2f);
                seq.Append(t1);
                Tweener t2 = m_Trans_SpecialElement.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0,490),0.5f);
                seq.Append(t2);
                seq.AppendInterval(0.2f);
                seq.OnComplete(()=>
                {
                    CoreEntry.gGameObjPoolMgr.Destroy(m_Trans_SpecialElement.GetChild(0).gameObject);
                    SlotCpt.PlayTexSpecialRateAni();
                });
            },612);
        }

        public async void PlayBigAwardEffect(int ele, int type = 1)
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f));

            if (Game1200Model.Instance.bAllLine == 0 && type == 1)
            {
                SlotCpt.ResetScale();
                SlotCpt.GetSpecialRate().gameObject.SetActive(false);
                SlotCpt.playTigerAni(8);
                OnSlotWinGold();
                ClickBtnCloseEffect();
                DestoryEffect();
                return;
            }

            CanvasGroup canvasGroup = m_Trans_BigAwardEffect.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.65f;
            DOTween.To(() => 0.65f, (value) =>
            {
                canvasGroup.alpha = value;
            }, 1, 1.5f).OnComplete(() =>
            {
                canvasGroup.alpha = 1;
            }).SetEase(Ease.Linear);

            m_Trans_BigAwardEffect.localScale = type == 1 ? new Vector3(0.92f,0.92f,0.92f):Vector3.one;

            GameObject effect = null;
            GameObject elementGo = null;
            if (type == 1)
            {
                m_Trans_BigAwardEffect.GetChild(0).gameObject.SetActive(true);
                effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Effect_3");
                effect.transform.SetParent(m_Trans_BigAwardEffect, true);
                effect.transform.localScale = new Vector3(130f, 130f, 1);
                effect.transform.localPosition = new Vector3(0, 0, 0);
                effect.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(effect.transform, "bz", 0);
                elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Tb" + ele);
                elementGo.transform.SetParent(m_Trans_BigAwardEffect, true);
                elementGo.gameObject.SetActive(true);
                elementGo.transform.localScale = new Vector3(1.9f, 1.9f, 1);
                elementGo.transform.localPosition = new Vector3(0, 0, 0);


                string aniName = "win_idle";
                if (ele == 7)
                    aniName = "idle";
                ToolUtil.PlayTwoAnimation(elementGo.transform, "spawn", aniName);
            }
 


            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.5f));
            //StartCoroutine(ShowGameTips(3));
            OnSlotWinGold();
            if (type == 1)
            {
                SlotCpt.PlayTexSpecialRateAni2(() =>
                {
                    BigWinAni(0, () =>
                    {
                        SlotCpt.ResetScale();
                        m_Trans_BigAwardEffect.GetChild(0).gameObject.SetActive(false);
                        CoreEntry.gGameObjPoolMgr.Destroy(effect);
                        CoreEntry.gGameObjPoolMgr.Destroy(elementGo);
                        SlotCpt.playTigerAni(8);
                        DestoryEffect();
                        ClickBtnCloseEffect();
                    });
                });
            }
            else
            {
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
                PlayRate10Ani(() =>
                {
                    PlayRate10AniCallBack(effect, elementGo);
                });
            }
        }

        private async void PlayRate10AniCallBack(GameObject effect,GameObject elementGo)
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            m_Txt_Rate10.gameObject.SetActive(false);
            BigWinAni(0, () =>
            {
         
                m_Trans_BigAwardEffect.GetChild(0).gameObject.SetActive(false);
                //CoreEntry.gGameObjPoolMgr.Destroy(effect);
                //CoreEntry.gGameObjPoolMgr.Destroy(elementGo);
                DestoryEffect();

                ClickBtnCloseEffect();
            });
        }
    }
}
