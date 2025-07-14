using Kubility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Item_mainhall_new : UIItem
{
    public GameObject mItemObj;
    public UILabel mLbDownSize;

    public bool mIsShow;
    public GameEnum mGameEnum;
    public ushort mKindID;
    public void InitData(UI_mainhall_new ui, GameEnum type, ushort kindid) {
        this.mGameEnum = type;
        this.mKindID = kindid;
        this.mItemObj.SetActive(false);

        long dd = ResVersionManager.TryGetDownSize(this.mGameEnum);
        if (dd == -1)
        {
            ResVersionManager.GetDownSize(ui, this.mGameEnum,(bool need_down)=>
            {
                this.mItemObj.SetActive(need_down);
            }, 
                (size) =>{
                    if (size > 0)
                    {
                        this.mLbDownSize.text = Tools.GetDownSpdStr(size);
                    }
                });
        }
        else
        {
            if (dd > 0)
            {
                this.mItemObj.SetActive(true);
                this.mLbDownSize.text = Tools.GetDownSpdStr(dd);
            }
        }
        this.CheckShow();
    }
    public void CheckShow() {//检测显示
        this.mIsShow = HallHandle.GetServerList(this.mKindID).Count > 0;
        this.gameObject.SetActive(this.mIsShow);
    }

    public override void OnButtonClick(GameObject obj)
    {
        if (obj == this.gameObject)
        {
             GameSceneManager.BackToHall(this.mGameEnum);
        }
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "item_download":
                this.mItemObj = tf.gameObject;
                break;
            case "item_lb_size":
                this.mLbDownSize = tf.GetComponent<UILabel>();
                break;
        }
    }
}
