using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 单条路径编辑器
 */
#if UNITY_EDITOR
public class CPathLinearRender : MonoBehaviour 
{
    public PathLinearInterpolator                                   Data;
    public List<SPoint>                                             SPointList = new List<SPoint>();

    public static CPathLinearRender                                 CurrentSelectedObj;

    public bool                                                     IsDrawPath = false;
    public bool                                                     IsAutoVisOnSelect = true;
    public List<bool> NodeVisble = new List<bool>();

    public float                                                    m_PostLaunch = 0f; //延迟发射时间

    private FishPathGroupData                                       m_FishPathGroupData = null;

    private Color                                                   m_KeySampleColor = Color.green;
    const float                                                     ConstKeySameplSize = 15f;

    public bool IsKeySamplePointsChanged { get; set; }


	public BossPathLinearInterpolator MBossPathData = null;
	public BossPathLinearInterpolator BossPathData
	{
		get {	return MBossPathData; }
		set 
		{	
			MBossPathData = value;
		}
	}
	public float pathDuration = 0f;

	public KeyValuePair<int,int>[] nodeEventList = null;

	[NonSerialized]
	public bool NeedReInterpolate = false;
    // Use this for initialization
	void Start () {
	
	}


	public static void OnlyShow(CPathLinearRender target)
	{
		CFishPathEditor.SettGroupPathOther ();
		if (CPathLinearRender.CurrentSelectedObj != null && CPathLinearRender.CurrentSelectedObj.IsAutoVisOnSelect) 
		{
			if (target.gameObject != CPathLinearRender.CurrentSelectedObj.gameObject) {
				CPathLinearRender.CurrentSelectedObj.gameObject.SetActive (false);
				CPathLinearRender.CurrentSelectedObj.IsSelected = false;
			}
		}

		if (target.IsAutoVisOnSelect)
		{
			target.gameObject.SetActive(true);
		}
		CPathLinearRender.CurrentSelectedObj = target;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdatePosAndRot()
	{
		if (this.transform.childCount <= 0)
			return;
		if (HasKeySample) {
			if (NeedReInterpolate)
				Interpolator ();

        } else {
            short nodeIdx = Data.m_SplineDataList[0].nodeIdx;
            float timeScaling = Data.m_SplineDataList[0].timeScaling;
            SplineSampleData[] sampdata = new SplineSampleData[this.transform.childCount];
            for (int i = 0; i < this.transform.childCount; i++) {
                Transform child = this.transform.GetChild(i);
                if (i < Data.m_SplineDataList.Length) {
                    nodeIdx = Data.m_SplineDataList[i].nodeIdx;
                    timeScaling = Data.m_SplineDataList[i].timeScaling;
                }
                sampdata[i] = new SplineSampleData();
                sampdata[i].pos = child.localPosition;
                sampdata[i].rot = child.localRotation;
                sampdata[i].timeScaling = timeScaling;
                sampdata[i].nodeIdx = nodeIdx;
            }
			Data.m_SplineDataList = sampdata;
		}
		Data.m_WorldMatrix = this.transform.localToWorldMatrix;
		Data.m_WorldRotation = this.transform.rotation;
	}


	void ClearSpheresList()
	{
		Transform container = this.transform.GetChild(0);

		while (container.childCount > 0)
		{
			GameObject cgo = container.GetChild (0).gameObject;
			if (Application.isPlaying)
				GameObject.Destroy(cgo.gameObject);
			else
				GameObject.DestroyImmediate(cgo.gameObject);
			
		}
	}

	public static List<GameObject> listShpereList = new List<GameObject>();
	[NonSerialized]
	public bool lastSelectChild = false;
    void OnDrawGizmos() {
#if UNITY_EDITOR
        if (lastSelectChild == false && (UnityEditor.Selection.activeTransform != null && UnityEditor.Selection.activeTransform.parent == this.transform))
            lastSelectChild = true;

        if (IsDrawPath) {
            DrawLine();
        }
        Vector3[] pos_arr;
        Vector3[] vearr;
        if (HasKeySample) {
            pos_arr = Data.keySamplePoints;

            Gizmos.color = Color.green;
            foreach (var item in pos_arr) {
                Gizmos.DrawWireSphere(item, 15f);
            }

            //for (int i = 0; i < Data.m_SplineDataList.Length; i++) {
            //    Vector3 wp = Data.m_WorldMatrix.MultiplyPoint(Data.m_SplineDataList[i].pos);
            //    Gizmos.color = Color.Lerp(Color.black, Color.red, (Data.m_SplineDataList[i].timeScaling * 1.0f / 2f));
            //    Gizmos.DrawWireSphere(wp, 1f);
            //}
        } else {
            pos_arr = new Vector3[Data.m_SplineDataList.Length];
            for (int i = 0; i < Data.m_SplineDataList.Length; i++) {
                pos_arr[i] = Data.m_SplineDataList[i].pos;
            }
        }
        vearr = iTween.PathControlPointGenerator(pos_arr);
        float total_len = 0;
        for (int i = 0; i < vearr.Length - 1; i++) {
            total_len += Vector3.Distance(vearr[i], vearr[i + 1]);
        }
        int len = Mathf.FloorToInt(total_len / 10);
        for (float i = 0; i < len; i++) {
            Gizmos.color = Color.Lerp(Color.red, Color.yellow, (Data.mAnimCurve.Evaluate(i / len) - 1) * 0.3f);
            Gizmos.DrawWireSphere(iTween.Interp(vearr, i / len), 1f);
        }

        if (NodeVisble.Count > 0) {
            for (int i = 0; i < NodeVisble.Count; i++) {
                if (NodeVisble[i]) {
                    NodeEvent node = Data.mEventList[i];
                    float perr = i * 1.0f / NodeVisble.Count;
                    Gizmos.color = Color.Lerp(Color.yellow, Color.green, perr);
                    Gizmos.DrawWireSphere(iTween.Interp(vearr, i / len), 3f);
                }
            }
        }
#endif
    }
/*
	[ContextMenu("DrawInGameView")]
	void DrawInGameView()
	{
		Transform container = this.transform.GetChild(0);
		ClearSpheresList();
		for (int i = 0; i < Data.m_SplineDataList.Length; i++) 
		{
			if (i % CFishPathEditor.only.disInterv == 0) {
				Vector3 wp = Data.m_WorldMatrix.MultiplyPoint(Data.m_SplineDataList[i].pos);
				GameObject sgo = GameObject.Instantiate (CFishPathEditor.only.sphereGo);
				sgo.transform.SetParent(container);
				sgo.transform.position = wp;
			}
		}
	}
	//*/



	public void childKeySampleFromParent()
	{
		if (Data == null) return;

		if (transform.childCount <= 0)
			return;
		
		if (!HasKeySample)
			return;
		Vector3[] worldposes = new Vector3[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			worldposes [i] = child.position;
		}

		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild (i);
			child.position = worldposes [i];
		}
	}

	public void OnSPointRotationUpdate()
	{
		IsKeySamplePointsChanged = true;
	}

	public bool isAutoAdjustPointDirection = true;
	public float offsetAngle = 0f;
    public void RefreshData() 
    {
        if (Data == null) return;

        if (IsKeySamplePointsChanged) { UpdateKeySamplePointsData(); }
        
        SPointList.Clear();
        //CleanChildGameObject();
        List<GameObject> delGoes = new List<GameObject>();
        SPoint sp = null;
        if (HasKeySample) 
        {
           // UnityEngine.Debug.Log("PathLinearRender HasKeySample num:" + Data.keySamplePoints.Length);
            int i = 0;
			Vector3 pointForwardDir = Vector3.zero;
            for (i = 0; i < Data.keySamplePoints.Length; i++)
            {
                //GameObject pointGO = new GameObject(i.ToString());
                //pointGO.transform.SetParent(transform);
                //SPoint sp = pointGO.AddComponent<SPoint>();
                if (i >= transform.childCount)
                {
                    GameObject pointGo = new GameObject(i.ToString());
                    pointGo.transform.SetParent(this.transform);
                    sp = pointGo.AddComponent<SPoint>();
                }
                else
                {
                    Transform pointTrans = transform.GetChild(i);
                    sp = pointTrans.GetComponent<SPoint>();
                }

                sp.transform.localPosition = Data.keySamplePoints[i]; 
				sp.forwardRot = (Data.keyPointsAngles == null || i >= Data.keyPointsAngles.Length) ? 
						0 : Data.keyPointsAngles [i];
                SPointList.Add(sp);

				if (isAutoAdjustPointDirection) {
					if (i + 1 < Data.keySamplePoints.Length) {
						pointForwardDir = Data.keySamplePoints [i + 1] - Data.keySamplePoints [i];
						sp.transform.right = pointForwardDir.normalized;
						sp.cachedRot = sp.transform.localRotation;
					}
				}
            }
			if (sp != null) {
				sp.transform.right = pointForwardDir;
				sp.cachedRot = sp.transform.localRotation;
			}

            while (i < transform.childCount)
            {
                delGoes.Add(transform.GetChild(i).gameObject);
                i++;
            }
        }
		else 
        {
           // UnityEngine.Debug.Log("PathLinearRenderer SplineDatalist num:" + Data.m_SplineDataList.Length);
            int i = 0;
            if (null != Data.m_SplineDataList)
            {
                for (i = 0; i < Data.m_SplineDataList.Length; i++)
                {
                    //GameObject pointGO = new GameObject(Data.m_SplineDataList[i].nodeIdx.ToString());
                    //pointGO.transform.SetParent(this.transform);
                    //SPoint sp = pointGO.AddComponent<SPoint>();
                    if (i >= transform.childCount) {
                        GameObject pointGo = new GameObject(i.ToString());
                        pointGo.transform.SetParent(this.transform);
                        sp = pointGo.AddComponent<SPoint>();
                    }
                    else
                    {
                        Transform pointTrans = transform.GetChild(i);
                        sp = pointTrans.GetComponent<SPoint>();
                    }

                    sp.transform.position = Data.m_WorldMatrix.MultiplyPoint(Data.m_SplineDataList[i].pos);
					sp.transform.rotation = Data.m_WorldRotation * Data.m_SplineDataList [i].rot;
					sp.cachedRot = sp.transform.localRotation;
                    SPointList.Add(sp);
                }
            }

            while (i < transform.childCount)
            {
                delGoes.Add(transform.GetChild(i).gameObject);
                i++;
            }
        }

        while (delGoes.Count > 0)
        {
            GameObject.DestroyImmediate(delGoes[0].gameObject);
            delGoes.RemoveAt(0);
        }
    }


    public void RemoveKeyPoint(int i)
    {
		#if UNITY_EDITOR
        if (i < transform.childCount && i >= 0)
        {
            Transform child = transform.GetChild(i);
            GameObject.DestroyImmediate(child.gameObject);
            IsKeySamplePointsChanged = true;
			if (i < transform.childCount)
				UnityEditor.Selection.activeGameObject = transform.GetChild (i).gameObject;

			for(int k = 0; k < this.transform.childCount; k++)
			{
				this.transform.GetChild(k).name = k.ToString();
			}
		}
		#endif
    }


	public void AddKeyPoint(int idx = -1)
	{
		#if UNITY_EDITOR
		if (Data.keySamplePoints == null || Data.keySamplePoints.Length <= 0) {
			UnityEditor.EditorUtility.DisplayDialog ("", "此路径数据不可编辑，因为没有控制点","ok");
			return;
		}

		Vector3 pos = new Vector3 ();
		if (transform.childCount >= 1) {
			pos = this.transform.GetChild (transform.childCount - 1).localPosition;
		}
		var go = new GameObject (transform.childCount+"");
		SPoint sp = go.AddComponent<SPoint> ();
		go.transform.SetParent (this.transform);
		go.transform.localPosition = pos + new Vector3 (UnityEngine.Random.Range (-5f, 5f), UnityEngine.Random.Range (-5f, 5f), UnityEngine.Random.Range (-5f, 5f));
		if (idx > 0) {
			go.transform.SetSiblingIndex (idx);
		}
		List<Vector3> tmp = new List<Vector3>(Data.keySamplePoints);
		tmp.Add (go.transform.localPosition);
		Data.keySamplePoints = tmp.ToArray ();

		UnityEditor.Selection.activeGameObject = go;
		IsKeySamplePointsChanged = true;
		if(idx >= 0)
		{
			for(int i = 0; i < this.transform.childCount; i++)
			{
				this.transform.GetChild(i).name = i.ToString();
			}
			
		}
		#endif
	}

	const float mRefrecenDistance = 5f;
    //List<Quaternion> angleList = new List<Quaternion>();

	public bool IsSelected { get; set;}
    [ContextMenu("Interpolator")]
    public void Interpolator()
    {
		if (Data.keySamplePoints.Length == 0) {
			for (int i = 0; i < Data.m_SplineDataList.Length; i++)
			{
				Quaternion rot = Quaternion.identity;
				if (i < Data.m_SplineDataList.Length - 1)
				{
					Vector3 ddir = (Data.m_SplineDataList [i + 1].pos - Data.m_SplineDataList [i].pos).normalized;
					Vector3 vv0 = new Vector3 (ddir.x, 0f, ddir.z).normalized;
					rot = Quaternion.FromToRotation (vv0, ddir) * Quaternion.FromToRotation(Vector3.right, vv0);
					Data.m_SplineDataList [i].rot = rot;
				}
			}
			return;
		}

        FilterChildTrans ();
        if (currentTransList == null || currentTransList.Length < 2)
            return;
        List<Vector3> vects = new List<Vector3>();
        vects.Add(currentTransList[0].position);
        for (int i = 1; i < currentTransList.Length; i++) {
            vects.Add(currentTransList[i].position);
        }

		float sampleDistance = 0f;
		Data.m_SplineDataList = new SplineSampleData[vects.Count];

		for (int i = 0; i < Data.m_SplineDataList.Length; i++)
        {
            Quaternion rot = Quaternion.identity;
			if (i < Data.m_SplineDataList.Length - 1)
            {
				Vector3 ddir = (vects [i + 1] - vects [i]).normalized;
				Vector3 vv0 = new Vector3 (ddir.x, 0f, ddir.z).normalized;
				rot = Quaternion.FromToRotation (vv0, ddir) * Quaternion.FromToRotation(Vector3.right, vv0);
            }

            if (i > 0) {
                sampleDistance += Vector3.Distance(vects[i], vects[i - 1]);
            }

            Data.m_SplineDataList[i] = new SplineSampleData();
            Data.m_SplineDataList[i].pos = this.transform.InverseTransformPoint(vects[i]);
            Data.m_SplineDataList[i].rot = rot;
        }

        Data.m_SampleMaxDistance = sampleDistance;
        UpdateKeySamplePointsData();
        NeedReInterpolate = false;
        if (BossPathData != null && BossPathData.bossCfgID > 0) {
            BossPathData.duration = CalPathDuration();
        }
    }

	public float CalPathDuration ()
	{
		FishVo fishvo = CFishPathEditor.only.GetFishVo (BossPathData.bossCfgID);
		FishAnimtorStatusVo animStatVo = CFishPathEditor.only.GetFishAnimtorVo (fishvo.SourceID);
		if (fishvo != null) {
			float delta = 0.02f;
            float m_DistTimeScaling = PathTimeController.DIST_TIME_SCALING / PathData.m_SampleMaxDistance;
			float m_Time = 0;
			int k = 0;
			float pathTimeScaling = 0f;
			float extTimes = 0f;

			while (m_Time < 1f) {
				pathTimeScaling = PathData.GetTimeScaling (m_Time);
				float t = fishvo.Speed * m_DistTimeScaling * pathTimeScaling * delta;
				m_Time += t;
				k++;
            }
            foreach (var item in PathData.mEventList) {
                switch ((PathEventType)item.EventType) {
                    case PathEventType.STAY:
                        extTimes += item.Times * 0.001f;
                        break;
                    case PathEventType.LAUGH:
                        if (animStatVo.Laugh > 0f) {
                            extTimes += item.Times * animStatVo.Laugh;
                        }
                        break;
                    case PathEventType.ACTTACK:
                        if (animStatVo.Attack > 0f) {
                            extTimes += item.Times * animStatVo.Attack;
                        }
                        break;
                    case PathEventType.ANIMATIONS:
                        ushort animaID = item.Times;
                        BossPathEventVo[] evtVos = CFishPathEditor.only.GetBossEventVo((uint)animaID);
                        if (evtVos != null && evtVos.Length > 0) {
                            Array.ForEach(evtVos, x => calAnimatimes(animStatVo, x.EventType, x.EventTimes, ref extTimes));
                        }
                        break;
                }
            }
			return k * delta + extTimes;
		}
		return 0f;
	}

	void calAnimatimes(FishAnimtorStatusVo animStatVo, byte EventType, uint times, ref float extTimes) {
		if (EventType == (byte)PathEventType.LAUGH) {
			extTimes += animStatVo.Laugh * times;
		} else if (EventType == (byte)PathEventType.ACTTACK) {
			extTimes += animStatVo.Attack * times;
		} else if (EventType == (byte)PathEventType.STAY) {
			extTimes += times*0.001f;
		}
	}


	public void LaunchFish(FishPathGroupData testData = null) 
    {
		if (testData == null)
		{
			testData = CFishPathEditor.only.testPathData;
		}

		if (testData != null) //有配置信息,读取配置信息
        {
			byte defClip = BossPathData != null ? BossPathData.defaultSwinClip :(byte)0;
			LogMgr.Log("CPathLinearRender 有配置信息,读取配置信息 !!!");
			Fish fish = new Fish();
			FishVo mFishVo = CFishPathEditor.only.GetFishVo (testData.FishCfgID);
			float fscale = mFishVo.Scale * testData.FishScaling;
			float fspeed = mFishVo.Speed * testData.Speed;
			fish.Init(++CFishPathEditor.FishID, 
				testData.FishCfgID, 
				fscale, 0, 
				testData.ActionSpeed, 
				fspeed, Data, defClip);
			//fish.AddElapsedTime(0.1f);

			fish.SetPostLaunch(m_PostLaunch);

			SetFish(fish);
        }
    }

	public uint currentPathID = 0;
    public PathLinearInterpolator PathData
    {
        set
        {
            if (null == value) return;
			Data = value;
			NeedReInterpolate = false;
			if (HasKeySample && Data.m_SplineDataList.Length <= Data.keySamplePoints.Length)
				NeedReInterpolate = true;
			currentPathID = Data.pathUDID;
				
        }
        get
        {
            return Data;
        }
    }

    private void UpdateKeySamplePointsData()
    {
		if (!HasKeySample)
			return;
        Data.keySamplePoints = new Vector3[transform.childCount];
		Data.keyPointsAngles = new float[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform subTF = transform.GetChild(i);
            subTF.gameObject.name = i.ToString();
            Data.keySamplePoints[i] = subTF.localPosition;
			Data.keyPointsAngles[i] = subTF.GetComponent<SPoint>().forwardRot;
        }

        IsKeySamplePointsChanged = false;
    }
    
    public bool HasKeySample
    {
        get
        {
            return Data != null && Data.keySamplePoints != null && Data.keySamplePoints.Length > 0;
        }
    }

    private void CleanChildGameObject() 
    {
        for (int i = 0; i < transform.childCount; ++i) 
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public FishPathGroupData FishPathGroupData
    {
        set 
        {
            m_FishPathGroupData = new FishPathGroupData();
            m_FishPathGroupData.PathGroupIndex = value.PathGroupIndex;
            m_FishPathGroupData.Speed = value.Speed;
            m_FishPathGroupData.FishCfgID = value.FishCfgID;
            m_FishPathGroupData.FishScaling = value.FishScaling;
            m_FishPathGroupData.ActionSpeed = value.ActionSpeed;
            m_FishPathGroupData.ActionUnite = value.ActionUnite;
        }
        get { return m_FishPathGroupData; }
    }

    private void SetFish(Fish fish) 
    {
        if (null != fish)
        {
            CFishPathEditor fpe = gameObject.GetComponentInParent<CFishPathEditor>();

            if (fpe)
            {
                fpe.AddFishToList(fish);
            }
        }
    }

	void DrawLine()
	{
		FilterChildTrans ();
		if (currentTransList.Length > 1)
			iTween.DrawPath (currentTransList);
	}

	Transform[] currentTransList = null;
	void FilterChildTrans()
	{
		currentTransList = new Transform[this.transform.childCount];
		for (int i = 0; i < currentTransList.Length; i++) {
			currentTransList [i] = this.transform.GetChild (i);
			currentTransList [i].name = i.ToString ();
		}
	}
}
#endif