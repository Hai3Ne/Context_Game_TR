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
    public class UIRoom500Top : UITop
    {
        UIRoom500_SlotCpt SlotCpt;

        public RollJackPot rollJackPot;

        private GameObject GoSound;

        public Transform TfSunGoldFlyPos;

        protected Transform TfSpecialGameWindow;
        public Transform TfSpecialGameMask;

        public Transform TfSpecialGame;
        protected Text TxtSpecialTimes;
        protected Text TxtSpecialTimes0;

        public override void Awake()
        {
            base.Awake();
            rollJackPot = transform.Find("Top/WinGold/RollJackPot").GetComponent<RollJackPot>();
            GoSound = transform.Find("GoSound").gameObject;
            TfSunGoldFlyPos = transform.Find("Bottom/Score/TfSunGoldFlyPos");
            TfSpecialGameWindow = transform.Find("TfSpecialGameWindow");
            Button BtnSpecialRoll = transform.Find("TfSpecialGameWindow/BtnSpecialRoll").GetComponent<Button>();
            BtnSpecialRoll.onClick.AddListener(CliSpecialRoll);
            TfSpecialGameMask = transform.Find("TfSpecialGameMask");
            TfSpecialGame = transform.Find("TfSpecialGame");
            TxtSpecialTimes = TfSpecialGame.transform.Find("left/TxtSpecialTimes").GetComponent<Text>();
            TxtSpecialTimes0 = TfSpecialGame.transform.Find("right/TxtSpecialTimes").GetComponent<Text>();
        }

        public override void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom500_SlotCpt;

            SlotCpt.commonTop.OnPointDown = onPointDown;
            SlotCpt.commonTop.OnRank = ClickBtnRank;
            SlotCpt.commonTop.OnHelp = ClickBtnHelp;
            SlotCpt.commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.commonTop.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.commonTop.beginSpin = BeginSlot;
            SlotCpt.commonTop.OnPointerUp = null;
            SlotCpt.commonTop.OnTolgAuto = null;
            SlotCpt.commonTop.ClickCondition = null;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BetGameRet500, BetGameRet500);// -- 500房间下注结果返回
            CoreEntry.gEventMgr.AddListener(GameEvent.Ge_BROADCAST_JACKPOT, UpdatPool500);// -- 更新奖次消息
            CoreEntry.gEventMgr.AddListener(GameEvent.Ge_BROADCAST_RefreshJACKPOT, RefreshJACKPOT);// --
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotSpecialGameEnd, OnSlotSpecialGameEnd);
        }

        public override void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game500Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi",5000000) : MainUIModel.Instance.Golds;
            if(SlotCpt.bDanJi)
                Game500Model.Instance.n64Jackpot = 3520000;
            SlotCpt.commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(2412);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }

        public bool onPointDown()
        {
            return Game500Model.Instance.toSpin.SpecialGame > 0;
        }

        public void CliSpecialRoll()
        {
            ShowSpecialGameWindow(false);
            ShowSpecialGameMask(true);
            SlotCpt.continueSpin();
        }

        public void ShowSpecialGameWindow(bool bShow)
        {
            TfSpecialGameWindow.gameObject.SetActive(bShow);
        }

        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game500Model.Instance.arrayAward.Count; i++)
            {
                if (Game500Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game500Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game500Model.Instance.nBet1 = (int)bet * 50;
        }

        public void OnSlotSpecialGameEnd(GameEvent ge, EventParameter parameter)
        {
            Game500Model.Instance.bSpecialFinish = true;
            SlotCpt.mgr.GoldEffect.Clear();
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].childCount > 1)
                    SlotCpt.mgr.GoldEffect.Add(SpineCell[i].GetChild(1));
            }

            for (int i = 0; i < SlotCpt.mgr.GoldEffect.Count; i++)
                SlotCpt.mgr.GoldEffect[i].transform.gameObject.SetActive(false);
            SlotCpt.mgr.sunGoldEffect.Clear();
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].childCount > 0)
                    SlotCpt.mgr.sunGoldEffect.Add(SpineCell[i].GetChild(0));
            }

            Canvas  canvas = SlotCpt.commonTop.GetScoreText() .gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 832;

            Canvas canvas1 = SlotCpt.commonTop.GetScoreTitle().gameObject.AddComponent<Canvas>();
            canvas1.overrideSorting = true;
            canvas1.sortingOrder = 832;

            for (int i = 0; i < SlotCpt.mgr.sunGoldEffect.Count; i++)
            {
                int tempIndex = i;

                CoreEntry.gTimeMgr.AddTimer((float)(1.1 * i), false, () =>
                {
                    CoreEntry.gAudioMgr.PlayUISound(28);
                    CommonTools.PlayArmatureAni(SlotCpt.mgr.sunGoldEffect[tempIndex].transform, "dz2", 1, () =>
                    { });

                    Text goldNum = SlotCpt.mgr.sunGoldEffect[tempIndex].transform.GetChild(0).Find("TxtSpineGold").GetComponent<Text>();
                    goldNum.transform.DOMove(TfSunGoldFlyPos.transform.position, 1).OnComplete(() =>
                    {
                        goldNum.gameObject.SetActive(false);
                        float gold;
                        float.TryParse(goldNum.text, out gold);// tonumber()
                        float currentGold;
                        float.TryParse(SlotCpt.commonTop.GetScoreText().text, out currentGold);
                        SlotCpt.commonTop.GetScoreText().text = gold + currentGold + "";
                        if (tempIndex == SlotCpt.mgr.sunGoldEffect.Count - 1)
                        {
                            CoreEntry.gTimeMgr.RemoveTimer(399);
                            SlotCpt.ResetCell();
                            for (int j = 0; j < SlotCpt.mgr.sunGoldEffect.Count; j++)
                                SlotCpt.mgr.sunGoldEffect[j].transform.gameObject.SetActive(false);
                            SlotCpt.ResetCellSpecial();
                            SlotCpt.mgr.sunGoldEffect.Clear();
                            SlotCpt.mgr.sunGoldEffectIndex.Clear();
                            SlotCpt.mgr.GoldEffect.Clear();

                            ShowSpecialGameMask(false);
                            ShowSpecialGame(false);
                            float score;
                            float.TryParse(SlotCpt.commonTop.GetScoreText().text, out score);
                            BigWinAni(()=> 
                            {
                                Destroy(SlotCpt.commonTop.GetScoreText().gameObject.GetComponent<Canvas>());
                                Destroy(SlotCpt.commonTop.GetScoreTitle().gameObject.GetComponent<Canvas>());


                                SlotCpt.continueSpin();
                                CoreEntry.gAudioMgr.PlayUIMusic(13);
                            }, false, (int)(score*ToolUtil.GetGoldRadio()));
                        }
                        CoreEntry.gTimeMgr.RemoveTimer(i + 10086);
                    });


                }, i + 10086);
            }

        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom500Help");
        }

        public override void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game500Model.Instance.GearList,Game500Model.Instance.nBet);
  
            SlotCpt.setState(slotState.Idle);
            if (Game500Model.Instance.nModelGame > 0)
            {

                if (Game500Model.Instance.nFreeGame > 0)
                {
                    SlotCpt.commonTop.SetFreeTimes(true, Game500Model.Instance.toSpin.FreeTimes + "");
                    Game500Model.Instance.toSpin.nModelGame = Game500Model.Instance.nModelGame;
                    SlotCpt.freeTimes.max = Game500Model.Instance.toSpin.nModelGame;
                    Game500Model.Instance.toSpin.FreeTimes = Game500Model.Instance.nFreeGame;
                    SlotCpt.commonTop.UpDateScore(Game500Model.Instance.n64FreeGold);
                    CoreEntry.gTimeMgr.AddTimer(1, false, () => 
                    {
                        if (SlotCpt.StateSlot == slotState.Idle)
                            SlotCpt.continueSpin();
                    }, 24121);
                }
                else if (Game500Model.Instance.nSunGame > 0)
                {
                    Game500Model.Instance.toSpin.nModelGame = Game500Model.Instance.nModelGame;

                    Game500Model.Instance.toSpin.SpecialGame = Game500Model.Instance.nSunGame;
                    Game500Model.Instance.bInSpecialGame = true;
                    SlotCpt.mgr.sunGoldEffectIndex.Clear();
                    for (int i = 0; i < 15; i++)
                    {
                        if (Game500Model.Instance.arraySunarrayLogo[i] == SlotData_500.specialelement)
                        {
                            if (SlotCpt.mgr.bContainElement(i) == false)
                            {
                                SlotCpt.mgr.sunGoldEffectIndex.Add(i);
                                int sunValue = Game500Model.Instance.arraySun[i];
                                float temp2 = Game500Model.Instance.sunGoldLValue_1[sunValue - 1] * SlotCpt.commonTop.Bet * 50 ;
                                SlotCpt.ShowOneCellLine(i / 3, i % 3, null, SlotData_500.specialelement, (int)temp2, true);
                            }
                        }
                    }
                    ShowSpecialGame(true);
                    SetSpecialTimes(Game500Model.Instance.toSpin.SpecialGame);
                    ShowSpecialGameMask(true);

                    CoreEntry.gTimeMgr.AddTimer(1.5f, false, () => 
                    {
                        if (SlotCpt.StateSlot == slotState.Idle)
                            SlotCpt.continueSpin(); 
                    }, 24121);
                }
            }
            rollJackPot.SetNum(Game500Model.Instance.n64Jackpot);
            SetTopRank();
        }


        public override void clkEndSpin()
        {
            SlotCpt.endSpin();
            GoAutoSpinNum.SetActive(false);
        }


        public void BeginSlot(int num,bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }

        public override void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            SlotCpt.commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            CoreEntry.gTimeMgr.AddTimer(1.7f, false, () =>
            {
                SlotCpt.continueSpin();
            }, 100000);
        }

        public void BetGameRet500(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void UpdatPool500(GameEvent ge, EventParameter parameter)
        {
            rollJackPot.RollNum(Game500Model.Instance.n64Jackpot);// ( / 10000).ToString("f2"));
        }
        public virtual void RefreshJACKPOT(GameEvent ge, EventParameter parameter)
        {
            SetTopRank();
        }


        public override void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold = Game500Model.Instance.toSpin.WinGold;
            if (Game500Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game500Model.Instance.toSpin.n64FreeGold;

            if (Game500Model.Instance.bSpecialFinish)
                return;

            if((Game500Model.Instance.toSpin.rate <=2&& Game500Model.Instance.toSpin.n64FreeGold <=0) || SlotCpt.isFreeSpin)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                CoreEntry.gAudioMgr.PlayUISound(Game500Model.Instance.toSpin.rate<=1?20:21);
                CoreEntry.gTimeMgr.AddTimer(0.2f,false,()=> 
                {
                    SlotCpt.commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
                },7);             
            }

            if (Game500Model.Instance.toSpin.n64FreeGold <= 0)
                BigWinAni(null, false);
        }

        public override void BigWinAni(Action callBack, bool freeEnd, int bSpecialNum = 0)
        {
            if (Game500Model.Instance.toSpin.rate > 2 || freeEnd || Game500Model.Instance.toSpin.n64RSPowerGold > 0 || bSpecialNum > 0)
            {
                Effect.SetActive(true);
                gold = 0;
                if (freeEnd)
                {
                    gold = Game500Model.Instance.toSpin.n64FreeGold;
                    Game500Model.Instance.toSpin.rate = gold / (float)Game500Model.Instance.nBet1;
                }                   
                else if (bSpecialNum > 0)
                {
                    gold = bSpecialNum;
                    Game500Model.Instance.toSpin.rate = gold / (float)Game500Model.Instance.nBet1;
                }
                else
                    gold = Game500Model.Instance.toSpin.WinGold;

                winGold = gold;
                playGoldType = 0;
                if (Game500Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game500Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;

                }
                else
                {
                    int soundID = 0;
               
                    if(Game500Model.Instance.toSpin.rate > 2 && Game500Model.Instance.toSpin.rate <=4)
                    {
                        soundID = 21;
                        playGoldType = 1;
                    }
                    else if(Game500Model.Instance.toSpin.rate > 4 &&  Game500Model.Instance.toSpin.rate <= 12)
                    {
                        soundID = 15;
                        playGoldType = 2;
                    }
                    else if (Game500Model.Instance.toSpin.rate <= 2)//免费可以中
                    {
                        soundID = 21;
                        playGoldType = 1;

                        if (callBack != null)
                            callBack();
                        ClickBtnCloseEffect();
                        return;
                    }
                    else
                    {
                        soundID = 14;
                        playGoldType = 3;
                      
                    }
                    CoreEntry.gAudioMgr.PlayUISound(soundID);
                    
                }
                CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                {
                    m_Gold_EffectNew.setData(playGoldType, gold, () =>
                    {
                        if (callBack != null)
                            callBack();
                        ClickBtnCloseEffect();
                    }, SlotCpt.autoSpinNum != 0,GoSound);
                }, 17);

            }
        }

        public void SetSpecialTimes(int num)
        {
            TxtSpecialTimes.text = num + "";
            TxtSpecialTimes0.text = num + "";
        }

        public void ShowSpecialGameMask(bool bShow)
        {
            TfSpecialGameMask.gameObject.SetActive(bShow);
        }
        public virtual void ShowSpecialGame(bool bShow)
        {
            TfSpecialGame.gameObject.SetActive(bShow);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BetGameRet500, BetGameRet500);// -- 500房间下注结果返回
            CoreEntry.gEventMgr.RemoveListener(GameEvent.Ge_BROADCAST_JACKPOT, UpdatPool500);// -- 更新奖次消息
            CoreEntry.gEventMgr.RemoveListener(GameEvent.Ge_BROADCAST_RefreshJACKPOT, RefreshJACKPOT);// --

            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotSpecialGameEnd, OnSlotSpecialGameEnd);
        }


        protected override void SetTopRank()
        {
 
            if (Game500Model.Instance.arrayAward.Count >= 1)
            {
                if (Game500Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game500Model.Instance.arrayAward[0].szName), Game500Model.Instance.arrayAward[0].n64Gold, Game500Model.Instance.arrayAward[0].nIconID);
            }
        }

        public override void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            Game500Model.Instance.bShowFreeAni = true;
            CoreEntry.gAudioMgr.PlayUISound(18);
            SlotCpt.commonTop.SetFreeTimes(true, Game500Model.Instance.toSpin.FreeTimes + "");
            TfFree.gameObject.SetActive(true);
            TxtFreeTimes.text = Game500Model.Instance.toSpin.nModelGame - Game500Model.Instance.lastCount + "";
            freeAni = Free.DOScale(new Vector3(08f, 0.8f, 1), 0.9f).OnComplete(() =>
            {
                freeAni = Free.DOScale(new Vector3(0.7f, 0.7f, 1), 0.9f).OnComplete(() => 
                {
                    TfFree.gameObject.SetActive(false);
                    CoreEntry.gAudioMgr.PlayUIMusic(16);
                });
            });
        }

        public override void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            TfFree.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(false,"");
            if (Game500Model.Instance.toSpin.n64FreeGold > 0)
                BigWinAni(null, true, 0);

            CoreEntry.gAudioMgr.PlayUIMusic(13);
            CoreEntry.gAudioMgr.PlayUISound(17);
        }

        public void HideEffect()
        {
            for (int i = 0; i < SpineCell.Count; i++)
            {
                if (SpineCell[i].transform.childCount > 0)
                {
                    for (int j = 0; j < SpineCell[i].transform.childCount; j++)
                        SpineCell[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }

        public override void ReloadGame()
        {    
            base.ReloadGame();
            if ( SlotCpt.gameStatus == 1)
            {
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
            }
        }

        public override void UpdateGold(long gold)
        {
            Game500Model.Instance.toSpin.n64Gold = gold;
            base.UpdateGold(gold);
            SlotCpt.commonTop.UpdateGold(gold);
        }
    }
}
