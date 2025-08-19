package  com.dwzy.xkmm.wxapi;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;

import com.game.gold.Constants;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler {

    private IWXAPI api;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        api = WXAPIFactory.createWXAPI(this, Constants.APP_ID,false);
        api.handleIntent(getIntent(), this);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        api.handleIntent(intent, this);
    }

    @Override
    public void onReq(BaseReq baseReq) {
        //...
    }

    @Override
    public void onResp(BaseResp baseResp) {
        if (baseResp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {
            if(baseResp.errCode == 0)
            {
                // PaySuccess(this);	// 可在此处，添加应用自己的支付结果统计相关逻辑
                UnityPlayer.UnitySendMessage("SdkCtrl","OnPaySucess","WX");
            }
            else if(baseResp.errCode == -2){
              //  Toast.makeText(this,"用户取消支付",Toast.LENGTH_SHORT).show();

            }
            else{
               // Toast.makeText(this,"支付失败，其他异常情形",Toast.LENGTH_SHORT).show();
                UnityPlayer.UnitySendMessage("SdkCtrl","OnPayFail","WX");
            }

        }
        finish();
    }
}