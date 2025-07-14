using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_Anima : UIItem
{
    /// <summary>
    /// 自己下注的金额
    /// </summary>
    public UILabel mOwen;

    /// <summary>
    /// 该动物下注的总金额
    /// </summary>
    public UILabel mTotal;

    /// <summary>
    /// 该动物名称
    /// </summary>
    public string mName;

    /// <summary>
    /// 遮罩
    /// </summary>
    public UISprite mZheZhao;

    /// <summary>
    /// 遮罩
    /// </summary>
    public UISprite mZheZhao1;

    /// <summary>
    /// 动画的图
    /// </summary>
    public UISprite mAnima;

    /// <summary>
    /// 和服务器发过来的押注数组做对应的索引
    /// </summary>
    public int GetIndex()
    {
        switch (mName)
        {
            case "shayu":
                return 0;
            case "feiqing":
                return 1;
            case "zoushou":
                return 2;
            case "yanzi":
                return 3;
            case "tuzi":
                return 4;
            case "gezi":
                return 5;
            case "xiongmao":
                return 6;
            case "kongque":
                return 7;
            case "houzi":
                return 8;
            case "laoying":
                return 9;
            case "shizi":
                return 10;
        }

        return -1;
    }

    /// <summary>
    /// 设置下注区域的各个动物的下注金额
    /// </summary>
    /// <param name="TotalBet"></param>
    /// <param name="selfTotalBet"></param>
    public void SetBetInfo(long TotalBet, long selfTotalBet)
    {
        if (selfTotalBet > 0)
        {
            mOwen.text = selfTotalBet.ToString();
        }
        else
        {
            mOwen.text ="0";
        }

        if (TotalBet > 0)
        {
            mTotal.text = TotalBet.ToString();
        }
        else
        {
            mTotal.text = "0";
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch(obj.name)
        {
            case "animal":
                Bet();
                break;
        }
    }

    /// <summary>
    /// 下注
    /// </summary>
    private void Bet()
    {
        if (FQZSGameManager.CurrSelectChouMa != null)
        {
            if (CheckIsLimitScore())
            {
                SystemMessageMgr.Instance.ShowMessageBox("该次竞猜超过区域竞猜上限");
                return;
            }
            NetClient.Send(NetCmdType.SUB_C_PLACE_JETTON_FQZS, new CMD_C_PlaceJetton_fqzs
            {
                JettonArea = (byte)(GetIndex() + 1),
                JettonScore = FQZSGameManager.CurrSelectChouMa.CurrValue,
            });
        }
    }

    /// <summary>
    /// 检查该下注区域是否已经达到了下注上限
    /// </summary>
    /// <returns></returns>
    private bool CheckIsLimitScore()
    {
        int curChouMaValue = FQZSGameManager.CurrSelectChouMa.CurrValue;

        long curAlreadyScore = FQZSGameManager.CurAllBet[GetIndex()];

        if (curChouMaValue + curAlreadyScore >= FQZSGameManager.AreaLimitScore)
            return true;
        return false;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch(name)
        {
            case "total":
                mTotal = tf.GetComponent<UILabel>();
                break;
            case "own":
                mOwen = tf.GetComponent<UILabel>();
                break;
            case "zhezhao":
                mZheZhao = tf.GetComponent<UISprite>();
                break;
            case "zhezhao1":
                mZheZhao1 = tf.GetComponent<UISprite>();
                break;
            case "animal":
                mAnima = tf.GetComponent<UISprite>();
                break;
        }
    }

    /// <summary>
    /// 设置当前是不是可用状态
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetEnable(bool isEnable)
    {
        if (isEnable)
        {
            mZheZhao.IsGray = false;
            mZheZhao1.IsGray = false;
            GetComponent<UISprite>().IsGray = false;
            mAnima.IsGray = false;
            mAnima.GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            mZheZhao.IsGray = true;
            mZheZhao1.IsGray = true;
            GetComponent<UISprite>().IsGray = true;
            mAnima.IsGray = true;
            mAnima.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
