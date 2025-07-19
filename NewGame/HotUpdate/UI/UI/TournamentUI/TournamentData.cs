using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum E_ARENA_LEVEL_ID
{
    E_ARENA_F_None = 0,
    E_ARENA_F_1 = 1,
    E_ARENA_F_2 = 2,
    E_ARENA_F_3 = 3,
    E_ARENA_F_4 = 4,
    E_ARENA_F_5 = 5,
    E_ARENA_F_6 = 6,
    E_ARENA_F_7 = 7,
}
public class TournamentData 
{
}
public class TournamentItemData
{
    public E_ARENA_LEVEL_ID type;//类型
    public string headSpriteName;//玩家头像
    public string playerName;//玩家姓名
    public long roleId;
    public TournamentItemData(int _type, string _headSpriteName, string _playerName,long _roleId)
    {
        type = (E_ARENA_LEVEL_ID)_type;
        headSpriteName = _headSpriteName;
        playerName = _playerName;
        roleId = _roleId;
    }
}
