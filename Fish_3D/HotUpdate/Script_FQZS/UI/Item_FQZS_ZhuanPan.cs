using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ZhuanPan : UIItem
{
    /// <summary>
    /// 转盘上的所有动物列表
    /// </summary>
    public List<Item_FQZS_ZhanPanAnima> mAnimaLst = new List<Item_FQZS_ZhanPanAnima>();

    /// <summary>
    /// 当前选中的动物
    /// </summary>
    public Item_FQZS_ZhanPanAnima mCurrSelectedAnima = null;

    /// <summary>
    /// 主UI
    /// </summary>
    public UI_FQZS_Main ui;

    /// <summary>
    /// 转到最后的动物
    /// </summary>
    public Item_FQZS_ZhanPanAnima mItemCurResult;

    public Item_FQZS_ZhanPanAnima preRotateAnima;

    /// <summary>
    /// 提示框
    /// </summary>
    public UILabel mTips;

    /// <summary>
    /// 当前游戏状态
    /// </summary>
    public FQZSEnumGameState mGameState;

    private Queue<float> mTimeQueue = new Queue<float>();

    public bool isRotate = false;

    private int curRoateIndex;

    private int countDown = 0;

    private float firstRotateTime = 0;

    private float deltaTime = 0;

    private Action<int> OpenBigShowEvent;

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="lastEndIndex"></param>
    public void InitData(bool isFlashing, Action<int> openBigShow)
    {
        for (int i = 0; i < mAnimaLst.Count; i++)
        {
            int curAinmaIndex = int.Parse(mAnimaLst[i].gameObject.name.Split('_')[1]);
            FQZSEnumOption curAnimaType = GetAnimaType(curAinmaIndex);
            mAnimaLst[i].InitData(curAnimaType, curAinmaIndex, isFlashing);
        }

        //按照索引进行排序
        mAnimaLst.Sort((x, y) => { return x.CurIndex.CompareTo(y.CurIndex); });

        OpenBigShowEvent = openBigShow;
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    public void RefreshData(bool isFlashing)
    {
        for (int i = 0; i < mAnimaLst.Count; i++)
        {
            mAnimaLst[i].RefreshData(isFlashing);
        }
    }

    /// <summary>
    /// 旋转中
    /// </summary>
    public void AnimaRotate()
    {
        countDown++;
        curRoateIndex++;

        int curTempIndex = curRoateIndex % mAnimaLst.Count;
        if (preRotateAnima != null)
        {
            preRotateAnima.SetState(false, false);
        }
        Item_FQZS_ZhanPanAnima curAnima = mAnimaLst[curTempIndex];

        if (countDown > mAnimaLst.Count * 3)
        {
            if (curTempIndex == FQZSGameManager.CurStopIndex)
            {
                //旋转结束
                isRotate = false;
                countDown = 0;
                curAnima.SetState(true, true);
                mTimeQueue.Clear();
                AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameManager.GetAnimaByCode(FQZSGameManager.SelectAni, true));

                if (OpenBigShowEvent != null)
                    OpenBigShowEvent(GetPlayBigShowIndex(FQZSGameManager.SelectAni));
            }
            else
            {
                curAnima.SetState(true, false);
            }
        }
        else
        {
            curAnima.SetState(true, false);
        }

        preRotateAnima = curAnima;
    }

    private int GetPlayBigShowIndex(byte code)
    {
        switch (code)
        {
            case 1:
            case 0:
            case 10:
                return 1;
            case 6:
            case 8:
            case 9:
            case 12:
                return 2;
            case 4:
            case 5:
            case 7:
            case 11:
                return 3;
            case 13:
                return 4;
        }

        return -1;
    }

    /// <summary>
    /// 开始旋转
    /// </summary>
    public void StartRotate()
    {
        mTimeQueue.Clear();

        curRoateIndex = FQZSGameManager.LastEndIndex;

        float startTime = 0.5f;

        int finalcircleNum = FQZSGameManager.CurStopIndex - FQZSGameManager.LastEndIndex;

        if (finalcircleNum <= 0)
        {
            finalcircleNum += 28;
        }

        int allAnimaRotateNum = mAnimaLst.Count * 3 + finalcircleNum + 1;

        //前6个是逐渐加速的
        for (int i = 0; i < 6; i++)
        {
            mTimeQueue.Enqueue(startTime);
            startTime -= 0.0833f;
        }

        int centerRoateNum = allAnimaRotateNum - 6 - 10;

        //中间的部分是匀速的
        for (int i = 0; i < centerRoateNum; i++)
        {
            mTimeQueue.Enqueue(startTime);
        }

        //最后10个逐渐减速的
        for (int i = 0; i < 10; i++)
        {
            startTime += 0.05f;
            mTimeQueue.Enqueue(startTime);
        }

        isRotate = true;

        if (preRotateAnima != null)
            preRotateAnima.SetState(false, false);

        for (int i = 0; i < mAnimaLst.Count; i++)
        {
            mAnimaLst[i].SetState(false, false);
        }

        firstRotateTime = mTimeQueue.Dequeue();

        AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_RUN);
    }

    private void Update()
    {
        if (isRotate)
        {
            firstRotateTime -= Time.deltaTime;
            if (firstRotateTime <= 0)
            {
                if (mTimeQueue.Count > 0)
                {
                    firstRotateTime = mTimeQueue.Dequeue();
                    AnimaRotate();
                }
            }   
        }
    }

    /// <summary>
    /// 根据索引获得动物类型
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private FQZSEnumOption GetAnimaType(int index)
    {
        switch(index)
        {
            case 0:
                return FQZSEnumOption.TongChi;
            case 1:
            case 2:
            case 3:
            case 4:
                return FQZSEnumOption.YanZi;
            case 5:
            case 6:
                return FQZSEnumOption.LaoYing;
            case 7:
                return FQZSEnumOption.YinSha;
            case 8:
            case 9:
                return FQZSEnumOption.ShiZi;
            case 10:
            case 11:
            case 12:
            case 13:
                return FQZSEnumOption.TuZi;
            case 14:
                return FQZSEnumOption.TongPei;
            case 15:
            case 16:
            case 17:
                return FQZSEnumOption.HouZi;
            case 18:
            case 19:
            case 20:
                return FQZSEnumOption.XiongMao;
            case 21:
                return FQZSEnumOption.JinSha;
            case 22:
            case 23:
            case 24:
                return FQZSEnumOption.KongQue;
            case 25:
            case 26:
            case 27:
                return FQZSEnumOption.GeZi;
        }

        return FQZSEnumOption.None;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        if (name.StartsWith("itemanima"))
        {
            Item_FQZS_ZhanPanAnima anima = BindItem<Item_FQZS_ZhanPanAnima>(tf.gameObject);
            mAnimaLst.Add(anima);
        }
        else if (name.Equals("lb_tips"))
        {
            mTips = tf.gameObject.GetComponent<UILabel>();
        }
    }

    /// <summary>
    /// 设置tip动画
    /// </summary>
    private void SetTipAnima()
    {
        mTips.transform.localPosition = new Vector3(-82.1f, 100, 0);
        TweenPosition tween = mTips.gameObject.AddComponent<TweenPosition>();
        tween.from = new Vector3(-82.1f, 100, 0);
        tween.to = new Vector3(-82.1f, 327.57f, 0);
        tween.duration = 0.5f;
        tween.PlayForward();
        tween.onFinished.Add(new EventDelegate(OnTipAnimaFinish));
    }

    private void OnTipAnimaFinish()
    {
        TweenPosition tween = mTips.GetComponent<TweenPosition>();
        if (tween != null)
            Destroy(tween);
    }

    /// <summary>
    /// 打开提示
    /// </summary>
    /// <param name="txt"></param>
    public void ShowTip(string txt)
    {
        mTips.gameObject.SetActive(true);
        SetTipAnima();
        mTips.text = txt;

        StartCoroutine(HideTip());
    }

    /// <summary>
    /// 延时隐藏tip
    /// </summary>
    /// <returns></returns>
    IEnumerator HideTip()
    {
        yield return new WaitForSeconds(1.5f);
        mTips.gameObject.SetActive(false);
    }
}
