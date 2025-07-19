using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_ChooseRoom : UIItem
{
    /// <summary>
    /// 房间的图片
    /// </summary>
    public UISprite mRoomSpr;

    /// <summary>
    /// 房间类型
    /// </summary>
    public UILabel mRoomType;

    /// <summary>
    /// 房间描述
    /// </summary>
    public UILabel mRoomDesc;

    /// <summary>
    /// 进入房间需要多少钱
    /// </summary>
    public UILabel mRoomNeedGold;

    /// <summary>
    /// 当前房间服务器
    /// </summary>
    private tagGameServer curServer;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "spr_room":
                mRoomSpr = tf.GetComponent<UISprite>();
                break;
            case "title":
                mRoomType = tf.GetComponent<UILabel>();
                break;
            case "lb_room_desc":
                mRoomDesc = tf.GetComponent<UILabel>();
                break;
            case "lb_need_gold":
                mRoomNeedGold = tf.GetComponent<UILabel>();
                break;
        }
    }
    /// <summary>
    /// 设置UI信息
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="roomType"></param>
    /// <param name="rooDesc"></param>
    /// <param name="needGold"></param>
    public void SetUIInfo(tagGameServer server)
    {
        curServer = server;

        if (curServer.MinEnterScore > FQZSGameConfig.HighLevelRoom)
        {
            mRoomSpr.spriteName = "icon_gold";
            mRoomType.text = "中级房";
        }
        else
        {
            mRoomSpr.spriteName = "icon_sliver";
            mRoomType.text = "初级房";
        }

        if (curServer.MinEnterScore == 0)
            mRoomNeedGold.text = "免费进入";
        else
            mRoomNeedGold.text = string.Format("需要{0}乐豆以上", curServer.MinEnterScore);

        if (curServer.ServerName.Length > 6) {
            mRoomDesc.text = curServer.ServerName.Replace("金鲨银鲨","");
        } else {
            mRoomDesc.text = curServer.ServerName;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        if (obj == gameObject)
        {
            GameManager.EnterGame(GameEnum.FQZS, curServer);
        }
    }
}
