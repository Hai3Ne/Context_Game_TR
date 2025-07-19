using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class GameSetting
{
	public float HeroXRange;
	public float HeroYRange;
	public int InitHeroItemID;
	public int InitHeroItemNum;
	public int DetectionRangeHorizontal;
	public int DetectionRangeVertical;
	public int TriggerRangeHorizontal;
	public int TriggerRangeVertical;
	public int BubbleLimit;
	public float BubbleCD;
	public float BubbleLiveTime;
	public float BubbleDifferenceTime;
	public float BubbleGroupRate;
	public float BubbleCheckTime;
	public float ComboCD;
	public int ComboLimit;
	public float ComboRewardCD;
	public float GoldCD;
	public float GoldSpeed;
	public float QuickSell;
	public float SkillLockAccelerate;
	public float SkillLockBaseSpeed;
	public float RollingMessageSpeed;
	public int PityScore;
	public int HDGoldLenth;
	public int KillMultiple;
	public int KillTime;
	public const string CLSID="HeroXRange,HeroYRange,InitHeroItemID,InitHeroItemNum,DetectionRangeHorizontal,DetectionRangeVertical,TriggerRangeHorizontal,TriggerRangeVertical,BubbleLimit,BubbleCD,BubbleLiveTime,BubbleDifferenceTime,BubbleGroupRate,BubbleCheckTime,ComboCD,ComboLimit,ComboRewardCD,GoldCD,GoldSpeed,QuickSell,SkillLockAccelerate,SkillLockBaseSpeed,RollingMessageSpeed,PityScore,HDGoldLenth,KillMultiple,KillTime";
}