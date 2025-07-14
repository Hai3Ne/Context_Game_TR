using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineRenderScaler : MonoBehaviour {
	public float startWidth, endWidth;
	LineRenderer lineRenders;
	public float rate = 1f;
	void Start(){
		lineRenders = GetComponent<LineRenderer> ();
	}

	void Update ()
	{
		setupWidth ();
	}

	protected virtual void OnValidate (){
		if (lineRenders == null)
			lineRenders = GetComponent<LineRenderer> ();
		setupWidth ();
	}

	void setupWidth()
	{
		float s = this.transform.parent == null ? this.transform.localScale.x : this.transform.lossyScale .x;
		s *= Mathf.Max (1f, rate);
        lineRenders.startWidth = startWidth * s;
        lineRenders.endWidth = endWidth * s;
        //lineRenders.SetWidth (startWidth * s, endWidth*s);
	}
}
