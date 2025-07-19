using UnityEngine;
using System.Collections;

/// <summary>
/// 位置跟随
/// </summary>
public class EffFollowPos : MonoBehaviour {
    public Transform mTarget;
    public void InitData(Transform tf) {
        this.mTarget = tf;
        this.enabled = true;
    }

    public void LateUpdate() {
        if (this.mTarget != null) {
            Vector3 pos = Utility.MainCam.WorldToViewportPoint(this.mTarget.position);
            pos.z = 0;
            this.transform.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(pos);
        } else {
            this.enabled = false;
        }
    }
}
