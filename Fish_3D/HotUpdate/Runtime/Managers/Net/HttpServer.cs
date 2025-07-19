using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;


public class HttpServer : BaseNetService<HttpServer>
{
	public NetCallbackHandle mNetHandler;
	public HttpServer():base()
	{
		base.bTCP = true;
		mNetHandler = new NetCallbackHandle ();
        isCanSend = true;
        RegisterHandler(mNetHandler);
	}

	bool isCanSend = true;


	float ReConnectTime = 3f, lastConnectTime = 0f;
    float mTimeOutTime;//超时时间
    public void ResetTimeOut() {//10秒不发送则主动断开
        this.mTimeOutTime = 6;
    }
	public override void Update(float delta)
	{
        if (!IsConnected && waitSendQueue.Count > 0) {
            if (Time.realtimeSinceStartup - lastConnectTime > ReConnectTime && this.IsBeginConnect == false)
			{
				lastConnectTime = Time.realtimeSinceStartup;
				base.Connect (mIp, (ushort)mPort);
			}
		}

		if(IsConnected){
            if (waitSendQueue.Count > 0) {
                this.ResetTimeOut();
                lastConnectTime = 0;
                var cmdPack = waitSendQueue.Dequeue();
                if (LogMgr.ShowLog) {
                    if (cmdPack.cmdDatas != null) {
                        LogMgr.Log(">>>>>SendBytes [" + cmdPack.cmdDatas.Length + " byte] main=" + ((cmdPack.cmdType >> 16) & short.MaxValue) + " sub=" + (cmdPack.cmdType & 0xFFFF));
                    } else {
                        LogMgr.Log(">>>>>SendBytes main=" + ((cmdPack.cmdType >> 16) & short.MaxValue) + " sub=" + (cmdPack.cmdType & 0xFFFF));
                    }
                }
                m_TCPClient.Send(cmdPack);
            } else {
                mTimeOutTime -= delta;
                if (mTimeOutTime < 0) {
                    this.Disconnect();
                }
            }
		}
		base.Update (delta);
	}

    public void Send<S>(NetCmdType type, S s) where S : NetCmdBase {
        if (isCanSend == false)
            return;
        s.SetCmdType(type);
        SendCmdPack scp = new SendCmdPack();
        scp.cmdType = (int)type;
        scp.Cmd = s;
        scp.Hash = TypeSize<S>.HASH;
        if (!IsConnected) {
            waitSendQueue.Enqueue(scp);
            return;
        }

        this.ResetTimeOut();
        m_TCPClient.Send(scp);

        //this.Send((int)type & 0x3FFFFFFF, NetCmdHelper.CmdToBytes<S>(s, 0));
    }
	Queue<SendCmdPack> waitSendQueue = new Queue<SendCmdPack>();
	public override void Send(int cmdtype, byte[] packetBytes)
	{
		if (isCanSend == false)
			return;
		SendCmdPack cmdPack = new SendCmdPack ();
		cmdPack.cmdType = cmdtype;
		cmdPack.cmdDatas = packetBytes;
		if (!IsConnected) {
			waitSendQueue.Enqueue (cmdPack);
			return;
        }
        this.ResetTimeOut();
        if (LogMgr.ShowLog) {
            LogMgr.Log(">>>>>SendBytes [" + packetBytes.Length + " byte] main=" + (cmdtype >> 16 & 0xFFFF) + " sub=" + (cmdtype & 0xFFFF));
        }
		m_TCPClient.Send (cmdPack);
	}


	IEnumerator WaitConnect(Action cb)
	{
        if (!IsConnected && this.IsBeginConnect == false) {
            base.Connect(mIp, (ushort)mPort);
        }
		while(!base.IsConnected)
			yield return 0f;
		cb.TryCall ();
	}

	public void CloearAndClose(){
		waitSendQueue.Clear ();
		base.Disconnect ();
        //isCanSend = false;
	}

	public void Resume(){
		isCanSend = true;
	}
}


public class NetCallbackHandle : ICmdHandler {

	public bool CanProcessCmd(){
		return true;
	}
	public bool Handle(NetCmdPack cmd){
		bool ret = false;
        int type_id = cmd.cmdTypeId & int.MaxValue;//lua相关特殊处理
        if (type_id == 1) {//心跳包
            //byte[] bytes = new byte[cmd.respData.Length - NetCmdBase.HeadSize];
            //Array.Copy(cmd.respData, NetCmdBase.HeadSize, bytes, 0, bytes.Length);
            HttpServer.Instance.Send(type_id, cmd.respData);
			return false;
		}
        //if (cmdHandleFun != null) {
        //    cmdHandleFun.BeginPCall ();
        //    cmdHandleFun.Push(MainEntrace.Instance.Deseriable(type_id, new LuaByteBuffer(cmd.respData)));
        //    cmdHandleFun.PCall();
        //    ret = cmdHandleFun.CheckBoolean ();
        //    cmdHandleFun.EndPCall();
        //}
        HallHandle.Handle(cmd);
		return ret;
	}



	public void StateChanged(NetState state){

	}
}