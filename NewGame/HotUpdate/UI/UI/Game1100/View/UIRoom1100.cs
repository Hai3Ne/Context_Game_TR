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
    public partial class UIRoom1100 : UIRoom_SlotCpt
    {
        public Top1100Panel uitop1100;
        public CommonTop commonTop;
        public UIRoom1100SmallGame smallGame;
        int soundBgID = 0;
        protected override void Awake()
        {
            TfSlot = transform.GetChild(2).GetChild(1);
            for (int i = 0; i < TfSlot.childCount; i++)
            {
                Transform child = TfSlot.GetChild(i);
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
            columnCount = 5;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Game1100Model.Instance.InitConfig();
            PlayBgSound(Game1100Model.Instance.nSpecialCount);
            if (uitop1100 != null)
            {
                autoSpinNum = 0;
                InitData();
                uitop1100.InitData();
                if (Game1100Model.Instance.nSpecialCount > 0)
                    SetSpecialElement();
            }
            else
            {
                GameObject common = CommonTools.AddSubChild(gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
                commonTop = common.GetComponent<CommonTop>();
                commonTop.SetDanJi(bDanJi, true, true);
                commonTop.SlotCpt = this;

                GameObject go = CommonTools.AddSubChild(gameObject, "UI/Prefabs/Game1100/FirstRes/Top1100Panel");
                uitop1100 = go.GetComponent<Top1100Panel>();
                uitop1100.gameObject.SetActive(true);
                uitop1100.Init(this);
                uitop1100.InitData();
                commonTop.transform.SetAsLastSibling();
            }
            gameStatus = 0;
            num = 0;
        }

        public void Reconnect()
        {
            if (uitop1100 != null)
            {
                commonTop.SetRollBtnRorate(false);
                InitData();
                commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold);
                continueSpin();
            }
        }

        public override void init()
        {
            autoSpinNum = 0;
            commonTop.SetSlotSpinNum(autoSpinNum);
            setState(slotState.Idle);
            slotColumns.Clear();
            for (int i = 0; i < SlotData_1100.column; i++)
            {
                UIRomm1100SlotColumn column = lstcolumns[i].GetComponent<UIRomm1100SlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for (int j = 0; j < 5; j++)
                {
                    UIRoom1100SlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom1100SlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override void preSpin()
        {
            if(Game1100Model.Instance.nSpecialCount >= 11)
                ShakeSlot(true);
            uitop1100.SetTxtScore(0);
    
            uitop1100.ShowMask(false);
            CoreEntry.gTimeMgr.RemoveTimer(22);
            CoreEntry.gTimeMgr.RemoveTimer(21);
            CoreEntry.gTimeMgr.RemoveTimer(32);
            CoreEntry.gTimeMgr.RemoveTimer(399);
            CoreEntry.gTimeMgr.RemoveTimer(400);
            CoreEntry.gTimeMgr.RemoveTimer(38);
            CoreEntry.gTimeMgr.RemoveTimer(1000);
            CoreEntry.gTimeMgr.RemoveTimer(100000);
            uitop1100.ShowAllLines();
            if (!Game1100Model.Instance.bHasNewSpeiclaElement)
                uitop1100.DestroyAni();
            uitop1100.SetAllCashAni(2);
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
                Game1100Ctrl.Instance.Send_CS_GAME7_BET_REQ((int)num);
            }
        }

        public override void RandomSpinData()
        {
            Game1100Model.Instance.RandomSpinData();
        }

        public override void SetSpinData()
        {
            recSpin();
        }

        public override void handlerSpin()
        {
            for (int i = 0; i < 5; i++)
            {
                int times = SlotData_1100.rollTimes + i * 2;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishSpin()
        {
            base.finishSpin();
            if (Game1100Model.Instance.toSpin == null)
                return;
            if (Game1100Model.Instance.bHasNewSpeiclaElement)
            {
                int count = Game1100Model.Instance.HasElementNum(SlotData_1100.specialelement);
                if (count > 2)
                    PlayBgSound(count);
            }
            else
                Game1100Model.Instance.nBet = 0;

            //if (Game1100Model.Instance.nSpecialCount >= 3)
                FinishShake();

            setState(slotState.SpinEnd);
            for (int i = 0; i < SlotData_1100.column; i++)
                slotColumns[i].onSpinFinish(Game1100Model.Instance.toSpin.WinGold > 0);
            //--设置亮的灯光
            CoreEntry.gTimeMgr.RemoveTimer(5);
            CoreEntry.gTimeMgr.RemoveTimer(4);
            CoreEntry.gTimeMgr.RemoveTimer(555);
            StepWinGold();
            if(!Game1100Model.Instance.bHasNewSpeiclaElement&& bActiveDragon_Cash())
                uitop1100.SetCashFinished();
            else
                ContinueGame();
            if (Game1100Model.Instance.bHasNewSpeiclaElement && Game1100Model.Instance.lines.Count <= 0)
            {
                uitop1100.SetAniParent();
                uitop1100.SetAllCashAni(2);
            }           
            if (bDanJi)
                SetCashAni(Game1100Model.Instance.bHasNewSpeiclaElement?Game1100Model.Instance.gearData[Game1100Model.Instance.betID].Count:0);
            else
                SetCashAni(Game1100Model.Instance.bHasNewSpeiclaElement ? Game1100Model.Instance.nSpecialCount : 0);
        }

        public override void showLines()
        {
            ShowAllCell(false);
            ShowLineOneByOne();
            float times = 0;
            for (int i = 0; i < Game1100Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1100Model.Instance.lines[i];
                List<int> elementPos = Game1100Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    float tempTimes = 1;
                    if (slotColumns[j].lstCells[3 - elementPos[j] + 1].TfSpine.childCount > 0)
                    {
                        string aniName = "a2";
                        if (slotColumns[j].lstCells[3 - elementPos[j] + 1].element == 10)
                            aniName = "a1";
                        tempTimes = ToolUtil.GetAnimationDuration(slotColumns[j].lstCells[3 - elementPos[j] + 1].TfSpine.GetChild(0), aniName);
                    }
                    if (tempTimes > times)
                        times = tempTimes;
                }
            }
            CoreEntry.gTimeMgr.AddTimer(Game1100Model.Instance.lines.Count*0.3f+0.2f, false,()=> 
            {
                for (int i = 0; i < Game1100Model.Instance.lines.Count; i++)
                {
                    KeyValuePair<int, int> tempLine = Game1100Model.Instance.lines[i];
                    List<int> elementPos = Game1100Model.Instance.lineData[tempLine.Key];
                    for (int j = 0; j < tempLine.Value; j++)
                    {
                        int temp = (j) * 3 + elementPos[j];
                        slotColumns[j].lstCells[3 - elementPos[j] + 1].index = temp - 1;
                        slotColumns[j].lstCells[3 - elementPos[j] + 1].showLine();
                    }
                }
                uitop1100.ShowMask(true);
                uitop1100.SetAniParent();
                uitop1100.SetAllCashAni(4);
                ShowAllCell(true, true);
                int index = 0;
                CoreEntry.gTimeMgr.AddTimer(times , true, () =>
                {
                    ShowAllCell(false);
                    if (index == Game1100Model.Instance.lines.Count)
                    {
                        index = 0;
                        CoreEntry.gTimeMgr.AddTimer(0, false, () =>
                        {
                            ShowAllCell(true, true);
                        }, 400);
                    }
                    else
                    {
                        KeyValuePair<int, int> tempLine = Game1100Model.Instance.lines[index];
                        List<int> elementPos = Game1100Model.Instance.lineData[tempLine.Key];
                        List<KeyValuePair<int, int>> temp = new List<KeyValuePair<int, int>>();
                        temp.Add(tempLine);

                        CoreEntry.gTimeMgr.AddTimer(0, false, () =>
                        {
                            for (int j = 0; j < tempLine.Value; j++)
                                slotColumns[j].lstCells[3 - elementPos[j] + 1].ShowCellEffect(true);
                            uitop1100.ShowOneLine(tempLine.Key - 1, true);
                            index++;
                        }, 400);
                    }
                }, 399);
            },21);
        }

        public override void ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game1100Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game1100Model.Instance.lines[i];
                List<int> elementPos = Game1100Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    slotColumns[j].lstCells[3 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
                }
                uitop1100.ShowOneLine(tempLine.Key - 1, Game1100Model.Instance.lines.Count != 1 ? bShow : true);
            }
        }

        public void ShowLineOneByOne()
        {
            int index = 0;
            float tempTimes = 0.3f;// 1f / Game1100Model.Instance.lines.Count > 0.3f ? 0.3f : 1f / Game1100Model.Instance.lines.Count;
            CoreEntry.gTimeMgr.AddTimer(tempTimes, true, () =>
            {
                CoreEntry.gAudioMgr.PlayUISound(118);
                KeyValuePair<int, int> tempLine = Game1100Model.Instance.lines[index];
                uitop1100.ShowOneLine(tempLine.Key - 1, true);
                index++;
                if (index == Game1100Model.Instance.lines.Count)
                    CoreEntry.gTimeMgr.RemoveTimer(32);
            }, 32);
        }

        public override int cr2i(int c, int r)
        {
            //服务端下发的是列行~~~
            //int idx = row * SlotData.column + col;
            int idx = c * slotRow + r;
            if (idx < 0 || idx >= Game1100Model.Instance.slotResult.Count) { }
            return idx;
        }

        public void SetSpecialElement()
        {
            for (int i = 0; i < Game1100Model.Instance.arrayLogo.Count; i++)
            {
                int col = i / 4;
                int row = i % 4;
                int element = Game1100Model.Instance.arrayLogo[i];
                slotColumns[col].lstCells[3 - row].setElement(element, row);
            }
            CoreEntry.gTimeMgr.AddTimer(0.03f,false,()=> { uitop1100.SetAniParent(); },5);
            //uitop1100.SetAniParent();
            SetCashAni(Game1100Model.Instance.nSpecialCount);
        }

        public void SetGrearData()
        {
            if(Game1100Model.Instance.lastResult.ContainsKey(Game1100Model.Instance.betID))
            {
                List<int> temp = Game1100Model.Instance.lastResult[Game1100Model.Instance.betID];
                for (int i = 0; i < temp.Count; i++)
                {
                    int col = i / 4;
                    int row = i % 4;
                    int element = temp[i];
                    slotColumns[col].lstCells[3 - row].setElement(element, row);
                }
            }
        }


        public override int cr2ele(int c, int r)
        {
            return Game1100Model.Instance.slotResult[cr2i(c, r)];
        }

        public override void beginSpin(int num0 = 0, bool fast = false)
        {
            if (uitop1100.m_Gold_EffectNew.gameObject.activeSelf)
            {
                uitop1100.m_Gold_EffectNew.callback?.Invoke();
                uitop1100.m_Gold_EffectNew.callback = null;
                uitop1100.m_Gold_EffectNew.gameObject.SetActive(false);
            }

            uitop1100.SetAniParent();
            num = num0;
            //--手动或免费时num为0
            isFastSpin = false;
            if (freeTimes.max > 0)
            {
                isFreeSpin = true;
                freeTimes.times++;
                commonTop.SetFreeTimes(true, Game1100Model.Instance.toSpin.FreeTimes + "");
            }
            else
            {
                if (Game1100Model.Instance.nBet > 0)
                {
                    if (Game1100Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                    {
                        autoSpinNum = 0;// -- 停止自动
                        commonTop.SetSlotSpinNum(autoSpinNum);
                        setState(slotState.Idle);
                        uitop1100.ShowNotEnoughMoneyTips();
                        return;
                    }

                }
                if (commonTop.Bet == commonTop.betMin && Game1100Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                {
                    autoSpinNum = 0;// -- 停止自动
                    commonTop.SetSlotSpinNum(autoSpinNum);
                    setState(slotState.Idle);
                    uitop1100.ShowNotEnoughMoneyTips();
                    return;
                }
                else
                {
                    bool bCanSlot = false;
                    if (Game1100Model.Instance.toSpin.n64Gold < (long)(commonTop.Bet))
                    {
                        int tempBet1 = commonTop.Bet;
                        for (int i = commonTop.BetList.Count - 1; i >= 0; i--)
                        {
                            int tempBet = commonTop.BetList[i];
                            if (tempBet < tempBet1)
                            {
                                commonTop.OnClickBtnMin();
                                if (Game1100Model.Instance.toSpin.n64Gold >= (long)tempBet)
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
                            uitop1100.ShowNotEnoughMoneyTips();
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
            for (int i = 0; i < SlotData_1100.column; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
            }

            if (Game1100Model.Instance.toSpin.nModelGame <= 0)
            {
                int betValue = commonTop.Bet;
                commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold - (int)(betValue));
            }
            setState(slotState.SpinBegin);
            Game1100Model.Instance.bShowFreeAni = false;
            preSpin();
            CoreEntry.gAudioMgr.PlayUISound(122);
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

            //if(Game1000Model.Instance.n64ModelGold)  Game1000Model.Instance.n64ModelGold

            commonTop.UpdateGold(Game1100Model.Instance.toSpin.n64Gold);
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
            return (isFreeSpin && (Game1100Model.Instance.toSpin.nModelGame == Game1100Model.Instance.toSpin.FreeTimes) && Game1100Model.Instance.toSpin.FreeTimes == 0);
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
            if (Game1100Model.Instance.toSpin.WinGold > 0 || Game1100Model.Instance.lines.Count > 0)
            {
                showLines();
                if (isFreeSpin)
                    freeTimes.gold = Game1100Model.Instance.toSpin.WinGold + freeTimes.gold;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotWinGold, null);
            }
        }

        public void ContinueGame()
        {
            if (effectAcc.activeSelf)
                CoreEntry.gTimeMgr.AddTimer(0.15f, false, () => { effectAcc.SetActive(false); }, 24);
            float delayTimes = 0.7f;

            if (Game1100Model.Instance.lines.Count > 0)
            {
                delayTimes = Game1100Model.Instance.lines.Count * 0.3f;
                delayTimes += 0.5f;
            }
                

            if (Game1100Model.Instance.bShowFreeAni)
                return;
            if (isFreeEnd())
                return;
            if (isFreeSpin)//免费没有动画
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () => { continueSpin(); }, 100000);
            else
            {
                if (Game1100Model.Instance.toSpin.rate > 2 || Game1100Model.Instance.toSpin.n64FreeGold > 0 || Game1100Model.Instance.toSpin.n64RSPowerGold > 0)
                    return;
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () => { continueSpin(); }, 100000);
            }
        }

        public void ShakeSlot(bool bLoop = false)
        {
            TfSlot.parent.transform.DOShakePosition(1f, 1f, 7, 5).SetLoops(bLoop?-1:1, LoopType.Restart);
        }
        public void FinishShake()
        {
            TfSlot.parent.transform.DOKill();
            uitop1100.FinishShake();
        }

        public void SetSlotBg(int type = 0)
        {
            if (type == 0)
            {
                m_Trans_SlotBg.gameObject.SetActive(false);
                return;
            }
            m_Trans_SlotBg.gameObject.SetActive(true);
            string bgName = type == 1 ? "a1" : "a2";
            CommonTools.PlayArmatureAni(m_Spine_0.transform, bgName, 0);
            CommonTools.PlayArmatureAni(m_Spine_1.transform, bgName, 0);
            CommonTools.PlayArmatureAni(m_Spine_2.transform, bgName, 0);
            CommonTools.PlayArmatureAni(m_Spine_3.transform, bgName, 0);
            CommonTools.PlayArmatureAni(m_Spine_4.transform, bgName, 0);
        }

        public void SetBgAni()
        {
            ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a3", false,()=>
            {
                ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a1", true);
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CoreEntry.gAudioMgr.StopMusic(soundBgID);
        }

        public void PlayBgSound(int elementCount)
        {
            if (elementCount <= 2)
            {
                soundBgID = 108;
            }
            else if (elementCount <= 10)
            {
                soundBgID = 124;
            }
            else if (elementCount <= 15)
            {
                soundBgID = 125;
            }
            else if (elementCount <= 20)
            {
                soundBgID = 126;
            }
            CoreEntry.gAudioMgr.PlayUIMusic(soundBgID);
        }

        public void PlayCashSound(int elementCount = 0)
        {
            if (elementCount <= 0)
                return;
            int soundId = 0;
            int nextSound = -1;
            if (elementCount <= 2)
                soundId = 114;
            else if (elementCount <= 10)
            {
                soundId = 115;
                nextSound = 111;
            }
            else if (elementCount <= 15)
            {
                soundId = 116;
                nextSound = 112;
            }
            else
            {
                soundId = 117;
                nextSound = 113;
            }
            CoreEntry.gAudioMgr.PlayUISound(soundId, m_Trans_Sound.gameObject);
            //float delayTimes = GetSoundTime(soundId);
            if (nextSound > 0)
                CoreEntry.gAudioMgr.PlayUISound(nextSound, m_Trans_Sound2.gameObject);
            //CoreEntry.gTimeMgr.AddTimer(0.1f, false,()=> 
            //{
            //    CoreEntry.gAudioMgr.PlayUISound(nextSound, m_Trans_Sound2.gameObject);
            //},7);
        }

        public float GetSoundTime(int id)
        {
            AudioConfig config = CoreEntry.gAudioMgr. GetAudioConfig(id);
            if (config == null)
            {
                LogMgr.UnityError("无效音效ID:" + id);
                return 0;
            }
            var finalPath = config.path;
            AudioClip clip = (AudioClip)CoreEntry.gResLoader.LoadAudio(finalPath, LoadModule.AssetType.AudioMp3);
            if (clip == null)
            {
                LogMgr.UnityError("音效配置错误 路径:" + finalPath + "  id" + id);
                return 0;
            }
            return clip.length;
        }

        public void SetCashAni(int index)
        {
            if (index < 3)
            {
                m_Spine_Cash.gameObject.SetActive(false);
                m_Trans_SlotBg.gameObject.SetActive(false);
                return;
            }
            string aniName = string.Format("{0}a", index);
            string aniName2 = string.Format("{0}b", index);
            if (index < 10)
            {
                aniName = string.Format("0{0}a", index);
                aniName2 = string.Format("0{0}b", index);
            }
            m_Spine_Cash.gameObject.SetActive(true);
            m_Spine_Cash.AnimationState.ClearTracks();
            ToolUtil.PlayAnimation(m_Spine_Cash.transform, aniName, false,()=> 
            {
                ToolUtil.PlayAnimation(m_Spine_Cash.transform, aniName2, true);
            });
            m_Spine_Cash.transform.localPosition = Game1100Model.Instance.cashPos[index];
            m_Spine_Cash.transform.localScale = Game1100Model.Instance.cashScale[index];

            int type = 0;
            if (index >= 11 && index < 16)
                type =1;
            else if (index >= 16)
                type = 2;
            SetSlotBg(type);
        }

        public bool bActiveDragon_Cash()
        {
            return m_Spine_Cash.gameObject.activeSelf;
        }
    }

}


  