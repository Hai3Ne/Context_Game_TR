using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class Game900Ctrl : Singleton<Game900Ctrl>
    {


        public void Send_CS_GAME8_BET_REQ(int nBet)
        {
            //Debug.LogError("---"+Time.realtimeSinceStartup);

            CS_GAME8_BET_REQ data = new CS_GAME8_BET_REQ();
            data.nBet = nBet;
            NetMgr.netMgr.send(NetMsgDef.CS_GAME8_BET_REQ, data);
        }

        #region
        [NetResponse(NetMsgDef.SC_GAME8_ROOM_INFO)]
        public void OnEnterRoomRet_900(MsgData msgData)
        {
            SC_GAME8_ROOM_INFO data = (SC_GAME8_ROOM_INFO)msgData;
            Game900Model.Instance.Init(data);
            //Debug.LogError("房间信息===============");
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }

        [NetResponse(NetMsgDef.SC_GAME8_BET_RET)]
        public void OnGAME8_BET_RET(MsgData msgData)
        {
            SC_GAME8_BET_RET data = (SC_GAME8_BET_RET)msgData;
            Game900Model.Instance.SetSpinData(data);
            Message.Broadcast(MessageName.GE_BetGameRet900);
            // CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_BetGameRet500, null);
        }




        [NetResponse(NetMsgDef.SC_GAME8_BROADCAST_ADD_AWARD)]
        public void GAME8_BROADCAST_ADD_AWARD(MsgData msgData)
        {
            SC_GAME8_BROADCAST_ADD_AWARD data = (SC_GAME8_BROADCAST_ADD_AWARD)msgData;
            SGame8AwardInfo jackpotList = data.info[0];
            if (Game900Model.Instance.arrayAward.Count >= 10)
                Game900Model.Instance.arrayAward.RemoveAt(9);

            SGame8AwardInfo temp = new SGame8AwardInfo();
            temp.n64RoleID = jackpotList.n64RoleID;
            temp.nIconID = jackpotList.nIconID;
            temp.n64Gold = jackpotList.n64Gold;
            temp.szName = jackpotList.szName;
            temp.ucRSID = jackpotList.ucRSID;
            SJackpotInfo temp2 = new SJackpotInfo() { n64Jackpot = jackpotList.sJackpot[0].n64Jackpot, n64TimeStamp = jackpotList.sJackpot[0].n64TimeStamp };
            temp.sJackpot[0] = temp2;
            Game900Model.Instance.arrayAward.Insert(0, temp);

            //Debug.LogError("-----"+ Encoding.Default.GetString(temp.szName) +"====" + Encoding.Default.GetString(MainUIModel.Instance.palyerData.m_roleName));

            //uid.Substring(uid.Length - 4, 4)


            //Debug.LogError("---"+(Encoding.Default.GetString(temp.szName) == Encoding.Default.GetString(MainUIModel.Instance.palyerData.m_roleName)));
            if(Encoding.Default.GetString(temp.szName) == Encoding.Default.GetString(MainUIModel.Instance.palyerData.m_roleName))
            {
                CoreEntry.gTimeMgr.AddTimer(temp.ucRSID <= 5?7:3, false,()=> 
                {
                    Message.Broadcast(MessageName.GE_BroadCast_JackpotList900);
                },6);
            }
            else
                Message.Broadcast(MessageName.GE_BroadCast_JackpotList900);

        }

        #endregion

    }
}
