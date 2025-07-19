using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MessageBox : MonoBehaviour
{
    bool isActive = false;
    float timePercent = 0f;
    float ShowTime;
    Vector3 StartPos;
    Action<Item_MessageBox> mOnHide;
    Vector3 TargetPos;
    bool mIsMove = false;
    TweenPosition mTween;

    public void InitData(string msg, Vector3 startPos, float showTime,Action<Item_MessageBox> onHide,bool IsMove)
    {
        isActive = true;
        gameObject.SetActive(true);
        transform.position = startPos;
        ShowTime = showTime;
        mOnHide = onHide;
        GetComponentInChildren<UILabel>().text = msg;
        TargetPos = Vector3.up * 100f;
        mIsMove = IsMove;
        mTween = GetComponent<TweenPosition>();
        mTween.enabled = IsMove;
        if (IsMove)
        {
            mTween.from = startPos;
            mTween.to = TargetPos;
            mTween.duration = 0.1f;
            mTween.PlayForward();
        }
    }

    private void Update()
    {
        if (!isActive)
            return;
        timePercent += Time.deltaTime;
        if (timePercent >= ShowTime)
        {
            isActive = false;
            timePercent = 0;
            mTween.ResetToBeginning();
            gameObject.SetActive(false);
            if (mOnHide != null)
                mOnHide(this);
        }
    }
}
