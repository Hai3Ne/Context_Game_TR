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
    public UILabel uiLabelProgress, uiLabelInfo;
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
    
    private int maxConcurrentDownloads = 5;
    private int activeDownloads = 0;
    private readonly object downloadCountLock = new object();
    private readonly Dictionary<string, string> md5Cache = new Dictionary<string, string>();

    void Start()
    {
        StartCoroutine(CheckFirst());
    }

    private IEnumerator CheckFirst()
    {
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

    private IEnumerator CheckUpdate()
    {
        mLoadingInfo = "检查版本更新...";
        mProgress = 1;
        
        var updateArr = new Dictionary<string, AssetBundleInfo>();
        long totalSize = 0;
        
        yield return StartCoroutine(CheckDllUpdate(updateArr, (size) => totalSize += size));
        yield return StartCoroutine(CheckResourceUpdate(updateArr, (size) => totalSize += size));
        
        if (updateArr.Count > 0)
        {
            mTotalSize = totalSize;
            mCurSize = 0;
            mlastSize = 0;
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
        
        if (newVersion != localVersion)
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
                DllAsset = text;
                newList = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
            }));
            
            var existingFileDict = new Dictionary<string, AssetBundleInfo>();
            foreach (var item in list)
            {
                existingFileDict[item.ABname] = item;
            }
            
            foreach (var newItem in newList)
            {
                var needUpdate = false;
                var localFilePath = Application.persistentDataPath + "/DLL/" + newItem.ABname;
                
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
                    newItem.root = GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/";
                    newItem.saveroot = Application.persistentDataPath + "/DIR/DLL/";
                    updateArr.Add(GameParams.Instance.AbDownLoadSite + "/ANDROID/DLL/" + newItem.ABname, newItem);
                }
            }
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
        
        if (newVersion != localVersion)
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
                resAsset = text;
                newList = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
            }));
            
            var existingFileDict = new Dictionary<string, AssetBundleInfo>();
            foreach (var item in list)
            {
                existingFileDict[item.ABname] = item;
            }
            
            foreach (var newItem in newList)
            {
                var needUpdate = false;
                var localFilePath = Application.persistentDataPath + "/AssetBundle_ALL/" + newItem.ABname;
                
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
                    newItem.root = GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/";
                    newItem.saveroot = Application.persistentDataPath + "/DIR/AssetBundle_ALL/";
                    updateArr.Add(GameParams.Instance.AbDownLoadSite + "/ANDROID/AssetBundle_ALL/" + newItem.ABname, newItem);
                }
            }
        }
    }

    private IEnumerator StartOptimizedDownload(Dictionary<string, AssetBundleInfo> updateArr)
    {
        mProgress = 0;
        mLoadingInfo = "准备下载...";
        var totalCount = updateArr.Count;
        var completedCount = 0;
        var sortedDownloads = new List<KeyValuePair<string, AssetBundleInfo>>(updateArr);
        sortedDownloads.Sort((x, y) => x.Value.Size.CompareTo(y.Value.Size));
        
        foreach (var item in sortedDownloads)
        {
            StartCoroutine(DownloadWithConcurrencyControl(item.Value.root, item.Value.saveroot, item.Value, (int downSize) => {
                isUpdate = true;
                completedCount++;
                
                lock (downloadCountLock)
                {
                    mCurSize += downSize;
                }
                
                mProgress = (float)completedCount / totalCount;
                
                if (completedCount >= totalCount)
                {
                    StartCoroutine(DownloadSuccess());
                }
            }));
        }
        
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator DownloadWithConcurrencyControl(string path, string savePath, AssetBundleInfo asset, Action<int> callback)
    {
        var waitTime = 0.05f;
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
        
        yield return StartCoroutine(SaveAssetBundle(path, savePath, asset, callback));
        
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
        
        if (md5Cache.TryGetValue(cacheKey, out var value))
        {
            return value.Equals(expectedMD5, StringComparison.OrdinalIgnoreCase);
        }
        
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                md5Cache[cacheKey] = hashString;
                return hashString.Equals(expectedMD5, StringComparison.OrdinalIgnoreCase);
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
        const int batchSize = 25;
        
        foreach (var operation in copyOperations)
        {
            yield return StartCoroutine(CopyFile(operation.sourcePath, operation.destPath));
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
            yield return StartCoroutine(CopyFolder(Application.persistentDataPath + "/DIR", Application.persistentDataPath));
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
        if (isUpdate)
        {
            var currentSize = mCurSize;
            var downloadSpeed = (currentSize - mlastSize) * 2;
            
            mLoadingInfo = string.Format("下载文件{0}/{1} 速度：{2}/s 安装中...", 
                GetDownSpdStr(currentSize), 
                GetDownSpdStr(mTotalSize), 
                GetDownSpdStr(downloadSpeed));
            mlastSize = currentSize;
        }
        setProgress(mProgress);
        setLoadingInfo(mLoadingInfo);
    }

    private IEnumerator CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            yield break;
        }

        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        var files = Directory.GetFiles(srcPath);
        foreach (var file in files)
        {
            string destFile = Path.Combine(tarPath, Path.GetFileName(file));
            
            if (Application.platform != RuntimePlatform.Android || !file.Contains("StreamingAssets"))
            {
                File.Copy(file, destFile, true);
            }
            else
            {
                yield return StartCoroutine(CopyFile(file, destFile));
            }
        }

        var folders = Directory.GetDirectories(srcPath);
        foreach (var folder in folders)
        {
            string destDir = Path.Combine(tarPath, Path.GetFileName(folder));
            yield return StartCoroutine(CopyFolder(folder, destDir));
        }
    }

    private IEnumerator CopyFile(string sourcePath, string destinationPath)
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
                fileData = File.ReadAllBytes(sourcePath);
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
            yield return StartCoroutine(CopyFolder(Application.persistentDataPath + "/DIR", Application.persistentDataPath));
            Directory.Delete(Application.persistentDataPath + "/DIR", true);
        }
        
        isUpdate = false;
        mLoadingInfo = "下载完毕,解压完成...";
        
        yield return StartCoroutine(LoadDll());
    }

    private IEnumerator LoadDll()
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
            var path = "/DLL/" + aotDllName + ".bytes";
            var fullPath = Application.persistentDataPath + path;
            
            if (File.Exists(fullPath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get("file://" + fullPath))
                {
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
        
#if UNITY_EDITOR
        var pre = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Arts_ALL/GameRes/Prefabs/UI/GameEntrance.prefab", typeof(GameObject)) as GameObject;
        var go = Instantiate(pre);
        go.SetActive(true);
        DontDestroyOnLoad(go);
        Destroy(gameObject);
#else
        AssetBundle loadingbundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AssetBundle_ALL/assets^arts_all^gameres^prefabs^ui^gameentrance.kb");
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

    private IEnumerator SaveAssetBundle(string path, string savePath, AssetBundleInfo asset, Action<int> DownLoad = null)
    {
        string originPath = path + asset.ABname;
        string fullSavePath = savePath + asset.ABname;
        int retryCount = 5;
        float retryDelay = 0.5f;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(originPath))
            {
                request.timeout = 45;
                request.SetRequestHeader("User-Agent", "Unity-Game-Client");
                
                yield return request.SendWebRequest();

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
                        DownLoad?.Invoke(0);
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

                File.WriteAllBytes(fullSavePath, results);
                
                if (File.Exists(fullSavePath) && new FileInfo(fullSavePath).Length == results.Length)
                {
                    DownLoad?.Invoke(results.Length);
                    yield break;
                }
                else
                {
                    retryCount--;
                }
            }
        }
    }

    private IEnumerator DownloadVersionFile(string path, Action<string> DownLoad = null)
    {
        int retryCount = 5;
        float retryDelay = 0.3f;
        
        while (retryCount > 0)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(path))
            {
                request.timeout = 20;
                request.SetRequestHeader("Cache-Control", "no-cache");
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
                        DownLoad?.Invoke(text);
                        yield break;
                    }
                    else
                    {
                        retryCount--;
                    }
                }
            }
        }
        
        DownLoad?.Invoke("");
    }
}