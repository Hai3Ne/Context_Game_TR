using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm1600SlotColumn : UISlotColumn
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
            if (times > 0 && times <= (column != 1 ? 3 : 4))
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
            {
                if (Game1600Model.Instance.lastFreeGameIndex > 0)
                    cell.setElement(Random.Range(0, 6) <= 1 ? 8 : 0, -1);
                else
                    cell.setElement(Random.Range(1, 8), -1);// --元素的数量
            }
        }

        public override void playAcc()
        {
            times = SlotData_1600.rollAccTimes;
            duration = SlotData_1600.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            for(int i = 0;i < lstCells.Count;i++)
                lstCells[i].onRollFinish();

            if (slotCpt.StateSlot == slotState.SpinStop)
            {
                if (column == 4)
                    CoreEntry.gAudioMgr.PlayUISound(267);
            }
            else
                CoreEntry.gAudioMgr.PlayUISound(267);

            if (Game1600Model.Instance.bHasElement(SlotData_1600.elementWild,column))
            {
                CoreEntry.gAudioMgr.PlayUISound(278, transform.GetChild(1).gameObject);
            }
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
                roll();
            else
            { //最后一行
                isRolling = false;
                onRollFinish();
                slotCpt?.finishRoll(column);
                if (true)
                {
                    //最后一下多个回弹效果
                    float from = startY - offsetY;
                    float to = from - slotCpt.heightCell / 20;
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



        public override void reset()
        {
            base.reset();
            if(Game1600Model.Instance.gameStates == 1)
                duration = SlotData_1600.specialRollTime;
            else if(slotCpt.isFastSpin)
                duration = SlotData_1600.fastRollTimes;
            else
                duration = SlotData_1600.rollTime;

        }
    }
}
