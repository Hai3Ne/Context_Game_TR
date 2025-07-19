using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 粒子层级
/// </summary>
public class PSOrder : MonoBehaviour {
    public UIWidget mTarget;
    public int RenderQueueOffset;//

    public bool IsOnce;//是否只设置一次

    private int pre_render;


    void LateUpdate() {
        if (this.mTarget != null && this.mTarget.drawCall != null && this.mTarget.drawCall.renderQueue != pre_render) {
            this.pre_render = this.mTarget.drawCall.renderQueue;

            GameUtils.SetPSRenderQueue(this.gameObject, this.mTarget.drawCall.sortingOrder, this.pre_render + this.RenderQueueOffset);
            if (this.IsOnce) {
                this.enabled = false;
            }
        }
	}
}
