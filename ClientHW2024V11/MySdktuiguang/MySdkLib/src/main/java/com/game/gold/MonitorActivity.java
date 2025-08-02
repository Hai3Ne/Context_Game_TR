package com.game.gold;

import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;

import com.kwai.monitor.log.TurboAgent;
import com.kwai.monitor.log.TurboConfig;
import com.kwai.monitor.payload.TurboHelper;

public class MonitorActivity {

    public void init() {

            initSdk();

    }

    public void initSdk() {
        MainActivity.app.isInit = true;
        ApplicationInfo ai = null;
        try {
            ai = MainActivity.app.getPackageManager().getApplicationInfo(MainActivity.app.getPackageName(),
                    PackageManager.GET_META_DATA);
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }
        Bundle bundle = ai.metaData;
        String id = "";
        String name = "";
        if (bundle != null && bundle.containsKey("turboid") && bundle.containsKey("turboname")) {
            id = bundle.getInt("turboid") + "";
            name = bundle.getString("turboname");
        }

        String channel = TurboHelper.getChannel(MainActivity.app);
        TurboAgent.init(TurboConfig.TurboConfigBuilder.create(MainActivity.app)
                .setAppId(id)
                .setAppName(name)
                .setAppChannel(channel)
                .setEnableDebug(true)
                .build());
        TurboAgent.onAppActive();
    }

    public void onResume() {
        if(MainActivity.app.isInit)
            TurboAgent.onPageResume();
    }

    public void onPause() {
        if(MainActivity.app.isInit)
            TurboAgent.onPagePause();
    }

    public void messageUp(String messageKey,int messageValue) {
        try {
            switch (messageKey) {
                case "register":    // 注册账号
                    TurboAgent.onRegister();
                    break;
                case "pay": // 付费成功
                    TurboAgent.onPay(messageValue);
                    break;
                case "pageResume":  // 进入游戏界面

                    break;
                case "pagePause":   // 离开游戏界面

                    break;
                default:
                    break;
            }

        } catch (Exception e) {
            Log.d("=== Exception", e.getMessage());
        }
    }
}
