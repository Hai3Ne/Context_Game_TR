using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持续一定时间后，渐隐消失
/// </summary>
public class LK_Delay_Hide : MonoBehaviour {
    public List<UIWidget> mUIWidgetList = new List<UIWidget>();
    public float mTime;
    public float mAlpha = 1;
    public void InitData(float time) {
        this.mUIWidgetList.Clear();
        this.mUIWidgetList.AddRange(this.GetComponentsInChildren<UIWidget>());
        this.mTime = time;
        this.mAlpha = 1;
    }
    public static void DelayHide(GameObject obj, float time) {
        LK_Delay_Hide hide = obj.GetComponent<LK_Delay_Hide>();
        if (hide == null) {
            hide = obj.AddComponent<LK_Delay_Hide>();
        }
        hide.InitData(time);
    }

	void Update () {
        if (this.mTime > 0) {
            this.mTime -= Time.deltaTime;
        } else {
            this.mAlpha = Mathf.Max(0, this.mAlpha - Time.deltaTime * 0.6f);
            foreach (var item in this.mUIWidgetList) {
                item.alpha = this.mAlpha;
            }
            if (this.mAlpha <= 0) {
                GameObject.Destroy(this.gameObject);
            }
        }
	}
}
