using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm700SlotColumn : UISlotColumn
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

                lstCells[i].setElement(UnityEngine.Random.Range(1, 11), i);
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(UnityEngine.Random.Range(1, 11), i);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 3)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(UnityEngine.Random.Range(1, 11), -1);// --元素的数量
        }

        public override async void playAcc()
        {
            times = SlotData_700.rollAccTimes;
            duration = SlotData_700.rollAccTime;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.35f));
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            base.onRollFinish();
            if (slotCpt.StateSlot == slotState.SpinStop)
            {
                if (column == 4)
                    CoreEntry.gAudioMgr.PlayUISound(224,transform.GetChild(1).gameObject);
            }
            else
                CoreEntry.gAudioMgr.PlayUISound(224, transform.GetChild(1).gameObject);

            bool bHasFree = false;
            for(int i = column *3;i < (column+1 * 3); i++)
            {
                if (Game700Model.Instance.slotResult[i] == 10)
                    bHasFree = true;
            }
            if(bHasFree)
                CoreEntry.gAudioMgr.PlayUISound(228,transform.GetChild(0).gameObject);


            for (int i = 0;i < 4;i++)
            {
                if((lstCells[i].element == 9 || lstCells[i].element == 11) && lstCells[i].row >=0 && lstCells[i].row <3)
                {
                    GameObject goBg = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game700/FirstRes/Tb"+ lstCells[i].element);
                    goBg.transform.SetParent(lstCells[i].TfSpine, true);
                    goBg.transform.localScale = Vector3.one * 1.1f;
                    goBg.transform.localPosition = new Vector3(0, 0, 0);
                    ToolUtil.PlayAnimation(goBg.transform, "idle",true);
                    lstCells[i].ImgElement.gameObject.SetActive(false);
                }
                
            }
        }

        protected override void moveDown()
        {
            float from = startY - offsetY;
            float to = from - slotCpt.heightCell ;

            if (times <=0 && slotCpt.effectAcc.activeSelf && Game700Model.Instance.freeColumn == column)
            {
                duration = SlotData_700.rollTime+1.35f;
            }

            if (tweenMove == null)
            {
                tweenMove = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), duration ).OnComplete(moveFinish).SetEase(Ease.Linear).SetAutoKill(false);
            }
            else
            {
                tweenMove.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), duration).Restart();
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
            {
                roll();
            }
            else
            { //最后一行
                isRolling = false;

                onRollFinish();
                slotCpt?.finishRoll(column);

                //if (playBounce)

                if (true)
                {
                    //最后一下多个回弹效果
                    float from = startY - offsetY;
                    float to = from - slotCpt.heightCell / 20;
                    if (tweenBounce == null)
                    {
                        tweenBounce = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), SlotData_700.rollElasticityTimes).SetEase(Ease.InOutSine).SetAutoKill(false).SetLoops(2, LoopType.Yoyo);
                    }
                    else
                    {
                        tweenBounce.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), SlotData_700.rollElasticityTimes).Restart();
                    }
                }
            }
        }

        public override void reset()
        {
            base.reset();
            duration = SlotData_700.rollTime;
        }
    }
}
