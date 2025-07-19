using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;
using cfg.Game;

namespace HotUpdate
{
    // 比武大会档次--对应配方表比武档次

    public class TournamentModel : Singleton<TournamentModel>
    {
        //本期数据
        public Dictionary<int, List<SCommArenaData>> tournamentData = new Dictionary<int, List<SCommArenaData>>();

        //上期数据
        public Dictionary<int, List<SCommArenaData>> lastTournamentData = new Dictionary<int, List<SCommArenaData>>();

        public long MyScores;
        public long nextTime;
        public int tourLevel = 1;
        public void GetTournamentData(SC_TASK_COMMARENA_CURRENT_INFO data) 
        {
            if (!tournamentData.ContainsKey(data.nLevel + data.nType * 10))
            {
                tournamentData[data.nLevel + data.nType * 10] = new List<SCommArenaData>();
            }
            else
            {
                tournamentData[data.nLevel + data.nType * 10].Clear();
            }

            for (int i = 0; i < data.arrayInfo.Count; i++)
            {
                tournamentData[data.nLevel + data.nType * 10].Add(data.arrayInfo[i]);
            }
            nextTime = data.n64Retime;
            MyScores = data.n64Total;
        }

        public void GetLastTournamentData(SC_TASK_COMMARENA_HISTORY_INFO data) 
        {
            if (!lastTournamentData.ContainsKey(data.nLevel + data.nType * 10))
            {
                lastTournamentData[data.nLevel + data.nType * 10] = new List<SCommArenaData>();
            }
            else
            {
                lastTournamentData[data.nLevel + data.nType * 10].Clear();
            }

            for (int i = 0; i < data.arrayInfo.Count; i++)
            {
                lastTournamentData[data.nLevel + data.nType * 10].Add(data.arrayInfo[i]);
            }
        }

        public GameArena FindGameArenaConfig(int type,int level)
        {
            GameArena data = null;
            var config = ConfigCtrl.Instance.Tables.TbGameArena.DataList;
            for (int i = 0; i < config.Count; i++)
            {
                if(config[i].Type == type && config[i].Levelid == level)
                {
                    data = config[i];
                    break;
                }
            }
            return data;
        }
    }

    
}

