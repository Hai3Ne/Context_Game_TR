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

    public class UIRoom1500SlotCell : UISlotCell
    {
        UIRoom1500 uiroom1500;
        GameObject elementGo;
        GameObject effect;
        FrameAnimation1500 frameAni;
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
        public override void Awake()
        {
            base.Awake();
           
            Button BtnClick = transform.Find("BtnClick").GetComponent<Button>();
            BtnClick.onClick.AddListener(ClickElement);
        }

        public void init(UISlotColumn Column)
        {
            slotColumn = Column;
            uiroom1500 = slotColumn.slotCpt as UIRoom1500;
        }

        private void ClickElement()
        {
            if (element <= 0)
                return;
            if(uiroom1500.StateSlot == slotState.Idle)
                uiroom1500.uitop1500.OpenGameTips(element, cellIndex);
        }


        public override void setElement(int ele, int row1 = 0)
        {
            element = ele;
            row = row1;
            if (row1 < 0)
                ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1500_3:" + "Tb" + ele+"_1");
            else
                ImgElement.sprite = AtlasSpriteManager.Instance.GetSprite("Game1500_2:" + "Tb" + ele);
            ImgElement.SetNativeSize();

            if(element !=1)
                ImgElement.transform.localScale = new Vector3(1.4f,1.4f,1);
            else
                ImgElement.transform.localScale = new Vector3(1.6f, 1.6f, 1);
        }

        public override void reset()
        {
            if(row <=(slotColumn.column !=1?2:3) && row >= 0)
            {
                if((Game1500Model.Instance.gameStates == 3 && Game1500Model.Instance.toSpin.WinGold > 0) || Game1500Model.Instance.gameStates ==0 || Game1500Model.Instance.gameStates == 2)
                {
                    if (uiroom1500.uitop1500.elementList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1500.uitop1500.elementList[cellIndex].GetChild(0).gameObject);
                    if (uiroom1500.uitop1500.elementBgList[cellIndex].childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).gameObject);
                    if (TfSpine.childCount > 0)
                        CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);
                }
            }
            ImgElement.gameObject.SetActive(true);
        }

        public override void showLine(Action callBack = null, int element0 = 0, int row0 = 0, int sunvalue0 = 0)
        {
            if(cellIndex < 0 || cellIndex >= uiroom1500.uitop1500.elementBgList.Count)
                Debug.LogError("======="+ cellIndex+"------"+ slotColumn.column+"==="+row);
            if (uiroom1500.uitop1500.elementBgList[cellIndex].childCount <= 0)
            {
                effect = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/Effect_Bg");
                effect.transform.SetParent(uiroom1500.uitop1500.elementBgList[cellIndex], true);
                effect.transform.localScale = new Vector3(01f, 1f, 1);
                effect.transform.localPosition = new Vector3(0, 0, 0);
                effect.gameObject.SetActive(true);
                ImgElement.gameObject.SetActive(false);
                CommonTools.PlayArmatureAni(effect.transform.GetChild(1).transform,"ps_pattern",0);
                CommonTools.PlayArmatureAni(effect.transform.GetChild(2).transform, "ps_particle", 0);
                CommonTools.PlayArmatureAni(effect.transform.GetChild(3).transform, "ps_particle", 0);
                DoScaleAni(effect.transform);
            }
            if (uiroom1500.uitop1500.elementList[cellIndex].childCount > 0)
            {
                ImgElement.gameObject.SetActive(false);
                return;
            }

            if (TfSpine.childCount > 0)
                CoreEntry.gGameObjPoolMgr.Destroy(TfSpine.GetChild(0).gameObject);

            elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/Tb" + element);
            elementGo.transform.SetParent(uiroom1500.uitop1500.elementList[cellIndex], true);
            elementGo.gameObject.SetActive(true);
            elementGo.transform.localScale = new Vector3(2.8f, 2.8f, 1);
            if (element == 1)
                elementGo.transform.localScale = new Vector3(3.2f, 3.2f, 1);
            else if(element == 2)
            {
                elementGo.transform.localScale = new Vector3(1.4f, 1.4f, 1);
            }

            if (element == 2)
                elementGo.transform.localPosition = new Vector3(0, 22.4f, 0);
            else
                elementGo.transform.localPosition = new Vector3(0, 0, 0);

            ToolUtil.PlayAnimation(elementGo.transform, "win", false, () =>
            {
                ToolUtil.PlayAnimation(elementGo.transform, "win_bg", true);
            });
        }

        public override void ShowCellEffect(bool bShow = false, bool bAllTrue = false)
        {
            if (cellIndex < 0 || cellIndex >= 10)
                return;
            if (uiroom1500.uitop1500.elementList[cellIndex].childCount > 0)
            {               
                if (uiroom1500.uitop1500.elementBgList[cellIndex].childCount > 0)
                {
                    CanvasGroup canvasGroup = uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).GetComponent<CanvasGroup>();
                    canvasGroup.transform.DOKill();
                    if (Game1500Model.Instance.lines.Count == 1)
                    {
                        uiroom1500.uitop1500.elementList[cellIndex].GetChild(0).gameObject.SetActive(true);
                        uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(true);

                        if (bShow)
                        {
                            DOTween.To(() => 0.7f, (value) => {
                                canvasGroup.alpha = value;
                            }, 1, 0.5f).OnComplete(() => {
                                canvasGroup.alpha = 1;
                            }).SetEase(Ease.Linear);
                            DoScaleAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0));
                        }
                        else
                            canvasGroup.alpha = 0.4f;
                    }
                    else
                    {
                        uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                        uiroom1500.uitop1500.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);

                        if(bShow)
                        {
                            DOTween.To(() => 0.4f, (value) => {
                                canvasGroup.alpha = value;
                            }, 1, 0.5f).OnComplete(() => {
                                canvasGroup.alpha = 1;
                            }).SetEase(Ease.Linear);

                            DoScaleAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0));
                        }                  
                    }
                }
                
                if (bShow)
                {
                    ToolUtil.PlayAnimation(uiroom1500.uitop1500.elementList[cellIndex].GetChild(0), "win");
                    CommonTools.PlayArmatureAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0), "ps_pattern", 0);

                    CommonTools.PlayArmatureAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).GetChild(1).transform, "ps_pattern",0);
                    CommonTools.PlayArmatureAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).GetChild(2).transform,  "ps_particle", 0);
                    CommonTools.PlayArmatureAni(uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).GetChild(3).transform, "ps_particle", 0);
                }
                if (bAllTrue)
                {
                    uiroom1500.uitop1500.elementBgList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                    uiroom1500.uitop1500.elementList[cellIndex].GetChild(0).gameObject.SetActive(bShow);
                }
            }
            ImgElement.gameObject.SetActive(!bShow);
        }

        public override void onSpinFinish()
        {
            if (!Game1500Model.Instance.bFreeGameFinished() && SlotData_1500.elementWild == element && row >=0)
            {
                MoveAni();
                if (uiroom1500.uitop1500.elementList[cellIndex].childCount <= 0)
                {
                    elementGo = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/Tb7");
                    elementGo.transform.SetParent(TfSpine, true);
                    elementGo.transform.localScale = new Vector3(2.8f, 2.8f, 1);
                    elementGo.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                    elementGo = uiroom1500.uitop1500.elementList[cellIndex].GetChild(0).gameObject;
                ImgElement.gameObject.SetActive(false);
                ToolUtil.PlayAnimation(elementGo.transform, "spawn", false, () =>
                {
                    string aniName = "win_idle";
                        aniName = "spawn_bg";
                    ToolUtil.PlayAnimation(elementGo.transform, aniName, true);
                });
            }
        }


        private void MoveAni()
        {
            GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/FlyAni");
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(200, 200, 100);
            go.transform.localPosition = Vector3.zero;
            GameObject go1 = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1500/FirstRes/FlyAni");
            go1.transform.SetParent(transform);
            go1.transform.localScale = new Vector3(100, 100, 100);
            go1.transform.localPosition = Vector3.zero;
            CommonTools.PlayArmatureAni(go.transform, "dj", 1);
            CommonTools.PlayArmatureAni(go1.transform, "tw", 1);
            Game1500Model.Instance.MovePos(go1.transform, uiroom1500.GetMoveTarget(), () =>
            {
                CoreEntry.gGameObjPoolMgr.Destroy(go);
                CoreEntry.gGameObjPoolMgr.Destroy(go1);
            },0.4f,true,true);
        }

        private void DoScaleAni(UnityEngine.Transform trans)
        {
            trans.localScale = new Vector3(0.8f, 0.8f, 1);
            Sequence seq = DOTween.Sequence();
            Tweener t1 = trans.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.25f);
            Tweener t2 = trans.transform.DOScale(new Vector3(1.05f, 1f, 1), 0.1f);
            seq.Append(t1);
            seq.Append(t2);
            seq.Play();
        }
    }

}
