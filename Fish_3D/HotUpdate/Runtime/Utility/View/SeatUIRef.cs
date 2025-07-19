using System;
using UnityEngine;
using System.Collections;


[Serializable]
public class UsrinfoUIRef 
{
	public GameObject panelObj;
	public UISprite listBgSp;
	public UITexture usrIconSp;
	public UILabel usrNickLabel, usrNameLabel;
}

public class SeatUIRef : MonoBehaviour {
	public UILabel GoldLabel, RateLabel;
	public Transform cannonTrans;
	public Transform goldIconTrans;
	public UsrinfoUIRef usrInfoUI;

}
