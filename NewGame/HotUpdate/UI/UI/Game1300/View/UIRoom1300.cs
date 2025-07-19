using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class UIRoom1300 : UIRoom_SlotCpt
    {
        public CommonTop commonTop;
        public Top1300Panel uitop1300;
        public smallGame1300 smallGame;
        public int bgSound = 0;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1300Model.Instance.InitConfig();
            bgSound = 147;
            CoreEntry.gAudioMgr.PlayUIMusic(bgSound);
            if (uitop1300 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject common = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
                commonTop = common.GetComponent<CommonTop>();
                commonTop.SetDanJi(bDanJi, true, false);
                commonTop.SlotCpt = this;

                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1300/FirstRes/Top1300Panel");
                uitop1300 = go.GetComponent<Top1300Panel>();
                uitop1300.gameObject.SetActive(true);
                uitop1300.Init(this);

                GameObject go1 = CommonTools.AddSubChild(m_Trans_smallGame.gameObject, "UI/Prefabs/Game1300/FirstRes/smallGame");
                smallGame = go1.GetComponent<smallGame1300>();
                smallGame.gameObject.SetActive(true);
                smallGame.uiroom1300 = this;
            }
            uitop1300.InitData();
            EnterGame();
            gameStatus = 0;
            num = 0;

            
        }

        private void EnterGame()
        {
            if (Game1300Model.Instance.nModelGame > 0 && Game1300Model.Instance.nFreeGame > 0)
                uitop1300.EnterGame();
        }

        public void Reconnect()
        {
            if (uitop1300 != null)
            {
                InitData();
                commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_1300.column; i++)
            {
                UIRomm1300SlotColumn column = lstcolumns[i].GetComponent<UIRomm1300SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < column.transform.childCount; j++)
                {
                    UIRoom1300SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1300SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }

        }

        public override void preSpin()
        {

            uitop1300.SetTxtScore(0);
            CoreEntry.gTimeMgr.RemoveTimer(288);
            CoreEntry.gTimeMgr.RemoveTimer(277);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(284);
            CoreEntry.gTimeMgr.RemoveTimer(285);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);

            if (bDanJi)
            {
                setState(slotState.SpinBegin);
                awaiting = true;
                RandomSpinData();
                CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { recSpin(); }, 1000);
            }

        }

        public override void sendSpin()
        {
            if (bDanJi == false)
            {
                setState(slotState.SpinBegin);
                gameStatus = 1;
                int num = commonTop.Bet;
               // Debug.LogError("<<<<<<<<<<<<<<<<<<<<<"+ num);
                Game1300Ctrl.Instance.Send_CS_GAME9_BET_REQ(num);
            }
        }

        public override void RandomSpinData()
        {
            Game1300Model.Instance.RandomSpinData();
        }

        public override void SetSpinData()
        {        
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_1300.rollTimes + i * 3;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game1300Model.Instance.toSpin == null)
                return;
            base.finishSpin();

            setState(slotState.SpinEnd);

            //-- 获得免费
            if (Game1300Model.Instance.toSpin.FreeTimes > 0)
            {
                freeTimes.max = Game1300Model.Instance.toSpin.FreeTimes;
                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game1300Model.Instance.toSpin.WinGold;

                if (!isFreeSpin && Game1300Model.Instance.toSpin.nModelGame == Game1300Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                else if (Game1300Model.Instance.lastCount > 0 && Game1300Model.Instance.toSpin.nModelGame > Game1300Model.Instance.lastCount)
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                isFreeSpin = true;
                Game1300Model.Instance.lastCount = Game1300Model.Instance.toSpin.nModelGame;
            }
     
            for (int i = 0; i < SlotData_1300.column; i++)
                slotColumns[i].onSpinFinish(Game1300Model.Instance.toSpin.WinGold > 0);
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);
            StepWinGold();
            OpenSmallGame();
            ContinueGame();
            //-- 免费结束
            if (isFreeSpin && (Game1300Model.Instance.toSpin.nModelGame == Game1300Model.Instance.toSpin.FreeTimes) && Game1300Model.Instance.toSpin.FreeTimes == 0)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
                Game1300Model.Instance.lastCount = 0;
            }

            CoreEntry.gTimeMgr.AddTimer(5,false,()=> 
            {
                bgSound = 158;
                CoreEntry.gAudioMgr.PlayUIMusic(bgSound);
            },277);
        }

        public override void showLines()
        {
            //ShowAllCell(true,true);
            float times = 0;
            float tempTimes = 1;
            //string aniName = "a2";
            for (int i = 0; i < SlotData_1300.column; i++)
            {
                CoreEntry.gAudioMgr.PlayUISound(156);
                UISlotCell cell = slotColumns[i].lstCells[0];
                if (Game1300Model.Instance.awardList.Contains(cell.element))
                {
                    cell.showLine();
                    tempTimes = ToolUtil.GetAnimationDuration(cell.TfSpine.GetChild(0), "win");
                }
                if (tempTimes > times)
                    times = tempTimes;
            }


            CoreEntry.gTimeMgr.AddTimer(times + 0.2f, true, () =>
               {
                //ShowAllCell(false);
                for (int i = 0; i < SlotData_1300.column; i++)
                   {
                       UISlotCell cell = slotColumns[i].lstCells[0];
                       if (Game1300Model.Instance.awardList.Contains(cell.element))
                           cell.ShowCellEffect(true);
                   }

                   CoreEntry.gAudioMgr.PlayUISound(156);
               }, 288);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < SlotData_1300.column; i++)
            {
                UISlotCell cell = slotColumns[i].lstCells[0];
                if (Game1300Model.Instance.awardList.Contains(cell.element))
                    cell.ShowCellEffect(bShow, bAllTrue);
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1300Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game1300Model.Instance.slotResult[cr2i(c, r)];
        }

        public GameObject GetSoundObj()
        {
            return m_Trans_Sound.gameObject;
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            if (uitop1300.m_Gold_EffectNew.gameObject.activeSelf)
            {
                uitop1300.m_Gold_EffectNew.callback?.Invoke();
                uitop1300.m_Gold_EffectNew.callback = null;
                uitop1300.m_Gold_EffectNew.gameObject.SetActive(false);
            }
            uitop1300.GetTrans_Win().gameObject.SetActive(false);
            uitop1300.GetSpine_light().gameObject.SetActive(false);
            num = num0;
            //--手动或免费时num为0
            isFastSpin = false;
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true, Game1300Model.Instance.toSpin.FreeTimes+"");
            }
            else
            {
                if (!commonTop.GetGoFreeTimes().gameObject.activeSelf)
                {
                    if (commonTop.Bet == commonTop.betMin && Game1300Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        commonTop.SetSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        commonTop.GetAutoToggle().isOn = false;
                        commonTop.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game1300Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                        {
                            int tempBet1 = commonTop.Bet;
                            for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = commonTop.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    commonTop.OnClickBtnMin();
                                    if (Game1300Model.Instance.toSpin.n64Gold >= (long)(tempBet ))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if (!bCanSlot)
                            {
                                autoSpinNum = 0;// -- 停止自动
                                commonTop.SetSlotSpinNum(autoSpinNum);
                                setState(slotState.Idle);
                                commonTop.GetAutoToggle().isOn = false;
                                commonTop.ShowNotEnoughMoneyTips();
                                return;
                            }
                        }
                    }
                }

                isFreeSpin = false;
                if (num > 0)
                {
                    autoSpinNum = num - 1;
                    commonTop.SetSlotSpinNum(autoSpinNum);
                }
                else
                {
                    autoSpinNum = num;
                       commonTop.SetSlotSpinNum(autoSpinNum);
                }
            }

            preSpin();
            for (int i = 0; i < SlotData_1300.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
            }       
            if (Game1300Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = commonTop.Bet;
                commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold - betValue);
            }
            setState(slotState.SpinBegin);
            Game1300Model.Instance.bShowFreeAni = false;
       

            if(bgSound != 147)
            {
                bgSound = 147;
                CoreEntry.gAudioMgr.PlayUIMusic(bgSound);
            }

            //CoreEntry.gAudioMgr.PlayUISound(41);
            sendSpin();
            commonTop.SetRollBtnRorate(true);
        }

        public void StartRoll()
        {
            gameStatus = 2;
            if (awaiting) //防止连点
                return;
            awaiting = true;
       
        }

        public override void continueSpin()
        {
            //普通或免费结束后再加金币
            if (Game1300Model.Instance.toSpin.FreeTimes == 0)
                freeTimes.reset();
            commonTop.UpdateGold(Game1300Model.Instance.toSpin.n64Gold);

            setState(slotState.Idle);
            if (freeTimes.max > 0)
            {
                beginSpin(0, isFastSpin);
            }
            else if (autoSpinNum != 0)
            {
                if (autoSpinNum != 0)
                    beginSpin(autoSpinNum, isFastSpin);
                else
                    isFastSpin = false;
            }
            else
                isFastSpin = false;
        }

        public override bool isFreeEnd()
        {
            return isFreeSpin && Game1300Model.Instance.toSpin.FreeTimes == 0; 
           // Debug.LogError("----"+ isFreeSpin+"===="+ (Game1300Model.Instance.toSpin.nModelGame == Game1300Model.Instance.toSpin.FreeTimes)+"=====" + Game1300Model.Instance.toSpin.FreeTimes);
            //return (isFreeSpin && (Game1300Model.Instance.toSpin.nModelGame == Game1300Model.Instance.toSpin.FreeTimes) && Game1300Model.Instance.toSpin.FreeTimes == 0);
        }
  
        public override void InitData()
        {
            for(int i = 0;i < slotColumns.Count;i++ )
            {
                if (slotColumns.Count == 0)
                    break;
                if (slotColumns[i].tweenMove != null)
                    slotColumns[i].KillTweener();
                slotColumns[i].initCell();
                slotColumns[i].reset();
            }
            setState(slotState.Idle);
        }

        public override void finishRoll(int column)
        {
            if (column == columnCount - 1)
            { //最后一列结束
                CoreEntry.gTimeMgr.RemoveTimer(9);
                commonTop.SetRollBtnRorate(false);
                CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>
                {
                    finishSpin();
                }, 9);
            }
            else if (column > 0)
            { //出现2个免费，下一列加速
                if (!isFastSpin && false)
                {
                    //下一列加速
                    slotColumns[column + 1].playAcc();
                    //再后列等待
                    for (int i = column + 2; i < slotColumns.Count; i++)
                    {
                        slotColumns[i].endRoll(-1);
                    }
                    setState(slotState.SpinStop);
                }
            }
        }


        public void StepWinGold()
        {
            if (Game1300Model.Instance.toSpin.WinGold > 0 || Game1300Model.Instance.lines.Count > 0)
            {
                if(Game1300Model.Instance.nDoublePower <=0)
                    showLines();
                if (isFreeSpin)
                    freeTimes.gold = Game1300Model.Instance.toSpin.WinGold + freeTimes.gold;
                uitop1300.OnSlotWinGold();
            }
        }

        private void OpenSmallGame()
        {
            if (Game1300Model.Instance.nDoublePower > 0)
                smallGame.StartRoll();
        }

        public void ContinueGame()
        {
            if (Game1300Model.Instance.toSpin.rate >= 2 || Game1300Model.Instance.nDoublePower > 0)
                return;
            CoreEntry.gTimeMgr.AddTimer(Game1300Model.Instance.toSpin.WinGold >0?1:0.5f, false, () => { continueSpin(); }, 100000);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(bgSound);
        }
    }

}


  