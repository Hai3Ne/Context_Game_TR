using System;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{

	public static void Begin(GameObject go, float life = -1f, Action cb = null)
	{
		if (go != null) {
			
			AutoDisable ad = go.GetComponent<AutoDisable> ();
			if (ad == null) {
				ad = go.AddComponent<AutoDisable> ();			
			}
			ad.duration = life;
			ad.finishCb = cb;
			go.SetActive (true);
			ad.duration = ad.duration <= 0 ? GameUtils.CalPSLife (go) : ad.duration;
		}else {
			cb.TryCall ();
		}
	}

	public Action finishCb;
	public float duration = 0;
	void Start()
	{
		
	}


	void Update()
	{
		if (duration > 0) {
			duration -= Time.deltaTime;
		}
		if (duration <= 0) {
			this.gameObject.SetActive(false);
			finishCb.TryCall ();
		}
	}
}

