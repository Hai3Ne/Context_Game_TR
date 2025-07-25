using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class ApplicationPackageNameSwith {


    string BaseAndroidPakName = "com.hoolai.yzsg.mi";

    public void SwithPackageName(string targetFilePath)
    {
        string fileStr = File.ReadAllText(targetFilePath);
        fileStr = fileStr.Replace(BaseAndroidPakName, PlayerSettings.applicationIdentifier);
        File.Delete(targetFilePath);
        File.WriteAllText(targetFilePath, fileStr,System.Text.Encoding.ASCII);
    }


}
