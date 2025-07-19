using UnityEngine;
using System.Collections;

public class EffectSkillParpre : MonoBehaviour {

	public System.Action OnComplete;
	public Vector3 targetWorldPosition;
	float delay = 0.01f;
	public float duratioin = 0.5f;
	public Transform skillNameTrans;
	Vector3 targetScale;
	void Start () {
		skillNameTrans.gameObject.SetActive (false);
		targetScale = skillNameTrans.localScale;
		skillNameTrans.localScale = Vector3.zero;

	}
	
	// Update is called once per frame
	void Update () {
		if (delay > 0) {
			delay -= Time.deltaTime;
			if (delay <= 0) {
				
				triggleMove ();
			}
		}
	}

	void triggleMove ()
	{
		float psTime = 1f;
		MeshTinColorFade.TryAddComponet (skillNameTrans.gameObject);
		iTween.FadeTo (skillNameTrans.gameObject, iTween.Hash("delay",psTime+Mathf.Max(0f,duratioin-0.5f), "alpha",0f, "time",0.3f));
		skillNameTrans.gameObject.SetActive (true);
		iTween.ScaleTo (skillNameTrans.gameObject, iTween.Hash("scale", targetScale, "time", 0.2f));
		iTween.MoveTo (skillNameTrans.gameObject, iTween.Hash("delay", psTime, "time", duratioin, "position", targetWorldPosition, "islocal", false, "oncomplete", "OnMoveOK", "oncompletetarget", this.gameObject));	
	}

	void OnMoveOK()
	{
		GameObject.Destroy (this.gameObject, 0.05f);
		OnComplete.TryCall ();
	}
}

public class MeshTinColorFade : MonoBehaviour, iTween.ISetColor
{
	public static void TryAddComponet(GameObject go)
	{
		if (!go.GetComponent<MeshTinColorFade> ()) {
			go.AddComponent<MeshTinColorFade> ();
		}
	}

	MeshRenderer mRender;
	void Awake()
	{
		mRender = this.GetComponent<MeshRenderer> ();
	}

	public Color color
	{
		get { return mRender.material.GetColor ("_TintColor");}
		set { mRender.material.SetColor ("_TintColor", value);}
	}
}
