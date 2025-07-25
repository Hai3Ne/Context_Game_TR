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
    public partial class UIRoom1500 : UIRoom_SlotCpt
    {
        public Top1500Panel uitop1500;

        public List<string> idleNames = new List<string> { "idle2","idle3","idle4","idle5"};
        public List<string> collectNames = new List<string> { "fx_wild_collect", "fx_wild_collect2", "fx_wild_collect3"};

        int index;

        protected override void Awake()
        {
            //--列数
            TfSlot = transform.Find("GoSlot/TfSlot");
            for (int i = 0; i < TfSlot.childCount; i++)
            {
                Transform child = TfSlot.GetChild(i).GetChild(0);
                if (child.name.Contains("Column"))
                    lstcolumns.Add(child);
            }
            //--行数
            Transform tfColumn = TfSlot.GetChild(0);
            slotRow = tfColumn.childCount - 1;
            //-- 行高
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;
            slotColumns = new List<UISlotColumn>();
            effectAcc = transform.Find("GoSlot/EffectAcc").gameObject;
            GetBindComponents(gameObject);
            columnCount = 3;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1500Model.Instance.InitConfig();
            CoreEntry.gAudioMgr.PlayUIMusic(247);
            if (uitop1500 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject go = CommonTools.AddSubChild(transform.gameObject, "UI/Prefabs/Game1500/FirstRes/Top1500Panel");
                uitop1500 = go.GetComponent<Top1500Panel>();
                uitop1500.gameObject.SetActive(true);
                uitop1500.Init(this);
            }
            uitop1500.InitData();
            gameStatus = 0;
            num = 0;
            uitop1500.GetTrans_Mask().gameObject.SetActive(false);
            bDanJi = param == null ? false : (bool)param;
            PlayTigerAni(1);
        }

        protected override void Start()
        {
            init();
        }

        public void Reconnect()
        {
            if (uitop1500 != null)
            {
                InitData();
                uitop1500.GetTextGold().text =  ToolUtil.AbbreviateNumber(Game1500Model.Instance.toSpin.n64Gold);
                uitop1500.autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            uitop1500.OnSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_1500.column; i++)
            {
                UIRomm1500SlotColumn column = lstcolumns[i].GetComponent<UIRomm1500SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < (i!=1?4:5);j++)
                {
                    UIRoom1500SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1500SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override void preSpin()
        {
            if(Game1500Model.Instance.gameStates != 1)
                uitop1500.ShowAllLines(false);
            uitop1500.ShowAllLineText(false);
            uitop1500.gameTips?.ClosePanel();

            uitop1500.GetTrans_Mask().gameObject.SetActive(false);
      
            uitop1500.SetTxtScore(0);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            SetRollBg(0);
            if (bDanJi)
            {
                setState(slotState.SpinBegin);
                awaiting = true;    
                RandomSpinData();
                SetSpinData();
            }
        }

        public async override void sendSpin()
        {
            if (bDanJi == false)
            {
                setState(slotState.SpinBegin);
                gameStatus = 1;
                float num = (uitop1500.Bet);
                if (!uitop1500.GetTogTurboIsOn())
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.28f));
                Game1500Ctrl.Instance.Send_CS_GAME13_BET_REQ((int)num);
            }
        }

        public override void RandomSpinData()
        {
            Game1500Model.Instance.RandomSpinData();
        }

        public async override void SetSpinData()
        {
            if (Game1500Model.Instance.bInFreeGame())
            {
                if(Game1500Model.Instance.gameStates == 1)
                {
                    StartCoroutine(DoGoSlotScale(1, () => PlayTigerAni(4)));
                    PlayTigerAni(3);
                    uitop1500.PlayGameTips(4);
                  //  CoreEntry.gAudioMgr.PlayUISound(250);
                }

                for (int i = 0; i < SlotData_1500.column; i++)
                    slotColumns[i].duration = SlotData_1500.specialRollTime;
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2.5f));
                m_Dragon_leftEffect.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_leftEffect.transform, "Sprite",1, () => m_Dragon_leftEffect.gameObject.SetActive(false));
               // ToolUtil.PlayAnimation(m_Dragon_leftEffect.transform,"Sprite",false,()=> m_Dragon_leftEffect.gameObject.SetActive(false));
                m_Dragon_rightEffect.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_rightEffect.transform, "Sprite", 1, () => m_Dragon_rightEffect.gameObject.SetActive(false));
                SetRollBg(1);
                CoreEntry.gAudioMgr.PlayUISound(253);
                if (!m_Trans_Rodar.gameObject.activeSelf)
                {
                    m_Trans_Rodar.gameObject.SetActive(true);
                    m_Trans_Rodar.localScale = Vector3.zero;
                    Sequence seq = DOTween.Sequence();
                    Tweener t1 = m_Trans_Rodar.DOScale(new Vector3(1.1f, 1.1f, 1), 0.5f);
                    Tweener t2 = m_Trans_Rodar.DOScale(Vector3.one, 0.5f);
                    seq.Append(t1);
                    seq.Append(t2);
                    seq.OnComplete(() =>
                    {
                        seq.Kill();
                        seq = null;
                        m_Trans_Rodar.DOScale(new Vector3(1.1f, 1.1f, 1), 0.5f).SetLoops(-1, LoopType.Yoyo);
                    });
                    seq.Play();
                }
              
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.75f));     
            }
            else
            {
                if (Game1500Model.Instance.gameStates == 2)
                {
                    StartCoroutine(DoGoSlotScale(2));
                    CoreEntry.gAudioMgr.PlayUISound(249);
                }
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(isFastSpin ? 0 : 0.38f));          
            }
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < SlotData_1500.column; i++)
            {
                int times = SlotData_1500.rollTimes + i*4 ;// -- 慢速
                if (isFastSpin)// --快速
                    times = 6;
                if (Game1500Model.Instance.bInFreeGame())
                {
                    if (i != 1)
                        times = SlotData_1500.specialRollTimes;
                    else
                        times = SlotData_1500.specialRollTimes+35;
                }                  
                if (Game1500Model.Instance.gameStates == 2)
                    times = 22 + i * 4;// -- 慢速
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            if (Game1500Model.Instance.toSpin == null)
                return;
            base.finishSpin();

            slotColumns[1].transform.parent.GetComponent<Canvas>().overrideSorting = false;
            uitop1500.GetTrans_SpecialMask().gameObject.SetActive(false);
            setState(slotState.SpinEnd); 
            if(Game1500Model.Instance.bHasElement(SlotData_1500.elementWild))
            {
                PlayTigerAni(9);
                CoreEntry.gAudioMgr.PlayUISound(255);
                CoreEntry.gTimeMgr.AddTimer(0.4f,false,()=> { CoreEntry.gAudioMgr.PlayUISound(256); },30);

                if (Game1500Model.Instance.toSpin.WinGold <=0)
                    CoreEntry.gTimeMgr.AddTimer(0.6f, false, () => { CoreEntry.gAudioMgr.PlayUISound(257,transform.GetChild(0).gameObject); }, 35);
            }
            for (int i = 0; i < SlotData_1500.column; i++)
                slotColumns[i].onSpinFinish(Game1500Model.Instance.toSpin.WinGold > 0);

            if (Game1500Model.Instance.ucAllSame == 0)
            {
                if(!Game1500Model.Instance.bInFreeGame())
                {
                    StepWinGold();
                    ContinueGame();
                }               
                else
                {
                    if (Game1500Model.Instance.toSpin.WinGold > 0)
                    {
                        m_Txt_SpecialRate.gameObject.SetActive(true);                 
                        m_Txt_SpecialRate.transform.localScale = new Vector3(0.2f,0.2f,1);
                        m_Txt_SpecialRate.GetComponent<RectTransform>().anchoredPosition = new Vector3(-2, 25, 0);
                        m_Trans_Rodar.gameObject.SetActive(false);
                        ResetScale();
                        StepWinGold();
                        uitop1500.ClickBtnCloseEffect();
                        Game1500Model.Instance.gameStates = 0;
                        return;
                    }
                    ContinueGame();
                }
            }     
            else
            {
                uitop1500.PlayBigAwardEffect();
                StepWinGold();
                return;
            }      
        }

        public override void showLines()
        {
            ShowAllCell(false);
            uitop1500.ShowAllLineText(false);
            ContinueShowLines();
        }

        int nBet1 = 0;
        private async void ContinueShowLines()
        {
            nBet1 = Game1500Model.Instance.nBet1;
            if (Game1500Model.Instance.bHasElement(7))
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            float times = 0.3f;
            float tempTimes = 1;
            for (int i = 0; i < Game1500Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1500Model.Instance.lines[i];
                List<int> elementPos = Game1500Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    UISlotCell cell = slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1];
                    cell.showLine();
                    string aniName = "win";
                    if (uitop1500.elementList[j * 3 + elementPos[j] - 1].childCount > 0)
                        tempTimes = ToolUtil.GetAnimationDuration(uitop1500.elementList[j * 3 + elementPos[j] - 1].GetChild(0), aniName);

                    if (tempTimes > times)
                        times = tempTimes;
                }
                uitop1500.ShowOneLine(tempLine.Key, true);
            }
            ShowAllCell(true, true);
            uitop1500.GetTrans_Mask().gameObject.SetActive(true);
            index = 0;
            CoreEntry.gTimeMgr.AddTimer(1.8f, true, () =>
            {
                ShowAllCell(false);
                if(Game1500Model.Instance.lines.Count != 1)
                {
                    uitop1500.ShowAllLines(false);
                    uitop1500.ShowAllLineText(false);
                }
            
                if (index == Game1500Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
                    {
                        for (int i = 0; i < Game1500Model.Instance.lines.Count; i++)
                        {
                            KeyValuePair<int, int> tempLine = Game1500Model.Instance.lines[i];
                            uitop1500.ShowOneLine(tempLine.Key, true);
                        }
                        ShowAllCell(true, false);
                    }, 400);
                }
                else             
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>ShowOneLine(), 400);
            }, 399);
        }

        private async void ShowOneLine()
        {
            KeyValuePair<int, int> tempLine = Game1500Model.Instance.lines[index];
            List<int> elementPos = Game1500Model.Instance.lineData[tempLine.Key];
            List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
            temp.Add(tempLine);
            uitop1500.ShowOneLine(tempLine.Key, true);
            int element0 = slotColumns[0].lstCells[2 - elementPos[0] + 1].element;
            int element1 = slotColumns[1].lstCells[3 - elementPos[1] + 1].element;
            int element2 = slotColumns[2].lstCells[2 - elementPos[2] + 1].element;
            if (element0 == element1 && element0 == element2)
                uitop1500.SetLineText(tempLine.Key, Game1500Model.Instance.elementRate3[element0 - 1] * nBet1);
            else
            {
                int element = 0;
                if (element0 != SlotData_1500.elementWild)
                    element = element0;
                if (element1 != SlotData_1500.elementWild)
                    element = element1;
                if (element2 != SlotData_1500.elementWild)
                    element = element2;
                uitop1500.SetLineText(tempLine.Key, Game1500Model.Instance.elementRate3[element - 1] * nBet1);
            }
            for (int j = 0; j < tempLine.Value; j++)
            {
                slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1].ShowCellEffect(true);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));
                if (slotColumns[0].isRolling)
                    return;
            }
            index++;
        }


        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game1500Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1500Model.Instance.lines[i];
                List<int> elementPos = Game1500Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1500Model.Instance.slotResult.Count) { }
            return idx;
        }

        public override int cr2ele(int c, int r)
        {
            int idx = 0;
            if (c == 0)
                idx = r;
            else if (c == 1)
                idx = 3 + r;
            else if (c == 2)
                idx = 7 + r;
            return Game1500Model.Instance.slotResult[idx];
        }

        public GameObject GetSoundObj()
        {
            return m_Trans_Sound.gameObject;
        }
        public GameObject GetSoundObj1()
        {
            return m_Trans_Sound1.gameObject;
        }

        public Transform GetTransRodar()
        {
            return m_Trans_Rodar;
        }

        public Transform GetSpecialRate()
        {
            return m_Txt_SpecialRate.transform;
        }

        public Transform GetMoveTarget()
        {
            return m_Trans_MoveTarget;
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
                uitop1500.SetFreeTimes();
            }
            else
            {
                if(!Game1500Model.Instance.bInFreeGame())
                {
                    if (uitop1500.Bet == uitop1500.betMin && Game1500Model.Instance.toSpin.n64Gold < (uitop1500.Bet*10))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        uitop1500.OnSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        uitop1500.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game1500Model.Instance.toSpin.n64Gold < (long)(uitop1500.Bet*10))
                        {
                            int tempBet1 = uitop1500.Bet;
                            for (int i = uitop1500.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = uitop1500.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    uitop1500.OnClickBtnMin();
                                    if (Game1500Model.Instance.toSpin.n64Gold >= (long)(tempBet*10))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if (!bCanSlot)
                            {
                                autoSpinNum = 0;// -- 停止自动
                                uitop1500.OnSlotSpinNum(autoSpinNum);
                                setState(slotState.Idle);
                                uitop1500.ShowNotEnoughMoneyTips();
                                return;
                            }
                        }
                    }
                }
               

                isFastSpin = uitop1500.GetTogTurboIsOn();
                if(Game1500Model.Instance.bInFreeGame())
                    uitop1500.OnSlotSpinNum(autoSpinNum);
                else if (num > 0)
                {
                    autoSpinNum = num - 1;
                    uitop1500.OnSlotSpinNum(autoSpinNum);
                }
                else
                {
                    autoSpinNum = num;
                    uitop1500.OnSlotSpinNum(autoSpinNum);
                }
            }
            setState(slotState.SpinBegin);
            preSpin();
            for (int i = 0; i < SlotData_1500.column; i++)
            {
                slotColumns[i].reset();
                if(isFastSpin)
                    slotColumns[i].duration = SlotData_1500.fastRollTimes;

                int index = i;
                if (!uitop1500.GetTogTurboIsOn()&& i != 2)
                    CoreEntry.gTimeMgr.AddTimer(0.08f* index, false,()=> { slotColumns[index].beginRoll(-1); },71+index);
                else
                    slotColumns[i].beginRoll(-1);
            }
            if (!Game1500Model.Instance.bInFreeGame())
            {
                int betValue = uitop1500.Bet *10;
                uitop1500.GetTextGold().text =  ToolUtil.AbbreviateNumber(Game1500Model.Instance.toSpin.n64Gold - betValue);
                uitop1500.autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold - betValue);
            }
     
            if(uitop1500.GetTrans_Win().gameObject.activeSelf)
                uitop1500.ContiunePlayGameTipsLoop();
            sendSpin();
  
            uitop1500.SetRollBtnRorate(true, Game1500Model.Instance.bInFreeGame());
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
            uitop1500.GetTextGold().text =  ToolUtil.AbbreviateNumber(Game1500Model.Instance.toSpin.n64Gold);// ((float)(Game1500Model.Instance.toSpin.n64Gold/10000f)).ToString("f2");// string.format("%.2f", )
            uitop1500.autoPanel.SetGoldText(Game1500Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (freeTimes.max > 0)
                beginSpin(0, isFastSpin);
            else if (Game1500Model.Instance.bInFreeGame())
            {
                beginSpin(0, false);
            }
            else if (autoSpinNum > 0)
            {
                if (autoSpinNum > 0)
                    beginSpin(autoSpinNum, isFastSpin);
                else
                    isFastSpin = false;
            }
            else
                isFastSpin = false;
        }

        public override bool isFreeEnd()
        {
            return (isFreeSpin && (Game1500Model.Instance.toSpin.nModelGame == Game1500Model.Instance.toSpin.FreeTimes) && Game1500Model.Instance.toSpin.FreeTimes == 0);
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

        public override async void finishRoll(int column)
        {
            int finalColumn = 2;
            if (Game1500Model.Instance.bInFreeGame())
                finalColumn = 1;
            if (column == finalColumn)
            { //最后一列结束
                CoreEntry.gTimeMgr.RemoveTimer(9);
                CoreEntry.gAudioMgr.StopSound();
                CoreEntry.gTimeMgr.AddTimer(0.3f, false, () => 
                {
                        uitop1500.SetRollBtnRorate(false);
                }, 91);
                CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>{finishSpin();}, 9);
            }
            else if (column ==0 && Game1500Model.Instance.bInFreeGame())
            {
                uitop1500.GetTrans_SpecialMask().gameObject.SetActive(true);
                slotColumns[1].transform.parent.GetComponent<Canvas>().overrideSorting = true;
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.4f));
                Shake(true);
                m_Dragon_midEffect.gameObject.SetActive(true);

                CommonTools.PlayArmatureAni(m_Dragon_midEffect.transform, "Sprite", 1, () => 
                {
                    m_Dragon_midEffect.gameObject.SetActive(false); Shake(false);
                });
                //ToolUtil.PlayAnimation(m_Dragon_midEffect.transform, "Sprite", false, () =>
                //{
                //    m_Dragon_midEffect.gameObject.SetActive(false); Shake(false);
                //});
                m_Img_Rate.gameObject.SetActive(true);
                m_Img_Rate.color = new Color32(255,255,255,40);
                DOTween.To(() => 40, (value) => {
                    m_Img_Rate.color = new Color32(255,255,255,(byte)value);
                }, 255, 0.55f).OnComplete(() => {
                    m_Img_Rate.color = new Color(255,255,40);
                }).SetEase(Ease.Linear).SetLoops(2,LoopType.Yoyo).OnComplete(()=> m_Img_Rate.gameObject.SetActive(false));
            }
        }

        public void StepWinGold()
        {
            if (Game1500Model.Instance.toSpin.WinGold > 0)
            {
                showLines();
                if (Game1500Model.Instance.toSpin.WinGold > 0 || Game1500Model.Instance.lines.Count > 0)
                    uitop1500.OnSlotWinGold();       
            }
        }

        public void ContinueGame()
        {        
            float delayTimes = 0.6f;
            if (Game1500Model.Instance.bHasElement(7))
                delayTimes += 1.2f;
            if (Game1500Model.Instance.toSpin.WinGold > 0)
                delayTimes += 1.5f;
            if (autoSpinNum > 0)
                delayTimes += 0.5f;
            if (Game1500Model.Instance.bInFreeGame())
                delayTimes += 1;
            CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () => { continueSpin(); }, 100000);
        }

        float tempTimes = 1.25f;
        public IEnumerator DoGoSlotScale(int type = 1,Callback callBack = null)
        {
            uitop1500.GetTrans_Lines().localScale = new Vector3(0.92f, 0.92f, 0.92f);
            uitop1500.GetTransElementEffect().DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTransElementBgEffect().DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            TfSlot.parent.transform.DOScale(new Vector3(0.92f,0.92f,0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTrans_Mask().DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTrans_LineNum().DOScale(new Vector3(0.92f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.MoveGoldBg(true, tempTimes);
            yield return new WaitForSeconds(tempTimes);
            if (type == 2)
                ResetScale();
            callBack?.Invoke();
        }

        public void ResetScale()
        {
            TfSlot.parent.transform.DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTrans_Mask().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTransElementEffect().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTransElementBgEffect().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTrans_Lines().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.GetTrans_LineNum().DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1500.MoveGoldBg(false, tempTimes);
        }

        public void PlayTexSpecialRateAni2(Action callBack = null)
        {
            m_Txt_SpecialRate.gameObject.SetActive(true);
            CoreEntry.gAudioMgr.PlayUISound(250);
            Sequence seq = DOTween.Sequence();
            Tweener t1 = m_Txt_SpecialRate.transform.DOScale(new Vector3(1f,1, 1), 0.35f);
            Tweener t2 = m_Txt_SpecialRate.transform.DOLocalMoveY(-337f, 0.35f);
            seq.Append(t1);
            seq.Join(t2);
            seq.Join(t2);
            seq.AppendInterval(0.5f);
            CoreEntry.gTimeMgr.AddTimer(0.15f, false, () => CoreEntry.gAudioMgr.PlayUISound(287), 311);
            CoreEntry.gTimeMgr.AddTimer(0.5f, false, () => CoreEntry.gAudioMgr.PlayUISound(288), 312);
            Tweener t3 = m_Txt_SpecialRate.transform.DOScale(new Vector3(0.2f, 0.2f, 1), 0.35f);
            Tweener t4 = m_Txt_SpecialRate.transform.DOMove(uitop1500.GetTransWin().position, 0.35f);
            seq.Append(t3);
            seq.Join(t4);
            seq.AppendInterval(0.5f);
            seq.OnComplete(()=> 
            {
                callBack?.Invoke();
                m_Txt_SpecialRate.gameObject.SetActive(false);
            });
            seq.Play();
        }
 

        public void Shake(bool bShake = false)
        {
            if (bShake)
                TfSlot.parent.DOShakePosition(100f, 3, 10).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            else
                TfSlot.parent.DOKill();
        }

        public void PlayTigerAni(int type)
        {
            if(type == 1)
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "idle", true, null);
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_idle3", true, null, 2, false);
            }
            else if(type == 2)//赢钱
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win", false, null);
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_wild_collect", false,() => PlayTigerAni(1), 2, false);
            }
            else if(type == 3)///进入特俗模式金牛
            {
                ToolUtil.Play3DAnimationAndLoop(m_Trans_SpineTiger.transform,new List<string> { "rs_start", "rs_idle" });
            }
            else if(type == 4)//转个圈跳个舞
            {
                ToolUtil.Play3DAnimationAndLoop(m_Trans_SpineTiger.transform,new List<string> { "dup", "rs_idle" });
            }
            else if (type == 5)//10倍摆酷动画
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rsg_win_idle", true, null);
            }
            else if(type == 6)//10倍数字滚动摆酷动画
            {
                ToolUtil.Play3DAnimationAndLoop(m_Trans_SpineTiger.transform,new List<string> { "rsg_spawn", "rsg_idle2" });
            }
            else if (type == 7)//最终得到数字最大动画
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rs_idle2", true, null);
            }
            else if(type == 8)///返回到idle动画
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "rs_exit", false, () => PlayTigerAni(1)); 
            }
            else if (type == 9)///手机百变
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_wild_collect2", true, null); 
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "wild_collect", true, ()=> { PlayTigerAni(1); }, 2, false);
            }
            else if (type == 10)//赢钱
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win2", false, null);
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_wild_collect", false, () => PlayTigerAni(1), 2, false);
            }
            else if (type == 10)//赢钱
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win2", false, null);
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "fx_wild_collect", false, () => PlayTigerAni(1), 2, false);
            }
        }

        public void SetRollBg(int type = 0)
        {
            m_Trans_normalBg.gameObject.SetActive(type == 0);
            m_Trans_specialBg.gameObject.SetActive(type == 1);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(247);
           // ChangeScreenCh(true);
        }

    }

}


  