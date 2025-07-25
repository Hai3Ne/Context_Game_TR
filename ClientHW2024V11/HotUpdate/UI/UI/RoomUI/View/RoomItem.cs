using DG.Tweening;
using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private Image titleImg;
    [SerializeField] private RawImage gameIcon;
    [SerializeField] private List<Image> btnTopTxtImgList = new List<Image>();
    [SerializeField] private Text ante1Txt;
    [SerializeField] private Text ante2Txt;
    [SerializeField] private Text ante3Txt;
    [SerializeField] private Text limiteTile1Txt;
    [SerializeField] private Text limiteTile2Txt;
    [SerializeField] private Text limiteTile3Txt;
    [SerializeField] private Text limite1Txt;
    [SerializeField] private Text limite2Txt;
    [SerializeField] private Text limite3Txt;
    [SerializeField] private Button enter1GameBtn;
    [SerializeField] private Button enter2GameBtn;
    [SerializeField] private Button enter3GameBtn;
    [SerializeField] private GameObject btnGroup;
    [SerializeField] private List<GameObject> personNumberList = new List<GameObject>();
    [SerializeField] private List<GameObject> roomItemAni = new List<GameObject>();
    [SerializeField] private GameObject tips;
    [SerializeField] private GameObject mask;
    private int id;
    private int roomid;
    private float room1Limite;
    private float room2Limite;
    private float room3Limite;
 
    [SerializeField] private GameObject normalIcon1_2;

    [SerializeField] private GameObject normalIcon2_2;
   
    [SerializeField] private GameObject normalIcon3_2;
    public void OnEnable()
    {
        enter1GameBtn.onClick.AddListener(OnEnter1GameBtn);
        enter2GameBtn.onClick.AddListener(OnEnter2GameBtn);
        enter3GameBtn.onClick.AddListener(OnEnter3GameBtn);
        enter1GameBtn.interactable = true;
        enter2GameBtn.interactable = true;
        enter3GameBtn.interactable = true;
        Canvas canvas = enter1GameBtn.GetComponent<Canvas>();
        if (GuideModel.Instance.bReachCondition(2))
        {
            GuideModel.Instance.SetFinish(2);
            tips.SetActive(true);
            canvas.overrideSorting = true;
        }
        else
        {
            canvas.overrideSorting = false;
    
            tips.SetActive(false);
        }


        normalIcon1_2.SetActive(!MainUIModel.Instance.bNormalGame);
        normalIcon2_2.SetActive(!MainUIModel.Instance.bNormalGame);
        normalIcon3_2.SetActive(!MainUIModel.Instance.bNormalGame);
    }

    public void OnDisable()
    {
        enter1GameBtn.onClick.RemoveListener(OnEnter1GameBtn);
        enter2GameBtn.onClick.RemoveListener(OnEnter2GameBtn);
        enter3GameBtn.onClick.RemoveListener(OnEnter3GameBtn);
    }
    public void SetUpItem(cfg.Game.GameRoomConfig data) 
    {
        id = data.Id;
        roomid = data.Id;
        
        room1Limite = data.Room1Limite * 10000;
        room2Limite = data.Room2Limite * 10000;
        room3Limite = data.Room3Limite * 10000;
        titleImg.sprite = AtlasSpriteManager.Instance.GetSprite("Room:" + data.TitleImg); 
        titleImg.SetNativeSize();
        ToolUtil.SetRawTexture(gameIcon, "UI/Texture/English/Room/" + data.GameIcon);
        gameIcon.SetNativeSize();
        ante1Txt.text = $"{data.Room1Ante*  10000 / ToolUtil.GetGoldRadio()}";
        ante2Txt.text = $"{data.Room2Ante* 10000/ ToolUtil.GetGoldRadio()}";
        ante3Txt.text = $"{data.Room3Ante* 10000 / ToolUtil.GetGoldRadio()}";
        limite1Txt.text = $"{data.Room1Limite* 10000 / ToolUtil.GetGoldRadio()}";
      //  limite1Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = 13 * (limite1Txt.text.Length - 1);
      //  limite1Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().spacing = -4 * (limite1Txt.text.Length - 1);
        limite2Txt.text = $"{data.Room2Limite*10000 / ToolUtil.GetGoldRadio()}";
      //  limite2Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = 13 * (limite2Txt.text.Length - 1);
     //   limite2Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().spacing = -4 * (limite2Txt.text.Length - 1);
        limite3Txt.text = $"{data.Room3Limite * 10000 / ToolUtil.GetGoldRadio()}";
     //   limite3Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().padding.left = 13 * (limite3Txt.text.Length - 1);
      //  limite3Txt.transform.parent.GetComponent<HorizontalLayoutGroup>().spacing = -4 * (limite3Txt.text.Length - 1);
        var onLineList = MainUIModel.Instance.onLineData.onlineList;
        onLineList.TryGetValue(id, out int[] onlineArr);
        if (onlineArr!=null)
        {
            personNumberList[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{onlineArr[1]}";
            personNumberList[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{onlineArr[2]}";
            personNumberList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{onlineArr[3]}";
        }
        else
        {
            personNumberList[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{0}";
            personNumberList[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{0}";
            personNumberList[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{0}";
        }
        if (data.RoomNum==1)
        {
            enter1GameBtn.gameObject.SetActive(true);
            enter2GameBtn.gameObject.SetActive(false);
            enter3GameBtn.gameObject.SetActive(false);
        }
        else
        {
            enter1GameBtn.gameObject.SetActive(true);
            enter2GameBtn.gameObject.SetActive(true);
            enter3GameBtn.gameObject.SetActive(true);
        }
         
        SetBtnSprite(data.Room1Limite * 10000, enter1GameBtn.GetComponent<Image>(), personNumberList[0].GetComponent<Image>(), roomItemAni[0]);
        SetBtnSprite(data.Room2Limite * 10000, enter2GameBtn.GetComponent<Image>(), personNumberList[1].GetComponent<Image>(), roomItemAni[1]);
        SetBtnSprite(data.Room3Limite * 10000, enter3GameBtn.GetComponent<Image>(), personNumberList[2].GetComponent<Image>(), roomItemAni[2]);
        if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room1Limite * 10000)
        {
            enter1GameBtn.gameObject.transform.SetAsFirstSibling();
        }
        if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room2Limite * 10000)
        {
            enter2GameBtn.gameObject.transform.SetAsFirstSibling();
        }
        if ((float)(MainUIModel.Instance.Golds / ToolUtil.GetGoldRadio()) >= data.Room3Limite * 10000)
        {
            enter3GameBtn.gameObject.transform.SetAsFirstSibling();
        }
    /*    for (int i = 0; i < btnTopTxtImgList.Count; i++)
        {//FJ_txt_JACKPOT
            btnTopTxtImgList[i].sprite = AtlasSpriteManager.Instance.GetSprite("Room:" + data.JackPOTName);
            btnTopTxtImgList[i].SetNativeSize();
            //if (data.JackPOTName.Equals("FJ_txt_JACKPOT"))
            //{
            //    //btnTopTxtImgList[i].rectTransform.anchoredPosition = new Vector3(-52f, 51.5f, 0f);
            //    personNumberList[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(170.9f,-36f,0f);
            //}
            //else if (data.JackPOTName.Equals("FJ_txt_CASH RECOMPENSA"))
            //{
            //    //btnTopTxtImgList[i].rectTransform.anchoredPosition = new Vector3(-58f, 51.5f, 0f);
            //    personNumberList[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(170.9f, -36f, 0f);
            //}
            //else
            //{
            //    //btnTopTxtImgList[i].rectTransform.anchoredPosition = new Vector3(-58f, 51.5f, 0f);
            //    personNumberList[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(170.9f, -36f, 0f);
            //}         
            
        }*/
    }

    public void OnEnter1GameBtn() 
    {
        mask.SetActive(false);
        tips.SetActive(false);
        StartCoroutine(ToolUtil.DelayResponse(enter1GameBtn, 1f));
        CoreEntry.gAudioMgr.PlayUISound(46);
        MainUIModel.Instance.RoomId = roomid ;
        RoomTypeData data = new RoomTypeData();
        data.id = id;
        data.roomType = 1;
        data.roomId = roomid;
    
        MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);
       
       // MainUICtrl.Instance.SendEnterGameRoom(id,1);
    }
    public void OnEnter2GameBtn()
    {
        StartCoroutine(ToolUtil.DelayResponse(enter2GameBtn, 1f));
        CoreEntry.gAudioMgr.PlayUISound(46);
        var currCoin = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));
        bool satisfy = (float)room2Limite > currCoin;
        float difference = (float)room2Limite - currCoin;

        if (satisfy)
        {
            enter2GameBtn.transform.DOShakePosition(0.2f,2);
            ShowTips(difference);
            return;
        }
        MainUIModel.Instance.RoomId = roomid ;
        RoomTypeData data = new RoomTypeData();
        data.id = id;
        data.roomType = 2;
        data.roomId = roomid;
        MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);

    }
    public void OnEnter3GameBtn()
    {
        StartCoroutine(ToolUtil.DelayResponse(enter3GameBtn, 1f));
        CoreEntry.gAudioMgr.PlayUISound(46);
        var currCoin = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.Golds), new CultureInfo("en"));
        bool satisfy = (float)room3Limite > currCoin;
        float difference = (float)room3Limite - currCoin;
        if (satisfy)
        {
            enter3GameBtn.transform.DOShakePosition(0.2f, 2);
            ShowTips(difference);
            return;
        }
        MainUIModel.Instance.RoomId = roomid ;
        RoomTypeData data = new RoomTypeData();
        data.id = id;
        data.roomType = 3;
        data.roomId = roomid;
        MainUICtrl.Instance.SendEnterGameRoom(data.roomId, data.roomType);

    }

    public void SetBtnSprite(float limit,Image image,Image peopleImage,GameObject ani) 
    {
        if ((float)(MainUIModel.Instance.Golds/ToolUtil.GetGoldRadio()) < limit)
        {
            image.sprite = AtlasSpriteManager.Instance.GetSprite("Room:hbdd");
            ani.SetActive(false);
            //peopleImage.sprite = AtlasSpriteManager.Instance.GetSprite("Room:rs2");
        }
        else
        {
            image.sprite = AtlasSpriteManager.Instance.GetSprite("Room:btn_room_5");
            ani.SetActive(true);
            //peopleImage.sprite = AtlasSpriteManager.Instance.GetSprite("Room:rs1");
        }
    }

    public void ShowTips(float num) 
    {
        ToolUtil.FloattingText("金币不足，无法进入房间", MainPanelMgr.Instance.GetPanel("RoomPanel").transform);
        /*        var str = string.Format("Também custa <color=#FFE684>{0}</color> Ficha para entrar na sala", num.ToString("f2", new CultureInfo("en")));
                SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
                tips.SetTipsPanel("Dicas", str, "Comprar", delegate
                {
                    MainUICtrl.Instance.OpenShopPanel();
                }, false);*/
    }

   
}
