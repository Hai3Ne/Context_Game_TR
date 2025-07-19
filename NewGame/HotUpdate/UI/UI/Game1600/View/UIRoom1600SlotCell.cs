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

    public class UIRoom1600SlotCell : UISlotCell
    {
        UIRoom1600 uiroom1600;
        GameObject elementGo;
        GameObject effect;
        private UnityEngine.Transform ElementBg;
        private Text TxtGold2;
        private Text TxtGold3;
        public int cellIndex 
        {
            get 
            {
                if (slotColumn.column == 0)
                    return  row;
                else if(slotColumn.column == 1)
                    return  3 + row;
                else
                    return 7 + row;
            }
        }

        public long goldValue;
        public override void Awake()
        {
            base.Awake();
           
            Button BtnClick = transform.Find("BtnClick").GetComponent<Button>();
            BtnClick.onClick.AddListener(ClickElement);
            ImgElement.transform.localScale = new Vector3(0.7f,0.7f,1);
            ElementBg = transform.Find("ElementBg");
            TxtGold2 = transform.Find("Element/TxtGold2").GetComponent<Text>();
            TxtGold3 = transform.Find("Element/TxtGold3").GetComponent<Text>();
            TxtGold.transform.localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1, 1, 1) : new Vector3(0.5f,0.5f,1);
            TxtGold2.transform.localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1, 1, 1) : new Vector3(0.5f, 0.5f, 1);
            TxtGold3.transform.localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1, 1, 1) : new Vector3(0.5f, 0.5f, 1);
        }

        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
            uiroom1600 = slotColumn.slotCpt as UIRoom1600;
        }

        private void ClickElement()
        {
            if (element <= 0 || element == 8)
                return;
            if(uiroom1600.StateSlot == slotState.Idle)
                uiroom1600.uitop1600.OpenGameTips(element, cellIndex);
        }

        int tempRate = 0;

        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            row = row1;
            SetGoldValueColor(true);
            if(row1 >= 0)
            {
                if (ele != 0)
                    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1600_2:" + "Tb" + ele);
                if (ele == 8)
                {
                    if (row >= 0 && Game1600Model.Instance.n64NumberPowerGold > 0)
                    {
                        tempRate = Game1600Model.Instance.goldValueResult[cellIndex];
                        goldValue = Game1600Model.Instance.goldValueResult[cellIndex] * uiroom1600.uitop1600.Bet * 10;
                        TxtGold.text = ToolUtil.ShowF2Num2(goldValue);
                        TxtGold2.text = ToolUtil.ShowF2Num2(goldValue);
                        TxtGold3.text = ToolUtil.ShowF2Num2(goldValue);
                    }
                    else
                    {
                        int num = UnityEngine.Random.Range(0, 5);
                        tempRate = Game1600Model.Instance.goldValue[num];
                        long temp = Game1600Model.Instance.goldValue[num] * uiroom1600.uitop1600.Bet * 10;
                        goldValue = temp;
                        TxtGold.text = ToolUtil.ShowF2Num2(temp);
                        TxtGold2.text = ToolUtil.ShowF2Num2(temp);
                        TxtGold3.text = ToolUtil.ShowF2Num2(temp);
                    }
                }
            }
            else
            {
                if (ele != 0)
                    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1600_3:" + "Tb" + ele);
                if (ele == 8)
                {
                    if (row >= 0 && Game1600Model.Instance.n64NumberPowerGold > 0)
                    {
                        goldValue = Game1600Model.Instance.goldValueResult[cellIndex] * uiroom1600.uitop1600.Bet * 10;
                        TxtGold.text = ToolUtil.ShowF2Num2(goldValue);
                        TxtGold2.text = ToolUtil.ShowF2Num2(goldValue);
                        TxtGold3.text = ToolUtil.ShowF2Num2(goldValue);
                    }
                    else
                    {
                        int num = UnityEngine.Random.Range(0, 5);
                        long temp = Game1600Model.Instance.goldValue[num] * uiroom1600.uitop1600.Bet * 10;
                        TxtGold.text = ToolUtil.ShowF2Num2(temp);
                        TxtGold2.text = ToolUtil.ShowF2Num2(temp);
                        TxtGold3.text = ToolUtil.ShowF2Num2(temp);
                    }
                }
            }
            ImgElement.gameObject.SetActive(ele != 0);
            ElementBg.gameObject.SetActive(ele == 0);
            TxtGold.gameObject.SetActive(ele == 8);
            TxtGold2.gameObject.SetActive(ele == 8);
            TxtGold3.gameObject.SetActive(ele == 8);
            if(ele == 8)
            {
                ShowGoldText(tempRate);
            }
            ImgElement.SetNativeSize();
            ImgElement.transform.localScale = new Vector3(1.3f, 1.3f, 1);// ele != 8 ? new Vector3(1.3f,1.3f,1):new Vector3(1.3f,1.3f,1);
            if(element == 7)
                ImgElement.transform.localScale = new Vector3(1.28f, 1.28f, 1);
        }

        private void ShowGoldText(int type)
        {
            if(type < 10)
            {
                TxtGold.gameObject.SetActive(true);
                TxtGold2.gameObject.SetActive(false);
                TxtGold3.gameObject.SetActive(false);
            }
            else if(type < 100 )
            {
                TxtGold.gameObject.SetActive(false);
                TxtGold2.gameObject.SetActive(true);
                TxtGold3.gameObject.SetActive(false);
            }
            else
            {
                TxtGold.gameObject.SetActive(false);
                TxtGold2.gameObject.SetActive(false);
                TxtGold3.gameObject.SetActive(true);
            }          
        }

        public override void reset()
        {
            if(row <=(slotColumn.column !=1?2:3) && row >= 0)
            {
                if((Game1600Model.Instance.gameStates == 3 && Game1600Model.Instance.toSpin.WinGold > 0) || Game1600Model.Instance.gameStates ==0 || Game1600Model.Instance.gameStates == 2)
                {
                    if (uiroom1600.elementList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1600.elementList[cellIndex].GetChild(0).gameObject);
                    if (uiroom1600.elementBgList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1600.elementBgList[cellIndex].GetChild(0).gameObject);
                    if (TfSpine.childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                }
            }
            ImgElement.transform.localPosition = Vector3.zero;
            if (Game1600Model.Instance.lastFreeGameIndex != 1)
                ImgElement.gameObject.SetActive(true);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if (uiroom1600.elementBgList[cellIndex].childCount <= 0)
            {
                effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/Effect_2");
                effect.transform.SetParent(uiroom1600.elementBgList[cellIndex], true);
                effect.transform.localScale = Vector3.one;
                effect.transform.localPosition = new Vector3(0, 0, 0);
                effect.gameObject.SetActive(true);
                ImgElement.gameObject.SetActive(false);
                effect.transform.GetChild(1).GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Game1600_2:" + "Tb" + element + "_b");
                effect.transform.GetChild(1).GetComponent<Image>().SetNativeSize();
                CommonTools.PlayArmatureAni(effect.transform.GetChild(0), "start", 1, () =>
                   {
                       CommonTools.PlayArmatureAni(effect.transform.GetChild(0), "idle", 0);
                   });
                CommonTools.PlayArmatureAni(effect.transform.GetChild(2), "newAnimation", 0);
            }
            if (uiroom1600.elementList[cellIndex].childCount > 0)
            {
                ImgElement.gameObject.SetActive(false);
                return;
            }
        }

        private void CloneElementEfffect(int ele)
        {
            elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/Tb" + element);
            elementGo.transform.SetParent(uiroom1600.elementList[cellIndex], true);
            elementGo.gameObject.SetActive(true);
            elementGo.transform.localScale = new Vector3(0.65f, 0.65f, 1);
            elementGo.transform.localPosition = new Vector3(0, 0, 0);
            if (element == 7)
                elementGo.transform.localScale = new Vector3(0.64f, 0.64f, 1);
        }


        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (uiroom1600.elementList[cellIndex].childCount > 0)
            {               
                if (uiroom1600.elementBgList[cellIndex].childCount > 0)
                {
                    CanvasGroup canvasGroup = uiroom1600.elementBgList[cellIndex].GetChild(0).GetComponent<CanvasGroup>();
                    canvasGroup.transform.DOKill();
                    if (Game1600Model.Instance.lines.Count == 1)
                    {
                        uiroom1600.elementList[cellIndex].GetChild(0).gameObject.SetActive(true);
                        uiroom1600.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(true);

                        if (bShow)
                        {
                            DOTween.To(() => 0.7f, (value) => {
                                canvasGroup.alpha = value;
                            }, 1, 0.5f).OnComplete(() => {
                                canvasGroup.alpha = 1;
                            }).SetEase(Ease.Linear);
                        }
                        else
                            canvasGroup.alpha = 0.4f;
                    }
                    else
                    {
                        uiroom1600.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                        uiroom1600.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);

                        if (bShow)
                        {
                            DOTween.To(() => 0.4f, (value) => {
                                canvasGroup.alpha = value;
                            }, 1, 0.5f).OnComplete(() => {
                                canvasGroup.alpha = 1;
                            }).SetEase(Ease.Linear);
                        }                  
                    }
                }
                
                if (bShow)
                {
                    string aniName_a = "spawn";
                    string aniName = "win";
                    if (element == 7)
                    {
                        aniName = "win_idle";
                        ToolUtil.PlayAnimation(uiroom1600.elementList[cellIndex].GetChild(0), aniName);
                    }
                    else
                        ToolUtil.PlayTwoAnimation(uiroom1600.elementList[cellIndex].GetChild(0), aniName_a, aniName);
                
      
                    CommonTools.PlayArmatureAni(uiroom1600.elementBgList[cellIndex].GetChild(0).GetChild(2), "newAnimation", 1);
                }
                if (bAllTrue)
                {
                    uiroom1600.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                    uiroom1600.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                }
            }
            ImgElement.gameObject.SetActive(!bShow);
        }
    

        public override void onRollFinish()
        {

            if (row < 0 || element == 8 || element == 0)
                return;
     
            CloneElementEfffect(element);
            string aniName = "spawn";
            if (element == 2)
                aniName = "win";

            if (element == 2)
            {
                elementGo.gameObject.SetActive(false);
                ImgElement.transform.DOShakePosition(1.2f, 3f, 7, 5).SetLoops(1, LoopType.Yoyo).OnComplete(() =>
                    ImgElement.transform.DOKill());
                return;
            }

            ImgElement.gameObject.SetActive(false);
            bool bLoop = false;
            if (element == 7)
                bLoop = true;

            ToolUtil.PlayAnimation(elementGo.transform, aniName, false, ()=> 
            {
                if(Game1600Model.Instance.toSpin.WinGold <= 0 && elementGo != null)
                {
                    if(element!= 7)
                    {
                        CoreEntry.gGameObjPoolMgr.Destroy(elementGo);
                        ImgElement.gameObject.SetActive(true);
                    }
                    else
                    {
                        if(Game1600Model.Instance.lines.Count <= 0)
                            ToolUtil.PlayAnimation(elementGo.transform, "win_idle", true);
                    }
                    
                }
            });
        }

        public override void onSpinFinish()
        {
            if (Game1600Model.Instance.gameStates != 1 && 8== element && Game1600Model.Instance.gameStates != 3 && row >=0)
            {
                MoveAni();
                //if (uiroom1600.elementList[cellIndex].childCount <= 0)
                //{
                //    elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/Tb7");
                //    elementGo.transform.SetParent(uiroom1600.elementList[cellIndex], true);
                //    elementGo.transform.localScale = new Vector3(0.65f, 0.65f, 1);
                //    elementGo.transform.localPosition = new Vector3(0, 0, 0);
                //}
                //else
                //    elementGo = uiroom1600.elementList[cellIndex].GetChild(0).gameObject;
                //ImgElement.gameObject.SetActive(false);
                //ToolUtil.PlayAnimation(elementGo.transform, "win_idle", true);
            }
        }


        private void MoveAni()
        {
            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/FlyAni");
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(200, 200, 100);
            go.transform.localPosition = Vector3.zero;
            GameObject go1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/FlyAni");
            go1.transform.SetParent(transform);
            go1.transform.localScale = new Vector3(100, 100, 100);
            go1.transform.localPosition = Vector3.zero;
            CommonTools.PlayArmatureAni(go.transform, "dj", 1);
            CommonTools.PlayArmatureAni(go1.transform, "tw", 1);
            Game1600Model.Instance.MovePos(go1.transform, uiroom1600.GetMoveTarget(), () =>
            {
                CoreEntry.gGameObjPoolMgr.Destroy(go);
                CoreEntry.gGameObjPoolMgr.Destroy(go1);
            },0.4f,true,true);
        }

        public void FlyGoldValue(Callback callBack = null)
        {
            if(element == 8)
            {
                SetGoldValueColor(false);
                GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1600/FirstRes/GoldValue");
  
                go.transform.GetChild(0).GetComponent<Text>().text = TxtGold.text;
                go.transform.GetChild(1).GetComponent<Text>().text = TxtGold.text;
                go.transform.GetChild(2).GetComponent<Text>().text = TxtGold.text;
                go.transform.GetChild(1).localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1.3f,1.3f,1):new Vector3(0.95f,0.95f,1);
                go.transform.GetChild(2).localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1.3f, 1.3f, 1) : new Vector3(0.95f, 0.95f, 1);

                if (tempRate < 10)
                {
                    go.transform.GetChild(0).gameObject.SetActive(true);
                    go.transform.GetChild(1).gameObject.SetActive(false);
                    go.transform.GetChild(2).gameObject.SetActive(false);
                }
                else if (tempRate < 100)
                {
                    go.transform.GetChild(0).gameObject.SetActive(false);
                    go.transform.GetChild(1).gameObject.SetActive(true);
                    go.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    go.transform.GetChild(0).gameObject.SetActive(false);
                    go.transform.GetChild(1).gameObject.SetActive(false);
                    go.transform.GetChild(2).gameObject.SetActive(true);
                }

                go.transform.localPosition = TxtGold.transform.position;
                go.transform.SetParent(uiroom1600.uitop1600.GetMoveGoldPos());
                go.transform.localScale = Vector3.one ;
                go.transform.DOMove(uiroom1600.uitop1600.GetMoveGoldPos().position,0.25f).SetEase(Ease.Linear).OnComplete(()=>
                {
                    go.transform.DOKill();
                    CoreEntry.gGameObjPoolMgr.Destroy(go);
                    callBack?.Invoke();
                });
            }
        }

        public void SetGoldValueColor(bool normal)
        {
            if (normal)
            {
                TxtGold.color = new Color32(255, 255, 255, 255);
                TxtGold2.color = new Color32(255, 255, 255, 255);
                TxtGold3.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                TxtGold.color = new Color32(222, 222, 222, 255);
                TxtGold2.color = new Color32(222, 222, 222, 255);
                TxtGold3.color = new Color32(222, 222, 222, 255);
            }     
        }
    }

}
