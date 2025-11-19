using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using I2.Loc.SimpleJSON;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using System.Text;
using System.Security.Cryptography;

public class Packager
{
    private enum BuildType
    {
        AssetBundleBuild,
        AssetImporter,
    }
    static BuildType buildType = BuildType.AssetImporter;

    private enum MType
    {
        UI,
        Lua,
        World,
        All,
    }

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();
    static Dictionary<string, List<string>> depends = new Dictionary<string, List<string>>();
    static List<string> bundleFiles = new List<string>();
    static List<string> searched = new List<string>();

    static bool keepManifest = false;
    static bool writeRefFile = false;
    static bool justSetNames = false;  //查看依赖使用
    static List<string> folderBundles = new List<string>();
    static MType curBundleType = MType.Lua;
    static Dictionary<string, List<string>> subPackList = new Dictionary<string, List<string>>();
    static List<string> packNameList = new List<string>();
    private static bool m_isSub;
    private static bool m_isCheckSub = false;
    private static bool m_isOut = false;
    static Dictionary<string, string> bundName = new Dictionary<string, string>();
    static Dictionary<string, string> NameArr = new Dictionary<string, string>();

    [MenuItem("BuildAppEditeTools/setSplashScreen", false, 700)]
    static public void SetSplashScreen()
    {

        /* UnityEditor.PlayerSettings.SplashScreen.show = false;
         // 取消Unity LOGO
         UnityEditor.PlayerSettings.SplashScreen.showUnityLogo = false;*/
    }


    [MenuItem("BuildAppEditeTools/Set Bundle Name/UI", false, 701)]
    static public void SetUIBundleNames()
    {
        SetBundleNames(MType.UI);
    }

    [MenuItem("BuildAppEditeTools/Set Bundle Name/Lua", false, 702)]
    static public void SetLuaBundleNames()
    {
        SetBundleNames(MType.Lua);
    }

    [MenuItem("BuildAppEditeTools/Set Bundle Name/World", false, 703)]
    static public void SetWorldBundleNames()
    {
        SetBundleNames(MType.World);
    }

    [MenuItem("BuildAppEditeTools/Set Bundle Name/All", false, 704)]
    static public void SetAllBundleNames()
    {
        SetBundleNames(MType.All);
    }


    private static bool getHaveSubPack(string value)
    {
        bool isShow = false;
        foreach (var item in subPackList)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                var str = item.Value[i];
                if (value.Contains(str))
                {
                    isShow = true;
                }
            }
        }
        return isShow;
    }

    private static bool getHaveSubPack(string key, string value)
    {
        bool isShow = false;
        foreach (var item in subPackList)
        {
            if (item.Key.Contains(key))
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    var str = item.Value[i];
                    if (value.Contains(str))
                    {
                        isShow = true;
                    }
                }
            }
        }
        return isShow;
    }
    private static void getSubPack()
    {
        if (!m_isCheckSub)
        {
            subPackList.Clear();
            packNameList.Clear();
            return;
        }

        string str = "";
        StreamReader sr = new StreamReader(Application.dataPath + "/Editor/Build/Config/subPack.json");
        if (sr.Peek() != -1)
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        JSONNode node = JSON.Parse(str);
        subPackList.Clear();
        packNameList.Clear();
        for (int i = 0; i < node["subPack"].Count; i++)
        {
            var name = node["subPack"][i]["name"];
            var arr = node["subPack"][i]["paths"];
            var list = new List<string>();
            for (int j = 0; j < arr.Count; j++)
            {
                list.Add(arr[j]);
            }
            packNameList.Add(name);
            subPackList.Add(name, list);
        }
    }

    private static void SetBundleNames(MType mType)
    {
        ClearBundleName();
        BuildType defaultType = buildType;
        justSetNames = true;
        buildType = BuildType.AssetImporter;

        if (mType == MType.UI)
        {
            HandleUIBundle();
        }
        else if (mType == MType.Lua)
        {
            //  HandleLuaBundle();
        }
        else if (mType == MType.World)
        {
            HandleWorldBundle();
        }
        else if (mType == MType.All)
        {
            HandleAllAndSubBundle();
        }

        buildType = defaultType;
        justSetNames = false;

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("Set Assets Bundle Name Over----");
    }

    [MenuItem("BuildAppEditeTools/Clear Assets Bundle Name", false, 704)]
    static public void ClearBundleNames()
    {
        ClearBundleName();
        AssetDatabase.Refresh();
        Debug.Log("Clear Asset Bundle Name Over----");
    }


    [MenuItem("BuildAppEditeTools/Publish/Export All Assets And SubPack", false, 15)]
    static public void HandleAllAndSubBundle()
    {
        Debug.Log("導出全部bundle資源！！！！！！！！！！！！");
        m_isSub = false;
        m_isCheckSub = true;
        ClearAllAssets();
        // HandFirstBundle();
        // Debug.Log("First End");

        HandleUIBundle();
        Debug.Log("UI End");
        HandleDllBoundle();
        Debug.Log("Dll End");
        HandleWorldBundle();
        Debug.Log("World End");
        CopyConfig();
        CreateUrlVersion();

        Debug.Log("Config End");
        WriteFiles();
        Debug.Log("WriteFiles End");


        m_isSub = true;
        HandleUIBundle();
        Debug.Log("Sub End");
        WriteSubFiles();
        Debug.Log("SubFile End");
        m_isCheckSub = false;

 
        CopySteamingAssetToAsset();
        //ObscureAsset();
        //  CompressAssets();
        /*       ZipAsset();
               CopyZipToAsset();*/
        var dirArr = Directory.GetDirectories(Application.streamingAssetsPath + "/" + AppConst.SubPackName);
        for (int i = 0; i < dirArr.Length; i++)
        {
            var path = Path.GetFileName(dirArr[i]);
            if (!AppConst.SubPackArr.Contains(path))
            {
                Directory.Delete(dirArr[i], true);
            }
            if (File.Exists(dirArr[i] + ".meta"))
                File.Delete(dirArr[i] + ".meta");
        }
        ClearBundleNames();
    }

    [MenuItem("BuildAppEditeTools/Publish/导出/主资源包", false, 15)]
    static public void HandleMainBundle()
    {
        Debug.Log("導出全部bundle資源！！！！！！！！！！！！");
        m_isSub = false;
        m_isCheckSub = true;
        ClearAllAssets();
        // HandFirstBundle();
        // Debug.Log("First End");

        HandleUIBundle();
        Debug.Log("UI End");
        //拷贝dll
        var configPath = Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/Dll";
        if (!Directory.Exists(configPath))
        {
            Debug.LogError("找不到路径:" + configPath);
            return;
        }
        CopyDirectory(configPath, Application.streamingAssetsPath);
        Debug.Log("Dll End");
        HandleWorldBundle();
        Debug.Log("World End");
        //拷贝dll
        var configPath1 = Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/LubanConfig";
        if (!Directory.Exists(configPath1))
        {
            Debug.LogError("找不到路径:" + configPath1);
            return;
        }
        CopyDirectory(configPath1, Application.streamingAssetsPath);
        // CopyConfig();


        Debug.Log("Config End");
        WriteFiles();
        Debug.Log("WriteFiles End");


        /*     m_isSub = true;
             HandleUIBundle();
             Debug.Log("Sub End");
             WriteSubFiles();
             Debug.Log("SubFile End");
             m_isCheckSub = false;


             CopySteamingAssetToAsset();
             ObscureAsset();
             //  CompressAssets();
             ClearBundleNames();*/
    }

    [MenuItem("BuildAppEditeTools/Publish/导出/代码包", false, 15)]
    static public void HandleMainCodeBundle()
    {
        HandleDllBoundle();
        WriteFiles();
        CopySteamingAssetToAsset();
        //ObscureAsset();
        //  CompressAssets();
        /*       ZipAsset();
               CopyZipToAsset();*/
        var dirArr = Directory.GetDirectories(Application.streamingAssetsPath + "/" + AppConst.SubPackName);
        for (int i = 0; i < dirArr.Length; i++)
        {
            var path = Path.GetFileName(dirArr[i]);
            if (!AppConst.SubPackArr.Contains(path))
            {
                Directory.Delete(dirArr[i], true);
            }
            if (File.Exists(dirArr[i] + ".meta"))
                File.Delete(dirArr[i] + ".meta");
        }
        ClearBundleNames();
        Debug.Log("copy 代码包 End");
    }

    [MenuItem("BuildAppEditeTools/Publish/导出/配置", false, 15)]

    static public void CopySteamingAssetToAsset()
    {
        var configPath = Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion;
        if (!Directory.Exists(configPath))
        {
            Directory.CreateDirectory(configPath);
        }
        else
        {
            Directory.Delete(configPath, true);
        }
        CopyDirectory(Application.streamingAssetsPath, configPath);
        RecursiveMeta(configPath);
    }

    static public void CreateUrlVersion()
    {
        string[] urlArr = new string[5] { "www.imFXB16V.com", "www.sFTpHQHQ.com", "www.oFPiV7gH.com", "www.beg1ydnw.com", "www.udTKVF6I.com" };
        FileStream fs = new FileStream(Application.streamingAssetsPath + "/urlVersion.ver", FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < urlArr.Length; i++)
        {
           var str = GameConst.EncryptDES(urlArr[i]);
            sw.WriteLine(str);
        }
        sw.Close();
        fs.Close();

    }

    [MenuItem("BuildAppEditeTools/Publish/CopyAssetToSteamingAsset", false, 15)]
    static public void CopyAssetToSteamingAsset()
    {
        var configPath = Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion;
        var dirs = Directory.GetDirectories(Application.streamingAssetsPath);
        var files = Directory.GetFiles(Application.streamingAssetsPath);
        for (int i = 0; i < dirs.Length; i++)
        {
            Directory.Delete(dirs[i], true);
        }
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        if (Directory.Exists(configPath))
        {
            CopyDirectory(configPath, Application.streamingAssetsPath);
        }
        Debug.Log("Copy End");
    }

    [MenuItem("BuildAppEditeTools/Publish/ObscureAsset", false, 15)]
    static public void ObscureAsset()
    {
        var files = Directory.GetFiles(Application.streamingAssetsPath);
        var dirs = Directory.GetDirectories(Application.streamingAssetsPath);
        for (int i = 0; i < dirs.Length; i++)
        {
            var dirName = Path.GetFileName(dirs[i]);
            if (dirName != "packOut")
            {
                Directory.Delete(dirs[i], true);
            }
        }
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        CopyDirectory(Application.streamingAssetsPath + "/packOut", Application.streamingAssetsPath);
        Directory.Delete(Application.streamingAssetsPath + "/packOut", true);

        if (AppConst.SubPackArr == "")
        {
            if (Directory.Exists(Application.streamingAssetsPath + "/" + AppConst.SubPackName))
                Directory.Delete(Application.streamingAssetsPath + "/" + AppConst.SubPackName, true);
        }
        else
        {
            if (Directory.Exists(Application.streamingAssetsPath + "/" + AppConst.SubPackName))
            {
                var deleteDirs = Directory.GetDirectories(Application.streamingAssetsPath + "/" + AppConst.SubPackName);


                var packArr = AppConst.SubPackArr.Split('|');
                var str = "";
                for (int i = 0; i < packArr.Length; i++)
                {
                    var name = packArr[i];
                    var name1 = CommonTools.EncryptDES(name);
                    var str1 = name1.Split('/');
                    str += str1[0];
                    if (i < packArr.Length - 1)
                    {
                        str += "|";
                    }
                }
                for (int i = 0; i < deleteDirs.Length; i++)
                {
                    var name = Path.GetFileName(deleteDirs[i]);
                    if (!str.Contains(name))
                    {
                        Directory.Delete(deleteDirs[i], true);
                        if (File.Exists(deleteDirs[i] + ".meta"))
                        {
                            File.Delete(deleteDirs[i] + ".meta");
                        }
                        Debug.Log("Delete " + deleteDirs[i]);
                    }
                }

            }
        }
        Debug.Log("Obscure End");
    }

    [MenuItem("BuildAppEditeTools/Publish/Clear All Assets And SubPack", false, 15)]
    static public void ClearAllAssets()
    {
        var dirs = Directory.GetDirectories(Application.streamingAssetsPath);
        var files = Directory.GetFiles(Application.streamingAssetsPath);


        for (int i = 0; i < dirs.Length; i++)
        {
            Directory.Delete(dirs[i], true);
        }

        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }

    }
    [MenuItem("BuildAppEditeTools/Publish/Copy Config", false, 15)]
    static public void CopyConfig()
    {
        var configPath = Application.dataPath + "/../CommitResources/Game/Windows64";
        if (Directory.Exists(configPath))
            CopyDirectory(configPath, Application.streamingAssetsPath);

    }

    static public void CopyZipToAsset()
    {

        File.Copy(Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/" + GameConst.zipName, Application.streamingAssetsPath + "/" + GameConst.zipName, true);
    }
    static public void ZipAsset()
    {
        var dirArr = Directory.GetDirectories(Application.streamingAssetsPath + "/" + AppConst.SubPackName);
        for (int i = 0; i < dirArr.Length; i++)
        {
            var path = Path.GetFileName(dirArr[i]);
            if (!path.Contains(AppConst.SubPackArr))
            {
                Directory.Delete(dirArr[i], true);
            }
            if (File.Exists(dirArr[i] + ".meta"))
                File.Delete(dirArr[i] + ".meta");
        }

        string[] paths = new string[7] { Application.streamingAssetsPath + "/files.txt",
            Application.streamingAssetsPath + "/DLL",
            Application.streamingAssetsPath + "/ConfigVersion.xml",
            Application.streamingAssetsPath + "/LubanConfig",
            Application.streamingAssetsPath + "/" + AppConst.UIBundName,
            Application.streamingAssetsPath + "/"+ AppConst.SubPackName,
            Application.streamingAssetsPath + "/" + AppConst.WorldBundName};
        if (File.Exists(Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/" + GameConst.zipName))
        {
            File.Delete(Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/" + GameConst.zipName);
        }
        ZipHelper.Zip(paths, Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/" + GameConst.zipName, GameConst.ZipKey);

        var dirArr1 = Directory.GetDirectories(Application.streamingAssetsPath);
        var dirArr2 = Directory.GetFiles(Application.streamingAssetsPath);
        for (int i = 0; i < dirArr1.Length; i++)
        {
            Directory.Delete(dirArr1[i], true);
        }

        for (int i = 0; i < dirArr2.Length; i++)
        {

            File.Delete(dirArr2[i]);
        }


    }
    public static void CopyDirectory(string sourceDirectory, string destDirectory)
    {
        //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
        if (!Directory.Exists(sourceDirectory))
        {
            Directory.CreateDirectory(sourceDirectory);
        }
        if (!Directory.Exists(destDirectory))
        {
            Directory.CreateDirectory(destDirectory);
        }
        //拷贝文件
        CopyFile(sourceDirectory, destDirectory);
        //拷贝子目录       
        //获取所有子目录名称
        string[] directionName = Directory.GetDirectories(sourceDirectory);
        foreach (string directionPath in directionName)
        {
            //根据每个子目录名称生成对应的目标子目录名称
            string directionPathTemp = Path.Combine(destDirectory, directionPath.Substring(sourceDirectory.Length + 1));// destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);
                                                                                                                        //递归下去
            CopyDirectory(directionPath, directionPathTemp);
        }
    }

    public static void CopyFile(string sourceDirectory, string destDirectory)
    {
        //获取所有文件名称
        string[] fileName = Directory.GetFiles(sourceDirectory);
        foreach (string filePath in fileName)
        {
            //根据每个文件名称生成对应的目标文件名称
            string filePathTemp = Path.Combine(destDirectory, filePath.Substring(sourceDirectory.Length + 1));// destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
                                                                                                              //若不存在，直接复制文件；若存在，覆盖复制
            if (File.Exists(filePathTemp))
            {
                File.Copy(filePath, filePathTemp, true);
            }
            else
            {
                File.Copy(filePath, filePathTemp);
            }
        }
    }


    public static string selec_subName = "";
    #region 资源打包
    [MenuItem("BuildAppEditeTools/Publish/Export UI Assets", false, 16)]
    static public void HandleUIBundle()
    {
        getSubPack();
        var count = 1;
        if (m_isSub)
        {
            count = subPackList.Count;
        }
        curBundleType = MType.UI;
        // 清空文件夹
        string strDir = "/StreamingAssets/" + AppConst.UIBundName;
        string bundlePath = "";
        string strDirPath = "";
        int n = 0;
        for (int i = 0; i < count; i++)
        {
            if (m_isSub)
            {
                strDir = "/StreamingAssets/" + AppConst.SubPackName + "/" + packNameList[i] + "/" + AppConst.UIBundName;
                selec_subName = packNameList[i];
            }
            bundlePath = "Assets" + strDir;
            strDirPath = Application.dataPath + strDir;
            if (m_isOut)
            {
                strDirPath.Replace("Assets/StreamingAssets", "AssetBundle/Out");
            }

            if (!justSetNames)
            {
                DelAndNewDir(strDirPath);
                ClearBundleName();
            }

            BuildInit();
            SetUIfolderBundles();

            string searchDir = AppConst.ResDataDir + "UI/";
            string dataPath = Application.dataPath + "/";

            Recursive(dataPath + searchDir);

            string ext = string.Empty;
            string bundleName = string.Empty;
            string bundleFile = string.Empty;
            string dir = string.Empty;


            foreach (string f in files)
            {
                if (m_isSub)
                {
                    if (!getHaveSubPack(packNameList[i], f))
                    {
                        UpdateProgress(n++, files.Count * count, bundleFile);
                        continue;
                    }
                }
                else
                {
                    if (getHaveSubPack(f))
                    {
                        UpdateProgress(n++, files.Count, bundleFile);
                        continue;
                    }
                }

                ext = Path.GetExtension(f);
                dir = Path.GetDirectoryName(f).Replace('\\', '/');

                bundleFile = "Assets/" + f.Replace(dataPath, "");
                if (bundleFile.Contains("UI/Fonts/") && !m_isSub)
                {
                    continue;
                }
                bool check = false;
                if (bundleFile.Contains("UI/Prefabs/"))
                {
                    bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
                }
                else if (bundleFile.Contains("UI/Dragon/"))
                {
                    bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
                }
                else if (bundleFile.Contains("UI/Animation/"))
                {
                    bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
                }
                else if (bundleFile.Contains("UI/Particle/"))
                {
                    bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
                }
                else if (bundleFile.Contains("UI/Spine/"))
                {
                    bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
                }
                else
                {
                    check = true;
                    bundleName = "ui/" + (f.Replace(dataPath, "").Replace(searchDir, "").Replace(ext, "") + AppConst.ExtName).ToLower();
                }

                if (bundleFile.Contains("UI/UITemplate/"))
                {
                    check = false;
                }
                AddDepends(bundleFile);
                SetFileBundleName(bundleFile, bundleName, check);
                UpdateProgress(n++, files.Count, bundleFile);
            }
            UpdateProgress(files.Count * count, files.Count * count, "Export UI bundles over");
            BuildDependsBundle();
            WriteRefInfoFile(strDirPath);

            StartBuild(bundlePath);

            BuildEnd(strDirPath);

        }

    }


    [MenuItem("BuildAppEditeTools/Publish/Export World Assets", false, 18)]
    static public void HandleWorldBundle()
    {
        curBundleType = MType.World;
        // 清空文件夹
        string strDir = "/StreamingAssets/" + AppConst.WorldBundName;
        if (m_isSub)
        {
            strDir = "/StreamingAssets/" + AppConst.SubPackName + "/" + AppConst.WorldBundName;
        }
        string bundlePath = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;
        if (m_isOut)
        {
            strDirPath.Replace("Assets/StreamingAssets", "AssetBundle/Out");
        }
        if (!justSetNames)
        {
            DelAndNewDir(strDirPath);
            ClearBundleName();
        }

        BuildInit();
        SetWorldfolderBundles();

        string searchDir = AppConst.ResDataDir;
        string dataPath = Application.dataPath + "/";

        Recursive(dataPath + searchDir);

        string ext = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {

            if (m_isSub)
            {
                if (!getHaveSubPack(f))
                {
                    UpdateProgress(n++, files.Count, bundleFile);
                    continue;
                }
            }
            else
            {
                if (getHaveSubPack(f))
                {
                    UpdateProgress(n++, files.Count, bundleFile);
                    continue;
                }
            }


            ext = Path.GetExtension(f);
            bundleFile = "Assets/" + f.Replace(dataPath, "");
            bundleName = (f.Replace(dataPath, "").Replace(searchDir, "").Replace(ext, "") + AppConst.ExtName).ToLower();

            //过滤 lua 和 UI Data
            string head = bundleName.Split('/')[0];
            if (head.Equals("lua") || head.Equals("ui"))
            {
                continue;
            }
            var dir = Path.GetDirectoryName(f).Replace('\\', '/');
            if (head.Contains("data"))
            {
                bundleName = (dir.Replace(dataPath, "").Replace(AppConst.ResDataDir, "") + AppConst.ExtName).ToLower();
            }

            AddDepends(bundleFile);
            SetFileBundleName(bundleFile, bundleName);
        }


        UpdateProgress(files.Count, files.Count, "Export 3d Assets bundles over");

        HandleSceneBundle();

        BuildDependsBundle();
        WriteRefInfoFile(strDirPath);

        StartBuild(bundlePath);

        BuildEnd(strDirPath);
    }
    [MenuItem("BuildAppEditeTools/Publish/Export Sub Assets", false, 19)]
    static public void HandleSubBoundle()
    {
        m_isSub = true;
        m_isOut = true;
        HandleUIBundle();
        m_isOut = false;
        // HandleWorldBundle();
    }
    /// <summary>
    /// 处理scene
    /// </summary>
    static void HandleSceneBundle()
    {
        string dataPath = Application.dataPath + "/";
        string scenePath = dataPath + "Scene/";

        paths.Clear();
        files.Clear();
        Recursive(scenePath);

        string ext = string.Empty;
        string sceneName = string.Empty;
        string bundleName = string.Empty;
        string bundleFile = string.Empty;

        int n = 0;
        foreach (string f in files)
        {
            if (f.EndsWith(".unity"))
            {
                ext = Path.GetExtension(f);
                sceneName = f.Substring(f.LastIndexOf("/") + 1);

                bundleFile = "Assets/" + f.Replace(dataPath, "");
                bundleName = "scenes/" + (sceneName.Replace(ext, "") + AppConst.ExtName).ToLower();

                AddDepends(bundleFile);
                SetFileBundleName(bundleFile, bundleName);

                UpdateProgress(n++, files.Count, bundleFile);
            }
        }

        UpdateProgress(files.Count, files.Count, "Scenes bundle over");
    }

    [MenuItem("BuildAppEditeTools/Publish/Export DLL", false, 20)]
    static public void HandleDllBoundle()
    {
        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;

        CompileDllCommand.CompileDll(target);
        DelAndNewDir(Application.streamingAssetsPath + "/Dll");
        CopyAOTAssembliesToStreamingAssets();
        CopyHotUpdateAssembliesToStreamingAssets();

    }

    public static byte[] EncryptFile(byte[] targetFile)
    {
        byte[] encryptedFile = new byte[targetFile.Length];
        byte keyValue = byte.Parse(GameConst.EncryptKey);

        for (int i = 0; i < targetFile.Length; i++)
        {
            encryptedFile[i] = (byte)(targetFile[i] ^ keyValue);
        }

        return encryptedFile;
    }

    public static void CopyAOTAssembliesToStreamingAssets()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = Application.streamingAssetsPath + "/Dll";

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
            var bytes = File.ReadAllBytes(dllBytesPath);
            // bytes = EncryptFile(bytes);
            bytes = BinaryEncryptHelper.EncryptFile(bytes);
            File.WriteAllBytes(dllBytesPath, bytes);
            Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }
    }

    public static void CopyHotUpdateAssembliesToStreamingAssets()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;

        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        string hotfixAssembliesDstDir = Application.streamingAssetsPath + "/Dll";

        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllPath = $"{hotfixDllSrcDir}/{dll}";
            string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
            var bytes = File.ReadAllBytes(dllBytesPath);
            // bytes = EncryptFile(bytes);
            bytes = BinaryEncryptHelper.EncryptFile(bytes);
            File.WriteAllBytes(dllBytesPath, bytes);
            Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
        }
    }
    #endregion
    static void DelAndNewDir(string dir)
    {
        DirectoryInfo dirOutDir = new DirectoryInfo(dir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(dir);
        }
        //else
        //{
        //    Directory.Delete(dir, true);
        //    Directory.CreateDirectory(dir);
        //}
    }

    static void BuildInit()
    {
        maps.Clear();
        depends.Clear();
        bundleFiles.Clear();
        searched.Clear();

        paths.Clear();
        files.Clear();

        folderBundles.Clear();
    }

    static void StartBuild(string outputPath)
    {
        if (justSetNames)
        {
            return;
        }

        BuildAssetBundleOptions opt = BuildAssetBundleOptions.ChunkBasedCompression;
        if (buildType == BuildType.AssetBundleBuild)
        {
            BuildPipeline.BuildAssetBundles(outputPath, maps.ToArray(), opt, EditorUserBuildSettings.activeBuildTarget);
        }
        else
        {
            BuildPipeline.BuildAssetBundles(outputPath, opt, EditorUserBuildSettings.activeBuildTarget);
        }
    }

    static void BuildEnd(string path)
    {
        if (justSetNames) return;

        ClearBundleName();
        ClearManifestFile(path);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void AddDepends(string bundleFile)
    {
        if (searched.Contains(bundleFile)) return;
        searched.Add(bundleFile);

        string[] ds = AssetDatabase.GetDependencies(bundleFile);
        string dependFile = string.Empty;
        for (int i = 0; i < ds.Length; i++)
        {
            dependFile = ds[i];
            if (dependFile.Equals(bundleFile) || dependFile.EndsWith(".cs") || dependFile.EndsWith(".asset") || dependFile.EndsWith(".exr") || dependFile.EndsWith(".js"))
            {
                continue;
            }

            if (!depends.ContainsKey(dependFile))
            {
                depends.Add(dependFile, new List<string>());
            }

            if (!depends[dependFile].Contains(bundleFile))
            {
                depends[dependFile].Add(bundleFile);
            }
            else
            {
                Debug.LogError(dependFile + " 多次添加被依赖----- " + bundleFile);
            }

            AddDepends(dependFile);
        }
    }

    static void BuildDependsBundle()
    {
        string baseFolder = "";
        if (curBundleType == MType.UI)
        {
            baseFolder = "ui/";
        }

        string bundleFile = string.Empty;
        string fileName = string.Empty;
        string bundleName = string.Empty;
        string ext = string.Empty;

        int n = 0;
        foreach (var item in depends)
        {
            UpdateProgress(n++, depends.Count, item.Key);

            if (item.Value.Count > 1 || item.Key.StartsWith("Assets/Atlas/"))
            {
                bundleFile = item.Key;

                ext = bundleFile.Substring(bundleFile.LastIndexOf(".") + 1);
                fileName = bundleFile.Substring(bundleFile.LastIndexOf("/") + 1);
                if (ext.IsTexture())
                {
                    bundleName = baseFolder + "depends/" + (bundleFile.Replace("Assets/", "").Replace(fileName, "") + "textures" + AppConst.ExtName).ToLower();
                }
                else
                {
                    bundleName = baseFolder + "depends/" + (bundleFile.Replace("Assets/", "").Replace(fileName, "") + ext.ToLower() + "s" + AppConst.ExtName).ToLower();
                }

                bool checkFolderBundle = true;
                if (ext.IsTexture())
                {
                    if (bundleFile.StartsWith("Assets/Atlas"))
                    {
                        bundleName = baseFolder + "depends/" + (bundleFile.Replace("Assets/", "").Replace(bundleFile.Substring(bundleFile.LastIndexOf(".")), "") + AppConst.ExtName).ToLower(); //单独打包
                    }

                    string shader = "Assets/" + AppConst.ResDataDir + "Shaders";
                    string file = string.Empty;
                    bool refOut = false;
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        file = item.Value[i];

                        if (file.StartsWith(shader) && file.EndsWith(".shader"))
                        {
                            refOut = true;
                            bundleName = baseFolder + "shaders" + AppConst.ExtName;
                            break;
                        }

                        if (!IsOtherMatRef(bundleFile, file))
                        {
                            refOut = true;
                        }
                    }

                    if (refOut)
                    {
                        checkFolderBundle = false;
                    }
                    else
                    {
                        checkFolderBundle = true;
                    }
                }

                SetFileBundleName(bundleFile, bundleName, checkFolderBundle);
            }
        }

        UpdateProgress(depends.Count, depends.Count, "depend assets build over");
    }

    static bool IsOtherMatRef(string str1, string str2)
    {
        if (str2.EndsWith(".mat"))
        {
            string path1 = str1.Substring(0, str1.LastIndexOf('/'));
            string path2 = str2.Substring(0, str2.LastIndexOf('/'));
            path2 = path2.Replace("/Materials", "");
            if (!path1.Equals(path2))
            {
                return false;
            }
        }

        return true;
    }

    static void ClearManifestFile(string path)
    {
        if (justSetNames) return;

        paths.Clear();
        files.Clear();
        Recursive(path);

        /*      for (int i = 0; i < files.Count; i++)
              {
                  string file = files[i];
                  if (File.Exists(file + ".manifest"))
                  {
                      byte[] oldData = File.ReadAllBytes(file);
                      int newOldLen = (int)AppConst.ByteNum + oldData.Length;//定死了,128个空byte
                      var newData = new byte[newOldLen];
                      for (int tb = 0; tb < oldData.Length; tb++)
                      {
                          newData[(int)AppConst.ByteNum + tb] = oldData[tb];
                      }
                      FileStream fs = File.OpenWrite(file);//打开写入进去
                      fs.Write(newData, 0, newOldLen);
                      fs.Close();
                  }
              }
      */
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            if (!keepManifest && file.EndsWith(".manifest"))
            {
                File.Delete(file);
                continue;
            }
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }


    // 清除AssetBundle设置
    public static void ClearBundleName()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (string name in names)
        {
            AssetDatabase.RemoveAssetBundleName(name, true);
        }
    }


    private static void SetUIfolderBundles()
    {
        string dataPath = Application.dataPath + "/";
        string[] dirs = Directory.GetDirectories(dataPath + "ResData/UI");
        foreach (string dir in dirs)
        {
            string bundleName = dir.Replace('\\', '/').Replace(dataPath, "").Replace("ResData/", "");
            if (!bundleName.Equals("UI/Sound") && !bundleName.Equals("UI/Fonts") && !bundleName.Equals("UI/Atlas") && !bundleName.Equals("UI/Prefabs") && !bundleName.Equals("UI/Texture") && !bundleName.Equals("UI/GameTexture"))
            {
                folderBundles.Add(bundleName.ToLower());
            }
            //else if (bundleName.Equals("UI/Prefabs"))
            //{
            //    string[] dirs1 = Directory.GetDirectories(dataPath + "ResData/UI/Prefabs", "*", SearchOption.AllDirectories);
            //    foreach (string dir1 in dirs1)
            //    {
            //        string bundleName1 = dir1.Replace('\\', '/').Replace(dataPath, "").Replace("ResData/", "");
            //        folderBundles.Add(bundleName1.ToLower());
            //    }
            //}
        }

        folderBundles.Add("ui/depends/plugin/igsoft_resources");
        folderBundles.Add("ui/depends/resdata/shaders");
    }

    private static void SetWorldfolderBundles()
    {

        folderBundles.Add("shaders");
        folderBundles.Add("mat");
        folderBundles.Add("depends/plugin/icefire");
        folderBundles.Add("depends/plugin/igsoft_resources");
        folderBundles.Add("depends/plugin/projector");
        folderBundles.Add("depends/plugin/t4m");
        folderBundles.Add("depends/plugin/t4mobj");

        string dataPath = Application.dataPath + "/";
        /*    AddFolderBundles(dataPath + "AssetsRaw/Animation");
            AddFolderBundles(dataPath + "AssetsRaw/Effect");
            AddFolderBundles(dataPath + "AssetsRaw/Scene");*/

        /*        string[] dirs = Directory.GetDirectories(dataPath + "ResData/Sound");
                foreach (string dir in dirs)
                {
                    string bundleName = dir.Replace('\\', '/').Replace(dataPath, "").Replace("ResData/", "");
                    if (!bundleName.Equals("Sound/scene"))
                    {
                        folderBundles.Add(bundleName.ToLower());
                    }
                }*/

        Debug.Log("SetWorldfolderBundles ---");
    }

    static bool AddFolderBundles(string path)
    {
        path = path.Replace('\\', '/');
        bool addChild = false;
        string dataPath = Application.dataPath + "/";
        string[] dirs = Directory.GetDirectories(path);
        foreach (string dir in dirs)
        {
            if (!dir.EndsWith("Materials") && !dir.EndsWith("ClipCurve") && !dir.EndsWith("path"))
            {
                string bundleName = "depends/" + dir.Replace('\\', '/').Replace(dataPath, "").ToLower();
                if (!AddFolderBundles(dir))
                {
                    bool hasChild = false;
                    for (int i = 0; i < folderBundles.Count; i++)
                    {
                        if (folderBundles[i].StartsWith(bundleName))
                        {
                            hasChild = true;
                        }
                    }

                    if (!hasChild)
                    {
                        folderBundles.Add(bundleName);
                        addChild = true;
                    }
                }
            }
        }

        return addChild;
    }

    private static void SetFileBundleName(string file, string abName, bool checkFolderBundle = true)
    {
        if (bundleFiles.Contains(file)) return;
        bundleFiles.Add(file);

        if (checkFolderBundle)
        {
            for (int i = 0; i < folderBundles.Count; i++)
            {
                if (abName.StartsWith(folderBundles[i]) && abName.Replace(folderBundles[i], "").StartsWith("/"))
                {
                    abName = folderBundles[i] + AppConst.ExtName;
                    break;
                }
            }
        }

        if (buildType == BuildType.AssetBundleBuild)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            if (m_isSub)
            {
                if (!abName.Contains("ui/prefabs/"))
                    abName = selec_subName + "/" + abName;
            }
            build.assetBundleName = abName;
            build.assetNames = new string[] { file };
            maps.Add(build);
        }
        else
        {
            AssetImporter importer = AssetImporter.GetAtPath(file);
            if (importer == null)
            {
                Debug.LogError("[路径错误] path: " + file);
                return;
            }
            if (m_isSub)
            {
                if (!abName.Contains("ui/prefabs/"))
                    abName = selec_subName + "/" + abName;

            }
            importer.assetBundleName = abName;
        }
    }


    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        string ext = string.Empty;
        foreach (string filename in names)
        {
            ext = Path.GetExtension(filename);
            if (ext.Equals(".meta") || ext.Equals(".cs")) continue;
            files.Add(filename.Replace('\\', '/'));
        }

        foreach (string dir in dirs)
        {
            if (Path.GetFileName(dir) == AppConst.SubPackName)
            {
                if (m_isCheckSub)
                {
                    paths.Add(dir.Replace('\\', '/'));
                    Recursive(dir);
                }
            }
            else if (Path.GetFileName(dir) == "packOut")
            {

            }
            else
            {
                paths.Add(dir.Replace('\\', '/'));
                Recursive(dir);
            }


        }
    }

    static void RecursiveMeta(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        string ext = string.Empty;
        foreach (string filename in names)
        {
            ext = Path.GetExtension(filename);
            if (ext.Equals(".meta"))
            {
                File.Delete(filename);
            }
        }
        foreach (string dir in dirs)
        {
            // paths.Add(dir.Replace('\\', '/'));
            RecursiveMeta(dir);
        }
    }

    //写引用文件
    static void WriteRefInfoFile(string folder)
    {
        if (!writeRefFile) return;

        ///----------------------创建文件列表-----------------------
        string newFilePath = folder + "/refInfo.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);

        foreach (var item in depends)
        {
            if (item.Value.Count > 1)
            {
                sw.WriteLine(item.Key);

                for (int i = 0; i < item.Value.Count; i++)
                {
                    sw.WriteLine(string.Format("    {0}: {1}", i, item.Value[i]));
                }

                sw.WriteLine("");
            }
        }

        sw.Close();
        fs.Close();
    }

    [MenuItem("BuildAppEditeTools/Test/Write Files", false, 905)]
    public static void WriteFiles()
    {
        BuildFileIndex();  //写files文件
        AssetDatabase.Refresh();
        Debug.Log("Write Files Over----");
    }

    [MenuItem("BuildAppEditeTools/Test/Write Sub Files", false, 905)]
    public static void WriteSubFiles()
    {
        m_isCheckSub = true;
        BuildFileIndex(true);  //写files文件
        AssetDatabase.Refresh();
        m_isCheckSub = false;
        Debug.Log("Write Files Over----");
    }

    static void BuildFileIndex(bool isSub = false)
    {
        bundName.Clear();
        NameArr.Clear();
        getSubPack();
        var count = 1;
        if (isSub)
        {
            count = subPackList.Count;
        }
        for (int j = 0; j < count; j++)
        {

            string resPath = Application.dataPath + "/StreamingAssets/";
            if (isSub)
            {
                resPath = Application.dataPath + "/StreamingAssets/" + AppConst.SubPackName + "/" + packNameList[j];
            }

            ///----------------------创建文件列表-----------------------
            string newFilePath = resPath + "/files.txt";
            if (File.Exists(newFilePath)) File.Delete(newFilePath);

            paths.Clear();
            files.Clear();
            Recursive(resPath);
            FileStream fs;
            string[] lines = null;
            if (isSub)
            {
                resPath += "/";
                lines = File.ReadAllLines(Application.dataPath + "/StreamingAssets/files.txt");
                fs = new FileStream(Application.dataPath + "/StreamingAssets/files.txt", FileMode.OpenOrCreate);
                

            }
            else
            {
                fs = new FileStream(newFilePath, FileMode.CreateNew);
       
            }

            StreamWriter sw = new StreamWriter(fs);
            if(lines != null)
            {
                foreach (var item in lines)
                {
                    sw.WriteLine(item);
                }
                sw.WriteLine(packNameList[j] + "====START");
            }
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.EndsWith(".manifest")) continue;

                string md5 = Util.md5file(file);
                FileInfo f = new FileInfo(file);
                string value = file.Replace(resPath, string.Empty);
                long fileSize = f.Length;
                sw.WriteLine(value + "|" + md5 + "|" + fileSize);
            }
            if (isSub)
            {
                sw.WriteLine(packNameList[j] + "====END");
            }
            sw.Close();
            fs.Close();
        }

    }



    [MenuItem("BuildAppEditeTools/Test/Apk Build", false, 906)]
    static void PublishAndroidAPK()
    {
        BuildFileIndex();

        string curDir = Directory.GetCurrentDirectory();
        curDir = curDir.Replace('\\', '/');
        curDir = curDir + "/AndroidApks/";
        DirectoryInfo dirOutDir = new DirectoryInfo(curDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(curDir);
        }

        string apkName = DateTime.Now.ToString("yyyymmddhhmmss") + "-rpg.apk";
        string path = curDir + apkName;

        var err = BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
        bool success = string.IsNullOrEmpty(err.ToString());

        if (success)
        {
            Debug.Log(path + " Publish Done! ");
        }
        else
        {
            Debug.Log(path + " Publish error! ");
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null) continue;
            if (e.enabled && (e.path.Contains("ThirdPartyUI") || e.path.Contains("Init")))
            {
                names.Add(e.path);
            }
        }

        return names.ToArray();
    }

    [MenuItem("删除缓存(Helper)/删除手机账号密码(Delete_phone)", false, 2)]
    public static void DeletePhoneAndPWD()
    {
        PlayerPrefs.DeleteKey("LOGIN_PHONE");
        PlayerPrefs.DeleteKey("LOGIN_PHONEPWD");
        //PersistentStorage.DeleteSetting("I2 Language");
    }

    [MenuItem("删除缓存(Helper)/删除所有缓存(DeleteAll)", false, 3)]
    public static void DeleteAllSetting()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("删除缓存(Helper)/删除文件缓存(Delete_File)", false, 3)]
    public static void DeleteFile()
    {
        if (Directory.Exists(Util.DataPath))
        {
            Directory.Delete(Util.DataPath, true);
        }
    }
    [MenuItem("BuildAppEditeTools/Generate Version JSON", false, 25)]
    public static void GenerateVersionJson() 
    {
        string versionJsonPath = Application.dataPath + "/../AssetsBundle/" + AppConst.ResVersion + "/version.json";
    
        JSONNode versionJson = new JSONClass();
    
        versionJson["resVer"].AsInt = AppConst.ResVersion;
        versionJson["resVersion"].AsInt = AppConst.ResVersion;
        versionJson["ip"] = "18.162.135.99";
        versionJson["port"] = "8200|8201|8202|8203|8204|8205";
        versionJson["backstage"] = AppConst.backstage;
        versionJson["customer"] = AppConst.customer;
        versionJson["CdnUrl"] = AppConst.CdnUrl;
        versionJson["isShow"].AsInt = 1;
        versionJson["isShowks"].AsInt = 0;
        File.WriteAllText(versionJsonPath, versionJson.ToString());
    
        UnityEngine.Debug.Log("Generated version.json at: " + versionJsonPath);
    }
}