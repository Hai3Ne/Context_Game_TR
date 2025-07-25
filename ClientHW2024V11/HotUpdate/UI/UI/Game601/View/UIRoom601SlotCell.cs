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

    public class UIRoom601SlotCell : UISlotCell
    {
        public void init(UISlotColumn Column, UITop uiTop_)
        {
            slotColumn = Column;
            uiTop = uiTop_;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game601:" + "soccer_icon_" + ele);
            ImgElement.SetNativeSize();
            TxtGold.gameObject.SetActive(ele == SlotData_601.specialelement);
        }

        public override void reset()
        {
            if (TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                CoreEntry.gGameObjPoolMgr.Destroy(EfffectSpine.GetChild(0).gameObject);
            }
            ImgElement.gameObject.SetActive(true);
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

            go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game601/FirstRes/Tb0");
            go.transform.SetParent(TfSpine, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(100, 100, 100);
            //if(element == 11)
            //    go.transform.localPosition = new Vector3(4.1f, 0, 0);
            //else if(element == 3)
            //    go.transform.localPosition = new Vector3(-1.4f, 0, 0);
            //else
                go.transform.localPosition = new Vector3(0, 0, 0);
            //if (element != 11)
            CommonTools.SetArmatureName(go.transform, Game601Model.Instance.AniNames[element]);

            CommonTools.PlayArmatureAni(go.transform, "newAnimation", 0, () => { });
            ImgElement.gameObject.SetActive(false);

            GameObject effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game601/FirstRes/Effect_1");
            effect.transform.SetParent(EfffectSpine, true);
            effect.transform.localScale = new Vector3(220, 170, 1);
            effect.transform.localPosition = new Vector3(0, 0, 0);
            effect.gameObject.SetActive(true);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    CommonTools.PlayArmatureAni(TfSpine.GetChild(0), "newAnimation", 1, () => { });
                    CommonTools.PlayArmatureAni(EfffectSpine.GetChild(0), "Sprite", 0, () => { });
                }

            }
            ImgElement.gameObject.SetActive(!bShow);
            //if (bAllTrue)
            //{
            //    ImgElement.gameObject.SetActive(true);
            //    TfSpine.GetChild(0).gameObject.SetActive(false);
            //    EfffectSpine.GetChild(0).gameObject.SetActive(true);
            //}
        }

    }
}
