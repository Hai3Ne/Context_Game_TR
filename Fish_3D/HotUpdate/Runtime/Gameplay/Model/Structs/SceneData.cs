using UnityEngine;
using System.Collections.Generic;

public enum SkillType
{
    SKILL_LIGHTING,
    SKILL_FREEZE,
    SKILL_TORNADO,
    SKILL_DISASTER,
    SKILL_LOCK,
    SKILL_MAX
}

//捕获金币的类型
public enum EnumCatchedType
{
    CATCHED_BULLET, //子弹捕获
    CATCHED_SKILL,  //技能捕获
	CATCHED_HERO, //英雄捕获
	CATCHED_OTHER,
}

public class CatchedData
{
	public byte     ClientSeat{ get; set;}     	//客户端座位
	public EnumCatchedType  CatchType;  //捕获的类型
    public uint     SubType;        	//子类型：子弹的类型，技能的类型ID
    public int      GoldNum;        	//此次获取的总金币数量
	public uint     RateValue;      	//当前的倍率
    public List<CatchFishData> FishList;//捕获的鱼列表。
	public List<CatchFishData> ColliderFishlist;
    public byte     DeadNum;        	//当前捕获列表中已经播放完死亡特效的数量
    public byte     GoldFinished;   	//金币到达终点的数量。
	public float 	Delay = 0f;
	public Dictionary<ushort, int> FishGoldMap = new Dictionary<ushort, int>();
	public int	GetFishGoldNum(ushort fishid)
	{
		int gold = 0;
		FishGoldMap.TryGetValue (fishid, out gold);
		return gold;
	}
}

public class ColliderTestInputData
{
	public Vector3 inputScreenPos;
	public ushort tarFishID = 0xFFFF;
	public byte tarFishPart = 0xFF;
	public short angle;
	public uint itemCfgID;
	public ushort skillId;
	public uint skillCfgID;
	public byte Handle;
	public ushort serverSeat;
}


public class FishNormalMaterial
{
    public Shader shaer;
    public Texture2D baseTex;
}
public class FishSkillMaterial
{
    public Shader shader;
    public Texture2D baseTex;
    public Texture2D extraTex;
}
public enum BlendType
{
	SIMPLE,
    BLEND_COLOR,
    BLEND_ADD_TEX,
    BLEND_LERP_TEX,
    BLEND_DISSOLVE_TEX,
}
public class BlendData
{
    public BlendData(Color blendColor, float factor, float d1, float d2, float d3)
    {
        Blend_Type = (byte)BlendType.BLEND_COLOR;
        BlendColor = blendColor;
        Factor = factor;
        Duration1 = d1;
        Duration2 = d2;
        Duration3 = d3;
    }
    public BlendData(BlendType bt, float factor, float d1, float d2, float d3, Texture2D effect, Texture2D baseTex = null)
    {
        Blend_Type = (byte)bt;
        Factor = factor;
        Duration1 = d1;
        Duration2 = d2;
        Duration3 = d3;
        EffectTex = effect;
        BaseTex = baseTex;
    }
    public BlendData()
    {

    }
    public float    Factor;
    public float    Duration1;
    public float    Duration2;
    public float    Duration3;
    public byte     Blend_Type;
    public Color     BlendColor;
    public Texture2D EffectTex;
    public Texture2D BaseTex;
}
public struct CatchFishData
{
    public uint     FishCfgID;
    public Fish     FishObj;
	public FishVo   Vo;
    public bool IsValidFishType()
    {
		return FishConfig.IsVaildFishCfgID (FishCfgID);
    }
}
public struct CatchBulletData
{
    public uint     LcrMegerID;
	public uint     RateValue;
	public EffectVo[] ExtraBuff;
    public Bullet   BulletObj;
    public bool IsValid()
    {
		return BulletObj != null || FishConfig.IsVaildFishCfgID (LcrMegerID);
    }
}