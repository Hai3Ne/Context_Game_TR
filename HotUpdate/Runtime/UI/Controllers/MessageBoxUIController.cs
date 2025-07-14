using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBoxUIController : IUIControllerImp
{
	public MessageBoxUIController ()
    {
        mPanelType = EnumPanelType.FloatUI;
	}
    Queue<Item_MessageBox> Item_MessageBoxList = new Queue<Item_MessageBox>();

	public override bool CanShow { get{ return false;}}

	public void PushMsgMove(string msg)
    {
        Item_MessageBox itemMsg = SetMsg(msg);
        if (itemMsg == null)
            return;
        itemMsg.gameObject.GetComponentInChildren<UISprite>().transform.localScale = Vector3.one;
   
        itemMsg.InitData(msg, Vector3.zero, 0.65f, OnHideMessage, true);
    }

    private Item_MessageBox SetMsg(string msg)
    {
        if (string.IsNullOrEmpty(msg))
            return null;

        Item_MessageBox item = null;

        if (Item_MessageBoxList.Count > 0)
        {
            item = Item_MessageBoxList.Dequeue();
        }
        else
        {
            //WndManager.LoadUIGameObject("MessageBoxUI", delegate (GameObject uiRefGo)
            //{
            //    GameObject CreateItem = GameUtils.CreateGo(uiRefGo, SceneObjMgr.Instance.UIContainerTransform);
            //    item = CreateItem.GetComponent<Item_MessageBox>();
            //});

            uiRefGo = Resources.Load<GameObject>("MessageBoxUI");
            uiRefGo.AddComponent<Item_MessageBox>();
            GameObject CreateItem = GameUtils.CreateGo(uiRefGo, SceneObjMgr.Instance.UIContainerTransform);
            item = CreateItem.GetComponent<Item_MessageBox>();
        }

        if (item != null)
            return item;
        else
            return null;
    }

    public void PushMsgStatic(string msg, Vector3 startPos, float showTime)
    {
        Item_MessageBox itemMsg = SetMsg(msg);
        if (itemMsg == null)
            return;
        itemMsg.gameObject.GetComponentInChildren<UISprite>().transform.localScale = new Vector3(0.7f, 1, 1);
 
        itemMsg.InitData(msg, startPos, showTime, OnHideMessage, false);
    }

    private void OnHideMessage(Item_MessageBox item)
    {
        Item_MessageBoxList.Enqueue(item);
    }

    public void Dispose()
    {
        if (Item_MessageBoxList.Count > 0)
        {
            while (Item_MessageBoxList.Count > 0)
            {
                Item_MessageBox go = Item_MessageBoxList.Dequeue();
                GameObject.Destroy(go.gameObject);
            }
            Item_MessageBoxList.Clear();
        }
    }
}
