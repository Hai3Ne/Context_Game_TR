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
    public static class SlotData_602
    {
        public static int column = 5;
        public static int elementFree = 10;
        public static int specialelement = 14;
        public static int elementWild = 5;
        public static int elementCount = 14;
        public static float rollTime = 0.08f;
        public static int rollTimes = 12;
        public static float rollAccTime = 0.025f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_602
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




    public class UIRoom602_SlotCpt : UIRoom_SlotCpt
    {
        public Room602Mgr mgr { get; private set; } = new Room602Mgr();

        private List<Transform> LineDeng = new List<Transform>();
        private List<Transform> LineDeng_2 = new List<Transform>();
        private List<Transform> rightLineDeng = new List<Transform>();
        private List<Transform> rightLineDeng_2 = new List<Transform>();
        private Transform RollBar;
        public Button BtnRollBar;
        private Animation Animator;

        private Transform Light;

        private Transform Dark;

        public CommonTop602 CommonTop602;


        public Transform mask;

        public Transform[] paos = new Transform[5];
        public Transform[] targets = new Transform[5];
        public Transform target;
        protected override void Awake()
        {
            base.Awake();
            mgr.Init(this, Game602Model.Instance.toSpin);

            Light = transform.Find("Bg/Deng/Light");
            Dark = transform.Find("Bg/Deng/Dark");
            Transform Left = transform.Find("Bg/Left");
            for(int i = 0;i < 9;i++)
            {
                LineDeng.Add(Left.GetChild(i).GetChild(0));
                LineDeng_2.Add(Left.Find((i + 1).ToString()).GetChild(0));
            }
            Transform right = transform.Find("Bg/right");
            for(int i = 0;i < 9;i++)
            {
                rightLineDeng.Add(right.GetChild(i).GetChild(0));
                rightLineDeng_2.Add(right.Find((i+1).ToString()).GetChild(0));
            }
            RollBar = transform.Find("Bg/Image/RollBar");
            BtnRollBar = transform.Find("Bg/Image/RollBar/Bg/BtnRollBar").GetComponent<Button>();
            BtnRollBar.onClick.AddListener(ClickRodar);

            Animator = RollBar.GetComponent<Animation>();
            mask = transform.Find("Trans_Mask");
            target = transform.Find("Bg1/SmallGame/Target");
            for (int i = 0;i < paos.Length;i++)
            {
                paos[i] = transform.Find("Bg1/SmallGame/Target/Trans_Pao" + (i+1));
                targets[i] = transform.Find("Bg1/SmallGame/Target/Trans_target"+ (i + 1));
            }
        }

        protected override void Start()
        {
            GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game602/FirstRes/CommonTop602");
            CommonTop602 = go.GetComponent<CommonTop602>();
            CommonTop602.SetDanJi(bDanJi,true,true,false,9);
            CommonTop602.SlotCpt = this;

            uiTop = transform.Find("TopPanel").GetComponent<UIRoom602Top>();
            uiTop.Init(this);
            init();
            uiTop.InitData();
            uiTop.transform.SetAsLastSibling();
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            Game602Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(291);
            if (uiTop != null)
            {
                autoSpinNum = 0;
                InitData();
                uiTop.Init(this);
                uiTop.InitData();
            }
            mask.gameObject.SetActive(false);
            PlayTopLight();
            gameStatus = 0;
            num = 0;


        }

        public void Reconnect()
        {
            if (uiTop != null)
            {
                CommonTop602.SetRollBtnRorate(false);
                InitData();
                CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold);
                UIRoom602Top top602 = uiTop as UIRoom602Top;
                top602.SetJackPot();
                continueSpin();
            }
        }


        public override void init()
        {
            autoSpinNum = 0;
            CommonTop602.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_602.column; i++)
            {
                UIRomm602SlotColumn column = lstcolumns[i].GetComponent<UIRomm602SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < 4;j++)
                {
                    UIRoom602SlotCell cell = column.SelfTransform.GetChild(j).GetComponent<UIRoom602SlotCell>();
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
            UIRoom602Top top602 = uiTop as UIRoom602Top;
            top602.SetTxtScore(0);
            top602.HideAllLines(false, null);
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
            for (int i = 0; i < targets.Length; i++)
                targets[i].localEulerAngles = Vector3.zero;
        }

        public override void sendSpin()
        {
            if (bDanJi == false)
            {
                setState(slotState.SpinBegin);
                gameStatus = 1;
                int num = (CommonTop602.Bet);
                //Debug.LogError("<<<<<<<<<<<<<<<<<<<<<"+ num);
                Game602Ctrl.Instance.Send_CS_GAME19_BET_REQ((int)num);
            }
        }

        public override void RandomSpinData()
        {
            Game602Model.Instance.slotResult.Clear();
            int freeElementCount = 0;
            for (int i = 0;i < 15;i++)
            {
                int element = 1;// UnityEngine.Random.Range(1, 11);

                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < Game602Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game602Model.Instance.PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                if (element == SlotData_602.elementFree)
                    freeElementCount++;
                Game602Model.Instance.slotResult.Add(element);
            }
            if (freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (Game602Model.Instance.slotResult[j] == SlotData_602.elementFree)
                            Game602Model.Instance.slotResult[j] = 3;
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
                for (int k = 0; k < Game602Model.Instance.PRelementList.Count; k++)
                {
                    if (rand < Game602Model.Instance.PRelementList[k])
                    {
                        element3 = k + 1;
                        break;
                    }
                }

                if (element3 == SlotData_602.elementFree)
                    element3 = 1;
                List<int> elements = Game602Model.Instance.lineData[index];
                int randomCount = UnityEngine.Random.Range(3, 6);
                for (int j = 0; j < randomCount; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    Game602Model.Instance.slotResult[pos] = element3;
                }
            }
    
            Game602Model.Instance.lines.Clear();
            Game602Model.Instance.elementList.Clear();
            for (int i = 1;i <= 9;i++)
            {
                List<int> elementPos = Game602Model.Instance.lineData[i];
                int sameElementCount = 0;
                int element = Game602Model.Instance.slotResult[elementPos[0] - 1];
                for (int k = 0; k < elementPos.Count; k++)
                {
                    int pos = (k) * 3 + elementPos[k] - 1;
                    if (element == Game602Model.Instance.slotResult[pos])
                        sameElementCount++;
                    else
                        break;
                }

                if (sameElementCount >= 3)
                {
                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    Game602Model.Instance.lines.Add(tempLine);
                    Game602Model.Instance.elementList.Add(element);
                    if (element == SlotData_602.elementFree)
                        Game602Model.Instance.toSpin.FreeTimes = 5;
                    else
                        Game602Model.Instance.toSpin.FreeTimes = 0;
                }
            }
            if (Game602Model.Instance.lines.Count > 0)
            {
                Game602Model.Instance.toSpin.WinGold = 0;
                for (int i = 0;i < Game602Model.Instance.lines.Count;i++ )
                {
                    int rate = 0;
                    element3 = Game602Model.Instance.elementList[i] - 1;
                    if (element3 > 0)
                    {
                        int count = Game602Model.Instance.lines[i].Value;
                        if (count == 3)
                            rate = Game602Model.Instance.elementRate3[element3];
                        else if (count == 4)
                            rate = Game602Model.Instance.elementRate4[element3];
                        else
                            rate = Game602Model.Instance.elementRate[element3];
                    }
                    else
                    {
                        rate = UnityEngine.Random.Range(1, 6);
                    }
                    Game602Model.Instance.toSpin.WinGold += Game602Model.Instance.nBet1 * rate;
                }
                Game602Model.Instance.toSpin.rate = Game602Model.Instance.toSpin.WinGold / Game602Model.Instance.nBet1;
                Game602Model.Instance.toSpin.WinGold = Game602Model.Instance.toSpin.WinGold / 9;
                Game602Model.Instance.toSpin.n64Gold += Game602Model.Instance.toSpin.WinGold;
                Game602Model.Instance.toSpin.n64Gold -= Game602Model.Instance.nBet1;
            }
            else
            {
                Game602Model.Instance.toSpin.rate = 0;
                Game602Model.Instance.toSpin.WinGold = 0;
                Game602Model.Instance.toSpin.n64Gold -= Game602Model.Instance.nBet1;
            }

            PlayerPrefs.SetInt("DanJi", (int)Game602Model.Instance.toSpin.n64Gold);
        }


        public override void SetSpinData()
        {        
            recSpin();
        }



        public override async void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                //int times = SlotData_602.rollTimes + i * slotRow;// -- 慢速
                //if (isFastSpin)// --快速
                //    times = slotRow * 2 + 1;
                //slotColumns[i].endRoll(times);
                UIRomm602SlotColumn slotColumn = slotColumns[i] as UIRomm602SlotColumn;
                slotColumn.SetRollElement();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));
            }
        }

        public override void finishSpin()
        {
            if (Game602Model.Instance.toSpin == null)
                return;
            base.finishSpin();
            setState(slotState.SpinEnd);
            //-- 获得免费
            if (Game602Model.Instance.toSpin.FreeTimes > 0)
            {
                    freeTimes.max = Game602Model.Instance.toSpin.FreeTimes;//-- + freeTimes.max
                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game602Model.Instance.toSpin.WinGold;
                if (!isFreeSpin && Game602Model.Instance.toSpin.nModelGame == Game602Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                else if (Game602Model.Instance.lastCount > 0 && Game602Model.Instance.toSpin.nModelGame > Game602Model.Instance.lastCount)
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                isFreeSpin = true;
                Game602Model.Instance.lastCount = Game602Model.Instance.toSpin.nModelGame;
            }
            //-- 免费结束
            if (isFreeSpin &&(Game602Model.Instance.toSpin.nModelGame == Game602Model.Instance.toSpin.FreeTimes) && Game602Model.Instance.toSpin.FreeTimes == 0)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
                Game602Model.Instance.lastCount = 0;
            }
            for (int i = 0; i < SlotData_602.column; i++)
                slotColumns[i].onSpinFinish(Game602Model.Instance.toSpin.WinGold > 0);
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);
            ShowLight(false);
            ShowOneLine(Game602Model.Instance.lines);
            mgr.RunStepList();
        }

        public override void showLines()
        {
            mask.gameObject.SetActive(true);
            ShowAllCell(false);
            UIRoom602Top top602 = uiTop as UIRoom602Top;
            for (int i = 0; i < Game602Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game602Model.Instance.lines[i];

                List<int> elementPos = Game602Model.Instance.lineData[tempLine.Key];
                top602.ShowOneLine(tempLine.Key);
                bool bLeft = Game602Model.Instance.arrayLineLeft[tempLine.Key - 1] == 1;
                for (int j = 0; j < tempLine.Value; j++)
                {
                    if (bLeft)
                        slotColumns[j].lstCells[2 - elementPos[j] + 1].showLine();
                    else
                        slotColumns[4 - j].lstCells[2 - elementPos[4 - j] + 1].showLine();
                }
            }


            if (Game602Model.Instance.lines.Count > 0)
            {
                top602.Line.sizeDelta = new(750, 516);
                top602.Line.DOSizeDelta(new Vector2(750, 516), 0.3f)/*.SetEase(Ease.InQuad)*/;

                //SengPao();
            }

            ShowLight(false);
            ShowOneLine(Game602Model.Instance.lines);
            ShowAllCell(true, true);
            int index = 0;
            CoreEntry.gTimeMgr.AddTimer(2f, true, () =>
            {
                ShowLight(false);
                ShowAllCell(false);
                if (index == Game602Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.8f, false, () =>
                    {
                        ShowAllCell(true, true);
                        ShowOneLine(Game602Model.Instance.lines);
                    }, 400);
                }
                else
                {
                    float times = 1;
                    KeyValuePair<int, int> tempLine = Game602Model.Instance.lines[index];
                    List<int> elementPos = Game602Model.Instance.lineData[tempLine.Key];
                    bool bLeft = Game602Model.Instance.arrayLineLeft[tempLine.Key - 1] == 1;
                    //float tempTimes = 1;
                    //for (int i = 0; i < tempLine.Value; i++)
                    //{
                    //    if (bLeft)
                    //        tempTimes = slotColumns[i].lstCells[2 - elementPos[i] + 1].TfSpine.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>().animation.GetState("newAnimation")._animationData.duration;
                    //    else
                    //        tempTimes = slotColumns[4 - i].lstCells[2 - elementPos[4 - i] + 1].TfSpine.GetChild(0).GetComponent<DragonBones.UnityArmatureComponent>().animation.GetState("newAnimation")._animationData.duration;
                    //    if (tempTimes > times)
                    //        times = tempTimes;
                    //}

                    CoreEntry.gTimeMgr.AddTimer(2f - times, false, () =>
                    {
                        top602.ShowOneLine(tempLine.Key);
                        List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                        temp.Add(tempLine);
                        ShowOneLine(temp, true);

                        for (int j = 0; j < tempLine.Value; j++)
                        {
                            if (bLeft)
                                slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);
                            else
                                slotColumns[4 - j].lstCells[2 - elementPos[4 - j] + 1].ShowCellEffect(true);
                        }
                        index++;
                    }, 400);
                }
            }, 399);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game602Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game602Model.Instance.lines[i];
                List<int> elementPos = Game602Model.Instance.lineData[tempLine.Key];
                bool bLeft = Game602Model.Instance.arrayLineLeft[tempLine.Key - 1] == 1;
                for (int j = 0; j < tempLine.Value; j++)
                {
                    if (bLeft)
                        slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
                    else
                        slotColumns[4 - j].lstCells[2 - elementPos[4 - j] + 1].ShowCellEffect(bShow, bAllTrue);
                }
            }

            UIRoom602Top top602 = uiTop as UIRoom602Top;
            top602.HideAllLines(bShow, Game602Model.Instance.lines);
            for (int i = 0; i < LineDeng_2.Count; i++)
                LineDeng_2[i].gameObject.SetActive(false);
            if (bShow)
            {
                for (int i = 0; i < Game602Model.Instance.lines.Count; i++)
                    LineDeng_2[Game602Model.Instance.lines[i].Key - 1].gameObject.SetActive(false);
            }
        }

        public UISlotCell GetCell(int col,int row)
        {
            return slotColumns[col].lstCells[row];
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game602Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game602Model.Instance.slotResult[cr2i(c, r)];
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {      
            num = num0;
            PlayAni();
            //--手动或免费时num为0
            isFastSpin = CommonTop602.GetToggleIsOn();
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                CommonTop602.SetFreeTimes(true, Game602Model.Instance.toSpin.FreeTimes + "");
            }
            else
            {
                if (!CommonTop602.GetGoFreeTimes().activeSelf)
                {
                    if (CommonTop602.Bet == CommonTop602.betMin && Game602Model.Instance.toSpin.n64Gold < (long)(CommonTop602.Bet * 9))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        CommonTop602.SetSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        CommonTop602.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game602Model.Instance.toSpin.n64Gold < (long)(CommonTop602.Bet * 9))
                        {
                            int tempBet1 = CommonTop602.Bet;
                            for (int i = CommonTop602.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = CommonTop602.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    CommonTop602.OnClickBtnMin();
                                    if (Game602Model.Instance.toSpin.n64Gold >= (long)(tempBet * 9 ))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if (!bCanSlot)
                            {
                                autoSpinNum = 0;// -- 停止自动
                                CommonTop602.SetSlotSpinNum(autoSpinNum);
                                setState(slotState.Idle);
                                CommonTop602.ShowNotEnoughMoneyTips();
                                return;
                            }
                        }
                    }
                }

                isFreeSpin = false;
                if (num > 0)
                {
                    autoSpinNum = num - 1;
                    CommonTop602.SetSlotSpinNum(autoSpinNum);
                }
                else
                {
                    autoSpinNum = num;
                    CommonTop602.SetSlotSpinNum(autoSpinNum);
                }
            }
            for (int i = 0; i < SlotData_602.column; i++)
            {
                slotColumns[i].reset();
                //slotColumns[i].beginRoll(-1);
            }
         
            if (Game602Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = CommonTop602.Bet * 9 ;
                CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold - (int)(betValue)); 
            }
            PlayLineLight();
            setState(slotState.SpinBegin);
            uiTop.freeAni.Kill();
            uiTop.freeAni = null;
            uiTop.TfFree.gameObject.SetActive(false);
            preSpin();
            CoreEntry.gAudioMgr.StopSound(uiTop.transform.GetChild(2).gameObject);
            CoreEntry.gAudioMgr.PlayUISound(294);
            sendSpin();
            mask.gameObject.SetActive(false);
            CommonTop602.SetRollBtnRorate(true);
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
            if (isFreeEnd())
                freeTimes.reset();
            CommonTop602.UpdateGold(Game602Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (freeTimes.max <= 0)
            {
                if (MainUIModel.Instance.palyerData.m_i8Diamonds >= 30)
                {
                    if (GuideModel.Instance.bReachCondition(9) && GuideModel.Instance.bReachCondition(6))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        CommonTop602.SetSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        GuideModel.Instance.SetFinish(9);
                        if (MainPanelMgr.Instance.IsShow("TaskPanel"))
                        {
                            MainPanelMgr.Instance.Close("TaskPanel");
                        }
                        MainUICtrl.Instance.OpenGuidePanel(CommonTop602.GetLeaveBtn().transform, CommonTop602.OnClickBtnLeave,5);
                        return;
                    }
              
                }
            }

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
            return Game602Model.Instance.toSpin.FreeTimes == 0;
        }

        protected override int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (Game602Model.Instance.slotResult == null)
                return count;
            for (int i = 0; i < Game602Model.Instance.slotResult.Count; i++)
            {
                if (Game602Model.Instance.slotResult[i] == ele || (orWild && Game602Model.Instance.slotResult[i] == SlotData_602.elementWild))
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
            CoreEntry.gTimeMgr.AddTimer(0.01f, true, () =>
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

            CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
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
                LineDeng_2[line -1].gameObject.SetActive(true);
                if (bOne)
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
            if(CommonTop602.GetBeginBtn().interactable == true && CommonTop602.GetBeginBtn().gameObject.activeSelf)
            {
                CommonTop602.BeginSlot(0);
                CommonTop602.GetBeginBtn().interactable = false;
            }
        }

        public void PlayAni()
        {
            //Animator["RollAni"].speed = 2.5f;
            //Animator.Play("RollAni");
        }


        private void SengPao()
        {
            for (int i = 0; i < 5; i++)
            {
                int count = 0;
                UIRomm602SlotColumn column = slotColumns[i] as UIRomm602SlotColumn;           
                for (int j = 0; j < column.lstCells.Count; j++)
                {
                    if (column.lstCells[j].TfSpine.childCount > 0)
                        count++;
                }
                column.MovePao(i, count);
            }
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
                    CommonTop602.SetRollBtnRorate(false);
                
                    CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                    {
                        finishSpin();
                    }, 9);
                }
                else if (column > 0)
                { //出现2个免费，下一列加速
                    if (!isFastSpin &&false)
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
            catch (Exception e)
            {
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(291);
        }
    }

}


  