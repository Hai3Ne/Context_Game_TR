using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class EngeryRoomVo
{
	public uint ID;
	public string Name;
	public uint IconID;
	public uint Gold;
	public uint[] Engery;
	public uint[] Multiples;
	public uint LootID;
	public uint HeroID0;
	public uint HeroID1;
	public uint HeroID2;
	public uint HeroID3;
	public uint FishLibID;
	public const string CLSID="ID,Name,IconID,Gold,Engery,Multiples,LootID,HeroID0,HeroID1,HeroID2,HeroID3,FishLibID";
}