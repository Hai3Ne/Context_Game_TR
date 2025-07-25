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

    public class UIRoom900SmallGameSlotCell : UISlotCell
    {
        GameObject effect;
        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
        }

        public override void setElement(int ele, int row1 = 0)
        {
           // base.setElement(ele, row1);
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game900_2:" + "tb_" + ele);
            ImgElement.SetNativeSize();
            if(row1 > 0)
            {
                ElementAni?.Kill();
                ElementAni = null;
                //if (row1 == 1)
                //    SelfTransform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
                //else
                //    SelfTransform.localScale = Vector3.one;
            }

           // TxtGold.gameObject.SetActive(ele == SlotData_900.specialelement);

        }

        public override void reset()
        {
           // base.reset();
            if(TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                //Destroy(TfSpine.GetChild(0).gameObject);
                if (EfffectSpine.childCount > 0)
                    EfffectSpine.GetChild(0).gameObject.SetActive(false);
            }
            if (SelfTransform.localScale.x > 1)
            {
                ElementAni = SelfTransform.DOScale(Vector3.one, 2f).OnComplete(() =>
                {
                    SelfTransform.localScale = Vector3.one;
                });
            }

            ImgElement.gameObject.SetActive(true);
           // TxtGold.gameObject.SetActive(element == SlotData_900.specialelement);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            TfSpine.gameObject.SetActive(true);
            GameObject go = null;
            if (TfSpine.childCount > 0)
            {
                go = TfSpine.GetChild(0).gameObject;
            }
            else
            {
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb" + element);
                go.transform.SetParent(TfSpine, true);
            }
           
           //      go = Instantiate(SpineAniItem);
   
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(100, 100, 100);
            if(element == 1)
                go.transform.localPosition = new Vector3(-4.3f, 0, 0);
            else if(element == 4)
                go.transform.localPosition = new Vector3(-0.5f, 1, 0);
            else if (element == 3)
                go.transform.localPosition = new Vector3(2.3f, 0.7f, 0);
            else
                go.transform.localPosition = new Vector3(0, 0, 0);

             CommonTools.PlayArmatureAni(go.transform, "a2", 0, () => {});
            //if (EfffectSpine.childCount > 0)
            //    effect = EfffectSpine.GetChild(0).gameObject;
            //else
            //{
            //    effect = Instantiate(EfffectSpineItem);
            //    effect.transform.SetParent(EfffectSpine, true);
            //    effect.transform.localScale = new Vector3(97, 97, 100);
            //    effect.transform.localPosition = new Vector3(0, 0, 0);
            //}
            //effect.gameObject.SetActive(true);
            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                     CommonTools.PlayArmatureAni(TfSpine.GetChild(0), "a2", (bShow && !bAllTrue)?1:0, () => { });
                }
            }
            ImgElement.gameObject.SetActive(!bShow);
            if (bAllTrue)
            {
                ImgElement.gameObject.SetActive(true);
                TfSpine.GetChild(0).gameObject.SetActive(false);
                EfffectSpine.GetChild(0).gameObject.SetActive(true);
            }
        }

        public override void onRollFinish()
        {
            //if(element >=13 && element <=17)
            //{
            //    GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb" + element);

            //    //      go = Instantiate(SpineAniItem);
            //    go.transform.SetParent(TfSpine, true);
            //    go.gameObject.SetActive(true);
            //    go.transform.localScale = new Vector3(100, 100, 100);
            //    go.transform.localPosition = new Vector3(0, 0, 0);
            //    go.gameObject.SetActive(true);
            //    CommonTools.PlayArmatureAni(go.transform, "a3", 1, () => { });
            //    ImgElement.gameObject.SetActive(false);
            //}
        }

        public override void onSpinFinish()
        {
            //if(element == 17)
            //{
            //    CommonTools.PlayArmatureAni(TfSpine.GetChild(0).transform, "a2", 0, () => { });
            //    ImgElement.gameObject.SetActive(false);
            //}

                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb" + element);

                //      go = Instantiate(SpineAniItem);
                go.transform.SetParent(TfSpine, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.gameObject.SetActive(true);
                ToolUtil.PlayAnimation(go.transform, "a2", false);
                //CommonTools.PlayArmatureAni(go.transform, "a2", 1, () => { });
                ImgElement.gameObject.SetActive(false);
        }
    }
}
