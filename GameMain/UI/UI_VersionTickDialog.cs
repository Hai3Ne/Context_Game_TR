using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_VersionTickDialog : MonoBehaviour {
    public static UI_VersionTickDialog Show() {
        GameObject prefab = Resources.Load<GameObject>("UI_VersionTickDialog");
        GameObject dialog = GameObject.Instantiate(prefab,GameObject.Find("SceneUIRoot/ParentPanel/ContainerPanel").transform) as GameObject;
        dialog.transform.localPosition = Vector3.zero;
        dialog.transform.localScale = Vector3.one;
        dialog.transform.localRotation = Quaternion.identity;

        return dialog.AddComponent<UI_VersionTickDialog>();
    }

    public UILabel mLbTick;
    public VoidDelegate mCall;
    public UILabel mTip;
    public void InitData(string tick,string tip, VoidDelegate call) {
        Traversal(this.transform, this.OnNodeAsset);
        this.mLbTick.text = tick;
        this.mCall = call;
        mTip.text = tip;
    }
    public void Traversal(Transform trans, VoidCall<string, Transform> call)
    {
        call(trans.name, trans);
        for (int i = 0; i < trans.childCount; i++)
        {
            Traversal(trans.GetChild(i), call);
        }
    }

    public void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_update":
                GameObject.Destroy(this.gameObject);
                if (this.mCall != null) {
                    this.mCall();
                }
                break;
        }
    }
    public void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_version_tick":
                this.mLbTick = tf.GetComponent<UILabel>();
                break;
            case "btn_update":
                UIEventListener.Get(tf.gameObject).onClick = this.OnButtonClick;
                break;
            case "lb_tip":
                mTip = tf.GetComponent<UILabel>();
                break;
        }
    }
}
