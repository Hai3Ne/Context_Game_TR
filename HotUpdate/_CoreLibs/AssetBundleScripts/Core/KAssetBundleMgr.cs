
#define FULLLOG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Object = UnityEngine.Object;

namespace Kubility
{

    public enum ResType
    {
        Texture,
        Txt,
        Audioclip,
		Prefab,
		Binary
    }

	public partial class KAssetBundleManger : SingleTon<KAssetBundleManger>
    {

		public void Init(){}
        private Dictionary<int, Stack<string>> PreLoadDic = new Dictionary<int, Stack<string>>();
        public Dictionary<string, ABData> VersionDic = new Dictionary<string, ABData>();
        public string nextScene;
        private long _localVersion;
        public long VersionCode
        {
            get
            {
				return _localVersion;
            }
        }

        public KAssetBundleManger()
        {
            ReLoadVersionFile();
        }

		public void SetDefaultProgress(LoaderHandler.ProgressHandler progrss){
			KAssetDispather.Instance.SetDefaultProcesshandle (progrss);
		}

        /// <summary>
        /// Reads the verion file info.
        /// </summary>
        public void ReLoadVersionFile()
        {
	        TryGetBytes(ABLoadConfig.VersionPath, delegate (byte[] bys)
            {
                if (bys != null)
                {
                    ParseStream(new MemoryStream(bys));
                }
            });

        }

        void ParseStream(Stream sr)
        {
            VersionDic.Clear();
            try
            {
                BinaryReader buffer = new BinaryReader(sr);
                int fhead = buffer.ReadInt32();
				if (fhead != ConstValue.VersionHeadCRC)
                {
                    LogMgr.LogError("ERROR>> 文件校验不一致！");
                    sr.Close();
                    return;
                }

				VersionManager.localVersion = buffer.ReadString();
				LogMgr.Log("版本号 -> " + VersionManager.localVersion);

                int ListCount = buffer.ReadInt32();
                for (int i = 0; i < ListCount; ++i)
                {
                    ABData info = new ABData();
                    info.Abname = buffer.ReadString();
                    info.Size = buffer.ReadInt64();
                    info.FileType = buffer.ReadInt16();
                    info.LoadName = buffer.ReadString();
                    info.RootType = buffer.ReadInt32();
                    info.VersionCode = buffer.ReadInt64();
                    info.Hash = buffer.ReadString();
                    int DepdLen = buffer.ReadInt32();
                    for (int j = 0; j < DepdLen; ++j)
                    {
                        var data = new DependData();
                        data.Abname = buffer.ReadString();
                        data.FileType = buffer.ReadInt16();
                        info.MyNeedDepends.TryAdd(data);
                    }

                    if (!string.IsNullOrEmpty(info.Abname) && !VersionDic.ContainsKey(info.Abname))
                        VersionDic.Add(info.Abname, info);
                }

                buffer.Close();
            }
            catch (Exception ex)
            {
                LogMgr.LogError(ex);
                sr.Close();
				string versionBytesfile = GetPlatformPersistentDataPath (ABLoadConfig.VersionPath);
                //以防万一，做了回滚操作
				File.Delete(versionBytesfile);
            }
			_localVersion = GenerateVersionCode(VersionManager.localVersion);
            sr.Close();
        }



        public int GenerateVersionCode(string version)
        {
            string[] verarr =version.Split('.');
            int main = int.Parse(verarr[0]);
            int sub = int.Parse(verarr[1]);
            int third = int.Parse(verarr[2]);

            int versionValue = main * 1000000 + sub * 100 + third;

            return versionValue;
        }

        public int GenerateVersionCode(VersionJsonInfo version)
        {
      
            int main = version.MainVersionID;
            int sub = version.SubVersionID;
            int third = version.ThirdVersionID;

            int versionValue = main * 1000000 + sub * 100 + third;

            return versionValue;
        }

        /// <summary>
        /// 卸载全体ab
        /// </summary>
        public void UnLoadAll()
        {
			KAssetRefPool.Instance.UnLoadAll();
        }

        public void UnLoadUnUsed()
        {
			KAssetRefPool.Instance.UnLoadUnUsed();
        }

        public void Unload<T>(string Key, bool includeAll = false) where T : UnityEngine.Object
        {
            string abname = GetAbName<T>(Key);
            KAssetBundleRef.Destroy(abname, includeAll);

        }
		/*
        /// <summary>
        /// Pres the load defiend.
        /// </summary>
        /// <param name="LoadList">LoadAsset list.</param>
        /// <param name="scene">Scene.</param>
        public void PreLoadDefiendList(List<string> LoadList, GameScene scene)
        {
            int index = (int)scene;
            if (PreLoadDic.TryAdd(index, new Stack<string>()))
            {

                for (int i = 0; i < LoadList.Count; ++i)
                {
                    PreLoadDic[index].Push(LoadList[i]);
                }
            }
            else
            {
                for (int i = 0; i < LoadList.Count; ++i)
                {
                    PreLoadDic[index].Push(LoadList[i]);
                }
            }
        }

        private void CheckAndLoad(string scene)
        {
            int sceneIndex = 0;
            if (scene.Equals("LoginScene"))
            {
                sceneIndex = 4;
            }
            else if (scene.Equals("Loading"))
            {
                sceneIndex = 1;
            }
            else if (scene.Equals("LoadBattleMap"))
            {
                sceneIndex = 2;
            }
            else if (scene.Equals("MapScene"))
            {
                sceneIndex = 3;
            }
            else if (scene.Equals("BattleScene"))
            {
                sceneIndex = 5;
            }

            if (PreLoadDic.ContainsKey(sceneIndex))
            {
                while (PreLoadDic[sceneIndex].Count > 0)
                {
                    ResourceLoad<Object>(PreLoadDic[sceneIndex].Pop(), null, LoaderType.SyncLoad);
                }
            }
        }

        public void PreLoadDepends<T>(string Key, Action<bool> callback) where T : Object
        {
            ReadFromCache<T>(Key, (data) =>
            {
                LoaderHandler.FinishHandler<T> handler = (sender) =>
                {
                    callback.TryCall(sender != null);

                };

					KAssetDispather.Instance.DispatchLoader(LoaderType.PreLoad, data, handler);
            });
        }


        public void PreLoadScene(GameScene scene, Action<bool> callback)
        {
            ReadFromCache<KSceneManager>("Scenes/" + scene.ToString(), (data) =>
            {
                LoaderHandler.FinishHandler<Object> handler = (sender) =>
                {
                    callback.TryCall(sender != null);

                };

					KAssetDispather.Instance.DispatchLoader(LoaderType.PreLoad, data, handler);
            });
        }
        //*/


        public void DownLoadABFiles(Queue<string> FileList, LoaderHandler.ProgressHandler handler, LoaderType type = LoaderType.SyncLoad)
        {
			KAssetDispather.Instance.DownLoadFiles(FileList, handler, type);
        }


        //public void LoadGameObject(string path, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default)
        //{
        //    ReadFromCache<GameObject>(path, (data) => {
        //        ResManager.LoadAsset<GameObject>(data, (sender) => {
        //            callback.TryCall(new SmallAbStruct(data, sender));
        //            ResManager.UnloadAB(data);
        //        });
        //    });
        //}

        //public void LoadGameObject(string path, GameObject parent, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default)
        //{
        //    ReadFromCache<GameObject>(path, (data) => {
        //        ResManager.LoadAsset<GameObject>(data, (sender) => {
        //            SmallAbStruct abs = new SmallAbStruct(data, sender);
        //            GameObject go = abs.Instantiate();
        //            go.AddComponent<ResCount>().ab_data = data;
        //            AddChild(parent, go);
        //            callback.TryCall(abs);
        //        });
        //    });
        //}

        //public void LoadGameObject(string path, Component parent, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default) {
        //    ReadFromCache<GameObject>(path, (data) => {
        //        ResManager.LoadAsset<GameObject>(data, (sender) => {
        //            SmallAbStruct abs = new SmallAbStruct(data, sender);
        //            GameObject go = abs.Instantiate();
        //            go.AddComponent<ResCount>().ab_data = data;
        //            AddChild(parent.gameObject, go);
        //            callback.TryCall(abs);

        //        });
        //    });
        //}

		public static Type Type_BinaryAsset = typeof(BinaryAsset);
		public static Type Type_GameObject = typeof(GameObject);
		public static Type Type_AudioClip = typeof(AudioClip);

        /// <summary>
        /// Resources the load.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="Target">Target.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        //public void ResourceLoad<T>(string path, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default) where T : UnityEngine.Object {
        //    if (typeof(T) == Type_GameObject) {
        //        throw new ArgumentException("please Use LoadGameObject");
        //    }

        //    if (typeof(T) == Type_BinaryAsset) {
        //        ABData data = ReadFromCache<T>(path);
        //        ResManager.LoadBytes(data, (bytes) => {
        //            BinaryAsset bs = new BinaryAsset();
        //            bs.bytes = bytes;
        //            callback.TryCall(new SmallAbStruct(data, bs));
        //        });
        //        return;
        //    }
        //    ReadFromCache<T>(path, (data) => {
        //        ResManager.LoadAsset<T>(data, (sender) => {
        //            callback.TryCall(new SmallAbStruct(data, sender));
        //            ResManager.UnloadAB(data);
        //        });
        //    });
        //}

        //public void ResourceLoad(string path, ResType target, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default)
        //{

        //    if (target == ResType.Texture) {
        //        ReadFromCache<Texture>(path, (data) => {
        //            ResManager.LoadAsset<Texture>(data, (sender) => {
        //                callback.TryCall(new SmallAbStruct(data, sender));
        //                ResManager.UnloadAB(data);
        //            });
        //        });
        //    } else if (target == ResType.Txt) {
        //        ReadFromCache<TextAsset>(path, (data) => {
        //            ResManager.LoadAsset<TextAsset>(data, (sender) => {
        //                callback.TryCall(new SmallAbStruct(data, sender));
        //                ResManager.UnloadAB(data);
        //            });
        //        });
        //    } else if (target == ResType.Audioclip) {
        //        ReadFromCache<AudioClip>(path, (data) => {
        //            ResManager.LoadAsset<AudioClip>(data, (sender) => {
        //                callback.TryCall(new SmallAbStruct(data, sender));
        //                ResManager.UnloadAB(data);
        //            });
        //        });
        //    }
        //}

   //     public IEnumerator YieldResourceLoad<T>(string path, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default, LoaderHandler.ProgressHandler OnProcessHandler = null) where T : UnityEngine.Object
   //     {
   //         if (typeof(T) == typeof(GameObject))
   //         {
   //             throw new ArgumentException("please Use YieldLoadGameObject");
   //         }
   //         yield return null;
			//ABData data = ReadFromCache<T>(path);
   //         ResManager.LoadAsset<T>(data, (obj) => {
   //             callback.TryCall(new SmallAbStruct(data, obj));
   //             ResManager.UnloadAB(data);
   //         });
   //     }


        /// <summary>
        /// it will  load Assetbundle 
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        //public void ResourceLoad<T>(string path, GameObject parent, Action<SmallAbStruct> callback = null, LoaderType loadertype = LoaderType.Default) where T : UnityEngine.Object {
        //    ReadFromCache<T>(path, (data) => {
        //        ResManager.LoadAsset<T>(data, (sender) => {
        //            if (data.FileType == (short)ABFileTag.Prefab) {
        //                SmallAbStruct abs = new SmallAbStruct(data, sender);
        //                GameObject go = abs.Instantiate();
        //                if (go != null && parent != null) {
        //                    Transform t = go.transform;
        //                    t.parent = parent.transform;
        //                    t.localPosition = Vector3.zero;
        //                    t.localRotation = Quaternion.identity;
        //                    t.localScale = Vector3.one;
        //                    go.layer = parent.layer;
        //                }
        //                callback.TryCall(abs);
        //            } else {
        //                callback.TryCall(new SmallAbStruct(data, sender));
        //            }
        //            ResManager.UnloadAB(data);
        //        });
        //    });
        //}

        
/*
        public IEnumerator YieldLoadScene(string path, Action<GameScene, KAssetBundleRef> OnFinish = null, LoaderHandler.ProgressHandler OnProcessHandler = null)
        {

            GenericLoader<ABData> req = YieldReadFromCache<KSceneManager>(path);

            yield return req;

            ABData data = req.MainObject;

            if (data != null)
            {
                CheckAndLoad(data.LoadName);
            }


			RefDataLoader<Object> loaderhander = KAssetDispather.Instance.YieldDispatchLoader<Object>(LoaderType.SceneLoad, data, OnProcessHandler);

            yield return loaderhander;

            KSceneManager.LoadScene(loaderhander.Data.AssetLoadName);

            if (OnFinish == null)
            {
                yield return new WaitForSeconds(ABLoadConfig.LoadDefaultTime);
                //卸载上一个场景的引用
                //LogMgr.Log("场景弱卸载 " + GlobalHelper.Instance.lastGameScene);

                //GlobalUtils.UnLoadScene(GlobalHelper.Instance.lastGameScene);

            }
            else
            {
                //卸载上一个场景的引用
                ABData Lastdata = ReadFromCache<KSceneManager>("Scenes/" + GlobalHelper.Instance.lastGameScene.ToString());
                if (Lastdata != null)
                {
					KAssetBundleRef node = KAssetRefPool.Instance.TryGet<KAssetBundleRef>(Lastdata.Abname);

                    OnFinish(GlobalHelper.Instance.lastGameScene, node);
                }
                else
                    OnFinish(GlobalHelper.Instance.lastGameScene, null);

            }

        }
		

        public SceneAsynacRequest YieldLoadSceneAsync(string path, bool defaultshow = false, LoaderHandler.ProgressHandler OnProcessHandler = null)
        {
            ABData data = ReadFromCache<KSceneManager>(path);

            if (data != null)
            {
                CheckAndLoad(data.LoadName);
            }


			RefDataLoader<Object> loaderReq = KAssetDispather.Instance.YieldDispatchLoader<Object>(LoaderType.SceneLoad, data, OnProcessHandler);

            return new SceneAsynacRequest(loaderReq, defaultshow);

        }

        public SceneAsynacRequest YieldLoadSceneAsync(string path, bool defaultshow = false, Action<bool ,float> OnProcessHandler = null)
        {
            ABData data = ReadFromCache<KSceneManager>(path);

            if (data != null)
            {
                CheckAndLoad(data.LoadName);
            }


			RefDataLoader<Object> loaderReq = KAssetDispather.Instance.YieldDispatchLoader<Object>(LoaderType.SceneLoad, data, (fullInfo,r,t,f,ex) =>
            {
                OnProcessHandler.TryCall(r >= t, (float)r / t);
            });

            return new SceneAsynacRequest(loaderReq, defaultshow);

        }
//*/
        //public IEnumerator YieldLoadGameObject(string path, Action<SmallAbStruct> callback, LoaderType loadertype = LoaderType.Default)
        //{
        //    yield return null;
        //    ABData data = KAssetBundleManger.Instance.ReadFromCache<GameObject>(path);
        //    ResManager.LoadAsset<GameObject>(data, (obj) => {
        //        callback.TryCall(new SmallAbStruct(data, obj));
        //        ResManager.UnloadAB(data);
        //    });

        //}

//#if UNITY_EDITOR && UNUSE_ASSETBOUNDLE_INEDITOR
//		public IEnumerator PreLoadGameObject(string path, Action<GameObject> callback)
//        {
//			string assetpath = "Assets/"+path+".prefab";
//			Object o = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetpath);
//			yield return null;
//			callback.TryCall (o as GameObject);
//		}
//#else
//        public IEnumerator PreLoadGameObject(string path, Action<GameObject> callback) {
//            yield return null;

//            ABData abData = ReadFromCache<GameObject>(path);
//            ResManager.LoadAsset<GameObject>(abData, (obj) => {
//                callback.TryCall(obj);
//                ResManager.UnloadAB(abData);
//            });
//        }
//#endif

        void GetAllABNames(ABData abData, List<ABData> list) 
		{
			for(int i = 0; i < abData.MyNeedDepends.Count; i++){
				ABData sub = VersionDic[abData.MyNeedDepends [i].Abname];
				GetAllABNames (sub, list);
			}
			list.Add (abData);
		}

        /// <summary>
        /// Yields the resource load.
        /// </summary>
        /// <returns>The resource load.</returns>
        /// <param name="path">Path.</param>
        /// <param name="Parent">Parent.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="loadertype">Loadertype.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        //public IEnumerator YieldResourceLoad<T>(string path, GameObject parent, Action<SmallAbStruct> callback = null, LoaderType loadertype = LoaderType.Default) where T : UnityEngine.Object {

        //    yield return null;
        //    ABData data = KAssetBundleManger.Instance.ReadFromCache<GameObject>(path);
        //    ResManager.LoadAsset<T>(data, (obj) => {
        //        if (data.FileType == (short)ABFileTag.Prefab) {//
        //            SmallAbStruct RetData = new SmallAbStruct(data, obj);
        //            GameObject go = RetData.Instantiate();
        //            if (go != null && parent != null) {
        //                Transform t = go.transform;
        //                t.parent = parent.transform;
        //                t.localPosition = Vector3.zero;
        //                t.localRotation = Quaternion.identity;
        //                t.localScale = Vector3.one;
        //                go.layer = parent.layer;
        //            }
        //            callback.TryCall(RetData);
        //        } else {
        //            callback.TryCall(new SmallAbStruct(data, obj));
        //        }
        //        ResManager.UnloadAB(data);
        //    });
        //}

        private string GetAbName<T>(string Key)
        {
            if (Key.EndsWith(ABLoadConfig.FileExtensions))
            {
                return Key;
            }
            else
            {
                string CurentAbName = "";
                InversePathAndTag<T>(Key, out CurentAbName);
                CurentAbName = Path.GetFileName(CurentAbName);
                return CurentAbName;
            }

        }

		public bool ConatainRes<T>(string path)
		{
			string key = GetAbName<T> (path);
			return VersionDic.ContainsKey (key);
		}

#if UNITY_EDITOR && UNUSE_ASSETBOUNDLE_INEDITOR
		Dictionary<string,ABData> mAbdataCached = new Dictionary<string, ABData>(); 
		public ABData ReadFromCache<T>(string Key) { return CreateAbDataInEditMode<T>(Key);}
		public void ReadFromCache<T>(string Key, Action<ABData> callback){ callback.Invoke(CreateAbDataInEditMode<T>(Key));}
		public GenericLoader<ABData> YieldReadFromCache<T>(string Key)
		{
			ABData abdata = CreateAbDataInEditMode<T>(Key);
			ClsTuple<bool, ABData> Tuple = new ClsTuple<bool, ABData>(true, abdata);
			return new GenericLoader<ABData>(Tuple);
		}

		ABData CreateAbDataInEditMode<T>(string key)
		{
			string assetloadName = "";
			string keyStr = key;
			ABFileTag tag = ABFileTag.NONE;
			if (!key.EndsWith (ABLoadConfig.FileExtensions)) {
				tag = InversePathAndTag<T> (key, out keyStr);
				keyStr = Path.GetFileName (keyStr);
				assetloadName = keyStr;
				assetloadName = assetloadName.Replace(".ab","").Replace ("=", "/").Replace ("^", ".");
			} else {
				if (mAbdataCached.ContainsKey(key))
					return mAbdataCached [key];
				Debug.Log("keyStr :"+key);
			}

			ABData abdat = new ABData ();
			abdat.Abname = keyStr;
			abdat.Size = 0;
			abdat.LoadName = assetloadName;
			abdat.FileType = (short)tag;
			mAbdataCached [keyStr] = abdat;
			return abdat;
		}

#else
        public ABData ReadFromCache<T>(string Key)
        {
            return VersionDic.TryGet(GetAbName<T>(Key));
        }

        public void ReadFromCache<T>(string Key, Action<ABData> callback)
        {
            if (VersionDic.Count > 0)
            {
                callback.TryCall(VersionDic.TryGet(GetAbName<T>(Key)));
            }
            else
            {
				MonoDelegate.Instance.StartCoroutine(WaitingVersion<T>(Key, callback));
            }

        }

		public GenericLoader<ABData> YieldReadFromCache<T>(string Key)
		{
			ClsTuple<bool, ABData> Tuple = new ClsTuple<bool, ABData>();
			if (VersionDic.Count > 0)
			{
				Tuple.field0 = true;
				Tuple.field1 = VersionDic.TryGet(GetAbName<T>(Key));
				return new GenericLoader<ABData>(Tuple);
			}
			else
			{
				Tuple.field0 = false;
				MonoDelegate.Instance.StartCoroutine(WaitingVersion<T>(Key, Tuple));
				return new GenericLoader<ABData>(Tuple);
			}
		}
#endif





        private IEnumerator WaitingVersion<T>(string Key, ClsTuple<bool, ABData> callback)
        {
            while (VersionDic.Count == 0)
            {
                yield return null;
            }

            callback.field1 = VersionDic.TryGet(GetAbName<T>(Key));
            callback.field0 = true;
        }

        private IEnumerator WaitingVersion<T>(string Key, Action<ABData> callback)
        {
            while (VersionDic.Count == 0)
            {
                yield return null;
            }

            callback.TryCall(VersionDic.TryGet(GetAbName<T>(Key)));
        }

        public bool CacheContains(string Key)
        {
            return VersionDic.ContainsKey(Key);
        }

        public bool CachePathContains(string Key,string hash)
        {
            return VersionDic.ContainsKey(Key) && VersionDic[Key].Hash.Equals(hash);
        }
    }
}
