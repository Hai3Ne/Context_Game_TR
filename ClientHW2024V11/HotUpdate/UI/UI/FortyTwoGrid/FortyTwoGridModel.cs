using SEZSJ;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace HotUpdate
{
    public class FortyTwoGridModel : Singleton<FortyTwoGridModel>
    {
        public long nAllBet = 0;
        public long Gold = 0;
        /// <summary>
        /// 奖池数量
        /// </summary>
        public long JackpotNum = 0;
        /// <summary>
        /// 玩家中奖列表
        /// </summary>
        public List<SGame15AwardInfo> awardList = new List<SGame15AwardInfo>();


        public List<FortyTwoGridRewardData> gameData = new List<FortyTwoGridRewardData>();

        public Dictionary<int, List<FortyTwoGridMultipleInfo>> FortyTwoGridMultipleDic = new Dictionary<int, List<FortyTwoGridMultipleInfo>>();
        /// <summary>
        /// 设置游戏房间信息
        /// </summary>
        /// <param name="data"></param>
        public void setRoomData(SC_GAME15_ROOM_INFO data)
        {
            JackpotNum = data.n64Jackpot;
            awardList.Clear();
            var len = data.arrayAward.Length > 10 ? 10 : data.arrayAward.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (data.arrayAward[i].n64Gold > 0)
                    awardList.Add(data.arrayAward[i]);// = .ToList();
            }

        }

        public void InitFortyTwoGridConfig()
        {
            var list = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.DataList;
            FortyTwoGridMultipleDic.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var list1 = new List<FortyTwoGridMultipleInfo>();
                var data = list[i];
                var arr = data.Interval.Split('|');
                if (arr.Length <= 1) continue;
                for (int j = 0; j < arr.Length; j++)
                {
                    var arr1 = arr[j].Split('_');

                    FortyTwoGridMultipleInfo info = new FortyTwoGridMultipleInfo();
                    info.multiple = data.Multiple;
                    info.min = int.Parse(arr1[0], new CultureInfo("en"));
                    info.times = int.Parse(arr1[1], new CultureInfo("en"));
                    list1.Add(info);
                }

                FortyTwoGridMultipleDic[data.Id] = list1;
            }
        }

        public FortyTwoGridMultipleInfo FindConfigByInterval(int id, int count)
        {
            FortyTwoGridMultipleInfo info = null;
            if (FortyTwoGridMultipleDic.ContainsKey(id))
            {
                for (int i = FortyTwoGridMultipleDic[id].Count - 1; i >= 0; i--)
                {
                    if (FortyTwoGridMultipleDic[id][i].min <= count)
                    {
                        info = FortyTwoGridMultipleDic[id][i];
                        break;
                    }
                }
            }
            return info;
        }

        /// <summary>
        /// 下注返回中奖信息
        /// </summary>
        public void setGameInfo(SC_GAME15_BET_RET data)
        {
            Gold = data.n64Gold;
            nAllBet = data.nAllBet;
            gameData.Clear();

            var dic = new Dictionary<int, int>();
            for (int i = 0; i < data.sInfo.Count; i++)
            {
                FortyTwoGridRewardData newData = new FortyTwoGridRewardData();
                newData.n64CommPowerGold = data.sInfo[i].n64CommPowerGold;
                newData.n64RSPowerGold = data.sInfo[i].n64RSPowerGold;
                newData.n64TotalGold = data.sInfo[i].n64TotalGold;
                newData.nTotalDouble = data.sInfo[i].nTotalDouble;

                dic.Clear();
                for (int j = 0; j < data.sInfo[i].arrayLogo.Length; j++)
                {
                    var id = (int)data.sInfo[i].arrayLogo[j];
                    if (!dic.ContainsKey(id))
                    {
                        dic[id] = 0;
                    }
                    dic[id]++;
                }
                newData.list = new List<FortyTwoGridRewardInfo>();
                for (int j = 0; j < data.sInfo[i].arrayLogo.Length; j++)
                {
                    FortyTwoGridRewardInfo info = new FortyTwoGridRewardInfo();
                    info.id = (int)data.sInfo[i].arrayLogo[j];
             
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(info.id);
                    if (config.Type == 1)
                    {
                        info.isShow = config.Beishu <= dic[info.id];
                    }
                    else
                    {

                        info.isShow = false;
                    }
                    newData.list.Add(info);
                }
                gameData.Add(newData);
            }
            Message.Broadcast(MessageName.GAME_ZEUS_START);


            FortyTwoGridModel.Instance.JackpotNum = data.n64Jackpot;
            Message.Broadcast(MessageName.GAME_ZEUS_RELOADUI);
        }

        /// <summary>
        /// 广播新增中奖列表
        /// </summary>
        /// <param name="info"></param>
        public void AddRoomInfoList(SGame15AwardInfo[] info)
        {
            for (int i = 0; i < info.Length; i++)
            {
                awardList.Insert(0, info[i]);
            }

            while (awardList.Count > 10)
            {
                awardList.RemoveAt(10);
            }

        }

        /// <summary>
        /// 初始化显示数据
        /// </summary>
        public void SetInitData()
        {
            var randlist = new Dictionary<int, int>();
            FortyTwoGridRewardData data = new FortyTwoGridRewardData();
            data.n64CommPowerGold = 0;
            data.n64RSPowerGold = 0;
            data.n64TotalGold = 0;
            data.nTotalDouble = 0;
            var list = new List<FortyTwoGridRewardInfo>();
            System.Random rand = new System.Random();
            for (int i = 0; i < 42; i++)
            {
                bool isNext = false;

                while (!isNext)
                {
                    isNext = true;
                    var num = rand.Next(1, 11);
                    if (!randlist.ContainsKey(num))
                    {
                        randlist[num] = 0;
                    }
                    randlist[num]++;
                    if(num <= 9)
                    {
                        if (randlist[num] >= 7)
                        {
                            isNext = false;

                        }
                        else
                        {
                            FortyTwoGridRewardInfo info = new FortyTwoGridRewardInfo();
                            info.id = num;
                            info.isShow = false;
                            list.Add(info);
                        }
                    }
                    else
                    {
                        if (randlist[num] >= 4)
                        {
                            isNext = false;

                        }
                        else
                        {
                            FortyTwoGridRewardInfo info = new FortyTwoGridRewardInfo();
                            info.id = num;
                            info.isShow = false;
                            list.Add(info);
                        }
                    }
  
                }
            }
            data.list = list;
            gameData.Add(data);
        }

        public void getServerData(long gold, int bet)
        {
            var isNext = true;
            long TotalGold = 0;
            var TotalDouble = 0;

            nAllBet = bet;
            gameData.Clear();
            var dic = new Dictionary<int, int>();
            var list = new List<int>();
            FortyTwoGridModel.Instance.JackpotNum = 4560000;
            while (isNext)
            {
                FortyTwoGridRewardData newData = new FortyTwoGridRewardData();
                newData.n64RSPowerGold = 0;


                isNext = false;

                list.Clear();
                for (int i = 0; i < 30; i++)
                {
                    list.Add(0);
                }
                if (gameData.Count > 0)
                {
                    for (int i = 0; i < gameData[gameData.Count - 1].list.Count; i++)
                    {
                        var data = gameData[gameData.Count - 1].list[i];

                        if (data.isShow)
                        {
                            list[i] = 0;
                        }
                        else
                        {
                            list[i] = data.id;
                        }
                    }

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var id = list[i];
                        if (id != 0)
                        {
                            var addNum = 24;
                            while (addNum > 0)
                            {
                                if (i + addNum < list.Count)
                                {
                                    if (list[i + addNum] == 0)
                                    {
                                        list[i + addNum] = id;
                                        list[i] = 0;
                                    }
                                }
                                addNum -= 6;
                            }

                        }
                    }

                }

                var rand1 = UnityEngine.Random.Range(0, 30);
                while (list[rand1] != 0)
                {
                    rand1 = UnityEngine.Random.Range(0, 30);
                }

                var rand2 = UnityEngine.Random.Range(0, 30);
                while (rand1 == rand2 || list[rand2] != 0)
                {
                    rand2 = UnityEngine.Random.Range(0, 30);
                }
                var newId = UnityEngine.Random.Range(1, 11);

                list[rand1] = newId;
                list[rand2] = newId;
                dic.Clear();
                var hasbeis = false;
                for (int i = 0; i < 30; i++)
                {
                    if (list[i] != 0)
                    {
                        if (!dic.ContainsKey(list[i]))
                        {
                            dic[list[i]] = 0;
                        }
                        dic[list[i]]++;
                        continue;
                    }

                    var id = 0;
                    var rand = UnityEngine.Random.Range(0, 10001);
                    if (rand == 10000 && !hasbeis)
                    {
                        TotalDouble += 10;
                        id = 20;
                        hasbeis = true;
                    }
                    else if ((rand == 8888 || rand == 6666) && !hasbeis)
                    {
                        TotalDouble += 9;
                        id = 19;
                        hasbeis = true;
                    }
                    else if ((rand == 7777 || rand == 5555 || rand == 4444) && !hasbeis)
                    {
                        TotalDouble += 8;
                        id = 18;
                        hasbeis = true;
                    }
                    else if ((rand == 1111 || rand == 2222 || rand == 3333) && !hasbeis)
                    {
                        TotalDouble += 7;
                        id = 17;
                        hasbeis = true;
                    }
                    else if ((rand == 1122 || rand == 2233 || rand == 3344 || rand == 4455) && !hasbeis)
                    {
                        TotalDouble += 6;
                        id = 16;
                        hasbeis = true;
                    }
                    else if ((rand == 5566 || rand == 6677 || rand == 7788 || rand == 8899) && !hasbeis)
                    {
                        TotalDouble += 5;
                        id = 15;
                        hasbeis = true;
                    }
                    else if ((rand == 2211 || rand == 3322 || rand == 4433 || rand == 5544) && !hasbeis)
                    {
                        TotalDouble += 4;
                        id = 14;
                        hasbeis = true;
                    }
                    else if ((rand == 6655 || rand == 7766 || rand == 8877 || rand == 9988) && !hasbeis)
                    {
                        TotalDouble += 3;
                        id = 13;
                        hasbeis = true;
                    }
                    else if ((rand == 2111 || rand == 3111 || rand == 4111 || rand == 5111
                       || rand == 6111 || rand == 7111 || rand == 8111 || rand == 9111) && !hasbeis)
                    {
                        TotalDouble += 2;
                        id = 12;
                        hasbeis = true;
                    }
                    else if (rand >= 9800)
                    {
                        id = 10;
                    }
                    else if (rand >= 9500)
                    {
                        id = 9;
                    }
                    else if (rand >= 9100)
                    {
                        id = 8;
                    }
                    else if (rand >= 8600)
                    {
                        id = 7;
                    }
                    else if (rand >= 8000)
                    {
                        id = 6;
                    }
                    else if (rand >= 6800)
                    {
                        id = 5;
                    }
                    else if (rand >= 5400)
                    {
                        id = 4;
                    }
                    else if (rand >= 3800)
                    {
                        id = 3;
                    }
                    else if (rand >= 2000)
                    {
                        id = 2;
                    }
                    else
                    {
                        id = 1;
                    }

                    list[i] = id;
                    if (!dic.ContainsKey(id))
                    {
                        dic[id] = 0;
                    }
                    dic[id]++;
                }

                float num = 0;
                foreach (var info in dic)
                {
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(info.Key);
                    if (config.Beishu <= info.Value)
                    {
                        isNext = true;
                        var data = FindConfigByInterval(info.Key, info.Value);

                        num += data.multiple  * data.times * bet / 100;
                    }
                }
                newData.n64CommPowerGold = (long)num;

                newData.nTotalDouble = TotalDouble;

                TotalGold += newData.n64CommPowerGold;
                newData.n64TotalGold = TotalGold;
                newData.list = new List<FortyTwoGridRewardInfo>();
                for (int j = 0; j < list.Count; j++)
                {
                    FortyTwoGridRewardInfo info = new FortyTwoGridRewardInfo();
                    info.id = list[j];
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(info.id);
                    if (config.Type == 1)
                    {
                        info.isShow = config.Beishu <= dic[info.id];
                    }
                    else
                    {
                        info.isShow = false;
                    }
                    newData.list.Add(info);
                }
                gameData.Add(newData);
            }
            if (TotalDouble == 0)
                TotalDouble = 1;
            Gold = gold - bet + TotalGold * TotalDouble;
            Debug.Log(isNext);

        }
    }

}

