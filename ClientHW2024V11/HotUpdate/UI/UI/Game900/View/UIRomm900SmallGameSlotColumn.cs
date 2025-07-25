using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm900SmallGameSlotColumn : UISlotColumn
    {

        public override void init()
        {
            startY = rtf.anchoredPosition.y;
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].SelfTransform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;
                lstCells[i].setElement(Random.Range(1, 11), i);
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Random.Range(1, 18), -1);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <=1)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(Game900Model.Instance.smallGameElementList[Random.Range(0, 5)] , -1);// --元素的数量
        }

        public override void playAcc()
        {
            times = SlotData_900.rollAccTimes;
            duration = SlotData_900.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
            //if(slotCpt.StateSlot == slotState.SpinStop)
            //{
            //    if(column == 4)
            //        CoreEntry.gAudioMgr.PlayUISound(40);
            //}
            //else
            //    CoreEntry.gAudioMgr.PlayUISound(40);
        }

        public override void onSpinFinish(bool isWin)
        {
            for (int i = lstCells.Count - 1; i >= 0; i--)
                lstCells[i].onSpinFinish();
        }
    }
}
