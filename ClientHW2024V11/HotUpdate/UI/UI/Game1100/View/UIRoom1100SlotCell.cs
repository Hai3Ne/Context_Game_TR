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
    public class UIRoom1100SlotCell : UISlotCell
    {
        UIRoom1100 uiroom1100;
        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
            uiroom1100 = slotColumn.slotCpt as UIRoom1100;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            row = row1;
            if(ele != 10)
            {
                //if (row1 < 0)
                //    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite(string.Format("Game1100:Tb{0}_1", ele));
                //else
                    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1100:" + "Tb" + ele);
            }
            ImgElement.SetNativeSize();
    
            if (TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
            ImgElement.gameObject.SetActive(ele < 10);
            if (ele >= 10)
            {
                element = 10;
                int temp = slotColumn.column * 4 + row;
                UnityEngine.Transform parent = TfSpine;
                if (parent.childCount > 0)
                    return;
                if (row >= 0 && uiroom1100.uitop1100.SpineCell[temp].childCount > 0)
                    return;                    
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Tb" + element);
                go.transform.SetParent(parent, true);
                ToolUtil.PlayAnimation(go.transform, row1 < 0 ? "a0" : "a1",true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(-4.3f, 0, 0);
            }
        }

        public override void reset()
        {
            if (row == 4 || row < 0)
                return;
            int temp = slotColumn.column * 4 + row;
            UnityEngine.Transform parent = element == 10 ? uiroom1100.uitop1100.SpineCell[temp] : uiroom1100.uitop1100.SpineCell2[temp];
            if (parent.childCount > 0)
            {
                if (element != 10)
                    CoreEntry.gGameObjPoolMgr.Destroy(parent.GetChild(0).gameObject);
                else
                {
                    GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Tb" + element);
                    go.transform.SetParent(TfSpine, true);
                   // CommonTools.PlayArmatureAni(go.transform, "a0", 0, () => { });
                    ToolUtil.PlayAnimation(go.transform,"a0", true);
                    go.transform.localScale = new Vector3(1, 1, 1);
                    //go.transform.localScale = new Vector3(100, 100, 100);
                    go.transform.localPosition = new Vector3(-4.3f, 0, 0);
                }
                if (EfffectSpine.childCount > 0)
                    CoreEntry.gGameObjPoolMgr.Destroy(EfffectSpine.GetChild(0).gameObject);
            }
            ImgElement.gameObject.SetActive(true);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            TfSpine.gameObject.SetActive(true);
            GameObject go = null;
            int temp = slotColumn.column * 4 + 3 - row;
            UnityEngine.Transform parent = TfSpine;
            if (parent.childCount > 0)
                go = parent.GetChild(0).gameObject;
            else
            {
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Tb" + element);
                go.transform.SetParent(parent, true);
            }
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(1, 1, 1);
            if(element == 9)
                go.transform.localPosition = new Vector3(-0.2f, 0.5f, 0);
            else if(element == 6)
                go.transform.localPosition = new Vector3(-0.5f, 0.5f, 0);
            else if (element == 7)
                go.transform.localPosition = new Vector3(-0.5f, 0.5f, 0);
            else if (element == 5)
                go.transform.localPosition = new Vector3(-0.4f, 0.7f, 0);
            else
                go.transform.localPosition = new Vector3(0, 0, 0);
            if (element != 10)
                ToolUtil.PlayAnimation(go.transform, "a2", true);
            else
                ToolUtil.PlayAnimation(go.transform, "a1", true);
            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            int temp = slotColumn.column * 4 + row;
            UnityEngine.Transform parent = element == 10? uiroom1100.uitop1100.SpineCell[temp]: uiroom1100.uitop1100.SpineCell2[temp];
            if (parent.childCount > 0)
            {
                if (element != 10)
                    ToolUtil.PlayAnimation(parent.GetChild(0), bShow ? "a2" : "a1", (bShow && !bAllTrue) ? false : true);
            }
        }

        public override void onRollFinish()
        {
        }

        public override void onSpinFinish()
        {
        }
    }
}
