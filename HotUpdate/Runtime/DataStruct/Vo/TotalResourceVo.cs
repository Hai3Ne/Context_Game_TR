using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class TotalResourceVo
{
	public uint NameID;
	public ushort Type;
	public uint Priority;
	public string AnimationID;
	public int PosType;
	public int[] Position;
	public string AnimationIDSelf;
	public int PosTypeSelf;
	public int[] PositionSelf;
	public int PicName;
	public int Audio;
	public bool IfShake;
	public const string CLSID="NameID,Type,Priority,AnimationID,PosType,Position,AnimationIDSelf,PosTypeSelf,PositionSelf,PicName,Audio,IfShake";
}