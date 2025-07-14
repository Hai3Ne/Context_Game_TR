using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections;
using System.Threading;
using System.Net;
using System;
using System.Text;
using Kubility;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Core;

public struct ResVersion
{
    public uint     ResCrc;
    public uint     ResSize;
}


public class VersionManager : MonoSingleTon<VersionManager>
{
	public static string localVersion = "0.0.4";
    public VoidCall mCall;

    public static long MakeVerionNo(string versionStr){
		string[] verarr = versionStr.Split ('.');
		int main = int.Parse (verarr [0]);
		int sub = int.Parse (verarr [1]);
		int third = int.Parse (verarr [2]);

		return (long)(main * 1000000 + sub * 100 + third);
	}

	public static long MakeVerionNo(int main, int sub, int third){
		return (long)(main * 1000000 + sub * 100 + third);
	}

	[HideInInspector]
	public GameObject goProgress;
    //private LinkedList<string> DownLoadQueue = new LinkedList<string> ();
    public VersionCheckUI mChkUI;
	private VersionJsonInfo versionInfo;
    //private bool isNetError = false;

	private Action Success_callback;
	private Action<bool> isNeedUpdate_callback;


    //private int mCurDownCount;//当前下载数
    private int mCurFinshCount;//下载完成数
    private int mTotalDownCount;//
    public Queue<DownLoadInfo> mAllDownList = new Queue<DownLoadInfo>();//下载列表
    public List<DownLoadInfo> mDownList = new List<DownLoadInfo>();//下载列表
    public bool mIsError = false;//是否下载错误
    //private Queue<string> mDownList = new Queue<string>();
    //private List<string> mErrorList = new List<string>();//下载错误列表

    private float mProgress;
    private string mLoadingInfo;
    private long mTotalSize;
    private long mCurSize;//已下载大小

	string DownLoadLogFile { get { return KApplication.persistentDataPath + "/Log.txt";}}

    private Action mMainCall;//线程之间调用处理
    private void DownLoadError(DownLoadInfo info) {//下载出错处理
        //LogMgr.Log("下载失败:" + info.url + "   error:" + info.www.error);
        //info.ErrorCount++;
        //if (info.ErrorCount < 5) {
            info.StartDown();//下载失败重试3次
        //} else {
        //    if (this.mIsError == false) {
        //        this.mIsError = true;
        //        SystemMessageMgr.Instance.HandlePormptMsg("资源下载异常，是否重试", () => {
        //            for (int j = 0; j < this.mDownList.Count; j++) {
        //                this.mDownList[j].ErrorCount = 0;
        //                this.mDownList[j].StartDown();
        //            }
        //            this.mIsError = false;
        //        }, AppQuit);
        //    }
        //}
    }
    private void RefershDownLoadList() {//更新下载内容
        DownLoadInfo info;
        for (int i = this.mDownList.Count - 1; i >= 0; i--)
        {
            if (this.mIsError)
            {
                break;
            }
            info = this.mDownList[i];
            if (info.www == null)
            {
                info.StartDown();
            }
            else if (info.www.isDone)
            {
                if (info.IsError == false) {//下载完成
                    LogMgr.Log("下载完成:" + info.url);
                    this.mCurSize += info.size;
                    info.DownLoadFinish();
                    this.mDownList.RemoveAt(i);

                    this.mCurFinshCount++;
                    this.mProgress = (float)this.mCurFinshCount / this.mTotalDownCount;
                    //this.mLoadingInfo = string.Format("下载资源文件:{0}/{1}", this.mCurFinshCount, this.mTotalDownCount);

                    if (this.mAllDownList.Count > 0)
                    {
                        this.mDownList.Add(this.mAllDownList.Dequeue());
                    }
                    else if (this.mDownList.Count == 0)
                    {
                        this.DownFinish();
                    }
                }
                else
                {//下载失败
                    this.DownLoadError(info);
                }
            }
            else if (info.TimeOutCheck())
            {
                //超时检测
                info.Dispose();//超时断开
                this.DownLoadError(info);
            }
        }
    }
    private void DownFinish() {//下载完成
        TimeManager.DelayExec(1, () => {
            //KAssetBundleManger.Instance.ReLoadVersionFile();
            mChkUI.IsActive = false;
            //this.Success_callback.TryCall();
            if (mCall != null)
                mCall();
        });

        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    private string GetDownSpdStr(long size) {
        size = Math.Max(size, 0);
        if (size > 1000000) {//M
            return string.Format("{0:0.##}M", size / 1024f / 1024f);
        } else if (size > 1000) {//KB
            return string.Format("{0:0.##}KB", size / 1024f);
        } else {//B
            return string.Format("{0:0.##}B", size);
        }
    }
    private List<KeyValuePair<float, long>> size_list = new List<KeyValuePair<float, long>>();
    void Update()
    {
        if (this.mDownList.Count > 0 && this.mIsError == false)
        {
            this.RefershDownLoadList();
        }

        if (this.mDownList.Count > 0)
        {
            long _cur_size = this.mCurSize;
            for (int i = 0; i < this.mDownList.Count; i++)
            {
                _cur_size += this.mDownList[i].CurDownSize;
            }
            float _time = Time.realtimeSinceStartup-1;
            long _size;
            if (size_list.Count == 0)
            {
                _size = 0;
            }
            else
            {
                _size = long.MaxValue;
                for (int i = size_list.Count - 1; i >= 0; i--)
                {
                    if (size_list[i].Key >= _time)
                    {
                        _size = Math.Min(_size, size_list[i].Value);
                    } else {
                        size_list.RemoveAt(i);
                    }
                }
            }
            size_list.Add(new KeyValuePair<float, long>(Time.realtimeSinceStartup, _cur_size));
            this.mLoadingInfo = string.Format("下载文件{0}/{1} 下载速度：{2}/s 炮台正在组装中...", this.GetDownSpdStr(_cur_size), this.GetDownSpdStr(this.mTotalSize), this.GetDownSpdStr((_cur_size - _size) * 3 / 2));
        }

        if (this.mChkUI != null)
        {
            this.mChkUI.setProgress(this.mProgress);
            if (this.mLoadingInfo != null)
            {
                this.mChkUI.setLoadingInfo(this.mLoadingInfo);
            }
        }
        if (this.mMainCall != null)
        {
            this.mMainCall();
            this.mMainCall = null;
        }
    }

    private void AppQuit() {
        SDKMgr.ExitGame();
	}

	public void VersionCheck (Action<bool> callback, Action sucFunc)
	{
		this.Success_callback = sucFunc;
		this.isNeedUpdate_callback = callback;
		mChkUI.IsActive = true;

        //   UnCompressAssets(delegate
        //   {
       // Success_callback.Invoke();
            this.mLoadingInfo = "检查版本更新...";
            this.mProgress = 1;

            ResVersionManager.CheckVersion(GameEnum.All, Success_callback, isNeedUpdate_callback);
    //    });
    }

    void UnCompressAssets(Action callback)
    {
        var assetbytes = KAssetBundleManger.GetPlatformPersistentDataPath (ABLoadConfig.VersionPath);
        if (!File.Exists (assetbytes))
        {
            string destDir = KAssetBundleManger.GetPlatformPersistentDataPath (null);
            string srcDir = KAssetBundleManger.GetPlatformStreamingAssets ("");
            ThreadPool.QueueUserWorkItem ((cbvalue) => {
                try
                {
                    ExecuteUncompress (callback,srcDir, destDir);
                }
                catch (System.Exception exp)
                {
                    LogMgr.LogError(exp.Message);
                    LogMgr.LogError(exp.StackTrace);
                }
            });
        }
        else
        {
			callback.TryCall ();
        }
	}

	void ExecuteUncompress(Action callback,string srcDir, string destDir)
    {		
		if (Directory.Exists (destDir))
			Directory.Delete (destDir, true);
		Directory.CreateDirectory (destDir);
		string fname = "";
		if (Application.platform == RuntimePlatform.Android)
        {
			long totalCount = 1;
			IEnumerator enumtor = KAssetBundleManger.GetApkEntryEnumerator (Application.dataPath, out totalCount);
			long currentNum = 0;
			string destLuaDir = destDir + "/Lua";
			if (!Directory.Exists (destLuaDir))
				Directory.CreateDirectory (destLuaDir);
			while (enumtor.MoveNext ())
            {
				ZipEntry entry = (ZipEntry)enumtor.Current;
				byte[] buffer = KAssetBundleManger.ReadApk (Application.dataPath, entry.Name);
				if (entry.Name.StartsWith ("assets/KB"))
                {
					fname = entry.Name.Substring (10);
					File.WriteAllBytes (destDir + "/" + fname, buffer);	
				}
                else if (entry.Name.StartsWith ("assets/Lua"))
                {
					fname = entry.Name.Substring (11);
					File.WriteAllBytes (destLuaDir + "/" + fname, buffer);
				}
				currentNum++;
				this.mProgress = (float)currentNum / totalCount;
                this.mLoadingInfo = "解压数据: " + currentNum + "/" + totalCount;
			}
		}
        else
        {
            string[] files;
            if (Directory.Exists(srcDir + "KB"))
            {
                files = Directory.GetFiles(srcDir + "KB", "*.kb");
            }
            else
            {
                files = new string[0];
            }
            string[] files2;
            if (Directory.Exists(srcDir + "Lua"))
            {
                files2 = Directory.GetFiles(srcDir + "Lua", "*.unity3d");
            }
            else
            {
                files2 = new string[0];
            }
			int totlaFiles = files.Length + files2.Length + 1;
			int k = 0;
            if (File.Exists(srcDir + "KB/" + ABLoadConfig.VersionPath))
            {
                File.Copy(srcDir + "KB/" + ABLoadConfig.VersionPath, destDir + "/" + ABLoadConfig.VersionPath);
            }
			for (int i = 0; i < files.Length; i++)
            {				
				File.Copy (files [i], destDir + "/" + Path.GetFileName (files [i]));
				this.mProgress = (float)k / totlaFiles;
                k++;
                this.mLoadingInfo = "解压数据: "+ (k) + "/" + totlaFiles;
			}
			destDir = destDir + "/Lua";
			if (Directory.Exists(destDir))
				Directory.Delete (destDir, true);
			Directory.CreateDirectory (destDir);
			for (int i = 0; i < files2.Length; i++)
            {
                File.Copy(files2[i], destDir + "/" + Path.GetFileName(files2[i]));
                this.mProgress = (float)k / totlaFiles;
                k++;
                this.mLoadingInfo = "解压数据: "+ k + "/" + totlaFiles;
			}
		}
        this.mMainCall = callback;
	}

    private void StartDownLoad()
    {
        this.mCurSize = 0;
        this.mDownList.Clear();
        DownLoadInfo info;
        while (this.mAllDownList.Count > 0 && this.mDownList.Count < 10)
        {
            info = this.mAllDownList.Dequeue();
            info.StartDown();
            this.mDownList.Add(info);
        }
        this.mProgress = 0;
        this.mLoadingInfo = "准备下载中...";
    }

    public void InitData(string local_path, string net_path, List<AssetBundleInfo> list, VoidCall call)
    {
        mCall = call;
        string url;
        string output_path;
        mCurFinshCount = 0;
        if (Directory.Exists(local_path) == false)
        {
            Directory.CreateDirectory(local_path);
        }
        this.mTotalSize = 0;
        foreach (var item in list)
        {
            url = net_path + item.ABname;
            output_path = local_path + item.ABname;
            this.mTotalSize += item.Size;
            this.mAllDownList.Enqueue(new DownLoadInfo(url, output_path, item.Size));
        }

        this.mTotalDownCount = mAllDownList.Count;
        if (this.mTotalDownCount > 0) {
            this.StartDownLoad();
        } else {
            this.DownFinish();
        }
    }

    public static void CheckVersion(Action<bool> call) {//检查版本是否需要更新
#if UNITY_EDITOR
        call(false);
        return;
#endif
        string url = ConstValue.ServerConfURL + "?" + UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        GameUtils.DownLoadWWW(TimeManager.Mono, url, (www) => {
            if (www.isDone && string.IsNullOrEmpty(www.error)) {
                var version = GameParams.CheckVersion(www.downloadHandler.data);
                if (version == 0)
                {
                    call(false);
                }
                else {
                    uint localClientVersion = GameConfig.ClientVersionCode;
                    if (version > localClientVersion)
                    {
                        call(true);
                    }
                    else
                    {
                        call(false);
                    }
                }
      
               
            } else {
                //检查错误直接不更新
                call(false);
            }
        });
    }
    public static void ShowVersionTick() {//显示版本更新提示
        GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ui_bg"), UI.UIRoot.transform);
        SystemMessageMgr.Instance.DialogShowByText("有新的版本需要更新.", () => {
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(ConstValue.ApkDownURL)) {
                Application.OpenURL(GameParams.Instance.downloadAppUrl);
                SDKMgr.ExitGame();
            } else {
                string local_path = string.Format("{0}/3dfish.apk", KApplication.persistentDataPath);
                GameObject obj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("UI_download"), UI.UIRoot.transform);
                UI._init_ui<UI_download>(obj).InitData(local_path, ConstValue.ApkDownURL, () => {
                    //启动APK
                    if (SDKMgr.Instance.mSysSDK.InstallApk(local_path) == false) {
                        Application.OpenURL(GameParams.Instance.downloadAppUrl);
                    }
                    SDKMgr.ExitGame();
                });
                UI.mUIList.Clear();
            }
#elif IOS_IAP
            Application.OpenURL(ConstValue.AppStoreDownloadURL);
            SDKMgr.ExitGame();
#else
            Application.OpenURL(GameParams.Instance.downloadAppUrl);
            SDKMgr.ExitGame();
#endif
        });
    }
}