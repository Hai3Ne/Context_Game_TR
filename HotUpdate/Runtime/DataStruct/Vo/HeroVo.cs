using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class HeroVo
{
	public uint CfgID;
	public uint SourceID;
	public uint IconID;
	public string Name;
	public string Desc;
	public uint EffID;
	public ushort AttDistance;
	public byte Quality;
	public uint Duration;
	public uint CD;
	public ushort MoveSpeed;
	public ushort RotSpeed;
	public uint[] ActionList;
	public uint EnterPrefab;
	public const string CLSID="CfgID,SourceID,IconID,Name,Desc,EffID,AttDistance,Quality,Duration,CD,MoveSpeed,RotSpeed,ActionList,EnterPrefab";
}