using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class CleanUpDir{


    public void CleanUpDirByPath(string path)
    {
        //Debug.Log(path);
        if (Directory.Exists(path))
        {
            DirectoryInfo dirInfo = new DirectoryInfo("Assets/Resources/UI/testScene");
            List<DirectoryInfo> dirGroup = new List<DirectoryInfo>();
            dirGroup.Add(dirInfo);
            GetDiresByPath(new DirectoryInfo(path), ref dirGroup);
            int leght = dirGroup.Count;
            for (int i = 0;i<leght;i++)
            {
                if (dirGroup[i].Exists)
                {
                    //Debug.Log(dirGroup[i].Name);
                    EditorUtility.DisplayProgressBar("CleanUp <__ori> File", dirGroup[i].FullName, i / leght);
                    dirGroup[i].Delete(true);
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }


    void GetDiresByPath(DirectoryInfo di, ref List<DirectoryInfo> result)
    {
        DirectoryInfo[] dics = di.GetDirectories();
        foreach (var file in dics)
        {
            if (file.Name == "__ori")
                result.Add(file);
        }
        if (dics.Length > 0)
        {
            foreach (DirectoryInfo sdi in dics)
            {
                GetDiresByPath(sdi, ref result);
            }
        }
    }
}
