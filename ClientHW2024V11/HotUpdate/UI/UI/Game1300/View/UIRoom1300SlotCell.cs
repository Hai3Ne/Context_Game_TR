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

    public class UIRoom1300SlotCell : UISlotCell
    {
        //GameObject effect;
        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            if(row1 >=0)
            {
    
                ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1300_2:" + "Tb" + ele);
            }               
            else
                ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1300_3:" + "Tb" + ele+"_1");
            ImgElement.SetNativeSize();
            if (ele != 6)
                ImgElement.transform.localPosition = new Vector3(0f,0f,0);
            else
                ImgElement.transform.localPosition = new Vector3(0f, -3.3f, 0);
        }

        public override void reset()
        {
            Canvas canvas = transform.GetComponent<Canvas>();

            if (canvas != null)
                transform.GetComponent<Canvas>().overrideSorting = false;
            if (TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                if (EfffectSpine.childCount > 0)
                    CoreEntry.gGameObjPoolMgr.Destroy(EfffectSpine.GetChild(0).gameObject);
            }
            ImgElement.gameObject.SetActive(true);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            TfSpine.gameObject.SetActive(true);
            GameObject go = null;
            if (TfSpine.childCount > 0)
                return;
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1300/FirstRes/Tb" + element);
                go.transform.SetParent(TfSpine, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);
            if (element == 1)
                go.transform.localPosition = new Vector3(-2.8f, -14.9f, 0);
            else if (element == 2)
                go.transform.localPosition = new Vector3(10f, 4.1f, 0);
            else if (element == 3)
                go.transform.localPosition = new Vector3(9.1f, 2.5f, 0);
            else if (element == 4)
                go.transform.localPosition = new Vector3(9.2f, 3.2f, 0);
            else if (element == 5)
            {
                go.transform.localScale = new Vector3(1f, 1f, 1);
                go.transform.localPosition = new Vector3(-26.7f, -11f, 0);
            }
            else if (element == 6)
                go.transform.localPosition = new Vector3(0f, 103.7f, 0);
            else if (element == 7)
                go.transform.localPosition = new Vector3(16.6f, 1.1f, 0);
            else if (element == 8)
                go.transform.localPosition = new Vector3(-3.3f, -95.3f, 0);
            //else if (element == 13)
            //    go.transform.localPosition = new Vector3(0f, 0.8f, 0);
            //else if (element == 14)
            //    go.transform.localPosition = new Vector3(0f, 0.5f, 0);
            //else if (element == 15)
            //    go.transform.localPosition = new Vector3(0f, 0.7f, 0);
            //else if (element == 16)
            //    go.transform.localPosition = new Vector3(0f, 0.5f, 0);
            //else if (element == 17)
            //    go.transform.localPosition = new Vector3(-0.5f, 0.0f, 0);


            ToolUtil.PlayAnimation(go.transform, "win",false);
           // CommonTools.PlayArmatureAni(go.transform, "a2", 0, () => {});
     
            //effect  = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1300/FirstRes/Effect_1");
            //effect.transform.SetParent(EfffectSpine, true);
            //effect.transform.localScale = new Vector3(1, 1, 1);
            //effect.transform.localPosition = new Vector3(0, 0, 0);
            //effect.gameObject.SetActive(true);
            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                if (EfffectSpine.childCount > 0)
                    EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    ToolUtil.PlayAnimation(TfSpine.GetChild(0).transform, "win", false);
                    if (EfffectSpine.childCount > 0)
                        ToolUtil.PlayAnimation(EfffectSpine.GetChild(0),"a1",true);
                }
               
            }
            ImgElement.gameObject.SetActive(false);
        }

        public override void onRollFinish()
        {
            if (element == 8)
                transform.GetComponent<Canvas>().overrideSorting = true;
        }

        public override void onSpinFinish()
        {
       
        }
    }
}
