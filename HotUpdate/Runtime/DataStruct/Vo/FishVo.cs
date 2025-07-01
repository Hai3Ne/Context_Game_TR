using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class FishVo
{
	public uint CfgID;
	public byte Type;
	public uint SourceID;
	public uint IconID;
	public uint HalfBodyID;
	public string Name;
	public string Desc;
	public bool IsLock;
	public uint Quality;
	public uint Speed;
	public uint Exp;
	public uint Socre;
	public uint Engery;
	public uint Catch0;
	public uint Catch1;
	public uint Multiple;
	public uint Resistance;
	public uint EventID;
	public float Scale;
	public float CallScale;
	public uint ShakeEffectID;
	public uint AwardEffect;
	public byte GoldSplitNum;
	public float GoldIconScaling;
	public float GoldLabelScaling;
	public uint DieAudio;
	public byte GoldRangeType;
	public int GoldRange;
	public byte BirthBG;
	public int[] BirthPosition;
	public int[] ComingPosition;
	public bool ComingTip;
	public bool FishDieAnim;
	public const string CLSID="CfgID,Type,SourceID,IconID,HalfBodyID,Name,Desc,IsLock,Quality,Speed,Exp,Socre,Engery,Catch0,Catch1,Multiple,Resistance,EventID,Scale,CallScale,ShakeEffectID,AwardEffect,GoldSplitNum,GoldIconScaling,GoldLabelScaling,DieAudio,GoldRangeType,GoldRange,BirthBG,BirthPosition,ComingPosition,ComingTip,FishDieAnim";
}