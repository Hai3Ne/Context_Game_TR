using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_Coin : UIItem
{
    /// <summary>
    /// 动画起点
    /// </summary>
    private Vector3 mStartPos = Vector3.zero;

    /// <summary>
    /// 动画终点
    /// </summary>
    private Vector3 mEndPos = Vector3.zero;

    /// <summary>
    /// 移动速度
    /// </summary>
    private float mSpeed = 5.0f;

    Action<GameObject> mHideEvent;

    /// <summary>
    /// 初始化动画的起点和终点
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    public void InitData(Vector3 startPos,Vector3 endPos, Action<GameObject> HideEvent)
    {
        mStartPos = startPos;
        mEndPos = endPos;
        gameObject.transform.position = mStartPos;
        gameObject.SetActive(true);
        mHideEvent = HideEvent;
    }

    private void Update()
    {
        if (mStartPos == Vector3.zero || mEndPos == Vector3.zero)
            return;

        if (Vector3.Distance(transform.position, mEndPos) < 0.1f)
        {
            transform.position = new Vector3(mEndPos.x + UnityEngine.Random.Range(-0.05f, 0.05f), mEndPos.y + UnityEngine.Random.Range(-0.05f, 0.05f), 0);
            mStartPos = Vector3.zero;
            mEndPos = Vector3.zero;

            if (mHideEvent != null)
                mHideEvent(gameObject);
        }
        else
        {
            Vector3 offSet = mEndPos - transform.position;
            transform.position += offSet.normalized * mSpeed * Time.deltaTime;
        }
    }
}
