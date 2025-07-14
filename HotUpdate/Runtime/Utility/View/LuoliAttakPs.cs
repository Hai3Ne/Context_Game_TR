using UnityEngine;
using System.Collections;

public class LuoliAttakPs : MonoBehaviour {

	public GameObject AtkShoujiBw;
	public GameObject Attack0Go,Attack1Go, Attack2Go;

	Animator mAnim;
	void Start() {
		mAnim = this.GetComponentInParent<Animator> ();
	}
	void Update()
	{
		int subclip = (int)mAnim.GetFloat ("SubClip");
		if (subclip == 0) {
			AtkShoujiBw.SetActive (true);
			Attack2Go.SetActive (false);
			Attack0Go.SetActive (true);
			Attack1Go.SetActive (false);
		} else if (subclip == 1) {
			AtkShoujiBw.SetActive (true);
			Attack2Go.SetActive (false);
			Attack0Go.SetActive (false);
			Attack1Go.SetActive (true);
		} else if (subclip == 2) {
			AtkShoujiBw.SetActive (false);
			Attack0Go.SetActive (false);
			Attack1Go.SetActive (false);
			Attack2Go.SetActive (true);
		}
	}
}
