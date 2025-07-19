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
    public partial class Top1100Panel : PanelBase
    {
        private UIRoom1100 SlotCpt;
        protected slotState m_curSlotState;
        protected long gold = 0;
        public long winGold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;

       // public GoldEffect m_Gold_Effect;
        public GoldEffectNew m_Gold_EffectNew;
        Sequence[] mySequences = new Sequence[10];

        public int freeRemainTimes = 5;

        public List<Transform> SpineCell = new List<Transform>();

        public List<Transform> SpineCell2 = new List<Transform>();

        List<Transform> linesList = new List<Transform>();
        /// <summary>
        /// 当前奖池类型
        /// </summary>
        private int currentType = 0;
        Sequence seq;
        Sequence seq2;
        public Material m_UIGrey;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "img_Head_7");
            m_Img_HeadMove.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + "img_Head_7");
            //GameObject go = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_Effect");
            //m_Gold_Effect = go.GetComponent<GoldEffect>();
            //m_Gold_Effect.gameObject.SetActive(false);

            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_EffectNew");
            m_Gold_EffectNew = go1.GetComponent<GoldEffectNew>();
            m_Gold_EffectNew.gameObject.SetActive(false);
            for (int i = 1; i <= 30; i++)
                linesList.Add(transform.Find("Trans_Middle/Lines/line" + i));
            Transform temp = transform.Find("Trans_Middle/TfEffectSpine/TfEffectSpine");
            for (int i = 0; i < 20; i++)
                SpineCell.Add(temp.GetChild(i));
            Transform temp2 = transform.Find("Trans_Middle/TfEffectSpine2/TfEffectSpine");
            for (int i = 0; i < 20; i++)
                SpineCell2.Add(temp2.GetChild(i));

            m_UIGrey = m_Img_Blur.material;
        }

        protected override void Start()
        {
            base.Start();
            PlayTitleAni();
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1100;
            SlotCpt.commonTop.OnRank = ClickBtnRank;
            SlotCpt.commonTop.OnHelp = ClickBtnHelp;
            SlotCpt.commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.commonTop.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.commonTop.beginSpin = BeginSlot;
            SlotCpt.commonTop.OnPointerUp = null;
            SlotCpt.commonTop.OnTolgAuto = null;
            SlotCpt.commonTop.ClickCondition = ClickCondition;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet1100, BetGameRet1100);// 1100房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1100, RefreshJACKPOT1100);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);

            RegisterListener();
            for(int i = 0;i < mySequences.Length;i++)
                mySequences[i]?.Kill();

            CoreEntry.gTimeMgr.AddTimer(0.1f,false,()=> 
            {
                m_Spine_lightning.gameObject.SetActive(true);

                ToolUtil.PlayAnimation(m_Spine_lightning.transform, "a1", false,()=> 
                {
                    m_Spine_lightning.gameObject.SetActive(false);
                });         
            },1);
            ShowMask(false);

            ShowAllLines(false);
        }

        public void RegisterListener()
        {
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Rank.onClick.AddListener(ClickBtnRank);

            m_Btn_Continue.onClick.AddListener(ClickContinue);
            m_Btn_Coletar.onClick.AddListener(ClickColetar);
        }

        public void UnRegisterListener()
        {
            m_Btn_Rank.onClick.RemoveListener(ClickBtnRank);
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Continue.onClick.RemoveListener(ClickContinue);
            m_Btn_Coletar.onClick.RemoveListener(ClickColetar);
        }

        public async void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game1100Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            if(SlotCpt.bDanJi)
                Game1100Model.Instance.n64Jackpot = 6850000;
            SlotCpt.commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            InitGame();
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;
            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game1100Model.Instance.arrayAward.Count; i++)
            {
                if (Game1100Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1100Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList,1100);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom1100Help");
        }

        public void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            CoreEntry.gTimeMgr.RemoveTimer(110000);
            SlotCpt.commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold);
        }
     
        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin && Game1100Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.commonTop.UpDateScore(0);
        }

        public void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game1100Model.Instance.GearList, Game1100Model.Instance.nBet);
            SlotCpt.setState(slotState.Idle);
            SetTopRank();
            if (Game1100Model.Instance.nSpecialCount > 0)
                SlotCpt.SetSpecialElement();
        }
        protected void SetTopRank()
        {
            currentType = 1;
           if (Game1100Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1100Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game1100Model.Instance.arrayAward[0].szName), Game1100Model.Instance.arrayAward[0].n64Gold, Game1100Model.Instance.arrayAward[0].nIconID, Game1100Model.Instance.arrayAward[0].nRate);
                //m_TxtM_ID.text = CommonTools.BytesToString(Game1100Model.Instance.arrayAward[0].szName);
                //m_TxtM_Money.text = "R$ " + ToolUtil.ShowF2Num(Game1100Model.Instance.arrayAward[0].n64Gold);
                //string imgurl = "img_Head_" + Game1100Model.Instance.arrayAward[0].nIconID;
                //m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
                //m_Txt_Rate.text = Game1100Model.Instance.arrayAward[0].nRate + "X";
            }
        }

        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1100, BetGameRet1100);// -- 1100房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1100, RefreshJACKPOT1100);// -- 1100房间下注结果返回
            UnRegisterListener();
        }

        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }
       
        public bool ClickCondition()
        {
            return Game1100Model.Instance.nBet > 0;
        }




        private void ClickContinue()
        {
            CoreEntry.gTimeMgr.RemoveTimer(3);
            m_Trans_TfFree.gameObject.SetActive(false);
            CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "2c", 1, () => { SlotCpt.continueSpin(); });
        }
        private void ClickColetar()
        {
            m_Trans_TfFree.gameObject.SetActive(false);
            CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "1c", 1, () => { SlotCpt.continueSpin(); });
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {

        }

        public void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold= Game1100Model.Instance.toSpin.WinGold;
            if (Game1100Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game1100Model.Instance.toSpin.n64FreeGold; 

            if ((Game1100Model.Instance.toSpin.rate <= 2 && Game1100Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin || Game1100Model.Instance.specialWinGold>0)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
                {
                    if (Game1100Model.Instance.specialWinGold <= 0)
                        SlotCpt.commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold);
                }, 7);
            }
            if (Game1100Model.Instance.toSpin.n64FreeGold <= 0 && !SlotCpt.bActiveDragon_Cash())
                CoreEntry.gTimeMgr.AddTimer(1.5f,false,()=> {BigWinAni(null, false);},22);
        }

        public void BigWinAni(Action callBack,bool freeEnd, int bSpecialNum = 0)
        {
            if (Game1100Model.Instance.toSpin.rate > 2 || freeEnd || bSpecialNum > 0)
            {
                m_Trans_Effect.gameObject.SetActive(true);
                gold = 0;
                if (freeEnd)
                {
                    gold = Game1100Model.Instance.toSpin.n64FreeGold;
                    Game1100Model.Instance.toSpin.rate = gold / Game1100Model.Instance.nBet1;
                }
                else if (bSpecialNum > 0)
                {
                    gold = Game1100Model.Instance.toSpin.WinGold + Game1100Model.Instance.specialWinGold + Game1100Model.Instance.n64SpecialPowerGold;
                    Game1100Model.Instance.toSpin.rate = gold / Game1100Model.Instance.nBet1;
                }
            
                else
                    gold = Game1100Model.Instance.toSpin.WinGold + Game1100Model.Instance.specialWinGold;
                winGold = gold;
                playGoldType = 0;

         
                if (Game1100Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game1100Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {
                    if (Game1100Model.Instance.toSpin.rate > 2 && Game1100Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game1100Model.Instance.toSpin.rate > 4 && Game1100Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else
                        playGoldType = 3;

                }

                if (bSpecialNum > 0 && Game1100Model.Instance.toSpin.rate <=2)
                    playGoldType = 1;

                m_Gold_EffectNew.setData(playGoldType, gold, () =>
                {
                    SlotCpt.commonTop.GetBeginBtn().GetComponent<Canvas>().overrideSorting = false;
                    ClickBtnCloseEffect();
                    if (SlotCpt.IsAutoSpin())
                    {
                        CoreEntry.gTimeMgr.AddTimer(0.7f,false,()=> 
                        {
                            SlotCpt.continueSpin();
                        },31);
                    }
                 
                }, SlotCpt.autoSpinNum != 0, null, () =>
                {
                    callBack?.Invoke();
                    if (!SlotCpt.IsAutoSpin() && SlotCpt.freeTimes.max <= 0)
                    {
                        SlotCpt.commonTop.GetBeginBtn().GetComponent<Canvas>().overrideSorting = true;
                        SlotCpt.continueSpin();
                    }
                });
            }
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game1100Model.Instance.nBet1 = bet;
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            SlotCpt.InitData();
            SlotCpt.ShowAllCell(false);
            m_Trans_Mask.gameObject.SetActive(false);
            DestroyAni();
            InitGearData();
            ShowAllLines(false);
            SlotCpt.SetGrearData();
        }

        private IEnumerator Handle()
        {
            yield return new WaitForSeconds(0.02f);
            InitCell();
        }


        private void InitCell()
        {
            for (int i = 0; i < 20; i++)
            {
                int col = i / 4;
                int row = i % 4;
                SlotCpt.slotColumns[col].lstCells[3 - row].element = SlotData_1100.specialelement;
                SlotCpt.slotColumns[col].lstCells[3 - row].showLine(null, SlotData_1100.specialelement);
            }
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            m_Trans_TfFree.gameObject.SetActive(true);
            m_Btn_Continue.interactable = false;
            CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "2a", 1, () => 
            {
                m_Btn_Continue.interactable = true;
                CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "2b", 0, () =>{});
            });


            var kk = m_Dragon_Free.transform.Find("a01");
            // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            m_Txt_FreeTimes.transform.SetParent(kk);
            m_Txt_FreeTimes.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            m_Txt_FreeTimes.transform.localPosition = new Vector3(0, 0, 0);


            m_Trans_FreeTimes.gameObject.SetActive(true);
            m_Trans_FreeGameEnd.gameObject.SetActive(false);
            //m_Txt_Times.text = Game1100Model.Instance.toSpin.FreeTimes + "/" + Game1100Model.Instance.toSpin.nModelGame;
           // m_Txt_FreeTimes.text = Game1100Model.Instance.toSpin.nModelGame - Game1100Model.Instance.lastCount + "";
            Game1100Model.Instance.bShowFreeAni = true;

            //CoreEntry.gAudioMgr.PlayUISound(38);

            m_Txt_FreeTimes.gameObject.SetActive(true);
            m_Txt_GetFreeGold.gameObject.SetActive(false);

            freeRemainTimes = 5;
            m_TxtM_Times.text = string.Format("continuar após {0} segundos", freeRemainTimes);
            CoreEntry.gTimeMgr.AddTimer(1, true, () =>
            {
                freeRemainTimes--;
                m_TxtM_Times.text = string.Format("continuar após {0} segundos", freeRemainTimes);
                if (freeRemainTimes == 0)
                {
                    ClickContinue();
                }

            }, 3);
        }

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            m_Trans_TfFree.gameObject.SetActive(true);
            m_Trans_FreeGameEnd.gameObject.SetActive(true);
            m_Trans_FreeTimes.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(false);
    
            m_Btn_Coletar.interactable = false;
            CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "1a", 1, () =>
            {
                m_Btn_Coletar.interactable = true;
                CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "1b", 0, () => {});
            });
            //if (Game1100Model.Instance.toSpin.n64FreeGold > 0)
            //    BigWinAni(null, true, 0);
            //SetBtnsScale(true);
            m_Txt_FreeTimes.gameObject.SetActive(false);
            m_Txt_GetFreeGold.gameObject.SetActive(true);
            var kk = m_Dragon_Free.transform.Find("a02");
            // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            m_Txt_GetFreeGold.transform.SetParent(kk);
            m_Txt_GetFreeGold.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            m_Txt_GetFreeGold.transform.localPosition = new Vector3(0, -0.17f, 0);

        }

        public void BetGameRet1100()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void RefreshJACKPOT1100()
        {
           // SetBoradCast();

            SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game1100Model.Instance.arrayAward[0].szName), Game1100Model.Instance.arrayAward[0].n64Gold, Game1100Model.Instance.arrayAward[0].nIconID, Game1100Model.Instance.arrayAward[0].nRate);
        }

        public void ReloadGame()
        {
            SlotCpt.commonTop.UpdateGold(MainUIModel.Instance.palyerData.m_i8Golds);
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
            Game1100Model.Instance.toSpin.n64Gold = gold;
            SlotCpt.commonTop.UpdateGold(gold);
        }

        private void ClickAuto(bool isOn)
        {
            if (isOn)
            {
                SlotCpt.autoSpinNum = -1;
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(100000);
                    SlotCpt.commonTop.BeginSlot(-1);
                }
            }
            else
                SlotCpt.autoSpinNum = 0;
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



        public void ShowMask(bool bShow = false)
        {
            m_Trans_Mask.gameObject.SetActive(bShow);
        }
        public void FinishShake()
        {
            m_Trans_Middle.DOKill();
            m_Spine_Title.DOKill();
        }
        public void PlayTitleAni()
        {
            int index = 1;
            CoreEntry.gTimeMgr.AddTimer(6f, true, () => 
            {
                index++;
                if (index > 3)
                    index = 1;
                string aniNameA = string.Format("{0}a",index);
               // string aniNameB = string.Format("{0}c", index);
                string aniNameC = string.Format("{0}b", index);
               // Debug.LogError("----"+ aniNameC);
                ToolUtil.PlayAnimation(m_Spine_Title.transform, aniNameA, false, () => 
                {
                    ToolUtil.PlayAnimation(m_Spine_Title.transform, aniNameC, false);
                });
            },25);
        }
        public void SetCashFinished()
        {
            SetAllCashAni(4);
            CreateGoldEffect();
            StartCoroutine(HandleSpecialResult());
        }

        private IEnumerator HandleSpecialResult()
        {
            yield return new WaitForSeconds(1.5f);
            CoreEntry.gAudioMgr.PlayUISound(119);
            m_Trans_Cash.gameObject.SetActive(true);
            m_Txt_WinRate.gameObject.SetActive(false);
            m_Trans_CashRate.gameObject.SetActive(true);
            var a4 = m_Spine_ResultIcon.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            m_Txt_ElementCount.transform.SetParent(a4);
            m_Txt_ElementCount.transform.localScale = Vector3.one;
            m_Txt_ElementCount.transform.localPosition = new Vector3(-147.104f, -43.2f,0);
            m_Txt_Double.transform.SetParent(a4);
            m_Txt_Double.transform.localScale = new Vector3(0.8f,0.8f,1);
            m_Txt_Double.transform.localPosition = new Vector3(289, -27.2f, 1);
            m_Txt_ElementCount.text = Game1100Model.Instance.nSpecialCount + "";
            m_Txt_Double.text = string.Format("{0}X", Game1100Model.Instance.specialRate[Game1100Model.Instance.nSpecialCount]);

            ToolUtil.PlayAnimation(m_Spine_Result.transform, "b2", false);
            ToolUtil.PlayAnimation(m_Spine_ResultIcon.transform, "b1", false,()=>
            {
                if (Game1100Model.Instance.toSpin.rate < -1)
                {
                    ToolUtil.PlayAnimation(m_Spine_Result.transform, "b2", false);
                    m_Txt_WinRate.gameObject.SetActive(true);
                    m_Txt_WinRate.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.5f).OnComplete(() =>
                    {
                        m_Txt_WinRate.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).OnComplete(() =>
                        {
                            m_Trans_Cash.gameObject.SetActive(false);
                            m_Txt_WinRate.gameObject.SetActive(false);
                            SlotCpt.ContinueGame();
                        });
                    });
                }
                else
                {
                    m_Trans_Cash.gameObject.SetActive(false);
                    SlotCpt.PlayBgSound(0);
                    BigWinAni(() => { }, false, 1);
                }
            });
        }


        public void SetAniParent()
        {
            for(int i = 0;i < SlotCpt.slotColumns.Count;i++)
            {
                for(int j = SlotCpt.slotColumns[i].lstCells.Count - 2;j >=0 ; j--)
                {
                    UISlotCell cell = SlotCpt.slotColumns[i].lstCells[j];
                    if (cell.TfSpine.childCount > 0)
                    {
                        if (SpineCell[i * 4 + 3 - j].childCount > 0)
                            continue;
                        Transform parent;
                        if(SlotCpt.slotColumns[i].lstCells[j].element == 10)
                            parent = SpineCell[i * 4 + 3 - j];
                        else
                            parent = SpineCell2[i * 4 + 3 - j];
                        Vector3 pos = cell.TfSpine.GetChild(0).position;
                        cell.TfSpine.GetChild(0).SetParent(parent);
                        if(cell.element == 10)
                        {
                            parent.GetChild(0).localScale = new Vector3(1, 1, 1);
                            parent.GetChild(0).position = pos;
                        }
                    }
                }
            }
        }

        public void DestroyAni()
        {
            SlotCpt.SetCashAni(0);
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].childCount > 0)
                {
                    for(int j = SpineCell[i].childCount - 1; j >=0 ;j--)
                        CoreEntry.gGameObjPoolMgr.Destroy(SpineCell[i].GetChild(j).gameObject);
                }
            }
        }

        public void SetAllCashAni(int type)//4静止
        {
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].childCount > 0)
                {
                    string aniName = type == 4 ? "a4" : "a1";
                    ToolUtil.PlayAnimation(SpineCell[i].GetChild(0), aniName, true, () => { });
                }
                
            }
        }

        public void CreateGoldEffect()
        {
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].childCount > 0)
                {
                    GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Gold_Effect");
                    go.transform.SetParent(SpineCell[i], true);
                    ToolUtil.PlayAnimation(go.transform, "a3", false);
                   // CommonTools.PlayArmatureAni(go.transform, "a3", 1, () => { });
                    go.transform.localScale = new Vector3(1, 1, 1);
                    go.transform.localPosition = new Vector3(-4.3f, 0, 0);
                }
            }
        }


        private void InitGearData()
        {
            if(Game1100Model.Instance.gearData.ContainsKey(Game1100Model.Instance.betID))
            {
                if(Game1100Model.Instance.gearData[Game1100Model.Instance.betID].Count > 0)
                {
                    for (int i = 0; i < Game1100Model.Instance.gearData[Game1100Model.Instance.betID].Count; i++)
                    {
                        GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Tb" + 10);
                        go.transform.SetParent(SpineCell[Game1100Model.Instance.gearData[Game1100Model.Instance.betID][i]], true);
                        ToolUtil.PlayAnimation(go.transform, "a1", true, () => { });
                        go.transform.localScale = new Vector3(100, 100, 100);
                        go.transform.localPosition = new Vector3(-4.3f, 0, 0);
                        int col = i / 4;
                        int row = i % 4;
                        SlotCpt.slotColumns[col].lstCells[3 - row].element = 10;
                    }
                }
            }
        }

        public void ShowOneLine(int index,bool bShow)
        {
            linesList[index].gameObject.SetActive(bShow);
        }

        public void ShowAllLines(bool bShow = false)
        {
            for (int i = 0; i < linesList.Count; i++)
                linesList[i].gameObject.SetActive(bShow);
        }

        public void SetBoradCast(int type = 0)
        {
            seq?.Kill();
            seq2?.Kill();
            string nameID = CommonTools.BytesToString(Game1100Model.Instance.arrayAward[0].szName);
            string gold = "R$ " + ToolUtil.ShowF2Num(Game1100Model.Instance.arrayAward[0].n64Gold);// (Game1100Model.Instance.arrayAward[0].n64Gold / 11000).ToString("f2");
            string imgurl = "img_Head_" + Game1100Model.Instance.arrayAward[0].nIconID;
            int nRate = Game1100Model.Instance.arrayAward[0].nRate;
            if (currentType ==0)
            {
                m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
                m_TxtM_ID.text = nameID;
                m_TxtM_Money.text = gold;
                m_Txt_Rate.text = nRate + "X";
                currentType = 1;
                m_Trans_Info.gameObject.SetActive(true);
                m_Trans_InfoMove.gameObject.SetActive(true);
                float moveTimes = 0.4f;
                m_Trans_Info.transform.localPosition = new Vector3(-6.190063f, -1.865015f,0);
                m_Trans_InfoMove.transform.localPosition = new Vector3(-6.190063f, -74.4f, 0);
                seq = DOTween.Sequence();
                Tweener t1 = m_Trans_Info.transform.DOLocalMoveY(70, moveTimes);
                Tweener t2 = m_Trans_InfoMove.transform.DOLocalMoveY(-1.865015f, moveTimes);
                seq.Append(t1);
                seq.Join(t2);
                seq.OnComplete(()=> 
                {
                    seq.Kill();
                    m_Trans_Info.gameObject.SetActive(false);
                });
            }
            else
            {
                m_Img_HeadMove.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
                m_TxtM_IDMove.text = nameID;
                m_TxtM_MoneyMove.text = gold;
                m_Txt_RateMove.text = nRate+ "X";
                currentType = 0;
                m_Trans_Info.gameObject.SetActive(true);
                m_Trans_InfoMove.gameObject.SetActive(true);
                float moveTimes = 0.4f;
                m_Trans_InfoMove.transform.localPosition = new Vector3(-6.190063f, -1.865015f, 0);
                m_Trans_Info.transform.localPosition = new Vector3(-6.190063f, -74.4f, 0);
                seq2 = DOTween.Sequence();
                Tweener t1 = m_Trans_Info.transform.DOLocalMoveY(-1.865015f, moveTimes);
                Tweener t2 = m_Trans_InfoMove.transform.DOLocalMoveY(70, moveTimes);
                seq2.Append(t1);
                seq2.Join(t2);
                seq2.OnComplete(() =>
                {
                    seq.Kill();
                    m_Trans_InfoMove.gameObject.SetActive(false);
                });
            }
        }
    }
}
