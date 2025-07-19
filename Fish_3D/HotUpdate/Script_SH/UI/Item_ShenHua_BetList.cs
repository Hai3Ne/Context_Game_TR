using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_BetList : UIItem {
    public GameObject mItemBet;

    public List<Item_ShenHua_BetList_Item> mItemList = new List<Item_ShenHua_BetList_Item>();
    public bool mIsEnable;
    public void InitData(List<long> bet_list) {
        Item_ShenHua_BetList_Item item;
        for (int i = 0; i < bet_list.Count; i++) {
            item = this.AddItem<Item_ShenHua_BetList_Item>(this.mItemBet, this.transform);
            item.InitData(this, bet_list[i]);
            this.mItemList.Add(item);
        }
        this.GetComponent<UIGrid>().Reposition();
    }

    public void SetEnable(bool is_enable) {//设置投注是否可用
        this.mIsEnable = is_enable;
        foreach (var item in this.mItemList) {
            item.SetEnable(is_enable);
        }

        if (is_enable == false) {
            if (this.mSelectItem != null) {
                this.SelectItem(null);
            }
        } else {
            this.RefershEnable();
        }
    }
    public void RefershEnable() {//刷新可用状态
        if (this.mIsEnable == false) {
            return;
        }
        long max_bet = SHGameManager.GetCurBetMax();
        RoleInfo leizhu = RoleManager.FindRoleByTable(RoleManager.Self.TableID, SHGameManager.LeiZhuSeat);
        foreach (var item in this.mItemList) {
            if ((leizhu != null || SHGameManager.IsSysLeiZhu) && SHGameManager.CurGold >= item.mSelectGold && max_bet >= item.mSelectGold && SHGameManager.LeiZhuSeat != RoleManager.Self.ChairSeat) {
                item.SetEnable(true);
            } else {
                item.SetEnable(false);
                if (this.mSelectItem == item) {
                    this.SelectItem(null);
                }
            }
        }
    }

    private Item_ShenHua_BetList_Item mSelectItem;
    public void SelectItem(Item_ShenHua_BetList_Item item) {
        if (this.mSelectItem != null) {
            this.mSelectItem.SetSelect(false);
        }
        this.mSelectItem = item;
        if (this.mSelectItem != null) {
            this.mSelectItem.SetSelect(true);
            SHGameManager.CurSelectBet = this.mSelectItem.mSelectGold;
        } else {
            SHGameManager.CurSelectBet = 0;
        }
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_bet":
                this.mItemBet = tf.gameObject;
                this.mItemBet.SetActive(false);
                break;
        }
    }
}
