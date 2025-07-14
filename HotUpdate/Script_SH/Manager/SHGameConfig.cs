using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHGameConfig {
    private static byte[] versions = new byte[] { 6, 42, 0, 0 };//新版五子棋客户端版本号
    public const ushort KindID = 124;//类型索引
    public const uint ClientGameID = 124;//神兽游戏ID
    public const int MaxAreaCount = 8;//最大区域个数

    public static void SetGameVersion(System.Xml.XmlNode node) {//设置游戏版本号
        if (node == null) {
            return;
        }
        string ver = node.Attributes["value"].Value;
        if (string.IsNullOrEmpty(ver)) {
            return;
        }

        string[] verarr = ver.Split('.');
        if (verarr.Length < 4) {
            return;
        }
        for (int i = 0; i < versions.Length; i++) {
            versions[i] = byte.Parse(verarr[i]);
        }
    }
    public static string VersionStr {
        get {
            return string.Format("{0}.{1}.{2}.{3}", versions[0], versions[1], versions[2], versions[3]);
        }
    }
    public static uint VersionCode {
        get {
            return GameUtils.ConvertToVersion(versions[0], versions[1], versions[2], versions[3]);
        }
    }

    public const string Audio_BG = "sh_BACK_GROUND";//背景音乐
    public const string Audio_ADD_GOLD = "sh_ADD_GOLD";//下注1
    public const string Audio_ADD_GOLD_EX = "sh_ADD_GOLD_EX";//下注2（大筹码）
    public const string Audio_END_DRAW = "sh_END_DRAW";//平局音效
    public const string Audio_END_LOST = "sh_END_LOST";//失败音效
    public const string Audio_END_WIN = "sh_END_WIN";//胜利音效
    public const string Audio_GAME_START = "sh_GAME_START";//游戏开始
    public const string Audio_idc_snd_ = "sh_idc_snd_";//转动音效
    //public const string Audio_maidinglishou = "sh_maidinglishou";//
    public const string Audio_qingxiazhu = "sh_qingxiazhu";//请下注
    //public const string Audio_shuohuasheng = "sh_shuashuosheng";//
    //public const string Audio_SND_FLASH = "sh_SND_FLASH";//
    public const string Audio_SND_SELGRID_BIGWARN = "sh_SND_SELGRID_BIGWARN";//转到神兽警告
    public const string Audio_SND_STARTSHOWCRAD = "sh_SND_STARTSHOWCARD";//开始转动
    //public const string Audio_TIME_WARIMG = "sh_TIME_WARIMG";//

    public static string[] VIPName = new string[]{//会员名称列表
        "",
        "蓝钻",
        "黄钻",
        "红钻",
        "红钻",
        "至尊",
        "至尊",
    };
    //VIP图标
    public static string[] VIPIcons = { "", "diamond_blue", "diamond_yellow", "diamond_red", "diamond_red", "diamond_red", "diamond_red" };
    
    //正常动物Icon //1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
    public static string[] OptionIcons = { "", "Big_Basaltic-min", "Big_Dragon-min", "Big_Suzaku-min", "Big_Tiger-min", "Small_Basaltic-min", "Small_Dragon-min", "Small_Suzaku-min", "Small_Tiger-min" };
    //选择动物Icon //1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
    public static string[] SelectIcons = { "", "Big_Basaltic_1", "Big_Dragon_1", "Big_Suzaku_1", "Big_Tiger_1", "Small_Basaltic_1", "Small_Dragon_1", "Small_Suzaku_1", "Small_Tiger_1" };
    //小动物Icon   //1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
    public static string[] SmallIcons = { "", "xw-min", "ql-min", "zq-min", "bh-min", "smallxw-min", "smallql-min", "smallzq-min", "smallbh-min" };
    //动物大图     //1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
    public static string[] BigImages = { "", "End_Big_Basaltic-min-min", "End_Big_Dragon-min", "End_Big_Suzaku-min", "End_Big_Tiger-min", "End_Small_Basaltic-min", "End_Small_Dragon-min", "End_Small_Suzaku-min", "End_Small_Tiger-min" };
    //动画名称     //1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
    public static string[] OptionNames = { "", "玄武", "青龙", "朱雀", "白虎", "小乌龟", "小白龙", "小凤凰", "小老虎" };
    //动画顺序
    public static SHEnumOption[] Options = { SHEnumOption.XuanWu,
                                             SHEnumOption.XiaoBaiLong,
                                             SHEnumOption.XiaoLaoHu,
                                             SHEnumOption.XiaoFengHuang,
                                             SHEnumOption.QingLong, 
                                             SHEnumOption.XiaoLaoHu,
                                             SHEnumOption.XiaoFengHuang,
                                             SHEnumOption.XiaoWuGui,
                                             SHEnumOption.BaiHu,
                                             SHEnumOption.XiaoFengHuang, 
                                             SHEnumOption.XiaoWuGui, 
                                             SHEnumOption.XiaoBaiLong, 
                                             SHEnumOption.ZhuQue,
                                             SHEnumOption.XiaoWuGui,
                                             SHEnumOption.XiaoLaoHu,
                                             SHEnumOption.XiaoBaiLong
                                           };


    public static string GetLvStr(long gold) {//根据乐豆 获取用户级别
        if (gold >= 1000000000) {//LevelItem23=帝王,1000000000
            return "帝王";
        } else if (gold >= 500000000) {//LevelItem22=丞相,500000000
            return "丞相";
        } else if (gold >= 300000000) {//LevelItem21=巡抚,300000000
            return "巡抚";
        } else if (gold >= 100000000) {//LevelItem20=总督,100000000
            return "总督";
        } else if (gold >= 40000000) {//LevelItem19=知府,40000000
            return "知府";
        } else if (gold >= 24000000) {//LevelItem18=通判,24000000 
            return "通判";
        } else if (gold >= 13600000) {//LevelItem17=知县,13600000 
            return "知县";
        } else if (gold >= 8000000) {//LevelItem16=大地主,8000000
            return "大地主";
        } else if (gold >= 4800000) {//LevelItem15=小地主,4800000
            return "小地主";
        } else if (gold >= 2720000) {//LevelItem14=大财主,2720000
            return "大财主";
        } else if (gold >= 1600000) {//LevelItem13=小财主,1600000
            return "小财主";
        } else if (gold >= 1080000) {//LevelItem12=衙役,1080000
            return "衙役";
        } else if (gold >= 720000) {//LevelItem11=商人,720000
            return "商人";
        } else if (gold >= 480000) {//LevelItem10=掌柜,480000
            return "掌柜";
        } else if (gold >= 320000) {//LevelItem9=富农,320000
            return "富农";
        } else if (gold >= 208000) {//LevelItem8=中农,208000
            return "中农";
        } else if (gold >= 140000) {//LevelItem7=猎人,140000
            return "猎人";
        } else if (gold >= 80000) {//LevelItem6=渔夫,80000
            return "渔夫";
        } else if (gold >= 60000) {//LevelItem5=贫农,60000
            return "贫农";
        } else if (gold >= 40000) {//LevelItem4=佃户,40000
            return "佃户";
        } else if (gold >= 20000) {//LevelItem3=长工,20000
            return "长工";
        } else if (gold >= 10000) {//LevelItem2=短工,10000
            return "短工";
        } else {//LevelItem1=包身工
            return "包身工";
        }
    }
    public static List<long> GetChip(uint chip_infor) {
        List<long> list = new List<long>();
        if ((chip_infor & 0x01) > 0) { list.Add(1000); }
        if ((chip_infor & 0x02) > 0) { list.Add(10000); }
        if ((chip_infor & 0x04) > 0) { list.Add(50000); }
        if ((chip_infor & 0x08) > 0) { list.Add(100000); }
        if ((chip_infor & 0x10) > 0) { list.Add(500000); }
        if ((chip_infor & 0x20) > 0) { list.Add(1000000); }
        if ((chip_infor & 0x40) > 0) { list.Add(2000000); }
        if ((chip_infor & 0x80) > 0) { list.Add(5000000); }

        return list;
    }
    public static string GetGoldStr(long gold) {
        if(gold <= 1000){
            return "1k-min";
        } else if (gold <= 10000) {
            return "1w-min";
        } else if (gold <= 50000) {
            return "5w-min";
        } else if (gold <= 100000) {
            return "10w-min";
        } else if (gold <= 500000) {
            return "50w-min";
        } else if (gold <= 1000000) {
            return "100w-min";
        } else if (gold <= 2000000) {
            return "200w-min";
        } else {//if (gold <= 5000000) {
            return "500w-min";
        }
        
    }
}
