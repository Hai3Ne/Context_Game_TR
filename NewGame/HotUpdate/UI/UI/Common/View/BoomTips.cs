using HotUpdate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BoomTips : PanelBase
{
    [SerializeField] private Button btn1;
    [SerializeField] private Button btn2;
    [SerializeField] private Button btn3;
    [SerializeField] private Button btn4;
    [SerializeField] private Text lab1;
    [SerializeField] private Text lab2;
    [SerializeField] private Text lab3;
    [SerializeField] private Text lab4;
    [SerializeField] private Image slot;
    private List<int> targetArr = new List<int>() { 69000, 2460000, 12000000 };

    Sequence[] mySequences = new Sequence[3];
    // Start is called before the first frame update

    protected override void OnEnable()
    {
        Message.AddListener(MessageName.BOOM_REWARD_RELOAD,reloadUI);
        btn1.onClick.AddListener(onClickBtn1);
        btn2.onClick.AddListener(onClickBtn2);
        btn3.onClick.AddListener(onClickBtn3);
        btn4.onClick.AddListener(onClickBtn4);
        lab2.text = ToolUtil.AbbreviateNumberF0(targetArr[0]);
        lab3.text = ToolUtil.AbbreviateNumberF0(targetArr[1]);
        lab4.text = ToolUtil.AbbreviateNumberF0(targetArr[2]);
        reloadUI();
        
    }







    private void reloadUI()
    {
   for(int i = 0;i < mySequences.Length;i++)
        {
            mySequences[i]?.Kill();
            mySequences[i] = null;
        }

        btn2.DOKill();
        btn3.DOKill();
        btn4.DOKill();
        btn4.transform.eulerAngles = Vector3.zero;
        btn2.transform.eulerAngles = Vector3.zero;
        btn3.transform.eulerAngles = Vector3.zero;
        lab1.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.boomTaskValue);
        if (MainUIModel.Instance.boomTaskValue >= targetArr[2])
        {
            slot.fillAmount = 1;
            PlayAni(0, btn2);
            PlayAni(1, btn3);
            PlayAni(2, btn4);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (MainUIModel.Instance.boomTaskValue < targetArr[i])
                {
                    float tempSize = 0;
                    if (i == 0)
                        tempSize = 0.3f;
                    else if (i == 1)
                    {
                        PlayAni(1, btn2);
                        tempSize = 0.37f;
                    }                    
                    else
                    {
                        PlayAni(2, btn3);
                        PlayAni(1, btn2);
                        tempSize = 0.33f;
                    }
             
                    if (i == 0)
                        slot.fillAmount = MainUIModel.Instance.boomTaskValue / (float)targetArr[i] * tempSize;
                    else
                    {
                        if (i == 1)
                            slot.fillAmount = 0.3f + (MainUIModel.Instance.boomTaskValue - targetArr[0]) / (float)(targetArr[i] - targetArr[0]) * tempSize;
                        else
                            slot.fillAmount = 0.67f + (MainUIModel.Instance.boomTaskValue - targetArr[1]) / (float)(targetArr[i] - targetArr[1]) * tempSize;
                    }
                    break;
                }
            }
        }    
    }

    private void PlayAni(int index,Button btn)
    {
        Debug.LogError(">>>>>>>>>+>>>>");
        mySequences[index] = DOTween.Sequence();
        Tween t1 = btn.transform.DOLocalRotate(new Vector3(0, 0, 11), 1);
        Tween t2 = btn.transform.DOLocalRotate(new Vector3(0, 0, -11), 1);
        mySequences[index].Append(t1);
        mySequences[index].Append(t2);
        mySequences[index].SetLoops(-1, LoopType.Yoyo);
        mySequences[index].Play();
    }

    private void onClickBtn1()
    {
        MainPanelMgr.Instance.ShowDialog("BoomRuleTips");
    }

    private void onClickBtn4()
    {
        if (MainUIModel.Instance.boomTaskValue >= targetArr[2])
        {
            MainUICtrl.Instance.SendBoomRewardReq(targetArr[2]);
        }
    }

    private void onClickBtn3()
    {
        if (MainUIModel.Instance.boomTaskValue >= targetArr[1])
        {
            MainUICtrl.Instance.SendBoomRewardReq(targetArr[1]);
        }
    }

    private void onClickBtn2()
    {
       if(MainUIModel.Instance.boomTaskValue >= targetArr[0])
        {
            MainUICtrl.Instance.SendBoomRewardReq(targetArr[0]);
        }
    }

    // Update is called once per frame
    void OnDisable()
    {
        for (int i = 0; i < mySequences.Length; i++)
        {
            mySequences[i]?.Kill();
            mySequences[i] = null;
        }
        btn1.onClick.RemoveListener(onClickBtn1);
        btn2.onClick.RemoveListener(onClickBtn2);
        btn3.onClick.RemoveListener(onClickBtn3);
        btn4.onClick.RemoveListener(onClickBtn4);
        Message.RemoveListener(MessageName.BOOM_REWARD_RELOAD, reloadUI);
    }


}
