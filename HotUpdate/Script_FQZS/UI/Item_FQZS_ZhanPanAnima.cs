using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ZhanPanAnima : UIItem
{
    /// <summary>
    /// 选中高亮图
    /// </summary>
    UISprite mChoose;

    /// <summary>
    /// 当前动物类型
    /// </summary>
    public FQZSEnumOption mOption;

    /// <summary>
    /// 当前索引
    /// </summary>
    public int CurIndex;

    private bool isFlashing = false;

    private float FlashingTime = 0;

    private bool isHideAlpha = false;

    private float hideAlphaTime = 0.2f;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="option"></param>
    /// <param name="index"></param>
    public void InitData(FQZSEnumOption option,int index,bool isFlahing)
    {
        CurIndex = index;

        mOption = option;

        if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Free)
        {
            if (CurIndex == FQZSGameManager.LastEndIndex)
                SetState(true, isFlahing);
            else
                SetState(false, false);
        }
        else if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
        {
            SetState(false, false);
        }
        else
        {
            if (CurIndex == FQZSGameManager.CurStopIndex)
                SetState(true, isFlahing);
            else
                SetState(false, false);
        }
    }

    private void Update()
    {
        if (isFlashing)
        {
            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
            {
                mChoose.gameObject.SetActive(true);
                FlashingTime = 0;
                isFlashing = false;
            }
            else
            {
                FlashingTime -= Time.deltaTime;

                if (FlashingTime <= 0)
                {
                    mChoose.gameObject.SetActive(!mChoose.gameObject.activeSelf);
                    FlashingTime = 0.5f;
                }
            }
        }

        if (isHideAlpha)
        {
            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.End)
            {
                hideAlphaTime -= Time.deltaTime;

                if (hideAlphaTime <= 0)
                {
                    mChoose.alpha = 0;
                    hideAlphaTime = 0.2f;
                    isHideAlpha = false;
                }
                else
                {
                    mChoose.alpha = hideAlphaTime / 0.2f;
                }
            }
            else
            {
                mChoose.gameObject.SetActive(false);
                isHideAlpha = false;
                hideAlphaTime = 0.2f;
            }
        }
    }

    public void RefreshData(bool isFlahing)
    {
        if (CurIndex == FQZSGameManager.CurStopIndex)
        {
            SetState(true, isFlahing);
        }
        else
            SetState(false, false);
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "choose":
                mChoose = tf.GetComponent<UISprite>();
                break;
        }
    }

    /// <summary>
    /// 是否选中
    /// </summary>
    /// <param name="isChoose"></param>
    public void SetState(bool isChoose,bool Flashing = false)
    {
        isFlashing = Flashing;

        if (isFlashing)
            FlashingTime = 0.5f;

        if (!isChoose)
        {
            isHideAlpha = true;
        }
        else
        {
            mChoose.gameObject.SetActive(isChoose);
            mChoose.alpha = 1;
        }
    }
}
