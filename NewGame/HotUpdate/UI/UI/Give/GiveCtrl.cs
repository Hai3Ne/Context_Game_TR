using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class GiveCtrl : Singleton<GiveCtrl>
    {
        public void sendGiveList(long id)
        {
            CS_TASK_SEND_GOLD_RECORD_INFO_REQ data = new CS_TASK_SEND_GOLD_RECORD_INFO_REQ();
            data.n64IndexID = id;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_SEND_GOLD_RECORD_INFO_REQ, data);
        }

        public void sendGiveGoldReq(long id,long gold)
        {
            CS_HUMAN_SEND_GOLD_REQ data = new CS_HUMAN_SEND_GOLD_REQ();
            data.n64ToRoleID = id;
            data.n64Gold = gold;
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_SEND_GOLD_REQ, data);
        }

        [NetResponse(NetMsgDef.SC_HUMAN_SEND_GOLD_RET)]
        public void OnRegisterGoldRet(MsgData msgData)
        {
            SC_HUMAN_SEND_GOLD_RET data = msgData as SC_HUMAN_SEND_GOLD_RET;
            if (data.cResult == 0)
            {
                GiveModel.Instance.setGiveTipsData(data);
                MainPanelMgr.Instance.ShowDialog("GiveTipsPanel");
            }
            else if(data.cResult == 1)
            {
                ToolUtil.FloattingText("请先绑定手机号码", MainPanelMgr.Instance.GetPanel("GivePanel").transform);
            }
            else
            {
                ToolUtil.FloattingText("赠送失败", MainPanelMgr.Instance.GetPanel("GivePanel").transform);
            }

        }

        [NetResponse(NetMsgDef.SC_HUMAN_SEND_GOLD_NOTE)]
        public void OnRegisterGoldNote(MsgData msgData)
        {
            SC_HUMAN_SEND_GOLD_NOTE data = msgData as SC_HUMAN_SEND_GOLD_NOTE;
   
        }

        [NetResponse(NetMsgDef.SC_TASK_SEND_GOLD_RECORD_INFO_RET)]
        public void OnRegisterGiveList(MsgData msgData)
        {
            SC_TASK_SEND_GOLD_RECORD_INFO_RET data = msgData as SC_TASK_SEND_GOLD_RECORD_INFO_RET;
            GiveModel.Instance.setGiveData(data);
            Message.Broadcast(MessageName.RELOAD_GIVE_UI);
        }
    }
}