using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 腾讯相关统计
/// </summary>
public class MtaManager {
    private static bool is_open {
        get {
#if UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return false;
#else
            return false;// SceneLogic.Instance.IsLOOKGuster == false;
#endif
        }
    }
    public static void InitMta() {
        if (is_open == false) {
            return;
        }
        // set配置接口
        // 开启debug，发布时请设置为false
        MtaService.SetDebugEnable(true);
        MtaService.SetCustomUserId(GameUtils.GetUnID());
        // 设置发布渠道，如果在androidManifest.xml配置，可不需要调用此接口
        MtaService.SetInstallChannel("1378");
        // 设置上报策略，默认为APP_LAUNCH
        MtaService.SetStatSendStrategy(MtaService.MTAStatReportStrategy.INSTANT);

        // 初始化，andriod可跳过此步骤
        // !!!!! 重要 !!!!!!!
        // MTA的appkey在android和ios系统上不同，请为根据不同平台设置不同appkey，否则统计结果可能会有问题。
        string mta_appkey = null;
#if UNITY_IPHONE || UNITY_IOS
		mta_appkey = "IA91Y6FMP9QD";
#elif UNITY_ANDROID
        mta_appkey = "AQMD7T393PKE";
#endif
        MtaService.StartStatServiceWithAppKey(mta_appkey);
        // 上报QQ号码
        // MtaService.reportQQ("123456");

        // 上报游戏用户，游戏高级模型需要用到
        // MtaGameUser gameUser = new MtaGameUser("account1", "worldname1", "level1");
        //MtaService.ReportGameUser(gameUser);

        //// 根据业务实际情况，填充monitor对象的值
        //MtaAppMonitor monitor = new MtaAppMonitor("download");
        //monitor.RequestSize = 1000;
        //monitor.ResponseSize = 304;
        //monitor.MillisecondsConsume = 1000;
        //monitor.ResultType = MtaAppMonitor.SUCCESS_RESULT_TYPE;
        //monitor.ReturnCode = 0;
        //monitor.Sampling = 1;
        //// 上报接口监控数据
        //MtaService.ReportAppMonitorStat(monitor);

#if UNITY_ANDROID
        MtaService.InitNativeCrashReport(System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
#endif
        //MtaService.ReportUserException("ReportUserException", "ReportUserException stacks");
        //MtaService.ReportCrash("ReportCrash", "ReportCrash stacks");

        //int i = 0;
        //int b = 3 / i;
        //string str = "b=" + b;
        //MtaService.printLog(MtaService.MTAStatLogLevel.ERROR, "mta", str);


        //// 进入场景
        //MtaService.TrackBeginPage("page1");
        //// 退出场景
        //MtaService.TrackEndPage("page1");

        //// 构建自定义事件的key-value
        //Dictionary<string, string> dict = new Dictionary<string, string>();
        //dict.Add("account", "12345");
        //dict.Add("amount", "100");
        //dict.Add("item", "firearm");
        //// 上报buy类型的自定义事件
        //MtaService.TrackCustomKVEvent("buy", dict);

        //// 构建自定义事件的key-value
        //Dictionary<string, string> beDict = new Dictionary<string, string>();
        //beDict.Add("account", "12345");
        //beDict.Add("level", "8");
        //beDict.Add("name", "model");
        //// 通关前
        //MtaService.TrackCustomBeginKVEvent("mission", beDict);
        //// 通关ing...
        //// 通关后
        //MtaService.TrackCustomEndKVEvent("mission", beDict);

        //// 获取在线配置，key为前台配置的在线配置信息
        //Debug.Log("getCustomProperty=" + MtaService.GetCustomProperty("key"));

#if UNITY_ANDROID
        // 监控www.qq.com域名
        Dictionary<string, int> speedMap = new Dictionary<string, int>();
        speedMap.Add("www.qq.com", 80);
        MtaService.TestSpeed(speedMap);

        //// 获取MID
        //string mid = MtaService.GetMid();
        //LogMgr.Log("mid is " + mid);
#endif
    }
    public static void BeginLauncherEvent(LauncherVo vo) {//炮台相关统计开始
        if (is_open == false) {
            return;
        }
        if (vo == null) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("launcher_id", vo.LrCfgID.ToString());
        dict.Add("launcher_lv", vo.Level.ToString());
        MtaService.TrackCustomBeginKVEvent("Launcher_Time", dict);
    }
    public static void EndLauncherEvent(LauncherVo vo) {//炮台相关统计结束
        if (is_open == false) {
            return;
        }
        if (vo == null) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("launcher_id", vo.LrCfgID.ToString());
        dict.Add("launcher_lv", vo.Level.ToString());
        MtaService.TrackCustomEndKVEvent("Launcher_Time", dict);
    }

    public static void AddItemEvent(ItemsVo vo, EnumItemChangeSource source, int count) {//道具相关统计
        if (is_open == false) {
            return;
        }
        List<uint> ignore_list = new List<uint>();
        ignore_list.Add(2012u);//2012	新手抽奖券
        ignore_list.Add(2013u);//2013	初级抽奖券
        ignore_list.Add(2014u);//2014	中级抽奖券
        ignore_list.Add(2015u);//2015	高级抽奖券
        if (vo.DropShowType == 0 && ignore_list.Contains(vo.CfgID) == false) {//奖卷也加到道具中
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("item_id", vo.CfgID.ToString());
        dict.Add("item_name", StringTable.GetString(vo.ItemName));
        dict.Add("change_count", count.ToString());
        switch (source) {
            case EnumItemChangeSource.ItemDropdown://掉落
                if (vo.ItemType == 4) {//英雄
                    MtaService.TrackCustomKVEvent("Hero_Drop", dict);
                } else {
                    MtaService.TrackCustomKVEvent("Item_Drop", dict);
                }
                break;
            case EnumItemChangeSource.ItemLottery://抽奖
                //MtaService.TrackCustomKVEvent("Item_Buy", dict);
                break;
            case EnumItemChangeSource.ItemPurchase://购买
                if (vo.ItemType == 4) {//英雄
                    MtaService.TrackCustomKVEvent("Hero_Buy", dict);
                } else {
                    MtaService.TrackCustomKVEvent("Item_Buy", dict);
                }
                break;
            case EnumItemChangeSource.ItemUse://使用
                if (vo.ItemType == 4) {//英雄
                    MtaService.TrackCustomKVEvent("Hero_Use", dict);
                } else {
                    MtaService.TrackCustomKVEvent("Item_Use", dict);
                }
                break;
        }
    }
    public static void AddLauncherSkillEvent(uint skill_id) {//炮台技能
        if (is_open == false) {
            return;
        }
        SkillVo skillVo = FishConfig.Instance.SkillConf.TryGet(skill_id);
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("skill_id", skillVo.CfgID.ToString());
        dict.Add("skill_name", StringTable.GetString(skillVo.Name));
        MtaService.TrackCustomKVEvent("Launcher_Use", dict);
    }

    public static void AddBossKillEvent(Fish fish, byte client_seat) {//击杀BOSS
        if (is_open == false) {
            return;
        }
        if (client_seat == SceneLogic.Instance.PlayerMgr.MySelf.ClientSeat) {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
            dict.Add("name", RoleInfoModel.Instance.Self.NickName);
            dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
            dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
            dict.Add("boss_id", fish.vo.CfgID.ToString());
            dict.Add("boss_name", StringTable.GetString(fish.vo.Name));
            MtaService.TrackCustomKVEvent("BOSSDieCount", dict);
        }
    }
    public static void AddAllSellEvent(long gold) {//全部出售
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("gold", gold.ToString());
        MtaService.TrackCustomKVEvent("AllSell", dict);
    }

    public static void AddLotteryUserEvent(int count,int item_count) {//转盘抽奖使用
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("use_count", count.ToString());
        dict.Add("item_count", item_count.ToString());//剩余道具数量
        MtaService.TrackCustomKVEvent("lottery_use", dict);
    }
    public static void AddLotteruEnter() {//进入转盘界面次数
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        MtaService.TrackCustomKVEvent("lottery_enter", dict);
    }
    public static void AddWorldBoxAward(long gold,int rank, int type) {//全服宝箱获取奖励  1.打掉一管血  2.眩晕  3.消耗榜奖励
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("datetime", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dict.Add("gold", gold.ToString());
        dict.Add("rank", rank.ToString());//第一管血/排行
        switch (type) {
            case 1://打掉一管血奖励
                dict.Add("func", "击杀");
                break;
            case 2://眩晕
                dict.Add("func", "眩晕");
                break;
            case 3://消耗榜
                dict.Add("func", "消耗榜奖励");
                break;
        }
        MtaService.TrackCustomKVEvent("world_box_award", dict);
    }

    public static void BeginWorldBox() {
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        MtaService.TrackCustomBeginKVEvent("world_box_join", dict);
    }
    public static void EndWorldBox() {
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        MtaService.TrackCustomEndKVEvent("world_box_join", dict);
    }

    public static void BeginGameInfo() {//游戏开始
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("game_type", RoleInfoModel.Instance.GameMode.ToString());
        MtaService.TrackCustomBeginKVEvent("game_info", dict);
    }
    public static void EndGameInfo() {//退出游戏
        if (is_open == false) {
            return;
        }
        if (SceneLogic.Instance.RoomVo == null) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("game_type", RoleInfoModel.Instance.GameMode.ToString());
        MtaService.TrackCustomEndKVEvent("game_info", dict);
    }
    public static void AddGameEnter(long gold) {//游戏进入
        if (is_open == false) {
            return;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("user_id", RoleInfoModel.Instance.Self.UserID.ToString());
        dict.Add("name", RoleInfoModel.Instance.Self.NickName);
        dict.Add("room_id", SceneLogic.Instance.RoomVo.CfgID.ToString());
        dict.Add("room_name", StringTable.GetString(SceneLogic.Instance.RoomVo.Name));
        dict.Add("game_type", RoleInfoModel.Instance.GameMode.ToString());
        dict.Add("gold", gold.ToString());
        MtaService.TrackCustomBeginKVEvent("game_enter", dict);
    }
}
