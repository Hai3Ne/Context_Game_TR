using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_Quit : UILayer
{
    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_change_table":
                ChangeTable();
                break;
            case "btn_back_hall":
                NetClient.Send(NetCmdType.SUB_GR_USER_STANDUP, new CS_UserStandUp
                {
                    ForceLeave = 1,
                    TableID = RoleManager.Self.TableID,
                    ChairID = RoleManager.Self.ChairSeat,
                });
                break;
            case "btn_to_game":
                Close();
                break;
        }
    }

    public void ChangeTable()
    {
        LKGameManager.mIsChangeTable = true;
        UI_LK_Battle.ui.SetAutoFire(false);
        UI_LK_Battle.ui.SetAutoLock(false);
        NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown
        {
            TableID = ushort.MaxValue,
            ChairID = ushort.MaxValue,
            Password = string.Empty,
        });
        Close();
    }
}
