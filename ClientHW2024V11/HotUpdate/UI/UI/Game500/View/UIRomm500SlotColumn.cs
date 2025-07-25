using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm500SlotColumn : UISlotColumn
    {

        public override void Awake()
        {
            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void init()
        {
            startY = rtf.anchoredPosition.y;
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].SelfTransform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;

                lstCells[i].setElement(Random.Range(1, 13), -1);
            }
        }


        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Random.Range(1, 13), -1);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 3)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(Random.Range(1, 14), -1);// --元素的数量
        }
        public override void playAcc()
        {
            times = SlotData_500.rollAccTimes;
            duration = SlotData_500.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();

            int sound = 0;
            if (column == 0)
                sound = 22;
            else if (column == 1)
                sound = 23;
            else if (column == 2)
                sound = 24;
            else if (column == 3)
                sound = 25;
            else
                sound = 26;
            CoreEntry.gAudioMgr.PlayUISound(sound);
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
                    float from = startY - offsetY;
                    float to = from - slotCpt.heightCell / 4;
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
