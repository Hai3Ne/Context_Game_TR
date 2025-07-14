using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DownLoadInfo {
    public string url;//下载地址
    public string FilePath;//本地保存地址
    public UnityWebRequest www;
    public int ErrorCount = 0;//下载失败次数
    public long size;

    public bool IsFinish {
        get {
            return this.www != null && this.www.isDone && string.IsNullOrEmpty(this.www.error); 
        }
    }
    public bool IsError {
        get {
            return  this.www.responseCode != 200;
        }
    }
    public long CurDownSize {//已下载大小
        get {
            if (www == null) {
                return 0;
            } else {
                if (this.size > 0) {
                    return (long)(this.size * this.www.downloadProgress);
                } else {
                    return (long)this.www.downloadedBytes;
                }
            }
        }
    }

    private static long _ticks = 0;
    public DownLoadInfo(string url, string path,long size) {
        if (_ticks == 0) {
            _ticks = System.DateTime.Now.Ticks;
        }

        this.url = string.Format("{0}?{1}", url, _ticks);
        this.FilePath = path;
        this.size = size;
    }

    public void StartDown() {
        this.Dispose();
        this.www = UnityWebRequest.Get(url);
        this.www.Send();
    }
    public void Dispose() {
        if (this.www != null) {
            this.www.Dispose();
            //this.www.Dispose();
            this.www = null;
        }
        __pre_pro = -1;
    }

    public void DownLoadFinish() {
        System.IO.File.WriteAllBytes(this.FilePath, this.www.downloadHandler.data);
        this.Dispose();
    }

    private float __pre_pro = -1;//上次检测进度
    private float __next_time;//下次检测时间
    public bool TimeOutCheck() {//超时检测  3秒检测一次
        if (__next_time > Time.realtimeSinceStartup) {
            return false;
        }
        __next_time = Time.realtimeSinceStartup + 5;

        if (this.www != null) {
            if (this.__pre_pro == this.www.downloadProgress) {
                return true;//进度相同  认为超时
            }
            this.__pre_pro = this.www.downloadProgress;
        }
        return false;
    }
}
