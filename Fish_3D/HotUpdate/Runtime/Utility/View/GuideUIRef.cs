using UnityEngine;
using System.Collections;

public class GuideUIRef : MonoBehaviour {
	public UITextureClipHole bgMask;
	public GameObject npc, nextBtn, bgTrans;
	public UISprite dialogBgSp, dialogArrowSp, selectFrameSp,titleLabel;
	public UILabel dialogText;
	public GameObject mFinger, skipBtn;

	public int arrowDirect = 2;
	public float range;
	// Use this for initialization
	void Start () {
		UIEventListener.Get (nextBtn).onClick = delegate(GameObject go) {
			OnClickNext.TryCall();
		};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setHoleRect(Vector4 holeRect){
		if (holeRect != Vector4.zero) {
			
		}
		bgMask.clipRect = holeRect;
		bgMask.mIsDirty = true;
	}

	public System.Action OnClickNext;
	public void ShowMsg(string content){
		dialogText.text = content;
	}

	Vector3 mPosition;
	public void setDialogPosition(Vector3 uiLocPos){
		bgTrans.transform.localPosition = uiLocPos;
		mPosition = bgTrans.transform.localPosition;
		LayoutUI ();
	}

	public void setFingerPos(Vector3 locpos){
		locpos.z = -1f;
		mFinger.transform.localPosition = locpos;
	}

	public float span = 2f;

	public void LayoutUI(){
		float bgWidth = dialogText.width;
		float bgHeight = dialogText.height;
		float titleHeight = titleLabel.gameObject.activeSelf ? titleLabel.height : 0f;
		dialogBgSp.width = (int)(bgWidth+30f);
		dialogBgSp.height = (int)(bgHeight + titleHeight + 25f);
		if (titleLabel.gameObject.activeSelf) {
			titleLabel.transform.localPosition = bgTrans.transform.localPosition + new Vector3 (-bgWidth * 0.5f + titleLabel.width * 0.5f, bgHeight * 0.5f);
			dialogText.transform.localPosition = bgTrans.transform.localPosition + new Vector3 ((bgWidth - dialogText.width) * 0.5f, (dialogText.height - bgHeight) * 0.5f-titleHeight+25f, 0f);
			nextBtn.transform.localPosition = bgTrans.transform.localPosition + new Vector3 (bgWidth*0.5f - 40f, -bgHeight*0.5f + 5f, 0f);
		} else {
			dialogText.transform.localPosition = bgTrans.transform.localPosition + new Vector3 ((bgWidth - dialogText.width) * 0.5f, (dialogText.height - bgHeight) * 0.5f, 0f);
			nextBtn.transform.localPosition = bgTrans.transform.localPosition + new Vector3 (bgWidth*0.5f - 40f, -bgHeight*0.5f + 10f, 0f);
		}


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
	}
	#if UNITY_EDITOR
	[ContextMenu("layout")]
	void testLayout(){
		LayoutUI ();
	}
	#endif
}
