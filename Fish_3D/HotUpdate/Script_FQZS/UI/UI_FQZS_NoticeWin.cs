using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FQZS_NoticeWin : UILayer
{
    private Action mDoFunc;

    private UILabel mNotice;

    public void InitData(Action DoFunc,string context)
    {
        mDoFunc = DoFunc;
        mNotice.text = context;
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_ok":
                Close();
                if (mDoFunc != null)
                    mDoFunc();
                break;
            case "btn_cancel":
                Close();
                break;
        }
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "lb_msg":
                mNotice = tf.GetComponent<UILabel>();
                break;
        }
    }
}
