using cfg.Game;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public class Game500Model : Singleton<Game500Model>
    {
        public int nRoomType = 1;//进入的房间类型
        public Game500_Gear gameGearConfig;

        public List<float> sunGoldLValue_1 = new List<float>();//配置的太阳值
        public Dictionary<int, List<int>> lineData = new Dictionary<int, List<int>>();
        public List<int> GearList = new List<int>();//下注挡位

        public int nBet;//服务器下发的挡位
        public int nBet1;//服务器下发的挡位
        public int nModelGame;
        public int nFreeGame;
        public long n64FreeGold;
        public int nSunGame;
        public List<byte> arraySunarrayLogo = new List<byte>();//太阳币出现的图标
        public List<byte> arraySun = new List<byte>();//在特俗游戏里的太阳值 
        public long n64Jackpot;
        public List<SGame1AwardInfo> arrayAward = new List<SGame1AwardInfo>();


        public List<int> slotResult = new List<int>();// -- SlotData.column* slotRow格元素，排列是列x行

        // line数据
        public List<KeyValuePair<int, int>> lines = new List<KeyValuePair<int, int>>();

        public TO_Spin toSpin { get; private set; } = new TO_Spin(); //免费数据

        public bool bInSpecialGame = false;


        public int lastCount = 0;

        public bool bShowFreeAni = false;

        public bool bSpecialFinish = false;

        public List<int> elementRate = new List<int>{25,25,25,30,30,50,50,50,70,80 };

        public List<int> elementRate4 = new List<int> {10,10,10,15,15,20,20,20,25,25 };

        public List<int> elementRate3 = new List<int> { 5, 5, 5, 5, 5, 10, 10, 10, 10, 10 };
        public List<int> elementList = new List<int>();
        /// <summary>
        /// 元素对应的概率   总的 435
        /// </summary>
        public List<int> PRelementList = new List<int>() {1000,2000,3000,3900,4800,5600,6400,7100,7800,8400,8800,9200,9600,10000};

        public void Init(SC_GAME1_ROOM_INFO roomInfo)
        {
            lastCount = 0;
            nModelGame = roomInfo.nModelGame;
            nFreeGame = roomInfo.nFreeGame;
            toSpin.FreeTimes = nFreeGame;
            bInSpecialGame = false;
            nSunGame = roomInfo.nSunGame;
            if (nSunGame > 0)
            {
                bInSpecialGame = true;
                toSpin.SpecialGame = nSunGame;
            }
            
            nBet = roomInfo.nBet;
        
            //Debug.LogError("挡位======"+ roomInfo.nBet+"==="+ nModelGame+"===="+ nSunGame);
            n64FreeGold = roomInfo.n64FreeGold;
            List<byte> temp = roomInfo.arrayLogo.ToList();
            arraySunarrayLogo.Clear();
            for (int i = 0;i < temp.Count;i++)
            {
                arraySunarrayLogo.Add(temp[0]);
                arraySunarrayLogo.Add(temp[5]);
                arraySunarrayLogo.Add(temp[10]);
                arraySunarrayLogo.Add(temp[1]);
                arraySunarrayLogo.Add(temp[6]);
                arraySunarrayLogo.Add(temp[11]);
                arraySunarrayLogo.Add(temp[2]);
                arraySunarrayLogo.Add(temp[7]);
                arraySunarrayLogo.Add(temp[12]);
                arraySunarrayLogo.Add(temp[3]);
                arraySunarrayLogo.Add(temp[8]);
                arraySunarrayLogo.Add(temp[13]);
                arraySunarrayLogo.Add(temp[4]);
                arraySunarrayLogo.Add(temp[9]);
                arraySunarrayLogo.Add(temp[14]);

            }
            List<byte> temp0 = roomInfo.arraySun.ToList();
            arraySun.Clear();
            for (int i = 0; i < temp.Count; i++)
            {
                arraySun.Add(temp0[0]);
                arraySun.Add(temp0[5]);
                arraySun.Add(temp0[10]);
                arraySun.Add(temp0[1]);
                arraySun.Add(temp0[6]);
                arraySun.Add(temp0[11]);
                arraySun.Add(temp0[2]);
                arraySun.Add(temp0[7]);
                arraySun.Add(temp0[12]);
                arraySun.Add(temp0[3]);
                arraySun.Add(temp0[8]);
                arraySun.Add(temp0[13]);
                arraySun.Add(temp0[4]);
                arraySun.Add(temp0[9]);
                arraySun.Add(temp0[14]);
            }
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
            gameGearConfig = ConfigCtrl.Instance.Tables.TbGame500_Gear.Get(nRoomType);
            sunGoldLValue_1.Clear();
            sunGoldLValue_1.Add(gameGearConfig.SunGold1);
            sunGoldLValue_1.Add(gameGearConfig.SunGold2);
            sunGoldLValue_1.Add(gameGearConfig.SunGold3);
            sunGoldLValue_1.Add(gameGearConfig.SunGold4);
            sunGoldLValue_1.Add(gameGearConfig.SunGold5);
            sunGoldLValue_1.Add(gameGearConfig.SunGold6);
            GearList.Clear();
            GearList.Add(gameGearConfig.Gear1);
            GearList.Add(gameGearConfig.Gear2);
            GearList.Add(gameGearConfig.Gear3);
            GearList.Add(gameGearConfig.Gear4);
            GearList.Add(gameGearConfig.Gear5);

            lineData.Clear();
            for (int i = 1; i <= 50; i++)
            {
                List<int> temp3 = new List<int>();
                var config = ConfigCtrl.Instance.Tables.TbGame500_Line_Config.Get(i);
                temp3.Add(config.Roulette1);
                temp3.Add(config.Roulette2);
                temp3.Add(config.Roulette3);
                temp3.Add(config.Roulette4);
                temp3.Add(config.Roulette5);
                lineData.Add(i, temp3);
            }
        }

        public void SetSpinData(SC_GAME1_BET_RET ret)
        {
            bShowFreeAni = false;
            bSpecialFinish = false;
            slotResult.Clear();
            List<byte> tempResult = ret.arrayLogo.ToList();
    
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

            //for (int i = 0; i < slotResult.Count; i++)
            //{
            //    Debug.Log("元素=====" + slotResult[i] + "=============" + i);
            //}


            lines.Clear();

            List<byte> tempLine = ret.arrayLine.ToList();
            for (int i = 0; i < tempLine.Count; i++)
            {
                if (tempLine[i] > 0)
                {
                    lines.Add(new KeyValuePair<int, int>(i + 1, tempLine[i]));
                }
            }

            arraySun.Clear();
            List<byte> tempSun = ret.arraySun.ToList();
            if (tempSun.Count == 15)
            {
                arraySun.Add(tempSun[0]);
                arraySun.Add(tempSun[5]);
                arraySun.Add(tempSun[10]);
                arraySun.Add(tempSun[1]);
                arraySun.Add(tempSun[6]);
                arraySun.Add(tempSun[11]);
                arraySun.Add(tempSun[2]);
                arraySun.Add(tempSun[7]);
                arraySun.Add(tempSun[12]);
                arraySun.Add(tempSun[3]);
                arraySun.Add(tempSun[8]);
                arraySun.Add(tempSun[13]);
                arraySun.Add(tempSun[4]);
                arraySun.Add(tempSun[9]);
                arraySun.Add(tempSun[14]);
            }

      

            toSpin.FreeTimes = ret.nFreeGame;
            toSpin.SpecialGame = ret.nSunGame;
    
            toSpin.nModelGame = ret.nModelGame;
           // Debug.LogError("------------" + toSpin.SpecialGame + "=====" + toSpin.nModelGame);
            // Debug.LogError("==============="+ ret.nModelGame+"===="+ ret.nSunGame);
            if (toSpin.nModelGame == toSpin.SpecialGame && toSpin.SpecialGame > 0)
                bInSpecialGame = true;
            //else if(toSpin.SpecialGame == 0)
            //    bInSpecialGame = false;


            toSpin.n64SunGold = ret.n64SunGold;// -- string.format("%.2f", num)
            toSpin.n64FreeGold = ret.n64FreeGold;
            toSpin.rate = (ret.n64CommPowerGold + ret.n64RSPowerGold) / ((float)(ret.nAllBet == 0 ? nBet1 : ret.nAllBet));
            toSpin.n64RSPowerGold = ret.n64RSPowerGold;
            toSpin.WinGold = (ret.n64CommPowerGold + ret.n64RSPowerGold);
            toSpin.LastWindGold = toSpin.WinGold;
            toSpin.n64Gold = ret.n64Gold;
            MainUIModel.Instance.Golds = toSpin.n64Gold;

           // Debug.LogError("-----" + ret.n64Jackpot);
            n64Jackpot = ret.n64Jackpot;

            CoreEntry.gEventMgr.TriggerEvent(GameEvent.Ge_BROADCAST_JACKPOT, null);

        }
      
    }
}
