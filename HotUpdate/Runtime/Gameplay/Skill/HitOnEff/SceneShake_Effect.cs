using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneShake_Effect : BaseSkillEffect {
    public float mShakeVal = 15;//震动幅度
    public float mDelay = 0f;//延迟时间
    private float _time;
    private float mTotalTime = 0.1f;//持续总时间

	List<Camera> shakeCameras = new List<Camera> ();
	protected override void Start ()
	{
		base.Start ();
		shakeCameras.Clear ();
        //shakeCameras.Add(SceneObjMgr.Instance.MainCam);
		shakeCameras.Add (SceneObjMgr.Instance.BgCam);
		mDelay = mEffVo.Value0 * 0.001f;
		mTotalTime = mEffVo.Value1 * 0.001f;
        if (mEffVo.Value2 > 0) {
            this.mShakeVal = mEffVo.Value2;
        }
        this._time = 0;
		shakeCameras.ForEach(x=>x.transform.position = Vector3.zero);
	}

	void OnDestroy()
	{
		shakeCameras.ForEach(x=>x.transform.position = Vector3.zero);
	}

    private Vector3 offset_;
    public override bool Update(float delta) {
        if (delta > 100f) {
            OnDestroy();
            return false;
        }

        mDelay -= delta;
        if (mDelay > 0) {
            return true;
        }

        if (this._time >= mTotalTime) {
            OnDestroy();
            return false;
        }
        this._time += delta;

        if (GameConfig.OP_Shake) {
            float val = this.mShakeVal;//Mathf.Lerp(this.mShakeVal, 0, this._time / this.mTotalTime);
            //offset_.x = UnityEngine.Random.Range(10, val + 10) * (Random.Range(0, 100) < 50 ? -1 : 1);
            //offset_.y = UnityEngine.Random.Range(10, val + 10) * (Random.Range(0, 100) < 50 ? -1 : 1);

            //if (IsOld) {
                val = Mathf.Lerp(this.mShakeVal, 0, this._time / this.mTotalTime);
                offset_.x = UnityEngine.Random.Range(-val, val);//UnityEngine.Random.Range(0, 100) % 2 == 0 ? (1f + UnityEngine.Random.Range(0f, 2f)) : (-1f + UnityEngine.Random.Range(-2f, 0f));
                offset_.y = UnityEngine.Random.Range(-val, val);//UnityEngine.Random.Range(0, 100) % 2 == 1 ? (1f + UnityEngine.Random.Range(0f, 2f)) : (-1f + UnityEngine.Random.Range(-2f, 0f));
            //}

            //offset_.x = UnityEngine.Random.Range(-val, val);
            //offset_.y = UnityEngine.Random.Range(-val, val);
            offset_.z = Mathf.Abs(offset_.y * 5) + UnityEngine.Random.Range(0, val);
            for (int i = 0; i < shakeCameras.Count; i++) {
                shakeCameras[i].transform.position = offset_;
            }
        }
        return true;
    }

}

