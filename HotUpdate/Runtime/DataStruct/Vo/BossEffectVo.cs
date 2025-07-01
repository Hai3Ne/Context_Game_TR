using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class BossEffectVo
{
	public uint BossID;
	public uint Atk0Eff;
	public uint Atk1Eff;
	public uint Atk2Eff;
	public uint LaughEff;
	public uint[] BulletIDs;
	public uint ShakeEff0;
	public uint ShakeEff1;
	public uint ShakeEff2;
	public const string CLSID="BossID,Atk0Eff,Atk1Eff,Atk2Eff,LaughEff,BulletIDs,ShakeEff0,ShakeEff1,ShakeEff2";
}