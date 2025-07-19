using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_AddRoleItem : UIItem
{
    public UISprite vip;

    public UILabel roleName;

    public RoleInfo mRoleInfo;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "item_spr_vip":
                vip = tf.GetComponent<UISprite>();
                break;
            case "item_lb_name":
                roleName = tf.GetComponent<UILabel>();
                break;
        }
    }

    public void SetInfo(RoleInfo roleinfo)
    {
        string nickName = roleinfo.NickName;
        if (nickName.Length > 6)
        {
            nickName = nickName.Substring(0, 6) + "...";
        }
        mRoleInfo = roleinfo;
        vip.spriteName = SHGameConfig.VIPIcons[roleinfo.MemberOrder];
        roleName.text = nickName;
    }
}
