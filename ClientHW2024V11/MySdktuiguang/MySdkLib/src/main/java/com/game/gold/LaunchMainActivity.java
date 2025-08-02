package com.game.gold;

import static android.text.Spanned.SPAN_EXCLUSIVE_EXCLUSIVE;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.text.SpannableString;
import android.text.method.LinkMovementMethod;
import android.text.style.URLSpan;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

public class LaunchMainActivity extends Activity {
    public Boolean anInt = false;
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        findViewById(android.R.id.content).setBackgroundResource(R.drawable.splash);

        SharedPreferences base = getSharedPreferences("base",MODE_PRIVATE);
        anInt = base.getBoolean("isFirstStart",true);
        if(anInt == true){
            changeGame();
        }else{
            StarGame();
        }
    }
    private void changeGame(){
        final SpannableString s = new SpannableString("\n" +
                "   欢迎来到本游戏,我们非常重视您的个人隐私信息保护,在您游戏"  +
                "前，请阅读并同意我们的《用户服务协议》和《隐私协议》.在您同"  +
                "意App隐私政策后，我们将进行集成SDK的初始化工作，会收集您的"  +
                "device_id,MAC地址,OAID和android_id以保障APP正常使用!"
        );
        URLSpan urlSpan = new URLSpan("https://apiv8.zhongheboy.cn/user.html");
        URLSpan urlSpan1 = new URLSpan("https://apiv8.zhongheboy.cn/user.html");
        s.setSpan(urlSpan, 44, 52, SPAN_EXCLUSIVE_EXCLUSIVE);
        s.setSpan(urlSpan1, 53, 59, SPAN_EXCLUSIVE_EXCLUSIVE);


        AlertDialog.Builder builder = new AlertDialog.Builder(LaunchMainActivity.this);
        LayoutInflater inflater = (LayoutInflater) getSystemService(LAYOUT_INFLATER_SERVICE);
        View view = inflater.inflate(R.layout.dalogtipslayout, null);
        Button cancelButton = view.findViewById(R.id.btn_alert_cancel);
        cancelButton.setText("拒绝");
        Button okButton = view.findViewById(R.id.btn_alert_ok);
        okButton.setText("同意");
        TextView title =  view.findViewById(R.id.tv_alert_title);
        title.setText(R.string.app_name);
        TextView content =  view.findViewById(R.id.tv_alert_content);
        content.setMovementMethod(LinkMovementMethod.getInstance());
        content.setText(s);
        builder.setView(view);
        final AlertDialog dialog = builder.create();
        cancelButton.setOnClickListener(v -> {
            dialog.hide();
            dialog.dismiss();
            ExitGame();
        });

        okButton.setOnClickListener(v -> {
            dialog.hide();
            dialog.dismiss();
            StarGame();
        });


        dialog.setCancelable(false);
        dialog.show();
    }

    private void ExitGame(){

        AlertDialog.Builder builder = new AlertDialog.Builder(LaunchMainActivity.this);
        LayoutInflater inflater = (LayoutInflater) getSystemService(LAYOUT_INFLATER_SERVICE);
        View view = inflater.inflate(R.layout.dalogtipslayout, null);
        Button cancelButton = view.findViewById(R.id.btn_alert_cancel);
        cancelButton.setText("退出游戏");
        Button okButton = view.findViewById(R.id.btn_alert_ok);
        okButton.setText("重新选择");
        TextView title =  view.findViewById(R.id.tv_alert_title);
        title.setText(R.string.app_name);
        TextView content =  view.findViewById(R.id.tv_alert_content);
        content.setMovementMethod(LinkMovementMethod.getInstance());
        content.setText("同意《用户服务协议》和《隐私协议》后才能正常进入游戏,确定要退出游戏吗?");
        builder.setView(view);
        final AlertDialog dialog = builder.create();
        cancelButton.setOnClickListener(v -> {
            dialog.hide();
            dialog.dismiss();
            android.os.Process.killProcess(android.os.Process.myPid());
        });

        okButton.setOnClickListener(v -> {
            dialog.hide();
            dialog.dismiss();
            changeGame();
        });


        dialog.setCancelable(false);
        dialog.show();
    }
    private void StarGame(){
        SharedPreferences base = getSharedPreferences("base",MODE_PRIVATE);
        base.edit().putBoolean("isFirstStart", false).apply();
        Intent intent = new Intent();
        intent.setClass(LaunchMainActivity.this,MainActivity.class);
        startActivity(intent);
    }

}
