using cfg.Game;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_900
    {
        public static int column = 5;
        public static int elementFree = 13;
        public static int jackpotElement = 11;
        public static int elementWild = 5;
        public static int elementCount = 14;
        public static float rollTime = 0.050f;
        public static int rollTimes = 14;
        public static float rollAccTime = 0.025f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_900
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int SpecialGame = 0;
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }
    public class Game900Model : Singleton<Game900Model>
    {
        public int nRoomType = 1;//进入的房间类型

        public int nModelGame;
        public int nFreeGame;
        public int nBet;
        public int nBet1;
        public List<long> jackpotInitVaLueList = new List<long>() { 15000, 18000, 34000, 43000, 55000, 70000, 85000, 95000, 150000, 18000 };
        public List<SJackpotInfo> jackpotList = new List<SJackpotInfo>();
        /// <summary>
        /// 进入房间的时间
        /// </summary>
        public long n64NowTimeStamp;
        /// <summary>
        /// 下注时的时间
        /// </summary>
        public long n64BetNowTimeStamp;

        /// <summary>
        /// 普通线赢金
        /// </summary>
        public long n64CommPowerGold;
        /// <summary>
        /// 中得奖池ID（奖池模式、子游戏模式共用；0代表未中奖池）
        /// </summary>
        public byte ucRSID;
        /// <summary>
        /// 奖池模式赢金
        /// </summary>
        public long n64JackPotGold;
        /// <summary>
        /// 子游戏赢金
        /// </summary>
        public long n64SubGameGold;
        /// <summary>
        /// 子游戏总倍数（0代表赢得奖池）
        /// </summary>
        public long nSubGameTotalDouble;

        public List<SGame8AwardInfo> arrayAward = new List<SGame8AwardInfo>();

        public string[] AniNames = new string[12] { "", "k", "a", "saozi", "qizi", "laba", "shoutao", "yifu", "jxz", "jkp", "scatter", "wild" };

        public TO_Spin_900 toSpin { get; private set; } = new TO_Spin_900(); //免费数据

        //public game gameGearConfig;

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        public SJackpotInfo jackpotInfo = new SJackpotInfo();

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public List<List<int>> JackPotList = new List<List<int>>();

        public List<int> GearList = new List<int>();//下注挡位

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        public int lastCount = 0;//上次免费次数

        public bool bShowFreeAni = false;

        public List<int> elementRate = new List<int> { 50, 100, 150, 200, 250, 300, 1000, 800, 6000, 500 };
        public List<int> elementRate4 = new List<int> { 10, 20, 30, 40, 50, 60, 200, 160, 120, 100 };
        public List<int> elementRate3 = new List<int> { 5, 10, 15, 20, 25, 30, 100, 80, 60, 50 };

        public List<int> elementList = new List<int>();

        public Dictionary<int, GameObject> effectList = new Dictionary<int, GameObject>();

        /// <summary>
        /// 初始时的奖池
        /// </summary>
        public long[] jackPotList = new long[10] { 15000, 18000, 34000, 43000, 55000, 70000, 85000, 95000, 150000, 18000 };
        public long[] jackSpeedPotList = new long[10] { 1500000,1500000,1500000,1500000,1500000,1500000,1500000,1500000,1500000,1500000};
        /// <summary>
        /// 下注时的奖池
        /// </summary>
        public long[] betJackPotList = new long[10] { 15000, 18000, 34000, 43000, 55000, 70000, 85000, 95000, 150000, 18000 };
        public List<int> smallGameElementList = new List<int>() {17,14,15,16,20};

        public List<byte> smallGameResults = new List<byte>();

        public long tempWinGold = 0;

        public bool bHasSmallGame = false;

        public bool bHasJackPot = false;

        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 2000, 3600, 4800, 5800, 6400, 6900, 7300, 7600, 7800,7900,8200,8500,8800,9100,9400,9700,10000 };
        /// <summary>
        /// 有免费游戏
        /// </summary>
        public bool bHasFreeGame = false;

        public void Init(SC_GAME8_ROOM_INFO roomInfo)
        {
            lastCount = 0;
            Debug.LogError("房间初始化" + roomInfo.nModelGame);
            nModelGame = roomInfo.nModelGame;
            nFreeGame = roomInfo.nFreeGame;
            toSpin.FreeTimes = nFreeGame;
            nBet = roomInfo.nBet*30;
            toSpin.n64FreeGold = roomInfo.n64FreeGold;
            jackpotList.Clear();
            jackpotList = new List<SJackpotInfo>(roomInfo.arrayJackpot.ToArray());
     
            n64NowTimeStamp = roomInfo.n64NowTimeStamp;
            CalculateJackPot(jackpotList);
            arrayAward.Clear();
            var len = roomInfo.arrayAward.Length > 10 ? 10 : roomInfo.arrayAward.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (roomInfo.arrayAward[i].n64Gold > 0)
                    arrayAward.Add(roomInfo.arrayAward[i]);// = .ToList();
            }
            Message.Broadcast(MessageName.GAME_INIT);
        }

        private void CalculateJackPot(List<SJackpotInfo> jackPotInfoList,int type = 0)
        {
            if(jackPotInfoList.Count == 10)
            {
             
                for (int i = 0;i < jackPotInfoList.Count;i++)
                {
                    jackSpeedPotList[i] = jackPotInfoList[i].nSpeed;
                    if (type == 0)
                    {
                        jackpotInitVaLueList[i] = jackPotInfoList[i].n64Jackpot;
                        jackPotList[i] = jackPotInfoList[i].n64Jackpot + 100*(n64NowTimeStamp - jackPotInfoList[i].n64TimeStamp)/ jackSpeedPotList[i];                    
                    }
                    else
                        betJackPotList[i] = jackPotInfoList[i].n64Jackpot + 100*(n64BetNowTimeStamp - jackPotInfoList[i].n64TimeStamp)/ jackSpeedPotList[i];
           
                }
            }
        }

        public void InitConfig()
        {
            var config0 = ConfigCtrl.Instance.Tables.TbGame900_Gear.Get(nRoomType);
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
                var config = ConfigCtrl.Instance.Tables.TbGame900_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }
        }
         
        public void SetSpinData(SC_GAME8_BET_RET msg)
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
            lines.Clear();

            List<byte> tempLine = msg.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                    lines.Add(new KeyValuePair<int, int>(i + 1, tempLine[i]));
            }

            toSpin.FreeTimes = msg.nFreeGame;

            toSpin.nModelGame = msg.nModelGame;
            toSpin.n64FreeGold = msg.n64FreeGold;
            n64CommPowerGold = msg.n64CommPowerGold;
            toSpin.rate =  msg.n64CommPowerGold  / (float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet);

            toSpin.n64Gold = msg.n64Gold;

            n64BetNowTimeStamp = msg.n64NowTimeStamp;
            CalculateJackPot(msg.arrayJackpot.ToList(),1);

            ucRSID = (byte)msg.ucRSID;
            n64JackPotGold = msg.n64JackPotGold;
            bHasJackPot = n64JackPotGold > 0;
            smallGameResults.Clear();
       
        
            bHasSmallGame = msg.arrayLogoSubGame[0] > 0;
            n64SubGameGold = msg.n64SubGameGold;
            nSubGameTotalDouble = msg.nSubGameTotalDouble;
            toSpin.WinGold = msg.n64CommPowerGold;


            if (msg.arrayLogoSubGame[0] > 0)
            {
                tempWinGold = toSpin.WinGold;
                if (toSpin.WinGold == 0)
                    toSpin.WinGold = nBet1 / 30;
                smallGameResults = msg.arrayLogoSubGame.ToList();
            }
            //else
            //{
            //    toSpin.WinGold += n64JackPotGold;
            //}
            //Debug.LogError("摇奖=="+toSpin.WinGold +"=====" + ucRSID + "===" + n64JackPotGold + "====" + toSpin.n64FreeGold + "----" + n64CommPowerGold);
            MainUIModel.Instance.Golds = toSpin.n64Gold;
        }



        public void RandomSpinData()
        {
            slotResult.Clear();
            int freeElementCount = 0;
            int specialGameCount = 0;
            int jackPotCount = 0;
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
                if (element == SlotData_900.elementFree)
                    freeElementCount++;
                if (element >= 14 && element <= 17)
                    specialGameCount += 1;
                if(element == SlotData_900.jackpotElement)
                {
                    jackPotCount++;
                    if (jackPotCount > 2)
                        element = 5;
                }
                slotResult.Add(element);
            }
            if(freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if(slotResult[j] == SlotData_900.elementFree)
                            slotResult[j] = 3;
                    }
                }
            }

            bool bHasOneLine = UnityEngine.Random.Range(1, 9) <= 2;
            int element3 = 0;
            bHasSmallGame =specialGameCount >= 3;
            if (specialGameCount >=3)
            {
                smallGameResults.Clear();
                for (int i = 0;i < 3;i++)
                {
                    System.Random rand1 = new System.Random(Guid.NewGuid().GetHashCode());
                    int element = smallGameElementList[rand1.Next(0, 3)];
                    smallGameResults.Add((byte)element);
                }
                n64JackPotGold = 0;
                if (smallGameResults[0] == smallGameResults[1] && smallGameResults[0] == smallGameResults[2])
                {
                    if (smallGameResults[0] == 15)
                        n64JackPotGold = (long)(0.2f * jackPotList[1]);
                    else if (smallGameResults[0] == 16)
                        n64JackPotGold = (long)(0.3f * jackPotList[2]);
                    else if (smallGameResults[0] == 17)
                        n64JackPotGold = (long)(0.4f * jackPotList[3]);
                    else if (smallGameResults[0] == 14)
                        n64JackPotGold = (long)(0.1f * jackPotList[0]);
                }
                if(smallGameResults[0] == 15 && smallGameResults[1] == 16 && smallGameResults[1] == 17)
                    n64JackPotGold = (long)(0.5f * jackPotList[4]);
                nSubGameTotalDouble = 0;
                for (int i = 0; i < smallGameResults.Count; i++)
                {
                    if (smallGameResults[i] == 14)
                        nSubGameTotalDouble += 1;
                    else if (smallGameResults[i] == 17)
                        nSubGameTotalDouble += 5;
                    else
                        nSubGameTotalDouble += smallGameResults[i] - 13;
                }
                int index = UnityEngine.Random.Range(1, 30);
                element3 = UnityEngine.Random.Range(1, 10);// 线上的元素
                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < PRelementList.Count; k++)
                {
                    if (rand < PRelementList[k])
                    {
                        element3 = k + 1;
                        break;
                    }
                }

                if (rand>= 7800)
                    element3 = 1;
                if (element3 == SlotData_900.elementFree)
                    element3 = 1;
                List<int> elements = lineData[index];
                int randomCount = UnityEngine.Random.Range(3, 6);
                for (int j = 0; j < randomCount; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    slotResult[pos] = element3;
                }
            }
            else if(bHasOneLine) //随机中1条线
            {
                int index = UnityEngine.Random.Range(1, 30);
                element3 = UnityEngine.Random.Range(1, 10);// 线上的元素
                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < PRelementList.Count; k++)
                {
                    if (rand < PRelementList[k])
                    {
                        element3 = k + 1;
                        break;
                    }
                }


                if (rand >= 7800)
                    element3 = 1;

                if (element3 == SlotData_900.elementFree)
                    element3 = 1;
                List<int> elements = lineData[index];
                int randomCount = UnityEngine.Random.Range(3, 6);
                for (int j = 0; j < randomCount; j++)
                {
                    int pos = j * 3 + elements[j] - 1;
                    slotResult[pos] = element3;
                }
            }
            specialGameCount = 0;
            for (int i = 0; i < slotResult.Count; i++)
            {
                int element = slotResult[i];
                if (element >= 14 && element <= 17)
                    specialGameCount += 1;
            }
            bHasSmallGame = specialGameCount >= 3;
            lines.Clear();
            elementList.Clear();
            for (int i = 1; i <= 30; i++)
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
                    if(element >=10)
                    {
                        slotResult[elementPos[0] - 1] = 1;
                        continue;
                    }

                    elementList.Add(element);
                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    lines.Add(tempLine);
                    if (element == SlotData_900.elementFree)
                        toSpin.FreeTimes = 5;
                    else
                        toSpin.FreeTimes = 0;
                }
            }
            if (lines.Count > 0)
            {
                toSpin.WinGold = 0;

                int rate = 0;

                
                for (int i = 0; i < lines.Count; i++)
                {
                    element3 = elementList[i] - 1;
                    int count = lines[i].Value;
                    if (count == 3)
                        rate = elementRate3[element3];
                    else if (count == 4)
                        rate = elementRate4[element3];
                    else
                        rate = elementRate[element3];
                    toSpin.WinGold += nBet1 * rate;
                  
                }
                toSpin.rate = toSpin.WinGold/ nBet1;
                toSpin.WinGold = toSpin.WinGold/30;
                toSpin.n64Gold += toSpin.WinGold;

                if (n64JackPotGold <= 0)
                {
                    if(!bHasSmallGame)
                        toSpin.n64Gold += toSpin.WinGold;
                    else
                        toSpin.n64Gold += (toSpin.WinGold * nSubGameTotalDouble);
                }
                else
                {
                    toSpin.n64Gold += toSpin.WinGold;
                    toSpin.n64Gold += n64JackPotGold;
                }
                toSpin.n64Gold -= nBet1;
            }
            else
            {
                toSpin.rate = 0;
                toSpin.WinGold = 0;
                toSpin.n64Gold -= nBet1;
            }

            PlayerPrefs.SetInt("DanJi", (int)toSpin.n64Gold);

           // Game900Model.Instance.toSpin.nModelGame = 3;
     

            //if(Game900Model.Instance.toSpin.FreeTimes> 0)
            //{
            
            //    Game900Model.Instance.toSpin.FreeTimes--;
            //    if (toSpin.FreeTimes == 0)
            //        toSpin.nModelGame = 0;
            //}
            //else
            //{
            //    Game900Model.Instance.toSpin.nModelGame = 3;
            //    Game900Model.Instance.toSpin.FreeTimes = 3;
            //}

        }
    }
}
