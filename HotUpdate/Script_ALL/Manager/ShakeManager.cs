using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeManager : MonoBehaviour {
    private static ShakeManager mono;
    public static void StartShake(float time) {//开始震动时间
        if (mono == null) {
            mono = SceneObjMgr.Instance.MainCam.gameObject.AddComponent<ShakeManager>();
        }
        mono._StartShake(time);
    }
    public float mShakeVal = 15;//震动幅度
    private List<Camera> shakeCameras = new List<Camera>();
    private float _time;
    private float mTotalTime = 0.1f;//持续总时间

    private void _StartShake(float time) {
        if (shakeCameras.Count == 0) {
            shakeCameras.Add(SceneObjMgr.Instance.MainCam);
            //shakeCameras.Add(SceneObjMgr.Instance.BgCam);
        }
        this._time = Mathf.Max(this._time, time);
        this.mTotalTime = this._time;
        this.enabled = true;
    }


    private Vector3 offset_;
    void Update() {
        this._time -= Time.deltaTime;
        if (this._time > 0) {
            if (GameConfig.OP_Shake) {
                float val = this.mShakeVal;
                //if (IsOld) {
                //val = Mathf.Lerp(0, this.mShakeVal, this._time / this.mTotalTime);
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
        } else {
            foreach (var item in shakeCameras) {
                item.transform.position = Vector3.zero;
            }
            this.enabled = false;
        }

    }

}
