using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 文件下载，特殊UI
/// </summary>
public class UI_download : UILayer {
    public UISlider mSliderBar;//下载进度显示
    public UILabel mLbBar;//下载进度文本

    public VoidCall mCall;
    public DownLoadInfo mCurDown;//当前下载任务
    public bool mIsError = false;//是否下载错误
    private float mProgress;
    private string mLoadingInfo;

    public void InitData(string local_path, string net_path, VoidCall call) {
        this.mCall = call;
        this.mCurDown = new DownLoadInfo(net_path, local_path, 0);
        this.mCurDown.StartDown();

        //this.StartCoroutine(this.DownLoad(queue));
        this.mSliderBar.value = 0;
        this.mLbBar.text = "准备下载中...";
    }

    private void DownLoadError(DownLoadInfo info) {//下载出错处理
        if (info.www == null) {
            LogMgr.LogError("下载失败:" + info.url);
        }else{
            LogMgr.LogError("下载失败:" + info.url + "  error_code:" + info.www.responseCode + "  error:" + info.www.error);
        }
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
    private void RefershDownLoadList() {//更新下载内容
        if (this.mCurDown.www.isDone) {
            if (this.mCurDown.IsError == false) {//下载完成
                LogMgr.Log("下载完成:" + this.mCurDown.url);
                this.mCurDown.DownLoadFinish();
                this.mProgress = 1;
                this.mLoadingInfo = "下载完成...";
                this.mCall();
            } else {//下载失败
                this.DownLoadError(this.mCurDown);
            }
        } else if (this.mCurDown.TimeOutCheck()) {//超时检测
            this.mCurDown.Dispose();//超时断开
            this.DownLoadError(this.mCurDown);
        }
    }
    private List<KeyValuePair<float, long>> size_list = new List<KeyValuePair<float, long>>();
    public void Update() {
        if (this.mCurDown.www != null) {
            this.RefershDownLoadList();

            long _cur_size = this.mCurDown.CurDownSize;
            float _time = Time.realtimeSinceStartup - 1;
            long _size;
            if (size_list.Count == 0) {
                _size = 0;
            } else {
                _size = long.MaxValue;
                for (int i = size_list.Count - 1; i >= 0; i--) {
                    if (size_list[i].Key >= _time) {
                        _size = System.Math.Min(_size, size_list[i].Value);
                    } else {
                        size_list.RemoveAt(i);
                    }
                }
            }
            size_list.Add(new KeyValuePair<float, long>(Time.realtimeSinceStartup, _cur_size));
            this.mProgress = this.mCurDown.www.downloadProgress;
            if (this.mCurDown.size == 0 && this.mProgress > 0) {
                this.mCurDown.size = (long)(_cur_size / this.mProgress);
            }
            if (this.mCurDown.size > 0) {
                this.mLoadingInfo = string.Format("下载文件{0}/{1} 下载速度：{2}/s", this.GetDownSpdStr(_cur_size), this.GetDownSpdStr(this.mCurDown.size), this.GetDownSpdStr((_cur_size - _size) * 3 / 2));
            } else {
                this.mLoadingInfo = string.Format("下载文件{0} 下载速度：{1}/s", this.GetDownSpdStr(_cur_size), this.GetDownSpdStr((_cur_size - _size) * 3 / 2));
            }
        } else {
            this.mCurDown.StartDown();
        }

        this.mSliderBar.value = this.mProgress;
        this.mLbBar.text = this.mLoadingInfo;
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_cancel":
                if (this.mCurDown.www != null) {
                    this.mCurDown.Dispose();
                }
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
        }
    }
}
