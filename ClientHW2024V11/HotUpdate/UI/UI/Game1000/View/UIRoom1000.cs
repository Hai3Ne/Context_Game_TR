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
    public partial class UIRoom1000 : UIRoom_SlotCpt
    {
        public CommonTop commonTop;
        public Top1000Panel uitop1000;
        public List<Transform> smallGamelstcolumns = new List<Transform>();
        smallGame1 smallGame1;
        smallGame2 smallGame2;
        smallGame3 smallGame3;
        DiceGames DiceGames;
        DoubleGames1000 doubleGame;

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            slotRow = 3;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1000Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(91);
            if (uitop1000 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject common = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
                commonTop = common.GetComponent<CommonTop>();
                commonTop.SetDanJi(bDanJi, true, true,false,30);
                commonTop.SlotCpt = this;

                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1000/FirstRes/Top1000Panel");
                uitop1000 = go.GetComponent<Top1000Panel>();
                uitop1000.gameObject.SetActive(true);
                uitop1000.Init(this);
            }
            uitop1000.InitData();
            gameStatus = 0;
            num = 0;

            Application.targetFrameRate = 90;
        }

 

        public void Reconnect()
        {
            if (uitop1000 != null)
            {
                commonTop.SetRollBtnRorate(false);
                InitData();
                commonTop.UpdateGold(Game1000Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_1000.column; i++)
            {
                UIRomm1000SlotColumn column = lstcolumns[i].GetComponent<UIRomm1000SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for (int j = 0; j < column.transform.childCount; j++)
                {
                    UIRoom1000SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1000SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public async override void preSpin()
        {
            uitop1000.SetTxtScore(0);
            uitop1000.SetPressJoga(true);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            uitop1000.ShowAllLines(false);
            if (bDanJi)
            {
                setState(slotState.SpinBegin);
                awaiting = true;
                RandomSpinData();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));
                recSpin();
            }

        }

        public override void sendSpin()
        {
            if (bDanJi == false)
            {
                //setState(slotState.SpinBegin);
                gameStatus = 1;
                float num = commonTop.Bet;
                Game1000Ctrl.Instance.Send_CS_GAME11_BET_REQ((int)num);
            }
        }

        public override void RandomSpinData()
        {
            Game1000Model.Instance.RandomSpinData();
        }

        public override void SetSpinData()
        {
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_1000.rollTimes + i * 1;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public async override void finishSpin()
        {
            if (Game1000Model.Instance.toSpin == null)
                return;
            base.finishSpin();
            setState(slotState.SpinEnd);

            for (int i = 0; i < SlotData_1000.column; i++)
                slotColumns[i].onSpinFinish(Game1000Model.Instance.toSpin.WinGold > 0);
            StepWinGold();
            if (Game1000Model.Instance.toSpin.rate > 1.5f || Game1000Model.Instance.nModelGame > 0)
                return;
            if(Game1000Model.Instance.toSpin.WinGold > 0)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1.2f));
            else
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.7f));
            continueSpin();
        }

        public async override void showLines()
        {      
            ShowAllCell(false);
            if (bDanJi)
                ShowDanJiLine();
            else
            {
                for (int i = 0; i < Game1000Model.Instance.lines2.Count; i++)
                {
                    int line = Game1000Model.Instance.lines2[i].Key;
                    int tempElement = Game1000Model.Instance.GetElementByLine(line);
                    List<int> posList = Game1000Model.Instance.GetElementPosByLine(Game1000Model.Instance.lines2[i].Value);
                    int lineType = Game1000Model.Instance.GetLineTypeByLine(Game1000Model.Instance.lines2[i].Value);
                    if (lineType == 0)
                    {
                        PlayUISound(tempElement);
                        for (int j = 0; j < 3; j++)
                            slotColumns[line - 21].lstCells[j].showLine();
                    }
                    else
                    {
                        List<int> elementPos = Game1000Model.Instance.lineData[line];
                        PlayUISound(tempElement);
                        for (int j = 0; j < 5; j++)
                        {
                            if (posList[j] == 0)
                                continue;
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].showLine();
                        }
                    }
                    uitop1000.ShowOneLine(line - 1);
                }

                float times = GetAniTimes();
                ShowAllCell(true, true);
                int index = 0;
                float extraTimes = 0.5f;
                CoreEntry.gTimeMgr.AddTimer(times + extraTimes, true, () =>
                {
                    if (Game1000Model.Instance.lines2.Count > 1)
                        uitop1000.ShowAllLines(false);
                    ShowAllCell(false);             
                    if (index == Game1000Model.Instance.lines2.Count)
                    {
                        index = 0;
                        CoreEntry.gTimeMgr.AddTimer(extraTimes, false, () =>
                        {
                            ShowAllCell(true, true);
                            for (int i = 0; i < Game1000Model.Instance.lines2.Count; i++)
                                uitop1000.ShowOneLine(Game1000Model.Instance.lines2[i].Key - 1);
                        }, 400);
                    }
                    else
                    {
                        KeyValuePair<int, int> tempLine = Game1000Model.Instance.lines2[index];
                        List<int> elementPos = Game1000Model.Instance.lineData[tempLine.Key];
                        List<int> posList = Game1000Model.Instance.GetElementPosByLine(tempLine.Value);
                  
                        int lineType = Game1000Model.Instance.GetLineTypeByLine(tempLine.Value);
                        if (lineType == 0)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                float tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[tempLine.Key - 21].lstCells[j].TfSpine.GetChild(0),"a2"); 
                                if (tempTimes > times)
                                    times = tempTimes;
                            }
                        }
                        else
                        {
                         
                            for (int j = 0; j < 5; j++)
                            {
                                if (posList[j] == 0)
                                    continue;
                                float tempTimes = 0;

                                tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[j].lstCells[2 - elementPos[j] + 1].TfSpine.GetChild(0),"a2"); 
                                if (tempTimes > times)
                                    times = tempTimes;
                            }
                        }

                        CoreEntry.gTimeMgr.AddTimer(extraTimes, false, () =>
                        {
                            uitop1000.ShowOneLine(tempLine.Key - 1);
                            if (lineType == 0)
                            {
                                for (int j = 0; j < 3; j++)
                                    slotColumns[tempLine.Key - 21].lstCells[j].ShowCellEffect(true);
                            }
                            else
                            {
                                for (int j = 0; j <5; j++)
                                {
                                    if (posList[j] == 0)
                                        continue;

                                    slotColumns[j].lstCells[2 - elementPos[j] +1].ShowCellEffect(true);
                                }
                            }
                            index++;
                        }, 400);
                    }
                }, 399);
            }           
        }

        private float GetAniTimes()
        {
            float times = 0;
            for (int i = 0; i < Game1000Model.Instance.lines2.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1000Model.Instance.lines2[i];

                if (tempLine.Key >= 21 && tempLine.Key <= 25)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        float tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[tempLine.Key - 21].lstCells[j].TfSpine.GetChild(0),"a2");
                        if (tempTimes > times)
                            times = tempTimes;
                    }
                }
                else
                {
                    List<int> posList = Game1000Model.Instance.GetElementPosByLine(tempLine.Value);
                    List<int> elementPos = Game1000Model.Instance.lineData[tempLine.Key];
                    for (int j = 0; j < 5; j++)
                    {
                        if (posList[j] == 0)
                            continue;
                        float tempTimes = 0;
                        tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[j].lstCells[2 - elementPos[j] + 1].TfSpine.GetChild(0),"a2"); 
                        if (tempTimes > times)
                            times = tempTimes;
                    }
                }
            }
            return times;
        }

        private void ShowDanJiLine()
        {
            for (int i = 0; i < Game1000Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, List<int>> tempLine = Game1000Model.Instance.lines[i];
                if (tempLine.Key >= 21 && tempLine.Key <= 25)
                {
                    PlayUISound(Game1000Model.Instance.slotResult[tempLine.Value[0]]);
                    for (int j = 0; j < 3; j++)
                        slotColumns[tempLine.Key - 21].lstCells[j].showLine();
                }
                else
                {
                    List<int> elementPos = Game1000Model.Instance.lineData[tempLine.Key];
                    PlayUISound(Game1000Model.Instance.slotResult[tempLine.Value[0]]);
                    for (int j = 0; j < tempLine.Value.Count; j++)
                    {
                        int col = tempLine.Value[j] / 3;
                        int row = tempLine.Value[j] % 3;
                        slotColumns[col].lstCells[2 - row].index = j;
                        slotColumns[col].lstCells[2 - row].showLine();
                    }
                }
                uitop1000.ShowOneLine(tempLine.Key - 1);
            }
            ShowAllCell(true, true);
            int index = 0;
            CoreEntry.gTimeMgr.AddTimer(2.1f, true, () =>
            {
                if (Game1000Model.Instance.lines.Count > 1)
                    uitop1000.ShowAllLines(false);
                ShowAllCell(false);
                float times = 2;
                if (index == Game1000Model.Instance.lines.Count)
                {
                    for (int i = 0; i < Game1000Model.Instance.lines.Count; i++)
                    {
                        KeyValuePair<int, List<int>> tempLine = Game1000Model.Instance.lines[i];

                        if (tempLine.Key >= 21 && tempLine.Key <= 25)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                float tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[tempLine.Key - 21].lstCells[j].TfSpine.GetChild(0), "a2");
                                if (tempTimes > times)
                                    times = tempTimes;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < tempLine.Value.Count; j++)
                            {
                                float tempTimes = 0;
                                int col = tempLine.Value[j] / 3;
                                int row = tempLine.Value[j] % 3;
                                if (slotColumns[col].lstCells[2 - row].TfSpine.childCount > 0)
                                    tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[col].lstCells[2 - row].TfSpine.GetChild(0), "a2");
                                if (tempTimes > times)
                                    times = tempTimes;
                            }
                        }
                    }
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(2.1f - times, false, () =>
                    {
                        ShowAllCell(true, true);
                        for (int i = 0; i < Game1000Model.Instance.lines.Count; i++)
                            uitop1000.ShowOneLine(Game1000Model.Instance.lines[i].Key - 1);
                    }, 400);
                }
                else
                {
                    KeyValuePair<int, List<int>> tempLine = Game1000Model.Instance.lines[index];
                    List<int> elementPos = Game1000Model.Instance.lineData[tempLine.Key];
                    uitop1000.ShowOneLine(tempLine.Key - 1);
                    if (tempLine.Key >= 21 && tempLine.Key <= 25)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            float tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[tempLine.Key - 21].lstCells[j].TfSpine.GetChild(0), "a2");
                            if (tempTimes > times)
                                times = tempTimes;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < tempLine.Value.Count; j++)
                        {
                            float tempTimes = 0;
                            int col = tempLine.Value[j] / 3;
                            int row = tempLine.Value[j] % 3;
                            if (slotColumns[col].lstCells[2 - row].TfSpine.childCount > 0)
                                tempTimes = ToolUtil.Get3DAnimationDuration(slotColumns[col].lstCells[2 - row].TfSpine.GetChild(0), "a2");
                            if (tempTimes > times)
                                times = tempTimes;
                        }
                    }

                    CoreEntry.gTimeMgr.AddTimer(2.1f - times, false, () =>
                    {

                        if (tempLine.Key >= 21 && tempLine.Key <= 25)
                        {
                            for (int j = 0; j < 3; j++)
                                slotColumns[tempLine.Key - 21].lstCells[j].ShowCellEffect(true);
                        }
                        else
                        {
                            for (int j = 0; j < tempLine.Value.Count; j++)
                            {
                                int col = tempLine.Value[j] / 3;
                                int row = tempLine.Value[j] % 3;
                                slotColumns[col].lstCells[2 - row].ShowCellEffect(true);
                            }
                        }
                        index++;
                    }, 400);
                }
            }, 399);
        }

        private void PlayUISound(int ele)
        {
            if (ele == 1)
                CoreEntry.gAudioMgr.PlayUISound(98,gameObject);
            else if (ele == 2)
                CoreEntry.gAudioMgr.PlayUISound(100, gameObject);
            //else if (ele == 3)
            //    CoreEntry.gAudioMgr.PlayUISound(87);
            else if (ele == 4)
                CoreEntry.gAudioMgr.PlayUISound(97, gameObject);
            //else if (ele == 5)
            //    CoreEntry.gAudioMgr.PlayUISound(87);
            else if (ele == 6)
                CoreEntry.gAudioMgr.PlayUISound(99, gameObject);
            else if (ele == 7)
                CoreEntry.gAudioMgr.PlayUISound(96, gameObject);
            else if (ele == 8)
                CoreEntry.gAudioMgr.PlayUISound(94, gameObject);
            else if (ele == 9)
                CoreEntry.gAudioMgr.PlayUISound(101, gameObject);
            else if (ele == 10)
                CoreEntry.gAudioMgr.PlayUISound(95, gameObject);
        }

        public override void ShowAllCell(bool bShow, bool bAllTrue = false)
        {

            if (bDanJi)
            {
                for (int i = 0; i < Game1000Model.Instance.lines.Count; i++)
                {
                    KeyValuePair<int, List<int>> tempLine = Game1000Model.Instance.lines[i];
                    if (tempLine.Key >= 21 && tempLine.Key <= 25)
                    {
                        for (int j = 0; j < 3; j++)
                            slotColumns[tempLine.Key - 21].lstCells[j].ShowCellEffect(bShow, bAllTrue);
                    }
                    else
                    {
                        for (int j = 0; j < tempLine.Value.Count; j++)
                        {
                            int col = tempLine.Value[j] / 3;
                            int row = tempLine.Value[j] % 3;
                            slotColumns[col].lstCells[2 - row].ShowCellEffect(bShow, bAllTrue);
                        }
                    }
                }

            }
            else
            {
                for (int i = 0; i < Game1000Model.Instance.lines2.Count; i++)
                {
                    KeyValuePair<int, int> tempLine = Game1000Model.Instance.lines2[i];
                    if (tempLine.Key >= 21 && tempLine.Key <= 25)
                    {
                        for (int j = 0; j < 3; j++)
                            slotColumns[tempLine.Key - 21].lstCells[j].ShowCellEffect(bShow, bAllTrue);
                    }
                    else
                    {
                        List<int> posList = Game1000Model.Instance.GetElementPosByLine(tempLine.Value);
                        List<int> elementPos = Game1000Model.Instance.lineData[tempLine.Key];
                        for (int j = 0; j < 5; j++)
                        {
                            if (posList[j] == 0)
                                continue;
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
                        }
                    }
                }
                    
            }
               
          
        }


        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1000Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game1000Model.Instance.slotResult[cr2i(c, r)];
        }

        public async override void beginSpin(int num0 = 0, bool fast = false)
        {
            num = num0;
            //--手动或免费时num为0
            //isFastSpin = uitop1000.GetToggleIsOn();
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                uitop1000.SetFreeTimes();
            }
            else
            {
                if (commonTop.Bet == commonTop.betMin && Game1000Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet*30))
                {
                    autoSpinNum = 0;// -- 停止自动
                    commonTop.SetSlotSpinNum(autoSpinNum);
                    setState(slotState.Idle);
                    uitop1000.ShowNotEnoughMoneyTips();
                    return;
                }
                else
                {
                    bool bCanSlot = false;
                    if (Game1000Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet*30))
                    {
                        int tempBet1 = commonTop.Bet;
                        for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                        {
                            int tempBet = commonTop.BetList[i];
                            if (tempBet < tempBet1)
                            {
                                commonTop.OnClickBtnMin();
                                if (Game1000Model.Instance.toSpin.n64Gold >= (long)(tempBet*30))
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
                            uitop1000.ShowNotEnoughMoneyTips();
                            return;
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
            setState(slotState.SpinBegin);
            preSpin();
            for (int i = 0; i < SlotData_1000.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.08f));
            }
                int betValue = commonTop.Bet;
            commonTop.UpdateGold(Game1000Model.Instance.toSpin.n64Gold - betValue * 30);      
            Game1000Model.Instance.bShowFreeAni = false; 
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
            if (!isFreeSpin)
                freeTimes.reset();
            commonTop.UpdateGold(Game1000Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (freeTimes.max > 0)
            {
                beginSpin(0, isFastSpin);
            }
            else if (autoSpinNum > 0)
            {
                if (autoSpinNum >0 )
                    beginSpin(autoSpinNum, isFastSpin);
                else
                    isFastSpin = false;
            }
            else
                isFastSpin = false;
        }


        protected override int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (Game1000Model.Instance.slotResult == null)
                return count;
            for (int i = 0; i < Game1000Model.Instance.slotResult.Count; i++)
            {
                if (Game1000Model.Instance.slotResult[i] == ele || (orWild && Game1000Model.Instance.slotResult[i] == SlotData_1000.elementWild))
                    count++;
            }
            return count;
        }

        public override void InitData()
        {
            for (int i = 0; i < slotColumns.Count; i++)
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

        public async override void finishRoll(int column)
        {
            try
            {
                if (column == columnCount - 1)
                { //最后一列结束
                    commonTop.SetRollBtnRorate(false);
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.2f));
                    finishSpin();
                }
                else if (column > 0)
                { //出现2个免费，下一列加速
                    if (!isFastSpin && false)
                    {
                        //下一列加速
                        slotColumns[column + 1].playAcc();
                        //再后列等待
                        for (int i = column + 2; i < slotColumns.Count; i++)
                            slotColumns[i].endRoll(-1);
                        setState(slotState.SpinStop);
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void StepWinGold()
        {
            if (Game1000Model.Instance.toSpin.WinGold > 0 || Game1000Model.Instance.nModelGame > 0)
            {
                showLines();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotWinGold, null);
            }
        }

        public void OpenSmallGame1()
        {
            if (smallGame1 == null)
            {
                GameObject go1 = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1000/FirstRes/smallGame1");
                smallGame1 = go1.GetComponent<smallGame1>();
                smallGame1.uiroom100 = this;
            }
            smallGame1.Open();
        }
        public void OpenSmallGame2()
        {
            if (smallGame2 == null)
            {
                GameObject go1 = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1000/FirstRes/smallGame2");
                smallGame2 = go1.GetComponent<smallGame2>();
                smallGame2.uiroom100 = this;
            }
            smallGame2.InitData();
            smallGame2.gameObject.SetActive(true);
        }

        public void OpenSmallGame3()
        {
            if (smallGame3 == null)
            {
                GameObject go1 = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1000/FirstRes/smallGame3");
                smallGame3 = go1.GetComponent<smallGame3>();
                smallGame3.uiroom100 = this;
            }
            smallGame3.gameObject.SetActive(true);
        }

        public void OpenDiceGames()
        {
            if (DiceGames == null)
            {
                GameObject go1 = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1000/FirstRes/DiceGames");
                DiceGames = go1.GetComponent<DiceGames>();
                DiceGames.uiroom100 = this;
            }
            DiceGames.gameObject.SetActive(true);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(91);
        }
    }

}


