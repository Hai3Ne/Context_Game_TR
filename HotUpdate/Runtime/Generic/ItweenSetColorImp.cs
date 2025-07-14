using System;
using UnityEngine;

[UnityEngine.RequireComponent(typeof(UIWidget))]
public class ItweenSetColorImp  : MonoBehaviour, iTween.ISetColor
{
	public static void Add(GameObject go){
		if (go.GetComponent<ItweenSetColorImp>()==null)
		go.AddComponent<ItweenSetColorImp> ();
		
	}

	UIWidget mWidget;
	public	Color color {
		get {
			if (mWidget == null)
				mWidget = this.GetComponent<UIWidget> ();
			return mWidget.color;
		}
		set {
			if (mWidget == null)
				mWidget = this.GetComponent<UIWidget> ();
			mWidget.color = value;
		}
	}
}