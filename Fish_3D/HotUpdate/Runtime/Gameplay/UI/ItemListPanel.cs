using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemListPanel
{
    List<Item_Battle_Item> itemUIList = new List<Item_Battle_Item>();
	Transform mListCon;
	uint currentItemID = 0;
	GameObject iconEffInst;
	uint[] skillItemList = null;
    private UIGrid mGridItem;

	public void Init(Transform listCon, GameObject itemPrefab) {
		mListCon = listCon;

        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_NotItem) {//无道具模式
            skillItemList = new uint[0];
        } else {
            skillItemList = SceneLogic.Instance.RoomVo.Items;
        }

        itemPrefab.SetActive(false);
        this.mGridItem = listCon.GetComponent<UIGrid>();
        Item_Battle_Item item;
        for (int i = 0; i < skillItemList.Length; i++) {
            item = UIItem.CreateItem<Item_Battle_Item>(itemPrefab, listCon);
            item.transform.SetSiblingIndex(i);
            item.InitData(this, skillItemList[i]);
            itemUIList.Add(item);
		}
        //this.RefershGridCellHeight();
		RoleItemModel.Instance.RegisterGlobalMsg (SysEventType.ItemInfoChange, HandleItemChange);
        TimeManager.DelayExec(0, this.RefershGridCellHeight);//延迟一帧 防止刷新位置错误
	}

    public void RefershGridCellHeight() {//刷新道具列表项高度  100：默认  120：高
        int height = 100;
        Vector3 pos = this.mGridItem.transform.localPosition;//道具项位置
        pos.y = 20;
        ScenePlayer sp_1 = SceneLogic.Instance.PlayerMgr.GetPlayer(1);//1号位
        ScenePlayer sp_2 = SceneLogic.Instance.PlayerMgr.GetPlayer(2);//2号位
        if (sp_1 == null || sp_1.Launcher == null || sp_1.Launcher.m_UserInfoPanel.mIsShow == false) {
            height = 120;
            pos.y -= 50;
        }
        if (sp_2 == null || sp_2.Launcher == null || sp_2.Launcher.m_UserInfoPanel.mIsShow == false) {
            height = 120;
            pos.y += 50;
        }
        TweenPosition.Begin(this.mGridItem.gameObject, 0.5f, pos);
        //this.mGridItem.transform.localPosition = pos;
        this.mGridItem.cellHeight = height;
        this.mGridItem.animateSmoothly = true;
        this.mGridItem.Reposition();
    }
    
	public void Shutdown()
	{
        if (iconEffInst != null) {
            GameObject.Destroy(iconEffInst);
            iconEffInst = null;
        }
		RoleItemModel.Instance.UnRegisterGlobalMsg (SysEventType.ItemInfoChange, HandleItemChange);

        itemUIList.Clear();
		mListCon = null;
		skillItemList = null;
	}

    public bool GetIconPos(uint itemCfgID, out Vector3 uiWorldPos) {
        foreach (var item in this.itemUIList) {
            if (item.mItemCfgID == itemCfgID) {
                uiWorldPos = item.transform.position;
                return true;
            }
        }
        uiWorldPos = Vector3.zero;
        return false;
    }

	public void OnUseSkillItem(uint itemID) {
        foreach (var item in this.itemUIList) {
            if (item.mItemCfgID == itemID) {
                item.UserItemResult(true);
                break;
            }
        }
	}
    public void OnUseItemFail(uint itemID, byte error_code) {//使用道具失败
        foreach (var item in this.itemUIList) {
            if (item.mItemCfgID == itemID) {
                item.UserItemResult(false);
                break;
            }
        }
    }

	public void Update(float delta)
	{
        ////技能快捷键
        //for (int i = 0; i < skill_arr.Length; i++) {
        //    if (GInput.GetKeyDown(skill_arr[i])) {
        //        this.OnClickItem(i);
        //    }
        //}

        if (GameConfig.OP_AutoSkill && SceneLogic.Instance.PlayerMgr.AutoShot) {
            ItemManager.AutoUseSkill(SceneLogic.Instance.PlayerMgr.MySelf);
        }
	}
    public void SetItemCD(uint cfg_id, float cd) {
        foreach (var item in this.itemUIList) {
            if (item.mItemCfgID == cfg_id) {
                item.SetCD(cd);
            }
        }
    }
    public bool UserSkill(uint config_id, bool is_quick_buy) {//根据道具ID使用道具技能
        foreach (var item in this.itemUIList) {
            if (item.mItemCfgID == config_id) {
                return this.UserItem(item, is_quick_buy);
            }
        }
        return false;
    }
	void HandleItemChange(object args) {
        foreach (var item in this.itemUIList) {
            item.RefershInfo();
        }
	}
    private float _close_time;//关闭快捷购买时间
    public bool UserItem(Item_Battle_Item item, bool is_quick_buy) {

        //is_quick_buy = true;//所有购买强制设置为快捷购买

        if (item.mItemVo.GainMax > 0 && (item.mItemInfo == null || item.mItemInfo.ItemUseTime <= 0)) {
            return false;//无道具可用次数
        }
		if (SceneLogic.Instance.IsLOOKGuster) {//旁观者模式屏蔽道具使用
            return false;
		}
		if (SceneLogic.Instance.mIsXiuYuQi) {//休渔期不能使用技能
            return false;
		}
        uint itemCfgID = item.mItemCfgID;

        if (item.isCDStats) {//CD状态下
            return false;
        }
		if (SceneLogic.Instance.SkillMgr.IsCastingSkill)
            return false;

        foreach (var _item in this.itemUIList) {
            if (_item.isUsing) {//其他道具正在使用
                return false;
            }
        }
		if (SceneLogic.Instance.bClearScene)
            return false;
		ItemsVo vo = FishConfig.Instance.Itemconf.TryGet (itemCfgID);
		int itemcnt = RoleItemModel.Instance.getItemCount (itemCfgID);
        if (itemcnt <= 0) {//技能道具开启购买后，会自动进行购买且使用
            if (vo.SaleNum > 0) {
                long min_gold = SceneLogic.Instance.RoomVo.CostScore + vo.SalePrice;//购买道具最低金币要求
                if (SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.PlayerMgr.MyClientSeat) < min_gold) {
                    //金币少于某些值，禁止购买
                    string gold_str;
                    if (min_gold > 10000 && (min_gold % 10000 == 0)) {
                        gold_str = string.Format("{0}万", min_gold / 10000);
                    } else {
                        gold_str = min_gold.ToString();
                    }
                    SystemMessageMgr.Instance.ShowMessageBox(string.Format(StringTable.GetString("Tip_42"), gold_str));
                    return false;
                }
                if (is_quick_buy == false) {
                    if (!WndManager.Instance.isActive(EnumUI.QuickBuyUI)) {
                        WndManager.Instance.ShowUI(EnumUI.QuickBuyUI, itemCfgID);
                        _close_time = Time.realtimeSinceStartup + 0.5f;
                    } else if (Time.realtimeSinceStartup >= _close_time) {
                        WndManager.Instance.CloseUI(EnumUI.QuickBuyUI);
                    }
                    return false;
                //} else {
                //    if (RoleInfoModel.Instance.CoinMode == EnumCoinMode.Score) {
                //        SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_33"));
                //    } else {
                //        UI.EnterUI<UI_poor>(ui => {
                //            ui.InitData(1);//乐豆不足
                //        });
                //        //SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_5"));
                //    }
                }
            } else {
                //最大购买数量为0，不开放购买
                SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_1"));
                return false;
            }
        }

		uint skillID = (uint)vo.Value0;
		SkillVo skvo = FishConfig.Instance.SkillConf.TryGet (skillID);

        item.isCDStats = false;
        item.isUsing = true;
        item.skillVo = skvo;
        item.time = 0f;

		currentItemID = itemCfgID;

		iconEffInst = GameUtils.CreateGo (FishResManager.Instance.Eff_ItemIconHalo);
        iconEffInst.transform.SetParent(item.transform);
		iconEffInst.transform.localPosition = Vector2.zero;
		AutoDestroy.Begin (iconEffInst, SceneLogic.Instance.PlayerMgr.MyClientSeat, skvo.BBuffID);
		if (itemcnt <= 0 && is_quick_buy) {//技能道具开启购买后，会自动进行购买且使用
			SendItemApply (skvo, true);
		} else {
			SendItemApply (skvo, false);
		}
        return true;
	}

	void SendItemApply(SkillVo skvo, bool isAutoUse) {
		Vector3 screenPos = Utility.MainCam.WorldToScreenPoint(new Vector3(0f, 0f, ConstValue.NEAR_Z+1f));
		ushort targetFishID = 0xFFFF;
		byte fishPartId = 0xFF;
		SceneLogic.Instance.SkillMgr.SkillPrepareForAim (skvo, delegate {				
			if (skvo.NeedTarget) {
				Fish tarFish = SceneLogic.Instance.FishMgr.GetAtkTargetFish ();
				if (tarFish != null) {
					screenPos = tarFish.ScreenPos;
					targetFishID = tarFish.FishID;
					fishPartId = SceneLogic.Instance.PlayerMgr.LockfishPartIndex;
				}
			}
			FishNetAPI.Instance.UseBattleItem (currentItemID, screenPos, targetFishID, fishPartId, short.MaxValue, isAutoUse);
		});
	}

    public void OnClickItem(int idx) {
        if (itemUIList.Count > idx) {
            this.UserItem(itemUIList[idx], GameConfig.OP_QuickBuy);
        }
    }
}
