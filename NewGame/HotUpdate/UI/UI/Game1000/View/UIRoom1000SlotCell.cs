using DG.Tweening;
using DragonBones;
using SEZSJ;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class UIRoom1000SlotCell : UISlotCell
    {
        GameObject effect;
        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            row = row1;
            if (row1 < 0)
            {
                UIRoom1000 uiroom1000 = slotColumn.slotCpt as UIRoom1000;
                //ImgElement.material = uiroom1000.uitop1000.m_UIGrey;
            }
            else
                ImgElement.material = null;
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1000_2:" + "tb_" + ele);
            ImgElement.SetNativeSize();
            if (ele == 10)
                ImgElement.transform.localPosition = new Vector3(-17.4f, 5.1f, 0);
            else
                ImgElement.transform.localPosition = Vector3.zero;
        }

        public override void reset()
        {
           // base.reset();
            if(TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                //Destroy(TfSpine.GetChild(0).gameObject);
                if (EfffectSpine.childCount > 0)
                    CoreEntry.gGameObjPoolMgr.Destroy(EfffectSpine.GetChild(0).gameObject);
            }

            ImgElement.gameObject.SetActive(true);
           // TxtGold.gameObject.SetActive(element == SlotData_1000.specialelement);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (EfffectSpine.childCount > 0)
                return;
            TfSpine.gameObject.SetActive(true);

            effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1000/FirstRes/Effect0");
            effect.transform.SetParent(EfffectSpine, true);
            effect.transform.localScale = new Vector3(97f, 97f, 1);
            effect.transform.localPosition = new Vector3(0, 0, 0);
            effect.gameObject.SetActive(true);

            ToolUtil.Play3DAnimation(effect.transform, "a1", true);
            GameObject go = null;
            if (TfSpine.childCount > 0)
                go = TfSpine.GetChild(0).gameObject;
            else
            {
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1000/FirstRes/Tb" + element);
                go.transform.SetParent(TfSpine, true);
            }   
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(100, 100, 1);
            if(element == 9)
                go.transform.localPosition = new Vector3(0.16f, -3.72f, 0);
            else if (element == 1)
                go.transform.localPosition = new Vector3(-0.2f, -9.7f, 0);
            else if (element == 2)
                go.transform.localPosition = new Vector3(-1.6f, -4.5f, 0);
            else if (element == 3)
                go.transform.localPosition = new Vector3(0.2f, -4.1f, 0);
            else if (element == 4)
                go.transform.localPosition = new Vector3(1.3f, -5.3f, 0);
            else if (element == 5)
                go.transform.localPosition = new Vector3(-7.8f,8.5f, 0);
            else if (element == 6)
                go.transform.localPosition = new Vector3(-3.9f, -3.1f, 0);
            else if (element == 7)
                go.transform.localPosition = new Vector3(0, -11, 0);
            else if (element == 8)
                go.transform.localPosition = new Vector3(0.3f, -6.9f, 0);
            else if (element == 10)
                go.transform.localPosition = new Vector3(-1.7f, -6.2f, 0);
            else
                go.transform.localPosition = new Vector3(0, 0, 0);
            ToolUtil.Play3DAnimation(go.transform, "a2");

            SkeletonAnimation self = go.GetComponent<SkeletonAnimation>();

           // self.
            //修改层级
            //spine.node.getComponent(UnityEngine.Renderer).sortingOrder = 2;

            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                TfSpine.GetChild(0).gameObject.SetActive(bShow);          
                if (bShow)
                {
                    ToolUtil.Play3DAnimation(effect.transform, "a1", true);
                    ToolUtil.Play3DAnimation(TfSpine.GetChild(0),"a2",false);
                    TfSpine.GetChild(0).GetComponent<MeshRenderer>().sortingOrder = slotColumn.column*3 + row+4;
                }
            }
            ImgElement.gameObject.SetActive(!bShow);
        }

        public override void onRollFinish()
        {
        }

        public override void onSpinFinish()
        {
        }
    }
}
