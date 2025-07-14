using UnityEngine;
using System.Collections.Generic;

using System.Net;
using System;
using System.IO;
public class NetCmdPack
{
    public NetCmdBase  cmd;
	public byte[] respData;
	public int cmdTypeId;
    public uint tickSpan;
    public uint localTick;
    public bool IsFull = false;
    public NetCmdBase _pack;
    public T ToObj<T>() where T : NetCmdBase {//序列化对象  新模式专用
        if (this._pack == null) {
            this._pack = (NetCmdBase)TypeReflector.BytesToObj(
                Utility.GetHash(typeof(T).ToString()),
                this.respData,
                0,
                this.respData.Length,
                3
            );
            if (this._pack != null) {
                this._pack.SetCmdType((NetCmdType)this.cmdTypeId);
            }
        }
        return this._pack as T;
    }

    public CMD_S_SwitchScene_lkpy GetSwitchScene_klpy() {//李逵劈鱼切换场景
        CMD_S_SwitchScene_lkpy cmd = new CMD_S_SwitchScene_lkpy();
        using (System.IO.MemoryStream ms = new MemoryStream(this.respData)) {
            using (BinaryReader bin = new BinaryReader(ms)) {
                cmd.MainCmd = bin.ReadUInt16();
                cmd.SubCmd = bin.ReadUInt16();
                cmd.scene_kind = bin.ReadInt32();
                cmd.bg_id = bin.ReadInt32();
                cmd.fish_count = bin.ReadInt32();
                cmd.fish_kind = new int[cmd.fish_count];
                cmd.fish_id = new int[cmd.fish_count];
                for (int i = 0; i < cmd.fish_count; i++) {
                    cmd.fish_kind[i] = bin.ReadInt32();
                }
                for (int i = 0; i < cmd.fish_count; i++) {
                    cmd.fish_id[i] = bin.ReadInt32();
                }
                cmd.tick_count = bin.ReadUInt32();
            }
        }
        this._pack = cmd;
        return cmd;
    }

    public CMD_S_CatchSweepFishResult_lkpy GetCatchSweepFishResult() {//特殊鱼捕获结果
        CMD_S_CatchSweepFishResult_lkpy cmd = new CMD_S_CatchSweepFishResult_lkpy();
        using (System.IO.MemoryStream ms = new MemoryStream(this.respData)) {
            using (BinaryReader bin = new BinaryReader(ms)) {
                cmd.MainCmd = bin.ReadUInt16();
                cmd.SubCmd = bin.ReadUInt16();
                cmd.chair_id = bin.ReadUInt16();
                cmd.fish_id = bin.ReadInt32();
                cmd.fish_kind = bin.ReadInt32();
                cmd.fish_score = bin.ReadInt64();
                cmd.sweep_score = bin.ReadInt64();
                cmd.catch_fish_count = bin.ReadInt32();
                cmd.delay_stunt = bin.ReadSingle();
                cmd.catch_fish_id = new int[cmd.catch_fish_count];
                cmd.catch_fish_score = new int[cmd.catch_fish_count];
                for (int i = 0; i < cmd.catch_fish_count; i++) {
                    cmd.catch_fish_id[i] = bin.ReadInt32();
                }
                for (int i = 0; i < cmd.catch_fish_count; i++) {
                    cmd.catch_fish_score[i] = bin.ReadInt32();
                }
            }
        }
        this._pack = cmd;
        return cmd;
    }
}

public class BaseNetService<T>:SingleTon<T>, INetHandler, IRunUpdate where T : class, new()
{
    protected INetClient  m_TCPClient;
	List<ICmdHandler> m_HandlerList = new List<ICmdHandler>() ;
	protected Queue<NetCmdPack> m_CmdList = new Queue<NetCmdPack>();

	public BaseNetService(){}
	protected string mIp;
	protected ushort mPort;
	public string IP { get{ return mIp;} set{ mIp = value;}}
	public ushort Port { get{ return mPort;} set{ mPort = value;}}


	public System.Action<string,int> OnError;
    public void GlobalInit()
    {   
		m_HandlerList.Clear ();
    }


	public void RegisterHandler(ICmdHandler cmdHandle)
	{
		if (!m_HandlerList.Contains (cmdHandle))
			m_HandlerList.Add (cmdHandle);
	}
	public void UnRegisterHandler(ICmdHandler cmdHandle)
	{
		if (m_HandlerList.Contains (cmdHandle))
			m_HandlerList.Remove (cmdHandle);
		
	}

	public int hashkey;
	public virtual void Disconnect()
    {
        if(m_TCPClient != null)
        {
            m_TCPClient.Disconnect();
            m_TCPClient = null;
        }
    }

	protected bool bTCP;
	protected uint newip = 0;
	protected ushort newport = 0;

	public virtual bool Connect(string ip, ushort port)
    {
		LogMgr.Log ("#Connecting Server:IP="+ip+" Port:"+port);
        if(m_TCPClient != null && m_TCPClient.IsConnected)
        {
            LogMgr.Log("TCP is connected.");
            return false;
        }
        INetClient tt;
        //bTCP = true;
        bool bret;
        
        if (bTCP)
        {
            tt = new TCPClient(this);
            bret = tt.Connect(ip, port, newip, newport);
        }
        else
        {
            tt = new UDPClient(this);
            bret = tt.Connect(ip, port, newip, newport);
        }
		
        m_TCPClient = tt;
        return bret;
    }
    public bool IsBeginConnect {
        get {
            return m_TCPClient != null && m_TCPClient.IsBeginConnect;
        }
    }
    public bool IsConnected
    {
        get
        {
            return m_TCPClient != null && m_TCPClient.IsConnected;
        }
    }
    public void StateChanged(NetState e)
    {
		for (int i = 0; i < m_HandlerList.Count; i++) {
			m_HandlerList [i].StateChanged (e);
		}
    }

	public virtual void Send(int cmdtype, byte[] packetBytes)
	{
		if (!IsConnected) {
			return;
		}

        if (LogMgr.ShowLog) {
            LogMgr.Log(">>>>>SendBytes [" + packetBytes.Length + " byte] main=" + (cmdtype >> 16 & 0xFF) + " sub=" + (cmdtype & 0xFF));
        }
		SendCmdPack cmdPack = new SendCmdPack ();
		cmdPack.cmdDatas = packetBytes;
		m_TCPClient.Send (cmdPack);
	}

	//LuaByteBuffer data, int Main, int Sub, LuaFunction func
	public void Send(int cmdTypeId,int respCmdType, byte[] bodyData, System.Action<NetCmdPack> callback)
	{
		if (!IsConnected) {
			callback.TryCall (null);
			return;
		}


		byte[] sendData = new byte[bodyData.Length + 4];
		byte[] cmdSize = BitConverter.GetBytes ((ushort)0);
		byte[] cmdType = BitConverter.GetBytes ((ushort)cmdTypeId);
		Array.Copy (cmdSize, 0, sendData, 0, 2);
		Array.Copy (cmdType, 0, sendData, 2, 2);
		Array.Copy (bodyData, 0, sendData, 4, bodyData.Length);

		SendCmdPack cmdPack = new SendCmdPack ();
		cmdPack.cmdDatas = sendData;
		m_TCPClient.Send (cmdPack);
		if (!sendcallbackCmdtypeList.ContainsKey (respCmdType)) {
			sendcallbackCmdtypeList [respCmdType] = new Queue<System.Action<NetCmdPack>> ();
		}
		sendcallbackCmdtypeList [respCmdType].Enqueue (callback);
	}

	Dictionary<int, Queue<System.Action<NetCmdPack>>> sendcallbackCmdtypeList = new Dictionary<int, Queue<System.Action<NetCmdPack>>>();
    public virtual void Send<S>(NetCmdType type, S ncb, System.Action<NetCmdPack> callback)
        where S : NetCmdBase
	{
		if (IsConnected) {
            int hashCode = (int)type;// TypeSize<U>.HASH;
			if (!sendcallbackCmdtypeList.ContainsKey (hashCode)) {
                sendcallbackCmdtypeList[hashCode] = new Queue<System.Action<NetCmdPack>>();
			}
			sendcallbackCmdtypeList [hashCode].Enqueue (callback);
			m_TCPClient.Send<S> (ncb);
			//LogMgr.Log (">>>>>>"+(NetCmdType)ncb.GetCmdType());
		} else {
			callback.TryCall (null);
		}
	}

	public virtual void Send<S>(NetCmdBase ncb)
    {
        if((ncb is S) == false)
        {
            if (LogMgr.ShowLog) {
                LogMgr.Log("命令类型不相等:" + ncb.ToString());
            }
            return;
        }
        if (IsConnected)
        {
            m_TCPClient.Send<S>(ncb);
			NetCmdType cmdType = (NetCmdType)(ncb.GetCmdType());
			bool exist = MainEntrace.Instance.CheckNetCmdFilter (cmdType);
			if (exist) {
                if (LogMgr.ShowLog) {
                    LogMgr.Log(UTool.GetTickCount() + "  send >>>>>>" + (NetCmdType)ncb.GetCmdType() + ": args:" + JsonUtility.ToJson(ncb));
                }
			}
        }
        else if (LogMgr.ShowLog) {
            LogMgr.Log("TCPClient don't connected, send cmd:" + ncb.ToString());
        }
    }

    public bool Handle(byte[] data, ushort offset, ushort length)
    {
        NetCmdPack pack = NetCmdHelper.ByteToPack(data, offset);
        if (pack.IsFull == true) {
            pack.localTick = (uint)(GlobalTimer.Ticks);
            m_CmdList.Enqueue(pack);
        }
        return true;
    }
    public void Handle(int verNo, int checkCode, byte[] body)
    {
        NetCmdPack pack = NetCmdHelper.ByteToPack(verNo, checkCode, body);
        if (pack.IsFull == true) {
            pack.localTick = (uint)(GlobalTimer.Ticks);
            m_CmdList.Enqueue(pack);
        }
    }
    public void ClearCmd()
    {
        m_CmdList.Clear();
    }
    public void ClearCmdAndHandler()
    {
		m_HandlerList.Clear ();
        m_CmdList.Clear();
    }

	public void ClearSends(){
        if (m_TCPClient != null) {
            m_TCPClient.ClearSendList();
        }
	}
	public virtual void Update(float delta)
    {
        if(m_TCPClient != null)
            m_TCPClient.Update();
		int kkhashkey = this.GetHashCode ();
		while (m_CmdList.Count > 0)
        {
			NetCmdPack nc = m_CmdList.Dequeue ();
			if (nc != null && !FilterNetPack (nc))
				continue;
			uint nowTicks = (uint)(GlobalTimer.Ticks);
			nc.tickSpan = Math.Min(nowTicks - nc.localTick, 1000);
			if (nc.tickSpan >= 1000) {
                LogMgr.LogError(((nc.cmdTypeId & int.MaxValue) >> 16) + ":" + (nc.cmdTypeId & ushort.MaxValue) + " : " + nc.localTick + " " + nowTicks);
			}
			if (HandleCallback (nc)) 
			{
				for (int i = 0; i < m_HandlerList.Count; i++) {
					if (m_HandlerList [i].CanProcessCmd ()) {
						if (m_HandlerList [i].Handle (nc))
							break;
					}
				}
			}
        }
    }

	protected virtual bool FilterNetPack(NetCmdPack nc){
		return true;
	}

    bool HandleCallback(NetCmdPack nc) {
        if (nc == null) {
            LogMgr.LogError("NetCmdPack is null");
            return false;
        }
        if (nc.respData != null) {
            if (sendcallbackCmdtypeList.ContainsKey(nc.cmdTypeId)) {
                System.Action<NetCmdPack> cb = sendcallbackCmdtypeList[nc.cmdTypeId].Dequeue();
                cb(nc);
            }
            return true;
        }
        return false;
    }
}
 