using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_BoxRankAwardController : IUIControllerImp {
    private UI_BoxRankAwardRef mUIRef;
    public SC_GR_WorldBossGrant resp_grant;//发放奖励信息

    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }

    public override void Init(object data) {
        base.Init(data);

        this.resp_grant = data as SC_GR_WorldBossGrant;
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_BoxRankAward",
			SceneObjMgr.Instance.UIPanelTransform,
			delegate(GameObject obj) {
				uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
                mUIRef = uiRefGo.GetComponent<UI_BoxRankAwardRef>();
				TweenShow();
                UIEventListener.Get(this.mUIRef.mBtnReceive).onClick = this.OnButtonClick;

                this.mUIRef.mLbGold.text = string.Format("x{0}", this.resp_grant.Grant);
                this.mUIRef.mLbInfo.text = string.Format(StringTable.GetString("ItemNotice10"), this.resp_grant.Rank + 1);

                uint RoomMultiple = FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID()).RoomMultiple;
                WorldBossGrantVo grant = FishConfig.Instance.GetWorldBossGrant(this.resp_grant.Rank);
                if (grant != null && grant.Grant * RoomMultiple > this.resp_grant.Grant) {//贡献不足，降低奖励
                    this.mUIRef.mLbWarning.text = StringTable.GetString("WorldChest3");
                    this.mUIRef.mLbGold.transform.localPosition = new Vector3(-80,-30);
                } else {
                    this.mUIRef.mLbWarning.text = string.Empty;
                    this.mUIRef.mLbGold.transform.localPosition = new Vector3(-80, -58);
                }
            }
        );
        base.Show();
    }

    public void ReveiveGold() {//领取奖励
        if (this.resp_grant != null) {
            Vector3 ui_world_pos = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
            SceneLogic.Instance.EffectMgr.GoldEffect.ShowGoldEffect(SceneLogic.Instance.FModel.SelfClientSeat, (int)this.resp_grant.Grant, 50, ui_world_pos, 2, 500, true);
            SceneLogic.Instance.FModel.OnAddUserGlobelByCatchedData(SceneLogic.Instance.FModel.SelfClientSeat, this.resp_grant.Grant);

            if (ScenePlayerMgr.mDicGoldChange.ContainsKey(-1)) {//-1 表示全服宝箱消耗榜奖励
                ScenePlayerMgr.mDicGoldChange[-1] -= (int)this.resp_grant.Grant;
            }
            this.resp_grant = null;
        }
    }
    public override void Close() {
        if (this.resp_grant != null) {
            this.ReveiveGold();

            TimeManager.DelayExec(0.5f, () => {
                FishNetAPI.Instance.SendWorldBOSSRank();
            });
        }
        base.Close();
    }

    public void OnButtonClick(GameObject obj) {
        if (this.mUIRef.mBtnReceive == obj) {//领取奖励
            this.Close();
        }
    }
}
