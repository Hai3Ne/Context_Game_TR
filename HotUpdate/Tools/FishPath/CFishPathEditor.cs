using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
public enum PathType { SinglePath, BossPath, GropPath}
/*
 总编辑器类，数据加载相关等
 */
public class CFishPathEditor : MonoBehaviour 
{
	
	public string                                           ConfigPath = "/Arts/GameRes/Config/Bytes/";
    public const string FishParadeDataFile = "./Assets/Arts/FishPath/FishPath.byte";
	public string                                           OpeningParadeFile = "openingParade.byte";
	public string                                           BossPathsFile = "BossFishPath.byte";

    public static ushort                                    FishID = 0;

	public GameObject sphereGo;
	public float disInterv = 3;

    [SerializeField]
    public FishPathConfData                                 FishData = null;

    public bool                                             Refresh = true; //是否根据数据重新生成列表
    public bool                                             IsRefreshEditorMode = true;

    Camera                                                  MainCamera;
	GameObject                                              PathContainerGO = null;
    GameObject                                              FishContainerGO = null;

    List<GameObject>                                        PathSubobjects = new List<GameObject>();
    int                                                     PathNumber = 0;
    int                                                     GroupNumber = 0;

    static Dictionary<ushort, Fish>                         m_FishList = new Dictionary<ushort, Fish>();
    
    public const string                                            FISH_PATH_NAME = "路径数据";
	public const string                                            FISH_PATH_GROUP_NAME = "路径群数据";
	public const string                                            BOSS_PATH_NAME = "BOSS路径数据";
	public const string                                            FISH_GROUP_NAME = "鱼群数据(一种鱼走多条路径)";
	public const string                                            FISH_GROUPS_NAME = "鱼阵数据（多条鱼走相同单条路径）";
	public const string                                            OPENING_FISH_PARADE_NAME = "出场鱼阵数据";

	public const string 											OPENING_FISH_PARADE_ITEM_NAME = "OpeningPattern";
    const string                                            PATH_GROUP_DATA_NAME 	= "FishGroup";
    const string                                            GROUP_DATA_NAME 		= "FishParade";

	List<OpeningParadeData[]> OpeningDataList = new List<OpeningParadeData[]> ();
	Dictionary<uint, BossPathLinearInterpolator[]> BosspathMap = new Dictionary<uint, BossPathLinearInterpolator[]>();
    void Awake() 
    {
        MainCamera = Camera.main;
		Utility.GlobalInit ();
		Resolution.GlobalInit ();
    }

    // Use this for initialization
	IEnumerator Start () 
    {
        yield return new WaitForSeconds(1.0f);
		GlobalLoading.Instance.ShowProgressUI = true;
		GlobalLoading.Instance.StartPreLoadGameRes (LoadingType.OnlyFishes,delegate {
			GlobalLoading.Instance.close();
		});
		Fish.ENABLE_HIDE_OUTVIEW = false;
	}

	public FishPathGroupData testPathData;

	public static CFishPathEditor only
	{
		get{ return GameObject.FindObjectOfType<CFishPathEditor> ();}
	}
	// Update is called once per frame
	void Update () 
    {
        if (Refresh) 
        {
            ShowPathMode();
            Refresh = false;
        }
		GlobalLoading.Instance.Update (Time.deltaTime);
        UpdateFish(Time.deltaTime);
	}

    void UpdateFish(float deltaTime) 
    {
        if (null == m_FishList || m_FishList.Count <= 0) return;
		Fish[] flist = new Fish[m_FishList.Count];
		m_FishList.Values.CopyTo (flist, 0);
		foreach (Fish f in flist) 
        {
			if (!f.Update (deltaTime)) {
				f.Destroy ();
				m_FishList.Remove (f.FishID);
			}
        }
    }

    public void ReloadData()
    {
		byte[] bytes = File.ReadAllBytes(FishParadeDataFile);
        FishData = FishPathConfParser.ParseData(bytes);
		FishPathSetting.Init(FishData);

		bytes = File.ReadAllBytes(Application.dataPath + ConfigPath + OpeningParadeFile);
		OpeningDataList = FishPathConfParser.UnSerialize_OpeningParades (bytes);

		BosspathMap.Clear ();
		if (File.Exists (Application.dataPath + ConfigPath + BossPathsFile)) {
			bytes = File.ReadAllBytes (Application.dataPath + ConfigPath + BossPathsFile);
			BosspathMap = FishPathConfParser.UnSerialize_BossPath (bytes);
		}

		bytes = File.ReadAllBytes (Application.dataPath + ConfigPath + "Fish.byte");
		FishConfig.Instance.FishConf = (Dictionary<uint, FishVo>)ConfigTables.Instance.Setobject<uint, FishVo>(bytes);

		LogMgr.Log ("成功载入数据...");        
    }

    public void AddFishToList(Fish fish) 
    {
        if (null == fish) return;
        m_FishList[fish.FishID] = fish;
        fish.Controller.ResetPath();
    }

    public static void ClearAllFish() 
    {
        foreach (KeyValuePair<ushort, Fish> pair in m_FishList)
        {
            pair.Value.Destroy();
            //m_FishList.Remove(pair.Key);
        }
        m_FishList.Clear();
    }

    public void ShowPathMode() 
    {
		int i = this.transform.childCount-1;
		while (i >= 0) {
			GameObject delGo = this.transform.GetChild (i).gameObject;
			GameObject.Destroy (delGo);
			i--;
		}
        ReloadData();
        InitFishPath();
    }

	GameObject pahtInvListgo;
    //编辑器模式下的路径(组),鱼群(组)配置列表
    public void EditorPathMode() 
    {
        if (!IsRefreshEditorMode) return;

		FishConfig.Instance.mBossPathEventConf = null;
        ReloadData();

        if (null == FishData) return;

		if (null != PathContainerGO) 
		{
			while (this.transform.childCount > 0) 
			{
				GameObject delGo = this.transform.GetChild (0).gameObject;
				if (Application.isEditor)
					GameObject.DestroyImmediate (delGo);
				else
					GameObject.Destroy (delGo);
			}
		}
        PathContainerGO = new GameObject("配置信息");
        CFishDataConfigManager fishDataConfigManager = PathContainerGO.AddComponent<CFishDataConfigManager>();
        PathContainerGO.transform.SetParent(transform);
        PathContainerGO.transform.position = Vector3.zero;
        PathContainerGO.transform.localScale = Vector3.one;

        GameObject PathListGO = new GameObject(FISH_PATH_NAME);
        GameObject PathGroupListGO = new GameObject(FISH_PATH_GROUP_NAME);
		GameObject BossPathGos = new GameObject (BOSS_PATH_NAME);
		pahtInvListgo = new GameObject ("反向路径");

        PathListGO.transform.SetParent(PathContainerGO.transform);
        PathGroupListGO.transform.SetParent(PathContainerGO.transform);
		BossPathGos.transform.SetParent (PathContainerGO.transform);

        PathListGO.transform.position = Vector3.one;
        PathGroupListGO.transform.position = Vector3.one;
		BossPathGos.transform.position = Vector3.one;




        PathSubobjects.Clear();
		SinglePathUsedPathID.Clear ();
		BossPathUsedPathID.Clear ();
        //路径(组)配置列表begin
        if (null != FishData.m_PathInterpList)
        {
            PathNumber = 0;
            for (int i = 0; i < FishData.m_PathInterpList.Count; i++)
            {
				GameObject pathGO = GeneratePath(FishData.m_PathInterpList[i], PathListGO, "path" + FishData.m_PathInterpList[i].pathUDID.ToString());
                PathSubobjects.Add(pathGO);
                pathGO.transform.localPosition = Vector3.zero;
                pathGO.SetActive(false);
                PathNumber++;
				SinglePathUsedPathID.Add (FishData.m_PathInterpList[i].pathUDID);

				GameObject pathInvGO = GeneratePath(FishData.m_PathInterpListInv[i], pahtInvListgo, "path" + FishData.m_PathInterpListInv[i].pathUDID.ToString());
				pathInvGO.transform.localPosition = Vector3.zero;
				pathInvGO.SetActive(false);
            }
        }

		GameObject spathGO = GeneratePath(FishData.m_BoLang, PathContainerGO, "BoLang");
		spathGO.transform.localPosition = Vector3.zero;
		spathGO.SetActive(false);

		spathGO = GeneratePath(FishData.m_LongJuanFeng[0], PathContainerGO, "LongJuanFeng");
		spathGO.transform.localPosition = Vector3.zero;
		spathGO.SetActive(false);

		spathGO = GeneratePath(FishData.m_DouDongPath, PathContainerGO, "DouDong");
		spathGO.transform.localPosition = Vector3.zero;
		spathGO.SetActive(false);

		//Boss 路径配置列表begin
		if (null != BosspathMap)
		{
			PathNumber = 0;
			foreach (var pair in BosspathMap) 
			{
				BossPathLinearInterpolator[] paths = pair.Value;
				for (int i = 0; i < paths.Length; i++) {
					GameObject pathGO = GeneratePath (paths [i], BossPathGos, "path" + paths[i].mPath.pathUDID.ToString ());
					PathSubobjects.Add (pathGO);
					pathGO.transform.localPosition = Vector3.zero;
					pathGO.SetActive (false);
					PathNumber++;
					BossPathUsedPathID.Add (paths [i].mPath.pathUDID);
				}
			}
		}


        if (null != FishData.m_PathGroupList)
        {
            GroupNumber = 0;
            for (int i = 0; i < FishData.m_PathGroupList.Count; i++)
            {
                GameObject groupGO = GeneratePathGroup(FishData.m_PathGroupList[i], PathGroupListGO, "GPath" + i);
                PathSubobjects.Add(groupGO);
                GroupNumber++;
            }
        }
        //路径(组)配置列表end

        //鱼(群)路径配置列表begin
        GameObject pathGroupDataGO = new GameObject(FISH_GROUP_NAME);
        GameObject groupDataGO = new GameObject(FISH_GROUPS_NAME);
        pathGroupDataGO.transform.SetParent(PathContainerGO.transform);
        groupDataGO.transform.SetParent(PathContainerGO.transform);
        pathGroupDataGO.transform.position = Vector3.zero;
        groupDataGO.transform.position = Vector3.zero;

        fishDataConfigManager.PathGroupDataGO = pathGroupDataGO;
        fishDataConfigManager.GroupDataGO = groupDataGO;


        int pathGroupIndex = 0; //路径组下标
        int groupDataIndex = 0; //鱼群组下标

		for (int i = 0; i < FishData.m_PathParadeDataList.Count; i++) {
			FishPathGroupData pathgroup = FishData.m_PathParadeDataList [i];
			GameObject subPathGroupGO = new GameObject (PATH_GROUP_DATA_NAME + pathGroupIndex);
			subPathGroupGO.transform.SetParent (pathGroupDataGO.transform);
			subPathGroupGO.transform.position = Vector3.zero;
			CPathGroupDataEditor pathGroupDataEditor = subPathGroupGO.AddComponent<CPathGroupDataEditor> ();
			pathGroupDataEditor.ParadePathGroupData = pathgroup;
			pathGroupDataEditor.GeneratePath ();
			++pathGroupIndex;
		}

		for (int i = 0; i < FishData.m_FishParadeDataList.Count; i++) {
			GameObject subGroupDataGO = new GameObject(GROUP_DATA_NAME + FishData.m_FishParadeDataList[i].FishParadeId);
			subGroupDataGO.transform.SetParent(groupDataGO.transform);
			subGroupDataGO.transform.position = Vector3.zero;
			CGroupDataEditor groupDataEditor = subGroupDataGO.AddComponent<CGroupDataEditor>();
			groupDataEditor.m_FishParadeData = FishData.m_FishParadeDataList[i];
			groupDataEditor.IsEditorMode = true; //编辑器模式
			groupDataIndex++;
		}
        //鱼(群)路径配置列表end

		GameObject openingParadeGo = new GameObject(OPENING_FISH_PARADE_NAME);
		openingParadeGo.transform.SetParent(PathContainerGO.transform);
		COpeningParadeEditor openingEidtor = openingParadeGo.AddComponent<COpeningParadeEditor> ();
		openingEidtor.mData = OpeningDataList;
		openingEidtor.RefreshDatas ();
		#if UNITY_EDITOR
		if (Application.isEditor) {
			UnityEditor.Selection.activeGameObject = openingParadeGo;
		}
		#endif
		pahtInvListgo.transform.SetParent (PathContainerGO.transform);
		pahtInvListgo.transform.localPosition = Vector3.zero;
//		COpeningPatternRender
        IsRefreshEditorMode = false;
    }

    private void DrawCameraFrameGizmos() 
    {
        float angle = MainCamera.fieldOfView;
        float length = MainCamera.farClipPlane;
        float aspet = MainCamera.aspect;
        float wide = Mathf.Tan(Mathf.Deg2Rad * angle) * length;
        float height = 0f, width = 0f;
        //Debug.Log (wide);
        if (aspet > 1f)
        {
            height = wide;
            width = aspet * height;
        }
        else
        {
            width = wide;
            height = aspet * width;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3(0f, 0f, length - 5f), new Vector3(width, height, 10));
    }

    private GameObject GeneratePath(PathLinearInterpolator pathLineare, GameObject parentGO, string pathName = "line")
    {
        GameObject go = new GameObject(pathName);
        
        CPathLinearRender pathRender = go.AddComponent<CPathLinearRender>();
        pathRender.PathData = pathLineare;
        go.transform.SetParent(parentGO.transform);
        go.transform.localPosition = Vector3.zero;

        return go;
    }

	private GameObject GeneratePath(BossPathLinearInterpolator pathLineare, GameObject parentGO, string pathName = "line")
	{
		GameObject go = GeneratePath (pathLineare.mPath, parentGO, pathName);
		CPathLinearRender render = go.GetComponent<CPathLinearRender> ();
		render.BossPathData = pathLineare;

		return go;
	}

	private GameObject GeneratePathGroup(PathLinearInterpolator[] pathLinearGroupData, GameObject parentGO, string pathName = "GPath")
    {
        GameObject groupGO = new GameObject(pathName);
        groupGO.transform.SetParent(parentGO.transform);
        groupGO.transform.localPosition = Vector3.zero;

        CPathGroupEditor pathGroupEditor = groupGO.AddComponent<CPathGroupEditor>();
        pathGroupEditor.SetData(pathLinearGroupData);
        for (int i = 0; i < pathLinearGroupData.Length; i++)
        {
			GameObject pathGo = GeneratePath(pathLinearGroupData[i], groupGO, "path" + pathLinearGroupData[i].pathUDID);
            pathGo.transform.localPosition = Vector3.zero;
            pathGo.SetActive(false);
        }
        return groupGO;
    }

    //运行模式下的鱼(群)组配置信息列表
    private void InitFishPath() 
    {
        if (null == FishData) return;

        if (null != FishContainerGO) Destroy(FishContainerGO);
        FishContainerGO = new GameObject("FishPathContainer");
        FishContainerGO.transform.SetParent(transform);
        FishContainerGO.transform.position = Vector3.zero;
        FishContainerGO.transform.localScale = Vector3.one;
        
        int pathGroupIndex = 0; //路径组下标
        int groupDataIndex = 0; //鱼群组下标
		GameObject pathGO = new GameObject("路径"); //路径组物体
		pathGO.transform.SetParent(FishContainerGO.transform);
		for (int i = 0; i < FishData.m_PathInterpList.Count; i++) {
			GameObject pgo = new GameObject ("Path" + FishData.m_PathInterpList[i].pathUDID);
			pgo.transform.SetParent(pathGO.transform);
			pgo.SetActive (false);
			pgo.transform.localPosition = Vector3.zero;
			CPathLinearRender pr = pgo.AddComponent<CPathLinearRender> ();
			pr.PathData = FishData.m_PathInterpList [i];

		}

		pathGO = new GameObject("反向路径"); //路径组物体
		pathGO.transform.SetParent(FishContainerGO.transform);
		for (int i = 0; i < FishData.m_PathInterpList.Count; i++) {
			GameObject pgo = new GameObject ("Path" + FishData.m_PathInterpList[i].pathUDID);
			pgo.transform.SetParent(pathGO.transform);
			pgo.SetActive (false);
			pgo.transform.localPosition = Vector3.zero;
			CPathLinearRender pr = pgo.AddComponent<CPathLinearRender> ();
			pr.PathData = FishData.m_PathInterpListInv [i];

		}



		GameObject bossPathGO = new GameObject("Boss 路径"); //路径组物体
		bossPathGO.transform.SetParent(FishContainerGO.transform);
	
		foreach(var pair in BosspathMap)
		{
			for (int i = 0; i < pair.Value.Length; i++) {
				GameObject pgo = new GameObject ("Path" + pair.Value [i].mPath.pathUDID);
				pgo.transform.SetParent(bossPathGO.transform);
				pgo.SetActive (false);
				pgo.transform.localPosition = Vector3.zero;
				CPathLinearRender pr = pgo.AddComponent<CPathLinearRender> ();
				pr.PathData = pair.Value [i].mPath;
				pr.BossPathData = pair.Value [i];
			}			
		}

        GameObject pathGroupDataGO = new GameObject(FISH_GROUP_NAME); //路径组物体
        GameObject groupDataGO = new GameObject(FISH_GROUPS_NAME); //鱼群组物体
        pathGroupDataGO.transform.SetParent(FishContainerGO.transform);
        groupDataGO.transform.SetParent(FishContainerGO.transform);
        pathGroupDataGO.transform.position = Vector3.zero;
        groupDataGO.transform.position = Vector3.zero;
        PathSubobjects.Add(pathGroupDataGO);
        PathSubobjects.Add(groupDataGO);
		foreach (FishPathGroupData pathParade in FishData.m_PathParadeDataList) {
			FishPathGroupData pathgroup = pathParade;
			GameObject subPathGroupGO = new GameObject (PATH_GROUP_DATA_NAME + pathGroupIndex);
			subPathGroupGO.transform.SetParent (pathGroupDataGO.transform);
			subPathGroupGO.transform.position = Vector3.zero;
			CPathGroupDataEditor pathGroupDataEditor = subPathGroupGO.AddComponent<CPathGroupDataEditor> ();
			pathGroupDataEditor.ParadePathGroupData = pathParade;
			pathGroupDataEditor.GeneratePath (); //运行模式
			++pathGroupIndex;
		}
		foreach(FishParadeData paradeData in FishData.m_FishParadeDataList)
		{
			GameObject subGroupDataGO = new GameObject(GROUP_DATA_NAME + paradeData.FishParadeId);
            subGroupDataGO.transform.SetParent(groupDataGO.transform);
            subGroupDataGO.transform.position = Vector3.zero;
            CGroupDataEditor groupDataEditor = subGroupDataGO.AddComponent<CGroupDataEditor>();
			groupDataEditor.m_FishParadeData = paradeData;
            groupDataEditor.IsEditorMode = false; //运行模式
            groupDataIndex++;
        }

		GameObject openingParadeGo = new GameObject(OPENING_FISH_PARADE_NAME);
		openingParadeGo.transform.SetParent(FishContainerGO.transform);
		COpeningParadeEditor openingEidtor = openingParadeGo.AddComponent<COpeningParadeEditor> ();
		openingEidtor.mData = OpeningDataList;
		openingEidtor.RefreshDatas ();
    }

	GameObject currentInvPathGo;
	public void ShowPathInv(int pathID)
	{
		if (currentInvPathGo != null)
			currentInvPathGo.SetActive (false);
		
		if (pahtInvListgo != null) {
			Transform child = pahtInvListgo.transform.Find ("path" + pathID);
			if (child != null) {
				child.gameObject.SetActive (true);
				currentInvPathGo = child.gameObject;
				currentInvPathGo.GetComponent<CPathLinearRender> ().RefreshData ();
			}
			
		}
	}

    public CPathLinearRender CreateNewPathForGroup(Transform parentTransform, int pathIndex)
    {
        if (FishData == null) return null;

        GameObject pathGO = new GameObject();
        pathGO.transform.SetParent(parentTransform);
		pathGO.transform.localPosition = Vector3.zero;
        
        PathLinearInterpolator piData = new PathLinearInterpolator();
        piData.pathUDID = CFishPathEditor.only.GetNextPathUID(PathType.SinglePath);
        piData.mEventList.Clear();
        piData.m_SplineDataList = new SplineSampleData[0];
		piData.keySamplePoints = new Vector3[]{ Vector3.zero};
		piData.keyPointsAngles = new float[0];
        CPathLinearRender pathLinearRender = pathGO.AddComponent<CPathLinearRender>();
		pathGO.transform.name = "path" + piData.pathUDID;
        pathLinearRender.PathData = piData;
        pathLinearRender.IsDrawPath = true;
        piData.m_WorldMatrix = pathGO.transform.localToWorldMatrix;
		piData.m_WorldRotation = pathGO.transform.rotation;
		#if UNITY_EDITOR
		UnityEditor.Selection.activeGameObject = pathGO;
		#endif

		FishData.m_PathInterpList.Add (piData);
        return pathLinearRender;
    }

	public uint GetNextPathUID(PathType pathtype)
	{
		HashSet<uint> setss = SinglePathUsedPathID;
		if (pathtype == PathType.SinglePath)
			setss = SinglePathUsedPathID;
		else if (pathtype == PathType.BossPath)
			setss = BossPathUsedPathID;
		HashSet<uint>.Enumerator emt = setss.GetEnumerator ();
		uint maxUint = 0;
		while(emt.MoveNext())
			maxUint = System.Math.Max(emt.Current, maxUint); 
		return maxUint + 1;
	}

	public bool CheckPathIDAvaible(uint pathID, PathType patht, uint oldPathID)
	{
		HashSet<uint> setss = SinglePathUsedPathID;
		if (patht == PathType.SinglePath)
			setss = SinglePathUsedPathID;
		else if (patht == PathType.BossPath)
			setss = BossPathUsedPathID;
		if (setss.Contains (pathID))
			return false;
		if (oldPathID != uint.MaxValue)
			setss.Remove (oldPathID);	
		setss.Add (pathID);
		return true;
	}


	HashSet<uint> SinglePathUsedPathID = new HashSet<uint> ();
	HashSet<uint> BossPathUsedPathID = new HashSet<uint> ();
	public void UpdateFishPathReferences(uint oldPathID, uint newPathID)
	{
		for (int i = 0; i < FishData.m_FishParadeDataList.Count; i++) {
			for (int j = 0; j < FishData.m_FishParadeDataList [i].PathList.Length; j++) {
				if (FishData.m_FishParadeDataList [i].PathList [j] == oldPathID)
					FishData.m_FishParadeDataList [i].PathList [j] = newPathID;
			}
		}

		for (var i = 0; i < OpeningDataList.Count; i++) {
			for (var j = 0; j < OpeningDataList [i].Length; j++) {
				for (var k = 0; k < OpeningDataList [i] [j].mFishParade.PathList.Length; k++) {
					if (OpeningDataList [i] [j].mFishParade.PathList [k] == oldPathID)
						OpeningDataList [i] [j].mFishParade.PathList [k] = newPathID;
				}
			}
		}

	}


	public FishVo GetFishVo(uint fishCfgID)
	{
		if (FishConfig.Instance.FishConf == null || FishConfig.Instance.FishConf.Count == 0)
		{
			byte[] bytes = System.IO.File.ReadAllBytes (Application.dataPath + CFishPathEditor.only.ConfigPath + "Fish.byte");
			FishConfig.Instance.FishConf = (Dictionary<uint, FishVo>)ConfigTables.Instance.Setobject<uint, FishVo>(bytes);
		}
		if (!FishConfig.Instance.FishConf.ContainsKey(fishCfgID)) {
			return null;
		}
		FishVo fishvo = FishConfig.Instance.FishConf.TryGet (fishCfgID);
		return fishvo;
	}

	public BossPathEventVo[] GetBossEventVo(uint Animatid)
	{
		if (FishConfig.Instance.mBossPathEventConf== null || FishConfig.Instance.mBossPathEventConf.Count == 0)
		{
			byte[] bytes = System.IO.File.ReadAllBytes (Application.dataPath + CFishPathEditor.only.ConfigPath + "BossEvent.byte");
			FishConfig.Instance.mBossPathEventConf = ConfigTables.Instance.ParseBossPathEvent (bytes);
		}
		if (!FishConfig.Instance.mBossPathEventConf.ContainsKey(Animatid)) {
			return null;
		}
		BossPathEventVo[] vos = FishConfig.Instance.mBossPathEventConf.TryGet (Animatid);
		return vos;
	}

	public FishAnimtorStatusVo GetFishAnimtorVo(uint sourceID)
	{
		if (FishConfig.Instance.fishAnimatorConf == null || FishConfig.Instance.fishAnimatorConf.Count == 0)
		{
			byte[] bytes = System.IO.File.ReadAllBytes (Application.dataPath + CFishPathEditor.only.ConfigPath + "FishAnimtor.byte");
			FishConfig.Instance.fishAnimatorConf = ConfigTables.Instance.ParseFishAnimatorStats(bytes);
		}

		FishAnimtorStatusVo fishStavo = FishConfig.Instance.fishAnimatorConf.TryGet (sourceID);
		return fishStavo;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawFrustum (Camera.main.transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane, Camera.main.farClipPlane, Camera.main.aspect);
	}

	public static void SettGroupPathOther()
	{
		if (Application.isPlaying)
			return;
		Transform groupTrans = GameObject.Find ("Main/配置信息/路径群数据").transform;
		for (int i = 0; i < groupTrans.childCount; i++) 
		{
			Transform ts = groupTrans.GetChild (i);
			if(!ts.GetChild(0).GetComponent<CPathLinearRender>().IsAutoVisOnSelect)
			{
				for (int j = 0; j < ts.childCount; j++) 
				{
					Transform t = ts.GetChild (j);
					CPathLinearRender pR = t.GetComponent<CPathLinearRender>();
					pR.IsAutoVisOnSelect = true;
					pR.gameObject.SetActive (false);
				}
			}
		}
	}

	public static void SettSinglePathOther()
	{
		Transform singleTrans = GameObject.Find ("Main/配置信息/路径数据").transform;
		Transform singleBossTrans = GameObject.Find ("Main/配置信息/BOSS路径数据").transform;
		Transform reverseSingleTrans = GameObject.Find ("Main/配置信息/反向路径").transform;

		for (int i = 0; i < singleTrans.childCount; i++) 
		{
			if (singleTrans.GetChild(i).gameObject.activeSelf) 
			{
				singleTrans.GetChild (i).gameObject.SetActive (false);
			}
		}

		for (int i = 0; i < singleBossTrans.childCount; i++) 
		{
			if (singleBossTrans.GetChild(i).gameObject.activeSelf) 
			{
				singleBossTrans.GetChild (i).gameObject.SetActive (false);
			}
		}

		for (int i = 0; i < reverseSingleTrans.childCount; i++) 
		{
			if (reverseSingleTrans.GetChild(i).gameObject.activeSelf) 
			{
				reverseSingleTrans.GetChild (i).gameObject.SetActive (false);
			}
		}
	}

	[ContextMenu("ExportPathID References.")]
	void ExportPathUIRef(){

		string outstr = "";
		foreach(var parade in FishData.m_FishParadeDataList)
		{
			outstr += string.Format ("\n渔阵ID:{0}\t 路径ID:[{1}]", parade.FishParadeId, joinIntarray(parade.PathList));
		}

		outstr += "\n\n 鱼群组：\n";
		for (int i = 0; i < FishData.m_PathParadeDataList.Count;i++) {
			var pgroup = FishData.m_PathParadeDataList[i];
			outstr += string.Format("\n{0}, PathGroupIdx:{1}", i, pgroup.PathGroupIndex);
		}
		File.WriteAllText (Application.dataPath + "/PathID.txt",outstr);
		Debug.Log (outstr);
		UnityEditor.EditorUtility.DisplayDialog ("", "导出成功", "OK");
	}

	string joinIntarray(uint[] ary){
		string str = "";
		for (int i = 0; i < ary.Length; i++) {
			str += ary[i] + ",";
		}
		str = str.Remove (str.Length - 1);
		return str;
	}

}

#endif