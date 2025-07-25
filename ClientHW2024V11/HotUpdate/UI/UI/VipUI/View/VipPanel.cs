using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class VipPanel : PanelBase
    {
        private LoopListView2 m_view = null;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_view = m_SRect_VipList.GetComponent<LoopListView2>();
            var initParam = new LoopListViewInitParam
            {
                mSmoothDumpRate =0.1f,
                mSnapVecThreshold = 9999,
            };
            m_view.mOnSnapNearestChanged = OnSnapNearestChanged;
            m_view.mOnEndDragAction = OnEndDrag;
            m_view.InitListView(-1, OnUpdate, initParam);
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            m_view.MovePanelToItemIndex(MainUIModel.Instance.palyerData.m_i4Viplev,0);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.A))
            {
                MainUIModel.Instance.palyerData.m_i4VipExp += 1000;
                Message.Broadcast(MessageName.REFRESH_VIP_PANEL);
            }
        }

        private void OnSnapNearestChanged(LoopListView2 view, LoopListViewItem2 item)
        {
            int itemIndex = m_view.CurSnapNearestItemIndex;
            int count = MainUIModel.Instance.vipData.vipItemDatas.Count;
            int currentIndex = itemIndex % count;
        }

        private LoopListViewItem2 OnUpdate(LoopListView2 view, int index)
        {
            var itemObj = view.NewListViewItem("VipItem");
            var item = itemObj.GetComponent<VipItem>();
            
            int count = MainUIModel.Instance.vipData.vipItemDatas.Count;

            int wrapindex = 0 <= index
                ? index % count
                : count + ((index + 1) % count) - 1;
            ;
            item.SetUpItem(MainUIModel.Instance.vipData.vipItemDatas[wrapindex]);
            return itemObj;
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Next.onClick.AddListener(OnNextBtn);
            m_Btn_last.onClick.AddListener(OnLastBtn);
            Message.AddListener(MessageName.REFRESH_VIP_PANEL, Refresh);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Next.onClick.RemoveListener(OnNextBtn);
            m_Btn_last.onClick.RemoveListener(OnLastBtn);
            Message.RemoveListener(MessageName.REFRESH_VIP_PANEL, Refresh);
        }
        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            VipCtrl.Instance.CloseVipPanel();
        }

        public void OnNextBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            SetSnapIndex(1);
        }
        public void OnLastBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            SetSnapIndex(-1);
        }

        private void SetSnapIndex(int offset)
        {
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

        public void Refresh()
        {
            //m_view.InitListView(-1, OnUpdate);
            m_view.RefreshAllShownItem();
            //SetSnapIndex(MainUIModel.Instance.palyerData.m_i4Viplev);
        }


    }
}
