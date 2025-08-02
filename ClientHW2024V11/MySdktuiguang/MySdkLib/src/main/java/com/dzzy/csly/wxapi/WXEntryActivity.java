package   com.dzzy.csly.wxapi;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;

import com.game.gold.Constants;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {

    public IWXAPI wxapi = null;
    static IWXAPIEventHandler app = null;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        app = this;
        //初始化wxapi
        Log.d("unity", "onReceive: ");
        wxapi = WXAPIFactory.createWXAPI(this, Constants.APP_ID,false);
        wxapi.handleIntent(getIntent(),this );
    }

    @Override
    public void onReq(BaseReq baseReq) {

    }

    @Override
    public void onResp(BaseResp baseResp) {
        Log.d("unity", "onReceive: " + baseResp.getType());
        Log.d("unity", "onReceive1: " + baseResp.errCode);
        if (baseResp.getType()==1){//微信登录
            if (baseResp.errCode==BaseResp.ErrCode.ERR_OK){//用户同意
                SendAuth.Resp authResp = (SendAuth.Resp)baseResp;
                final String code = authResp.code;
                UnityPlayer.UnitySendMessage("SdkCtrl","OnWxLoginSucess",code);
            }
            else{
                UnityPlayer.UnitySendMessage("SdkCtrl","OnWxLoginFail",((SendAuth.Resp)baseResp).code);
            }
        }


        finish();
    }
}