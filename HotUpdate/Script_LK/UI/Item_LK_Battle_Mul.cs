using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_LK_Battle_Mul : UIItem {
    public UILabel mLbMul;

    private LKFish mFish;
    private Transform mTf;
    private Transform mParent;

    public void InitData(LKFish fish) {
        this.mFish = fish;
        this.mTf = this.transform;
        this.mParent = this.mTf.parent;

        this.SetMul(fish.mMul);
    }

    public void SetMul(int mul) {
        this.mLbMul.text = string.Format("{0}倍",mul);
    }

    public void LateUpdate() {
        if (this.mFish == null || this.mFish.mIsValid == false) {
            GameObject.Destroy(this.gameObject);
        } else {
            //this.mTf.position = UICamera.mainCamera.ScreenToWorldPoint(this.mFish.ScreenPos);
            Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(this.mFish.ScreenPos);
            this.mTf.localPosition = this.mParent.InverseTransformPoint(pos) + new Vector3(0, 225);
        }
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_mul":
                this.mLbMul = tf.GetComponent<UILabel>();
                break;
        }
    }
}
