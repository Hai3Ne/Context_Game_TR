using DG.Tweening;
using SEZSJ;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_500
    {
        public static int column = 5;
        public static int elementFree = 12;
        public static int specialelement = 14;
        public static int elementWild = 5;
        public static int elementCount = 14;
        public static float rollTime = 0.05f;
        public static int rollTimes = 12;
        public static float rollAccTime = 0.045f;
        public static int rollAccTimes = 30;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }


    public class UIRoom500_SlotCpt : UIRoom_SlotCpt
    {
        public Room50Mgr mgr { get; private set; } = new Room50Mgr();

        public CommonTop commonTop;
        public UIRoom500Top Top500;
        SkeletonGraphic playSke;

        private string[] playerAniName = new string[] { "idle","skill1"};
        int aniIndex = 0;

        protected override void Awake()
        {
            base.Awake();
            mgr.Init(this, Game500Model.Instance.toSpin);
            playSke = transform.Find("Spine_Player").GetComponent<SkeletonGraphic>();
        }
        protected override void Start()
        {
            GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
            commonTop = go.GetComponent<CommonTop>();
            commonTop.SetDanJi(bDanJi,true,false,false,50);
            commonTop.SlotCpt = this;

            Top500 = transform.Find("TopPanel").GetComponent<UIRoom500Top>();
            Top500.Init(this);
            init();
            Top500.InitData();
            Top500.transform.SetAsLastSibling();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            bDanJi = param == null ? false : (bool)param;
            Game500Model.Instance.InitConfig();
            if (Game500Model.Instance.nModelGame <=0)
                CoreEntry.gAudioMgr.StopMusic(47);
            else
            {
                if (Game500Model.Instance.nFreeGame > 0)
                    CoreEntry.gAudioMgr.PlayUIMusic(16);
                else if(Game500Model.Instance.nSunGame > 0)
                    CoreEntry.gAudioMgr.PlayUIMusic(29);
            }
            if (Top500 != null)
            {
                autoSpinNum = 0;
                InitData();
                Top500.Init(this);
                Top500.InitData();
            }
            gameStatus = 0;
            num = 0;
            CoreEntry.gTimeMgr.RemoveTimer(697);
            CoreEntry.gTimeMgr.AddTimer(15,true,()=>
            {
                ToolUtil.PlayAnimation(playSke.transform, playerAniName[aniIndex],true);
                aniIndex++;
                if (aniIndex > 1)
                    aniIndex = 0;
            },697);
        }

        public void Reconnect()
        {
            commonTop.SetRollBtnRorate(false);
            CoreEntry.gAudioMgr.PlayUIMusic(29);
            if (Top500 != null)
            {
                InitData();
                commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
                Top500.rollJackPot.SetNum(Game500Model.Instance.n64Jackpot);
                    continueSpin();
            }
            if(commonTop != null)
                commonTop.Reconnect();
        
        }


        public override bool isFreeEnd()
        {
            return Game500Model.Instance.toSpin.FreeTimes == 0;
        }

        public override void preSpin()
        {
            CoreEntry.gAudioMgr.StopSound();
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);

            if (bDanJi)
            {
                setState(slotState.SpinBegin);
                RandomSpinData();
                CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { recSpin(); }, 1000);
            }
        }


        public override void sendSpin()
        {
            if (bDanJi == false)
            {             
                setState(slotState.SpinBegin);
                int num = (commonTop.Bet);
                gameStatus = 1;
                Game500Ctrl.Instance.Send_CS_GAME1_BET_REQ((int)num);
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_500.column; i++)
            {
                UISlotColumn column = lstcolumns[i].GetComponent<UISlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for (int j = 0; j < 4; j++)
                {
                    UIRoom500SlotCell cell = column.SelfTransform.GetChild(j).GetComponent<UIRoom500SlotCell>();
                    cell.init(column, commonTop, Top500);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override void finishSpin()
        {
            if (Game500Model.Instance.toSpin == null)
                return;
            base.finishSpin();

            if (isFastSpin)
                CoreEntry.gAudioMgr.PlayUISound(26);
            setState(slotState.SpinEnd);

            //-- 获得免费
            if (Game500Model.Instance.toSpin.FreeTimes > 0)
            {
                freeTimes.max = Game500Model.Instance.toSpin.FreeTimes;
                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game500Model.Instance.toSpin.WinGold;

                if (!isFreeSpin && Game500Model.Instance.toSpin.nModelGame == Game500Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                else if (Game500Model.Instance.lastCount > 0 && Game500Model.Instance.toSpin.nModelGame > Game500Model.Instance.lastCount)
                {
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                }
                isFreeSpin = true;

                Game500Model.Instance.lastCount = Game500Model.Instance.toSpin.nModelGame;
            }
            //-- 免费结束
            if (isFreeSpin && (Game500Model.Instance.toSpin.nModelGame == Game500Model.Instance.toSpin.FreeTimes) && Game500Model.Instance.toSpin.FreeTimes == 0)
            {
                Game500Model.Instance.lastCount = 0;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
            }

        
            if(Game500Model.Instance.bInSpecialGame && Game500Model.Instance.toSpin.SpecialGame == 0)
            {
                Game500Model.Instance.bInSpecialGame = false;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotSpecialGameEnd, null);
            }
            for (int i = 0; i < SlotData_500.column; i++)
                slotColumns[i].onSpinFinish(Game500Model.Instance.toSpin.WinGold > 0);
            mgr.RunStepList();
        }


        public override void showLines()
        {
            ShowAllCell(false);
            Top500.HideEffect();
            for (int i = 0; i < Game500Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game500Model.Instance.lines[i];
                List<int> elementPos = Game500Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    int temp = (j) * 3 + elementPos[j];
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].index = temp - 1;
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].showLine();
                }
            }
            ShowAllCell(true, true);
            int index = 0;
            CoreEntry.gTimeMgr.AddTimer(2.3f, true, () =>
            {
                ShowAllCell(false);
                if (index == Game500Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>
                    {
                        ShowAllCell(true, true);
                        index = 0;
                    }, 400);
                }
                else
                {

                    KeyValuePair<int, int> tempLine = Game500Model.Instance.lines[index];
                    List<int> elementPos = Game500Model.Instance.lineData[tempLine.Key];
                    List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                    temp.Add(tempLine);

                    float times = 0;
                    for (int i = 0; i < tempLine.Value; i++)
                    {
                        int element = slotColumns[i].lstCells[2 - elementPos[i] + 1].element;
                        string aniName = "newAnimation";
                        if (element == 9)
                            aniName = "aiji_rw2";
                        else if (element == 10)
                            aniName = "aiji_rw1";
                        else if (element == 14)
                            aniName = "dz1";
                        else if (element == 13)
                            aniName = "Sprite";
                        else { }

                        float tempTimes = 1;// 
                        if(slotColumns[i].lstCells[2 - elementPos[i] + 1].TfSpine.childCount > 0)
                            tempTimes = slotColumns[i].lstCells[2 - elementPos[i] + 1].TfSpine.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>().animation.GetState(aniName)._animationData.duration;
                        if (tempTimes > times)
                            times = tempTimes;
                    }
                    CoreEntry.gTimeMgr.AddTimer(2.3f - times, false, () =>
                    {
                        bool bHasElement13 = false;
                        for (int j = 0; j < tempLine.Value; j++)
                        {
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);
                            if (slotColumns[j].lstCells[2 - elementPos[j] + 1].element == 13)
                                bHasElement13 = true;
                        }
                        if (bHasElement13)
                        {
                            CoreEntry.gAudioMgr.PlayUISound(35);
                            CoreEntry.gAudioMgr.StopMusic(13);
                        }
                        index++;
                    }, 400);
                }
            }, 399);
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            num = num0;
            commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
            CoreEntry.gTimeMgr.RemoveTimer(255);
            if (!CoreEntry.gAudioMgr.IsPlaying())
            {
                if (Game500Model.Instance.toSpin.nModelGame <= 0)
                    CoreEntry.gAudioMgr.PlayUIMusic(13);
            }

            if (isFreeSpin == false)
                commonTop.UpDateScore(0);

            //--手动或免费时num为0
            isFastSpin = commonTop.GetToggleIsOn();
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true,Game500Model.Instance.toSpin.FreeTimes + "");
            }
            else
            {
                if(Game500Model.Instance.toSpin.nModelGame <=0)
                {
                    if (commonTop.Bet == commonTop.betMin && Game500Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet * 50))
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
                        if (Game500Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet * 50))
                        {
                            int tempBet1 = commonTop.Bet;
                            for (int i = commonTop.BetList.Count - 1;i>=0;i--)
                            {
                                int tempBet = commonTop.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    commonTop.OnClickBtnMin();
                                    if (Game500Model.Instance.toSpin.n64Gold >= (long)(tempBet * 50 ))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if(!bCanSlot)
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
            if (Game500Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = commonTop.Bet * 50 ;
                commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold - (int)(betValue));
            }

            Top500.freeAni.Kill();
            Top500.freeAni = null;
            Top500.TfFree.gameObject.SetActive(false);
            Game500Model.Instance.bShowFreeAni = false;

            awaiting = true;
            for (int i = 0; i < SlotData_500.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
            }
            preSpin();
            sendSpin();
            commonTop.SetRollBtnRorate(true);
        }

        public void StartRoll()
        {
            gameStatus = 2;
            if (awaiting) //防止连点
                return;
        }
        public override void InitData()
        {
      
            for (int i = 0; i < SlotData_500.column; i++)
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
            try
            {
                if (column == columnCount - 1)
                { //最后一列结束
                    CoreEntry.gTimeMgr.RemoveTimer(9);

                    if (effectAcc.activeSelf)
                        CoreEntry.gAudioMgr.StopSound();
                    commonTop.SetRollBtnRorate(false);
                    CoreEntry.gTimeMgr.AddTimer(0.4f, false, () =>
                    {
                        finishSpin();
                    }, 9);
                }
                else if (column > 0)
                { //出现2个免费，下一列加速
                    if (!Game500Model.Instance.bInSpecialGame && shouldAcc(column))
                    {
                        CoreEntry.gAudioMgr.StopSound();
                        CoreEntry.gAudioMgr.PlayUISound(19);
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
            catch 
            {
            }
        }

        public override void continueSpin()
        {
            //普通或免费结束后再加金币
            if (isFreeEnd())
                freeTimes.reset();
            commonTop.UpdateGold(Game500Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);

            CoreEntry.gTimeMgr.AddTimer(2f,false,()=> CoreEntry.gAudioMgr.StopMusic(13), 255);
            if (Game500Model.Instance.bInSpecialGame)
            {
                if (Game500Model.Instance.toSpin.nModelGame > 0)
                {
                    beginSpin(0, isFastSpin);
                    Game500Model.Instance.toSpin.SpecialGame -= 1;
                }
                Top500.SetSpecialTimes(Game500Model.Instance.toSpin.SpecialGame);
            }
            else if (freeTimes.max > 0)
                beginSpin(0, isFastSpin);
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

        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_500.rollTimes + i * slotRow;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void RandomSpinData()
        {
            Game500Model.Instance.slotResult.Clear();
            int freeElementCount = 0;
            int sunElementCount = 0;
            for (int i = 0; i < 15; i++)
            {
                int element =1;
                int rand = UnityEngine.Random.Range(0, 10001);
                for(int k =0 ;k < Game500Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game500Model.Instance.PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                if(element == SlotData_500.elementFree)
                    freeElementCount++;
                if (element == SlotData_500.specialelement)
                    sunElementCount++;
                Game500Model.Instance.slotResult.Add(element);
            }
            if(freeElementCount >=3)
            {
                for(int i = 0;i < freeElementCount - 2;i++)
                {
                    for(int j =0;j < 15;j++)
                    {
                        if (Game500Model.Instance.slotResult[j] == SlotData_500.elementFree)
                            Game500Model.Instance.slotResult[j] = 3;
                    }
                }
            }

            if(sunElementCount >=4)
            {
                for (int i = 0; i < sunElementCount - 3; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (Game500Model.Instance.slotResult[j] == SlotData_500.specialelement)
                            Game500Model.Instance.slotResult[j] = 2;
                    }
                }
            }

            int element3 = 0;
            bool bHasOneLine = UnityEngine.Random.Range(1, 5) <=2;
            if(bHasOneLine) //随机中1条线
            {
                int index = UnityEngine.Random.Range(1, 50);
                element3 = UnityEngine.Random.Range(1, 10);// 线上的元素
                int rand = UnityEngine.Random.Range(0, 9000);
                for (int k = 0; k < Game500Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game500Model.Instance.PRelementList[k])
                    {
                        element3 = k + 1;
                        break;
                    }
                }
                if (rand > 8400)
                    element3 = 1;
                List<int> elements = Game500Model.Instance.lineData[index];
                for (int j = 0; j < elements.Count; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    Game500Model.Instance.slotResult[pos] = element3;
                }
            }
            Game500Model.Instance.lines.Clear();
            Game500Model.Instance.elementList.Clear();
            for (int i = 1; i <= 50; i++)
            {
                List<int> elementPos = Game500Model.Instance.lineData[i];
                int sameElementCount = 0;
                int element = Game500Model.Instance.slotResult[elementPos[0] -1];
                for (int k = 0; k < elementPos.Count; k++)
                {
                    int pos = (k) * 3 + elementPos[k] - 1;
                    if (element == Game500Model.Instance.slotResult[pos])
                        sameElementCount++;
                    else
                        break;
                }

                if (sameElementCount >= 3)
                {
                    if (element > 10)
                    {
                        Game500Model.Instance.slotResult[elementPos[0] - 1] = 1;
                        continue;
                    }     

                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    Game500Model.Instance.lines.Add(tempLine);
                    Game500Model.Instance.elementList.Add(element);
                    if (element == SlotData_500.elementFree)
                        Game500Model.Instance.toSpin.FreeTimes = 5;
                    else
                        Game500Model.Instance.toSpin.FreeTimes = 0;
                }
            }

            if(Game500Model.Instance.lines.Count > 0)
            {
                for(int i = 0;i < Game500Model.Instance.lines.Count;i++)
                {
                    int rate = 0;
                    element3 = Game500Model.Instance.elementList[i] -1;
                    if (element3 > 0)
                    {
                        int count = Game500Model.Instance.lines[i].Value;
                        if (count == 3)
                            rate = Game500Model.Instance.elementRate3[element3];
                        else if (count == 4)
                            rate = Game500Model.Instance.elementRate4[element3];
                        else
                            rate = Game500Model.Instance.elementRate[element3];
                    }
                    else
                        rate = UnityEngine.Random.Range(1, 6);

                    Game500Model.Instance.toSpin.WinGold += Game500Model.Instance.nBet1 * rate;       
                }
                Game500Model.Instance.toSpin.rate = Game500Model.Instance.toSpin.WinGold/ Game500Model.Instance.nBet1;
                Game500Model.Instance.toSpin.n64Gold += Game500Model.Instance.toSpin.WinGold;
                Game500Model.Instance.toSpin.n64Gold -= Game500Model.Instance.nBet1;
            }
            else
            {
                Game500Model.Instance.toSpin.rate = 0;
                Game500Model.Instance.toSpin.WinGold = 0;
                Game500Model.Instance.toSpin.n64Gold -= Game500Model.Instance.nBet1;
            }

            PlayerPrefs.SetInt("DanJi", (int)Game500Model.Instance.toSpin.n64Gold);
        }

        public override bool hasElement(int ele)
        {
            if (Game500Model.Instance.slotResult == null)
                return false;
            for (int i = 0; i < Game500Model.Instance.slotResult.Count; i++)
            {
                if (Game500Model.Instance.slotResult[i] == ele)
                    return true;
            }
            return false;
        }


        protected override int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (Game500Model.Instance.slotResult == null)
                return count;
            for (int i = 0; i < Game500Model.Instance.slotResult.Count; i++)
            {
                if (Game500Model.Instance.slotResult[i] == ele || (orWild && Game500Model.Instance.slotResult[i] == SlotData_500.elementWild))
                    count++;
            }
            return count;
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game500Model.Instance.slotResult.Count) { }
            return idx;
        }


        public void ResetCellSpecial()
        {
            for (int i = 0; i < SlotData_500.column; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    UIRoom500SlotCell cell = slotColumns[i].lstCells[j] as UIRoom500SlotCell;
                    cell.resetSpecial();
                }
            }
        }
        public void ResetCell()
        {
            for (int i = 0; i < SlotData_500.column; i++)
                slotColumns[i].reset();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(13);
        }

        public void ShowOneCellLine(int col, int row, Action callBack = null, int element = 0, int sunvalue = 0, bool bShowEffect = false)
        {
            int temp = (col) * 3 + row;
            slotColumns[col].lstCells[2 - row].index = temp;
            slotColumns[col].lstCells[2 - row].showLine(callBack, element, 2 - row, sunvalue);
            if (bShowEffect)
            {
                Top500.SpineCell[temp].GetChild(1).gameObject.SetActive(true);
            }
        }
    }

}


  