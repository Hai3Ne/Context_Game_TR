//#define FULLLOG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.IO;
using Object = UnityEngine.Object;

namespace Kubility
{
    /// <summary>
    /// 同步加载器
    /// </summary>
    public class SyncAssetLoader : AssetInterface, AssetRequestInterface, AssetLoaderDataInterface
    {

        Action DependCall;
        private int trytimes = 0;

        private KAssetBundleRef _DataRef;
        public KAssetBundleRef DataRef {
			get { return _DataRef;}
            protected set { _DataRef = value; }
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

        private WebClient client;

        private LoaderHandler.FinishHandler<UnityEngine.Object> _OnFinishHander;

        public LoaderHandler.FinishHandler<UnityEngine.Object> OnFinishHander
        {
            get
            {
                return _OnFinishHander;
            }
        }

        
        public void LoadAssetBundle<T>(string Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object
        {
			ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<T>(Key);
			LoadAssetBundle (data, callback);
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
                LogMgr.Log("准备同步加载。。。。数据配置完毕");
#endif
            }
            else
            {
				LogMgr.Log("传入的数据为空");
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

        public void UnLoadAssetBundle(string Key)
        {
            KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(Key);
            if (node != null)
            {
                node.UnLoad(false);
            }
        }

        public void DownLoad(string name)
        {
			_DownLoad (name, Progresss);
        }

		void _DownLoad(string name, LoaderHandler.ProgressHandler progress)
		{
			string outputPath = KAssetBundleManger.GetPlatformPersistentDataPath(Path.GetFileName(name));
			string DownloadPath = KAssetBundleManger.GetPlatformAssetBundleDownLoadPath(name);
			if (KApplication.isPlaying) {
				GameUtils.ResumeFromBreakPoint (DownloadPath, outputPath, progress);
			}
		}

        public void Stop ()
        {
            if(client != null)
            {
                client.CancelAsync();
                client.Dispose();
                client = null;
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

        public void Start()
        {
            //检查新资源
            if (LoaderData.VersionCode > Kubility.KAssetBundleManger.Instance.VersionCode)
            {
				_DownLoad(LoaderData.Hash + ABLoadConfig.ConvertFileExtension, delegate(string fullInfo, long HasReceived, long Total, string Filename, Exception ex) {
					Progresss.TryCall(fullInfo, HasReceived, Total, Filename, ex);
					if (ex == null && HasReceived >= Total){
						LoadAsset();
					}
				});
            }
            else
            {
                LoadAsset();
            }

        }

        private void LoadAsset()
        {

			if (LoaderData == null) 
			{
				LogMgr.Log ("loaderdata 为Null");
				Error ();
			}
			else if ((ABFileTag)LoaderData.FileType == ABFileTag.Bytes) 
			{
				KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
				if (node == null || node.MainObject == null) {
					KAssetBundleManger.TryGetBytes (LoaderData, (byte[] bys) => {
						BinaryAsset binObj = new BinaryAsset ();
						binObj.bytes = bys;
						DataRef = KAssetBundleRef.Create(LoaderData, null, binObj, 0);
//						DataRef.Retain(binObj);
						DataRef.Lock();
						Finish();
					});
				}
				else
				{
					DataRef = node;
					DataRef.Retain(DataRef.MainObject);
					Finish();
				}

			} else {
				LoadDepends ();
			}

        }

        private void LoadDepends()
        {
            /////depends
            //检查依赖
            int count = LoaderData.MyNeedDepends.Count;
#if FULLLOG
            LogMgr.Log("开始同步加载。。。。检查依赖");
#endif
            if (count > 0)
            {
                List<ABData> LoadList = new List<ABData>();
                for (int i = 0; i < count; ++i)
                {
                    DependData Key = LoaderData.MyNeedDepends[i];
                    if (Kubility.KAssetBundleManger.Instance.CacheContains(Key))
                    {
                        ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<UnityEngine.Object>(Key);

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
						LogMgr.Log("未从配置信息中发现该内容"+Key);

                }

                LoadListLoad(LoadList);
            }
            else
            {
                LoadMain();
            }

        }

        private void LoadListLoad(List<ABData> LoadList)
        {
            for (int i = LoadList.Count - 1; i >= 0; i--)
            {
                ABData data = LoadList[i];
                KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(data.Abname);
                if (node != null && node.Bundle != null)
                {
                    node.Retain(node.MainObject);
                    continue;
                }

				KAssetDispather.Instance.DispatchLoader<Object>(LoaderType.SyncLoad, data, null, Progresss);
            }


#if FULLLOG
            LogMgr.Log("开始同步加载。。。。依赖加载结束"+LoaderData.Abname);
#endif
            LoadMain();
        }

        private void LoadMain()
        {

            KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
            if (node != null && node.Bundle != null)
            {
#if FULLLOG
                LogMgr.Log("开始同步加载。。。。在缓存中发现资源直接加载 " + LoaderData.Abname);
#endif
                LoadAssetBundleAsset(node.Bundle, node, node.Size);
            }
            else
            {
                if (ABLoadConfig.AbLoadFile)
                {
                    LoadABFile(node);
                }
                else
                {
					string fileName = KAssetBundleManger.GetPlatformPersistentDataPath(LoaderData.Hash+ABLoadConfig.ConvertFileExtension);
					AssetBundle assetBundle = AssetBundle.LoadFromFile (fileName);
					long byteSize = 0;
					if (assetBundle != null)
                    {
						node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
						if (node == null || node.Bundle == null)//double check
                        {
							LoadAssetBundleAsset(assetBundle, node, byteSize);
                        }
                        else
                            LoadAssetBundleAsset(node.Bundle, node, node.Size);								
                    }
                    else
                    {
                        LogMgr.Log("文件流中读取的字节数据错误 "+LoaderData.Abname);
                        Error();
                    }
                }
            }
        }

        private void LoadABFile(KAssetBundleRef node)
        {
            string path = KAssetBundleManger.GetIOSTargetPath(node, LoaderData);
            if (!string.IsNullOrEmpty(path))
            {
#if FULLLOG
                LogMgr.Log("开始同步加载。。。。LoadFile开始加载对象"+LoaderData.Abname);
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

        private void LoadAssetBundleAsset(AssetBundle assetBundle, KAssetBundleRef abRef, long size)
        {
            if (assetBundle != null)
            {
#if FULLLOG
                LogMgr.Log("开始同步加载。。。。开始加载对象assetbundle"+LoaderData.Abname);
#endif
                if ((ABFileTag)LoaderData.FileType != ABFileTag.Scene)
                {
                    TryGetLoadObject(assetBundle, abRef, size);
                }
                else
                {
                    DataRef = KAssetBundleRef.Create(LoaderData, assetBundle, null, size);
                    DataRef.Lock();
                    Finish();
                }

            }
            else
            {
				LogMgr.Log("LoadAssetBundleAsset assetBundle 对象为空 "+LoaderData.Abname);
                Error();

            }
        }

        private void TryGetLoadObject(AssetBundle assetBundle, KAssetBundleRef abRef, long size)
        {
            if (abRef == null || abRef.MainObject == null || abRef.Bundle == null)
            {
				Object asset = null;
				Object[] assets = assetBundle.LoadAllAssets ();
				for (int i = 0; i < assets.Length; i++) {
					if (assets [i].name == LoaderData.LoadName) {
						asset = assets [i];
						break;
					}
				}
                if (asset.LogNull())
                {
                    //设置对象引用
                    DataRef = KAssetBundleRef.Create(LoaderData, assetBundle, asset, size);
                    DataRef.Retain(asset);
                    DataRef.Lock();
                    Finish();

                }
                else
                {
					
                    LogMgr.Log("LoadAsset加载对象为空 "+LoaderData.Abname);
                    Error();
                }


            }
            else
            {
                DataRef = abRef;

                DataRef.Retain(DataRef.MainObject);

                Finish();
                this.OnFinishHander.TryCall(DataRef.MainObject);

            }
        }

        private void Error()
        {

            LogMgr.Log("开始同步加载。。。。加载错误"+LoaderData.Abname);

            _OnFinishHander.TryCall(null);
            this._isDone = true;

            DependCall.TryCall();
            DependCall = null;
        }

        public void Finish()
        {
#if FULLLOG
            LogMgr.Log("结束同步加载。。。。全部加载完毕"+LoaderData.Abname);
#endif
            //更新引用池
            KAssetRefPool.Instance.PushToPool<KAssetBundleRef>(DataRef);
			Progresss.TryCall(null, DataRef.Size, DataRef.Size, null, null);
            this.OnFinishHander.TryCall(DataRef.MainObject);
            this._isDone = true;
            DependCall.TryCall();
            DependCall = null;
        }
    }
}
