using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using uGUI;
using SEZSJ;
using System.Globalization;

namespace HotUpdate
{
    public partial class Top900Panel : PanelBase
    {
        private UIRoom900 SlotCpt;
        public int betID = 0;//--挡位

        protected float timeBeginSpin = 0;
        protected slotState m_curSlotState;

        protected long gold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;

        //public GoldEffect m_Gold_Effect;
        public GoldEffectNew m_Gold_EffectNew;
        Sequence[] mySequences = new Sequence[10];

        public int freeRemainTimes = 5;

        List<TextMeshProUGUI> jackpotText = new List<TextMeshProUGUI>();
        List<Text> rightJackpotText = new List<Text>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            //GameObject go = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_Effect");
            //m_Gold_Effect = go.GetComponent<GoldEffect>();
            //m_Gold_Effect.gameObject.SetActive(false);

            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_EffectNew");
            m_Gold_EffectNew = go1.GetComponent<GoldEffectNew>();
            m_Gold_EffectNew.gameObject.SetActive(false);

            jackpotText.Add(m_TxtM_Diamond1);
            jackpotText.Add(m_TxtM_Diamond2);
            jackpotText.Add(m_TxtM_Diamond3);
            jackpotText.Add(m_TxtM_Diamond4);
            jackpotText.Add(m_TxtM_Diamond5);
            rightJackpotText.Add(m_Txt_Pagar5);
            rightJackpotText.Add(m_Txt_Pagar6);
            rightJackpotText.Add(m_Txt_Pagar7);
            rightJackpotText.Add(m_Txt_Pagar8);
            rightJackpotText.Add(m_Txt_Pagar9);
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom900;
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
            Message.AddListener(MessageName.GE_BetGameRet900, BetGameRet900);// 900房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList900, RefreshJACKPOT900);

            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            RegisterListener();

            for (int i = 0; i < mySequences.Length; i++)
            {
                mySequences[i]?.Kill();
                mySequences[i] = null;
                SetDiamondsNums(i);
            }
            CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
            {
                ToolUtil.PlayAnimation(m_Spine_effect1.transform, "a2", true);
                ToolUtil.PlayAnimation(m_Spine_effect2.transform, "a2", true);
                ToolUtil.PlayAnimation(m_Spine_effect3.transform, "a2", true);
                ToolUtil.PlayAnimation(m_Spine_effect4.transform, "a2", true);
                ToolUtil.PlayAnimation(m_Spine_effect5.transform, "a2", true);
            }, 9);
        }

        public void EnterGame()
        {
            SlotCpt.commonTop.SetFreeTimes(true, Game900Model.Instance.toSpin.FreeTimes+"");
            Game900Model.Instance.toSpin.nModelGame = Game900Model.Instance.nModelGame;
            SlotCpt.freeTimes.max = Game900Model.Instance.toSpin.nModelGame;
            Game900Model.Instance.toSpin.FreeTimes = Game900Model.Instance.nFreeGame;
            SlotCpt.commonTop.UpDateScore(Game900Model.Instance.toSpin.n64FreeGold);
            //m_Txt_Times.text = Game900Model.Instance.toSpin.FreeTimes + "/" + Game900Model.Instance.toSpin.nModelGame;
            CoreEntry.gTimeMgr.AddTimer(1, false, () =>
            {
                if (SlotCpt.StateSlot == slotState.Idle)
                    SlotCpt.continueSpin();
            }, 24121);
        }

        public void RegisterListener()
        {
            m_Btn_Continue.onClick.AddListener(ClickContinue);
            m_Btn_Coletar.onClick.AddListener(ClickColetar);
            CoreEntry.gEventMgr.AddListener(GameEvent.GE_Focus_On, OnFocus);
        }

        private void OnFocus(GameEvent ge, EventParameter parameter)
        {
            var time = parameter.floatParameter;
            for (int i = 0; i < mySequences.Length; i++)
            {

                double curretValue = double.Parse(i < 5 ? jackpotText[i].text : rightJackpotText[i - 5].text, new CultureInfo("en")) * ToolUtil.GetGoldRadio();

                curretValue += (time * 1000 * 100 / Game900Model.Instance.jackSpeedPotList[i]);

                UpdateOneJackpot((long)curretValue, i + 1);
            }
        }

        public void UnRegisterListener()
        {
            m_Btn_Continue.onClick.RemoveListener(ClickContinue);
            m_Btn_Coletar.onClick.RemoveListener(ClickColetar);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_Focus_On, OnFocus);
        }

        public async void InitData()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            SlotCpt.commonTop.UpDateScore(0);
            Game900Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            SlotCpt.commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold);
            InitGame();
        }

        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game900Model.Instance.arrayAward.Count; i++)
            {
                if (Game900Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game900Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList, 900);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom900Help");
        }




        public void SetTxtScore(long score)
        {
            if (SlotCpt.isFreeSpin && Game900Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.commonTop.UpDateScore(score);
        }

        public void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game900Model.Instance.GearList, Game900Model.Instance.nBet);
            SlotCpt.setState(slotState.Idle);
            SetTopRank();
        }

        protected void SetTopRank()
        {
            if (Game900Model.Instance.arrayAward.Count >= 1)
            {
                if (Game900Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game900Model.Instance.arrayAward[0].szName), Game900Model.Instance.arrayAward[0].n64Gold, Game900Model.Instance.arrayAward[0].nIconID);
            }
        }


        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet900, BetGameRet900);// -- 900房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList900, RefreshJACKPOT900);// -- 900房间下注结果返回

            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            UnRegisterListener();
        }


        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, false);
        }


        private void ClickContinue()
        {
            CoreEntry.gTimeMgr.RemoveTimer(1);
            CoreEntry.gAudioMgr.StopSound();
            CoreEntry.gTimeMgr.RemoveTimer(3);
            ToolUtil.PlayAnimation(m_Spine_Free.transform, "2c", false, () =>
            {
                m_Trans_TfFree.gameObject.SetActive(false);
                SlotCpt.continueSpin();
            });
        }
        private void ClickColetar()
        {
            CoreEntry.gTimeMgr.RemoveTimer(7741);
            m_Txt_FreeTimes.gameObject.SetActive(false);
            CoreEntry.gAudioMgr.StopSound();
            if (m_Trans_TfFree.gameObject.activeSelf && m_Spine_Free.gameObject.activeSelf)
                ToolUtil.PlayAnimation(m_Spine_Free.transform, "1c", false, () =>
                {
                    SlotCpt.continueSpin();
                    m_Trans_TfFree.gameObject.SetActive(false);
                });
            else
            {
                m_Trans_TfFree.gameObject.SetActive(false);
                SlotCpt.continueSpin();
            }

            //Debug.LogError("-"+ "===========" + Game900Model.Instance.toSpin.WinGold + "=======" + Game900Model.Instance.n64JackPotGold + "======" + Game900Model.Instance.toSpin.n64FreeGold);
            long totalWingold = Game900Model.Instance.n64JackPotGold + Game900Model.Instance.toSpin.n64FreeGold;
            SlotCpt.commonTop.UpDateScore(totalWingold);
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {

        }

        public void OnSlotWinGold(Action callBack, Action continueGame)
        {
            bool bEnd = Game900Model.Instance.bHasSmallGame ? false : true;///是否可以直接结束
            bool bHasJackpot = Game900Model.Instance.n64JackPotGold > 0 && Game900Model.Instance.ucRSID > 5;///普通奖池
            if (bHasJackpot)
                bEnd = false;
            if (Game900Model.Instance.toSpin.rate <= 2 || SlotCpt.isFreeSpin || Game900Model.Instance.bHasSmallGame)//免费金额小于2倍并且有奖池的直接显示
            {
                if (SlotCpt.isFreeSpin)
                    SlotCpt.commonTop.UpDateScore(Game900Model.Instance.toSpin.n64FreeGold);
                else
                    SlotCpt.commonTop.UpDateScore(Game900Model.Instance.toSpin.WinGold);
                if (bEnd)
                {
                    SlotCpt.commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold);
                    continueGame?.Invoke();
                }
                else
                    callBack?.Invoke();//普通奖池
            }
            else
            {
                BigWinAni(Game900Model.Instance.toSpin.WinGold, bEnd, false, callBack, continueGame);
            }
        }

        public void BigWinAni(long winGold1 = 0, bool bEnd = true, bool bJackpot = false, Action callBack = null, Action continueGame = null)
        {
            m_Trans_Effect.gameObject.SetActive(true);
            gold = winGold1;
            if (bJackpot)
            {
                playGoldType = 4;
            }
            else
            {
                if (Game900Model.Instance.toSpin.rate > 2 && Game900Model.Instance.toSpin.rate <= 4)
                    playGoldType = 1;
                else if (Game900Model.Instance.toSpin.rate > 4 && Game900Model.Instance.toSpin.rate <= 12)
                    playGoldType = 2;
                else
                    playGoldType = 3;
            }

            bool bCanContinueGame = true;

            m_Gold_EffectNew.setData(playGoldType, gold, () =>
            {
                SlotCpt.commonTop.GetBeginBtn().GetComponent<Canvas>().overrideSorting = false;
                if (bJackpot)
                    gold += Game900Model.Instance.n64JackPotGold;
                if (SlotCpt.isFreeSpin)
                    gold = Game900Model.Instance.toSpin.n64FreeGold;
                SlotCpt.commonTop.UpDateScore(gold);
                if (bEnd)
                {
                    if (bCanContinueGame)
                    {
                        CoreEntry.gTimeMgr.AddTimer(0.7f,false,()=> 
                        {
                            continueGame?.Invoke();
                        },31);
                    }
                    SlotCpt.commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold);
                }
                else
                    callBack?.Invoke();

            }, SlotCpt.autoSpinNum != 0, null, () =>
            {
                if (!SlotCpt.IsAutoSpin() && SlotCpt.freeTimes.max <= 0 && bEnd)
                {
                    bCanContinueGame = false;
                    SlotCpt.commonTop.GetBeginBtn().GetComponent<Canvas>().overrideSorting = true;
                    SlotCpt.continueSpin();
                }
            });
        }

        public void OnBetChangeCallBack(int bet)
        {
            Game900Model.Instance.nBet1 = bet;
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gAudioMgr.PlayUISound(70);

            m_Trans_TfFree.gameObject.SetActive(true);
            m_Btn_Continue.interactable = false;

            ToolUtil.PlayAnimation(m_Spine_Free.transform, "2a", false, () =>
            {
                m_Btn_Continue.interactable = true;
                ToolUtil.PlayAnimation(m_Spine_Free.transform, "2b");
            });
            var kk = m_Spine_Free.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("kk3/kk4");
            m_Txt_FreeTimes.transform.SetParent(kk);
            m_Txt_FreeTimes.transform.localScale = new Vector3(0.1262626f, 0.1262626f, 1);
            m_Txt_FreeTimes.transform.localPosition = new Vector3(0, 0.5f, 0);

            m_Btn_Continue.transform.SetParent(kk);
            m_Btn_Continue.transform.localScale = new Vector3(0.1280554f, 0.1280554f, 1);
            m_Btn_Continue.transform.localPosition = new Vector3(0, -22.30986f, 0);

            m_Trans_FreeTimes.gameObject.SetActive(true);
            m_Trans_FreeGameEnd.gameObject.SetActive(false);
            SlotCpt.commonTop.SetFreeTimes(true,Game900Model.Instance.toSpin.FreeTimes+"");
            //m_Txt_Times.text = Game900Model.Instance.toSpin.FreeTimes + "/" + Game900Model.Instance.toSpin.nModelGame;
            m_Txt_FreeTimes.text = Game900Model.Instance.toSpin.nModelGame - Game900Model.Instance.lastCount + "";
            Game900Model.Instance.bShowFreeAni = true;
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

        //public void SetFreeTimes()
        //{
        //    m_Trans_GoFreeTimes.gameObject.SetActive(true);
        //    m_Txt_Times.text = Game900Model.Instance.toSpin.FreeTimes + "/" + Game900Model.Instance.toSpin.nModelGame;
        //}

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            SlotCpt.commonTop.SetFreeTimes(false);
            if (Game900Model.Instance.toSpin.n64FreeGold == 0)
            {
                ClickColetar();
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(71);
            m_Trans_TfFree.gameObject.SetActive(true);
            m_Trans_FreeGameEnd.gameObject.SetActive(true);
            m_Trans_FreeTimes.gameObject.SetActive(false);

            m_Btn_Coletar.interactable = false;

            m_Txt_GetFreeGold.gameObject.SetActive(true);

            ToolUtil.PlayAnimation(m_Spine_Free.transform, "1a", false, () =>
            {
                m_Btn_Coletar.interactable = true;

                ToolUtil.PlayAnimation(m_Spine_Free.transform, "1b", true, () =>
                {


                });
            });
            m_Txt_FreeTimes.gameObject.SetActive(false);
            m_Txt_GetFreeGold.gameObject.SetActive(true);
            var kk = m_Spine_Free.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).Find("kk3/kk4");
            m_Txt_GetFreeGold.transform.SetParent(kk);
            m_Txt_GetFreeGold.transform.localScale = new Vector3(0.1262626f, 0.1262626f, 1);
            m_Txt_GetFreeGold.transform.localPosition = new Vector3(0, -5.4f, 0);
            m_Txt_GetFreeGold.text = ToolUtil.ShowF2Num2(Game900Model.Instance.toSpin.n64FreeGold);

            m_Btn_Coletar.transform.SetParent(kk);
            m_Btn_Coletar.transform.localScale = new Vector3(0.1280554f, 0.133485f, 1);
            m_Btn_Coletar.transform.localPosition = new Vector3(0, -22.30986f, 0);

            CoreEntry.gTimeMgr.AddTimer(5,false,()=> 
            {
                ClickColetar();
            },7741);
        }

        public void BetGameRet900()
        {
            if (!Game900Model.Instance.bHasSmallGame)
                UpdateJackpot();
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void UpdateJackpot()
        {
            for (int i = 0; i < mySequences.Length; i++)
            {

                double curretValue = double.Parse(i < 5 ? jackpotText[i].text : rightJackpotText[i - 5].text, new CultureInfo("en"));
                double dis = Math.Abs(Game900Model.Instance.betJackPotList[i] - curretValue * ToolUtil.GetGoldRadio());
                if (dis > 50000)
                {
                    mySequences[i]?.Kill();
                    mySequences[i] = null;
                    SetDiamondsNums(i);
                }
            }
        }

        public void RefreshJACKPOT900()
        {
            SetTopRank();
            int ucRSID = Game900Model.Instance.arrayAward[0].ucRSID;
            long jackPotValue = Game900Model.Instance.arrayAward[0].sJackpot[0].n64Jackpot;
            UpdateOneJackpot(jackPotValue, ucRSID);
        }

        public void ReloadGame()
        {
            SlotCpt.commonTop.UpdateGold(MainUIModel.Instance.palyerData.m_i8Golds);
            if (SlotCpt.gameStatus == 1)
            {
                // SlotCpt.continueSpin();
                CoreEntry.gTimeMgr.Reset();
                SlotCpt.awaiting = true;
                SlotCpt.Reconnect();
                SlotCpt.effectAcc.gameObject.SetActive(false);
            }

            for (int i = 0; i < mySequences.Length; i++)
            {
                mySequences[i]?.Kill();
                mySequences[i] = null;
                SetDiamondsNums(i);
            }
        }

        public void UpdateGold(long gold)
        {
            SlotCpt.commonTop.UpdateGold(gold);
            Game900Model.Instance.toSpin.n64Gold = gold;
        }

        public void onPointerUp(bool isOn)
        {
            if (Game900Model.Instance.toSpin.SpecialGame > 0)
                return;
            if (SlotCpt.commonTop.GetBeginBtn().interactable == false)
                return;

            CoreEntry.gAudioMgr.PlayUISound(1);
            CoreEntry.gTimeMgr.RemoveTimer(10);
            if(slotState.Idle == SlotCpt.StateSlot)
                SlotCpt.commonTop.BeginSlot(SlotCpt.commonTop.GetTolAuto() ?0 : 0);
            timeBeginSpin = 0;
        }


        private void OnTolAuto(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            if (isOn)
            {
                SlotCpt.autoSpinNum = -1;
            }
            else
                SlotCpt.autoSpinNum = 0;
        }  

        public void SetDiamondsNums(int index)
        {
            long targetValue = Game900Model.Instance.jackPotList[index] + 30 * 1000 * 100 / Game900Model.Instance.jackSpeedPotList[index];// 1000 * 10000/  Game900Model.Instance.jackSpeedPotList[index]*30;

            // Debug.LogError(Game900Model.Instance.jackPotList[index]+"=========================" +index+"====="+ targetValue);

            if (index == 0)
                mySequences[0] = ToolUtil.RollText(Game900Model.Instance.jackPotList[0], targetValue, m_TxtM_Diamond1, 30, () => { FinishOneRollText(0); });
            else if (index == 1)
                mySequences[1] = ToolUtil.RollText(Game900Model.Instance.jackPotList[1], targetValue, m_TxtM_Diamond2, 30, () => { FinishOneRollText(1); });
            else if (index == 2)
                mySequences[2] = ToolUtil.RollText(Game900Model.Instance.jackPotList[2], targetValue, m_TxtM_Diamond3, 30, () => { FinishOneRollText(2); });
            else if (index == 3)
                mySequences[3] = ToolUtil.RollText(Game900Model.Instance.jackPotList[3], targetValue, m_TxtM_Diamond4, 30, () => { FinishOneRollText(3); });
            else if (index == 4)
                mySequences[4] = ToolUtil.RollText(Game900Model.Instance.jackPotList[4], targetValue, m_TxtM_Diamond5, 30, () => { FinishOneRollText(4); });
            else if (index == 5)
                mySequences[5] = ToolUtil.RollText(Game900Model.Instance.jackPotList[5], targetValue, m_Txt_Pagar5, 30, () => { FinishOneRollText(5); });
            else if (index == 6)
                mySequences[6] = ToolUtil.RollText(Game900Model.Instance.jackPotList[6], targetValue, m_Txt_Pagar6, 30, () => { FinishOneRollText(6); });
            else if (index == 7)
                mySequences[7] = ToolUtil.RollText(Game900Model.Instance.jackPotList[7], targetValue, m_Txt_Pagar7, 30, () => { FinishOneRollText(7); });
            else if (index == 8)
                mySequences[8] = ToolUtil.RollText(Game900Model.Instance.jackPotList[8], targetValue, m_Txt_Pagar8, 30, () => { FinishOneRollText(8); });
            else if (index == 9)
                mySequences[9] = ToolUtil.RollText(Game900Model.Instance.jackPotList[9], targetValue, m_Txt_Pagar9, 30, () => { FinishOneRollText(9); });
        }

        /// <summary>
        /// 中得奖池ID  
        /// </summary>
        /// <param name="jackPot"></param>
        /// <param name="ucRSID"></param>
        public void UpdateOneJackpot(long jackPot, int ucRSID)
        {
            mySequences[ucRSID - 1]?.Kill();
            mySequences[ucRSID - 1] = null;
            Game900Model.Instance.jackPotList[ucRSID - 1] = jackPot;
            SetDiamondsNums(ucRSID - 1);
        }
        private void FinishOneRollText(int index)
        {
            mySequences[index]?.Kill();
            mySequences[index] = null;

            long targetValue = Game900Model.Instance.jackPotList[index] + 30 * 1000 * 100 / Game900Model.Instance.jackSpeedPotList[index];
            Game900Model.Instance.jackPotList[index] = targetValue;
            SetDiamondsNums(index);
        }
    }
}
