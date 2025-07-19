using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using SEZSJ;

namespace HotUpdate
{
    public partial class GuidePanel : PanelBase
    {
        private Tweener tween;
        Action callBack;
        private GameObject go;

        protected override void Awake()
        {
            base.Awake();

            GetBindComponents(gameObject);
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            m_Btn_Click.onClick.AddListener(ClickBtn);
        }

        protected override void OnDisable()
        {
          
            base.OnDisable();
            tween.Kill();
            m_Btn_Click.onClick.RemoveListener(ClickBtn);
    
        }
  

        public void Init(Transform CloneItem,Action callBack,int guideID,bool b2D = true)
        {
            tween?.Kill();
            AlignBottom();
            if (guideID == 5)
                AlignBottomLeft();
            GameObject obj =  Instantiate(CloneItem.gameObject);
            obj.transform.SetParent(transform);
            obj.transform.localScale = Vector3.one;
            obj.transform.position = CloneItem.position;  
            //if(b2D)
            // obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(CloneItem.GetComponent<RectTransform>().anchoredPosition.x, CloneItem.GetComponent<RectTransform>().anchoredPosition.y,0);
            if (obj.GetComponent<Image>() != null)
                obj.GetComponent<Image>().SetNativeSize();
            this.callBack = callBack;
            go = obj;
            Button btn = obj.GetComponentInChildren<Button>(true);
            if (guideID == 8)
            {
                btn = obj.transform.Find("Btn_Guide").GetComponent<Button>();
                btn.gameObject.SetActive(true);
            }
  
            m_Btn_Click.transform.SetParent(btn.transform, true);
            m_Btn_Click.GetComponent<RectTransform>().anchorMax = btn.GetComponent<RectTransform>().anchorMax;
            m_Btn_Click.GetComponent<RectTransform>().anchorMin = btn.GetComponent<RectTransform>().anchorMin;
            m_Btn_Click.GetComponent<RectTransform>().anchoredPosition = btn.GetComponent<RectTransform>().anchoredPosition;
            m_Btn_Click.transform.localPosition = Vector3.zero;
            m_Btn_Click.GetComponent<RectTransform>().sizeDelta = btn.GetComponent<RectTransform>().sizeDelta;
            m_Rect_Tap.transform.SetParent(btn.transform, true);
            m_Rect_Tap.transform.localPosition = new Vector3(0,-60f,0);
            tween = m_Rect_Tap.DOAnchorPos(m_Rect_Tap.anchoredPosition + new Vector2(0, -30), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        
            m_Btn_Click.transform.SetAsLastSibling();
            m_Rect_Tap.transform.SetAsLastSibling();
        }

        public void AlignBottom()
        {
            m_Rect_Tap.anchorMin = new Vector2(0.5f, 0);
            m_Rect_Tap.anchorMax = new Vector2(0.5f, 0);
            m_Btn_Click.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
            m_Btn_Click.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0f);
            
        }

        //public void AlignTopRight()
        //{
        //    m_Rect_Tap.anchorMin = new Vector2(1, 0);
        //    m_Rect_Tap.anchorMax = new Vector2(1, 0);
        //}

        public void AlignBottomLeft()
        {
            m_Rect_Tap.anchorMin = new Vector2(0.5f, 0.5f);
            m_Rect_Tap.anchorMax = new Vector2(0.5f, 0.5f);
            m_Btn_Click.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            m_Btn_Click.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        }

        //public void AlignBottomRight()
        //{
        //    m_Rect_Tap.anchorMin = new Vector2(1, 1);
        //    m_Rect_Tap.anchorMax = new Vector2(1, 1);
        //}


        private void ClickBtn()
        {
            m_Btn_Click.transform.SetParent(transform);
            m_Rect_Tap.transform.SetParent(transform);
            GameObject.Destroy(go);
            MainPanelMgr.Instance.Close("GuidePanel");
            if (callBack!= null)
            callBack();
            //this.callBack = null;
        }


        public void SetGuideUp(Action callBack = null)
        {
            //guide1.SetActive(true);
            //guide2.SetActive(false);
            //this.callBack = callBack;
            //tween = shouzhi.transform.DOLocalMoveY(120f,0.5f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);

        }

    }
}
