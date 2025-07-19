using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class AssetBundleExport {
    private static BuildAssetBundleOptions AssetBundleOptions = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression;
    public static void StartPack(BuildTarget target, string res_path,string export_path) {
        if (EditorUserBuildSettings.activeBuildTarget != target) {
            Debug.LogError("请先切换平台");
            return;
        }

        //删除打包残留
        if (Directory.Exists(export_path)) {
            Directory.Delete(export_path,true);
        }
        Directory.CreateDirectory(export_path);

        List<FileDepend> list = DependManager.AnalysisDepend(res_path);
        for (int i = 0, count = list.Count; i < count; i++)
        {
            AssetImporter importer = AssetImporter.GetAtPath(list[i].AssetPath);
            if (importer == null)
            {
                Debug.LogError("null:" + list[i].AssetPath);
            }
            importer.assetBundleName = list[i].ab_name;
            EditorUtility.DisplayProgressBar("AssetBundleName", string.Format("AssetBundleName ...{0}/{1}", (i + 1), count), (i + 1) * 1f / count);
        }
        Debug.Log("开始打包 >> 将会打包当前平台的assetbundle >>" + target);
        BuildPipeline.BuildAssetBundles(export_path, AssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);
        //清除打包标志
        for (int i = 0, count = list.Count; i < count; i++)
        {
            AssetImporter importer = AssetImporter.GetAtPath(list[i].AssetPath);
            importer.assetBundleName = string.Empty;
            EditorUtility.DisplayProgressBar("UnBundle", string.Format("UnBundle All ...", i + 1, count), (i + 1) * 1f / count);
        }
        //清除.manifest文件
        string[] files = Directory.GetFiles(export_path, "*.manifest", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        //写入关联以及文件MD5信息
        EditorUtility.DisplayProgressBar("处理 ", "依赖关系处理", 0f);
        Dictionary<string, AssetBundleInfo> dic_ab = new Dictionary<string, AssetBundleInfo>();
        AssetBundleInfo ab;
        string ab_name;
        FileInfo file_info;
        for (int i = 0, count = list.Count; i < count; i++)
        {
            ab_name = list[i].ab_name;
            if (dic_ab.ContainsKey(ab_name) == false)
            {
                file_info = new FileInfo(export_path + ab_name);
                if (file_info.Exists == false)
                {
                    File.Copy(list[i].AssetPath, file_info.FullName);
                    file_info = new FileInfo(list[i].AssetPath);
                }
                ab = new AssetBundleInfo();
                ab.ABname = ab_name;
                ab.MD5 = Tools.GetMD5HashFromFile(file_info.FullName);
                ab.Size = file_info.Length;
                foreach (var depend in list[i].mUseList)
                {
                    if (depend.IsAssetBundle && ab.DependList.Contains(depend.ab_name) == false)
                    {
                        ab.DependList.Add(depend.ab_name);
                    }
                }
                dic_ab.Add(ab.ABname, ab);
            }
            EditorUtility.DisplayProgressBar("处理 ", "依赖关系处理" + list[i].ab_name, (i + 1) * 1f / count);
        }

        byte[] versionNO = System.Text.Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddHHmm"));

        //生成版本号文件
        File.WriteAllBytes(export_path + ABLoadConfig.VersionNO, versionNO);

        //生成资源列表文件
        File.WriteAllLines(export_path + ABLoadConfig.VersionPath, new string[]
        {
            LitJson.JsonMapper.ToJson(new List<AssetBundleInfo>(dic_ab.Values)),
        });

        Debug.Log("打包完成 path:" + export_path);
        EditorUtility.ClearProgressBar();
    }
    //public static void AnalysisDepend(string path) {//分析引用
    //    Dictionary<string, FileDepend> dic = new Dictionary<string, FileDepend>();
    //    int count = 0;
    //    string select_path = AssetDatabase.GetAssetPath(Selection.activeObject);
    //    LogMgr.LogError("select_path:" + select_path);
    //    LogMgr.LogError("浅引用：");
    //    string[] deps = AssetDatabase.GetDependencies(select_path, false);
    //    foreach (var item in deps) {
    //        LogMgr.LogError(item);
    //    }
    //    LogMgr.LogError("深引用：");
    //    deps = AssetDatabase.GetDependencies(select_path, true);
    //    foreach (var item in deps) {
    //        LogMgr.LogError(item);
    //    }
    //    LogMgr.LogError("完成");
    //}
}