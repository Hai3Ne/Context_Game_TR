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
    public partial class UIRoom900 : UIRoom_SlotCpt
    {
        public Top900Panel uitop900;
        public CommonTop commonTop;
        public UIRoom900SmallGame smallGame;
        float delayTimes = 0.5f;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game900Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(66);
            if (uitop900 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject common = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
                commonTop = common.GetComponent<CommonTop>();
                commonTop.SetDanJi(bDanJi, true, true);
                commonTop.SlotCpt = this;

                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game900/FirstRes/Top900Panel");
                uitop900 = go.GetComponent<Top900Panel>();
                uitop900.gameObject.SetActive(true);
                uitop900.Init(this);
                uitop900.transform.SetAsLastSibling();
            }
            uitop900.InitData();
            EnterGame();
            gameStatus = 0;
            num = 0;
            CoreEntry.gTimeMgr.AddTimer(0.2f, false, () => { ToolUtil.PlayAnimation(m_Spine_bgEffect.transform, "a1", true); },88);
        }

        private void EnterGame()
        {
            if (Game900Model.Instance.nModelGame > 0 && Game900Model.Instance.nFreeGame > 0)
                uitop900.EnterGame();
        }

        public void Reconnect()
        {
            if (uitop900 != null)
            {
                commonTop.SetRollBtnRorate(false);
                InitData();
                commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public void OpenSmallGame()
        {
            if(smallGame == null)
            {
                GameObject go1 = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game900/FirstRes/UIRoom900SmallGame");
                smallGame = go1.GetComponent<UIRoom900SmallGame>();
                smallGame.uiroom900 = this;
            }
            smallGame.gameObject.SetActive(true);
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_900.column; i++)
            {
                UIRomm900SlotColumn column = lstcolumns[i].GetComponent<UIRomm900SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < 4;j++)
                {
                    UIRoom900SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom900SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }

        }

        public override void preSpin()
        {
            uitop900.SetTxtScore(0);
            CoreEntry.gTimeMgr.RemoveTimer(399);
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
                int num = commonTop.Bet /30;
                //Debug.LogError("<<<<<<<<<<<<<<<<<<<<<"+ num);
                Game900Ctrl.Instance.Send_CS_GAME8_BET_REQ(num);
            }
        }

        public override void RandomSpinData()
        {
            Game900Model.Instance.RandomSpinData();
        }

        public override void SetSpinData()
        {        
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_900.rollTimes + i * 2;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game900Model.Instance.toSpin == null)
                return;
            base.finishSpin();

            setState(slotState.SpinEnd);

            //-- 获得免费
            if (Game900Model.Instance.toSpin.FreeTimes > 0)
            {
                freeTimes.max = Game900Model.Instance.toSpin.FreeTimes;
                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game900Model.Instance.toSpin.WinGold;

                if (!isFreeSpin && Game900Model.Instance.toSpin.nModelGame == Game900Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                else if (Game900Model.Instance.lastCount > 0 && Game900Model.Instance.toSpin.nModelGame > Game900Model.Instance.lastCount)
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                isFreeSpin = true;
                Game900Model.Instance.lastCount = Game900Model.Instance.toSpin.nModelGame;
            }
     
            for (int i = 0; i < SlotData_900.column; i++)
                slotColumns[i].onSpinFinish(Game900Model.Instance.toSpin.WinGold > 0);
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);
            HnadleSteps();
            //-- 免费结束
            if (isFreeSpin && (Game900Model.Instance.toSpin.nModelGame == Game900Model.Instance.toSpin.FreeTimes) && Game900Model.Instance.toSpin.FreeTimes == 0)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
                Game900Model.Instance.lastCount = 0;
            }

        }

        public override void showLines()
        {
            ShowAllCell(false);
            if (Game900Model.Instance.bHasSmallGame)
                ShowSpecialElement(false);
            for (int i = 0; i < Game900Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game900Model.Instance.lines[i];

                List<int> elementPos = Game900Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    int temp = (j) * 3 + elementPos[j];
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].index = temp -1;
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].showLine();
                }
            }
            ShowAllCell(true, true);
            if (Game900Model.Instance.bHasSmallGame)
                ShowSpecialElement(true);
            int index = Game900Model.Instance.bHasSmallGame ? -1 : 0;
            CoreEntry.gTimeMgr.AddTimer(1.5f, true, () =>
            {
                ShowAllCell(false);
                if (Game900Model.Instance.bHasSmallGame)
                    ShowSpecialElement(false);
                if(index == -1)
                {
                    if (Game900Model.Instance.bHasSmallGame)
                        ShowSpecialElement(true);
                    index++;
                }
                else if (index == Game900Model.Instance.lines.Count)
                {
                    index = Game900Model.Instance.bHasSmallGame? - 1:0;
                    CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                    {
                        ShowAllCell(true, true);
                    }, 400);
                }
                else
                {
                    KeyValuePair<int, int> tempLine = Game900Model.Instance.lines[index];
                    List<int> elementPos = Game900Model.Instance.lineData[tempLine.Key];
                    List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                    temp.Add(tempLine);
                    float times = 0;
         
                    for(int i = 0;i < tempLine.Value;i++)
                    {

                        float tempTimes = ToolUtil.GetAnimationDuration(slotColumns[i].lstCells[2 - elementPos[i] + 1].TfSpine.GetChild(0), "a2");

                       // float tempTimes = slotColumns[i].lstCells[2 - elementPos[i] + 1].TfSpine.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>().animation.GetState("a2")._animationData.duration;
                        if (tempTimes > times)
                            times = tempTimes;
                    }
                    CoreEntry.gTimeMgr.AddTimer(1.5f- times, false, () =>
                    {
                        for (int j = 0; j < tempLine.Value; j++)
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);
                        index++;
                    }, 400);
                }
            }, 399);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game900Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game900Model.Instance.lines[i];
                List<int> elementPos = Game900Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }
        }

        private void ShowSpecialElement(bool bShow)
        {
            for (int i = 0; i < slotColumns.Count; i++)
            {
                for (int j = 0; j < slotColumns[i].lstCells.Count - 1; j++)
                {
                    UISlotCell cell = slotColumns[i].lstCells[j];
                    int element = cell.element;
                    if (cell.TfSpine.childCount > 0 && element >= 14 && element <= 17)
                        ToolUtil.PlayAnimation(cell.TfSpine.GetChild(0).transform, bShow ? "a2" : "a1", false);
                   // CommonTools.PlayArmatureAni(cell.TfSpine.GetChild(0).transform, bShow ? "a2" : "a1", 1, () => { });
                }
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game900Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game900Model.Instance.slotResult[cr2i(c, r)];
        }

        public GameObject GetSoundObj()
        {
            return m_Trans_Sound.gameObject;
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            if (uitop900.m_Gold_EffectNew.gameObject.activeSelf)
            {
                uitop900.m_Gold_EffectNew.callback?.Invoke();
                uitop900.m_Gold_EffectNew.callback = null;
                uitop900.m_Gold_EffectNew.gameObject.SetActive(false);
            }

            num = num0;
            //--手动或免费时num为0
            isFastSpin = false;
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true, Game900Model.Instance.toSpin.FreeTimes+"");
            }
            else
            {
                if (!commonTop.GetGoFreeTimes().gameObject.activeSelf)
                {
                    if (commonTop.Bet == commonTop.betMin && Game900Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        commonTop.SetSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        commonTop.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game900Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                        {
                            int tempBet1 = commonTop.Bet;
                            for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = commonTop.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    commonTop.OnClickBtnMin();
                                    if (Game900Model.Instance.toSpin.n64Gold >= (long)(tempBet ))
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
            for (int i = 0; i < SlotData_900.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
            }
         
            if (Game900Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = commonTop.Bet;
                commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold - betValue);
            }
            setState(slotState.SpinBegin);
            Game900Model.Instance.bShowFreeAni = false;
            preSpin();
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
            if (Game900Model.Instance.toSpin.FreeTimes == 0)
                freeTimes.reset();
            commonTop.UpdateGold(Game900Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (freeTimes.max > 0)
            {
                beginSpin(0, isFastSpin);
            }
            else if (autoSpinNum >0)
            {
                if (autoSpinNum >0)
                    beginSpin(autoSpinNum, isFastSpin);
                else
                    isFastSpin = false;
            }
            else
                isFastSpin = false;
        }

        public override bool isFreeEnd()
        {
            return isFreeSpin && Game900Model.Instance.toSpin.FreeTimes == 0; 
           // Debug.LogError("----"+ isFreeSpin+"===="+ (Game900Model.Instance.toSpin.nModelGame == Game900Model.Instance.toSpin.FreeTimes)+"=====" + Game900Model.Instance.toSpin.FreeTimes);
            //return (isFreeSpin && (Game900Model.Instance.toSpin.nModelGame == Game900Model.Instance.toSpin.FreeTimes) && Game900Model.Instance.toSpin.FreeTimes == 0);
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


        public void HnadleSteps()
        {
            if (Game900Model.Instance.toSpin.WinGold > 0 || Game900Model.Instance.toSpin.n64FreeGold > 0 || (Game900Model.Instance.n64JackPotGold > 0 && Game900Model.Instance.ucRSID==0)) 
            {
                Game900Model.Instance.toSpin.WinGold += Game900Model.Instance.n64JackPotGold;

                showLines();
                //if (isFreeSpin)
                //    Game900Model.Instance.toSpin.WinGold += Game900Model.Instance.toSpin.n64FreeGold;
                uitop900.OnSlotWinGold(HandleNormalJackpot, ContinueGame);
            }
            else 
            {
                HandleNormalJackpot();
            }
        }

        private void HandleNormalJackpot()
        {
            bool bHasJackpot = Game900Model.Instance.n64JackPotGold > 0 && Game900Model.Instance.ucRSID > 5;///普通奖池
            if (bHasJackpot )
            {
                bool bEnd = Game900Model.Instance.bHasSmallGame ? false : true;///是否可以直接结束 
                uitop900.BigWinAni(Game900Model.Instance.n64JackPotGold, bEnd, true, OpenSmallGame, ContinueGame);
            }
            else
            {
                if(Game900Model.Instance.bHasSmallGame)
                    OpenSmallGame();
                else
                    ContinueGame();
            }
        }

        public void ContinueGame()
        {
            if (Game900Model.Instance.bShowFreeAni || isFreeEnd() || Game900Model.Instance.bHasSmallGame)//免费时 免费结束 小游戏 奖池
                return;
            if (Game900Model.Instance.toSpin.WinGold > 0)
                delayTimes = 0.7f;
            else
                delayTimes = 0.5f;
            if (isFreeSpin)//免费没有动画
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () =>{continueSpin();}, 100000);
            else
            {
                //if (Game900Model.Instance.toSpin.rate > 2)
                //    return;
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () =>{continueSpin();}, 100000);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(66);
        }
    }

}


  