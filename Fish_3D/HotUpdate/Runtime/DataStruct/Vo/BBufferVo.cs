using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class BBufferVo
{
	public uint BBuffCfgID;
	public int BulletNum;
	public float Duration;
	public uint[] EffectID;
	public uint LCRHalo;
	public uint BulletHalo;
	public uint AppearAnim;
	public uint GetAudio;
	public uint HitAnim;
	public const string CLSID="BBuffCfgID,BulletNum,Duration,EffectID,LCRHalo,BulletHalo,AppearAnim,GetAudio,HitAnim";
}