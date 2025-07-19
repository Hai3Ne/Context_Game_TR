using UnityEngine;
using System.Collections;

/// <summary>
/// 弹跳动画
/// </summary>
public class AnimBounce : MonoBehaviour {
    public Transform tf;
    public float once_time = 0.3f;//第一次弹起时间
    public float height = 200;//弹起最大高度
    public float time_speed = 1;

    private float aspd;//加速度
    private int count;//弹起次数
    public float _time;
	Vector3 initLocPos = Vector3.zero;

    public void StartPlay(float height, float once_time, int count) {
        this.tf = this.transform;
        this.count = count;
        this.once_time = once_time;
        this.height = height;
        this.aspd = height * 2 / (once_time * once_time);
        this._time = 0;
        this.enabled = true;
        this.initLocPos = this.tf.localPosition;
    }
	
	void Update () {
        this._time += Time.deltaTime;
        if (this._time > this.once_time * 2) {
            float _t = this._time - this.once_time * 2;
            this.tf.localPosition = initLocPos;
            if(this.count > 1){
                this.StartPlay(this.height * 0.5f, this.once_time * 0.8f, this.count - 1);
            }else{
                this.enabled = false;
				OnFinish.TryCall ();
            }
            this._time = _t;
        }else{
            float t = this.once_time - this._time;
			this.tf.localPosition = initLocPos + new Vector3(0, this.height - this.aspd * t * t / 2);
        }
	}

	System.Action OnFinish = null;
	public static AnimBounce Begin(GameObject go, System.Action onFinish, float height, float oneceTime, int count){
		var ab = go.GetComponent<AnimBounce> ();
		if (ab == null)
			ab = go.AddComponent<AnimBounce> ();
		ab.StartPlay (height, oneceTime, count);
		ab.OnFinish = onFinish;
		return ab;
	}
}
