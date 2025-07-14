using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager {
    private static List<PayItem> mItemList = new List<PayItem>();
    private static PayItem mShouChongItem;//首充项
    public static uint ShouChongGold;//首充金币数
    public static CMD_GP_FirstChargeAward msg_cs_award;//首充奖励
    public static bool mIsShowFrist = false;//是否显示首充
    public static void InitData(CMD_GP_PayInfo info) {
        mItemList.Clear();
        mItemList.AddRange(info.PayItems);
        mShouChongItem = info.FirstPayItem;
        PayItem item = GetShouChongInfo();
        if (item != null) {
            ShouChongGold = (uint)item.Score;
        } else {
            ShouChongGold = 0u;
        }
        mIsShowFrist = false;
    }
    public static void SetFristTick(bool is_show) {//更新首充标记
        mIsShowFrist = is_show;
        EventManager.Notifiy(GameEvent.Hall_SCUpdate, is_show);
        if (mIsShowFrist) {
            MainEntrace.Instance.ShowLoginTick();
        }
    }

    public static void SetFristBuyAward(CMD_GP_FirstChargeAward awards) {//设置首充奖励
        msg_cs_award = awards;
    }

    public static List<PayItem> GetPayList() {
        return mItemList;
    }
    public static PayItem GetPayByVIP(int vip) {//获取VIP最低档次
        PayItem pay = null;
        foreach (var item in mItemList) {
            if (item.MemberOrder >= vip) {
                if (pay == null || pay.Price > item.Price) {
                    pay = item;
                }
            }
        }
        return pay;
    }
    public static PayItem GetPayByIndex(int index) {//根据索引获取购买项
        if (mItemList.Count > index) {
            return mItemList[index];
        }else if (mItemList.Count > 0) {
            return mItemList[mItemList.Count - 1];
        } else {
            return null;
        }
    }

    public static bool IsFirstPay(string pro_id) {//是否首充
        if (mShouChongItem != null && mShouChongItem.szProductID == pro_id) {
            return true;
        } else {
            return false;
        }
    }
    public static PayItem GetShouChongInfo() {
        //if (mItemList.Count > 0) {
        //    return mItemList[0];
        //}
        return mShouChongItem;
    }
}
