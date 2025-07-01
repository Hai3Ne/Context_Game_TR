using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public struct SendCmdPack
{
	public int cmdType;
	public byte[] cmdDatas;
    public NetCmdBase Cmd;
    public int Hash;
}
public class NetCmdHelper
{
    public static bool CheckCmdType(NetCmdType cmdType)
    {
        if (NetCmdMapping.IsRegisterCmd(cmdType))
            return true;
        else
            return false;
    }


    public static byte[] CmdToBytes(SendCmdPack pack, int prefixSize)
    {
		if (pack.cmdDatas != null && pack.cmdDatas.Length > 0) 
		{
			int headSize = NetCmdBase.HeadSize;
			byte[] body = pack.cmdDatas;
			byte[] encryBody = null;//new byte[pack.cmdDatas.Length];
			int chkCode = body.Length + headSize;
			NetEncryptBuffer.Bm53Cipher (body, body.Length, out encryBody);

			byte[] head = new byte[headSize];
			int dataLen = encryBody.Length + headSize;
			head [0] = 5;
			head [1] = (byte)(chkCode & 0xFF);
			head [2] = (byte)dataLen;
			head [3] = (byte)(dataLen >> 8);

			byte[] bytes = new byte[headSize + encryBody.Length];
			Array.Copy (head, 0, bytes, 0, head.Length);
			Array.Copy (encryBody, 0, bytes, head.Length, encryBody.Length);
			return bytes;
		}

		return CmdToBytes (pack.Cmd, pack.Hash, prefixSize);
    }

	public static byte[] CmdToBytes<T>(NetCmdBase cmd, int prefixSize)
	{
		return CmdToBytes (cmd, TypeSize<T>.HASH, prefixSize);
	}

	public static byte[] CmdToBytes(NetCmdBase cmd, int hash, int prefixLength)
	{
		cmd.MainCmd = (ushort)(cmd.MainCmd & 0x3FFF);
		byte[] data = TypeReflector.ObjToBytes(cmd, hash, prefixLength);
		if (data == null)
		{
            if (LogMgr.ShowLog) {
                LogMgr.Log("Unregister cmd type:" + cmd.GetCmdType());
            }
		}
		else
		{
			int headSize = NetCmdBase.HeadSize;
			byte[] head = new byte[headSize];
			byte[] body = new byte[data.Length - headSize];
			Array.Copy (data, 0, head, 0, head.Length);
			Array.Copy (data, headSize, body, 0, body.Length);
			byte[] encryBody = null;
			int chkCode = data.Length;
			NetEncryptBuffer.Bm53Cipher (body, body.Length, out encryBody);
			int dataLen = encryBody.Length + headSize;
			head [1] = (byte)(chkCode & 0xFF);
			head [2] = (byte)dataLen;
			head [3] = (byte)(dataLen >> 8);

	
			data = new byte[headSize + encryBody.Length];
			Array.Copy (head, 0, data, 0, head.Length);
			Array.Copy (encryBody, 0, data, head.Length, encryBody.Length);
		}

		return data;
	}

    private static MemoryStream buffs_cache = new MemoryStream();//分包数据缓存
    public static NetCmdBase ByteToCmd(byte[] byteData, int offset, int plength, out byte[] packBody) {
        packBody = byteData;
        int headSize = NetCmdBase.HeadSize;
        if (plength < headSize) {
            LogMgr.LogError("包体大小错误");
            return null;
        }

        int verNo = byteData[offset];
        int checkCode = byteData[offset + 1];
        int packSize = System.BitConverter.ToUInt16(byteData, offset + 2);

        byte[] body = new byte[packSize - headSize];
        Array.Copy(byteData, offset + headSize, body, 0, body.Length);

        byte[] deEncryBuff = null;
        NetEncryptBuffer.Bm53InvCipher(body, body.Length, out deEncryBuff);
        //int realChCode = (checkCode - headSize) % 16 ;
        int bodyLen = deEncryBuff.Length + (checkCode - headSize + 16) % 16 - 16;//-  (realChCode > 0 ? 16 - realChCode : 0);

        uint mainCmd = System.BitConverter.ToUInt16(deEncryBuff, 0);
        uint subCmd = System.BitConverter.ToUInt16(deEncryBuff, 2);
        NetCmdType cmdType = (NetCmdType)(1 << 31 | mainCmd << 16 | subCmd);

        byte[] dataBuffer = new byte[bodyLen + 4];
        Array.Copy(byteData, 0, dataBuffer, 0, headSize);
        Array.Copy(deEncryBuff, 0, dataBuffer, headSize, bodyLen);
        packBody = deEncryBuff;
        headSize = 8;//Unity中包头是8个字节
        //-8

        if (cmdType == NetCmdType.SUB_S_WORLDBOSS_RANK || cmdType == NetCmdType.SUB_S_WORLDBOSS_SYNC) {//分包消息处理
            if (buffs_cache.Length == 0) {
                buffs_cache.Write(dataBuffer, 0, dataBuffer.Length);
            } else {
                buffs_cache.Write(dataBuffer, headSize, dataBuffer.Length - headSize);
            }
            if (dataBuffer.Length == 4000 - 8 + headSize) {
                return null;
                //return new NetCmdBase {//返回非完整包标志，逻辑不处理
                //    IsFull = false
                //};
            } else {//包体小于分包大小 则说明包已经全部发送完成
                dataBuffer = new byte[buffs_cache.Length];
                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.Read(dataBuffer, 0, dataBuffer.Length);

                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.SetLength(0);
            }
        }
        if (buffs_cache.Length > 0) {
            buffs_cache.Seek(0, SeekOrigin.Begin);
            buffs_cache.SetLength(0);
        }

        NetTypeInfo typeInfo = NetCmdMapping.GetTypeInfo(cmdType);
        NetCmdBase cmd = null;
        if (typeInfo != null) {
            cmd = (NetCmdBase)TypeReflector.BytesToObj(
                typeInfo.TypeHashCode,
                dataBuffer,
                0,
                dataBuffer.Length,
                0
            );
            if (cmd == null || MainEntrace.Instance.CheckValidPack(typeInfo.TypeHashCode, cmd) == false) {
                string fn = cmdType.ToString() + "__" + System.DateTime.Now.ToString("yyyy-M-d_HH-mm-ss-ffff") + ".byte";
                LogMgr.SaveDebugData(fn, byteData, offset, plength);
            }
        } else {
            //			LogMgr.LogError ("#pack Error mainCmd:" + mainCmd + "  subCmd:" + subCmd);
        }
        if (cmd != null) {
            cmd.SetCmdType(cmdType);
        }
        return cmd;
    }
    public static NetCmdPack ByteToPack(byte[] byteData, int offset) {
        NetCmdPack pack = new NetCmdPack();
        int headSize = NetCmdBase.HeadSize;
        int verNo = byteData[offset];
        int checkCode = byteData[offset + 1];
        int packSize = System.BitConverter.ToUInt16(byteData, offset + 2);

        byte[] body = new byte[packSize - headSize];
        Array.Copy(byteData, offset + headSize, body, 0, body.Length);

        byte[] deEncryBuff = null;
        NetEncryptBuffer.Bm53InvCipher(body, body.Length, out deEncryBuff);
        //int realChCode = (checkCode - headSize) % 16 ;
        int bodyLen = deEncryBuff.Length + (checkCode - headSize + 16) % 16 - 16;//-  (realChCode > 0 ? 16 - realChCode : 0);

        uint mainCmd = System.BitConverter.ToUInt16(deEncryBuff, 0);
        uint subCmd = System.BitConverter.ToUInt16(deEncryBuff, 2);
        pack.cmdTypeId = (int)(1 << 31 | mainCmd << 16 | subCmd);

        pack.respData = new byte[bodyLen];
        Array.Copy(deEncryBuff, 0, pack.respData, 0, bodyLen);
        //pack.respData = deEncryBuff;// new byte[bodyLen + 4];
        //Array.Copy(byteData, 0, pack.respData, 0, headSize);
        //Array.Copy(deEncryBuff, 0, pack.respData, headSize, bodyLen);
        headSize = 4;//Unity中包头是8个字节
        //-8

        if ((NetCmdType)pack.cmdTypeId == NetCmdType.SUB_S_WORLDBOSS_RANK || (NetCmdType)pack.cmdTypeId == NetCmdType.SUB_S_WORLDBOSS_SYNC) {//分包消息处理
            if (buffs_cache.Length == 0) {
                buffs_cache.Write(pack.respData, 0, pack.respData.Length);
            } else {
                buffs_cache.Write(pack.respData, headSize, pack.respData.Length - headSize);
            }
            if (pack.respData.Length == 4000 - 8 + headSize) {
                return new NetCmdPack {//返回非完整包标志，逻辑不处理
                    IsFull = false,
                };
                //return new NetCmdBase {//返回非完整包标志，逻辑不处理
                //    IsFull = false
                //};
            } else {//包体小于分包大小 则说明包已经全部发送完成
                pack.respData = new byte[buffs_cache.Length];
                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.Read(pack.respData, 0, pack.respData.Length);

                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.SetLength(0);
            }
        }
        if (buffs_cache.Length > 0) {
            buffs_cache.Seek(0, SeekOrigin.Begin);
            buffs_cache.SetLength(0);
        }
        pack.IsFull = true;
        return pack;
    }
    public static NetCmdPack ByteToPack(int verNo, int checkCode, byte[] body) {
        NetCmdPack pack = new NetCmdPack();
        int headSize = NetCmdBase.HeadSize;

        byte[] deEncryBuff = null;
        NetEncryptBuffer.Bm53InvCipher(body, body.Length, out deEncryBuff);
        //int realChCode = (checkCode - headSize) % 16 ;
        int bodyLen = deEncryBuff.Length + (checkCode - headSize + 16) % 16 - 16;//-  (realChCode > 0 ? 16 - realChCode : 0);

        uint mainCmd = System.BitConverter.ToUInt16(deEncryBuff, 0);
        uint subCmd = System.BitConverter.ToUInt16(deEncryBuff, 2);
        pack.cmdTypeId = (int)(1 << 31 | mainCmd << 16 | subCmd);

        pack.respData = new byte[bodyLen];
        Array.Copy(deEncryBuff, 0, pack.respData, 0, bodyLen);
        //pack.respData = deEncryBuff;// new byte[bodyLen + 4];
        //Array.Copy(byteData, 0, pack.respData, 0, headSize);
        //Array.Copy(deEncryBuff, 0, pack.respData, headSize, bodyLen);
        headSize = 4;//Unity中包头是8个字节
        //-8

        if ((NetCmdType)pack.cmdTypeId == NetCmdType.SUB_S_WORLDBOSS_RANK || (NetCmdType)pack.cmdTypeId == NetCmdType.SUB_S_WORLDBOSS_SYNC) {//分包消息处理
            if (buffs_cache.Length == 0) {
                buffs_cache.Write(pack.respData, 0, pack.respData.Length);
            } else {
                buffs_cache.Write(pack.respData, headSize, pack.respData.Length - headSize);
            }
            if (pack.respData.Length == 4000 - 8 + headSize) {
                return new NetCmdPack {//返回非完整包标志，逻辑不处理
                    IsFull = false,
                };
                //return new NetCmdBase {//返回非完整包标志，逻辑不处理
                //    IsFull = false
                //};
            } else {//包体小于分包大小 则说明包已经全部发送完成
                pack.respData = new byte[buffs_cache.Length];
                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.Read(pack.respData, 0, pack.respData.Length);

                buffs_cache.Seek(0, SeekOrigin.Begin);
                buffs_cache.SetLength(0);
            }
        }
        if (buffs_cache.Length > 0) {
            buffs_cache.Seek(0, SeekOrigin.Begin);
            buffs_cache.SetLength(0);
        }
        pack.IsFull = true;
        return pack;
    }
}
