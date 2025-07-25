using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public partial class UIHelpPanel : PanelBase
    {
        private LoopListView2 m_view = null;
        protected int pageCount = 3;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_view = m_SRect_VipList.GetComponent<LoopListView2>();
            var initParam = new LoopListViewInitParam
            {
                mSmoothDumpRate = 0.1f,
                mSnapVecThreshold = 99999,
            };
            m_view.mOnEndDragAction = OnEndDrag;
            m_view.mOnSnapNearestChanged = OnSnapNearestChanged;
            m_view.InitListView(-1, OnUpdate, initParam);
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


        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            m_view.MovePanelToItemIndex(0, 0);
            //m_PageScroll_Vip.SelecetPage(MainUIModel.Instance.palyerData.m_i4Viplev);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        private void OnSnapNearestChanged(LoopListView2 view, LoopListViewItem2 item)
        {
            int itemIndex = m_view.CurSnapNearestItemIndex;
            int count = pageCount;
            int currentIndex = itemIndex % count;
        }

        public virtual LoopListViewItem2 OnUpdate(LoopListView2 view, int index)
        {
            string itemName = "CloneItem";
            var itemObj = view.NewListViewItem(itemName);
            int count = pageCount;
            int wrapindex = 0 <= index ? index % count
                : count + ((index + 1) % count) - 1;

            for (int i = 0; i < pageCount; i++)
                itemObj.transform.GetChild(i).gameObject.SetActive(i == wrapindex);
            return itemObj;
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Next.onClick.AddListener(OnNextBtn);
            m_Btn_Last.onClick.AddListener(OnLastBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Next.onClick.RemoveListener(OnNextBtn);
            m_Btn_Last.onClick.RemoveListener(OnLastBtn);
        }
        #endregion

        public void OnCloseBtn()
        {
            MainPanelMgr.Instance.Close(transform.name);
        }

        public void OnNextBtn()
        {
            //m_PageScroll_Vip.NextPage();
            SetSnapIndex(1);
        }
        public void OnLastBtn()
        {
            //m_PageScroll_Vip.LastPage();
            SetSnapIndex(-1);
        }

        private void SetSnapIndex(int offset)
        {
            int currentIndex = m_view.CurSnapNearestItemIndex;
            int nextIndex = currentIndex + offset;
            m_view.SetSnapTargetItemIndex(nextIndex);
        }
    }
}
