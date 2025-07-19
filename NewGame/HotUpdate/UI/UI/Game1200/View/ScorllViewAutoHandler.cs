using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate
{

    public class ScorllViewAutoHandler : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        private ScrollRect scrollRect;//滑动框组件
        private RectTransform content;//滑动框的Content
        private GridLayoutGroup layout;//布局组件

        private int totalPage; //总页数
        private int curPage; //当前页的下标
        private float[] eachPageNUPos; //每页的NormalizedPositon的值
        private float targetNUPos; //目标页的NormalizedPositon的值

        private Vector2 beginMousePos; //鼠标开始按下的位置
        private Vector2 endMousePos; //鼠标结束按下的位置
        private bool isDrag; //是否在拖拽

        [Header("是否可以滑动多页")]
        public bool sliderMultPage;

        [Header("视为滑动一页的距离")]
        [Space(25)]
        public float sliderOnePageDis;
        [Header("视为滑动多页的距离")]
        public float sliderMultPageDis;
        [Header("缓动到目标页的持续时间")]
        public float duration;

        #region Init

        private void Awake()
        {
            scrollRect =GetComponent<ScrollRect>();
            content = scrollRect.content;
            layout = content.GetComponent<GridLayoutGroup>();

       
        }

        public void Init(int count)
        {
            for (int i = 0; i < content.childCount; i++)
                content.GetChild(i).gameObject.SetActive(false);
            for (int i = 0; i < count; i++)
            {
                if (i >= content.childCount)
                {
                    GameObject go = CommonTools.AddSubChild(content.gameObject, "UI/Prefabs/Game1200/FirstRes/BetSizeCell");
                    go.gameObject.SetActive(true);
                }
                var cell = content.GetChild(i).GetComponent<BetSizeCell>();
                cell.Init(i);
            }
            InitData();//初始化
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            totalPage = content.childCount;

            SetContentSize();//设置Content大小

            CalcEachPageNUPos();//计算每一页的NormalizedPositon值

           // SetLayout();//设置布局
        }

        /// <summary>
        /// 设置Content大小
        /// </summary>
        private void SetContentSize()
        {
            content.sizeDelta = new Vector2
                (
                    content.sizeDelta.x,
                    layout.padding.top + layout.padding.bottom + (totalPage) * (layout.cellSize.y + layout.spacing.y) - layout.spacing.y
                //layout.padding.right + layout.padding.left + (totalPage - 1) * (layout.cellSize.x + layout.spacing.x) - layout.spacing.x,
                // content.sizeDelta.y
                ); ;
        }

        /// <summary>
        /// 计算每一页的NormalizedPositon值
        /// </summary>
        private void CalcEachPageNUPos()
        {
            float tempNUPos = 0;
            eachPageNUPos = new float[totalPage];
            for (int i = 0; i < totalPage; i++)
            {
                eachPageNUPos[i] = tempNUPos;
                tempNUPos += 1f / (totalPage - 1);
            }
        }

        ///// <summary>
        ///// 设置布局
        ///// </summary>
        //private void SetLayout()
        //{
        //    scrollRect.horizontal = true;
        //    scrollRect.vertical = false;
        //    layout.padding.right = layout.padding.left;
        //    layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        //    layout.childAlignment = TextAnchor.MiddleCenter;
        //    layout.constraintCount = 1;
        //}

        #endregion

        #region Main

        /// <summary>
        /// 拖拽开始
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.LogError(">>>>>>>>>>>>>>>");
            isDrag = true;
            beginMousePos = Input.mousePosition;
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            isDrag = false;
            coe = 0;

            endMousePos = Input.mousePosition;
            Vector2 offset = endMousePos - beginMousePos;
            Debug.Log("滑动距离为：" + offset);

            if (sliderMultPage)
            {
                //单页滑动
                if (Mathf.Abs(offset.x) >= sliderOnePageDis && Mathf.Abs(offset.x) < sliderMultPageDis)
                {
                    float tempHorizontalNUPos = scrollRect.verticalNormalizedPosition;
                    FindNearlyPage(tempHorizontalNUPos);
                }
                //多页滑动
                else if (Mathf.Abs(offset.x) >= sliderMultPageDis)
                {
                    if (offset.x > 0)
                    {
                        curPage = 0;
                    }
                    else if (offset.x < 0)
                    {
                        curPage = totalPage - 1;
                    }
                }
            }
            else
            {
                //单页滑动
                if (Mathf.Abs(offset.x) >= sliderOnePageDis)
                {
                    float tempHorizontalNUPos = scrollRect.verticalNormalizedPosition;
                    FindNearlyPage(tempHorizontalNUPos);
                }
            }

            targetNUPos = eachPageNUPos[curPage];
        }

        private float coe;//比例系数
        private void Update()
        {
            if (isDrag)
            {
                return;
            }
            coe += Time.deltaTime / duration;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, targetNUPos, coe);
        }

        #endregion

        #region Tool

        /// <summary>
        /// 寻找距离当前NormalizedPositon最近的页
        /// </summary>
        private void FindNearlyPage(float tempHorizontalNUPos)
        {
            float minOffset = Mathf.Abs(eachPageNUPos[0] - tempHorizontalNUPos);
            for (int i = 0; i < totalPage; i++)
            {
                float tempHorizontalOffset = Mathf.Abs(eachPageNUPos[i] - tempHorizontalNUPos);
                if (tempHorizontalOffset <= minOffset)
                {
                    minOffset = tempHorizontalOffset;
                    curPage = i;
                }
            }
        }

        #endregion
    }
}