using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class Room50Mgr : Singleton<Room50Mgr>
    {
        
        public UIRoom500_SlotCpt slot;
        public TO_Spin toSpin;
        private List<Action> StepList = new List<Action>();
        private float waitTmes = 0;
        public List<Transform> sunGoldEffect = new List<Transform>();
        public List<int> sunGoldEffectIndex = new List<int>();
        public List<Transform> GoldEffect = new List<Transform>();

        public void Init(UIRoom_SlotCpt slot, TO_Spin spin)
        {
            this.slot = slot as UIRoom500_SlotCpt;
            this.toSpin = spin;
            StepList.Clear();
            StepList.Add(StepWinGold);
            StepList.Add(SpecialGameSunList);
            StepList.Add(StepSpecialGame);
            StepList.Add(StepFreeBegin);
            StepList.Add(StepFreeEnd);
        }



        public void RunStepList()
        {
            waitTmes = 0;
            if(toSpin.SpecialGame == toSpin.nModelGame && toSpin.SpecialGame > 0)
            {
                for (int i = 0; i < StepList.Count - 1;i++)
                    StepList[i]();
            }
            else
            {
                for (int i = 0; i < StepList.Count; i++)
                    StepList[i]();
            }
        }

        public void SpecialGameSunList()
        {
            if (Game500Model.Instance.bInSpecialGame && (toSpin.nModelGame > 0 && Game500Model.Instance.toSpin.FreeTimes <= 0))
            {
                if (slot.hasElement(SlotData_500.specialelement))
                {
                    for (int i = 0;i < Game500Model.Instance.slotResult.Count;i++)
                    {
                        if(Game500Model.Instance.slotResult[i] == SlotData_500.specialelement)
                        {
                            if(!bContainElement(i))
                            {
                                sunGoldEffectIndex.Add(i);

                                int temp = i / 3;
                                slot.ShowOneCellLine(temp, i  % 3 ,null,0,0);
                            }
                        }
                    }
                }
            }
        }

        public bool bContainElement(int index)
        {
            bool bContain = false;
            for(int i = 0;i < sunGoldEffectIndex.Count;i++)
            {
                if (sunGoldEffectIndex[i] == index)
                    bContain = true;
            }
            return bContain;
        }

        public void StepWinGold()
        {
            if (toSpin.SpecialGame == toSpin.nModelGame && toSpin.nModelGame > 0 && slot.Top500.TfSpecialGameMask.gameObject.activeSelf == false)
            {
                sunGoldEffect.Clear();
                sunGoldEffectIndex.Clear();
                GoldEffect.Clear();
            }
            if (toSpin.WinGold > 0 || Game500Model.Instance.lines.Count > 0)
            {
                slot.showLines();
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotWinGold, null);
            }
        }

        public void StepSpecialGame()
        {
            if (toSpin.SpecialGame == toSpin.nModelGame && toSpin.nModelGame > 0)
            {
                waitTmes = 3;
                CoreEntry.gTimeMgr.RemoveTimer(380);
                CoreEntry.gTimeMgr.AddTimer(2.1f, false, () => 
                {
                    sunGoldEffect.Clear();
                    for(int i = 0;i < slot.uiTop.SpineCell.Count;i++)
                    {
                        if(slot.uiTop.SpineCell[i].childCount > 0)
                            sunGoldEffect.Add(slot.uiTop.SpineCell[i].GetChild(0));
                    }
                    CoreEntry.gAudioMgr.PlayUISound(30);
                    for (int i = 0;i < sunGoldEffect.Count;i++)
                         CommonTools.PlayArmatureAni(sunGoldEffect[i].transform, "dz2", 1, () => { });

                    CoreEntry.gTimeMgr.RemoveTimer(380);
                }, 380);

                CoreEntry.gTimeMgr.AddTimer(3, false, () => 
                {
                    if (slot.Top500.TfSpecialGame.gameObject.activeSelf)
                    {
                        slot.Top500.CliSpecialRoll();
                        for (int i = 0; i < GoldEffect.Count; i++)
                            GoldEffect[i].transform.gameObject.SetActive(true);
                    }
                    else
                    {
                        CoreEntry.gAudioMgr.PlayUIMusic(29);
                        slot.Top500.ShowSpecialGame(true);
                        slot.Top500.ShowSpecialGameWindow(true);
                        slot.Top500.SetSpecialTimes(toSpin.SpecialGame);
                        for (int i = 0; i < GoldEffect.Count; i++)
                            GoldEffect[i].transform.gameObject.SetActive(true);
                        CoreEntry.gTimeMgr.RemoveTimer(381555);
                    }

                }, 381555);
            }
        }

        public void StepFreeBegin()
        {

        }

        public void StepFreeEnd()
        {
            if(slot.effectAcc.activeSelf)
                CoreEntry.gTimeMgr.AddTimer(0.15f, false, () => { slot.effectAcc.SetActive(false); }, 24);

            if (Game500Model.Instance.bSpecialFinish)
                return;

            float delayTimes = 0.8f + waitTmes;
            if (Game500Model.Instance.toSpin.rate > 1 && Game500Model.Instance.toSpin.rate <= 2)
                delayTimes = 1.2f;
            if (Game500Model.Instance.bShowFreeAni)
                delayTimes = 2;
            if (slot.isFreeSpin)//免费没有动画
            {
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () =>
                {
                    slot.continueSpin();
                }, 100000);
            }
            else
            {
                if (toSpin.rate > 2 || Game500Model.Instance.toSpin.n64FreeGold > 0 || toSpin.n64RSPowerGold > 0)
                    return;
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () =>
                {
                    slot.continueSpin();
                }, 100000);
            }
        }

    }
}
