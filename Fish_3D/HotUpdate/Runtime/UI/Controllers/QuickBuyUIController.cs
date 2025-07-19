using System;
using UnityEngine;

public class QuickBuyUIController : IUIControllerImp
{
	const string CHAR_TOTAL_PRICES = "{0}";
	QuickBuyUIRef mUIRef;
	int currentBuyNum = 1;
	int singlePrice = 1;

	ItemsVo mItemVo;
	public override void Init (object data)
	{
		base.Init (data);

		if (data.GetType() == typeof(uint)) {		
			uint itemCfgID = (uint)data;
			mItemVo = FishConfig.Instance.Itemconf.TryGet (itemCfgID);
		}
		else if (data.GetType() == typeof(ItemsVo))
		{
			mItemVo = data as ItemsVo;
		}

	}

	public override void Show ()
	{
		WndManager.LoadUIGameObject("QuickBuyUI", SceneObjMgr.Instance.UIPanelTransform, 
			delegate(GameObject obj)
			{
				uiRefGo = obj;
				WndManager.Instance.Push(uiRefGo);
				TweenShow();
				mUIRef = uiRefGo.GetComponent<QuickBuyUIRef>();
				initUI();

		});
		base.Show ();
	}

	public override void Close ()
	{
		base.Close ();
	}

	void initUI()
	{
		UIEventListener.Get (mUIRef.btnclose).onClick = HandleBtnEvent;
		UIEventListener.Get (mUIRef.btnOkay).onClick = HandleBtnEvent;
		UIEventListener.Get (mUIRef.btnAdd).onClick = HandleBtnEvent;
		UIEventListener.Get (mUIRef.btnSub).onClick = HandleBtnEvent;
		UIEventListener.Get (mUIRef.btnAdd10).onClick = HandleBtnEvent;
		UIEventListener.Get (mUIRef.btnSub10).onClick = HandleBtnEvent;

        EventDelegate.Add(this.mUIRef.mToggleQuick.onChange, onValueChange);

        singlePrice = mItemVo.SalePrice;
        mUIRef.mSprItemIcon.spriteName = this.mItemVo.ItemIcon;

        if (this.mItemVo.ItemType == (byte)EnumItemType.Hero) {
            mUIRef.mSprItemFrame.spriteName = ConstValue.HeroFrameSpList[this.mItemVo.Star];
        } else {
            mUIRef.mSprItemFrame.spriteName = ConstValue.ItemFrameSpList[this.mItemVo.Star];
        }
        mUIRef.mLbItemName.text = StringTable.GetString(this.mItemVo.ItemName);
        this.mUIRef.mToggleQuick.value = GameConfig.OP_QuickBuy;
        this.SetBuyNum(1);
	}

    private void onValueChange() {
        GameConfig.OP_QuickBuy = this.mUIRef.mToggleQuick.value;
        GameConfig.SaveData();
		SceneLogic.Instance.Notifiy (SysEventType.QuickByKeyActive, this.mUIRef.mToggleQuick.transform.position);
    }

    private void SetBuyNum(int num) {
        int max = (int)Math.Min(this.mItemVo.MaxCount, this.mItemVo.SaleNum);
        //可用于购买道具金币数
        long gold = SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.PlayerMgr.MyClientSeat) - SceneLogic.Instance.RoomVo.CostScore;
        if (max > gold / this.singlePrice) {
            max = (int)(gold / this.singlePrice);
        }
        if (num > max) {
            num = max;
        }
        num = Math.Max(num, 1);
        currentBuyNum = num;
        mUIRef.numLabel.text = currentBuyNum.ToString();
        mUIRef.totalPriceLabel.text = string.Format(CHAR_TOTAL_PRICES, CalTotalPrice());
    }

	void HandleBtnEvent(GameObject go)
	{
		if (mUIRef.btnclose == go) {
			Close ();
		} else if (mUIRef.btnOkay == go) {
            if (this.CalTotalPrice() + SceneLogic.Instance.RoomVo.CostScore > SceneLogic.Instance.FModel.GetPlayerGlobelBySeat(SceneLogic.Instance.PlayerMgr.MyClientSeat)) {
                if (RoleInfoModel.Instance.CoinMode == EnumCoinMode.Score) {
                    SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_33"));
                } else {
                    PayItem item;
                    if (SceneLogic.Instance.RoomVo.QuickCharge > 0) {
                        item = ShopManager.GetPayByIndex(SceneLogic.Instance.RoomVo.QuickCharge - 1);
                    } else {
                        item = null;
                    }
                    if (item != null)
                    {
                        //UI.EnterUI<UI_QuickRecharge>(ui =>
                        //{
                        //    ui.InitData(1, item, () => 
                        //    {
                     
                        //    });
                        //});

                        UI.EnterUI<UI_QuickRecharge>(GameEnum.All).InitData(1, item, () =>
                        {
                            //乐豆不足
                            NetServices.Instance.Send(NetCmdType.SUB_C_SAFE, new CS_GR_Safe());//检查救济金领取
                        });

                    } else {
                        SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_5"));
                    }
                    //UI.EnterUI<UI_poor>(ui => {
                    //    ui.InitData(1);//乐豆不足
                    //});
                    //SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_5"));
                }
                return;
            }
            if (currentBuyNum > 0) {
                FishNetAPI.Instance.SendBuyItem(mItemVo.CfgID, (ushort)currentBuyNum);
                Close();
            }
		} else if (mUIRef.btnAdd == go) {
            this.SetBuyNum(currentBuyNum + 1);
        } else if (mUIRef.btnSub == go) {
            this.SetBuyNum(currentBuyNum - 1);
        } else if (mUIRef.btnAdd10 == go) {
            this.SetBuyNum(currentBuyNum + 10);
        } else if (mUIRef.btnSub10 == go) {
            this.SetBuyNum(currentBuyNum - 10);
		}
	}

	int CalTotalPrice()
	{
		return singlePrice * currentBuyNum;
	}
		
}

