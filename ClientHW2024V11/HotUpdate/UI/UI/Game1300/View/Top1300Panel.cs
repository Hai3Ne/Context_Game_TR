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
    public partial class Top1300Panel : PanelBase
    {
        private UIRoom1300 SlotCpt;
        public int betID = 0;//--挡位

        protected float timeBeginSpin = 0;
        protected slotState m_curSlotState;
        public long winGold = 0;
        protected long gold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;
        public BigWin1300 m_Gold_EffectNew;

        public int freeRemainTimes = 5;

        List<Image> elementImagList = new List<Image>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            GameObject go1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1300/FirstRes/BigWin1300");
            m_Gold_EffectNew = go1.GetComponent<BigWin1300>();
            m_Gold_EffectNew.gameObject.SetActive(false);
            m_Gold_EffectNew.transform.SetParent(m_Trans_Effect, true);
            m_Gold_EffectNew.gameObject.SetActive(true);
            m_Gold_EffectNew.transform.localScale = new Vector3(1, 1, 1);
            m_Gold_EffectNew.transform.localPosition = new Vector3(0, 0, 0);


            elementImagList.Add(m_Img_0);
            elementImagList.Add(m_Img_1);
            elementImagList.Add(m_Img_2);
            elementImagList.Add(m_Img_3);
            elementImagList.Add(m_Img_4);
        }

        public void Init(UIRoom_SlotCpt slot)
        {
            SlotCpt = slot as UIRoom1300;
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
            Message.AddListener(MessageName.GE_BetGameRet1300, BetGameRet1300);// 1300房间下注结果返回
            Message.AddListener(MessageName.GE_BroadCast_JackpotList1300, RefreshJACKPOT1300);

            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            RegisterListener();

            GetTrans_Win().gameObject.SetActive(false);
            m_Spine_light.gameObject.SetActive(false);
            if (m_Gold_EffectNew == null)
            {
                GameObject go1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1300/FirstRes/BigWin1300");
                m_Gold_EffectNew = go1.GetComponent<BigWin1300>();
                m_Gold_EffectNew.gameObject.SetActive(false);
                m_Gold_EffectNew.transform.SetParent(m_Trans_Effect, true);
                m_Gold_EffectNew.gameObject.SetActive(true);
                m_Gold_EffectNew.transform.localScale = new Vector3(1, 1, 1);
                m_Gold_EffectNew.transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        public void EnterGame()
        {
            m_Trans_GoFreeTimes.gameObject.SetActive(true);
            Game1300Model.Instance.toSpin.nModelGame = Game1300Model.Instance.nModelGame;
            SlotCpt.freeTimes.max = Game1300Model.Instance.toSpin.nModelGame;
            Game1300Model.Instance.toSpin.FreeTimes = Game1300Model.Instance.nFreeGame;
            SlotCpt.commonTop.UpDateScore(Game1300Model.Instance.toSpin.n64FreeGold);
            m_Txt_Times.text = Game1300Model.Instance.toSpin.FreeTimes + "/" + Game1300Model.Instance.toSpin.nModelGame;
            CoreEntry.gTimeMgr.AddTimer(1, false, () =>
            {
                if (SlotCpt.StateSlot == slotState.Idle)
                    SlotCpt.continueSpin();
            }, 24121);
        }

        public void RegisterListener()
        {
        }

        public void UnRegisterListener()
        {
        }

        public async void InitData()
        {
            SlotCpt.commonTop.UpDateScore(0);
            Game1300Model.Instance.toSpin.n64Gold = SlotCpt.bDanJi ? PlayerPrefs.GetInt("DanJi", 500000) : MainUIModel.Instance.Golds;
            SlotCpt.commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            InitGame();
        }


        public void ClickBtnRank()
        {
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < Game1300Model.Instance.arrayAward.Count; i++)
            {
                if (Game1300Model.Instance.arrayAward[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(Game1300Model.Instance.arrayAward[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList, 1300);
        }

        public void ClickBtnHelp()
        {
            MainPanelMgr.Instance.ShowDialog("UIRoom1300Help");
        }




        public void SetTxtScore(long score)
        {
            CoreEntry.gAudioMgr.StopSound(m_Trans_Sound.gameObject);
            if (SlotCpt.isFreeSpin && Game1300Model.Instance.toSpin.n64FreeGold > 0)
                return;
            SlotCpt.commonTop.UpDateScore(score);
        }

        public void InitGame()
        {
            SlotCpt.commonTop.InitGame(Game1300Model.Instance.GearList, Game1300Model.Instance.nBet);
            SlotCpt.setState(slotState.Idle);
            SetTopRank();
        }

        protected void SetTopRank()
        {
            if (Game1300Model.Instance.arrayAward.Count >= 1)
            {
                if (Game1300Model.Instance.arrayAward[0].n64Gold <= 0)
                    return;
                SlotCpt.commonTop.SetTopRank(CommonTools.BytesToString(Game1300Model.Instance.arrayAward[0].szName), Game1300Model.Instance.arrayAward[0].n64Gold, Game1300Model.Instance.arrayAward[0].nIconID);
            }
        }


        protected override void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_BetGameRet1300, BetGameRet1300);// -- 1300房间下注结果返回
            Message.RemoveListener(MessageName.GE_BroadCast_JackpotList1300, RefreshJACKPOT1300);// -- 1300房间下注结果返回
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            UnRegisterListener();
        }


        public void BeginSlot(int num, bool isOn)
        {
            SlotCpt.beginSpin(num, isOn);
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {
        }

        public void OnSlotWinGold()
        {
            winGold = Game1300Model.Instance.toSpin.WinGold;
            GetSpine_light().gameObject.SetActive(true);

            if (Game1300Model.Instance.toSpin.rate < 2 && Game1300Model.Instance.nDoublePower <= 0)
            {
                SlotCpt.commonTop.UpDateScore(winGold);
                SetBottomIcons();
                CoreEntry.gAudioMgr.PlayUISound(160, m_Trans_Sound.gameObject);
                CoreEntry.gTimeMgr.AddTimer(0.2f, false, () =>
                {
                    SlotCpt.commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold);
                }, 7);
                return;
            }
            if (Game1300Model.Instance.nDoublePower <= 0)
                CoreEntry.gTimeMgr.AddTimer(1.5f, false, () => { BigWinAni(); }, 22);
        }

        public void SetBottomIcons()
        {
            m_Trans_Win.gameObject.SetActive(true);
            m_TxtM_Score2.text = "R$" + ToolUtil.ShowF2Num2(winGold);
            CoreEntry.gTimeMgr.RemoveTimer(284);
            for (int i = 0; i < elementImagList.Count; i++)
            {
                int ele = SlotCpt.slotColumns[i].lstCells[0].element;
                if (Game1300Model.Instance.awardList.Contains(ele))
                {
                    elementImagList[i].gameObject.SetActive(true);
                    elementImagList[i].sprite = AtlasSpriteManager.Instance.GetSprite("Game1300_2:" + "Tb" + ele);
                }
                else
                    elementImagList[i].gameObject.SetActive(false);
            }
            CoreEntry.gTimeMgr.AddTimer(1.5f, true, () =>
            {
                m_Trans_Win.gameObject.SetActive(false);

                CoreEntry.gTimeMgr.AddTimer(0.5f, false, () =>
                {
                    m_Trans_Win.gameObject.SetActive(true);
                    for (int i = 0; i < elementImagList.Count; i++)
                    {
                        int ele = SlotCpt.slotColumns[i].lstCells[0].element;
                        if (Game1300Model.Instance.awardList.Contains(ele))
                        {
                            elementImagList[i].gameObject.SetActive(true);
                            elementImagList[i].sprite = AtlasSpriteManager.Instance.GetSprite("Game1300_2:" + "Tb" + ele);
                        }
                        else
                        {
                            elementImagList[i].gameObject.SetActive(false);
                        }
                    }
                }, 285);
            }, 284);
        }

        public void BigWinAni(Action callBack = null)
        {
            m_Trans_Effect.gameObject.SetActive(true);
            gold = Game1300Model.Instance.toSpin.WinGold;
            if (false)
                playGoldType = 4;
            else
            {
                if (Game1300Model.Instance.toSpin.rate >= 2 && Game1300Model.Instance.toSpin.rate <= 15)
                    playGoldType = 2;
                else if (Game1300Model.Instance.toSpin.rate > 15 && Game1300Model.Instance.toSpin.rate <= 30)
                    playGoldType = 3;
                else if (Game1300Model.Instance.toSpin.rate > 30 && Game1300Model.Instance.toSpin.rate <= 50)
                    playGoldType = 4;
                else
                    playGoldType = 2;
            }

            m_Gold_EffectNew.setData(playGoldType, gold, () =>
            {
                callBack?.Invoke();
                ClickBtnCloseEffect();
            }, SlotCpt.autoSpinNum != 0, m_Trans_Sound.gameObject);
        }
        public void ClickBtnCloseEffect()
        {
            SlotCpt.commonTop.UpDateScore(winGold);
            CoreEntry.gTimeMgr.RemoveTimer(110000);
            SlotCpt.commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold);
            SetBottomIcons();
            if (Game1300Model.Instance.nDoublePower > 0)
                m_TxtM_Score2.text = ToolUtil.ShowF2Num2(winGold / Game1300Model.Instance.nDoublePower) + "X" + Game1300Model.Instance.nDoublePower + "=" + ToolUtil.ShowF2Num2(winGold);
            CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            {
                m_Trans_Effect.gameObject.SetActive(false);
                SlotCpt.continueSpin();
            }, 100000);

        }





        public void OnBetChangeCallBack(int bet)
        {
            Game1300Model.Instance.nBet1 = bet;
        }

        public void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {

        }

        //public void SetFreeTimes()
        //{
        //    m_Trans_GoFreeTimes.gameObject.SetActive(true);
        //    m_Txt_Times.text = Game1300Model.Instance.toSpin.FreeTimes + "/" + Game1300Model.Instance.toSpin.nModelGame;
        //}

        public void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {

        }

        public void BetGameRet1300()
        {
            SlotCpt.StartRoll();
            SlotCpt.SetSpinData();
        }

        public void RefreshJACKPOT1300()
        {
            SetTopRank();
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
        }

        public void UpdateGold(long gold)
        {
            Game1300Model.Instance.toSpin.n64Gold = gold;
            SlotCpt.commonTop.UpdateGold(gold);
        }

 

        public Transform GetTrans_Win()
        {
            return m_Trans_Win;
        }
        public Transform GetSpine_light()
        {
            return m_Spine_light.transform;
        }
    }
}
