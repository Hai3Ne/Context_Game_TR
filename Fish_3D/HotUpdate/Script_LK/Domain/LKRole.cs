using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKRole {
    public RoleInfo RoleInfo;
    public long FishGold;//鱼币
    public int CannonMul;//炮台倍率
    public bool IsVipCannon;//是否是会员炮

    public long ChangeGold;//乐豆
    public void AddFishGold(long gold) {
        this.FishGold += gold;
        EventManager.Notifiy(GameEvent.UserInfoChange, this.RoleInfo);
    }
    public long GetBaseGold() {//获取剩余乐豆
        return this.RoleInfo.GoldNum + this.ChangeGold;
    }
}
