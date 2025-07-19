using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace HotUpdate
{
    public class TaskInfo
    {
        public int typeSize;
        //public List<Task> taskInfos = new List<Task>();
        public Dictionary<int, List<Task>> taskInfos = new Dictionary<int, List<Task>>();
        public List<Task> allTasks = new List<Task>();
        public Dictionary<int, long> taskTimeList = new Dictionary<int, long>();
        public int availableCount = 0;
        public TaskInfo(SC_TASK_LIST_INFO info)
        {
            taskInfos.Clear();
            taskTimeList.Clear();
            var gameTask = ConfigCtrl.Instance.Tables.TbGameTask.DataList;

            typeSize = info.nTypeSize;
            for (int i = 0; i < typeSize; i++)
            {

                List<Task> typeTaskList = new List<Task>();
                var typeList = gameTask.FindAll(x => x.Type == info.arrayInfo[i].nType);
                for (int k = 0; k < typeList.Count; k++)
                {

                    if (typeList[k].Valid == 1)
                    {
                        var taskTitleList = ConfigCtrl.Instance.Tables.TbGameTask.DataList.FindAll(x => x.Type == typeList[k].Type);
                        typeTaskList.Add(new Task(
                                    typeList[k].Id,
                                    typeList[k].Type,
                                    typeList[k].Taskid,
                                    typeList[k].Target,
                                    typeList[k].Award,
                                    typeList[k].Valid,
                                    (float)info.arrayInfo[i].n64Total,
                                    ToolUtil.ValueByBit(info.arrayInfo[i].n64Reward, k + 1)));
                    }

                }
                taskInfos.Add(info.arrayInfo[i].nType, typeTaskList);
                taskTimeList.Add(info.arrayInfo[i].nType, info.arrayInfo[i].n64Retime);
            }
        }
    }

    public class Task
    {
        public int selfID;//自增id
        public int type;//任务类型
        public int taskID;//任务id
        public long taskTarget;//任务目标 
        public int award;//奖励数额
        public int valid;//任务是否有效
        public float total;//累计
        public bool IsCollect;//是否领取奖励
        public string title;
        public int gameID;
        public Task(int _selfID, int _type, int _taskID, long _taskTarget, int _award, int _valid, float _total, bool _IsCollect)
        {
            selfID = _selfID;
            type = _type;
            taskID = _taskID;
            taskTarget = _taskTarget;
            award = _award;
            valid = _valid;
            total = _total;
            IsCollect = _IsCollect;

        }

    }
}
