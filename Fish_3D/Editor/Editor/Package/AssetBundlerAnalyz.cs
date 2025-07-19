#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using Kubility;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;

public class AssetBundlerAnalyz : AssetBundlerBase
{
    public const bool UseDataBase = true;
    public const bool UseShaderAB = false;
    private const string Atlaspath = "Arts/UIAtlas/";
    private ABCacheDataMgr data;

    private string Default_Path = Application.dataPath;
	public static string[] Default_Options = {
		"*.prefab",
		"*.png",
		"*.tga",
		"*.mp3",
		"*.txt",
		"*.ttf",
		"*.wav",
		"*.ogg",
		"*.jpg",
		"*.byte",
		"*.xml",
		"*.unity"
	};

	List<ABFileTag> exCludetBundleFileTypes = new List<ABFileTag>(new ABFileTag[]{ABFileTag.Bytes});
    private const string Default_Scene = "Main.unity";
    string[] Curent_Path;
    string[] Curent_Options;
    string Curent_Scene;

    string splt_string = "Assets/";

    private long versionValue = 0;
    List<string> ExincludeFiles = new List<string> ();
   
    public AssetBundlerAnalyz ()
    {
        Curent_Options = Default_Options;
        Curent_Path = new string[] { Default_Path };
        Curent_Scene = Default_Scene;
		versionValue = VersionManager.MakeVerionNo (versionStr);
        InitABCacheData ();
    }

	string shaderBuffStr = "";
	string versionStr { get { return AssetBundlePacker.localVersion;}}
    public void InitABCacheData ()
    {
        try {

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
                splt_string = "Assets\\";
            }

			if (File.Exists(CacheDepFilePath))
			{
	            string text = "";
				using (var fs = new FileStream (CacheDepFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
	                var bys = new byte[fs.Length];
	                fs.Read (bys, 0, bys.Length);
	                text = System.Text.Encoding.UTF8.GetString (bys);
	                fs.Close ();
	            }

	            ABCacheDataMgr temp = ParseUtils.Json_Deserialize<ABCacheDataMgr> (text);
	            if (temp != null && temp.GetList ().Count > 0) {
	                data = temp;
	                data.OldList.Clear ();
	                data.NewList.Clear ();
	            } else {
	                data = new ABCacheDataMgr ();
	            }
			}
			else
			{
				data = new ABCacheDataMgr ();
			}
			data.Version = AssetBundlePacker.localVersion;
			if (KApplication.isIOS) {
				data.Platform = "IOS";
			} else if (KApplication.isAndroid) {
				data.Platform = "Android";
			} else {
				data.Platform = Application.platform.ToString ();
			}
            LogMgr.Log (" >> 本地版本号为 >>" + data.Version + " >> 打包平台为  >> " + data.Platform);

        } catch (Exception ex) {
            LogMgr.LogError (ex);
        }
    }

    public void UpdateSearchOptions (string[] Opts)
    {
        if (CheckRunning) {
            Curent_Options = Opts;
            string str = "";
            foreach (var sub in Opts) {
                str += sub + ";";
            }
            LogMgr.Log (">>Opts 更新  >> 最新搜索条件设置为 >>" + str);
        }

    }

    public void UpdateFilePaths (string[] Paths)
    {
        if (CheckRunning) {
            Curent_Path = Paths;
            string str = "";
            foreach (var sub in Paths) {
                str += sub + ";";
            }
            LogMgr.Log (">>Paths 更新  >> 最新搜索路径设置为 >>" + str);
        }

    }

    public void UpdateMainScene (string Path)
    {
        if (CheckRunning) {
            Curent_Scene = Path;

            LogMgr.Log (">>MainScene 更新  >> 最新MainScene设置为 >>" + Curent_Scene);
        }

    }

    public void PushExincludeFile (string filepath)
    {
        if (CheckRunning) {

            ExincludeFiles.TryAdd (Path.GetFullPath (filepath));
            LogMgr.Log (">>ExincludeFile 更新  >>" + filepath);
        }
    }


    public void Start ()
    {
		shaderBuffStr = "";
		shaderDictmap.Clear ();
		noBuildBundleFileAbName.Clear ();
        LogMgr.Log ("开始分析>>");
        Running = true;
        string targetPath = "";
        for (int i = 0; i < Curent_Path.Length; i++) {
            targetPath = Curent_Path [i];
            string subopt = "";
            if (File.Exists (targetPath)) { //单个文件
                EditorUtility.DisplayCancelableProgressBar ("分析 ", "开始分析 ....", 0f);
                for (int j = 0; j < Curent_Options.Length; j++) {
                    subopt = Curent_Options [j];
                    if (!string.IsNullOrEmpty (subopt)) {
                        string endOpt = subopt.TrySplit ('.', -1);
                        if (targetPath.EndsWith (endOpt)) {
                            EditorUtility.DisplayProgressBar ("分析", "分析中", 0.9f);
							AnalyzeSingleFile (targetPath);
                        }
                    } else {
                        LogMgr.LogError (subopt + " 》》 此搜素条件为空");
                    }
                }
            } else { //目录
                DirectoryInfo dirinfo = new DirectoryInfo (targetPath);
                EditorUtility.DisplayCancelableProgressBar ("分析 ", "开始分析 ....", 0f);

                for (int j = 0; j < Curent_Options.Length; j++) {
                    subopt = Curent_Options [j];
                    if (!string.IsNullOrEmpty (subopt)) {
                        FileInfo[] files = dirinfo.GetFiles (subopt, SearchOption.AllDirectories);
                        this.ShowProgressAndForeach<FileInfo> (files, "分析 ", "分析中", "源目录 >>" + targetPath, delegate (FileInfo file) {
                            if (file != null) {
								AnalyzeSingleFile(file.FullName);
                            }
                        });
                    } else {
                        LogMgr.LogError (subopt + " 》》 此搜素条件为空");
                    }

                }
            }
        }

        ABCacheDataInfo.PopCheck();

		AssetImporter[] imports = new AssetImporter[assetimportSetMap.Keys.Count];
		assetimportSetMap.Keys.CopyTo (imports, 0);

		float v = 0f;
		int len = imports.Length;
		string abnameStr;
		Dictionary<string, bool> aboundAbNamedict = new Dictionary<string, bool> ();
		string abnameBuff = "";
		for (int i = 0; i < len; i++){
			v = (float)i / len;
			abnameStr = assetimportSetMap [imports [i]].abname;
			EditorUtility.DisplayCancelableProgressBar ("AssetBundleName", "AssetBundleName ..." + " ..." +abnameStr , v);
			if (assetimportSetMap [imports [i]].times > 1 || assetimportSetMap [imports [i]].isRootNode) {
				imports [i].assetBundleName = abnameStr;
				abnameBuff += abnameStr+"\n";
				aboundAbNamedict [abnameStr] = true;
			}
		}
		LogMgr.Log (abnameBuff);

		List<ABCacheDataInfo>  cacheinfos = data.GetList ();
		for (int i = cacheinfos.Count-1; i >= 0; i--) {
			if (!aboundAbNamedict.ContainsKey (cacheinfos [i].Data.Abname)) {
				if (cacheinfos [i].Data.FileType == (short)ABFileTag.Bytes)
					continue;
				LogMgr.Log ("Removing assets: " + cacheinfos [i].Data.Abname);
				cacheinfos.RemoveAt (i);
			} else {
				List<DependData> MyNeedDepends = cacheinfos [i].Data.MyNeedDepends;
				for (int j = MyNeedDepends.Count-1; j >= 0; j--){
					string abname = MyNeedDepends [j].Abname;
					if (!aboundAbNamedict.ContainsKey (MyNeedDepends [j].Abname)) {
						MyNeedDepends.RemoveAt (j);
					}
					if (abname.IndexOf ("oldres") > 0) {
						LogMgr.Log (abname+" : "+cacheinfos [i].Data.Abname);
					}
				}
			}
		}

	//	ToLuaMenu.BuildNotJitBundles (false);
        EditorUtility.ClearProgressBar ();
    }

	void AnalyzeSingleFile(string fullFilePath)
	{
		int index = Mathf.Max (0, fullFilePath.IndexOf (splt_string));
		string filename = fullFilePath.Substring (index).Replace ("\\", "/");
		AssetDatabaseCollectDepends (filename, 0, true);
	}

	Dictionary<string,bool> shaderDictmap = new Dictionary<string, bool>();
	public List<string> noBuildBundleFileAbName = new List<string>();
    private void AssetDatabaseCollectDepends (string FullPath, int Deep, bool buildEnable, ABNodeTag node = ABNodeTag.ROOT)
    {
        if (ExincludeFiles.Contains (Path.GetFullPath (FullPath)))
            return;

        int Fullindex = Mathf.Max (0, FullPath.IndexOf (splt_string));
        FullPath = FullPath.Substring (Fullindex);
        string abname = ConvertABName (FullPath);
        ABFileTag fileType = KAssetBundleManger.GetDependTagWithAbName (FullPath);


		string LoadeName = Path.GetFileName (FullPath);
		LoadeName = LoadeName.Substring (0, LoadeName.LastIndexOf ("."));

        //try find
        ABCacheDataInfo info = data.GetList ().Find (p => p != null && p.Filepath.Equals (FullPath));
		if (exCludetBundleFileTypes.Contains(fileType))
		{
			noBuildBundleFileAbName.Add (abname);
		}

		if (CheckAbNeedUpdate (FullPath, fileType)) {
            string[] deps = AssetDatabase.GetDependencies (new string[] { FullPath });
            AssetImporter importer = AssetImporter.GetAtPath (FullPath);

            info = RefreshCacheData (buildEnable, importer, node, info, LoadeName, FullPath, abname, fileType);

            this.ShowProgressAndForeach<string> (deps, "分析 ", "分析依赖", "源资源 >>" + FullPath, delegate (string file) {

                if (file.Equals (FullPath)
                    || file.StartsWith ("Resources")
                    || file.ToLower ().Contains ("unity default resources")
                    || file.EndsWith (".cs")
                    || file.EndsWith (".dll")
                    || file.EndsWith (".js")) 
				{
                    //LogMgr.Log ("跳过资源 >>" + file);
                }
				else
				{
                    if (!file.EndsWith (".shader"))
                        Dep_add (file, info, ABNodeTag.ASSET, Deep++, fileType == ABFileTag.Scene ? true : false);
                    else {
						if (!shaderDictmap.ContainsKey(file))
						{
							string shaderName = GetShaderNameFromFile(Application.dataPath+"/../"+file);
							if (shaderName != null)
							{
								shaderDictmap.Add(file,true);
								shaderBuffStr += shaderName+"\n";
							}
						}
                        int index = Mathf.Max (0, file.IndexOf (splt_string));
                        string Dep_filename = file.Substring (index);
                        AssetImporter Depimporter = AssetImporter.GetAtPath (Dep_filename);
                        if (Depimporter != null && !string.IsNullOrEmpty (Depimporter.assetBundleName)) {
                            Depimporter.assetBundleName = null;
                        }
                    }
                }
            });
        } else {
            // LogMgr.Log(" >> 未发现变化 不需要更新 >> " + FullPath);
            if (info != null) {
                info.old = true;
                int index = Mathf.Max (0, FullPath.IndexOf (splt_string));
                string Dep_abname = ConvertABName (FullPath.Substring (index));

                if(buildEnable)
                {
                    DependData depData = DependData.Create (FullPath);
                    depData.Abname = Dep_abname;
                    depData.FileType = (short)fileType;
                    if(!info.Data.MyNeedDepends.Exists(p => p.Abname.Equals(Dep_abname)) && !info.Filepath.Equals(FullPath))
                    {
                        info.Data.MyNeedDepends.Add (depData);
                    }
                } 
                else if(!info.Filepath.Equals(FullPath))
                    ABCacheDataInfo.PushToPreDepends (FullPath, info);
            }
            data.OldList.TryAdd (FullPath);

        }
    }


	string GetShaderNameFromFile(string filepath){
		string[] lines = File.ReadAllLines(filepath);
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].StartsWith("Shader"))
			{
				string shaderStr = lines[i].Substring(6).Replace("{","").Replace("\"","");
				shaderStr = shaderStr.TrimStart ();
				shaderStr = shaderStr.TrimEnd ();
				return shaderStr;
			}
		}
		return null;
	}						


    private void Dep_add (string file, ABCacheDataInfo info, ABNodeTag node, int deep, bool ignore)
    {
        int index = Mathf.Max (0, file.IndexOf (splt_string));
        string Dep_filename = file.Substring (index);
        string Dep_abname = ConvertABName (file.Substring (index));
        ABFileTag Dep_fileType = KAssetBundleManger.GetDependTagWithAbName (Dep_filename);

        DependData depData = DependData.Create (file);
        depData.Abname = Dep_abname;
        depData.FileType = (short)Dep_fileType;
        if(!info.Data.MyNeedDepends.Exists(p => p.Abname.Equals(Dep_abname)) && !info.Filepath.Equals(file))
        {
            info.Data.MyNeedDepends.Add (depData);
        }
		AssetDatabaseCollectDepends (Dep_filename, deep, true, node);
      

    }

    private ABCacheDataInfo RefreshCacheData (bool buildEnable, AssetImporter importer, ABNodeTag node, ABCacheDataInfo info, string LoadeName, string FullPath, string abname, ABFileTag fileType)
    {
		if (LoadeName == "jpg") {
			LogMgr.Log (LoadeName);
		}
        byte[] bys = File.ReadAllBytes (Path.GetFullPath (FullPath));
        ///发现上次打包的数据
        if (info != null) {
            
            info.MD5 = MD5 (Path.GetFullPath (FullPath));
            info.Data.MyNeedDepends.Clear ();
            info.Data.Abname = abname;
            info.Filepath = FullPath;
            info.Data.Size = bys.Length;
            info.old = false;
            info.Data.FileType = (short)fileType;
            if (FullPath.Contains (Atlaspath) && fileType == ABFileTag.Prefab) {
                LogMgr.Log ("发现图集预设 " + FullPath);
                info.Data.RootType = (int)ABNodeTag.STANDALONE;
            } else if (FullPath.EndsWith (".unity")) {
                info.Data.RootType = (int)ABNodeTag.SCENEASSET;
            } else
                info.Data.RootType |= (int)node;
            
            info.Data.LoadName = LoadeName;

            info.Data.VersionCode = versionValue;

			if (buildEnable && importer.LogNull () && !FullPath.Contains (Curent_Scene) && !exCludetBundleFileTypes.Contains(fileType)) {
                //importer.assetBundleName = abname;
				setupAssetbundleName(importer, info.Data);
            } else if (!buildEnable) {
                //ABCacheDataInfo.PushToPreDepends (DependData.Create (FullPath), info);
            }
        } else {
            info = new ABCacheDataInfo ();
            info.MD5 = MD5 (Path.GetFullPath (FullPath));
            info.Filepath = FullPath;
            info.Data.Abname = abname;
            info.Data.Size = bys.Length;
            info.old = false;
            if (FullPath.Contains (Atlaspath) && fileType == ABFileTag.Prefab) {
                LogMgr.Log ("发现图集预设 " + FullPath);
                info.Data.RootType = (int)ABNodeTag.STANDALONE;
            } else if (FullPath.EndsWith (".unity")) {
                info.Data.RootType = (int)ABNodeTag.SCENEASSET;
            } else
                info.Data.RootType = (int)node;
            
            info.Data.LoadName = LoadeName;
            info.Data.FileType = (short)fileType;

            info.Data.VersionCode = versionValue;

            if (buildEnable && !data.GetList ().Exists (p => p.Filepath.Equals (FullPath))) {
                data.GetList ().Add (info);
            }

			if (buildEnable && importer.LogNull () && !FullPath.Contains (Curent_Scene) && !exCludetBundleFileTypes.Contains(fileType)) {
                //importer.assetBundleName = abname;
				setupAssetbundleName(importer, info.Data);
            } else if (!buildEnable) {
                //ABCacheDataInfo.PushToPreDepends (DependData.Create (FullPath), info);
            }
        }

        if (!buildEnable)
            data.NewList.TryAdd (FullPath);

        return info;
    }

	class importorData{
		public string abname;
		public int times;
		public bool isRootNode;
	}

	Dictionary<AssetImporter, importorData> assetimportSetMap = new Dictionary<AssetImporter, importorData>();
	void setupAssetbundleName(AssetImporter importer, ABData abdata) {// string abname, short fileType, bool isRootNode){
		string abname = abdata.Abname;
		bool isRootNode = abdata.RootType == (int)ABNodeTag.ROOT;
		if (!assetimportSetMap.ContainsKey (importer)) {
			assetimportSetMap [importer] = new importorData ();
			assetimportSetMap [importer].times = 1;
			assetimportSetMap [importer].abname = abname;
			assetimportSetMap [importer].isRootNode = isRootNode;
		}else{
			assetimportSetMap [importer].times += 1;
			assetimportSetMap [importer].isRootNode = isRootNode;
			if (assetimportSetMap [importer].abname != abname) {
				LogMgr.LogError (" error: "+assetimportSetMap [importer].abname+" "+abname);
			}
		}
	}



	private bool CheckAbNeedUpdate (string fullpath, ABFileTag fileType)
    {
        AssetImporter importer = AssetImporter.GetAtPath (fullpath);
		if (importer != null && (string.IsNullOrEmpty (importer.assetBundleName) && !exCludetBundleFileTypes.Contains(fileType))) {
            return true;
        }

        string realfullpath = Path.GetFullPath (fullpath);
        string md5 = MD5 (realfullpath);

        var temp = data.GetList ().Find (p => p != null && p.MD5.Equals (md5) && p.Filepath.Equals (fullpath));
        if (temp == null)
            return true;
        else
            return false;
    }

	public static string MD5(string filepath)
	{
		try
		{
			FileStream file = new FileStream(filepath, System.IO.FileMode.Open);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
			}
			return sb.ToString();
		}
		catch (Exception ex)
		{
			throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
		}
	}

    public ABCacheDataMgr End ()
    {
		File.WriteAllText (Application.dataPath + "/includeShaders.txt", shaderBuffStr);
        LogMgr.Log ("结束分析>>");
        Running = false;
        EditorUtility.ClearProgressBar ();
		return data;
    }
}
#endif