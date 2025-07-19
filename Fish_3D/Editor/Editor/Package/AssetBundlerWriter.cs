#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using Kubility;
using System.Text;
using System.Linq;

public class AssetBundlerWriter : AssetBundlerBase
{
    private AssetBundleManifest _mainfest;
    private ABCacheDataMgr data;
    private bool openBuild = true;

	public void Start(ABCacheDataMgr pdata, AssetBundleManifest manifest, List<string> directCopyAssetsAbnames)
    {
		data = pdata;
		_mainfest = manifest;
        LogMgr.Log(">> 准备写入");
        if (data.LogNull())
        {
			CopyDirectFiles (directCopyAssetsAbnames);
            WriteCacheData();
            if (ABLoadConfig.OpenSecret && openBuild)
                ConvertBytes();
            if (openBuild)
            {
                ClearManifest();
				CopyLuaFiles ();
                CopyFiles();
                ConvertFileNames();
            }
            else
            {
                File.Delete(Application.streamingAssetsPath + "/KB/"+ ABLoadConfig.VersionPath);
				File.Copy(ABLoadConfig.GetAssetBundleOutPutPath() + "/" + ABLoadConfig.VersionPath, Application.streamingAssetsPath + "/KB/" + ABLoadConfig.VersionPath);
            }
        }
    }

	void CopyDirectFiles(List<string> directCopyAssetsAbnames)
	{
		foreach(var str in directCopyAssetsAbnames)
		{
			
			string newstr = str.Replace ("=", "/");
			newstr = newstr.Substring (0, str.IndexOf ("."));
			newstr = newstr.Replace ("^", ".");
			newstr = newstr.Replace ("assets/", "");
			File.Copy (Application.dataPath+"/"+newstr, ABLoadConfig.GetAssetBundleOutPutPath ()+"/"+str);
		}
	}

	void CreateNewOrOldFiles()
	{
		string newDir = Application.dataPath + "/" + ABLoadConfig.BasePath + "/New_" + KApplication.GetPlatfomrName();
		string oldDir = Application.dataPath + "/" + ABLoadConfig.BasePath + "/Old_" +  KApplication.GetPlatfomrName();
		WriteNewOrOld(newDir, data.NewList);
		WriteNewOrOld(oldDir, data.OldList);
	}

    public void ConvertFileNames()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/KB");
        FileInfo[] files = dir.GetFiles("*" + ABLoadConfig.FileExtensions, SearchOption.AllDirectories);
        this.ShowProgressAndForeach<FileInfo>(files, "加密过程", "转换文件名...", "", delegate (FileInfo f)
        {
            if (f != null && !f.FullName.Contains(ABLoadConfig.VersionPath))
            {
                string Dir = Path.GetDirectoryName(f.FullName);
                string filename = Path.GetFileName(f.FullName);
                ABCacheDataInfo info = data.GetList().Find(p => p.Data.Abname.Equals(filename));
                if (info != null)
                {
                    string newFileName = info.Data.Hash + ABLoadConfig.ConvertFileExtension;
                    string outname = Path.Combine(Dir, newFileName);
                    if (File.Exists(outname))
                    {
                        var HashFiles = data.GetList().FindAll(p => p.Data.Hash.Equals(info.Data.Hash));
                        if (HashFiles.Count > 1)
                        {
                            LogMgr.LogError("危险的文件名， 》》" + HashFiles[0].Filepath + " other " + HashFiles[1].Filepath);
                        }
                        else
                        {
                            LogMgr.LogError("危险的文件名， 》》" + HashFiles[0].Filepath + "  检查代码！");
                        }
                        f.Delete();
                        DeleteMeta(f);
                    }
                    else
                    {
                        File.Move(f.FullName, outname);
                        f.Delete();
                        DeleteMeta(f);
                    }
                }
                else
                {
					LogMgr.LogError("未记录的文件 "+f.FullName);
                    f.Delete();
                    DeleteMeta(f);
                }
            }
            else
            {
                LogMgr.Log(f.FullName + ">> 不需要convert");
            }
        });
		string BaseDirFile = Application.streamingAssetsPath + "/KB/" + KApplication.GetPlatfomrName();
        File.Delete(BaseDirFile);
    }

    private void WriteCacheData()
    {
		string assetbytesFn = ABLoadConfig.GetAssetBundleOutPutPath() + "/" + ABLoadConfig.VersionPath;
        string dir = Path.GetDirectoryName(assetbytesFn);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (FileStream fs = new FileStream(assetbytesFn, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fs.SetLength(0);
            BinaryWriter buffer = new BinaryWriter(fs);
			buffer.Write(ConstValue.VersionHeadCRC);
			buffer.Write(AssetBundlePacker.localVersion);
            buffer.Write(data.GetList().Count);
            this.ShowProgressAndForeach<ABCacheDataInfo>(data.GetList(), "写入过程", "Hashing", "", delegate (ABCacheDataInfo info)
            {
                if (info != null)
                {
                    if (_mainfest != null)
                    {
                        Hash128 hash = _mainfest.GetAssetBundleHash(info.Data.Abname);
                        string hashstring = hash.ToString();
                        if (!hashstring.Equals("00000000000000000000000000000000"))
                        {
                            info.Data.Hash = hashstring;
                            LogMgr.Log("Hashing ....." + info.Data.Hash + "  >>  " + info.Data.Abname);
                        }
                        else
                        {
								info.Data.Hash = info.MD5;
                            LogMgr.Log("上次打包的文件 ....." + info.Data.Hash + "  >>  " + info.Data.Abname);
                        }                        
                    }

                    buffer.Write(info.Data.Abname);
                    buffer.Write(info.Data.Size);
                    buffer.Write(info.Data.FileType);
                    buffer.Write(info.Data.LoadName);
                    buffer.Write(info.Data.RootType);
                    buffer.Write(info.Data.VersionCode);
                    buffer.Write(info.Data.Hash);
                    buffer.Write(info.Data.MyNeedDepends.Count);
                    foreach (var sub in info.Data.MyNeedDepends)
                    {
                        buffer.Write(sub.Abname);
                        buffer.Write(sub.FileType);
                    }
                }
            });

            fs.Close();
        }

		File.WriteAllText(ABLoadConfig.GetCacheDepFilePath(), ParseUtils.Json_Serialize(data));
    }

    private void ConvertBytes()
    {
		DirectoryInfo dir = new DirectoryInfo(ABLoadConfig.GetAssetBundleOutPutPath());
        FileInfo[] files = dir.GetFiles("*" + ABLoadConfig.FileExtensions, SearchOption.AllDirectories);
        this.ShowProgressAndForeach<FileInfo>(files, "加密过程", "加密...", "", delegate (FileInfo f)
        {
            if (f != null && !f.FullName.Contains(ABLoadConfig.VersionPath))
            {
                using (var fs = new FileStream(f.FullName, FileMode.Open, FileAccess.ReadWrite))
                {
                    byte[] bys = new byte[fs.Length];

                    fs.Read(bys, 0, bys.Length);
                    fs.SetLength(0);

                    KAssetBundleManger.Ab_Convert(bys);

                    fs.Write(bys, 0, bys.Length);
                    fs.Close();
                    LogMgr.Log("加密的文件名  :"+ f.FullName);
                }
            }
            else
            {
                LogMgr.Log(f.FullName + ">> 不需要convert");
            }

        });

    }

    private void WriteNewOrOld(string DirName, List<string> list)
    {
        if (Directory.Exists(DirName))
        {
            EditorHelper.DeleteFolder(DirName);
        }
        else
        {
            Directory.CreateDirectory(DirName);
        }

        this.ShowProgressAndForeach<string>(list, "Write Copy", "writing...", "", delegate (string eachNew)
        {
            string abname = "";
            if (eachNew.Contains(ABLoadConfig.VersionPath))
            {
                abname = ABLoadConfig.VersionPath;
            }
            else
                abname = ConvertABName(eachNew);

				string src = ABLoadConfig.GetAssetBundleOutPutPath() + "/" + abname;
            if (File.Exists(src) && !File.Exists(DirName + "/" + abname))
                File.Copy(src, DirName + "/" + abname);
        });
    }

    private void CopyFiles()
    {
        EditorUtility.DisplayProgressBar("Title", "Copy Files", 0.5f);
        EditorHelper.DeleteFolder(Application.streamingAssetsPath + "/KB");
		EditorHelper.CopyDirectory(ABLoadConfig.GetAssetBundleOutPutPath(), Application.streamingAssetsPath + "/KB");
    }

	private void CopyLuaFiles(){
		DirectoryInfo dir = new DirectoryInfo(ABLoadConfig.GetAssetBundleOutPutPath());
		FileInfo[] files = dir.GetFiles("lua*", SearchOption.AllDirectories);
		string output = Application.streamingAssetsPath+"/Lua/";
		this.ShowProgressAndForeach<FileInfo>(files, "Lua 脚本Copying", "Lua 脚本移动StreamAssets目录", "", delegate (FileInfo f)
			{
				File.Move(f.FullName, output+f.Name);
			});
	}

    private void ClearManifest()
    {
		DirectoryInfo dir = new DirectoryInfo(ABLoadConfig.GetAssetBundleOutPutPath());
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

    public void End()
    {
        EditorUtility.ClearProgressBar();
        LogMgr.Log(">> 写入完成");
        AssetDatabase.Refresh();
    }

}
#endif