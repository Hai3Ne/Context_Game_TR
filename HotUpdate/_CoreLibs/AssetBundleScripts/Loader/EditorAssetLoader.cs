//#define FULLLOG
using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kubility
{
    public class EditorAssetLoader : AssetLoaderDataInterface, AssetInterface, AssetRequestInterface
    {

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
#if UNITY_EDITOR
        private static ABCacheDataMgr Cache;

        static EditorAssetLoader()
        {
            try
            {
                string text = "";
                using (var fs = new FileStream(Application.dataPath + "/Settings/Cache.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    var bys = new byte[fs.Length];
                    fs.Read(bys, 0, bys.Length);
                    text = System.Text.Encoding.UTF8.GetString(bys);
                    fs.Close();
                }

                Cache = ParseUtils.Json_Deserialize<ABCacheDataMgr>(text);



            }
            catch (Exception ex)
            {
                LogMgr.LogError(ex);
            }
        }
#endif


        public void LoadAssetBundle<T>(string Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object
        {
            //配置回调
            LoaderHandler.FinishHandler<UnityEngine.Object> handler = null;
            handler = (Object obj) =>
            {
                callback((T)obj);

                _OnFinishHander -= handler;
            };

            _OnFinishHander += handler;

            //查询数据
			ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<T>(Key);
            if (data != null)
            {
                LoaderData = data;
#if FULLLOG
				LogMgr.Log("准备编辑器加载。。。。数据配置完毕");
#endif
            }
            else
            {
				LogMgr.Log("未从配置文件中发现该信息 , LoadabName " + Key+"原始路径>>" + Key);
            }
        }

        public void LoadAssetBundle<T>(ABData Key, LoaderHandler.FinishHandler<T> callback) where T : UnityEngine.Object
        {
            //配置回调
            LoaderHandler.FinishHandler<UnityEngine.Object> handler = null;
            handler = (Object obj) =>
            {
                callback((T)obj);

                _OnFinishHander -= handler;
            };

            _OnFinishHander += handler;


            if (Key != null)
            {
                LoaderData = Key;
#if FULLLOG
				LogUtils.Log("准备编辑器加载。。。。数据配置完毕");
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


        public void Start()
        {
            //检查新资源
            if (LoaderData.VersionCode > Kubility.KAssetBundleManger.Instance.VersionCode)
            {
                DownLoad(LoaderData.Hash + ABLoadConfig.FileExtensions);
                //webclient end
            }
            else
            {
                LoadAsset();
            }

        }

        public void DownLoad(string name)
        {
            string outputPath = KAssetBundleManger.GetPlatformPersistentDataPath(Path.GetFileName(name));
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            try
            {

                //MLocalization.Instance.Get ("ui_tips_10008");
                WebClient client =  new WebClient();
				string DownloadPath = KAssetBundleManger.GetPlatformAssetBundleDownLoadPath(name);
                client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                {

                    string full = string.Format("{0}%  ", e.ProgressPercentage.ToString("N2"));

                    if (Progresss != null)
                        Progresss(full, e.BytesReceived, e.TotalBytesToReceive, DownloadPath, null);


                };

                client.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                {
                    if (e.Cancelled)
                    {
						LogMgr.Log("下载文件取消 >>"+ Path.GetFileName(DownloadPath));
                        if (Progresss != null)
                            Progresss("Cancel", 0, 0, DownloadPath, new Exception("Net error"));
                    }
                    else
                    {
                        if (e.Error != null)
                        {
							LogMgr.Log("下载文件错误>>"+Path.GetFileName(DownloadPath)+ e.Error.ToString());
                            if (Progresss != null)
								Progresss("Error", 0, 0, DownloadPath, new Exception("Net error"));
                        }
                        else
                        {
							LogMgr.Log("下载文件完成>>"+Path.GetFileName(DownloadPath));
                        }
                    }
                };

                client.DownloadFileAsync(new Uri(DownloadPath), outputPath);


            }
            catch (Exception ex)
            {
                if (Progresss != null)
                {
                    Progresss(null, 0, 0, null, ex);
                }
            }

        }

        public void Stop ()
        {

        }

        public void ReTry()
        {
            trytimes++;
            if (trytimes >= 3)
            {
                this.OnFinishHander(null);
            }
            else
                Start();
        }

        private void LoadAsset()
        {

            if (LoaderData == null)
            {
				LogMgr.LogError("loaderdata 为Null");

                Error();

            }
            else
            {

                LoadMain();

            }


        }


        private void LoadMain()
        {
#if UNITY_EDITOR			
            //查询数据
            KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(LoaderData.Abname);
            if (node != null && node.MainObject != null)
            {
#if FULLLOG
				LogUtils.Log("开始编辑器加载。。。。在缓存中发现资源直接加载",LoaderData.Abname);
#endif
                DataRef = node;
                Finish();
            }
            else
            {

			#if UNUSE_ASSETBOUNDLE_INEDITOR
				HandleResObject(LoaderData.FileType, LoaderData.LoadName);
            #else
				ABCacheDataInfo cData = Cache.GetCacheInfo(LoaderData.Abname);
                if (cData.LogNull())
                {
					HandleResObject(cData.Data.FileType, cData.Filepath);
                }
        	#endif

            }
#endif
        }


#if UNITY_EDITOR
		void HandleResObject(short FileType, string pAssetFilepath)
		{
			if ((ABFileTag)FileType == ABFileTag.Scene)
			{
				DataRef = KAssetBundleRef.Create(LoaderData, null, null, 0);
				DataRef.Lock();
				Finish();
			}
			else if ((ABFileTag)FileType == ABFileTag.Bytes)
			{
				pAssetFilepath = pAssetFilepath.Substring (6);
				string fileFullPath = Application.dataPath + pAssetFilepath;
				using (var fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
				{
					byte[] bys = new byte[fs.Length];
					fs.Read(bys, 0, bys.Length);
					BinaryAsset o = new BinaryAsset();
					o.bytes = bys;
					DataRef = KAssetBundleRef.Create(LoaderData, null, o, 0);
					DataRef.Lock();
					Finish();
				}
			}
			else
			{
				Object o = AssetDatabase.LoadAssetAtPath<Object>(pAssetFilepath);
				DataRef = KAssetBundleRef.Create(LoaderData, null, o, 0);
				DataRef.Lock();
				Finish();
			}
		}
#endif
        private void Error()
        {
			LogMgr.LogError("开始编辑器加载。。。。加载错误"+LoaderData.Abname);
			Progresss.TryCall("", 1, 1, "", null);
            _OnFinishHander(null);
            this._isDone = true;
        }

        public void Finish()
        {
#if FULLLOG
			LogUtils.Log("结束编辑器加载。。。。全部加载完毕",LoaderData.Abname);
#endif
			Progresss.TryCall("", 1, 1, "", null);
            //更新引用池
			KAssetRefPool.Instance.PushToPool<KAssetBundleRef>(DataRef);
            _OnFinishHander(DataRef.MainObject);
            this._isDone = true;
        }
    }
}