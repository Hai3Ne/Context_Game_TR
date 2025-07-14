using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class FishBookVo
{
	public uint PicCfgID;
	public uint Multiple;
	public int MultipleMax;
	public uint Type;
	public string PicName;
	public string PicDes;
	public uint ModelName;
	public uint ResName;
	public uint Speed;
	public uint Difficulty;
	public float Scale;
	public float Face;
	public float AnimSpeed;
	public const string CLSID="PicCfgID,Multiple,MultipleMax,Type,PicName,PicDes,ModelName,ResName,Speed,Difficulty,Scale,Face,AnimSpeed";
}