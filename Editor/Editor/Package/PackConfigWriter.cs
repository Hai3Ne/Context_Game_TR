#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PackConfigWriter
{
	private long versionValue = 0;
	List<ABCacheDataInfo> newByteDataInfos = new List<ABCacheDataInfo>();
	List<ABCacheDataInfo> initAssetDataInfos = new List<ABCacheDataInfo>();
	List<string> needRemovefile = new List<string> ();
	string KBPath = Application.dataPath + "/StreamingAssets/KB/";
	string assetVersionFile;
	string versinString = "";
	string luaBundleOutputDir;
	public bool Start(){
		assetVersionFile = KBPath + ABLoadConfig.VersionPath;
		if (!File.Exists (assetVersionFile)) {
			EditorUtility.DisplayDialog ("错误", ABLoadConfig.VersionPath + "文件不存在哦", "OK");
			return false;
		}

		UpdateLuaScripts ();

		needRemovefile.Clear ();
		initAssetDataInfos.Clear ();
		using (FileStream fs = new FileStream (assetVersionFile, FileMode.Open, FileAccess.ReadWrite)) {
			BinaryReader fReader = new BinaryReader (fs);
			Debug.Assert (fReader.ReadInt32 () == ConstValue.VersionHeadCRC);
			versinString = fReader.ReadString ();
			LogMgr.Log ("localVersion:"+versinString);
			int count = fReader.ReadInt32 ();
			for (int i = 0; i < count; i++) {
				ABCacheDataInfo info = new ABCacheDataInfo ();
				info.Data = new ABData ();

				info.Data.Abname = fReader.ReadString();
				info.Data.Size = fReader.ReadInt64();
				info.Data.FileType = fReader.ReadInt16();
				info.Data.LoadName = fReader.ReadString();
				info.Data.RootType = fReader.ReadInt32 ();
				info.Data.VersionCode = fReader.ReadInt64 ();
				info.Data.Hash = fReader.ReadString();
				info.Data.MyNeedDepends = new System.Collections.Generic.List<DependData> ();

				int dependsCnt = fReader.ReadInt32 ();
				for (int j = 0; j < dependsCnt; j++)
				{
					DependData dep = new DependData ();
					dep.Abname = fReader.ReadString ();
					dep.FileType = fReader.ReadInt16 ();
					info.Data.MyNeedDepends.Add (dep);
				}

				versionValue = info.Data.VersionCode;
				if (info.Data.FileType == (short)ABFileTag.Bytes) {
					string fn = KBPath + info.Data.Hash + ABLoadConfig.ConvertFileExtension;
					needRemovefile.Add (fn);
				} else {
					initAssetDataInfos.Add (info);
				}
			}
		}

		if (RemoveOldByteFiles () == false)
			return false;
		
		if (AnalyzByteConfigInfos () == false) {
			EditorUtility.DisplayDialog ("错误", "没有找到bytes文件哦", "OK");
			return false;
		}

		if (ReWriteNewVersionfiles () == false)
			return false;
		
		return true;
	}

	bool RemoveOldByteFiles()
	{
		foreach (var fullpath in needRemovefile) {
			if (File.Exists (fullpath)) {
				File.Delete (fullpath);
			}
		}
		return true;
	}

	bool AnalyzByteConfigInfos()
	{
		newByteDataInfos.Clear ();
		DirectoryInfo dir = new DirectoryInfo (Application.dataPath + "/Arts/GameRes/Config/Bytes");
		if (!dir.Exists) 
			return false;

		FileInfo[] flist = dir.GetFiles ();
		if (flist.Length == 0)
			return false;
		
		foreach (var file in flist) {
			if (file.Extension == ".meta")
				continue;
			string assetPath = GetAssetPath(file.FullName);
			ABCacheDataInfo info = new ABCacheDataInfo ();
			info.Data = new ABData ();
			info.Data.Hash = AssetBundlerAnalyz.MD5 (file.FullName);
			info.Data.Abname = AssetBundlerAnalyz.ConvertABName (assetPath);
			info.Data.FileType = (short)ABFileTag.Bytes;
			info.Data.Size = file.Length;
			info.Data.LoadName = file.Name;
			info.Data.VersionCode = versionValue;
			info.Data.MyNeedDepends = new List<DependData> ();
			info.Data.RootType = (int)ABNodeTag.ROOT;

			string newfn = KBPath + info.Data.Hash + ABLoadConfig.ConvertFileExtension;
			File.Copy (file.FullName, newfn);
			LogMgr.Log ("update."+newfn);
			newByteDataInfos.Add (info);
		}
		AssetDatabase.Refresh ();
		return true;
	}

	bool ReWriteNewVersionfiles(){
		newByteDataInfos.ForEach (x => initAssetDataInfos.Insert (0, x));
		using (FileStream fs = new FileStream (assetVersionFile, FileMode.Open, FileAccess.Write)) {
			BinaryWriter bwrite = new BinaryWriter (fs);
			bwrite.Write (ConstValue.VersionHeadCRC);
			bwrite.Write (versinString);
			bwrite.Write (initAssetDataInfos.Count);
			for (int i = 0; i < initAssetDataInfos.Count; i++) {
				var info = initAssetDataInfos [i];
				bwrite.Write (info.Data.Abname);

				bwrite.Write (info.Data.Size);
				bwrite.Write (info.Data.FileType);
				bwrite.Write (info.Data.LoadName);
				bwrite.Write (info.Data.RootType);
				bwrite.Write (info.Data.VersionCode);
				bwrite.Write (info.Data.Hash);
				bwrite.Write (info.Data.MyNeedDepends.Count);
				for (int k = 0; k < info.Data.MyNeedDepends.Count; k++) {
					var dep = info.Data.MyNeedDepends [k];
					bwrite.Write (dep.Abname);
					bwrite.Write (dep.FileType);
				}
			}
		}
		return true;
	}

	const string splt_string = "Assets/";
	const string splt_string2 = "Assets\\";
	string GetAssetPath(string FullPath){
		int Fullindex = FullPath.IndexOf (splt_string);
		if (Fullindex < 0)
			Fullindex = FullPath.IndexOf (splt_string2);
		string str = FullPath.Substring (Fullindex);
		return str;
	}

	void UpdateLuaScripts(){
	//	ToLuaMenu.BuildNotJitBundles (false);

		luaBundleOutputDir = Application.dataPath + "/../" + ABLoadConfig.BasePath+"/Lua";
		if (!Directory.Exists (luaBundleOutputDir))
			Directory.CreateDirectory (luaBundleOutputDir);

		AssetBundlerBuilder.DeleteFolder (luaBundleOutputDir);
		BuildPipeline.BuildAssetBundles ( luaBundleOutputDir, AssetBundlerBuilder.DefaultSettings, EditorUserBuildSettings.activeBuildTarget);
		ClearManifest ();
		Directory.Delete(Application.dataPath + "/temp/", true);
		CopyLuaFiles ();
	}

	private void CopyLuaFiles(){
		DirectoryInfo dir = new DirectoryInfo(luaBundleOutputDir);
		FileInfo[] files = dir.GetFiles("lua*", SearchOption.AllDirectories);
		string output = Application.streamingAssetsPath+"/Lua/";
		AssetBundlerBuilder.DeleteFolder (output);
		this.ShowProgressAndForeach<FileInfo>(files, "Lua 脚本Copying", "Lua 脚本移动StreamAssets目录", "", delegate (FileInfo f)
			{
				File.Move(f.FullName, output+f.Name);
			});
	}

	protected void ShowProgressAndForeach<T> (T[] list, string title, string content,string append, Action<T> callback)
	{
		EditorUtility.ClearProgressBar ();
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
			EditorUtility.ClearProgressBar ();

			callback (val);
		}
	}

	private void ClearManifest()
	{
		DirectoryInfo dir = new DirectoryInfo(luaBundleOutputDir);
		FileInfo[] files = dir.GetFiles("*.manifest", SearchOption.AllDirectories);
		this.ShowProgressAndForeach<FileInfo>(files, "清除", "清除manifest...", "", delegate (FileInfo f)
			{
				f.Delete();
				DeleteMeta(f);
			});
	}

	void DeleteMeta(FileInfo f)
	{
		string path = f.FullName + ".meta";
		File.Delete(path);
	}
}



#endif