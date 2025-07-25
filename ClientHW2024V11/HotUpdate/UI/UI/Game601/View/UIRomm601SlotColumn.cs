using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm601SlotColumn : UISlotColumn
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
                lstCells[i].setElement(Random.Range(1, 11), -1);
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
            times = SlotData_601.rollAccTimes;
            duration = SlotData_601.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
            CoreEntry.gAudioMgr.PlayUISound(145);
        }
        public override void reset()
        {
            base.reset();
            duration = SlotData_601.rollTime;
        }

        public override void roll(int reduce = 1)
        {
            //小于0代表无限roll
            if (times != 0)
            {
                times -= reduce;
                isRolling = true;
                moveDown();
            }
        }

        int rate = 1;
        protected override void moveDown()
        {
            rate = times >= 6 ? 3 : 1;
            if (times < 0)
                rate = 3;
            float from = startY - offsetY;
            float to = from - slotCpt.heightCell * rate;
            if (tweenMove == null)
            {
                tweenMove = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), duration * rate).OnComplete(moveFinish).SetEase(Ease.Linear).SetAutoKill(false);
            }
            else
            {
                tweenMove.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), duration * rate).Restart();
            }
        }

        protected override void moveFinish()
        {
            for (int i = 0; i < rate; i++)
            {
                offsetY += slotCpt.heightCell;
                //最下一格移到最上
                UISlotCell cell = lstCells[0];
                lstCells.RemoveAt(0);
                cell.rtf.anchoredPosition = new Vector2(0, maxY + offsetY);
                cell.rtf.SetSiblingIndex(0);
                lstCells.Add(cell);
                UISlotCell cell3 = rate==3?cell: lstCells[3];
                //UISlotCell cell3 = cell;
                nextElement(cell3);
            }
            if (times != 0)
            {
                roll(rate);
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
                    float from = startY - offsetY;
                    float to = from - slotCpt.heightCell / 50;
                    if (tweenBounce == null)
                    {
                        tweenBounce = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).SetEase(Ease.InOutSine).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
                    }
                    else
                    {
                        tweenBounce.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), SlotData_500.rollElasticityTimes).Restart();
                    }
                }
            }
        }
    }
}
