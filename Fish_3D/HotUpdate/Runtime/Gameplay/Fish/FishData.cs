using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GroupData {
    [Header("鱼的cfgID")]
    public uint FishCfgID;
    [Header("鱼的数量")]
    public ushort FishNum;
    [Header("鱼的模型比例")]
    public float FishScaling;
    [Header("鱼的移动速度")]
    public float SpeedScaling;
    [Header("鱼的动画速度")]
    public float ActionSpeed = 1;
    [Header("是否播放swim动画")]
    public bool ActionUnite;
    [Header("鱼的形状索引 注：更改此项后PosList、FishNum相关属性失效")]
    public uint FishShapeID;
    [Header("生成鱼的密度")]
    public uint Density = 1;//
    [Header("形状偏移值")]
    public Vector3 ShapeOffset;//
    [Header("形状缩放值")]
    public float ShapeScale = 1;
    public Vector3[] PosList = null;
    [Header("延迟发射")]
    public float[] DelayList;
}

[Serializable]
public class FishPathGroupData
{
    [Header("对应路径组配置下标")]
    public ushort PathGroupIndex;
    [Header("鱼的移动速度")]
    public float Speed;
    [Header("鱼的cfgID")]
	public uint FishCfgID;
    [Header("鱼的模型比例")]
    public float FishScaling;               //特效大小
    [Header("鱼的动画速度")]
    public float ActionSpeed = 1;           //鱼的动画速度
    [Header("是否播放swim动画")]
    public bool ActionUnite;                //是否播放Base Layer.Swim动画
}

[Serializable]
public class FishParadeData
{
	[Header("渔阵ID")]
	public uint FishParadeId;
	[Header("渔阵列表")]
	public GroupData[] GroupDataArray;// 鱼群组 

	[Header("渔阵对应的路径ID列表")]
	public uint[] PathList;
	public Vector3 FrontPosition;
	public byte[] FishGraphicData;
}

//首次出场渔阵
[Serializable]
public class OpeningParadeData
{
	public float delay;
	public FishParadeData mFishParade;
}

public enum FishClipType
{
    CLIP_YOUYONG = 0,
    CLIP_DEAD,
    CLIP_LAUGH,
    CLIP_BEATTACK,
	CLIP_ATTACK,
	CLIP_Dizzy,
	CLIP_IDLE,
	CLIP_MAX,
   
};

public enum PathEventType
{
	NONE = 0,
	STAY,     //停留
	LAUGH,    //嘲讽
	BEATTACK,   //受击
	ACTTACK,	//攻击
	ANIMATIONS
}

public class FishAnimatorStatusMgr
{
	public static int YouYongHashName 	= Animator.StringToHash("Swim");
	public static int IdleHashName 	= Animator.StringToHash("Idle");

    public static int DeadHashName = Animator.StringToHash("Dead");
	public static int LaughHashName 	= Animator.StringToHash("Laugh");
    public static int BeAttackHashName = Animator.StringToHash("BeAttack");
	private static int AttackHashName 	= Animator.StringToHash("Attack");
	private static int DizzyHashName 	= Animator.StringToHash("Dizzy");


	public const string STATUS_SUBCLIP = "SubClip";
	public const string ANYSTATE_TRIGGER = "trigger";

	public static int[] ActionHashList = new int[] {
		YouYongHashName,
		DeadHashName,
		LaughHashName,
		BeAttackHashName,
		AttackHashName,
		DizzyHashName,
		IdleHashName,
	};

	static FishClipType[]  PathEvent2ClipList = new FishClipType[]{
		
		FishClipType.CLIP_YOUYONG,
		FishClipType.CLIP_IDLE, 
		FishClipType.CLIP_LAUGH, 
		FishClipType.CLIP_BEATTACK,
		FishClipType.CLIP_ATTACK,
		FishClipType.CLIP_MAX, 
	};

	public static bool IsOnceStatus(FishClipType cliptype)
	{
		return cliptype == FishClipType.CLIP_BEATTACK;
	}

	public static bool IsOnceStatus(byte cliptype)
	{
		return cliptype == (byte)FishClipType.CLIP_BEATTACK;
	}

	public static FishClipType PathEvent2FishClip(PathEventType evtType)
	{
		int i = (byte)evtType;
		return PathEvent2ClipList [i];
	}
}

[Serializable]
public class ResFishData
{
    public uint FishIndex;
    public Vector3 Size;
    public float[] ClipLength = new float[(int)FishClipType.CLIP_MAX];
};
