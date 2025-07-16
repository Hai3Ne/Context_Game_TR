using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;

public class CustomMenus : MonoBehaviour {
	[MenuItem("Tools/ClearData")]
	static void cleardata(){
		PlayerPrefs.DeleteAll ();
	}
    [MenuItem("Tools/Android/基础资源打包(Build)")]
    static void BuildAssetBundleAndoird() {
#if !UNITY_ANDROID
        Debug.LogError("请先切换平台");
        return;
#endif
        string path = "Assets/Arts/GameRes/";

        AssetDatabase.DeleteAsset("Assets/Settings/Cache.txt");
        AssetDatabase.DeleteAsset("Assets/Settings/Version.json");
        FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/");
        AssetDatabase.CreateFolder("Assets", "StreamingAssets");

        AssetDatabase.Refresh();

        Debug.Log("开始打包assetbundle");
        AssetBundlePacker.PrepareBuild(true,BuildTargetGroup.Android, BuildTarget.Android, path);
    }

    [MenuItem("Tools/Android/仅打包(Normal_build)")]
    static void BuildAndroid_0() {//非渠道包
        BuildAndroid("UNUSE_ASSETBOUNDLE_INEDITOR;Android_WX","");
    }
    [MenuItem("Tools/Android/渠道打包/渠道201(淘新闻)")]
    static void BuildAndroid_201() {//201
        BuildAndroid("UNUSE_ASSETBOUNDLE_INEDITOR;Android_WX;CHANCEL_201","201");
    }
    [MenuItem("Tools/Android/渠道打包/渠道202")]
    static void BuildAndroid_202() {//202
        BuildAndroid("UNUSE_ASSETBOUNDLE_INEDITOR;Android_WX;CHANCEL_202","202");
    }
    [MenuItem("Tools/Android/渠道打包/渠道203")]
    static void BuildAndroid_203() {//203
        BuildAndroid("UNUSE_ASSETBOUNDLE_INEDITOR;Android_WX;CHANCEL_203","203");
    }
    static void BuildAndroid(string defines,string chancel) {
#if !UNITY_ANDROID
        Debug.LogError("请先切换平台");
        return;
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);//"Self;Android360");
        //touchmind147258
        string[] levels = {
                              "Assets/Scene/Main.unity",
                          };
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.touchmind.fishing3D");
        PlayerSettings.bundleVersion = GameConfig.ClientVersionStr;
        PlayerSettings.Android.bundleVersionCode = (int)GameConfig.ClientVersionCode;
        // PlayerSettings.Android.keystoreName = "fishing.keystore";
        // PlayerSettings.Android.keystorePass = "touchmind147258";
        // PlayerSettings.Android.keyaliasName = "fishing3d";
        // PlayerSettings.Android.keyaliasPass = "touchmind147258";

        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "UNUSE_ASSETBOUNDLE_INEDITOR;Android_WX");//"Self;Android360");
        string path;
        if (string.IsNullOrEmpty(chancel)) {
            path = "D:/build_source/捕鱼_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".apk";
        } else {
            path = "D:/build_source/捕鱼_" + DateTime.Now.ToString("yyyyMMddHHmm") + "_" + chancel + ".apk";
        }
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.Android, BuildOptions.None);
    }

    [MenuItem("Tools/IOS/基础资源打包")]
    static void BuildAssetBundleIOS() {
#if !UNITY_IOS
        Debug.LogError("请先切换平台");
        return;
#endif
        string path = "Assets/Arts/GameRes/";

        AssetDatabase.DeleteAsset("Assets/Settings/Cache.txt");
        AssetDatabase.DeleteAsset("Assets/Settings/Version.json");
        FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/");
        AssetDatabase.CreateFolder("Assets", "StreamingAssets");

        AssetDatabase.Refresh();

        Debug.Log("开始打包assetbundle");
        AssetBundlePacker.PrepareBuild(true,BuildTargetGroup.iOS, BuildTarget.iOS, path);
    }

    [MenuItem("Tools/IOS/签名包打包")]
    static void BuildIOS_0() {//非渠道包
        BuildSignIOS("UNUSE_ASSETBOUNDLE_INEDITOR;IOS_WX");
    }
    [MenuItem("Tools/IOS/渠道打包/渠道201")]
    static void BuildIOS_201() {//201
        BuildSignIOS("UNUSE_ASSETBOUNDLE_INEDITOR;IOS_WX;CHANCEL_201");
    }
    [MenuItem("Tools/IOS/渠道打包/渠道202")]
    static void BuildIOS_202() {//202
        BuildSignIOS("UNUSE_ASSETBOUNDLE_INEDITOR;IOS_WX;CHANCEL_202");
    }
    [MenuItem("Tools/IOS/渠道打包/渠道203")]
    static void BuildIOS_203() {//203
        BuildSignIOS("UNUSE_ASSETBOUNDLE_INEDITOR;IOS_WX;CHANCEL_203");
    }

    static void BuildSignIOS(string defines) {
#if !UNITY_IOS
        Debug.LogError("请先切换平台");
        return;
#endif
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines);//"Self;Android360");
        //CustomMenus.CopyPath("_IOS/2.IOSWX/", IOSPath);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.touchmind.fish3d");
        string[] levels = { "Assets/Scene/Main.unity" };
        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", GameConfig.clientVersions[0], GameConfig.clientVersions[1] * 100 + GameConfig.clientVersions[2], GameConfig.clientVersions[3]);
        PlayerSettings.iOS.buildNumber = DateTime.Now.ToString("yyMMdd");
        string path = @"C:\Users\Administrator\Desktop\ios_pro\";
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.iOS, BuildOptions.None);
    }
    [MenuItem("Tools/IOS/渠道打包/AppStore")]
    static void BuildAppStoreIOS() {
#if !UNITY_IOS
        Debug.LogError("请先切换平台");
        return;
#endif
        //CustomMenus.CopyPath("_IOS/1.IOSIAP/", IOSPath);
        PlayerSettings.applicationIdentifier = "com.touchmind.buyu";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "UNUSE_ASSETBOUNDLE_INEDITOR;IOS_IAP");
        string[] levels = { "Assets/Scene/Main.unity" };
        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2}", GameConfig.clientVersions[0]+1, GameConfig.clientVersions[1] * 100 + GameConfig.clientVersions[2], GameConfig.clientVersions[3]);
        PlayerSettings.iOS.buildNumber = DateTime.Now.ToString("yyMMdd");
        string path = @"C:\Users\Administrator\Desktop\ios_pro\";
        BuildPipeline.BuildPlayer(levels, path, BuildTarget.iOS, BuildOptions.None);
    }

    private const string IOSPath = "Assets/Plugins/Config/";
    private static void CopyPath(string source_path, string target_path) {
        DirectoryInfo soucre = new DirectoryInfo(source_path);
        DirectoryInfo target = new DirectoryInfo(target_path);
        if (soucre.Exists == false) {
            Debug.LogError("不存在的文件夹：" + source_path);
            return;
        }
        if (target.Exists == false) {
            target.Create();
        }
        //删除文件保留mate
        foreach (var item in soucre.GetFiles("*.*", SearchOption.AllDirectories)) {
            if (item.Extension != ".meta") {
                item.Delete();
            }
        }
        //文件夹复制
        foreach (var item in soucre.GetDirectories()) {
            CopyPath(item.FullName, Path.Combine(target_path, item.Name));
        }
        //文件复制
        foreach (var item in soucre.GetFiles()) {
            File.Copy(item.FullName, Path.Combine(target_path, item.Name));
        }
    }

	static GuideUIRef currUIref;
	[MenuItem("Tools/GuideEditor/Open")]
	public static void OpenGuideEditor() {
		if (Application.isPlaying) {
			currUIref = FindObjectOfType<GuideUIRef> ();
			if (currUIref != null) {
				currUIref.gameObject.SetActive (false);
				UICamera.ChkGameObjCanClick = null;
			}
            //Kubility.KAssetBundleManger.Instance.LoadGameObject (ResPath.UIPath + "UI_Guide", delegate(SmallAbStruct obj) 
            //         {
            //	GameObject guideObj = GameUtils.CreateGo (obj.MainObject, SceneObjMgr.Instance.UIPanelTransform);
            //	guideObj.name = "TestGuideEditor";
            //	var ui = guideObj.GetComponent<GuideUIRef>();
            //	ui.bgMask.GetComponent<BoxCollider>().enabled = false;
            //	GameObject editorGo = new GameObject ("GuideEditor");
            //	editorGo.AddComponent<GuideStepEditor> ();
            //	Selection.activeGameObject = editorGo;
            //});

            GameObject go = ResManager.LoadAsset<GameObject>(GameEnum.Fish_3D, ResPath.NewUIPath + "UI_Guide");
            GameObject guideObj = GameUtils.CreateGo(go, SceneObjMgr.Instance.UIPanelTransform);
            guideObj.name = "TestGuideEditor";
            var ui = guideObj.GetComponent<GuideUIRef>();
            ui.bgMask.GetComponent<BoxCollider>().enabled = false;
            GameObject editorGo = new GameObject("GuideEditor");
            editorGo.AddComponent<GuideStepEditor>();
            Selection.activeGameObject = editorGo;
        }
    }

	[MenuItem("Tools/GuideEditor/Close")]
	public static void CloseGuideEditor(){
		if (!Application.isPlaying)
			return;
		var ed = FindObjectOfType<GuideStepEditor> ();
		GameObject.Destroy (ed.gameObject);

		var reff = FindObjectOfType<GuideUIRef> ();
		GameObject.Destroy(reff.gameObject);
		if (currUIref != null)
			currUIref.gameObject.SetActive (true);
		GuideMgr.Instance.ResetClickEvent();		
	}

	[MenuItem("Tools/GuideEditor/Test")]
	public static void TestGuide(){
        GuideMgr.Instance.Reset();
	}

	[MenuItem("Tools/Included shader", false, 11)]
	 public static void TestIncludedShader()
	{
		string[] myShaders = new string[] {
			"FishShader/Background",
			"FishShader/AddTex",
			"FishShader/AddTexAlpha",
			"FishShader/BlendColor",
			"FishShader/BlendColorAlpha",
			"FishShader/Simple",
			"FishShader/SimpleAlpha",
			"HeroShader/HeroSimple",
			"HeroShader/HeroSimpleBlendColor",
			"Mobile/Particles/Additive",
			"Mobile/Particles/Alpha Blended",
			"Mobile/Unlit (Supports Lightmap)"
		};
	 	
		string[] incudeShaders = File.ReadAllLines (Application.dataPath + "/includeShaders.txt");
		List<string> tmp = new List<string>();

		foreach(var s in myShaders)
			if (!tmp.Contains(s))
				tmp.Add(s);
				
		foreach(var s in incudeShaders)
			if (!tmp.Contains(s))
				tmp.Add(s);
		myShaders = tmp.ToArray();
		SerializedObject graphicsSettings = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/GraphicsSettings.asset") [0]);
		SerializedProperty it = graphicsSettings.GetIterator ();
		SerializedProperty dataPoint;
		while (it.NextVisible (true)) {
			if (it.name == "m_AlwaysIncludedShaders") {
				it.ClearArray ();

				for (int i = 0; i < myShaders.Length; i++) { 
					it.InsertArrayElementAtIndex (i);
					dataPoint = it.GetArrayElementAtIndex (i);
					dataPoint.objectReferenceValue = Shader.Find (myShaders [i]);
				}
				graphicsSettings.ApplyModifiedProperties ();
			}
		}
		EditorUtility.DisplayDialog ("msg", "打包shader 成功", "OK");
	}

	[MenuItem("Tools/Make Version")]
	static void ShowVersionMakeWindow()
	{
		FishVersionTools window = (FishVersionTools)EditorWindow.GetWindow<FishVersionTools> (false, "VersionTools", true);
		window.Show ();
	}

	[MenuItem("Tools/鱼动画时间导出")]
	public static void exportAnim()
	{
		FishAnimatorExport.exportAnim ();
		FishAnimatorExport.exportHeroAnim ();
	}

	[MenuItem("Tools/GenFishResID")]
	static void generatefishresID()
	{
		string SavePath = Application.dataPath + "/Arts/GameRes/Config/Bytes/Resmap.byte";
		GlobalLoading.Instance.LoadConfigOnly (delegate {

			Dictionary<byte, uint[]> idmaps = new Dictionary<byte, uint[]> ();


			idmaps [(byte)FishResType.LauncherObject] = new uint[]{ 0, 1, 2, 3, 4 };
			idmaps [(byte)FishResType.GunBarrelObjList] = FishConfig.GetLauncherSourceIDList ().ToArray ();
			idmaps [(byte)FishResType.HeroObjMap] = FishConfig.GetHeroSourceIDs ().ToArray ();

			List<uint>[] bulletSourceList = FishConfig.GetBulletSourceList ();
			idmaps [(byte)FishResType.BulletObjMap] = bulletSourceList [0].ToArray ();
			idmaps [(byte)FishResType.LauncherFireEff] = bulletSourceList [1].ToArray ();
			idmaps [(byte)FishResType.LauncherIdleEff] = bulletSourceList [2].ToArray ();
			idmaps [(byte)FishResType.LauncherHitOnEff] = bulletSourceList [3].ToArray ();

			idmaps [(byte)FishResType.BufferHaloMap] = FishConfig.GetBufferHaloIDList ().ToArray ();
			idmaps [(byte)FishResType.FishPrefabMap] = FishConfig.GetFishSourceIDList ().ToArray ();
			idmaps [(byte)FishResType.SkillEffectMap] = FishConfig.GetSkillEffIDList ().ToArray ();
			idmaps [(byte)FishResType.ComboEffResMap] = FishConfig.GetCombEffSourceIDList ().ToArray ();
			idmaps [(byte)FishResType.BossEffMap] = FishConfig.GetBossEffSourceIDList ().ToArray ();

			idmaps [(byte)FishResType.AudioList] = GetAudioList ();

            idmaps[(byte)FishResType.HeroEnterScene] = FishConfig.GetHeroEnterSceneIDs();
            idmaps[(byte)FishResType.FishShape] = FishConfig.GetFishShapeIDs();

			using (MemoryStream ms = new MemoryStream ()) {
				BinaryWriter bw = new BinaryWriter (ms);
				foreach (var pari in idmaps) {
					bw.Write (pari.Key);
					bw.Write (pari.Value.Length);
					for (int i = 0; i < pari.Value.Length; i++)
						bw.Write (pari.Value [i]);				
				}
				byte[] bytes = ms.GetBuffer ();
				byte[] newBuff = new byte[ms.Length];
				Array.Copy (bytes, 0, newBuff, 0, newBuff.Length);
				File.WriteAllBytes (SavePath, newBuff);
			}
			EditorUtility.DisplayDialog ("保存成功", "保存成功", "OK");
		});

	}

    //[MenuItem("Tools/RePack FishPath")]
    //public static void RePackFishPath()
    //{
    //    CFishDataConfigManager.RePackFishPath();
    //}


	static void ShowProgressAndForeach<T> (T[] list, string title, string content,string append, Action<T> callback)
	{
		float v = 0f;
		for (int i = 0; i < list.Length; ++i) {
			T val = list [i];

			v = (float)i / list.Length;
			if (typeof(T) == typeof(string)) {
				EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + val + append, v);
			} else if (typeof(T) == typeof(FileInfo)) {
				EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + Path.GetFileName (val.GetType ().GetProperty ("FullName").GetValue (val, null).ToString ()), v);
			} else{
				EditorUtility.DisplayCancelableProgressBar (title, content + " ..." +append , v);
			}


			callback (val);
		}
	}

	static string splt_string = "Assets\\";
	[MenuItem("Tools/CheckDDSDepands")]
	public static void CheckDDSDepands(){
		dit.Clear ();
		string targetPath = Application.dataPath+"/Arts/GameRes";
		DirectoryInfo dirinfo = new DirectoryInfo (targetPath);
		EditorUtility.DisplayCancelableProgressBar ("分析 ", "开始分析 ....", 0f);
		string[] Curent_Options = AssetBundlerAnalyz.Default_Options;
		for (int j = 0; j < Curent_Options.Length; j++) {
			string subopt = Curent_Options [j];
			if (!string.IsNullOrEmpty (subopt)) {
				FileInfo[] files = dirinfo.GetFiles (subopt, SearchOption.AllDirectories);
				ShowProgressAndForeach<FileInfo> (files, "分析 ", "分析中", "源目录 >>" + targetPath, delegate (FileInfo file) {
					if (file != null) {						
						int Fullindex = Mathf.Max (0, file.FullName.IndexOf (splt_string));
						string assetFullPath = file.FullName.Substring (Fullindex);
						CollectDepends(assetFullPath);
					}
				});
			}
		}
		EditorUtility.ClearProgressBar ();
	}

	static List<string> dit = new List<string>();
	static void CollectDepends (string assetFullPath){
		string[] deps = AssetDatabase.GetDependencies (new string[] { assetFullPath });
		for (int i = 0; i < deps.Length; i++){
			if (deps[i] == assetFullPath)
				continue;
			
				
			if (deps[i].EndsWith(".dds") || deps[i].EndsWith(".DDS")){
				if (assetFullPath.EndsWith (".mat") && dit.Contains(assetFullPath) == false) {
					//dit.Add (assetFullPath);
					LogMgr.Log (deps [i] + " 依赖 " + assetFullPath);
				}
			}else{
				CollectDepends (deps[i]);
			}
		}
	}

	static uint[] GetAudioList()
	{
		List<uint> idList = new List<uint> ();
		uint idd = 0;
		System.Type t = typeof(OrdianryAudio);
		var fields = OrdianryAudio.CLSID.Split (',');
		foreach (var fd in fields) {
			if (fd.StartsWith ("GameBgm"))
				continue;
			string val = t.GetField (fd).GetValue (FishConfig.Instance.AudioConf).ToString();
			idd = uint.Parse (val);
			if (idd > 0 && !idList.Contains(idd))
				idList.Add (idd);
        }
        foreach (var pair in FishConfig.Instance.mBulletBuffConf) {
            idd = pair.Value.GetAudio;
            if (idd > 0 && !idList.Contains(idd))
                idList.Add(idd);
        }
		foreach (var pair in FishConfig.Instance.FishConf) {
			idd = pair.Value.DieAudio;
			if (idd > 0 && !idList.Contains (idd))
				idList.Add (idd);
		}

		foreach (var pair in FishConfig.Instance.LauncherConf) {
			idd = pair.Value.AudioID;
			if(idd > 0 && !idList.Contains(idd))
				idList.Add (idd);
		}

		foreach (var pair in FishConfig.Instance.SkillConf) {
			idd = pair.Value.AudioID;
			if (idd > 0 && !idList.Contains(idd))
                idList.Add(idd);
            idd = pair.Value.HitAudio;
            if (idd > 0 && !idList.Contains(idd))
                idList.Add(idd);
		}

		foreach (var pair in FishConfig.Instance.HeroActionConf) {
			idd = pair.Value.AudioID;
			if (idd > 0 && !idList.Contains(idd))
				idList.Add (idd);
        }
        foreach (var pair in FishConfig.Instance.mTotalResource) {
            idd = (uint)pair.Value.Audio;
            if (idd > 0 && !idList.Contains(idd))
                idList.Add(idd);
        }

		foreach (var pair in FishConfig.Instance.BossAudioConf) {
			TryAddUintArray (pair.Value.AppearAudio, idList);
			TryAddUintArray (pair.Value.EscapeAudio, idList);
			TryAddUintArray (pair.Value.HurtAudio, idList);
			TryAddUintArray (pair.Value.DizzyAudio, idList);
			TryAddUintArray (pair.Value.DieAudio, idList);
        }

        foreach (var pair in FishConfig.Instance.mBossPathEventConf) {
            foreach (var elem in pair.Value) {
                TryAddUintArray(elem.Audio, idList);
            }
        }

        foreach (var pair in FishConfig.Instance.FishBubbleConf) {
            TryAddUintArray(pair.Value.HitAudioLibs, idList);
            TryAddUintArray(pair.Value.RandAudioLibs, idList);
        }
		return idList.ToArray ();
	}

	static void TryAddUintArray(uint[] ary, List<uint> li)
	{
		foreach (var idd in ary) {
			if (idd > 0 && !li.Contains (idd))
				li.Add (idd);
		}
	}

	[MenuItem("Tools/UnbundleAll")]
	static void Unbundle()
	{
		EditorUtility.ClearProgressBar ();
		string[] respaths = AssetDatabase.GetAllAssetPaths ();
		float progress = 0f;
		for (int i = 0; i < respaths.Length; i++) {
			string[] deps = AssetDatabase.GetDependencies (new string[] { respaths[i] });
			for (int j = 0; j < deps.Length; j++) {
				AssetImporter importer = AssetImporter.GetAtPath (deps[j]);
				if (!String.IsNullOrEmpty (importer.assetBundleName)) {
					LogMgr.Log (importer.assetBundleName);
					importer.assetBundleName = "";
				}
			}
			EditorUtility.DisplayCancelableProgressBar ("UnBundle", "UnBundle All" + " ..." + i, progress);
			progress = i * 1.0f / respaths.Length;
		}

		EditorUtility.ClearProgressBar ();

	}

	struct abc{
		public int idx;
		public float val;
		public abc(int i, float j){this.idx=i;this.val=j;}
	}
	static string outlogs;
	[MenuItem("Tools/checkParde")]
	static void checkParde()
	{
		ushort ssid = 1;
		string p = Application.dataPath + "/" + ResPath.ConfigDataPath + "openingParade.byte";
		byte[] openingparadebytes = File.ReadAllBytes (p);
		FishPathSetting.openingParadeList = FishPathConfParser.UnSerialize_OpeningParades(openingparadebytes);
		OpeningParadeData[] ll = FishPathSetting.openingParadeList [3];
		for (int i = 0; i < ll.Length; i++) {
			ssid = LcrF_FishGroup (ll [i].mFishParade, 0, ssid, 0);
		}
		Debug.Log (outlogs);
	}


	static ushort LcrF_FishGroup(FishParadeData fParadeData, uint pathIndex, ushort startID, float elapsedTime)
	{
		if (fParadeData == null) {
			LogMgr.LogError ("LcrF_FishGroup FishParadeData is Null.");
			return startID;
		}
		GroupData[] GroupDataArray = fParadeData.GroupDataArray;
		for (int idx = 0; idx < GroupDataArray.Length; idx++)
		{
			GroupData gd = GroupDataArray [idx];
			if (gd == null)
				break;
			if(gd.FishNum > gd.PosList.Length)
			{
				LogMgr.Log("错误的鱼群路径点:" + gd.FishNum + ", posnum:" + gd.PosList.Length);
				return startID;
			}
			for (int i = 0; i < gd.FishNum; ++i)
			{
				outlogs += startID.ToString ()+","+gd.FishCfgID+"\n";
				++startID;
				if (startID == 0)
					startID = 1;
			}
		}
		return startID;
	}
    [MenuItem("Assets/查找引用")]
    private static void OnSearchForReferences() {
        if (Selection.activeObject == null) {
            return;
        }
        int count = 0;
        string select_path = AssetDatabase.GetAssetPath(Selection.activeObject);

        string[] respaths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in respaths) {
            if (path.EndsWith(".prefab")) {
                string[] deps;
                if (select_path.EndsWith(".prefab")) {
                    deps = AssetDatabase.GetDependencies(path, false);
                } else {
                    deps = AssetDatabase.GetDependencies(path, true);
                }
                foreach (var item in deps) {
                    if (item == select_path) {
                        Debug.LogError(path);
                        count++;
                        break;
                    }
                }
            }
        }
        Debug.LogError(select_path + "检查完成，共有" + count + "个引用关系");
    }
    [MenuItem("Tools/PC/Build PC platform", false, 5)]
    [Obsolete("Obsolete")]
    static void BuildPC() {
#if !UNITY_STANDALONE_WIN
    Debug.LogError("请先切换平台到PC");
    return;
#endif
	    PlayerSettings.defaultScreenWidth = 1280;
	    PlayerSettings.defaultScreenHeight = 720;
	    PlayerSettings.runInBackground = true;
	    PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
	    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "UNUSE_ASSETBOUNDLE_INEDITOR;PC_BUILD");
	    string streamingAssetsPath = "Assets/StreamingAssets";
	    if (!Directory.Exists(streamingAssetsPath))
	    {
		    Directory.CreateDirectory(streamingAssetsPath);
	    }
	    string resourcesPath = "Assets/Resources";
	    if (!Directory.Exists(resourcesPath))
	    {
		    Directory.CreateDirectory(resourcesPath);
	    }
	    string[] levels = {
		    "Assets/Scene/Main.unity",
	    };
	    PlayerSettings.bundleVersion = GameConfig.ClientVersionStr;
	    string path = "D:/build_source/PC/捕鱼_PC_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".exe";
	    BuildOptions buildOptions = BuildOptions.None;
#if DEBUG
	    buildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
#endif
    
	    BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows64, buildOptions);
    
	    Debug.Log("PC Build completed: " + path);
    }
}
