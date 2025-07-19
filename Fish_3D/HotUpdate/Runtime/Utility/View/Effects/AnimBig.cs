using UnityEngine;
using System.Collections;

public class AnimBig : MonoBehaviour {
    public static AnimBig Begin(UIWidget ui, float big_time, float reset_time, float scale, Color start_color, Color big_color) {//开始变大
        AnimBig anim = ui.gameObject.GetComponent<AnimBig>();
        if (anim == null) {
            anim = ui.gameObject.AddComponent<AnimBig>();
        }
        anim.StartPlay(ui, big_time, reset_time, scale, start_color, big_color);
        return anim;
    }
    public static AnimBig Begin(UIWidget ui, float big_time, float reset_time, float scale) {//开始变大
        AnimBig anim = ui.gameObject.GetComponent<AnimBig>();
        if (anim == null) {
            anim = ui.gameObject.AddComponent<AnimBig>();
        }
        anim.StartPlay(big_time, reset_time, scale);
        return anim;
    }

    private Transform mTf;
    private UIWidget mUIWidget;
    public Color mStartColor;
    public Color mInitColor;
    public Color mBigColor;
    private float mBigTime;//变大时间
    private float mResetTime;//还原大小时间
    private Vector3 mStartScale;//动画开始缩放大小
    private Vector3 mBigScale;//最大缩放值
    private bool mIsBig;//当前是否在变大过程
    public void StartPlay(UIWidget ui, float big_time, float reset_time, float scale, Color start_color, Color big_color) {
        if (this.transform.localScale.x > scale) {
            return;
        } else if (this.mIsBig && this.mBigScale.x > scale) {
            return;
        }

        this.StartPlay(big_time, reset_time, scale);

        this.mUIWidget = ui;
        this.mStartColor = this.mUIWidget.color;
        this.mInitColor = start_color;
        this.mBigColor = big_color;
    }
    public void StartPlay(float big_time, float reset_time, float scale) {
        if (this.transform.localScale.x > scale) {
            return;
        } else if (this.mIsBig && this.mBigScale.x > scale) {
            return;
        }

        this.mTf = this.transform;
        this.mResetTime = reset_time;
        this.mStartScale = this.mTf.localScale;
        this.mBigTime = Mathf.Lerp(0, big_time, this.mStartScale.x / scale);
        this.mBigScale = new Vector3(scale, scale, scale);
        this.enabled = true;
        this._time = 0;
        this.mIsBig = true;
    }

    private float _time;
    private float _t;
    public void Update() {
        this._time += Time.deltaTime;
        if (this.mIsBig) {
            if (this._time >= this.mBigTime) {
                this.mTf.localScale = this.mBigScale;
                if (this.mUIWidget != null) {
                    this.mUIWidget.color = this.mBigColor;
                }
                this.mIsBig = false;
                this._time = 0;
            } else {
                _t = this._time / this.mBigTime;
                this.mTf.localScale = Vector3.Lerp(Vector3.one, this.mBigScale, _t);
                if (this.mUIWidget != null) {
                    this.mUIWidget.color = Color.Lerp(this.mStartColor, this.mBigColor, _t * _t);
                }
            }
        } else {
            if (this._time >= this.mResetTime) {
                this.mTf.localScale = Vector3.one;
                if (this.mUIWidget != null) {
                    this.mUIWidget.color = this.mInitColor;
                }
                this.enabled = false;
            } else {
                _t = this._time / this.mResetTime;
                this.mTf.localScale = Vector3.Lerp(this.mBigScale, Vector3.one, _t);
                if (this.mUIWidget != null) {
                    this.mUIWidget.color = Color.Lerp(this.mBigColor, this.mInitColor, _t * _t);
                }
            }
        }
    }


}
