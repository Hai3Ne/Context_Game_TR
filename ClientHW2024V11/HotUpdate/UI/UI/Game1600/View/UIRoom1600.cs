using DG.Tweening;
using SEZSJ;
using Spine.Unity;
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
    public partial class UIRoom1600 : UIRoom_SlotCpt
    {
        public Top1600Panel uitop1600;
        public List<Transform> elementList = new List<Transform>();
        public List<Transform> elementBgList = new List<Transform>();
        public List<Transform> linesList0 = new List<Transform>();
        public List<Transform> linesList1 = new List<Transform>();
        public List<Transform> linesList2 = new List<Transform>();
        int index;
        protected override void Awake()
        {
            TfSlot = transform.Find("GoSlot/TfSlot");
            for (int i = 0; i < TfSlot.childCount; i++)
            {
                Transform child = TfSlot.GetChild(i).GetChild(0);
                if (child.name.Contains("Column"))
                    lstcolumns.Add(child);
            }
            Transform tfColumn = TfSlot.GetChild(0);
            slotRow = tfColumn.childCount - 1;
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;
            slotColumns = new List<UISlotColumn>();
            effectAcc = transform.Find("GoSlot/EffectAcc").gameObject;
            GetBindComponents(gameObject);
            columnCount = 3;


            for (int i = 0; i < m_Trans_ElementEffect0.childCount; i++)
                elementList.Add(m_Trans_ElementEffect0.GetChild(i));
            for (int i = 0; i < m_Trans_ElementEffect1.childCount; i++)
                elementList.Add(m_Trans_ElementEffect1.GetChild(i));
            for (int i = 0; i < m_Trans_ElementEffect2.childCount; i++)
                elementList.Add(m_Trans_ElementEffect2.GetChild(i));

            for (int i = 0; i < m_Trans_ElementEffectBg0.childCount; i++)
                elementBgList.Add(m_Trans_ElementEffectBg0.GetChild(i));
            for (int i = 0; i < m_Trans_ElementEffectBg1.childCount; i++)
                elementBgList.Add(m_Trans_ElementEffectBg1.GetChild(i));
            for (int i = 0; i < m_Trans_ElementEffectBg2.childCount; i++)
                elementBgList.Add(m_Trans_ElementEffectBg2.GetChild(i));
            for (int i = 0; i < m_Trans_Lines0.childCount; i++)
                linesList0.Add(m_Trans_Lines0.GetChild(i));
            for (int i = 0; i < m_Trans_Lines1.childCount; i++)
                linesList1.Add(m_Trans_Lines1.GetChild(i));
            for (int i = 0; i < m_Trans_Lines2.childCount; i++)
                linesList2.Add(m_Trans_Lines2.GetChild(i));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1600Model.Instance.InitConfig();

            if (uitop1600 != null)
            {
                autoSpinNum = 0;
                InitData();
            }
            else
            {
                GameObject go = CommonTools.AddSubChild(transform.gameObject, "UI/Prefabs/Game1600/FirstRes/Top1600Panel");
                uitop1600 = go.GetComponent<Top1600Panel>();
                uitop1600.Init(this);
                uitop1600.gameObject.SetActive(true);

            }
            CoreEntry.gAudioMgr.PlayUIMusic(265);
            uitop1600.InitData();
            gameStatus = 0;
            num = 0;
            m_Trans_Mask0.gameObject.SetActive(false);
            m_Trans_Mask1.gameObject.SetActive(false);
            m_Trans_Mask2.gameObject.SetActive(false);
            bDanJi = param == null ? false : (bool)param;
            PlayTigerAni(1);
            uitop1600.GetTrans_Free2().gameObject.SetActive(false);
        }

        protected override void Start()
        {
            init();
        }

        public void Reconnect()
        {
            if (uitop1600 != null)
            {
                InitData();
                uitop1600.GetTextGold().text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(Game1600Model.Instance.toSpin.n64Gold);
                uitop1600.autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            uitop1600.OnSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            for (int i = 0; i < SlotData_1600.column; i++)
            {
                UIRomm1600SlotColumn column = lstcolumns[i].GetComponent<UIRomm1600SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for(int j = 0;j < (i!=1?4:5);j++)
                {
                    UIRoom1600SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1600SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override void preSpin()
        {
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            //if(Game1600Model.Instance.gameStates != 1)
            uitop1600.ShowAllLines(false);
            uitop1600.ShowAllLineText(false);
            uitop1600.gameTips?.ClosePanel();
            m_Trans_Mask0.gameObject.SetActive(false);
            m_Trans_Mask1.gameObject.SetActive(false);
            m_Trans_Mask2.gameObject.SetActive(false);
            uitop1600.SetTxtScore(0);

            if(bDanJi)
            {
                setState(slotState.SpinBegin);
                awaiting = true;    
                Game1600Model.Instance.gameStates = 0;
                RandomSpinData();
                SetSpinData();
            }
        }

        public override void sendSpin()
        {
            if (bDanJi == false)
            {
                setState(slotState.SpinBegin);
                gameStatus = 1;
                float num = (uitop1600.Bet);
                Game1600Ctrl.Instance.Send_CS_GAME14_BET_REQ((int)num);                
            }
        }

        public override void RandomSpinData()
        {
            Game1600Model.Instance.RandomSpinData();
        }

        public async override void SetSpinData()
        {
            //Game1600Model.Instance.gameStates = 2;
            //Game1600Model.Instance.nFreeGame = 5;
            //Game1600Model.Instance.bInFreeStates = false;

            

            if (Game1600Model.Instance.nFreeGame == 5 && !Game1600Model.Instance.bInFreeStates)
            {
                uitop1600.GetTrans_Free2().gameObject.SetActive(true);
                CoreEntry.gAudioMgr.PlayUIMusic(276);
                CoreEntry.gAudioMgr.PlayUISound(269);
                CoreEntry.gAudioMgr.PlayUISound(275, gameObject);
                PlayTigerAni(5);
                StartCoroutine(DoGoSlotScale(1));
                uitop1600.bPlayRabbitAni(true);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2.2f));
                SetFreeBg(true);
                PlayTigerAni(3);
                CoreEntry.gAudioMgr.PlayUISound(284);
      
                
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.7f));
                uitop1600.SetFreeTimes(true, 5, false);
                m_Dragon_1.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_1.transform,"Sprite",1,()=> m_Dragon_1.gameObject.SetActive(false));
                m_Dragon_2.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_2.transform, "Sprite", 1, () => m_Dragon_2.gameObject.SetActive(false));
                m_Dragon_3.gameObject.SetActive(true);
                CommonTools.PlayArmatureAni(m_Dragon_3.transform, "Sprite", 1, () => m_Dragon_3.gameObject.SetActive(false));
                m_Trans_SpineTiger.GetComponent<MeshRenderer>().sortingOrder = 1;
                m_Trans_SpineTiger.gameObject.SetActive(false);
                uitop1600.GetTrans_Free2().gameObject.SetActive(false);
                uitop1600.ShowFreeAni(true);
                uitop1600.DoFreeAni(()=> 
                {
                    DoFreeAniCallBack();
           
                },true,()=> 
                {
   
                    m_Dragon_RollFreeEffect.gameObject.SetActive(true);
                    m_Dragon_RollFreeEffect.transform.GetComponent<Canvas>().overrideSorting = true;
                    CommonTools.PlayArmatureAni(m_Dragon_RollFreeEffect.transform, "newAnimation", 1);
                    m_Dragon_RollFreeEffect2.gameObject.SetActive(false);
           
                });     
            }
            else
            {
                if (Game1600Model.Instance.nFreeGame > 0)
                {
                    m_Dragon_RollFreeEffect.gameObject.SetActive(false);
                    if (!m_Dragon_RollFreeEffect2.gameObject.activeSelf)
                    {
                        m_Dragon_RollFreeEffect2.gameObject.SetActive(true);
                        CommonTools.PlayArmatureAni(m_Dragon_RollFreeEffect2.transform, "Sprite", 0);
                    }
                    SetFreeData();
                }        
                if (Game1600Model.Instance.nFreeGame > 0 || Game1600Model.Instance.lastFreeGameIndex == 1)
                    uitop1600.SetFreeTimes(true, Game1600Model.Instance.nFreeGame, true);
                if (Game1600Model.Instance.gameStates == 2)
                {
                    PlayTigerAni(5);
                    StartCoroutine(DoGoSlotScale(2));
                    CoreEntry.gAudioMgr.PlayUISound(269);
                    CoreEntry.gAudioMgr.PlayUISound(275,gameObject);
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.3f));
                    uitop1600.GetTrans_Free2().gameObject.SetActive(true);
                    uitop1600.bPlayRabbitAni(true);
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.8f));
                    SetFreeBg(true);
                    uitop1600.DoFreeAni(null,false);
                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2.2f));
                    uitop1600.bPlayRabbitAni(false);
                    uitop1600.GetTrans_Free2().gameObject.SetActive(false);
                    SetFreeBg(false);
                    PlayTigerAni(1);
                }
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(isFastSpin ? 0 : 0.38f));
                recSpin();
            }
        }

        private void FinishedAni()
        {
            m_Dragon_RollFreeEffect.gameObject.SetActive(false);
           // m_Dragon_RollFreeEffect.transform.GetComponent<Canvas>().overrideSorting = false;
            m_Dragon_RollFreeEffect2.gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(m_Dragon_RollFreeEffect2.transform,"Sprite",0);
        }

        private async void DoFreeAniCallBack()
        {
            m_Dragon_RollFreeEffect.transform.GetComponent<Canvas>().overrideSorting = false;
            uitop1600.SetFreeTimes(true, 5, true);
            uitop1600.ShowFreeAni(false);
            m_Img_Para.gameObject.SetActive(true);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.35f));
    
            for (int i = 0; i < SlotData_1600.column; i++)
                slotColumns[i].duration = SlotData_1600.specialRollTime;
            SpecialGameAni(recSpin);
        }

        Sequence seq;
        float doScaleTimes = 0.1f;
        float move_Xtimes = 0.3f;
        private async void SpecialGameAni(Callback callBack)
        {           
            for(int i = 0;i < 5;i++)
            {
                seq?.Kill();
                seq = null;
                seq = DOTween.Sequence();
                GameObject goldElement = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/SpecialElementEffectGold");
                goldElement.transform.SetParent(m_Trans_SpawnGoldPos, true);
                goldElement.transform.localScale = Vector3.one;
                goldElement.transform.localPosition = Vector3.zero;
                goldElement.transform.DOScale(Vector3.one * 1.3f, doScaleTimes).SetEase(Ease.Linear);
                int num = UnityEngine.Random.Range(0, 5);
                long goldValue = Game1600Model.Instance.goldValue[num];

                if (goldValue < 4)
                {
                    goldElement.transform.GetChild(0).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue* uitop1600.Bet * 10);
                    goldElement.transform.GetChild(0).gameObject.SetActive(true);
                    goldElement.transform.GetChild(1).gameObject.SetActive(false);
                    goldElement.transform.GetChild(2).gameObject.SetActive(false);
                }
                else if (goldValue < 20)
                {
                    goldElement.transform.GetChild(1).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue * uitop1600.Bet * 10);
                    goldElement.transform.GetChild(0).gameObject.SetActive(false);
                    goldElement.transform.GetChild(1).gameObject.SetActive(true);
                    goldElement.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    goldElement.transform.GetChild(2).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue * uitop1600.Bet * 10);
                    goldElement.transform.GetChild(0).gameObject.SetActive(false);
                    goldElement.transform.GetChild(1).gameObject.SetActive(false);
                    goldElement.transform.GetChild(2).gameObject.SetActive(true);
                }
               

                GameObject goldElement2 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/SpecialElementEffectGold");
                goldElement2.transform.SetParent(m_Trans_SpawnGoldPos, true);
                goldElement2.transform.localScale = Vector3.one;
                goldElement2.transform.localPosition = Vector3.zero;
                goldElement2.transform.DOScale(Vector3.one * 1.3f, doScaleTimes).SetEase(Ease.Linear);
                int num2 = UnityEngine.Random.Range(0, 5);
                long goldValue2 = Game1600Model.Instance.goldValue[num];

                if (goldValue2 < 4)
                {
                    goldElement2.transform.GetChild(0).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue * uitop1600.Bet * 10);
                    goldElement2.transform.GetChild(0).gameObject.SetActive(true);
                    goldElement2.transform.GetChild(1).gameObject.SetActive(false);
                    goldElement2.transform.GetChild(2).gameObject.SetActive(false);
                }
                else if (goldValue2 < 20)
                {
                    goldElement2.transform.GetChild(1).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue * uitop1600.Bet * 10);
                    goldElement2.transform.GetChild(0).gameObject.SetActive(false);
                    goldElement2.transform.GetChild(1).gameObject.SetActive(true);
                    goldElement2.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    goldElement2.transform.GetChild(2).GetComponent<Text>().text = ToolUtil.ShowF2Num2(goldValue * uitop1600.Bet * 10);
                    goldElement2.transform.GetChild(0).gameObject.SetActive(false);
                    goldElement2.transform.GetChild(1).gameObject.SetActive(false);
                    goldElement2.transform.GetChild(2).gameObject.SetActive(true);
                }
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(doScaleTimes + 0.05));
                int rand = UnityEngine.Random.Range(0, 3);
                PlayNextAni(goldElement,rand);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02));
                PlayNextAni(goldElement2,(rand+1)%3);
                if(i == 4)
                    FinishedAni();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.3f));
            }

      

            Game1600Model.Instance.bInFreeStates = true;
            callBack?.Invoke();
        }

        private async void PlayNextAni(GameObject goldElement,int rand)
        {
            if (rand == 0)
                goldElement.transform.DOMove(m_Trans_Left.position, move_Xtimes).SetEase(Ease.Linear).OnComplete(() => { goldElement.transform.SetParent(slotColumns[0].transform.parent, true); });
            else if (rand == 2)
                goldElement.transform.DOMove(m_Trans_Right.position, move_Xtimes).SetEase(Ease.Linear).OnComplete(() => { goldElement.transform.SetParent(slotColumns[2].transform.parent, true); });
            else
                goldElement.transform.SetParent(slotColumns[1].transform.parent, true);
            if (rand != 1)
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(move_Xtimes));
            goldElement.transform.DOLocalMoveY(-500, 0.7f).SetEase(Ease.Linear).OnComplete(() => { CoreEntry.gGameObjPoolMgr.Destroy(goldElement); });
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < SlotData_1600.column; i++)
            {
                int times = SlotData_1600.rollTimes + i * 4;// -- 慢速
                if (isFastSpin)// --快速
                    times = 6;
                if (Game1600Model.Instance.gameStates == 2)
                    times = 15 + i * 4;// -- 慢速
                if (Game1600Model.Instance.nFreeGame > 0)
                    times += 7;
                slotColumns[i].endRoll(times);
            }
        }

        public async override void finishSpin()
        {
            if (Game1600Model.Instance.toSpin == null)
                return;
            base.finishSpin();   
            setState(slotState.SpinEnd); 
            if(Game1600Model.Instance.bHasElement(8) && !uitop1600.GetTrans_Free().gameObject.activeSelf )
            {
                PlayTigerAni(4);
                CoreEntry.gAudioMgr.PlayUISound(270);
                CoreEntry.gTimeMgr.AddTimer(0.4f,false,()=> { CoreEntry.gAudioMgr.PlayUISound(271); },30);             
            }

            for (int i = 0; i < SlotData_1600.column; i++)
                slotColumns[i].onSpinFinish(Game1600Model.Instance.toSpin.WinGold > 0);
            StepWinGold();
            int goldElementCount = Game1600Model.Instance.GetElement(8);
            if (Game1600Model.Instance.toSpin.WinGold > 0 || Game1600Model.Instance.n64NumberPowerGold > 0 )
            {
               
                    uitop1600.OnSlotWinGold();
                    PlayTigerAni(2);
            }
          
            if (goldElementCount >= 5)
            {
                if(!uitop1600.GetTrans_Win().gameObject.activeSelf)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(312);
                    uitop1600.PlayGameTips(3);
                    uitop1600.SetWinGoldBg((int)Game1600Model.Instance.toSpin.rate);
                }
                CoreEntry.gAudioMgr.PlayUISound(290);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
                CollectGold();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(goldElementCount *0.4f));
            }
            if (Game1600Model.Instance.lastFreeGameIndex == 1)
            {
                Game1600Model.Instance.lastFreeGameIndex = 0;
                uitop1600.BigWinAni(()=> 
                {
                    PlayTigerAni(1);
                    m_Dragon_RollFreeEffect.gameObject.SetActive(false);
                    m_Dragon_RollFreeEffect2.gameObject.SetActive(false);
                    Game1600Model.Instance.bInFreeStates = false;
                    isFreeSpin = false;
                });
            }
            else
                ContinueGame();
        }

        public override void showLines()
        {
            SetElementEffect();
            uitop1600.ShowAllLineText(false);
            ContinueShowLines();
        }

        int nBet1 = 0;
        private async void ContinueShowLines()
        {
            nBet1 = Game1600Model.Instance.nBet1;
            if (Game1600Model.Instance.bHasElement(8))
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.5f));
            if (slotColumns[0].isRolling)
                return;
            float times = 0.3f;
            float tempTimes = 1;
            for (int i = 0; i < Game1600Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1600Model.Instance.lines[i];

                List<int> elementPos = Game1600Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    UISlotCell cell = slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1];
                    cell.showLine();
                    if (elementList[j * 3 + elementPos[j] - 1].childCount > 0)
                        tempTimes = ToolUtil.GetAnimationDuration(elementList[j * 3 + elementPos[j] - 1].GetChild(0), "win");

                    if (tempTimes > times)
                        times = tempTimes;
                }
                if (slotColumns[0].isRolling)
                    return;
                uitop1600.ShowOneLine(tempLine.Key, true);
            }
            ShowAllCell(true, true);
            m_Trans_Mask0.gameObject.SetActive(true);
            m_Trans_Mask1.gameObject.SetActive(true);
            m_Trans_Mask2.gameObject.SetActive(true);
            index = 0;
            CoreEntry.gTimeMgr.AddTimer(1.7f, true, () =>
            {
                ShowAllCell(false);
                if(Game1600Model.Instance.lines.Count != 1)
                {
                    uitop1600.ShowAllLines(false);
                    uitop1600.ShowAllLineText(false);
                }            
                if (index == Game1600Model.Instance.lines.Count)
                {
                    index = 0;
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
                    {
                            CoreEntry.gAudioMgr.PlayUISound(277, transform.GetChild(1).gameObject);
                        for (int i = 0; i < Game1600Model.Instance.lines.Count; i++)
                        {
                            if (slotColumns[0].isRolling)
                                return;
                            KeyValuePair<int, int> tempLine = Game1600Model.Instance.lines[i];
                            uitop1600.ShowOneLine(tempLine.Key, true);
                        }
                        ShowAllCell(true, false);
                    }, 400);
                }
                else
                {
                    CoreEntry.gAudioMgr.PlayUISound(277, transform.GetChild(1).gameObject);
                    CoreEntry.gTimeMgr.AddTimer(0.1f, false, () => { ShowOneLine(); }, 400);
                }

            }, 399);
        }

        private async void ShowOneLine()
        {
      
            KeyValuePair<int, int> tempLine = Game1600Model.Instance.lines[index];
            List<int> elementPos = Game1600Model.Instance.lineData[tempLine.Key];
            List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
            temp.Add(tempLine);

            int element0 = slotColumns[0].lstCells[2 - elementPos[0] + 1].element;
            int element1 = slotColumns[1].lstCells[3 - elementPos[1] + 1].element;
            int element2 = slotColumns[2].lstCells[2 - elementPos[2] + 1].element;
            if (element0 == element1 && element0 == element2)
                uitop1600.SetLineText(tempLine.Key, Game1600Model.Instance.elementRate3[element0 - 1] * nBet1);
            else
            {
                int element = 0;
                if (element0 != SlotData_1600.elementWild)
                    element = element0;
                if (element1 != SlotData_1600.elementWild)
                    element = element1;
                if (element2 != SlotData_1600.elementWild)
                    element = element2;
                uitop1600.SetLineText(tempLine.Key, Game1600Model.Instance.elementRate3[element - 1] * nBet1);
            }
            for (int j = 0; j < tempLine.Value; j++)
            {
                slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1].ShowCellEffect(true);
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));
                if (slotColumns[0].isRolling)
                    return;
                if (j == 1)
                    uitop1600.ShowOneLine(tempLine.Key, true);
           
            }
            index++;
        }

        public override void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game1600Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1600Model.Instance.lines[i];
                List<int> elementPos = Game1600Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                    slotColumns[j].lstCells[(j != 1 ? 2 : 3) - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
            }
        }

        private void SetElementEffect()
        {
            for(int i = 0;i < elementList.Count;i++)
            {
                if (elementList[i].childCount > 0)
                    elementList[i].GetChild(0).gameObject.SetActive(false);
            }
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0;j < slotColumns[i].lstCells.Count -1;j++)
                    slotColumns[i].lstCells[j].ImgElement.gameObject.SetActive(true);
            }
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1600Model.Instance.slotResult.Count) { }
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
            return Game1600Model.Instance.slotResult[idx];
        }
      
        public GameObject GetSoundObj1()
        {
            return m_Trans_Sound1.gameObject;
        }

        public Transform GetMoveTarget()
        {
            return m_Trans_MoveTarget;
        }

        public override async void beginSpin(int num0 = 0, bool fast = false)
        {
            num = num0;
            isFastSpin = false;
            if (Game1600Model.Instance.nFreeGame > 0)
            {
                isFreeSpin = true;
            }
            else
            {
                if(Game1600Model.Instance.gameStates != 3 && Game1600Model.Instance.gameStates != 1)
                {
                    if (uitop1600.Bet == uitop1600.betMin && Game1600Model.Instance.toSpin.n64Gold < (uitop1600.Bet*10))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        uitop1600.OnSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        uitop1600.ShowNotEnoughMoneyTips();
                        return;
                    }
                    else
                    {
                        bool bCanSlot = false;
                        if (Game1600Model.Instance.toSpin.n64Gold < (long)(uitop1600.Bet*10))
                        {
                            int tempBet1 = uitop1600.Bet;
                            for (int i = uitop1600.BetList.Count - 1; i >= 0; i--)
                            {
                                int tempBet = uitop1600.BetList[i];
                                if (tempBet < tempBet1)
                                {
                                    uitop1600.OnClickBtnMin();
                                    if (Game1600Model.Instance.toSpin.n64Gold >= (long)(tempBet*10))
                                    {
                                        bCanSlot = true;
                                        break;
                                    }
                                }
                            }
                            if (!bCanSlot)
                            {
                                autoSpinNum = 0;// -- 停止自动
                                uitop1600.OnSlotSpinNum(autoSpinNum);
                                setState(slotState.Idle);
                                uitop1600.ShowNotEnoughMoneyTips();
                                return;
                            }
                        }
                    }
                }               

                isFastSpin = uitop1600.GetTogTurboIsOn();
                if(Game1600Model.Instance.gameStates == 1 || (Game1600Model.Instance.gameStates == 3 && Game1600Model.Instance.toSpin.WinGold <= 0))
                {}
                else if (num > 0)
                    autoSpinNum = num - 1;
                else
                    autoSpinNum = num;
                uitop1600.OnSlotSpinNum(autoSpinNum);
            }
            setState(slotState.SpinBegin);
            preSpin();
            for (int i = 0; i < SlotData_1600.column; i++)
            {
                slotColumns[i].reset();
                if(isFastSpin)
                    slotColumns[i].duration = SlotData_1600.fastRollTimes;
                int index = i;
                if(!uitop1600.GetTogTurboIsOn())
                {
                    slotColumns[index].beginRoll(-1);
                    if(i!= 2)
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.11f));
                }
                else
                    slotColumns[i].beginRoll(-1);
            }
            if (Game1600Model.Instance.gameStates != 3 && Game1600Model.Instance.gameStates != 1)
            {
                int betValue = uitop1600.Bet *10;
                uitop1600.GetTextGold().text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(Game1600Model.Instance.toSpin.n64Gold - betValue);
                uitop1600.autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold - betValue);
            }
    
       
            if(uitop1600.GetTrans_Win().gameObject.activeSelf && !uitop1600.GetTrans_Free().gameObject.activeSelf)
            {
                uitop1600.ContiunePlayGameTipsLoop();
                SetFreeBg(false);
            }
         
            sendSpin();
            DoRollLightAni();
            uitop1600.SetRollBtnRorate(true, Game1600Model.Instance.gameStates == 1);
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
            uitop1600.GetTextGold().text = ToolUtil.GetCurrencySymbol()+":" + ToolUtil.ShowF2Num(Game1600Model.Instance.toSpin.n64Gold);// ((float)(Game1600Model.Instance.toSpin.n64Gold/10000f)).ToString("f2");// string.format("%.2f", )
            uitop1600.autoPanel.SetGoldText(Game1600Model.Instance.toSpin.n64Gold);
            setState(slotState.Idle);
            if (Game1600Model.Instance.nFreeGame > 0)
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
                    if (Game1600Model.Instance.gameStates != 1)
                        uitop1600.SetRollBtnRorate(false);
                }, 91);
                CoreEntry.gTimeMgr.AddTimer(0.6f, false, () =>{finishSpin();}, 9);
            }
        }

        public void StepWinGold()
        {
            if (Game1600Model.Instance.toSpin.WinGold > 0 && Game1600Model.Instance.n64FreeGold <= 0)
                showLines();
        }

        public void ContinueGame()
        {
            if (Game1600Model.Instance.toSpin.rate >= 5)
                return;
                float delayTimes = 0.6f;
            if (Game1600Model.Instance.toSpin.WinGold > 0 && Game1600Model.Instance.toSpin.rate <5)
                delayTimes = 1f;
            if(Game1600Model.Instance.toSpin.rate >5)
                delayTimes = 2.6f;
            if (autoSpinNum > 0 && Game1600Model.Instance.toSpin.WinGold > 0)
                delayTimes += 1.2f;
            CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () => { continueSpin(); }, 100000);
        }

        float tempTimes = 1.5f;
        public IEnumerator DoGoSlotScale(int type = 1)
        {
            TfSlot.parent.transform.DOScale(new Vector3(0.95f,0.92f,0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1600.transform.DOScale(new Vector3(0.95f, 0.92f, 0.92f), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            if (type == 1 || type == 2)
            {
                m_Trans_LeftMove.DOAnchorPos(new Vector2(68,0), tempTimes).SetEase(Ease.Linear).SetAutoKill();
                m_Trans_RightMove.DOAnchorPos(new Vector2(-50, 0), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            } 
            yield return new WaitForSeconds(tempTimes);
            if (type == 2)
                ResetScale();
        }

        public async void ResetScale()
        {
        
            TfSlot.parent.transform.DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            uitop1600.transform.DOScale(Vector3.one, tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_LeftMove.DOAnchorPos(new Vector2(-85, 0), tempTimes).SetEase(Ease.Linear).SetAutoKill();
            m_Trans_RightMove.DOAnchorPos(new Vector2(85, 0), tempTimes).SetEase(Ease.Linear).SetAutoKill();

        }
        public void ReturnNormal()
        {
            m_Trans_SpineTiger.GetComponent<MeshRenderer>().sortingOrder = 1;
            m_Trans_SpineTiger.gameObject.SetActive(true);
            m_Img_Para.gameObject.SetActive(false);
        }

        public void PlayTigerAni(int type)
        {
            if (Game1600Model.Instance.nFreeGame > 0 && Game1600Model.Instance.nFreeGame < 5)
                return;
            if (type == 1)
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "idle", true, null);
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "vfx_lvl3", true, null, 2, false);
            }
            else if (type == 2)//赢钱
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "win", false, () => PlayTigerAni(1));
               
            else if(type == 3)//免费旋转
            {
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "freespin", false, ()=> 
                {
                    m_Trans_SpineTiger.GetComponent<MeshRenderer>().sortingOrder = 6;
                });
            }
            else if(type == 4)//卷宗发光
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "vfx_collect_fx1", false, () => PlayTigerAni(1));
            else if(type == 5)///出现特俗游戏 兔子蹦蹦跳跳
                ToolUtil.Play3DAnimationAndLoop(m_Trans_SpineTiger.transform, new List<string> { "tease_start", "tease_idle" });
            else if(type == 6)//结束回到idle状态
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "tease_exit", false,()=> PlayTigerAni(1));
            else if(type == 7)//兔子出现
                ToolUtil.Play3DAnimation(m_Trans_SpineTiger.transform, "respawn", false, () => PlayTigerAni(1));
        }

        public void SetFreeBg(bool bFree = false)
        {
            m_Trans_normalBg.gameObject.SetActive(!bFree);
            m_Trans_FreeBg.gameObject.SetActive(bFree);
        }

        public async void CollectGold()
        {
            long temp = Game1600Model.Instance.tempWinGold;
            for (int i = 0; i <3;i++)
            {
                for(int j = slotColumns[i].lstCells.Count - 1;j>=0;j--)
                {            
                    if(slotColumns[i].lstCells[j].element == 8 && slotColumns[i].lstCells[j].row >=0)
                    {
                        UIRoom1600SlotCell cell = slotColumns[i].lstCells[j] as UIRoom1600SlotCell;
                        temp += cell.goldValue;
                        CoreEntry.gAudioMgr.PlayUISound(289,transform.GetChild(2).gameObject);
                        cell.FlyGoldValue(()=> 
                        {
                            uitop1600.GetmDragon_AddGoldEffect().gameObject.SetActive(transform);
                            CommonTools.PlayArmatureAni(uitop1600.GetmDragon_AddGoldEffect(),"Sprite",1,()=> uitop1600.GetmDragon_AddGoldEffect().gameObject.SetActive(false));
                            uitop1600.SetWinGoldValue(temp);
                        });
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.3f));
                    }
                }
            }           
        }



        private async void DoRollLightAni()
        {
            m_Img_RollBg0.color = new Color32(255,255,255,0);
            m_Img_RollBg0.gameObject.SetActive(true);
            m_Img_RollBg0.DOColor(new Color32(255, 255, 255, 255), 0.3f).SetDelay(0.2f).SetEase(Ease.Linear); 
            m_Img_RollBg1.color = new Color32(255, 255, 255, 0);
            m_Img_RollBg1.gameObject.SetActive(true);
            m_Img_RollBg1.DOColor(new Color32(255, 255, 255,255), 0.3f).SetEase(Ease.Linear);
            m_Img_RollBg2.color = new Color32(255, 255, 255, 0);
            m_Img_RollBg2.gameObject.SetActive(true);
            m_Img_RollBg2.DOColor(new Color32(255, 255, 255, 255), 0.3f).SetDelay(0.2f).SetEase(Ease.Linear);


            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.45f));
        
            m_Img_RollBg1.gameObject.SetActive(false);
        
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.2f));
            m_Img_RollBg0.gameObject.SetActive(false);
            m_Img_RollBg2.gameObject.SetActive(false);
            uitop1600.GetImg_LeftLight().color = new Color32(255, 255, 255, 0);
            uitop1600.GetImg_LeftLight().gameObject.SetActive(true);
            uitop1600.GetImg_LeftLight().DOColor(new Color32(255, 255, 255, 255), 0.3f).SetEase(Ease.Linear);
            uitop1600.GetImg_RightLight().color = new Color32(255, 255, 255, 0);
            uitop1600.GetImg_RightLight().gameObject.SetActive(true);
            uitop1600.GetImg_RightLight().DOColor(new Color32(255, 255, 255, 100), 0.3f).SetEase(Ease.Linear);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.3f));
            uitop1600.GetImg_LeftLight().gameObject.SetActive(false);
            uitop1600.GetImg_RightLight().gameObject.SetActive(false);
        }

        public void SetFreeData()
        {
            m_Trans_SpineTiger.GetComponent<MeshRenderer>().sortingOrder = 1;
            m_Trans_SpineTiger.gameObject.SetActive(false);
            m_Img_Para.gameObject.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(265);
            CoreEntry.gAudioMgr.StopMusic(276);
        }
    }
}


  