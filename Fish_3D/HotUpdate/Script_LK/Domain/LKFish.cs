using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKFish : MonoBehaviour {
    public int mFishID;
    public LK_FishVo vo;
    public int mMul;//倍率
    public Vector2 mBounds;//碰撞区域
    public float mWidthHalf;
    public float mHeightHalf;
    public int mHitRadius;//碰撞半径检测
    public uint mTickCount;//出鱼时间戳
    public bool mIsValid = true;//鱼是否有效
    public float mMoveSpd = 1;//移动速度加成

    public float mDelayTime;//死亡时间
    public bool mIsMaskFish;//是否是蒙面鱼王

    public LKAnimSprite mSprAnim;
    public LKFishPath mFishPath = new LKFishPath();
    public Transform mTf;
    public Item_LK_Battle_Mul mItemMul;//倍率显示

    private Vector3 _position;
    public Vector3 Position {//鱼的坐标
        get {
            return this._position;
        }
        set {
            if (this._position != value) {
                this._position = value;
                this.mTf.localPosition = new Vector3(this._position.x, this._position.y * Resolution.ViewAdaptAspect,this._position.z);//根据分辨率自适应
                this._is_view = this.CheckIsView();

                _is_refersh = true;
            }
        }
    }
    private Vector2[] _rect_pos = new Vector2[4];
    private bool _is_refersh = false;
    public Vector2[] RectPos {//四角坐标
        get {
            if (_is_refersh) {
                _is_refersh = false;
                _rect_pos[0] = this.mSprAnim.mTf.TransformPoint(new Vector2(-this.mWidthHalf, -this.mHeightHalf));
                _rect_pos[1] = this.mSprAnim.mTf.TransformPoint(new Vector2(-this.mWidthHalf, this.mHeightHalf));
                _rect_pos[2] = this.mSprAnim.mTf.TransformPoint(new Vector2(this.mWidthHalf, this.mHeightHalf));
                _rect_pos[3] = this.mSprAnim.mTf.TransformPoint(new Vector2(this.mWidthHalf, -this.mHeightHalf));
            }
            return _rect_pos;
        }
    }
    public Vector2 ScreenPos {//屏幕坐标
        get {
            return SceneObjMgr.Instance.MainCam.WorldToScreenPoint(this.Position);
        }
    }
    private bool _is_view;
    public bool IsView {//当前鱼是否在场景中
        get {
            return this._is_view;
        }
    }

    public void InitData(int fish_id, LK_FishVo vo, float spd, LKFishPath.PathType path_type, LKPathInfo path_info, Vector3 offset, float time,uint tick_count) {
        this.mFishID = fish_id;
        this.vo = vo;
        this.mTickCount = tick_count;
        this.mMul = LKGameManager.mFishMuls[vo.ID];
        this.mBounds = LKGameManager.mFishBounds[vo.ID];
        this.mWidthHalf = this.mBounds.x * 0.5f;
        this.mHeightHalf = this.mBounds.y * 0.5f;
        this.mHitRadius = LKGameManager.mFishHitRadius[vo.ID];
        this.mTf = this.transform;
#if UNITY_EDITOR
        this.mTf.name = string.Format("fish_{0}",fish_id);
#endif

        this.mSprAnim = this.GetComponent<LKAnimSprite>();
        //this.mSprAnim.SetDepth(this.vo.Depth);
        this.mFishPath.InitData(this, spd, path_type, path_info, offset);
        this.mSprAnim.PlayAnim(LKEnumFishAnim.Idle, true);

        this.UpdateFish(time);

        if (vo.ID == LKGameConfig.Fish_LiKui) {//李逵
            this.mSprAnim.mEventList.Clear();
            for (int i = 0; i < 7; i++) {
                this.mSprAnim.mEventList.Add(new LKAnimSprite.EventInfo {
                    FishAnim = LKEnumFishAnim.Dead,
                    Frames = 5 + i * 3,
                    mCall = this.Catch_LK_Spin,
                });
            }
        }else if (vo.ID == LKGameConfig.Fish_WuSong) {//武松
            this.mSprAnim.mEventList.Clear();
            for (int i = 0; i < 7; i++) {
                this.mSprAnim.mEventList.Add(new LKAnimSprite.EventInfo {
                    FishAnim = LKEnumFishAnim.Dead,
                    Frames = 5 + i * 3,
                    mCall = this.Catch_WS_Spin,
                });
            }
        }
    }
    private void Catch_LK_Spin() {//李逵捕获音效
        AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.lk_spin);
    }
    private void Catch_WS_Spin() {//武松捕获音效
        AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.ws_spin);
    }
    private int _hit_count = 0;
    public void Hit() {
        if (this.vo.RandomAudio.Length > 0) {
            if (_hit_count++ % 30 == 0) {
                AudioManager.PlayAudio(GameEnum.Fish_LK, this.vo.RandomAudio[Random.Range(0, this.vo.RandomAudio.Length)]);
            }
        }
    }
//#if UNITY_EDITOR
//    void OnDrawGizmos() {
//        Gizmos.color = Color.red;
//        Vector2[] pos = RectPos;
//        Gizmos.DrawLine(pos[0], pos[1]);
//        Gizmos.DrawLine(pos[1], pos[2]);
//        Gizmos.DrawLine(pos[2], pos[3]);
//        Gizmos.DrawLine(pos[3], pos[0]);
//    }
//#endif

    public void SetMul(int mul) {//设置鱼的倍率
        this.mMul = mul;

        if (this.mItemMul != null) {
            this.mItemMul.SetMul(this.mMul);
        }else if(vo.ID == LKGameConfig.Fish_LiKui || vo.ID == LKGameConfig.Fish_WuSong) {//  李逵,武松倍率显示
            this.mItemMul = UI_LK_Battle.ui.BindFishMul(this);
        }
    }
    public bool IsCollision(LKFish fish, Vector2 size) {
        if (fish.mIsValid == false) {
            return false;
        }
        if (fish.IsView == false) {
            return false;
        }
        if (size == Vector2.zero) {//全屏逻辑
            return true;
        }
        float x = size.x * 0.5f;
        float y = size.y * 0.5f;
        Vector2 pos = fish.Position;
        Rect rect = new Rect(pos + new Vector2(-x, -y), size);

        for (int i = 0; i < RectPos.Length; i++) {
            if (rect.Contains(RectPos[i])) {
                return true;
            }
        }

        Vector2 p_1 = this.mSprAnim.mTf.InverseTransformPoint(pos + new Vector2(-x, -y));
        Vector2 p_2 = this.mSprAnim.mTf.InverseTransformPoint(pos + new Vector2(-x, y));
        Vector2 p_3 = this.mSprAnim.mTf.InverseTransformPoint(pos + new Vector2(x, -y));
        Vector2 p_4 = this.mSprAnim.mTf.InverseTransformPoint(pos + new Vector2(x, y));

        rect = new Rect(-this.mWidthHalf, -this.mHeightHalf, this.mBounds.x, this.mBounds.y);
        if (rect.Contains(p_1) || rect.Contains(p_2) || rect.Contains(p_3) || rect.Contains(p_4)) {
            return true;
        }

        return false;
    }
    public bool IsCollision(Vector2 pos, float range) {//是否被打中
        if (this.IsView == false || this.mIsValid == false) {
            return false;
        }
        if (range == float.MaxValue) {//全屏逻辑
            return true;
        }
        float dis = Vector2.Distance(pos, this.Position);
        if (dis <= this.mHitRadius + range) {
            return true;
        }

        pos = this.mSprAnim.mTf.InverseTransformPoint(pos);
        if (Mathf.Abs(pos.x) - range > this.mWidthHalf) {
            return false;
        }
        if (Mathf.Abs(pos.y) - range > this.mHeightHalf) {
            return false;
        }
        return true;
    }
    public bool CheckIsView() {//检查是否在屏幕中
        Vector2 pos = this.Position;
        if (pos.x - this.mHitRadius > LKGameManager.mMaxPos.x) {
            return false;
        }
        if (pos.x + this.mHitRadius < LKGameManager.mMinPos.x) {
            return false;
        }
        if (pos.y - this.mHitRadius > LKGameManager.mMaxPos.y) {
            return false;
        }
        if (pos.y + this.mHitRadius < LKGameManager.mMinPos.y) {
            return false;
        }
        return true;
    }
    public void SetDepth(int depth) {
        this.mSprAnim.SetDepth(depth);
    }
    public void SetFlip(bool is_flip) {
        if (this.vo.IsLockAngle == 2) {
            this.mSprAnim.SetFlip(false, false);
        } else if (this.vo.IsLockAngle == 1) {
            this.mSprAnim.SetFlip(is_flip, false);
        } else {
            this.mSprAnim.SetFlip(false, is_flip);
        }
    }

    public void SetAngle(float angle) {//设置角度
        this.mSprAnim.SetAngle(angle);
    }

    private float _color_time = 0;
    public void SetColor(Color col, float time) {//变色
        this._color_time = time;
        this.mSprAnim.SetColor(col);
    }

    public void Update() {
        if (this._color_time > 0) {
            if (this._color_time > Time.deltaTime) {
                this._color_time -= Time.deltaTime;
            } else {
                this._color_time = 0;
                this.mSprAnim.SetColor(Color.white);
            }
        }
    }

    public bool UpdateFish(float detal) {
        if (this.mTf == null) {
            return true;
        }
        return this.mFishPath.Update(detal * this.mMoveSpd);
    }
    public void Death() {//死亡
        if (this.mTf != null) {
            if (this.mSprAnim.PlayAnim(LKEnumFishAnim.Dead, false, false)) {
                this.Destroy(this.mSprAnim.lenght - this.mSprAnim.time);
            } else {
                this.Destroy();
            }
        }
    }
    public void Destroy(float time = 0) {
        if (this.mTf != null) {
            if (time > 0) {
                GameObject.Destroy(this.mTf.gameObject, time);
            } else {
                GameObject.Destroy(this.mTf.gameObject);
            }
            this.mTf = null;
        }
    }
}

