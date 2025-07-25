using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyDiffData
{
    public string name;
    public string md5; 
}

public class CopyDiffFile 
{
   
    static string path1 = "D:/cn/ClientHW2024V8/AssetsBundle/10001";
    static string path2 = "D:/cn/ClientHW2024V8/AssetsBundle/1000";
    static string path3 = "D:/cn/ClientHW2024V8/AssetsBundle/out/";

    static Dictionary<string, CopyDiffData> Dic1 = new Dictionary<string, CopyDiffData>();
    static Dictionary<string, CopyDiffData> Dic2 = new Dictionary<string, CopyDiffData>();

    [MenuItem("文件差异拷贝/copy", false, 16)]
    static public void copyFile()
    {
        Dic1.Clear();
        Dic2.Clear();

        files.Clear();
        paths.Clear();
        Recursive(path1);
        for (int i = 0; i < files.Count; i++)
        {
            var name = files[i].Replace(path1 + "/", "");
            CopyDiffData data= new CopyDiffData();
            string md5 = Util.md5file(files[i]);
            data.md5 = md5;
            data.name = files[i];
            Dic1.Add(name, data);
        }
        files.Clear();
        paths.Clear();
        Recursive(path2);
        for (int i = 0; i < files.Count; i++)
        {
            var name = files[i].Replace(path2 + "/", "");
            CopyDiffData data = new CopyDiffData();
            string md5 = Util.md5file(files[i]);
            data.md5 = md5;
            data.name = files[i];
            Dic2.Add(name, data);
        }

        var list = new List<string>();
        foreach (var item in Dic2)
        {
            if (Dic1.ContainsKey(item.Key))
            {
                if (Dic1[item.Key].md5 != item.Value.md5)
                {
                    
                    list.Add(item.Value.name);
                }
            }
            else
            {
                list.Add(item.Value.name);
            }
        }

        if (Directory.Exists(path3))
            Directory.Delete(path3,true);
        foreach (var item in list)
        {
            var name = item.Replace(path2 + "/","");

            var dir = Path.GetDirectoryName(path3 + name);
            DirectoryInfo dirOutDir = new DirectoryInfo(dir);

            if (!dirOutDir.Exists)
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(item, path3 + name);
        }
        Debug.Log("Copy End");
    }
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
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
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }


}
