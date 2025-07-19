using DG.Tweening;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class RollJackPot : MonoBehaviour
    {

        protected List<Transform> jackPool = new List<Transform>();
        private long lastNumCount = 0;//上次的值

        private List<Vector2> initPos = new List<Vector2>();

        float height = 0;
        private List<int> timerList = new List<int>();

        protected void OnEnable()
        {
            lastNumCount = 0;
        }

        private void OnDisable()
        {
            jackPool.Clear();
        }

        public void init()
        {
            for (int i = 0; i < transform.childCount; i++)
                jackPool.Add(transform.GetChild(i));
            for (int i = 0; i < 3; i++)
                initPos.Add(jackPool[0].GetChild(i).GetComponent<RectTransform>().localPosition);
            height = jackPool[0].GetComponent<RectTransform>().sizeDelta.y;
        }
        public void Reset()
        {
            for (int i = 0; i < jackPool.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                    jackPool[i].GetChild(j).GetComponent<RectTransform>().localPosition = initPos[j];
            }
        }

        /// <summary>
        /// 设置数值  是否是整数
        /// </summary>
        /// <param name="num"></param>
        public void SetNum(long num, bool bInteger = false)
        {
            if (jackPool.Count <= 0)
            {
                init();
            }
            Reset();
            lastNumCount = num;
            for (int i = 0; i < jackPool.Count; i++)
                jackPool[i].gameObject.SetActive(false);
            string temp = bInteger ? ToolUtil.ShowF0Num(num) : ToolUtil.ShowF2Num(num);
            for (int i = 0; i < temp.Length; i++)
                jackPool[i].gameObject.SetActive(true);
            for (int i = 0; i < temp.Length; i++)
            {
                string tempNum = temp.Substring(i, 1);
                if (tempNum != ".")
                {
                    int tempNum0 = int.Parse(tempNum, new CultureInfo("en"));
                    jackPool[i].GetChild(1).GetChild(0).GetComponent<Text>().text = tempNum0 + "";
                    jackPool[i].GetChild(0).GetChild(0).GetComponent<Text>().text = (tempNum0 + 1) + "";
                    jackPool[i].GetChild(2).GetChild(0).GetComponent<Text>().text = (tempNum0 - 1) + "";
                    if (tempNum0 == 9)
                    {
                        jackPool[i].GetChild(0).GetChild(0).GetComponent<Text>().text = "0";
                        jackPool[i].GetChild(2).GetChild(0).GetComponent<Text>().text = "8";
                    }
                    else if (tempNum0 == 0)
                    {
                        jackPool[i].GetChild(0).GetChild(0).GetComponent<Text>().text = "1";
                        jackPool[i].GetChild(2).GetChild(0).GetComponent<Text>().text = "9";
                    }
                }
                else
                {
                    jackPool[i].GetChild(0).GetChild(0).GetComponent<Text>().text = ".";
                    jackPool[i].GetChild(1).GetChild(0).GetComponent<Text>().text = ".";
                    jackPool[i].GetChild(2).GetChild(0).GetComponent<Text>().text = ".";
                }
            }
        }

        public void RollNum(long num, bool bInteger = false, bool set = false)
        {
            if (jackPool.Count <= 0)
            {
                init();
                for (int i = 0; i < jackPool.Count; i++)
                    jackPool[i].gameObject.SetActive(false);
                SetNum(num, bInteger);
                return;
            }
            for (int i = 0; i < timerList.Count; i++)
                CoreEntry.gTimeMgr.RemoveTimer(timerList[i]);
            if (lastNumCount <= 0)
                lastNumCount = num;
            SetNum(lastNumCount, bInteger);
            string tempValue = bInteger ? ToolUtil.ShowF0Num(num) : ToolUtil.ShowF2Num(num);
            int len = tempValue.Length;
            int lastNumLen = bInteger ? ToolUtil.ShowF0Num(lastNumCount).Length : ToolUtil.ShowF2Num(lastNumCount).Length;
            if (lastNumLen != len)
            {
                SetNum(num, bInteger);
                return;
            }


            timerList.Clear();
            for (int i = 0; i < len; i++)
            {
                string tempNum = tempValue.Substring(i, 1);
                if (tempNum != ".")
                {
                    string lastNum = bInteger ? ToolUtil.ShowF0Num(lastNumCount) : ToolUtil.ShowF2Num(lastNumCount);
                    string currentText = lastNum.Substring(i, 1);
                    if (currentText == tempNum)
                        continue;
                    //Debug.LogError("<<<<<<<<<<<<<<"+ tempNum);
                    int tempNum0 = int.Parse(tempNum, new CultureInfo("en"));
                    int tempNum1 = int.Parse(currentText, new CultureInfo("en"));
                    int rollTime = 0;
                    if (tempNum0 > tempNum1)
                        rollTime = tempNum0 - tempNum1;
                    else
                        rollTime = 9 - tempNum1 + tempNum0 + 1;
                    int index = i;
                    int recordTime = 0;
                    timerList.Add(100001 + index);
                    CoreEntry.gTimeMgr.AddTimer(0.16f, true, () =>
                    {
                        recordTime = recordTime + 1;
                        if (recordTime == rollTime)
                            CoreEntry.gTimeMgr.RemoveTimer(100001 + index);
                        Roll(jackPool[index], tempNum1, recordTime == rollTime);
                        tempNum1 = tempNum1 + 1;
                        if (tempNum1 > 9)
                            tempNum1 = 0;

                    }, 100001 + index);
                }
            }
            lastNumCount = num;
        }

        private void Roll(Transform jackpot, int tempNum1, bool finished = false)
        {

            for (int i = 0; i < 3; i++)
            {
                Transform jackpot1 = jackpot;
                Transform tran = jackpot.transform.GetChild(i);
                tran.DOLocalMoveY(tran.localPosition.y + height, 0.09f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    if (tran.localPosition.y > (height + 2))
                    {
                        tran.localPosition = new Vector3(tran.localPosition.x, -height, 0);
                        tran.localPosition = new Vector3(tran.localPosition.x, -height, 0);
                        if (tempNum1 + 2 > 9)
                            tran.GetChild(0).GetComponent<Text>().text = (tempNum1 + 2 - 10) + "";
                        else
                        {
                            tran.GetChild(0).GetComponent<Text>().text = (tempNum1 + 2) + "";
                            tran.GetChild(0).GetComponent<Text>().text = (tempNum1 + 2) + "";
                        }
                    }
                });
            }
        }
    }
}
