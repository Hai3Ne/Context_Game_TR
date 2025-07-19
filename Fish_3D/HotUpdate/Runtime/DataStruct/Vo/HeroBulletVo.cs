using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class HeroBulletVo
{
	public uint CfgID;
	public uint SourceID;
	public float Speed;
	public float SpeedMax;
	public float Accelerate;
	public bool LockFish;
	public float Scale;
	public const string CLSID="CfgID,SourceID,Speed,SpeedMax,Accelerate,LockFish,Scale";
}