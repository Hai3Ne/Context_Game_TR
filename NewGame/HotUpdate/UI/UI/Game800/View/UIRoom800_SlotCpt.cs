using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public static class SlotData_800
    {
        public static int column = 3;
        public static int elementFree = 10;
        public static int specialelement = 14;
        public static int elementWild = 5;
        public static int elementCount = 14;
        public static float rollTime = 0.08f;
        public static int rollTimes = 18;
        public static float rollAccTime = 0.018f;
        public static int rollAccTimes = 34;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_800
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int nHandSize = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }




    public class UIRoom800_SlotCpt : UIRoom_SlotCpt
    {
        public Room800Mgr mgr { get; private set; } = new Room800Mgr();

 
        private Transform RollBar;
        public Button BtnRollBar;
        private Animation Animator;

        public DragonBones.UnityArmatureComponent SpineAniKuang;
        public DragonBones.UnityArmatureComponent SpineAniKuang2;

        private Transform TfLight;

        private int clickCount = 0;

        public CommonTop commonTop;
        protected override void Awake()
        {
            base.Awake();
            mgr.Init(this, Game800Model.Instance.toSpin);

     
            RollBar = transform.Find("bg/Image/RollBar");
            BtnRollBar = transform.Find("bg/Image/RollBar/Bg/BtnRollBar").GetComponent<Button>();
            BtnRollBar.onClick.AddListener(ClickRodar);


            Animator = RollBar.GetComponent<Animation>();
            columnCount = 3;


            //--行数
            Transform tfColumn = TfSlot.GetChild(2);
            slotRow = tfColumn.childCount - 1;
            //-- 行高
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;

            SpineAniKuang = transform.Find("GoSlot/TfSlot/SpineAniKuang").GetComponent<DragonBones.UnityArmatureComponent>();
            SpineAniKuang2 = transform.Find("GoSlot/TfSlot/SpineAniKuang2").GetComponent<DragonBones.UnityArmatureComponent>();

            TfLight = transform.Find("bg/Image/TfLight");
         
        }

        protected override void Start()
        {
            GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
            commonTop = go.GetComponent<CommonTop>();
            commonTop.SetDanJi(bDanJi, true, true);
            commonTop.SlotCpt = this;

            uiTop = transform.Find("TopPanel").GetComponent<UIRoom800Top>();
            uiTop.Init(this);
            init();
            uiTop.InitData();
            uiTop.transform.SetAsLastSibling();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            Game800Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(49);
            if (uiTop != null)
            {
                autoSpinNum = 0;
                InitData();
                uiTop.Init(this);
                uiTop.InitData();
            }
            gameStatus = 0;
            num = 0;
            clickCount = 0;
            PlayTopLight();
            Game800Model.Instance.bOneList = new List<bool> { true, true, true };
        }

        public void PlayTopLight()
        {
            CoreEntry.gTimeMgr.RemoveTimer(8889);
            CoreEntry.gTimeMgr.AddTimer(0.35f, true, () =>
            {
                TfLight.gameObject.SetActive(!TfLight.gameObject.activeSelf);
            }, 8889);
        }

        public void Reconnect()
        {
            if (uiTop != null)
            {
                commonTop.SetRollBtnRorate(false);
                InitData();
                if (IsAutoSpin())
                {
                    CoreEntry.gTimeMgr.RemoveTimer(100000);
                    commonTop.BeginSlot(-1);
                }
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            Game800Model.Instance.bOneList.Clear();
            for (int i = 0; i < SlotData_800.column; i++)
            {
                UIRomm800SlotColumn column = lstcolumns[i].GetComponent<UIRomm800SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < column.transform.childCount; j++)
                {
                    UIRoom800SlotCell cell = column.SelfTransform.GetChild(j).GetComponent<UIRoom800SlotCell>();
                    cell.init(column, uiTop);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }



        public override void preSpin()
        {
            CoreEntry.gTimeMgr.RemoveTimer(9);//快速停止和普通停止会一直执行
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);

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
                if (Game800Model.Instance.freeGameData.Count <=0)
                {
                    setState(slotState.SpinBegin);
                    int num = (commonTop.Bet);
                    Game800Ctrl.Instance.Send_CS_GAME5_BET_REQ((int)num);
                }
                else
                {
                    setState(slotState.SpinBegin);
                    gameStatus = 1;
                    SC_GAME5_BET_RET temp = new SC_GAME5_BET_RET();
                    temp.nAllBet = 0;
                    temp.n64Gold = Game800Model.Instance.toSpin.n64Gold;
                    temp.n64Jackpot = Game800Model.Instance.n64Jackpot;

                    SGame5HandInfo info = new SGame5HandInfo();
                    info.n64CommPowerGold = Game800Model.Instance.freeGameData[0].n64CommPowerGold;
                    info.n64RSPowerGold = Game800Model.Instance.freeGameData[0].n64RSPowerGold;
                    info.n64TotalGold = Game800Model.Instance.freeGameData[0].n64TotalGold;
                    info.arrayLogo = Game800Model.Instance.freeGameData[0].arrayLogo;
                    temp.sInfo.Add(info);
                    Game800Model.Instance.freeGameData.RemoveAt(0);
                    SetKuangAni(5,false);

                    Game800Ctrl.Instance.OnGAME3_BET_RE_800(temp);
                }                    
            }
        }
   

        public override void RandomSpinData()
        {
            Game800Model.Instance.bPlayFinished = false;
            Game800Model.Instance.bFreeGameFinished = false;

            Game800Model.Instance.slotResult.Clear();

            bool bWin = UnityEngine.Random.Range(1, 8) <= 3; ;

            int element = UnityEngine.Random.Range(1,10);
            for (int i = 0;i < 3;i++)
                Game800Model.Instance.slotResult.Add(element);
            if (!bWin)
            {
                Game800Model.Instance.toSpin.WinGold = 0;
                Game800Model.Instance.toSpin.rate = 0;
                Game800Model.Instance.slotResult[1] = 10;
             
                if (Game800Model.Instance.slotResult[0] == 1)
                {
                    Game800Model.Instance.slotResult[0] = 2;
                    Game800Model.Instance.slotResult[2] = 2;
                }
                Game800Model.Instance.slotResult[0] = UnityEngine.Random.Range(5, 10);
            }
            else
            {
                int rate = Game800Model.Instance.rate[element - 1];
                Game800Model.Instance.toSpin.WinGold = Game800Model.Instance.nBet1 * rate;
                Game800Model.Instance.toSpin.rate = rate;
                if (Game800Model.Instance.slotResult[0] == 1)
                {
                    Game800Model.Instance.slotResult[0] = 2;
                    Game800Model.Instance.slotResult[2] = 2;
                    if (Game800Model.Instance.slotResult[1] == 1)
                        Game800Model.Instance.slotResult[1] = 7;
                }
                else
                {
                    if(Game800Model.Instance.slotResult[1] == 2 || Game800Model.Instance.slotResult[1] == 3 || Game800Model.Instance.slotResult[1] == 4)
                    {
                        Game800Model.Instance.slotResult[1] = 10;
                        Game800Model.Instance.toSpin.WinGold = 0;
                        Game800Model.Instance.toSpin.rate = 0;
                    }
                }
            }
           if(Game800Model.Instance.toSpin.WinGold > 0)
            {
                Game800Model.Instance.toSpin.rate = Game800Model.Instance.toSpin.rate;
                Game800Model.Instance.toSpin.n64Gold += Game800Model.Instance.toSpin.WinGold;
                Game800Model.Instance.toSpin.n64Gold -= Game800Model.Instance.nBet1;
            }
           else
            {
                Game800Model.Instance.toSpin.rate = 0;
                Game800Model.Instance.toSpin.WinGold = 0;
                Game800Model.Instance.toSpin.n64Gold -= Game800Model.Instance.nBet1;
            }
     
        }
   

        public void SetKuangAni(int type = 0,bool bLoop = true)
        {
        
            string spineAni = "";
            string spineAniName = "newAnimation";
            Vector3 pos = Vector3.zero;
            if (type == 0)
            {
                pos = slotColumns[0].transform.localPosition;// new Vector2(slotColumns[0].rtf.anchoredPosition.x, 0);
                spineAni = "bai1";
            }            
            else if(type == 1)
            {
                pos = slotColumns[2].transform.localPosition;// new Vector2(slotColumns[2].rtf.anchoredPosition.x, 0);
                spineAni = "bai2";
            }
            else if(type == 3)
            {
                pos = slotColumns[1].transform.localPosition;// new Vector2(slotColumns[1].rtf.anchoredPosition.x, 0);
                spineAni = "hongse";
            }
            else if(type == 4)
            {
                pos = slotColumns[1].transform.localPosition;// new Vector2(slotColumns[1].rtf.anchoredPosition.x, 0);
                spineAni = "huangse";
            }
            else
            {
                pos = slotColumns[1].transform.localPosition;// new Vector2(slotColumns[1].rtf.anchoredPosition.x, 0);
                spineAni = "zhongyang";
                spineAniName = "start";
            }

            if(type == 0)
            {
                SpineAniKuang2.gameObject.SetActive(true);
                CommonTools.SetArmatureName(SpineAniKuang2.transform, spineAni);
                CommonTools.PlayArmatureAni(SpineAniKuang2.transform, spineAniName, bLoop ? 0 : 1, () =>
                {
                   
                            SpineAniKuang2.gameObject.SetActive(false);

                });
                SpineAniKuang2.transform.localPosition = new Vector3(pos.x, 0, 0);
            }
            else
            {
                SpineAniKuang.gameObject.SetActive(true);
                CommonTools.SetArmatureName(SpineAniKuang.transform, spineAni);
                CommonTools.PlayArmatureAni(SpineAniKuang.transform, spineAniName, bLoop ? 0 : 1, () =>
                {
                    if (type == 5)
                    {
                        CommonTools.PlayArmatureAni(SpineAniKuang.transform, "idle", 0);
                    }
                    else
                    {
                            SpineAniKuang.gameObject.SetActive(false);
                    }

                });
                SpineAniKuang.transform.localPosition = new Vector3(pos.x, 0, 0);
            }
        }

        public override void SetSpinData()
        {
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < SlotData_800.column; i++)
            {
                int times = SlotData_800.rollTimes + i * slotRow*3;// -- 慢速
                //if (Application.isEditor)
                //    times += 250;
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                else if (uiTop.TfFree.gameObject.activeSelf)
                    times = SlotData_800.rollAccTimes;

                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game800Model.Instance.toSpin == null)
                return;
            base.finishSpin();
            setState(slotState.SpinEnd);
            if (Game800Model.Instance.freeGameData.Count > 0) //-- 触发免费
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                isFreeSpin = true;
            }
                

            //-- 免费结束
            if (Game800Model.Instance.freeGameData.Count <=0 && uiTop.TfFree.gameObject.activeSelf)
            {
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFreeEnd, null);
                isFreeSpin = false;
            }
            for (int i = 0; i < SlotData_800.column; i++)
                slotColumns[i].onSpinFinish(Game800Model.Instance.toSpin.WinGold > 0);
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(555);
            mgr.RunStepList();

        }

        public override void showLines()
        {
            UIRoom800Top top800 = uiTop as UIRoom800Top;
            //for (int i = 0; i < Game800Model.Instance.lines.Count; i++)
            //{
            //    KeyValuePair<int, int> tempLine = Game800Model.Instance.lines[i];

            //    List<int> elementPos = Game800Model.Instance.lineData[tempLine.Key];
            for (int j = 0; j < SlotData_800.column; j++)
            {
                slotColumns[j].lstCells[2].index = j;
                if(Game800Model.Instance.slotResult[j] != 10)
                    slotColumns[j].lstCells[2].showLine();
            }
            //}
     
            //CoreEntry.gTimeMgr.AddTimer(2, true, () =>
            //{
            //    //ShowAllCell(false);


            //    if (index == Game800Model.Instance.lines.Count)
            //    {
            //        index = 0;
            //        CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>
            //        {
            //            ShowAllCell(true, true);
            //            index = index + 1;
            //            if (index > Game800Model.Instance.lines.Count)
            //                index = 0;
            //        }, 400);
            //    }
            //    else
            //    {
            //        CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>
            //        {
            //            KeyValuePair<int, int> tempLine = Game800Model.Instance.lines[index];
            //            List<int> elementPos = Game800Model.Instance.lineData[tempLine.Key];
            //            for (int j = 0; j < tempLine.Value; j++)
            //                slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);

            //            index++;
            //            //if (index >= Game500Model.Instance.lines.Count)
            //            //    index = 0;
            //        }, 400);
            //    }
            //}, 399);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game800Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game800Model.Instance.lines[i];
                List<int> elementPos = Game800Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game800Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game800Model.Instance.slotResult[cr2i(c, r)];
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            num = num0;
            UIRoom800Top top600 = uiTop as UIRoom800Top;
            top600.SetTxtScore(0);
            SpineAniKuang.gameObject.SetActive(false);
            CoreEntry.gAudioMgr.StopSound();
            PlayAni();
            //--手动或免费时num为0
            isFastSpin = false;// commonTop.GetToggleIsOn();
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true, Game800Model.Instance.toSpin.FreeTimes + "");
            }
            else
            {
                if(!uiTop.TfFree.gameObject.activeSelf)
                {
                    if (commonTop.Bet == commonTop.betMin && Game800Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
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
                        if (Game800Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                        {
                            int tempBet1 = commonTop.Bet;
                            for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = commonTop.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    commonTop.OnClickBtnMin();
                                    if (Game800Model.Instance.toSpin.n64Gold >= tempBet)
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
            for (int i = 0; i < SlotData_800.column; i++)
            {
                if (Game800Model.Instance.freeGameData.Count > 0 && i == 1)
                    continue;
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
                if (Game800Model.Instance.bOneList[i])
                    slotColumns[i].lstCells[5].ImgElement.gameObject.SetActive(true);
            }

            if (Game800Model.Instance.toSpin.nModelGame <= 0 || Game800Model.Instance.freeGameData.Count <= 0)
            {
                int betValue = commonTop.Bet;
                commonTop.UpdateGold(Game800Model.Instance.toSpin.n64Gold - (int)(betValue));
            }

            CoreEntry.gAudioMgr.StopSound();
            if (Game800Model.Instance.freeGameData.Count > 0)
                CoreEntry.gAudioMgr.PlayUISound(65);
            else
            {
                int sounID = 60 + (clickCount % 4);
                CoreEntry.gAudioMgr.PlayUISound(sounID);
                clickCount++;
            }
             
            awaiting = true;
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

        public override void continueSpin()
        {
            //普通或免费结束后再加金币
            if (isFreeEnd())
                freeTimes.reset();
            commonTop.UpdateGold(Game800Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);

            UIRoom800Top top800 = uiTop as UIRoom800Top;

            if (freeTimes.max > 0)
            {
                beginSpin(0, isFastSpin);
            }
            else if (autoSpinNum > 0)
            {
                if (autoSpinNum >0)
                    beginSpin(autoSpinNum, isFastSpin);
                else
                    isFastSpin = false;
            }
            else if (Game800Model.Instance.freeGameData.Count > 0)
            {
                beginSpin(0, true);
            }
            else if (autoSpinNum >0 )
            {
                beginSpin(0, false);
            }
            else
                isFastSpin = false;
        }

        public override bool isFreeEnd()
        {
            return Game800Model.Instance.toSpin.FreeTimes == 0;
        }

        protected override int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (Game800Model.Instance.slotResult == null)
                return count;
            for (int i = 0; i < Game800Model.Instance.slotResult.Count; i++)
            {
                if (Game800Model.Instance.slotResult[i] == ele || (orWild && Game800Model.Instance.slotResult[i] == SlotData_800.elementWild))
                    count++;
            }
            return count;
        }

        //private bool nextColumn(int col, int card)
        //{
        //    if (col >= SlotData_500.column)
        //        return false;

        //    bool hasCard = false;
        //    for (int r = 0; r < slotRow; r++)
        //    {
        //        int idx = cr2i(col, r);
        //        if (slotResult[idx] == card || slotResult[idx] == SlotData_500.elementWild)
        //        {
        //            line[col] = idx;
        //            //下列没有且已有3列
        //            if (nextColumn(col + 1, card) == false && col > 1)
        //            {
        //                //保存线
        //                List<int> cards = new List<int>();
        //                for (int i = 0; i < line.Length; i++)
        //                {
        //                    if (line[i] >= 0)
        //                        cards.Add(line[i]);
        //                }
        //                lines.Add(cards);
        //            }
        //            hasCard = true;
        //        }
        //    }
        //    //清空当前列标志
        //    line[col] = -1;
        //    return hasCard;
        //}

        public void ClickRodar()
        {
            BtnRollBar.interactable = false;
            if(commonTop.GetBeginBtn().interactable == true && commonTop.GetBeginBtn().gameObject.activeSelf)
            {
                if (IsAutoSpin())
                {
                    autoSpinNum = 0;
                    if (StateSlot == slotState.SpinSuccess)
                    {
                        endSpin();
                        CoreEntry.gAudioMgr.StopMusic(50);
                    }
                }
                else
                {
                    if (StateSlot == slotState.Idle)
                    {
                        CoreEntry.gTimeMgr.RemoveTimer(100000);
                        commonTop.BeginSlot(0);
                    }
                }

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
                slotColumns[i].init();
            }
            setState(slotState.Idle);
        }

        public override void finishRoll(int column)
        {
            try
            {
                if (column == columnCount - 1)
                { //最后一列结束

                    if (Game800Model.Instance.bPlayFinished)
                        return;
                    Game800Model.Instance.bPlayFinished = true;
                    UIRoom800Top top800 =uiTop as UIRoom800Top;
                    if (Game800Model.Instance.slotResult[1] != 1)
                        CoreEntry.gAudioMgr.StopSound(top800.GoSound);

           


                  
                    if (Game800Model.Instance.toSpin.WinGold > 0)
                    {
                        CoreEntry.gAudioMgr.StopSound(top800.GoSound2);
                        SetKuangAni(Game800Model.Instance.toSpin.rate < 20 ? 3 : 4);
                        int soundID = 54;
                        if (Game800Model.Instance.toSpin.rate < 2)
                            soundID = 54;
                        else if (Game800Model.Instance.toSpin.rate < 4)
                            soundID = 57;
                        else if (Game800Model.Instance.toSpin.rate < 12)
                            soundID = 58;
                        else
                            soundID = 59;

                        CoreEntry.gAudioMgr.PlayUISound(soundID, top800.GoSound2);


                    }
                    commonTop.SetRollBtnRorate(false);
                    CoreEntry.gTimeMgr.RemoveTimer(9);//快速停止和普通停止会一直执行

                    CoreEntry.gTimeMgr.AddTimer(0.9f, false, () =>
                    {
                        finishSpin();
                    }, 9);
                }
                else if (column > 0)
                { //出现2个免费，下一列加速
                    if (!isFastSpin && shouldAcc(column))
                    {
                        ////下一列加速
                        //slotColumns[column + 1].playAcc();
                        ////再后列等待
                        //for (int i = column + 2; i < slotColumns.Count; i++)
                        //{
                        //    slotColumns[i].endRoll(-1);
                        //}
                        //setState(slotState.SpinStop);
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
            if (column <= 1)
                return false;

            return Game500Model.Instance.slotResult[0] == Game500Model.Instance.slotResult[1];

            //出现免费元素数量大等于2
            //int freeNum = 0;
            //for (int i = 0; i < 3 * (column + 1); i++)
            //{
            //    if (Game500Model.Instance.slotResult[i] == SlotData_500.elementFree)
            //    {
            //        freeNum++;
            //    }
            //}
            //return freeNum >= 2;
            //if (column == 2)
            //{
            //    if (getElementByColumn(0) && getElementByColumn(1))
            //        return true;
            //}
            //else if(column == 3)
            //{
            //    if (getElementByColumn(1) && getElementByColumn(2))
            //        return true;
            //}
            //else if (column == 4)
            //{
            //    if (getElementByColumn(2) && getElementByColumn(3))
            //        return true;
            //}
            //else
            //{
            //    return false;
            //}
            //return false;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
             CoreEntry.gAudioMgr.StopMusic(49);
        }
    }

}


  