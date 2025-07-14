using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FQZSGameConfig  {

    private static byte[] versions = new byte[] { 6, 13, 0, 0 };//飞禽走兽版本号
    public const ushort KindID = 601;//类型索引
    public const uint ClientGameID = 601;//神兽游戏ID
    public const int MaxAreaCount = 11;//最大区域个数

    public const int HighLevelRoom = 100000;//中级房间进入的条件

    public const string SOUND_WARING = "IDC_SND_WARING";
    public const string SOUND_RUN = "IDC_SND_RUN";
    public const string SOUND_START = "IDC_SND_START";
    public const string SOUND_CHANGE_BANKER = "IDC_SND_CHANGE_BANKER";
    public const string SOUND_ADDCHIP = "IDC_SND_ADDCHIP";
    public const string SOUND_LOST = "IDC_END_LOST";
    public const string SOUND_WIN = "IDC_SND_WIN";
    public const string SOUND_BGM = "IDC_BACK_GROUND";

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
    public static string VersionStr
    {
        get
        {
            return string.Format("{0}.{1}.{2}.{3}", versions[0], versions[1], versions[2], versions[3]);
        }
    }
    public static uint VersionCode
    {
        get
        {
            return GameUtils.ConvertToVersion(versions[0], versions[1], versions[2], versions[3]);
        }
    }

    public static string GetLvStr(long gold)
    {//根据乐豆 获取用户级别
        if (gold >= 1000000000)
        {//LevelItem23=帝王,1000000000
            return "帝王";
        }
        else if (gold >= 500000000)
        {//LevelItem22=丞相,500000000
            return "丞相";
        }
        else if (gold >= 300000000)
        {//LevelItem21=巡抚,300000000
            return "巡抚";
        }
        else if (gold >= 100000000)
        {//LevelItem20=总督,100000000
            return "总督";
        }
        else if (gold >= 40000000)
        {//LevelItem19=知府,40000000
            return "知府";
        }
        else if (gold >= 24000000)
        {//LevelItem18=通判,24000000 
            return "通判";
        }
        else if (gold >= 13600000)
        {//LevelItem17=知县,13600000 
            return "知县";
        }
        else if (gold >= 8000000)
        {//LevelItem16=大地主,8000000
            return "大地主";
        }
        else if (gold >= 4800000)
        {//LevelItem15=小地主,4800000
            return "小地主";
        }
        else if (gold >= 2720000)
        {//LevelItem14=大财主,2720000
            return "大财主";
        }
        else if (gold >= 1600000)
        {//LevelItem13=小财主,1600000
            return "小财主";
        }
        else if (gold >= 1080000)
        {//LevelItem12=衙役,1080000
            return "衙役";
        }
        else if (gold >= 720000)
        {//LevelItem11=商人,720000
            return "商人";
        }
        else if (gold >= 480000)
        {//LevelItem10=掌柜,480000
            return "掌柜";
        }
        else if (gold >= 320000)
        {//LevelItem9=富农,320000
            return "富农";
        }
        else if (gold >= 208000)
        {//LevelItem8=中农,208000
            return "中农";
        }
        else if (gold >= 140000)
        {//LevelItem7=猎人,140000
            return "猎人";
        }
        else if (gold >= 80000)
        {//LevelItem6=渔夫,80000
            return "渔夫";
        }
        else if (gold >= 60000)
        {//LevelItem5=贫农,60000
            return "贫农";
        }
        else if (gold >= 40000)
        {//LevelItem4=佃户,40000
            return "佃户";
        }
        else if (gold >= 20000)
        {//LevelItem3=长工,20000
            return "长工";
        }
        else if (gold >= 10000)
        {//LevelItem2=短工,10000
            return "短工";
        }
        else
        {//LevelItem1=包身工
            return "包身工";
        }
    }
}
