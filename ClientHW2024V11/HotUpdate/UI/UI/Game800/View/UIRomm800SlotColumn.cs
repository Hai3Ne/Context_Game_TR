using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class UIRomm800SlotColumn : UISlotColumn
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
            bool bShowOne = Random.Range(1, 2)<=1;
            Game800Model.Instance.bOneList.Add(bShowOne);
            for (int i = 0; i < lstCells.Count; i++)
            {
                RectTransform cell = lstCells[i].SelfTransform.GetComponent<RectTransform>();
                if (cell.anchoredPosition.y > maxY)
                    maxY = cell.anchoredPosition.y;
                int rand = 0;
                if (bShowOne)
                {
                    if (i == 2 || i == 5)
                        rand = Random.Range(1, 9);
                    else
                        rand = 108;
                    if ((column == 0 || column == 2) && rand == 1)
                        rand = 2;
                    else
                    {
                        if( column == 1)
                        {
                            if (rand == 2 || rand == 3 || rand == 4)
                                rand = 5;
                        }
                    }


                   if(i == 5)
                        lstCells[i].setElement(rand, 100);
                   else
                        lstCells[i].setElement(rand, i);
                }
                else
                {
                    if (i == 0 || i == 4)
                        rand = Random.Range(1, 9);
                    else
                        rand = 108;
                    lstCells[i].setElement(rand, i);
                }
                
            }
        }

        protected override void moveDown()
        {
            float from = startY - offsetY;
            float to = from - slotCpt.heightCell;
            float temp = 0.005f;
            //if (Application.isEditor)
            //    temp *= 10;
            if (tweenMove == null)
            {
                tweenMove = rtf.DOAnchorPos(new Vector2(rtf.anchoredPosition.x, to), temp).OnComplete(moveFinish).SetEase(Ease.Linear).SetAutoKill(false);
            }
            else
            {
                tweenMove.ChangeValues(new Vector2(rtf.anchoredPosition.x, from), new Vector2(rtf.anchoredPosition.x, to), temp).Restart();
            }
        }

        public override void initCell()
        {
            for (int i = 0; i < lstCells.Count; i++)
                lstCells[i].setElement(Random.Range(1, 7), -1);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 5)
            {
                int ele = Game800Model.Instance.slotResult[column];
                if (ele != 10)
                {
                 
                    if(times == 3)
                    {
                        cell.setElement(ele, times - 1);
                    }
                    else
                    {
                        ele = Random.Range(1, 9);
                        cell.setElement(ele, 100);
                    }
              
                }
                else
                {
                    int rand = Random.Range(1, 9);
                    if ((column == 0 || column == 2) && rand == 1)
                        rand = 2;
                    else
                    {
                        if (column == 1)
                        {
                            if (rand == 2 || rand == 3 || rand == 4)
                                rand = 5;
                        }
                    }

                    ele = rand;
                    if (times == 5 || times == 1)
                        cell.setElement(ele, times);
                    else
                        cell.setElement(ele, 100);

                }               
            }
            else
            {
                if(times == 0)
                    cell.setElement(Random.Range(1, 9), 100);// --元素的数量
                else
                    cell.setElement(Random.Range(1, 9), -1);// --元素的数量
            }
                
        }

        public override void playAcc()
        {
            times = SlotData_800.rollAccTimes;
            duration = SlotData_800.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public override void onRollFinish()
        {
            CoreEntry.gAudioMgr.PlayUISound(55);
            if (column == 0)
            {
                if (Game800Model.Instance.slotResult[0] == 3 || Game800Model.Instance.slotResult[0] == 4 || Game800Model.Instance.slotResult[0] == 1 || Game800Model.Instance.slotResult[0] == 2)
                {
                    UIRoom800_SlotCpt tempSlotCpt = slotCpt as UIRoom800_SlotCpt;
                    tempSlotCpt.SetKuangAni(0, false);
                    CoreEntry.gAudioMgr.PlayUISound(52);
                }
            }
            else if (column == 1)
            {
                if(Game800Model.Instance.slotResult[1] != 1)
                {
                    if ((Game800Model.Instance.slotResult[0] == Game800Model.Instance.slotResult[1] && Game800Model.Instance.slotResult[0] != 10)
                        || ((Game800Model.Instance.slotResult[0] == 1 || Game800Model.Instance.slotResult[0] == 2) && Game800Model.Instance.slotResult[1] != 10)
                        || ((Game800Model.Instance.slotResult[1] == 1 || Game800Model.Instance.slotResult[1] == 2) && Game800Model.Instance.slotResult[0] != 10))
                    {
                        UIRoom800_SlotCpt tempSlotCpt = slotCpt as UIRoom800_SlotCpt;
                        UIRoom800Top top800 = tempSlotCpt.uiTop as UIRoom800Top;
                        tempSlotCpt.SetKuangAni(1, false);
                        tempSlotCpt.slotColumns[2].times = SlotData_800.rollAccTimes;
                        tempSlotCpt.slotColumns[2].duration = SlotData_800.rollAccTime;
                        CoreEntry.gAudioMgr.PlayUISound(51, top800.GoSound);
                    }
                }
            }
            else
            {
                if (Game800Model.Instance.slotResult[1] == 1 && Game800Model.Instance.freeGameData.Count == Game800Model.Instance.freeGameCount && Game800Model.Instance.freeGameData.Count > 0)
                {
                    UIRoom800_SlotCpt tempSlotCpt = slotCpt as UIRoom800_SlotCpt;
                    UIRoom800Top top800 = tempSlotCpt.uiTop as UIRoom800Top;
                    CoreEntry.gAudioMgr.PlayUISound(56);
                    CoreEntry.gAudioMgr.PlayUISound(64, top800.GoSound);
                }
            }
        }

        public override void stop()
        {
            tweenMove.Kill();
            tweenMove = null;
            tweenBounce.Kill();
            tweenBounce = null;

            isRolling = false;
            times = 0;
            slotCpt.setState(slotState.Idle);
            reset();
            int ele = Game800Model.Instance.slotResult[column];
            bool bTen = ele < 10;
            for (int i = 0; i < lstCells.Count; i++)
            {
                if (bTen)
                {
                    if (i == 2)
                    {
                        lstCells[i].setElement(Game800Model.Instance.slotResult[column], i);
                    }
                    else
                    {
                        ele = Random.Range(1, 9);
                        lstCells[i].setElement(ele, 100);
                    }
                }
                else
                {
                    int rand = Random.Range(1, 9);
                    if ((column == 0 || column == 2) && rand == 1)
                        rand = 2;
                    else
                    {
                        if (column == 1)
                        {
                            if (rand == 2 || rand == 3 || rand == 4)
                                rand = 5;
                        }
                    }

                    ele = rand;
                    if (i == 4 || i == 0)
                        lstCells[i].setElement(ele, times);
                    else
                        lstCells[i].setElement(ele, 100);

                }
            }
            slotCpt?.finishRoll(column);
        }
    }
}
