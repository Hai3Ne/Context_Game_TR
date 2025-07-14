using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AutoUseVo
{
	public uint AutoType;
	public bool LockFish;
	public uint BigFishMulti;
	public int FishNum;
	public uint[] NoneFishID;
	public float UseCD;
	public const string CLSID="AutoType,LockFish,BigFishMulti,FishNum,NoneFishID,UseCD";
}