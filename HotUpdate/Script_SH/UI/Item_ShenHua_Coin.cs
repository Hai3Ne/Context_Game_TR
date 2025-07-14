using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_Coin : UIItem
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

    /// <summary>
    /// 是否销毁
    /// </summary>
    private bool IsDestroy = false;

    /// <summary>
    /// 初始化动画的起点和终点
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    public void InitData(Vector3 startPos, Vector3 endPos, bool isSelfBet,bool isDestroy = false,float speed = 5.0f)
    {
        mStartPos = startPos;
        mEndPos = endPos;

        IsDestroy = isDestroy;

        gameObject.transform.position = mStartPos;

        gameObject.SetActive(true);

        mSpeed = speed;

        if (isSelfBet)
        {
            if (SHGameManager.curMousePos != Vector3.zero)
                gameObject.transform.position = SHGameManager.curMousePos;
            else
                gameObject.transform.position = new Vector3(mEndPos.x + Random.Range(-0.06f, 0.06f), mEndPos.y + Random.Range(-0.06f, 0.06f), 0);
            mStartPos = Vector3.zero;
            mEndPos = Vector3.zero;
        }
    }

    private void Update()
    {
        if (mStartPos == Vector3.zero || mEndPos == Vector3.zero)
            return;

        if (mSpeed > 0.5)
        {
            if (Vector3.Distance(transform.position, mEndPos) < 0.3f)
            {
                transform.position = new Vector3(mEndPos.x + Random.Range(-0.06f, 0.06f), mEndPos.y + Random.Range(-0.06f, 0.06f), 0);
                mStartPos = Vector3.zero;
                mEndPos = Vector3.zero;

                if (IsDestroy)
                    Destroy(gameObject);
            }
            else
            {
                Vector3 offSet = mEndPos - transform.position;
                transform.position += offSet.normalized * mSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, mEndPos) < 0.1f)
            {
                transform.position = new Vector3(mEndPos.x + Random.Range(-0.05f, 0.05f), mEndPos.y + Random.Range(-0.06f, 0.06f), 0);
                mStartPos = Vector3.zero;
                mEndPos = Vector3.zero;

                if (IsDestroy)
                    Destroy(gameObject);
            }
            else
            {
                Vector3 offSet = mEndPos - transform.position;
                transform.position += offSet.normalized * mSpeed * Time.deltaTime;
            }
        }
    }
}
