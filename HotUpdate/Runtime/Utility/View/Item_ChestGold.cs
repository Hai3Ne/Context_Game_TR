using UnityEngine;
using System.Collections;

public class Item_ChestGold : MonoBehaviour {
    public UITexture mTexturePlayer;
    public UILabel mLbName;
    public UILabel mLbGold;

    private Transform mTran;
    private Fish mFish;
    private Vector3 mTarget;

    public void InitData(tagUserWBData data) {
        this.mTexturePlayer.gameObject.SetActive(true);
        this.mTexturePlayer.mainTexture = FishResManager.Instance.playerIconTexAltas;
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(data.FaceID);

        string nick_name = GameUtils.SubStringByWidth(this.mLbName, data.NickName, 100);
        this.mLbName.text = string.Format("[00fffc]{0}桌[-] [00ff2a]{1}", data.TableID + 1, nick_name);
        this.mLbGold.text = data.Score.ToString();
    }
    public void RePlay(Fish boss) {//重置初始点/重新开始播放
        this.mFish = boss;
        this.mTran = this.transform;

        //float view_min = -0.2f;
        //float view_max = 1.2f;
        //switch(Random.Range(0, 100)/25){
        //    case 0:
        //        this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(view_min, Random.Range(view_min, view_max)));
        //        break;
        //    case 1:
        //        this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(view_max, Random.Range(view_min, view_max)));
        //        break;
        //    case 2:
        //        this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(view_min, view_max), view_min));
        //        break;
        //    case 3:
        //        this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(view_min, view_max), view_min));
        //        break;
        //}

        switch (Random.Range(0, 800) % 8) {
            case 0://左上
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.3f), Random.Range(0.7f, 0.9f)));
                break;
            case 1://上
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.7f, 0.9f)));
                break;
            case 2://右上
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.7f, 0.9f), Random.Range(0.7f, 0.9f)));
                break;
            case 3://右
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.7f, 0.9f), Random.Range(0.3f, 0.7f)));
                break;
            case 4://右下
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.7f, 0.9f), Random.Range(0.1f, 0.3f)));
                break;
            case 5://下
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.1f, 0.3f)));
                break;
            case 6://左下
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f)));
                break;
            case 7://左
                this.mTran.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(Random.Range(0.1f, 0.3f), Random.Range(0.3f, 0.7f)));
                break;
        }

        this.mTran.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        this.GetComponent<UIPanel>().Refresh();
        this.SetStep(0);

    }

    private float _time;
    private int step;//进度  0:放大1.5  1:缩小1  2:停留  3:缩小0
    private const float BigTime = 0.3f;//放大时间
    private const float SmallTime = 0.1f;//缩小时间
    private const float WaitTime = 1.5f;//等待时间
    private const float HideTime = 1.0f;//消失时间

    private void SetStep(int step) {
        this.step = step;
        this._time = 0;
    }
    public void Update() {
        this._time += Time.deltaTime;
        switch (this.step) {
            case 0://放大到1.5
                this.mTran.localScale = Vector3.one * Mathf.Lerp(0, 1.5f, this._time / BigTime);
                if (this._time >= BigTime) {
                    this.SetStep(1);
                }
                break;
            case 1://缩小到1
                this.mTran.localScale = Vector3.one * Mathf.Lerp(1.5f, 1f, this._time / SmallTime);
                if (this._time >= SmallTime) {
                    this.SetStep(2);
                }
                break;
            case 2://停留
                if (this._time >= WaitTime) {
                    this.SetStep(3);
                }
                break;
            case 3://缩小0 消失
                if(mFish != null && mFish.Transform != null){
                    Vector3 src = Utility.MainCam.WorldToScreenPoint(mFish.Transform.position);
                    this.mTarget = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(src);
                    this.mTarget.z = 0;
                }
                if (this._time < HideTime) {
                    this.mTran.localScale = Vector3.one * Mathf.Lerp(1.0f, 0f, this._time / HideTime);
                    this.mTran.position = Vector3.Lerp(this.mTran.position, this.mTarget, Time.deltaTime / (HideTime - this._time));
                } else {
                    this.gameObject.SetActive(false);
                }
                break;
        }


        //this._time += Time.deltaTime;
        //if(mFish != null && mFish.Transform != null){
        //    Vector3 src = Utility.MainCam.WorldToScreenPoint(mFish.Transform.position);
        //    this.mTarget = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(src);
        //    this.mTarget.z = 0;
        //}
        //if(this._time < this.mTotalTime){
        //    this.mTran.position = Vector3.Lerp(this.mTran.position, this.mTarget, Time.deltaTime / (this.mTotalTime - this._time));
        //    float pro = 0.75f;
        //    if (this._time > this.mTotalTime * pro) {//位移一半的时候才进行缩放处理
        //        this.mTran.localScale = Vector3.one * Mathf.Lerp(1, 0, Mathf.Pow((this._time / this.mTotalTime - pro) / (1 - pro), 2));
        //    }
        //}else{
        //    this.mTran.position = this.mTarget;
        //    this.gameObject.SetActive(false);
        //}
    }


}
