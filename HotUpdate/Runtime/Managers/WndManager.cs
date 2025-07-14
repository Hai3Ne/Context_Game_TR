using UnityEngine;
using System.Collections.Generic;
using System;

public enum EnumPanelType{
	CenterPopUI = 0, FloatUI = 1
}

public enum EnumPanelPRI{
	HIGHT,
	MIDDLE,
	Low,
}

public interface IUIController
{
	EnumPanelPRI PRI{ get;}
	void Init (object data);
	void Show ();
	void Close ();
	EnumPanelType PanelType{ get;}
	bool CanShow { get;}
	bool IsActive{ get; }
	object BindData{ get; set;}
	event System.Action<IUIController> OnCloseEvent;
}

public class WndManager:SingleTon<WndManager>
{
	Dictionary<EnumUI, IUIController> uiCtrlDict = new Dictionary<EnumUI, IUIController> ();
	public void Init()
	{
		uiCtrlDict [EnumUI.GameRoomUI] = new GameRoomUIController ();
		uiCtrlDict [EnumUI.QuickBuyUI] = new QuickBuyUIController ();
		uiCtrlDict [EnumUI.RollMsgUI] = new ScrollingMessageUIController ();
		uiCtrlDict [EnumUI.PromptMsgUI] = new PromptSysMessageController ();
        uiCtrlDict [EnumUI.MsgBoxUI] = new MessageBoxUIController();
        uiCtrlDict [EnumUI.UI_GetAward] = new UI_GetAwardController();
        uiCtrlDict [EnumUI.UI_Setting] = new UI_SettingController();
        uiCtrlDict [EnumUI.UI_TuJian] = new UI_TuJianController();
        uiCtrlDict [EnumUI.UI_ExitGame] = new UI_ExitGameController();
        uiCtrlDict [EnumUI.UI_BoxRank] = new UI_BoxRankController();
        uiCtrlDict[EnumUI.UI_BoxRankAward] = new UI_BoxRankAwardController();
        uiCtrlDict[EnumUI.UI_FishInfo] = new UI_FishInfoController();
        uiCtrlDict[EnumUI.UI_LauncherInfo] = new UI_LauncherInfoController();
        uiCtrlDict[EnumUI.UI_Help] = new UI_HelpController();
        uiCtrlDict[EnumUI.UI_LotteryDraw] = new UI_LotteryDrawController();

        //RegisterGlobalMsg(SysEventType.PlayerSitInTalbe, HandleAction);
    }

	void HandleAction (object obj)
	{
		//ShowUI (EnumUI.GameRoomUI, obj);
	}

	public int currentPanelDepth = 10;
	List<Transform> m_WndList = new List<Transform>();
	Dictionary<Transform, int> panelStarDethDic = new Dictionary<Transform, int> ();
	public void Push(GameObject obj, bool sort = true)  { if (obj == null || obj.transform == null) return;	Push(obj.transform, sort); }
	public void Push(Transform obj, bool sort = true)
	{
		if(sort)
			SetPanelSort(obj.gameObject);
		m_WndList.Add(obj);
	}

    public void SetPanelSort(GameObject go)
    {
        int maxdepth = UI.GetDepth();
        panelStarDethDic[go.transform] = maxdepth;// currentPanelDepth;
		UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
        //int maxdepth = currentPanelDepth;
		if (panels != null && panels.Length > 0)
        {
			for (int i = 0; i < panels.Length; i++) {
                panels[i].depth += maxdepth + 1;
				maxdepth = Mathf.Max(maxdepth, panels [i].depth);
			}
			currentPanelDepth = maxdepth;
        }
    }

	int activeWinCount = 0;
	public bool HasTopWnd   { get  { return activeWinCount > 0; }}

    public void Pop(GameObject go) { if (go == null || go.transform == null) return; Pop(go.transform); }
    public void Pop(Transform fo)
    {
        for (int i = 0; i < m_WndList.Count; ++i)
        {
            if(m_WndList[i] == fo)
            {
				currentPanelDepth = panelStarDethDic[fo];
				panelStarDethDic.Remove (fo);
				for (int k = i+1; k < m_WndList.Count; k++) {
					SetPanelSort(m_WndList[k].gameObject);
				}
                m_WndList.RemoveAt(i);
				GameObject.Destroy (fo.gameObject);
				return;
            }
        }
    }

    public void Clear()
    {
        m_WndList.Clear();
    }

	public void ShowDialog(string msg, Action<int> opCallback){
		PromptSysMessageController ctrl = (PromptSysMessageController)ShowUI (EnumUI.PromptMsgUI, msg);	
		ctrl.onCancelCb = delegate() {
			opCallback.TryCall(2);	
		};

		ctrl.onConfirmCb = delegate {
			opCallback.TryCall(1);	
		};
	}

	public IUIController ShowUI(EnumUI ui, object data = null, bool isForceShow = false)
	{
		IUIController ctrl = null;
		if (uiCtrlDict.TryGetValue (ui, out ctrl)) {
			if (ctrl.PanelType == EnumPanelType.CenterPopUI && isForceShow == false) {
                //2.同级界面，或无优先级界面，在其他界面已打开时，不允许打开
                //3.打开更高优先级界面时，自动关闭当前所有低优先级界面或无优先级界面
                var t = uiCtrlDict.Values.GetEnumerator();
                while (t.MoveNext()) {
                    if (t.Current.IsActive && t.Current.PanelType == EnumPanelType.CenterPopUI) {
                        if (t.Current.PRI <= ctrl.PRI) {
                            return null;
                        } else {
                            t.Current.Close();
                        }
                    }
                }
            }
            
			if (ctrl.CanShow) {
                GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.OpenUI,type:GameEnum.All);
				ctrl.Init (data);
				ctrl.Show ();
				ctrl.OnCloseEvent += HandleClose;
				if (ctrl.PanelType == EnumPanelType.CenterPopUI) {
					
					activeWinCount++;
				}
			}
		}
		return ctrl;
	}

	void HandleClose(IUIController ctrl)
	{
		ctrl.OnCloseEvent -= HandleClose;
		if (ctrl.CanShow) {
			if (ctrl.PanelType == EnumPanelType.CenterPopUI) {
				activeWinCount--;
			}
		}
	}

	public void CloseUI(EnumUI ui)
	{
		IUIController ctrl = null;
        if (uiCtrlDict.TryGetValue(ui, out ctrl)) {
            GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.CloseUI,type:GameEnum.All);
			ctrl.Close ();
		}
	}

    public void CloseAllUI() {//全部所有界面
        foreach (var item in uiCtrlDict.Values) {
            if (item.IsActive) {
                item.Close();
            }
        }
    }
    public void CleatCount()
	{
		activeWinCount = 0;
	}

	public bool isActive(EnumUI ui)
	{
		IUIController ctrl = null;
		if (uiCtrlDict.TryGetValue (ui, out ctrl)) {
			return ctrl.IsActive;
		}
		return false;
	}
    public IUIController GetCurActive() {//获取当前激活的界面
        var t = uiCtrlDict.Values.GetEnumerator();
        while (t.MoveNext()) {
            if (t.Current.IsActive) {
                return t.Current;
            }
        }
        return null;
    }

	public T GetController<T>()where T :IUIController
	{
		Type findType = typeof(T);
		foreach (var pair in uiCtrlDict) {
			if (pair.Value.GetType () == findType) {
				return (T)pair.Value;
			}
		}
		return default(T);
	}

    public static void LoadUIGameObject(string uiName, System.Action<GameObject> callback)
    {
        //Kubility.KAssetBundleManger.Instance.LoadGameObject(ResPath.UIPath + uiName, delegate (SmallAbStruct obj)
        //{
        //    callback.TryCall((GameObject)obj.MainObject);
        //});

        GameObject go = ResManager.LoadAsset<GameObject>(GameEnum.Fish_3D,ResPath.NewUIPath + uiName);
        callback.TryCall(go);
    }
    public static void LoadUIGameObject(string uiName, Transform transCon, System.Action<GameObject> callback) {
        //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.UIPath + uiName);
        //ResManager.LoadAsset<GameObject>(data, (sender) =>
        //{
        //    GameObject go = GameObject.Instantiate(sender);
        //    if (go != null && transCon != null)
        //    {
        //        Transform t = go.transform;
        //        t.parent = transCon;
        //        t.localPosition = Vector3.zero;
        //        t.localRotation = Quaternion.identity;
        //        t.localScale = Vector3.one;
        //        if (go.layer != transCon.gameObject.layer)
        //        {
        //            go.layer = transCon.gameObject.layer;
        //        }
        //    }
        //    go.AddComponent<ResCount>().ab_data = data;
        //    callback.TryCall(go);
        //});

        ResManager.LoadAsset<GameObject>(ResPath.NewUIPath + uiName, (abinfo, asset) =>
        {
            GameObject go = GameObject.Instantiate(asset);
            if (go != null && transCon != null)
            {
                Transform t = go.transform;
                t.parent = transCon;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                if (go.layer != transCon.gameObject.layer)
                {
                    go.layer = transCon.gameObject.layer;
                }
            }
            go.AddComponent<ResCount>().ab_info = abinfo;
            callback.TryCall(go);
        }, GameEnum.Fish_3D);
    }
}
