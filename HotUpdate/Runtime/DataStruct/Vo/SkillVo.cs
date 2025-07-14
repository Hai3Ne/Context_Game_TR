using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class SkillVo
{
	public uint CfgID;
	public string Icon;
	public string Name;
	public string Desc;
	public bool NeedTarget;
	public float CD;
	public uint BBuffID;
	public uint EffID0;
	public uint EffID1;
	public uint EffID2;
	public uint EffID3;
	public uint PrepareEffID;
	public uint SightEffID;
	public uint CastEffID;
	public uint HurtEffID;
	public uint CastDuation;
	public uint AudioID;
	public uint AudioTime;
	public uint WorthFactor;
	public uint HitAudio;
	public const string CLSID="CfgID,Icon,Name,Desc,NeedTarget,CD,BBuffID,EffID0,EffID1,EffID2,EffID3,PrepareEffID,SightEffID,CastEffID,HurtEffID,CastDuation,AudioID,AudioTime,WorthFactor,HitAudio";
}