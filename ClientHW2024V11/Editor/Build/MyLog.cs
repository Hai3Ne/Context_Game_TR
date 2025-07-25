using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MyLog
{
    #region Log
    static List<string> listLog = new List<string>();
    public static void ClearLog()
    {
        listLog.Clear();
    }

    public static void ExportLog(string logFileName)
    {
        StringBuilder sb = new StringBuilder("");
        foreach (var item in listLog)
        {
            sb.AppendLine(item);
        }
        //-- 保存文件
        using (FileStream file = new FileStream(logFileName + ".log", FileMode.Create, FileAccess.Write))
        {
            using (TextWriter writer = new StreamWriter(file, Encoding.UTF8))
                writer.Write(sb.ToString());
        }
    }

    public static void Log(string s)
    {
        Debug.Log(s);
    }

    public static void LogWarning(string s)
    {
        listLog.Add(s);
        Debug.LogWarning(s);
    }

    public static void LogError(string s)
    {
        listLog.Add(s);
        Debug.LogError(s);
    }
    #endregion
}