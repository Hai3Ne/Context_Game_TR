using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_History : UIItem {
    private int ItemWidth = 68;
    private int ShowMaxCount = 8;//每页显示最大数量

    public GameObject mItemHistory;
    public UIScrollView mScrollView;

    public UISprite mItemBtnDown;
    public UISprite mItemBtnDownArrow;

    public UISprite mItemBtnUp;
    public UISprite mItemBtnUpArrow;

    public List<SHEnumOption> mOptionList = new List<SHEnumOption>();
    public List<UISprite> mHistoryList = new List<UISprite>();

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="list"></param>
    public void InitData(List<SHEnumOption> list)
    {
        mOptionList.Clear();

        while (list.Count > 50)
        {
            list.RemoveAt(0);
        }

        mOptionList.AddRange(list);

        foreach (var item in this.mHistoryList)
        {
            Destroy(item.gameObject);
        }

        mHistoryList.Clear();

        UISprite spr_img;

        for (int i = 0; i < mOptionList.Count; i++)
        {
            spr_img = Instantiate(mItemHistory, mScrollView.transform).GetComponent<UISprite>();
            spr_img.gameObject.SetActive(true);
            spr_img.spriteName = SHGameConfig.SmallIcons[(int)list[i]];
            spr_img.MakePixelPerfect();
            spr_img.transform.localPosition = new Vector3(ItemWidth * i, 0);
            mHistoryList.Add(spr_img);
        }

        if (mHistoryList.Count < ShowMaxCount)
        {
            int rightMove = (ShowMaxCount - mHistoryList.Count) * ItemWidth;

            for (int i = 0; i < mScrollView.transform.childCount; i++)
            {
                mScrollView.transform.GetChild(i).localPosition += new Vector3(rightMove, 0);
            }
        }
    }

    private void MoveBack(int cellNum)
    {
        float currX = 0;
        for (int i = 0; i < mHistoryList.Count; i++)
        {
            currX = mHistoryList[i].transform.localPosition.x - ItemWidth * cellNum;
            mHistoryList[i].transform.localPosition = new Vector3(currX, 0);
        }
    }

    /// <summary>
    /// 刷新历史记录
    /// </summary>
    /// <param name="option"></param>
    public void RefreshHistroy()
    {
        if (mOptionList.Count == 0)
            return;

        if (mOptionList.Count == 1)
        {
            UISprite spr_img = Instantiate(mItemHistory, mScrollView.transform).GetComponent<UISprite>();
            spr_img.gameObject.SetActive(true);
            spr_img.spriteName = SHGameConfig.SmallIcons[(int)mOptionList[0]];
            spr_img.MakePixelPerfect();
            spr_img.transform.localPosition = new Vector3(ItemWidth * 7, 0);
            mHistoryList.Add(spr_img);
        }
        else
        {
            int CreateCount = mOptionList.Count - mHistoryList.Count;

            if (CreateCount <= 0)
                return;

            MoveBack(CreateCount);

            Vector3 curLastPos = mHistoryList[mHistoryList.Count - 1].transform.localPosition;

            int index = 0;

            for (int i = mHistoryList.Count; i < mOptionList.Count; i++)
            {
                index++;
                UISprite spr_img = Instantiate(mItemHistory, mScrollView.transform).GetComponent<UISprite>();
                spr_img.gameObject.SetActive(true);
                spr_img.spriteName = SHGameConfig.SmallIcons[(int)mOptionList[i]];
                spr_img.MakePixelPerfect();
                spr_img.transform.localPosition = curLastPos + new Vector3(ItemWidth * index, 0);
                mHistoryList.Add(spr_img);
            }
        }

        if (mHistoryList.Count > ShowMaxCount)
            ShowLast();
    }

    /// <summary>
    /// 滑动到最右边
    /// </summary>
    public void ShowLast()
    {
        StartCoroutine(moveEnd());
    }

    IEnumerator moveEnd()
    {
        yield return new WaitForSeconds(0.2f);
        mScrollView.horizontalScrollBar.value = 1;
    }

    private void Update()
    {
         if (mHistoryList.Count <= ShowMaxCount)
         {
            ChangeUpBtnState(false);
            ChangeDwonBtnState(false);
            mScrollView.horizontalScrollBar.enabled = false;
            mScrollView.enabled = false;
        }
         else
         {
            mScrollView.enabled = true;
            mScrollView.horizontalScrollBar.enabled = true;
            if (mScrollView.horizontalScrollBar.value > 0.995)
            {
                ChangeUpBtnState(true);
                ChangeDwonBtnState(false);
            }
            else if (mScrollView.horizontalScrollBar.value < 0.001)
            {
                ChangeUpBtnState(false);
                ChangeDwonBtnState(true);
            }
            else
            {
                ChangeUpBtnState(true);
                ChangeDwonBtnState(true);
            }
        }
    }

    private void ChangeUpBtnState(bool active)
    {
        mItemBtnUp.IsGray = !active;
        mItemBtnUpArrow.IsGray = !active;
        mItemBtnUp.GetComponent<BoxCollider>().enabled = active;
    }

    private void ChangeDwonBtnState(bool active)
    {
        mItemBtnDown.IsGray = !active;
        mItemBtnDownArrow.IsGray = !active;
        mItemBtnDown.GetComponent<BoxCollider>().enabled = active;
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "item_btn_up":
                MoveUp();
                break;
            case "item_btn_down":
                MoveDwon();
                break;
        }
    }

    /// <summary>
    /// 向前移动
    /// </summary>
    private void MoveDwon()
    {
        float curHistroyNum = mHistoryList.Count;

        float moveRate = 1 / (curHistroyNum - 8);

        mScrollView.horizontalScrollBar.value += moveRate;

        if (mScrollView.horizontalScrollBar.value > 1)
            mScrollView.horizontalScrollBar.value = 1;
    }

    /// <summary>
    /// 向后移动
    /// </summary>
    private void MoveUp()
    {
        float curHistroyNum = mHistoryList.Count;

        float moveRate = 1 / (curHistroyNum - 8);

        mScrollView.horizontalScrollBar.value -= moveRate;

        if (mScrollView.horizontalScrollBar.value < 0)
            mScrollView.horizontalScrollBar.value = 0;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "item_history":
                this.mItemHistory = tf.gameObject;
                this.mItemHistory.SetActive(false);
                break;
            case "scrollview_history":
                this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "item_btn_up":
                mItemBtnUp = tf.GetComponent<UISprite>();
                break;
            case "item_btn_down":
                mItemBtnDown = tf.GetComponent<UISprite>();
                break;
            case "item_spr_up":
                mItemBtnUpArrow = tf.GetComponent<UISprite>();
                break;
            case "item_spr_down":
                mItemBtnDownArrow = tf.GetComponent<UISprite>();
                break;
        }
    }
}
