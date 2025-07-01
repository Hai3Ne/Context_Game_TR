using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_ZhuanPan_Option : UIItem {
    public UISprite mBtnBetSpr;
    public UILabel mLbAllGold;
    public UILabel mLbSelfGold;
    public UISprite mSprFullTick;//已经达到上限

    public UI_Shenhua ui;
    public SHEnumOption mOption;
    public bool mIsFull;
    private Color mOldCol;
    public void InitData(UI_Shenhua ui,SHEnumOption option)
    {
        this.ui = ui;
        this.mOption = option;
    }
    public void SetGold(long all_gold, long self_gold)
    {
        if (all_gold == 0)
        {
            this.mLbAllGold.text = string.Empty;
        }
        else
        {
            this.mLbAllGold.text = all_gold.ToString();
        }
        if (self_gold > 0)
        {
            this.mLbSelfGold.text = self_gold.ToString();
        }
        else
        {
            this.mLbSelfGold.text = string.Empty;
        }
        this.mIsFull = SHGameManager.UserMaxGold > 0 && self_gold >= SHGameManager.UserMaxGold;
        this.mSprFullTick.gameObject.SetActive(this.mIsFull);
    }

    private void Update()
    {
        if (this.mLbAllGold.text.Equals(string.Empty))
            this.mLbAllGold.gameObject.SetActive(false);
        else
            this.mLbAllGold.gameObject.SetActive(true);

        if (this.mLbSelfGold.text.Equals(string.Empty))
            this.mLbSelfGold.gameObject.SetActive(false);
        else
            this.mLbSelfGold.gameObject.SetActive(true);
    }
    public void SetState(SHEnumGameState state) {
        switch (state) {
            case SHEnumGameState.Wait://等待擂主上擂
                this.mLbAllGold.text = string.Empty;
                this.mLbSelfGold.text = string.Empty;
                this.mSprFullTick.gameObject.SetActive(false);
                this.mIsFull = false;
                break;
            case SHEnumGameState.Bet://开始下注
                this.mLbAllGold.text = string.Empty;
                this.mLbSelfGold.text = string.Empty;
                this.mSprFullTick.gameObject.SetActive(false);
                this.mIsFull = false;
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_bet"://下注
                if (SHGameManager.CurSelectBet > 0 && this.mIsFull == false)
                {
                    ui.item_zhuanpan.PlaceJetton(this.mOption, SHGameManager.CurSelectBet);

                    if (ui.item_zhuanpan.mDownCount > 0)
                    {
                        SHGameManager.curMousePos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
                        mBtnBetSpr.color = new Color(1, 1, 1);
                        StartCoroutine(FlsahLight());
                    }
                }
                break;
        }
    }

   IEnumerator FlsahLight()
    {
        yield return new WaitForSeconds(0.1f);
        mBtnBetSpr.color = mOldCol;
    }


    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "btn_bet":
                mBtnBetSpr = tf.GetComponent<UISprite>();
                mOldCol = mBtnBetSpr.color;
                break;
            case "item_lb_all_gold":
                this.mLbAllGold = tf.GetComponent<UILabel>();
                break;
            case "item_lb_self_gold":
                this.mLbSelfGold = tf.GetComponent<UILabel>();
                break;
            case "item_spr_full_tick":
                this.mSprFullTick = tf.GetComponent<UISprite>();
                break;
        }
    }
}
