using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class LauncherVo
{
	public uint LrCfgID;
	public byte Level;
	public uint Source;
	public uint SourceSelf;
	public string Name;
	public uint Icon;
	public float FrontTime;
	public float Range;
	public byte MaxCatch;
	public float Interval;
	public float Speed;
	public Vector2 Size;
	public byte HiddenLevel;
	public uint SkillID;
	public uint BulletEff0;
	public uint BulletEff1;
	public uint BulletEff2;
	public uint BulletEff3;
	public byte ExtPropType0;
	public int ExtPropValue0;
	public byte ExtPropType1;
	public int ExtPropValue1;
	public byte ExtPropType2;
	public int ExtPropValue2;
	public uint BulletEffID;
	public uint BulletEffIDSelf;
	public uint TrailEffID;
	public uint FireEffID;
	public uint HitOnEffID;
	public uint HitOnEffIDSelf;
	public uint IdleFireEffID;
	public uint IdleEffID;
	public uint AudioID;
	public uint AutoType;
	public int VIPLevel;
	public const string CLSID="LrCfgID,Level,Source,SourceSelf,Name,Icon,FrontTime,Range,MaxCatch,Interval,Speed,Size,HiddenLevel,SkillID,BulletEff0,BulletEff1,BulletEff2,BulletEff3,ExtPropType0,ExtPropValue0,ExtPropType1,ExtPropValue1,ExtPropType2,ExtPropValue2,BulletEffID,BulletEffIDSelf,TrailEffID,FireEffID,HitOnEffID,HitOnEffIDSelf,IdleFireEffID,IdleEffID,AudioID,AutoType,VIPLevel";
}