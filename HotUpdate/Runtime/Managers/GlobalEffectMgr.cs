using UnityEngine;
using System.Collections.Generic;
using Kubility;
public interface ILifeTimer
{
    float LifeTime
    {
        get;
    }
    bool IsEnd
    {
        get;
    }
}
public interface IGlobalEffect
{
    Vector3 Position
    {
        get;
    }
}

public class ILifeTimerImp : ILifeTimer
{
	public float        Delay;          //延迟多少秒开始播放
	public float        Life;           //持续播放时间
	public float        Time;

	public ILifeTimerImp(float delay = 0.0f, float life = float.MaxValue)
	{
		this.Delay = delay;
		this.Life = life;
	}

	public float LifeTime
	{
		get
		{
			return Time;
		}
	}
	public bool IsEnd
	{
		get
		{
			return Time >= Life + Delay;
		}
	}
}

public class GlobalEffectPosConverter
{
    public static Vector3 LightingPosConvert(Vector3 pos)
    {
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 110;
        pos = Camera.main.ScreenToWorldPoint(pos);
        return pos;
    }
    public static Vector3 FreezePosConvert(Vector3 pos)
    {
        pos.z -= 20;
        return pos;
    }
    public static Vector3 LaserPosConvert(Vector3 pos)
    {
        pos.z -= 20;
        return pos;
    }
}
public class GlobalEffectData : ILifeTimerImp
{
    public delegate Vector3 PosConverter(Vector3 pos);

	public GlobalEffectData(GameObject effect, float delay = 0.0f, float life = float.MaxValue, float scaling = 1.0f) : base(delay, life)
    {
        EffectHost  = null;
        EffectObj   = effect;
        Offset      = Vector3.zero;
        Active      = delay <= 0;
        effect.SetActive(Active);
        if (Active)
            SetScaling(scaling);
        else
            Scaling = scaling;
    }

	public GlobalEffectData(IGlobalEffect host, GameObject effect, float delay = 0.0f, float life = float.MaxValue, PosConverter converter = null, float scaling = 1.0f) : base(delay, life)
    {
        EffectHost = host;
        EffectObj = effect;
        Offset = Vector3.zero;
        Active = delay <= 0;
        effect.SetActive(Active);
        PosConvert = converter;
        if (Active)
            SetScaling(scaling);
        else
            Scaling = scaling;
    }
	public GlobalEffectData(IGlobalEffect host, GameObject effect, Vector3 offset, float delay = 0.0f, float life = float.MaxValue, PosConverter converter = null, float scaling = 1.0f) : base(delay, life)
    {
        EffectHost  = host;
        EffectObj   = effect;
        Offset      = offset;
        Active      = delay <= 0;
        effect.SetActive(Active);
        PosConvert = converter;
        if (Active)
            SetScaling(scaling);
        else
            Scaling = scaling;
    }
    public void SetScaling(float scl)
    {
        ParticleSystem[] ps = EffectObj.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < ps.Length; ++i)
        {
            ps[i].startSize *= scl;
        }
    }

    public float        Scaling;
    public PosConverter  PosConvert;
    public IGlobalEffect EffectHost;    //此特效关联的对象。
    public GameObject   EffectObj;      //特效
    public Vector3      Offset;
    public bool         Active;
	public System.Action FinishCb;
}



public class GlobalEffectMgr:SingleTon<GlobalEffectMgr>,IRunUpdate
{
    List<GlobalEffectData>      m_EffectList = new List<GlobalEffectData>();
	List<ILifeTimerImp> 		m_LifeTimers = new List<ILifeTimerImp>();

    public bool Init()
    {
        return true;
    }

    public void GlobalInit()
    {}

    public void AddEffect(GlobalEffectData effect)
    {
        m_EffectList.Add(effect);
    }

	public void AddLifeTimer(ILifeTimerImp lifeTimer)
	{
		if (!m_LifeTimers.Contains(lifeTimer))
			m_LifeTimers.Add (lifeTimer);
	}

	public void ClearEff(GlobalEffectData effect)
	{
		int i = m_EffectList.IndexOf (effect);
		if (i >= 0) {
			m_EffectList [i].FinishCb.TryCall ();
			m_EffectList [i].FinishCb = null;
			GameObject.Destroy(m_EffectList[i].EffectObj);
			m_EffectList.RemoveAt (i);
		}
	}

    public void Clear()
    {
        for(int i = 0; i < m_EffectList.Count; ++i)
        {
			m_EffectList [i].FinishCb.TryCall ();
			m_EffectList [i].FinishCb = null;
            GameObject.Destroy(m_EffectList[i].EffectObj);
        }
        m_EffectList.Clear();
		m_LifeTimers.Clear ();
    }
    public static void SetEffectOnUI(GameObject effect)
    {
        Renderer renderer = effect.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
            renderer.sharedMaterial.renderQueue = 30000;
    }
    public static void SetMeshRendererOnTopWnd(GameObject effect)
    {
        MeshRenderer[] tr = effect.GetComponentsInChildren<MeshRenderer>();
        if (tr != null)
        {
            foreach (MeshRenderer rr in tr)
                rr.sortingLayerName = "ParsLayer";
        }
    }
    public static void SetTrailEffectOnTopWnd(GameObject effect)
    {
        TrailRenderer[] tr = effect.GetComponentsInChildren<TrailRenderer>();
        if (tr != null)
        {
            foreach(TrailRenderer rr in tr)
                rr.sortingLayerName = "ParsLayer";
        }
    }
    void UpdateEffectPos(GlobalEffectData effect)
    {
        if (effect.PosConvert != null)
            effect.EffectObj.transform.position = effect.PosConvert(effect.EffectHost.Position) + effect.Offset;
        else
            effect.EffectObj.transform.position = effect.EffectHost.Position + effect.Offset;
    }

    public void Update(float delta)
    {
		for(int i = 0; i < m_LifeTimers.Count;)
		{
			m_LifeTimers [i].Time += delta;
			if (m_LifeTimers [i].IsEnd) {
				m_LifeTimers.RemoveAt (i);
				continue;
			}
			i++;
		}

        for(int i = 0; i < m_EffectList.Count; )
        {
            GlobalEffectData effect = m_EffectList[i];
            effect.Time += delta;
            if(!effect.Active)
            {
                if(effect.Time >= effect.Delay)
                {
                    if (effect.EffectHost != null)
                        UpdateEffectPos(effect);
                    effect.Active = true;
                    effect.EffectObj.SetActive(true);
                    effect.SetScaling(effect.Scaling);
                }
                else
                {
                    ++i;
                    continue;
                }
            }
            if(effect.IsEnd)
            {
				try{
					effect.FinishCb.TryCall ();
					effect.FinishCb = null;
				}catch(System.Exception e){
					LogMgr.LogError (e.Message);
				}
                GameObject.Destroy(effect.EffectObj);
                Utility.ListRemoveAt(m_EffectList, i);
                continue;
            }
            if (effect.EffectHost != null)
            {
                UpdateEffectPos(effect);
            }
            ++i;
        }
    }
}
