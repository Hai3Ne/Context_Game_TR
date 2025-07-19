using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class LK_FishVo
{
	public int ID;
	public string Name;
	public string Model;
	public int Type;
	public int LinkFish;
	public int TotalType;
	public bool Tips;
	public bool IsLock;
	public int IsLockAngle;
	public int Depth;
	public int Speed;
	public int[] Multiple;
	public int[] BoundingBox;
	public int HitRadius;
	public float captureProbability;
	public int GoldType;
	public int GoldDistance;
	public string[] AppearAudio;
	public string[] RandomAudio;
	public string[] DieAudio;
	public float Shake;
	public const string CLSID="ID,Name,Model,Type,LinkFish,TotalType,Tips,IsLock,IsLockAngle,Depth,Speed,Multiple,BoundingBox,HitRadius,captureProbability,GoldType,GoldDistance,AppearAudio,RandomAudio,DieAudio,Shake";
}