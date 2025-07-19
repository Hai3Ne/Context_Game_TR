using UnityEngine;
using System.Collections;

public class LotteryEff : MonoBehaviour {

	public AnimationCurve stopCure, playCure;
	public GameObject light0, light1;
	ParticleSystem[] pslist;
	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[ContextMenu("StopPS")]
	public void StopPS()
	{
		Init ();
		var fot = new ParticleSystem.MinMaxCurve(2f,  stopCure);
		for (int i = 0; i < pslist.Length; i++) {
			var tmp = pslist [i].textureSheetAnimation;
			tmp.frameOverTime = fot;
		}
	}

	[ContextMenu("PlayPS")]
	public void PlayPS()
	{
		Init ();
		var fot = new ParticleSystem.MinMaxCurve(2f, playCure);
		for (int i = 0; i < pslist.Length; i++) {
			var tmp = pslist [i].textureSheetAnimation;
			tmp.frameOverTime = fot;
		}
	}

	bool isInit = false;
	void Init()
	{
		if (isInit)
			return;
		isInit = true;
		ParticleSystem[] p0 = light0.GetComponentsInChildren<ParticleSystem> ();
		ParticleSystem[] p1 = light1.GetComponentsInChildren<ParticleSystem> ();
		pslist = new ParticleSystem[p0.Length + p1.Length];
		System.Array.Copy (p0, 0, pslist, 0, p0.Length);
		System.Array.Copy (p1, 0, pslist, p0.Length, p1.Length);
	}
}
