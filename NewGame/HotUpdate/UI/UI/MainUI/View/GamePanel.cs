using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private GameItem item1;
    [SerializeField] private GameItem item2;

    public GameItem GameItem2() 
    {
        return item2;
    }
    public void SetUpPanel(List<cfg.Game.GameRoomConfig> list ) 
    {
        var data1 = list[0];
        var data2 = list[1];

       /* var Id = data.Id.Split('|');
        var roomid = data.RoomID.Split('|');
        var bannerBg = data.Bannerbg.Split('|');
        var str = data.PeopleNumPos.Replace("?", "");
        var strSplit = str.Split('|');
        var jxPos0 = new Vector3(float.Parse(strSplit[0].Split(',')[0]), float.Parse(strSplit[0].Split(',')[1]), 0f);
        var jxPos1 = new Vector3(float.Parse(strSplit[1].Split(',')[0]), float.Parse(strSplit[0].Split(',')[1]), 0f);*/
        item1.SetUpItem1(data1);

        item2.SetUpItem1(data2);
    }
}
