using UnityEngine;
using System.Collections;

public class AnimScale : MonoBehaviour {
    public Vector3 mTarget = new Vector3(0,20,0);
    public float mCD = 1.8f;

    private float _time = 0;
	void Update () {
        _time += Time.deltaTime;
        float pro = _time / this.mCD;
        while (pro > 2) {
            pro -= 2;
        }
        if (pro > 1) {
            this.transform.localPosition = Vector3.Lerp(this.mTarget, Vector3.zero, pro - 1);
        } else {
            this.transform.localPosition = Vector3.Lerp(Vector3.zero, this.mTarget, pro);
        }
	}
}
