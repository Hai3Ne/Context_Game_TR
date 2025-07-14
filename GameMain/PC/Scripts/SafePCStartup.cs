using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class SafePCStartup : MonoBehaviour
{
    [Header("PC Safety Settings")]
    public bool enablePCLogging = true;
    public bool disableUnityDefaultLogging = true;
    public bool enableFileConflictPrevention = true;
    public float startupDelay = 0.5f;
    
    [Header("Debug Settings")]
    public bool verboseLogging = false;
    public bool enablePerformanceMonitoring = true;
    
    private static bool isInitialized = false;
    private bool initializationCompleted = false;
    private bool hasInitializationErrors = false;
    private string initializationErrors = "";
    private int initializationStep = 0;
    private const int totalInitSteps = 4;

    void Awake()
    {
        // Prevent multiple initializations
        if (isInitialized)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        #if !UNITY_EDITOR && (UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX)
        StartCoroutine(SafeInitializePC());
        #else
        StartCoroutine(InitializeNormal());
        #endif
        
        isInitialized = true;
    }
    
    IEnumerator SafeInitializePC()
    {
        Debug.Log("[SafePCStartup] Starting PC-safe initialization...");
        
        // Wait a bit to let Unity fully initialize
        yield return new WaitForSeconds(startupDelay);
        
        // Step 1: Setup custom logging system
        if (enablePCLogging)
        {
            bool step1Success = false;
            yield return StartCoroutine(InitializePCLoggingSafe(success => step1Success = success));
            UpdateInitializationProgress(1, step1Success, "PCLogging");
        }
        else
        {
            UpdateInitializationProgress(1, true, "PCLogging (Skipped)");
        }
        
        // Step 2: Disable Unity default logging to prevent conflicts
        if (disableUnityDefaultLogging)
        {
            bool step2Success = false;
            yield return StartCoroutine(DisableUnityDefaultLoggingSafe(success => step2Success = success));
            UpdateInitializationProgress(2, step2Success, "UnityLoggingDisable");
        }
        else
        {
            UpdateInitializationProgress(2, true, "UnityLoggingDisable (Skipped)");
        }
        
        // Step 3: Setup file conflict prevention
        if (enableFileConflictPrevention)
        {
            bool step3Success = false;
            yield return StartCoroutine(SetupFileConflictPreventionSafe(success => step3Success = success));
            UpdateInitializationProgress(3, step3Success, "FileConflictPrevention");
        }
        else
        {
            UpdateInitializationProgress(3, true, "FileConflictPrevention (Skipped)");
        }
        
        // Step 4: Initialize game systems safely
        bool step4Success = false;
        yield return StartCoroutine(InitializeGameSystemsSafe(success => step4Success = success));
        UpdateInitializationProgress(4, step4Success, "GameSystems");
        
        // Final result
        if (hasInitializationErrors)
        {
            Debug.LogWarning($"[SafePCStartup] Initialization completed with warnings: {initializationErrors}");
            yield return StartCoroutine(FallbackInitializationSafe());
        }
        else
        {
            initializationCompleted = true;
            PCLogManager.LogInfo("[SafePCStartup] PC initialization completed successfully");
        }
        
        // Start performance monitoring if enabled
        if (enablePerformanceMonitoring)
        {
            StartCoroutine(PerformanceMonitoringLoop());
        }
    }
    
    private void UpdateInitializationProgress(int step, bool success, string systemName)
    {
        initializationStep = step;
        
        if (!success)
        {
            hasInitializationErrors = true;
            initializationErrors += $"{systemName} failed; ";
            Debug.LogWarning($"[SafePCStartup] Step {step}/{totalInitSteps} failed: {systemName}");
        }
        else if (verboseLogging)
        {
            Debug.Log($"[SafePCStartup] Step {step}/{totalInitSteps} completed: {systemName}");
        }
    }
    
    IEnumerator InitializePCLoggingSafe(System.Action<bool> onComplete)
    {
        bool success = InitializePCLoggingSync();
        yield return new WaitForEndOfFrame();
        
        if (success && verboseLogging)
        {
            PCLogManager.LogInfo("PC Logging system initialized successfully");
        }
        
        onComplete(success);
    }
    
    private bool InitializePCLoggingSync()
    {
        try 
        {
            // Ensure PCLogManager is initialized
            var logManager = PCLogManager.Instance;
            if (logManager != null)
            {
                return true;
            }
            else
            {
                Debug.LogError("Failed to initialize PCLogManager - Instance is null");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"PC Logging initialization failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator DisableUnityDefaultLoggingSafe(System.Action<bool> onComplete)
    {
        bool success = DisableUnityDefaultLoggingSync();
        yield return new WaitForEndOfFrame();
        
        if (success && verboseLogging)
        {
            PCLogManager.LogInfo("Unity default logging management setup successfully");
        }
        
        onComplete(success);
    }
    
    private bool DisableUnityDefaultLoggingSync()
    {
        try 
        {
            // Create UnityLogDisabler if it doesn't exist
            if (FindObjectOfType<UnityLogDisabler>() == null)
            {
                var go = new GameObject("UnityLogDisabler");
                go.AddComponent<UnityLogDisabler>();
                DontDestroyOnLoad(go);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unity logging disable failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator SetupFileConflictPreventionSafe(System.Action<bool> onComplete)
    {
        bool directoriesSuccess = SetupSafeDirectoriesSync();
        
        if (directoriesSuccess)
        {
            bool cleanupSuccess = false;
            yield return StartCoroutine(CleanupPotentialLocksSafe(success => cleanupSuccess = success));
            
            if (cleanupSuccess && verboseLogging)
            {
                PCLogManager.LogInfo("File conflict prevention setup completed successfully");
            }
            
            onComplete(cleanupSuccess);
        }
        else
        {
            onComplete(false);
        }
    }
    
    private bool SetupSafeDirectoriesSync()
    {
        try
        {
            string[] safeDirs = {
                Path.Combine(Application.persistentDataPath, "SafeLogs"),
                Path.Combine(Application.persistentDataPath, "SafeTemp"),
                Path.Combine(Application.persistentDataPath, "SafeCache")
            };
            
            foreach (string dir in safeDirs)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create safe directories: {e.Message}");
            return false;
        }
    }
    
    IEnumerator CleanupPotentialLocksSafe(System.Action<bool> onComplete)
    {
        bool fileCleanupSuccess = PerformFileCleanupSync();
        
        if (fileCleanupSuccess)
        {
            // Force garbage collection to release any file handles
            yield return StartCoroutine(ForceGarbageCollectionSafe());
            
            if (verboseLogging)
            {
                PCLogManager.LogInfo("Potential file locks cleanup completed");
            }
        }
        
        onComplete(fileCleanupSuccess);
    }
    
    private bool PerformFileCleanupSync()
    {
        try
        {
            string persistentPath = Application.persistentDataPath;
            string[] tempPatterns = { "*.tmp", "*.temp", "*.lock" };
            int cleanedFiles = 0;
            
            foreach (string pattern in tempPatterns)
            {
                try 
                {
                    var files = Directory.GetFiles(persistentPath, pattern, SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        try 
                        {
                            File.Delete(file);
                            cleanedFiles++;
                            if (verboseLogging)
                            {
                                PCLogManager.LogInfo($"Cleaned up temp file: {Path.GetFileName(file)}");
                            }
                        }
                        catch 
                        {
                            // Ignore files we can't delete
                        }
                    }
                }
                catch 
                {
                    // Ignore pattern search errors
                }
            }
            
            if (cleanedFiles > 0)
            {
                PCLogManager.LogInfo($"Cleaned up {cleanedFiles} temporary files");
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"File cleanup failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator ForceGarbageCollectionSafe()
    {
        try
        {
            System.GC.Collect();
            new WaitForEndOfFrame();
            System.GC.WaitForPendingFinalizers();
            new WaitForEndOfFrame();
            System.GC.Collect();
            
            if (verboseLogging)
            {
                PCLogManager.LogInfo("Garbage collection completed");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Garbage collection warning: {e.Message}");
          
        }
        yield return true;
    }
    
    IEnumerator InitializeGameSystemsSafe(System.Action<bool> onComplete)
    {
        bool allSystemsOK = true;
        
        if (verboseLogging)
        {
            PCLogManager.LogInfo("Starting game systems initialization...");
        }
        
        // System 1: Audio Manager (safe to initialize early)
        bool audioSuccess = InitializeAudioManagerSync();
        if (!audioSuccess) allSystemsOK = false;
        yield return new WaitForEndOfFrame();
        
        // System 2: Input Manager
        bool inputSuccess = InitializeInputManagerSync();
        if (!inputSuccess) allSystemsOK = false;
        yield return new WaitForEndOfFrame();
        
        // System 3: Asset Management
        bool assetSuccess = InitializeAssetManagerSync();
        if (!assetSuccess) allSystemsOK = false;
        yield return new WaitForEndOfFrame();
        
        // System 4: Hot Update System (use improved version)
        bool hotUpdateSuccess = false;
        yield return StartCoroutine(InitializeHotUpdateSystemSafe(success => hotUpdateSuccess = success));
        if (!hotUpdateSuccess) allSystemsOK = false;
        
        if (allSystemsOK && verboseLogging)
        {
            PCLogManager.LogInfo("All game systems initialized successfully");
        }
        
        onComplete(allSystemsOK);
    }
    
    private bool InitializeAudioManagerSync()
    {
        // try 
        // {
        //     // Safe audio manager initialization
        //     if (FindObjectOfType<AudioManager>() == null)
        //     {
        //         var audioGO = new GameObject("AudioManager");
        //         audioGO.AddComponent<AudioManager>();
        //         DontDestroyOnLoad(audioGO);
        //     }
        //     
        //     if (verboseLogging)
        //     {
        //         PCLogManager.LogInfo("Audio Manager initialized successfully");
        //     }
        //     return true;
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError($"Audio Manager initialization failed: {e.Message}");
        //     return false;
        // }
        return true;
    }
    
    private bool InitializeInputManagerSync()
    {
        try 
        {
            // PC input manager setup - placeholder for actual implementation
            if (verboseLogging)
            {
                PCLogManager.LogInfo("Input Manager initialized successfully");
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Input Manager initialization failed: {e.Message}");
            return false;
        }
    }
    
    private bool InitializeAssetManagerSync()
    {
        try 
        {
            // Asset manager initialization - placeholder for actual implementation
            if (verboseLogging)
            {
                PCLogManager.LogInfo("Asset Manager initialized successfully");
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Asset Manager initialization failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator InitializeHotUpdateSystemSafe(System.Action<bool> onComplete)
    {
        bool success = InitializeHotUpdateSystemSync();
        yield return new WaitForEndOfFrame();
        
        if (success && verboseLogging)
        {
            PCLogManager.LogInfo("Hot Update System initialized successfully");
        }
        
        onComplete(success);
    }
    
    private bool InitializeHotUpdateSystemSync()
    {
        try 
        {
            // Use improved HotMain that handles file conflicts
            var hotMain = FindObjectOfType<HotMain>();
            if (hotMain != null)
            {
                // Check if HotMain_Improved already exists
                var existingImproved = FindObjectOfType<HotMain_Improved>();
                if (existingImproved == null)
                {
                    // Replace with improved version
                    var hotMainGO = hotMain.gameObject;
                    var improvedHotMain = hotMainGO.AddComponent<HotMain_Improved>();
                    
                    // Copy UI references if they exist
                    if (hotMain.uiLabelProgress != null)
                        improvedHotMain.uiLabelProgress = hotMain.uiLabelProgress;
                    if (hotMain.uiLabelInfo != null)
                        improvedHotMain.uiLabelInfo = hotMain.uiLabelInfo;
                    if (hotMain.uiSlider != null)
                        improvedHotMain.uiSlider = hotMain.uiSlider;
                    
                    // Remove old component
                    DestroyImmediate(hotMain);
                    
                    if (verboseLogging)
                    {
                        PCLogManager.LogInfo("Hot Update System upgraded to improved version");
                    }
                }
                else
                {
                    if (verboseLogging)
                    {
                        PCLogManager.LogInfo("HotMain_Improved already exists");
                    }
                }
            }
            else
            {
                if (verboseLogging)
                {
                    PCLogManager.LogInfo("No existing HotMain found - will be created when needed");
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Hot Update System initialization failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator FallbackInitializationSafe()
    {
        PCLogManager.LogWarning("Starting fallback initialization due to errors...");
        
        bool fallbackSuccess = PerformFallbackInitSync();
        
        if (fallbackSuccess)
        {
            yield return new WaitForSeconds(1f);
            PCLogManager.LogInfo("Fallback initialization completed");
            initializationCompleted = true; // Mark as completed despite errors
        }
        else
        {
            Debug.LogError("Even fallback initialization failed");
        }
    }
    
    private bool PerformFallbackInitSync()
    {
        try 
        {
            // Ensure basic logging works
            if (!enablePCLogging || PCLogManager.Instance == null)
            {
                Debug.LogWarning("PCLogManager not available, using Unity Debug.Log");
            }
            
            // Just try to start the game without advanced features
            var hotMain = FindObjectOfType<HotMain>();
            if (hotMain == null)
            {
                var existingHotMainImproved = FindObjectOfType<HotMain_Improved>();
                if (existingHotMainImproved == null)
                {
                    var go = new GameObject("FallbackHotMain");
                    go.AddComponent<HotMain>();
                    Debug.LogWarning("Created fallback HotMain component");
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Fallback initialization failed: {e.Message}");
            return false;
        }
    }
    
    IEnumerator InitializeNormal()
    {
        // Normal initialization for non-PC platforms
        Debug.Log("[SafePCStartup] Starting normal initialization for mobile/editor...");
        yield return new WaitForSeconds(0.1f);
        
        initializationCompleted = true;
        Debug.Log("[SafePCStartup] Normal initialization completed");
    }
    
    IEnumerator PerformanceMonitoringLoop()
    {
        while (this != null && gameObject != null)
        {
            yield return new WaitForSeconds(30f); // Check every 30 seconds
            
            PerformMemoryCheck();
        }
    }
    
    private void PerformMemoryCheck()
    {
        try
        {
            // Monitor memory usage
            long memoryUsage = System.GC.GetTotalMemory(false);
            float memoryMB = memoryUsage / (1024f * 1024f);
            
            if (memoryMB > 800f) // 800MB threshold
            {
                PCLogManager.LogWarning($"High memory usage detected: {memoryMB:F1}MB");
                
                if (memoryMB > 1200f) // 1.2GB critical threshold
                {
                    PCLogManager.LogError($"Critical memory usage: {memoryMB:F1}MB - forcing garbage collection");
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                }
            }
            else if (verboseLogging && memoryMB > 500f)
            {
                PCLogManager.LogInfo($"Memory usage: {memoryMB:F1}MB");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Performance monitoring error: {e.Message}");
        }
    }
    
    void OnApplicationQuit()
    {
        if (verboseLogging)
        {
            PCLogManager.LogInfo("[SafePCStartup] Application quitting, cleaning up...");
        }
        
        PerformFinalCleanup();
    }
    
    private void PerformFinalCleanup()
    {
        try 
        {
            // Final cleanup
            if (enableFileConflictPrevention)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
        }
        catch 
        {
            // Ignore cleanup errors during quit
        }
    }
    
    // Public API for external systems
    public bool IsInitializationCompleted()
    {
        return initializationCompleted;
    }
    
    public bool HasInitializationErrors()
    {
        return hasInitializationErrors;
    }
    
    public string GetInitializationErrors()
    {
        return initializationErrors;
    }
    
    public int GetInitializationStep()
    {
        return initializationStep;
    }
    
    public float GetInitializationProgress()
    {
        return (float)initializationStep / totalInitSteps;
    }
    
    public void ForceRetryInitialization()
    {
        if (!initializationCompleted)
        {
            Debug.Log("[SafePCStartup] Forcing retry initialization...");
            StartCoroutine(FallbackInitializationSafe());
        }
    }
    
    // Debug helper methods
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugLogSystemStatus()
    {
        Debug.Log($"[SafePCStartup] Status - Completed: {initializationCompleted}, Errors: {hasInitializationErrors}");
        Debug.Log($"[SafePCStartup] Progress: {GetInitializationProgress():P0} ({initializationStep}/{totalInitSteps})");
        Debug.Log($"[SafePCStartup] Error Details: {initializationErrors}");
        Debug.Log($"[SafePCStartup] PCLogManager exists: {PCLogManager.Instance != null}");
        Debug.Log($"[SafePCStartup] UnityLogDisabler exists: {FindObjectOfType<UnityLogDisabler>() != null}");
        Debug.Log($"[SafePCStartup] HotMain exists: {FindObjectOfType<HotMain>() != null}");
        Debug.Log($"[SafePCStartup] HotMain_Improved exists: {FindObjectOfType<HotMain_Improved>() != null}");
    }
    
    // Runtime diagnostics
    public SystemDiagnostics GetSystemDiagnostics()
    {
        return new SystemDiagnostics
        {
            isCompleted = initializationCompleted,
            hasErrors = hasInitializationErrors,
            errorDetails = initializationErrors,
            currentStep = initializationStep,
            totalSteps = totalInitSteps,
            memoryUsageMB = System.GC.GetTotalMemory(false) / (1024f * 1024f),
            pcLogManagerExists = PCLogManager.Instance != null,
            unityLogDisablerExists = FindObjectOfType<UnityLogDisabler>() != null,
            hotMainExists = FindObjectOfType<HotMain>() != null,
            hotMainImprovedExists = FindObjectOfType<HotMain_Improved>() != null
        };
    }
    
    [System.Serializable]
    public struct SystemDiagnostics
    {
        public bool isCompleted;
        public bool hasErrors;
        public string errorDetails;
        public int currentStep;
        public int totalSteps;
        public float memoryUsageMB;
        public bool pcLogManagerExists;
        public bool unityLogDisablerExists;
        public bool hotMainExists;
        public bool hotMainImprovedExists;
        
        public override string ToString()
        {
            return $"SafePCStartup Diagnostics:\n" +
                   $"- Completed: {isCompleted}\n" +
                   $"- Has Errors: {hasErrors}\n" +
                   $"- Progress: {currentStep}/{totalSteps}\n" +
                   $"- Memory: {memoryUsageMB:F1}MB\n" +
                   $"- PCLogManager: {pcLogManagerExists}\n" +
                   $"- UnityLogDisabler: {unityLogDisablerExists}\n" +
                   $"- HotMain: {hotMainExists}\n" +
                   $"- HotMain_Improved: {hotMainImprovedExists}\n" +
                   $"- Errors: {errorDetails}";
        }
    }
}