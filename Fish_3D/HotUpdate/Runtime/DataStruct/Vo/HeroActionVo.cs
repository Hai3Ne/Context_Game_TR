using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class HeroActionVo
{
	public uint ActionCfgID;
	public uint AttEffDelay;
	public uint FireEffID;
	public uint HitEffID;
	public uint AttDistance;
	public uint HitEffDelay;
	public float Range;
	public ushort MaxCatch;
	public uint WorthFactor;
	public int AttTime;
	public int AniTime;
	public uint BulletCfgID;
	public uint AudioID;
	public const string CLSID="ActionCfgID,AttEffDelay,FireEffID,HitEffID,AttDistance,HitEffDelay,Range,MaxCatch,WorthFactor,AttTime,AniTime,BulletCfgID,AudioID";
}