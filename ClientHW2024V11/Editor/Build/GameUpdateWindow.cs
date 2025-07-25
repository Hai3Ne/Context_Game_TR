using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class GameUpdateWindow : EditorWindow
{
    static string TORTOISEPROC_PATH = "C:/Program Files/TortoiseSVN/bin/TortoiseProc.exe";

    [MenuItem("BuildAppEditeTools/SVN/Update Copy", false, 110)]
    public static void ShowWindow()
    {
        Rect rect = new Rect(0, 0, 300, 200);
        GameUpdateWindow window = (GameUpdateWindow)EditorWindow.GetWindowWithRect(typeof(GameUpdateWindow), rect, true, "Update Copy");
        window.Show();
    }

    private static void processCommand(string command, string argument)
    {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = true;

        if (start.UseShellExecute)
        {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        }
        else
        {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }

        Process p = Process.Start(start);

        p.WaitForExit();
        p.Close();
    }

    void OnGUI()
    {
        GUILayout.Label("Develop Update");
        if (GUILayout.Button("Config"))
        {
            string strWorkDirOld = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(strWorkDirOld + "/UtilsDevelopResourceCopy");
            Process p = Process.Start(Directory.GetCurrentDirectory() + "/Copy_Config.bat");
            p.WaitForExit();
            p.Close();
            Directory.SetCurrentDirectory(strWorkDirOld);
        }

        if (GUILayout.Button("All Res"))
        {
            string strWorkDirOld = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(strWorkDirOld + "/UtilsDevelopResourceCopy");

            Process p = null;

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                p = Process.Start(Directory.GetCurrentDirectory() + "/Copy_Android.bat");
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                p = Process.Start(Directory.GetCurrentDirectory() + "/Copy_IOS.bat");
            }
            else
            {
                p = Process.Start(Directory.GetCurrentDirectory() + "/Copy_Widnows.bat");
            }
            p.WaitForExit();
            p.Close();
            Directory.SetCurrentDirectory(strWorkDirOld);
        }
        GUILayout.Space(20);

        GUILayout.Label("Project SVN");
        if (GUILayout.Button("Update All"))
        {
            string path = Directory.GetCurrentDirectory() + "/";
            processCommand(TORTOISEPROC_PATH, "/command:update /path:" + path + " /closeonend:4");
        }

        if (GUILayout.Button("Commit All..."))
        {
            string path = Directory.GetCurrentDirectory() + "/";
            processCommand(TORTOISEPROC_PATH, "/command:commit /path:" + path + " /closeonend:4");
        }
    }

    [MenuItem("BuildAppEditeTools/SVN/SVN Show Log", false, 111)]
    static void SvnShowLog()
    {
        string assetPath = "";
        assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(assetPath) != "")
        {
            assetPath = Path.GetDirectoryName(assetPath);
        }
        else
        {
            assetPath = Application.dataPath;
        }

        processCommand(TORTOISEPROC_PATH, "/command:log /path:" + assetPath + " /closeonend:4");
    }

    [MenuItem("BuildAppEditeTools/SVN/SVN Update", false, 112)]
    static void SvnUpdate()
    {
        string assetPath = "";
        assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(assetPath) != "")
        {
            assetPath = Path.GetDirectoryName(assetPath);
        }
        else
        {
            assetPath = Application.dataPath;
        }

        processCommand(TORTOISEPROC_PATH, "/command:update /path:" + assetPath + " /closeonend:4");
    }

    [MenuItem("BuildAppEditeTools/SVN/SVN Commit...", false, 113)]
    static void SvnCommit()
    {
        string assetPath = "";
        assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (Path.GetExtension(assetPath) != "")
        {
            assetPath = Path.GetDirectoryName(assetPath);
        }
        else
        {
            assetPath = Application.dataPath;
        }

        processCommand(TORTOISEPROC_PATH, "/command:commit /path:" + assetPath + " /closeonend:4");
    }
}
