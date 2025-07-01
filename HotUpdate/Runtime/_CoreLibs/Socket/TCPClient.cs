using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.IO;
using System.Threading;
using System;
public interface INetHandler
{
    bool Handle(byte[] data, ushort offset, ushort length);
    void Handle(int verNo, int checkCode, byte[] body);
    void StateChanged(NetState state);
}
public enum ConnectResult
{
    CONNECT_OK,
    CONNECT_ERROR,
    CONNECT_VERSION,
}
public struct ConnectResultData
{
    public uint Ver;
    public uint PathCrc;
    public ConnectResult Result;
}
struct ClientSetting
{
    public const uint CONNECT_RESULT = 0x1343316f;
    public const int CONNECT_TIMEOUT = 6000;
    public const int TIMEOUT = 10000;
    public const int SOCKET_RECV_BUFF_SIZE = 1024 * 8;
    public const int SOCKET_SEND_BUFF_SIZE = 1024 * 8;
    public const int RECV_BUFF_SIZE = 1024 * 8;
    public const int CMD_MIN_SIZE = 4;
    public const int SLEEP_TIME = 1;
    public const int UDP_RESEND_TICK = 35;
}
public interface INetClient {
    bool IsBeginConnect { get; }//是否正在申请连接中
    bool Connect(string ip, int port, uint newip, ushort newport, bool bThread = true);
    void Disconnect();
    bool IsConnected { get; }
    void Update();
    void Send<T>(NetCmdBase dd);
	void Send (SendCmdPack scp);
	void ClearSendList();
	event Action OnHeartBeat;
    event Action OnConnectError;//连接失败回调
}
public enum NetState
{
    NET_DISCONNECT = 0,
    NET_CONNECTED  = 1,
    NET_ERROR	   = 2,
	NET_SERVER_KITOUT = 3,
}
public enum ConnectState
{
    CONNECT_WAIT,
    CONNECT_ERROR,
    CONNECT_OK,
}
public class ConnectData
{
    public Socket ConnectSocket;
    public IPEndPoint ConnectIP;
    public volatile ConnectState ConnectState;
}
public class RecvBuff
{
    public byte[] Buff = new byte[2048];
    public ushort Offset;
    public ushort RecvLength;
}


public class TCPClient:INetClient
{
    //---------------------------------------
    volatile bool m_bConnected = false;
	volatile int mNetState = 0x0;

    uint    m_SendTick = 0;
    Socket m_Socket;
    INetHandler m_Handler;
	Queue<SendCmdPack> m_SendList = new Queue<SendCmdPack>();
    private MemoryStream mRecvStram = new MemoryStream();
    int     m_RecvSize;
    //---------------------------------------
    public static Socket CreateSocket(bool bTCP, bool blocking)
    {
        Socket s;
        if (bTCP)
        {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.ExclusiveAddressUse = false;
            s.LingerState = new LingerOption(true, 1);
            s.NoDelay = true;
            s.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
        }
        else
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        s.ExclusiveAddressUse = false;
        s.ReceiveBufferSize = ClientSetting.SOCKET_RECV_BUFF_SIZE;
        s.SendBufferSize = ClientSetting.SOCKET_SEND_BUFF_SIZE;
        s.ReceiveTimeout = ClientSetting.CONNECT_TIMEOUT;
        s.SendTimeout = ClientSetting.TIMEOUT;
		s.Blocking = blocking;
        return s;
    }
    public TCPClient(INetHandler cmd)
    {
        m_Handler = cmd;
    }
    uint GetTickCount()
    {
        return UTool.GetTickCount();
    }
    public bool IsBeginConnect {
        set;
        get;
    }

    public bool IsConnected
    {
        get
        {
            return m_Socket != null && m_Socket.Connected;
        }
    }

    Action mOnHeartBeat;
    public event Action OnHeartBeat {
        add { mOnHeartBeat += value; }
        remove { mOnHeartBeat -= value; }
    }
    Action mOnConnectError;
    public event Action OnConnectError {
        add { mOnConnectError += value; }
        remove { mOnConnectError -= value; }
    }

	Queue<Thread> threadQueue = new Queue<Thread>();
    public void Disconnect()
    {
		while (threadQueue.Count > 0) {
			Thread thread = threadQueue.Dequeue ();
			thread.Abort ();
		}

        if (m_Socket != null)
        {
			if (m_bConnected && m_SendList.Count > 0) {
				try{
				SendSync ();
				} catch(Exception Exception){ 
					LogMgr.LogError (Exception.Message);
				}
			}
			CloseSocket ();
			mNetState = mNetState | (1 << (int)NetState.NET_DISCONNECT);
        }

		LogMgr.Log ("TcpClient DisConnect...");
    }

	void ThreadConnect(IAsyncResult ar)
    {
        try
        {
			Socket s = (Socket)ar.AsyncState;
			s.EndConnect (ar);
            m_bConnected = m_Socket.Connected;
        }
        catch
        {

        }
    }


	bool ChkNetUnAvaible(){
        //if (Application.internetReachability == NetworkReachability.NotReachable) {
        //    mNetState = mNetState | (1 << (int)NetState.NET_ERROR);
        //    Disconnect();//CloseSocket ();
        //    LogMgr.Log ("NetworkReachability.NotReachable...");
        //    return false;
        //}
		return true;
	}

	IEnumerator mSendCoroutine;
	IAsyncResult ccs = null;
	public bool Connect(string ip, int port, uint newip, ushort newport, bool bThread = true)
    {
		if (!ChkNetUnAvaible ())
			return false;		
//        const uint CONNECT_TIME_OUT = 3000;
        try
        {
			isClosing = false;
            m_bConnected = false;
            CloseSocket();
            IsBeginConnect = true;
            m_Socket = CreateSocket(true, true);
            ccs = m_Socket.BeginConnect(ip, port, new System.AsyncCallback((result) => {
                IsBeginConnect = false;
                m_Socket.EndSend(result);
				LogMgr.Log("Tcp Connect Succ. "+m_Socket.Connected);
				m_bConnected = m_Socket.Connected;
                this.mRecvStram.Seek(0, SeekOrigin.Begin);
                this.mRecvStram.SetLength(0);
				m_RecvSize = 0;
				m_SendTick = GetTickCount();
//				Thread thread = new Thread(new ThreadStart(ThreadSend));
//				thread.Start();
//				threadQueue.Enqueue(thread);
				receive();

                if (m_bConnected == true) {
                    ccs = null;
                }
			}), m_Socket);
			isStopSendCor = false;
			mSendCoroutine = ThreadSendCoroutine();
			MonoDelegate.Instance.StartCoroutine(mSendCoroutine);
            return true;
        }
		catch(Exception exp)
        {
			LogMgr.LogError ("connect tcp exception: "+exp.Message);
            return false;
        }
    }

	bool isClosing = false;
	bool isStopSendCor = false;
    void CloseSocket()
    {
		m_bConnected = false;
        if (m_Socket != null)
        {
			
			try{

				isStopSendCor = true;
				m_SendList.Clear();
				this.isSending = false;
            }catch (Exception e){
				LogMgr.LogError(e.Message);
            }
			
            try
            {
				m_Socket.SafeClose();
                m_Socket = null;
				ccs = null;
            }
            catch
            {

            }
        }
    }

    int ProcessRecvData(MemoryStream ms)
    {
        byte[] buff = new byte[4];
		int packageheadSize = NetCmdBase.HeadSize;
        int offset = 0;
        ms.Seek(0, SeekOrigin.Begin);
		while (m_RecvSize >= packageheadSize) {
            ms.Read(buff, 0, NetCmdBase.HeadSize);
            int verNo = buff[0];
            int checkCode = buff[1];
            int packetSize = System.BitConverter.ToInt16(buff, 2);
            if (m_RecvSize >= packetSize) {
                byte[] bytes = new byte[packetSize - NetCmdBase.HeadSize];
                ms.Read(bytes, 0, bytes.Length);
                m_Handler.Handle(verNo, checkCode, bytes);
                offset += packetSize;
                m_RecvSize -= packetSize;
            } else {
                break;
            }
		}

        return offset;
    }




    public bool Send(byte[] sendData)
    {
        try
        {
            return m_Socket.Send(sendData, SocketFlags.None) == sendData.Length;
        }
        catch
        {
        }
        return false;
    }
	/*
	void ThreadRecv()
	{
		while (m_bConnected) 
		{
			int ret = 0;
			Socket socket = m_Socket; 
			int idx = m_Offset + m_RecvSize;
			try
			{
				ret = socket.Receive (m_Buff, idx, m_Buff.Length - idx, SocketFlags.None);
			}catch(Exception e) {
				LogMgr.Log (e.Message);
				CloseSocket ();
				m_Handler.StateChanged(NetState.NET_ERROR);
			}
			uint tick = GetTickCount();
			if (ret > 0) {
				m_RecvSize += ret;
				if (!ProcessRecvData (m_Buff))
					break;;
				m_RecvTick = tick;
			}
			Thread.Sleep (1);
		}
	}
//*/
	
	const uint HEART_BEART = 5000;
	IEnumerator ThreadSendCoroutine()
	{
		while (true)
		{
			if (isStopSendCor)
				break;
            if (m_bConnected) {
                if (this.isSending == false) {
                    if (m_SendList.Count > 0) {
                        SendSync();
                    } else {
                        if (isClosing) {
                            CloseSocket();
                            LogMgr.Log("Socket Closed");
                            break;
                        }
                        uint nowTick = GetTickCount();
                        if (nowTick - m_SendTick > HEART_BEART) {
                            mOnHeartBeat.TryCall();
                        }
                    }
                } else {
                    uint nowTick = GetTickCount();
                    if (nowTick - m_SendTick > 100) {
                        LogMgr.LogWarning("Send Block.." + (nowTick - m_SendTick));
                        this.isSending = false;
                    }
                }
            } else if (ccs != null) {
                IsBeginConnect = false;
                if (ccs.IsCompleted && (m_Socket == null || m_Socket.Connected == false)) {
                    m_Socket = null;
                    mOnConnectError.TryCall();
                    ccs = null;
                }
            }
			yield return null;
		}

	}


	private IAsyncResult asyncReceive, asyncSend = null;
	bool isSending = false;
    private byte[] bytes = new byte[1024];
	void receive()
	{
        this.asyncReceive = m_Socket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(endReceive), null);
	}

    private MemoryStream __tmp_msg = new MemoryStream();
	private void endReceive(IAsyncResult asyncReceive)
	{
		if (!m_bConnected)
			return;
		
		Socket socket = m_Socket;
        try {
            int length = socket.EndReceive(asyncReceive);
            if (length > 0) {
                this.mRecvStram.Seek(0, SeekOrigin.End);
                this.mRecvStram.Write(bytes, 0, length);

                m_RecvSize += length;
                int offset = ProcessRecvData(this.mRecvStram);
                if(offset > 0){
                    this.mRecvStram.Seek(offset, SeekOrigin.Begin);
                    long len = this.mRecvStram.Length;
                    for (int i = offset; i < len; i++) {
                        __tmp_msg.WriteByte(Convert.ToByte(this.mRecvStram.ReadByte()));
                    }
                    MemoryStream ms = this.mRecvStram;
                    this.mRecvStram = __tmp_msg;
                    __tmp_msg = ms;

                    __tmp_msg.Seek(0, SeekOrigin.Begin);
                    __tmp_msg.SetLength(0);
                }

                if (m_bConnected)
                    receive();
            } else {
                Disconnect();
            }
        } catch (System.Net.Sockets.SocketException sockeexp) {			
			ErrorMsg(sockeexp.Message);
            Disconnect();
        } catch (Exception ex) {			
			mNetState = mNetState | (1 << (int)NetState.NET_ERROR);
			ErrorMsg(ex.Message+" stackTrace:"+ex.StackTrace);
            Disconnect();
        }
	}

	Queue<string> errLoggerQeue = new Queue<string>();
	void ErrorMsg(string errMsg)
	{
		errLoggerQeue.Enqueue (errMsg);
	}

	byte[] sendData;
	bool SendSync(byte[] bytes = null)
	{
		Socket socket = m_Socket;
		if (socket.Connected) {
			if (bytes == null) {
				var sendPack = m_SendList.Dequeue ();
                sendData = NetCmdHelper.CmdToBytes(sendPack, 0);
			} else {
				sendData = bytes;
			}
			try {
				asyncSend = socket.BeginSend (sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback (EndSend), sendData.Length);
				isSending = true;
				m_SendTick = GetTickCount ();
			} catch (Exception exp) {
				LogMgr.LogError (exp.Message);
				mNetState = mNetState | (1 << (int)NetState.NET_ERROR);
				Disconnect ();//CloseSocket ();
			}
				
		} else {
			LogMgr.LogError ("socket connected is false.................................................................");
		}
		
		return true;
	}

	void EndSend(IAsyncResult asyncsnd)
	{
		Socket socket = m_Socket;
		try
		{
			asyncSend = null;
			int sendSize = (int)asyncsnd.AsyncState;
			int length = socket.EndSend(asyncsnd);
			if (length == sendSize)
			{
				if (m_SendList.Count > 0)
					SendSync();
				else
					isSending = false;					
			}
			else
			{
				Disconnect ();
			}
		}
		catch
		{
			Disconnect ();
		}
	}



    public void Update()
    {
		if (mNetState > 0)
        {
			if ((mNetState & (1 << (int)NetState.NET_DISCONNECT)) != 0) {
				m_Handler.StateChanged(NetState.NET_DISCONNECT);	
			}

			if ((mNetState & (1 << (int)NetState.NET_ERROR)) != 0) {
				m_Handler.StateChanged(NetState.NET_ERROR);	
			}
			mNetState = 0;
        }
		while (errLoggerQeue.Count > 0) {
			LogMgr.LogError(errLoggerQeue.Dequeue ());
		}
    }

    public void Send<T>(NetCmdBase ncb)
    {
		if (isClosing)
			return;
		if (!ChkNetUnAvaible())
			return;
		SendCmdPack scp = new SendCmdPack();
        scp.Cmd = ncb;
        scp.Hash = TypeSize<T>.HASH;
		if (m_SendList.Count < SendQueueMax)
			m_SendList.Enqueue(scp);
        else
            LogMgr.Log("发送命令队列已满");
    }

	public void Send(SendCmdPack scp)
	{
		if (isClosing)
			return;
		if (!ChkNetUnAvaible())
			return;
		
		if (m_SendList.Count < SendQueueMax)
			m_SendList.Enqueue(scp);
		else
			LogMgr.Log("发送命令队列已满");
	}
	public void ClearSendList(){
		m_SendList.Clear();
	}

	const int SendQueueMax = 128;
}