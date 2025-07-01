using UnityEngine;
using System.Collections;

public class DragDropItemAnim : UIDragDropItem {
    private Vector3 min_pos;
    private Vector3 max_pos;
    public VoidDelegate event_drag_start;
    public VoidDelegate event_drag_end;
    protected override void Start() {
        this.min_pos = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(Vector3.zero);
        this.max_pos = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(Vector3.one);
        base.Start();
    }



    protected override void StartDragging() {
        this.time = 0;

        if (this.gameObject == UICamera.hoveredObject) {
            base.StartDragging();
        }
    }
    protected override void OnDragDropStart() {
        if (event_drag_start != null) {
            event_drag_start();
        }
        base.OnDragDropStart();
    }
    protected override void OnDragEnd() {
        base.OnDragEnd();
        if (event_drag_end != null) {
            event_drag_end();
        }
    }
    protected override void OnDragDropMove(Vector2 delta) {
        base.OnDragDropMove(delta);
        Vector3 pos = mTrans.position;
        pos.x = Mathf.Clamp(pos.x, this.min_pos.x, this.max_pos.x);
        pos.y = Mathf.Clamp(pos.y, this.min_pos.y, this.max_pos.y);
        this.mTrans.position = pos;
    }

    private float time;//变化时间
    public void LateUpdate() {
        if (mDragging) {
            this.time += Time.deltaTime * 5f;
            while(this.time > 2){
                this.time -= 2;
            }
            if (this.time > 1) {
                this.transform.localScale = Vector3.one * Mathf.Lerp(1.2f, 1.1f, this.time - 1);
            } else {
                this.transform.localScale = Vector3.one * Mathf.Lerp(1.1f, 1.2f, this.time);
            }

        }
    }
}
