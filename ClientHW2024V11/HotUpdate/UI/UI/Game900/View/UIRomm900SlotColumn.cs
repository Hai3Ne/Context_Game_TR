using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm900SlotColumn : UISlotColumn
    {

        public override void init()
        {
            startY = rtf.anchoredPosition.y;
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].SelfTransform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;
                lstCells[i].setElement(Random.Range(1, 18), i);
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Random.Range(1, 18), -1);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 3)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(Random.Range(1, 11), -1);// --元素的数量
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

            if (slotCpt.StateSlot == slotState.SpinStop)
            {
                if (column == 4)
                    CoreEntry.gAudioMgr.PlayUISound(79);
            }
            else
                CoreEntry.gAudioMgr.PlayUISound(79);


            bool bHasDiamon = false;
            bool bHasJackPot = false;
            for (int i = lstCells.Count - 1; i >= 0; i--)
            {
                if (SlotData_900.elementFree == lstCells[i].element)
                    bHasDiamon = true;
                if (SlotData_900.jackpotElement == lstCells[i].element)
                    bHasJackPot = true;
                lstCells[i].onRollFinish();
            }

            UIRoom900 slotCpt900 = slotCpt as UIRoom900;
            if (bHasDiamon)
                CoreEntry.gAudioMgr.PlayUISound(68, slotCpt900.GetSoundObj());
            if (bHasJackPot)
                CoreEntry.gAudioMgr.PlayUISound(81, slotCpt900.GetSoundObj());
        }

        protected override void moveFinish()
        {
            offsetY += slotCpt.heightCell;
            //最下一格移到最上
            UISlotCell cell = lstCells[0];
            lstCells.RemoveAt(0);
            cell.rtf.anchoredPosition = new Vector2(0, maxY + offsetY);
            cell.rtf.SetSiblingIndex(0);
            lstCells.Add(cell);
            nextElement(cell);
            if (times != 0)
            {
                roll();
            }
            else
            { //最后一行
                isRolling = false;
                //RoomMgr.roomUI?.playColumnAcc(-1);

                onRollFinish();
                slotCpt?.finishRoll(column);

                //if (playBounce)

                if (true)
                {
                    //最后一下多个回弹效果
                    //float from = startY - offsetY;
                    //float to = from - slotCpt.heightCell / 2;
                    //if (tweenBounce == null)
                    //{
                    //    tweenBounce = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).SetEase(Ease.Linear).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
                    //}
                    //else
                    //{
                    //    tweenBounce.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).Restart();
                    //}
                }
            }
        }

        public override void onSpinFinish(bool isWin)
        {
            for (int i = lstCells.Count - 1; i >= 0; i--)
                lstCells[i].onSpinFinish();
        }

        public override void reset()
        {
            base.reset();
            duration = SlotData_900.rollTime;
        }
    }
}
