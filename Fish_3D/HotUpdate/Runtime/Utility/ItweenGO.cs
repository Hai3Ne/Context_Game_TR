using System;
using UnityEngine;
using System.Collections;
public class ItweenGO : MonoBehaviour
{
	public Action<object> updateFn;
	public Action completeFn;
	
	public static ItweenGO ValueTo(Vector3 fromVal, Vector3 toVal, float time, iTween.EaseType easetype, Action<Vector3> onupdate, Action oncomplete)
	{
		return ValueTo<Vector3> (fromVal, toVal, time, easetype, onupdate, oncomplete);
	}

	public static ItweenGO ValueTo<T>(T fromVal, T toVal, float time, iTween.EaseType easetype, Action<T> onupdate, Action oncomplete)
	{
		GameObject tmpGo = new GameObject ();
		ItweenGO go = tmpGo.AddComponent<ItweenGO> ();
		go.updateFn = delegate(object oo) {
			onupdate.Invoke ((T)oo);	
		};

		go.completeFn = oncomplete;
		iTween.ValueTo (tmpGo, iTween.Hash("from",fromVal, "to", toVal, "islocal",true, "time",time, "easetype", easetype, "onupdate", "UpdateFun", "oncomplete", "CompletedFun"));

		return go;
	}

	public void Destroy()
	{
		GameObject.Destroy (this.gameObject);
	}

	void UpdateFun(object o)
	{
		updateFn.Invoke (o);
	}

	void CompletedFun()
	{
		completeFn.TryCall ();
		GameObject.Destroy (this.gameObject, 0.1f);
	}
}

