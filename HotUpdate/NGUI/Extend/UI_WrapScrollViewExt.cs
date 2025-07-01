using UnityEngine;
using System.Collections;

public class UI_WrapScrollViewExt : UIScrollView {
	public Transform _childTrans;
	public Color disableColor = new Color (0.8f, 0.8f, 0.8f, 1f);
	float scalevalue = 0.001f;
	UIGrid mGrid;
	Vector2 boxSize = new Vector2(550f, 550f);
	Vector2 clipSize = new Vector2(1743f, 686f);
	float CellWidth = 550;
	SpringPanel mSpring;
	UICenterOnChild uiCenterOnChild;
	Transform centerChildTrans;
	public int centerChildIndex = 0;
	bool isFirst;
	void Start(){
		isFirst = true;
		uiCenterOnChild = this.GetComponentInChildren<UICenterOnChild> (true);
		mGrid = GetComponentInChildren<UIGrid> (true);
		mGrid.onReposition = delegate() {
			if (mGrid.transform.childCount > 1)
			{
				centerChildTrans = mGrid.transform.GetChild(centerChildIndex);
				CenterOn(centerChildTrans);
				MoveRelative (Vector3.zero);
			}
		};
		uiCenterOnChild.onCenter = HandleOnCenterChild;
	}

	public void CenterOn(Transform childTrans){
		if (childTrans == null)
			return;
		_childTrans = childTrans;
		if (centerOnChild != null)
			centerOnChild.CenterOn (childTrans);
	}

	public GameObject currentSelectedChild;
	void HandleOnCenterChild(GameObject centeredObject){
		currentSelectedChild = centeredObject;
	}

	void Update(){
		if (isFirst && centerOnChild != null && _childTrans!=null) {
			if (centerOnChild.centeredObject != _childTrans.gameObject) {
				centerOnChild.CenterOn (_childTrans);
				isFirst = false;
			}
		}
		if (mSpring == null) {
			mSpring = GetComponent<SpringPanel> ();
		    if (mSpring != null) {
			    mSpring.onSping = delegate(Vector3 offset) {
				    MoveRelative (offset);
				    UICenterOnClick.enabledCenterOne = true;
			    };
			    mSpring.onFinished = delegate {
				    UICenterOnClick.enabledCenterOne = true;
			    };
		    }
		}

	}

	public override void MoveRelative (Vector3 relative)
	{
		base.MoveRelative (relative);
		//LogMgr.Log (relative.x);
		SortItem (relative.x);

	}

	void SortItem(float moveDir){
		Transform child = null;
		int cn = mGrid.transform.childCount;
		if (moveDir < 0f) {
			child = mGrid.transform.GetChild (0);
			float right = mTrans.localPosition.x + child.localPosition.x + boxSize.x * 0.5f;
			if (right < -clipSize.x * 0.5f) {
				child.SetAsLastSibling ();
				child.localPosition = mGrid.transform.GetChild (cn-2).localPosition + Vector3.right * CellWidth;
			}
		}else{
			child = mGrid.transform.GetChild (cn - 1);
			float left = (mTrans.localPosition.x + child.localPosition.x) - boxSize.x * 0.5f;
			if (left > clipSize.x * 0.5f) {
				child.SetAsFirstSibling ();
				child.localPosition = mGrid.transform.GetChild (1).localPosition + Vector3.left * CellWidth;
			}
		}


		for (int i = 0; i < cn; i++) {
			child = mGrid.transform.GetChild (i);
			float xx = mTrans.parent.InverseTransformPoint (child.position).x;
			child.localScale = Vector3.one * Mathf.Max(1f-Mathf.Abs (xx) * scalevalue, 0.8f);
			child.localPosition = new Vector3 (child.localPosition.x, 0f,0f);
			child.localPosition += Vector3.up * Mathf.Abs (xx) * scalevalue*3;
			if (child.gameObject != currentSelectedChild) {
				child.GetComponent<ColorWidgets> ().color = disableColor;
				child.GetComponent<Animator> ().enabled = false;
				GameUtils.StopPS (child.gameObject);
			} else {
				child.GetComponent<Animator> ().enabled = true;
				child.GetComponent<ColorWidgets> ().color = Color.white;
				child.transform.localScale = Vector3.one;
				GameUtils.PlayPS (child.gameObject);
				UIWidget w = GetMaxDepthChild (child);

				if (w!= null&& w.drawCall != null)
					GameUtils.SetPSRenderQueue (child.gameObject, 0, w.drawCall.renderQueue + 5);
			}
		}
	}

	UIWidget GetMaxDepthChild(Transform trans){
		int k = -100;
		UIWidget[] dgs = trans.GetComponentsInChildren<UIWidget> ();
		UIWidget findWg = dgs[0];
		for (int i = 0; i < dgs.Length; i++) {
			if (dgs [i].depth > k) {
				k = dgs [i].depth;
				findWg = dgs [i];
			}
		}
		return findWg;
	}
}
