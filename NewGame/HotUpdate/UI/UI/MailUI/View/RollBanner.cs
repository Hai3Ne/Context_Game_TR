using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RollBanner : MonoBehaviour
{
	[SerializeField] private LoopListView2 m_view = null;
    [SerializeField] private GameObject m_original = null;
    [SerializeField] private List<Sprite> bannerList = new List<Sprite>();
    [SerializeField] private float m_interval = 0;
    [SerializeField] private List<Toggle> toggles = new List<Toggle>();
    private float m_timer;

    private List<Sprite> m_bannerList = new List<Sprite>();
    private List<Toggle> m_toggles = new List<Toggle>();
    private ScrollRect scrollRect;
    private bool isHorizontal = true;
    private void Start()
    {
        var initParam = new LoopListViewInitParam
        {
            mSmoothDumpRate = 0.1f,
            mSnapVecThreshold = 99999,
        };
        m_view.mOnEndDragAction = OnEndDrag;
        m_view.mOnSnapNearestChanged = OnSnapNearestChanged;
        m_view.InitListView(-1, OnUpdate, initParam);
    }

    private void OnSnapNearestChanged(LoopListView2 view, LoopListViewItem2 item)
    {
        int itemIndex = m_view.CurSnapNearestItemIndex;
        int wrapindex = 0 <= itemIndex
                ? itemIndex % m_bannerList.Count
                : m_bannerList.Count + ((itemIndex + 1) % m_bannerList.Count) - 1; ;
        if (m_toggles[wrapindex] != null)
            m_toggles[wrapindex].isOn = true;
        m_timer = 0;
    }

    private LoopListViewItem2 OnUpdate(LoopListView2 view, int index)
    {

        var isScrol = m_bannerList.Count != 1;
        if(isHorizontal != isScrol)
        {
            scrollRect.horizontal = isScrol;
            isHorizontal = isScrol;
            Debug.Log("---------------------------" + isScrol);
        }
        var itemObj = view.NewListViewItem(m_original.name);
        var script = itemObj.GetComponent<ListItemUI>();
        int wrapindex = 0 <= index
                ? index % m_bannerList.Count
                : m_bannerList.Count + ((index + 1) % m_bannerList.Count) - 1;
        itemObj.GetComponent<Image>().sprite = m_bannerList[wrapindex];
        script.SetUpItem(wrapindex, itemObj.GetComponent<Image>().sprite.name);
        return itemObj;
    }

    public void SetBannerData() 
    {

    }

    private void Update()
    {
        if (m_view.IsDraging|| m_bannerList.Count==1)
        {
            m_timer = 0;
            return;
        } 
        m_timer += Time.deltaTime;
        if (m_timer < m_interval) return;
        SetSnapIndex(1);
    }

    private void OnEnable()
    {
        scrollRect = gameObject.GetComponent<ScrollRect>();
        scrollRect.horizontal = true;
        
        isHorizontal = true;
        m_bannerList.Clear();
        foreach (var item in bannerList)
        {
            m_bannerList.Add(item);
        }
        m_toggles.Clear();
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = i == 0;
   
        }
        foreach (var item in toggles)
        {
            item.gameObject.SetActive(true);
            m_toggles.Add(item);
        }
        Message.AddListener(MessageName.REFRESH_ROLLBANNER_PANEL,UpdateBannerList);
        UpdateBannerList();

        m_view.RefreshAllShownItem();
    }


    private void OnDisable()
    {
        m_view.SetSnapTargetItemIndex(0);
        m_view.GetShownItemByItemIndex(0);
        Message.RemoveListener(MessageName.REFRESH_ROLLBANNER_PANEL, UpdateBannerList);
    }

    private void UpdateBannerList()
    {
        MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
        MainUIModel.Instance.signInData.signInDayDatas.TryGetValue(7, out bool isSign);
        if (isSign)
        {
            for (int i = 0; i < m_bannerList.Count; i++)
            {
                if (m_bannerList[i].name == "DT_img_QiRi")
                {
                    m_bannerList.RemoveAt(i);
                    if (m_toggles.Count > 0)
                    {
                        m_toggles[0].gameObject.SetActive(false);
                        m_toggles.Remove(m_toggles[0]);
                    }
                    break;
                }
            }
        }


        if (isState)
        {
            for (int i = 0; i < m_bannerList.Count; i++)
            {
                if (m_bannerList[i].name == "DT_img_ShouChong")
                {
                    m_bannerList.RemoveAt(i);
                    if (m_toggles.Count > 0)
                    {
                        m_toggles[0].gameObject.SetActive(false);
                        m_toggles.Remove(m_toggles[0]);
                    }
                    break;
                }
            }
        }
        


    }

    private void SetSnapIndex(int offset)
    {
        m_timer = 0;
        int currentIndex = m_view.CurSnapNearestItemIndex;
        int nextIndex = currentIndex + offset;
        m_view.SetSnapTargetItemIndex(nextIndex);
    }

    void OnEndDrag()
    {
        float vec = m_view.ScrollRect.velocity.x;
        int curNearestItemIndex = m_view.CurSnapNearestItemIndex;
        LoopListViewItem2 item = m_view.GetShownItemByItemIndex(curNearestItemIndex);
        if (item == null)
        {
            m_view.ClearSnapData();
            return;
        }
        if (Mathf.Abs(vec) < 50f)
        {
            m_view.SetSnapTargetItemIndex(curNearestItemIndex);
            return;
        }
        Vector3 pos = m_view.GetItemCornerPosInViewPort(item, ItemCornerEnum.LeftTop);
        if (pos.x > 0)
        {
            if (vec > 0)
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex - 1);
            }
            else
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex);
            }
        }
        else if (pos.x < 0)
        {
            if (vec > 0)
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex);
            }
            else
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex + 1);
            }
        }
        else
        {
            if (vec > 0)
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex - 1);
            }
            else
            {
                m_view.SetSnapTargetItemIndex(curNearestItemIndex + 1);
            }
        }
    }

}
