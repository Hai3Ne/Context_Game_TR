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

    public class UIRoom1200SlotCell : UISlotCell
    {
        UIRoom1200 uiroom1200;
        GameObject elementGo;
        GameObject effect;
        public int cellIndex => slotColumn.column * 3 + row;
        public override void Awake()
        {
            base.Awake();
           
            Button BtnClick = transform.Find("BtnClick").GetComponent<Button>();
            BtnClick.onClick.AddListener(ClickElement);
        }

        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
            uiroom1200 = slotColumn.slotCpt as UIRoom1200;
        }

        private void ClickElement()
        {
            if (element <= 0)
                return;
            if(uiroom1200.StateSlot == slotState.Idle)
                uiroom1200.uitop1200.OpenGameTips(element,slotColumn.column*3+row);
        }


        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            row = row1;
            ImgElement.gameObject.SetActive(ele != 0);
            if (ele == 0)
            {
            }
            else
            {
                if(row1 < 0)
                {
                    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1200_2:" + "Tb" + ele+"_1");
                }
                else
                {
                    ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1200:" + "Tb" + ele);
                }
       
                ImgElement.SetNativeSize();
            }
            if (row >= 0&& row <= 2)
            {
                if (uiroom1200.uitop1200.elementList[cellIndex].childCount > 0)
                    ImgElement.gameObject.SetActive(false);

                ImgElement.transform.localScale = element == 7 ? new Vector3(1.2f,1.2f,1):Vector3.one;
            }
        }

        public override void reset()
        {
            if(row <=2 && row >= 0)
            {
                if((Game1200Model.Instance.gameStates == 3 && Game1200Model.Instance.toSpin.WinGold > 0) || Game1200Model.Instance.gameStates ==0 || Game1200Model.Instance.gameStates == 2)
                {
                    if (uiroom1200.uitop1200.elementList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1200.uitop1200.elementList[cellIndex].GetChild(0).gameObject);
                    if (uiroom1200.uitop1200.elementBgList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0).gameObject);
                    if (TfSpine.childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                }
            }
            ImgElement.gameObject.SetActive(true);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if(uiroom1200.uitop1200.elementBgList[cellIndex].childCount<=0)
            {
                effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Effect_2");
                effect.transform.SetParent(uiroom1200.uitop1200.elementBgList[cellIndex], true);
                effect.transform.localScale = new Vector3(45f, 45f, 1);
                effect.transform.localPosition = new Vector3(0, 0, 0);
                effect.gameObject.SetActive(true);
                ImgElement.gameObject.SetActive(false);
                CommonTools.PlayArmatureAni(effect.transform, "newAnimation", 0);
            }
            if (uiroom1200.uitop1200.elementList[cellIndex].childCount > 0)
            {
                ImgElement.gameObject.SetActive(false);
                return;
            }

            if (TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);

            elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Tb" + element);
            elementGo.transform.SetParent(uiroom1200.uitop1200.elementList[cellIndex], true);
            elementGo.gameObject.SetActive(true);
            elementGo.transform.localScale = new Vector3(0.68f, 0.68f, 1);
            elementGo.transform.localPosition = new Vector3(0, 0, 0);
            ToolUtil.PlayAnimation(elementGo.transform, "spawn", false, () =>
            {
                string aniName = "win_idle";
                if (element == 7)
                    aniName = "idle";
                ToolUtil.PlayAnimation(elementGo.transform, aniName, true);
            });
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (uiroom1200.uitop1200.elementList[cellIndex].childCount > 0)
            {
               
                if (uiroom1200.uitop1200.elementBgList[cellIndex].childCount > 0)
                {
                    CanvasGroup canvasGroup = uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0).GetComponent<CanvasGroup>();
                    canvasGroup.transform.DOKill();
                    if (Game1200Model.Instance.lines.Count == 1)
                    {
                        uiroom1200.uitop1200.elementList[cellIndex].GetChild(0).gameObject.SetActive(true);
                        uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(true);

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
                        uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                        uiroom1200.uitop1200.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);

                        if(bShow)
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
                    string aniName = "win_idle";
                    if (element == 7)
                        aniName = "idle";
                    ToolUtil.PlayTwoAnimation(uiroom1200.uitop1200.elementList[cellIndex].GetChild(0), "win", aniName);
                    CommonTools.PlayArmatureAni(uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0), "newAnimation", 0);
                }
                if (bAllTrue)
                {
                    uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                    uiroom1200.uitop1200.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                }
            }
            ImgElement.gameObject.SetActive(!bShow);
        }

        public void ReSetSpecilaElement()
        {
            if (uiroom1200.uitop1200.elementList[cellIndex].childCount > 0)
            {
                string aniName = "win_idle";
                if (element == 7)
                    aniName = "idle";
                ToolUtil.PlayAnimation(uiroom1200.uitop1200.elementList[cellIndex].GetChild(0), aniName);
                CommonTools.PlayArmatureAni(uiroom1200.uitop1200.elementBgList[cellIndex].GetChild(0), "newAnimation", 0);

            }
        }

        public override void onSpinFinish()
        {
            if (Game1200Model.Instance.gameStates != 1 && SlotData_1200.elementWild == element && Game1200Model.Instance.gameStates != 3)
            {
                MoveAni();
                if (uiroom1200.uitop1200.elementList[cellIndex].childCount <= 0)
                {
                    elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/Tb7");
                    elementGo.transform.SetParent(TfSpine, true);
                    elementGo.transform.localScale = new Vector3(0.68f, 0.68f, 1);
                    elementGo.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                    elementGo = uiroom1200.uitop1200.elementList[cellIndex].GetChild(0).gameObject;
                ImgElement.gameObject.SetActive(false);
                ToolUtil.PlayAnimation(elementGo.transform, "spawn", false, () =>
                {
                    string aniName = "win_idle";
                    //if (element == 7)
                        aniName = "idle";
                    ToolUtil.PlayAnimation(elementGo.transform, aniName, true);
                });
            }

        }


        private void MoveAni()
        {
            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/FlyAni");
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(200, 200, 100);
            go.transform.localPosition = Vector3.zero;
            GameObject go1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1200/FirstRes/FlyAni");
            go1.transform.SetParent(transform);
            go1.transform.localScale = new Vector3(100, 100, 100);
            go1.transform.localPosition = Vector3.zero;
            CommonTools.PlayArmatureAni(go.transform, "dj", 1);
            CommonTools.PlayArmatureAni(go1.transform, "tw", 1);
           // await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.1f));

            Game1200Model.Instance.MovePos(go1.transform, uiroom1200.GetMoveTarget(), () =>
            {
                CoreEntry.gGameObjPoolMgr.Destroy(go);
                CoreEntry.gGameObjPoolMgr.Destroy(go1);
                uiroom1200.playTigerAni(7);
            },0.4f,true,true);
        }
    }

}
