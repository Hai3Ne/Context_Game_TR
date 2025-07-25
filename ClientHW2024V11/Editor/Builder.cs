
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class Builder : Editor
{ 

    public static void CpOutListToStream()
    {
        string srcfile = Application.dataPath.Replace("Assets", "") + "/Out/ABList_Compress";
        string targetTmpFile = Application.streamingAssetsPath + "/ABList_Compress";

        if (System.IO.File.Exists(targetTmpFile))
        {
            try
            {
                System.IO.File.Delete(targetTmpFile);
            }
            catch (System.IO.IOException e)
            {
            }
        }


        System.IO.File.Copy(srcfile, targetTmpFile, true);
    }
    //dll 生成到stream 目录
    public static void UpdateDllVersionToStream()
    {
        string targetTmpFile = Application.streamingAssetsPath + "/DllVersion.txt";
        if(File.Exists(targetTmpFile))File.Delete(targetTmpFile);
        string _txt = "1.0.1";
        FileStream fs1 = new FileStream(targetTmpFile, FileMode.Create, FileAccess.Write);//创建写入文件 
        StreamWriter sw = new StreamWriter(fs1);
        sw.WriteLine(_txt);//开始写入值
        sw.Close();
        fs1.Close();

    }

    public static void DeleteFolder(string dir)
    {
        foreach(string d in Directory.GetFileSystemEntries(dir))
        {
            if(File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("readOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if(d1.GetFiles().Length != 0)
                {
                    DeleteFolder(d1.FullName);
                }
                Directory.Delete(d);
            }
        }
    }
    public static void CopyDirectory(string sourcePath,string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach(FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string desName = Path.Combine(destinationPath, fsi.Name);
            if (fsi is System.IO.FileInfo)
                File.Copy(fsi.FullName, desName);
            else
            {
                Directory.CreateDirectory(desName);
                CopyDirectory(fsi.FullName, desName);
            }
        }
    }

    [MenuItem("Tools/BuildTest/buildpc")] 
    static public void Buildpc()
    {
        if (!Directory.Exists(Application.dataPath + "/../bd/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/../bd/"); 
        }

        BuildPipeline.BuildPlayer(GetBuildScenes(), Application.dataPath + "/../bd/mssj.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
    } 
    [MenuItem("Tools/BuildTest/buildAndroidTest")]
    static public void BuildAndroidTest()
    {
        string outputPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        BuildPipeline.BuildPlayer(GetBuildScenes(), outputPath + "/app.apk", BuildTarget.Android, BuildOptions.None);
    }
    static string[] GetBuildScenes()
    {
        List<string> pathList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && (scene.path.Contains("ThirdPartyUI") || scene.path.Contains("Init")))
            {
                pathList.Add(scene.path);
            }
        }
        return pathList.ToArray();
    }
}