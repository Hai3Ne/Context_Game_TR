using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class UIRoom1200 : UIRoom_SlotCpt
    {
        public Top1200Panel uitop1200;

        public List<string> idleNames = new List<string> { "idle2","idle3","idle4","idle5"};
        public List<string> collectNames = new List<string> { "fx_wild_collect", "fx_wild_collect2", "fx_wild_collect3"};

        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            columnCount = 3;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1200Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(206);
            if (uitop1200 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1200/FirstRes/Top1200Panel");
                uitop1200 = go.GetComponent<Top1200Panel>();
                uitop1200.gameObject.SetActive(true);
                uitop1200.Init(this);
            }
            uitop1200.InitData();
            gameStatus = 0;
            num = 0;
            uitop1200.GetTrans_Mask().gameObject.SetActive(false);
          //  ChangeScreenCh(false);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { playTigerAni(1); }, 34);
        }

        protected override void Start()
        {
            init();
        }


        public void ChangeScreenCh(bool bHorizontal)
        {
            CanvasScaler m_canvasScaler = transform.GetComponent<CanvasScaler>();
            Camera camera = MainPanelMgr.Instance.uiCamera;
            ////修改设计分辨率
            //Vector2 vecReso = m_canvasScaler.referenceResolution;
            //if ((bHorizontal && vecReso.x < vecReso.y) || (!bHorizontal && vecReso.x > vecReso.y))
            //    m_canvasScaler.referenceResolution = new Vector2(vecReso.y, vecReso.x);

            //m_canvasScaler.matchWidthOrHeight = bHorizontal ? 1 : 0;

            //修改相机横竖比
            if ((bHorizontal && camera.aspect < 1) || (!bHorizontal && camera.aspect > 1))
                camera.aspect = 1 / camera.aspect;
            if (!bHorizontal)
                Screen.orientation = ScreenOrientation.Portrait;
            else
                Screen.orientation = ScreenOrientation.LandscapeLeft;

            ////通知编辑器切换
            //if (Application.isEditor)
            //{
            //    var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            //    var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
            //            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //    var gvWnd = EditorWindow.GetWindow(gvWndType);
            //    selectedSizeIndexProp.SetValue(gvWnd, bHorizontal ? 18 : 21, null);
            //}
        }


        public void Reconnect()
        {
            if (uitop1200 != null)
            {
                InitData();
                uitop1200.GetTextGold().text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(Game1200Model.Instance.toSpin.n64Gold);
                uitop1200.autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            uitop1200.OnSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_1200.column; i++)
            {
                UIRomm1200SlotColumn column = lstcolumns[i].GetComponent<UIRomm1200SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < 4;j++)
                {
                    UIRoom1200SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1200SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override void preSpin()
        {
            if(Game1200Model.Instance.gameStates != 1)
                uitop1200.ShowAllLines(false);
            uitop1200.ShowAllLineText(false);
            uitop1200.gameTips?.ClosePanel();

            uitop1200.GetTrans_Mask().gameObject.SetActive(false);
            uitop1200.SetTxtScore(0);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);

            if(bDanJi)
            {
                setState(slotState.SpinBegin);
                awaiting = true;

                if (Game1200Model.Instance.specialGameLists.Count > 0)
                {

                    Game1200Model.Instance.slotResult = Game1200Model.Instance.specialGameLists[0];

                    Game1200Model.Instance.specialGameLists.RemoveAt(0);
                    if (Game1200Model.Instance.specialGameLists.Count == 0)
                        Game1200Model.Instance.gameStates = 0;
                }
                else
                {
                    Game1200Model.Instance.gameStates = 0;
                    RandomSpinData();
                }


                float timer = (Game1200Model.Instance.specialGameLists.Count > 0 && !Game1200Model.Instance.bConfirmSpecialElement) ? 4f : 0.38f;
                if(isFastSpin)
                    timer = (Game1200Model.Instance.specialGameLists.Count > 0 && !Game1200Model.Instance.bConfirmSpecialElement) ? 4f : 0.1f;
                if (Game1200Model.Instance.gameStates == 1)
                {
                    StartCoroutine(DoGoSlotScale()); 
                    uitop1200.PlayGameTips(4);

                    for (int i = 0; i < SlotData_1200.column; i++)
                        slotColumns[i].duration = SlotData_1200.specialRollTime;
                }
                else
                {
                    if(Game1200Model.Instance.gameStates == 2)
                    {
                        StartCoroutine(DoGoSlotScale());
                        CoreEntry.gAudioMgr.PlayUISound(216);
                    }
                }

                if(timer > 1)
                {
                    playTigerAni(2);
                    uitop1200.PlayAppearRedPapgerAni(Game1200Model.Instance.ucLogo);

                    CoreEntry.gTimeMgr.AddTimer(timer - 1,false,()=> { PlaySpecialEffect(); },12);
                }

                CoreEntry.gTimeMgr.AddTimer(timer, false, () => 
                {
                    if (timer > 1)
                    {
                        CoreEntry.gAudioMgr.PlayUIMusic(205);
                        Game1200Model.Instance.bStartRollSpecialGame = true;
                    }
                    recSpin(); 
                }, 1000);
            }

        }

        public async override void sendSpin()
        {
            if (bDanJi == false)
            {
                setState(slotState.SpinBegin);
                gameStatus = 1;
                float num = (uitop1200.Bet);
                if (!uitop1200.GetTogTurboIsOn())
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.38f));
                Game1200Ctrl.Instance.Send_CS_GAME10_BET_REQ((int)num);
            }
        }

        public override void RandomSpinData()
        {
            Game1200Model.Instance.RandomSpinData();
        }

        public async override void SetSpinData()
        {

            if (Game1200Model.Instance.gameStates == 1)
            {

                StartCoroutine(DoGoSlotScale(1));
                uitop1200.PlayGameTips(4);

                for (int i = 0; i < SlotData_1200.column; i++)
                    slotColumns[i].duration = SlotData_1200.specialRollTime;

                playTigerAni(2);
                uitop1200.PlayAppearRedPapgerAni(Game1200Model.Instance.ucLogo);

                CoreEntry.gTimeMgr.AddTimer(3 - 1, false, () => { PlaySpecialEffect(); }, 12);


                CoreEntry.gTimeMgr.AddTimer(3f, false, () => 
                {
                    CoreEntry.gAudioMgr.PlayUIMusic(205);
                    Game1200Model.Instance.bStartRollSpecialGame = true;
                    recSpin(); 
                }, 51);
            }
            else
            {
                if(autoSpinNum >0)
                    PlayWaiBianKuangAni();
                if (Game1200Model.Instance.gameStates == 2)
                {
                    StartCoroutine(DoGoSlotScale(2));
                    CoreEntry.gAudioMgr.PlayUISound(216);
                }

                if (isFastSpin)
                    recSpin();
                else
                {
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.38f));
                    recSpin();
                }

            }    
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < SlotData_1200.column; i++)
            {
                int times = SlotData_1200.rollTimes + i * 4;// -- 慢速
                if (isFastSpin)// --快速
                    times = 6;
                if (Game1200Model.Instance.gameStates == 1)
                    times = SlotData_1200.specialRollTimes + i * 4;// -- 慢速
                if (Game1200Model.Instance.gameStates == 2)
                    times = 22 + i * 4;// -- 慢速
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game1200Model.Instance.toSpin == null)
                return;
            base.finishSpin();

            if (Game1200Model.Instance.specialGameLists.Count > 0 || Game1200Model.Instance.gameStates == 1)
                Game1200Model.Instance.bConfirmSpecialElement = true;   
      

            //Game1200Model.Instance.bFirstAppearSpecialGame = false;

            setState(slotState.SpinEnd);
    
            //-- 获得免费
            if (Game1200Model.Instance.toSpin.FreeTimes > 0)
            {
                //if (freeTimes.max <= 0)
                    freeTimes.max = Game1200Model.Instance.toSpin.FreeTimes;//-- + freeTimes.max


                if (freeTimes.winGold == 0)
                    freeTimes.winGold = Game1200Model.Instance.toSpin.WinGold;

                if (!isFreeSpin && Game1200Model.Instance.toSpin.nModelGame == Game1200Model.Instance.toSpin.FreeTimes) //-- 触发免费
                    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                //else if (Game1200Model.Instance.lastCount > 0 && Game1200Model.Instance.toSpin.nModelGame > Game1200Model.Instance.lastCount)
                //{
                //    CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotFree, null);
                //}
                isFreeSpin = true;

               // Game1200Model.Instance.lastCount = Game1200Model.Instance.toSpin.nModelGame;

            }

            if(Game1200Model.Instance.bHasElement(SlotData_1200.elementWild))
            {
                CoreEntry.gAudioMgr.PlayUISound(207);
                CoreEntry.gTimeMgr.AddTimer(0.4f,false,()=> { CoreEntry.gAudioMgr.PlayUISound(209); },30);
             
            }

            for (int i = 0; i < SlotData_1200.column; i++)
                slotColumns[i].onSpinFinish(Game1200Model.Instance.toSpin.WinGold > 0);

            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);

            if (Game1200Model.Instance.bAllLine == 0)
                StepWinGold();
            ContinueGame();
            if (Game1200Model.Instance.gameStates == 1 || (Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold <= 0))
                uitop1200.SpecialGameEffect();
            else if (Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold > 0)
            {
                uitop1200.SpecialGameEffect();
                Game1200Model.Instance.bStartRollSpecialGame = false;
                Game1200Model.Instance.bConfirmSpecialElement = false;
                //Game1200Model.Instance.gameStates = 0;
                m_Dragon_SpecialEffectLuoXia0.gameObject.SetActive(false);
                m_Dragon_SpecialEffectLuoXia1.gameObject.SetActive(false);
                m_Dragon_SpecialEffectLuoXia2.gameObject.SetActive(false);
                playTigerAni(3);           
                    uitop1200.PlayBigAwardEffect(Game1200Model.Instance.ucLogo);
            }
            else if(Game1200Model.Instance.bAllLine == 1)
            {
                Game1200Model.Instance.ucLogo = Game1200Model.Instance.GetElement(SlotData_1200.elementWild);
                uitop1200.PlayBigAwardEffect(Game1200Model.Instance.ucLogo, 2);
                showLines(); 
            }
           
        }

        public override void showLines()
        {
            ShowAllCell(false);
            uitop1200.ShowAllLineText(false);
            ContinueShowLines();

        }

        int nBet1 = 0;
        private async void ContinueShowLines()
        {
            nBet1 = Game1200Model.Instance.nBet1;
            if (Game1200Model.Instance.bHasElement(7))
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            float times = 0.3f;
            float tempTimes = 1;
            for (int i = 0; i < Game1200Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1200Model.Instance.lines[i];

                List<int> elementPos = Game1200Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    UISlotCell cell = slotColumns[j].lstCells[2 - elementPos[j] + 1];
                    cell.showLine();
                    string aniName = "win";
                    if (uitop1200.elementList[j * 3 + elementPos[j] - 1].childCount > 0)
                        tempTimes = ToolUtil.GetAnimationDuration(uitop1200.elementList[j * 3 + elementPos[j] - 1].GetChild(0), aniName);

                    if (tempTimes > times)
                        times = tempTimes;
                }
                uitop1200.ShowOneLine(tempLine.Key, true);
            }
            ShowAllCell(true, true);
            uitop1200.GetTrans_Mask().gameObject.SetActive(true);

            // Debug.LogError("========="+ times);
            int index = 0;
            CoreEntry.gTimeMgr.AddTimer(1.8f, true, () =>
            {

                ShowAllCell(false);
                if(Game1200Model.Instance.lines.Count != 1)
                {
                    uitop1200.ShowAllLines(false);
                    uitop1200.ShowAllLineText(false);
                }
            
                if (index == Game1200Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
                    {
                        //ShowAllCell(false);
                        //uitop1200.ShowAllLines(false);
                        //uitop1200.ShowAllLineText(false);
                        for (int i = 0; i < Game1200Model.Instance.lines.Count; i++)
                        {
                            KeyValuePair<int, int> tempLine = Game1200Model.Instance.lines[i];
                            uitop1200.ShowOneLine(tempLine.Key, true);
                        }
                        ShowAllCell(true, false);
                    }, 400);
                }
                else
                {
                    KeyValuePair<int, int> tempLine = Game1200Model.Instance.lines[index];
                    List<int> elementPos = Game1200Model.Instance.lineData[tempLine.Key];
                    List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                    temp.Add(tempLine);
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
                    {
                        uitop1200.ShowOneLine(tempLine.Key, true);
                        for (int j = 0; j < tempLine.Value; j++)
                        {
                            slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(true);
                        }
                        int element0 = slotColumns[0].lstCells[2 - elementPos[0] + 1].element;
                        int element1 = slotColumns[1].lstCells[2 - elementPos[1] + 1].element;
                        int element2 = slotColumns[2].lstCells[2 - elementPos[2] + 1].element;
                        if (element0 == element1 && element0 == element2)
                            uitop1200.SetLineText(tempLine.Key, Game1200Model.Instance.elementRate3[element0 - 1] * nBet1);
                        else
                        {
                            int element = 0;
                            if (element0 != SlotData_1200.elementWild)
                                element = element0;
                            if (element1 != SlotData_1200.elementWild)
                                element = element1;
                            if (element2 != SlotData_1200.elementWild)
                                element = element2;
                            uitop1200.SetLineText(tempLine.Key, Game1200Model.Instance.elementRate3[element - 1] * nBet1);
                        }

                        index++;
                    }, 400);
                }
            }, 399);
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game1200Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1200Model.Instance.lines[i];
                List<int> elementPos = Game1200Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }
        }



        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1200Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            return Game1200Model.Instance.slotResult[cr2i(c, r)];
        }

        public GameObject GetSoundObj()
        {
            return m_Trans_Sound.gameObject;
        }
        public GameObject GetSoundObj1()
        {
            return m_Trans_Sound1.gameObject;
        }

        public Transform GetMoveTarget()
        {
            return m_Trans_MoveTarget;
        }

        public Transform GetEffectParent()
        {
            return m_Trans_Effect;
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            //Debug.LogError("开始---");
            num = num0;
            //--手动或免费时num为0
            isFastSpin = false;
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                uitop1200.SetFreeTimes();
            }
            else
            {
                if(Game1200Model.Instance.gameStates != 3 && Game1200Model.Instance.gameStates != 1)
                {
                    if (uitop1200.Bet == uitop1200.betMin && Game1200Model.Instance.toSpin.n64Gold < (uitop1200.Bet*5))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        uitop1200.OnSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        uitop1200.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game1200Model.Instance.toSpin.n64Gold < (long)(uitop1200.Bet*5))
                        {
                            int tempBet1 = uitop1200.Bet;
                            for (int i = uitop1200.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = uitop1200.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    uitop1200.OnClickBtnMin();
                                    if (Game1200Model.Instance.toSpin.n64Gold >= (long)(tempBet*5))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if (!bCanSlot)
                            {
                                autoSpinNum = 0;// -- 停止自动
                                uitop1200.OnSlotSpinNum(autoSpinNum);
                                setState(slotState.Idle);
                                uitop1200.ShowNotEnoughMoneyTips();
                                return;
                            }
                        }
                    }
                }
               

                isFastSpin = uitop1200.GetTogTurboIsOn();
                if(Game1200Model.Instance.gameStates == 1 || (Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold <= 0))
                {
                    uitop1200.OnSlotSpinNum(autoSpinNum);
                }
                else if (num > 0)
                {
                    autoSpinNum = num - 1;
                    uitop1200.OnSlotSpinNum(autoSpinNum);
                }
                else
                {
                    autoSpinNum = num;
                    uitop1200.OnSlotSpinNum(autoSpinNum);
                }
            }
            setState(slotState.SpinBegin);
            preSpin();
            for (int i = 0; i < SlotData_1200.column; i++)
            {
                slotColumns[i].reset();
                if(isFastSpin)
                    slotColumns[i].duration = SlotData_1200.fastRollTimes;

                int index = i;
                if(!uitop1200.GetTogTurboIsOn())
                    CoreEntry.gTimeMgr.AddTimer(0.12f* index, false,()=> { slotColumns[index].beginRoll(-1); },71+index);
                else
                    slotColumns[i].beginRoll(-1);
            }
            if (Game1200Model.Instance.gameStates != 3 && Game1200Model.Instance.gameStates != 1)
            {
                int betValue = uitop1200.Bet *5;
                uitop1200.GetTextGold().text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(Game1200Model.Instance.toSpin.n64Gold - betValue);
                uitop1200.autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold - betValue);
            }
      
            if(uitop1200.GetTrans_Win().gameObject.activeSelf)
                uitop1200.ContiunePlayGameTipsLoop();
            sendSpin();

            uitop1200.SetRollBtnRorate(true, Game1200Model.Instance.gameStates == 1);
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
            uitop1200.GetTextGold().text = ToolUtil.GetCurrencySymbol()+"" + ToolUtil.ShowF2Num(Game1200Model.Instance.toSpin.n64Gold);// ((float)(Game1200Model.Instance.toSpin.n64Gold/10000f)).ToString("f2");// string.format("%.2f", )
            uitop1200.autoPanel.SetGoldText(Game1200Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (freeTimes.max > 0)
            {
                beginSpin(0, isFastSpin);
            } 
            else if (Game1200Model.Instance.specialGameLists.Count > 0 || Game1200Model.Instance.gameStates == 1 || (Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold <= 0))
            {
                beginSpin(autoSpinNum, false);
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
            return (isFreeSpin && (Game1200Model.Instance.toSpin.nModelGame == Game1200Model.Instance.toSpin.FreeTimes) && Game1200Model.Instance.toSpin.FreeTimes == 0);
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
               
                CoreEntry.gTimeMgr.AddTimer(0.3f, false, () => 
                {
                    if (Game1200Model.Instance.gameStates != 1)
                        uitop1200.SetRollBtnRorate(false);
                }, 91);

                CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>{finishSpin();}, 9);
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

        public void StepWinGold()
        {
            if (Game1200Model.Instance.toSpin.WinGold > 0&& Game1200Model.Instance.gameStates != 3)
            {
                //if (Game1200Model.Instance.gameStates == 1)
                //    return;
                showLines();

                if (Game1200Model.Instance.toSpin.WinGold > 0 || Game1200Model.Instance.lines.Count > 0)
                    uitop1200.OnSlotWinGold();

                if (Game1200Model.Instance.gameStates ==1 || Game1200Model.Instance.gameStates == 3)
                    uitop1200.SetAniParents();
       
            }
        }

        public void ContinueGame()
        {
            if (Game1200Model.Instance.toSpin.WinGold >0 && Game1200Model.Instance.gameStates == 3)//特俗游戏结算
                return;
            if (Game1200Model.Instance.bAllLine == 1)
                return;
            float delayTimes = 0.6f;
            if (Game1200Model.Instance.toSpin.WinGold > 0 && Game1200Model.Instance.toSpin.rate <=2)
                delayTimes = 1f;
            if(Game1200Model.Instance.toSpin.rate >2)
                delayTimes = 2.6f;
            if (autoSpinNum > 0 && Game1200Model.Instance.toSpin.WinGold > 0)
                delayTimes += 1.2f;
            CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () => { continueSpin(); }, 100000);
        }

        float tempTimes = 0.9f;
        public IEnumerator DoGoSlotScale(int type = 1)
        {
            uitop1200.GetTrans_Lines().localScale = new Vector3(0.92f, 0.92f, 0.92f);
            uitop1200.GetTransElementEffect().localScale = new Vector3(0.92f, 0.92f, 0.92f);
            TfSlot.parent.transform.DOScale(new Vector3(0.92f,0.92f,0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1200.GetTrans_Mask().DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_Bg.DOLocalMoveY(-30f, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_TopBg.DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            yield return new WaitForSeconds(tempTimes);
            if (type == 2)
                ResetScale();
        }

        public void ResetScale()
        {
            TfSlot.parent.transform.DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1200.GetTrans_Mask().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_Bg.DOLocalMoveY(0, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1200.GetTransElementEffect().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1200.GetTrans_Lines().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_TopBg.DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
        }

        public void PlayTexSpecialRateAni()
        {
            m_Txt_SpecialRate.transform.GetComponent<Canvas>().overrideSorting = false;
            m_Txt_SpecialRate.gameObject.SetActive(true);
            m_Txt_SpecialRate.transform.localPosition = new Vector3(25,487f,0);
            m_Txt_SpecialRate.transform.localScale = Vector3.one;
            Sequence seq = DOTween.Sequence();
            Tweener t1 = m_Txt_SpecialRate.transform.DOScale(new Vector3(1.2f,1.2f,1),0.25f);
            Tweener t2 = m_Txt_SpecialRate.transform.DOScale(Vector3.one, 0.25f);
            Tweener t3 = m_Txt_SpecialRate.transform.DOLocalMoveY(421, 0.25f);
            seq.Append(t1);
            seq.Append(t2);
            seq.Append(t3);
            seq.Play();
        }
        public void PlayTexSpecialRateAni2(Action callBack = null)
        {
            m_Txt_SpecialRate.gameObject.SetActive(true);
            m_Txt_SpecialRate.transform.GetComponent<Canvas>().overrideSorting = true;
            m_Txt_SpecialRate.transform.localPosition = new Vector3(0,421,0);
            Sequence seq = DOTween.Sequence();
            Tweener t1 = m_Txt_SpecialRate.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.35f);
            Tweener t2 = m_Txt_SpecialRate.transform.DOLocalMoveY(487f, 0.35f);
            seq.Append(t1);
            seq.Join(t2);
            seq.AppendInterval(0.5f);
            Tweener t3 = m_Txt_SpecialRate.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.8f);
            Tweener t4 = m_Txt_SpecialRate.transform.DOMove(uitop1200.GetTransWin().position, 0.8f);
            seq.Append(t3);
            seq.Join(t4);
            seq.AppendInterval(0.5f);
            seq.OnComplete(()=> 
            {
                m_Txt_SpecialRate.transform.GetComponent<Canvas>().overrideSorting = false;
                callBack?.Invoke();
                m_Txt_SpecialRate.gameObject.SetActive(false);
            });
            seq.Play();
        }

        public void playTigerAni(int type)
        {
            //Debug.LogError("播放动画类型========"+type);
            CoreEntry.gTimeMgr.RemoveTimer(212);
            if (type == 1)
            {
                int rand = UnityEngine.Random.Range(0, 4);
                string idleName = idleNames[rand];

                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, idleName, false, () =>
                  {
                      //ToolUtil.PlayAnimationAndLoop(m_Spine_Tigger.transform, "idle", "fx_idle3");
                      ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "idle", true, null);
                      ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_idle3", true, null, 2,false);

                      CoreEntry.gTimeMgr.AddTimer(10f,false,()=> { playTigerAni(1); },212);
                      //skeletonAnimation.AnimationState.SetAnimation(1, gunkeep, false);
                  });

                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_idle3", true, null, 2, false);

            }
            else if (type == 2)//懵逼
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "zo_start",false,()=>
                {
                    //ToolUtil.PlayAnimation(m_Spine_Tigger.transform, "zo_idle", false, () => 
                    //{
                    //});
                });
            }
            else if (type == 3)//赢钱
            {
                int rand = 0;// UnityEngine.Random.Range(0, 3);
                if (rand == 0)
                {
                    ToolUtil.Play3DAnimationAndLoop(m_Trans_SpineTiger.transform, new List<string> { "rs_win", "rs_win_idle"});
                }
                else if (rand == 1)
                {
                    ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win", false, () =>
                    {
                        playTigerAni(1);
                    });
                }
                else
                {
                    ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win2", false, () =>
                    {
                        playTigerAni(1);
                    });
                }

            }
            else if(type == 4)//开始特殊
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rs_start", false);
            }
            else if(type == 5)///跳舞
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rs_idle", true);
            }
            else if (type == 6)///不高兴
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "zo_exit", true);
            }
            else if(type == 7)///百变收集
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "wild_collect", false, () =>
                {
                    playTigerAni(1);
                });
                int rand = UnityEngine.Random.Range(0, 3);
                string idleName = collectNames[rand];
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, idleName, true, null, 2, false);
            }
            else if(type == 8)///举手庆祝  回复正常
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rs_win_exit",false,()=> { playTigerAni(1); });
            }
        }

        public void PlaySpecialEffect()
        {
            m_Dragon_SpecialEffect.gameObject.SetActive(true);

      
            m_Dragon_SpecialEffectLuoXia0.gameObject.SetActive(true);
            m_Dragon_SpecialEffectLuoXia1.gameObject.SetActive(true);
            m_Dragon_SpecialEffectLuoXia2.gameObject.SetActive(true);


            CommonTools.PlayArmatureAni(m_Dragon_SpecialEffectLuoXia0.transform, "LuoXiaLiZi", 0);
            CommonTools.PlayArmatureAni(m_Dragon_SpecialEffectLuoXia1.transform, "LuoXiaLiZi", 0);
            CommonTools.PlayArmatureAni(m_Dragon_SpecialEffectLuoXia2.transform, "LuoXiaLiZi", 0, () =>
            {

            });


            CommonTools.PlayArmatureAni(m_Dragon_SpecialEffect.transform, "LiZiKuang",1,()=> 
            {
                m_Dragon_SpecialEffect.gameObject.SetActive(false);
                PlayWaiBianKuangAni();
            });

            m_Dragon_BetiHouLeft.gameObject.SetActive(true);
            m_Dragon_BetiHouRight.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_BetiHouLeft.transform, "BeiHouLiZi", 1);
            CommonTools.PlayArmatureAni(m_Dragon_BetiHouRight.transform, "BeiHouLiZi", 1,()=> 
            {
                m_Dragon_BetiHouLeft.gameObject.SetActive(false);
                m_Dragon_BetiHouRight.gameObject.SetActive(false);
            });

        }

        public void PlayWaiBianKuangAni()
        {
            m_Dragon_SpecialKuangEffect.transform.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_SpecialKuangEffect.transform, "WaiBianKuangFaGaung", 1, () =>
            {
                m_Dragon_SpecialKuangEffect.transform.gameObject.SetActive(false);
            });
        }

        public Transform GetSpecialRate()
        {
            return m_Txt_SpecialRate.transform;
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(206);
           // ChangeScreenCh(true);
        }

    }

}


  