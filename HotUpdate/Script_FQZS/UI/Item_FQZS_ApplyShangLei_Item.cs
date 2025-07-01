using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ApplyShangLei_Item : UIItem
{
    /// <summary>
    /// 名字
    /// </summary>
    public UILabel mNickName;

    /// <summary>
    /// 乐豆
    /// </summary>
    public UILabel mLeDou;

    [HideInInspector]
    public RoleInfo userInfo;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "user_name":
                mNickName = tf.GetComponent<UILabel>();
                break;
            case "user_ledou":
                mLeDou = tf.GetComponent<UILabel>();
                break;
        }
    }

    public void InitData(RoleInfo roleinfo)
    {
        string nickName = roleinfo.NickName;
        if (nickName.Length > 6)
        {
            nickName = nickName.Substring(0, 6) + "...";
        }
        mNickName.text = nickName;
        mLeDou.text = roleinfo.GoldNum.ToString();
        userInfo = roleinfo;
    }
}
