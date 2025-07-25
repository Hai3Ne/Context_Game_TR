using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm1200SlotColumn : UISlotColumn
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
            if (times > 0 && times <= 3)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else if(Game1200Model.Instance.bStartRollSpecialGame)
            {
                List<int> cells = null;
                ///特俗游戏只会出现空 百变 和特定的元素
                if (Game1200Model.Instance.bConfirmSpecialElement)
                    cells = new List<int> {  0, Game1200Model.Instance.ucLogo, Game1200Model.Instance.ucLogo, 7,7,7,};
                else
                    cells = new List<int> {  0, 7, 7,7,7,7};
                cell.setElement(cells[Random.Range(0, 6)], -1);// --元素的数量
            }
            else
                cell.setElement(Random.Range(1, 8), -1);// --元素的数量
        }

        public override void playAcc()
        {
            times = SlotData_1200.rollAccTimes;
            duration = SlotData_1200.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
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
                if (true)
                {
                    if (!slotCpt.isFastSpin|| Game1200Model.Instance.gameStates == 1)
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
        }

        public override void onSpinFinish(bool isWin)
        {
            for (int i = lstCells.Count - 2; i >= 0; i--)
            {
                lstCells[i].onSpinFinish();
            }
        }

        public override void reset()
        {
            base.reset();
            if(Game1200Model.Instance.specialGameLists.Count > 0)
                duration = SlotData_1200.specialRollTime;
            else if(slotCpt.isFastSpin)
                duration = SlotData_1200.fastRollTimes;
            else
                duration = SlotData_1200.rollTime;

        }
    }
}
