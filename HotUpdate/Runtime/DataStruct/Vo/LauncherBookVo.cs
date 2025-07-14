using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class LauncherBookVo
{
	public uint PicCfgID;
	public uint Type;
	public string PicName;
	public string PicDes;
	public uint ResName;
	public float AttackSpeed;
	public uint EnergySpeed;
	public uint DropRate;
	public uint HitRate;
	public string SkillIcon;
	public string SkillName;
	public string SkillDes;
	public const string CLSID="PicCfgID,Type,PicName,PicDes,ResName,AttackSpeed,EnergySpeed,DropRate,HitRate,SkillIcon,SkillName,SkillDes";
}