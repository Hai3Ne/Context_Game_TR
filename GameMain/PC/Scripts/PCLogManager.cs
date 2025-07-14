using System.IO;
using UnityEngine;
using System;

public class PCLogManager : MonoBehaviour 
{
    private static PCLogManager instance;
    private string customLogPath;
    private StreamWriter logWriter;
    
    public static PCLogManager Instance 
    {
        get 
        {
            if (instance == null) 
            {
                GameObject go = new GameObject("PCLogManager");
                instance = go.AddComponent<PCLogManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCustomLogging();
        } 
        else if (instance != this) 
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeCustomLogging() 
    {
        try 
        {
            // Tạo custom log path để tránh conflict với Unity's Player.log
            string logDir = Path.Combine(Application.persistentDataPath, "GameLogs");
            if (!Directory.Exists(logDir)) 
            {
                Directory.CreateDirectory(logDir);
            }
            
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            customLogPath = Path.Combine(logDir, $"GameLog_{timestamp}.txt");
            
            // Tạo custom log writer
            logWriter = new StreamWriter(customLogPath, true);
            logWriter.AutoFlush = true;
            
            // Subscribe to Unity log events
            Application.logMessageReceived += OnLogMessageReceived;
            
            WriteLog("=== Game Started ===", LogType.Log);
            WriteLog($"Platform: {Application.platform}", LogType.Log);
            WriteLog($"Unity Version: {Application.unityVersion}", LogType.Log);
            WriteLog($"Log Path: {customLogPath}", LogType.Log);
        } 
        catch (Exception e) 
        {
            Debug.LogError($"Failed to initialize custom logging: {e.Message}");
        }
    }
    
    void OnLogMessageReceived(string logString, string stackTrace, LogType type) 
    {
        WriteLog(logString, type, stackTrace);
    }
    
    public void WriteLog(string message, LogType logType = LogType.Log, string stackTrace = "") 
    {
        if (logWriter == null) return;
        
        try 
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logTypeStr = GetLogTypeString(logType);
            
            logWriter.WriteLine($"[{timestamp}] [{logTypeStr}] {message}");
            
            if (!string.IsNullOrEmpty(stackTrace) && (logType == LogType.Error || logType == LogType.Exception)) 
            {
                logWriter.WriteLine($"Stack Trace: {stackTrace}");
            }
        } 
        catch (Exception e) 
        {
            // Nếu không thể write log, ít nhất output to console
            Console.WriteLine($"Log write failed: {e.Message}");
        }
    }
    
    private string GetLogTypeString(LogType logType) 
    {
        switch (logType) 
        {
            case LogType.Error: return "ERROR";
            case LogType.Assert: return "ASSERT";
            case LogType.Warning: return "WARNING";
            case LogType.Log: return "INFO";
            case LogType.Exception: return "EXCEPTION";
            default: return "UNKNOWN";
        }
    }
    
    void OnApplicationPause(bool pauseStatus) 
    {
        WriteLog($"Application Pause: {pauseStatus}", LogType.Log);
    }
    
    void OnApplicationFocus(bool hasFocus) 
    {
        WriteLog($"Application Focus: {hasFocus}", LogType.Log);
    }
    
    void OnApplicationQuit() 
    {
        WriteLog("=== Game Closing ===", LogType.Log);
        CleanupLogging();
    }
    
    void OnDestroy() 
    {
        CleanupLogging();
    }
    
    private void CleanupLogging() 
    {
        try 
        {
            if (logWriter != null) 
            {
                Application.logMessageReceived -= OnLogMessageReceived;
                logWriter.Close();
                logWriter.Dispose();
                logWriter = null;
            }
        } 
        catch (Exception e) 
        {
            Console.WriteLine($"Cleanup logging failed: {e.Message}");
        }
    }
    
    // Public methods for external logging
    public static void LogInfo(string message) 
    {
        Instance.WriteLog(message, LogType.Log);
    }
    
    public static void LogWarning(string message) 
    {
        Instance.WriteLog(message, LogType.Warning);
    }
    
    public static void LogError(string message) 
    {
        Instance.WriteLog(message, LogType.Error);
    }
    
    public static string GetLogFilePath() 
    {
        return Instance.customLogPath;
    }
}