using cfg.Game;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_1000
    {
        public static int column = 5;
        public static int elementFree = 17;
        public static int specialelement = 11;
        public static int elementWild = 5;
        public static int elementCount = 20;
        public static float rollTime = 0.056f;// 0.07f;
        public static int rollTimes = 16;
        public static float rollAccTime = 0.025f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_1000
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;
        public float finaRate = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public long n64RSPowerGold = 0;//奖池金币

    }
    public class Game1000Model : Singleton<Game1000Model>
    {
        public int nRoomType = 1;//进入的房间类型
        public int nFreeGame;
        public int nBet;
        public int nBet1;
        public long n64FreeGold;
        public long n64Jackpot;
        public List<SGame11AwardInfo> arrayAward = new List<SGame11AwardInfo>();

        public string[] AniNames = new string[12] { "", "k", "a", "saozi", "qizi", "laba", "shoutao", "yifu", "jxz", "jkp", "scatter", "wild" };

        public TO_Spin_1000 toSpin { get; private set; } = new TO_Spin_1000(); //免费数据
        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        // line数据
        public List<KeyValuePair<int, List<int>>> lines = new List<KeyValuePair<int, List<int>>>();
        public List<KeyValuePair<int, int>> lines2 = new List<KeyValuePair<int, int>>();

        public List<List<int>> JackPotList = new List<List<int>>();

        public List<int> GearList = new List<int>();//下注挡位

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        public bool bShowFreeAni = false;

        public List<int> element21_25 = new List<int> { 1,2,3,4,5,9};
        public List<int> rate21_25 = new List<int> { 200,250,100,4,20,300,20 };

        public List<int> elementNormal = new List<int> { 1, 2, 3, 4, 5,6, 10 };
        public List<int> normalRate= new List<int> { 250, 5, 10, 5, 200, 7, 500 };
        public List<int> rightLines = new List<int>() {};

        public Dictionary<int, GameObject> effectList = new Dictionary<int, GameObject>();

        /// <summary>
        /// 双倍游戏能点击几次
        /// </summary>
        public int doubleGameOpenCount = 0;
        /// <summary>
        /// 特俗游戏类型
        /// </summary>
        public int nModelGame = 0;

        public List<int> smallGameElementList = new List<int>() {13,14,15,16,20};

        public bool bInSmallGame = false;

        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000,9000,10000 };


        /// <summary>
        /// 进入房间的时间
        /// </summary>
        public long n64NowTimeStamp;
        /// <summary>
        /// 下注时的时间
        /// </summary>
        public long n64BetNowTimeStamp;
        /// <summary>
        /// 初始时的奖池
        /// </summary>
        public long[] jackPotList = new long[4] { 15000, 18000, 34000, 43000 };
        public long[] jackSpeedPotList = new long[4] { 15000, 15000, 15000, 15000};
        public List<long> jackpotInitVaLueList = new List<long>() { 15000, 18000, 34000, 43000 };
        /// <summary>
        /// 下注时的奖池
        /// </summary>
        public long[] betJackPotList = new long[10] { 15000, 18000, 34000, 43000, 55000, 70000, 85000, 95000, 150000, 18000 };

        public long n64ModelGold;
        public int nRSID;
        public long n64BaseGold;
        public int nRate;
        public List<byte> arrayMagic;
        public void Init(SC_GAME11_ROOM_INFO roomInfo)
        {
            n64NowTimeStamp = roomInfo.n64NowTimeStamp;
            CalculateJackPot(new List<SJackpotInfo>(roomInfo.arrayJackpot.ToArray()));
            arrayAward.Clear();
            var len = roomInfo.arrayAward.Length > 10 ? 10 : roomInfo.arrayAward.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (roomInfo.arrayAward[i].n64Gold > 0)
                    arrayAward.Add(roomInfo.arrayAward[i]);// = .ToList();
            }
            Message.Broadcast(MessageName.GAME_INIT);
        }

        private void CalculateJackPot(List<SJackpotInfo> jackPotInfoList, int type = 0)
        {
            if (jackPotInfoList.Count == 4)
            {
                for (int i = 0; i < jackPotInfoList.Count; i++)
                {
                    jackSpeedPotList[i] = jackPotInfoList[i].nSpeed;
                    long temp = jackPotInfoList[i].n64Jackpot + 100 * (n64NowTimeStamp - jackPotInfoList[i].n64TimeStamp) / jackSpeedPotList[i];
                    if (type == 0)
                    {
                         //Debug.LogError("====="+ jackPotInfoList[i].n64Jackpot+"===="+ temp);
                        //jackpotInitVaLueList[i] = jackPotInfoList[i].n64Jackpot;
                        //Debug.LogError("---->>>===" + temp+"==="+ jackPotInfoList[i].n64Jackpot);
                        jackPotList[i] = temp;
                    }
                    else
                    {
                        long temp1 = jackPotInfoList[i].n64Jackpot + 100 * (n64BetNowTimeStamp -jackPotInfoList[i].n64TimeStamp) / jackSpeedPotList[i];
                        betJackPotList[i] = temp1;

                        //Debug.LogError("---->>>"+ temp1+"====="+ temp1);
                        jackPotList[i] = temp1;
                    }
                   
                }
            }
        }

        public void InitConfig()
        {
            var config0 = ConfigCtrl.Instance.Tables.TbGame1000_Gear.Get(nRoomType);
            GearList.Clear();
            GearList.Add(config0.Gear1);
            GearList.Add(config0.Gear2);
            GearList.Add(config0.Gear3);
            GearList.Add(config0.Gear4);
            if (config0.Gear5 != 0)
                GearList.Add(config0.Gear5);

            lineData.Clear();
            for (int i = 1; i <= 30; i++)
            {
                List<int> temp3 = new List<int>();
                var config = ConfigCtrl.Instance.Tables.TbGame1000_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }
        }

        public void SetSpinData(SC_GAME11_BET_RET msg)
        {
            slotResult.Clear();
            List<byte> tempResult = msg.arrayLogo.ToList();

            if (tempResult.Count == 15)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[5]);
                slotResult.Add(tempResult[10]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[6]);
                slotResult.Add(tempResult[11]);
                slotResult.Add(tempResult[2]);
                slotResult.Add(tempResult[7]);
                slotResult.Add(tempResult[12]);
                slotResult.Add(tempResult[3]);
                slotResult.Add(tempResult[8]);
                slotResult.Add(tempResult[13]);
                slotResult.Add(tempResult[4]);
                slotResult.Add(tempResult[9]);
                slotResult.Add(tempResult[14]);
            }
            lines2.Clear();
            List<byte> tempLine = msg.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                {
                    KeyValuePair<int, int> temp = new KeyValuePair<int, int>(i + 1, tempLine[i]);
                    lines2.Add(temp);
                }
            }
            toSpin.n64Gold = msg.n64Gold;

            n64BetNowTimeStamp = msg.n64NowTimeStamp;
            CalculateJackPot(new List<SJackpotInfo>(msg.arrayJackpot.ToArray()),1);

            nModelGame = msg.ucModel;
            n64ModelGold = msg.n64ModelGold;
            nRSID = msg.nRSID;
            n64BaseGold = msg.n64BaseGold;
            nRate = msg.nRate;
            arrayMagic = new List<byte> (msg.arrayMagic.ToArray());
            toSpin.rate = msg.n64CommPowerGold / ((float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
            toSpin.WinGold = msg.n64CommPowerGold;
            if (nModelGame == 3 || nModelGame == 4)
            {
                toSpin.WinGold = msg.n64CommPowerGold+ n64BaseGold;
                toSpin.rate = (msg.n64CommPowerGold+ n64BaseGold) / ((float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
            }

            toSpin.finaRate = (msg.n64CommPowerGold + n64ModelGold) / ((float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
       
            MainUIModel.Instance.Golds = toSpin.n64Gold;
            Message.Broadcast(MessageName.GE_BroadCast_Jackpot1000);
        }



        public void RandomSpinData()
        {
            Instance.slotResult.Clear();
            int freeElementCount = 0;
            for (int i = 0; i < 15; i++)
            {
                int element = 1;// UnityEngine.Random.Range(1, 11);
                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < PRelementList.Count; k++)
                {
                    if (rand < PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                if(element == SlotData_1000.elementFree)
                    freeElementCount++;
                Instance.slotResult.Add(element);
            }
            if (freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (slotResult[j] == SlotData_1000.elementFree)
                            slotResult[j] = 3;
                    }
                }
            }

            bool bHasOneLine = UnityEngine.Random.Range(1, 6) <= 7;


            int element3 = 0;
            int rateIndex = 0;
            if (bHasOneLine) //随机中1条线
            {
                int index =  UnityEngine.Random.Range(1, 30);
                List<int> elements = lineData[index];
                if(index >= 21 && index <= 25)
                {
                    int rand = UnityEngine.Random.Range(0, 6);
                    rateIndex = rand;
                    element3 = 2;// element21_25[rand];
                    for (int i=0;i < 3;i++)
                        slotResult[3 * (index - 21) + i] = element3;
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, 7);
                    rateIndex = rand;
                    element3 = 2;// elementNormal[rand];
                    for (int j = 0; j < 3; j++)
                    {
                        int pos = j * 3 + elements[j] - 1;
                        slotResult[pos] = element3;
                    }
                }
            }

            lines.Clear();
            for (int i = 1; i <= 30; i++)
            {
                if (i >= 21 && i <= 25)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if(slotResult[3 * (i - 21)] == slotResult[3 * (i - 21)+1] && slotResult[3 * (i - 21)] == slotResult[3 * (i - 21) + 2] && element21_25.Contains(slotResult[3 * (i - 21) + 2]))
                        {
                            KeyValuePair<int, List<int>> tempLine = new KeyValuePair<int, List<int>>(i,new List<int> { 3 * (i - 21), 3 * (i - 21)+1, 3 * (i - 21)+2 });
                            lines.Add(tempLine);
                        }
                    }
                }
                else
                {
                    List<int> elementPos = lineData[i];
                    int sameElementCount = 0;
                    int element = 0;
                    List<int> tempList = new List<int>();
                    if (rightLines.Contains(i))
                    {
                        element = slotResult[elementPos[4] - 1];

                        int randomCount = UnityEngine.Random.Range(2, 4);
                        for (int k = 4; k >= (4 - randomCount ); k--)
                        {
                            int pos = (k) * 3 + elementPos[k] - 1;
                            if (element == slotResult[pos])
                                sameElementCount++;
                            else
                                break;
                        }
                    }
                    else
                    {
                        element = slotResult[elementPos[0] - 1];
         
                        //int randomCount = UnityEngine.Random.Range(3, 6);
                        for (int k = 0; k < 5; k++)
                        {
                            int pos = (k) * 3 + elementPos[k] - 1;
                            if (element == slotResult[pos])
                            {
                                sameElementCount++;
                                tempList.Add(pos);
                            }
                            else
                                break;
                            if(sameElementCount == 3)
                            {
                                if (!elementNormal.Contains(element))
                                {
                                    int random = UnityEngine.Random.Range(0, 7);
                                    rateIndex = random;
                                    for (int m = 0; m < 3; m++)
                                    {
                                        int pos1 = (m) * 3 + elementPos[m] - 1;
                                        slotResult[pos1] = elementNormal[random];
                                    }
                                }
                            }
                            if(sameElementCount >=4)
                            {
                                slotResult[pos] = element + 1 >10?1: element + 1;
                                sameElementCount = 3;
                                tempList.RemoveAt(tempList.Count -1);
                            }
                        }
                    }

                    if (sameElementCount >= 3)
                    {
                        KeyValuePair<int, List<int>> tempLine = new KeyValuePair<int, List<int>>(i, tempList);
                        lines.Add(tempLine);
                        if (element == SlotData_1000.elementFree)
                            toSpin.FreeTimes = 5;
                        else
                            toSpin.FreeTimes = 0;
                    }
                }
            }
            
            if (lines.Count > 0)
            {
                toSpin.WinGold = 0;
                if (lines.Count >= 2)
                {
                    RandomSpinData();
                    return;
                }
                for (int i = 0; i < lines.Count; i++)
                {
                    int rate = 0;


                    if (element3 > 0)
                    {
                        if(i>=21 && i <=25)
                            rate = rate21_25[rateIndex];
                        else
                            rate = normalRate[rateIndex];
                    }
                    else
                        rate = UnityEngine.Random.Range(1, 6);
                    toSpin.WinGold += nBet1 * rate;
                    toSpin.rate = rate;
                }

                doubleGameOpenCount = UnityEngine.Random.Range(1,5);
                toSpin.WinGold = toSpin.WinGold;
                toSpin.n64Gold += (toSpin.WinGold - nBet1);
                nModelGame = 0;// UnityEngine.Random.Range(1, 5);
            }
            else
            {
                nModelGame = 0;
                int element3Count = 0;
                int element10Count = 0;
                for (int i = 1;i <= lineData.Count; i++)
                {
                    element3Count = 0;
                    element10Count = 0;
                    if (i >= 21 && i <= 25)
                        continue;
                    List<int> elementPos = lineData[i];
                    for(int j = 0;j < elementPos.Count;j++)
                    {
                        int pos = j * 3 + elementPos[j] - 1;
                        if (slotResult[pos] == 3)
                            element3Count++;
                        if (slotResult[pos] == 10)
                            element10Count++;
                    }
                    if(element3Count >=3 || element10Count >= 3)
                        RandomSpinData();
                }
                toSpin.rate = 0;
                toSpin.WinGold = 0;
                toSpin.n64Gold -= nBet1;
            }
            PlayerPrefs.SetInt("DanJi", (int)toSpin.n64Gold);
        }
        public int GetElementByLine(int line)
        {
            return ConfigCtrl.Instance.Tables.TbGame1000_AwardLine.Get(line).Element;
        }

        public int GetLineTypeByLine(int line)
        {
            return ConfigCtrl.Instance.Tables.TbGame1000_AwardLine.Get(line).Type;
        }

        /// <summary>
        /// 78线以上是3个元素
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public List<int> GetElementPosByLine(int line)
        {
            string temp = ConfigCtrl.Instance.Tables.TbGame1000_AwardLine.Get(line).AwardPos;
            int lineType = Game1000Model.Instance.GetLineTypeByLine(line);
            string[] tempPos = temp.Split('#');
            if (lineType == 0)
                return new List<int> { int.Parse(tempPos[0], new CultureInfo("en")) };
            else
                return new List<int> { int.Parse(tempPos[0], new CultureInfo("en")), int.Parse(tempPos[1], new CultureInfo("en")), int.Parse(tempPos[2], new CultureInfo("en")), int.Parse(tempPos[3], new CultureInfo("en")), int.Parse(tempPos[4], new CultureInfo("en")), };
        }
    }
}
