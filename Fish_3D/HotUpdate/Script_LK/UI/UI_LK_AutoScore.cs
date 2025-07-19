using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_AutoScore : UILayer {
    public UILabel mLbMul;//炮台当前倍率
    public UILabel mLbScore;//本次上分
    public UILabel mLbGold;//剩余乐豆

    public void InitData(int mul, long score, long gold) {
        this.mLbMul.text = mul.ToString();
        this.mLbScore.text = score.ToString();
        this.mLbGold.text = gold.ToString();
    }
    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_mul":
                this.mLbMul = tf.GetComponent<UILabel>();
                break;
            case "lb_score":
                this.mLbScore = tf.GetComponent<UILabel>();
                break;
            case "lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
        }
    }
}
