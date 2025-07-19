using UnityEngine;
using System.Collections;

/// <summary>
/// 奖卷相关
/// </summary>
public class Item_Battle_Lottery : MonoBehaviour {
    public static Item_Battle_Lottery item_lottery;
    public uint LotteryTickItemID = 12343u;

    public GameObject btnCj;
    public GameObject btnDraw;
    public UISprite bgSp;
    public UISlider slider;
    public UILabel txtJifen;
    public UILabel txtInfo;
    public UISprite mSprTickNum;//奖卷数量提示背景
    public UILabel labNumber;

    public bool isExpose = false;
    public bool isTweening = false;
    public uint max_count;//最大数量

    public void Awake() {
        Item_Battle_Lottery.item_lottery = this;
        RoleItemModel.Instance.RegisterGlobalMsg(SysEventType.ItemInfoChange, this.RefershNum, false);
    }

    public void OnDestroy() {
        RoleItemModel.Instance.UnRegisterGlobalMsg(SysEventType.ItemInfoChange, this.RefershNum);
    }

    public void InitData(LotteryVo vo) {
        GameUtils.Traversal(this.transform, this.OnNodeAsset);

        this.LotteryTickItemID = vo.LotteryTicketID;

        this.bgSp.alpha = 0;
        this.bgSp.width = 144;
        UIEventListener.Get(this.btnCj).onClick = this.OnClickBtnCJ;
        UIEventListener.Get(this.btnDraw).onClick = (button) => {
            //MVCFacade: Open("LotteryDraw");
            WndManager.Instance.ShowUI(EnumUI.UI_LotteryDraw);
        };

        this.RefershNum(null);
        this.max_count = FishConfig.Instance.GetItemMaxCount(LotteryTickItemID);
        this.ReqLotteryTicks();
    }

    public void RefershNum(object obj) {
        int cnt = RoleItemModel.Instance.getItemCount(LotteryTickItemID);

        if (cnt >= this.max_count) {
            this.labNumber.text = this.max_count.ToString();
            this.txtInfo.text = "当前奖券数量已达上限";
        } else {
            this.labNumber.text = cnt.ToString();
            this.txtInfo.text = "奖券产出进度";
        }
        if (cnt > 0) {
            this.mSprTickNum.gameObject.SetActive(true);
        } else {
            this.mSprTickNum.gameObject.SetActive(false);
        }
    }

    public void ReqLotteryTicks() {
        CS_GR_LotteryTicket regReq = new CS_GR_LotteryTicket();
        regReq.SetCmdType(NetCmdType.SUB_C_LOTTERY_TICKET);
        NetServices.Instance.Send<CS_GR_LotteryTicket>(regReq);
    }

    public void OnClickBtnCJ(GameObject button) {
        if (this.isTweening) {
            return;
        }

        TweenWidth tw;
        if (this.isExpose) {
            tw = TweenWidth.Begin(this.bgSp, 0.3f, 144);
            TweenAlpha.Begin(this.bgSp.gameObject, 0.3f, 0);
            this.isExpose = false;
        } else {
            tw = TweenWidth.Begin(this.bgSp, 0.3f, 445);
            TweenAlpha.Begin(this.bgSp.gameObject, 0.3f, 1);
            this.isExpose = true;

            this.ReqLotteryTicks();
            TimeManager.DelayExec(3, () => {
                if (this.isExpose) {
                    this.OnClickBtnCJ(button);
                }
            });
        }

        this.isTweening = true;
        tw.SetOnFinished(() => {
            this.isTweening = false;
        });
    }

    public void OnLotteryTickUpdate(SC_GR_LotteryTicket cmd) {
        this.slider.value = cmd.TicketVolume * 1f / cmd.MaxVolume - (cmd.TicketVolume / cmd.MaxVolume);
        this.txtJifen.text = cmd.TicketVolume + "/" + cmd.MaxVolume;
    }

    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "btn_cj":
                this.btnCj = tf.gameObject;
                break;
            case "btn_draw":
                this.btnDraw = tf.gameObject;
                break;
            case "btn_bg":
                this.bgSp = tf.GetComponent<UISprite>();
                break;
            case "slider_jindu":
                this.slider = tf.GetComponent<UISlider>();
                break;
            case "lb_jifen":
                this.txtJifen = tf.GetComponent<UILabel>();
                break;
            case "lb_lottery_tick":
                this.txtInfo = tf.GetComponent<UILabel>();
                break;
            case "lb_lottery_num":
                this.labNumber = tf.GetComponent<UILabel>();
                break;
            case "TickNumSp":
                this.mSprTickNum = tf.GetComponent<UISprite>();
                break;
        }
    }
}
