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
    public partial class Top1000Panel : PanelBase
    {
        private UIRoom1000 SlotCpt;
        protected slotState m_curSlotState;

        protected long gold = 0;

        public long winGold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;

       //public GoldEffect m_Gold_Effect;
        public GoldEffectNew m_Gold_EffectNew;
        public WinTipsEffect winTips;
        Sequence[] mySequences = new Sequence[4];
        List<TextMeshProUGUI> jackpotText = new List<TextMeshProUGUI>();

        public int freeRemainTimes = 5;

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
            GameObject go1 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/UITemplate/Gold_EffectNew");
            m_Gold_EffectNew = go1.GetComponent<GoldEffectNew>();
            m_Gold_EffectNew.gameObject.SetActive(false);
            GameObject go3 = CommonTools.AddSubChild(m_Trans_Effect.gameObject, "UI/Prefabs/Game1000/FirstRes/WinTipsEffect");// CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1000/FirstRes/WinTipsEffect");
            winTips = go3.GetComponent<WinTipsEffect>();
            winTips.gameObject.SetActive(false);
            for (int i = 1; i <= 30; i++)
                linesList.Add(transform.Find("Middle/Lines/line" + i));
            jackpotText.Add(m_TxtM_Pago1);
            jackpotText.Add(m_TxtM_Pago2);
            jackpotText.Add(m_TxtM_Pago3);
            jackpotText.Add(m_TxtM_Pago4);
        }

        protected override void Start()
        {
            base.Start();
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1000;
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
            Message.AddListener(MessageName.GE_BetGameRet1000, BetGameRet1000);// 1000房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_Jackpot1000, UpdatPool1000);// -- 更新奖次消息
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1000, RefreshJACKPOT1000);

            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);

            RegisterListener();
            ShowAllLines(true);
            CoreEntry.gTimeMgr.AddTimer(1, false, () => { ShowAllLines(false); }, 1);

            SetPressJoga(true);
            m_TxtM_Pago.text = "PAGO:0";

            //CoreEntry.gTimeMgr.AddTimer(5, true, () => { SetBoradCast(); }, 20);

            for (int i = 0; i < mySequences.Length; i++)
            {
                mySequences[i]?.Kill();
                mySequences[i] = null;
                SetJackPotNums(i);
            }
        }

        public void RegisterListener()
        {       

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_Focus_On, OnFocus);
        }

        public void UnRegisterListener()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_Focus_On, OnFocus);

        }

        public async void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game1000Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            if (SlotCpt.bDanJi)
                Game1000Model.Instance.n64Jackpot = 6850000;
            SlotCpt.commonTop.UpdateGold(Game1000Model.Instance.toSpin.n64Gold);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));
            InitGame();
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game1000Model.Instance.arrayAward.Count; i++)
            {
                if (Game1000Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1000Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList, 1000);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom1000Help");
        }

        public async void ClickBtnCloseEffect(bool bGameEnd = true)
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            if (bGameEnd)
            {
                CoreEntry.gTimeMgr.RemoveTimer(100000);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
                SlotCpt.continueSpin();
            }

            if (!bGameEnd)
            {
      
                int type = Game1000Model.Instance.nModelGame;
                if(type == 5)
                {
                    StartCoroutine(DelayOpenSmallGame(type));
                }                   
                else
                {
                    m_Spine_GameTips.gameObject.SetActive(true);

                    int temp = 4;
                    if (type == 2)
                        temp = 4;
                    else if (type == 4)
                        temp = 1;
                    else if (type == 3)
                        temp = 2;
                    else if (type == 1)
                        temp = 3;
                    ToolUtil.PlayAnimation(m_Spine_GameTips.transform.transform, "a" + temp, false,()=> { StartCoroutine(DelayOpenSmallGame(type)); });
                }
      
            }
        }

        public void SetTxtScore(long score)
        {
            // Debug.LogError("-------------"+ SlotCpt.isFreeSpin +"======"+ Game1000Model.Instance.toSpin.n64FreeGold);
            if (SlotCpt.isFreeSpin && Game1000Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.commonTop.UpDateScore(score);
            m_TxtM_Pago.text = string.Format("PAGO:{0}", ToolUtil.ShowF2Num(score));
        }

        public void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game1000Model.Instance.GearList, Game1000Model.Instance.nBet);
            SlotCpt.setState(slotState.Idle);
            SetTopRank();
        }

     

        protected void SetTopRank()
        {
            currentType = 1;
            if (Game1000Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1000Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game1000Model.Instance.arrayAward[0].szName), Game1000Model.Instance.arrayAward[0].n64Gold, Game1000Model.Instance.arrayAward[0].nIconID);
            }
        }


        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1000, BetGameRet1000);// -- 1000房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_Jackpot1000, UpdatPool1000);// -- 1000房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1000, RefreshJACKPOT1000);// -- 1000房间下注结果返回
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            UnRegisterListener();
        }

        private void ClickPegue()
        {
            if (Game1000Model.Instance.nModelGame > 0 && Game1000Model.Instance.nModelGame != 5)
                return;
            SlotCpt.commonTop.UpdateGold(Game1000Model.Instance.toSpin.n64Gold);
            m_TxtM_Pago.text = string.Format("PAGO:{0}", ToolUtil.ShowF2Num(Game1000Model.Instance.toSpin.WinGold + Game1000Model.Instance.n64ModelGold));
            SlotCpt.commonTop.UpDateScore(0);
            SetPressJoga(true);
            SlotCpt.setState(slotState.Idle);
        }


        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {
        }

        public async void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {   
            SetPressJoga(false);
            winGold = Game1000Model.Instance.toSpin.WinGold;
            if (Game1000Model.Instance.toSpin.rate < 1.5 && Game1000Model.Instance.nModelGame <= 0)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                return;
            }
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
            BigWinAni(Game1000Model.Instance.nModelGame <=0);
        }

        public void BigWinAni(bool bGameEnd = true)
        {
            if (Game1000Model.Instance.toSpin.rate >= 1.5)
            {
                m_Trans_Effect.gameObject.SetActive(true);
                gold = bGameEnd ? Game1000Model.Instance.toSpin.WinGold + Game1000Model.Instance.n64ModelGold : Game1000Model.Instance.toSpin.WinGold;
                playGoldType = 0;
                if (Game1000Model.Instance.nModelGame == 52222)
                {
                    gold = Game1000Model.Instance.toSpin.n64RSPowerGold;
                    playGoldType = 6;
                }
                else
                {
                    if (Game1000Model.Instance.toSpin.rate >= 1.5f && Game1000Model.Instance.toSpin.rate <3)
                        playGoldType = 1;
                    else if (Game1000Model.Instance.toSpin.rate >= 3 && Game1000Model.Instance.toSpin.rate <5)
                        playGoldType = 2;
                    else if (Game1000Model.Instance.toSpin.rate >= 5 && Game1000Model.Instance.toSpin.rate < 10)
                        playGoldType = 3;
                    else if (Game1000Model.Instance.toSpin.rate >= 10 && Game1000Model.Instance.toSpin.rate < 30)
                        playGoldType = 4;
                    else
                        playGoldType = 5;
                }

                if(gold > 0)
                {
                    winTips.setData(playGoldType, gold, () =>
                    {
                        ClickBtnCloseEffect(bGameEnd);
                    }, SlotCpt.autoSpinNum != 0);
                }
            }
            else
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                ClickBtnCloseEffect(bGameEnd);
            }
        }



        public void OnBetChangeCallBack(int bet)
        {
            Game1000Model.Instance.nBet1 = bet;
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {

        }

        public void SetFreeTimes()
        {
            //m_Trans_GoFreeTimes.gameObject.SetActive(true);
            //m_Txt_Times.text = Game1000Model.Instance.toSpin.FreeTimes + "/" + Game1000Model.Instance.toSpin.nModelGame;
        }

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
            // m_Trans_TfFree.gameObject.SetActive(true);
            // m_Trans_FreeGameEnd.gameObject.SetActive(true);
            // m_Trans_FreeTimes.gameObject.SetActive(false);
            //// m_Trans_GoFreeTimes.gameObject.SetActive(false);

            // m_Btn_Coletar.interactable = false;
            // CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "1a", 1, () =>
            // {
            //     m_Btn_Coletar.interactable = true;
            //     CommonTools.PlayArmatureAni(m_Dragon_Free.transform, "1b", 0, () => {});
            // });
            // //if (Game1000Model.Instance.toSpin.n64FreeGold > 0)
            // //    BigWinAni(null, true, 0);
            // //SetBtnsScale(true);
            // m_Txt_FreeTimes.gameObject.SetActive(false);
            // m_Txt_GetFreeGold.gameObject.SetActive(true);
            // var kk = m_Dragon_Free.transform.Find("a02");
            // // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            // m_Txt_GetFreeGold.transform.SetParent(kk);
            // m_Txt_GetFreeGold.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            // m_Txt_GetFreeGold.transform.localPosition = new Vector3(0, -0.17f, 0);

        }

        public void BetGameRet1000()
        {
            UpdateJackpot();
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void UpdateJackpot()
        {
            for (int i = 0; i < mySequences.Length; i++)
            {

                double curretValue = double.Parse(jackpotText[i].text, new CultureInfo("en"));


                double dis = Math.Abs(Game1000Model.Instance.betJackPotList[i] - curretValue * ToolUtil.GetGoldRadio());
                //Debug.Log("<<<<<<<<<" + Game1000Model.Instance.betJackPotList[i] + "=====" + curretValue * 10000 + "====" + dis);
                if (dis > 50000)
                {
                    mySequences[i]?.Kill();
                    mySequences[i] = null;
                    SetJackPotNums(i);
                }
            }
        }
        public void UpdatPool1000()
        {
            long jackPot = Game1000Model.Instance.n64Jackpot;

            //rollJackPot1.RollNum(Game1000Model.Instance.JackPotList[betID][0] * jackPot/100,true);
            //rollJackPot2.RollNum(Game1000Model.Instance.JackPotList[betID][1] * jackPot/100, true);
            //rollJackPot3.RollNum(Game1000Model.Instance.JackPotList[betID][2] * jackPot/100, true);
        }

        public void SetBtnsScale(bool bOne)
        {
            Vector3 scale = new Vector3(1, 1, 1);
            if (bOne == false)
                scale = new Vector3(0, 0, 0);
            // m_Btn_BeginSpin.transform.localScale = scale;
        }

        public void RefreshJACKPOT1000()
        {
            SetTopRank();
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
            for (int i = 0; i < mySequences.Length; i++)
            {
                mySequences[i]?.Kill();
                mySequences[i] = null;
                SetJackPotNums(i);
            }
        }

        public void UpdateGold(long gold)
        {
            Game1000Model.Instance.toSpin.n64Gold = gold;
            SlotCpt.commonTop.UpdateGold(gold);
        }




        private void OnFocus(GameEvent ge, EventParameter parameter)
        {
            var time = parameter.floatParameter;
            for (int i = 0; i < mySequences.Length; i++)
            {

                double curretValue = double.Parse(jackpotText[i].text, new CultureInfo("en")) * ToolUtil.GetGoldRadio();

                curretValue += (time * 1000 * 100 / Game900Model.Instance.jackSpeedPotList[i]);

                UpdateOneJackpot((long)curretValue, i + 1);
            }
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

        public void SetJackPotNums(int index)
        {
            long targetValue = Game1000Model.Instance.jackPotList[index] + 30 * 1000 * 100 / Game1000Model.Instance.jackSpeedPotList[index];
            if(index == 0)
                mySequences[0] = ToolUtil.RollText(Game1000Model.Instance.jackPotList[0], targetValue, m_TxtM_Pago1, 30, () => { FinishOneRollText(0); });
            else if (index == 1)
                mySequences[1] = ToolUtil.RollText(Game1000Model.Instance.jackPotList[1], targetValue, m_TxtM_Pago2, 30, () => { FinishOneRollText(1); });
            else if (index == 2)
                mySequences[2] = ToolUtil.RollText(Game1000Model.Instance.jackPotList[2], targetValue, m_TxtM_Pago3, 30, () => { FinishOneRollText(2); });
            else if (index == 3)
                mySequences[3] = ToolUtil.RollText(Game1000Model.Instance.jackPotList[3], targetValue, m_TxtM_Pago4, 30, () => { FinishOneRollText(3); });
        }

        private void FinishOneRollText(int index)
        {
            mySequences[index]?.Kill();
            mySequences[index] = null;

            long targetValue = Game1000Model.Instance.jackPotList[index] + 30 * 1000 * 100 / Game1000Model.Instance.jackSpeedPotList[index];
            Game1000Model.Instance.jackPotList[index] = targetValue;
            SetJackPotNums(index);
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
            Game1000Model.Instance.jackPotList[ucRSID - 1] = jackPot;
            SetJackPotNums(ucRSID - 1);
        }

        public void ShowAllLines(bool bShow)
        {
            for (int i = 0; i < linesList.Count; i++)
                linesList[i].gameObject.SetActive(bShow);
        }

        public void ShowOneLine(int line)
        {
            linesList[line].gameObject.SetActive(true);
        }

        public void SetPressJoga(bool bSet = false)
        {
            if (bSet)
            {
                CoreEntry.gTimeMgr.RemoveTimer(5);
                m_TxtM_PressJoga.gameObject.SetActive(true);
                m_TxtM_PressJoga.text = "PRESS JOGA";
            }
            else
            {
                m_TxtM_PressJoga.text = "LINHA DOBRA";
                CoreEntry.gTimeMgr.AddTimer(1f, true, () =>
                  {
                      m_TxtM_PressJoga.gameObject.SetActive(!m_TxtM_PressJoga.gameObject.activeSelf);
                  }, 5);
            }
        }



        /// <summary>
        ///  1 转盘 2骰子 3草莓  4锅炉  5南瓜
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        private IEnumerator DelayOpenSmallGame(int type)
        {
            if(type < 5)
                yield return new WaitForSeconds(1.1f);
            else
                yield return new WaitForSeconds(0.1f);
            m_Spine_GameTips.gameObject.SetActive(false);
            if (type == 4)
                SlotCpt.OpenSmallGame3();
            else if (type == 3)
                SlotCpt.OpenDiceGames();
            else if (type == 1)
                SlotCpt.OpenSmallGame2();
            else if (type == 2)
                SlotCpt.OpenSmallGame1();
            else if (type == 5)
            {
                winTips.setData(6,Game1000Model.Instance.n64ModelGold, () =>
                {
                    ClickBtnCloseEffect(true);
                    SetTotalWinGold();
                }, SlotCpt.autoSpinNum != 0);
            }
        }


        public void SetTotalWinGold()
        {
            long totalGold = Game1000Model.Instance.toSpin.WinGold + Game1000Model.Instance.n64ModelGold;
            if (Game1000Model.Instance.nModelGame == 3 || Game1000Model.Instance.nModelGame == 4)
            {
                totalGold -= Game1000Model.Instance.n64BaseGold;
            }
            SlotCpt.commonTop.UpDateScore(totalGold);
        }
    }
}
