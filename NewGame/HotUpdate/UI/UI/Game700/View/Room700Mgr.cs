using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class Room700Mgr : Singleton<Room700Mgr>
    {
        public UIRoom700_SlotCpt slot;
        public TO_Spin_700 toSpin;
        private List<Action> StepList = new List<Action>();
        private float waitTmes = 0;
        public List<Transform> sunGoldEffect = new List<Transform>();
        public List<int> sunGoldEffectIndex = new List<int>();
        public List<Transform> GoldEffect = new List<Transform>();

        public void Init(UIRoom700_SlotCpt slot, TO_Spin_700 spin)
        {
            this.slot = slot;
            this.toSpin = spin;
            StepList.Clear();
            StepList.Add(StepWinGold);
            StepList.Add(StepFreeEnd);
        }


        public void RunStepList()
        {
            waitTmes = 0;
            for (int i = 0; i < StepList.Count; i++)
                StepList[i]();
        }

        public void StepWinGold()
        {
            if (toSpin.WinGold > 0 || Game700Model.Instance.lines.Count > 0)
            {
                slot.showLines();
                if (slot.isFreeSpin)
                    slot.freeTimes.gold = toSpin.WinGold + slot.freeTimes.gold;
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotWinGold, null);
            }
        }

        public void StepFreeEnd()
        {
            if (slot.effectAcc.activeSelf)
                CoreEntry.gTimeMgr.AddTimer(0.15f, false, () => { slot.effectAcc.SetActive(false); }, 24);

            float delayTimes = 1f + waitTmes;
            if (Game700Model.Instance.bShowFreeAni)
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
                if (toSpin.rate > 2 || Game700Model.Instance.toSpin.n64FreeGold > 0 || toSpin.n64RSPowerGold > 0)
                    return;
                CoreEntry.gTimeMgr.AddTimer(delayTimes, false, () =>
                {
                    slot.continueSpin();
                }, 100000);
            }
        }

    }
}
