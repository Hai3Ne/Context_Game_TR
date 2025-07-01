using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleInfo {
    public uint GameID;//游戏ID
    public uint UserID;//玩家ID
    public string NickName;
    public int MemberOrder;//会员等级
    public uint FaceID;//头像CRC
    public long GoldNum;//金币数量
    public byte Gender;//性别

    public ushort TableID = ushort.MaxValue;
    public ushort ChairSeat = ushort.MaxValue;

    public EnumUserStats UserStatus = EnumUserStats.US_FREE;
    public long Grade;


    public void AddGold(long gold) {
        this.GoldNum += gold;
    }
    public void SetGold(long gold) {
        this.GoldNum = gold;
    }
}
