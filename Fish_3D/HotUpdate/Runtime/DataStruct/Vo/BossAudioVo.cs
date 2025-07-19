using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class BossAudioVo
{
	public uint BossID;
	public uint[] AppearAudio;
	public uint[] EscapeAudio;
	public uint[] HurtAudio;
	public uint[] DizzyAudio;
	public uint[] DieAudio;
	public const string CLSID="BossID,AppearAudio,EscapeAudio,HurtAudio,DizzyAudio,DieAudio";
}