using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class HotMain : MonoBehaviour
{
    public UILabel uiLabelProgress, uiLabelInfo, uiLabelSpeed, uiLabelETA;
    public UISlider uiSlider;
    
    private string mLoadingInfo;
    private float mProgress;
    private bool isUpdate = false;
    private long mTotalSize;
    private long mCurSize;
    private long mlastSize;
    private string DllVersion;
    private string resVersion;
    private string DllAsset;
    private string resAsset;
    
    private int maxConcurrentDownloads = 3;
    private int activeDownloads = 0;
    private readonly object downloadCountLock = new object();
    private readonly Dictionary<string, string> md5Cache = new Dictionary<string, string>();
    
    private const int BUFFER_SIZE = 8192;
    private const int MAX_RETRY_COUNT = 3;
    private const float PROGRESS_UPDATE_INTERVAL = 0.1f;
    
    private readonly Dictionary<string, float> serverResponseTimes = new Dictionary<string, float>();
    private float lastProgressUpdate = 0f;
    
    private readonly Queue<float> speedHistory = new Queue<float>();
    private const int SPEED_HISTORY_SIZE = 10;
    private DateTime downloadStartTime;

    void Start()
    {
        Application.runInBackground = true;
        StartCoroutine(CheckFirst());
    }
    
    private bool IsPCPlatform()
    {
        return Application.platform == RuntimePlatform.WindowsPlayer || 
               Application.platform == RuntimePlatform.WindowsEditor ||
               Application.platform == RuntimePlatform.OSXPlayer ||
               Application.platform == RuntimePlatform.LinuxPlayer;
    }
        
    // private IEnumerator CheckFirst()
    // {
    //     if (!CheckDiskSpace())
    //     {
    //         mLoadingInfo = "存储空间不足!";
    //         yield break;
    //     }
    //     
    //     var versionPath = Application.persistentDataPath + "/resversion.ver";
    //     if (File.Exists(versionPath))
    //     {
    //         var str = File.ReadAllText(versionPath);
    //         if (str != Application.version)
    //         {
    //             yield return StartCoroutine(CopyStreamingAssetsFile());
    //             yield return StartCoroutine(CheckUpdate());
    //         }
    //         else
    //         {
    //             yield return StartCoroutine(CheckUpdate());
    //         }
    //     }
    //     else
    //     {
    //         yield return StartCoroutine(CopyStreamingAssetsFile());
    //         yield return StartCoroutine(CheckUpdate());
    //     }
    // }
    private IEnumerator CheckFirst()
    {
        if (!CheckDiskSpace())
        {
            mLoadingInfo = "存储空间不足!";
            yield break;
        }
        if (IsPCPlatform())
        {
            mLoadingInfo = "PC平台直接启动...";
            yield return StartCoroutine(LoadDll());
            yield break;
        }
        var versionPath = Application.persistentDataPath + "/resversion.ver";
        if (File.Exists(versionPath))
        {
            var str = File.ReadAllText(versionPath);
            if (str != Application.version)
            {
                yield return StartCoroutine(CopyStreamingAssetsFile());
                yield return StartCoroutine(CheckUpdate());
            }
            else
            {
                yield return StartCoroutine(CheckUpdate());
            }
        }
        else
        {
            yield return StartCoroutine(CopyStreamingAssetsFile());
            yield return StartCoroutine(CheckUpdate());
        }
    }

    
    private bool CheckDiskSpace()
    {
        const long MIN_FREE_SPACE = 500L * 1024 * 1024;
        
        try
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(Application.persistentDataPath));
            return driveInfo.AvailableFreeSpace > MIN_FREE_SPACE;
        }
        catch
        {
            return true;
        }
    }

    private IEnumerator CheckUpdate()
    {
        mLoadingInfo = "检查版本更新...";
        mProgress = 0.02f;
        
        var updateArr = new Dictionary<string, AssetBundleInfo>();
        long totalSize = 0;
        
        yield return StartCoroutine(CheckDllUpdate(updateArr, (size) => totalSize += size));
        mProgress = 0.05f;
        
        yield return StartCoroutine(CheckResourceUpdate(updateArr, (size) => totalSize += size));
        mProgress = 0.08f;
        
        if (updateArr.Count > 0)
        {
            mTotalSize = totalSize;
            mCurSize = 0;
            mlastSize = 0;
            downloadStartTime = DateTime.Now;
            
            string tick = string.Format("资源大小：[ff0000]{0}[-]", GetDownSpdStr(mTotalSize));
            
            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                UI_VersionTickDialog.Show().InitData(tick, "(当前处于非wifi网络环境,是否继续下载?)", () =>
                {
                    StartCoroutine(StartOptimizedDownload(updateArr));
                });
            }
            else
            {
                UI_VersionTickDialog.Show().InitData(tick, "(检测到wifi网络继续下载)", () =>
                {
                    StartCoroutine(StartOptimizedDownload(updateArr));
                });
            }
        }
        else
        {
            UpdateVersionFiles();
            yield return StartCoroutine(LoadDll());
        }
    }

    private IEnumerator CheckDllUpdate(Dictionary<string, AssetBundleInfo> updateArr, Action<long> onSizeAdd)
    {
        var path = Application.persistentDataPath + "/DLL/version.bytes";
        var outfile = GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/version.bytes";
        var localVersion = "";
        var newVersion = "";
        
        if (File.Exists(path))
        {
            localVersion = File.ReadAllText(path);
        }

        yield return StartCoroutine(DownloadVersionFile(outfile, (text) => newVersion = text));
        
        if (newVersion != localVersion && !string.IsNullOrEmpty(newVersion))
        {
            DllVersion = newVersion;
            path = Application.persistentDataPath + "/DLL/assets.bytes";
            outfile = GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/assets.bytes";
            
            var list = new List<AssetBundleInfo>();
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
            }
            
            var newList = new List<AssetBundleInfo>();
            yield return StartCoroutine(DownloadVersionFile(outfile, (text) => {
                if (!string.IsNullOrEmpty(text))
                {
                    DllAsset = text;
                    newList = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
                }
            }));
            
            ProcessUpdateList(list, newList, "/DLL/", "/ANDROID/DLL/", updateArr, onSizeAdd);
        }
    }

    private IEnumerator CheckResourceUpdate(Dictionary<string, AssetBundleInfo> updateArr, Action<long> onSizeAdd)
    {
        var path = Application.persistentDataPath + "/AssetBundle_ALL/version.bytes";
        var outfile = GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/version.bytes";
        var localVersion = "";
        var newVersion = "";
        
        if (File.Exists(path))
        {
            localVersion = File.ReadAllText(path);
        }
        
        yield return StartCoroutine(DownloadVersionFile(outfile, (text) => newVersion = text));
        
        if (newVersion != localVersion && !string.IsNullOrEmpty(newVersion))
        {
            resVersion = newVersion;
            path = Application.persistentDataPath + "/AssetBundle_ALL/assets.bytes";
            outfile = GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/assets.bytes";
            
            var list = new List<AssetBundleInfo>();
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
            }
            
            var newList = new List<AssetBundleInfo>();
            yield return StartCoroutine(DownloadVersionFile(outfile, (text) => {
                if (!string.IsNullOrEmpty(text))
                {
                    resAsset = text;
                    newList = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
                }
            }));
            
            ProcessUpdateList(list, newList, "/AssetBundle_ALL/", "/ANDROID/AssetBundle_ALL/", updateArr, onSizeAdd);
        }
    }
    
    private void ProcessUpdateList(List<AssetBundleInfo> oldList, List<AssetBundleInfo> newList, 
        string localPath, string remotePath, Dictionary<string, AssetBundleInfo> updateArr, Action<long> onSizeAdd)
    {
        var existingFileDict = new Dictionary<string, AssetBundleInfo>();
        foreach (var item in oldList)
        {
            existingFileDict[item.ABname] = item;
        }
        
        foreach (var newItem in newList)
        {
            var needUpdate = false;
            var localFilePath = Application.persistentDataPath + localPath + newItem.ABname;
            
            if (existingFileDict.ContainsKey(newItem.ABname))
            {
                if (existingFileDict[newItem.ABname].MD5 != newItem.MD5 || 
                    !File.Exists(localFilePath) || 
                    !ValidateFileMD5(localFilePath, newItem.MD5))
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
                newItem.root = GameParams.Instance.AbDownLoadSite + remotePath;
                newItem.saveroot = Application.persistentDataPath + "/DIR" + localPath;
                updateArr.Add(GameParams.Instance.AbDownLoadSite + remotePath + newItem.ABname, newItem);
            }
        }
    }

    private IEnumerator StartOptimizedDownload(Dictionary<string, AssetBundleInfo> updateArr)
    {
        mProgress = 0.1f;
        mLoadingInfo = "准备下载...";
        isUpdate = true;
        downloadStartTime = DateTime.Now;
        
        var sortedDownloads = new List<KeyValuePair<string, AssetBundleInfo>>(updateArr);
        
        sortedDownloads.Sort((x, y) => {
            var sizeX = x.Value.Size;
            var sizeY = y.Value.Size;
            var avgSize = mTotalSize / updateArr.Count;
            
            var diffX = Math.Abs(sizeX - avgSize);
            var diffY = Math.Abs(sizeY - avgSize);
            
            return diffX.CompareTo(diffY);
        });
        
        var totalCount = updateArr.Count;
        var completedCount = 0;
        
        foreach (var item in sortedDownloads)
        {
            StartCoroutine(DownloadWithConcurrencyControl(item.Value.root, item.Value.saveroot, item.Value, (long downSize, bool success) => {
                if (success)
                {
                    lock (downloadCountLock)
                    {
                        mCurSize += downSize;
                        completedCount++;
                    }
                    
                    if (completedCount >= totalCount)
                    {
                        StartCoroutine(DownloadSuccess());
                    }
                }
            }));
        }
        
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DownloadWithConcurrencyControl(string path, string savePath, AssetBundleInfo asset, Action<long, bool> callback)
    {
        var serverUrl = new Uri(path + asset.ABname).Host;
        var waitTime = serverResponseTimes.ContainsKey(serverUrl) ? 
            Math.Max(0.05f, serverResponseTimes[serverUrl] * 0.1f) : 0.05f;
        
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
            waitTime = Math.Min(waitTime * 1.1f, 0.5f);
        }
        
        yield return StartCoroutine(SaveAssetBundleOptimized(path, savePath, asset, callback));
        
        lock (downloadCountLock)
        {
            activeDownloads--;
        }
    }

    private bool ValidateFileMD5(string filePath, string expectedMD5)
    {
        if (!File.Exists(filePath)) return false;
        
        var fileInfo = new FileInfo(filePath);
        var cacheKey = filePath + "_" + fileInfo.LastWriteTime.Ticks + "_" + fileInfo.Length;
        
        if (md5Cache.TryGetValue(cacheKey, out var cachedMD5))
        {
            return cachedMD5.Equals(expectedMD5, StringComparison.OrdinalIgnoreCase);
        }
        
        StartCoroutine(ValidateFileMD5Async(filePath, expectedMD5, cacheKey));
        return true;
    }
    
    private IEnumerator ValidateFileMD5Async(string filePath, string expectedMD5, string cacheKey)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var buffer = new byte[BUFFER_SIZE];
                int bytesRead;
                
                while ((bytesRead = stream.Read(buffer, 0, BUFFER_SIZE)) > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    yield return new WaitForEndOfFrame();
                }
                
                md5.TransformFinalBlock(new byte[0], 0, 0);
                var hashString = BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                md5Cache[cacheKey] = hashString;
            }
        }
    }

    private void UpdateVersionFiles()
    {
        if (!string.IsNullOrEmpty(DllVersion))
        {
            var dllDir = Path.GetDirectoryName(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionNO);
            if (!Directory.Exists(dllDir)) Directory.CreateDirectory(dllDir);
            File.WriteAllText(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionNO, DllVersion);
        }
        
        if (!string.IsNullOrEmpty(DllAsset))
        {
            var dllDir = Path.GetDirectoryName(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionPath);
            if (!Directory.Exists(dllDir)) Directory.CreateDirectory(dllDir);
            File.WriteAllText(Application.persistentDataPath + "/DLL/" + ABLoadConfig.VersionPath, DllAsset);
        }

        if (!string.IsNullOrEmpty(resVersion))
        {
            var resDir = Path.GetDirectoryName(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionNO);
            if (!Directory.Exists(resDir)) Directory.CreateDirectory(resDir);
            File.WriteAllText(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionNO, resVersion);
        }
        
        if (!string.IsNullOrEmpty(resAsset))
        {
            var resDir = Path.GetDirectoryName(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionPath);
            if (!Directory.Exists(resDir)) Directory.CreateDirectory(resDir);
            File.WriteAllText(Application.persistentDataPath + "/AssetBundle_ALL/" + ABLoadConfig.VersionPath, resAsset);
        }
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
    
    private string GetETAString(float currentSpeed)
    {
        if (currentSpeed <= 0 || mTotalSize <= mCurSize) return "计算中...";
        
        var remainingBytes = mTotalSize - mCurSize;
        var etaSeconds = remainingBytes / currentSpeed;
        
        if (etaSeconds > 3600)
        {
            return $"{etaSeconds / 3600:0.1}小时";
        }
        else if (etaSeconds > 60)
        {
            return $"{etaSeconds / 60:0.1}分钟";
        }
        else
        {
            return $"{etaSeconds:0}秒";
        }
    }

    private IEnumerator CopyStreamingAssetsFile()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            Directory.Delete(Application.persistentDataPath, true);
        }
        Directory.CreateDirectory(Application.persistentDataPath);

        mLoadingInfo = "解压资源...";
        mProgress = 0;
        var list = new List<string>();
        
        using (UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/folder.byte"))
        {
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                yield break;
            }
            
            var arr = request.downloadHandler.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            list.AddRange(arr);
        }

        var copyOperations = new List<CopyOperation>();
        
        for (int i = 0; i < list.Count; i++)
        {
            string path = list[i] + "/";
            var assetsPath = Application.persistentDataPath + "/" + path + "assets.bytes";
            
            if (!File.Exists(assetsPath))
            {
                var dirPath = Application.persistentDataPath + "/DIR/" + path;
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                using (UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + path + "assets.bytes"))
                {
                    yield return request.SendWebRequest();
                        
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        continue;
                    }
                        
                    File.WriteAllBytes(dirPath + "assets.bytes", request.downloadHandler.data);
                    var json = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(request.downloadHandler.text);

                    foreach (var asset in json)
                    {
                        copyOperations.Add(new CopyOperation
                        {
                            sourcePath = Application.streamingAssetsPath + "/" + path + asset.ABname,
                            destPath = dirPath + asset.ABname,
                            size = asset.Size
                        });
                    }
                }
            }
        }

        copyOperations.Sort((a, b) => a.size.CompareTo(b.size));
        
        var totalCount = copyOperations.Count;
        var currentNum = 0;
        var processedInBatch = 0;
        const int batchSize = 20;
        
        foreach (var operation in copyOperations)
        {
            yield return StartCoroutine(CopyFileOptimized(operation.sourcePath, operation.destPath));
            currentNum++;
            processedInBatch++;
            
            mProgress = (float)currentNum / totalCount;
            mLoadingInfo = "解压资源: " + currentNum + "/" + totalCount;
            
            if (processedInBatch >= batchSize)
            {
                processedInBatch = 0;
                yield return new WaitForEndOfFrame();
            }
        }

        if (Directory.Exists(Application.persistentDataPath + "/DIR"))
        {
            yield return StartCoroutine(CopyFolderWithProgress(Application.persistentDataPath + "/DIR", Application.persistentDataPath, mProgress, 1.0f));
            Directory.Delete(Application.persistentDataPath + "/DIR", true);
        }
        
        File.WriteAllText(Application.persistentDataPath + "/resversion.ver", Application.version);
    }

    private class CopyOperation
    {
        public string sourcePath;
        public string destPath;
        public long size;
    }

    void Update()
    {
        if (Time.time - lastProgressUpdate < PROGRESS_UPDATE_INTERVAL) return;
        lastProgressUpdate = Time.time;
        
        if (isUpdate)
        {
            var currentSize = mCurSize;
            var downloadSpeed = (currentSize - mlastSize) * (1f / PROGRESS_UPDATE_INTERVAL);
            
            speedHistory.Enqueue(downloadSpeed);
            if (speedHistory.Count > SPEED_HISTORY_SIZE)
            {
                speedHistory.Dequeue();
            }
            
            float avgSpeed = 0;
            foreach (var speed in speedHistory)
            {
                avgSpeed += speed;
            }
            avgSpeed /= speedHistory.Count;
            
            var targetProgress = Math.Min(0.85f, (float)currentSize / mTotalSize);
            mProgress = Mathf.Lerp(mProgress, targetProgress, Time.deltaTime * 2f);
            
            if (mProgress < 0.85f)
            {
                mLoadingInfo = string.Format("下载中 {0}/{1}", 
                    GetDownSpdStr(currentSize), 
                    GetDownSpdStr(mTotalSize));
                
                if (uiLabelSpeed != null)
                {
                    uiLabelSpeed.text = $"速度: {GetDownSpdStr((long)downloadSpeed)}/s";
                }
                
                if (uiLabelETA != null)
                {
                    uiLabelETA.text = $"剩余: {GetETAString(avgSpeed)}";
                }
            }
            
            mlastSize = currentSize;
        }
        
        setProgress(mProgress);
        setLoadingInfo(mLoadingInfo);
    }

    private IEnumerator CopyFolderWithProgress(string srcPath, string tarPath, float startProgress, float endProgress)
    {
        if (!Directory.Exists(srcPath))
        {
            yield break;
        }

        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        var allFiles = GetAllFiles(srcPath);
        var totalFiles = allFiles.Count;
        var currentFile = 0;

        foreach (var file in allFiles)
        {
            var relativePath = file.Substring(srcPath.Length + 1);
            var destFile = Path.Combine(tarPath, relativePath);
            var destDir = Path.GetDirectoryName(destFile);
            
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            if (Application.platform != RuntimePlatform.Android || !file.Contains("StreamingAssets"))
            {
                File.Copy(file, destFile, true);
            }
            else
            {
                yield return StartCoroutine(CopyFileOptimized(file, destFile));
            }

            currentFile++;
            mProgress = startProgress + (endProgress - startProgress) * currentFile / totalFiles;
            mLoadingInfo = $"安装资源: {currentFile}/{totalFiles}";
            
            if (currentFile % 5 == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private List<string> GetAllFiles(string path)
    {
        var result = new List<string>();
        if (Directory.Exists(path))
        {
            result.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
        }
        return result;
    }

    private IEnumerator CopyFileOptimized(string sourcePath, string destinationPath)
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
                    yield break;
                }
            }
        }
        else
        {
            if (File.Exists(sourcePath))
            {
                var fileInfo = new FileInfo(sourcePath);
                if (fileInfo.Length > 10 * 1024 * 1024)
                {
                    yield return StartCoroutine(CopyLargeFile(sourcePath, destinationPath));
                    yield break;
                }
                else
                {
                    fileData = File.ReadAllBytes(sourcePath);
                }
            }
            else
            {
                yield break;
            }
        }
        
        if (fileData != null && fileData.Length > 0)
        {
            string destinationFolder = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            
            File.WriteAllBytes(destinationPath, fileData);
        }
    }
    
    private IEnumerator CopyLargeFile(string sourcePath, string destinationPath)
    {
        var destinationFolder = Path.GetDirectoryName(destinationPath);
        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }
        
        using (var sourceStream = File.OpenRead(sourcePath))
        using (var destStream = File.Create(destinationPath))
        {
            var buffer = new byte[BUFFER_SIZE];
            int bytesRead;
            var totalBytes = sourceStream.Length;
            var copiedBytes = 0L;
            
            while ((bytesRead = sourceStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
            {
                destStream.Write(buffer, 0, bytesRead);
                copiedBytes += bytesRead;
                
                if (copiedBytes % (BUFFER_SIZE * 10) == 0)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    public void setProgress(float percent)
    {
        if (Math.Abs(uiSlider.value - percent) > 0.001f)
        {
            uiLabelProgress.text = Mathf.CeilToInt(percent * 100) + "%";
            uiSlider.value = percent;
        }
    }

    public void setLoadingInfo(string message)
    {
        if (uiLabelInfo.text != message)
        {
            uiLabelInfo.text = message;
        }
    }

    private IEnumerator DownloadSuccess()
    {
        mProgress = 0.85f;
        mLoadingInfo = "正在安装资源...";
        isUpdate = false;
        
        if (!string.IsNullOrEmpty(DllVersion))
        {
            var dllDir = Application.persistentDataPath + "/DIR/DLL";
            if (Directory.Exists(dllDir))
            {
                File.WriteAllText(dllDir + "/" + ABLoadConfig.VersionNO, DllVersion);
                File.WriteAllText(dllDir + "/" + ABLoadConfig.VersionPath, DllAsset);
            }
        }

        if (!string.IsNullOrEmpty(resVersion))
        {
            var resDir = Application.persistentDataPath + "/DIR/AssetBundle_ALL";
            if (Directory.Exists(resDir))
            {
                File.WriteAllText(resDir + "/" + ABLoadConfig.VersionNO, resVersion);
                File.WriteAllText(resDir + "/" + ABLoadConfig.VersionPath, resAsset);
            }
        }

        if (Directory.Exists(Application.persistentDataPath + "/DIR"))
        {
            mProgress = 0.87f;
            yield return StartCoroutine(CopyFolderWithProgress(Application.persistentDataPath + "/DIR", Application.persistentDataPath, 0.87f, 0.98f));
            Directory.Delete(Application.persistentDataPath + "/DIR", true);
        }
        
        mProgress = 1.0f;
        mLoadingInfo = "安装完成!";
        
        md5Cache.Clear();
        speedHistory.Clear();
        
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(LoadDll());
    }
    private IEnumerator LoadDll()
{
    // Skip HybridCLR cho PC platform
    if (!IsPCPlatform())
    {
#if !UNITY_EDITOR
        List<string> aotMetaAssemblyFiles = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll"
        };
        
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            var fullPath = Application.persistentDataPath + "/DLL/" + aotDllName + ".bytes";
            
            if (File.Exists(fullPath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get("file://" + fullPath))
                {
                    request.timeout = 30;
                    yield return request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(request.downloadHandler.data, mode);
                    }
                }
            }
        }

        List<string> hotMetaAssemblyFiles = new List<string>()
        {
            "HotUpdate.dll.bytes"
        };
        
        foreach (var item in hotMetaAssemblyFiles)
        {
            var fullPath = Application.persistentDataPath + "/DLL/" + item;
            
            if (File.Exists(fullPath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get("file://" + fullPath))
                {
                    request.timeout = 30;
                    yield return request.SendWebRequest();
                    
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        System.Reflection.Assembly.Load(request.downloadHandler.data);
                    }
                }
            }
        }
        
        yield return new WaitForSecondsRealtime(0.1f);
#endif
    }
    
    // Load GameEntrance - logic cũ giữ nguyên
#if UNITY_EDITOR
    var pre = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Arts_ALL/GameRes/Prefabs/UI/GameEntrance.prefab", typeof(GameObject)) as GameObject;
    var go = Instantiate(pre);
    go.SetActive(true);
    DontDestroyOnLoad(go);
    Destroy(gameObject);
#else
    var bundlePath = Application.persistentDataPath + "/AssetBundle_ALL/assets^arts_all^gameres^prefabs^ui^gameentrance.kb";
    AssetBundle loadingbundle = AssetBundle.LoadFromFile(bundlePath);
    if (loadingbundle != null)
    {
        var pre = loadingbundle.LoadAsset("gameentrance", typeof(GameObject)) as GameObject;
        var go = Instantiate(pre);
        go.SetActive(true);
        DontDestroyOnLoad(go);
    }
    Destroy(gameObject);
#endif
    yield break;
}

    private IEnumerator SaveAssetBundleOptimized(string path, string savePath, AssetBundleInfo asset, Action<long, bool> callback)
    {
        string originPath = path + asset.ABname;
        string fullSavePath = savePath + asset.ABname;
        string tempPath = fullSavePath + ".tmp";
        
        int retryCount = MAX_RETRY_COUNT;
        float retryDelay = 0.5f;
        var serverUrl = new Uri(originPath).Host;
        var requestStartTime = Time.time;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(originPath))
            {
                request.timeout = Math.Max(30, (int)(asset.Size / (1024 * 1024)) * 5);
                request.SetRequestHeader("User-Agent", "Unity-Game-Client");
                
                var asyncOp = request.SendWebRequest();
                
                while (!asyncOp.isDone)
                {
                    if (request.downloadedBytes > 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                var responseTime = Time.time - requestStartTime;
                serverResponseTimes[serverUrl] = responseTime;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    retryCount--;
                    
                    if (retryCount > 0)
                    {
                        yield return new WaitForSeconds(retryDelay);
                        retryDelay = Math.Min(retryDelay * 2, 5f);
                        continue;
                    }
                    else
                    {
                        callback?.Invoke(0, false);
                        yield break;
                    }
                }

                byte[] results = request.downloadHandler.data;
                
                if (results == null || results.Length == 0)
                {
                    retryCount--;
                    continue;
                }
                
                var dir = Path.GetDirectoryName(fullSavePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllBytes(tempPath, results);
                
                if (File.Exists(tempPath) && new FileInfo(tempPath).Length == results.Length)
                {
                    if (File.Exists(fullSavePath))
                    {
                        File.Delete(fullSavePath);
                    }
                    File.Move(tempPath, fullSavePath);
                    
                    callback?.Invoke(results.Length, true);
                    yield break;
                }
                else
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    retryCount--;
                }
            }
        }
        
        callback?.Invoke(0, false);
    }

    private IEnumerator DownloadVersionFile(string path, Action<string> callback)
    {
        int retryCount = MAX_RETRY_COUNT;
        float retryDelay = 0.3f;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                request.timeout = 15;
                request.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                request.SetRequestHeader("Pragma", "no-cache");
                request.SetRequestHeader("Expires", "0");
                
                yield return request.SendWebRequest();
                
                if (request.result != UnityWebRequest.Result.Success)
                {
                    retryCount--;
                    
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
                    }
                }
            }
        }
        
        callback?.Invoke("");
    }
    
    private void OnDestroy()
    {
        md5Cache?.Clear();
        speedHistory?.Clear();
        serverResponseTimes?.Clear();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && isUpdate)
        {
            downloadStartTime = DateTime.Now;
            mlastSize = mCurSize;
        }
    }
}