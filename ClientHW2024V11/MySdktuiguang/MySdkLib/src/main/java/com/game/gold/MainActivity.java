package com.game.gold;

import android.Manifest;
import android.annotation.SuppressLint;
import android.app.AlertDialog;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.Settings;
import android.telephony.TelephonyManager;
import android.text.TextUtils;
import android.text.method.LinkMovementMethod;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import com.alipay.sdk.app.PayTask;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelpay.PayReq;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

import java.util.Map;

public class MainActivity extends UnityPlayerActivity implements com.game.gold.DemoHelper.AppIdsUpdater{
    public IWXAPI wxapi = null;
    static MainActivity app = null;
    public MonitorActivity monitorActivity;
    public DouyinActivity douyinActivity;
    public boolean isInit = false;

    public int gameType = 0;
    public int channel = 0;
    public String oaid = "";

    public Boolean isCheck = false;
    public Handler m_Handler = null;
    public DemoHelper demoHelper = null;
    String lib = "msaoaidsec";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        app = this;
        m_Handler = new Handler();

        if( Build.VERSION.SDK_INT  >= 29){
            demoHelper = new DemoHelper(this, lib);
            demoHelper.getDeviceIds(MainActivity.app);
        }
        if (wxapi==null){
            wxapi = WXAPIFactory.createWXAPI(this,Constants.APP_ID,false);
        }
        wxapi.registerApp(Constants.APP_ID);

        ApplicationInfo ai = null;
        try {
            ai = this.getPackageManager().getApplicationInfo(this.getPackageName(),
                    PackageManager.GET_META_DATA);
            Bundle bundle = ai.metaData;
            if (bundle != null && bundle.containsKey("sdkType")) {
                Log.e("messageUp = ",  bundle.getInt("sdkType") + "");
                gameType = bundle.getInt("sdkType");
            }
            if (bundle != null && bundle.containsKey("sdkChannel")) {
                channel = bundle.getInt("sdkChannel");
            }
            if (bundle != null && bundle.containsKey("Check")) {
                isCheck = bundle.getBoolean("Check");
            }
        } catch (PackageManager.NameNotFoundException e) {
            e.printStackTrace();
        }

        initSdk();
    }
    public void initSdk(){
        if(checkPermission()){
            if(gameType == 0){
                return;
            }

            if(gameType == 1){
                monitorActivity = new MonitorActivity();
                monitorActivity.init();
            }else if(gameType == 2){
                douyinActivity = new DouyinActivity();
                douyinActivity.init();
            }

 /*           try {
                if (Integer.valueOf(Build.VERSION.RELEASE).intValue() >= 10) {
                    UMConfigure.preInit(app, "65a8c272a7208a5af19fac02", appChannel);
                    UMConfigure.getOaid(app, new OnGetOaidListener() { // from class: org.cocos2dx.javascript.AppActivity.1
                        @Override // com.umeng.commonsdk.listener.OnGetOaidListener
                        public void onGetOaid(String str) {
                            Log.e("ContentValues", "YMSDK get oaid: " + str);
                            oaid = str;
                        }
                    });
                }
            } catch (Exception e) {
                Log.d("=== Exception", e.getMessage());
            }*/

        }
    }

    public void perMission() {
        if (!checkPermission()) {
            Log.e("perMission1111",   "perMission111111");
            AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.this);
            LayoutInflater inflater = (LayoutInflater) getSystemService(LAYOUT_INFLATER_SERVICE);
            View view = inflater.inflate(R.layout.dalogtipslayout, null);
            Button cancelButton = view.findViewById(R.id.btn_alert_cancel);
            cancelButton.setText("取消");
            Button okButton = view.findViewById(R.id.btn_alert_ok);
            okButton.setText("去授权");
            TextView title =  view.findViewById(R.id.tv_alert_title);
            title.setText("权限申请");
            TextView content =  view.findViewById(R.id.tv_alert_content);
            content.setMovementMethod(LinkMovementMethod.getInstance());
            content.setText("我们将获取您设备的部分权限用于数据统计。请放心，我们保证权限仅用于必要的功能。");
            builder.setView(view);
            final AlertDialog dialog = builder.create();
            cancelButton.setOnClickListener(v -> {
                dialog.hide();
                dialog.dismiss();
            });
            okButton.setOnClickListener(v -> {

                dialog.hide();
                dialog.dismiss();
                requestPermissions();
            });
            dialog.setCancelable(false);
            dialog.show();

        }
    }

    public int getChannle(){
        return channel;
    }

    public String getDeviceIds(){
        if(Build.VERSION.SDK_INT  >= 29){
            return oaid;
        }else{
            String mei = GetIMEI();
            Log.d("UNITY IMEI", mei);
            return mei;
        }
    }
    public String GetIMEI() {
        String str;
        Context applicationContext = getApplicationContext();
        TelephonyManager telephonyManager = (TelephonyManager) applicationContext.getSystemService(Context.TELEPHONY_SERVICE);
        if (Build.VERSION.SDK_INT <= 28) {
            str = telephonyManager.getDeviceId();
            if (TextUtils.isEmpty(str) || str.equals("0")) {
                str = getIMEI(0);
            }
            if (TextUtils.isEmpty(str) || str.equals("0")) {
                str = getIMEI(1);
            }
        } else {
            str = "";
        }
        return str;
    }

    public String getIMEI(int i) {
        try {
            TelephonyManager telephonyManager = (TelephonyManager) app.getSystemService(Context.TELEPHONY_SERVICE);
            return (String) telephonyManager.getClass().getMethod("getImei", Integer.TYPE).invoke(telephonyManager, Integer.valueOf(i));
        } catch (Exception unused) {
            return "";
        }
    }

    public boolean checkPermission(){
        if( Build.VERSION.SDK_INT  >= 29){
            return true;
        }else {
            if(checkSelfPermission(Manifest.permission.READ_PHONE_STATE) == PackageManager.PERMISSION_GRANTED){
                return true;
            }
            return false;
        }
    }

    public void Login(){
        SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo";
        req.state = "demony";
        wxapi.sendReq(req);
    }
    public void requestPermissions(){
        requestPermissions(new String[]{ Manifest.permission.READ_PHONE_STATE}, 1);
    }

    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults){
        switch (requestCode){
            case 1:{
                initSdk();
            }
            default:
                break;
        }
    }

    public void onResume() {
        super.onResume();
        if(monitorActivity != null){
            monitorActivity.onResume();
        }
        if(douyinActivity != null){
            douyinActivity.onResume();
        }
    }

    public void onPause() {
        super.onPause();
        if(monitorActivity != null){
            monitorActivity.onPause();
        }
        if(douyinActivity != null){
            douyinActivity.onPause();
        }
    }

    public void messageUp(String messageKey,int messageValue) {
        if (!isInit) return;  // 判断sdk是否初始化完成


        if(douyinActivity != null){
            douyinActivity.messageUp(messageKey,messageValue);
        }

        if(monitorActivity != null){
            monitorActivity.messageUp(messageKey,messageValue);
        }

    }

    public boolean isCanLogin(){
        if(isCheck){
            return true;
        }
        if(hasSimCard() == 0 || EmutorUtils.isEmulator(MainActivity.app)){
            return false;
        }
        return true;
    }
    public  boolean isOpenDevelopmentSetting() {
        boolean enableAdb = Settings.Secure.getInt(app.getContentResolver(), Settings.Global.DEVELOPMENT_SETTINGS_ENABLED, 0) != 0;
        return enableAdb;
    }

    public  boolean isUSBDebugSetting(){
        boolean enableAdb = Settings.Secure.getInt(app.getContentResolver(), Settings.Global.ADB_ENABLED, 0) != 0;
        return enableAdb;
    }

    public int checkUsb(){
        if(isCheck){
            return 0;
        }
        if(isOpenDevelopmentSetting() || isUSBDebugSetting()){
            AlertDialog.Builder builder = new AlertDialog.Builder(MainActivity.this);
            LayoutInflater inflater = (LayoutInflater) getSystemService(LAYOUT_INFLATER_SERVICE);
            View view = inflater.inflate(R.layout.dalogtipslayout, null);

            Button cancelButton = view.findViewById(R.id.btn_alert_cancel);
            cancelButton.setText("取消");
            Button okButton = view.findViewById(R.id.btn_alert_ok);
            okButton.setText("确定");
            TextView title =  view.findViewById(R.id.tv_alert_title);
            title.setText("提示");
            TextView content =  view.findViewById(R.id.tv_alert_content);
            content.setMovementMethod(LinkMovementMethod.getInstance());
            content.setText("检测到您手机打开了开发者模式，请关闭该模式及USB调试后继续使用本软件");
            builder.setView(view);
            final AlertDialog dialog = builder.create();
            cancelButton.setOnClickListener(v -> {
                dialog.hide();
                dialog.dismiss();
            });
            okButton.setOnClickListener(v -> {
                dialog.hide();
                dialog.dismiss();
                Intent intent =  new Intent(Settings.ACTION_APPLICATION_DEVELOPMENT_SETTINGS);
                startActivity(intent);
            });
            dialog.setCancelable(false);
            dialog.show();
            return 1;
        }
        return 0;
    }

    public int hasSimCard() {
        TelephonyManager telMgr = (TelephonyManager) app.getSystemService(Context.TELEPHONY_SERVICE);
        int simState = telMgr.getSimState();
        int result = 1;
        if (simState == TelephonyManager.SIM_STATE_ABSENT) {
            result = 0;
        }else if(simState == TelephonyManager.SIM_STATE_UNKNOWN) {
            result = 0;
        }
        Log.d("resultCard = " ,result + "");
        return result;
    }

    public  void AliPay(final String orderInfo) {
        Log.i("Unity", " >>> enter alipay native");
        Runnable payRun = new Runnable() {
            @Override
            public void run() {
                PayTask task=new PayTask(MainActivity.this);
                String result= task.pay(orderInfo, true);
                Log.i("Unity", "onALIPayFinish, result = " + result);
                // 这里可以自己添加Unity回调接收
                // UnityPlayer.UnitySendMessage(callBackObjectName, CallBackFuncName, result);
            }
        };
        Thread payThread = new Thread(payRun);
        payThread.start();
    }
    public void WeChatPayReq(String APP_ID,String MCH_ID,String prepayid,String packageValue,String noncestr,String timestamp,String sign) {
        PayReq req = new PayReq();
        req.appId = APP_ID;
        req.partnerId = MCH_ID;
        req.prepayId = prepayid;
        req.packageValue =packageValue;
        req.nonceStr = noncestr;
        req.timeStamp = timestamp;
        req.sign = sign;
        wxapi.sendReq(req);
        Log.d("unity", "WeChatPayReq");
    }

    @SuppressLint("HandlerLeak")
    private static Handler mHandler = new Handler() {
        @SuppressWarnings("unused")
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case 1: {
                    @SuppressWarnings("unchecked")
                    PayResult payResult = new PayResult((Map<String, String>) msg.obj);
                    /**
                     对于支付结果，请商户依赖服务端的异步通知结果。同步通知结果，仅作为支付结束的通知。
                     */
                    String resultInfo = payResult.getResult();// 同步返回需要验证的信息
                    String resultStatus = payResult.getResultStatus();
                    // 判断resultStatus 为9000则代表支付成功
                    if (TextUtils.equals(resultStatus, "9000")) {
                        UnityPlayer.UnitySendMessage("SdkCtrl", "OnPaySucess", "Ali");
                    } else {
                        UnityPlayer.UnitySendMessage("SdkCtrl", "OnPayFail", "Ali");
                    }
                    break;
                }
                default:
                    break;
            }
        }
    };
    @Override
    public void onIdsValid(String ids) {
        runOnUiThread(() -> {
            oaid = ids;
        });
    }
}

