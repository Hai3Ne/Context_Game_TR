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

    public class UIRoom900SlotCell : UISlotCell
    {
        GameObject effect;
        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game900_2:" + "tb_" + ele);
            ImgElement.SetNativeSize();
            if(ele < 7)
            {
                ImgElement.transform.localScale = new Vector3(1.4f,1.4f,1.4f);
                TfSpine.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            }
            else
            {
                ImgElement.transform.localScale = new Vector3(1.05f, 1.05f, 1f);
                TfSpine.localScale = new Vector3(1.05f, 1.05f, 1f);
            }
        }

        public override void reset()
        {
            if (TfSpine.childCount > 0)
            {
                ToolUtil.StopAnimation(TfSpine.GetChild(0));
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
            go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb" + element);
            go.transform.SetParent(TfSpine, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);
            if (element == 1)
                go.transform.localPosition = new Vector3(-4.3f, 0, 0);
            else if (element == 3)
                go.transform.localPosition = new Vector3(2.3f, 0.7f, 0);
            else if (element == 4)
                go.transform.localPosition = new Vector3(-0.5f, 1, 0);
            else if (element == 5)
                go.transform.localPosition = new Vector3(-0.5f, 1, 0);
            else if (element == 6)
                go.transform.localPosition = new Vector3(-0.6f, 1, 0);
            else if (element == 10)
                go.transform.localPosition = new Vector3(-0.3f, 0, 0);
            else if (element == 12)
                go.transform.localPosition = new Vector3(0f, -3.2f, 0);
            else if (element == 13)
                go.transform.localPosition = new Vector3(0f, 0.8f, 0);
            else if (element == 14)
                go.transform.localPosition = new Vector3(0f, 0.5f, 0);
            else if (element == 15)
                go.transform.localPosition = new Vector3(0f, 0.7f, 0);
            else if (element == 16)
                go.transform.localPosition = new Vector3(0f, 0.5f, 0);
            else if (element == 17)
                go.transform.localPosition = new Vector3(-0.5f, 0.0f, 0);


            ToolUtil.PlayAnimation(go.transform, "a2", true);
            // CommonTools.PlayArmatureAni(go.transform, "a2", 0, () => {});

            effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Effect_1");
            effect.transform.SetParent(EfffectSpine, true);
            effect.transform.localScale = new Vector3(1, 1, 1);
            effect.transform.localPosition = new Vector3(0, 0, 0);
            effect.gameObject.SetActive(true);
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
                    ToolUtil.PlayAnimation(TfSpine.GetChild(0).transform, "a2", (bShow && !bAllTrue) ? false : true);
                    //CommonTools.PlayArmatureAni(TfSpine.GetChild(0), "a2", (bShow && !bAllTrue) ? 1 : 0, () => { });
                    if (EfffectSpine.childCount > 0)
                        ToolUtil.PlayAnimation(EfffectSpine.GetChild(0), "a1", true);
                }

            }
            ImgElement.gameObject.SetActive(!bShow);
            if (bAllTrue)
            {
                ImgElement.gameObject.SetActive(true);
                if (TfSpine.childCount > 0)
                    TfSpine.GetChild(0).gameObject.SetActive(false);
                if (EfffectSpine.childCount > 0)
                    EfffectSpine.GetChild(0).gameObject.SetActive(true);
            }
        }

        public override void onRollFinish()
        {
            if ((element >= 13 && element <= 17) || element == 11)
            {

                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb" + element);
                go.transform.SetParent(TfSpine, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.gameObject.SetActive(true);
                ToolUtil.PlayAnimation(go.transform, "a3", false);
                //CommonTools.PlayArmatureAni(go.transform,"a3", 1, () => { });
                ImgElement.gameObject.SetActive(false);
            }
        }

        public override void onSpinFinish()
        {
            if (element == SlotData_900.elementFree && Game900Model.Instance.bHasFreeGame)//免费小游戏
            {
                ToolUtil.PlayAnimation(TfSpine.GetChild(0).transform, "a2", false);
                //CommonTools.PlayArmatureAni(TfSpine.GetChild(0).transform, "a2", 3, () => { });
                ImgElement.gameObject.SetActive(false);
            }
            else if (Game900Model.Instance.bHasSmallGame)//特俗玩法
            {
                if (Game900Model.Instance.smallGameElementList.Contains(element))
                {
                    ToolUtil.PlayAnimation(TfSpine.GetChild(0).transform, "a2", false);
                    //CommonTools.PlayArmatureAni(TfSpine.GetChild(0).transform, "a2", 1, () => { });
                    ImgElement.gameObject.SetActive(false);
                }
            }
            else if (element == 11 && Game900Model.Instance.bHasJackPot)//奖池
            {
                if (TfSpine.childCount > 0)
                    CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game900/FirstRes/Tb22");
                go.transform.SetParent(TfSpine, true);
                go.gameObject.SetActive(true);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.localPosition = new Vector3(0, 0, 0);


                ToolUtil.PlayAnimation(TfSpine.GetChild(0).transform, "a2", true);
                //CommonTools.PlayArmatureAni(TfSpine.GetChild(0).transform, "a2", 0, () => { });
                ImgElement.gameObject.SetActive(false);
            }
        }
    }
}
