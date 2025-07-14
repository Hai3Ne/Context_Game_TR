using UnityEngine;
using System.Collections;

public class UIItem : MonoBehaviour {
    public void _init_data(Transform tf) {
        Collider[] colls = tf.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colls.Length; i++) {
            UIEventListener.Get(colls[i].gameObject).onClick = this.OnButtonClick;
        }
        this._node_asset(tf);
    }
    private void _node_asset(Transform tf)
    {
        this.OnNodeAsset(tf.name, tf);
        for (int i = 0; i < tf.childCount; i++)
        {
            this._node_asset(tf.GetChild(i));
        }
        //if (tf.GetComponent<Collider>() != null) {
        //    UIEventListener.Get(tf.gameObject).onClick = this.OnButtonClick;
        //}
    }

    public static T CreateItem<T>(GameObject obj, Transform parent) where T : UIItem {
        Transform tf = (GameObject.Instantiate(obj, parent) as GameObject).transform;
        tf.gameObject.SetActive(true);
        tf.localScale = Vector3.one;
        tf.rotation = Quaternion.identity;
        tf.localPosition = Vector3.zero;
        T t = tf.gameObject.AddComponent<T>();
        t._init_data(tf);
        return t;
    }

    public T AddItem<T>(GameObject obj, Transform parent) where T : UIItem {
        Transform tf = (GameObject.Instantiate(obj, parent) as GameObject).transform;
        tf.gameObject.SetActive(true);
        tf.localScale = Vector3.one;
        tf.rotation = Quaternion.identity;
        tf.localPosition = Vector3.zero;
        return this.BindItem<T>(tf.gameObject);
    }
    public T BindItem<T>(GameObject obj) where T : UIItem {
        T t = obj.AddComponent<T>();
        t._init_data(obj.transform);
        return t;
    }

    //public virtual void OnNodeLoad() { }
    //public virtual void OnEnter() { }
    //public virtual void OnExit() { }
    public virtual void OnButtonClick(GameObject obj) { }
    public virtual void OnNodeAsset(string name, Transform tf) { }


    //public override void OnButtonClick(GameObject obj) { 
    //}
    //public override void OnNodeAsset(string name, Transform tf) { 
    //}
}
