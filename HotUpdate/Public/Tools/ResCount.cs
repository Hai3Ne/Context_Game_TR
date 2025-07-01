using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源引用计数器
/// </summary>
public class ResCount : MonoBehaviour
{
    //public ABData ab_data;//旧版
    public AssetBundleInfo ab_info;//新版

    public void OnDestroy()
    {
        //if (this.ab_data != null) {
        //    ResManager.UnloadAB(this.ab_data, false);
        //}
        if (this.ab_info != null) {
            ResManager.UnloadAB(this.ab_info, false);
        }
    }
}
