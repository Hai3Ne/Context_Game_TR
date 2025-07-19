using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public enum slotState
    {
        None = 0,
        Idle = 1,// -- 待机
        SpinBegin = 2,// -- 开始Spin
        SpinSuccess = 3,// -- 收到Spin消息
        SpinStop = 4,// -- 点击Stop或禁止点击Stop(触发加速表现)
        SpinEnd = 5,//-- 全部列停止
    }

    public class UIRoom_SlotCpt : PanelBase
    {
        /// <summary>
        /// 是否是单机游戏
        /// </summary>
        public bool bDanJi = false;

        public int slotRow = 1;//-- 行数 实际prefab中元素比这个值多1
        public float heightCell { get; protected set; } //行高
       
        public List<UISlotColumn> slotColumns = new List<UISlotColumn>();// -- SlotData.column列
        public bool isFreeSpin = false;
        public bool isFastSpin = false;
        //剩余spin次数
        public int autoSpinNum = 0;

        public freeTimes_Data freeTimes { get; private set; } = new freeTimes_Data(); //免费数据

        //等待请求中，预防连点
        public bool awaiting = false;
        // 任务队列
        public List<Transform> lstcolumns = new List<Transform>();
        /// <summary>
        /// 滚动列数
        /// </summary>
        public int columnCount = 5;
        /// <summary>
        /// 滚动次数
        /// </summary>
        public int num = 0;

        public class freeTimes_Data
        {
            public int times = 0;
            public int max = 0;
            public int winTimes = 0;
            public long gold = 0;
            public long winGold = 0;
            public void reset()
            {
                times = 0;
                max = 0;
                winTimes = 0;
                gold = 0;
                winGold = 0;
            }
        }

        public Transform TfSlot;

        public UITop uiTop;
        public GameObject effectAcc;
        public slotState StateSlot = slotState.None;
        public int gameStatus = 0;
        protected override void Awake()
        {
            //--列数
            TfSlot = transform.Find("GoSlot/TfSlot");
            for (int i = 0; i < TfSlot.childCount; i++)
            {
                Transform child = TfSlot.GetChild(i);
                if (child.name.Contains("Column"))
                    lstcolumns.Add(child);
            }
            //--行数
            Transform tfColumn = TfSlot.GetChild(0);
            slotRow = tfColumn.childCount - 1;
            //-- 行高
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;
            slotColumns = new List<UISlotColumn>();
            effectAcc = transform.Find("GoSlot/EffectAcc").gameObject;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            bDanJi = param == null ? false : (bool)param;
        }

        protected override void Start()
        {
            init();
        }


  
        public virtual void init()
        {
        }

        public virtual void preSpin()
        {
        }

        public virtual void sendSpin()
        {
        }

        public virtual void recSpin()
        {
            awaiting = false;
            setState(slotState.SpinSuccess);
            handlerSpin();
        }

        public virtual void RandomSpinData()
        {

        }

        public virtual void SetSpinData()
        {
            recSpin();
        }

        public virtual void handlerSpin()
        {
   
        }

        public virtual void finishSpin()
        {
            CoreEntry.gAudioMgr.StopSound();
        }

        public virtual void showLines()
        {
        }

        public virtual void  ShowAllCell(bool bShow, bool bAllTrue = false)
        {
            for (int i = 0; i < Game500Model.Instance.lines.Count; i++)
            {
                KeyValuePair<int, int> tempLine = Game500Model.Instance.lines[i];
                List<int> elementPos = Game500Model.Instance.lineData[tempLine.Key];
                for (int j = 0; j < tempLine.Value; j++)
                {
                    slotColumns[j].lstCells[2 - elementPos[j] + 1].ShowCellEffect(bShow, bAllTrue);
                }
            }
        }

   


        public virtual void setState(slotState state)
        {
            StateSlot = state;
          
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.OnSlotState, EventParameter.Get((int)StateSlot));
        }
        public virtual bool IsAutoSpin()
        {
            return autoSpinNum > 0;
        }

        public virtual void finishRoll(int column)
        {
        }

        protected virtual bool shouldAcc(int column)
        {
            //第一列不加速
            if (column < 1 || column >= SlotData_500.column - 1)
                return false;
            //出现免费元素数量大等于2
            int freeNum = 0;
            for (int i = 0; i < 3 * (column + 1); i++)
            {
                if (Game500Model.Instance.slotResult[i] == SlotData_500.elementFree)
                {
                    freeNum++;
                }
            }
            return freeNum >= 2;

        }
        public virtual bool hasElement(int ele)
        { 
            return false;
        }

        public virtual int cr2i(int c, int r)
        {
            return 0;
        }

        public virtual int cr2ele(int c, int r)
        {
            return Game500Model.Instance.slotResult[cr2i(c, r)];
        }

        public virtual void beginSpin(int num = 0, bool fast = false)
        {
        }

        public virtual void endSpin()
        {
            autoSpinNum = 0;
            //isFastSpin = true;
            for (int i = 0; i < columnCount; i++)
                slotColumns[i].stop();
            setState(slotState.SpinStop);
        }

        public virtual void continueSpin()
        {
        }

        public virtual bool isFreeEnd()
        {
            return freeTimes.times == freeTimes.max;
        }

        protected virtual int getElementCount(int ele, bool orWild = false)
        {
            return 0;
        }

        public virtual void playColumnAcc(int column)
        {
            if (effectAcc == null)
                return;
            effectAcc.SetActive(column >= 0);
            if (column >= 0)
            {
                UISlotColumn col = slotColumns[column];
                effectAcc.GetComponent<Transform>().position = col.transform.position;
                effectAcc.GetComponent<RectTransform>().anchoredPosition = new Vector2(effectAcc.GetComponent<RectTransform>().anchoredPosition.x,9.5f);
            }
        }
   

        public virtual void InitData()
        {
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            gameStatus = 0;
        }
    }

}


  