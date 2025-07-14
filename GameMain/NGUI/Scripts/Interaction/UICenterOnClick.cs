//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attaching this script to an element of a scroll view will make it possible to center on it by clicking on it.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Center Scroll View on Click")]
public class UICenterOnClick : MonoBehaviour
{

	public static bool enabledCenterOne = true;
	Vector2 lastPressPos;
	bool isCenter = false;
    private float touch_len = 0;
	void OnPress (bool pressed)
	{

        if (pressed) {
            touch_len = 0;
            lastPressPos = UICamera.currentTouch.pos;
            isCenter = false;
		} else {
            if (enabledCenterOne == false || isCenter == true) {
                return;
            }
			UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(gameObject);
            if (center.centeredObject == this.gameObject && Mathf.Abs(touch_len) < Screen.width * 0.05f) {
                return;
            }
			Transform parnentCon = this.transform.parent;
			Vector2 nowPos = UICamera.currentTouch.pos;
			Vector2 dir = (nowPos - lastPressPos);
            //LogMgr.Log ("dir :: "+dir);
			if (dir.magnitude < 2f) {
				CenterOne (this.transform);
			} else if (dir.x < 0) {
				CenterOne (parnentCon.GetChild (center.centeredObject.transform.GetSiblingIndex () + 1));
			} else {
				CenterOne (parnentCon.GetChild (center.centeredObject.transform.GetSiblingIndex () - 1));
			}
		}
	}

	void OnDrag(Vector2 delta){
        if (isCenter == true) {
            return;
        }
        touch_len += delta.x;
        if (Mathf.Abs(touch_len) > Screen.width*0.05f) {
			UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(gameObject);
			Vector2 nowPos = UICamera.currentTouch.pos;
			Vector2 dir = (nowPos - lastPressPos);
			Transform parnentCon = this.transform.parent;
			if (dir.x < 0) {
				CenterOne (parnentCon.GetChild (center.centeredObject.transform.GetSiblingIndex()+1));
			} else {
				CenterOne (parnentCon.GetChild (center.centeredObject.transform.GetSiblingIndex()-1));
			}
		}
	}

	void CenterOne (Transform targetTrans)
	{
		if (isCenter)
			return;
		isCenter = true;
		UICenterOnChild center = NGUITools.FindInParents<UICenterOnChild>(gameObject);
		UIPanel panel = NGUITools.FindInParents<UIPanel>(gameObject);

		if (center != null)
		{
			if (center.enabled)
				center.CenterOn(targetTrans);
		}
		else if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
		{
			UIScrollView sv = panel.GetComponent<UIScrollView>();
			Vector3 offset = -panel.cachedTransform.InverseTransformPoint(targetTrans.position);
			if (!sv.canMoveHorizontally) offset.x = panel.cachedTransform.localPosition.x;
			if (!sv.canMoveVertically) offset.y = panel.cachedTransform.localPosition.y;
			SpringPanel.Begin(panel.cachedGameObject, offset, 6f);
		}
	}
}
