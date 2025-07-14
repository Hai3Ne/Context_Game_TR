using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 角色自己信息
/// </summary>
public class RoleInfoModel : SingleTon<RoleInfoModel>
{
	public uint RoomCfgID;
	public EnumGameMode GameMode;
    public EnumCoinMode CoinMode;
    public int RoomDeduct;//房间抽水（目前只处理子弹相关抽水）

	public Dictionary<uint, PlayerInfo> LobbyUsersMap = new Dictionary<uint, PlayerInfo> ();
	public void Init()
	{
	}

    public PlayerInfo GetPlayer(uint user_id) {
        return LobbyUsersMap.TryGet(user_id);
    }

	public bool isInRoom { get; set;}
	public PlayerInfo Self = new PlayerInfo ();
	public uint GetGold(){ return (uint)Self.GoldNum;	}

	public long CalLcrConsume(uint LcrCfgID, byte LcrLevel, uint timeDuration, uint multips)
	{
		uint mergerID = ((uint)LcrLevel) << 24 | LcrCfgID;
		LauncherVo lcrVo = FishConfig.Instance.LauncherConf.TryGet (mergerID);
		int bcnt = Mathf.FloorToInt(timeDuration * 60f / lcrVo.Interval);
		long totalCost = (long)(bcnt * (lcrVo.HiddenLevel + lcrVo.Level) * multips);
		return totalCost;
	}

	public long CalGameModeConsume(uint LcrCfgID, byte LcrLevel, uint roomCfgID, uint timeDuration, uint multips, IDictionary<uint,ushort> HeroCnts, IDictionary<uint,ushort> ItemCtns)
	{
		uint roomMultips = FishConfig.Instance.TimeRoomConf.TryGet(roomCfgID).RoomMultiple;
		long lcrCost = CalLcrConsume (LcrCfgID, LcrLevel, timeDuration, multips);
		long allHeroConsume = 0;
		foreach (var pair in HeroCnts) {
			HeroVo herovo = FishConfig.Instance.HeroConf.TryGet(pair.Key);
			long heroCosume = 0;
			foreach (var actID in herovo.ActionList) {
				HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet (actID);
				long actConsume = (long)(actVo.WorthFactor * actVo.AttTime * roomMultips);
				heroCosume += actConsume;
			}
			allHeroConsume += (long)(heroCosume * pair.Value);
		}

		long allItemConsume = 0;
		foreach (var pair in ItemCtns) {
			ItemsVo itemvo = FishConfig.Instance.Itemconf.TryGet (pair.Key);
			allItemConsume += (long)(itemvo.Worth * pair.Value);
		}

		return lcrCost + allHeroConsume + allItemConsume;
	}

	public uint[] GetAvaibleLauncherList()
	{
        var __en_launcher = FishConfig.Instance.LauncherConf.Keys.GetEnumerator();
        List<uint> lcrcfgIDList = new List<uint>();
        while (__en_launcher.MoveNext()) {
            uint cfgID = __en_launcher.Current & 0xFFFFFF;
            if (!lcrcfgIDList.Contains(cfgID))
                lcrcfgIDList.Add(cfgID);
        }
		return lcrcfgIDList.ToArray();
	}
}