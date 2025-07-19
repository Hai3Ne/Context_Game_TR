using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class TrailRenderScaler : MonoBehaviour {
	public float startWidth, endWidth;
	TrailRenderer trailRenders;
	public float rate = 1f;
	void Start(){
		trailRenders = GetComponent<TrailRenderer> ();
	}

	void Update ()
	{
		setupWidth ();
	}

	protected virtual void OnValidate (){
		if (trailRenders == null)
			trailRenders = GetComponent<TrailRenderer> ();
		setupWidth ();
	}

	void setupWidth()
	{
		
		float s = this.transform.parent == null ? this.transform.localScale.x : this.transform.lossyScale .x;
		s *= Mathf.Max (1f, rate);
		trailRenders.endWidth = endWidth * s;
		trailRenders.startWidth = startWidth * s;
	}
}
