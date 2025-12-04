using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;

namespace HotUpdate
{
    public static class SlotData_700
    {
        public static int column = 5;
        public static int elementFree = 10;
        public static int specialelement = 14;
        public static int elementWild = 11;
        public static int elementCount = 14;
        public static float rollTime = 0.05f;
        public static int rollTimes = 3;
        public static float rollAccTime = 0.2f;
        public static int rollAccTimes = 15;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_700
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




    public class UIRoom700_SlotCpt : UIRoom_SlotCpt
    {
        public Room700Mgr mgr { get; private set; } = new Room700Mgr();
       // protected GameObject SpineAniItem11;

        private List<Transform> LineDeng = new List<Transform>();
        private List<Transform> LineDeng_2 = new List<Transform>();
        private List<Transform> rightLineDeng = new List<Transform>();
        private List<Transform> rightLineDeng_2 = new List<Transform>();
        private Transform RollBar;
        public Button BtnRollBar;
        private Animation Animator;

        private Transform Light;

        private Transform Dark;


        public CommonTop commonTop;

        RectTransform ske;

        protected override void Awake()
        {

            //--列数
            TfSlot = transform.Find("GoSlot/TfSlot");
            for (int i = 0; i < TfSlot.childCount; i++)
            {
                Transform child = TfSlot.GetChild(i);
                if (child.name.Contains("Column"))
                    lstcolumns.Add(child);
            }
            //--行数
            Transform tfColumn = TfSlot.GetChild(1);
            slotRow = tfColumn.childCount - 1;
            //-- 行高
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;
            slotColumns = new List<UISlotColumn>();
            effectAcc = transform.Find("GoSlot/TfSlot/EffectAcc").gameObject;
            // SpineAniItem11 = transform.Find("SpineAniItem11").gameObject;
            mgr.Init(this, Game700Model.Instance.toSpin);

            Light = transform.Find("Bg/Deng/Light");
            Dark = transform.Find("Bg/Deng/Dark");
            Transform Left = transform.Find("Bg/Deng/Left");
            for(int i = 0;i < 9;i++)
            {
                LineDeng.Add(Left.GetChild(i).GetChild(0));
                LineDeng_2.Add(Left.Find((i + 1).ToString()).GetChild(0));
            }
            Transform right = transform.Find("Bg/Deng/right");
            for(int i = 0;i < 9;i++)
            {
                rightLineDeng.Add(right.GetChild(i).GetChild(0));
                rightLineDeng_2.Add(right.Find((i+1).ToString()).GetChild(0));
            }
            RollBar = transform.Find("Bg/Image/RollBar");
            BtnRollBar = transform.Find("Bg/Image/RollBar/Bg/BtnRollBar").GetComponent<Button>();
            BtnRollBar.onClick.AddListener(ClickRodar);


            Animator = RollBar.GetComponent<Animation>();
            ske = transform.Find("Bg/Spine_Ani").GetComponent<RectTransform>();
 
        }

        protected override void Start()
        {
            GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
            commonTop = go.GetComponent<CommonTop>();
            commonTop.SetDanJi(bDanJi, true, false ,false, 9);
            commonTop.SlotCpt = this;
            //commonTop.GetComponent<Canvas>().overrideSorting = true;

            uiTop = transform.Find("TopPanel").GetComponent<UIRoom700Top>();
            uiTop.Init(this);
            init();
            uiTop.InitData();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Game700Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(223);

            if (uiTop != null)
            {
                autoSpinNum = 0;
                InitData();
                uiTop.Init(this);
                uiTop.InitData();
                UIRoom700Top top700 = uiTop as UIRoom700Top;
                top700.TfEffectSpine.gameObject.SetActive(false);
            }

            PlayTopLight();
            gameStatus = 0;
            num = 0;
            CoreEntry.gTimeMgr.AddTimer(12, true, () => PlayAni2(), 87);
        }

        public void Reconnect()
        {
            if (uiTop != null)
            {
                commonTop.SetRollBtnRorate(false);
                InitData();
                commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
                UIRoom700Top top600 = uiTop as UIRoom700Top;
                top600.SetJackPot();
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_700.column; i++)
            {
                UIRomm700SlotColumn column = lstcolumns[i].GetComponent<UIRomm700SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < 4;j++)
                {
                    UIRoom700SlotCell cell = column.SelfTransform.GetChild(j).GetComponent<UIRoom700SlotCell>();
                    cell.init(column, uiTop);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public void PlayTopLight()
        {
            CoreEntry.gTimeMgr.RemoveTimer(8889);
            CoreEntry.gTimeMgr.AddTimer(0.35f, true, () => 
            {
                Light.gameObject.SetActive(!Light.gameObject.activeSelf);

                Dark.gameObject.SetActive(!Dark.gameObject.activeSelf);
            }, 8889);
        }

        public override void preSpin()
        {
            UIRoom700Top top700 = uiTop as UIRoom700Top;
            top700.TfEffectSpine.gameObject.SetActive(false);
            top700.SetTxtScore(0);
            top700.HideAllLines(false, null);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);


            if (bDanJi)
            {
                awaiting = true;
                setState(slotState.SpinBegin);
                RandomSpinData();
                CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { recSpin(); }, 1000);
            }

        }

        public override void sendSpin()
        {
            if (bDanJi == false)
            {
                gameStatus = 1;
                int num = (commonTop.Bet);
                Game700Ctrl.Instance.Send_CS_GAME3_BET_REQ((int)num);
            }
        }
   

        public override void RandomSpinData()
        {
            Game700Model.Instance.slotResult.Clear();
            int freeElementCount = 0;
            for (int i = 0; i < 15; i++)
            {
                int element = 1;// UnityEngine.Random.Range(1, 11);

                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < Game700Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game700Model.Instance.PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                if (element == SlotData_700.elementFree)
                    freeElementCount++;
                Game700Model.Instance.slotResult.Add(element);
            }
            if (freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (Game700Model.Instance.slotResult[j] == SlotData_700.elementFree)
                            Game700Model.Instance.slotResult[j] = 3;
                    }
                }
            }

            bool bHasOneLine = UnityEngine.Random.Range(1, 6) <= 2;

            int element3 = 0;
            if (bHasOneLine) //随机中1条线
            {
                int index = UnityEngine.Random.Range(1, 10);
                element3 = UnityEngine.Random.Range(1, 10);// 线上的元素
                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < Game700Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game700Model.Instance.PRelementList[k])
                    {
                        element3 = k + 1;
                        break;
                    }
                }

                if (element3 == SlotData_700.elementFree)
                    element3 = 1;
                List<int> elements = Game700Model.Instance.lineData[index];
                int randomCount = UnityEngine.Random.Range(3, 6);
                for (int j = 0; j < randomCount; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    Game700Model.Instance.slotResult[pos] = element3;
                }
            }

            Game700Model.Instance.lines.Clear();
            Game700Model.Instance.elementList.Clear();
            for (int i = 1; i <= 9; i++)
            {
                List<int> elementPos = Game700Model.Instance.lineData[i];
                int sameElementCount = 0;
                int element = Game700Model.Instance.slotResult[elementPos[0] - 1];
                for (int k = 0; k < elementPos.Count; k++)
                {
                    int pos = (k) * 3 + elementPos[k] - 1;
                    if (element == Game700Model.Instance.slotResult[pos])
                        sameElementCount++;
                    else
                        break;
                }

                if (sameElementCount >= 3)
                {
                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    Game700Model.Instance.lines.Add(tempLine);
                    Game700Model.Instance.elementList.Add(element);
                    if (element == SlotData_700.elementFree)
                        Game700Model.Instance.toSpin.FreeTimes = 5;
                    else
                        Game700Model.Instance.toSpin.FreeTimes = 0;
                }
            }
            if (Game700Model.Instance.lines.Count > 0)
            {
                Game700Model.Instance.toSpin.WinGold = 0;
                for (int i = 0; i < Game700Model.Instance.lines.Count; i++)
                {
                    int rate = 0;
                    element3 = Game700Model.Instance.elementList[i] - 1;
                    if (element3 > 0)
                    {
                        int count = Game700Model.Instance.lines[i].Value;
                        if (count == 3)
                            rate = Game700Model.Instance.elementRate3[element3];
                        else if (count == 4)
                            rate = Game700Model.Instance.elementRate4[element3];
                        else
                            rate = Game700Model.Instance.elementRate5[element3];
                    }
                    else
                    {
                        rate = UnityEngine.Random.Range(1, 6);
                    }

                    Game700Model.Instance.toSpin.WinGold += Game700Model.Instance.nBet1 * rate;                  
                }
                Game700Model.Instance.toSpin.rate = Game700Model.Instance.toSpin.WinGold / Game700Model.Instance.nBet1;
                Game700Model.Instance.toSpin.WinGold = Game700Model.Instance.toSpin.WinGold / 9;
                Game700Model.Instance.toSpin.n64Gold += Game700Model.Instance.toSpin.WinGold;
                Game700Model.Instance.toSpin.n64Gold -= Game700Model.Instance.nBet1;
            }
            else
            {
                Game700Model.Instance.toSpin.rate = 0;
                Game700Model.Instance.toSpin.WinGold = 0;
                Game700Model.Instance.toSpin.n64Gold -= Game700Model.Instance.nBet1;
            }

            PlayerPrefs.SetInt("DanJi", (int)Game700Model.Instance.toSpin.n64Gold);
        }


        public override void SetSpinData()
        {
            recSpin();
        }



        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_700.rollTimes + i *2;// -- 慢速
                //if (isFastSpin)// --快速
                //    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game700Model.Instance.toSpin == null)
                return;
            base.finishSpin();
            setState(slotState.SpinEnd);

            //-- 获得免费
            if (Game700Model.Instance.toSpin.FreeTimes > 0)
            {
                //if (freeTimes.max <= 0)
                    freeTimes.max = Game700Model.Instance.toSpin.nModelGame;//-- + freeTimes.max


                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game700Model.Instance.toSpin.WinGold;


                if (!isFreeSpin && Game700Model.Instance.toSpin.nModelGame == Game700Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                else if (Game700Model.Instance.lastCount > 0 && Game700Model.Instance.toSpin.nModelGame > Game700Model.Instance.lastCount)
                {
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                }
                isFreeSpin = true;

                Game700Model.Instance.lastCount = Game700Model.Instance.toSpin.nModelGame;
            }
            //-- 免费结束
            if (isFreeSpin &&(Game700Model.Instance.toSpin.nModelGame == Game700Model.Instance.toSpin.FreeTimes) && Game700Model.Instance.toSpin.FreeTimes == 0)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
                Game700Model.Instance.lastCount = 0;
            }
            for (int i = 0; i < SlotData_700.column; i++)
                slotColumns[i].onSpinFinish(Game700Model.Instance.toSpin.WinGold > 0);

            UIRoom700Top top700 = uiTop as UIRoom700Top;
            top700.SetTopBomb();
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);
            ShowLight(false);
            ShowOneLine(Game700Model.Instance.lines);

            mgr.RunStepList();
            //if (isFreeSpin && (Game700Model.Instance.toSpin.nModelGame == Game700Model.Instance.toSpin.FreeTimes) && Game700Model.Instance.toSpin.FreeTimes == 0)
            //    isFreeSpin = false;

        }

        public override void showLines()
        {
            UIRoom700Top top700 = uiTop as UIRoom700Top;
            top700.TfEffectSpine.gameObject.SetActive(true);
            ShowAllCell(false);
            for (int i = 0; i < Game700Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game700Model.Instance.lines[i];

                List<int> elementPos = Game700Model.Instance.lineData[tempLine.Key];
                top700.ShowOneLine(tempLine.Key);
                for (int j = 0; j < tempLine.Value; j++)
                {
                    int temp = (j) * 3 + elementPos[j];
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].index = temp -1;
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].showLine();
                }
            }
            ShowLight(false);
            ShowOneLine(Game700Model.Instance.lines);
            ShowAllCell(true, true);
            int index = 0;
            CoreEntry.gAudioMgr.PlayUISound(231,transform.GetChild(2).gameObject);
            CoreEntry.gTimeMgr.AddTimer(1.35f, true, () =>
            {
                ShowLight(false);
                ShowAllCell(false);
                if (index == Game700Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.15f, false, () =>
                    {
                        CoreEntry.gAudioMgr.PlayUISound(231, transform.GetChild(2).gameObject);
                        ShowAllCell(true, true);
                        ShowOneLine(Game700Model.Instance.lines);
                    }, 400);
                }
                else
                {
                    float times = 0;
                    KeyValuePair<int, int> tempLine = Game700Model.Instance.lines[index];
                    List<int> elementPos = Game700Model.Instance.lineData[tempLine.Key];
                    for (int i = 0; i < tempLine.Value; i++)
                    {
                        UIRoom700SlotCell cell = slotColumns[i].lstCells[2 - elementPos[i] + 1] as UIRoom700SlotCell;
                        string aniName = "win";
                        if (cell.element == 3)
                            aniName = "animation";
                        float tempTimes = ToolUtil.GetAnimationDuration(uiTop.SpineCell[cell.cellIndex].GetChild(0), aniName);
                        if (tempTimes > times)
                            times = tempTimes;
                    }
                    CoreEntry.gTimeMgr.AddTimer(1.35f - times, false, () =>
                    {
                        CoreEntry.gAudioMgr.PlayUISound(231, transform.GetChild(2).gameObject);
                        top700.ShowOneLine(tempLine.Key,true);
      
                        List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                        temp.Add(tempLine);
                        ShowOneLine(temp,true);
                        int element = 0;
                        for (int j = 0; j < tempLine.Value; j++)
                        {
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);
                            if (slotColumns[j].lstCells[2 - elementPos[j] + 1].element != SlotData_700.elementWild)
                                element = slotColumns[j].lstCells[2 - elementPos[j] + 1].element;
                        }
                        if (element == 0)
                            element = SlotData_700.elementWild;
                        top700.SetOneLineWinGoldText(tempLine.Key, tempLine.Value,element);

                        index++;
                    }, 400);
                }
            }, 399);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game700Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game700Model.Instance.lines[i];
                List<int> elementPos = Game700Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }

            UIRoom700Top top600 = uiTop as UIRoom700Top;
            top600.HideAllLines(bShow,Game700Model.Instance.lines);
            for (int i = 0; i < LineDeng_2.Count; i++)
                LineDeng_2[i].gameObject.SetActive(false);
            if(bShow)
            {
                for (int i = 0; i < Game700Model.Instance.lines.Count; i++)
                    LineDeng_2[Game700Model.Instance.lines[i].Key -1].gameObject.SetActive(false);
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game700Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game700Model.Instance.slotResult[cr2i(c, r)];
        }

        public override async void beginSpin(int num0 = 0, bool fast = false)
        {
            num = num0;
        
            PlayAni();
            //--手动或免费时num为0
            isFastSpin = commonTop.GetToggleIsOn();
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true, Game700Model.Instance.toSpin.FreeTimes + "");
            }
            else
            {
                if(!commonTop.GetGoFreeTimes().activeSelf)
                {
                    if (commonTop.Bet == commonTop.betMin && Game700Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet * 9))
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
                        if (Game700Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet * 9 ))
                        {
                            int tempBet1 = commonTop.Bet;
                            for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = commonTop.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    commonTop.OnClickBtnMin();
                                    if (Game700Model.Instance.toSpin.n64Gold >= (long)(tempBet * 9 ))
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

            if (Game700Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = commonTop.Bet * 9 ;
                commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold - (int)(betValue));
            }
            uiTop.freeAni.Kill();
            uiTop.freeAni = null;
            uiTop.TfFree.gameObject.SetActive(false);
            Game700Model.Instance.bShowFreeAni = false;
            PlayLineLight();
            CoreEntry.gAudioMgr.PlayUISound(227);
            preSpin();
            awaiting = true;
            commonTop.SetRollBtnRorate(true);
            setState(slotState.SpinBegin);
            for (int i = 0; i < SlotData_700.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.08f));
            }
            sendSpin();
    
        }

        public void StartRoll()
        {
            gameStatus = 2;
            if (awaiting) //防止连点
                return;
        }

        public override void continueSpin()
        {
            //普通或免费结束后再加金币
            if (isFreeEnd())
                freeTimes.reset();
            commonTop.UpdateGold(Game700Model.Instance.toSpin.n64Gold);
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
            return Game700Model.Instance.toSpin.FreeTimes == 0;
        }

        protected override int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (Game700Model.Instance.slotResult == null)
                return count;
            for (int i = 0; i < Game700Model.Instance.slotResult.Count; i++)
            {
                if (Game700Model.Instance.slotResult[i] == ele || (orWild && Game700Model.Instance.slotResult[i] == SlotData_700.elementWild))
                    count++;
            }
            return count;
        }

        public void PlayLineLight()
        {
            int index = 0;
            GameObject go = null;
            GameObject go1 = null;
            ShowLight(false);
            CoreEntry.gTimeMgr.AddTimer(0.03f, true, () =>
            {
                if (go != null)
                {
                    go.gameObject.gameObject.SetActive(false);
                    go1.gameObject.gameObject.SetActive(false);
                }
                go = LineDeng[index].gameObject;
                go.SetActive(true);
                go1 = rightLineDeng[index].gameObject;
                go1.SetActive(true);
                index = index + 1;
                if (index >= 9)
                    index = 0;
            }, 4);

            CoreEntry.gTimeMgr.AddTimer(0.8f, false, () =>
            {
               
                CoreEntry.gTimeMgr.RemoveTimer(4);
                CoreEntry.gTimeMgr.AddTimer(0.08f, true, () => 
                {
                    if (go != null)
                    {
                        go.gameObject.gameObject.SetActive(false);
                        go1.gameObject.gameObject.SetActive(false);
                    }
                    go = LineDeng[index].gameObject;
                    go.SetActive(true);
                    go1 = rightLineDeng[index].gameObject;
                    go1.SetActive(true);
                    index = index + 1;
                    if (index >= 9)
                        index = 0;
                }, 555);
            }, 5);
        }

        public void ShowLight(bool bShow)
        {
            for (int i = 0; i < LineDeng.Count; i++)
                LineDeng[i].gameObject.SetActive(bShow);
            for (int i = 0; i < rightLineDeng.Count; i++)
                rightLineDeng[i].gameObject.SetActive(bShow);

        }

        public void ShowOneLine(List<KeyValuePair<int,int>> lines,bool bOne = false)
        {
            for(int i = 0;i < lines.Count;i++)
            {
                int line = lines[i].Key;
                LineDeng_2[line -1 ].gameObject.SetActive(true);
                if(bOne)
                {
                    Transform temp = LineDeng_2[line - 1];
                    temp.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).OnComplete(() =>
                    {
                        temp.transform.DOScale(Vector3.one, 0.5f);
                        //temp.localScale = Vector3.one;
                     });
                }
          
                rightLineDeng_2[line - 1].gameObject.SetActive(true);
            }
        }

        public void ClickRodar()
        {
            BtnRollBar.interactable = false;
            if(commonTop.GetBeginBtn().interactable == true && commonTop.GetBeginBtn().gameObject.activeSelf)
            {
                //Animator["RollAni"].speed = 1.8f;
                //Animator.Play("RollAni");
                commonTop.BeginSlot(0);
                commonTop.GetBeginBtn().interactable = false;
            }
        }

        public void PlayAni()
        {
            //Animator["RollAni"].speed = 2.5f;
            //Animator.Play("RollAni");
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
            try
            {
                if (column == columnCount - 1)
                { //最后一列结束
                    CoreEntry.gTimeMgr.RemoveTimer(9);
                    commonTop.SetRollBtnRorate(false);
                    CoreEntry.gTimeMgr.AddTimer(0.4f, false, () => 
                    {
                        finishSpin();
                    }, 9);                 
                }
                else if (column > 0)
                { //出现2个免费，下一列加速
                    if (!isFastSpin &&  shouldAcc(column))
                    {
                        CoreEntry.gAudioMgr.StopSound(transform.GetChild(1).gameObject);
                        CoreEntry.gAudioMgr.PlayUISound(229,transform.GetChild(1).gameObject);
                        Game700Model.Instance.freeColumn = column + 1;
             
                    
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

        protected override bool shouldAcc(int column)
        {
            //第一列不加速
            if (column < 1 || column >= SlotData_700.column - 1)
                return false;
            //出现免费元素数量大等于2
            int freeNum = 0;
            for (int i = 0; i < 3 * (column + 1); i++)
            {
                if (Game700Model.Instance.slotResult[i] == SlotData_700.elementFree)
                {
                    freeNum++;
                }
            }
            return freeNum >= 2;

        }


        private void PlayAni2()
        {
            int random = UnityEngine.Random.Range(0, 2);
            if(random == 1)///
            {
                ske.anchoredPosition = new Vector2(561,170);
                ske.localScale = new Vector3(1,1,1);
                int random2 = UnityEngine.Random.Range(0, 2);

                if(random2 ==1)
                {
                    ske.DOAnchorPos(new Vector2(-561, 170), 4).SetEase(Ease.Linear);
                    ToolUtil.PlayAnimation(ske.transform, "left_boxwalk", true);
                }
                else
                {
                    ToolUtil.PlayAnimation(ske.transform, "left_walk", true, null);
                    ske.DOAnchorPos(new Vector2(17, 170), 2).SetEase(Ease.Linear).OnComplete(()=> 
                    {
                        string aniName = "left_apple";
                        int random3 = UnityEngine.Random.Range(0, 2);
                        if (random3 == 1)
                            aniName = "left_starfruit";
                        ToolUtil.PlayAnimation(ske.transform, aniName, false ,()=> 
                        {
                            ske.DOAnchorPos(new Vector2(-561, 170), 2).SetEase(Ease.Linear);
                            ToolUtil.PlayAnimation(ske.transform, "left_walk", true, null);
                        });
                    });
                 
                  
                }
            }
            else
            {
                ske.anchoredPosition = new Vector2(-561, 170);
                ske.localScale = new Vector3(-1, 1, 1);
                int random2 = UnityEngine.Random.Range(0, 2);
                if(random2 == 1)
                {
                    ske.DOAnchorPos(new Vector2(561, 170), 4).SetEase(Ease.Linear);
                    ToolUtil.PlayAnimation(ske.transform, "left_boxwalk", true);
                }
                else
                {
                    ToolUtil.PlayAnimation(ske.transform, "left_walk", true, null);
                    ske.DOAnchorPos(new Vector2(17, 170), 2).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        string aniName = "left_apple";
                        int random3 = UnityEngine.Random.Range(0, 2);
                        if (random3 == 1)
                            aniName = "left_starfruit";
                        ToolUtil.PlayAnimation(ske.transform, aniName, false, () =>
                        {
                            ske.DOAnchorPos(new Vector2(561, 170), 2).SetEase(Ease.Linear);
                            ToolUtil.PlayAnimation(ske.transform, "left_walk", true, null);
                        });
                    });
                }
               
            }
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(223);
        }
    }

}


  