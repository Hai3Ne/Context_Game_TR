using UnityEngine;
using System.Collections;

public class RateItemToggleUI : MonoBehaviour {
	public UISprite bgSp;
	public UILabel valueLabel;

    public string[] bgspStatus = new string[] { "beilv_btn_notc", "beilv_btn_choose" };
	public Color[] gradientTop = new Color[]{};
	public Color[] gradientBottom = new Color[]{};
	public Color[] outlineColors = new Color[]{};

	void Awake()
	{
		toggleValue = toggleValue;
	}

	bool mToggleValue = false;
	public bool toggleValue
	{
		get { return mToggleValue;}
		set 
		{
			mToggleValue = value;
			bgSp.spriteName = mToggleValue ? bgspStatus [1] : bgspStatus [0];
			valueLabel.applyGradient = true;
			valueLabel.effectStyle = UILabel.Effect.Outline;
			valueLabel.gradientTop = mToggleValue ? gradientTop [1] : gradientTop [0];
			valueLabel.gradientBottom = mToggleValue ? gradientBottom [1] : gradientBottom [0];
			valueLabel.effectColor = mToggleValue ? outlineColors [1] : outlineColors [0];
		}
	}
}

