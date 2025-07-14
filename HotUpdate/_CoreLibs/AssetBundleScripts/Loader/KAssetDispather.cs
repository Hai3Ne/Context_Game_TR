//#define KDEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading;
using Object = UnityEngine.Object;

namespace Kubility
{

	public class KAssetDispather : SingleTon<KAssetDispather>
    {
        private int LockCount = 0;

        Queue<AssetRequestInterface> LoaderQueue;

        Queue<AssetRequestInterface> PreQueue;

        private AssetRequestInterface DownLoadLoader;

        //Dictionary<string, AssetRequestInterface> CacheloadDic;

        /// <summary>
        /// The default size.
        /// </summary>
        const int default_size = 100;

        public bool CanUnLoad
        {
            get
            {
                return LockCount == 0;
            }
        }


        public KAssetDispather()
        {
            this.LoaderQueue = new Queue<AssetRequestInterface>();
            this.PreQueue = new Queue<AssetRequestInterface>();
            //this.CacheloadDic = new Dictionary<string, AssetRequestInterface>();
            ServicePointManager.DefaultConnectionLimit = 500;
        }


        Dictionary<string,byte[]> mbundUniqueKeyCached = new Dictionary<string, byte[]>();
        Dictionary<byte[], System.Action<UnityEngine.AssetBundle>[]> loadBundleReqList = new Dictionary<byte[], Action<UnityEngine.AssetBundle>[]> ();
        Dictionary<byte[], UnityEngine.AssetBundle> loadBundleRespList = new Dictionary<byte[], UnityEngine.AssetBundle> ();

        public void LoadBundleFromMemeorySync(string keyID, byte[] bytes, System.Action<UnityEngine.AssetBundle> cb){
            if (mbundUniqueKeyCached.ContainsKey (keyID)) {
                byte[] bkey = mbundUniqueKeyCached [keyID];
                if (loadBundleRespList.ContainsKey (bkey)) {
                    cb.TryCall(loadBundleRespList[bkey]);
                    return;
                }
                var cblist = loadBundleReqList [bkey];
                Action<AssetBundle>[] newcbList = new Action<AssetBundle>[cblist.Length+1];
                Array.Copy (cblist, 0, newcbList, 0, cblist.Length);
                newcbList[newcbList.Length-1] = cb;
                loadBundleReqList [bkey] = newcbList;
                return;
            }
            mbundUniqueKeyCached.Add(keyID, bytes);
            loadBundleReqList.Add (bytes, new Action<AssetBundle>[]{cb});
        }

        Dictionary<AssetBundleRequest, System.Action<UnityEngine.Object>> loadReqList = new Dictionary<AssetBundleRequest, Action<UnityEngine.Object>> ();
        public void LoadAssetSync(AssetBundle bundle, string name, System.Action<UnityEngine.Object> cb) {
            AssetBundleRequest req = bundle.Contains(name) ? bundle.LoadAssetAsync (name) : bundle.LoadAllAssetsAsync();
            loadReqList.TryAdd (req, cb);
        }

        AssetBundleCreateRequest currLoadRequest = null;
        Action<AssetBundle>[] currLoadReponseCb = null;
        byte[] currentLoadBytes = null;
        public void Update(float delta){
            if (delta > 0) {
                AssetBundleRequest[] keys = new AssetBundleRequest[loadReqList.Keys.Count];
                loadReqList.Keys.CopyTo (keys, 0);
                for (int i = 0; i < keys.Length; i++) {
                    if (keys [i].isDone) {
                        if (keys [i].asset != null)
                            loadReqList [keys [i]].TryCall (keys [i].asset);
                        else if (keys [i].allAssets.Length > 0)
                            loadReqList [keys [i]].TryCall (keys [i].allAssets [0]);
                        loadReqList.Remove (keys [i]);
                    }
                }
            }

            if (currLoadRequest == null || currLoadRequest.isDone)
            {
                if (currLoadRequest != null && currLoadReponseCb != null) {
                    loadBundleRespList[currentLoadBytes] = currLoadRequest.assetBundle;
                    loadBundleReqList.Remove (currentLoadBytes);
                    AssetBundleCreateRequest tmp = currLoadRequest;
                    currLoadRequest = null;
                    Array.ForEach(currLoadReponseCb, x=>x.TryCall (tmp.assetBundle));
                }
                foreach (var pair in loadBundleReqList) {
                    currLoadRequest = AssetBundle.LoadFromMemoryAsync (pair.Key);
                    currLoadReponseCb = pair.Value;
                    currentLoadBytes = pair.Key;
                    break;
                }
            }
        }
        

		LoaderHandler.ProgressHandler mDefaultProgress = null;
		public void SetDefaultProcesshandle(LoaderHandler.ProgressHandler progrss){
			mDefaultProgress = progrss;
		}



        public void Close()
        {

            LoaderQueue.Clear();
            PreQueue.Clear();

            if(DownLoadLoader != null)
                DownLoadLoader.Stop();

			GameUtils.Close();
        }

        public RefDataLoader<T> YieldDispatchLoader<T>(LoaderType LoadType, ABData info, LoaderHandler.ProgressHandler OnProcessHandler = null) where T : Object
        {
            if (info == null)
                throw new ArgumentException(" ABData is Null");
			if (OnProcessHandler == null)
				OnProcessHandler = mDefaultProgress;

            AssetInterface loader = null;

            ClsTuple<bool, T, KAssetBundleRef> tuple = new ClsTuple<bool, T, KAssetBundleRef>();

            LoaderHandler.FinishHandler<T> mhandler = (Sender) =>
            {

                tuple.field0 = true;
                if (LoadType == LoaderType.SceneLoad)
                    Interlocked.Decrement(ref LockCount);

                AssetLoaderDataInterface datainter = loader as AssetLoaderDataInterface;
                if (datainter != null)
                {
                    tuple.field2 = datainter.DataRef;
                    if (datainter.DataRef != null && datainter.DataRef.FileType != ABFileTag.Scene)
                        tuple.field1 = Sender;
                    else
                        tuple.field1 = null;
                    
                }

               // CacheloadDic.Remove(info.Abname);
                if (OnProcessHandler != null)
                {
                    AssetRequestInterface ReqLodaer = loader as AssetRequestInterface;
                    if (ReqLodaer != null)
                        ReqLodaer.Progresss -= OnProcessHandler;
                }

            };

            if (LoadType == LoaderType.SceneLoad)
                Interlocked.Increment(ref LockCount);

            loader = FetchLoaderType(loader, LoadType, info, mhandler);
            if (OnProcessHandler != null)
            {
                AssetRequestInterface ReqLodaer = loader as AssetRequestInterface;
                if (ReqLodaer != null)
                    ReqLodaer.Progresss += OnProcessHandler;
            }

            Start();

            return new RefDataLoader<T>(tuple);
        }

		public int AllLoadedNum = 0;
        public AssetInterface DispatchLoader<T>(LoaderType LoadType, ABData info, LoaderHandler.FinishHandler<T> callback, LoaderHandler.ProgressHandler OnProcessHandler = null) where T : Object
        {
            return _DispatchLoader (LoadType, info, callback, OnProcessHandler);
        }

        public void DispatchLoaderAll<T>(LoaderType LoadType, ABData[] infos, LoaderHandler.FinishHandler<T> callback, LoaderHandler.ProgressHandler OnProcessHandler = null) where T : Object
        {
            if (infos.Length <= 0) {
                callback.TryCall (default(T));
                return;
            }
            foreach (var info in infos) {
                _DispatchLoader<T> (LoadType, info, callback, OnProcessHandler, false);
            }
            Start ();
        }

        AssetInterface _DispatchLoader<T>(LoaderType LoadType, ABData info, LoaderHandler.FinishHandler<T> callback, LoaderHandler.ProgressHandler OnProcessHandler = null, bool start = true) where T : Object
        {
            if (info == null)
                throw new ArgumentException(" ABData is Null");
			if (OnProcessHandler == null)
				OnProcessHandler = mDefaultProgress;
            AssetInterface loader = null;
            LoaderHandler.FinishHandler<T> mhandler = (Sender) =>
            {
                AllLoadedNum ++;
                if (Sender == null && typeof(T) != typeof(BinaryAsset))
                {
                    callback.TryCall(null);
                }
                else
                {
                    AssetLoaderDataInterface datainter = loader as AssetLoaderDataInterface;
                    if (datainter != null)
                    {
                        if (datainter.DataRef != null && datainter.DataRef.FileType == ABFileTag.Scene)
                        {
                            if (LoadType == LoaderType.SceneLoad)
                                Interlocked.Decrement(ref LockCount);

                            callback.TryCall(null);
                        }
                        else
                        {
                            callback.TryCall(Sender);
                        }
                    }
                }

                // CacheloadDic.Remove(info.Abname);

                if (OnProcessHandler != null)
                {
                    AssetRequestInterface ReqLodaer = loader as AssetRequestInterface;
                    if (ReqLodaer != null)
                        ReqLodaer.Progresss -= OnProcessHandler;
                }
            };

            if (LoadType == LoaderType.SceneLoad)
                Interlocked.Increment(ref LockCount);


            loader = FetchLoaderType(loader, LoadType, info, mhandler);

            if (OnProcessHandler != null)
            {
                AssetRequestInterface ReqLodaer = loader as AssetRequestInterface;
                if (ReqLodaer != null)
                    ReqLodaer.Progresss += OnProcessHandler;
            }
            if (start)
                Start();

            return loader;
        }

        public void DownLoadFiles(Queue<string> FileList, LoaderHandler.ProgressHandler handler, LoaderType type = LoaderType.SyncLoad)
        {

            if (FileList.Count == 0)
                return;

            if (type == LoaderType.YieldLoad)
            {
                DownLoadLoader = new YieldAssetLoader();

            }
            else if (type == LoaderType.SceneLoad)
            {
                DownLoadLoader = new SceneAssetLoader();

            }
            else
            {
                DownLoadLoader = new SyncAssetLoader();

            }

            DownLoadLoader.Progresss += (string fullInfo, long HasReceived, long Total, string Filename, Exception ex) =>
            {
				if (KApplication.isPlaying)
				{
	                if (ex == null)
	                {
	                    handler(fullInfo, HasReceived, Total, Filename, null);

	                    if (HasReceived == Total && FileList.Count > 0)
	                    {
	                        DownLoadLoader.DownLoad(FileList.Dequeue());
	                    }
	                }
	                else
	                {
	                    handler(fullInfo, HasReceived, Total, Filename, ex);
	                }
				}
            };

            //first
            if (FileList.Count > 0)
                DownLoadLoader.DownLoad(FileList.Dequeue());
        }

        private AssetInterface FetchLoaderType<T>(AssetInterface loader, LoaderType LoadType, ABData info, LoaderHandler.FinishHandler<T> callback) where T : Object
        {

            if (ABLoadConfig.Editor_MODE)
            {
                loader = new EditorAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);

                TryPushToQueue<EditorAssetLoader>(loader as EditorAssetLoader);
            }
            else if (LoadType == LoaderType.Default)
            {

                loader = new SyncAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);

                TryPushToQueue<SyncAssetLoader>(loader as SyncAssetLoader);

            }
            else if (LoadType == LoaderType.SyncLoad)
            {

                loader = new SyncAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);

                TryPushToQueue<SyncAssetLoader>(loader as SyncAssetLoader);
            }
            else if (LoadType == LoaderType.YieldLoad)
            {

                loader = new YieldAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);

                TryPushToQueue<YieldAssetLoader>(loader as YieldAssetLoader);
            }
            else if (LoadType == LoaderType.PreLoad)
            {
                //预加载的话没关系
                loader = new PreLoadLoader();
                loader.LoadAssetBundle(info.Abname, callback);
                TryPushToQueue<PreLoadLoader>(loader as PreLoadLoader);
            }
            else if (LoadType == LoaderType.SceneLoad)
            {
                loader = new SceneAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);
                TryPushToQueue<SceneAssetLoader>(loader as SceneAssetLoader);
            }
            else
            { //if(LoadType == LoaderType.AsyncLoad)
              //now use this
                loader = new YieldAssetLoader();
                loader.LoadAssetBundle(info.Abname, callback);
                TryPushToQueue<YieldAssetLoader>(loader as YieldAssetLoader);
            }

            return loader;

        }

        private void TryPushToQueue<T>(T loader) where T : AssetInterface, AssetRequestInterface, AssetLoaderDataInterface
        {
            if (loader.LoaderData.LogNull("加载所需数据不存在"))
            {

                if (LoaderQueue.Count > default_size)
                {
                    PreQueue.Enqueue(loader);
                }
                else
                {
                    //LogUtils.Log("加入加载队列", loader.LoaderData.Abname);
                    LoaderQueue.Enqueue(loader);
                }
            }


        }



        private void Start()
        {

            while (LoaderQueue.Count > 0)
            {
                AssetRequestInterface request = LoaderQueue.Dequeue();
                if (request != null)
                {
                    request.Start();
                }
                else
                {
					LogMgr.Log("未知错误 request 为Null");
                }

            }

			MonoDelegate.Instance.Coroutine_DelayNextFrame(delegate()
            {
                while (PreQueue.Count > 0)
                {
					AssetRequestInterface request = PreQueue.Dequeue();
                    if (request != null)
                    {
                        request.Start();
                    }
                    else
                    {
						LogMgr.Log("未知错误 request 为Null");
                    }

                }
            });



        }


    }
}
