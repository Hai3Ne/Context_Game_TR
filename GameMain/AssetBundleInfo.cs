using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AssetBundleInfo {
    public string ABname;//资源名称
    public string MD5;//资源MD5码
    public long Size;//文件大小
    public string root;
    public string saveroot;
    public List<string> DependList = new List<string>();//依赖包名字
    [NonSerializedAttribute]
    public List<AssetBundleInfo> DependABList = new List<AssetBundleInfo>();
    [NonSerializedAttribute]
    public string FullPath;//
    [NonSerializedAttribute]
    public AssetBundleCreateRequest ab_request = null;//是否正在加载中
}
