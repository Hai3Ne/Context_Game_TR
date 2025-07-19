using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class TimeRoomVo
{
	public uint CfgID;
	public string Name;
	public uint IconID;
	public uint Gold;
	public float GoldNum;
	public uint GoldNumMax;
	public uint[] Multiple;
	public uint[] Duration;
	public uint[] Items;
	public uint LootID;
	public uint[] Heroes;
	public uint[] Launchers;
	public uint RoomMultiple;
	public int CostScore;
	public int QuickCharge;
	public const string CLSID="CfgID,Name,IconID,Gold,GoldNum,GoldNumMax,Multiple,Duration,Items,LootID,Heroes,Launchers,RoomMultiple,CostScore,QuickCharge";
}