using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TableUserInfoPanel
{
    public GameObject mObj;
    private byte mClientSeat;
	UISprite infoBgSp;
	int infobgWidth = 0;
    private Vector3 mShowPos;//用户信息显示坐标
    private Vector3 mHidePos;//用户信息隐藏坐标
	public void Init(UsrinfoUIRef usrInfoUi, byte  clientSeat)
	{
        this.mClientSeat = clientSeat;
        TablePlayerInfo playerInfo = SceneLogic.Instance.FModel.GetTabPlayerByCSeat(clientSeat);
        usrInfoUi.usrNickLabel.text = FishConfig.GetTitle(SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(clientSeat));//玩家昵称暂时不显示
        //usrInfoUi.usrNameLabel.text = playerInfo.NickName;
        usrInfoUi.usrNameLabel.text = GameUtils.SubStringByWidth(usrInfoUi.usrNameLabel, playerInfo.NickName, 110);
		usrInfoUi.usrIconSp.mainTexture = FishResManager.Instance.playerIconTexAltas;
        usrInfoUi.usrIconSp.uvRect = GameUtils.FaceUVRect(playerInfo.FaceID);
		infoBgSp = usrInfoUi.listBgSp;
		if (infoBgSp != null) {
			infobgWidth = infoBgSp.width;
		}
        this.mObj = usrInfoUi.panelObj;
        this.mShowPos = this.mObj.transform.localPosition;
        if (clientSeat > 1) {
            this.mHidePos = this.mShowPos + new Vector3(0, 250);
        } else {
            this.mHidePos = this.mShowPos - new Vector3(0, 250);
        }
        this.ShowInfo(true);
        TimeManager.AddDelayEvent(this.mObj.GetInstanceID(), 5, () => {
            if (this.mObj != null) {
                this.ShowInfo(false);
            }
        });
	}

    private bool mIsMove = false;
    private bool mInitIsShow = false;
    public void MoveUserInfo(bool is_move) {//能量炮出现时，移动用户信息UI，防止挡住能量炮提示
        if (mIsMove == is_move) {
            return;
        }
        this.mIsMove = is_move;

        if (this.mIsMove) {
            this.mInitIsShow = this.mIsShow;
            if (this.mIsShow) {
                this.ShowInfo(false);
            }
        } else {
            if (this.mInitIsShow) {
                this.ShowInfo(true);
            }
        }
    }

    public bool mIsShow = true;
    public void ShowInfo(bool is_show) {
        this.mIsShow = is_show;
        if (is_show) {
            TweenPosition.Begin(this.mObj, 0.5f, this.mShowPos);
        } else {
            TimeManager.RemoveDelayEvent(this.mObj.GetInstanceID());
            TweenPosition.Begin(this.mObj, 0.5f, this.mHidePos);
        }
        if (this.mClientSeat == 1 || this.mClientSeat == 2) {
            //延迟一帧操作，防止未初始化完成就进行获取
            SceneLogic.Instance.LogicUI.ItemListUI.RefershGridCellHeight();
        }
	}
}

