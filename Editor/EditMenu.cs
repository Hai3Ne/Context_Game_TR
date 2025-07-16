using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using System.IO;
using UnityEngine.Networking;
using System;

public class EditMenu
{
    [MenuItem("Tools/安卓打包导出资源", false, 4)]
    private static void CopyFileToBoundle()
    {
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.Delete(Application.streamingAssetsPath, true);
        }
        else
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        HandleDllBoundle();
        Debug.Log("DLL打包");
        MainHallAssetBundleExport_Android();
        Debug.Log("大厅资源打包");
        CopyFolder(Application.dataPath + "/../DLL", Application.streamingAssetsPath + "/DLL");
        Debug.Log("DLL复制完成");
        CopyFolder(Application.dataPath + "/../AssetBundle_ALL", Application.streamingAssetsPath + "/AssetBundle_ALL");
        Debug.Log("大厅资源复制完成");
        HandlefolderBoundle();


    }
    [MenuItem("Tools/Android/配置文件", false, 4)]
    private static void HandlefolderBoundle()
    {
        var fileArr = Directory.GetDirectories(Application.streamingAssetsPath);
        var arr = new string[fileArr.Length];
        for (int i = 0; i < fileArr.Length; i++)
        {
            arr[i] = Path.GetFileName(fileArr[i]);
        }
        File.WriteAllLines(Application.streamingAssetsPath + "/folder.byte", arr);
        Debug.Log("配置文件生成完成");
    }
    private static void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            Debug.Log("CopyFolder is finish.");
            return;
        }

        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        //获得源文件下所有文件
        List<string> files = new List<string>(Directory.GetFiles(srcPath));
        files.ForEach(f =>
        {
            string destFile = Path.Combine(tarPath, Path.GetFileName(f));
            File.Copy(f, destFile, true); //覆盖模式
        });

        //获得源文件下所有目录文件
        List<string> folders = new List<string>(Directory.GetDirectories(srcPath));
        folders.ForEach(f =>
        {
            string destDir = Path.Combine(tarPath, Path.GetFileName(f));
            CopyFolder(f, destDir); //递归实现子文件夹拷贝
        });
    }



    [MenuItem("Tools/Android/DLL", false, 4)]
    private static void HandleDllBoundle()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        CompileDllCommand.CompileDll(target);
        var dir = Application.dataPath + "/../DLL";
        DirectoryInfo dirOutDir = new DirectoryInfo(dir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(dir);
        }
        CopyAOTAssembliesToStreamingAssets();
        CopyHotUpdateAssembliesToStreamingAssets();

        var list = new List<AssetBundleInfo>();
        DirectoryInfo dirOutDir1= new DirectoryInfo(dir);
        var files = dirOutDir1.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var ab = new AssetBundleInfo();
            ab.ABname = file.Name;
            ab.MD5 = Tools.GetMD5HashFromFile(file.FullName);
            ab.Size = file.Length;
            list.Add(ab);
        }
        byte[] versionNO = System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddHHmm"));

        //生成版本号文件
        File.WriteAllBytes(dir + "/" + ABLoadConfig.VersionNO, versionNO);

        File.WriteAllLines(dir+"/" + ABLoadConfig.VersionPath, new string[]{
            LitJson.JsonMapper.ToJson(list),});
    }


    public static void CopyAOTAssembliesToStreamingAssets()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = Application.dataPath + "/../DLL";

        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
            if (!File.Exists(srcDllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                continue;
            }

            string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }
    }

    public static void CopyHotUpdateAssembliesToStreamingAssets()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;

        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        string hotfixAssembliesDstDir = Application.dataPath + "/../Dll";

        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllPath = $"{hotfixDllSrcDir}/{dll}";
            string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
            Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
        }
    }

    [MenuItem("Tools/Android/大厅资源(All)", false, 5)]
    private static void MainHallAssetBundleExport_Android()
    {
        StartPack(GameEnum.All, BuildTarget.Android);
    }



    [MenuItem("Tools/IOS/大厅资源", false, 5)]
    private static void MainHallAssetBundleExport_IOS()
    {
        StartPack(GameEnum.All, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/3D捕鱼资源 (Fish3D)", false, 5)]
    private static void Fish3DAssetBundleExport_Android()
    {
        StartPack(GameEnum.Fish_3D, BuildTarget.Android);
    }

    [MenuItem("Tools/IOS/3D捕鱼资源", false, 5)]
    private static void Fish3DAssetBundleExport_IOS()
    {
        StartPack(GameEnum.Fish_3D, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/李逵劈鱼资源(LK)",false,5)]
    private static void LKAssetBundleExport_Android() {
        StartPack(GameEnum.Fish_LK, BuildTarget.Android);
    }
    [MenuItem("Tools/IOS/李逵劈鱼资源(LK)", false, 5)]
    private static void LKAssetBundleExport_IOS() {
        StartPack(GameEnum.Fish_LK, BuildTarget.iOS);
    }
    [MenuItem("Tools/Android/飞禽走兽资源(FQZS)", false, 5)]
    private static void FQAssetBundleExport_Android() {
        StartPack(GameEnum.FQZS, BuildTarget.Android);
    }
    [MenuItem("Tools/IOS/飞禽走兽资源", false, 5)]
    private static void FQAssetBundleExport_IOS() {
        StartPack(GameEnum.FQZS, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/神话资源(SH)",false,5)]
    private static void SHAssetBundleExport_Android()
    {
        StartPack(GameEnum.SH, BuildTarget.Android);
    }

    [MenuItem("Tools/IOS/神话资源", false, 5)]
    private static void SHAssetBundleExport_IOS()
    {
        StartPack(GameEnum.SH, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/五子棋资源(WZQ)", false, 5)]
    private static void WZQAssetBundleExport_Android()
    {
        StartPack(GameEnum.WZQ, BuildTarget.Android);
    }

    [MenuItem("Tools/IOS/五子棋资源", false, 5)]
    private static void WZQAssetBundleExport_IOS()
    {
        StartPack(GameEnum.WZQ, BuildTarget.iOS);
    }

    private static void StartPack(GameEnum type, BuildTarget target) {
        AssetBundleExport.StartPack(target, GameManager.GetResPath(type), GameManager.GetAbPath(type));
    }
}
