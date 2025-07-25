using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
using System;
using ICSharpCode.SharpZipLib.Zip;

public class MenuItemUI
{
    const string strTestServerDefine = "SDK_TEST";
    //[MenuItem("BuildAppEditeTools/P. Publish/Export UI", false, 501)]
    public static void PublishExportUI()
    {
        // 清空文件夹
        string strDir = "/StreamingAssets/ui";
        string strDirAssets = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;
        DirectoryInfo dirOutDir = new DirectoryInfo(strDirPath);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strDirPath);
        }
        else
        {
            Directory.Delete(strDirPath, true);
            Directory.CreateDirectory(strDirPath);
        }
        ClearAssetBundlesName();
        string sourcePathUI = Application.dataPath + "/Prefab/UI";
        string sourcePathUIAtlas = Application.dataPath + "/UIAtlas";
        string sourcePathUIFont = Application.dataPath + "/UIFonts";
        string sourcePathIconAtlas = Application.dataPath + "/iconAtlas";

        SetAssetBundlesName(sourcePathUI, "panel/", ".gui", false, true);
        SetAssetBundlesName(sourcePathUIAtlas, "atlas/", ".gui", false, true);
        SetAssetBundlesName(sourcePathUIFont, "font/", ".gui", false, true);
        SetAssetBundlesName(sourcePathIconAtlas, "iconatlas/", ".gui", false, true);

        //根据BuildSetting里面所激活的平台进行打包
        //BuildPipeline.BuildAssetBundles(strDirAssets, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        BuildPipeline.BuildAssetBundles(strDirAssets, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        ClearManifestFile(strDirPath);
        ClearAssetBundlesName();
        AssetDatabase.Refresh();
        Debug.Log("PublishExportUI Done");
    }

    public static void PublishExportSounds()
    {
        // 清空文件夹
        string strDir = "/StreamingAssets/sound";
        string strDirAssets = "Assets" + strDir;
        string strDirPath = Application.dataPath + strDir;
        DirectoryInfo dirOutDir = new DirectoryInfo(strDirPath);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strDirPath);
        }
        else
        {
            Directory.Delete(strDirPath, true);
            Directory.CreateDirectory(strDirPath);
        }

        ClearAssetBundlesName();
        string sourcePathMusic = Application.dataPath + "/BaseResources/Sounds/Music";
        string sourcePathEffect = Application.dataPath + "/BaseResources/Sounds/Effect";
        SetAssetBundlesName(sourcePathMusic, "music/", ".sound", false, false);
        SetAssetBundlesName(sourcePathEffect, "effect/", ".sound", false, false);
        BuildPipeline.BuildAssetBundles(strDirAssets, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        ClearManifestFile(strDirPath);
        ClearAssetBundlesName();
        AssetDatabase.Refresh();
        Debug.Log("PublishExportSounds Done");
    }

    public static void PublishExportConfig()
    {
#if CONFIG_COMPRESS
        CConfigLoader.CompressZIPConfigData();
#endif

        Debug.Log("PublishExportConfig Done");
    }
    
    public static void PublishExportDecompressFile()
    {
        string strContent = "";

        // 解压Map文件
        string strContentTmp = "";
        string strTmpDir = "";

        strContentTmp = "";
        strTmpDir = Application.dataPath + "/StreamingAssets/map";
        PublishExportDecompressFile(strTmpDir, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        strTmpDir = Application.dataPath + "/StreamingAssets/map0";
        PublishExportDecompressFile(strTmpDir, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        strTmpDir = Application.dataPath + "/StreamingAssets/config";
        PublishExportDecompressFile(strTmpDir, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        strTmpDir = Application.dataPath + "/StreamingAssets/res/background";
        PublishExportDecompressFile(strTmpDir, ref strContentTmp);
        strContent += strContentTmp;

        strContentTmp = "";
        strTmpDir = Application.dataPath + "/StreamingAssets/lua";
        PublishExportDecompressFile(strTmpDir, ref strContentTmp);
        strContent += strContentTmp;

        // 写入解压文件信息
        string strDecompressFile = Application.dataPath + "/StreamingAssets/DecompressFile.ver";
        File.Delete(strDecompressFile);
        File.WriteAllText(strDecompressFile, strContent, Encoding.UTF8);

        Debug.Log("PublishExportDecompressFile Done");
    }

    public static void PublishExportAll()
    {
        if (!EditorUtility.DisplayDialog("提示", "确定发布全部资源，它可能花费你大量时间？", "是", "否"))
        {
            return;
        }

        PublishExportAllBase();
    }
    public static void PublishExportDecompressFile(string strDirPath, ref string strContent)
    {
        DirectoryInfo dirDir = new DirectoryInfo(strDirPath);
        if (!dirDir.Exists)
        {
            return;
        }
        string strPathBase = Application.dataPath + "/StreamingAssets/";
        string strValueDir = strDirPath.Replace(strPathBase, "");
        strValueDir = strValueDir.Replace("\\", "/");
        if (!string.IsNullOrEmpty(strValueDir))
        {
            strContent += "dir," + strValueDir + "\n";
        }
        string[] strFileNames = Directory.GetFiles(strDirPath);
        foreach (string file in strFileNames)
        {
            if (!File.Exists(file))
            {
                continue;
            }
            if (file.Contains(".meta"))
            {
                continue;
            }
            string strValue = file.Replace(strPathBase, "");
            strValue = strValue.Replace("\\", "/");
            strContent += "file," + strValue + "\n";
        }

        string[] strDir = Directory.GetDirectories(strDirPath);
        foreach (string dir in strDir)
        {
            PublishExportDecompressFile(dir, ref strContent);
        }
    }

    public static void PublishExportAllBase()
    {
        Packager.HandleAllAndSubBundle();
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// </summary>
    public static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("ClearAssetBundlesName: " + length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }
        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
    }

    /// <summary>
    /// 设置AssetBundleName
    /// </summary>
    private static void SetAssetBundlesName(string source, string strPrefix, string strSuffix, bool bSubDir, bool prefab = true)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                if (bSubDir)
                {
                    SetAssetBundlesName(files[i].FullName, strPrefix, strSuffix, bSubDir);
                }
            }
            else
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                if (prefab)
                {
                    if (files[i].Name.EndsWith(".prefab"))
                    {
                        SetAssetBundlesNameFile(files[i].FullName, strPrefix, strSuffix);
                    }
                }
                else
                {
                    SetAssetBundlesNameFile(files[i].FullName, strPrefix, strSuffix);
                }
            }
        }
    }

    private static void SetAssetBundlesNameFile(string source, string strPrefix, string strSuffix)
    {
        string _source = Replace(source);
        string _sourceDir = Replace(strPrefix);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _sourceDir + Path.GetFileName(_source);
        assetName = assetName.Replace(Path.GetExtension(assetName), strSuffix);
        assetImporter.assetBundleName = assetName;
    }

    private static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }

    private static void ClearManifestFile(string source)
    {
        if (!Directory.Exists(source))
        {
            return;
        }

        ClearManifestFileSingle(source);
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                ClearManifestFile(files[i].FullName);
            }
        }
    }

    private static void ClearManifestFileSingle(string source)
    {
        if (!Directory.Exists(source))
        {
            return;
        }
        ArrayList urlAddr = new ArrayList();
        string[] fileList = Directory.GetFileSystemEntries(source);
        foreach (string file in fileList)
        {
            string newStr = Path.GetFullPath(file);
            string filename = Path.GetFileName(file);
            string strExtension = Path.GetExtension(newStr).ToLower();
            if (strExtension == ".manifest")
            {
                urlAddr.Add(filename);
            }
        }

        for (int i = 0; i < urlAddr.Count; i++)
        {
            FileInfo ofile = new FileInfo(source + "/" + urlAddr[i]);
            if (ofile.Exists)
            {
                ofile.Delete();
            }
        }
    }

    [MenuItem("BuildAppEditeTools/Tools/CheckZhongWenInCode", false, 601)]
    public static void CheckZhongWenInCode()
    {
        DirectoryInfo dictoryInfo = new DirectoryInfo(Application.dataPath + "/Scripts");
        if (!dictoryInfo.Exists)
        {
            return;
        }

        FileInfo[] fileInfos = dictoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);
        foreach (FileInfo files in fileInfos)
        {
            string path = files.FullName;
            int i = path.LastIndexOf("Assets");
            if (i < 0)
            {
                continue;
            }
            string assetPath = path.Substring(i, path.Length - i);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
            string text = textAsset.text;
            text = text.Replace("\r", "");
            string[] strLines = text.Split('\n');
            foreach (string strLine in strLines)
            {
                string strValue = strLine.Trim();
                if (strValue.IndexOf("//") == 0)
                {
                    continue;
                }
                //用正则表达式把代码里面两种字符串中间的字符串提取出来。
                Regex reg = new Regex("\".*?\"");
                MatchCollection mc = reg.Matches(strValue);
                foreach (Match m in mc)
                {
                    if (IsChina(m.Value))
                    {
                        if (files.Name.Contains("Editor"))
                        {
                            continue;
                        }

                        if (files.Name.Equals("CurrentBundleVersion.cs"))
                        {
                            continue;
                        }

                        if (strValue.LastIndexOf("//", m.Index) != -1)
                        {
                            continue;
                        }
                        Debug.Log(files.Name + " : " + m.Value);
                    }
                }
            }
        }

        Debug.Log("CheckZhongWenInCode Done");
    }

    [MenuItem("BuildAppEditeTools/Tools/CheckEventDelegateRegistPair", false, 609)]
    public static void CheckEventDelegateRegistPair()
    {
        DirectoryInfo dictoryInfo = new DirectoryInfo(Application.dataPath + "/Scripts");

        if (!dictoryInfo.Exists)
        {
            return;
        }

        FileInfo[] fileInfos = dictoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);

        for (int nIndex = 0; nIndex < fileInfos.Length; nIndex++)
        {
            string path = fileInfos[nIndex].FullName;

            int nIndexContinue = path.LastIndexOf("Assets");
            if (nIndexContinue < 0)
            {
                continue;
            }

            string assetPath = path.Substring(nIndexContinue, path.Length - nIndexContinue);
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
            string text = textAsset.text;

            text = text.Replace("\r", "");
            string[] strLines = text.Split('\n');

            List<KeyValuePair<string, int>> lstRegist = new List<KeyValuePair<string, int>>(); ;
            List<KeyValuePair<string, int>> lstUnRegist = new List<KeyValuePair<string, int>>();

            for (int nColumn = 0; nColumn < strLines.Length; nColumn++)
            {
                if (strLines[nColumn].Contains("EventDispatcher.GameWorld.Regist"))
                {
                    lstRegist.Add(new KeyValuePair<string, int>(strLines[nColumn], nColumn));
                }
                else if (strLines[nColumn].Contains("EventDispatcher.GameWorld.UnRegist"))
                {
                    lstUnRegist.Add(new KeyValuePair<string, int>(strLines[nColumn], nColumn));
                }
            }

            for (int i = lstRegist.Count-1; i >= 0; i--)
            {
                bool bIsDelete = false;
                int nLastIndex = lstRegist[i].Key.IndexOf("EventDispatcher.GameWorld.Regist") + "EventDispatcher.GameWorld.Regist".Length;
                string strRegist = Regex.Replace(lstRegist[i].Key.Substring(nLastIndex), @"\s", "").Trim();

                for (int j = lstUnRegist.Count-1; j >= 0; j--)
                {
                    int nLastIndexUn = lstUnRegist[j].Key.IndexOf("EventDispatcher.GameWorld.UnRegist") + "EventDispatcher.GameWorld.UnRegist".Length;
                    string strUnRegist = Regex.Replace(lstUnRegist[j].Key.Substring(nLastIndexUn), @"\s", "").Trim();

                    if (strRegist == strUnRegist) 
                    {
                        lstUnRegist.RemoveAt(j);
                        bIsDelete = true;
                    }
                }

                if (bIsDelete)
                {
                    lstRegist.RemoveAt(i);
                }
            }

            for (int i = 0; i < lstRegist.Count; i++)
            {
                Debug.Log(string.Format("Resist error occur in \"{0}.cs-{1}, content = \"{2}\"\"", fileInfos[nIndex].Name, lstRegist[i].Value, lstRegist[i].Key.Trim()));
            }

            for (int i = 0; i < lstUnRegist.Count; i++)
            {
                Debug.Log(string.Format("UnResist error occur in \"{0}.cs-{1}, content = \"{2}\"\"", fileInfos[nIndex].Name, lstUnRegist[i].Value , lstUnRegist[i].Key.Trim()));
            }
        }

        Debug.Log("CheckEventDelegateRegistPair Done");
    }

    public static bool IsChina(string CString)
    {
        bool BoolValue = false;

        for (int i = 0; i < CString.Length; i++)
        {
            if (System.Convert.ToInt32(System.Convert.ToChar(CString.Substring(i, 1))) < System.Convert.ToInt32(System.Convert.ToChar(128)))
            {
                BoolValue = false;
            }
            else
            {
                return BoolValue = true;
            }
        }
        return BoolValue;
    }



}
