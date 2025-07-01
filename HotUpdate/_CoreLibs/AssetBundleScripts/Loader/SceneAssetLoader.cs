//#define FULLLOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Object = UnityEngine.Object;

namespace Kubility
{

    public class SceneAssetLoader : AssetInterface, AssetRequestInterface, AssetLoaderDataInterface
    {

        Action DependCall;

        private int trytimes = 0;
        private KAssetBundleRef _DataRef;

        public KAssetBundleRef DataRef
        {
            get
            {
                return _DataRef;
            }
            protected set
            {
                _DataRef = value;
            }
        }

        private LoaderHandler.ProgressHandler _Progresss;

        public LoaderHandler.ProgressHandler Progresss
        {
            get
            {
                return _Progresss;
            }
            set
            {
                _Progresss = value;
            }
        }

        private bool _isDone;

        public bool isDone
        {
            get
            {
                return _isDone;
            }
            set
            {
                _isDone = value;
            }
        }

        private ABData _LoaderData;

        public ABData LoaderData
        {
            get
            {
                return _LoaderData;
            }
            set
            {
                _LoaderData = value;
            }
        }

        private LoaderHandler.FinishHandler<Object> _OnFinishHander;

        public LoaderHandler.FinishHandler<Object> OnFinishHander
        {
            get
            {
                return _OnFinishHander;
            }
        }

       

        public void LoadAssetBundle<T>(string Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object
        {

            //配置回调
            LoaderHandler.FinishHandler<UnityEngine.Object> handler = null;
            handler = (Object obj) =>
            {
                callback.TryCall((T)obj);

                _OnFinishHander -= handler;
            };

            _OnFinishHander += handler;

            //查询数据
			ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<T>(Key);
            if (data != null)
            {
                LoaderData = data;
#if FULLLOG
				LogMgr.Log("准备非阻塞场景加载。。。。数据配置完毕");
#endif

            }
            else
            {
                LogMgr.Log("未从配置文件中发现该信息 , LoadabName " + Key+ "原始路径>>" + Key);
            }
        }

        public void LoadAssetBundle<T>(ABData Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object
        {
            //配置回调
            LoaderHandler.FinishHandler<UnityEngine.Object> handler = null;
            handler = (Object obj) =>
            {
                callback.TryCall((T)obj);

                _OnFinishHander -= handler;
            };

            _OnFinishHander += handler;


            if (Key != null)
            {
                LoaderData = Key;
#if FULLLOG
				LogMgr.Log("准备非阻塞场景加载。。。。数据配置完毕");
#endif
            }
            else
            {
                LogMgr.Log("传入的数据为空");
            }
        }


        public void UnLoadAssetBundle(string Key)
        {
            KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(Key);
            if (node != null)
            {
                node.UnLoad(false);
            }
        }

        public void UnLoadAssetBundle(ABData Key)
        {
            AssetObjectInterface node = KAssetRefPool.Instance.TryGet(Key);
            if (node != null)
            {
                node.UnLoad(false);
            }
        }


        public void Start()
        {

            if (LoaderData == null)
            {
                LogMgr.Log("loaderdata 为Null");

                Error();
            }
            else
            {
                //检查新资源
                if (LoaderData.VersionCode > Kubility.KAssetBundleManger.Instance.VersionCode)
                {

                    DownLoad(LoaderData.Hash + ABLoadConfig.FileExtensions);
                }
                else
                {
#if FULLLOG
										LogMgr.Log("开始非阻塞场景加载。。。。启动非阻塞场景");
#endif

                    //KCoroutine.StartNewCoroutine(LoadDepends());
                    //协程加载时间过长，还是优化为同步，依赖采用defualt
                    //查询数据
					MonoDelegate.Instance.StartCoroutine(  LoadDepends());



                }

            }


        }

        public void DownLoad(string name)
        {
			MonoDelegate.Instance.StartCoroutine(DownLoadFile(name));
        }

        private IEnumerator LoadDepends()
        {
            /////depends
            //检查依赖
            //var time = KTool.StartTiming();
            int count = LoaderData.MyNeedDepends.Count;
#if FULLLOG
						LogMgr.Log("开始非阻塞场景加载。。。。检查依赖");
#endif
            if (count > 0)
            {
                List<ABData> LoadList = new List<ABData>();
                for (int i = 0; i < count; ++i)
                {
                    DependData Key = LoaderData.MyNeedDepends[i];
                    if (Kubility.KAssetBundleManger.Instance.CacheContains(Key))
                    {
                        //考虑到异步的情况，使用delegate

                        ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<Object>(Key);

                        if (data != null)
                        {

                            LoadList.Add(data);
                        }
                        else
                        {
                            this.Error();
                        }

                    }
                    else
                    {
                        LogMgr.Log("未从配置信息中发现该内容"+ Key);
                    }

                }

                yield return LoadListLoad(LoadList);

            }
            else
            {
#if FULLLOG
								LogMgr.Log("开始非阻塞场景加载。。。。无依赖直接加载主资源"+LoaderData.Abname);
#endif
                LoadMain();
            }

        }

        IEnumerator LoadListLoad(List<ABData> LoadList)
        {
            int LoadCount = LoadList.Count;
            for (int i = LoadList.Count-1; i >=0; --i)
            {
                ABData data = LoadList[i];
#if FULLLOG
                    LogMgr.Log("开始非阻塞场景加载。。。。依赖加载"+ data.Abname, i.ToString(), LoadCount.ToString());
#endif
                //LoaderHandler.FinishHandler<Object> DepandLoadCallBack = (sender) =>
                //{
                //    //LoadList.Remove(data);
                //    //System.Threading.Interlocked.Increment(ref j);

                //    KAssetBundleRef DpendNode = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(data.Abname);

                //    if (DpendNode != null)
                //        DpendNode.Lock();


                //    //当依赖全部加载完成之后加载主资源,忽略成功失败
                //    if (LoadList.Count == 0)
                //    {



                //        //LogMgr.Log("依赖加载耗时 " + time.Elapsed.TotalSeconds.ToString("F5") + "  Load : " + LoaderData.Abname);

                //    }
                //};

                KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(data.Abname);
                if (node != null && node.Bundle != null)
                {
                    node.Retain(node.MainObject);
                    if (i == 0)
                    {
                        node.Lock();
#if FULLLOG
                            LogMgr.Log("开始场景加载。。。。依赖加载结束"+ LoaderData.Abname);
#endif
                        Progresss.TryCall("", LoadCount - i, LoadCount, "", null);
                        LoadMain();
                    }
                }
                else
                {
                    var loader = KAssetDispather.Instance.YieldDispatchLoader<Object>(LoaderType.YieldLoad, data);
                    yield return loader;
                    node = loader.Data;

                    if (node != null)
                        node.Lock();

                    Progresss.TryCall("", LoadCount - i, LoadCount, "", null);

#if FULLLOG
                        LogMgr.Log("场景加载进度完成 " + data.LoadName, "总数为:" + LoadCount , "当前已完成 :" + (LoadCount - LoadList.Count));
#endif
                    if (i == 0)
                    {
#if FULLLOG
                            LogMgr.Log("开始场景加载。。。。依赖加载结束"+ LoaderData.Abname);
#endif
                        LoadMain();
                    }
                }
            }
        }

        public void ReTry()
        {
            trytimes++;
            if (trytimes >= 3)
            {
                this.OnFinishHander.TryCall(null);
            }
            else
                Start();
        }

        IEnumerator DownLoadFile(string name)
        {
			string DownloadPath = KAssetBundleManger.GetPlatformAssetBundleDownLoadPath(name);
            string outputPath = KAssetBundleManger.GetPlatformPersistentDataPath(Path.GetFileName(name));
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }


            WWW www = new WWW(DownloadPath);

            while (!www.isDone && string.IsNullOrEmpty(www.error))
            {
                if (Progresss != null)
                {
                    long total = (long)www.bytesDownloaded;
                    long curent = (long)www.bytes.Length;

                    string full = string.Format(MLocalization.Instance.Get("ui_tips_10008"),
                                     curent / 1024 / 1024,
                                     total / 1024 / 1024,
                                     www.progress.ToString("N2"));
                    Progresss.TryCall(full, curent, total, DownloadPath, null);
                }

                yield return new WaitForSeconds(0.2f);
            }


            if (string.IsNullOrEmpty(www.error))
            {
                File.WriteAllBytes(outputPath, www.bytes);
                LoadMain();

            }
            else
            {
                Progresss.TryCall(null, 0, 0, DownloadPath, new Exception(www.error));
                LogMgr.Log("文件下载错误>>, 下载路径:" + Path.GetFileName(DownloadPath)+"输出路径:" + outputPath);
                this.Error();
            }


            www.Dispose();

        }

        public void Stop ()
        {

        }

        void LoadMain()
        {
            KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
            if (ABLoadConfig.AbLoadFile)
            {
                LoadABFile(node);
            }
            else
            {
                if (node != null && node.Bundle != null)
                {
#if FULLLOG
                    LogMgr.Log("开始非阻塞场景加载。。。。在缓存中发现资源直接加载"+LoaderData.Abname);
#endif
                    LoadAssetBundleAsset(node.Bundle, node, node.Size);

                }
                else
                {
					KAssetBundleManger.LoadFromMemeroy(LoaderData, (long bysLength, AssetBundle assetBundle) => {
						if (assetBundle != null)
                        {
							node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
                            if (node == null)
                            {
								LoadAssetBundleAsset(assetBundle, node, bysLength);
                            }
                            else
                                LoadAssetBundleAsset(node.Bundle, node, node.Size);
                        }
                        else
                        {
                            LogMgr.Log("文件流中读取的字节数据错误 "+ LoaderData.Abname);
                            Error();
                        }
                    });
             	}
            }
        }

        private void LoadABFile(KAssetBundleRef node)
        {
            string path = KAssetBundleManger.GetIOSTargetPath(LoaderData);
            if (!string.IsNullOrEmpty(path))
            {
#if FULLLOG
                LogMgr.Log ("开始场景步加载。。。。LoadFile开始加载对象"+ LoaderData.Abname);
#endif
                if (node == null || node.Bundle == null)
                {
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
                    LoadAssetBundleAsset(assetBundle, node, LoaderData.Size);
                }
                else
                {
                    LoadAssetBundleAsset(node.Bundle, node, LoaderData.Size);
                }

            }
            else
            {
                Error();
            }

        }


        void LoadAssetBundleAsset(AssetBundle assetBundle, KAssetBundleRef abRef, long size)
        {
            if (assetBundle != null)
            {
                //设置对象引用
                if (abRef == null)
                {
                    DataRef = KAssetBundleRef.Create(LoaderData, assetBundle, null, size);

                }
                else
                {
                    DataRef = abRef;
                }
#if FULLLOG
				LogMgr.Log("开始非阻塞场景加载。。。。开始加载对象assetbundle"+LoaderData.Abname);
#endif

                DataRef.Lock();

                Finish();

            }
            else
            {
                LogMgr.Log("AssetBundle.LoadFromMemory 加载对象为空 "+ LoaderData.Abname);
                Error();

            }
        }

        private void Error()
        {
            LogMgr.Log("开始非阻塞场景加载。。。。加载错误"+ LoaderData.Abname);
            this.OnFinishHander.TryCall(null);
            this._isDone = true;

            DependCall.TryCall();
            DependCall = null;
        }

        public void Finish()
        {

            LogMgr.Log("结束非阻塞场景加载。。。。全部加载完毕"+ LoaderData.Abname);
            //更新引用池

            KAssetRefPool.Instance.PushToPool<KAssetBundleRef>(DataRef);
            this.OnFinishHander.TryCall(DataRef.Bundle);

            this._isDone = true;

            DependCall.TryCall();
            DependCall = null;

        }

    }


}
