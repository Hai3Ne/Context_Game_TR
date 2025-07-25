using cfg.Game;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public class Game800Model : Singleton<Game800Model>
    {
        public int nRoomType = 1;//进入的房间类型

        public List<bool> bOneList = new List<bool>();
    

        public int nModelGame;
        public int nFreeGame;
        public int nBet;
        public int nBet1;
        public long n64FreeGold;
        public long n64Jackpot;
        public long n64ExpendTotal;
        public List<SGame5AwardInfo> arrayAward = new List<SGame5AwardInfo>();

        public TO_Spin_800 toSpin { get; private set; } = new TO_Spin_800(); //免费数据

        //public game gameGearConfig;

        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();

        public Game800_Base_Config config;
        public List<int> GearList = new List<int>();//下注挡位
        public List<int> RateList = new List<int>();//下注挡位

        public long GetGold = 0;//宝箱获得金币

        public List<string> iconAni = new List<string> {"wild1","wild2","5x","3x","7","7bar","b3","b2","b1", };
        /// <summary>
        /// 是否是免费游戏
        /// </summary>
        public bool bFreeGame = false;
        /// <summary>
        /// 免费游戏数据
        /// </summary>
        public List<SGame5HandInfo> freeGameData = new List<SGame5HandInfo>();

        public int freeGameCount = 0;

        public bool bFreeGameFinished = false;

        public bool bPlayFinished = false;

        public List<int> rate = new List<int> { 10,0,0,0,10,7,5,4,3};

        public void Init(SC_GAME5_ROOM_INFO roomInfo)
        {
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
            config = ConfigCtrl.Instance.Tables.TbGame800_Base_Config.Get(nRoomType);

            lineData.Clear();
            for (int i = 1; i <= 9; i++)
            {
                List<int> temp3 = new List<int>();
                var config = ConfigCtrl.Instance.Tables.TbGame800_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }

            GearList.Clear();
            GearList.Add(config.Gear1);
            GearList.Add(config.Gear2);
            GearList.Add(config.Gear3);
            GearList.Add(config.Gear4);
            if (config.Gear5 != 0)
                GearList.Add(config.Gear5);

            RateList.Clear();
            RateList.Add(config.Rate1);
            RateList.Add(config.Rate2);
            RateList.Add(config.Rate3);
            RateList.Add(config.Rate4);
            RateList.Add(config.Rate5);
        }

        public void SetSpinData(SC_GAME5_BET_RET msg)
        {
            bPlayFinished = false;
            bFreeGameFinished = false;
            slotResult.Clear();
            List<byte> tempResult= msg.sInfo[0].arrayLogo.ToList();
            if (tempResult.Count == 3)
            {
                slotResult.Add(tempResult[0]);
                slotResult.Add(tempResult[1]);
                slotResult.Add(tempResult[2]);
            }

            //if (slotResult[1] == 1)
            //{
            //    Debug.LogError("=============================免费游戏"+ msg.sInfo.Count);
             
            //}
            //Debug.LogError("===========================" + tempResult[0] + "=======" + tempResult[1] + "=======" + tempResult[2]);

            //slotResult[0] = 2;
            //slotResult[1] = 10;
            //slotResult[2] = 10;
            // Debug.LogError("====="+ tempResult[0]+"====="+ tempResult[1]+"====="+ tempResult[2]);

            bOneList[0] = tempResult[0] != 10;
            bOneList[1] = tempResult[1] != 10;
            bOneList[2] = tempResult[2] != 10;
            toSpin.rate = (msg.sInfo[0].n64CommPowerGold + msg.sInfo[0].n64RSPowerGold) /( (float)(msg.nAllBet == 0 ? nBet1 : msg.nAllBet));
            toSpin.WinGold = msg.sInfo[0].n64TotalGold;// msg.sInfo[0].n64CommPowerGold + msg.sInfo[0].n64RSPowerGold;
            toSpin.nHandSize = msg.nHandSize;
           // Debug.LogError("====" + msg.nHandSize+"====="+ msg.sInfo[0].n64TotalGold + "===="+ msg.sInfo[0].n64CommPowerGold+"===="+ msg.sInfo[0].n64RSPowerGold);
            toSpin.LastWindGold = toSpin.WinGold ;
            toSpin.n64Gold = msg.n64Gold ;
            toSpin.n64RSPowerGold = msg.sInfo[0].n64RSPowerGold;
            if (msg.sInfo.Count > 1)
            {
                for(int i = 1;i < msg.sInfo.Count;i++)
                {
                    SGame5HandInfo temp = new SGame5HandInfo();
                    temp.n64CommPowerGold = msg.sInfo[i].n64CommPowerGold;
                    temp.n64RSPowerGold = msg.sInfo[i].n64RSPowerGold;
                    temp.n64TotalGold = msg.sInfo[i].n64TotalGold;
                    temp.arrayLogo = msg.sInfo[i].arrayLogo;
                    freeGameData.Add(temp);
                }
                freeGameCount = freeGameData.Count;
            }
            Game800Model.Instance.n64Jackpot = msg.n64Jackpot;
            Message.Broadcast(MessageName.BroadCast_Jackpot80);
            MainUIModel.Instance.Golds = toSpin.n64Gold;

      
        }


    }
}
