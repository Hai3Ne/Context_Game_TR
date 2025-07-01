using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class FishBossVo
{
	public uint ID;
	public uint CfgID;
	public uint AppearEffID;
	public float Speed;
	public bool IsCritPoint;
	public uint PointCD;
	public uint ShakeEffectID;
	public uint Color;
	public float DizzyTime;
	public const string CLSID="ID,CfgID,AppearEffID,Speed,IsCritPoint,PointCD,ShakeEffectID,Color,DizzyTime";
}