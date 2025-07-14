using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

public class HotMain_Improved : MonoBehaviour
{
    [Header("UI References")]
    public UILabel uiLabelProgress, uiLabelInfo;
    public UISlider uiSlider;
    
    [Header("Download Settings")]
    [SerializeField] private int maxConcurrentDownloads = 3;
    [SerializeField] private float downloadTimeoutSeconds = 45f;
    [SerializeField] private int maxRetryAttempts = 5;
    [SerializeField] private float baseRetryDelay = 0.5f;
    
    // Progress tracking
    private string mLoadingInfo = "";
    private float mProgress = 0f;
    private bool isUpdate = false;
    private long mTotalSize = 0;
    private long mCurSize = 0;
    private long mlastSize = 0;
    
    // Version tracking
    private string DllVersion = "";
    private string resVersion = "";
    private string DllAsset = "";
    private string resAsset = "";
    
    // Concurrency control
    private int activeDownloads = 0;
    private readonly object downloadCountLock = new object();
    private readonly Dictionary<string, string> md5Cache = new Dictionary<string, string>();
    
    // File operation safety
    private readonly object fileOperationLock = new object();
    private readonly HashSet<string> lockedFiles = new HashSet<string>();
    
    // Error tracking
    private bool hasErrors = false;
    private string lastError = "";
    private List<string> errorLog = new List<string>();

    void Start()
    {
        PCLogManager.LogInfo("[HotMain_Improved] Starting enhanced hot update system...");
        StartCoroutine(CheckFirstSafe());
    }

    private IEnumerator CheckFirstSafe()
    {
        PCLogManager.LogInfo("Starting first-time check process");
        
        var versionPath = GetSafeFilePath(Application.persistentDataPath + "/resversion.ver");
        
        if (SafeFileExists(versionPath))
        {
            var str = SafeReadAllText(versionPath);
            if (str != Application.version)
            {
                PCLogManager.LogInfo("Version mismatch detected, copying streaming assets");
                
                bool copySuccess = false;
                yield return StartCoroutine(CopyStreamingAssetsFileSafe(result => copySuccess = result));
                
                if (copySuccess)
                {
                    yield return StartCoroutine(CheckUpdateSafe());
                }
                else
                {
                    yield return StartCoroutine(HandleError("StreamingAssets copy failed", true));
                }
            }
            else
            {
                PCLogManager.LogInfo("Version match, checking for updates");
                yield return StartCoroutine(CheckUpdateSafe());
            }
        }
        else
        {
            PCLogManager.LogInfo("No version file found, performing first-time setup");
            
            bool copySuccess = false;
            yield return StartCoroutine(CopyStreamingAssetsFileSafe(result => copySuccess = result));
            
            if (copySuccess)
            {
                yield return StartCoroutine(CheckUpdateSafe());
            }
            else
            {
                yield return StartCoroutine(HandleError("First-time setup failed", true));
            }
        }
    }

    private string GetSafeFilePath(string originalPath)
    {
        // Ensure the path doesn't conflict with Unity's internal files
        if (originalPath.Contains("Player.log") || originalPath.ToLower().Contains("unity"))
        {
            string dir = Path.GetDirectoryName(originalPath);
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            return Path.Combine(dir, $"safe_{filename}{extension}");
        }
        return originalPath;
    }

    private bool SafeFileExists(string path)
    {
        lock (fileOperationLock)
        {
            if (lockedFiles.Contains(path))
            {
                PCLogManager.LogWarning($"File is currently locked: {Path.GetFileName(path)}");
                return false;
            }
            
            try
            {
                return File.Exists(path);
            }
            catch (Exception e)
            {
                RecordError($"SafeFileExists failed for {Path.GetFileName(path)}: {e.Message}");
                return false;
            }
        }
    }

    private string SafeReadAllText(string path)
    {
        int retryCount = maxRetryAttempts;
        float delay = baseRetryDelay;
        
        while (retryCount > 0)
        {
            lock (fileOperationLock)
            {
                if (lockedFiles.Contains(path))
                {
                    Thread.Sleep((int)(delay * 1000));
                    retryCount--;
                    delay *= 1.5f;
                    continue;
                }
                
                lockedFiles.Add(path);
                
                try
                {
                    using (var reader = new StreamReader(path, System.Text.Encoding.UTF8))
                    {
                        string result = reader.ReadToEnd();
                        lockedFiles.Remove(path);
                        return result;
                    }
                }
                catch (IOException ioEx) when (ioEx.Message.Contains("being used by another process"))
                {
                    lockedFiles.Remove(path);
                    PCLogManager.LogWarning($"File in use, retrying: {Path.GetFileName(path)} (attempts left: {retryCount})");
                    Thread.Sleep((int)(delay * 1000));
                    retryCount--;
                    delay *= 1.5f;
                }
                catch (Exception e)
                {
                    lockedFiles.Remove(path);
                    RecordError($"SafeReadAllText failed for {Path.GetFileName(path)}: {e.Message}");
                    break;
                }
            }
        }
        return "";
    }

    private bool SafeWriteAllText(string path, string content)
    {
        return SafeWriteAllBytes(path, System.Text.Encoding.UTF8.GetBytes(content));
    }

    private bool SafeWriteAllBytes(string path, byte[] data)
    {
        int retryCount = maxRetryAttempts;
        float delay = baseRetryDelay;
        
        while (retryCount > 0)
        {
            lock (fileOperationLock)
            {
                if (lockedFiles.Contains(path))
                {
                    Thread.Sleep((int)(delay * 1000));
                    retryCount--;
                    delay *= 1.5f;
                    continue;
                }
                
                lockedFiles.Add(path);
                
                try
                {
                    // Ensure directory exists
                    var dir = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    
                    // Write to temporary file first (atomic operation)
                    string tempPath = path + ".tmp_" + DateTime.Now.Ticks;
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Flush();
                    }
                    
                    // Atomic move
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Move(tempPath, path);
                    
                    lockedFiles.Remove(path);
                    PCLogManager.LogInfo($"Successfully wrote file: {Path.GetFileName(path)} ({data.Length} bytes)");
                    return true;
                }
                catch (IOException ioEx) when (ioEx.Message.Contains("being used by another process"))
                {
                    lockedFiles.Remove(path);
                    PCLogManager.LogWarning($"File locked during write, retrying: {Path.GetFileName(path)} (attempts left: {retryCount})");
                    Thread.Sleep((int)(delay * 1000));
                    retryCount--;
                    delay *= 1.5f;
                }
                catch (Exception e)
                {
                    lockedFiles.Remove(path);
                    RecordError($"SafeWriteAllBytes failed for {Path.GetFileName(path)}: {e.Message}");
                    break;
                }
            }
        }
        return false;
    }

    private void RecordError(string error)
    {
        hasErrors = true;
        lastError = error;
        errorLog.Add($"[{DateTime.Now:HH:mm:ss}] {error}");
        PCLogManager.LogError(error);
        
        // Keep only last 50 errors
        if (errorLog.Count > 50)
        {
            errorLog.RemoveAt(0);
        }
    }

    private IEnumerator HandleError(string errorMessage, bool isRecoverable)
    {
        RecordError(errorMessage);
        
        if (isRecoverable)
        {
            PCLogManager.LogWarning($"Attempting error recovery: {errorMessage}");
            mLoadingInfo = "遇到错误，尝试恢复中...";
            yield return new WaitForSeconds(2f);
            
            // Try to load DLL with existing files
            yield return StartCoroutine(LoadDllSafe());
        }
        else
        {
            mLoadingInfo = $"严重错误: {errorMessage}";
            PCLogManager.LogError($"Unrecoverable error: {errorMessage}");
        }
    }

    private IEnumerator CheckUpdateSafe()
    {
        mLoadingInfo = "检查版本更新...";
        mProgress = 0.1f;
        
        var updateArr = new Dictionary<string, AssetBundleInfo>();
        long totalSize = 0;
        
        PCLogManager.LogInfo("Starting update check process");
        
        // Check DLL updates
        bool dllCheckSuccess = false;
        yield return StartCoroutine(CheckDllUpdateSafe(updateArr, 
            size => totalSize += size, 
            success => dllCheckSuccess = success));
        
        if (!dllCheckSuccess)
        {
            yield return StartCoroutine(HandleError("DLL update check failed", true));
            yield break;
        }
        
        // Check resource updates  
        bool resCheckSuccess = false;
        yield return StartCoroutine(CheckResourceUpdateSafe(updateArr, 
            size => totalSize += size,
            success => resCheckSuccess = success));
        
        if (!resCheckSuccess)
        {
            yield return StartCoroutine(HandleError("Resource update check failed", true));
            yield break;
        }
        
        if (updateArr.Count > 0)
        {
            mTotalSize = totalSize;
            mCurSize = 0;
            mlastSize = 0;
            
            string sizeStr = GetDownSpdStr(mTotalSize);
            string tick = string.Format("发现更新，资源大小：[ff0000]{0}[-]", sizeStr);
            
            PCLogManager.LogInfo($"Update found: {updateArr.Count} files, total size: {sizeStr}");
            
            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                UI_VersionTickDialog.Show().InitData(tick, "(当前处于非wifi网络环境,是否继续下载?)", () =>
                {
                    StartCoroutine(StartOptimizedDownloadSafe(updateArr));
                });
            }
            else
            {
                UI_VersionTickDialog.Show().InitData(tick, "(检测到wifi网络，建议继续下载)", () =>
                {
                    StartCoroutine(StartOptimizedDownloadSafe(updateArr));
                });
            }
        }
        else
        {
            PCLogManager.LogInfo("No updates found, proceeding to load DLL");
            UpdateVersionFilesSafe();
            yield return StartCoroutine(LoadDllSafe());
        }
    }

    private IEnumerator CheckDllUpdateSafe(Dictionary<string, AssetBundleInfo> updateArr, 
        Action<long> onSizeAdd, Action<bool> onComplete)
    {
        PCLogManager.LogInfo("Checking DLL updates...");
        
        var localVersionPath = GetSafeFilePath(Application.persistentDataPath + "/DLL/version.bytes");
        var remoteVersionUrl = GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/version.bytes";
        var localVersion = "";
        var remoteVersion = "";
        
        if (SafeFileExists(localVersionPath))
        {
            localVersion = SafeReadAllText(localVersionPath);
        }

        bool versionDownloadSuccess = false;
        yield return StartCoroutine(DownloadVersionFileSafe(remoteVersionUrl, 
            text => {
                remoteVersion = text;
                versionDownloadSuccess = !string.IsNullOrEmpty(text);
            }));
        
        if (!versionDownloadSuccess)
        {
            RecordError("Failed to download DLL version file");
            onComplete(false);
            yield break;
        }
        
        if (remoteVersion != localVersion && !string.IsNullOrEmpty(remoteVersion))
        {
            PCLogManager.LogInfo($"DLL version change detected: {localVersion} -> {remoteVersion}");
            DllVersion = remoteVersion;
            
            var localAssetsPath = GetSafeFilePath(Application.persistentDataPath + "/DLL/assets.bytes");
            var remoteAssetsUrl = GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/assets.bytes";
            
            var existingAssets = GetExistingAssetList(localAssetsPath);
            
            var newAssets = new List<AssetBundleInfo>();
            bool assetsDownloadSuccess = false;
            yield return StartCoroutine(DownloadVersionFileSafe(remoteAssetsUrl, text => {
                DllAsset = text;
                assetsDownloadSuccess = ParseAssetList(text, out newAssets);
            }));
            
            if (!assetsDownloadSuccess)
            {
                RecordError("Failed to download or parse DLL assets file");
                onComplete(false);
                yield break;
            }
            
            ProcessAssetUpdates(existingAssets, newAssets, updateArr, onSizeAdd, 
                GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/",
                Application.persistentDataPath + "/DIR/DLL/");
        }
        else
        {
            PCLogManager.LogInfo("DLL is up to date");
        }
        
        onComplete(true);
    }

    private IEnumerator CheckResourceUpdateSafe(Dictionary<string, AssetBundleInfo> updateArr,
        Action<long> onSizeAdd, Action<bool> onComplete)
    {
        PCLogManager.LogInfo("Checking resource updates...");
        
        var localVersionPath = GetSafeFilePath(Application.persistentDataPath + "/AssetBundle_ALL/version.bytes");
        var remoteVersionUrl = GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/version.bytes";
        var localVersion = "";
        var remoteVersion = "";
        
        if (SafeFileExists(localVersionPath))
        {
            localVersion = SafeReadAllText(localVersionPath);
        }
        
        bool versionDownloadSuccess = false;
        yield return StartCoroutine(DownloadVersionFileSafe(remoteVersionUrl, 
            text => {
                remoteVersion = text;
                versionDownloadSuccess = !string.IsNullOrEmpty(text);
            }));
        
        if (!versionDownloadSuccess)
        {
            RecordError("Failed to download resource version file");
            onComplete(false);
            yield break;
        }
        
        if (remoteVersion != localVersion && !string.IsNullOrEmpty(remoteVersion))
        {
            PCLogManager.LogInfo($"Resource version change detected: {localVersion} -> {remoteVersion}");
            resVersion = remoteVersion;
            
            var localAssetsPath = GetSafeFilePath(Application.persistentDataPath + "/AssetBundle_ALL/assets.bytes");
            var remoteAssetsUrl = GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/assets.bytes";
            
            var existingAssets = GetExistingAssetList(localAssetsPath);
            
            var newAssets = new List<AssetBundleInfo>();
            bool assetsDownloadSuccess = false;
            yield return StartCoroutine(DownloadVersionFileSafe(remoteAssetsUrl, text => {
                resAsset = text;
                assetsDownloadSuccess = ParseAssetList(text, out newAssets);
            }));
            
            if (!assetsDownloadSuccess)
            {
                RecordError("Failed to download or parse resource assets file");
                onComplete(false);
                yield break;
            }
            
            ProcessAssetUpdates(existingAssets, newAssets, updateArr, onSizeAdd,
                GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/",
                Application.persistentDataPath + "/DIR/AssetBundle_ALL/");
        }
        else
        {
            PCLogManager.LogInfo("Resources are up to date");
        }
        
        onComplete(true);
    }

    private List<AssetBundleInfo> GetExistingAssetList(string assetPath)
    {
        var existingAssets = new List<AssetBundleInfo>();
        if (SafeFileExists(assetPath))
        {
            var text = SafeReadAllText(assetPath);
            if (!string.IsNullOrEmpty(text))
            {
                if (!ParseAssetList(text, out existingAssets))
                {
                    existingAssets = new List<AssetBundleInfo>();
                }
            }
        }
        return existingAssets;
    }

    private bool ParseAssetList(string jsonText, out List<AssetBundleInfo> assetList)
    {
        assetList = new List<AssetBundleInfo>();
        
        if (string.IsNullOrEmpty(jsonText))
        {
            return false;
        }
        
        try
        {
            assetList = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(jsonText);
            return true;
        }
        catch (Exception e)
        {
            RecordError($"Failed to parse asset list: {e.Message}");
            return false;
        }
    }

    private void ProcessAssetUpdates(List<AssetBundleInfo> existingAssets, List<AssetBundleInfo> newAssets,
        Dictionary<string, AssetBundleInfo> updateArr, Action<long> onSizeAdd, string rootUrl, string saveRoot)
    {
        var existingDict = new Dictionary<string, AssetBundleInfo>();
        foreach (var item in existingAssets)
        {
            existingDict[item.ABname] = item;
        }
        
        foreach (var newItem in newAssets)
        {
            var needUpdate = false;
            var localFilePath = GetSafeFilePath(Application.persistentDataPath + "/" + 
                (rootUrl.Contains("DLL") ? "DLL/" : "AssetBundle_ALL/") + newItem.ABname);
            
            if (existingDict.ContainsKey(newItem.ABname))
            {
                if (existingDict[newItem.ABname].MD5 != newItem.MD5 || 
                    !SafeFileExists(localFilePath) || 
                    !ValidateFileMD5Safe(localFilePath, newItem.MD5))
                {
                    needUpdate = true;
                }
            }
            else
            {
                needUpdate = true;
            }
            
            if (needUpdate)
            {
                onSizeAdd(newItem.Size);
                newItem.root = rootUrl;
                newItem.saveroot = saveRoot;
                updateArr.Add(rootUrl + newItem.ABname, newItem);
                PCLogManager.LogInfo($"Update needed: {newItem.ABname} ({GetDownSpdStr(newItem.Size)})");
            }
        }
    }

    private bool ValidateFileMD5Safe(string filePath, string expectedMD5)
    {
        if (!SafeFileExists(filePath)) return false;
        
        var fileInfo = new FileInfo(filePath);
        var cacheKey = filePath + "_" + fileInfo.LastWriteTime.Ticks + "_" + fileInfo.Length;
        
        if (md5Cache.TryGetValue(cacheKey, out var cachedMD5))
        {
            return cachedMD5.Equals(expectedMD5, StringComparison.OrdinalIgnoreCase);
        }
        
        lock (fileOperationLock)
        {
            if (lockedFiles.Contains(filePath))
            {
                return false;
            }
            
            lockedFiles.Add(filePath);
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var hash = md5.ComputeHash(stream);
                        var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        md5Cache[cacheKey] = hashString;
                        return hashString.Equals(expectedMD5, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch (Exception e)
            {
                RecordError($"MD5 validation failed for {Path.GetFileName(filePath)}: {e.Message}");
                return false;
            }
            finally
            {
                lockedFiles.Remove(filePath);
            }
        }
    }

    private IEnumerator StartOptimizedDownloadSafe(Dictionary<string, AssetBundleInfo> updateArr)
    {
        PCLogManager.LogInfo($"Starting optimized download of {updateArr.Count} files");
        
        mProgress = 0;
        mLoadingInfo = "准备下载...";
        isUpdate = true;
        
        var totalCount = updateArr.Count;
        var completedCount = 0;
        var failedCount = 0;
        
        // Sort downloads by size (smaller files first for faster initial progress)
        var sortedDownloads = new List<KeyValuePair<string, AssetBundleInfo>>(updateArr);
        sortedDownloads.Sort((x, y) => x.Value.Size.CompareTo(y.Value.Size));
        
        foreach (var item in sortedDownloads)
        {
            StartCoroutine(DownloadWithConcurrencyControlSafe(
                item.Value.root + item.Value.ABname, 
                item.Value.saveroot, 
                item.Value, 
                (success, downloadedSize) => {
                    if (success)
                    {
                        completedCount++;
                        lock (downloadCountLock)
                        {
                            mCurSize += downloadedSize;
                        }
                        PCLogManager.LogInfo($"Download completed: {item.Value.ABname} ({completedCount}/{totalCount})");
                    }
                    else
                    {
                        failedCount++;
                        RecordError($"Download failed: {item.Value.ABname}");
                    }
                    
                    mProgress = (float)(completedCount + failedCount) / totalCount;
                    
                    if (completedCount + failedCount >= totalCount)
                    {
                        if (failedCount > 0)
                        {
                            PCLogManager.LogWarning($"Download completed with {failedCount} failures out of {totalCount} files");
                        }
                        StartCoroutine(DownloadSuccessSafe());
                    }
                }));
        }
        
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DownloadWithConcurrencyControlSafe(string url, string savePath, AssetBundleInfo asset, Action<bool, int> callback)
    {
        var waitTime = 0.1f;
        var maxWaitTime = 5f;
        
        // Wait for available slot
        while (true)
        {
            lock (downloadCountLock)
            {
                if (activeDownloads < maxConcurrentDownloads)
                {
                    activeDownloads++;
                    break;
                }
            }
            yield return new WaitForSeconds(waitTime);
            waitTime = Mathf.Min(waitTime * 1.2f, maxWaitTime);
        }
        
        // Perform download
        var success = false;
        var downloadedSize = 0;
        
        yield return StartCoroutine(SaveAssetBundleSafe(url, savePath, asset, (result, size) => {
            success = result;
            downloadedSize = size;
        }));
        
        // Release slot
        lock (downloadCountLock)
        {
            activeDownloads--;
        }
        
        callback(success, downloadedSize);
    }

    private void UpdateVersionFilesSafe()
    {
        PCLogManager.LogInfo("Updating version files...");
        
        if (!string.IsNullOrEmpty(DllVersion))
        {
            var versionPath = GetSafeFilePath(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionNO);
            SafeWriteAllText(versionPath, DllVersion);
        }
        
        if (!string.IsNullOrEmpty(DllAsset))
        {
            var assetPath = GetSafeFilePath(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionPath);
            SafeWriteAllText(assetPath, DllAsset);
        }

        if (!string.IsNullOrEmpty(resVersion))
        {
            var versionPath = GetSafeFilePath(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionNO);
            SafeWriteAllText(versionPath, resVersion);
        }
        
        if (!string.IsNullOrEmpty(resAsset))
        {
            var assetPath = GetSafeFilePath(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionPath);
            SafeWriteAllText(assetPath, resAsset);
        }
        
        PCLogManager.LogInfo("Version files updated successfully");
    }

    private string GetDownSpdStr(long size)
    {
        size = Math.Max(size, 0);
        if (size >= 1073741824)
        {
            return $"{size / 1024f / 1024f / 1024f:0.##}GB";
        }
        else if (size >= 1048576)
        {
            return $"{size / 1024f / 1024f:0.##}MB";
        }
        else if (size >= 1024)
        {
            return $"{size / 1024f:0.##}KB";
        }
        else
        {
            return $"{size}B";
        }
    }

    private IEnumerator CopyStreamingAssetsFileSafe(Action<bool> onComplete)
    {
        PCLogManager.LogInfo("Starting StreamingAssets copy process");
        
        // Clear existing data safely
        bool cleanupSuccess = CleanupExistingDataSafe();
        if (!cleanupSuccess)
        {
            RecordError("Failed to cleanup existing data");
            onComplete(false);
            yield break;
        }

        Directory.CreateDirectory(Application.persistentDataPath);

        mLoadingInfo = "解压资源...";
        mProgress = 0;
        var fileList = new List<string>();
        
        // Read folder list
        bool folderReadSuccess = false;
        yield return StartCoroutine(ReadFolderListSafe(list => {
            fileList = list;
            folderReadSuccess = list.Count > 0;
        }));
        
        if (!folderReadSuccess)
        {
            RecordError("Failed to read folder list");
            onComplete(false);
            yield break;
        }

        var copyOperations = new List<CopyOperation>();
        
        // Collect all copy operations
        bool collectSuccess = true;
        for (int i = 0; i < fileList.Count && collectSuccess; i++)
        {
            yield return StartCoroutine(CollectCopyOperationsSafe(fileList[i], copyOperations, success => {
                if (!success)
                {
                    collectSuccess = false;
                }
            }));
        }
        
        if (!collectSuccess)
        {
            RecordError("Failed to collect copy operations");
            onComplete(false);
            yield break;
        }

        // Perform copy operations
        bool copySuccess = true;
        yield return StartCoroutine(PerformCopyOperationsSafe(copyOperations, success => copySuccess = success));
        
        if (!copySuccess)
        {
            RecordError("Failed to perform copy operations");
            onComplete(false);
            yield break;
        }

        // Move files from DIR to final location
        bool moveSuccess = true;
        yield return StartCoroutine(MoveFinalFilesSafe(success => moveSuccess = success));
        
        if (!moveSuccess)
        {
            RecordError("Failed to move files to final location");
            onComplete(false);
            yield break;
        }
        
        // Write version file
        bool versionWriteSuccess = SafeWriteAllText(Application.persistentDataPath + "/resversion.ver", Application.version);
        if (!versionWriteSuccess)
        {
            RecordError("Failed to write version file");
            onComplete(false);
            yield break;
        }
        
        PCLogManager.LogInfo("StreamingAssets copy completed successfully");
        onComplete(true);
    }

    private bool CleanupExistingDataSafe()
    {
        var persistentPath = Application.persistentDataPath;
        if (Directory.Exists(persistentPath))
        {
            try
            {
                var tempBackupPath = persistentPath + "_backup_" + DateTime.Now.Ticks;
                Directory.Move(persistentPath, tempBackupPath);
                
                // Schedule cleanup of backup (don't block on this)
                StartCoroutine(DelayedDirectoryCleanupSafe(tempBackupPath, 10f));
                return true;
            }
            catch (Exception e)
            {
                RecordError($"Could not backup existing data: {e.Message}");
                try
                {
                    Directory.Delete(persistentPath, true);
                    return true;
                }
                catch (Exception deleteEx)
                {
                    RecordError($"Failed to delete existing data: {deleteEx.Message}");
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator DelayedDirectoryCleanupSafe(string path, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                PCLogManager.LogInfo($"Cleaned up backup directory: {Path.GetFileName(path)}");
            }
        }
        catch (Exception e)
        {
            PCLogManager.LogWarning($"Could not clean up backup directory: {e.Message}");
        }
    }

    private IEnumerator ReadFolderListSafe(Action<List<string>> onComplete)
    {
        var fileList = new List<string>();
        
        using (UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/folder.byte"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var arr = request.downloadHandler.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                fileList.AddRange(arr);
            }
            else
            {
                RecordError($"Failed to read folder.byte: {request.error}");
            }
        }
        
        onComplete(fileList);
    }

    private IEnumerator CollectCopyOperationsSafe(string folderName, List<CopyOperation> copyOperations, Action<bool> onComplete)
    {
        string folderPath = folderName + "/";
        var assetsPath = Application.persistentDataPath + "/" + folderPath + "assets.bytes";
        
        if (!SafeFileExists(assetsPath))
        {
            var dirPath = Application.persistentDataPath + "/DIR/" + folderPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            using (UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + folderPath + "assets.bytes"))
            {
                yield return request.SendWebRequest();
                    
                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (SafeWriteAllBytes(dirPath + "assets.bytes", request.downloadHandler.data))
                    {
                        var assetList = new List<AssetBundleInfo>();
                        if (ParseAssetList(request.downloadHandler.text, out assetList))
                        {
                            foreach (var asset in assetList)
                            {
                                copyOperations.Add(new CopyOperation
                                {
                                    sourcePath = Application.streamingAssetsPath + "/" + folderPath + asset.ABname,
                                    destPath = dirPath + asset.ABname,
                                    size = asset.Size
                                });
                            }
                            onComplete(true);
                        }
                        else
                        {
                            onComplete(false);
                        }
                    }
                    else
                    {
                        onComplete(false);
                    }
                }
                else
                {
                    RecordError($"Failed to read assets.bytes for {folderPath}: {request.error}");
                    onComplete(false);
                }
            }
        }
        else
        {
            onComplete(true);
        }
    }

    private IEnumerator PerformCopyOperationsSafe(List<CopyOperation> copyOperations, Action<bool> onComplete)
    {
        // Sort by size for better progress indication
        copyOperations.Sort((a, b) => a.size.CompareTo(b.size));
        
        var totalOperations = copyOperations.Count;
        var currentOp = 0;
        var processedInBatch = 0;
        const int batchSize = 10;
        bool allSuccess = true;
        
        foreach (var operation in copyOperations)
        {
            bool copySuccess = false;
            yield return StartCoroutine(CopyFileSafe(operation.sourcePath, operation.destPath, success => copySuccess = success));
            
            if (!copySuccess)
            {
                allSuccess = false;
                // Continue with other files even if one fails
            }
            
            currentOp++;
            processedInBatch++;
            
            mProgress = (float)currentOp / totalOperations;
            mLoadingInfo = $"解压资源: {currentOp}/{totalOperations}";
            
            if (processedInBatch >= batchSize)
            {
                processedInBatch = 0;
                yield return new WaitForEndOfFrame();
                
                // Periodic garbage collection
                if (currentOp % 50 == 0)
                {
                    System.GC.Collect();
                }
            }
        }
        
        onComplete(allSuccess);
    }

    private IEnumerator MoveFinalFilesSafe(Action<bool> onComplete)
    {
        var dirPath = Application.persistentDataPath + "/DIR";
        if (Directory.Exists(dirPath))
        {
            bool moveSuccess = false;
            yield return StartCoroutine(CopyFolderSafe(dirPath, Application.persistentDataPath, success => moveSuccess = success));
            
            if (moveSuccess)
            {
                try
                {
                    Directory.Delete(dirPath, true);
                    onComplete(true);
                }
                catch (Exception e)
                {
                    RecordError($"Could not delete temporary DIR: {e.Message}");
                    onComplete(true); // Still consider success since files were copied
                }
            }
            else
            {
                onComplete(false);
            }
        }
        else
        {
            onComplete(true);
        }
    }

    private class CopyOperation
    {
        public string sourcePath;
        public string destPath;
        public long size;
    }

    void Update()
    {
        if (isUpdate)
        {
            var currentSize = mCurSize;
            var downloadSpeed = (currentSize - mlastSize) * 2; // Rough estimate
            
            mLoadingInfo = string.Format("下载文件 {0}/{1} 速度：{2}/s", 
                GetDownSpdStr(currentSize), 
                GetDownSpdStr(mTotalSize), 
                GetDownSpdStr(downloadSpeed));
            
            mlastSize = currentSize;
        }
        
        SetProgress(mProgress);
        SetLoadingInfo(mLoadingInfo);
    }

    private IEnumerator CopyFolderSafe(string srcPath, string tarPath, Action<bool> onComplete)
    {
        if (!Directory.Exists(srcPath))
        {
            onComplete(true);
            yield break;
        }

        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        var files = Directory.GetFiles(srcPath);
        var processedFiles = 0;
        bool allSuccess = true;
        
        foreach (var file in files)
        {
            string destFile = Path.Combine(tarPath, Path.GetFileName(file));
            bool copySuccess = false;
            yield return StartCoroutine(CopyFileSafe(file, destFile, success => copySuccess = success));
            
            if (!copySuccess)
            {
                allSuccess = false;
            }
            
            processedFiles++;
            if (processedFiles % 10 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        var folders = Directory.GetDirectories(srcPath);
        foreach (var folder in folders)
        {
            string destDir = Path.Combine(tarPath, Path.GetFileName(folder));
            bool folderSuccess = false;
            yield return StartCoroutine(CopyFolderSafe(folder, destDir, success => folderSuccess = success));
            
            if (!folderSuccess)
            {
                allSuccess = false;
            }
        }
        
        onComplete(allSuccess);
    }

    private IEnumerator CopyFileSafe(string sourcePath, string destinationPath, Action<bool> onComplete)
    {
        byte[] fileData = null;
        
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(sourcePath))
            {
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    fileData = www.downloadHandler.data;
                }
                else
                {
                    RecordError($"Failed to copy file via WWW: {sourcePath} -> {www.error}");
                    onComplete(false);
                    yield break;
                }
            }
        }
        else
        {
            if (File.Exists(sourcePath))
            {
                try
                {
                    fileData = File.ReadAllBytes(sourcePath);
                }
                catch (Exception e)
                {
                    RecordError($"Failed to read source file: {sourcePath} -> {e.Message}");
                    onComplete(false);
                    yield break;
                }
            }
            else
            {
                RecordError($"Source file not found: {sourcePath}");
                onComplete(false);
                yield break;
            }
        }
        
        if (fileData != null && fileData.Length > 0)
        {
            bool writeSuccess = SafeWriteAllBytes(destinationPath, fileData);
            onComplete(writeSuccess);
        }
        else
        {
            onComplete(false);
        }
    }

    public void SetProgress(float percent)
    {
        if (uiSlider != null && Math.Abs(uiSlider.value - percent) > 0.001f)
        {
            if (uiLabelProgress != null)
            {
                uiLabelProgress.text = Mathf.CeilToInt(percent * 100) + "%";
            }
            uiSlider.value = percent;
        }
    }

    public void SetLoadingInfo(string message)
    {
        if (uiLabelInfo != null && uiLabelInfo.text != message)
        {
            uiLabelInfo.text = message;
        }
    }

    private IEnumerator DownloadSuccessSafe()
    {
        PCLogManager.LogInfo("All downloads completed, updating version files");
        
        // Update version files in temporary directory first
        bool updateSuccess = UpdateVersionFilesInTempDirSafe();
        
        if (!updateSuccess)
        {
            yield return StartCoroutine(HandleError("Failed to update version files", true));
            yield break;
        }

        // Move downloaded files to final location
        bool moveSuccess = false;
        yield return StartCoroutine(MoveFinalFilesSafe(success => moveSuccess = success));
        
        if (!moveSuccess)
        {
            yield return StartCoroutine(HandleError("Failed to move downloaded files", true));
            yield break;
        }
        
        isUpdate = false;
        mLoadingInfo = "下载完成，准备启动游戏...";
        mProgress = 1f;
        
        PCLogManager.LogInfo("Download and installation completed successfully");
        
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(LoadDllSafe());
    }

    private bool UpdateVersionFilesInTempDirSafe()
    {
        bool allSuccess = true;
        
        if (!string.IsNullOrEmpty(DllVersion))
        {
            var dllDir = Application.persistentDataPath + "/DIR/DLL";
            if (Directory.Exists(dllDir))
            {
                allSuccess &= SafeWriteAllText(dllDir + "/" + ABLoadConfig.VersionNO, DllVersion);
                allSuccess &= SafeWriteAllText(dllDir + "/" + ABLoadConfig.VersionPath, DllAsset);
            }
        }

        if (!string.IsNullOrEmpty(resVersion))
        {
            var resDir = Application.persistentDataPath + "/DIR/AssetBundle_ALL";
            if (Directory.Exists(resDir))
            {
                allSuccess &= SafeWriteAllText(resDir + "/" + ABLoadConfig.VersionNO, resVersion);
                allSuccess &= SafeWriteAllText(resDir + "/" + ABLoadConfig.VersionPath, resAsset);
            }
        }
        
        return allSuccess;
    }

    private IEnumerator LoadDllSafe()
    {
        PCLogManager.LogInfo("Starting DLL loading process");
        
// #if !UNITY_EDITOR
        bool aotLoadSuccess = true;
        yield return StartCoroutine(LoadAOTAssembliesSafe(success => aotLoadSuccess = success));
        
        if (!aotLoadSuccess)
        {
            yield return StartCoroutine(HandleError("AOT assemblies loading failed", false));
            yield break;
        }
        
        bool hotUpdateLoadSuccess = true;
        yield return StartCoroutine(LoadHotUpdateAssembliesSafe(success => hotUpdateLoadSuccess = success));
        
        if (!hotUpdateLoadSuccess)
        {
            yield return StartCoroutine(HandleError("Hot update assemblies loading failed", false));
            yield break;
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
// #endif
        
        // Load game entrance
        bool entranceLoadSuccess = false;
        yield return StartCoroutine(LoadGameEntranceSafe(success => entranceLoadSuccess = success));
        
        if (!entranceLoadSuccess)
        {
            yield return StartCoroutine(HandleError("Game entrance loading failed", false));
            yield break;
        }
        
        // Cleanup
        CleanupSafe();
        yield break;
    }

// #if !UNITY_EDITOR
    private IEnumerator LoadAOTAssembliesSafe(Action<bool> onComplete)
    {
        List<string> aotMetaAssemblyFiles = new List<string>()
        {
            "mscorlib.dll",
            "System.dll", 
            "System.Core.dll"
        };
        
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        bool allSuccess = true;
        
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            var path = "/DLL/" + aotDllName + ".bytes";
            var fullPath = GetSafeFilePath(Application.persistentDataPath + path);
            
            if (SafeFileExists(fullPath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get("file://" + fullPath))
                {
                    yield return request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(request.downloadHandler.data, mode);
                            PCLogManager.LogInfo($"Loaded AOT assembly: {aotDllName} (result: {err})");
                        }
                        catch (Exception e)
                        {
                            RecordError($"Failed to load AOT assembly {aotDllName}: {e.Message}");
                            allSuccess = false;
                        }
                    }
                    else
                    {
                        RecordError($"Failed to download AOT assembly: {aotDllName} -> {request.error}");
                        allSuccess = false;
                    }
                }
            }
            else
            {
                RecordError($"AOT assembly not found: {aotDllName}");
                allSuccess = false;
            }
        }
        
        onComplete(allSuccess);
    }

    private IEnumerator LoadHotUpdateAssembliesSafe(Action<bool> onComplete)
    {
        List<string> hotMetaAssemblyFiles = new List<string>()
        {
            "HotUpdate.dll.bytes"
        };
        
        bool allSuccess = true;
        
        foreach (var dllName in hotMetaAssemblyFiles)
        {
            var fullPath = GetSafeFilePath(Application.persistentDataPath + "/DLL/" + dllName);
            
            if (SafeFileExists(fullPath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get("file://" + fullPath))
                {
                    yield return request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            System.Reflection.Assembly.Load(request.downloadHandler.data);
                            PCLogManager.LogInfo($"Loaded hot update assembly: {dllName}");
                        }
                        catch (Exception e)
                        {
                            RecordError($"Failed to load hot update assembly {dllName}: {e.Message}");
                            allSuccess = false;
                        }
                    }
                    else
                    {
                        RecordError($"Failed to download hot update assembly: {dllName} -> {request.error}");
                        allSuccess = false;
                    }
                }
            }
            else
            {
                RecordError($"Hot update assembly not found: {dllName}");
                allSuccess = false;
            }
        }
        
        onComplete(allSuccess);
    }
// #endif

    private IEnumerator LoadGameEntranceSafe(Action<bool> onComplete)
    {
        try
        {
#if UNITY_EDITOR
            var prefabPath = "Assets/Arts_ALL/GameRes/Prefabs/UI/GameEntrance.prefab";
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                var gameObject = Instantiate(prefab);
                gameObject.SetActive(true);
                DontDestroyOnLoad(gameObject);
                PCLogManager.LogInfo("Game entrance loaded successfully (Editor)");
                onComplete(true);
            }
            else
            {
                RecordError("Failed to load game entrance prefab in Editor");
                onComplete(false);
            }
#else
            var bundlePath = GetSafeFilePath(Application.persistentDataPath + "/AssetBundle_ALL/assets^arts_all^gameres^prefabs^ui^gameentrance.kb");
            if (SafeFileExists(bundlePath))
            {
                AssetBundle loadingBundle = AssetBundle.LoadFromFile(bundlePath);
                if (loadingBundle != null)
                {
                    var prefab = loadingBundle.LoadAsset("gameentrance", typeof(GameObject)) as GameObject;
                    if (prefab != null)
                    {
                        var gameObject = Instantiate(prefab);
                        gameObject.SetActive(true);
                        DontDestroyOnLoad(gameObject);
                        PCLogManager.LogInfo("Game entrance loaded successfully");
                        onComplete(true);
                    }
                    else
                    {
                        RecordError("Game entrance prefab not found in bundle");
                        onComplete(false);
                    }
                }
                else
                {
                    RecordError("Failed to load game entrance bundle");
                    onComplete(false);
                }
            }
            else
            {
                RecordError("Game entrance bundle not found");
                onComplete(false);
            }
#endif
        }
        catch (Exception e)
        {
            RecordError($"Failed to load game entrance: {e.Message}");
            onComplete(false);
        }
        
        yield return null; // Make this a proper coroutine
    }

    private void CleanupSafe()
    {
        try
        {
            Destroy(gameObject);
            PCLogManager.LogInfo("HotMain_Improved cleanup completed");
        }
        catch (Exception e)
        {
            PCLogManager.LogWarning($"Cleanup warning: {e.Message}");
        }
    }

    private IEnumerator SaveAssetBundleSafe(string url, string savePath, AssetBundleInfo asset, Action<bool, int> callback = null)
    {
        string fullUrl = url;
        string fullSavePath = savePath + asset.ABname;
        int retryCount = maxRetryAttempts;
        float retryDelay = baseRetryDelay;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
            {
                request.timeout = (int)downloadTimeoutSeconds;
                request.SetRequestHeader("User-Agent", "Unity-Game-Client-Enhanced");
                
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    retryCount--;
                    RecordError($"Download failed for {asset.ABname}: {request.error} (retries left: {retryCount})");
                    
                    if (retryCount > 0)
                    {
                        yield return new WaitForSeconds(retryDelay);
                        retryDelay = Math.Min(retryDelay * 2, 5f);
                        continue;
                    }
                    else
                    {
                        callback?.Invoke(false, 0);
                        yield break;
                    }
                }

                byte[] data = request.downloadHandler.data;
                
                if (data == null || data.Length == 0)
                {
                    retryCount--;
                    RecordError($"Empty data received for {asset.ABname} (retries left: {retryCount})");
                    continue;
                }
                
                if (!SafeWriteAllBytes(fullSavePath, data))
                {
                    retryCount--;
                    RecordError($"Failed to write file {asset.ABname} (retries left: {retryCount})");
                    continue;
                }
                
                // Verify file integrity
                if (SafeFileExists(fullSavePath) && ValidateFileMD5Safe(fullSavePath, asset.MD5))
                {
                    callback?.Invoke(true, data.Length);
                    yield break;
                }
                else
                {
                    retryCount--;
                    RecordError($"File integrity check failed for {asset.ABname} (retries left: {retryCount})");
                    
                    // Delete corrupted file
                    try
                    {
                        if (File.Exists(fullSavePath))
                        {
                            File.Delete(fullSavePath);
                        }
                    }
                    catch (Exception e)
                    {
                        RecordError($"Could not delete corrupted file: {e.Message}");
                    }
                }
            }
            
            if (retryCount > 0)
            {
                yield return new WaitForSeconds(retryDelay);
                retryDelay = Math.Min(retryDelay * 1.5f, 3f);
            }
        }
        
        RecordError($"Download completely failed for {asset.ABname} after all retries");
        callback?.Invoke(false, 0);
    }

    private IEnumerator DownloadVersionFileSafe(string url, Action<string> callback = null)
    {
        int retryCount = maxRetryAttempts;
        float retryDelay = 0.3f;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.timeout = 20;
                request.SetRequestHeader("Cache-Control", "no-cache");
                request.SetRequestHeader("Pragma", "no-cache");
                
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    retryCount--;
                    RecordError($"Version file download failed: {request.error} (retries left: {retryCount})");
                    
                    if (retryCount > 0)
                    {
                        yield return new WaitForSeconds(retryDelay);
                        retryDelay = Math.Min(retryDelay * 1.5f, 3f);
                        continue;
                    }
                }
                else
                {
                    var text = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        callback?.Invoke(text);
                        yield break;
                    }
                    else
                    {
                        retryCount--;
                        RecordError($"Empty version file received (retries left: {retryCount})");
                    }
                }
            }
        }
        
        RecordError("Version file download completely failed");
        callback?.Invoke("");
    }

    void OnDestroy()
    {
        PCLogManager.LogInfo("HotMain_Improved destroyed");
        
        // Clean up any remaining resources
        lock (fileOperationLock)
        {
            lockedFiles.Clear();
        }
        
        md5Cache.Clear();
        errorLog.Clear();
    }

    // Public API for debugging
    public bool HasErrors() { return hasErrors; }
    public string GetLastError() { return lastError; }
    public List<string> GetErrorLog() { return new List<string>(errorLog); }
}