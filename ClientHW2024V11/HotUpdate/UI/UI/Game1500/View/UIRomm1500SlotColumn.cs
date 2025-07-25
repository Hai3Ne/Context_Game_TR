using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm1500SlotColumn : UISlotColumn
    {
        public override void init()
        {
            startY = transform.GetComponent<RectTransform>().anchoredPosition.y;
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].transform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;
                lstCells[i].setElement(Random.Range(1, 8), lstCells.Count - 2 - i);
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Random.Range(1, 8), lstCells.Count - 2 - i);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= (column!=1? 3:4))
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(Random.Range(1, 8), -1);// --元素的数量
        }

        public override void playAcc()
        {
            times = SlotData_1500.rollAccTimes;
            duration = SlotData_1500.rollAccTime;
            slotCpt.playColumnAcc(column);
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
                onRollFinish();
                slotCpt?.finishRoll(column);
                if (!slotCpt.isFastSpin || Game1500Model.Instance.gameStates == 1)
                {
                    //最后一下多个回弹效果
                    float from = startY - offsetY;
                    float to = from - slotCpt.heightCell / 10;
                    if (tweenBounce == null)
                        tweenBounce = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).SetEase(Ease.Linear).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
                    else
                        tweenBounce.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).Restart();
                }
            }
        }

        public override void onSpinFinish(bool isWin)
        {
            int index = column != 1 ? 3 : 4;
            for (int i = index; i >= 0; i--)
            {
                lstCells[i].onSpinFinish();
            }
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
            if (slotCpt.StateSlot == slotState.SpinStop)
            {
                if (column == 4)
                    CoreEntry.gAudioMgr.PlayUISound(286);
            }
            else
                CoreEntry.gAudioMgr.PlayUISound(286);
        }

        public override void reset()
        {
            base.reset();
            if(Game1500Model.Instance.bInFreeGame())
                duration = SlotData_1500.specialRollTime;
            else if(slotCpt.isFastSpin)
                duration = SlotData_1500.fastRollTimes;
            else
                duration = SlotData_1500.rollTime;

        }
    }
}
