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
    [MenuItem("Tools/Android/飞禽走兽资源", false, 5)]
    private static void FQAssetBundleExport_Android() {
        StartPack(GameEnum.FQZS, BuildTarget.Android);
    }
    [MenuItem("Tools/IOS/飞禽走兽资源", false, 5)]
    private static void FQAssetBundleExport_IOS() {
        StartPack(GameEnum.FQZS, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/神话资源",false,5)]
    private static void SHAssetBundleExport_Android()
    {
        StartPack(GameEnum.SH, BuildTarget.Android);
    }

    [MenuItem("Tools/IOS/神话资源", false, 5)]
    private static void SHAssetBundleExport_IOS()
    {
        StartPack(GameEnum.SH, BuildTarget.iOS);
    }

    [MenuItem("Tools/Android/五子棋资源", false, 5)]
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
    // Thêm vào EditMenu.cs class

    [MenuItem("Tools/PC/Full PC Build Pipeline", false, 100)]
    private static void FullPCBuildPipeline()
    {
        Debug.Log("=== Starting Full PC Build Pipeline ===");
        
        try 
        {
            // Step 1: Setup PC Build Environment
            SetupPCBuildEnvironment();
            
            // Step 2: Handle DLL Bundle for PC
            HandleDllBoundlePC();
            
            // Step 3: Build PC Assets
            BuildPCAssets();
            
            // Step 4: Copy PC Resources
            CopyPCResources();
            
            // Step 5: Build PC Executable
            BuildPCExecutable();
            
            Debug.Log("=== PC Build Pipeline Completed Successfully ===");
            EditorUtility.DisplayDialog("Build Complete", "PC Build completed successfully!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PC Build Pipeline failed: {e.Message}");
            EditorUtility.DisplayDialog("Build Failed", $"PC Build failed: {e.Message}", "OK");
        }
    }

    [MenuItem("Tools/PC/1. Setup PC Environment", false, 101)]
    private static void SetupPCBuildEnvironment()
    {
        Debug.Log("Setting up PC build environment...");
        
        // Switch to PC platform
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        
        // Set PC-specific scripting defines
        string pcDefines = "UNUSE_ASSETBOUNDLE_INEDITOR;PC_BUILD;STANDALONE_BUILD";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, pcDefines);
        
        // PC-specific settings
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.tamron.fishing3D.pc");
        PlayerSettings.bundleVersion = GameConfig.ClientVersionStr;
        PlayerSettings.companyName = "Tamron";
        PlayerSettings.productName = "1378捕鱼";
        
        // PC Performance settings
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.runInBackground = true;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
        PlayerSettings.defaultIsNativeResolution = true;
        
        Debug.Log("PC build environment setup completed");
    }

    [MenuItem("Tools/PC/2. Build PC DLL", false, 102)]
    private static void HandleDllBoundlePC()
    {
        Debug.Log("Building DLL for PC...");
        
        BuildTarget target = BuildTarget.StandaloneWindows64;
        
        CompileDllCommand.CompileDll(target);
        var dir = Application.dataPath + "/../DLL_PC";
        DirectoryInfo dirOutDir = new DirectoryInfo(dir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(dir);
        }
        
        CopyAOTAssembliesToPC(dir);
        CopyHotUpdateAssembliesToPC(dir);
        
        var list = new List<AssetBundleInfo>();
        DirectoryInfo dirOutDir1 = new DirectoryInfo(dir);
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
        
        // Generate version files
        File.WriteAllBytes(dir + "/" + ABLoadConfig.VersionNO, versionNO);
        File.WriteAllLines(dir + "/" + ABLoadConfig.VersionPath, new string[]{
            LitJson.JsonMapper.ToJson(list),
        });
        
        Debug.Log("PC DLL build completed");
    }

    [MenuItem("Tools/PC/3. Build PC Assets", false, 103)]
    private static void BuildPCAssets()
    {
        Debug.Log("Building PC Assets...");
        
        // Build main hall assets for PC
        StartPack(GameEnum.All, BuildTarget.StandaloneWindows64);
        
        // Build other game assets if needed
        StartPack(GameEnum.Fish_3D, BuildTarget.StandaloneWindows64);
        StartPack(GameEnum.Fish_LK, BuildTarget.StandaloneWindows64);
        StartPack(GameEnum.FQZS, BuildTarget.StandaloneWindows64);
        StartPack(GameEnum.SH, BuildTarget.StandaloneWindows64);
        StartPack(GameEnum.WZQ, BuildTarget.StandaloneWindows64);
        
        Debug.Log("PC Assets build completed");
    }

    [MenuItem("Tools/PC/4. Copy PC Resources", false, 104)]
    private static void CopyPCResources()
    {
        Debug.Log("Copying PC Resources...");
        
        // Clean and create StreamingAssets for PC
        if (Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.Delete(Application.streamingAssetsPath, true);
        }
        Directory.CreateDirectory(Application.streamingAssetsPath);
        
        // Copy DLL
        CopyFolder(Application.dataPath + "/../DLL_PC", Application.streamingAssetsPath + "/DLL");
        Debug.Log("PC DLL copied");
        
        // Copy main assets
        CopyFolder(Application.dataPath + "/../AssetBundle_ALL", Application.streamingAssetsPath + "/AssetBundle_ALL");
        Debug.Log("PC main assets copied");
        
        // Copy other game assets
        string[] gameAssetPaths = {
            "AssetBundle_Fish3D",
            "AssetBundle_LK", 
            "AssetBundle_FQZS",
            "AssetBundle_SH",
            "AssetBundle_WZQ"
        };
        
        foreach (string assetPath in gameAssetPaths)
        {
            string srcPath = Application.dataPath + "/../" + assetPath;
            if (Directory.Exists(srcPath))
            {
                CopyFolder(srcPath, Application.streamingAssetsPath + "/" + assetPath);
                Debug.Log($"PC {assetPath} copied");
            }
        }
        
        // Generate folder configuration
        HandlefolderBoundle();
        
        Debug.Log("PC Resources copy completed");
    }

    [MenuItem("Tools/PC/5. Build PC Executable", false, 105)]
    private static void BuildPCExecutable()
    {
        Debug.Log("Building PC Executable...");
        
        string[] levels = {
            "Assets/Scene/Main.unity",
        };
        
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
        string buildPath = $"D:/build_source/PC/Fishing3D_PC_{timestamp}/";
        string exePath = buildPath + "Fishing3D.exe";
        
        // Ensure build directory exists
        if (!Directory.Exists(buildPath))
        {
            Directory.CreateDirectory(buildPath);
        }
        
        // PC Build options
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = levels;
        buildOptions.locationPathName = exePath;
        buildOptions.target = BuildTarget.StandaloneWindows64;
        buildOptions.targetGroup = BuildTargetGroup.Standalone;
        
        // PC-specific build options
        buildOptions.options = BuildOptions.None;
        // buildOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging; // For debug builds
        
        var result = BuildPipeline.BuildPlayer(buildOptions);
        
        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"PC Build succeeded: {exePath}");
            
            // Copy additional PC files
            CopyPCAdditionalFiles(buildPath);
            
            // Create installer or zip if needed
            CreatePCDistribution(buildPath, timestamp);
        }
        else
        {
            Debug.LogError("PC Build failed: " + result.summary.ToString());
            throw new System.Exception("PC executable build failed");
        }
    }

    private static void CopyAOTAssembliesToPC(string targetDir)
    {
        var target = BuildTarget.StandaloneWindows64;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        
        foreach (var dll in SettingsUtil.AOTAssemblyNames)
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
            if (!File.Exists(srcDllPath))
            {
                Debug.LogWarning($"AOT DLL not found: {srcDllPath}");
                continue;
            }
            
            string dllBytesPath = $"{targetDir}/{dll}.dll.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            Debug.Log($"[PC AOT] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }
    }

    private static void CopyHotUpdateAssembliesToPC(string targetDir)
    {
        var target = BuildTarget.StandaloneWindows64;
        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        
        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllPath = $"{hotfixDllSrcDir}/{dll}";
            string dllBytesPath = $"{targetDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
            Debug.Log($"[PC HotUpdate] copy hotfix dll {dllPath} -> {dllBytesPath}");
        }
    }

    private static void CopyPCAdditionalFiles(string buildPath)
    {
        try
        {
            // Copy PC-specific config files
            string configSrc = Application.dataPath + "/StreamingAssets/PC_Config";
            if (Directory.Exists(configSrc))
            {
                string configDest = buildPath + "Config";
                CopyFolder(configSrc, configDest);
            }
            
            // Create PC launcher script
            CreatePCLauncher(buildPath);
            
            // Create README
            CreatePCReadme(buildPath);
            
            Debug.Log("PC additional files copied");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to copy additional PC files: {e.Message}");
        }
    }

    private static void CreatePCLauncher(string buildPath)
    {
        string launcherContent = @"@echo off
    echo Starting Fishing 3D...
    echo.

    REM Check if game executable exists
    if not exist ""Fishing3D.exe"" (
        echo Error: Fishing3D.exe not found!
        pause
        exit /b 1
    )

    REM Set compatibility mode
    echo Setting up game environment...

    REM Launch game
    echo Launching game...
    start """" ""Fishing3D.exe""

    REM Optional: Wait for game to close
    REM ""Fishing3D.exe""

    echo Game closed.
    pause
    ";
        
        File.WriteAllText(buildPath + "Launch_Fishing3D.bat", launcherContent);
    }

    private static void CreatePCReadme(string buildPath)
    {
        string readmeContent = @"Fishing 3D - PC Version
    ========================

    System Requirements:
    - Windows 10 64-bit or later
    - DirectX 11 compatible graphics card
    - 4GB RAM minimum
    - 2GB free disk space

    Installation:
    1. Extract all files to a folder
    2. Run Launch_Fishing3D.bat or Fishing3D.exe
    3. Enjoy the game!

    Controls:
    - Mouse: Aim and shoot
    - Keyboard: Various game functions
    - ESC: Exit game

    Troubleshooting:
    - If game doesn't start, run as Administrator
    - Ensure Windows Defender/Antivirus allows the game
    - Update graphics drivers if you experience issues

    Version: " + GameConfig.ClientVersionStr + @"
    Build Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"

    Contact: support@touchmind.com
    ";
        
        File.WriteAllText(buildPath + "README.txt", readmeContent);
    }

    private static void CreatePCDistribution(string buildPath, string timestamp)
    {
        try
        {
            // Create zip file for distribution
            string zipPath = $"D:/build_source/PC/Fishing3D_PC_{timestamp}.zip";
            
            // You can use System.IO.Compression or third-party zip library
            Debug.Log($"PC build ready for distribution at: {buildPath}");
            Debug.Log($"Create ZIP manually or programmatically: {zipPath}");
            
            // Open build folder
            System.Diagnostics.Process.Start("explorer.exe", buildPath);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to create PC distribution: {e.Message}");
        }
    }

    [MenuItem("Tools/PC/Quick PC Build (No Assets)", false, 106)]
    private static void QuickPCBuild()
    {
        Debug.Log("=== Quick PC Build (Executable Only) ===");
        
        SetupPCBuildEnvironment();
        
        string[] levels = { "Assets/Scene/Main.unity" };
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
        string exePath = $"D:/build_source/PC/Quick_Fishing3D_PC_{timestamp}.exe";
        
        BuildPlayerOptions buildOptions = new BuildPlayerOptions();
        buildOptions.scenes = levels;
        buildOptions.locationPathName = exePath;
        buildOptions.target = BuildTarget.StandaloneWindows64;
        buildOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
        
        var result = BuildPipeline.BuildPlayer(buildOptions);
        
        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Quick PC Build succeeded: {exePath}");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + exePath);
        }
        else
        {
            Debug.LogError("Quick PC Build failed");
        }
    }

    [MenuItem("Tools/PC/Open PC Build Folder", false, 107)]
    private static void OpenPCBuildFolder()
    {
        string buildFolder = "D:/build_source/PC/";
        if (!Directory.Exists(buildFolder))
        {
            Directory.CreateDirectory(buildFolder);
        }
        System.Diagnostics.Process.Start("explorer.exe", buildFolder);
    }
}
