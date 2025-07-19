using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishPathConfData
{
	public List<PathLinearInterpolator>    m_PathInterpList = new List<PathLinearInterpolator>(); // 单条路径
	public List<PathLinearInterpolator>    m_PathInterpListInv = new List<PathLinearInterpolator>(); // 单条路径反向

	public List<PathLinearInterpolator[]>  m_PathGroupList = new List<PathLinearInterpolator[]>();  //路径组
	public List<PathLinearInterpolator[]>  m_PathGroupListInv = new List<PathLinearInterpolator[]>(); //路径组反向

	public PathLinearInterpolator      m_DouDongPath;  // 抖动
	public PathLinearInterpolator[]    m_LongJuanFeng; // 龙转风
	public PathLinearInterpolator      m_BoLang; // 波浪

	public List<FishPathGroupData> m_PathParadeDataList = new List<FishPathGroupData>(); //鱼群
	public List<FishParadeData> m_FishParadeDataList = new List<FishParadeData> (); // 渔阵
}

[System.Serializable]
public class BossPathLinearInterpolator
{
	public uint bossCfgID;
	public PathLinearInterpolator mPath,mPathInv;
	public byte defaultSwinClip;
	public float duration;
	public float delay;
}
