using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 道具逻辑管理
/// </summary>
public class ItemManager {
    private class ItemMessage{
        public ItemsVo Vo;
        public AutoUseVo AutoUseVo;
        public int Count;
        public float NextTime;
    }
    public static void InitData(){
        RoleItemModel.Instance.RegisterGlobalMsg(SysEventType.ItemInfoChange, HandleItemChange);
    }

    private static List<ItemMessage> mItemList = new List<ItemMessage>();
    private static List<ItemMessage> mHeroList = new List<ItemMessage>();
    private static void HandleItemChange(object args) {
        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_NotItem) {
            return;
        }
        TimeRoomVo roomVo = SceneLogic.Instance.RoomVo;// FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID());
        Dictionary<uint, float> dic_time = new Dictionary<uint, float>();
        for (int i = 0; i < mItemList.Count; i++) {
            dic_time[mItemList[i].Vo.CfgID] = mItemList[i].NextTime;
        }
        for (int i = 0; i < mHeroList.Count; i++) {
            dic_time[mHeroList[i].Vo.CfgID] = mHeroList[i].NextTime;
        }
        mItemList.Clear();
        mHeroList.Clear();
        ItemsVo vo;
        float _next_time;
        for (int i = 0; i < roomVo.Items.Length; i++) {//道具技能
            vo = FishConfig.Instance.Itemconf.TryGet(roomVo.Items[i]);
            if (dic_time.TryGetValue(vo.CfgID, out _next_time) == false) {
                _next_time = 0;
            }
            mItemList.Insert(0, new ItemMessage {
                Vo = vo,
                AutoUseVo = FishConfig.Instance.mAutoUseConf.TryGet(vo.AutoType),
                Count = RoleItemModel.Instance.getItemCount(vo.CfgID),
                NextTime = _next_time,
            });
        }
        for (int i = 0; i < roomVo.Heroes.Length; i++) {//英雄召唤卡
            vo = FishConfig.Instance.Itemconf.TryGet(roomVo.Heroes[i]);
            if (dic_time.TryGetValue(vo.CfgID, out _next_time) == false) {
                _next_time = 0;
            }
            mHeroList.Insert(0, new ItemMessage {
                Vo = vo,
                AutoUseVo = FishConfig.Instance.mAutoUseConf.TryGet(vo.AutoType),
                Count = RoleItemModel.Instance.getItemCount(vo.CfgID),
                NextTime = _next_time,
            });
        }
    }

    private static float mNextLauncherTime = 0;//下次检测炮台技能时间
    public static bool IsUseLauncher(ScenePlayer sp, AutoUseVo vo) {//是否满足释放炮台技能的条件
        if (Time.time >= mNextLauncherTime && ItemManager.IsMeet(sp, vo)) {
            mNextLauncherTime = Time.time + vo.UseCD;
            return true;
        } else {
            return false;
        }
    }
    private static ItemMessage __msg = null;
    public static void AutoUseSkill(ScenePlayer sp) {//判断是否有符合技能释放的条件
        for (int i = 0; i < mItemList.Count; i++) {
            __msg = mItemList[i];
            if (__msg.Count > 0 && Time.time >= __msg.NextTime) {
                if (ItemManager.IsMeet(sp, __msg.AutoUseVo)) {
                    if (SceneLogic.Instance.LogicUI.ItemListUI.UserSkill(__msg.Vo.CfgID, false)) {
                        __msg.NextTime = Time.time + __msg.AutoUseVo.UseCD;
                    }
                }
            }
        }
        __msg = null;
    }
    public static void AutoUseHero(ScenePlayer sp) {//判断是否有符合召唤英雄的条件
        for (int i = 0; i < mHeroList.Count; i++) {
            __msg = mHeroList[i];
            if (__msg.Count > 0 && Time.time >= __msg.NextTime) {
                if (ItemManager.IsMeet(sp, __msg.AutoUseVo)) {
                    if (SceneLogic.Instance.HeroMgr.LaunchHero(__msg.Vo.CfgID)) {
                        __msg.NextTime = Time.time + __msg.AutoUseVo.UseCD;
                    }
                }
            }
        }
        __msg = null;
    }

    public static bool IsMeet(ScenePlayer sp, AutoUseVo vo) {//判断条件是否满足
        if (SceneLogic.Instance.FishMgr.ContainsViewFish(vo.NoneFishID)) {
            return false;
        }
        //if (SceneLogic.Instance.BulletMgr.BufferMgr.ContainsBuffer(sp.ClientSeat, vo.Condition2Buff)) {
        //    return true;
        //} else {
            if (vo.LockFish && sp.LockedFishID == 0) {
                return false;
            }
            int num = SceneLogic.Instance.FishMgr.GetViewFishCount(vo.BigFishMulti, Mathf.Abs(vo.FishNum));
            if (vo.FishNum >= 0) {//正值：表示大于等于判定   负值：表示小于判定
                if (num < vo.FishNum) {
                    return false;
                }
            } else {
                if (num >= -vo.FishNum) {
                    return false;
                }
            }

            return true;
        //}
    }
}
