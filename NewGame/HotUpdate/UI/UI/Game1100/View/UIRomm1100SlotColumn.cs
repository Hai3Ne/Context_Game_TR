using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm1100SlotColumn : UISlotColumn
    {

        public override void init()
        {
            startY = rtf.anchoredPosition.y;
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].SelfTransform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;
                lstCells[i].setElement(Game1100Model.Instance.initElement[column], i);
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Game1100Model.Instance.initElement[column], i);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 4)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(Random.Range(1, 11), -1);// --元素的数量
        }

        public override void playAcc()
        {
            times = SlotData_1100.rollAccTimes;
            duration = SlotData_1100.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
            if (slotCpt.StateSlot == slotState.SpinStop)
            {
                if (column == 4)
                    CoreEntry.gAudioMgr.PlayUISound(123);
            }
            else
                CoreEntry.gAudioMgr.PlayUISound(123);

            for (int i = lstCells.Count - 1; i >= 0; i--)
            {
                lstCells[i].onRollFinish();
            }
            UIRoom1100 uiroom1100 = slotCpt as UIRoom1100;
            for (int i = 0; i < lstCells.Count - 1; i++)
            {
                int index = column * 4 + lstCells[i].row;
                if (lstCells[i].element == SlotData_1100.specialelement)
                {
                    if(uiroom1100.uitop1100.SpineCell[index].childCount <= 0)
                    {
                        if (Game1100Model.Instance.nSpecialCount > 0)
                            Game1100Model.Instance.bHasNewSpeiclaElement = true;
                        uiroom1100.ShakeSlot();
                        uiroom1100.SetBgAni();
                        int count = Game1100Model.Instance.HasElementNum(column, SlotData_1100.specialelement);
                        uiroom1100.PlayCashSound(count);

                        lstCells[i].TfSpine.GetChild(0).transform.SetParent(uiroom1100.uitop1100.SpineCell[index], true);
                    }                   
           
                }
            }
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
            //rate = times >= 8 ? 3 : 1;
            //if (times < 0)
            //    rate = 3;
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
                //UISlotCell cell3 = rate == 3 ? cell : lstCells[3];
                UISlotCell cell3 = cell;
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
            {
                lstCells[i].onSpinFinish();
                if(i == 4)
                {
                    if (lstCells[i].element == 10 && lstCells[i].TfSpine.childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(lstCells[i].TfSpine.GetChild(0).gameObject);
                }
            }            
        }




        public override void reset()
        {
            base.reset();
            duration = SlotData_1100.rollTime;
        }
    }
}
