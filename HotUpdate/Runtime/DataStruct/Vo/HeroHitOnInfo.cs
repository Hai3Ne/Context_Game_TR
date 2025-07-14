using System;

public class AttackClipInfo{
	public float length;
	public float[] hitTimes;
}

public class HeroHitOnInfo
{
	public uint heroCfgID;
	public AttackClipInfo[] hitClips;
}
