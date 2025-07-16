using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// <summary>
/// 引用管理
/// </summary>
public class DependManager {
    public static string AssetPath = new DirectoryInfo(".").FullName;//项目根目录
    public static Dictionary<string, FileDepend> dic_file = new Dictionary<string, FileDepend>();
    private static List<string> shader_list = new List<string>();

    public static string AbsToAssetPath(string path) {//绝对路径改为相对路径
        return path.Replace(AssetPath, "");
    }
    public static bool Filter(string file_name) {//是否过滤文件
        if (file_name.EndsWith(".shader")) {//如果是shader则加入需要打包的列表中
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(file_name);
            if (shader_list.Contains(shader.name) == false) {
                shader_list.Add(shader.name);
            }
            return true;
        }
        if (file_name.EndsWith(".meta")
            || file_name.EndsWith(".cs")
            //|| file_name.EndsWith(".shader")
            || file_name.EndsWith(".dll")
            || file_name.EndsWith(".js")) {
            return true;
        } else {
            return false;
        }
    }
    public static List<FileDepend> AnalysisDepend(string path) {//开始分析
        dic_file.Clear();
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        string asset_path;
        shader_list.Clear();
        foreach (var item in files) {
            if (DependManager.Filter(item)) {
                continue;
            }
            asset_path = DependManager.AbsToAssetPath(item);
            dic_file.Add(asset_path, new FileDepend(asset_path, true));
        }
        EditorUtility.DisplayProgressBar("分析 ", "开始分析 ....", 0f);
        List<FileDepend> dep_list = new List<FileDepend>(dic_file.Values);
        for (int i = 0; i < dep_list.Count; i++) {
            EditorUtility.DisplayProgressBar("分析 ", "开始分析" + dep_list[i].AssetPath, i * 1f / dep_list.Count);
            dep_list[i].StartFind();
        }
        DependManager.AddIncludedShader(shader_list);
        EditorUtility.DisplayProgressBar("处理 ", "开始导出文件依赖信息 ....", 0f);
        List<FileDepend> export_list = new List<FileDepend>();
        List<FileDepend> check_list = new List<FileDepend>();
        List<string> export_asset_paths = new List<string>();
        foreach (var item in dic_file.Values)
        {
            if (item.IsAssetBundle)
            {
                export_list.Add(item);
                export_asset_paths.Add(item.AssetPath);
            }
            else
            {
                check_list.Add(item);
            }
        }

        CheckMissingAsset(check_list, export_asset_paths, export_list);

        EditorUtility.ClearProgressBar();
        dic_file.Clear();
        return export_list;
    }

    /// <summary>
    /// 检查遗漏的需要打包的资源
    /// </summary>
    /// <param name="check_list"></param>
    /// <param name="exportList"></param>
    /// <param name="export_list"></param>
    private static void CheckMissingAsset(List<FileDepend> check_list,List<string> exportList, List<FileDepend> export_list)
    {
        for (int i = 0; i < check_list.Count; i++)
        {
            string[] paths = AssetDatabase.GetDependencies(check_list[i].AssetPath, false);

            for (int j = 0; j < paths.Length; j++)
            {
                if (exportList.Contains(paths[j]))
                {
                    check_list[i].IsAssetBundle = true;
                    export_list.Add(check_list[i]);
                    break;
                }
            }
        }
        for (int i = 0; i < check_list.Count; i++)
        {
            if (check_list[i].IsAssetBundle)
                continue;

            if (check_list[i].AssetPath.EndsWith(".prefab"))
            {
                try
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(check_list[i].AssetPath);
                    if (prefab != null)
                    {
                        Component uiAtlas = prefab.GetComponent("UIAtlas");
                        if (uiAtlas != null)
                        {
                            check_list[i].IsAssetBundle = true;
                            export_list.Add(check_list[i]);
                            Debug.Log("Force included UIAtlas prefab: " + check_list[i].AssetPath);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Error checking UIAtlas for: " + check_list[i].AssetPath + " - " + e.Message);
                }
            }
        }
    }

    public static FileDepend AddDepend(string path) {//添加一个引用文件
        FileDepend fd;
        if (dic_file.TryGetValue(path,out fd) == false) {
            fd = new FileDepend(path, false);
            dic_file.Add(path, fd);
        }
        fd.AddUse();
        return fd;
    }
    public static void AddIncludedShader(List<string> list) {//添加需要打包的shader
        SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
        SerializedProperty it = graphicsSettings.GetIterator();
        SerializedProperty dataPoint;
        Shader shader;
        while (it.NextVisible(true)) {
            if (it.name == "m_AlwaysIncludedShaders") {
                for (int i = 0; i < it.arraySize; i++) {
                    dataPoint = it.GetArrayElementAtIndex(i);
                    shader = dataPoint.objectReferenceValue as Shader;
                    if (list.Contains(shader.name) == false) {
                        list.Add(shader.name);
                    }
                }
                it.ClearArray();

                for (int i = 0; i < list.Count; i++) {
                    it.InsertArrayElementAtIndex(i);
                    dataPoint = it.GetArrayElementAtIndex(i);
                    dataPoint.objectReferenceValue = Shader.Find(list[i]);
                }

                graphicsSettings.ApplyModifiedProperties();
            }
        }
    }
}

public class FileDepend {//文件引用关系
    public string AssetPath;//资源相对路径
    public int UseCount;//引用个数
    public bool IsAssetBundle;//是否需要打包
    public List<FileDepend> mUseList = new List<FileDepend>();//被引用列表
    public bool IsFind;//是否已经查找过内部
    public string ab_name;//ab文件名称

    public FileDepend(string path, bool is_assetbundle) {
        this.AssetPath = path;
        this.IsAssetBundle = is_assetbundle;
        this.UseCount = 0;
        this.IsFind = false;
        this.UpdateABName();
    }
    private void UpdateABName() {
        if (this.AssetPath.EndsWith(".png") || this.AssetPath.EndsWith(".jpg")) {
            if (AssetDatabase.LoadAssetAtPath<Sprite>(this.AssetPath) != null) {
                TextureImporter ti = AssetImporter.GetAtPath(this.AssetPath) as TextureImporter;
                string pack_tag = ti.spritePackingTag.Trim();
                if (string.IsNullOrEmpty(pack_tag) == false) {
                    this.ab_name = AssetBundleManager.PathToAbName(string.Format("_img_pack/{0}",pack_tag));
                    return;
                }
            }
        }
        this.ab_name = AssetBundleManager.PathToAbName(this.AssetPath);
    }
    public void StartFind() {//开始查找引用
        if (this.IsFind) {
            return;
        }
        this.IsFind = true;
        string[] paths = AssetDatabase.GetDependencies(this.AssetPath,false);
        FileDepend fd;
        for (int i = 0; i < paths.Length; i++) {
            if (DependManager.Filter(paths[i])) {
                continue;
            }
            fd = DependManager.AddDepend(paths[i]);
            this.mUseList.Add(fd);
            fd.StartFind();
        }
    }
    public void AddUse() {
        this.UseCount++;
        if (this.UseCount > 2)
        {
            this.IsAssetBundle = true;
        }
    }
}
