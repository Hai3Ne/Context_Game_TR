using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class FishVersionTools : EditorWindow 
{
	string versioninfo = null;
	string resServerPath;
	void OnGUI()
	{
		if (versioninfo == null) {
            versioninfo = PlayerPrefs.GetString("MakeVersionNo", "");
		}
		if (resServerPath == null)
            resServerPath = PlayerPrefs.GetString("assetServerPath", "");
		
		versioninfo = EditorGUILayout.TextField ("版本号设置:", versioninfo);
		resServerPath = EditorGUILayout.TextField ("同步资源到服务器:", resServerPath);
		string srvPath = resServerPath + "/" + KApplication.GetPlatfomrName ().ToUpper();

		if (GUILayout.Button ("打版本资源")) {
			PlayerPrefs.SetString ("MakeVersionNo",versioninfo);
            PlayerPrefs.SetString("assetServerPath", resServerPath);
            PlayerPrefs.Save();
			AssetBundlePacker.BuildVersionFile (versioninfo);
			if (!string.IsNullOrEmpty (versioninfo))
				RefreshAssets ();
			AssetDatabase.Refresh ();
			if (string.IsNullOrEmpty (srvPath))
				return;
			DirectoryInfo dirInfo = new DirectoryInfo (srvPath);

			if (dirInfo.Exists) {
				var fss = dirInfo.GetFiles ("*.*", SearchOption.AllDirectories);
				foreach (var fs in fss) {
					fs.Delete ();
				}
				RemoveEmptyDir (dirInfo, true);
			} else {
				Directory.CreateDirectory (srvPath);
			}

			File.Copy (Application.dataPath + "/Settings/Version.json", srvPath+"/Version.json");
			copyDirecty(Application.dataPath + "/StreamingAssets/KB", srvPath + "/KB");
			copyDirecty(Application.dataPath + "/StreamingAssets/Lua", srvPath + "/Lua");
			EditorUtility.DisplayDialog ("tishi", "版本资源更新..", "OK");
		}
	}

	void RemoveEmptyDir(DirectoryInfo dirInfo, bool isroot){
		var dirs = dirInfo.GetDirectories();
		if (dirs.Length == 0) {
			if (isroot == false)
				dirInfo.Delete ();
		}
		else
		{
			foreach(var d in dirs)
				RemoveEmptyDir(d, false);
			if (isroot == false)
				dirInfo.Delete ();
		}
	}

	void copyDirecty(string src, string tar){
		DirectoryInfo srcDir = new DirectoryInfo (src);
		DirectoryInfo tarDir = new DirectoryInfo (tar);
		if (!tarDir.Exists)
			tarDir = Directory.CreateDirectory (tar);
		var srcfs = srcDir.GetFiles ();
		foreach (var s in srcfs) {
			if (s.FullName.EndsWith (".meta"))
				continue;
			File.Copy (s.FullName, tarDir.FullName + "/" + s.Name);
		}
	}

	void RefreshAssets()
    {
		if (File.Exists(Application.streamingAssetsPath + "/KB/"+ABLoadConfig.VersionPath))
        {
            Dictionary<string, ABData> VersionDic = new Dictionary<string, ABData>();
            var fs = new FileStream(Application.streamingAssetsPath + "/KB/assets.bytes", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader buffer = new BinaryReader(fs);
            int fhead = buffer.ReadInt32();
			if (fhead != ConstValue.VersionHeadCRC)
            {
				LogMgr.LogError("ERROR>> 文件校验不一致！");
                buffer.Close();
                return;
            }


			VersionManager.localVersion = buffer.ReadString();
			long newVersionCode = VersionManager.MakeVerionNo (VersionManager.localVersion);
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
				info.VersionCode = newVersionCode;
                for (int j = 0; j < DepdLen; ++j)
                {

                    var data = new DependData();
                    data.Abname = buffer.ReadString();
                    data.FileType = buffer.ReadInt16();
                    info.MyNeedDepends.TryAdd(data);
                }


                if (!string.IsNullOrEmpty(info.Abname) && !VersionDic.ContainsKey(info.Abname))
                    VersionDic.Add(info.Abname, info);
                else
					LogMgr.LogError("重复键值  " + info.Abname);
            }

            fs.Close();
            fs = new FileStream(Application.streamingAssetsPath + "/KB/assets.bytes", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fs.SetLength(0);
            BinaryWriter writer = new BinaryWriter(fs);

			writer.Write(ConstValue.VersionHeadCRC);
            writer.Write(versioninfo);
            writer.Write(ListCount);

            foreach (var sub in VersionDic)
            {
                ABData info = sub.Value;
                writer.Write(info.Abname);
                writer.Write(info.Size);
                writer.Write(info.FileType);
                writer.Write(info.LoadName);
                writer.Write(info.RootType);
                writer.Write(info.VersionCode);
                writer.Write(info.Hash);
                writer.Write(info.MyNeedDepends.Count);
                foreach (var msub in info.MyNeedDepends)
                {
                    writer.Write(msub.Abname);
                    writer.Write(msub.FileType);
                }
            }

            writer.Close();
            buffer.Close();
            fs.Close();
        }

    }
}
