using DG.Tweening;
using DragonBones;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class UISlotCell : MonoBehaviour
    {
        public Image ImgElement;
        public UnityEngine.Transform TfSpine;
        public int element = -1;
        protected UISlotColumn slotColumn;
        public int row = -1;
      
        public UnityEngine.Transform SelfTransform;
        public RectTransform rtf;
        protected UnityEngine.Transform EfffectSpine;
        protected Text TxtGold;
        protected Text TxtSpineGold;
        protected UITop uiTop;
        public int index = 1;

        public Tweener ElementAni;
        public virtual void Awake()
        {
            ImgElement = transform.Find("Element").GetComponent<Image>();
            TfSpine = transform.Find("Spine");
            SelfTransform = transform;
            rtf = transform.GetComponent<RectTransform>();
            EfffectSpine = transform.Find("EfffectSpine");
            TxtGold = transform.Find("Element/TxtGold").GetComponent<Text>();
        }

        public virtual void setElement(int ele, int row1 = 0)
        {
        }

        public virtual void reset()
        {            
        }

        public virtual void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
        }
        public virtual void onRollFinish()
        {
        }
        public virtual void onSpinFinish()
        {
        }

        public virtual void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
     
        }
    }
}
