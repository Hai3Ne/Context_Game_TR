using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Qiandao : UILayer {
    public Item_QianDao[] mItems = new Item_QianDao[7];
    //public Item_QianDao mVIPItem;
    public GameObject dialog_rule;
    //public UILabel mLbRuleInfo;

    private int mCurWeek;
    public void InitData(CMD_GP_SignInfo sign_info) {
        if (sign_info.CurWeekDay == byte.MaxValue) {
            //UI.EnterUI<UI_BindNotice>(ui => ui.InitData(false));
            UI.EnterUI<UI_BindNotice>(GameEnum.All).InitData(false);
            this.Close();
            return;
        }
        //bool vip_get = Random.Range(0, 100) < 50;
        this.mCurWeek = sign_info.CurWeekDay;
        if (this.mCurWeek == 0) {
            this.mCurWeek = 7;
        }

        for (int i = 0; i < this.mItems.Length; i++) {
            this.mItems[i].InitData(sign_info.SignInfo[i].Score, sign_info.SignInfo[i].MemberOrder, this.mCurWeek == i + 1);
            this.mItems[i].SetShowGetTick(sign_info.SignInfo[i].Signed == 1);
        }
        this.SetShowRule(false);
        //this.mLbRuleInfo.text = StringTable.GetString("Tip_35");
    }
    public void Sign(byte day) {//当日签到
        if (day == 0) {
            day = 7;
        }
        if (this.mCurWeek != day) {
            this.mCurWeek = day;
            for (int i = 0; i < this.mItems.Length; i++) {
                this.mItems[i].SetCur(this.mCurWeek == i + 1);
            }
        }
        //this.mVIPItem.SetShowGetTick(is_vip);
        this.mItems[this.mCurWeek - 1].SetShowGetTick(true);
        SignManager.IsSign = false;
    }

    public void SetShowRule(bool is_show) {
        this.dialog_rule.SetActive(is_show);
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GP_WEEKSIGNINFO://签到列表
                this.InitData(pack.ToObj<CMD_GP_SignInfo>());
                this.gameObject.SetActive(true);
                break;
            case NetCmdType.SUB_GP_WEEKSIGN_RET://签到结果
                CMD_GP_Sign_Ret result = pack.ToObj<CMD_GP_Sign_Ret>();
                if (result.ResultInfo.ErrorCode == 0) {
                    this.Sign(result.SignDay);
                } else {
                    SystemMessageMgr.Instance.ShowMessageBox(result.ResultInfo.ErrorString);
                }
                break;
        }
    }
    
    public override void OnNodeLoad() {
        this.gameObject.SetActive(false);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_WEEKSIGNINFO, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_WEEKSIGN_RET, this.OnNetHandle);

        //获取签到信息
        HttpServer.Instance.Send<CMD_GP_QueryWeekSign>(NetCmdType.SUB_GP_QUERYWEEKSIGN, new CMD_GP_QueryWeekSign {
            UserID = HallHandle.UserID,
        });
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_WEEKSIGNINFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_WEEKSIGN_RET, this.OnNetHandle);

        MainEntrace.Instance.EnterNextTick();
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close":
                this.Close();
                break;
            case "btn_rule":
                this.SetShowRule(true);
                break;
            case "item_btn_close":
                this.SetShowRule(false);
                break;
            case "btn_share":
                //UI.EnterUI<UI_Share>(ui => ui.InitData(false));
                UI.EnterUI<UI_Share>(GameEnum.All).InitData(false);
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_1":
                this.mItems[0] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_2":
                this.mItems[1] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_3":
                this.mItems[2] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_4":
                this.mItems[3] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_5":
                this.mItems[4] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_6":
                this.mItems[5] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            case "item_7":
                this.mItems[6] = this.BindItem<Item_QianDao>(tf.gameObject);
                break;
            //case "item_vip":
            //    this.mVIPItem = this.BindItem<Item_QianDao>(tf.gameObject);
            //    break;
            case "dialog_rule":
                this.dialog_rule = tf.gameObject;
                break;
        }
    }
}
