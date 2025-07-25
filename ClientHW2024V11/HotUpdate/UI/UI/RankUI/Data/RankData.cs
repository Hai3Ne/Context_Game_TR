using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RankItemType
{
    E_RANK_G2_DAY = 1,		// game2 日排行
    E_RANK_G2_WEEK = 2,		// game2 周排行
    E_RANK_G2_MONTH = 3,	// game2 月排行

    E_RANK_G3_DAY = 4,		// game3 日排行
    E_RANK_G3_WEEK = 5,		// game3 周排行
    E_RANK_G3_MONTH = 6,	// game3 月排行

    E_RANK_G4_DAY = 7,		// game4 日排行
    E_RANK_G4_WEEK = 8,		// game4 周排行
    E_RANK_G4_MONTH = 9,    // game4 月排行
    E_RANK_COMPERHENSIVE_DAY =50,//综合日排行
    E_RANK_COMPERHENSIVE_WEEK = 51,//综合日排行
    E_RANK_COMPERHENSIVE_MONTH = 52,//综合日排行
    E_RANK_G_MAX,
}

enum E_RANK_TYPE_G
{
    
}
public class RankData
{
    
}

public class RankItemData 
{
    public RankItemType type;//类型
    public string headSpriteName;//玩家头像
    public string playerName;//玩家姓名
    public long bets;
    public int rank;
    public long roleId;
    public RankItemData(int _type,string _headSpriteName,string _playerName,long _bets,int _rank,long _roleId) 
    {
        type =(RankItemType) _type;
        headSpriteName = _headSpriteName;
        playerName = _playerName;
        bets = _bets;
        rank = _rank;
        roleId = _roleId;
    }
}
