using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class LotteryVo
{
	public uint CfgID;
	public bool IsOpen;
	public uint RefreshTime;
	public uint LotteryTicketID;
	public uint TicketNum;
	public uint ComboNum;
	public float ComboDiscount;
	public uint LotteryScore;
	public const string CLSID="CfgID,IsOpen,RefreshTime,LotteryTicketID,TicketNum,ComboNum,ComboDiscount,LotteryScore";
}