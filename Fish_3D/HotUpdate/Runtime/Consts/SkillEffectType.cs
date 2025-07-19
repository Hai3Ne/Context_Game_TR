using System;

public enum SkillCatchChkType
{
	RandomInNumber 	= 1, //全屏范围内指定个数随机
	SustainRange 	= 2, //在制定区域内持续检测捕获
	AllInView 		= 3, //全屏幕随机捕获
	LockCatch 		= 4, //锁定捕获
	PirateBox		= 5, // 海盗保险捕获检测
}

public enum SkillCatchOnEffType
{
	BombArea		= 10,
	CatchOneByOne 	= 11,
	Frozen	  		= 12,
	Dizzy	  		= 13,
	SceneShake		= 14, 
	OverlapEffect 	= 15, //氛围效果
    LCRHaloEffect   = 16,
    LCRSpeed        = 17,
    HitLCR          = 19,//重击炮<提高炮台命中率,参数:0.改变炮台等级  1.子弹捕获范围附加   2.子弹捕获个数附加  3.发射速度/1000>
	FishReduce		= 20,
	AltaMulti		= 21,
	BranchLCR		= 22,
	FreeLCR			= 23,
	SuckUpFish		= 24,
	
}