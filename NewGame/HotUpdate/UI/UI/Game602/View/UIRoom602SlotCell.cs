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

    public class UIRoom602SlotCell : UISlotCell
    {
        private UnityEngine.Transform Bg;
        public UnityEngine.Transform Trans_Mask;
        public override void Awake()
        {
            base.Awake();
            Bg = transform.Find("Bg");
            Trans_Mask = transform.Find("Trans_Mask");
        }

        public void init(UISlotColumn Column,UITop uiTop_)
        {
            slotColumn = Column;
            uiTop = uiTop_;
        }

        public override void setElement(int ele, int row1 = 0)
        {
           // base.setElement(ele, row1);
            element = ele;
            Debug.LogError(("ele:" + "ele" + ele));
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game602New:" + "icon_" + ele);
            ImgElement.SetNativeSize();
            if(row1 > 0)
            {
                ElementAni?.Kill();
                ElementAni = null;
            }   
            TxtGold.gameObject.SetActive(ele == SlotData_602.specialelement);

        }

        public override void reset()
        {
            if(TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
            Bg.gameObject.SetActive(false);
            ImgElement.gameObject.SetActive(true);
            TxtGold.gameObject.SetActive(element == SlotData_602.specialelement);
            Trans_Mask.gameObject.SetActive(true);
            Trans_Mask.localEulerAngles = Vector3.zero;
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (TfSpine.childCount > 0)
                return;
            TfSpine.gameObject.SetActive(true);
            if (uiTop.SpineCell[index].childCount > 0)
                return;

            TfSpine.gameObject.SetActive(true);
            GameObject go = null;

            go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game602/FirstRes/Tb_1");
            go.transform.SetParent(TfSpine, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = Vector3.one;
            //Debug.LogError(("Game602New:" + "icon_" + element));
            go.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game602New:" + "icon_" + element);
            go.GetComponent<Image>().SetNativeSize();
            go.transform.localPosition = Vector3.zero;
            //if (element == 10)
            //    go.transform.localPosition = new Vector3(4.1f, 0, 0);
            //else if (element == 8)
            //    go.transform.localPosition = new Vector3(-1.4f, 0, 0);
            //else if (element == 1)
            //    go.transform.localPosition = new Vector3(-3.6f, -3.6f, 0);
            //else
            //    go.transform.localPosition = new Vector3(0, 0, 0);
            //if (element != 2)
            //    CommonTools.SetArmatureName(go.transform, Game602Model.Instance.AniNames[element]);

            //CommonTools.PlayArmatureAni(go.transform, "newAnimation", 0, () => { });
            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    TfSpine.GetChild(0).localScale = Vector3.one;
                    TfSpine.GetChild(0).DOScale(new Vector3(1.15f,1.15f,1),0.4f).SetEase(Ease.Linear).SetLoops(2,LoopType.Yoyo);
                    //CommonTools.PlayArmatureAni(TfSpine.GetChild(0), "newAnimation", 0, () => { });
                    //EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                }

            }
            Bg.gameObject.SetActive(bShow);
            ImgElement.gameObject.SetActive(!bShow);
            //if (bAllTrue)
            //{
            //    Bg.gameObject.SetActive(true);
            //    ImgElement.gameObject.SetActive(true);
            //    TfSpine.GetChild(0).gameObject.SetActive(false);
            //    //EfffectSpine.GetChild(0).gameObject.SetActive(true);
            //}
        }
        
    }
}
