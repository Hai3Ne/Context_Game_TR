using cfg.Game;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public class Game700Model : Singleton<Game700Model>
    {
        public int nRoomType = 1;//进入的房间类型

        public int nModelGame;
        public int nFreeGame;
        public int nBet;
        public int nBet1;
        public long n64FreeGold;
        public long n64Jackpot;
        public long n64ExpendTotal;
        public List<SGame2AwardInfo> arrayAward = new List<SGame2AwardInfo>();

        public TO_Spin_700 toSpin { get; private set; } = new TO_Spin_700(); //免费数据

        //public game gameGearConfig;

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public List<List<int>> JackPotList = new List<List<int>>();

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        public Game700_Base_Config config;
        public List<int> GearList = new List<int>();//下注挡位
        public List<float>   BombList = new List<float>();//下注挡位

        public long GetGold = 0;//宝箱获得金币


        public int lastCount = 0;//上次免费次数
        public bool bShowFreeAni = false;

        public List<List<string>> awardList = new List<List<string>>();

        public List<int> elementRate5 = new List<int> { 75, 85, 250, 400, 550, 650, 800, 1250, 1750, 400 };
        public List<int> elementRate4 = new List<int> { 10, 10, 40, 50, 70, 80, 100, 175, 200, 50 };
        public List<int> elementRate3 = new List<int> { 3, 3, 15, 25, 30, 35, 45, 75, 100, 25 };

        public List<int> elementList = new List<int>();

        public int freeColumn = 0;
        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() { 2700, 4400, 5800, 6900, 7900, 8800, 9700, 9850, 10000 };
        public void Init(SC_GAME2_ROOM_INFO roomInfo)
        {
            lastCount = 0;
            nModelGame = roomInfo.nModelGame;
            nFreeGame = roomInfo.nFreeGame;
            toSpin.FreeTimes = nFreeGame;
            nBet = roomInfo.nBet;
            n64FreeGold = roomInfo.n64FreeGold;
            n64Jackpot = roomInfo.n64Jackpot;
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

            JackPotList.Clear();
            Dictionary<int, Game700_JackPot> Configs = ConfigCtrl.Instance.Tables.TbGame700_JackPot.DataMap;
            for (int i = 1; i <= Configs.Count; i++)
            {
                Game700_JackPot config = Configs[i];
                int roomType = config.RoomType;
                if (roomType != nRoomType)
                    continue;
                List<int> temp2 = new List<int>() { config.Minor, config.Major, config.Grand };
                JackPotList.Add(temp2);
            }
            config = ConfigCtrl.Instance.Tables.TbGame700_Base_Config.Get(nRoomType);

            lineData.Clear();
            for (int i = 1; i <= 9; i++)
            {
                List<int> temp3 = new List<int>();
                var config = ConfigCtrl.Instance.Tables.TbGame700_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }

            List<int> temp = lineData[9];

            GearList.Clear();
            GearList.Add(config.Gear1);
            GearList.Add(config.Gear2);
            GearList.Add(config.Gear3);
            GearList.Add(config.Gear4);
            if (config.Gear5 != 0)
                GearList.Add(config.Gear5);
            BombList.Clear();
            BombList.Add(config.Bomb1);
            BombList.Add(config.Bomb2);
            BombList.Add(config.Bomb3);
            awardList.Clear();
            awardList.Add(config.Award1.Split('|').ToList());
            awardList.Add(config.Award2.Split('|').ToList());
            awardList.Add(config.Award3.Split('|').ToList());
        }

        public void SetSpinData(SC_GAME2_BET_RET msg)
        {
            bShowFreeAni = false;
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

            //slotResult[0] = 10;
            //slotResult[5] = 10;
            lines.Clear();

            List<byte> tempLine = msg.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                {
                    lines.Add(new KeyValuePair<int, int>(i + 1, tempLine[i]));
                }

            }
         
            toSpin.FreeTimes = msg.nFreeGame;

            toSpin.nModelGame = msg.nModelGame;
            toSpin.n64FreeGold = msg.n64FreeGold;

            toSpin.rate = (msg.n64CommPowerGold + msg.n64RSPowerGold) / ((float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
            toSpin.n64RSPowerGold = msg.n64RSPowerGold;
            toSpin.WinGold = (msg.n64CommPowerGold + msg.n64RSPowerGold);

            toSpin.LastWindGold = toSpin.WinGold ;
            toSpin.n64Gold = msg.n64Gold ;
            MainUIModel.Instance.Golds = toSpin.n64Gold;
            Game700Model.Instance.n64Jackpot = msg.n64Jackpot;
            Message.Broadcast(MessageName.GE_BroadCast_Jackpot700);
        }

        public void OnAWARD_BOX_700(SC_GAME3_AWARD_BOX_RET msg)
        {
            n64ExpendTotal = msg.n64ExpendTotal;
            toSpin.n64Gold = msg.n64Gold;
            GetGold = msg.nGold;
        }
    }
}
