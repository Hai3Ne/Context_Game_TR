using cfg.Game;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public static class SlotData_1100
    {
        public static int column = 5;
        public static int elementFree = 17;
        public static int specialelement = 10;
        public static int elementWild = 5;
        public static int elementCount = 14;
        public static float rollTime = 0.05f;
        public static int rollTimes = 25;
        public static float rollAccTime = 0.01f;
        public static int rollAccTimes = 27;
        public static float rollElasticityTimes = 0.15f;
    }

    public class TO_Spin_1100
    {
        public List<int> Elements = new List<int>();
        public int FreeTimes = 0;
        public long WinGold = 0;
        public float rate = 0;

        public int SpecialGame = 0;
        public long LastWindGold = 0;//上一把赢的钱
        public long n64Gold = 10;//当前金币
        public long n64SunGold = 0;//太阳模式总金币
        public long n64FreeGold = 0;//免费游戏模式总金币
        public int nModelGame = 0;//特俗游戏总次数 太阳模式或免费模式
        public long n64RSPowerGold = 0;//奖池金币

    }
    public class Game1100Model : Singleton<Game1100Model>
    {
        public int nRoomType = 1;//进入的房间类型
        /// <summary>
        /// 特殊图标数量
        /// </summary>
        public int nSpecialCount;
        public int nFreeGame;
        public int nBet;
        public int nBet1;
        public long n64FreeGold;
        public long n64Jackpot;
        public List<SGame7AwardInfo> arrayAward = new List<SGame7AwardInfo>();

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        public string[] AniNames = new string[12] { "", "k", "a", "saozi", "qizi", "laba", "shoutao", "yifu", "jxz", "jkp", "scatter", "wild" };

        public TO_Spin_1100 toSpin { get; private set; } = new TO_Spin_1100(); //免费数据
        public List<int> arrayLogo = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public List<List<int>> JackPotList = new List<List<int>>();

        public List<int> GearList = new List<int>();//下注挡位

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        public bool bShowFreeAni = false;

        public List<int> elementRate = new List<int> { 150, 120, 100, 80, 70, 60, 50, 40, 30, 400 };
        public List<int> elementRate_4 = new List<int> { 30, 24, 20, 16, 14, 12, 10, 8, 6, 400 };
        public List<int> elementRate_3 = new List<int> { 15, 12, 10, 8, 7, 6, 5, 4, 3, 400 };

        public List<int> elementList = new List<int>();

        public Dictionary<int, GameObject> effectList = new Dictionary<int, GameObject>();

        public List<int> initElement = new List<int>() { 5,6,4,3,1};

        public List<Vector3> cashPos = new List<Vector3>() 
        {
            Vector3.zero,Vector3.zero,Vector3.zero,
            new Vector3(134f,459f,0), new Vector3(155f,438.7f, 0),new Vector3(174.6f, 437.4f, 0), new Vector3(186f, 437.4f, 0), new Vector3(203.2f,440.8f, 0),
            new Vector3(-31.3f,312f,0),new Vector3(-22.9f,312f,0),new Vector3(-13.6f,313.7f,0),new Vector3(-3.4f,313.7f,0),new Vector3(-3.3f,314.8f,0) ,new Vector3(8.5f,296.6f,0),

            new Vector3(-240.8f,136.9f,0), new Vector3(-237.1f,137.4f,0),new Vector3(-235.3f,137.4f,0), new Vector3(-231.8f,133.4f,0),new Vector3(-225.7f,130.6f,0),new Vector3(-223.1f,128.2f,0), new Vector3(-219.4f,-8f,0), 
        };

        public List<Vector3> cashScale = new List<Vector3>()
        {
            Vector3.zero,Vector3.zero,Vector3.zero,
            new Vector3(2.35f,1f,0), new Vector3(2.1f, 0.85f, 0),new Vector3(1.9f, 0.85f, 0), new Vector3(1.8f, 0.85f, 0), new Vector3(1.6f,0.85f, 0),
            new Vector3(1.8f,1f,0),new Vector3(1.7f,0.9f,0),new Vector3(1.6f,0.9f,0),new Vector3(1.5f,0.9f,0),new Vector3(1.5f,0.9f,0) ,new Vector3(1.35f,1.1f,0),

            new Vector3(1.55f,1f,0), new Vector3(1.5f,1f,0),new Vector3(1.46f,1f,0), new Vector3(1.4f,1f,0),new Vector3(1.32f,1f,0),new Vector3(1.28f,1f,0), new Vector3(1.22f,1.4f,0),
        };

        public List<int> specialRate = new List<int> {0,0,0,2,3,4,5,6,7,8,10,15,25,42,72,125,250,500,1000,2000,5000 };
        public long specialWinGold = 0;


        public List<int> specialElementPos = new List<int>();
        public bool bHasNewSpeiclaElement = false;
        bool bHasSpecialGame = false;

        public Dictionary<int, List<int>> gearData = new Dictionary<int, List<int>>();

        public int betID = 0;//--挡位
        /// <summary>
        /// 特殊模式倍率赢金（结算时才赋值）
        /// </summary>
        public long n64SpecialPowerGold = 0;

        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 2000, 3510, 4800, 5800, 6700, 7500, 8300, 9000, 10000 };
        /// <summary>
        /// 前面的结果
        /// </summary>
        public Dictionary<int, List<int>> lastResult = new Dictionary<int, List<int>>();

        public void Init(SC_GAME7_ROOM_INFO roomInfo)
        {
        
            nSpecialCount = roomInfo.nSpecialCount;
            bHasNewSpeiclaElement = nSpecialCount > 0;
            nBet = roomInfo.nBet *30;
            arrayAward.Clear();
            var len = roomInfo.arrayAward.Length > 10 ? 10 : roomInfo.arrayAward.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (roomInfo.arrayAward[i].n64Gold > 0)
                    arrayAward.Add(roomInfo.arrayAward[i]);// = .ToList();
            }

            arrayLogo.Clear();
            List<byte> tempResult = roomInfo.arrayLogo.ToList();

            if (tempResult.Count == 20)
            {
                arrayLogo.Add(tempResult[0]);
                arrayLogo.Add(tempResult[5]);
                arrayLogo.Add(tempResult[10]);
                arrayLogo.Add(tempResult[15]);
                arrayLogo.Add(tempResult[1]);
                arrayLogo.Add(tempResult[6]);
                arrayLogo.Add(tempResult[11]);
                arrayLogo.Add(tempResult[16]);
                arrayLogo.Add(tempResult[2]);
                arrayLogo.Add(tempResult[7]);
                arrayLogo.Add(tempResult[12]);
                arrayLogo.Add(tempResult[17]);
                arrayLogo.Add(tempResult[3]);
                arrayLogo.Add(tempResult[8]);
                arrayLogo.Add(tempResult[13]);
                arrayLogo.Add(tempResult[18]);
                arrayLogo.Add(tempResult[4]);
                arrayLogo.Add(tempResult[9]);
                arrayLogo.Add(tempResult[14]);
                arrayLogo.Add(tempResult[19]);
            }

            gearData.Clear();
            Message.Broadcast(MessageName.GAME_INIT);
        }

        public void InitConfig()
        {
           // JackPotList.Clear();
            //Dictionary<int, Game1100_JackPot> Configs = ConfigCtrl.Instance.Tables.TbGame1100_JackPot.DataMap;
            //for (int i = 1; i <= Configs.Count; i++)
            //{
            //    Game1100_JackPot config = Configs[i];
            //    int roomType = config.RoomType;
            //    if (roomType != nRoomType)
            //        continue;
            //    List<int> temp2 = new List<int>() { config.Minor, config.Major, config.Grand };
            //    JackPotList.Add(temp2);
            //}

            var config0 = ConfigCtrl.Instance.Tables.TbGame1100_Gear.Get(nRoomType);
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
                var config = ConfigCtrl.Instance.Tables.TbGame1100_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }
        }

        public void SetSpinData(SC_GAME7_BET_RET msg)
        {
            bHasNewSpeiclaElement = false;
            slotResult.Clear();
            List<byte> tempResult = msg.arrayLogo.ToList();

            if (tempResult.Count == 20)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[5]);
                slotResult.Add(tempResult[10]);
                slotResult.Add(tempResult[15]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[6]);
                slotResult.Add(tempResult[11]);
                slotResult.Add(tempResult[16]);
                slotResult.Add(tempResult[2]);
                slotResult.Add(tempResult[7]);
                slotResult.Add(tempResult[12]);
                slotResult.Add(tempResult[17]);
                slotResult.Add(tempResult[3]);
                slotResult.Add(tempResult[8]);
                slotResult.Add(tempResult[13]);
                slotResult.Add(tempResult[18]);
                slotResult.Add(tempResult[4]);
                slotResult.Add(tempResult[9]);
                slotResult.Add(tempResult[14]);
                slotResult.Add(tempResult[19]);
            }

            lines.Clear();
            List<byte> tempLine = msg.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                    lines.Add(new KeyValuePair<int, int>(i + 1, tempLine[i]));
            }
         
            n64SpecialPowerGold = msg.n64SpecialPowerGold;
            nSpecialCount = msg.nSpecialCount;
            if (nSpecialCount > 0)
                nBet = nBet1;
            toSpin.rate =  (msg.n64CommPowerGold + n64SpecialPowerGold) / ((float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
            toSpin.WinGold = msg.n64CommPowerGold ;
            toSpin.LastWindGold = toSpin.WinGold ;
            toSpin.n64Gold = msg.n64Gold;
          //  Debug.LogError("====="+ msg.n64SpecialPowerGold+"----"+ nSpecialCount+"====");
            MainUIModel.Instance.Golds = toSpin.n64Gold;

            if (lastResult.ContainsKey(betID))
            {
                lastResult[betID] = new List<int>(slotResult.ToArray());
            }
            else
                lastResult.Add(betID, new List<int>(slotResult.ToArray()));
        }



        public void RandomSpinData()
        {
            bHasNewSpeiclaElement = false;
            bHasSpecialGame = false;
            if (gearData.ContainsKey(betID))
            {
                if (gearData[betID].Count > 0)
                {
                    specialElementPos = gearData[betID];
                    nSpecialCount = gearData[betID].Count;
                }
                else
                {
                    specialElementPos.Clear();
                    nSpecialCount = 0;
                    specialWinGold = 0;
                }
            }
            else
            {
                specialElementPos.Clear();
                nSpecialCount = 0;
                specialWinGold = 0;
            }
            slotResult.Clear();
            int freeElementCount = 0;
            for (int i = 0; i < 20; i++)
            {
                int element = 1;
                int rand = UnityEngine.Random.Range(0, 10001);
                for (int k = 0; k < PRelementList.Count; k++)
                {
                    if (rand < PRelementList[k])
                    {
                        element = k + 1;
                        break;
                    }
                }
                if (element == SlotData_1100.elementFree)
                    freeElementCount++;
                slotResult.Add(element);
            }
            if (freeElementCount >= 3)
            {
                for (int i = 0; i < freeElementCount - 2; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (slotResult[j] == SlotData_1100.elementFree)
                            slotResult[j] = 3;
                    }
                }
            }

            bool bHasOneLine = UnityEngine.Random.Range(1, 5) <=2;
            int element3 = 0;
            if (bHasOneLine) //随机中1条线
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
                if (element3 == SlotData_1100.elementFree)
                    element3 = 1;
                List<int> elements = lineData[index];
                int elementCount = UnityEngine.Random.Range(3, 6);// 线上的元素
                for (int j = 0; j < elementCount; j++)
                {
                    int pos = j * 4 + elements[j] - 1;
                    slotResult[pos] = element3;
                }
            }

            lines.Clear();
            elementList.Clear();
            for (int i = 1; i <= 30; i++)
            {
                List<int> elementPos = lineData[i];
                int sameElementCount = 0;
                int element = slotResult[elementPos[0] - 1];
                for (int k = 0; k < elementPos.Count; k++)
                {
                    int pos = (k) * 4 + elementPos[k] - 1;
                    if (element == slotResult[pos])
                        sameElementCount++;
                    else
                        break;
                }

                if (sameElementCount >= 3)
                {
                    elementList.Add(element);
                    KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                    lines.Add(tempLine);
                }
            }

            if(lines.Count <= 0 || specialElementPos.Count > 0)
            {
                if(specialElementPos.Count >0)
                {
                    for (int i = 0; i < specialElementPos.Count; i++)
                        slotResult[specialElementPos[i]] = 10;
                    specialElementPos.Clear();
                }
                else
                {
                    int special = UnityEngine.Random.Range(0, 10);
                    if(special < 50)
                    {
                        specialElementPos.Clear();
                        int specialElementCount = UnityEngine.Random.Range(3, 6);
                        for (int i = 0; i < specialElementCount; i++)
                        {
                            int index = (specialElementCount + 3+i) % 5 * 4 + (i + 3) % 4;
                            slotResult[index] = 10;
                        }
                        bHasSpecialGame = true;
                    }
                }
                lines.Clear();
                elementList.Clear();
                for (int i = 1; i <= 30; i++)
                {
                    List<int> elementPos = lineData[i];
                    int sameElementCount = 0;
                    int element = slotResult[elementPos[0] - 1];
                    if (element == SlotData_1100.specialelement)
                        continue;
                    for (int k = 0; k < elementPos.Count; k++)
                    {
                        int pos = (k) * 4 + elementPos[k] - 1;
                        if (element == slotResult[pos])
                            sameElementCount++;
                        else
                            break;
                    }

                    if (sameElementCount >= 3)
                    {
                        elementList.Add(element);
                        KeyValuePair<int, int> tempLine = new KeyValuePair<int, int>(i, sameElementCount);
                        lines.Add(tempLine);
                    }
                }
            }
            if(bHasSpecialGame)
            {
                for (int i = 0; i < slotResult.Count; i++)
                {
                    if (slotResult[i] == SlotData_1100.specialelement)
                    {
                        if (specialElementPos.Contains(i) == false)
                            specialElementPos.Add(i);
                    }
                }
                specialWinGold = nBet1 * (specialElementPos.Count - 1);
            }
            if (lines.Count > 0)
            {
                toSpin.WinGold = 0;
                for (int i = 0;i < lines.Count;i++)
                {
                    int rate = 0;
                    element3 = elementList[i] - 1;
                    if (element3 > 0)
                    {
                        int count = lines[0].Value;
                        if (count == 3)
                            rate = elementRate_3[element3];
                        else if (count == 4)
                            rate = elementRate_4[element3];
                        else
                            rate = elementRate[element3];
                    }
                    else
                        rate = UnityEngine.Random.Range(1, 6);
                    toSpin.WinGold = (nBet1 * rate);
                }
                toSpin.rate = toSpin.WinGold / nBet1;
                toSpin.WinGold = toSpin.WinGold / 30;
                toSpin.n64Gold += (toSpin.WinGold - nBet1);
            }
            else
            {
                toSpin.rate = 0;
                toSpin.WinGold = 0;
                toSpin.n64Gold -= nBet1;
            }
            if(specialWinGold > 0 || toSpin.WinGold > 0)
            {
                long winGold = 0;
                if (specialElementPos.Count <= 0)
                {
                    toSpin.n64Gold += specialWinGold;
                    winGold = specialWinGold;
                }
                else
                {
                    if (specialElementPos.Count <= 0)
                        specialWinGold = 0;
                }
                toSpin.rate = (toSpin.WinGold + winGold) / nBet1;
            }
        
            if (gearData.ContainsKey(betID))
                gearData[betID] = new List<int>(specialElementPos.ToArray()) ;  
            else
                gearData.Add(betID, new List<int>(specialElementPos.ToArray()));
            PlayerPrefs.SetInt("DanJi", (int)toSpin.n64Gold);
        }

        public int HasElementNum(int ele)
        {
            int count = 0;
            for(int i = 0;i < slotResult.Count;i++)
            {
                if (slotResult[i] == ele)
                    count++;
            }
            return count;
        }

        public int HasElementNum(int col,int ele)
        {
            int count = 0;
            for (int i = 0; i < (col+1)*4; i++)
            {
                if (slotResult[i] == ele)
                    count++;
            }
            return count;
        }
    }
}
