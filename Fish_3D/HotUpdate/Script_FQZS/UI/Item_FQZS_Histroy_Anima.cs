using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_Histroy_Anima : UIItem
{
    public GameObject mNow;

    public UISprite mAnima;

    public override void OnNodeAsset(string name, Transform tf)
    {
        if (name.StartsWith("animal"))
            mAnima = tf.GetComponent<UISprite>();
        else if (name.Equals("now"))
            mNow = tf.gameObject;
    }

    /// <summary>
    /// 更新动画
    /// </summary>
    /// <param name="code"></param>
    /// <param name="isLast"></param>
    public void InitData(byte code,bool isLast = false)
    {
        mAnima.spriteName = GetAnimaNameWithCode(code);
        if(!mAnima.gameObject.activeSelf)
            mAnima.gameObject.SetActive(true);
        if (isLast)
            mNow.SetActive(true);
        else
            mNow.SetActive(false);
    }

    public void ChangeSelectState(bool isSelect)
    {
        mNow.SetActive(isSelect);
    }

    /// <summary>
    /// 根据服务器返回的索引来更新动物显示
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private string GetAnimaNameWithCode(byte code)
    {
        switch (code)
        {
            case 101:
                return "shayu";
            case 104:
                return "yanzi";
            case 105:
                return "tuzi";
            case 106:
                return "gezi";
            case 107:
                return "xiongmao";
            case 108:
                return "kongque";
            case 109:
                return "houzi";
            case 110:
                return "laoying";
            case 111:
                return "shizi";
            case 100:
                return "tongchi";
            case 113:
                return "jinsha";
            case 112:
                return "tongpei";
        }

        return null;
    }
}
