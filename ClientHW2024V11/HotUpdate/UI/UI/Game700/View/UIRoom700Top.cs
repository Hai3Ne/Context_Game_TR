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
    public class UIRoom700Top : UITop
    {
        RollJackPot rollJackPot1;
        RollJackPot rollJackPot2;
        RollJackPot rollJackPot3;

        protected List<Transform> LineList = new List<Transform>();
        public List<Transform> lineGold = new List<Transform>();
        public List<Text> lineGoldText = new List<Text>();


        private Scrollbar Scrollbar;
        private Button BtnBomb1;
        private Button BtnBomb2;
        private Button BtnBomb3;
        private TextMeshProUGUI TxtBomb1;
        private TextMeshProUGUI TxtBomb2;
        private TextMeshProUGUI TxtBomb3;
        private List<TextMeshProUGUI> BombTxtList = new List<TextMeshProUGUI>();
        private List<Transform> BombEffect = new List<Transform>();
        private List<Transform> BombImage = new List<Transform>();
        List<Button> buttonList = new List<Button>();


        private TextMeshProUGUI TextCurrent;


        private UIRoom700_SlotCpt SlotCpt;
        public List<Transform> ElementCellBg = new List<Transform>();

        private Button Btn_Rank;
        private Button Btn_Tour;
        private Button m_Btn_TourRank;

        public int betID = 0;//--挡位
        public override void Awake()
        {
            base.Awake();

            rollJackPot1 = transform.Find("BigWin/JackPoolMinor").GetComponent<RollJackPot>();
            rollJackPot2 = transform.Find("BigWin/JackPoolMajor").GetComponent<RollJackPot>();
            rollJackPot3 = transform.Find("BigWin/JackPoolGrand").GetComponent<RollJackPot>();
            for (int i = 0; i < 9; i++)
                LineList.Add(transform.Find("Lines").GetChild(i));
            for (int i = 0; i < 9; i++)
            {
                lineGold.Add(transform.Find("Trans_winGoldLine").GetChild(i));
                lineGoldText.Add(lineGold[i].Find("LineGold").GetComponent<Text>());
            }            

            Scrollbar = transform.Find("Bomb/Scrollbar").GetComponent<Scrollbar>();

            buttonList.Add(BtnBomb1);
            buttonList.Add(BtnBomb2);
            buttonList.Add(BtnBomb3);

            //TxtBomb1 = BtnBomb1.transform.Find("TxtBomb1").GetComponent<TextMeshProUGUI>();
            //TxtBomb2 = BtnBomb2.transform.Find("TxtBomb2").GetComponent<TextMeshProUGUI>();
            //TxtBomb3 = BtnBomb3.transform.Find("TxtBomb3").GetComponent<TextMeshProUGUI>();
            //BombTxtList.Add(TxtBomb1);
            //BombTxtList.Add(TxtBomb2);
            //BombTxtList.Add(TxtBomb3);
            //BombEffect.Add(BtnBomb1.transform.GetChild(2));
            //BombEffect.Add(BtnBomb2.transform.GetChild(2));
            //BombEffect.Add(BtnBomb3.transform.GetChild(2));
            //BombImage.Add(BtnBomb1.transform.GetChild(1));
            //BombImage.Add(BtnBomb2.transform.GetChild(1));
            //BombImage.Add(BtnBomb3.transform.GetChild(1));
            TextCurrent = transform.Find("Bomb/TextCurrent").GetComponent<TextMeshProUGUI>();
            TfEffectSpine = transform.Find("Trans_WinGoldBg");
            Transform ElementBg = transform.Find("Trans_ElementBg/Bg");
            for(int i = 0;i< ElementBg.childCount;i++)
                ElementCellBg.Add(ElementBg.GetChild(i));
            Btn_Rank = transform.Find("Btn_Rank").GetComponent<Button>();
            Btn_Tour = transform.Find("Btn_Tour").GetComponent<Button>();
            m_Btn_TourRank = transform.Find("Btn_TourRank").GetComponent<Button>();
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

        public override void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom700_SlotCpt;
            SlotCpt.commonTop.OnRank = ClickBtnRank;
            SlotCpt.commonTop.OnHelp = ClickBtnHelp;
            SlotCpt.commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            SlotCpt.commonTop.OnClickEnd = SlotCpt.endSpin;
            SlotCpt.commonTop.beginSpin = BeginSlot;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_BetGameRet700, BetGameRet700);// 700房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_Jackpot700, UpdatPool700);// -- 更新奖次消息
            Message.AddListener(MessageName.GE_BroadCast_JackpotList700, RefreshJACKPOT700);
            Btn_Rank.onClick.AddListener(OnRankBtn);
            Btn_Tour.onClick.AddListener(OnTourBtn);
            m_Btn_TourRank.onClick.AddListener(OnTourRankBtn);
            HideAllLines(false,null);
        }

        public override void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game700Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            if (SlotCpt.bDanJi)
                Game700Model.Instance.n64Jackpot = 6850000;
            SlotCpt.commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(2412);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { InitGame(); }, 2412);
        }

        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game700Model.Instance.arrayAward.Count; i++)
            {
                if (Game700Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game700Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        public override void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            SlotCpt.commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            {
                //Effect.SetActive(false);
                SlotCpt.continueSpin();
            }, 100000);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom700Help");
        }

  

        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin)
                return;
            SlotCpt.commonTop.UpDateScore(score);
        }

        public void ShowOneLine(int index,bool bShowLineText = false)
        {
            LineList[index - 1].gameObject.SetActive(true);
            if(bShowLineText)
                lineGold[index - 1].gameObject.SetActive(true);
        }

        public void SetOneLineWinGoldText(int index,int elementCount,int element)
        {
            int rate = 1;
            if (elementCount == 2)
                rate = 1;
            else if (elementCount == 3)
                rate = Game700Model.Instance.elementRate3[element - 1];
            else if (elementCount == 4)
                rate = Game700Model.Instance.elementRate4[element - 1];
            else if(elementCount == 5)
                rate = Game700Model.Instance.elementRate5[element - 1];

            lineGoldText[index - 1].text = ToolUtil.ShowF2Num(rate * (Game700Model.Instance.nBet1 / 9));
        }

        public void HideAllLines(bool bHide, List<KeyValuePair<int, int>> lines)
        {
            if (!bHide)
            {
                for (int i = 0; i < 9; i++)
                {
                    lineGold[i].gameObject.SetActive(false);
                    LineList[i].gameObject.SetActive(false);
                }
             
            }
            else
                for (int i = 0; i < lines.Count; i++)
                    LineList[lines[i].Key - 1].gameObject.SetActive(bHide);
        }

        public override void InitGame()
        {
            SetTopBomb();
            SlotCpt.commonTop.InitGame(Game700Model.Instance.GearList, Game700Model.Instance.nBet);
            SetJackPot();
            SlotCpt.setState(slotState.Idle);
            if (Game700Model.Instance.nModelGame > 0)
            {
                if (Game700Model.Instance.nFreeGame > 0)
                {
                    SlotCpt.commonTop.SetFreeTimes(true, Game700Model.Instance.toSpin.FreeTimes + "");
                    Game700Model.Instance.toSpin.nModelGame = Game700Model.Instance.nModelGame;
                    SlotCpt.freeTimes.max = Game700Model.Instance.toSpin.nModelGame;
                    Game700Model.Instance.toSpin.FreeTimes = Game700Model.Instance.nFreeGame;
                    SlotCpt.commonTop.UpDateScore(Game700Model.Instance.n64FreeGold);
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
            long jackPot = Game700Model.Instance.n64Jackpot;
            rollJackPot3.SetNum(jackPot,true);
           // rollJackPot1.SetNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][0] * jackPot / 100, true);
            //rollJackPot2.SetNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][1] * jackPot / 100, true);
            //rollJackPot3.SetNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][2] * jackPot / 100, true);
        }

        protected override void SetTopRank()
        {
            //if(Game700Model.Instance.arrayAward.Count >= 1)
            // {
            //     if (Game700Model.Instance.arrayAward[0].n64Gold <= 0)
            //         return;
            // }
            // TxtID.text = CommonTools.BytesToString(Game700Model.Instance.arrayAward[0].szName);
            // TxtMoney.text = "R$ " + (Game700Model.Instance.arrayAward[0].n64Gold / 10000).ToString("f2");

            // string imgurl = "img_Head_" + Game700Model.Instance.arrayAward[0].nIconID;
            // ImgHead.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
        }


        public override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_BetGameRet700, BetGameRet700);// -- 700房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_Jackpot700, UpdatPool700);// -- 更新奖次消息
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList700, RefreshJACKPOT700);
            Btn_Rank.onClick.RemoveListener(OnRankBtn);
            Btn_Tour.onClick.RemoveListener(OnTourBtn);
            m_Btn_TourRank.onClick.RemoveListener(OnTourRankBtn);
        }
        public void BeginSlot(int num, bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(226);
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
            slotState state = SlotCpt.StateSlot;
            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    return;
                }
                SlotCpt.BtnRollBar.interactable = true;
                return;
            }
        }

        public override void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
            winGold = Game700Model.Instance.toSpin.WinGold;

            if (Game700Model.Instance.toSpin.n64FreeGold > 0)
                winGold = Game700Model.Instance.toSpin.n64FreeGold;

            if ((Game700Model.Instance.toSpin.rate <= 2 && Game700Model.Instance.toSpin.n64FreeGold <= 0) || SlotCpt.isFreeSpin)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
                {
                    SlotCpt.commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
                }, 7);
                CoreEntry.gAudioMgr.PlayUISound(230);
                CoreEntry.gAudioMgr.PlayUISound(23,transform.GetChild(3).gameObject);
            }
            if (Game700Model.Instance.toSpin.n64FreeGold <= 0)
                BigWinAni(null, false);
        }

        public override void BigWinAni(Action callBack, bool freeEnd, int bSpecialNum = 0)
        {
            if (Game700Model.Instance.toSpin.rate > 2 || freeEnd || Game700Model.Instance.toSpin.n64RSPowerGold > 0)
            {
                Effect.SetActive(true);
                gold = 0;
                if (freeEnd)
                {
                    gold = Game700Model.Instance.toSpin.n64FreeGold;
                    Game700Model.Instance.toSpin.rate = gold / Game700Model.Instance.nBet1;
                    winGold = this.gold;
                }
                else if (bSpecialNum > 0)
                    gold = bSpecialNum;
                else
                    gold = Game700Model.Instance.toSpin.WinGold;


                playGoldType = 0;
                if (Game700Model.Instance.toSpin.n64RSPowerGold > 0)
                {
                    gold = Game700Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 4;
                }
                else
                {

                    if (Game700Model.Instance.toSpin.rate > 2 && Game700Model.Instance.toSpin.rate <= 4)
                        playGoldType = 1;
                    else if (Game700Model.Instance.toSpin.rate > 4 && Game700Model.Instance.toSpin.rate <= 12)
                        playGoldType = 2;
                    else if (Game700Model.Instance.toSpin.rate <= 2)//免费可以中
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
                }, SlotCpt.autoSpinNum != 0);
            }
        }


        public void OnBetChangeCallBack(int bet)
        {
            Game700Model.Instance.nBet1 = (int)bet * 9; ;
            SetJackPot();
        }

        public override void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            Game700Model.Instance.bShowFreeAni = true;
            CoreEntry.gAudioMgr.PlayUISound(222);
            SlotCpt.commonTop.SetFreeTimes(true, Game700Model.Instance.toSpin.FreeTimes + "");
            TfFree.gameObject.SetActive(true);
            TxtFreeTimes.text = Game700Model.Instance.toSpin.nModelGame - Game700Model.Instance.lastCount + "";
            freeAni = Free.DOScale(new Vector3(1.2f, 1.2f, 1), 0.75f).OnComplete(() =>
            {
                freeAni = Free.DOScale(new Vector3(1, 1, 1), 0.75f).OnComplete(() => { TfFree.gameObject.SetActive(false); });
            });
        }

        public override void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            TfFree.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(false);
            if (Game700Model.Instance.toSpin.n64FreeGold > 0)
                BigWinAni(null, true, 0);
            SetBtnsScale(true);
        }

        public void BetGameRet700(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void UpdatPool700()
        {
            long jackPot = Game700Model.Instance.n64Jackpot;
            rollJackPot3.RollNum(jackPot, true);
            //rollJackPot1.RollNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][0] * jackPot / 100, true);
            //rollJackPot2.RollNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][1] * jackPot / 100, true);
            //rollJackPot3.RollNum(Game700Model.Instance.JackPotList[SlotCpt.commonTop.BetID][2] * jackPot / 100, true);
        }

        public void SetBtnsScale(bool bOne)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (bOne == false)
                scale = new Vector3(0, 0, 0);
            SlotCpt.commonTop.GetBeginBtn().transform.localScale = scale;
        }

        public void OnAWARD_BOX_700(GameEvent ge, EventParameter parameter)
        {
            SetTopBomb();
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;

            reward.SetJackPotNum(Game700Model.Instance.GetGold / ToolUtil.GetGoldRadio());

            SlotCpt.commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
        }

        public void SetTopBomb()
        {
            //for (int i = 0; i < BombTxtList.Count; i++)
            //    BombTxtList[i].text = ToolUtil.ShowF2Num2((long)Game700Model.Instance.BombList[i]);// (Game700Model.Instance.BombList[i]/10000).ToString();

            //var num1 = (Double)Game700Model.Instance.n64ExpendTotal / 10000.00;
            //string temp = num1.ToString("F1");

            //TextCurrent.text = temp;// (Game700Model.Instance.n64ExpendTotal / 10000f).ToString("f2");
            //Scrollbar.size = 1;
            //for (int i = 0; i < 3; i++)
            //{
            //    if (Game700Model.Instance.n64ExpendTotal < Game700Model.Instance.BombList[i])
            //    {
            //        float tempSize = 0;
            //        if (i == 0)
            //            tempSize = 0.25f;
            //        else if (i == 1)
            //            tempSize = 0.35f;
            //        else
            //            tempSize = 0.4f;
            //        if (i == 0)
            //            Scrollbar.size = Game700Model.Instance.n64ExpendTotal / Game700Model.Instance.BombList[i] * tempSize;
            //        else
            //        {
            //            if (i == 1)
            //                Scrollbar.size = 0.25f + (Game700Model.Instance.n64ExpendTotal - Game700Model.Instance.BombList[0]) / (Game700Model.Instance.BombList[i] - Game700Model.Instance.BombList[0]) * tempSize;
            //            else
            //                Scrollbar.size = 0.6f + (Game700Model.Instance.n64ExpendTotal - Game700Model.Instance.BombList[1]) / (Game700Model.Instance.BombList[i] - Game700Model.Instance.BombList[1]) * tempSize;
            //        }
            //        break;
            //    }
            //}

            //for (int i = 0; i < 3; i++)
            //{
            //    BombEffect[i].gameObject.SetActive(Game700Model.Instance.n64ExpendTotal >= Game700Model.Instance.BombList[i]);
            //    BombImage[i].gameObject.SetActive(Game700Model.Instance.n64ExpendTotal < Game700Model.Instance.BombList[i]);
            //}
        }


        public void RefreshJACKPOT700()
        {
            SetTopRank();
        }

        public override void ReloadGame()
        {
            base.ReloadGame();

            if (SlotCpt.gameStatus == 1)
            {
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.PlayTopLight();
            }
            else
            {
            }
        }




        public override void UpdateGold(long gold)
        {
            Debug.LogError("------------------"+gold);
            Game700Model.Instance.toSpin.n64Gold = gold;
            base.UpdateGold(gold);
            SlotCpt.commonTop.UpdateGold(gold);
        }
    }
}
