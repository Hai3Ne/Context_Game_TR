/********************************************************************
生成日期:	10:7:2019   12:06
类    名: 	NetHandler
作    者:	HappLI
描    述:	该类用于注册消息函数的回调,代码需要自动生成
*********************************************************************/

using System.Collections.Generic;

namespace SEZSJ
{
    public delegate void OnRevicePacketMsg(MsgData msg);
    public class NetHandler:Singleton<NetHandler>
    {
        Dictionary<int, OnRevicePacketMsg> m_vHandles;
        //------------------------------------------------------
        public NetHandler()
        {
            m_vHandles = new Dictionary<int, OnRevicePacketMsg>();
        }
        //------------------------------------------------------
        public void Register(int code, OnRevicePacketMsg onHandler)
        {
            if (m_vHandles.ContainsKey(code)) return;
            m_vHandles.Add(code, onHandler);
        }
        //------------------------------------------------------
        public void UnRegister(int code)
        {
            m_vHandles.Remove(code);
        }
        //------------------------------------------------------
        public void OnPakckage(int id, MsgData msg)
        {
            OnRevicePacketMsg onHandler = null;
            if(m_vHandles.TryGetValue(id, out onHandler) && onHandler!=null)
            {
                onHandler(msg);
            }
            else
            {
                LogMgr.Log("NetClient.dispatchMsg Failed ,Unknow NetMessage Recevied cmd:{0}", id);
            }
        }
    }
}
