using UnityEngine;
using System.Collections;

public struct TrailConf
{
	public string tarState;
	public float fadeInTime;
	public float tweenToTime;
}


public class AnimtionTrail : MonoBehaviour {

	public WeaponTrail Trail;
	public TrailConf[] trailConfs;

	public float trailTime = 1f;

	float t = 0.33f;
	protected float animationIncrement = 0.003f; 
	private float tempT = 0;
	AnimationClip mAnimClip;
	Animator mAnimator;

	// Use this for initialization
	AnimationEventListener AnimListener;
	void Start () {
		Trail.SetTime (trailTime, 0, 1);
		mAnimator = GetComponentInChildren<Animator> ();
		AnimListener = mAnimator.GetComponent<AnimationEventListener> ();
		AnimListener.OnAnimatorEvent += AnimListener_OnAnimatorEvent;
	}

	void AnimListener_OnAnimatorEvent (string obj)
	{
		if (obj == "TrailBegin") {
			Trail.SetTime (trailTime, 0f, 1f);
			Trail.StartTrail (0.1f, 0.4f);
		} else if (obj == "TrailEnd") {
			Trail.FadeOut(0.1f);
		}
	}

	// Update is called once per frame
	void LateUpdate () {

		t = Mathf.Clamp (Time.deltaTime, 0, 0.066f);
		if (t > 0) {
			while (tempT < t) {
				tempT += animationIncrement;

				if (Trail.time > 0) {
					Trail.Itterate (Time.time - t + tempT);
				} else {
					Trail.ClearTrail ();
				}
			}
			tempT -= t;
			if (Trail.time > 0) {
				Trail.UpdateTrail (Time.time, t);
			}
		}
	}
	 
}

