using System;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{

	public GameObject OnEffectObj;
	public GameObject[] OnEffectObjList;
	void Start(){
		if (OnEffectObj != null) {
			if (OnEffectObj.scene.isLoaded)
				OnEffectObj.SetActive (false);
		}
	}

	void OnAnimEvent(string evetname)
	{
		if (mOnAnimatorEventCallback != null)
			mOnAnimatorEventCallback.Invoke (evetname);
		if (evetname == "OnEffect") {
			if (OnEffectObj != null) {
				if (GameUtils.IsPrefab (OnEffectObj)) {
					var effObjInst = GameUtils.CreateGo(OnEffectObj);
					effObjInst.transform.position = OnEffectObj.transform.position;
					AutoDestroy.Begin (effObjInst);
					effObjInst.SendMessage ("OnEffect", this.transform, SendMessageOptions.DontRequireReceiver);
				} else {
					OnEffectObj.SetActive (false);
					AutoDisable.Begin (OnEffectObj);
					OnEffectObj.SendMessage ("OnEffect", this.transform, SendMessageOptions.DontRequireReceiver);
				}
			} else if (OnEffectObjList != null && OnEffectObjList.Length > 0) {
				foreach (var objeff in OnEffectObjList) {
					if (GameUtils.IsPrefab (objeff)) {
						var effInst = GameUtils.CreateGo (objeff);
						AutoDestroy.Begin (effInst);
					} else {
						objeff.SetActive (false);
						AutoDisable.Begin (objeff);
					}
				}
			}
		}
        //LogMgr.Log (evetname);
	}

	private event Action<string> mOnAnimatorEventCallback;
	public event Action<string> OnAnimatorEvent
	{
		add { mOnAnimatorEventCallback += value; }
		remove { mOnAnimatorEventCallback -= value; }
	}
}
