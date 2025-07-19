using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class NetCmdBase 
{
	public const int HeadSize = 4;
    //public bool IsFull = true;//是否是完整包
    [TypeInfo(0)]
    public byte dataKind = 5;
    [TypeInfo(1)]
    public byte checkCode;
    [TypeInfo(2)]
    public ushort CmdSize;    //命令长度
    [TypeInfo(3)]
	public ushort    MainCmd; //子命令ID
    [TypeInfo(4)]
	public ushort     SubCmd;    //主命令ID
    public uint GetCmdType()
    {
		return (uint)(((int)MainCmd) << 16 | SubCmd);
    }

	public override string ToString(){
		NetCmdType cmdt = (NetCmdType)GetCmdType ();
		return cmdt.ToString ();
	}
	public void SetCmdType(NetCmdType nct)
    {
		uint cmd     = (uint)nct;
		MainCmd     = (ushort)(cmd >> 16);
		SubCmd  	= (ushort)cmd;
    }
}