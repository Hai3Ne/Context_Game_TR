using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

using System.IO;

public class NetClient {
    public const int HeartTime = 5 * 30;//心跳包发送间隔/单位：帧
    private const int HeartWaitFrame = 30 * 30;//心跳包等待总帧数
    public static Socket mSocket;
    private static IAsyncResult mAsynConnect = null;
    private static MemoryStream mRecvStram = new MemoryStream();
    private static int m_RecvSize;
    private static Queue<SendCmdPack> m_SendList = new Queue<SendCmdPack>();
    private static VoidCall<NetCmdPack> mHandleCall = null;
    private static VoidDelegate mConnectSucc;//连接成功回调
    private static VoidDelegate mConnectError;//连接错误回调

    public static bool IsConnected {
        get {
            return mSocket != null && mSocket.Connected;
        }
    }
    private static Queue<string> errLoggerQeue = new Queue<string>();
    private static void ErrorMsg(string errMsg) {
        errLoggerQeue.Enqueue(errMsg);
    }
    private static int _pre_validate_time = 0;//校验等待帧数
    public static void Update() {
        if (mAsynConnect != null) {
            if (mAsynConnect.IsCompleted){
                ConnectFinish();
            }
            return;
        }
        if (IsConnected) {
            _pre_validate_time++;
            if (_pre_validate_time > HeartWaitFrame) {//校验包超过一定帧数未收到  直接断开
                LogMgr.LogError("校验包超时断开");
                Disconnect();
            } else if (_pre_validate_time == HeartTime) {
                NetClient.Send(NetCmdType.SUB_KN_VALIDATE_SOCKET, new CMD_GP_Validate());
            }
        }

        while (errLoggerQeue.Count > 0) {
            LogMgr.LogError(errLoggerQeue.Dequeue());
        }

        lock (m_CmdList) {
            while (m_CmdList.Count > 0) {
                NetCmdPack nc = m_CmdList.Dequeue();
                if (nc == null || !FilterNetPack(nc))
                    continue;
                if (GlobalTimer.Ticks >= nc.localTick + 1000) {
                    long ticks = GlobalTimer.Ticks;
                    LogMgr.LogError(string.Format("{0}-{1}:{2}({3}-{4})", (nc.cmdTypeId >> 16) & short.MaxValue, nc.cmdTypeId & short.MaxValue, ticks - nc.localTick, nc.localTick, ticks));
                }

                if (NetClient.mHandleCall != null) {
                    NetClient.mHandleCall(nc);
                }
            }
        }
    }
    private static bool FilterNetPack(NetCmdPack nc) {
        if (nc.cmdTypeId == (int)(1 << 31 | 0 << 16 | 1)) {//心跳包逻辑处理
            //byte[] bytes = new byte[nc.respData.Length - NetCmdBase.HeadSize];
            //Array.Copy(nc.respData, NetCmdBase.HeadSize, bytes, 0, bytes.Length);
            NetClient.Send(nc.cmdTypeId & int.MaxValue, nc.respData);
            return false;
        //} else {
            //LuaTable luaPack = MainEntrace.Instance.Deseriable(nc.cmdTypeId & int.MaxValue, new LuaByteBuffer(nc.respData));
            //if (luaPack != null) {
            //    HttpServer.Instance.mNetHandler.Handle(luaPack);
            //}
        }
        return true;
    }
    private static void ConnectFinish() {
        lock (TimeManager.Mono) {
            if (mAsynConnect == null) {
                return;
            }
            if (mSocket == null || mSocket.Connected == false) {
                mSocket = null;
                //mOnConnectError.TryCall();
                if (mConnectError != null) {
                    TimeManager.AddCallThread(mConnectError);
                    mConnectError = null;
                }
                Debug.LogError("连接失败");
            } else {
                try {
                    mSocket.EndSend(mAsynConnect);
                } catch {
                }
                LogMgr.Log("Tcp Connect Succ. " + mSocket.Connected);
                NetClient.mRecvStram.Seek(0, SeekOrigin.Begin);
                NetClient.mRecvStram.SetLength(0);
                m_RecvSize = 0;
                StartReceive();
                if (mConnectSucc != null) {
                    TimeManager.AddCallThread(mConnectSucc);
                    mConnectSucc = null;
                }
            }
            mAsynConnect = null;
        }
    }
    public static void Connect(string ip, int port, VoidCall<NetCmdPack> call, VoidDelegate conn_succ, VoidDelegate conn_err) {
        HttpServer.Instance.CloearAndClose();
        NetClient.m_SendList.Clear();
        lock (m_CmdList) {
            NetClient.m_CmdList.Clear();
        }
        NetClient.mHandleCall = call;
        NetClient.mConnectSucc = conn_succ;
        NetClient.mConnectError = conn_err;
        if(mSocket != null){
            CloseSocket(mSocket);
        }
        LogMgr.Log("start connect ip:" + ip + "  port:" + port);
        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        mSocket.ExclusiveAddressUse = false;
        mSocket.LingerState = new LingerOption(true, 1);
        mSocket.NoDelay = true;
        mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

        mSocket.ExclusiveAddressUse = false;
        mSocket.ReceiveBufferSize = ClientSetting.SOCKET_RECV_BUFF_SIZE;
        mSocket.SendBufferSize = ClientSetting.SOCKET_SEND_BUFF_SIZE;
        //mSocket.ReceiveTimeout = ClientSetting.CONNECT_TIMEOUT;
        //mSocket.SendTimeout = ClientSetting.TIMEOUT;
        mSocket.Blocking = true;

        _pre_validate_time = 0;
        mAsynConnect = mSocket.BeginConnect(ip, port, new System.AsyncCallback((result) => {
            ConnectFinish();
        }), mSocket);
    }
    private static void Disconnect() {//异常断开
        if (IsConnected && m_SendList.Count > 0) {
            try {
                SendSync();
            } catch (Exception Exception) {
                LogMgr.LogError(Exception.Message);
            }
        }
        if (mSocket != null) {
            Socket socket = mSocket;
            mSocket = null;
            CloseSocket(socket);

            TimeManager.AddCallThread(() => {
                MainEntrace.Instance.HideLoad();
                SystemMessageMgr.Instance.DialogShow("与服务器连接断开了...", delegate {
                    GameSceneManager.BackToHall(GameManager.CurGameEnum);
                });
            });

            LogMgr.Log("TcpClient DisConnect...");
        }
    }
    public static void CloseConnect() {
        m_SendList.Clear();
        Socket socket = mSocket;
        mSocket = null;
        CloseSocket(socket);
    }
    private static void CloseSocket(Socket socket) {//
        if (socket != null) {
            try {
                LogMgr.Log("断开链接");
                m_SendList.Clear();
                lock (m_CmdList) {
                    m_CmdList.Clear();
                }
                mIsSending = false;
                socket.SafeClose();
                mAsynConnect = null;
            } catch {

            }
        }
    }

    private static IAsyncResult mAsyncReceive, asyncSend = null;
    private static bool mIsSending = false;
    private static byte[] bytes = new byte[1024];
    private static void StartReceive() {
        mAsyncReceive = mSocket.BeginReceive(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(EndReceive), null);
    }
    private static MemoryStream __tmp_msg = new MemoryStream();
    private static void EndReceive(IAsyncResult asyncReceive) {
        if (IsConnected == false) {
            return;
        }

        try {
            int length = mSocket.EndReceive(asyncReceive);
            if (length > 0) {
                NetClient.mRecvStram.Seek(0, SeekOrigin.End);
                NetClient.mRecvStram.Write(bytes, 0, length);
                m_RecvSize += length;

                int offset = ProcessRecvData(NetClient.mRecvStram);
                if (offset > 0) {
                    NetClient.mRecvStram.Seek(offset, SeekOrigin.Begin);
                    long len = NetClient.mRecvStram.Length;
                    for (int i = offset; i < len; i++) {
                        __tmp_msg.WriteByte(Convert.ToByte(NetClient.mRecvStram.ReadByte()));
                    }
                    MemoryStream ms = NetClient.mRecvStram;
                    NetClient.mRecvStram = __tmp_msg;
                    __tmp_msg = ms;

                    __tmp_msg.Seek(0, SeekOrigin.Begin);
                    __tmp_msg.SetLength(0);
                }

                StartReceive();
            } else {
                ErrorMsg("接受不到数据断开");
                Disconnect();
            }
        } catch (Exception ex) {
            ErrorMsg("NetClient.EndReceive" + ex.Message + " stackTrace:" + ex.StackTrace);
            Disconnect();
        }
    }
    public static void Send<T>(NetCmdType type, T ncb) where T : NetCmdBase {
        ncb.SetCmdType(type);
        NetClient.Send(new SendCmdPack {
            Cmd = ncb,
            Hash = TypeSize<T>.HASH,
        });
    }
    public static void Send<T>(NetCmdBase ncb) {
        NetClient.Send(new SendCmdPack {
            Cmd = ncb,
            Hash = TypeSize<T>.HASH,
        });
    }
    private static void Send(int cmdtype, byte[] packetBytes) {
        SendCmdPack cmdPack = new SendCmdPack();
        cmdPack.cmdType = cmdtype;
        cmdPack.cmdDatas = packetBytes;
        if (LogMgr.ShowLog) {
            LogMgr.Log(">>>>>SendBytes [" + packetBytes.Length + " byte] main=" + (cmdtype >> 16 & 0xFFFF) + " sub=" + (cmdtype & 0xFFFF));
        }
        NetClient.Send(cmdPack);
    }
    public static void Send(SendCmdPack scp) {
        if (IsConnected == false) {
            return;
        }

        if (mIsSending == false) {
            SendSync(NetCmdHelper.CmdToBytes(scp, 0));
        } else {
            m_SendList.Enqueue(scp);
        }
    }
    public void ClearSendList() {
        m_SendList.Clear();
    }
    private static byte[] __sendData;
    private static bool SendSync(byte[] bytes = null) {
        if (mSocket.Connected) {
            if (bytes == null) {
                var sendPack = m_SendList.Dequeue();
                __sendData = NetCmdHelper.CmdToBytes(sendPack, 0);
            } else {
                __sendData = bytes;
            }
            try {
                mIsSending = true;
                asyncSend = mSocket.BeginSend(__sendData, 0, __sendData.Length, SocketFlags.None, new AsyncCallback(EndSend), __sendData.Length);
            } catch (Exception exp) {
                LogMgr.LogError("NetClient.SendSync");
                LogMgr.LogError(exp.Message);
                Disconnect();//CloseSocket ();
            }
            __sendData = null;
        } else {
            LogMgr.LogError("socket connected is false.................................................................");
        }
        return true;
    }
    private static void EndSend(IAsyncResult asyncsnd) {
        try {
            asyncSend = null;
            int sendSize = (int)asyncsnd.AsyncState;
            int length = mSocket.EndSend(asyncsnd);
            if (length == sendSize) {
                if (m_SendList.Count > 0)
                    SendSync();
                else
                    mIsSending = false;
            } else {
                ErrorMsg("发包大小不一致断开");
                Disconnect();
            }
        } catch (Exception ex) {
            ErrorMsg("网络异常");
            ErrorMsg(ex.Message);
            ErrorMsg(ex.StackTrace);
            Disconnect();
        }
    }
    private static Queue<NetCmdPack> m_CmdList = new Queue<NetCmdPack>();
    private static int ProcessRecvData(MemoryStream ms) {
        int packageheadSize = NetCmdBase.HeadSize;
        int offset = 0;
        byte[] buff = new byte[4];
        ms.Seek(0, SeekOrigin.Begin);
        while (m_RecvSize >= packageheadSize) {
            ms.Read(buff, 0, NetCmdBase.HeadSize);
            int verNo = buff[0];
            int checkCode = buff[1];
            int packetSize = System.BitConverter.ToInt16(buff, 2);

            if (m_RecvSize >= packetSize) {
                byte[] bytes = new byte[packetSize - NetCmdBase.HeadSize];
                ms.Read(bytes, 0, bytes.Length);
                NetCmdPack pack = NetCmdHelper.ByteToPack(verNo, checkCode, bytes);
                if (pack.IsFull == true) {
                    _pre_validate_time = 0;//
                    if ((NetCmdType)pack.cmdTypeId == NetCmdType.SUB_KN_VALIDATE_SOCKET) {//socket校验
                    } else {
                        pack.localTick = (uint)(GlobalTimer.Ticks);
                        lock (m_CmdList) {
                            m_CmdList.Enqueue(pack);
                        }
                    }
                }
                offset += packetSize;
                m_RecvSize -= packetSize;
            } else {
                break;
            }
        }
        return offset;
    }
}
