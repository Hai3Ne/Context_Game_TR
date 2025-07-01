using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_Coin_Fly_Ctrl : UIItem
{
    /// <summary>
    /// 动画起点列表
    /// </summary>
    private List<Vector3> mStartPosLst = new List<Vector3>();

    /// <summary>
    /// 押注时生成的筹码
    /// </summary>
    private List<GameObject> mCoinLst = new List<GameObject>();

    /// <summary>
    /// 动画终点的字典
    /// </summary>
    private Dictionary<string, Vector3> mEndPosDic = new Dictionary<string, Vector3>();

    private Item_ShenHua_Coin mCoin;

    /// <summary>
    /// 不是真实的押注金币,只是为了效果的飞行金币字典
    /// </summary>
    private Dictionary<int, int> unRealCoinDic = new Dictionary<int, int>();

    private List<GameObject> mUnRealCoinLst = new List<GameObject>();

    public override void OnNodeAsset(string name, Transform tf)
    {
        if (name.StartsWith("start_pos_"))
        {
            mStartPosLst.Add(tf.position);
        }
        else if (name.StartsWith("end_pos_"))
        {
            mEndPosDic[name] = tf.transform.position;
        }
        else if (name.Equals("coin"))
        {
            mCoin = BindItem<Item_ShenHua_Coin>(tf.gameObject);
        }
    }

    /// <summary>
    /// 删除押注筹码
    /// </summary>
    public void ClearCoin()
    {
        //清除真实的押注金币
        if (mCoinLst.Count > 0)
        {
            for (int i = 0; i < mCoinLst.Count; i++)
            {
                Destroy(mCoinLst[i]);
            }

            mCoinLst.Clear();
        }

        //清除假的押注金币
        if (mUnRealCoinLst.Count > 0)
        {
            for (int i = 0; i < mUnRealCoinLst.Count; i++)
            {
                Destroy(mUnRealCoinLst[i]);
            }

            mUnRealCoinLst.Clear();
        }

        //清除假的押注数据
        unRealCoinDic.Clear();
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="index"></param>
    public void PlayAnima(int index, long coinValue,bool isSelfBet = false)
    {
        //GameObject coin = Instantiate(mCoin.gameObject, transform);

        //coin.GetComponent<UISprite>().spriteName = GetCoinSpriteName(coinValue);

        ////从15个随机起点中随机一个动画起点
        //int random = Random.Range(0, mStartPosLst.Count);
        //Vector3 StartPos = mStartPosLst[random];

        ////获得动画终点
        //string endPosName = string.Format("end_pos_{0}", index);
        //Vector3 EndPos = mEndPosDic[endPosName];

        //coin.GetComponent<Item_ShenHua_Coin>().InitData(StartPos, EndPos, isSelfBet,speed: 8);

        //mCoinLst.Add(coin);
    }

    public void PlayAnimaRandom()
    {
        //int randomEndPos = Random.Range(1, 9);

        //if (SHGameManager.CurAllBet[randomEndPos -1] <= 0)
        //{
        //    return;
        //}

        //GameObject coin = Instantiate(mCoin.gameObject, transform);

        //coin.GetComponent<UISprite>().spriteName = GetRandomSpirte();

        ////从15个随机起点中随机一个动画起点
        //int random = Random.Range(0, mStartPosLst.Count);
        //Vector3 StartPos = mStartPosLst[random];

        //string endPosName = string.Format("end_pos_{0}", randomEndPos);

        //Vector3 EndPos = mEndPosDic[endPosName];

        //if (unRealCoinDic.ContainsKey(randomEndPos))
        //{
        //    int curvalue = unRealCoinDic[randomEndPos];
        //    curvalue++;
        //    unRealCoinDic[randomEndPos] = curvalue;
        //}
        //else
        //{
        //    unRealCoinDic[randomEndPos] = 1;
        //}

        //if (unRealCoinDic[randomEndPos] > 4)
        //{
        //    coin.GetComponent<Item_ShenHua_Coin>().InitData(StartPos, EndPos, false, true);
        //}
        //else
        //{
        //    coin.GetComponent<Item_ShenHua_Coin>().InitData(StartPos, EndPos, false, false);
        //    mUnRealCoinLst.Add(coin);
        //}
    }

    private string GetRandomSpirte()
    {
        int random = Random.Range(1, 4);

        switch (random)
        {
            case 1:
                return "1k-min";
            case 2:
                return "1w-min";
            case 3:
                return "5w-min";
        }

        return string.Empty;
    }

    private string GetCoinSpriteName(long value)
    {
        if (value >= 1000 && value < 10000)
        {
            return "1k-min";
        }
        else if (value >= 10000 && value < 50000)
        {
            return "1w-min";
        }
        else if (value >= 50000 && value < 100000)
        {
            return "5w-min";
        }
        else if (value >= 100000 && value < 500000)
        {
            return "10w-min";
        }
        else if (value >= 500000 && value < 1000000)
        {
            return "50w-min";
        }
        else if (value >= 1000000 && value < 2000000)
        {
            return "100w-min";
        }
        else if (value >= 2000000 && value < 5000000)
        {
            return "200w-min";
        }
        else
              return "500w-min";
    }
}
