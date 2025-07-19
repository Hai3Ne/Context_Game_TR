using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_CoinAnimaCtrl : UIItem
{
    /// <summary>
    /// 动画起点列表
    /// </summary>
    private List<Vector3> mStartPosLst = new List<Vector3>();

    /// <summary>
    /// 动画终点的字典
    /// </summary>
    private Dictionary<string, Vector3> mEndPosDic = new Dictionary<string, Vector3>();

    /// <summary>
    /// 飞行筹码
    /// </summary>
    private Item_FQZS_Coin mCoin;

    private Vector3 mFar = new Vector3(10000, 0, 0);

    /// <summary>
    /// 飞行的假筹码
    /// </summary>
    private Queue<GameObject> mFakeQue = new Queue<GameObject>();

    /// <summary>
    /// 真实筹码队列(出)
    /// </summary>
    private Queue<GameObject> mRealQueOut = new Queue<GameObject>();

    /// <summary>
    /// 真实筹码队列(进)
    /// </summary>
    private Queue<GameObject> mRealQueIn = new Queue<GameObject>();

    public override void OnNodeAsset(string name, Transform tf)
    {
        if (name.StartsWith("StartPos"))
        {
            mStartPosLst.Add(tf.transform.position);
        }
        else if (name.StartsWith("EndPos"))
        {
            mEndPosDic[name] = tf.transform.position;
        }
        else if (name.Equals("Coin"))
        {
            mCoin = BindItem<Item_FQZS_Coin>(tf.gameObject);
        }
    }

    /// <summary>
    /// 删除押注筹码
    /// </summary>
    public void ResetCoin()
    {
        while (mRealQueIn.Count > 0)
        {
            GameObject coin = mRealQueIn.Dequeue();
            coin.transform.localPosition = mFar;
            mRealQueOut.Enqueue(coin);
        }

        foreach (GameObject obj in mFakeQue)
        {
            obj.transform.localPosition = mFar;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="index"></param>
    public void PlayAnima(int  index,long coinValue)
    {
        GameObject coin = null;
        if (mRealQueOut.Count > 0)
        {
            coin = mRealQueOut.Dequeue();
        }
        else
        {
            coin = Instantiate(mCoin.gameObject, transform);
        }

        coin.GetComponent<UISprite>().spriteName = GetCoinSpriteName(coinValue);

        //从15个随机起点中随机一个动画起点
        int random = Random.Range(0, mStartPosLst.Count);
        Vector3 StartPos = mStartPosLst[random];

        //获得动画终点
        string endPosName = string.Format("EndPos_{0}", index);
        Vector3 EndPos = mEndPosDic[endPosName];

        coin.GetComponent<Item_FQZS_Coin>().InitData(StartPos, EndPos,null);

        mRealQueIn.Enqueue(coin);
    }

    private void OnDestroy()
    {
        mRealQueOut.Clear();
        mRealQueIn.Clear();
        mFakeQue.Clear();
    }

    public void PlayAnimaRandom()
    {
        GameObject coin = null;
        if (mFakeQue.Count > 0)
        {
            coin = mFakeQue.Dequeue();
        }
        else
        {
            coin = Instantiate(mCoin.gameObject, transform);
        }

        int index = Random.Range(1, 7);
        coin.GetComponent<UISprite>().spriteName = string.Format("cm{0}", index);

        //从15个随机起点中随机一个动画起点
        int random = Random.Range(0, mStartPosLst.Count);
        Vector3 StartPos = mStartPosLst[random];

        string endPosName = string.Format("EndPos_{0}", Random.Range(1, 12));

        Vector3 EndPos = mEndPosDic[endPosName];

        coin.GetComponent<Item_FQZS_Coin>().InitData(StartPos, EndPos,(obj)=> { obj.transform.localPosition = mFar; mFakeQue.Enqueue(obj); });
    }

    private string GetCoinSpriteName(long value)
    {
        switch (value)
        {
            case 1000:
                return "cm1";
            case 10000:
                return "cm2";
            case 50000:
                return "cm3";
            case 100000:
                return "cm4";
            case 500000:
                return "cm5";
            case 1000000:
                return "cm6";
            case 5000000:
                return "cm7";
        }
        return null;
    }
}
