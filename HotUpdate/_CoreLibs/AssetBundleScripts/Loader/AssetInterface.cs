//#define KDEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Kubility
{
    ///<summary> 
    ///AB加载的行为接口
    ///</summary>
    public interface AssetInterface
    {
        void LoadAssetBundle<T>(string Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object;
        void LoadAssetBundle<T>(ABData Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object;
        void UnLoadAssetBundle(string Key);
        void UnLoadAssetBundle(ABData Key);

    }

    ///<summary> 
    ///ab加载的接口
    ///</summary>
    public interface AssetLoaderDataInterface
    {

        bool isDone { get; set; }

        ABData LoaderData { get; set; }

        KAssetBundleRef DataRef { get; }
    }

    ///<summary> 
    ///ab加载的 请求接口
    ///</summary>
    public interface AssetRequestInterface
    {
        LoaderHandler.ProgressHandler Progresss { get; set; }

        LoaderHandler.FinishHandler<UnityEngine.Object> OnFinishHander { get; }

        void DownLoad(string name);

        void ReTry();

        void Start();

        void Stop();

        void Finish();

    }


    public interface AssetObjectInterface
    {

        RefState State { get; }
        ABFileTag FileType { get; }
        ABNodeTag NodeTag { get; }
        int refCount { get; }

        string AssetName { get; }

        bool AutoManaged { get; }

        UnityEngine.Object MainObject { get; }

        long TimeStamp { get; }

        long Size { get; }

        void Release(UnityEngine.Object gameuser);

        void Retain(UnityEngine.Object gameuser);

        void SetPreRes(bool state = true);

        void Lock();

        void UnLock();

        void AutoRelease();

        void Update(AssetObjectInterface newObject);

        void Reset();

        void UnLoad(bool strong);


    }
}



