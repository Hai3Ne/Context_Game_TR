using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ResultPlayerInfo : UIItem
{
    public UILabel mPlayerName;

    public UILabel mPlayerResult;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "playername":
                mPlayerName = tf.GetComponent<UILabel>();
                break;
            case "playerreslut":
                mPlayerResult = tf.GetComponent<UILabel>();
                break;
        }
    }

    public void SetUI(string name,long result)
    {
        mPlayerName.text = name;
        if (result < 0)
            mPlayerResult.text = result.ToString();
        else
            mPlayerResult.text = result.ToString();
    }
}
