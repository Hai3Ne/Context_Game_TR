using UnityEngine;
using System.Collections;

public class HeroItemRender : MonoBehaviour {
	public UISprite maskObj, iconObj,frameSp;
	public UILabel LifeLabel, countLabel;
    public UILabel mLbKey;//快捷键文本
    public UILabel mLbName;//英雄名称

	[System.NonSerialized]
	public uint heroCfgID, heroItemCfgId;


}
