#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using Kubility;
using System.Text;

public class AssetBundlerBuilder : AssetBundlerBase
{
	public static  BuildAssetBundleOptions DefaultSettings = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
//#if UNITY_5_3
//#if UNITY_IPHONE || UNITY_IOS
//    private BuildAssetBundleOptions Default_Settings = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle ;
//#else

//    private BuildAssetBundleOptions Default_Settings = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle;
//#endif

//#else
//#if UNITY_IPHONE || UNITY_IOS
//    private BuildAssetBundleOptions Default_Settings = BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DeterministicAssetBundle ;
//#else

	private BuildAssetBundleOptions Default_Settings = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression ;
//#endif
    
//#endif
    private BuildAssetBundleOptions Curent_Settings;
	private AssetBundleManifest manifest;
	private BuildTarget Curent_target = EditorUserBuildSettings.activeBuildTarget;

	public AssetBundlerBuilder ()
	{

		string outpath = ABLoadConfig.GetAssetBundleOutPutPath ();
		if(!Directory.Exists(outpath))
		{
			Directory.CreateDirectory( outpath);
		}
		Curent_Settings = Default_Settings;
	}

	public void Start ()
	{
		LogMgr.Log ("开始打包 >> 将会打包当前平台的assetbundle >>");
		Running = true;
		EditorUtility.DisplayProgressBar ("打包", "清空打包残留", 0.5f);
		DeleteFolder (ABLoadConfig.GetAssetBundleOutPutPath ());
		manifest =	BuildPipeline.BuildAssetBundles ( ABLoadConfig.GetAssetBundleOutPutPath(), Curent_Settings, Curent_target);

	}

	public void UpdateBuilderTarget (BuildTarget target)
	{
		if (CheckRunning) {
			Curent_target = target;
			LogMgr.Log (">>  打包设置已经更新  >>" + Curent_target);
		}
	}

	public AssetBundleManifest End ()
	{
		LogMgr.Log ("打包结束");
		Running = false;
		return manifest;
	}

	public static bool DeleteFolder(string dir)
	{
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}
		else
		{
			foreach (string d in Directory.GetFileSystemEntries(dir))
			{
				if (File.Exists(d))
				{
					FileInfo fi = new FileInfo(d);
					if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
						fi.Attributes = FileAttributes.Normal;
					File.Delete(d);
				}
				else
				{
					DirectoryInfo d1 = new DirectoryInfo(d);
					if (d1.GetFiles().Length != 0)
					{
						DeleteFolder(d1.FullName);////递归删除子文件夹
					}
					Directory.Delete(d);
				}
			}
		}
		return true;
	}
}
#endif