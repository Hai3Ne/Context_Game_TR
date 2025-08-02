package com.game.gold;

import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;

import com.bytedance.ads.convert.hume.readapk.HumeSDK;
import com.bytedance.applog.AppLog;
import com.bytedance.applog.InitConfig;
import com.bytedance.applog.game.GameReportHelper;
import com.bytedance.applog.util.UriConstants;

import org.json.JSONException;
import org.json.JSONObject;

public class DouyinActivity {

    public void init() {
        initSdk();
    }

    public void AddMustLog(String event) {
        JSONObject paramsObj = new JSONObject();
        try {
            Log.e("DouyinActivity======","AddMustLog: " + event);
            paramsObj.put("origin_event", event); // 添加原始事件名称参数
        } catch (JSONException e) {
            e.printStackTrace();
        }
        AppLog.onEventV3("game_addiction", paramsObj);
    }

    public static void AddFreeLog(String event, String concent) {
        if (concent == "") {
            concent = "be_null";
        }
        JSONObject paramsObj = new JSONObject();
        try {
            Log.e("DouyinActivity======","AddFreeLog: " + event + "--" + concent);
            paramsObj.put(event, concent); //事件属性 播放时⻓长
        } catch (JSONException e) {
            e.printStackTrace();
        }
        AppLog.onEventV3(event, paramsObj);
    }

    public  String getTTChannel() {
        String ttChannel = "";
        try {
            ttChannel = HumeSDK.getChannel(MainActivity.app);
            if (ttChannel == "" || ttChannel == null) {
                ttChannel = "byteDance";
            }
        }catch (Exception e) {
            ttChannel = "byteDance";
        }
        return ttChannel;
    }

    public void initSdk() {
        // 抖音sdk初始化
        ApplicationInfo ai = null;
        try {
            ai = MainActivity.app.getPackageManager().getApplicationInfo(MainActivity.app.getPackageName(),
                    PackageManager.GET_META_DATA);
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }
        Bundle bundle = ai.metaData;
        String id = "";
        if (bundle != null && bundle.containsKey("dyid") ) {
            id = bundle.getInt("dyid") + "";
        }
        String APPID = id;
        String CHANNEL = this.getTTChannel();
        Log.e("DouyinActivity======","bytedance channel :" + CHANNEL);
        final InitConfig config = new InitConfig(APPID , CHANNEL);
        // 设置数据上送地址
        config.setUriConfig(UriConstants.DEFAULT);
        config.setImeiEnable(false);//建议关停获取IMEI（出于合规考虑）
        config.setAutoTrackEnabled(true); // 全埋点开关，true开启，false关闭
        config.setLogEnable(false); // true:开启日志，参考4.3节设置logger，false:关闭日志
        config.setEnablePlay(true); // 心跳事件

        AppLog.setEncryptAndCompress(true); // 加密开关，true开启，false关闭
        AppLog.init(MainActivity.app,config,MainActivity.app);
        Log.e("DouyinActivity======","bytedance init over");
        this.AddMustLog("active");
        this.AddFreeLog("active", "");
        MainActivity.app.isInit = true;
    }
    public void onResume() {
        AppLog.onResume(MainActivity.app);
    }
    public void onPause() {
        AppLog.onPause(MainActivity.app);
    }
    public void createAccount() {
        GameReportHelper.onEventRegister("wechat",true);
        this.AddMustLog("active_register");
        this.AddFreeLog("active_register", "wechat");
    }
    public void pay(int value) {
        GameReportHelper.onEventPurchase("gift","goods", "1",1,"wechat","¥", true, value);
        this.AddMustLog("active_pay");
        this.AddFreeLog("active_pay", "" + value);
    }
    public void messageUp(String messageKey,int messageValue) {
        try {
            switch (messageKey) {
                case "register":    // 注册账号
                    createAccount();
                    break;
                case "pay": // 付费成功
                    pay(messageValue);
                    break;
                case "pageResume":  // 进入游戏界面
                    onResume();
                    break;
                case "pagePause":   // 离开游戏界面
                    onPause();
                    break;
                default:
                    break;
            }
        } catch (Exception e) {
            Log.d("=== Exception", e.getMessage());
        }
    }
}
