using DG.Tweening;
using SEZSJ;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRoom601Top : UITop
    {

        protected List<Transform> LineList = new List<Transform>();

        private UIRoom601_SlotCpt SlotCpt;

        RollJackPot rollJackPot;

        private Text TxtScore_1;


        public int betID = 0;//--挡位
        public override void Awake()
        {
            base.Awake();
            rollJackPot = transform.Find("BigWin/JackPot").GetComponent<RollJackPot>();
            for (int i = 0; i < 9; i++)
                LineList.Add(transform.Find("Lines").GetChild(i));
            TxtScore_1 = transform.Find("Bottom/Score/TfCanvas/TxtScore_1").GetComponent<Text>();
     
        }


        public override void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom601_SlotCpt;
            SlotCpt.commonTop.OnRank = ClickBtnRank;
            SlotCpt.commonTop.OnHelp = ClickBtnHelp;
            SlotCpt.commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.commonTop.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.commonTop.beginSpin = BeginSlot;
            SlotCpt.commonTop.OnPointerUp = null;
            SlotCpt.commonTop.OnTolgAuto = null;// ClickAuto;
            SlotCpt.commonTop.ClickCondition = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet600, BetGameRet601);// 601房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_Jackpot600, UpdatPool601);// -- 更新奖次消息
            Message.AddListener(MessageName.GE_BroadCast_JackpotList600, RefreshJACKPOT601);

            HideAllLines(false, null);
        }

        public override void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game601Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            if (SlotCpt.bDanJi)
                Game601Model.Instance.n64Jackpot = 6850000;
            TxtScore_1.text ="0";
            SlotCpt.commonTop.UpdateGold(Game601Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(2412);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }

    


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game601Model.Instance.arrayAward.Count; i++)
            {
                if (Game601Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game601Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom601Help");
        }

        public override void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            TxtScore_1.text = ToolUtil.ShowF2Num2(winGold);// (gold/10000f).ToString("f2");
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            SlotCpt.commonTop.UpdateGold(Game601Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            {
                SlotCpt.continueSpin();
            }, 100000);
        }

        public void SetTxtScore(long score)
        {
            // Debug.LogError("-------------"+ SlotCpt.isFreeSpin +"======"+ Game601Model.Instance.toSpin.n64FreeGold);
            if (SlotCpt.isFreeSpin && Game601Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.commonTop.UpDateScore(score);
            TxtScore_1.text = ToolUtil.ShowF2Num2(score);// (score/10000f).ToString();
        }

        public void ShowOneLine(int index)
        {
            LineList[index - 1].gameObject.SetActive(true);
        }

        public void HideAllLines(bool bHide, List<KeyValuePair<int, int>> lines)
        {
            if (bHide == false)
            {
                for (int i = 0; i < 9; i++)
                    LineList[i].gameObject.SetActive(bHide);
            }
            else
                for (int i = 0; i < lines.Count; i++)
                    LineList[lines[i].Key - 1].gameObject.SetActive(bHide);
        }

        public override void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game601Model.Instance.GearList, Game601Model.Instance.nBet);
            SetJackPot();
            SlotCpt.setState(slotState.Idle);
            if (Game601Model.Instance.nModelGame > 0)
            {
                if (Game601Model.Instance.nFreeGame > 0)
                {
                    SlotCpt.commonTop.SetFreeTimes(true, Game601Model.Instance.toSpin.FreeTimes + "");
                    Game601Model.Instance.toSpin.nModelGame = Game601Model.Instance.nModelGame;
                    SlotCpt.freeTimes.max = Game601Model.Instance.toSpin.nModelGame;
                    Game601Model.Instance.toSpin.FreeTimes = Game601Model.Instance.nFreeGame;
                    SlotCpt.commonTop.UpDateScore(Game601Model.Instance.n64FreeGold);
                    TxtScore_1.text = ToolUtil.ShowF2Num2(Game601Model.Instance.n64FreeGold);// (Game601Model.Instance.n64FreeGold/10000f).ToString("f2");
                    CoreEntry.gTimeMgr.AddTimer(1, false, () => 
                    {
                        if(SlotCpt.StateSlot == slotState.Idle)
                            SlotCpt.continueSpin(); 
                    }, 24121);
                }
            }
            SetTopRank();


        }

        public void SetJackPot()
        {
            long jackPot = Game601Model.Instance.n64Jackpot;

            rollJackPot.SetNum(jackPot, true);
        }

        protected override void SetTopRank()
        {
            if (Game601Model.Instance.arrayAward.Count >= 1)
            {
                if (Game601Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game601Model.Instance.arrayAward[0].szName), Game601Model.Instance.arrayAward[0].n64Gold, Game601Model.Instance.arrayAward[0].nIconID);
            }
        }


        public override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener(MessageName.GE_BetGameRet600, BetGameRet601);// -- 601房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_Jackpot600, UpdatPool601);// -- 601房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList600, RefreshJACKPOT601);// -- 601房间下注结果返回
        }


        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }



        public override void clkEndSpin()
        {
            SlotCpt.endSpin();
            GoAutoSpinNum.SetActive(false);
            SetBtnsScale(true);
        }

        public override void OnSlotState(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gTimeMgr.RemoveTimer(250);
            slotState state = (slotState)parameter.intParameter;
            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                    return;
                SlotCpt.BtnRollBar.interactable = true;
                return;
            }
        }

        public override void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold = Game601Model.Instance.toSpin.WinGold;
            if (Game601Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game601Model.Instance.toSpin.n64FreeGold;
            if ((Game601Model.Instance.toSpin.rate <= 2 && Game601Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                TxtScore_1.text = ToolUtil.ShowF2Num2(winGold);// (gold/10000f).ToString("f2");
                CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
                {
                    SlotCpt.commonTop.UpdateGold(Game601Model.Instance.toSpin.n64Gold);
                }, 7);
                CoreEntry.gAudioMgr.PlayUISound(243);
            }

            if (Game601Model.Instance.toSpin.n64FreeGold <= 0)
            {
                CoreEntry.gTimeMgr.AddTimer(0.7f,false,()=> 
                {
                    BigWinAni(null, false);
                },333);
            }
          
        }

        public override void BigWinAni(Action callBack, bool freeEnd, int bSpecialNum = 0)
        {
            if (Game601Model.Instance.toSpin.rate > 2 || freeEnd || Game601Model.Instance.toSpin.n64RSPowerGold > 0)
            {
                //if (SlotCpt.isFreeSpin && !freeEnd)
                //    return;
                Effect.SetActive(true);
                gold = 0;
                if (freeEnd)
                {
                    gold = Game601Model.Instance.toSpin.n64FreeGold;
                    Game601Model.Instance.toSpin.rate = gold / Game601Model.Instance.nBet1;
                }
                else if (bSpecialNum > 0)
                    gold = bSpecialNum;
                else
                    gold = Game601Model.Instance.toSpin.WinGold;

                playGoldType = 0;
                if (Game601Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game601Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {
                    if (Game601Model.Instance.toSpin.rate > 2 && Game601Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game601Model.Instance.toSpin.rate > 4 && Game601Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else if (Game601Model.Instance.toSpin.rate <= 2)//免费可以中
                    {
                        playGoldType = 1;
                        if (callBack != null)
                            callBack();
                        ClickBtnCloseEffect();
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
                }, SlotCpt.autoSpinNum != 0);

            }
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game601Model.Instance.nBet1 = (int)bet * 9; ;
            SetJackPot();
        }

        public override void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.commonTop.SetFreeTimes(true, Game601Model.Instance.toSpin.FreeTimes + "");
            TfFree.gameObject.SetActive(true);
            TxtFreeTimes.text = Game601Model.Instance.toSpin.nModelGame - Game601Model.Instance.lastCount + "";
            Game601Model.Instance.bShowFreeAni = true;

            CoreEntry.gAudioMgr.PlayUISound(139);

            freeAni = Free.DOScale(new Vector3(1.2f, 1.2f, 1), 0.75f).OnComplete(() =>
            {
                freeAni = Free.DOScale(new Vector3(1, 1, 1), 0.75f).OnComplete(() => { TfFree.gameObject.SetActive(false); });
            });
        }

        public override void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            TfFree.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(false);
            if (Game601Model.Instance.toSpin.n64FreeGold > 0)
                BigWinAni(null, true, 0);
            SetBtnsScale(true);
        }

        public void BetGameRet601()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
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

        public void UpdatPool601()
        {
            long jackPot = Game601Model.Instance.n64Jackpot;

            rollJackPot.RollNum(jackPot, true);
        }

        public void SetBtnsScale(bool bOne)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (bOne == false)
                scale = new Vector3(0, 0, 0);
            SlotCpt.commonTop.GetBeginBtn().transform.localScale = scale;
        }

        public void RefreshJACKPOT601()
        {
            SetTopRank();
        }

        public override void ReloadGame()
        {
            base.ReloadGame();
            if (SlotCpt.gameStatus == 1)
            {
                HideAllLines(false, null);
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
                SlotCpt.PlayTopLight();
            }
        }
        public override void UpdateGold(long gold)
        {
            Game601Model.Instance.toSpin.n64Gold = gold;
            base.UpdateGold(gold);
            SlotCpt.commonTop.UpdateGold(gold);
        }
    }
}
