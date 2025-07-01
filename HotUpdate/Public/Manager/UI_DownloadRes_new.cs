using Kubility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UI_DownloadRes_new : UILayer {
    public UISlider mSliderBar;//下载进度显示
    public UILabel mLbBar;//下载进度文本

    public VoidCall mCall;
    private int mCurFinshCount;//下载完成数
    private int mTotalDownCount;//
    public Queue<DownLoadInfo> mAllDownList = new Queue<DownLoadInfo>();//下载列表
    public List<DownLoadInfo> mDownList = new List<DownLoadInfo>();//下载列表
    public bool mIsError = false;//是否下载错误
    private float mProgress;
    private string mLoadingInfo;
    private long mTotalSize = 0;
    private long mCurSize;//已下载大小
    private string mDownLoadEndStr;
    private LoadType mLoadType;
    float mLoadConfigProgress;
    public GameObject mBtnCancel;
    public UILabel mlbCancel;

    /// <summary>
    /// 除了3D捕鱼资源外的其他资源下载
    /// </summary>
    /// <param name="local_path"></param>
    /// <param name="net_path"></param>
    /// <param name="list"></param>
    /// <param name="call"></param>
    public void InitData(string local_path, string net_path, List<AssetBundleInfo> list, VoidCall call, GameEnum type)
    {
        this.mCall = call;
        mLoadType = LoadType.DownLoad;
        if (type == GameEnum.Fish_3D)
            mDownLoadEndStr = "正在加载配置...";
        else
            mDownLoadEndStr = "正在进入游戏...";
        string url;
        string output_path;
        mAllDownList.Clear();
        mTotalSize = 0;
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
        this.StartDownLoad();
    }

    public void LoadConfig(float progress)
    {
        mLoadType = LoadType.LoadConfig;
        mLoadConfigProgress = progress;
    }

    /// <summary>
    /// 3D捕鱼资源下载
    /// </summary>
    //public void Init3DFishDownLoadInfo(Queue<string> queue, Dictionary<string, long> assetsSizeMap, VoidCall cal)
    //{
    //    mCall = cal;
    //    mAllDownList.Clear();
    //    mTotalSize = 0;
    //    string filename;
    //    long size;
    //    while (queue.Count > 0)
    //    {
    //        filename = queue.Dequeue();
    //        string url = KAssetBundleManger.GetPlatformAssetBundleDownLoadPath(filename);
    //        string outputPath;

    //        if (filename.EndsWith(".unity3d"))
    //        {
    //            //lua文件
    //            string localPath = "";
    //            int idx = filename.IndexOf("Lua");
    //            if (idx >= 0)
    //            {
    //                localPath = filename.Substring(idx + 4);
    //            }
    //            outputPath = KAssetBundleManger.GetPlatformPersistentDataPath("/Lua/" + localPath);
    //        }
    //        else
    //        {
    //            outputPath = KAssetBundleManger.GetPlatformPersistentDataPath(Path.GetFileName(filename));
    //        }
    //        size = assetsSizeMap.TryGet(filename);
    //        mTotalSize += size;
    //        mAllDownList.Enqueue(new DownLoadInfo(url, outputPath, size));
    //    }
    //    StartDownLoad();
    //}

    private void StartDownLoad() {
        this.mCurSize = 0;
        this.mDownList.Clear();
        DownLoadInfo info;
        while (this.mAllDownList.Count > 0 && this.mDownList.Count < 5) {
            info = this.mAllDownList.Dequeue();
            info.StartDown();
            this.mDownList.Add(info);
        }

        //this.StartCoroutine(this.DownLoad(queue));
        this.mSliderBar.value = 0;
        this.mLbBar.text = "准备下载中...";
    }

    private void DownLoadError(DownLoadInfo info)
    {
        if (info == null )
        {
            LogMgr.Log("有文件下载失败:");
            return;
        }

        if (info.www == null)
        {
            LogMgr.Log("下载失败:" + info.url);
            return;
        }

        //下载出错处理
        LogMgr.Log("下载失败:" + info.url + "   error:" + info.www.error);
        info.ErrorCount++;
        info.StartDown();//下载失败重试3次
    }
    private string GetDownSpdStr(long size) {
        if (size < 0) {
            size = 0;
        }
        if (size > 1000000) {//M
            return string.Format("{0:0.##}M", size / 1024f / 1024f);
        } else if (size > 1000) {//KB
            return string.Format("{0:0.##}KB", size / 1024f);
        } else {//B
            return string.Format("{0:0.##}B", size);
        }
    }
    private void RefershDownLoadList()
    {
        //更新下载内容
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
                if (info.IsError == false)
                {
                    //下载完成
                    LogMgr.Log("下载完成:" + info.url);
                    this.mCurSize += info.size;
                    info.DownLoadFinish();
                    this.mDownList.RemoveAt(i);

                    this.mCurFinshCount++;
                    //this.mProgress = (float)this.mCurFinshCount / this.mTotalDownCount;
                    //this.mLoadingInfo = string.Format("下载资源文件:{0}/{1}", this.mCurFinshCount, this.mTotalDownCount);

                    if (this.mAllDownList.Count > 0)
                    {
                        this.mDownList.Add(this.mAllDownList.Dequeue());
                    }
                    else if (this.mDownList.Count == 0)
                    {
                        //下载完成
                        TimeManager.DelayExec(0.5f, () =>
                        {
                            if (mCall !=null)
                                mCall();
                        });
                        this.mProgress = 1;
                        this.mLoadingInfo = mDownLoadEndStr;
                        Resources.UnloadUnusedAssets();
                        GC.Collect();
                    }
                }
                else
                {
                    //下载失败
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
    private List<KeyValuePair<float, long>> size_list = new List<KeyValuePair<float, long>>();
    public void Update()
    {
        if (mLoadType == LoadType.DownLoad)//下载资源
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
                float _time = Time.realtimeSinceStartup - 1;
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
                        }
                        else
                        {
                            size_list.RemoveAt(i);
                        }
                    }
                }
                size_list.Add(new KeyValuePair<float, long>(Time.realtimeSinceStartup, _cur_size));
                if (this.mTotalSize > 0)
                {
                    this.mProgress = (float)_cur_size / this.mTotalSize;
                }
                else
                {
                    this.mProgress = 0;
                }
                this.mLoadingInfo = string.Format("下载文件{0}/{1} 下载速度：{2}/s", this.GetDownSpdStr(_cur_size), this.GetDownSpdStr(this.mTotalSize), this.GetDownSpdStr((_cur_size - _size) * 3 / 2));
            }

            this.mSliderBar.value = this.mProgress;
            this.mLbBar.text = this.mLoadingInfo;
        }
        else if (mLoadType == LoadType.LoadConfig)//加载配置
        {
            this.mSliderBar.value = mLoadConfigProgress;
            this.mLbBar.text = "正在加载配置...";
        }
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_cancel":
                foreach (var item in this.mDownList) {
                    item.Dispose();
                }
                this.mDownList.Clear();
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "slider_bar":
                this.mSliderBar = tf.GetComponent<UISlider>();
                break;
            case "lb_bar":
                this.mLbBar = tf.GetComponent<UILabel>();
                break;
            case "btn_cancel":
                mBtnCancel = tf.gameObject;
                break;
            case "lb_cancel":
                mlbCancel = tf.GetComponent<UILabel>();
                break;
        }
    }
}
