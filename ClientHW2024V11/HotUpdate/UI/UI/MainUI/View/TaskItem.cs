using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SEZSJ;
using System;

namespace HotUpdate
{
    public class TaskItem : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text iconNumTxt;
        [SerializeField] private Text taskTitle;
        [SerializeField] private Image progress;
        [SerializeField] private Button claimBtn;
        [SerializeField] private GameObject calimedTxt;
        [SerializeField] private GameObject canNotClaimBtn;
        [SerializeField] private Text progressTxt;
        private int taskType;
        private int taskId;
        private int selfId;

        private Button BtncanNotClaim;
        TaskPanel taskPanel;
        int gameID;
        private void Awake()
        {
            BtncanNotClaim = canNotClaimBtn.GetComponent<Button>();
        }
        public void SetUpItem(Task data, TaskPanel taskPanel)
        {
            this.taskPanel = taskPanel;
            taskType = data.type;
            taskId = data.taskID;
            selfId = data.selfID;
            gameID = data.gameID;
            //var targetNum = ToolUtil.AbbreviateNumber(data.taskTarget, true);
            //if (targetNum.Contains($".0"))
            //{
            //    targetNum = (float.Parse(targetNum)).ToString("f0");
            //}
            taskTitle.text = $"海底爬虫消耗 {ToolUtil.AbbreviateNumberf0(data.taskTarget, true)} ";
            iconNumTxt.text = $"x{(double)data.award / 100f}";
            progress.fillAmount = (float)data.total / data.taskTarget;
            calimedTxt.gameObject.SetActive(data.IsCollect);
            canNotClaimBtn.SetActive(data.total < data.taskTarget);
            claimBtn.gameObject.SetActive(!data.IsCollect && data.total >= data.taskTarget);

            progressTxt.text = $"{ToolUtil.AbbreviateNumber(data.total)}/{ToolUtil.AbbreviateNumber(data.taskTarget, true)}";
            if (data.IsCollect)
            {
                transform.SetAsLastSibling();

            }
        }

        private void OnEnable()
        {
            claimBtn.onClick.AddListener(OnClaimBtn);
            BtncanNotClaim.onClick.AddListener(EnterGameID);
            calimedTxt.GetComponent<Button>().onClick.AddListener(OnClaimedBtn);
        }

        private void OnDisable()
        {
            claimBtn.onClick.RemoveListener(OnClaimBtn);
            BtncanNotClaim.onClick.RemoveListener(EnterGameID);
            calimedTxt.GetComponent<Button>().onClick.RemoveListener(OnClaimedBtn);
        }

        public void OnClaimBtn()
        {
            //StartCoroutine(ToolUtil.DelayResponse(claimBtn, 2f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendGetTaskAward(taskType, taskId);
        }

        private void OnClaimedBtn()
        {
            ToolUtil.FloattingText("已领取完毕，请去完成其他任务吧", MainPanelMgr.Instance.GetPanel("TaskPanel").transform);
        }

        private void EnterGameID()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!taskPanel)
            {
                var panel = MainPanelMgr.Instance.GetPanel("GuildPanel");
                panel?.Close();
            }
            else
            {
                taskPanel.Close();
            }
           
            if(MainUIModel.Instance.RoomData != null)
            {
                if (MainUIModel.Instance.RoomData.nGameType != 15)
                {
                    MainUICtrl.Instance.SendLevelGameRoom();
                    MainUIModel.Instance.RoomLevelCallback = () =>
                    {
                        MainUICtrl.Instance.SendEnterGameRoom(15, 1);
                    };
                   
                }
            }
            else
            {
                MainUICtrl.Instance.SendEnterGameRoom(15, 1);
            }

          
        }
    }
}
