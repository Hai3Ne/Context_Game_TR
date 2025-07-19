using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class TournamentCtrl : Singleton<TournamentCtrl>
    {
        #region 请求
        public void SendTournamentInfo(int level,int type)
        {
            CS_TASK_COMMARENA_CURRENT_REQ data = new CS_TASK_COMMARENA_CURRENT_REQ();
            data.nType = type;
            data.nLevel = level;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_COMMARENA_CURRENT_REQ, data);
        }

        public void SendLastTournamentnfo(int level,int type)
        {
            CS_TASK_COMMARENA_HISTORY_REQ data = new CS_TASK_COMMARENA_HISTORY_REQ();
            data.nType = type;
            data.nLevel = level;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_COMMARENA_HISTORY_REQ, data);

        }
        #endregion

        #region 返回
        [NetResponse(NetMsgDef.SC_TASK_COMMARENA_CURRENT_INFO)]
        public void GetTournamentInfoResult(MsgData msgData)
        {
            SC_TASK_COMMARENA_CURRENT_INFO data = msgData as SC_TASK_COMMARENA_CURRENT_INFO;

            TournamentModel.Instance.GetTournamentData(data);
            Message.Broadcast(MessageName.RELOAD_TOURNAMENTCTRL_UI,data.nLevel);
        }
        [NetResponse(NetMsgDef.SC_TASK_COMMARENA_HISTORY_INFO)]
        public void GetLastTournamentInfoResult(MsgData msgData)
        {
            SC_TASK_COMMARENA_HISTORY_INFO data = msgData as SC_TASK_COMMARENA_HISTORY_INFO;
            TournamentModel.Instance.GetLastTournamentData(data);
   
            Message.Broadcast(MessageName.RELOAD_TOURNAMENTCTRLLAST_UI, data.nLevel);
        }
        #endregion
    }
}
