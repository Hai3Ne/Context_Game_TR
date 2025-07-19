using UnityEngine;
using System.Collections.Generic;

public class ComboItem
{
	public GameObject mTransGo;
	public byte clientSeat;
	UILabel[] Numlabels;
	UISprite preLabelSp, sliderBgSp, slidForBg;
	float m_LifeTime;
	float doubleHit_IntervalTime;
	UISlider mSlider;
	int[] bgidx = new int[]{10, 20, 30, 40};
	UIPanel mPanel;
	bool mIsActive = false;

	Animator mAnimtor;
	public Vector3 targetPos;
	float nowTime = 0f;
	public ComboItem(GameObject obj)
	{
		mTransGo = obj;
		Transform cmb = obj.transform.GetChild (0);
		mPanel = cmb.GetComponent<UIPanel> ();

		Transform num = cmb.GetChild (0);
		Numlabels = new UILabel[]{
			num.GetChild(0).GetComponent<UILabel>(),
			num.GetChild(1).GetComponent<UILabel>(),
			num.GetChild(2).GetComponent<UILabel>(),
			num.GetChild(3).GetComponent<UILabel>()
		};
        foreach (var item in Numlabels) {
            item.gameObject.SetActive(true);
        }
		preLabelSp = cmb.GetChild (2).GetComponent<UISprite>();
		sliderBgSp = cmb.GetChild (1).GetComponent<UISprite>();
		slidForBg  = cmb.GetChild (1).GetChild(0).GetComponent<UISprite>();

		doubleHit_IntervalTime = FishConfig.Instance.GameSettingConf.ComboCD;
		mAnimtor = mTransGo.GetComponentInChildren<Animator> ();
		mAnimtor.enabled = false;
		mPanel.alpha = 0f;
		mSlider = mAnimtor.GetComponentInChildren<UISlider> ();
	}

	public void Init(ushort ComboCount)
	{
		int idx = 1;
		for (int i = bgidx.Length-1; i >= 0; i--) {
			if (ComboCount / bgidx [i] > 0) {
				idx = i + 1;
				break;
			}
		}
		System.Array.ForEach (Numlabels, x => x.alpha = 0f);
		Numlabels [idx - 1].alpha = 1f;
		Numlabels [idx - 1].text = ComboCount.ToString ();
		preLabelSp.spriteName = string.Format ("DoubleHit_ZT_{0}", idx);
		sliderBgSp.spriteName = string.Format("DoubleHit_bg_{0}", idx);
		slidForBg.spriteName  = string.Format("DoubleHit_energy_{0}", idx);
		GameUtils.PlayAnimator (mAnimtor);

		mPanel.alpha = 1f;
		mIsActive = true;
		mSlider.value = 1f;
		nowTime = 0f;
	}

	public bool Update(float delta) 
	{
		if (!mIsActive)
			return false;
		nowTime += delta;
		if (mPanel.alpha < 0.1f)
		{
			Close ();
			return false;
		}
		float percent = (doubleHit_IntervalTime - nowTime) / doubleHit_IntervalTime;
		mSlider.value = percent;
		return true;
	}

	public bool IsActive { get {return mIsActive;}}
	public void Close()
	{
		mAnimtor.enabled = false;
		mIsActive = false;
		mPanel.alpha = 0f;
	}

	public void Destroy()
	{
		GameObject.Destroy (mTransGo.gameObject);
		mTransGo = null;
	}
}