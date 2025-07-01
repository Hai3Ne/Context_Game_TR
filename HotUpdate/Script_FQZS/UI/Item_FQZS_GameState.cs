using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_GameState : UIItem
{
    /// <summary>
    /// 倒计时
    /// </summary>
    public UILabel mlblCountTime;

    /// <summary>
    /// 所有玩家该局总下注金额
    /// </summary>
    public UILabel mTotalBet;

    /// <summary>
    /// 状态
    /// </summary>
    public UILabel mNotice;

    /// <summary>
    /// 倒计时
    /// </summary>
    public float CountTime = 0;

    public delegate void GemeInWait(Item_FQZS_AnimaSelect animaSelect);
    public GemeInWait OnGameInWait;

    private Item_FQZS_AnimaSelect mAnimaSelect;

    /// <summary>
    /// 倒计时还有5秒的警告
    /// </summary>
    private bool IsStartWarning = false;

    /// <summary>
    /// 初始化游戏状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="all_gold"></param>
    /// <param name="time"></param>
    public void RefreshGameState(float time = 0, Item_FQZS_AnimaSelect animaSelect = null)
    {
        if(animaSelect != null)
            mAnimaSelect = animaSelect;

        if (time != 0)
            CountTime = time;

        if (time > 20)
            mlblCountTime.text = string.Empty;
        else
            mlblCountTime.text = time.ToString();
        long total = 0;

        for (int i = 0; i < FQZSGameManager.CurAllBet.Length; i++)
        {
            total += FQZSGameManager.CurAllBet[i];
        }

        mTotalBet.text = total.ToString();

        if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Free)
        {
            mNotice.text = "准备开始";
            mTotalBet.text = "0";
        }
        else if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
        {
            mNotice.text = "正在竞猜";
            mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
        }
        else if(FQZSGameManager.CurrGameState == FQZSEnumGameState.End)
        {
            mNotice.text = "正在开奖";
            mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
            mlblCountTime.text = string.Empty;
        }
    }

    private void Update()
    {
        if (CountTime == 0) return;

        if (FQZSGameManager.CurrGameState == FQZSEnumGameState.End)
        {
            mNotice.text = "正在开奖";
            mlblCountTime.text = string.Empty;
            mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
            return;
        }

        if (CountTime >= 0)
        {
            mNotice.transform.localPosition = new Vector3(-27, mNotice.transform.localPosition.y, 0);
            CountTime -= Time.deltaTime;

            if (CountTime > 20)
                mlblCountTime.text = string.Empty;
            else
                mlblCountTime.text = ((int)CountTime).ToString();

            if (CountTime <= 1f)
            {
                FQZSGameManager.IsFlyCoin = false;
            }
            
            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
            {
                if (CountTime <= 5)
                {
                    if (!IsStartWarning)
                        StartWarning();
                }
            }
      
            if (CountTime == 0)
            {
                mlblCountTime.text = string.Empty;
                if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
                {
                    mNotice.text = "请稍后...";
                    mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
                    FQZSGameManager.CurrGameState = FQZSEnumGameState.End;
                    if (OnGameInWait != null)
                        OnGameInWait(mAnimaSelect);
                    //这里如果玩家有投注就将投注数据保存到mXuTouArr里
                     for (byte i = 0; i < FQZSGameManager.CurSelfBet.Length; i++)
                     {
                         FQZSGameManager.mXuTouArr[i] = FQZSGameManager.CurSelfBet[i];
                     }
                }
                else if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Free)
                    mNotice.text = "请稍后...";
            }
        }
        else
        {
            CountTime = 0;
            IsStartWarning = false;
            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
            {
                FQZSGameManager.CurrGameState = FQZSEnumGameState.End;
                if (OnGameInWait != null)
                    OnGameInWait(mAnimaSelect);
                //这里如果玩家有投注就降投注数据保存到mXuTouArr里

                for (byte i = 0; i < FQZSGameManager.CurSelfBet.Length; i++)
                {
                    FQZSGameManager.mXuTouArr[i] = FQZSGameManager.CurSelfBet[i];
                }
                mNotice.text = "请稍后...";
                mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
            }
            else if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Free)
            {
                mNotice.text = "请稍后...";
                mNotice.transform.localPosition = new Vector3(-7, mNotice.transform.localPosition.y, 0);
            }
            mlblCountTime.text = string.Empty;
        }
    }

    private void StartWarning()
    {
        IsStartWarning = true;
        StartCoroutine(Warning());
    }

    IEnumerator Warning()
    {
        for (int i = 0; i < 5; i++)
        {
            if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet)
            {
                AudioManager.PlayAudio(GameEnum.FQZS, FQZSGameConfig.SOUND_WARING);
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield break;
            }
        }
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch(name)
        {
            case "counttime":
                mlblCountTime = tf.GetComponent<UILabel>();
                break;
            case "totalbet":
                mTotalBet = tf.GetComponent<UILabel>();
                break;
            case "djs":
                mNotice = tf.GetComponent<UILabel>();
                break;
        }
    }
}
