using UnityEngine;
using System.Collections;

using System;

public class NetServices : BaseNetService<NetServices> {
	public NetServices():base()
	{
		base.bTCP = true;
	}

	bool mIsValide = true;
	public bool Enabled {
		set {
			mIsValide = value;
		}
	}



	protected override bool FilterNetPack(NetCmdPack nc) {
        if (nc.cmdTypeId == 1 || nc.cmdTypeId == ((1 << 31) | 1)) {//心跳包逻辑处理
            //byte[] bytes = new byte[nc.respData.Length - NetCmdBase.HeadSize];
            //Array.Copy(nc.respData, NetCmdBase.HeadSize, bytes, 0, bytes.Length);
            this.Send(nc.cmdTypeId & int.MaxValue, nc.respData);
            return false;
        //} else if((nc.cmdTypeId & int.MaxValue) >> 16 != 200){
        //    LuaTable luaPack = MainEntrace.Instance.Deseriable(nc.cmdTypeId & int.MaxValue, new LuaByteBuffer(nc.respData));
        //    if (luaPack != null) {
        //        HttpServer.Instance.mNetHandler.Handle(luaPack);
        //        CallLuaCmdhandle(luaPack);
        //    }
        }
		return base.FilterNetPack (nc);
	}
	void M_TCPClient_OnHeartBeat ()
	{
		FishNetAPI.Instance.SendClockSync ();
	}

    public void ConnectServer(System.Action cb, System.Action error)
	{
        MonoDelegate.Instance.StartCoroutine(WaitConnect(cb, error));
	}

    private void OnConnectError() {
        m_TCPClient = null;
        SystemMessageMgr.Instance.DialogShow(StringTable.GetString("Tips_NetworkError"), () => {
            SceneLogic.Instance.Notifiy(SysEventType.FishRoomFail);
            SceneLogic.Instance.BackToHall();
        });
    }
	public override bool Connect (string ip, ushort port)
	{
        if (m_TCPClient != null) {
            m_TCPClient.OnHeartBeat -= M_TCPClient_OnHeartBeat;
            m_TCPClient.OnConnectError -= OnConnectError;
        }
		bool ret = base.Connect (ip, port);		
		m_TCPClient.OnHeartBeat += M_TCPClient_OnHeartBeat;
        m_TCPClient.OnConnectError += OnConnectError;
        if (ret == false) {
            m_TCPClient.OnHeartBeat -= M_TCPClient_OnHeartBeat;
            m_TCPClient.OnConnectError -= OnConnectError;
            m_TCPClient = null;
        }
		return ret;
	}
	IEnumerator WaitConnect(System.Action cb,Action error)
	{
        if (!IsConnected) {
            if (Connect(mIp, (ushort)mPort) == false) {
                error.TryCall();
                yield break;
            }
        }
        while (!base.IsConnected && m_TCPClient != null)
			yield return null;
        if (base.IsConnected) {
            cb.TryCall();
        } else {
            error.TryCall();
        }
	}

    public void Send<S>(NetCmdBase ncb, bool refersh_exit_time) {//refersh_exit_time:是否刷新操作时间
        if (mIsValide) {
            if (refersh_exit_time) {
                SceneLogic.Instance.ResetExitTime();
            }
            base.Send<S>(ncb);
        }
	}
    public void Send<S>(NetCmdType type, S s) where S : NetCmdBase{
        s.SetCmdType(type);
        this.Send<S>(s);
    }
	public override void Send<S>(NetCmdBase ncb)
	{
        if (mIsValide) {
            SceneLogic.Instance.ResetExitTime();
            base.Send<S>(ncb);
        }
	}

	public override void Send (int cmdtype, byte[] packetBytes)
	{
		if (mIsValide) {
			SendCmdPack cmdPack = new SendCmdPack ();
			cmdPack.cmdType = cmdtype;
			cmdPack.cmdDatas = packetBytes;
			if (!IsConnected) {
				return;
			}
            if (LogMgr.ShowLog) {
                LogMgr.Log(">>>>>SendBytes [" + packetBytes.Length + " byte] main=" + (cmdtype >> 16 & 0xFFFF) + " sub=" + (cmdtype & 0xFFFF));
            }
			m_TCPClient.Send (cmdPack);
		}
	}

	public void ForceSend<S>(NetCmdBase ncb)
	{
		base.Send<S> (ncb);
	}

    public override void Send<S>(NetCmdType type, S ncb, System.Action<NetCmdPack> callback) {
        if (mIsValide)
            base.Send<S>(type,ncb, callback);
	}
}
