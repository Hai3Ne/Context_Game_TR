using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager {
    public static bool IsBingWX;//是否绑定了微信
    public static bool IsShare;//是否已经分享过了

    public static void GoLogin() {
        MainEntrace.Instance.ClearTick();
        ShopManager.mIsShowFrist = false;
        UserManager.IsBingWX = false;
        UserManager.IsShare = false;
        UI.ExitAllUI();
        ////删除当前登录数据
        //PlayerPrefs.DeleteKey("wx_openid");
        //PlayerPrefs.DeleteKey("wx_access_token");
        //PlayerPrefs.DeleteKey("wx_refresh_token");
        VersionManager.CheckVersion((is_update) => {
            if (is_update) {
                VersionManager.ShowVersionTick();
            }
        });
        GameConfig.InsurePassword = string.Empty;
        GameConfig.OP_AutoLoginBank = false;
        MainEntrace.Instance.OpenLogin();
    }
}
