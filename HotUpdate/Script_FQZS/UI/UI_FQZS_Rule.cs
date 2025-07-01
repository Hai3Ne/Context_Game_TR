using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FQZS_Rule : UILayer
{
    public UILabel mRule;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "lb_rule":
                mRule = tf.GetComponent<UILabel>();
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_ok":
                Close();
                break;
        }
    }

    public void InitData(string context)
    {
        mRule.text = context;
    }
}
