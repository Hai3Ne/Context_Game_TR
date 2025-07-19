using DragonBones;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SEZSJ;

namespace HotUpdate
{
   
    public class UIRoom700SlotCell : UISlotCell
    {
        public int cellIndex => slotColumn.column * 3 + row;
        UIRoom700_SlotCpt room700;
        UIRoom700Top top700;

        UnityEngine.Transform elementBg;
        public override void Awake()
        {
            base.Awake();
            elementBg = transform.Find("Bg");
        }

        public void init(UISlotColumn Column,UITop uiTop_)
        {
            slotColumn = Column;
            uiTop = uiTop_;
            top700 = uiTop as UIRoom700Top;
            room700 = Column.slotCpt as UIRoom700_SlotCpt;
        }

        public override void setElement(int ele, int row1 = 0)
        {
            row = row1;
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game700:" + "fruit_icon_" + ele);
            ImgElement.SetNativeSize();
            if (row1 > 0)
            {
                ElementAni?.Kill();
                ElementAni = null;
                SelfTransform.localScale = Vector3.one;
            }
            elementBg.gameObject.SetActive(ele == 9);
            TxtGold.gameObject.SetActive(ele == SlotData_700.specialelement);
        }

        public override void reset()
        {
            if (row < 0 || row >= 3)
                return;
            ImgElement.gameObject.SetActive(true);
            TxtGold.gameObject.SetActive(element == SlotData_700.specialelement);
            if(uiTop.SpineCell[cellIndex].childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(uiTop.SpineCell[cellIndex].GetChild(0).gameObject);
            if (top700.ElementCellBg[cellIndex].childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(top700.ElementCellBg[cellIndex].GetChild(0).gameObject);
   
            if (TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (uiTop.SpineCell[cellIndex].childCount > 0)
                return;
           TfSpine.gameObject.SetActive(true);

            if (TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);

            GameObject goBg = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game700/FirstRes/ElementBg");
            goBg.transform.SetParent(top700.ElementCellBg[cellIndex], true);
            goBg.transform.localScale = Vector3.one*1.1f;
            goBg.transform.localPosition = new Vector3(0, 0, 0);
            DoScaleAni(goBg.transform,0.5f);
            goBg.transform.GetChild(0).gameObject.SetActive(true);
            CommonTools.PlayArmatureAni(goBg.transform.GetChild(0),"Sprite",1);
            CommonTools.PlayArmatureAni(goBg.transform.GetChild(1), "Sprite", 1);


            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game700/FirstRes/Tb"+ element);
            go.transform.SetParent(uiTop.SpineCell[cellIndex], true);
            go.gameObject.SetActive(true);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(0, 0, 0);
            if (element == 1)
            {
                go.transform.localScale = new Vector3(1.14f, 1.14f, 1);
                go.transform.localPosition = new Vector3(-1.9f,-0.3f,0);
            }
            else if(element == 2)
            {
                go.transform.localScale = new Vector3(1.02f, 1.02f, 1);
                go.transform.localPosition = new Vector3(0f, 1.2f, 0);
            }
            else if(element == 3)
            {
                go.transform.localScale = new Vector3(1f, 1f, 1);
                go.transform.localPosition = new Vector3(-9.4f, -12.6f, 0);
            }
            else if(element == 4)
            {
                go.transform.localScale = new Vector3(1.25f, 1.25f, 1);
                go.transform.localPosition = new Vector3(3.3f, 3f, 0);
            }
            else if(element == 5)
            {
                go.transform.localScale = new Vector3(1.1f, 1.1f, 1);
                go.transform.localPosition = new Vector3(4.7f, 3.5f, 0);
            }
            else if(element == 6)
            {
                go.transform.localScale = new Vector3(1.06f, 1.06f, 1);
                go.transform.localPosition = new Vector3(6.2f, 4.6f, 0);
            }
            else if(element == 7)
            {
                go.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                go.transform.localPosition = new Vector3(2.2f, -2.5f, 0);
            }
            else if(element == 8)
            {
                go.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                go.transform.localPosition = new Vector3(4.4f, -0.5f, 0);
            }
            else if(element == 9)
            {
                go.transform.localScale = new Vector3(0.98f, 0.98f, 1);
                go.transform.localPosition = new Vector3(-0.7f, 1.9f, 0);
            }
            else if(element == 10)
            {
                go.transform.localScale = new Vector3(1.75f, 1.75f, 1);
                go.transform.localPosition = new Vector3(3.5f, -3.9f, 0);
            }
            else if(element == 11)
            {
                go.transform.localScale = new Vector3(1f, 1f, 1);
                go.transform.localPosition = new Vector3(0f, 12.5f, 0);
            }

            string aniName = "win";
            if (element == 3)
                aniName = "animation";
            ToolUtil.PlayAnimation(go.transform, aniName);
            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (uiTop.SpineCell[cellIndex].childCount > 0)
            {
                uiTop.SpineCell[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                top700.ElementCellBg[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    string aniName = "win";
                    if (element == 3)
                        aniName = "animation";
                    ToolUtil.PlayAnimation(uiTop.SpineCell[cellIndex].GetChild(0), aniName);
                    DoScaleAni(top700.ElementCellBg[cellIndex].GetChild(0).transform, 0.5f);

                    CommonTools.PlayArmatureAni(top700.ElementCellBg[cellIndex].GetChild(0).transform.GetChild(0), "Sprite", 1);
                    CommonTools.PlayArmatureAni(top700.ElementCellBg[cellIndex].GetChild(0).GetChild(1), "Sprite", 1);
                }        
            }
            ImgElement.gameObject.SetActive(!bShow);
        }
        
        private void DoScaleAni(UnityEngine.Transform tran,float times)
        {
            tran?.DOKill();
            tran.localScale = Vector3.one * 0.8f;
            tran.DOScale(Vector3.one*1.1f,times);
        }

        public override void onSpinFinish()
        {
            if (Game1200Model.Instance.gameStates != 1 && SlotData_1200.elementWild == element && Game1200Model.Instance.gameStates != 3)
            {
        
            }

        }
    }
}
