using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class DeletFbxDefaultMat
{
    private static Dictionary<string, GameObject> fbxs = new Dictionary<string, GameObject>();
    public static bool reimportModle = false;


    static void GetModel(string path)
    {
        string pathLower = path.ToLower();
        if (pathLower.EndsWith(".fbx") || pathLower.EndsWith(".3ds") || pathLower.EndsWith(".obj"))
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go != null)
            {
                fbxs.Add(path, go);
            }
        }
    }

    static void StartDelete()
    {
        reimportModle = true;
        int n = 0;
        foreach (var item in fbxs)
        {
            UpdateProgress(++n, fbxs.Count, item.Key);
            AssetDatabase.ImportAsset(item.Key);
        }

        reimportModle = false;
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        Debug.Log("delete default mat over---");
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }
}

public class ModelMatTool : AssetPostprocessor
{
    private void OnPostprocessModel(GameObject model)
    {
        if (null == model || !DeletFbxDefaultMat.reimportModle) return;

        Renderer[] renders = model.GetComponentsInChildren<Renderer>();
        if (null == renders) return;
        foreach (Renderer render in renders)
        {
            render.sharedMaterials = new Material[render.sharedMaterials.Length];
        }
    }
}