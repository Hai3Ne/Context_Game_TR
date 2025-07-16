#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundlePacker
{
	public static string localVersion = "0.0.1";
	public static string VersionJsonFile = Application.dataPath + "/Settings/"+ConstValue.VersionJsonFile;


	[MenuItem ("Assets/AssetBundles/BuildForPC")]
	public static void BuildAssetBundleForPC ()
	{
		bool includeFolder = EditorUtility.DisplayDialog ("Assetbundle 打包对话框", "是否打包所在文件夹？", "yes", "no");
		try {
			PrepareBuild (includeFolder,BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
		} catch (Exception ex) {
			LogMgr.LogError (ex.Message);
			EditorUtility.ClearProgressBar ();
		}
	}

	[MenuItem ("Assets/AssetBundles/BuildForAndroid")]
	static void BuildForAndroid(){
		bool onlyconfig = EditorUtility.DisplayDialog ("Assetbundle 打包对话框", "是否只打包配置", "yes", "no");
		if (onlyconfig) {
			_confPacker = new PackConfigWriter ();
			if (_confPacker.Start ()) {
				EditorUtility.DisplayDialog ("提示", "恭喜您,配置表打包完成了", "OK");
			}
			return;
		}

		bool includeFolder = EditorUtility.DisplayDialog ("Assetbundle 打包对话框", "是否打包所在文件夹？", "yes", "no");

		try {
			PrepareBuild (includeFolder,BuildTargetGroup.Android, BuildTarget.Android);
		} catch (Exception ex) {
			LogMgr.LogError (ex.Message);
			EditorUtility.ClearProgressBar ();
		}
	}

	[MenuItem ("Assets/AssetBundles/BuildForIOS")]
	static void BuildForIos(){
		bool includeFolder = EditorUtility.DisplayDialog ("Assetbundle 打包对话框", "是否打包所在文件夹？", "yes", "no");

		try {
			PrepareBuild (includeFolder,BuildTargetGroup.iOS, BuildTarget.iOS);
		} catch (Exception ex) {
			LogMgr.LogError (ex.Message);
			EditorUtility.ClearProgressBar ();
		}
	}


	[MenuItem ("Assets/AssetBundles/SearchRefercens")]
	static void SearchRefercens(){
		string FullPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		string[] respaths = AssetDatabase.GetAllAssetPaths ();
		string outstr = "";
		//		respaths = new string[]{"Assets/Arts/UIAtlas/CommonAtlas.mat"};
		EditorUtility.ClearProgressBar ();
		float progress = 0f;
		for (int i = 0; i < respaths.Length; i++) {
			EditorUtility.DisplayCancelableProgressBar ("Search Refercens", "Refercens " + FullPath+" ..." + i, progress);
			progress = i * 1.0f / respaths.Length;
			if (respaths [i].StartsWith ("Assets")) {
				if (respaths [i] == FullPath)
					continue;
				string[] deps = AssetDatabase.GetDependencies (new string[] { respaths [i] });
				for (int j = 0; j < deps.Length; j++) {
					if (deps [j] == FullPath) {
						outstr += respaths [j] + "\n";
						break;
					}
				}
			}
		}

		EditorUtility.ClearProgressBar ();
		Debug.Log (outstr);
		EditorUtility.DisplayDialog ("tishi", outstr, "OK");
	}

	public static void PrepareBuild (bool includeFolder,BuildTargetGroup group, BuildTarget target,string path = null)
	{
		if (Selection.assetGUIDs.Length > 0 || !string.IsNullOrEmpty (path)) {
			string curFilePath = null;
			if (string.IsNullOrEmpty (path)) 
			{
				curFilePath = Path.GetFullPath (AssetDatabase.GUIDToAssetPath (Selection.assetGUIDs [0]));
				if (!EditorUtility.DisplayDialog ("对话框", "当前路径为" + curFilePath + "是否确定？", "OK", "No")) 
					return;
			}
			else 
			{
				curFilePath = path;
			}

			if (EditorUserBuildSettings.activeBuildTarget != target) {
				EditorUtility.DisplayDialog ("tips", "当前平台不匹配，先执行转换！", "OK");
                EditorUserBuildSettings.SwitchActiveBuildTarget(group, target);
			}
			else 
			{
				string dirname = "";
				if (File.Exists (curFilePath)) {
					dirname = Path.GetDirectoryName (curFilePath);
				} else if (Directory.Exists (curFilePath)) {
					dirname = curFilePath;
				}
				string[] arr = includeFolder ? new string[] { dirname } : new string[] { curFilePath };

                StartPack(target, arr);
                EditorUtility.ClearProgressBar();
            }

		} else {
			LogMgr.Log ("Must select file first ");
		}
	}

	static PackConfigWriter _confPacker;
	static AssetBundlerAnalyz _analy;
	static AssetBundlerBuilder _builder;
	static AssetBundlerWriter _writer;
	static void StartPack(BuildTarget target,string[] arr, bool needBuild = true)
	{
		_analy = new AssetBundlerAnalyz ();
		_builder = new AssetBundlerBuilder ();
		_writer = new AssetBundlerWriter ();

		_builder.UpdateBuilderTarget (target);
		_analy.UpdateFilePaths (arr);
		_analy.Start ();
		if (needBuild)
			_builder.Start();
		Directory.Delete(Application.dataPath + "/temp/", true);
		ABCacheDataMgr data = _analy.End ();
		AssetBundleManifest mainfest = needBuild ? _builder.End () : null;
		_writer.Start (data, mainfest, _analy.noBuildBundleFileAbName);
		_writer.End ();
		EditorUtility.ClearProgressBar ();
	}


	public static void BuildVersionFile (string Pversion)
	{
//		string text  ="";
//		if(File.Exists(VersionJsonFile))
//			text = File.ReadAllText(VersionJsonFile);
//		VersionJsonInfo jsonInfo = ParseUtils.Json_Deserialize<VersionJsonInfo>(text);

		string version = VersionManager.localVersion;
		if(!string.IsNullOrEmpty(Pversion))
			version = Pversion;
		
		VersionJsonInfo jsonInfo = new VersionJsonInfo();
		jsonInfo.MainVersionID = int.Parse (version.TrySplit (".", -3));
		jsonInfo.SubVersionID = int.Parse (version.TrySplit (".", -2));
		jsonInfo.ThirdVersionID = int.Parse (version.TrySplit (".", -1));

		var fs = new FileStream(Application.streamingAssetsPath + "/KB/"+ABLoadConfig.VersionPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		BinaryReader buffer = new BinaryReader(fs);
		int fhead = buffer.ReadInt32();
		if (fhead != ConstValue.VersionHeadCRC)
		{
			LogMgr.LogError("ERROR>> 文件校验不一致！");
			buffer.Close();
			fs.Close();
			return;
		}

		LogMgr.Log("上次打包文件版本号-->->-> "+ buffer.ReadString());
		var VersionDic = new Dictionary<string,ABData>();
		int ListCount = buffer.ReadInt32();
		for (int i = 0; i < ListCount; ++i)
		{
			ABData info = new ABData();

			info.Abname = buffer.ReadString();
			info.Size = buffer.ReadInt64();
			info.FileType = buffer.ReadInt16();
			info.LoadName = buffer.ReadString();
			info.RootType = buffer.ReadInt32();
			info.VersionCode = buffer.ReadInt64();
			info.Hash = buffer.ReadString();
			int DepdLen = buffer.ReadInt32();
			// LogMgr.LogError("abname -> "+ info.Abname +"  ->  "+ info.Hash);
			for (int j = 0; j < DepdLen; ++j)
			{
				var data = new DependData();
				data.Abname = buffer.ReadString();
				data.FileType = buffer.ReadInt16();
				info.MyNeedDepends.TryAdd(data);
			}

			if (!string.IsNullOrEmpty(info.Hash) && !VersionDic.ContainsKey(info.Hash))
				VersionDic.Add(info.Hash, info);
			else
				LogMgr.LogError("重复键值  " + info.Abname);
		}

		buffer.Close();
		fs.Close();

		var list = jsonInfo.getList();
		byte[] bys;
		foreach (var sub in VersionDic)
		{
			bys = File.ReadAllBytes (Application.streamingAssetsPath + "/KB/" + sub.Key + ".kb");
			string platPath = KApplication.GetPlatfomrName ().ToUpper () + "/KB/" + sub.Key + ".kb?" + bys.Length;
			if (!list.ContainsKey(sub.Value.Abname))
			{
				list.Add(sub.Value.Abname, platPath);
			}
		}

		bys = File.ReadAllBytes (Application.streamingAssetsPath + "/KB/" + ABLoadConfig.VersionPath);
		list.Add (ABLoadConfig.VersionPath, KApplication.GetPlatfomrName ().ToUpper () + "/KB/" + ABLoadConfig.VersionPath+"?"+bys.Length);

		DirectoryInfo luaRoot = new DirectoryInfo(Application.streamingAssetsPath+"/Lua");
		FileInfo[] files = luaRoot.GetFiles ("*.unity3d", SearchOption.AllDirectories);
		foreach (var lua_file in files)
		{
			bys = File.ReadAllBytes (lua_file.FullName);
			string luapath = lua_file.Name;
			string key = luapath.Substring (0,luapath.LastIndexOf("."));
			string platLuaPath = KApplication.GetPlatfomrName().ToUpper()+"/Lua/" + luapath;
			platLuaPath+="?" + bys.Length;
			list.TryAdd (key, platLuaPath);
		}

		using (var mfs = new FileStream (VersionJsonFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
			string info = ParseUtils.Json_Serialize (jsonInfo);
			bys = System.Text.Encoding.UTF8.GetBytes (info);

			mfs.SetLength (0);

			mfs.Write (bys, 0, bys.Length);

			mfs.Close ();
		}

	}

}
#endif

