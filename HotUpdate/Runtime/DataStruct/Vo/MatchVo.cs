using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class MatchVo
{
	public uint ModeID;
	public uint CfgID;
	public uint TeamNum;
	public uint LeastNum;
	public uint[] MatchStartTime;
	public uint StartTime;
	public uint OverTime;
	public uint SingleTime;
	public uint FreeNum;
	public uint FreeTime;
	public uint FightNum;
	public uint TicketID;
	public uint TicketNum;
	public uint BulletID;
	public uint BulletNum;
	public int[] FightAward;
	public string[] Enounce;
	public int Charge;
	public uint WinRule;
	public const string CLSID="ModeID,CfgID,TeamNum,LeastNum,MatchStartTime,StartTime,OverTime,SingleTime,FreeNum,FreeTime,FightNum,TicketID,TicketNum,BulletID,BulletNum,FightAward,Enounce,Charge,WinRule";
}