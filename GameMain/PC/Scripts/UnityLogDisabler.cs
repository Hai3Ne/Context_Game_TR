using UnityEngine;
using System.IO;
using System;

public class UnityLogDisabler : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void DisableUnityLoggingOnPC()
    {
        // Chỉ disable logging trên PC build, giữ nguyên cho Editor
        #if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX)
        
        try 
        {
            // Disable Unity's default logging to avoid file conflicts
            Debug.unityLogger.logEnabled = false;
            
            // Redirect Unity logs to our custom logger
            Application.logMessageReceived += OnLogMessageReceived;
            
            // Set log level to minimum to reduce file I/O
            Debug.unityLogger.filterLogType = LogType.Exception;
            
            Debug.Log("[UnityLogDisabler] Unity default logging disabled for PC build");
        }
        catch (Exception e)
        {
            // If we can't disable logging, at least try to log the error
            Console.WriteLine($"Failed to disable Unity logging: {e.Message}");
        }
        
        #endif
    }
    
    private static void OnLogMessageReceived(string logString, string stackTrace, LogType type)
    {
        // Redirect Unity logs to our custom PC logger
        if (PCLogManager.Instance != null)
        {
            PCLogManager.Instance.WriteLog($"[Unity] {logString}", type, stackTrace);
        }
        else
        {
            // Fallback to console if PC logger not available
            Console.WriteLine($"[{type}] {logString}");
            if (!string.IsNullOrEmpty(stackTrace))
            {
                Console.WriteLine($"Stack: {stackTrace}");
            }
        }
    }
    
    void Awake()
    {
        // Ensure this object persists across scenes
        DontDestroyOnLoad(gameObject);
        
        #if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX)
        
        // Additional PC-specific log handling
        SetupPCLogRedirection();
        
        #endif
    }
    
    void SetupPCLogRedirection()
    {
        try 
        {
            // Try to prevent Unity from creating/accessing Player.log
            string logPath = Path.Combine(Application.dataPath, "../Player.log");
            string altLogPath = Path.Combine(Application.persistentDataPath, "Player.log");
            
            // Create dummy files to prevent Unity from using them
            CreateDummyLogFile(logPath);
            CreateDummyLogFile(altLogPath);
            
            Debug.Log("[UnityLogDisabler] PC log redirection setup complete");
        }
        catch (Exception e)
        {
            PCLogManager.LogError($"SetupPCLogRedirection failed: {e.Message}");
        }
    }
    
    void CreateDummyLogFile(string path)
    {
        try 
        {
            if (!File.Exists(path))
            {
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                // Create a small dummy file
                File.WriteAllText(path, "// Dummy log file to prevent Unity conflicts\n");
                
                // Set file attributes to read-only if possible
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.Attributes |= FileAttributes.ReadOnly;
            }
        }
        catch 
        {
            // Silently ignore if we can't create dummy file
        }
    }
    
    void OnApplicationQuit()
    {
        #if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX)
        
        try 
        {
            // Re-enable Unity logging before quit
            Debug.unityLogger.logEnabled = true;
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        catch 
        {
            // Ignore errors during cleanup
        }
        
        #endif
    }
}