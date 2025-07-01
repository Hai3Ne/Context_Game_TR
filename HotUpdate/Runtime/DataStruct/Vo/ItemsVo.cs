using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class ItemsVo
{
	public uint CfgID;
	public byte ItemType;
	public byte DropShowType;
	public string ItemName;
	public string ItemIcon;
	public string ItemDec;
	public byte Quality;
	public byte Star;
	public int SalePrice;
	public uint SaleNum;
	public uint Worth;
	public uint MaxCount;
	public uint EffDuration;
	public int GainCD;
	public int GainMax;
	public bool CanUse;
	public byte UseType;
	public int Value0;
	public int Value1;
	public int Value2;
	public uint AutoType;
	public const string CLSID="CfgID,ItemType,DropShowType,ItemName,ItemIcon,ItemDec,Quality,Star,SalePrice,SaleNum,Worth,MaxCount,EffDuration,GainCD,GainMax,CanUse,UseType,Value0,Value1,Value2,AutoType";
}