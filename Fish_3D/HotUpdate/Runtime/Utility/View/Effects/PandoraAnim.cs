using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandoraAnim : MonoBehaviour {
    public GameObject mOldObj;
    public GameObject mNewObj;
    public float mStartTime = 0;
    public Vector3 mInitScale;
    public Vector3 mTargetScale;
    public Fish mFish;

    public void InitData(GameObject old_obj,Fish fish) {
        this.mOldObj = old_obj;
        this.mNewObj = fish.Model;
        this.mStartTime = 0;
        this.mFish = fish;

        float life = GameUtils.CalPSLife(FishResManager.Instance.mEffPandoraDie);
        fish.SetWait(life + mBigTime);
        this.mInitScale = this.mOldObj.transform.localScale;
        this.mTargetScale = this.mInitScale * 2.5f;
        this.mNewObj.SetActive(false);
        this.mOldObj.SetActive(true);
    }

    private float mBigTime = 0.5f;
    void Update() {
        if (this.mNewObj == null) {
            GameObject.Destroy(this.mOldObj);
            GameObject.Destroy(this);
            return;
        }
        if (this.mStartTime < mBigTime) {
            this.mStartTime += Time.deltaTime;
            if (this.mStartTime < this.mBigTime) {
                this.mOldObj.transform.localScale = Vector3.Lerp(this.mInitScale, this.mTargetScale, this.mStartTime / this.mBigTime);
            } else {
                this.mOldObj.transform.localScale = this.mTargetScale;
                TimeManager.DelayExec(this,0.1f, () => {
                    this.mOldObj.SetActive(false);
                    GameObject.Destroy(this.mOldObj);
                });
                if (this.mFish.FishCfgID != ConstValue.FootFish) {//足球不需要显示
                    TimeManager.DelayExec(this, 0.3f, () => {
                        this.mNewObj.SetActive(true);
                    });
                }
                GameObject obj = GameObject.Instantiate(FishResManager.Instance.mEffPandoraDie);
                obj.transform.localPosition = this.mOldObj.transform.position;
                obj.transform.localScale = this.mOldObj.transform.localScale * 2;
                float life = GameUtils.CalPSLife(obj);
                GameObject.Destroy(obj, life + 0.5f);
            }
        }
    }
}
