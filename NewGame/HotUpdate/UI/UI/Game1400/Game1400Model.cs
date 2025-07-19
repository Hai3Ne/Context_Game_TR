using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SEZSJ;
using System;
using System.Linq;

namespace HotUpdate
{
    [NetHandler]
    public class Game1400Model : Singleton<Game1400Model>
    {
        public int nRoomType = 1;//进入的房间类型
        public List<int> chipValues = new List<int>();
        public List<string> chipTextColor = new List<string> { "024BAF", "346921",  "BD2FF1", "F635DC",  "F44B30" , "E6B303" };
        public List<Color32> outLineColor = new List<Color32> { new Color32(253,244,23,255), new Color32(233, 206, 206, 255) , new Color32(55, 0, 0, 255) , new Color32(55, 0, 0, 255) , new Color32(0, 0, 0, 255) , new Color32(0,0, 0, 255) };

        public int selectChipType = -1;

        public long remainTimes = 15;
        /// <summary>
        /// 中奖位置
        /// </summary>
        public int winPos = 5;

        public List<MsgData> messageList = new List<MsgData>();

        public int ucModelType;// 游戏模式类型（1：下注模式 2：开奖模式）
        public long n64Jackpot;//当前奖池
        public long[] arrayAreaBet = new long[8];//区域下注列表
        public int[] arrayAreaRate;//区域概率计数列表
        public List<byte> arrayHistory;//历史数据列表
        public List<SGame12RoomPlayerInfo> arrayPlayer;//玩家信息列表

        public int ucArea_my;//自己下注区域
        public int nBet_my;// 自己当前下注;
        public long[] myBet = new long[8];//我的下注

        public int ucArea;//下注区域
        public int nBet;//当前下注
        public long n64AllBet;//当前区域所有玩家总下注

        public int ucShowArea;	// 中奖显示区域（区域值+(0-2)*8）
        public List<SGame12RoomAwardInfo> arrayAward;

        public long n64PowerGold;//倍率赢金

        public float receiveGameStartTimes = 0;
        public float receiveGameBetTimes = 0;
        public float receiveGameResultTimes = 0;

        /// <summary>
        /// 当前金币
        /// </summary>
        public long currentGold;

        public float AddSpeend = 0.009f;


        public long timeServer; //服务端时间
        public long timeClient; //客户端时间
        public long timeSyncServer; //同步用服务端时间戳
        private long timeSyncClient; //同步用客户端时间戳

       

        public int gameState;//游戏状态
        public int remainTimesLeave = 0;//离开时剩余时间
        public int betTimes = 15;//下注时长
        public int openAwardTimes = 15;//开奖时长


        //public List<SGame12RoomBetInfo> arrayBet;//广播下注

        public List<List<SGame12RoomBetInfo>> totalBroadCast = new List<List<SGame12RoomBetInfo>>();

        public List<int> rateList = new List<int>() { 40, 5, 30, 5, 20, 5, 10, 5 };
        public void InitConfig()
        {
            var config0 = ConfigCtrl.Instance.Tables.TbGame1400_BetChips.Get(nRoomType);
            chipValues.Clear();
            chipValues.Add(config0.Chips1);
            chipValues.Add(config0.Chips2);
            chipValues.Add(config0.Chips3);
            chipValues.Add(config0.Chips4);
            chipValues.Add(config0.Chips5);
            chipValues.Add(config0.Chips6);
        }

        public void InitRoomInfo(SC_GAME12_ROOM_INFO roomInfo)
        {
            totalBroadCast.Clear();
            ucModelType = roomInfo.ucModelType;
            syncServerTime(roomInfo.n64ModelTime);
            n64Jackpot = roomInfo.n64Jackpot;
            arrayAreaBet = roomInfo.arrayAreaBet;
            arrayAreaRate = roomInfo.arrayAreaRate;
            arrayHistory = roomInfo.arrayHistory.ToList();
            arrayPlayer = roomInfo.arrayPlayer;

            n64PowerGold = 0;
        }

        public void SetGAME12_BET_RET(SC_GAME12_BET_RET betRes)
        {
            ucArea_my = betRes.ucArea;
            nBet_my = betRes.nBet;
            currentGold = betRes.n64Gold;
            myBet[ucArea_my] = betRes.n64AllBet;
        }

        public void SetGAME12_BET_RET(SC_GAME12_BROADCAST_BET broadcastBet)
        {
            totalBroadCast.Add(broadcastBet.arrayBet);
            //arrayBet = broadcastBet.arrayBet;
            Message.Broadcast(MessageName.GE_BroadCastBetGameRet1400);
  
        }

        public void BroadCastAddPlayer(SC_GAME12_BROADCAST_ADD_PLAYER player)
        {
            arrayPlayer.Add(player.info[0]);
        }
        public void BroadCastDelPlayer(SC_GAME12_BROADCAST_DEL_PLAYER player)
        {
            int pos = arrayPlayer.FindIndex((a) => { return a.n64LoginGuid == player.n64LoginGuid; });
            if(pos >= 0)
                arrayPlayer.RemoveAt(pos);
        }

        public void OnGAME12_BROADCAST_BET_START(SC_GAME12_BROADCAST_BET_START start)
        {
            totalBroadCast.Clear();
            syncServerTime(start.n64ServerTime);
            n64PowerGold = 0;
            ucModelType = 1;
        }

        public void OnGAME12_BROADCAST_BET_END(SC_GAME12_BROADCAST_BET_END end)
        {
            totalBroadCast.Clear();
            int index = end.ucShowArea % 8;
            if (index == 0)
                index = 8;

            SetArrayAreaRate(index - 1);
            syncServerTime(end.n64ServerTime);
            ucShowArea = end.ucShowArea+1;
            arrayAward = end.arrayAward.ToList();
            ucModelType = 2;
            bool bTo20 = true;
            for(int i = 0;i < arrayHistory.Count;i++)
            {
                if (arrayHistory[i] == 0)
                {
                    bTo20 = false;
                    arrayHistory[i] = (byte)(ucShowArea % 8);
                }               
            }
            if(bTo20)
            {
                arrayHistory.RemoveAt(0);
                arrayHistory.Add((byte)(ucShowArea % 8));
            }
            n64Jackpot = end.n64Jackpot;
        }

        public void OnGAME12_CALCULATE(SC_GAME12_CALCULATE calculate)
        {
            n64PowerGold = calculate.n64PowerGold;
            currentGold = calculate.n64Gold;
        }      

        public void FlyChip(Transform chip,Transform area,bool bWithAni = true,Callback callBack = null)
        {
            chip.SetParent(area);
            chip.localScale = Vector3.one;
            if (bWithAni)
                chip.DOLocalMove(GetRandPointPos(area), 0.45f).OnComplete(()=> { callBack?.Invoke(); });
            else
            {
                chip.localPosition = GetRandPointPos(area);
                callBack?.Invoke();
            }
         
        }

        public Vector3 GetRandPointPos(Transform area)
        {
            RectTransform trfAddRect = area.GetComponent<RectTransform>();
            //float min_x = trfAddRect.anchoredPosition.x - trfAddRect.rect.width / 2f;
            //float max_x = trfAddRect.anchoredPosition.x + trfAddRect.rect.width / 2f;
            //float min_y= trfAddRect.anchoredPosition.y - trfAddRect.rect.width / 2f;
            //float max_y = trfAddRect.anchoredPosition.y + trfAddRect.rect.width / 2f;

            Vector3 endPos = new Vector3();
            endPos.x = UnityEngine.Random.Range(-trfAddRect.rect.width / 2f+16, trfAddRect.rect.width / 2f-16);
            endPos.y = UnityEngine.Random.Range(-trfAddRect.rect.height / 2f+16, trfAddRect.rect.height / 2f-16);
            return endPos;
        }

        int test = 1;
        public void syncTime()
        {
            //timeClient = ClientNow();
            //timeServer = timeClient - timeSyncClient + timeSyncServer;
        }

        
        public void syncServerTime(long timestamp)
        {
            timeServer = timeSyncServer = timestamp;
            timeSyncClient = timeClient;
        }

      

        private long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

        public string CalculateRate(int index)
        {
            long totalValue = 0;

            for(int i = 0;i < arrayAreaRate.Length;i++)
                totalValue += arrayAreaRate[i];

            var num1 = (Double)arrayAreaRate[index] * 100 / (float)totalValue;
            return num1.ToString("F2");

        }

        public void SetArrayAreaRate(int index)
        {
            arrayAreaRate[index]++;
        }

        public void InitData()
        {
            for(int i = 0;i < arrayAreaBet.Length;i++)
            {
                arrayAreaBet[i] = 0;
            }
        }

        public void SetArrayAreaBet(int index,long value)
        {
            arrayAreaBet[index - 1] = value;
        }

        public bool CanBet(int nBet,int ucArea)
        {
            long n64All = n64Jackpot;
            for(int i = 0;i < arrayAreaBet.Length;i++)
            {
                if (ucArea != i)
                    n64All += arrayAreaBet[i];
            }
            n64All = n64All / rateList[ucArea] - arrayAreaBet[ucArea];
            return (nBet <= n64All);
        }

        public bool CanLeave()
        {
            for(int i = 0;i < myBet.Length;i++)
            {
                if (myBet[i] > 0)
                    return false;
            }
            return true;
        }
    }
}
