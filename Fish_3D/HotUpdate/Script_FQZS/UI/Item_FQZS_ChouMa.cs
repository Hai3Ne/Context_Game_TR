using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ChouMa : UIItem
{
    /// <summary>
    /// 当前筹码的数值
    /// </summary>
    private int m_CurrValue;

    /// <summary>
    /// 选择了当前筹码
    /// </summary>
    private Action<Item_FQZS_ChouMa> OnSelectChouMa;

    /// <summary>
    /// 选择后的高亮图片
    /// </summary>
    public GameObject mChoose;

    /// <summary>
    /// 提供给外部使用
    /// </summary>
    public int CurrValue
    {
        get
        {
            return m_CurrValue;
        }
    }
    /// <summary>
    /// 初始化筹码数据
    /// </summary>
    /// <param name="value"></param>
    public void InitData(int value,Action<Item_FQZS_ChouMa> onSelectCurChouMa)
    {
        m_CurrValue = value;
        OnSelectChouMa = onSelectCurChouMa;
    }

    /// <summary>
    /// 加载控件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tf"></param>
    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "choose":
                mChoose = tf.gameObject;
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        if (OnSelectChouMa != null)
            OnSelectChouMa(this);
    }

    public void SetChooseState(bool isActive)
    {
        mChoose.SetActive(isActive);
    }
}
