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
    public class UIRoom800Top : UITop
    {
        private UIRoom800_SlotCpt SlotCpt;

        RollJackPot rollJackPot;

        public int betID = 0;//--挡位



        public GameObject GoSound;
        public GameObject GoSound2;
        public override void Awake()
        {
            base.Awake();
            rollJackPot = transform.Find("Top/WinGold/Roll_JackPot").GetComponent<RollJackPot>();
            GoSound = transform.Find("GoSound").gameObject;
            GoSound2 = transform.Find("GoSound2").gameObject;
        }

        public void onPointerUp(bool isOn)
        {
            //if (!isOn)
            //{
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(100000);
                    SlotCpt.commonTop.BeginSlot(0);
                }
            //}
        }

        private void OnTolAuto(bool isOn)
        {
            if (isOn)
            {
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(100000);
                    SlotCpt.commonTop.BeginSlot(-1);
                }
            }
            else
            {
                SlotCpt.autoSpinNum = 0;
                if (SlotCpt.StateSlot == slotState.SpinSuccess)
                    SlotCpt.endSpin();
            }
        }

        public override void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom800_SlotCpt;
            SlotCpt.commonTop.OnRank = ClickBtnRank;
            SlotCpt.commonTop.OnHelp = ClickBtnHelp;
            SlotCpt.commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.commonTop.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.commonTop.beginSpin = BeginSlot;
            SlotCpt.commonTop.OnPointerUp = onPointerUp;
            SlotCpt.commonTop.OnTolgAuto = null;
            SlotCpt.commonTop.ClickCondition = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.BetGameRet800, BetGameRet800);// 800房间下注结果返回
            Message.AddListener(MessageName.BroadCast_Jackpot80, UpdatPool800);//-- 更新奖次消息
            Message.AddListener(MessageName.BroadCast_JackpotList800, RefreshJACKPOT800);//
            Game800Model.Instance.toSpin.n64Gold = MainUIModel.Instance.Golds;
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }

        public override void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game800Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 5000000) : MainUIModel.Instance.Golds;
            if (SlotCpt.bDanJi)
                Game800Model.Instance.n64Jackpot = 6850000;
            SlotCpt.commonTop.UpdateGold(Game800Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(2412);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }



        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game800Model.Instance.arrayAward.Count; i++)
            {
                if (Game800Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game800Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);

        }


        public override void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            SlotCpt.commonTop.UpdateGold(Game800Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            //Effect.SetActive(false);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            {
                SlotCpt.continueSpin();
            }, 100000);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom800Help");
        }

        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin)
                return;
            SlotCpt.commonTop.UpDateScore(score);
        }



        public override void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game800Model.Instance.GearList, Game800Model.Instance.nBet);
            rollJackPot.SetNum(Game800Model.Instance.n64Jackpot);
            SlotCpt.setState(slotState.Idle);
            if (Game800Model.Instance.nModelGame > 0)
            {
                if(Game800Model.Instance.nFreeGame > 0)
                {
                    SlotCpt.commonTop.SetFreeTimes(true, Game800Model.Instance.toSpin.FreeTimes+"");
                    Game800Model.Instance.toSpin.nModelGame = Game800Model.Instance.nModelGame;
                    SlotCpt.freeTimes.max = Game800Model.Instance.toSpin.nModelGame;
                    Game800Model.Instance.toSpin.FreeTimes = Game800Model.Instance.nFreeGame;
                    SlotCpt.commonTop.UpDateScore(Game800Model.Instance.n64FreeGold);
                    CoreEntry.gTimeMgr.AddTimer(1, false, () => { SlotCpt.continueSpin(); }, 24121);
                }
            }
            SetTopRank();
        }



        protected override void SetTopRank()
        {
            if (Game800Model.Instance.arrayAward.Count >= 1)
            {
                if (Game800Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game800Model.Instance.arrayAward[0].szName), Game800Model.Instance.arrayAward[0].n64Gold, Game800Model.Instance.arrayAward[0].nIconID);
            }

        }


        public override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener(MessageName.BetGameRet800, BetGameRet800);// 800房间下注结果返回
            Message.RemoveListener(MessageName.BroadCast_Jackpot80, UpdatPool800);//-- 更新奖次消息
            Message.RemoveListener(MessageName.BroadCast_JackpotList800, RefreshJACKPOT800);//

            //CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_CONNECTINFO, GE_CONNECTINFO);// -- 连接阶段信息 6：开始连接 8：成功
        }


        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }

        public override void clkEndSpin()
        {
            SlotCpt.endSpin();
            GoAutoSpinNum.SetActive(false);
           // SetBtnsScale(true);
        }

        public override void OnSlotState(GameEvent ge, EventParameter parameter)
        {
            slotState state = SlotCpt.StateSlot;
            if (state == slotState.Idle)
                SlotCpt.BtnRollBar.interactable = true;
        }

        public override void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold = Game800Model.Instance.toSpin.WinGold;
            if (Game800Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game800Model.Instance.toSpin.n64FreeGold;
            if ((Game800Model.Instance.toSpin.rate <= 2 && Game800Model.Instance.toSpin.n64FreeGold <=0) || SlotCpt.isFreeSpin)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
                {
                    SlotCpt.commonTop.UpdateGold(Game800Model.Instance.toSpin.n64Gold);
                }, 7);
                //CoreEntry.gAudioMgr.PlayUISound(20);
            }
           
           
            if (Game800Model.Instance.bFreeGameFinished)
            {
                BigWinAni(null, true);
            }
            else if (!SlotCpt.isFreeSpin || Game800Model.Instance.toSpin.n64RSPowerGold > 0)
                BigWinAni(null, false);
        }

        public override void BigWinAni(Action callBack,bool freeEnd, int bSpecialNum = 0)
        {
            if(Game800Model.Instance.toSpin.rate > 2 || freeEnd || Game800Model.Instance.toSpin.n64RSPowerGold > 0)
            {
                //if (SlotCpt.isFreeSpin && !freeEnd)
                //    return;

                Effect.SetActive(true);
                gold = 0;
                if (Game800Model.Instance.toSpin.rate <=0)
                {

                    gold = Game800Model.Instance.toSpin.WinGold;
                    Game800Model.Instance.toSpin.rate = gold / (float)Game800Model.Instance.nBet1;
                }                   
                else if (bSpecialNum > 0)
                    gold = bSpecialNum;
                else
                    gold = Game800Model.Instance.toSpin.WinGold;

                playGoldType = 0;
                if (Game800Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game800Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {
                    if (Game800Model.Instance.toSpin.rate > 2 && Game800Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game800Model.Instance.toSpin.rate > 4 && Game800Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else if (Game800Model.Instance.toSpin.rate <= 2)//免费可以中
                    {
                        if (callBack != null)
                            callBack();
                        ClickBtnCloseEffect();
                        playGoldType = 1;
                        return;
                    }
                    else
                        playGoldType = 3;
                }
                m_Gold_EffectNew.setData(playGoldType, gold, () =>
                {
                    if (callBack != null)
                        callBack();
                    ClickBtnCloseEffect();
                }, SlotCpt.IsAutoSpin());
            }
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game800Model.Instance.nBet1 = (int)bet;

        }

        public override void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            TfFree.gameObject.SetActive(true);
        }

        public override void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.SpineAniKuang.gameObject.SetActive(false);

            TfFree.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(false);
            Game800Model.Instance. bFreeGameFinished = true;
            SlotCpt.SpineAniKuang.gameObject.SetActive(false);
            SlotCpt.SpineAniKuang2.gameObject.SetActive(false);
        }

     

        public void BetGameRet800()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }


        public void UpdatPool800()
        {
            rollJackPot.RollNum(Game800Model.Instance.n64Jackpot);// (Game800Model.Instance.n64Jackpot / 10000).ToString("f2"));
        }

    

        public void RefreshJACKPOT800()
        {
            SetTopRank();
        }

        public override void ReloadGame()
        {
            if (SlotCpt.gameStatus == 1)
            {
                //SlotCpt.continueSpin();
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
            }
            else
            {
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
            }
        }

        public override void UpdateGold(long gold)
        {
            Game800Model.Instance.toSpin.n64Gold = gold;
            base.UpdateGold(gold);
            SlotCpt.commonTop.UpdateGold(gold);
        }
    }
}
