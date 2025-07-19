using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

public class CustomMenuPath
{



	[MenuItem("路径工具/添加单体路径")]
	static void AddSinglePath()
	{
		Transform singleTrans = CFishPathEditor.only.transform.GetChild (0).GetChild (0);
		int pathidx = singleTrans.childCount;
		CFishPathEditor.only.CreateNewPathForGroup (singleTrans, pathidx);
	}

	[MenuItem("路径工具/添加单体Boss路径")]
	static void AddSingleBossPath()
	{
		Transform parentTransform = CFishPathEditor.only.transform.GetChild (0).GetChild (2);
		GameObject pathGO = new GameObject();
		pathGO.transform.SetParent(parentTransform);
		pathGO.transform.localPosition = Vector3.zero;

		BossPathLinearInterpolator bossPathData = new BossPathLinearInterpolator ();
		bossPathData.bossCfgID = 10901;
		bossPathData.mPath = new PathLinearInterpolator ();
		bossPathData.mPath.pathUDID = CFishPathEditor.only.GetNextPathUID (PathType.BossPath);

        PathLinearInterpolator piData = bossPathData.mPath;
        piData.mEventList.Clear();
		piData.m_SplineDataList = new SplineSampleData[0];
		piData.keySamplePoints = new Vector3[]{ Vector3.zero};
        piData.keyPointsAngles = new float[0];
        piData.mAnimCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

		CPathLinearRender pathLinearRender = pathGO.AddComponent<CPathLinearRender>();
		pathGO.transform.name = "path" + bossPathData.mPath.pathUDID;
		pathLinearRender.PathData = bossPathData.mPath;
		pathLinearRender.BossPathData = bossPathData;
		pathLinearRender.IsDrawPath = true;
		piData.m_WorldMatrix = pathGO.transform.localToWorldMatrix;
		piData.m_WorldRotation = pathGO.transform.rotation;
		#if UNITY_EDITOR
		UnityEditor.Selection.activeGameObject = pathGO;
		#endif
	}

	[MenuItem("路径工具/保存路径数据")]
	static void SavePathsss()
	{
		CFishDataConfigManager mgr = GameObject.FindObjectOfType<CFishDataConfigManager> ();
		if (mgr!=null)
			mgr.SaveData ();
	}

	[MenuItem("路径工具/添加路径组")]
	static void Convertsssss()
	{
		Transform parentTransform = CFishPathEditor.only.transform.GetChild (0).GetChild (1);
		int pathIndex = parentTransform.childCount;
		GameObject GpathGO = new GameObject("GPath" + pathIndex);
		GpathGO.transform.SetParent(parentTransform);
		GpathGO.transform.localPosition = Vector3.zero;
		CPathGroupEditor editor = GpathGO.AddComponent<CPathGroupEditor> ();
		PathLinearInterpolator[] mdata = new PathLinearInterpolator[0];
		editor.SetData (mdata);

		#if UNITY_EDITOR
		UnityEditor.Selection.activeGameObject = GpathGO;
		#endif
	}

	private static string[] Default_Options = {
		"*.prefab",
		"*.png",
		"*.tga",
		"*.mp3",
		"*.txt",
		"*.ttf",
		"*.wav",
		"*.jpg",
		"*.byte",
		"*.xml",
		"*.anim",
		".mat",
		"*.controller",
		"*.unity"
	};

	[MenuItem("路径工具/FFEFE")]
	static void unpack()
	{
		string str = Application.dataPath + "/Arts";
		AnalyzeSingleFile (str);
	}

	static string splt_string = "Assets/";
	static void AnalyzeSingleFile(string targetPath)
	{
		if (File.Exists (targetPath)) 
		{
			targetPath = targetPath.Replace ("\\", "/");
			int Fullindex = Mathf.Max (0, targetPath.IndexOf (splt_string));
			string FullPath = targetPath.Substring (Fullindex);
			AssetImporter importer = AssetImporter.GetAtPath (FullPath);
			if (importer != null) {
				if (!string.IsNullOrEmpty (importer.assetBundleName))
					LogMgr.Log (importer.assetBundleName);
				importer.assetBundleName = null;
			}
		}
		else
		{
			DirectoryInfo dirinfo = new DirectoryInfo (targetPath);
			for (int j = 0; j < Default_Options.Length; j++) {
				string subopt = Default_Options [j];
				if (!string.IsNullOrEmpty (subopt)) {
					FileInfo[] files = dirinfo.GetFiles (subopt, SearchOption.AllDirectories);
					foreach (var f in files) {
						AnalyzeSingleFile (f.FullName);
					}
				}
			}
		}
	}
}


