using UnityEngine;
using System.Collections;

public class EffectLCRBufProgress : MonoBehaviour {

	public UISlider progress;
	EffectArgs eArgs;
	float duration = 10f;
	// Use this for initialization
	void Start () {
		eArgs = this.gameObject.GetComponent<EffectArgs>();
        if (this.eArgs != null) {
            duration = eArgs.duration;
            nowTime = duration;
        }
	}

	float nowTime = 0f;
	void Update()
	{
		if (nowTime < 0f)
			return;
		float percent = Mathf.Clamp01(nowTime / duration);
		nowTime -= Time.deltaTime;
		if (progress != null)
			progress.value = percent;
	}
}
