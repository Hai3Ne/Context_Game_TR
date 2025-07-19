using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameItem : MonoBehaviour
{
    [SerializeField] private RawImage bannerBg;
    [SerializeField] private Image maskImg;
    [SerializeField] private GameObject tagObject;
    [SerializeField] private Button clickBtn;
    [SerializeField] private Transform animaPos;
    [SerializeField] private Transform jx;
    [SerializeField] private TextMeshProUGUI peopleNumTxt;
    [SerializeField] private GameObject StayTunedObject;
    public bool isInit;
    private int roomid;
    private int type;
    private GameObject anima;
    public string gamename;
    private void OnEnable()
    {
        clickBtn.onClick.AddListener(OnClickBtn);
    }

    private void OnDisable()
    {
        clickBtn.onClick.RemoveListener(OnClickBtn);
    }

    public void SetUpItem(cfg.Game.GameRoomConfig data)
    {

    }

    public void SetUpItem1(cfg.Game.GameRoomConfig data)
    {
        
    }

    public void OnClickBtn()
    {
        if (type >= 2)
        {
            //ToolUtil.FloattingText("111122223333", MainPanelMgr.Instance.GetPanel("MainUIPanel").transform);
            return;
        }
        if (roomid == 12)
        {
            if (MainUIModel.Instance.palyerData.m_i4Viplev < 1)
            {
                ToolUtil.FloattingText("Necessidade de alcançar o VIP1", MainPanelMgr.Instance.GetPanel("MainUIPanel").transform);
                return;
            }
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.RoomId = roomid;
            RoomTypeData data = new RoomTypeData();
            data.id = 12;
            data.roomType = 1;
            data.roomId = roomid;
            UICtrl.Instance.OpenView("CommonUpdatePanel", data);
        }
        else
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.RoomId = roomid;
            MainPanelMgr.Instance.ShowDialog("RoomPanel");
            //UICtrl.Instance.OpenView("RoomPanel");
        }
    }


}
