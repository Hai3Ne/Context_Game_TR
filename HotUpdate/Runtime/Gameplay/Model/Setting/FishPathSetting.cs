using System;
using System.Collections;
using System.Collections.Generic;

public class FishPathSetting
{
	public static List<OpeningParadeData[]> openingParadeList;
	public static Dictionary<uint, BossPathLinearInterpolator[]> bossPathMap;
	static FishPathConfData FishPathConf;
	public static void Init(FishPathConfData pathdata)
	{
		FishPathConf = pathdata;
	}

	public static List<FishParadeData> ParadeFishGroup {get{ return FishPathConf.m_FishParadeDataList; }}
	public static List<FishPathGroupData> ParadePathGroup {get{ return FishPathConf.m_PathParadeDataList;}}

	public static PathLinearInterpolator[] GetPathGroup(int idx, bool bInverse = false)
	{
		if (bInverse)
			return FishPathConf.m_PathGroupListInv[idx];
		else
			return FishPathConf.m_PathGroupList[idx];
	}

	public static PathLinearInterpolator GetPath(uint pathID, bool bInverse = false)
	{
		PathLinearInterpolator pathline = null;
		if(bInverse)
			pathline = FishPathConf.m_PathInterpListInv.Find(x=>x.pathUDID == pathID);
		else
			pathline = FishPathConf.m_PathInterpList.Find(x=>x.pathUDID == pathID);
		if (pathline == null)
			LogMgr.LogError (pathID+" Has Not exist");
		return pathline;
	}

	public static PathLinearInterpolator GetBossPath(uint bossCfgID, uint pathID, bool bInverse, out byte defSwinClip, out uint duration)
	{
		BossPathLinearInterpolator[] bosspaths = FishPathSetting.bossPathMap [bossCfgID];
		BossPathLinearInterpolator bossPath = Array.Find(bosspaths, x=>x.mPath.pathUDID == pathID);
		if (bossPath == null)
			LogMgr.LogError (pathID+" Has Not exist");
		PathLinearInterpolator pathline = bInverse ? bossPath.mPathInv : bossPath.mPath;
		defSwinClip = bossPath.defaultSwinClip;
		duration = (uint)UnityEngine.Mathf.FloorToInt(bossPath.duration * 1000);
		return pathline;
	}

	public static PathLinearInterpolator BoLang
	{
		get { return FishPathConf.m_BoLang; }
	}

	public static PathLinearInterpolator[] LongJuanFeng
	{
		get { return FishPathConf.m_LongJuanFeng; }
	}

	public static PathLinearInterpolator DouDongPath
	{
		get { return FishPathConf.m_DouDongPath; }
	}
}