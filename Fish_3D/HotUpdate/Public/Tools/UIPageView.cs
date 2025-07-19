using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPageView : MonoBehaviour {
    /// 
    /// 每页宽度(游-客-学-院)
    /// 
    public float mPageWidth;
    /// 
    /// 翻页力度(游.客.学.院)
    /// 
    public int EffortsFlip = 50;
    /// 
    /// 总页数
    /// 
    public int mPageCount = 0;

    /// 
    /// 当前所在页
    /// 
    public int PageIndex {
        get {
            return mPageIndex;
        }
    }
    /// 
    /// 当前所在页
    /// 
    private int mPageIndex = 1;
    private UIScrollView mScrollView = null;
    private float nowLocation = 0;
    private bool isDrag = false;
    private bool isSpringMove = false;
    private SpringPanel mSp = null;
    private bool isHorizontal = true;

    public VoidCall<int> OnPageChange;

    public void InitData(UIScrollView scrollview, int width, int page,VoidCall<int> page_call) {
        mScrollView = scrollview;
        this.mPageWidth = width;
        this.mPageCount = page;
        this.OnPageChange = page_call;
        mScrollView.onDragStarted = OnDragStarted;
        mScrollView.onMomentumMove = onMomentumMove;
        mScrollView.onStoppedMoving = onStoppedMoving;
        if (mScrollView.movement == UIScrollView.Movement.Horizontal) {
            isHorizontal = true;
        } else {
            isHorizontal = false;
        }
        onStoppedMoving();
    }

    //void Awake() {
    //    mScrollView = gameObject.GetComponent<UIScrollView>();
    //    if (mScrollView == null) {
    //        mScrollView = gameObject.AddComponent<UIScrollView>();
    //    }
    //    mScrollView.onDragStarted = OnDragStarted;
    //    mScrollView.onMomentumMove = onMomentumMove;
    //    mScrollView.onStoppedMoving = onStoppedMoving;
    //    if (mScrollView.movement == UIScrollView.Movement.Horizontal) {
    //        isHorizontal = true;
    //    } else {
    //        isHorizontal = false;
    //    }
    //    onStoppedMoving();
    //}
    void OnDragStarted() {
        isDrag = false;
        SetNowLocation();
    }
    void onMomentumMove() {
        if (isDrag) return;
        Vector3 v3 = transform.localPosition;
        float value = 0;
        if (isHorizontal) {
            value = nowLocation - v3.x;
            if (Mathf.Abs(value) < EffortsFlip) {
                Page(0);
            }else if (value > 0) {
                if (mPageIndex < mPageCount) {
                    Page(-mPageWidth);
                } else {
                    Page(0);
                }
            } else {
                if (mPageIndex > 1) {
                    Page(mPageWidth);
                } else {
                    Page(0);
                }
            }
        } else {
            value = nowLocation - v3.y;
            if (Mathf.Abs(value) < EffortsFlip) {
                Page(0);
            }else if (value > 0) {
                if (mPageIndex > 1) {
                    Page(-mPageWidth);
                } else {
                    Page(0);
                }
            } else {
                if (mPageIndex < mPageCount) {
                    Page(mPageWidth);
                } else {
                    Page(0);
                }
            }
        }
    }
    void Page(float value) {
        isSpringMove = true;
        isDrag = true;
        mSp = GetComponent<SpringPanel>();
        if (mSp == null) mSp = gameObject.AddComponent<SpringPanel>();
        //mSp.enabled = false;
        Vector3 pos = mSp.target;
        pos = isHorizontal ? new Vector3(pos.x + value, pos.y, pos.z) : new Vector3(pos.x, pos.y + value, pos.z);
        if (!SetIndexPage(pos)) return;
        SpringPanel.Begin(gameObject, pos, 13f).strength = 8f;
        mSp.onFinished = SpringPanleMoveEnd;

        if (this.OnPageChange != null) {
            this.OnPageChange(this.mPageIndex);
        }
    }
    void SpringPanleMoveEnd() {
        isSpringMove = false;
    }
    void onStoppedMoving() {
        isDrag = false;
        SetNowLocation();
    }
    void SetNowLocation() {
        if (isHorizontal) {
            nowLocation = gameObject.transform.localPosition.x;
        } else {
            nowLocation = gameObject.transform.localPosition.y;
        }
    }
    bool SetIndexPage(Vector3 v3) {
        float value = isHorizontal ? v3.x : v3.y;
        //Debug.Log((pageNums - 1) * pageWidth);
        //if (isHorizontal) {
        //    if (value > 0 || value < (mPageCount - 1) * -mPageWidth) return false;
        //} else {
        //    if (value < 0 || value > (mPageCount - 1) * mPageWidth) return false;
        //}
        mPageIndex = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(value) / mPageWidth) + 1, 1, mPageCount);
        return true;
    }
    public void SetPage(int page) {//设置第几页
        this.Page((this.mPageIndex - page) * mPageWidth);
    }
    public void PrePage() {//上一页
        if (isHorizontal) {
            if (mPageIndex > 1) Page(mPageWidth);
        } else {
            if (mPageIndex < mPageCount) Page(mPageWidth);
        }
    }
    public void NextPage() {//下一页
        if (isHorizontal) {
            if (mPageIndex < mPageCount) Page(-mPageWidth);
        } else {
            if (mPageIndex > 1) Page(-mPageWidth);
        }
    }
}
