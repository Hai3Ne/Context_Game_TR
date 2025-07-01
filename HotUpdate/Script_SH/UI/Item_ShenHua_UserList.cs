using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_UserList : UIItem {
    private int ItemHeight = 30;
    public GameObject mItemUser;
    public UIScrollView mScrollView;

    public List<Item_ShenHua_UserList_Item> mItemList = new List<Item_ShenHua_UserList_Item>();

    public void InitData(List<RoleInfo> role_list) {
        Item_ShenHua_UserList_Item item;
        for (int i = 0; i < role_list.Count; i++) {
            item = this.AddItem<Item_ShenHua_UserList_Item>(this.mItemUser, this.mScrollView.transform);
            item.InitData(role_list[i]);
            this.mItemList.Add(item);
        }
        this.RefershPos();
    }
    public void RefershInfo(RoleInfo role) {//刷新用户信息
        foreach (var item in this.mItemList) {
            if (item.mRoleInfo == role) {
                item.InitData(role);
                break;
            }
        }
    }
    public void AddRole(RoleInfo role) {
        Item_ShenHua_UserList_Item item = this.AddItem<Item_ShenHua_UserList_Item>(this.mItemUser, this.mScrollView.transform);
        item.InitData(role);
        this.mItemList.Add(item);
        this.RefershPos();
    }
    public void RemoveRole(RoleInfo role) {
        for (int i = 0; i < this.mItemList.Count; i++) {
            if (this.mItemList[i].mRoleInfo == role) {
                GameObject.Destroy(this.mItemList[i].gameObject);
                this.mItemList.RemoveAt(i);
                break;
            }
        }
        this.RefershPos();
    }

    private void RefershPos() {
        for (int i = 0; i < this.mItemList.Count; i++) {
            this.mItemList[i].transform.localPosition = new Vector3(0, -ItemHeight * i);
        }
        this.mScrollView.ResetPosition();
    }

    public void Start() {
        this.mScrollView.ResetPosition();
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_user":
                this.mItemUser = tf.gameObject;
                this.mItemUser.SetActive(false);
                break;
            case "scrollview_user":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
        }
    }
}