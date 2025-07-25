using DG.Tweening;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class RoomPanel : PanelBase
    {
        [SerializeField] public Material material;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);


        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            Screen.orientation = ScreenOrientation.Portrait;
            m_Btn_Room1.interactable = true;
            m_Btn_Room2.interactable = true;
            m_Btn_Room3.interactable = true;
            reloadUI();
            //SetRoomList();

            if (GuideModel.Instance.bReachCondition(0)&& !PlayerPrefs.HasKey(MainUIModel.Instance.palyerData.m_i8roleID + "===>-=-" + 11)) 
            {
                GuideModel.Instance.SetFinish(1);
                MainUICtrl.Instance.OpenGuidePanel(m_Btn_Room1.transform, onRoomBtn1, 11);
            }

        }

        private void reloadUI()
        {

            var data = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList.Find(x => x.Id == MainUIModel.Instance.RoomId);
            var room1Limite = data.Room1Limite * 10000;
            var room2Limite = data.Room2Limite * 10000;
            var room3Limite = data.Room3Limite * 10000;
            var textLimt1 = m_Btn_Room1.transform.Find("textlimt").GetComponent<Text>();
            var textLimt2 = m_Btn_Room2.transform.Find("textlimt").GetComponent<Text>();
            var textLimt3 = m_Btn_Room3.transform.Find("textlimt").GetComponent<Text>();

            var textgold1 = m_Btn_Room1.transform.Find("textgold").GetComponent<Text>();
            var textgold2 = m_Btn_Room2.transform.Find("textgold").GetComponent<Text>();
            var textgold3 = m_Btn_Room3.transform.Find("textgold").GetComponent<Text>();

            var textNum1 = m_Btn_Room1.transform.Find("Imagebg/textNum").GetComponent<Text>();
            var textNum2 = m_Btn_Room2.transform.Find("Imagebg/textNum").GetComponent<Text>();
            var textNum3 = m_Btn_Room3.transform.Find("Imagebg/textNum").GetComponent<Text>();

            textLimt1.text = $"{data.Room1Limite * 10000 / ToolUtil.GetGoldRadio()}";
            textLimt2.text = $"{data.Room2Limite * 10000 / ToolUtil.GetGoldRadio()}";
            textLimt3.text = $"{data.Room3Limite * 10000 / ToolUtil.GetGoldRadio()}";

            textgold1.text = $"{data.Room1Ante * 10000 / ToolUtil.GetGoldRadio()}";
            textgold2.text = $"{data.Room2Ante * 10000 / ToolUtil.GetGoldRadio()}";
            textgold3.text = $"{data.Room3Ante * 10000 / ToolUtil.GetGoldRadio()}";
            var onLineList = MainUIModel.Instance.onLineData.onlineList;
            onLineList.TryGetValue(data.Id, out int[] onlineArr);
            if (onlineArr != null)
            {
                textNum1.text = $"{onlineArr[1]}";
                textNum2.text = $"{onlineArr[2]}";
                textNum3.text = $"{onlineArr[3]}";
            }
            else
            {
                textNum1.text = $"{0}";
                textNum2.text = $"{0}";
                textNum3.text = $"{0}";
            }

            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) < data.Room1Limite * 10000)
            {
                m_Btn_Room1.transform.GetComponent<Image>().material = material;
            }
            else
            {
                m_Btn_Room1.transform.GetComponent<Image>().material = null;
            }
            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) < data.Room2Limite * 10000)
            {
                m_Btn_Room2.transform.GetComponent<Image>().material = material;
            }
            else
            {
                m_Btn_Room2.transform.GetComponent<Image>().material = null;
            }

            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) < data.Room3Limite * 10000)
            {
                m_Btn_Room3.transform.GetComponent<Image>().material = material;
            }
            else
            {
                m_Btn_Room3.transform.GetComponent<Image>().material = null;
            }


            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room1Limite * 10000)
            {
                m_Btn_Room1.gameObject.transform.SetAsFirstSibling();
            }
            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room2Limite * 10000)
            {
                m_Btn_Room2.gameObject.transform.SetAsFirstSibling();
            }
            if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room3Limite * 10000)
            {
                m_Btn_Room3.gameObject.transform.SetAsFirstSibling();
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Room1.onClick.AddListener(onRoomBtn1);
            m_Btn_Room2.onClick.AddListener(onRoomBtn2);
            m_Btn_Room3.onClick.AddListener(onRoomBtn3);
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Room1.onClick.RemoveListener(onRoomBtn1);
            m_Btn_Room2.onClick.RemoveListener(onRoomBtn2);
            m_Btn_Room3.onClick.RemoveListener(onRoomBtn3);
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);

        }

        private void onRoomBtn3()
        {
            var config = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList.Find(x => x.Id == MainUIModel.Instance.RoomId);
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Room3, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            var room3Limite = config.Room3Limite * 10000;
            var currCoin = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));
            bool satisfy = (float)room3Limite > currCoin;
            float difference = (float)room3Limite - currCoin;

            if (satisfy)
            {
                m_Btn_Room3.transform.DOShakePosition(0.2f, 2);
                ShowTips(difference);
                return;
            }
            MainUIModel.Instance.RoomId = config.Id;
            RoomTypeData data = new RoomTypeData();
            data.id = config.Id;
            data.roomType = 3;
            data.roomId = config.Id;
            MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);
        }

        private void onRoomBtn2()
        {
            var config = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList.Find(x => x.Id == MainUIModel.Instance.RoomId);
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Room2, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            var room2Limite = config.Room2Limite * 10000;
            var currCoin = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));
            bool satisfy = (float)room2Limite > currCoin;
            float difference = (float)room2Limite - currCoin;

            if (satisfy)
            {
                m_Btn_Room2.transform.DOShakePosition(0.2f, 2);
                ShowTips(difference);
                return;
            }
            MainUIModel.Instance.RoomId = config.Id;
            RoomTypeData data = new RoomTypeData();
            data.id = config.Id;
            data.roomType = 2;
            data.roomId = config.Id;
            MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);

        }

        private void onRoomBtn1()
        {
            var config = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList.Find(x => x.Id == MainUIModel.Instance.RoomId);

            StartCoroutine(ToolUtil.DelayResponse( m_Btn_Room1, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.RoomId = config.Id;
            RoomTypeData data = new RoomTypeData();
            data.id = config.Id;
            data.roomType = 1;
            data.roomId = config.Id;

            MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);
            GuideModel.Instance.SetFinish(11);
        }

        public void ShowTips(float num)
        {
            ToolUtil.FloattingText("金币不足，无法进入房间", transform);
        }
        #endregion



        public void OnCloseBtn() 
        {

            CoreEntry.gAudioMgr.PlayUISound(46);
         
            MainPanelMgr.Instance.HideDialog("RoomPanel");
  
        }



  


       
    }
}
