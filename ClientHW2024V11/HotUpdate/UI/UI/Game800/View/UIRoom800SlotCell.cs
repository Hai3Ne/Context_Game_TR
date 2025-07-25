using DragonBones;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class UIRoom800SlotCell : UISlotCell
    {
        public void init(UISlotColumn Column,UITop uiTop_)
        {
            slotColumn = Column;
            uiTop = uiTop_;
        }

        public override void setElement(int ele, int row1 = 0)
        {
           // Debug.Log("=================="+row1+"======"+ele);
            // base.setElement(ele, row1);
           if(ele > 9 || row1 ==100)
            {
                ImgElement.gameObject.SetActive(false);
                return;
            }
            element = ele;
            string imgUrl = "Game800_2:" + "b" + ele;
            if (row1 < 0)
                imgUrl = string.Format("Game800_2:" + "b{0}_2",ele);// "Game800_2:" + "b1_2" + ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite(imgUrl);
            ImgElement.SetNativeSize();
            TxtGold.gameObject.SetActive(ele == SlotData_800.specialelement);
            if (row1 < 0)
                return;
            ImgElement.gameObject.SetActive(ele <=9);
        }

        public override void reset()
        {
           // base.reset();
            if(TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                //EfffectSpine.GetChild(0).gameObject.SetActive(false);
                ImgElement.gameObject.SetActive(true);
            }
            
            SelfTransform.localScale = new Vector3(1, 1, 1);

            if (row < 0)
                return;
            ImgElement.gameObject.SetActive(true);
            //TxtGold.gameObject.SetActive(element == SlotData_800.specialelement);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (TfSpine.childCount > 0)
                return;
            TfSpine.gameObject.SetActive(true);
            //if (uiTop.SpineCell[index].childCount > 0)
            //    return;

           TfSpine.gameObject.SetActive(true);
           GameObject go = null;
            go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game800/FirstRes/Tb0");
            go.transform.SetParent(TfSpine, true);
            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(100, 100, 1);
            if(element == 4)
                go.transform.localPosition = new Vector3(-3.1f, -6.9f, 0);
            else if(element == 3)
                go.transform.localPosition = new Vector3(-4.6f, -7.4f, 0);
            else if (element == 6)
                go.transform.localPosition = new Vector3(-2.4f, 1.4f, 0);
            else
                go.transform.localPosition = new Vector3(0, 0, 0);

            if (element != 11)
                CommonTools.SetArmatureName(go.transform, Game800Model.Instance.iconAni[element - 1]);

            CommonTools.PlayArmatureAni(go.transform, "newAnimation", 0, () => {});

            ImgElement.gameObject.SetActive(false);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    CommonTools.PlayArmatureAni(TfSpine.GetChild(0), "newAnimation", 0, () => { });
                    //EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                }
         
            }
            ImgElement.gameObject.SetActive(!bShow);
            if (bAllTrue)
            {
                ImgElement.gameObject.SetActive(true);
                TfSpine.GetChild(0).gameObject.SetActive(false);
                //EfffectSpine.GetChild(0).gameObject.SetActive(true);
            }
        }
        
    }
}
