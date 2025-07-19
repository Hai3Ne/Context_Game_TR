using UnityEngine;
using System.Collections;

public class UI_GuideTips : MonoBehaviour {

	public UISprite dialogBgSp,dialogArrowSp;
	public UILabel dialogText; 
	public float range;
	public int arrowDirect = 0;

	const float span = 14.7f;
	// Use this for initialization

	public bool isClicked = false;
    public bool mFlipH = false;//是否水平翻转
	void Start () {
		layout ();
		UIEventListener.Get (dialogBgSp.gameObject).onClick = HandleClick;
		GetComponent<UIPanel> ().depth = 20;
	}

	void HandleClick(GameObject go)
	{
		isClicked = true;	
	}


	[System.NonSerialized]
	public bool isAutoHide = true;
	// Update is called once per frame
	float showDuration = 3f;
	float loadedTime = 0f;
	void Update () {
		if (isAutoHide == false)
			return;
		if (this.gameObject.activeSelf) {
			loadedTime += Time.deltaTime;
			if (isClicked)
				this.gameObject.SetActive (false);
			if (loadedTime >= showDuration)
				this.gameObject.SetActive (false);
		}
	}
	
	public void Init(string text, Vector3 wp, Vector3 offset){
		setMessage (text);
		layout ();
		setWorldPosition (wp,offset);
		this.gameObject.SetActive (true);
		loadedTime = 0f;
        this.mFlipH = false;
	}

	void setMessage(string text){
		dialogText.text = text;

	}

	void setWorldPosition(Vector3 wp, Vector3 offset){
		Vector3 lpos = this.transform.parent.InverseTransformPoint (wp);
		lpos += offset;
		this.transform.localPosition = lpos + dialogArrowSp.transform.localPosition * -1f;
	}

	public void layout(){
		float bgWidth = dialogText.width;
		float bgHeight = dialogText.height;
		dialogText.transform.localPosition = new Vector3 ((bgWidth - dialogText.width) * 0.5f, (dialogText.height - bgHeight) * 0.5f, 0f);
		dialogArrowSp.transform.localRotation = Quaternion.Euler(Vector3.forward);
		dialogBgSp.width = (int)(bgWidth + 25f);
		dialogBgSp.height = (int)(bgHeight + 25f);

		bgWidth = (float)dialogBgSp.width;
		bgHeight = (float)dialogBgSp.height;
		float hw = (bgWidth - dialogArrowSp.width) * 0.5f;
		float hh = (bgHeight - dialogArrowSp.width) * 0.5f;
		if (arrowDirect == 0) {
			dialogArrowSp.transform.localRotation = Quaternion.Euler(Vector3.forward*180f);
			dialogArrowSp.transform.localPosition = new Vector3(Mathf.Lerp(-hw, hw, range), bgHeight * 0.5f - span, 0f) ;	
		} else if (arrowDirect == 1) {
			dialogArrowSp.transform.localRotation = Quaternion.Euler(Vector3.forward);
			dialogArrowSp.transform.localPosition = new Vector3(Mathf.Lerp(-hw, hw, range), -bgHeight * 0.5f + span, 0f) ;	

		} else if (arrowDirect == 2) {
			dialogArrowSp.transform.localRotation = Quaternion.Euler(Vector3.forward*-90f);
			dialogArrowSp.transform.localPosition = new Vector3(-bgWidth * 0.5f + span, Mathf.Lerp(-hh, hh, range), 0f);

		} else if (arrowDirect == 3) {
			dialogArrowSp.transform.localRotation = Quaternion.Euler(Vector3.forward*90f);
			dialogArrowSp.transform.localPosition = new Vector3(bgWidth * 0.5f -span, Mathf.Lerp(-hh, hh, range), 0f);
		}

        if (this.mFlipH) {
            Vector3 pos = dialogArrowSp.transform.localPosition;
            dialogText.transform.localPosition += new Vector3(pos.x * 2, 0);
            dialogBgSp.transform.localPosition = dialogText.transform.localPosition;
        }
	}

	#if UNITY_EDITOR
	[ContextMenu("layout")]
	void testLayout(){
		layout ();
	}
	#endif

}
