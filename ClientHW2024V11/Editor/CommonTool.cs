using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using SEZSJ;

public class FormatWindow : EditorWindow
{

	[MenuItem("Assets/改变目录下所有图片的格式")]
    public static void Init()
    {

        FormatWindow formatWindow = EditorWindow.GetWindow<FormatWindow>();
        formatWindow.maxSize = new Vector2(250, 100);
        formatWindow.minSize = new Vector2(250,100);
    }

    static TextureImporterFormat tif = TextureImporterFormat.DXT1;
    static bool genMipMap = false;
    void OnGUI()
    {

        tif = (TextureImporterFormat)EditorGUILayout.EnumPopup("图片格式", tif);
        genMipMap = EditorGUILayout.Toggle("MipMap", genMipMap);

        if (GUILayout.Button("确定"))
        {
            foreach (string filePath in GetAllTextures())
            {
                TextureImporter ti = AssetImporter.GetAtPath(filePath) as TextureImporter;
                if (ti != null)
                {
                    ti.textureType = TextureImporterType.Default;
                    ti.textureFormat = tif;
                    ti.mipmapEnabled = false;
                    AssetDatabase.ImportAsset(filePath);
                }
            }

            Close();
        }
    }


    static string[] GetFiles(string path, string pattern)
    {
        List<string> files = new List<string>();
        string[] dirs = Directory.GetDirectories(path);
        foreach (var dir in dirs)
            files.AddRange(GetFiles(dir, pattern));
        files.AddRange(Directory.GetFiles(path, pattern));
        return files.ToArray();
    }

    public static string[] GetAllTextures()
    {
        List<string> paths = new List<string>();

        Object[] objs = Selection.objects;
        if (objs == null || objs.Length < 1)
        {
            LogMgr.LogError("请选中图片或者图片目录");
            return new string[0];
        }

        HashSet<string> dirs = new HashSet<string>();
        foreach (Object o in objs)
        {
            string path = AssetDatabase.GetAssetPath(o);
            if (Directory.Exists(path))
            {
                dirs.Add(path);
            }
            else
            {
                string t = Path.GetDirectoryName(path);
                if (t != null) dirs.Add(t);
            }
        }

        HashSet<string> filePaths = new HashSet<string>();

        foreach (string dir in dirs)
        {
            string[] files = GetFiles(dir, "*");
            foreach (string file in files)
                filePaths.Add(file);
        }

        foreach (string filePath in filePaths)
        {
            TextureImporter ti = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (ti != null)
                paths.Add(filePath);
        }

        return paths.ToArray();
    }
}


