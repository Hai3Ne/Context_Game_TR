using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ClientSettingDraw {

    string ClientSettingPath { get { return Path.Combine(Application.dataPath, "ResData/Data/ClientSetting.csv"); } }

    string mMainVersion;
    int mChildVersion;

    public void OnDraw()
    {
        if (string.IsNullOrEmpty(mMainVersion))
        {
            string Version = ReadVerSion();
            int lastIndex = Version.LastIndexOf(".");
            mMainVersion = Version.Remove(lastIndex);
            mChildVersion = int.Parse( Version.Substring(lastIndex + 1));
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("版本号:{0}.{1}", mMainVersion, mChildVersion), GUILayout.Height(30));
        if (GUILayout.Button("-", GUILayout.Height(30)))
            mChildVersion--;
        if (GUILayout.Button("+", GUILayout.Height(30)))
            mChildVersion++;
        if(GUILayout.Button("保存版本",GUILayout.Height(30)))
        {
            WriteVerSion();
            EditorUtility.DisplayDialog("", "保存成功", "确定");
        }
        GUILayout.EndHorizontal();
    }

    string ReadVerSion()
    {
        string getStr = "";
        string[] lineGroup = File.ReadAllLines(ClientSettingPath);
        for (int i = 0; i < lineGroup.Length;i++ )
        {
            var child =lineGroup[i];
            if (child.Contains("version = "))
            {
                getStr = child.Replace("version = ", "");
                break;
            }
        }
        return getStr;
    }

    void WriteVerSion()
    {
        string versoinStart = "version = ";
        string[] lineGroup = File.ReadAllLines(ClientSettingPath);
        for (int i = 0; i < lineGroup.Length; i++)
        {
            var child = lineGroup[i];
            if (child.Contains(versoinStart))
            {
                string newstr = string.Format("version = {0}.{1}",mMainVersion,mChildVersion);
                lineGroup[i] = newstr;
                PlayerSettings.bundleVersion = newstr.Replace(versoinStart,"");
                break;
            }
        }
        File.WriteAllLines(ClientSettingPath, lineGroup);
        
    }
}
