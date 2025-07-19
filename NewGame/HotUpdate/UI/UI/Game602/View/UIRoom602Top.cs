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
    public class UIRoom602Top : UITop
    {

        protected List<Transform> LineList = new List<Transform>();

        private UIRoom602_SlotCpt SlotCpt;

        RollJackPot rollJackPot1;
        private Button closeFreeBtn;
        public RectTransform Line;
        public override void Awake()
        {
            base.Awake();
            rollJackPot1 = transform.Find("BigWin/JackPoolMinor").GetComponent<RollJackPot>();
            Line = transform.Find("Lines").GetComponent<RectTransform>();
            Transform closeFreeNode = transform.Find("TfFree/Free/closeFree");
            if (closeFreeNode) closeFreeBtn = closeFreeNode.GetComponent<Button>();
            for (int i = 0; i < 9; i++)
                LineList.Add(Line.transform.GetChild(i));
        }

        public override void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom602_SlotCpt;
            SlotCpt.CommonTop602.OnRank = ClickBtnRank;
            SlotCpt.CommonTop602.OnHelp = ClickBtnHelp;
            SlotCpt.CommonTop602.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.CommonTop602.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.CommonTop602.beginSpin = BeginSlot;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener(MessageName.GE_BetGameRet602, BetGameRet602);// 602房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_Jackpot602, UpdatPool602);// -- 更新奖次消息
            Message.AddListener(MessageName.GE_BroadCast_JackpotList602, RefreshJACKPOT602);
            if (closeFreeBtn) closeFreeBtn.onClick.AddListener(OnClickCloseFree);

            HideAllLines(false, null);
        }

        public override void InitData()
        {
            SlotCpt.CommonTop602.UpDateScore(0);
            Game602Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 5000000) : MainUIModel.Instance.Golds;
            if(SlotCpt.bDanJi)
                Game602Model.Instance.n64Jackpot = 6850000;
            //rollJackPot1.RollNum(Game602Model.Instance.JackPotList[betID][0] * jackPot / 100, true);
            //rollJackPot2.RollNum(Game602Model.Instance.JackPotList[betID][1] * jackPot / 100, true);
            //rollJackPot3.RollNum(Game602Model.Instance.JackPotList[betID][2] * jackPot / 100, true);

            SlotCpt.CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold);

            CoreEntry.gTimeMgr.RemoveTimer(2412);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game602Model.Instance.arrayAward.Count; i++)
            {
                if (Game602Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game602Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

      

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom602Help");
        }

        public override void ClickBtnCloseEffect()
        {
            SlotCpt.CommonTop602.UpDateScore(winGold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            SlotCpt.CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold);

            CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            {
                //Effect.SetActive(false);
                SlotCpt.continueSpin();
            }, 100000);

            //CoreEntry.gTimeMgr.AddTimer(1.2f, false, () =>
            //{
            //Effect.SetActive(false);
      
            //}, 100000);
        }

        public void SetTxtScore(long score)
        {
           // Debug.LogError("-------------"+ SlotCpt.isFreeSpin +"======"+ Game602Model.Instance.toSpin.n64FreeGold);
            if (SlotCpt.isFreeSpin && Game602Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.CommonTop602.UpDateScore(score);
        }

        public void ShowOneLine(int index)
        {
            LineList[index -1].gameObject.SetActive(true);
        }

        public void HideAllLines(bool bHide,List<KeyValuePair<int,int>> lines)
        {
            if (bHide == false)
            {
                for (int i = 0; i < 9; i++)
                    LineList[i].gameObject.SetActive(bHide);
            }
            else
                for (int i = 0; i < lines.Count; i++)
                    LineList[lines[i].Key -1].gameObject.SetActive(bHide);
        }

        public override void InitGame()
        {
            SlotCpt.CommonTop602.InitGame(Game602Model.Instance.GearList, Game602Model.Instance.nBet);
            SetJackPot();
            SlotCpt.setState(slotState.Idle);
            if (Game602Model.Instance.nModelGame > 0)
            {
                if(Game602Model.Instance.nFreeGame > 0)
                {
                    SlotCpt.CommonTop602.SetFreeTimes(true, Game602Model.Instance.toSpin.FreeTimes + "");
                    Game602Model.Instance.toSpin.nModelGame = Game602Model.Instance.nModelGame;
                    SlotCpt.freeTimes.max = Game602Model.Instance.toSpin.nModelGame;
                    Game602Model.Instance.toSpin.FreeTimes = Game602Model.Instance.nFreeGame;
                    SlotCpt.CommonTop602.UpDateScore(Game602Model.Instance.n64FreeGold);
                    CoreEntry.gTimeMgr.AddTimer(1, false, () => 
                    {
                        if (SlotCpt.StateSlot == slotState.Idle)
                            SlotCpt.continueSpin(); 
                    }, 24121);
                }
            }
            SetTopRank();
        }

        public void SetJackPot()
        {
            long jackPot = Game602Model.Instance.n64Jackpot;

            rollJackPot1.SetNum(jackPot, true);
        }

        protected override void SetTopRank()
        {
           if(Game602Model.Instance.arrayAward.Count >= 1)
            {
                if (Game602Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.CommonTop602.SetTopRank(CommonTools.BytesToString(Game602Model.Instance.arrayAward[0].szName), Game602Model.Instance.arrayAward[0].n64Gold, Game602Model.Instance.arrayAward[0].nIconID);
            }
        }


        public override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener(MessageName.GE_BetGameRet602, BetGameRet602);// -- 602房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_Jackpot602, UpdatPool602);// -- 602房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList602, RefreshJACKPOT602);// -- 602房间下注结果返回
            if (closeFreeBtn) closeFreeBtn.onClick.RemoveListener(OnClickCloseFree);
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

        slotState m_curSlotState;

        public override void OnSlotState(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gTimeMgr.RemoveTimer(250);
            slotState state = (slotState)parameter.intParameter;
            m_curSlotState = state;
            if (state == slotState.Idle)
            {
                SlotCpt.BtnRollBar.interactable = true;
                return;
            }
        }

        public override void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold= Game602Model.Instance.toSpin.WinGold;
            CoreEntry.gAudioMgr.PlayUISound(293,transform.GetChild(2).gameObject);
            if (Game602Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game602Model.Instance.toSpin.n64FreeGold;
 

            if ((Game602Model.Instance.toSpin.rate <= 2 && Game602Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                SlotCpt.CommonTop602.UpDateScore(winGold);
                CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                {
                    SlotCpt.CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold);
                }, 7);
                CoreEntry.gAudioMgr.PlayUISound(292);
            }

            if (Game602Model.Instance.toSpin.n64FreeGold <= 0)
                BigWinAni(null, false);
        }

        public override void BigWinAni(Action callBack,bool freeEnd, int bSpecialNum = 0)
        {
            if(Game602Model.Instance.toSpin.rate > 2 || freeEnd || Game602Model.Instance.toSpin.n64RSPowerGold > 0)
            {
                //if (SlotCpt.isFreeSpin && !freeEnd)
                //    return;
                Effect.SetActive(true);
                gold = 0;
                if (freeEnd)
                {
                    gold = Game602Model.Instance.toSpin.n64FreeGold;
                    Game602Model.Instance.toSpin.rate = gold / Game602Model.Instance.nBet1;
                    winGold = this.gold;
                }
                else if (bSpecialNum > 0)
                    gold = bSpecialNum;
                else
                    gold = Game602Model.Instance.toSpin.WinGold;

                playGoldType = 0;
                if (Game602Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game602Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {
                    if (Game602Model.Instance.toSpin.rate > 2 && Game602Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game602Model.Instance.toSpin.rate > 4 && Game602Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else if (Game602Model.Instance.toSpin.rate <= 2)//免费可以中
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

                //playGoldType = 4;
                //s playGoldType = 4;
                m_Gold_EffectNew.setData(playGoldType, gold, () =>
                {
                    if (callBack != null)
                        callBack();
                    ClickBtnCloseEffect();
                }, SlotCpt.autoSpinNum != 0,null,null,602);

            }
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game602Model.Instance.nBet1 = (int)bet * 9;
            SetJackPot();
        }

        private void OnClickCloseFree()
        {
            if (TfFree) TfFree.gameObject.SetActive(false);
        }

        public override void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.CommonTop602.SetFreeTimes(true, Game602Model.Instance.toSpin.FreeTimes + "");
            TfFree.gameObject.SetActive(true);
            /*ToolUtil.PlayAnimation(Spine_Free,"animation1",false,()=> 
            {
                ToolUtil.PlayAnimation(Spine_Free, "animation2",false,()=> 
                {
                    TfFree.gameObject.SetActive(false);
                });
            });*/
            TxtFreeTimes.text = "x" + (Game602Model.Instance.toSpin.nModelGame - Game602Model.Instance.lastCount);
            Transform freeEffect = TfFree.Find("Free/TxtFreeTimesEffect");
            if (freeEffect) freeEffect.GetComponent<Text>().text = TxtFreeTimes.text;
            Game602Model.Instance.bShowFreeAni = true;

            CoreEntry.gAudioMgr.PlayUISound(290);

            if (SlotCpt.autoSpinNum > 0)
            {
                var seq = DOTween.Sequence();
                seq.AppendInterval(1.5f)
                    .AppendCallback(() => 
                    {
                        TfFree.gameObject.SetActive(false);
                    });
            }

            //freeAni = Free.DOScale(new Vector3(1.2f, 1.2f, 1), 0.75f).OnComplete(() => 
            //{
            //    freeAni = Free.DOScale(new Vector3(1, 1, 1), 0.75f).OnComplete(() => { TfFree.gameObject.SetActive(false); });
            //});
        }



        public override void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            TfFree.gameObject.SetActive(false);
            SlotCpt.CommonTop602.SetFreeTimes(false);
            if (Game602Model.Instance.toSpin.n64FreeGold > 0)
                BigWinAni(null, true, 0);
            SetBtnsScale(true);
        }

        public void BetGameRet602()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void UpdatPool602()
        {
            long jackPot = Game602Model.Instance.n64Jackpot;

            rollJackPot1.RollNum(jackPot, true);
        }

        public void SetBtnsScale(bool bOne)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (bOne == false)
                scale = new Vector3(0, 0, 0);
            SlotCpt.CommonTop602.GetBeginBtn().transform.localScale = scale;
        }

        public void RefreshJACKPOT602()
        {
            SetTopRank();
        }

        public override void ReloadGame()
        {
            base.ReloadGame();

            if (SlotCpt.gameStatus == 1)
            {
               // SlotCpt.continueSpin();
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
                SlotCpt.PlayTopLight();
            }
            else
            {
        
            }
        }

        public override void UpdateGold(long gold)
        {
            Game602Model.Instance.toSpin.n64Gold = gold;
            base.UpdateGold(gold);
            SlotCpt.CommonTop602.UpdateGold(gold);
        }
    }
}
