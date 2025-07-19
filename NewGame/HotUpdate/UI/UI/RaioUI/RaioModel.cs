using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg.Game;
using SEZSJ;
using System.Linq;
namespace HotUpdate
{

    public class RaioToSpin
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }
    public class RaioModel : Singleton<RaioModel>
    {
        public int RoomType = 0;
        public int nBet1;
        public List<int> slotResult = new List<int>();//结果
        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();//线数
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();
        public List<int> elementRate = new List<int> { 25, 25, 25, 30, 30, 50, 50, 50, 70, 80 };
        public bool bInSpecialGame = false;
        public RaioToSpin raioToSpin = new RaioToSpin();
        public void RandomSpinData()
        {
            slotResult.Clear();
            int freeElementCount = 0;
            for (int i = 0; i < 15; i++)
            {
                int element = UnityEngine.Random.Range(1, 14);
                if (element == RaioData.elementFree)
                    freeElementCount++;
                slotResult.Add(element);
            }
            if (freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (slotResult[j] == RaioData.elementFree)
                            slotResult[j] = 3;
                    }
                }
            }


            int element3 = 0;
            bool bHasOneLine = UnityEngine.Random.Range(1, 5) <= 2;
            if (bHasOneLine) //随机中1条线
            {
                int index = UnityEngine.Random.Range(1, 50);
                element3 = UnityEngine.Random.Range(1, 10);// 线上的元素

                if ((element3 == 12 || element3 == 14))
                    element3 = 1;
                List<int> elements = lineData[index];
                for (int j = 0; j < elements.Count; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    slotResult[pos] = element3;
                }
            }
            lines.Clear();
            for (int i = 1; i <= 50; i++)
            {
                List<int> elementPos = lineData[i];
                int sameElementCount = 0;
                int element = slotResult[elementPos[0] - 1];
                for (int k = 0; k < elementPos.Count; k++)
                {
                    int pos = (k) * 3 + elementPos[k] - 1;
                    if (element == slotResult[pos])
                        sameElementCount++;
                    else
                        break;
                }

                if (sameElementCount >= 3)
                {
                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    lines.Add(tempLine);
                    if (element == SlotData_500.elementFree)
                        raioToSpin.FreeTimes = 5;
                    else
                        raioToSpin.FreeTimes = 0;
                }
            }

            if (lines.Count > 0)
            {
                int rate = element3 > 0 ? elementRate[element3 - 1] : UnityEngine.Random.Range(1, 6);
                raioToSpin.WinGold = Game500Model.Instance.nBet1 * rate;
                raioToSpin.rate = rate;
                if (getElementCount(RaioData.specialelement) >= 5)
                {
                    bInSpecialGame = true;
                    raioToSpin.SpecialGame = 3;
                }
                Game500Model.Instance.toSpin.n64Gold += Game500Model.Instance.toSpin.WinGold;
            }
            else
            {
                raioToSpin.rate = 0;
                raioToSpin.WinGold = 0;
                raioToSpin.n64Gold -= Game500Model.Instance.nBet1;
            }

            PlayerPrefs.SetInt("DanJi", (int)raioToSpin.n64Gold);
        }

        public  int getElementCount(int ele, bool orWild = false)
        {
            int count = 0;
            if (slotResult == null)
                return count;
            for (int i = 0; i < slotResult.Count; i++)
            {
                if (slotResult[i] == ele || (orWild && slotResult[i] == RaioData.elementWild))
                    count++;
            }
            return count;
        }
    }
}