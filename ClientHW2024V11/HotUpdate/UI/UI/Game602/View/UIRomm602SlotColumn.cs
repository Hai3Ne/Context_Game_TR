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
    public class UIRomm602SlotColumn : UISlotColumn
    {

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
                lstCells[i].setElement(UnityEngine.Random.Range(1, 11), -1);
        }

        public override void nextElement(UISlotCell cell)
        {
            if (times > 0 && times <= 3)
                cell.setElement(slotCpt.cr2ele(column, times - 1), times - 1);
            else
                cell.setElement(UnityEngine.Random.Range(1, 11), -1);// --元素的数量
        }

        public override void playAcc()
        {
            times = SlotData_602.rollAccTimes;
            duration = SlotData_602.rollAccTime;
            slotCpt.playColumnAcc(column);
        }

        public async void MovePao(int index, int count)
        {
            UIRoom602_SlotCpt slot = slotCpt as UIRoom602_SlotCpt;
            for (int i = 0; i < count*2; i++)
            {
                var go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game602/FirstRes/zidan");
       
                float dis = slot.targets[index].localPosition.y - slot.paos[index].transform.localPosition.y;
                go.transform.SetParent(slot.target);
                go.transform.position = slot.paos[index].position;
                go.SetActive(true);
                go.transform.localScale = Vector3.one;
                go.transform.DOLocalMoveY(slot.targets[index].localPosition.y, dis / 350f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    slot.targets[index].DOShakeRotation(0.1f,30,10,10);
                    CoreEntry.gGameObjPoolMgr.Destroy(go);
                });
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.15f));
          
            }
        }


        public void SetRollElement()
        {
            float tempTimes = 0.15f;
            for (int i = 0; i < 3; i++)
            {
                int index = i;
                UIRoom602SlotCell cell602 = lstCells[i] as UIRoom602SlotCell;
                cell602.ImgElement.gameObject.SetActive(false);
                cell602.Trans_Mask.gameObject.SetActive(true);
                cell602.Trans_Mask.localEulerAngles = Vector3.zero;

                Sequence seg = DOTween.Sequence();
                Tweener t1 = cell602.Trans_Mask.DOLocalRotate(new Vector3(0, 90, 0), tempTimes);
                seg.Append(t1);
                seg.InsertCallback(tempTimes, () =>
                {
                    cell602.Trans_Mask.gameObject.SetActive(false);
                    cell602.ImgElement.gameObject.SetActive(true);
                    cell602.ImgElement.transform.eulerAngles = new Vector3(0, 90, 0);
                    cell602.setElement(slotCpt.cr2ele(column, 2 - index), 2 - index);
                });
                Tweener t2 = cell602.ImgElement.transform.DOLocalRotate(new Vector3(0, 0, 0), tempTimes);
                seg.Insert(tempTimes, t2);
                seg.OnComplete(() =>
                {
                    isRolling = false;
                    onRollFinish();
                    slotCpt?.finishRoll(column);
                });
            }
        }
    }
}
