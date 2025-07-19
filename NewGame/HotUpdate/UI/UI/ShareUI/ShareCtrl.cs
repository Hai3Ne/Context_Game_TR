using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class ShareCtrl : Singleton<ShareCtrl>
    {
        #region 请求
        /// <summary>
        /// 请求推广记录
        /// </summary>
        public void SendCS_TASK_EXPAND_INFO_REQ()
        {
            CS_TASK_EXPAND_INFO_REQ data = new CS_TASK_EXPAND_INFO_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_EXPAND_INFO_REQ);
        }

        /// <summary>
        /// 推广兑换金币
        /// </summary>
        /// <param name="gold"></param>
        public void SendCS_TASK_EXPAND_EXTRACT_REQ(long gold)
        {
            CS_TASK_EXPAND_EXTRACT_REQ data = new CS_TASK_EXPAND_EXTRACT_REQ();
            data.n64Gold = gold;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_EXPAND_EXTRACT_REQ, data);
        }
        #endregion

        #region 接收
        [NetResponse(NetMsgDef.SC_TASK_EXPAND_INFO_RET)]
        public void OnSC_TASK_EXPAND_INFO_RETResult(MsgData msg)
        {
            SC_TASK_EXPAND_INFO_RET data = msg as SC_TASK_EXPAND_INFO_RET;
            ShareModel.Instance.rankList.Clear();
            foreach (var item in data.arrayRank)
            {
                ShareModel.Instance.rankList.Add(item);
            }
            //排序
            ShareModel.Instance.rankList = ShareModel.Instance.rankList.OrderByDescending(x => x.nLower).ThenByDescending(y => y.n64Gold).ToList();
            ShareModel.Instance.shareData = data;
            Message.Broadcast(MessageName.GAME_RELOADSHARE_OPENUI);
            Debug.Log($"{data.n64ExpandGold}");
        }
        [NetResponse(NetMsgDef.SC_TASK_EXPAND_EXTRACT_RET)]
        public void OnSC_TASK_EXPAND_EXTRACT_RETResult(MsgData msg)
        {
            SC_TASK_EXPAND_EXTRACT_RET data = msg as SC_TASK_EXPAND_EXTRACT_RET;
            if (data.nResult == 1)
            {
                Debug.Log($"{data.nResult}");
                ShareModel.Instance.shareData.n64ExpandGold = 0;
                ShareModel.Instance.shareData.n64ExtractGold = data.n64ExtractGold;
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum(data.n64Gold / ToolUtil.GetGoldRadio(), delegate
                {
                    Message.Broadcast(MessageName.REFRESH_SHARE_PANEL);
                });
            }


        }
        #endregion
    }
}
