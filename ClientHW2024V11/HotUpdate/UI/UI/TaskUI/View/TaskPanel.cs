using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class TaskItemData
    {
        public CanvasGroup canvasGroup;
        public RectTransform rect;
        public Vector2 pos;
    }
    public enum TaskType
    {
        Game2Day = 1,
        Game2Week = 2,
        Game2Month = 3,
        Game3Day = 4,
        Game3Week = 5,
        Game3Month = 6,
        Game4Day = 7,
        Game4Week = 8,
        Game4Month = 9,
    }
    public partial class TaskPanel : PanelBase
    {
        private string prefabName = "taskItem";
        private int taskType = 1;
        private long leftTime = 0;
        List<Transform> Selects = new List<Transform>();
        List<Transform> UnSelects = new List<Transform>();
        private bool bFirst = true;
        List<Transform> tempList = new List<Transform>();

        ScrollRect scrollRect;
        List< DragonBones.UnityArmatureComponent> DragonList = new List<DragonBones.UnityArmatureComponent>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            DragonList.Add(m_Dragon_Red1);
            DragonList.Add(m_Dragon_Red2);
            DragonList.Add(m_Dragon_Red3);
            Selects.Add(m_Tog_Page1.transform.GetChild(2));
            Selects.Add(m_Tog_Page2.transform.GetChild(2));
            Selects.Add(m_Tog_Page3.transform.GetChild(2));

            UnSelects.Add(m_Tog_Page1.transform.GetChild(1));
            UnSelects.Add(m_Tog_Page2.transform.GetChild(1));
            UnSelects.Add(m_Tog_Page3.transform.GetChild(1));

            taskType = MainUIModel.Instance.taskInfo.taskInfos.First().Key;
            leftTime = MainUIModel.Instance.taskInfo.taskTimeList[taskType];
            m_VGridScroll_HeadList.InitGridView(MainUIModel.Instance.taskInfo.taskInfos[taskType].OrderBy(x => x.IsCollect).ToList().Count, OnGetItemByRowColumn);
            scrollRect = m_VGridScroll_HeadList.GetComponent<ScrollRect>();

            //for(int i = 0;i < titles.Count;i++)
            //    titles[i].text = MainUIModel.Instance.taskInfo.taskInfos[]
        }
        long time = 0;
        protected override void Update()
        {
            base.Update();

   
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);
                
                //RefreshDayTaskTime();
            }

            if (taskType == 1)
            {
                m_Txt_Time.text = TimeUtil.DateDiffByDay("", TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(leftTime));
                //m_Txt_Time.alignment = TextAnchor.MiddleCenter;
            }
            else
            {
                if (taskType == 2)
                {
                    m_Txt_Time.text = TimeUtil.DateDiff(TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(leftTime));
                    //m_Txt_Time.alignment = TextAnchor.MiddleCenter;
                }
                else
                {
                    m_Txt_Time.text = TimeUtil.DateDiff(TimeUtil.TimestampToDataTime(ToolUtil.getServerTime()), TimeUtil.TimestampToDataTime(leftTime));
                    //m_Txt_Time.alignment = TextAnchor.MiddleLeft;
                }
            }
        }



        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
           // UpdataTaskList();
         
            SelectIndexPos();
            // PlayAni();

            
               
        }

        private async void SelectIndexPos()
        {
            var roomType = MainUIModel.Instance.RoomData != null ? MainUIModel.Instance.RoomData.nRoomType : 0;
            m_VGridScroll_HeadList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            if (m_Tog_Page1.isOn)
                UpdataTaskList();
            m_Tog_Page1.isOn = true;
           
        }

        private List<TaskItemData> tewen = new List<TaskItemData>();
        private async void PlayAni()
        {
            if (tewen.Count > 0)
            {
                foreach (var item in tewen)
                {
                    item.canvasGroup.DOKill();
                    item.rect.DOKill();
                    item.canvasGroup.alpha = 1;
                }
                tewen.Clear();
            }
            m_VGridScroll_HeadList.MovePanelToItemByIndex(0);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.02f));
            scrollRect.enabled = false;
            scrollRect.verticalNormalizedPosition = 1;
            Transform content = m_VGridScroll_HeadList.transform.GetChild(0).GetChild(0);
            float aniTimes = 0.4f;
            tempList.Clear();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
            for (int i = 0; i < m_VGridScroll_HeadList.ItemTotalCount; i++)
            {
                var item = m_VGridScroll_HeadList.GetShownItemByItemIndex(i);
                if (item != null)
                {
                    if(i <20)
                    {
         
                        tempList.Add(item.transform);
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            
       
            for (int i = 0; i < tempList.Count; i++)
            {
                int index = i;
                tempList[index].gameObject.SetActive(true);
                var rect = tempList[index].GetComponent<RectTransform>();
                var posY = rect.anchoredPosition.y;
             
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - 200);
                CanvasGroup canvasGroup = tempList[index].GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                var pos1 = new Vector2(rect.anchoredPosition.x, posY);
                var tween1 = rect.DOAnchorPos(pos1, aniTimes).SetDelay(0.15f*i); ;
                canvasGroup.DOFade(1,0.8f).OnComplete(()=>
                {
                    canvasGroup.alpha = 1;
                }).SetEase(Ease.Linear).SetDelay(0.15f * i);
   
                TaskItemData data = new TaskItemData();
   
                data.pos = pos1;
                data.rect = rect;
                data.canvasGroup = canvasGroup;
                tewen.Add(data);
                //await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.15f));
            }
            bFirst = false;
            CoreEntry.gTimeMgr.RemoveTimer(251);
            CoreEntry.gTimeMgr.AddTimer(tempList.Count*0.15f,false,()=> {
              if (scrollRect != null)
                  scrollRect.enabled = true;
              CoreEntry.gTimeMgr.RemoveTimer(251);

                var taskCount = 0;
                if (MainUIModel.Instance.taskInfo != null)
                {
                    foreach (var item in MainUIModel.Instance.taskInfo.taskInfos)
                    {
                        taskCount += item.Value.FindAll(x => x.total > x.taskTarget && !x.IsCollect).Count;
                    }
                }
                if(taskCount > 0)
                {
                    if (GuideModel.Instance.bReachCondition(5))
                    {
                        GuideModel.Instance.SetFinish(5);
                        LoopGridViewItem item = m_VGridScroll_HeadList.GetShownItemByItemIndex(0);

                        MainUICtrl.Instance.OpenGuidePanel(item.transform, () =>
                        {
                            item.GetComponent<TaskItem>().OnClaimBtn();
                        }, 5);
                    }
                }

      
            },251);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            if (tewen.Count > 0)
            {
                foreach (var item in tewen)
                {
                    item.canvasGroup.DOKill();
                    item.rect.DOKill();
                    item.canvasGroup.alpha = 1;
                }
                tewen.Clear();
            }
            scrollRect.enabled = true;
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Tog_Page1.onValueChanged.AddListener(Toggle1Changed);
            m_Tog_Page2.onValueChanged.AddListener(Toggle2Changed);
            m_Tog_Page3.onValueChanged.AddListener(Toggle3Changed);
            Message.AddListener(MessageName.REFRESH_TASK_PANEL, UpdataTaskList);
            Message.AddListener<int, long>(MessageName.REFRESH_TASK_PANEL2, UpdateTask);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Tog_Page1.onValueChanged.RemoveListener(Toggle1Changed);
            m_Tog_Page2.onValueChanged.RemoveListener(Toggle2Changed);
            m_Tog_Page3.onValueChanged.RemoveListener(Toggle3Changed);
            Message.RemoveListener(MessageName.REFRESH_TASK_PANEL, UpdataTaskList);
            Message.RemoveListener<int,long>(MessageName.REFRESH_TASK_PANEL2, UpdateTask);
        }
        #endregion
        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            TaskCtrl.Instance.CloseTaskPanel();
        }
        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > MainUIModel.Instance.taskInfo.taskInfos[taskType].OrderBy(x => x.IsCollect).ToList().Count
                || MainUIModel.Instance.taskInfo.taskInfos[taskType].OrderBy(x => x.IsCollect).ToList().Count == 0)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<TaskItem>();
            script.SetUpItem(MainUIModel.Instance.taskInfo.taskInfos[taskType].OrderBy(x => x.IsCollect).ToList()[itemIndex],this);
            return item;
        }

        private void Toggle1Changed(bool isOn)
        {
            Selects[0].gameObject.SetActive(isOn);
            UnSelects[0].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }
                m_TxtM_PageTxt1.color = new Color(104f / 255, 66f / 255, 56f / 255, 1);
                taskType = 1;
                leftTime = MainUIModel.Instance.taskInfo.taskTimeList[1];
                UpdataTaskList();
            }
            else
            {
                m_TxtM_PageTxt1.color = new Color(236f / 255, 221f / 255, 172f / 255, 1f);
            }
        }

        private void Toggle2Changed(bool isOn)
        {
            Selects[1].gameObject.SetActive(isOn);
            UnSelects[1].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }
                
                m_TxtM_PageTxt2.color = new Color(104f / 255, 66f / 255, 56f / 255, 1);
                taskType = 2;
                leftTime = MainUIModel.Instance.taskInfo.taskTimeList[2];
              
                UpdataTaskList();
            }
            else
            {
                m_TxtM_PageTxt2.color = new Color(236f / 255, 221f / 255, 172f / 255, 1f);
            }
        }

        private void Toggle3Changed(bool isOn)
        {
            Selects[2].gameObject.SetActive(isOn);
            UnSelects[2].gameObject.SetActive(!isOn);
            if (isOn)
            {
                scrollRect.enabled = true;
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                }
                m_TxtM_PageTxt3.color = new Color(104f / 255, 66f / 255, 56f / 255, 1);
                leftTime = MainUIModel.Instance.taskInfo.taskTimeList[3];
                taskType = 3;
                UpdataTaskList();
            }
            else
            {
                m_TxtM_PageTxt3.color = new Color(236f / 255, 221f / 255, 172f / 255, 1f);
            }
        }


        private void UpdateTask(int type,long time)
        {
            if (type == 1)
            {
                RefreshDayTaskTime(time);
            }
            else
                RefreshMonthTaskTime(time);
        }

        private void UpdataTaskList()
        {
            foreach (var item in MainUIModel.Instance.taskInfo.taskInfos)
            {
      
                var num = item.Value.FindAll(x => x.total > x.taskTarget && !x.IsCollect).Count;
                DragonList[item.Key - 1].gameObject.SetActive(num > 0);
            }

            leftTime = MainUIModel.Instance.taskInfo.taskTimeList[taskType];
            m_Tog_Page1.gameObject.SetActive(MainUIModel.Instance.taskInfo.taskInfos.ContainsKey(1) && MainUIModel.Instance.taskInfo.taskInfos[1].Count > 0);
            m_Tog_Page2.gameObject.SetActive(MainUIModel.Instance.taskInfo.taskInfos.ContainsKey(2) && MainUIModel.Instance.taskInfo.taskInfos[2].Count > 0);
            m_Tog_Page3.gameObject.SetActive(MainUIModel.Instance.taskInfo.taskInfos.ContainsKey(3) && MainUIModel.Instance.taskInfo.taskInfos[3].Count > 0);

            m_VGridScroll_HeadList.SetListItemCount(MainUIModel.Instance.taskInfo.taskInfos[taskType].Count);
            m_VGridScroll_HeadList.RefreshAllShownItem();
            PlayAni();

        }
        private void RefreshDayTaskTime(long data)
        {
            var dataList = MainUIModel.Instance.taskInfo.taskTimeList;
            var taskList = dataList.ToList().FindAll(x => x.Key == 1);
            var curTime = TimeUtil.TimestampToDataTime(data);
            var taskType = 1;

            if (curTime.Date != TimeUtil.TimestampToDataTime(MainUIModel.Instance.taskInfo.taskTimeList[taskType]).Date)
            {
                for (int i = 0; i < taskList.Count; i++)
                {
                    Debug.LogError(taskList[i]);
                    var taskDataList = MainUIModel.Instance.taskInfo.taskInfos[taskList[i].Key];
                    for (int l = 0; l < taskDataList.Count; l++)
                    {
                        taskDataList[l].total = 0;
                    }
                   if (MainUIModel.Instance.taskInfo.taskTimeList.ContainsKey(1))
                        MainUIModel.Instance.taskInfo.taskTimeList[1] = data + 86400;
        
                }
            }
            if (m_Tog_Page1.isOn)
                UpdataTaskList();
        }

        private void RefreshWeekTaskTime(long data)
        {
            var dataList = MainUIModel.Instance.taskInfo.taskTimeList;
            var taskList = dataList.ToList().FindAll(x => x.Key == 2);
            var curTime = TimeUtil.TimestampToDataTime(data);
   


            if (curTime.DayOfWeek == DayOfWeek.Monday &&
                curTime != DateTime.Now
                )
            {
                for (int i = 0; i < taskList.Count; i++)
                {
                    Debug.LogError(taskList[i]);
                    var taskDataList = MainUIModel.Instance.taskInfo.taskInfos[taskList[i].Key];
                    for (int l = 0; l < taskDataList.Count; l++)
                    {
                        taskDataList[l].total = 0;
                    }
                    if (MainUIModel.Instance.taskInfo.taskTimeList.ContainsKey(2))
                        MainUIModel.Instance.taskInfo.taskTimeList[2] = data + 86400 * 7;

                }
            }
            if (m_Tog_Page2.isOn)
                UpdataTaskList();
        }

        private void RefreshMonthTaskTime(long data)
        {
            var dataList = MainUIModel.Instance.taskInfo.taskTimeList;
            var taskList = dataList.ToList().FindAll(x => x.Key == 3 );
            var curTime = TimeUtil.TimestampToDataTime(data);


            if (curTime.Month !=DateTime.Now.Month)
            {
                for (int i = 0; i < taskList.Count; i++)
                {
                    Debug.LogError(taskList[i]);
                    var taskDataList = MainUIModel.Instance.taskInfo.taskInfos[taskList[i].Key];
                    for (int l = 0; l < taskDataList.Count; l++)
                    {
                        taskDataList[l].total = 0;
                    }
                    var nextMonthTime = TimeUtil.TimestampToDataTime(data).AddMonths(1);
                    DateTime firstDayOfNextMonth = new DateTime(nextMonthTime.Year, nextMonthTime.Month, 1);
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(TimeUtil.DataTimeToTimestamp(firstDayOfNextMonth)).ToLocalTime();
                    //DateTime zeroDateTime = dateTime.Date;
                    DateTimeOffset dateTimeOffset = new DateTimeOffset(firstDayOfNextMonth);
                    long zeroTimestamp = dateTimeOffset.ToUnixTimeSeconds();
                    if (MainUIModel.Instance.taskInfo.taskTimeList.ContainsKey(3))
                        MainUIModel.Instance.taskInfo.taskTimeList[3] = zeroTimestamp;

                }
            }
            if(m_Tog_Page3.isOn)
            UpdataTaskList();
        }




    }
}
