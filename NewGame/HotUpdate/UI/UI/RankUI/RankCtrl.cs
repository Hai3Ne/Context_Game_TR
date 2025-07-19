using LitJson;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HotUpdate
{
    [NetHandler]
    public class RankCtrl : Singleton<RankCtrl>
    {
        #region 请求
        public void SendRankInfo(int type)
        {
            CS_TASK_COMMRANK_CURRENT_REQ data = new CS_TASK_COMMRANK_CURRENT_REQ();
            data.nType = type;
     
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_COMMRANK_CURRENT_REQ, data);
        }

        public void SendLastRankInfo(int type)
        {
            CS_TASK_COMMRANK_HISTORY_REQ data = new CS_TASK_COMMRANK_HISTORY_REQ();
            data.nType = type;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_COMMRANK_HISTORY_REQ, data);

        }
        #endregion

        #region 返回
        [NetResponse(NetMsgDef.SC_TASK_COMMRANK_CURRENT_INFO)]
        public void GetRankInfoResult(MsgData msgData)
        {
            SC_TASK_COMMRANK_CURRENT_INFO data = msgData as SC_TASK_COMMRANK_CURRENT_INFO;
         
            RankModel.Instance.GetRankData(data);
            RankModel.Instance.rankType = data.nType;
            Message.Broadcast(MessageName.RELOAD_RANK_UI,data.nType);
        }
        [NetResponse(NetMsgDef.SC_TASK_COMMRANK_HISTORY_INFO)]
        public void GetLastRankInfoResult(MsgData msgData)
        {
            SC_TASK_COMMRANK_HISTORY_INFO data = msgData as SC_TASK_COMMRANK_HISTORY_INFO;
  
            RankModel.Instance.GetLastRankData(data);
			RankModel.Instance.rankType = data.nType;
            Message.Broadcast(MessageName.RELOAD_LASTRANK_UI, data.nType);

        }
        #endregion
    }
}
