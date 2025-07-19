using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using uGUI;
using SEZSJ;

namespace HotUpdate
{
    public class UITop : MonoBehaviour
    {
        protected GameObject GoAutoSpinNum;
        protected Text TxtAutoSpinNum;


        public List<Transform> SpineCell = new List<Transform>();
 

        public Transform TfFree;
        protected Transform Free;
        protected Text TxtFreeTimes;
        public Transform TfEffectSpine;
        protected Transform tempTfEffectSpine;
        protected GameObject Effect;
        public GoldEffectNew m_Gold_EffectNew;
        protected long gold = 0;

        public Tweener freeAni;


        public long winGold = 0;
        /// <summary>
        /// 播放金币类型
        /// </summary>
        public int playGoldType = 0;
        public virtual void Awake()
        {

            GoAutoSpinNum = transform.Find("Bottom/GoAutoSpinNum").gameObject;
            TxtAutoSpinNum = transform.Find("Bottom/GoAutoSpinNum/TxtAutoSpinNum").GetComponent<Text>();

            Effect = transform.Find("Effect").gameObject;
            TfFree = transform.Find("TfFree");
            Free = transform.Find("TfFree/Free");
            TxtFreeTimes = transform.Find("TfFree/Free/TxtFreeTimes").GetComponent<Text>();   
            TfEffectSpine = transform.Find("TfEffectSpine");
            tempTfEffectSpine = TfEffectSpine.Find("TfEffectSpine");
            for (int i = 0; i < tempTfEffectSpine.childCount; i++)
                SpineCell.Add(tempTfEffectSpine.GetChild(i));

            GameObject go1 = CommonTools.AddSubChild(Effect.gameObject, "UI/UITemplate/Gold_EffectNew");
            m_Gold_EffectNew = go1.GetComponent<GoldEffectNew>();
            m_Gold_EffectNew.gameObject.SetActive(false);
        }


        public virtual void Init(UIRoom_SlotCpt slot)
        {}

        public virtual void ClickBtnCloseEffect()
        { }

        protected virtual void OnEnable()
        {
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
   
            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
        }

        public virtual void InitData()
        { }

        public virtual void UpdateGold(long gold)
        {

        }

        public virtual void InitGame()
        {
        }

        protected virtual void SetTopRank()
        {
        }

        public virtual void OnDisable()
        {
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotWinGold, OnSlotWinGold);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFree, PlayFreeAni);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotFreeEnd, PlayFreeEnd);
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
        }

        public virtual void ReloadGame()
        {       
        }

  

        public virtual void clkEndSpin()
        {
        }


        public virtual void OnSlotState(GameEvent ge, EventParameter parameter)
        { 
        }

        public virtual void OnSlotWinGold(GameEvent ge, EventParameter parameter)
        {
        }

        public virtual void BigWinAni(Action callBack,bool freeEnd, int bSpecialNum = 0)
        {
        }

        public virtual void PlayFreeAni(GameEvent ge, EventParameter parameter)
        {      
        }



        public virtual void PlayFreeEnd(GameEvent ge, EventParameter parameter)
        {
        }
    }
}
