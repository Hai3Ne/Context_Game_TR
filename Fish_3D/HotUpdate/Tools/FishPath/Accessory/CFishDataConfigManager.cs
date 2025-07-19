using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
/*
 配置管理编辑器
 */
public class CFishDataConfigManager : MonoBehaviour 
{
    public GameObject                                       PathGroupDataGO;
    public GameObject                                       GroupDataGO;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string selObjName;
    public void SaveData() 
    {
		selObjName = Utility.GetObjName (UnityEditor.Selection.activeTransform);
        FishPathConfData newFishData = new FishPathConfData();
		Dictionary<uint, List<BossPathLinearInterpolator>> BosspathMap = new Dictionary<uint, List<BossPathLinearInterpolator>>();
		List<OpeningParadeData[]> openingparade = new List<OpeningParadeData[]> ();

        CFishPathEditor mainEditor = GetComponentInParent<CFishPathEditor>();
		bool isValid = true;
        if (mainEditor)
        {
            if (mainEditor.FishData == null)
            {
                mainEditor.ReloadData();
            }
			isValid = isValid && refreshTrans (newFishData, mainEditor);
			isValid = isValid && refreshPathGroupTrans (newFishData, mainEditor);            
			isValid = isValid && refreshBossPathList (BosspathMap, mainEditor);            
			isValid = isValid && RefreshOpeningParade (openingparade, mainEditor);

            newFishData.m_DouDongPath = mainEditor.FishData.m_DouDongPath;
            newFishData.m_LongJuanFeng = mainEditor.FishData.m_LongJuanFeng;
            newFishData.m_BoLang = mainEditor.FishData.m_BoLang;
        }
		if (!isValid)
			return;

        //鱼群(组)配置数据 m_FishGroupList
        if (null != GroupDataGO)
        {
            CGroupDataEditor[] groupDataEditorArray = GroupDataGO.GetComponentsInChildren<CGroupDataEditor>();
			LogMgr.Log("groupdata num:" + groupDataEditorArray.Length);
            for (int i = 0; i < groupDataEditorArray.Length; ++i)
            {
				if (groupDataEditorArray [i].mFishGraphic != null)
					groupDataEditorArray [i].m_FishParadeData.FishGraphicData = groupDataEditorArray [i].mFishGraphic.GraphicData;
				newFishData.m_FishParadeDataList.Add(groupDataEditorArray[i].m_FishParadeData);
            }
        }

        if (null != PathGroupDataGO)
        {
            CPathGroupDataEditor[] pathGroupDataEditorArray = PathGroupDataGO.GetComponentsInChildren<CPathGroupDataEditor>();
			LogMgr.Log("pathgroupdata num:" + pathGroupDataEditorArray.Length);
            for (int i = 0; i < pathGroupDataEditorArray.Length; ++i)
            {
				newFishData.m_PathParadeDataList.Add(pathGroupDataEditorArray[i].ParadePathGroupData);
            }
        }

        byte[] newbytes = FishPathConfParser.Serialize(newFishData);
        File.WriteAllBytes(CFishPathEditor.FishParadeDataFile, newbytes);

		byte[] bossPathBytes = FishPathConfParser.Serialize_BossPath (BosspathMap);
		File.WriteAllBytes(Application.dataPath + mainEditor.ConfigPath+mainEditor.BossPathsFile, bossPathBytes);

		byte[] openingbytes = FishPathConfParser.Serialize_OpeningParades (openingparade);
		File.WriteAllBytes (Application.dataPath + CFishPathEditor.only.ConfigPath + CFishPathEditor.only.OpeningParadeFile, openingbytes);

		#if UNITY_EDITOR
		UnityEditor.EditorUtility.DisplayDialog ("提示", "恭喜你，保存成功了~", "ok");
		#endif
		while(CFishPathEditor.only.transform.childCount > 0)
			GameObject.DestroyImmediate (CFishPathEditor.only.transform.GetChild(0).gameObject);
		CFishPathEditor.only.IsRefreshEditorMode = true;
		CFishPathEditor.only.EditorPathMode();
		CFishPathEditor.only.IsRefreshEditorMode = false;

		GameObject selGo = GameObject.Find (selObjName);
		if (selGo != null)
			UnityEditor.Selection.activeTransform = selGo.transform;

        RePackFishPath();
    }
    public static void RePackFishPath() {
		string srvconfPath = Application.dataPath + "\\..\\..\\..\\..\\code\\pc_code\\运行\\debug\\unicode\\NewFishCfg\\";

        List<ushort> fgroupIDList = new List<ushort>();//鱼群ID
        List<uint> pathIDList = new List<uint>();//路径ID
        List<ushort> pathGroupIDs = new List<ushort>();//路径组ID

        byte[] flowBytes = File.ReadAllBytes(srvconfPath + "FishFlowData.byte");
        using (MemoryStream ms = new MemoryStream(flowBytes)) {
            BinaryReader br = new BinaryReader(ms);
            br.ReadByte();
            int len = (int)br.ReadUInt32();
            for (int i = 0; i < len; i++) {
                br.ReadUInt32();
                br.ReadSingle();
                ushort aryCnt = br.ReadUInt16();
                for (ushort j = 0; j < aryCnt; j++) {
                    ushort groupid = br.ReadUInt16();
                    if (!fgroupIDList.Contains(groupid))
                        fgroupIDList.Add(groupid);
                }
                aryCnt = br.ReadUInt16();
                for (ushort j = 0; j < aryCnt; j++) {
                    br.ReadUInt16();
                }
                br.ReadUInt32();
                br.ReadUInt32();
            }
        }

        byte[] lcrBytes = File.ReadAllBytes(srvconfPath + "FishLauncher.byte");
        using (MemoryStream ms = new MemoryStream(lcrBytes)) {
            BinaryReader br = new BinaryReader(ms);
            br.ReadByte();
            int len = (int)br.ReadUInt32();
            for (int i = 0; i < len; i++) {
                br.ReadUInt32();
                br.ReadSingle();
                br.ReadByte();
                br.ReadByte();
                ushort aryCnt = br.ReadUInt16();
                for (ushort j = 0; j < aryCnt; j++) {
                    uint pid = br.ReadUInt32();
                    if (!pathIDList.Contains(pid))
                        pathIDList.Add(pid);
                }

                aryCnt = br.ReadUInt16();
                for (ushort j = 0; j < aryCnt; j++) {
                    ushort pgid = br.ReadUInt16();
                    if (!pathGroupIDs.Contains(pgid))
                        pathGroupIDs.Add(pgid);
                }
            }
        }

        //开场鱼阵
        byte[] openingparadebytes = File.ReadAllBytes("Assets/" + ResPath.ConfigDataPath + "openingParade.byte");
        List<OpeningParadeData[]> openingParadeList = FishPathConfParser.UnSerialize_OpeningParades(openingparadebytes);

        byte[] datas = File.ReadAllBytes(CFishPathEditor.FishParadeDataFile);
        FishPathConfData fishPathConf = FishPathConfParser.ParseData(datas);//,fgroupIDList,pathIDList,pathGroupIDs);

        foreach (var item in fishPathConf.m_PathParadeDataList) {//鱼群 --> 路径组
            pathGroupIDs.Add(item.PathGroupIndex);
        }
        foreach (var item in fishPathConf.m_FishParadeDataList) {//鱼阵 --> 路径
            //if (item.PathList.Length > 0) {
            //    pathIDList.Add(item.PathList[0]);
            //}
            foreach (var path in item.PathList) {
                pathIDList.Add(path);
            }
        }
        foreach (var parades in openingParadeList) {//开场鱼阵 --> 路径
            foreach (var item in parades) {
                foreach (var path in item.mFishParade.PathList) {
                    pathIDList.Add(path);
                }
            }
        }
        //foreach (var groupid in fgroupIDList) {
        //    if (groupid < 1000) {//鱼阵ID < 1000
        //        //fishPathConf.m_PathParadeDataList;
        //    } else {//鱼群ID >　1000

        //    }
        //}
        for (int i = fishPathConf.m_PathInterpList.Count - 1; i >= 0; i--) {
            if (pathIDList.Contains(fishPathConf.m_PathInterpList[i].pathUDID) == false) {
                Debug.Log("remove path : " + fishPathConf.m_PathInterpList[i].pathUDID);
                fishPathConf.m_PathInterpList.RemoveAt(i);
            }
        }
        for (int i = fishPathConf.m_PathInterpListInv.Count - 1; i >= 0; i--) {
            if (pathIDList.Contains(fishPathConf.m_PathInterpListInv[i].pathUDID) == false) {
                Debug.Log("remove path : " + fishPathConf.m_PathInterpListInv[i].pathUDID);
                fishPathConf.m_PathInterpListInv.RemoveAt(i);
            }
        }

		string savePathStr = Application.dataPath + "/Arts/GameRes/Config/Bytes/FishPathSimple{0}.byte";
        byte[] bytes = FishPathConfParser.SerializeSimple(fishPathConf);
		List<byte[]> partList = FishPathConfParser.SplitPath2Parts (bytes);
		for (int i = 0; i < partList.Count; i++) {
            string ppp = string.Format(savePathStr, i);
            //File.WriteAllBytes(ppp, partList[i]);//简化版  游戏运行使用
            ZipManager.ZipFile(ppp, partList[i]);//简化版  游戏运行使用
		}
        Debug.Log("RePack FishPath Success!   bytes len : " + bytes.Length);
    }

	bool refreshTrans(FishPathConfData newFishData, CFishPathEditor mainEditor)
	{
		List<PathLinearInterpolator> inters = new List<PathLinearInterpolator> ();
		List<PathLinearInterpolator> interInvs = new List<PathLinearInterpolator> ();
		Transform pathTrans = mainEditor.transform.GetChild (0).GetChild(0);
		for (int i = 0; i < pathTrans.childCount; i++) {
			Transform childT = pathTrans.GetChild (i);
			CPathLinearRender render = childT.GetComponent<CPathLinearRender> ();
			render.UpdatePosAndRot ();
			inters.Add(render.PathData);
			interInvs.Add (CalInverPaths (render.PathData));
		}
		newFishData.m_PathInterpList = inters;
		newFishData.m_PathInterpListInv = interInvs;
		return true;
	}

	bool refreshPathGroupTrans(FishPathConfData newFishData, CFishPathEditor mainEditor)
	{
		List<PathLinearInterpolator[]> pathGroupList = new List<PathLinearInterpolator[]> ();
		List<PathLinearInterpolator[]> pathInvGroupList = new List<PathLinearInterpolator[]> ();
		Transform pathGTrans = mainEditor.transform.GetChild (0).GetChild(1);
		for (int i = 0; i < pathGTrans.childCount; i++) 
		{
			Transform childT = pathGTrans.GetChild (i);
			List<PathLinearInterpolator> inters = new List<PathLinearInterpolator> ();
			List<PathLinearInterpolator> interInvs = new List<PathLinearInterpolator> ();
			for (int j = 0; j < childT.childCount; j++) {
				CPathLinearRender render = childT.GetChild(j).GetComponent<CPathLinearRender> ();
				render.UpdatePosAndRot ();
				inters.Add (render.PathData);
				interInvs.Add (CalInverPaths(render.PathData));
			}
			pathGroupList.Add (inters.ToArray());
			pathInvGroupList.Add (interInvs.ToArray());
		}

		newFishData.m_PathGroupList = pathGroupList;
		newFishData.m_PathGroupListInv = pathInvGroupList;
		return true;
	}

	bool refreshBossPathList(Dictionary<uint, List<BossPathLinearInterpolator>> bossMap, CFishPathEditor mainEditor)
	{
		Transform pathTrans = mainEditor.transform.GetChild (0).GetChild(2);
		for (int i = 0; i < pathTrans.childCount; i++) 
		{
			Transform childT = pathTrans.GetChild (i);
			CPathLinearRender render = childT.GetComponent<CPathLinearRender> ();
			if (render.BossPathData.bossCfgID <= 0)
			{
				#if UNITY_EDITOR
				UnityEditor.EditorUtility.DisplayDialog ("错误", "boss ConfigID 无效", "ok");
				#endif
				return false;
			}
			render.UpdatePosAndRot ();
			uint bossCfgID = render.BossPathData.bossCfgID;
			if (!bossMap.ContainsKey(bossCfgID))
				bossMap.Add(bossCfgID, new List<BossPathLinearInterpolator>());
			BossPathLinearInterpolator bossPath = new BossPathLinearInterpolator();
			bossPath.duration = render.BossPathData.duration;
			bossPath.delay = render.BossPathData.delay;
			bossPath.mPath = render.PathData;
			bossPath.mPathInv = CalInverPaths(render.PathData);
			bossPath.defaultSwinClip = render.BossPathData.defaultSwinClip;
			bossMap[bossCfgID].Add(bossPath);
		}
		return true;
	}

	public bool RefreshOpeningParade(List<OpeningParadeData[]> openingParade, CFishPathEditor mainEditor)
	{
		Transform openingTrans = mainEditor.transform.GetChild (0).GetChild(5);

		for (int i = 0; i < openingTrans.childCount; i++) {
			COpeningPatternRender patterR = openingTrans.GetChild (i).GetComponent<COpeningPatternRender>();

			List<OpeningParadeData> parades = new List<OpeningParadeData> ();
			for (int j = 0; j < patterR.transform.childCount; j++) {
				COpeningPatternElementRender elem = patterR.transform.GetChild (j).GetComponent<COpeningPatternElementRender> ();
				elem.GetPoslist (elem.m_OpeningParadeData.mFishParade);
				parades.Add (elem.m_OpeningParadeData);
			}
			if (parades.Count > 0)
				openingParade.Add (parades.ToArray());
		}
		return true;
	}

	PathLinearInterpolator CalInverPaths(PathLinearInterpolator path)
	{
		Matrix4x4 newMat = path.m_WorldMatrix;
		PathLinearInterpolator pathInv = new PathLinearInterpolator ();
		pathInv.pathUDID = path.pathUDID;
        pathInv.m_HasPathEvent = path.m_HasPathEvent;
        pathInv.mAnimCurve = path.mAnimCurve;
		pathInv.keyPointsAngles = path.keyPointsAngles;
        pathInv.keySamplePoints = path.keySamplePoints;
        pathInv.mEventList.AddRange(path.mEventList);
		pathInv.m_SampleMaxDistance = path.m_SampleMaxDistance;
		pathInv.m_WorldMatrix = newMat;
		pathInv.m_WorldRotation = path.m_WorldRotation;
		pathInv.m_SplineDataList = convertSampleDatas(path.m_SplineDataList,newMat);

		return pathInv;
	}


	SplineSampleData[] convertSampleDatas(SplineSampleData[] mSplineDataList, Matrix4x4 newMat)
	{
		
		SplineSampleData[] newSplineDataList = new SplineSampleData[mSplineDataList.Length];
		for (int i = 0; i < mSplineDataList.Length; i++) {
/*			Quaternion rot = Quaternion.identity;
			if (i < mSplineDataList.Length - 1) {
				Vector3 p0 = newMat.MultiplyPoint (mSplineDataList [i].pos);
				Vector3 p1 = newMat.MultiplyPoint (mSplineDataList [i + 1].pos);
				Vector3 ddir = (p1 - p0).normalized;
				Vector3 vv0 = new Vector3 (ddir.x, 0f, ddir.z).normalized;
				rot = Quaternion.FromToRotation (vv0, ddir) * Quaternion.FromToRotation (Vector3.right, vv0);
			}
//*/
			Vector3 pos = new Vector3(mSplineDataList [i].pos.x, mSplineDataList [i].pos.y, mSplineDataList [i].pos.z);
			pos = newMat.MultiplyPoint (pos);
			pos.y *= -1f;
			pos = newMat.inverse.MultiplyPoint (pos);
			newSplineDataList [i] = new SplineSampleData ();
            //newSplineDataList [i].nodeIdx = mSplineDataList [i].nodeIdx;
			newSplineDataList [i].pos = pos;
			newSplineDataList [i].rot = Quaternion.Euler (-mSplineDataList [i].rot.eulerAngles.x, mSplineDataList [i].rot.eulerAngles.y, -mSplineDataList [i].rot.eulerAngles.z);
            //newSplineDataList [i].timeScaling = mSplineDataList [i].timeScaling;
		}

		return newSplineDataList;
	}
}

#endif