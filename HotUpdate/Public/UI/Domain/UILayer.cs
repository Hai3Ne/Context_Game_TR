using UnityEngine;
using System.Collections;

public class UILayer : UIItem {
    public int depth;//当前层级

    public void TweenShow() {
        Transform t0 = this.transform.Find("bg");
        Transform t1 = this.transform.Find("scale");
        float dura = 0.3f;
        Debug.Log("11111111111111");
        if (t0 != null) {
            Debug.Log("111111111111112");
            t0.GetComponent<UIWidget>().alpha = 0.1f;
            Debug.Log("111111111111113");
            TweenAlpha.Begin(t0.gameObject, dura, 1f);
            Debug.Log("111111111111114");
        }
        Debug.Log("111111111111115");
        if (t1 != null) {
            Debug.Log("111111111111116");
            t1.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Debug.Log("111111111111117");
            TweenScale ts = TweenScale.Begin(t1.gameObject, dura, Vector3.one);
            Debug.Log("111111111111118");
            ts.animationCurve = GameParams.Instance.uiPanelshowCure;
            Debug.Log("111111111111119");
            //ts.SetOnFinished(this.OnTweenFinish);
        }
    }

    public void Start() {
        this.OnEnter();
    }

    public void Close() {
        UI.ExitUI(this);
    }

    public virtual void OnNodeLoad() { }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    //public virtual void OnButtonClick(GameObject obj) { }
    //public virtual void OnNodeAsset(string name, Transform tf) { }


    //public override void OnNodeLoad() { }
    //public override void OnEnter() { }
    //public override void OnExit() { }
    //public override void OnButtonClick(GameObject obj) { }
    //public override void OnNodeAsset(string name, Transform tf) { }
}
