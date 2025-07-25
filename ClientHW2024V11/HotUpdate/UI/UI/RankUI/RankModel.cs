using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HotUpdate
{
    public class RankModel : Singleton<RankModel>
    {
        public Dictionary<int, List<SCommRankData>> rankItemDic = new Dictionary<int, List<SCommRankData>>();
        //头像缓存
        public Dictionary<int, List<string>> rankItemImg = new Dictionary<int, List<string>>();
        
        //上期排行版数据
        public Dictionary<int, List<SCommRankData>> lastRankItemDic = new Dictionary<int, List<SCommRankData>>();

        public Dictionary<int, List<List<SCommRankData>>> rankItems = new Dictionary<int, List<List<SCommRankData>>>();

        public int rankType = 1;
        public Dictionary<int,long> myScoreDict = new Dictionary<int, long>();

        public int RankItemCount 
        {
            get {return ConfigCtrl.Instance.Tables.TbGameRank.DataList.FindAll(x =>x.Type== rankType).Count; }
        }

        public void GetRankData(SC_TASK_COMMRANK_CURRENT_INFO data)
        {
            if (rankItemDic.ContainsKey(data.nType))
            {
                rankItemDic[data.nType].Clear();
                rankItems[data.nType].Clear();


            }
            else{
                rankItemDic[data.nType] = new List<SCommRankData>();
                rankItems[data.nType] = new List<List<SCommRankData>>();
            }
           
            for (int i = 0; i < data.arrayInfo.Count; i++)
            {
                rankItemDic[data.nType].Add(data.arrayInfo[i]);
            }

            rankItemDic[data.nType].Sort((x, y) => 
            {
                if (y.n64Total > x.n64Total)
                    return 1;
                else
                    return -1;
            });
            if (myScoreDict.ContainsKey(data.nType)) 
            {
                myScoreDict[data.nType] = data.n64Total;
            }
            else 
            {
                myScoreDict.Add(data.nType, data.n64Total);
            }
            
        }

        public int FindMyRankData(int rankType)
        {
            if (!rankItemDic.ContainsKey(rankType))
            {
                return 0;
            }
            int rank = 0;
            for (int i = 0; i < rankItemDic[rankType].Count; i++)
            {
                if(rankItemDic[rankType][i].n64Charguid == MainUIModel.Instance.palyerData.m_i8roleID)
                {
                    rank = i + 1;
                }
            }
            return rank;
        }

        public int FindLastMyRankData(int rankType)
        {
            if (!lastRankItemDic.ContainsKey(rankType))
            {
                return 0;
            }
            int rank = 0;
            for (int i = 0; i < lastRankItemDic[rankType].Count; i++)
            {
                if (lastRankItemDic[rankType][i].n64Charguid == MainUIModel.Instance.palyerData.m_i8roleID)
                {
                    rank = i + 1;
                }
            }
            return rank;
        }

        public void GetLastRankData(SC_TASK_COMMRANK_HISTORY_INFO data) 
        {
            if (lastRankItemDic.ContainsKey(data.nType))
            {
                lastRankItemDic[data.nType].Clear();
            }
            else
            {
                lastRankItemDic[data.nType] = new List<SCommRankData>();
            }
     
          

            for (int i = 0; i < data.arrayInfo.Count; i++)
            {
                lastRankItemDic[data.nType].Add(data.arrayInfo[i]);

            }

            lastRankItemDic[data.nType].Sort((x, y) => 
            {
                if (y.n64Total > x.n64Total)
                    return 1;
                else
                    return -1;
            });
        }


    }
}
