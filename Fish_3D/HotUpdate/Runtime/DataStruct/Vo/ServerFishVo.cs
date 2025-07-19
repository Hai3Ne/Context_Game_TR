using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class ServerFishVo
{
	public int CfgID;
	public uint FishID;
	public int Waves;
	public int FishNum;
	public float[] WaveCD;
	public float Scale;
	public float MoveSpeed;
	public float AnimSpeed;
	public string ApearAnim;
	public uint KingID;
	public float KingScale;
	public float KingSpeed;
	public float KingAnimSpeed;
	public uint[] KingPath;
	public int[] OffsetXYZ;
	public int[] StartPoint;
	public int Shape;
	public int Value0;
	public int[] Value1;
	public const string CLSID="CfgID,FishID,Waves,FishNum,WaveCD,Scale,MoveSpeed,AnimSpeed,ApearAnim,KingID,KingScale,KingSpeed,KingAnimSpeed,KingPath,OffsetXYZ,StartPoint,Shape,Value0,Value1";
}