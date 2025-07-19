using UnityEngine;
using System.Collections;

//拖动旋转组件
public class DragRotate : UIDragDropItem {
    public Transform mTfFish;

    //protected override void OnDrag(Vector2 delta) {
    //    base.OnDrag(delta);
    //}
    protected override void OnDragDropMove(Vector2 delta) {
        this.mTfFish.Rotate(Vector3.up, -delta.x*0.3f);
    }
}
