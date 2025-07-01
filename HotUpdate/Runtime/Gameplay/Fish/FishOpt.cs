using UnityEngine;
using System.Collections.Generic;

public enum FishOptState
{
    FOS_DELAY,
    FOS_FIRST,
    FOS_UPDATE,
}
public enum FishDeadType
{
    DEAD_NONE,
    DEAD_ACTION,        //播放死亡动作
    DEAD_IMMEDIATE,     //立即死亡
}
public enum FishOptType
{
    FOT_ACTION,
    FOT_REDUCTION,
    FOT_PATH,
    FOT_OFFSET,
    FOT_TIMER,
	FOT_EFFECT,
}

public abstract class IFishOpt
{
    private byte        m_OptType;
    private float       m_Delay = 0.00001f;
    private byte        m_DeadType = (byte)FishDeadType.DEAD_NONE;
    private CatchedData m_CatchData;
	private bool m_isDestroy = false;
	public bool isDestroy
	{
		get { return m_isDestroy;}
		set { m_isDestroy = value; }
	}
    public IFishOpt(FishOptType opt)
    {
        OptType = opt;
    }
    public FishOptType OptType
    {
        get { return (FishOptType)m_OptType; }
        set { m_OptType = (byte)value; }
    }
    public CatchedData CatchData
    {
        get { return m_CatchData; }
        set { m_CatchData = value; }
    }

    public FishDeadType DeadType
    {
        get { return (FishDeadType)m_DeadType; }
        set { m_DeadType = (byte)value; }
    }

    public float Delay
    {
        get
        {
            return m_Delay;
        }
        set
        {
            if (value <= 0.0f)
                m_Delay = 0.00001f;
            else
                m_Delay = value;
        }
    }
    public FishOptState CheckDelay(float delta)
    {
        if (m_Delay > 0)
        {
            m_Delay -= delta;
            if (m_Delay <= 0)
                return FishOptState.FOS_FIRST;
            else
                return FishOptState.FOS_DELAY;
        }
        else
            return FishOptState.FOS_UPDATE;
    }

    public void SetDeadData(float delay, bool deadAction, CatchedData cd)
    {
        Delay = delay;
        if(deadAction)
            DeadType = FishDeadType.DEAD_ACTION;
        else
            DeadType = FishDeadType.DEAD_IMMEDIATE;
        this.CatchData = cd;
    }

    public bool Finish(Fish fish)
    {
        if (DeadType == FishDeadType.DEAD_ACTION)
        {
            if (OptType == FishOptType.FOT_ACTION)
            {
                LogMgr.Log("Action不能再使用死亡动作");
                return true;
            }
			fish.Controller.OrignalSpeed = 0.0f;
            FishOptAction foa = new FishOptAction(FishClipType.CLIP_DEAD, 1.0f, 0, false);
            foa.DeadType = FishDeadType.DEAD_IMMEDIATE;
            foa.CatchData = CatchData;
            fish.AddOpt(foa);

            if (m_CatchData != null)
            {
                ++m_CatchData.DeadNum;
				SceneLogic.Instance.EffectMgr.FishCatched(fish, m_CatchData);
            }
            return true;
        }
        else if (DeadType == FishDeadType.DEAD_IMMEDIATE)
        {
            //调用接口
            if (OptType != FishOptType.FOT_ACTION && m_CatchData != null)
            {
                ++m_CatchData.DeadNum;
				SceneLogic.Instance.EffectMgr.FishCatched(fish, m_CatchData);
            }
            return false;
        }
        else
            return true;
    }
    public abstract void Init(Fish fish);
    public abstract bool Update(float delta, Fish fish, out bool bRemove);
}


//可只改变游动的速度。
public class FishOptAction:IFishOpt
{
    byte    m_ActionType;
    float   m_Time;
    float   m_Duration;
    float   m_NewSpeed;
    bool 	m_bAddGold;
	int     m_SubClipIdx = 0;
	int  m_times = 1;
	float oldSubClipIdx = 0;
	ActionSpeedAlter mActionSpeedAlter;
	public FishOptAction(FishClipType eType, float newSpeed = 1.0f, int subClipIdx = 0,  bool bAddGold = true):base(FishOptType.FOT_ACTION)
    {
        m_NewSpeed = newSpeed;
        m_ActionType = (byte)eType;
        m_bAddGold = bAddGold;
		m_SubClipIdx = subClipIdx;
		m_times = 1;
    }

	public FishOptAction(FishClipType eType, int times = 1):base(FishOptType.FOT_ACTION)
	{
		m_NewSpeed = 1f;
		m_ActionType = (byte)eType;
		m_bAddGold = true;
		m_SubClipIdx = 0;
		m_times = times;
	}

    public override bool Update(float delta, Fish fish, out bool bRemove)
    {
        m_Time += delta;
		if (m_Time >= m_Duration || delta >= 100f)
        {
            if(this.DeadType != FishDeadType.DEAD_ACTION)
            {
				fish.Anim.Set_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP, oldSubClipIdx);
				fish.Anim.Set_Bool(FishAnimatorStatusMgr.ActionHashList[m_ActionType], false);
				fish.RemoveActionSpeed (mActionSpeedAlter);
            }
            bRemove = true;
            return Finish(fish);
        }
        else
        {
            bRemove = false;
        }
        return true;
    }

    public override void Init(Fish fish)
    {
        if (m_ActionType != (byte)FishClipType.CLIP_MAX)
        {
            if (m_ActionType == (byte)FishClipType.CLIP_DEAD)
            {
				fish.ResetActionSpeed ();
				if (this.DeadType == FishDeadType.DEAD_IMMEDIATE && m_bAddGold && this.CatchData != null)
                {
                    //死亡金币。
                    ++this.CatchData.DeadNum;
					SceneLogic.Instance.EffectMgr.FishCatched(fish, this.CatchData);
                }
            }

			oldSubClipIdx = fish.Anim.Get_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP);
			m_Duration = fish.GetClipLength ((FishClipType)m_ActionType, m_SubClipIdx);
			m_Duration *= m_times;
			fish.Anim.Set_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP, (float)m_SubClipIdx);
			fish.Anim.Set_Bool(FishAnimatorStatusMgr.ActionHashList[m_ActionType], true);
			fish.Anim.Set_Trigger (FishAnimatorStatusMgr.ANYSTATE_TRIGGER);
        }
		mActionSpeedAlter = new ActionSpeedAlter (fish.ActionSpeed * m_NewSpeed);
		fish.AddActionSpeed (mActionSpeedAlter);
        
    }
    public void Shutdown()
    {

    }
}

public class FishOptReduction:IFishOpt
{
    ReductionData m_Reduction;
    float   m_ActionSpeed;
    float   m_OldSpeed;
    float   m_AllTime;
    BlendData m_OptData;
    float   m_Time;
    float   m_Delay;
    bool    m_bInitTex;
	ActionSpeedAlter mActionspeedAlter;
    public FishOptReduction(float actionSpeed, ReductionData reduction):base(FishOptType.FOT_REDUCTION)
    {
        m_Reduction = reduction;
        m_ActionSpeed = actionSpeed;
        m_AllTime = reduction.Duration1 + reduction.Duration2 + reduction.Duration3;
		m_Time = 0f;
    }
    public FishOptReduction(float actionSpeed, ReductionData reduction, float delay, BlendData bd):base(FishOptType.FOT_REDUCTION)
    {
        m_Reduction = reduction;
        m_ActionSpeed = actionSpeed;
        m_AllTime = reduction.Duration1 + reduction.Duration2 + reduction.Duration3;
        m_OptData = bd;
        m_Delay = delay;
        m_bInitTex = false;
		m_Time = 0f;
    }
    public override bool Update(float delta, Fish fish, out bool bRemove)
    {
        m_Time += delta;
		if (m_Time >= m_AllTime || delta >= 100f)
        {
			fish.RemoveActionSpeed (mActionspeedAlter);
            if (m_OptData != null)
            {
				if (this.DeadType == FishDeadType.DEAD_NONE) {
					if (m_OptData.Blend_Type == (byte)BlendType.BLEND_COLOR)
						fish.ResetColor ();
					else
						fish.shader = SceneLogic.Instance.EffectMgr.FishEffectShader (BlendType.SIMPLE, fish.shader);
				}
            }
            bRemove = true;
			isDestroy = true;
			fish.Controller.TimeController.ReSetTimeScaling (m_Reduction);
            return Finish(fish);
        }
        else
        {
            if(m_OptData != null)
            {
                if (m_Time > m_Delay)
                {
                    if (!m_bInitTex)
                        InitTex(fish);

                    float time = m_Time - m_Delay;
                    float factor;
                    if (time < m_OptData.Duration1)
                        factor = time / m_OptData.Duration1;
                    else if (time < m_OptData.Duration2 + m_OptData.Duration1)
                        factor = 1.0f;
                    else
                    {
                        factor = (time - m_OptData.Duration2 - m_OptData.Duration1) / m_OptData.Duration3;
                        factor = Mathf.Max(0, 1.0f - factor);
                    }
                    factor *= m_OptData.Factor;
					fish.SetMatFloatVal("_BlendFactor", factor);
                }
            }
//            if(m_ActionSpeed != 1.0f)
  //              fish.ActionSpeed = Utility.LerpFloat(m_ActionSpeed ,m_OldSpeed, fish.Controller.TimeController.DelayFactor);
            bRemove = false;
        }
        return true;
    }

    void InitTex(Fish fish)
    {
        m_bInitTex = true;
        if (m_OptData.Blend_Type == (byte)BlendType.BLEND_COLOR)
        {
			fish.SetColor (m_OptData.BlendColor);
//			fish.shader = SceneLogic.Instance.EffectMgr.FishEffectShader (BlendType.BLEND_COLOR, fish.shader);
//			fish.SetColor("_BlendColor", m_OptData.BlendColor);
        }
        else
        {
			Texture2D mainTex = (Texture2D)fish.GetTexture("_MainTex");
			Texture2D refraceTex = (Texture2D)fish.GetTexture("_RefracTex");

			if (m_OptData.Blend_Type == (byte)BlendType.BLEND_ADD_TEX)
				fish.shader = SceneLogic.Instance.EffectMgr.FishEffectShader (BlendType.BLEND_ADD_TEX, fish.shader);
			else if (m_OptData.Blend_Type == (byte)BlendType.BLEND_LERP_TEX)
				fish.shader = SceneLogic.Instance.EffectMgr.FishEffectShader (BlendType.BLEND_LERP_TEX, fish.shader);
			else if (m_OptData.Blend_Type == (byte)BlendType.BLEND_DISSOLVE_TEX)
				fish.shader = SceneLogic.Instance.EffectMgr.FishEffectShader (BlendType.BLEND_DISSOLVE_TEX, fish.shader);

			fish.SetTexture("_MainTex", m_OptData.BaseTex != null ? m_OptData.BaseTex : mainTex);
			fish.SetTexture("_RefracTex", refraceTex);
            if(m_OptData.EffectTex != null)
				fish.SetTexture("_EffectTex", m_OptData.EffectTex);
        }
    }

    public override void Init(Fish fish)
    {
		fish.Controller.TimeController.AddSkillTimeScaling (m_Reduction, FISH_DELAY_TYPE.DELAY_SKILL);
		mActionspeedAlter = new ActionSpeedAlter (m_ActionSpeed,m_Reduction.Duration1);
		fish.AddActionSpeed (mActionspeedAlter);
        if(m_OptData != null)
        {
            if (m_Delay == 0.0f)
                InitTex(fish);
        }
    }

    public void Shutdown()
    {

    }
}


//效果buff
public class FishOptEffects:IFishOpt
{
	float   m_AllTime;
	float   m_Time;
	float   m_Delay;

	bool inited = false;
	GameObject effectPrefabObj;
	public FishOptEffects(GameObject effectObj, float delay, float duration):base(FishOptType.FOT_EFFECT)
	{
		m_Delay = delay;
		m_AllTime = m_Delay + duration;
		effectPrefabObj = effectObj;
		m_Time = 0f;
		inited = false;
	}

	string effectResId = null;
	public FishOptEffects(string resId, float delay, float duration):base(FishOptType.FOT_EFFECT)
	{
		effectResId = resId;
		m_Delay = delay;
		m_AllTime = m_Delay + duration;
		m_Time = 0f;
	}

	public override bool Update(float delta, Fish fish, out bool bRemove)
	{
		if (inited == false) {
			bRemove = false;
			return true;
		}
			
		m_Time += delta;
		if (m_Time >= m_AllTime || delta >= 100f)
		{
			int prefID = effectPrefabObj.GetInstanceID ();
			if (fish.mCacheEffObjRefMap.ContainsKey (prefID)) {
				fish.mCacheEffObjRefMap [prefID].RefCount--;
			}
			if (fish.mCacheEffObjRefMap.ContainsKey(prefID) && fish.mCacheEffObjRefMap [prefID].RefCount <= 0) {				
				GameObject.Destroy (fish.mCacheEffObjRefMap [prefID].mTrans);	
				fish.mCacheEffObjRefMap.Remove (prefID);
			}
			bRemove = true;
			return Finish(fish);
		}
		else
		{
			if (m_Time > m_Delay)
			{
				int prefID = effectPrefabObj.GetInstanceID ();
				if (fish.mCacheEffObjRefMap.ContainsKey(prefID) && fish.mCacheEffObjRefMap[prefID].RefCount==1)
					fish.mCacheEffObjRefMap[prefID].mTrans.SetActive (true);
			}
			bRemove = false;
		}
		return true;
	}

	public override void Init (Fish fish)
	{
		
		if (effectPrefabObj != null) {
			createInstGoForFish (fish, effectPrefabObj);
			inited = true;
		}
		else
		{
			//Kubility.KAssetBundleManger.Instance.LoadGameObject (effectResId, delegate(SmallAbStruct obj) 
            //{
			//	effectPrefabObj = obj.MainObject  as GameObject;
			//	createInstGoForFish(fish,effectPrefabObj);
			//	inited = true;
			//});

            effectPrefabObj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_3D,effectResId);
            createInstGoForFish(fish, effectPrefabObj);
            inited = true;
        }
	}

	GameObject createInstGoForFish(Fish fish, GameObject prefabGo)
	{
		GameObject effObjInst = null;
		int instId = prefabGo.GetInstanceID();
		if (fish.mCacheEffObjRefMap.ContainsKey(instId))
		{
			effObjInst = fish.mCacheEffObjRefMap[instId].mTrans;
			fish.mCacheEffObjRefMap[instId].RefCount++;
		}else{
			effObjInst = GameObject.Instantiate (prefabGo);	
			fish.mCacheEffObjRefMap [instId] = new GameObjRef ();
			fish.mCacheEffObjRefMap [instId].prefabID = instId;
			fish.mCacheEffObjRefMap [instId].mTrans = effObjInst;
			fish.mCacheEffObjRefMap[instId].RefCount = 1;
			effObjInst.SetActive (m_Delay > 0f);
		}
		FishBuffEffectSetup.SetupDizzy (fish, effObjInst);
		return effObjInst;
	}
}



public class FishOptPath:IFishOpt
{
    class InitData
    {
        public PathLinearInterpolator Path;
        public float Speed;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Vector3 Offset;
    }
    InitData m_InitData;
    float m_TransTime;
    float m_Time;
    bool m_bPathUpdate;
    float m_Scaling;
    float m_TransRotate;
    float m_PathRotate;
    float m_DeadTime;
    bool m_bScaling;
    public FishOptPath(PathLinearInterpolator path, float transRotate, float pathRotate, float speed, float transTime, Vector3 offset, bool bScaling = true):base(FishOptType.FOT_PATH)
    {
        m_InitData = new InitData();
        m_InitData.Path = path;
        m_InitData.Speed = speed;
        m_InitData.Offset = offset;
        m_TransTime = transTime;
        m_TransRotate = transRotate;
        m_PathRotate = pathRotate;
        m_bPathUpdate = false;
        m_bScaling = bScaling;
        m_DeadTime = Utility.Range(0.6f, 0.9f);
    }
    public override  bool Update(float delta, Fish fish, out bool bRemove)
    {
        m_Time += delta;
        bRemove = false;
        if(m_Time < m_TransTime)
        {
            fish.Position = Vector3.Lerp(m_InitData.StartPos, m_InitData.EndPos, m_Time / m_TransTime);
            fish.Rotation = Quaternion.AngleAxis(m_Time * m_TransRotate, Vector3.up);
        }
        else 
        {
            if (m_bPathUpdate == false)
            {
                m_bPathUpdate = true;
                fish.Time = 0.0f;
                m_InitData = null;
            }
            fish.OrgRot = Quaternion.AngleAxis(m_Time * m_PathRotate, Vector3.up);
            if (m_bScaling)
            {
                float scl = Mathf.Min(1.0f, 1.0f - fish.Time);
                scl *= m_Scaling;
                fish.Transform.localScale = new Vector3(scl, scl, scl);
            }
            if (fish.Time >= m_DeadTime)
            {
                bRemove = true;
                return Finish(fish);
            }
        }
        return true;
    }
    public override void Init(Fish fish)
    {
        m_Scaling = fish.Transform.localScale.x;
        m_InitData.StartPos = fish.Position;
		fish.Controller.ResetController(m_InitData.Path, m_InitData.Speed, fish.ActionSpeed, float.MinValue);
        m_InitData.EndPos = m_InitData.Path.m_WorldMatrix.MultiplyPoint(m_InitData.Path.GetPos(0.0f)) + m_InitData.Offset;
        fish.WorldOffset = m_InitData.Offset;
    }
    public void Shutdown()
    {

    }
}

public class FishOptOffset:IFishOpt
{
    public float m_Time;
    public PathLinearInterpolator m_Path;
    public float m_Duration;
    public Vector3 m_Postion;
    public FishOptOffset(float duration, PathLinearInterpolator path):base(FishOptType.FOT_OFFSET)
    {
        m_Duration = duration;
        m_Path = path;
    }
    public override bool Update(float delta, Fish fish, out bool bRemove)
    {
        m_Time += delta;
        if (m_Time >= m_Duration)
        {
            bRemove = true;
            return Finish(fish);
        }
        else
        {
            
            Vector3 pos = m_Postion;
            pos.y += m_Path.GetPos(m_Time / m_Duration).y * 3;
            fish.Position = pos;
            bRemove = false;
            return true;
        }
        
    }
    public override void Init(Fish fish)
    {
        fish.Time = -1000.0f;
        m_Postion = fish.Position;
    }
}

public class FishOptTimer:IFishOpt
{
    public PathLinearInterpolator m_Path;
    public ILifeTimer m_Timer;
    public bool m_LeftToRight;
    public FishOptTimer(ILifeTimer timer, PathLinearInterpolator path, bool leftToRight)
        : base(FishOptType.FOT_TIMER)
    {
        m_Timer = timer;
        m_Path = path;
        m_LeftToRight = leftToRight;
    }

    public override bool Update(float delta, Fish fish, out bool bRemove)
    {
        float x = Camera.main.WorldToScreenPoint(fish.Position).x;

        if (m_LeftToRight)
            x /= Screen.width;
        else
            x = (Screen.width - x) / Screen.width;
        
        float delay = x + 0.5f;
        if (delay <= m_Timer.LifeTime || m_Timer.IsEnd)
        {
            fish.StopAnimEvents();
            float transRot = -Utility.Range(100, 200);
            float pathRot = -Utility.Range(300, 500);
			float speed = Utility.Range(15, 20) * SceneLogic.Instance.FModel.ComputeZScaling(fish, 1.0f);

			FishOptPath fop = new FishOptPath(m_Path, transRot, pathRot, speed, 0, fish.Position, true);
            fop.SetDeadData(0, false, null);
            fish.AddOpt(fop);
            bRemove = true;
        }
        else
            bRemove = false;
        return true;
    }
    public override void Init(Fish fish)
    {
       
    }
}
//*/