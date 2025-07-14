using UnityEngine;
using System.Collections;

public class SkillCasterListenter : MonoBehaviour{
	public 	IceAttackPs ps;
	void OnCaseSkillAtk()
	{
		if (ps != null)
			ps.OnCaseSkillAtk();
	}
}

public class IceAttackPs : MonoBehaviour {

	public GameObject xuehuaBgGo,skHaloGo;
	public Vector3 bgOffset = new Vector3(0f,-12f, 35f);
	public Vector3 haloOffset = new Vector3(0f,15f, 20f);
	Vector3 mXuehuaPos,mHolaPos;
	Quaternion mRot;
	void Awake(){
		xuehuaBgGo.SetActive (false);
		skHaloGo.SetActive (false);
		mXuehuaPos = xuehuaBgGo.transform.position;
		mHolaPos = skHaloGo.transform.position;
		mRot = xuehuaBgGo.transform.rotation;
		excuteNextFrame ();
	}

	bool isSetSkCast = false;
	void excuteNextFrame(){
		if (isSetSkCast)
			return;
		if (this.transform.parent == null)
			return;
		isSetSkCast = true;
		Animator animtor = this.gameObject.GetComponent<Animator> ();
		if (animtor == null) {
			animtor = this.GetComponentInParent<Animator> ();
			if (animtor != null) {
				SkillCasterListenter ps = animtor.gameObject.GetComponent<SkillCasterListenter> ();
				if (ps == null)
					ps = animtor.gameObject.AddComponent<SkillCasterListenter> ();
				ps.ps = this;
			}
		}
	}

	public void OnCaseSkillAtk()
	{
		xuehuaBgGo.SetActive (true);
		skHaloGo.SetActive (true);
		mXuehuaPos = bgOffset + Camera.main.transform.position + (Camera.main.nearClipPlane)*Vector3.forward;
		mHolaPos = haloOffset + (this.transform.position);
		AutoDisable.Begin(xuehuaBgGo);
		AutoDisable.Begin(skHaloGo);
	}

	void Update()
	{
		if (!isSetSkCast) {
			excuteNextFrame ();
		}
		mHolaPos = haloOffset + (this.transform.position);

		xuehuaBgGo.transform.rotation = mRot;
		xuehuaBgGo.transform.position = mXuehuaPos;
		skHaloGo.transform.position = mHolaPos;
	}

	void OnAnimEvent(string evtname)
	{
//		if ("AtkEnd" == evtname) {
///			xuehuaBgGo.SetActive (false);
//			skHaloGo.SetActive (false);
//		}
	}
}
