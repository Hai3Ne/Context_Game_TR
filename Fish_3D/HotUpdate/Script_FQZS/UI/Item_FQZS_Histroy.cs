using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_Histroy : UIItem
{
    public List<GameObject> mHistroyLst = new List<GameObject>();

    /// <summary>
    /// 向上的箭头
    /// </summary>
    private UISprite mUpArrow;

    /// <summary>
    /// 向下的箭头
    /// </summary>
    private UISprite mDownArrow;

    /// <summary>
    /// 历史结果
    /// </summary>
    private byte[] mHistroySocre = new byte[50];

    private int ShowMaxCount = 9;

    private int mItemHight = 70;

    /// <summary>
    /// 历史记录滑动列表
    /// </summary>
    private UIScrollView mScorllView;

    private GameObject mItemHistroy;

    private int mCurrLastIndex = -1;

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "up":
                UpdateHistroy(2);
                break;
            case "down":
                UpdateHistroy(3);
                break;
        }
    }

    public void InitData()
    {
        mHistroyLst.Clear();
        mCurrLastIndex = -1;
    }

    private void ChangeUpBtnState(bool active)
    {
        mUpArrow.IsGray = !active;
        mUpArrow.GetComponent<BoxCollider>().enabled = active;
    }

    private void ChangeDwonBtnState(bool active)
    {
        mDownArrow.IsGray = !active;
        mDownArrow.GetComponent<BoxCollider>().enabled = active;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "animal":
                BindItem<Item_FQZS_Histroy_Anima>(tf.gameObject);
                mItemHistroy = tf.gameObject;
                break;
            case "up":
                mUpArrow = tf.GetComponent<UISprite>();
                break;
            case "down":
                mDownArrow = tf.GetComponent<UISprite>();
                break;
            case "socrllview":
                mScorllView = tf.GetComponent<UIScrollView>();
                break;
        }
    }

    /// <summary>
    /// 更新中奖历史显示
    /// </summary>
    /// <param name="type">1 显示最新的 2显示上一次的 3 显示下一次的</param>
    public void UpdateHistroy(int type, byte[] HistroySocre = null)
    {
        if (HistroySocre != null)
        {
            for (int i = 0; i < HistroySocre.Length; i++)
            {
                mHistroySocre[i] = HistroySocre[i];
            }
        }

        ShowNow(type);
    }

    /// <summary>
    /// 找到当前需要显示的9个中奖历史数据
    /// </summary>
    private void ShowNow(int type)
    {
        if (mHistroySocre == null)
            return;

        mCurrLastIndex = FindeLastIndex();

        if (mCurrLastIndex == -1)
            return;

        if (type == 1)
        {
            if (mHistroyLst.Count == 0)
            {
                for (int i = 0; i <= mCurrLastIndex; i++)
                {
                    GameObject itemHistroy = Instantiate(mItemHistroy, mScorllView.transform);
                    itemHistroy.SetActive(true);
                    itemHistroy.transform.localPosition = new Vector3(0, -i * mItemHight);

                    itemHistroy.GetComponent<Item_FQZS_Histroy_Anima>().InitData(mHistroySocre[i], i == mCurrLastIndex);
                    mHistroyLst.Add(itemHistroy);
                }
            }
            else
            {
                if (mHistroyLst.Count > 50)
                {
                    Destroy(mHistroyLst[0]);
                    mHistroyLst.RemoveAt(0);
                }
                GameObject itemHistroy = Instantiate(mItemHistroy, mScorllView.transform);
                itemHistroy.SetActive(true);
                itemHistroy.GetComponent<Item_FQZS_Histroy_Anima>().InitData(mHistroySocre[mCurrLastIndex], true);
                mHistroyLst.Add(itemHistroy);

                for (int i = 0; i < mHistroyLst.Count; i++)
                {
                    mHistroyLst[i].transform.localPosition = new Vector3(0, -i * mItemHight);

                    if (i == mHistroyLst.Count - 1)
                    {
                        mHistroyLst[i].GetComponent<Item_FQZS_Histroy_Anima>().ChangeSelectState(true);
                    }
                    else
                    {
                        mHistroyLst[i].GetComponent<Item_FQZS_Histroy_Anima>().ChangeSelectState(false);
                    }
                }
            }

            if(mHistroyLst.Count > ShowMaxCount)
                StartCoroutine(MoveEnd());
        }
        else if (type == 2)
        {
            float curHitstroyNum = mHistroyLst.Count;

            float moveRate = 1 / (curHitstroyNum - 8);

            mScorllView.verticalScrollBar.value -= moveRate;
            if (mScorllView.verticalScrollBar.value < 0)
                mScorllView.verticalScrollBar.value = 0;
        }
        else
        {
            float curHitstroyNum = mHistroyLst.Count;

            float moveRate = 1 / (curHitstroyNum - 8);

            mScorllView.verticalScrollBar.value += moveRate;
            if (mScorllView.verticalScrollBar.value > 1)
                mScorllView.verticalScrollBar.value = 1;
        }
    }

    private void Update()
    {
        if (mCurrLastIndex != -1)
        {
            if (mCurrLastIndex <= ShowMaxCount - 1)
            {
                mScorllView.enabled = false;
                ChangeDwonBtnState(false);
                ChangeUpBtnState(false);
                mScorllView.verticalScrollBar.value = 0;
            }
            else
            {
                mScorllView.enabled = true;

                if (mScorllView.verticalScrollBar.value == 1)
                {
                    ChangeDwonBtnState(false);
                    ChangeUpBtnState(true);
                }
                else if (mScorllView.verticalScrollBar.value == 0)
                {
                    ChangeDwonBtnState(true);
                    ChangeUpBtnState(false);
                }
                else
                {
                    ChangeDwonBtnState(true);
                    ChangeUpBtnState(true);
                }
            }
        }
        else
        {
            mScorllView.enabled = false;
            ChangeDwonBtnState(false);
            ChangeUpBtnState(false);
            mScorllView.verticalScrollBar.value = 0;
        }
    }

    IEnumerator MoveEnd()
    {
        yield return new WaitForSeconds(0.2f);
        mScorllView.verticalScrollBar.value = 1;
    }

    private int FindeLastIndex()
    {
        for (int i = mHistroySocre.Length -1; i >= 0; i--)
        {
            if (mHistroySocre[i] != 150 && mHistroySocre[i] != 0)
                return i;
        }

        return -1;
    }
}
