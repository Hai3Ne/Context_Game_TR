using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using System;
public class GlobalLoading:SingleTon<GlobalLoading>,IRunUpdate
{
	LoadingViewer mLoadingViewer = null;
	LoadingType curLoadingType;
	List<GameObjDictLoadItem> loadDictList = new List<GameObjDictLoadItem>();
	List<ResLoadItem> loadList = new List<ResLoadItem>();

	bool isInitedLoadlistCompleted = false;
	string[] loadingTipsList;
	public bool ShowProgressUI = true;
	public bool isFirstLaunching = false;

    public LoadingViewer GetLoadingView {
        get {
            return this.mLoadingViewer;
        }
    }

	public void GlobalInit()
	{
		isFirstLaunching = true;
		Kubility.KAssetBundleManger.Instance.SetDefaultProgress (DefLoadingProgressHandle);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishRoomReady, OnEnteredRoom);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishRoomFail, this.OnEnterRoomFail);
	}

	void DefLoadingProgressHandle(string fullInfo,long HasReceived,long Total,string Filename, System.Exception ex){
		if (ex != null) {
			LogMgr.LogError (ex.Message);
			return;
		}

	}

	System.Action mOnCompleteFn = null;
    public void StartPreLoadGameRes(LoadingType t, System.Action completeFn, bool isAutoClose = true, bool is_start_game = false) {
		autoClose = isAutoClose;
		curLoadingType = t;
		mOnCompleteFn = completeFn;
		if (isFirstLaunching) {
			isInitedLoadlistCompleted = false;
			if (Application.isPlaying && loadingTipsList == null) {
				string str = Resources.Load<TextAsset> ("LoadingString").text;
				loadingTipsList = str.Split ('\n');
			}

			loadList.Clear ();
			loadDictList.Clear ();

            if (curLoadingType == LoadingType.ALL)
            {
                ConfigTables.Instance.LunchAllConf(loadList);

                byte[] bytes = ResManager.LoadBytes(GameEnum.Fish_3D, "Config/Bytes/Resmap");

                FishConfig.SetupResMap(bytes);
                //GlobalAudioMgr.Instance.LunchConfAudioRes(loadList);
                FishResManager.Instance.Init(loadList, loadDictList);
                isInitedLoadlistCompleted = true;
            }
            else if (curLoadingType == LoadingType.GameIn)
            {
                byte[] bytes = ResManager.LoadBytes(GameEnum.Fish_3D, "Config/Bytes/Resmap");
                FishConfig.SetupResMap(bytes);
                //GlobalAudioMgr.Instance.LunchConfAudioRes(loadList);
                FishResManager.Instance.Init(loadList, loadDictList);
                isInitedLoadlistCompleted = true;
            }
            else if(curLoadingType == LoadingType.MainHall)
            {
#if UNITY_EDITOR
                ConfigTables.Instance.LunchAllConf(loadList);
#else
                ConfigTables.Instance.LunchConfWithMainHall(loadList);
#endif
                isInitedLoadlistCompleted = true;
            }
            else if (curLoadingType == LoadingType.OnlyFishes)
            {
                ConfigTables.Instance.LunchAllConf(loadList);
                byte[] bytes = ResManager.LoadBytes(GameEnum.Fish_3D, "Config/Bytes/Resmap");
                FishConfig.SetupResMap(bytes);
                uint[] fishSourceIds = FishConfig.GetLoadResIDList(FishResType.FishPrefabMap);
                GameObjDictLoadItem fishloadDict = new GameObjDictLoadItem(FishResType.FishPrefabMap, ResPath.FishModel, fishSourceIds, FishResManager.Instance.FishPrefabMap);
                loadDictList.Add(fishloadDict);
                uint[] shape_ids = FishConfig.GetLoadResIDList(FishResType.FishShape);
                loadDictList.Add(new GameObjDictLoadItem(FishResType.FishShape, ResPath.FishShapePath, shape_ids, FishResManager.Instance.FishShapeMap));
                isInitedLoadlistCompleted = true;
            }
            else
            {
                isInitedLoadlistCompleted = true;
            }
		}
        else
        {
			isInitedLoadlistCompleted = true;
		}
		GlobalUpdate.RegisterUpdate (GlobalLoading.Instance);
        MonoDelegate.Instance.StartCoroutine(StartLaunch(is_start_game));
    }

	IEnumerator StartLaunch(bool is_start_game)
	{
		while (!isInitedLoadlistCompleted)
			yield return null;

        if (Application.isPlaying && ShowProgressUI)
        {
            if (is_start_game)
            {
                GameObject go = Resources.Load<GameObject>("InnerRes/Progress_bar_new");
                mLoadingViewer = new LoadingViewer(GameUtils.CreateGo(go, SceneObjMgr.Instance.UIPanelTransform));
                mLoadingViewer.Init(loadingTipsList);
            }
            else
            {
                ResManager.LoadAsset<GameObject>(ResPath.NewUIPath + "__res", (abinfo, asset) =>
                {
                    if(asset != null)
                        ResManager.save_list.Add(asset);
                }, GameEnum.Fish_3D);

                WndManager.LoadUIGameObject("Progress_bar", delegate(GameObject uiRefGo) 
                {
                    mLoadingViewer = new LoadingViewer(GameUtils.CreateGo(uiRefGo, SceneObjMgr.Instance.UIPanelTransform));
                    mLoadingViewer.Init(loadingTipsList);
                });
            }
		}
		float percent = 0f;
		if (!isFirstLaunching)
        {
			while (percent < 1f)
            {
				percent += Time.deltaTime * 1;
				UpdateLoadingProgress (percent);
				yield return null;
            }
			yield break;
		}

        int total = loadList.Count;
		loadDictList.ForEach (x => total += x.idList.Count);

        float delta_time = 0.03f;
        float _time = Time.realtimeSinceStartup + delta_time;
        List<string> ab_list = new List<string>();//需要释放的ab列表
        int error_count = 0;
        for (int i = 0; i < loadList.Count; i++)
        {
            var itm = loadList[i];
            error_count = 0;
            while (true)
            {
                try
                {
                    if (itm.loadType == Kubility.ResType.Binary)
                    {
                        byte[] data = ResManager.LoadBytes(itm.configType, itm.resId);

                        itm.finishCb.TryCall(new BinaryAsset { bytes = data });
                    }
                    else if (itm.loadType == Kubility.ResType.Prefab)
                    {
                        ResManager.LoadAsset<GameObject>(itm.resId, (abinfo, asset) => 
                        {
                            if(asset != null)
                                itm.finishCb.TryCall(GameUtils.ResumeShader(asset));
                        }, GameEnum.Fish_3D);

                        if (itm.mIsUnloadAB)
                        {
                            ab_list.Add(itm.resId);
                        }
                    }
                    else if (itm.loadType == Kubility.ResType.Texture)
                    {
                        ResManager.LoadAsset<Texture>(itm.resId, (abinfo, asset) =>
                        {
                            if(asset != null)
                                itm.finishCb.TryCall(asset);

                            if(abinfo != null)
                                ResManager.UnloadAB(abinfo);
                        }, GameEnum.Fish_3D);
                    }
                    break;
                }
                catch (Exception e)
                {
                    LogMgr.LogError(e.Message);
                    LogMgr.LogError(e.StackTrace);
                    error_count++;
                }
                if (error_count > 3)
                {
                    LogMgr.LogError("文件加载失败：" + itm.resId);
                    break;
                }
                else
                {
                    yield return null;
                }
            }
            UpdateLoadingProgress((i + 1f) / (float)total);
            if (Time.realtimeSinceStartup > _time)
            {
                _time = Time.realtimeSinceStartup + delta_time;
                yield return null;
            }
        }
        for (int i = 0; i < ab_list.Count; i++)
        {
            ResManager.UnloadAB<GameObject>(ab_list[i]);
        }
        ab_list.Clear();

        int loadedNum = loadList.Count;
		foreach (var itmDic in loadDictList) {
			foreach (var id in itmDic.idList)
            {
                string url = string.Format(itmDic.resFormat, id);
                error_count = 0;
                while (true) {
                    try {
                        ResManager.LoadAsset<GameObject>(url, (abinfo, asset) => 
                        {
                            if(asset != null)
                                itmDic.dicts[id] = GameUtils.ResumeShader(asset);
                        }, GameEnum.Fish_3D);

                        ab_list.Add(url);
                        break;
                    }
                    catch (Exception e)
                    {
                        LogMgr.LogError(e.Message);
                        LogMgr.LogError(e.StackTrace);
                        error_count++;
                    }
                    if (error_count > 3)
                    {
                        LogMgr.LogError("文件加载失败：" + url);
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                if (Time.realtimeSinceStartup > _time)
                {
                    _time = Time.realtimeSinceStartup + delta_time;
                    yield return null;
                }
				UpdateLoadingProgress ((float)loadedNum / total);
				loadedNum++;
            }
            if (ab_list.Count > 50)
            {
                for (int i = 0; i < ab_list.Count; i++)
                {
                    ResManager.UnloadAB<GameObject>(ab_list[i]);
                }
                ab_list.Clear();
            }
        }
        for (int i = 0; i < ab_list.Count; i++)
        {
            ResManager.UnloadAB<GameObject>(ab_list[i]);
        }

        Resources.UnloadUnusedAssets();
        GC.Collect();

        for (int i = 0; i < 10; i++) {//暂停10帧，进行GC相关释放
            yield return null;
        }
        UpdateLoadingProgress(1f);
    }

	public void LoadLuaAssetbundles() {

	}

	void UpdateLoadingProgress(float percent)
	{
		if (!Application.isPlaying) {
			return;	
		}
		if (mLoadingViewer == null)
			return;
		if (ShowProgressUI)
			mLoadingViewer.SetProgress (percent);

	}

	bool autoClose = true;


	public void close()
	{
		if (mLoadingViewer != null)
			mLoadingViewer.Close ();
		mLoadingViewer = null;
		loadDictList.Clear ();
		loadList.Clear ();
		GlobalUpdate.UnRegisterUpdate (GlobalLoading.Instance);
	}

	public void Update (float delta) 
	{
		Kubility.KAssetDispather.Instance.Update (delta);
        if (mLoadingViewer != null) {
            mLoadingViewer.Update(delta);
            if (mLoadingViewer.IsCompleted) {
                if (autoClose)
                    close();
                mOnCompleteFn.TryCall();
                mOnCompleteFn = null;
            }
        }
	}

	void OnEnteredRoom(object data)
	{
		close ();
		isFirstLaunching = false;
	}
    void OnEnterRoomFail(object data) {
        close();
        //isFirstLaunching = false;
    }

#if UNITY_EDITOR
	public void LoadConfigOnly(System.Action cb){
		loadList.Clear ();
		ConfigTables.Instance.LunchAllConf (loadList);
		for (int i = 0; i < loadList.Count; i++) {
			var itm = loadList [i];
			byte[] buffer = File.ReadAllBytes (Application.dataPath + "/" + itm.resId+".byte");
			BinaryAsset bAsst = new BinaryAsset();
			bAsst.bytes = buffer;
            itm.finishCb.TryCall(bAsst);
		}
		cb.TryCall ();
	}
#endif
}
