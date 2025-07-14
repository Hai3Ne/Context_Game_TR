using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ItemCachInfo
{
    public uint itemCfgID;
    public int Count;
    public int ItemUseTime;//剩余可用次数
    public float ItemIncTick;//累积剩余时间 单位：秒
}

public class RoleItemModel : SingleTon<RoleItemModel>
{
	private Dictionary<UInt32, ItemCachInfo> ItemMap = new Dictionary<UInt32, ItemCachInfo>();
	public void Init()
	{
		ItemMap.Clear ();
		/*
		tagItemInfo tagitemifo = new tagItemInfo ();
		tagitemifo.ItemCfgID = 101;
		tagitemifo.Count = 10;
		ItemMap.Add (101, tagitemifo);

		tagitemifo = new tagItemInfo ();
		tagitemifo.ItemCfgID = 102;
		tagitemifo.Count = 10;
		ItemMap.Add (102, tagitemifo);


		tagitemifo = new tagItemInfo ();
		tagitemifo.ItemCfgID = 103;
		tagitemifo.Count = 10;
		ItemMap.Add (103, tagitemifo);

		tagitemifo = new tagItemInfo ();
		tagitemifo.ItemCfgID = 104;
		tagitemifo.Count = 10;
		ItemMap.Add (104, tagitemifo);


		tagitemifo = new tagItemInfo ();
		tagitemifo.ItemCfgID = 105;
		tagitemifo.Count = 10;
		ItemMap.Add (105, tagitemifo);

		uint[] heroIDS = new uint[]{ 10001, 10002, 10003, 10004 };
		for (int i = 0; i < heroIDS.Length; i++) {
			tagitemifo = new tagItemInfo ();
			tagitemifo.ItemCfgID = heroIDS[i];
			tagitemifo.Count = 10;
			ItemMap.Add (heroIDS[i], tagitemifo);
		}
		*/
	}

    public void Clear() {
        ItemMap.Clear();
    }

	public void InitData(tagItemData[] tagItemInfos)
	{
		ItemMap.Clear ();
		if (tagItemInfos == null || tagItemInfos.Length <= 0)
			return;
		foreach (var item in tagItemInfos) {
			ItemCachInfo itemCache = new ItemCachInfo ();
			itemCache.itemCfgID = item.ItemCfgID;
            itemCache.Count = item.Count;
            itemCache.ItemUseTime = item.ItemUseTime;
            itemCache.ItemIncTick = item.ItemIncTick + 0.999f;
			ItemMap.Add (item.ItemCfgID, itemCache);
		}

		RoleItemModel.Instance.Notifiy (SysEventType.ItemInfoChange);
	}

	public Dictionary<uint,ushort> GetItemListByType(EnumItemType type)
	{
		Dictionary<uint,ushort> itemDic = new Dictionary<uint,ushort> ();
		Dictionary<uint, ItemCachInfo>.KeyCollection idlst = ItemMap.Keys;
		foreach (var id in idlst) {
			if (FishConfig.Instance.Itemconf.TryGet (id).ItemType == (byte)type) {
				itemDic[id] = (ushort)ItemMap [id].Count;
			}
		}
		return itemDic;
	}

	public uint ApplyForData(uint itemCfgId)
	{
		ItemsVo itemvo = FishConfig.Instance.Itemconf.TryGet (itemCfgId);	
		if (itemvo != null) {
			switch ((EnumItemType)itemvo.ItemType) {
			case EnumItemType.Hero:
				return (uint)itemvo.Value0;

			case EnumItemType.Skill:
				return (uint)itemvo.Value0;

			default:
				return (uint)itemvo.Value0;
			}
		}
		return 0;
	}
    public ItemCachInfo GetItemInfo(uint cfg_id) {
        ItemCachInfo info;
        if (ItemMap.TryGetValue(cfg_id, out info)== false) {
            info = new ItemCachInfo{
                Count = 0,
                itemCfgID = cfg_id,
            };
            ItemMap.Add(cfg_id, info);
        }
        return info;
    }
	public int getItemCount(uint itemCfgId)
	{
		if (ItemMap.ContainsKey (itemCfgId)) {
			return ItemMap [itemCfgId].Count;
		}
		return 0;
	}

	public bool IsGoldenItem(uint itemCfgID)
	{
		ItemsVo itemvo = FishConfig.Instance.Itemconf.TryGet (itemCfgID);	
		if (itemvo != null) {
			return itemvo.ItemType == (byte)EnumItemType.Money;
		}
		return false;
	}

    public void UpdateItem(uint config_id, int count) {
        if (IsGoldenItem(config_id)) {
            if (LogMgr.ShowLog) {
                LogMgr.Log("Gold Item Update: " + count);
            }
            if (SceneLogic.Instance.IsGameOver == false) {
                TablePlayerInfo tplayer = SceneLogic.Instance.FModel.GetTabPlayerByCSeat(SceneLogic.Instance.PlayerMgr.MyClientSeat);
                if (tplayer != null)
                    tplayer.GlobeNum += count;
            } else {
                RoleInfoModel.Instance.Self.GoldNum += count;
            }
        }
        ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(config_id);
        ItemCachInfo info = this.GetItemInfo(config_id);
        info.Count = Math.Min((int)vo.MaxCount, Mathf.Max(0, info.Count + count));

        RoleItemModel.Instance.Notifiy(SysEventType.ItemInfoChange);
    }

	public void OnItemUpdate(SC_GR_ItemCountChange pInfo) {
		if (pInfo != null) {
			if (IsGoldenItem(pInfo.ItemCfgID)) {
                if (LogMgr.ShowLog) {
                    LogMgr.Log("Gold Item Update: " + pInfo.ChangeCount);
                }
				if (SceneLogic.Instance.IsGameOver == false) {
					TablePlayerInfo tplayer = SceneLogic.Instance.FModel.GetTabPlayerByCSeat (SceneLogic.Instance.PlayerMgr.MyClientSeat);
					if (tplayer != null)
						tplayer.GlobeNum += pInfo.ChangeCount;
				} else {
					RoleInfoModel.Instance.Self.GoldNum += pInfo.ChangeCount;
				}
			}
            ItemCachInfo info = this.GetItemInfo(pInfo.ItemCfgID);
            info.Count = Mathf.Max(0, info.Count + pInfo.ChangeCount);

            if ((EnumItemChangeSource)pInfo.Source == EnumItemChangeSource.ItemUse) {
                info.ItemUseTime -= 1;
            }

            if (pInfo.Source != (int)EnumItemChangeSource.ItemLottery) {
                RoleItemModel.Instance.Notifiy(SysEventType.ItemInfoChange);
            }
		}
	}

    public void ClearAllItem() {//清空所有道具

        uint[] items = SceneLogic.Instance.RoomVo.Heroes;
        ItemCachInfo item;
        for (int i = 0; i < items.Length; i++) {
            if (ItemMap.TryGetValue(items[i], out item)) {
                item.Count = 0;
            }
        }
        items = SceneLogic.Instance.RoomVo.Items;
        for (int i = 0; i < items.Length; i++) {
            if (ItemMap.TryGetValue(items[i], out item)) {
                item.Count = 0;
            }
        }
        RoleItemModel.Instance.Notifiy(SysEventType.ItemInfoChange);
    }
}
