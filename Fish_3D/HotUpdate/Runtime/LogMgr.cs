//#define LOG_IN_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine.Profiling;

public static class LogMgr
{
	public static long AppInstanceID = 1;
	public static bool ShowLog = true;

	static string DebugDataPath;
	static  StringBuilder mLogConentBuffer = new StringBuilder();
	static  StringBuilder mExceptionLogConentBuffer = new StringBuilder();
	static int logRowCnt = 0,excepCnt=0;

	static Thread writeLogThread = null;
	static bool isRunThreading = false;
	static uint LogIntervalTime = 20, writeTime = 0;
	static int maxBufferlength = 10000;
	public static int CollectMemeoryInfoTimes = 0;

    public static string LogContent { get { return mLogConentBuffer.ToString(); } }
    public static string LogErrorContent { get { return mExceptionLogConentBuffer.ToString(); } }
	public static void Init(string savepath)
	{
		#if !UNITY_EDITOR || LOG_IN_EDITOR
		DebugDataPath = savepath+"/logs";
		CollectMemeoryInfoTimes = 0;
		Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
        writeLogThread = new Thread (WriteLogLoop);
		writeLogThread.IsBackground = true;
        writeLogThread.Start ();
		isRunThreading = true;
		LogMemeroyConsueInfo ();

		if (!Directory.Exists(DebugDataPath)){
			Directory.CreateDirectory(DebugDataPath);
		}
		if (Directory.GetFiles(DebugDataPath).Length > 20){
			Directory.Delete(DebugDataPath, true);
			Directory.CreateDirectory(DebugDataPath);
		}
		#endif
	}
    
    static void WriteLogLoop() {
        while (isRunThreading) {
            writeTime += 1;
            if (mLogConentBuffer.Length > maxBufferlength || writeTime > LogIntervalTime) {
                SaveLogMessage();
                writeTime = 0;
            }
            Thread.Sleep(500);
        }
    }
	static void ClearLog(){
		mLogConentBuffer = new StringBuilder();
        mExceptionLogConentBuffer = new StringBuilder();
		logRowCnt = 0;
		excepCnt = 0;
	}

	public static void Log (object info)
    {
        if (ShowLog) {
//#if UNITY_EDITOR && !LOG_IN_EDITOR
			Debug.Log (info);
/*#else
			mLogConentBuffer.AppendLine (info.ToString ());
			logRowCnt ++;
#endif*/
        }

	}

	public static void LogError (object info){
		//if (ShowLog) {
		Debug.LogError(info);
/*#if UNITY_EDITOR && !LOG_IN_EDITOR
		Debug.LogError (info);
#else
#if UNITY_IOS
            SDKMgr.Instance.PrintLog("[Error]", info.ToString());
#endif
		    mLogConentBuffer.AppendLine ("[Error]" + info);
		    logRowCnt ++;
#endif*/
        //}
	}

	public static void LogWarning (object info){
		//if (ShowLog) {
		Debug.LogWarning(Time.realtimeSinceStartup + ":" + info);
/*#if UNITY_EDITOR && !LOG_IN_EDITOR
		Debug.LogWarning (Time.realtimeSinceStartup + ":" + info);
#else
		    mLogConentBuffer.AppendLine ("[Warn]" + info);
		    logRowCnt ++;
#endif*/
        //}
	}


	public static void OnAppQuit (){
#if !UNITY_EDITOR || LOG_IN_EDITOR
		isRunThreading = false;
		SaveLogMessage ();
	#endif
	}

	public static void SaveDebugData(string fn, byte[] buff, int offset, int length)
	{
		string path = DebugDataPath + "/" + fn;
		byte[] buffer = new byte[length];
		Array.Copy (buff, offset, buffer,0, length);
		System.IO.File.WriteAllBytes (path, buffer);
	}

	static void Application_logMessageReceivedThreaded (string condition, string stackTrace, LogType type)
	{
        if (type == LogType.Exception) {
#if UNITY_IOS
            SDKMgr.Instance.PrintLog("[Error]", condition + "\n:" + stackTrace);
#endif
			mExceptionLogConentBuffer.AppendLine (condition + " stackTrace:" + stackTrace);
			excepCnt++;
		}	
	}


	static string FormatMSize(uint size){
		uint m = size / 1024 / 1024;
		uint k = (size - m * 1024 * 1024) / 1024;
		return string.Format ("{0}.{1} M", m, Math.Round(k*1000.0f/1024));
	}

	static string logPath = null, errLogPath = null;
	static void SaveLogMessage(){
		if (logPath == null)
			logPath = DebugDataPath + "/log_"+ System.DateTime.Now.ToString ("yyyy-M-d_HH-mm-ss") + ".log";
		if (errLogPath == null)
			errLogPath = DebugDataPath + "/errorLog_" + System.DateTime.Now.ToString ("yyyy-M-d_HH-mm-ss") + ".log";

		if (mLogConentBuffer.Length > 0) {
			CollectMemeoryInfoTimes++;
            File.AppendAllText(logPath,mLogConentBuffer.ToString ());
            //using (FileStream fs = new FileStream (logPath, FileMode.Append)) {
            //    byte[] buffer = Encoding.UTF8.GetBytes (mLogConentBuffer.ToString ());
            //    fs.Seek (0, SeekOrigin.End);
            //    fs.Write (buffer, 0, buffer.Length);
            //    fs.Flush ();
            //}
            mLogConentBuffer = new StringBuilder();
			logRowCnt = 0;
		}

        if (mExceptionLogConentBuffer.Length > 0) {
            File.AppendAllText(errLogPath, mExceptionLogConentBuffer.ToString());
            //using (FileStream fs = new FileStream (errLogPath, FileMode.Append)) {
            //    byte[] buffer = Encoding.UTF8.GetBytes (mExceptionLogConentBuffer.ToString ());
            //    fs.Seek (0, SeekOrigin.End);
            //    fs.Write (buffer, 0, buffer.Length);
            //    fs.Flush ();
            //}
            mExceptionLogConentBuffer = new StringBuilder();
			excepCnt = 0;
		}
	}

	public static void LogMemeroyConsueInfo(){		
		string str = "*********||*********"
            + "\nMonoUsed Memeroy:" + FormatMSize((uint)Profiler.GetMonoUsedSizeLong())
            + "\nMonoHeap Memeroy:" + FormatMSize((uint)Profiler.GetMonoHeapSizeLong())
            + "\nAllowcated Memory: " + FormatMSize((uint)Profiler.GetTotalAllocatedMemoryLong())
            + "\nReserved Memory:" + FormatMSize((uint)Profiler.GetTotalReservedMemoryLong())
            + "\nUnReserved Memory: " + FormatMSize((uint)Profiler.GetTotalUnusedReservedMemoryLong())
			+"\n*********||*********\n";
		LogMgr.Log (str);
	}

	public static void tryUploadLog(){
		string fn = null;
		string content = null;

		if (!string.IsNullOrEmpty (logPath) && File.Exists(logPath)) {
			fn = logPath.Replace (DebugDataPath + "/", "");
			content = File.ReadAllText (logPath);
			uploadLog (fn, content, null);
		}

		if (!string.IsNullOrEmpty (errLogPath) && File.Exists(errLogPath)) {
			fn = errLogPath.Replace (DebugDataPath + "/", "");
			content = File.ReadAllText (errLogPath);
			uploadLog (fn, content, null);
		}
	}

	static void uploadLog(string fn, string content, System.Action cb){
		if (string.IsNullOrEmpty (GameParams.Instance.loguploadurl))
			return;
		SDKMgr.Instance.PrintLog (fn, content);
		string url = GameParams.Instance.loguploadurl + "?filename=" + fn;
		WWWForm wForm = new WWWForm ();
		wForm.AddBinaryData ("content", System.Text.UTF8Encoding.UTF8.GetBytes(content));
		WWW w = new WWW (url, wForm);
		MonoDelegate.Instance.SendWWWRequest (w, delegate() {
			Debug.Log(w.text);
			cb.TryCall();
		});
		Debug.Log ("update..");
	}
}