using System;

public enum EnumLauncherEffectType
{
	REBOUND      = 1,  //反弹
	REDUCTION	 = 2,  //减速
	SWOON		 = 3,  //眩晕
	CROSS_BULLET = 4   //穿透
}

public enum EnumSrvCatchEffectType
{
	REDUCTION	 = 0x1,  //减速
	SWOON		 = 0x2,  //眩晕
	FROZEN		 = 0x4, // 冰冻
}

public enum EnumBuffOP
{
	Set,
	Add,
	Multip,
}