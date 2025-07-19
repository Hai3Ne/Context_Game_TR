using DragonBones;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class UIRoom500SlotCell : UISlotCell
    {
        CommonTop commonTop;

        public override void Awake()
        {
            base.Awake();
            if(MainUIModel.Instance.bNormalGame)
            {
                TxtGold.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                TxtGold.transform.localPosition = new Vector3(5.39f,32.5f,0);
            }
            else
            {
                TxtGold.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                TxtGold.transform.localPosition = new Vector3(5.39f, 20, 0);
            }
        }

        public void init(UISlotColumn Column, CommonTop uiTop_,UITop uitop)
        {
            slotColumn = Column;
            commonTop = uiTop_;
            uiTop = uitop;
        }


        public override void setElement(int ele, int row1 = 0)
        {
            row = row1;
            element = ele;
            ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game50:" + "tb" + ele);
            ImgElement.SetNativeSize();
            TxtGold.gameObject.SetActive(ele == SlotData_500.specialelement);
            if (ele == SlotData_500.specialelement)
            {
                row = row1;
                if (Game500Model.Instance.toSpin.SpecialGame > 0 && row >= 0)
                {
                    int index = slotColumn.slotCpt.cr2i(slotColumn.column, row);
                    int sunValue = Game500Model.Instance.arraySun[index];
                    long temp = (long)(Game500Model.Instance.sunGoldLValue_1[sunValue - 1] * commonTop.Bet * 50);
                    TxtGold.text = ToolUtil.ShowF2Num2(temp);// sunValue + "";
                }
                else
                {
                    int num = UnityEngine.Random.Range(0, 5);
                    long value = (long)(Game500Model.Instance.sunGoldLValue_1[num] * commonTop.Bet * 50);
                    TxtGold.text = ToolUtil.ShowF2Num2(value); //--(rate == 1 and 0.01 or 100)..""
                }
            }
            ImgElement.transform.localPosition = Vector3.zero;
            
            if (element == 9 || element == 10)
            {
                ImgElement.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                TfSpine.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            }
            else if(element == 1 || element == 4 || element == 5 || element == 6 || element == 7)
            {
                ImgElement.transform.localScale = new Vector3(1.1f, 1.1f, 1);
                TfSpine.transform.localScale = new Vector3(1.1f, 1.1f, 1);
            }
            else if(element == 2)
            {
                ImgElement.transform.localScale = new Vector3(1.3f, 1.3f, 1);
                TfSpine.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            }
            else if(element == 3)
            {
                ImgElement.transform.localScale = new Vector3(1.4f, 1.4f, 1);
                TfSpine.transform.localScale = new Vector3(1.4f, 1.4f, 1);
            }
            else if(element == 8)
            {
                ImgElement.transform.localScale = new Vector3(1.2f, 1.2f, 1);
                TfSpine.transform.localScale = new Vector3(1.2f, 1.2f, 1);
            }
            else if(element == 13)
            {
                ImgElement.transform.localPosition = new Vector3(0f,6.7f,0);
                ImgElement.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                TfSpine.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            }
            else
            {
                ImgElement.transform.localScale = Vector3.one;
                TfSpine.transform.localScale = Vector3.one;
            }
        }


        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (element0 > 0)
            {
                element = element0;
                ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game50:" + "tb14");
                TxtGold.text = ToolUtil.ShowF2Num2(sunvalue0);
                if (row0 >= 0)
                    row = row0;
            }
            if (TfSpine.childCount > 0 || uiTop.SpineCell[index].childCount > 0)
                return;
            TfSpine.gameObject.SetActive(true);
            GameObject go = null;
            if (element == 9)
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Tb9");
            else if (element == 10)
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Tb10");
            else if (element == 13)
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Tb13");
            else
                go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Tb0");

            if (element == 14)
            {
                go.transform.SetParent(uiTop.SpineCell[index], true);
                UIRomm500SlotColumn temp = slotColumn as UIRomm500SlotColumn;
                UIRoom500_SlotCpt tempSlotCpt = temp.slotCpt as UIRoom500_SlotCpt;
                tempSlotCpt.mgr.sunGoldEffect.Add(go.transform);
            }
            else
                go.transform.SetParent(TfSpine, true);

            go.gameObject.SetActive(true);
            go.transform.localScale = new Vector3(100, 100, 100);
            go.transform.localPosition = new Vector3(0, 0, 0);

            if (element == 9)
                go.transform.localPosition = new Vector3(3.6f, -1.9f, 0);
            else if (element == 13)
                go.transform.localPosition = new Vector3(-0.15f, 8.4f, 0);
            else if (element == 8)
                go.transform.localPosition = new Vector3(-2.1f, -4.5f, 0);
            else if (element == 6)
                go.transform.localPosition = new Vector3(1.7f, 2.9f, 0);
            else if (element == 11)
                go.transform.localPosition = new Vector3(1.7f, 1.7f, 0);
            else if(element == 12)
                go.transform.localPosition = new Vector3(3.3f, 2.6f, 0);
            else
            { }

            GameObject effect = null;
            if (element != 14)
            {
                if (EfffectSpine.childCount > 0)
                    effect = EfffectSpine.GetChild(0).gameObject;
                else
                {
                    effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Effect0");
                    effect.transform.SetParent(EfffectSpine, true);
                }
                effect.gameObject.SetActive(true);
            }
            else
            {
                effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game500/FirstRes/Effect1");
                effect.transform.SetParent(uiTop.SpineCell[index], true);
                effect.gameObject.SetActive(false);
                UIRomm500SlotColumn temp = slotColumn as UIRomm500SlotColumn;
                UIRoom500_SlotCpt tempSlotCpt = temp.slotCpt as UIRoom500_SlotCpt;
                tempSlotCpt.mgr.GoldEffect.Add(effect.transform);
            }

            effect.transform.localScale = element != 14 ? new Vector3(105, 140, 1) : new Vector3(120, 185, 140);
            effect.transform.localPosition = new Vector3(0, 0, 0);

            if (element != 9 && element != 10 && element != 13)
                 CommonTools.SetArmatureName(go.transform, "tb" + element);

            string aniName = "newAnimation";
            if (element == 9)
                aniName = "aiji_rw2";
            else if (element == 10)
                aniName = "aiji_rw1";
            else if (element == 14)
                aniName = "dz1";
            else if(element == 13)
                aniName = "Sprite";

            int playTimes = 0;
            if (Game500Model.Instance.bInSpecialGame)
                playTimes = 1;

             CommonTools.PlayArmatureAni(go.transform, aniName, playTimes, () =>
            {
                if (callBack != null)
                {
                    effect.gameObject.SetActive(true);
                    CommonTools.PlayArmatureAni(go.transform, "dz2", 1, () => { });
                }
            });

            if (element == 14)
            {
                TxtSpineGold = go.transform.Find("TxtSpineGold").GetComponent<Text>();
                if (TxtSpineGold != null)
                {
                    TxtSpineGold.transform.SetParent(go.transform.GetChild(0), true);
                    TxtSpineGold.gameObject.SetActive(true);
                    TxtSpineGold.text = TxtGold.text;
                    if(MainUIModel.Instance.bNormalGame)
                    {
                        TxtSpineGold.transform.localScale = new Vector3(0.017f, 0.017f, 1);
                        TxtSpineGold.transform.localPosition = new Vector3(0.008140992f, 0.340576f, 0.008010864f);
                    }
                    else
                    {
                        TxtSpineGold.transform.localScale = new Vector3(0.009f, 0.009f, 1);
                        TxtSpineGold.transform.localPosition = new Vector3(0.008140992f, 0.2035751f, 0.008010864f);
                    }
                 
                }
            }
            ImgElement.gameObject.SetActive(false);
        }

        public override void reset()
        {
            if (TfSpine.childCount > 0)
            {
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                CoreEntry.gGameObjPoolMgr.Destroy(EfffectSpine.GetChild(0).gameObject);
            }
            if (Game500Model.Instance.toSpin.SpecialGame <= 0)
            {
                if (uiTop.SpineCell[index].childCount > 0 && row >= 0)
                {
                    UnityEngine.Transform trans = uiTop.SpineCell[index];
                    for (int i = trans.childCount -1; i >=0 ; i--)
                    {
                        GameObject temp = trans.GetChild(i).gameObject;
                        if (temp.name.Contains("Tb") )
                        {
                            if( TxtSpineGold == null)
                            {
                                TxtSpineGold = null;
                                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_exit, null);
                                break;
                            }
                            TxtSpineGold.transform.SetParent(temp.transform, true);
                            TxtSpineGold = null;
                      
                        }
                        CoreEntry.gGameObjPoolMgr.Destroy(temp);
                    }
                }
         
            }
            ImgElement.gameObject.SetActive(true);
            TxtGold.gameObject.SetActive(element == SlotData_500.specialelement);
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (TfSpine.childCount > 0)
            {
                EfffectSpine.GetChild(0).gameObject.SetActive(bShow);
                ImgElement.gameObject.SetActive(!bShow);
                TfSpine.GetChild(0).gameObject.SetActive(bShow);
                if (bShow)
                {
                    string aniName = "newAnimation";
                    if (element == 9)
                        aniName = "aiji_rw2";
                    else if (element == 10)
                        aniName = "aiji_rw1";
                    else if (element == 14)
                        aniName = "dz1";
                    else if (element == 13)
                        aniName = "Sprite";
                    else { }
                    CommonTools.PlayArmatureAni(TfSpine.GetChild(0), aniName, (bShow && !bAllTrue) ? 1 : 0, () => 
                    {
                        ImgElement.gameObject.SetActive(true);
                    });
                }
            }
            if (bAllTrue)
            {
                ImgElement.gameObject.SetActive(true);
                if(TfSpine.childCount > 0)
                {
                    TfSpine.GetChild(0).gameObject.SetActive(false);
                    EfffectSpine.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

      

        public void resetSpecial()
        {
            if (element == 14)
            {
                int index = slotColumn.slotCpt.cr2i(slotColumn.column, row);
                if (index < 0 && row < 0)
                    return;

                int sunValue = Game500Model.Instance.arraySun[index];
                long temp = (long)(Game500Model.Instance.sunGoldLValue_1[sunValue - 1] * commonTop.Bet * 50);
                TxtGold.text = ToolUtil.ShowF2Num2(temp);// sunValue + "";


                TxtGold.gameObject.SetActive(true);
            }
        }
    }
}
