using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LKAnimSprite : MonoBehaviour {
    public class EventInfo {//动画事件
        public LKEnumFishAnim FishAnim;
        public int Frames;
        public VoidDelegate mCall;
    }
    public Transform mTf;
    public Transform mTfShadow;//影子对象
    public List<SpriteRenderer> mImgList = new List<SpriteRenderer>();
    public List<LKSpriteInfo> mAnimList = new List<LKSpriteInfo>();


    private bool is_loop = true;
    [System.NonSerialized]
    public LKSpriteInfo mCurAnim;//当前播放动画
    [System.NonSerialized]
    public int mFrames;
    private float fps;
    private float _time;
    private float anim_spd = 1;//动画速度

    [System.NonSerialized]
    public List<EventInfo> mEventList = new List<EventInfo>();
    [System.NonSerialized]
    public int mEventIndex = -1;

    public float speed {
        set {
            this.anim_spd = value;
        }
        get {
            return this.anim_spd;
        }
    }
    public float lenght {
        get {
            if (this.mCurAnim != null) {
                return this.mCurAnim.lenght;
            } else {
                return 0;
            }
        }
    }
    public float time {
        get {
            if (this.mCurAnim != null) {
                return this.mFrames * 1f / this.mCurAnim.FPS;
            } else {
                return 0;
            }
        }
    }

    public void Awake() {
        if (this.mImgList.Count == 0) {
            this.mImgList.Add(this.GetComponent<SpriteRenderer>());
        }
        if (this.mTf == null) {
            this.mTf = this.transform;
        }
        if (this.mAnimList.Count > 0) {
            this.PlayAnim(this.mAnimList[0], true);
        }
    }

    private Renderer[] mRenderers;
    public void SetDepth(int depth) {
        if (this.mRenderers == null) {
            this.mRenderers = this.GetComponentsInChildren<Renderer>();
        }
        foreach (var item in this.mRenderers) {
            item.sortingOrder = depth;
        }
    }
    private SpriteRenderer[] mSRs;
    public void SetColor(Color color) {
        if (this.mSRs == null) {//屏蔽影子的颜色
            if (this.mTfShadow != null) {
                this.mTfShadow.gameObject.SetActive(false);
            }
            this.mSRs = this.GetComponentsInChildren<SpriteRenderer>();
            if (this.mTfShadow != null) {
                this.mTfShadow.gameObject.SetActive(true);
            }
        }
        foreach (var item in this.mSRs) {
            item.color = color;
        }
    }
    public void SetAngle(float angle) {
        this.mTf.localEulerAngles = new Vector3(0, 0, angle);
        if (this.mTfShadow != null) {
            this.mTfShadow.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
    public void SetFlip(bool x_flip, bool y_flip) {
        this.mTf.localScale = new Vector3(x_flip ? -1 : 1, y_flip ? -1 : 1, 1);

        if (this.mTfShadow != null) {
            this.mTfShadow.localScale = new Vector3(x_flip ? -0.5f : 0.5f, y_flip ? -0.5f : 0.5f, 1);
        }
    }

    public bool PlayAnim(LKEnumFishAnim anim,bool is_loop,bool is_replay = true) {
        for (int i = 0; i < this.mAnimList.Count; i++) {
            if (this.mAnimList[i].FishAnim == anim) {
                this.PlayAnim(this.mAnimList[i], is_loop,is_replay);
                return true;
            }
        }
        return false;
    }
    public void PlayAnim(LKSpriteInfo info, bool is_loop, bool is_replay = true) {
        this.is_loop = is_loop;
        if (is_replay == true || this.mCurAnim != info) {
            this.mCurAnim = info;
            this.fps = 1f / this.mCurAnim.FPS;
            this.ResetToBeginning();
        }
        if (this.mCurAnim.mRotate.Count > 0) {//控制光环旋转速度
            foreach (var item in this.mCurAnim.mRotate) {
                item.RotateSpd = new Vector3(0, 0, this.mCurAnim.AngleSpd);
            }
        }
        this.enabled = true;
    }
    public void ResetToBeginning() {
        this.mFrames = -1;
        this._time = 0;
        this.SetIndex(0);
        if (this.mEventList.Count > 0) {
            this.mEventIndex = 0;
        } else {
            this.mEventIndex = -1;
        }
    }

    public void SetIndex(int index) {//设置当前帧索引
        if (this.mFrames == index) {
            return;
        }
        int pre_frame = this.mFrames;
        int count = this.mCurAnim.mSprList.Count;
        if (index >= count) {
            if (this.is_loop) {
                this.mFrames = index % count;
            } else {
                this.mFrames = count - 1;
                this.enabled = false;
            }
        } else {
            this.mFrames = index;
        }
        foreach (var item in this.mImgList) {
            item.sprite = this.mCurAnim.mSprList[this.mFrames];
        }
        //事件触发
        if (this.mEventIndex >= 0) {
            EventInfo info = this.mEventList[this.mEventIndex];
            for (int i = pre_frame + 1; i <= index; i++) {
                if (info.Frames == i % count) {
                    info.mCall();
                    this.mEventIndex = (this.mEventIndex + 1) % this.mEventList.Count;
                    info = this.mEventList[this.mEventIndex];
                }
            }
        }
    }

	void Update () {
        if (this.mCurAnim == null || this.mCurAnim.mSprList.Count == 0) {
            this.enabled = false;
            return;
        }
        
#if UNITY_EDITOR
        this.fps = 1f / this.mCurAnim.FPS;
#endif

        this._time += Time.deltaTime * anim_spd;
        if (this._time > this.fps) {
            int frame = Mathf.FloorToInt(this._time / this.fps);
            this.SetIndex(this.mFrames + frame);
            this._time -= this.fps * frame;
        }
	}
    [ContextMenu("SortSprite")]
    private void Sort() {
        foreach (var item in this.mAnimList) {
            item.Sort();
        }
    }
}
