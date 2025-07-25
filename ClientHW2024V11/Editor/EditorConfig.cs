using UnityEngine;
using System.Collections;

public class EditorConfig  {
 

    //不想压缩的图集名称 
    public static  string []  filterAtlasNames=
    {   
        "headIcons", 
        //"MatchBattle",

    };
    public static string[] IOSFilterAtlasNames =
    {   
        "headIcons", 
        //"PanelFightUI",

    };


    public static  bool hasFilter(string str,bool isIOS)
    {
        bool re = false;
        string[] filterGroup = isIOS ? IOSFilterAtlasNames : filterAtlasNames;
        for (int i = 0; i < filterGroup.Length; i++)
        {
            if (str.Contains(filterGroup[i]))
            {
                re = true;
                break; 
            }
        }
        return re; 
    }

}
