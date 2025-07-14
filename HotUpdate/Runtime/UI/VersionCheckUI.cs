using System;
using UnityEngine;

public class VersionCheckUI
{
	GameObject versionProgresUI;
	Action completeFn;
	Transform barTrans;
	UILabel uiLabelProgress, uiLabelInfo;
	UISlider uiSlider;

	public void CheckVersion(Action cb)
	{
		completeFn = cb;
		uint localClientVersion = GameConfig.ClientVersionCode;
		//if (LocalSaver.TryGetData ("ClientVersion", out localClientVersion)) {
		if (GameParams.Instance.clientVersion > localClientVersion)
		{
			VersionManager.ShowVersionTick();
			return;
		}
		//}
		//LocalSaver.SetData ("ClientVersion", GameParams.Instance.clientVersion.ToString());
		barTrans = GameUtils.CreateGo<Transform>("InnerRes/Progress_bar_new", SceneObjMgr.Instance.UIPanelTransform);
		versionProgresUI = barTrans.gameObject;
		versionProgresUI.transform.localScale = Vector3.one;
		versionProgresUI.transform.localPosition = Vector3.zero;
		uiLabelProgress = barTrans.Find("start").GetComponentInChildren<UILabel>();
		uiLabelInfo = barTrans.Find("progressLabel").GetComponent<UILabel>();
		uiSlider = barTrans.GetComponent<UISlider>();
		VersionManager.Instance.mChkUI = this;
		GameObject.Destroy(versionProgresUI);
		versionProgresUI = null;
		completeFn.TryCall();
	}
		/*VersionManager.Instance.VersionCheck 
        (
			delegate(bool obj) 
            {
				
			},
            delegate 
            {
                GameObject.Destroy(versionProgresUI);
                versionProgresUI = null;
                completeFn.TryCall();
			});
	}*/

		//void VersionCheckComplete()
		//{
		//    MonoDelegate.Instance.Dispatch2MainThread (delegate() {
		//        GameObject.Destroy (versionProgresUI);
		//        versionProgresUI = null;
		//        completeFn.TryCall ();
		//    });
		//}


	public bool IsActive{set{ barTrans.gameObject.SetActive (value); }}
	public void setProgress (float percent)
    {
        if (this.uiSlider.value != percent && versionProgresUI != null)
        {
            uiLabelProgress.text = Mathf.CeilToInt(percent * 100) + "%";
            uiSlider.value = percent;
        }
	}
	public float progress{get{ return uiSlider.value;}}
	public void setLoadingInfo (string message)
    {
        if (versionProgresUI != null)
        {
            uiLabelInfo.text = message;
        }
	}
}

