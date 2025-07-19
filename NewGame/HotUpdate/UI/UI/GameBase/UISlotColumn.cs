using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UISlotColumn : MonoBehaviour
    {
        public UIRoom_SlotCpt slotCpt;
        public RectTransform rtf;
        public List<UISlotCell> lstCells = new List<UISlotCell>();
        public int column = -1;
        public Tweener tweenMove;
        public Tweener tweenBounce;
        protected float startY = 0;// --列坐标
        protected float offsetY = 0;// --列偏移
        protected float maxY = 0;// -- 最上方格子位置
        public int times = 0;// -- 剩余roll的次数(下移一行算一次)
        protected float duiation = -1;//-- roll一行时间
        public bool isRolling = false;
        public float duration = 0.085f;
        public Transform SelfTransform;
        public virtual void Awake()
        {
            SelfTransform = transform;
            rtf = transform.GetComponent<RectTransform>();
        }

        public virtual void OnDestroy()
        {
            tweenMove.Kill();
            tweenMove = null;
            tweenBounce.Kill();
            tweenBounce = null;
        }

        public void KillTweener()
        {
            tweenMove.Kill();
            tweenMove = null;
        }


        public virtual void init()
        {
        }

        public virtual void initCell()
        {
        }

        public virtual void nextElement(UISlotCell cell)
        {
        }

        public virtual void onRollFinish()
        {
            //for (int i = 0; i < lstCells.Count; i++)
            //    lstCells[i].onRollFinish()
        }

        public virtual void onSpinFinish()
        {
            //for (int i = 0; i < lstCells.Count; i++)
            //    lstCells[i].onSpinFinish();
        }

        public virtual void onSpinFinish(bool isWin)
        {
            //for (int i = 0; i < lstCells.Count; i++)
            //    lstCells[i].onSpinFinish();
        }

        public virtual void beginRoll(int time)
        {
            times = time;
            roll();
        }

        public virtual void endRoll(int time)
        {
            if (times < 0)
                times = time;
            else
                times = Mathf.Min(times, time);
        }

        public virtual void stop()
        {        
            times = 0;
            if (isRolling)
            {
                for (int i = 1; i < lstCells.Count; i++)
                {
                    lstCells[i].setElement(slotCpt.cr2ele(column, slotCpt.slotRow - i));
                }
            }
        }

        public virtual void roll(int reduce = 1)
        {
            //小于0代表无限roll
            if (times != 0)
            {
                times--;
                isRolling = true;
                moveDown();
            }
        }
        protected virtual void moveDown()
        {
            float from = startY - offsetY;
            float to = from - slotCpt.heightCell;
            if (tweenMove == null)
            {
                tweenMove = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to) , duration).OnComplete(moveFinish).SetEase(Ease.Linear).SetAutoKill(false);
            }
            else
            {
                tweenMove.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), duration).Restart();
            }
        }

        protected virtual void moveFinish()
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
                    float to = from - slotCpt.heightCell / 2;
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

        public virtual void reset()
        {
            //滚动停止后全部复位
            for (int i = 0; i < lstCells.Count; i++)
            {
                lstCells[i].rtf.anchoredPosition = new Vector2(0, lstCells[i].rtf.localPosition.y - offsetY);
                lstCells[i].reset();
            }
            rtf.anchoredPosition = new Vector2(rtf.anchoredPosition.x, startY);
            offsetY = 0;
            duration = SlotData_500.rollTime;
        }

        public virtual void playAcc()
        {
          
        }

    }
}
