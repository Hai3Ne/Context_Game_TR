using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadingViewer
{
	Transform mEffTrans;
	GameObject mTransGo;
	GameObject mEffectgo;
	UISlider mProgressSlider;
	UILabel m_ProgressLabel;
	UILabel m_Percentage;
	float mProgressValue;
	bool m_bShow;
    List<string> loadingStrs = new List<string>();
	float time = 3;
	float starTime = 0;
    public GameObject mEffObj;//飘雪特效

    public GameObject TransGo {
        get {
            return this.mTransGo;
        }
    }

	public LoadingViewer(GameObject viewGo)
	{
		mTransGo = viewGo;
        mTransGo.transform.localPosition = Vector3.zero;
		GameUtils.SetGOLayer (mTransGo, LayerMask.NameToLayer ("UI"));

		mEffTrans = mTransGo.transform.GetChild (2).GetChild (0);
		mEffectgo = mEffTrans.GetChild (0).gameObject;
		mEffectgo.SetActive (false);

		mProgressSlider = mTransGo.transform.GetComponent<UISlider> ();
		m_ProgressLabel = mTransGo.transform.GetChild (1).GetComponent<UILabel> ();
		m_Percentage = mTransGo.transform.GetChild (2).GetChild (1).GetComponent<UILabel> ();

        Transform img_1 = mTransGo.transform.Find("texture_1");
        Transform img_2 = mTransGo.transform.Find("texture_2");
        if (img_1 != null && img_2 != null) {
            if (UnityEngine.Random.Range(0, 100) < 50) {
                img_1.gameObject.SetActive(true);
                img_2.gameObject.SetActive(false);
            } else {
                img_1.gameObject.SetActive(false);
                img_2.gameObject.SetActive(true);
            }
        }
	}

	public void SetProgress(float percent)
	{
		mProgressValue = Mathf.Clamp01(percent);
	}

	public void Init(string[] loadstrs)
	{
		mProgressValue = 0f;
        loadingStrs.Clear();
        for (int i = 0; i < loadstrs.Length; i++) {
            if (string.IsNullOrEmpty(loadstrs[i]) == false) {
                loadingStrs.Add(loadstrs[i]);
            }
        }
		mProgressSlider.value = mProgressValue;
		mTransGo.SetActive (true);
        mEffectgo.SetActive(false);
        this.RefershTick();
		mIsCompleted = false;
	}
    private void RefershTick() {
        if (loadingStrs.Count > 0) {
            m_ProgressLabel.text = loadingStrs[UnityEngine.Random.Range(0, loadingStrs.Count)];
        } else {
            m_ProgressLabel.text = string.Empty;
        }
    }

	bool mIsCompleted = false;
	public bool IsCompleted{
		get { return mIsCompleted;}
	}

	public void Update(float delta)
	{
		if (mTransGo == null || !mTransGo.activeSelf) 
			return;

		float nowProgress = mProgressSlider.value;
		nowProgress = Mathf.Lerp(nowProgress, mProgressValue, 0.5f);
		if (nowProgress >= 0.1f && !mEffectgo.activeSelf)
			mEffectgo.SetActive(true);

		starTime += delta;
		if(starTime > time)
		{
            starTime = 0f;
            this.RefershTick();
		}
		m_Percentage.text = (int)(nowProgress * 100) + "%";
		mProgressSlider.value = nowProgress;
		if (nowProgress >= 1f)
			mIsCompleted = true;
	}

	public void Close()
	{
		if (mTransGo != null)
			GameObject.Destroy (mTransGo);
		mTransGo = null;
        if (mEffObj != null) {
            GameObject.Destroy(mEffObj);
        }
        mEffObj = null;
	}
}
