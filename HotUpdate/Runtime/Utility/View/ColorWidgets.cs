using UnityEngine;
using System.Collections;

public class ColorWidgets : MonoBehaviour {

	Color mColor;
	public Color color {
		get { return mColor;}
		set {
			bool isUp = this.mColor != value;
			this.mColor = value;
			if (isUp) {
				UIWidget[] ws = GetComponentsInChildren<UIWidget> ();
				foreach (var w in ws)
					w.color = this.mColor;
			}
		}
	}


}
