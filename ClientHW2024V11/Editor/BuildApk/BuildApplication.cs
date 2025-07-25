using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

public class BuildApplication : EditorWindow
{
    string TargetAndroidPath { get { return Application.dataPath.Replace("Assets", "ForEclipse"); } }
    string TargetIOSPath { get { return Application.dataPath.Replace("Assets", "ForXcode"); } }
    string AndroidProjectPath { get { return string.Format("{0}/{1}", TargetAndroidPath,PlayerSettings.productName); } }
    ClientSettingDraw mClientSettingDraw = new ClientSettingDraw();
    //ApplicationPackageNameSwith mApplicationPackageNameSwith = new ApplicationPackageNameSwith();
    

    [MenuItem("Custom Editor/一键打包加密App")]
    static void Init()
    {
        BuildApplication window = GetWindow<BuildApplication>();
        window.InitWindow();
    }
    GUIStyle mGUIStyle;

    string KeyStorePath = "";
    bool IsTranslateUIAtlash = false;
    bool IsDestroyOriFile = false;
    bool IsDeleteCsvFile = true;
    //bool IsReNamePackageName = false;
    string CsvFolderPath = "";

    struct ChannelInfo
    {
        public string Name;
        public string Path;
    };
    List<ChannelInfo> ChannelSDKList = new List<ChannelInfo>();
    string[] SdkNames = null;
    int SelectionName = 0;
    void InitWindow()
    {
        GetKeyStorePath();
        GetCsvPath();
        GetChannelSDKList();
    }

    void GetChannelSDKList()
    {
        ChannelSDKList.Clear();
        string searchPath = "/ChannelSDK";
        string fullPath = Application.dataPath + searchPath;
        if (Directory.Exists(fullPath) == false)
        {
            Directory.CreateDirectory(fullPath);
        }
        string[] sdkList = Directory.GetDirectories(fullPath);

        SdkNames = new string[sdkList.Length];
        for (int i = 0; i < sdkList.Length; ++i)
        {
            ChannelInfo info = new ChannelInfo();
            info.Path = sdkList[i];
            info.Path = info.Path.Replace("\\", "/");
            int pos = info.Path.IndexOf(searchPath) + searchPath.Length;

            info.Name = info.Path.Substring(pos);
            ChannelSDKList.Add(info);
            SdkNames[i] = "渠道："+info.Name;
        }
    }

    void ClearSDKFolder()
    {
        Function.DeleteFolder(Application.dataPath + "/Plugins/Android");
    }

    void ReplaceSDK(int index)
    {
        UnityEngine.Debug.Log("选择： "+index);
        ClearSDKFolder();
        if (index < ChannelSDKList.Count)
        {
            UnityEngine.Debug.Log("拷贝 sdk： " + ChannelSDKList[index].Path);
            Function.CopyDirectory(ChannelSDKList[index].Path, Application.dataPath + "/Plugins/Android");
        }
        CopyCommonSDK();
    }

    void CopyCommonSDK()
    {
        if (Directory.Exists(Application.dataPath + "/CommonSDK") == false )
        {
            Directory.CreateDirectory(Application.dataPath + "/CommonSDK");
        }
        Function.CopyDirectory(Application.dataPath +"/CommonSDK", Application.dataPath + "/Plugins/Android");
    }

    void OnGUI()
    {
        if (mGUIStyle == null)
        {
            mGUIStyle = new GUIStyle(EditorStyles.boldLabel);
            mGUIStyle.normal.textColor = Color.green;
            mGUIStyle.fontStyle = FontStyle.Bold;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("签名及.so文件路径：",GUILayout.Width(200));
        GUILayout.TextArea(KeyStorePath);
        if (GUILayout.Button("设置", GUILayout.Width(100)))
        {
            PlayerPrefs.SetString("KeyStorePath", "");
            GetKeyStorePath();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Csv文件路径：", GUILayout.Width(200));
        GUILayout.TextArea(CsvFolderPath);
        if (GUILayout.Button("设置", GUILayout.Width(100)))
        {
            PlayerPrefs.SetString("CsvFolderPath", "");
            GetCsvPath();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("1、转换Csv文件：(已废弃)", mGUIStyle);
        //GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Csv生成成assets代码(步骤1)", GUILayout.Height(50)))
        //{
        //    if (string.IsNullOrEmpty(CsvFolderPath))
        //        GetCsvPath();
        //    List<string> FilePathGroup = GetObjGroupBuyPath(CsvFolderPath);
        //    CsvToScriptTable.SwithCsvFile(FilePathGroup,true);
        //}
        //if (GUILayout.Button("拷贝Csv数据到assets(步骤2)", GUILayout.Height(50)))
        //{
        //    if (string.IsNullOrEmpty(CsvFolderPath))
        //        GetCsvPath();
        //    List<string> FilePathGroup = GetObjGroupBuyPath(CsvFolderPath);
        //    CsvToScriptTable.SwithCsvFile(FilePathGroup,false);
        //}
        //GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.Label("2、选择渠道 sdk：", mGUIStyle);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("清空SDk目录", GUILayout.Height(50)))
        {
            ClearSDKFolder();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (SdkNames != null)
        {
            SelectionName = GUILayout.SelectionGrid(SelectionName, SdkNames, 1, GUILayout.Width(200));
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("替换SDK", GUILayout.Height(50)))
        {
            ReplaceSDK(SelectionName);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        GUILayout.Label("3、打包apk文件：", mGUIStyle);
        IsDestroyOriFile = EditorGUILayout.Toggle("1、删除__ori文件：", IsDestroyOriFile,GUILayout.Width(500));
        IsTranslateUIAtlash = EditorGUILayout.Toggle("2、重新压缩图集：", IsTranslateUIAtlash, GUILayout.Width(500));
        IsDeleteCsvFile = EditorGUILayout.Toggle("3、删除Csv文件：", IsDeleteCsvFile, GUILayout.Width(500));
        GUILayout.Space(5);
        mClientSettingDraw.OnDraw();
#if UNITY_ANDROID
        if (GUILayout.Button("开始打包安卓apk",GUILayout.Height(50)))
        {
            if (Directory.Exists(AndroidProjectPath))
                Directory.Delete(AndroidProjectPath, true);
            Directory.CreateDirectory(AndroidProjectPath);
            if (IsDeleteCsvFile)
                DeleteCsvFile();
            if (IsDestroyOriFile)
                CleanUp__oriFile();
            BuildPipeline.BuildPlayer(getActiveScenes(), TargetAndroidPath, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
            UnityEngine.Debug.Log("BuildSucess!:" + AndroidProjectPath);
            EditorUtility.DisplayProgressBar("Progress", "CopyFile", 0.2f);
            CopyFile();
            EditorUtility.DisplayProgressBar("Progress", "DllEditor", 0.3f);
            DllEditor();
            EditorUtility.DisplayProgressBar("Progress", "BuildApk", 0.5f);
            GeneratorAPK();
            EditorUtility.ClearProgressBar();

        }
#elif UNITY_IPHONE
        if (GUILayout.Button("开始打包IOS工程", GUILayout.Height(50)))
        {
            if (Directory.Exists(TargetIOSPath))
                Directory.Delete(TargetIOSPath, true);
            Directory.CreateDirectory(TargetIOSPath);
            if (IsDeleteCsvFile)
                DeleteCsvFile();
            if (IsDestroyOriFile)
                CleanUp__oriFile();
			BuildPipeline.BuildPlayer(getActiveScenes(), TargetIOSPath, BuildTarget.iOS, BuildOptions.None);
        }
#endif
    }


    List<string> GetObjGroupBuyPath(string path)
    {
        List<string> getObjGroup = new List<string>();
        List<FileInfo> fileGroup = GetAllFileByDir(path,".csv");
        foreach (var child in fileGroup)
        {
            if (child.Name.Contains(".meta"))
                continue;
            string mpath = "Assets" + (child.FullName.Replace("\\", "/").Replace(Application.dataPath, ""));
            //Object obj = AssetDatabase.LoadMainAssetAtPath(mpath);
            getObjGroup.Add(mpath);
        }
        return getObjGroup;
    }

    public static List<FileInfo> GetAllFileByDir(string dir, string extension = "")
    {
        List<FileInfo> result = new List<FileInfo>();
        List<DirectoryInfo> m_DirInfo = new List<DirectoryInfo>() { new DirectoryInfo(dir) };
        while (m_DirInfo != null && m_DirInfo.Count > 0)
        {
            List<DirectoryInfo> child_ChildDir = new List<DirectoryInfo>();
            foreach (var childDir in m_DirInfo)
            {
                foreach (FileInfo file in childDir.GetFiles())
                {
                    if (string.IsNullOrEmpty(extension))
                    {
                        result.Add(file);
                    }
                    else
                    {
                        if (Path.GetExtension(file.FullName) == extension)
                        {
                            result.Add(file);
                        }
                    }
                }
                DirectoryInfo[] dics = childDir.GetDirectories();
                if (dics.Length > 0)
                {
                    foreach (var dicsChild in dics)
                    {
                        child_ChildDir.Add(dicsChild);
                    }
                }
            }
            m_DirInfo = child_ChildDir;
        }
        return result;
    }

    void DeleteCsvFile()
    {
        string csvFilePath = Application.dataPath + "/Resources/Data/csv";
        UnityEngine.Debug.Log(csvFilePath);
        if (Directory.Exists(csvFilePath))
            Directory.Delete(csvFilePath,true);
    }

    void CleanUp__oriFile()
    {
        CleanUpDir mCleanUpDir = new CleanUpDir();
        mCleanUpDir.CleanUpDirByPath(string.Format("{0}/Resources", Application.dataPath));
    }

    string[] getActiveScenes()
    {
        var activeScenes = EditorBuildSettings.scenes;
        List<string> getScenesStr = new List<string>();
        for (int i = 0; i < activeScenes.Length; i++)
        {
            if (activeScenes[i].enabled)
                getScenesStr.Add(activeScenes[i].path);
            //UnityEngine.Debug.Log(activeScenes[i].path);
        }
        return getScenesStr.ToArray();
    }

    string GetKeyStorePath()//签名文件路径
    {
        KeyStorePath = PlayerPrefs.GetString("KeyStorePath", "");
        while(string.IsNullOrEmpty(KeyStorePath))
        {
            KeyStorePath = EditorUtility.OpenFolderPanel("设置签名及.so文件路径", Application.dataPath, "");
            PlayerPrefs.SetString("KeyStorePath", KeyStorePath);
        }
        return KeyStorePath;
    }

    string GetCsvPath()//Csv文件路径
    {
        CsvFolderPath = PlayerPrefs.GetString("CsvFolderPath", "");
        while (string.IsNullOrEmpty(CsvFolderPath))
        {
            CsvFolderPath = EditorUtility.OpenFolderPanel("设置Csv文件路径", Application.dataPath, "");
            PlayerPrefs.SetString("CsvFolderPath", CsvFolderPath);
        }
        return CsvFolderPath;
    }
    void CopyFile()
    {
        if (string.IsNullOrEmpty(KeyStorePath))
            GetKeyStorePath();
        var m_DirInfo = new DirectoryInfo(KeyStorePath);
        List<FileInfo> FileResult = new List<FileInfo>();
        GetFilesByPath(m_DirInfo, ref FileResult);
        foreach (var child in FileResult)
        {
            
            string projectTargetPath = AndroidProjectPath + "/" + child.Name;
            string x86SoPath = string.Format("{0}/libs/x86/libmono.so", AndroidProjectPath);
            string arm7SoPath = string.Format("{0}/libs/armeabi-v7a/libmono.so", AndroidProjectPath);
            //UnityEngine.Debug.Log(child.Name);
            switch (child.Name)
            {
                case "libmonoArm7.so":
                    child.CopyTo(arm7SoPath, true);
                    //UnityEngine.Debug.Log("CopyFile:" + arm7SoPath);
                    break;
                case "libmonox86.so":
                    child.CopyTo(x86SoPath, true);
                    //UnityEngine.Debug.Log("CopyFile:" + x86SoPath);
                    break;
                default:
                    child.CopyTo(projectTargetPath, true);
                    //UnityEngine.Debug.Log("CopyFile:" + projectTargetPath);
                    break;
            }
        }
    }


    void GetFilesByPath(DirectoryInfo di, ref List<FileInfo> result)
    {
        foreach (FileInfo file in di.GetFiles())
        {
            result.Add(file);
        }
        DirectoryInfo[] dics = di.GetDirectories();
        if (dics.Length > 0)
        {
            foreach (DirectoryInfo sdi in dics)
            {
                GetFilesByPath(sdi, ref result);
            }
        }
    }

    void GeneratorAPK()
    {
        string CsPath = AndroidProjectPath.Replace("/","\\");
        string code = string.Format("android update project -p . -t android-21 -s");
        RunCmd(CsPath, code);
        code = "ant release";
        RunCmd(CsPath, code);

        string resFile = GetReleaseApkPath();
        if (string.IsNullOrEmpty(resFile))
        {
            EditorUtility.DisplayDialog("提示", "没有生成带签名的apk包", "确定");
            return;
        }
        string targetFile = string.Format(string.Format("{0}/{1}_{2}.apk", TargetAndroidPath, PlayerSettings.productName,
            System.DateTime.Now.ToString().Replace("/", "_").Replace(":", ".")));
        File.Copy(resFile, targetFile);
        Application.OpenURL(TargetAndroidPath);
    }

    string GetReleaseApkPath()
    {
        string getPath = null;
        string resFile = string.Format("{0}/bin/", AndroidProjectPath);
        var m_DirInfo = new DirectoryInfo(resFile);
        FileInfo[] FileResult = m_DirInfo.GetFiles();
        foreach(var child in FileResult)
        {
            //UnityEngine.Debug.Log(child.Name);
            if (child.Name.EndsWith("-release.apk"))
            {
                getPath = child.FullName;
                UnityEngine.Debug.Log(child.FullName);
                break;
            }
        }
        return getPath;
    }

    void DllEditor()
    {
        string dllPath = string.Format("{0}/assets/bin/Data/Managed/Assembly-CSharp.dll", AndroidProjectPath);
        //UnityEngine.Debug.Log("EditorDll:"+dllPath);
        EditorDll.Instance.Do(dllPath);
    }

    private static void RunCmd(string workingPath, string command)
    {
        Process p = new Process();
        p.StartInfo.FileName = "cmd.exe";           
        p.StartInfo.WorkingDirectory = workingPath;
        p.StartInfo.Arguments = "/c " + command;    
        p.StartInfo.UseShellExecute = false;      
        p.StartInfo.RedirectStandardInput = true; 
        p.StartInfo.RedirectStandardOutput = false;
        p.StartInfo.RedirectStandardError = true;  
        p.StartInfo.CreateNoWindow = true;          
        p.Start();
		p.WaitForExit();
		UnityEngine.Debug.Log("CmdError:"+p.StandardError.ReadToEnd ());
		p.Close();
    }
}
