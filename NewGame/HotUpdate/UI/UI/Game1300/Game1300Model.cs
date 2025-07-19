using cfg.Game;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_1300
    {
        public static int column = 5;
        public static int elementFree = 13;
        public static int wildElement = 8;
        public static int elementCount = 14;
        public static float rollTime = 0.1f;
        public static int rollTimes = 13;
        public static float rollAccTime = 0.025f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_1300
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
    public class Game1300Model : Singleton<Game1300Model>
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

        public TO_Spin_1300 toSpin { get; private set; } = new TO_Spin_1300(); //免费数据

        //public game gameGearConfig;

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        public SJackpotInfo jackpotInfo = new SJackpotInfo();

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public List<List<int>> JackPotList = new List<List<int>>();

        public List<int> GearList = new List<int>();//下注挡位

        public int lastCount = 0;//上次免费次数

        public bool bShowFreeAni = false;

        public List<int> elementRate = new List<int> { 4,8,8,15,20,25,50 };
        public List<int> elementRate4 = new List<int> {1,4,4,8,10,15,25};
        public List<float> elementRate3 = new List<float> {0.5f,2,2,3,4,5,10};

        public List<int> elementList = new List<int>();

        public Dictionary<int, GameObject> effectList = new Dictionary<int, GameObject>();



        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 3000, 4500, 6000, 7000, 8000, 9000, 10000};
        /// <summary>
        /// 有免费游戏
        /// </summary>
        public bool bHasFreeGame = false;

        public List<int> awardList = new List<int>();
        public List<int> awardCountList = new List<int>();
        /// <summary>
        /// 转盘的倍率
        /// </summary>
        public List<int> awardRate = new List<int> {100,25,3,10,50,15,2,5 };

        public int nDoublePower;

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
            arrayAward.Clear();
            var len = roomInfo.arrayAward.Length > 10 ? 10 : roomInfo.arrayAward.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (roomInfo.arrayAward[i].n64Gold > 0)
                    arrayAward.Add(roomInfo.arrayAward[i]);// = .ToList();
            }
            Message.Broadcast(MessageName.GAME_INIT);
        }



        public void InitConfig()
        {
            var config0 = ConfigCtrl.Instance.Tables.TbGame1300_Gear.Get(nRoomType);
            GearList.Clear();
            GearList.Add(config0.Gear1);
            GearList.Add(config0.Gear2);
            GearList.Add(config0.Gear3);
            GearList.Add(config0.Gear4);
            if (config0.Gear5 != 0)
                GearList.Add(config0.Gear5);
        }

        public void SetSpinData(SC_GAME9_BET_RET msg)
        {
            nBet1 = msg.nAllBet;
            nDoublePower = msg.nDoublePower;
            slotResult.Clear();
            List<byte> tempResult = msg.arrayLogo.ToList();
            if (tempResult.Count == 5)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[2]);
                slotResult.Add(tempResult[3]);
                slotResult.Add(tempResult[4]);
            }
            GetAwardElements();
            lines.Clear();
            n64CommPowerGold = msg.n64CommPowerGold;
   
            toSpin.n64Gold = msg.n64Gold;
            toSpin.WinGold = nDoublePower == 0 ? msg.n64CommPowerGold : msg.n64CommPowerGold * nDoublePower;

            toSpin.rate = toSpin.WinGold / (float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet);
            //Debug.LogError("----"+ toSpin.WinGold+"===" + nDoublePower);
            MainUIModel.Instance.Golds = toSpin.n64Gold;
        }

        public void RandomSpinData()
        {
            slotResult.Clear();
            for (int i = 0; i < 5; i++)
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
                element = 5;
                slotResult.Add(element);
            }
            GetAwardElements();
            toSpin.WinGold = 0;
            if (awardList.Count > 0)
            {
                float rate = 0;
                for (int i = 0;i < awardList.Count;i++)
                {
                    int count = GetElementCount(awardList[i]);
                    if (count == 3)
                        rate = elementRate3[awardList[i] -1];
                    else if (count == 4)
                        rate = elementRate4[awardList[i] - 1];
                    else
                        rate = elementRate[awardList[i] - 1];
                    toSpin.WinGold += (long)(nBet1 * rate);
                }
                toSpin.rate = toSpin.WinGold / nBet1;

                toSpin.rate = 20;

                int rand = UnityEngine.Random.Range(1, 8);
                Debug.LogError(">>>>>>"+ awardRate[rand]+"===="+rand);
              //  nDoublePower =  awardRate[rand];
            }
            else
            {
                nDoublePower = 0;
                toSpin.rate = 0;
                toSpin.WinGold = 0;
                toSpin.n64Gold -= nBet1;
            }

            Debug.LogError("--=--"+ awardList.Count+"==="+ toSpin.WinGold+"====="+ toSpin.rate);
            //lines.Clear();
            //elementList.Clear();    
            //if (lines.Count > 0)
            //{
            //    toSpin.WinGold = 0;

            //    int rate = 0;


            //    for (int i = 0; i < lines.Count; i++)
            //    {
            //        element3 = elementList[i] - 1;
            //        int count = lines[i].Value;
            //        if (count == 3)
            //            rate = elementRate3[element3];
            //        else if (count == 4)
            //            rate = elementRate4[element3];
            //        else
            //            rate = elementRate[element3];
            //        toSpin.WinGold += nBet1 * rate;

            //    }
            //    toSpin.rate = toSpin.WinGold/ nBet1;
            //    toSpin.WinGold = toSpin.WinGold/30;
            //    toSpin.n64Gold += toSpin.WinGold;

            //    if (n64JackPotGold <= 0)
            //    {
            //        if(!bHasSmallGame)
            //            toSpin.n64Gold += toSpin.WinGold;
            //        else
            //            toSpin.n64Gold += (toSpin.WinGold * nSubGameTotalDouble);
            //    }
            //    else
            //    {
            //        toSpin.n64Gold += toSpin.WinGold;
            //        toSpin.n64Gold += n64JackPotGold;
            //    }
            //    toSpin.n64Gold -= nBet1;
            //}
            //else
            //{
            //    toSpin.rate = 0;
            //    toSpin.WinGold = 0;
            //    toSpin.n64Gold -= nBet1;
            //}

            PlayerPrefs.SetInt("DanJi", (int)toSpin.n64Gold);

           // Game1300Model.Instance.toSpin.nModelGame = 3;
     

            //if(Game1300Model.Instance.toSpin.FreeTimes> 0)
            //{
            
            //    Game1300Model.Instance.toSpin.FreeTimes--;
            //    if (toSpin.FreeTimes == 0)
            //        toSpin.nModelGame = 0;
            //}
            //else
            //{
            //    Game1300Model.Instance.toSpin.nModelGame = 3;
            //    Game1300Model.Instance.toSpin.FreeTimes = 3;
            //}

        }


        public List<int> GetAwardElements()
        {
            awardList.Clear();
            awardCountList.Clear();
            if (slotResult[2] == SlotData_1300.wildElement)
            {
                awardList.Add(slotResult[2]);
                for (int i = 0; i < slotResult.Count; i++)
                {
                    if (GetElementCount(slotResult[i]) >= 2&& !awardList.Contains(slotResult[i]))
                    {
                        awardList.Add(slotResult[i]);
                        awardCountList.Add(GetElementCount(slotResult[i]));
                    }
                
                }
            }
            else
            {
                for (int i = 0; i < slotResult.Count; i++)
                {
                    if (GetElementCount(slotResult[i]) >= 3 && !awardList.Contains(slotResult[i]))
                    {
                        awardList.Add(slotResult[i]);
                        awardCountList.Add(GetElementCount(slotResult[i]));
                    }
                      
                }
            }
            return awardList;
        }

        private int GetElementCount(int ele)
        {
            int count = 0;
            for(int i = 0;i < slotResult.Count;i++)
            {
                if (slotResult[i] == ele)
                    count++;
            }
            return count;
        }

        public int GetDoubleIndex(int doubleIndex)
        {
            for(int i = 0;i < awardRate.Count;i++)
            {
                if (awardRate[i] == doubleIndex)
                    return i + 1;
            }
            Debug.LogError("错误");
            return 0;
        }

    }
}
