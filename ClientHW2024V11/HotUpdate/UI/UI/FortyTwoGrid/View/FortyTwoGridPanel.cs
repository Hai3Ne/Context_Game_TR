using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class FortyTwoGridPanel : PanelBase
    {
        private Dictionary<int, List<GameObject>> ItemPool = new Dictionary<int, List<GameObject>>();

        private int m_gameState = 0;
        private int[] m_chipArr = new int[5];
        private int m_selecChipIndex = 0;
        private Dictionary<int, FortyTwoGridTabItemInfo> itemList = new Dictionary<int, FortyTwoGridTabItemInfo>();
        private List<FortyTwoGridTabInfo> FortyTwoGridTabList = new List<FortyTwoGridTabInfo>();
        private Dictionary<int, List<GameObject>> ClearItemArr = new Dictionary<int, List<GameObject>>();
        private long nowGold = 0;
        private int roomType = 0;
        private GoldEffectNew m_Gold_Effect;
        private bool isDanji = false;
        private long danjiGold = 0;
        private bool isZidong = false;
        private int itemHeight = 321;
        private int lineHeight = 107;
        protected override void Awake()
        {
            base.Awake();

            GetBindComponents(gameObject);
            FortyTwoGridModel.Instance.InitFortyTwoGridConfig();

            //CoreEntry.gTimeMgr.AddTimer(5, true, () =>
            //  {
            //      FortyTwoGridModel.Instance.JackpotNum += 120000;
            //      ReloadUI();
            //  }, 555555);

            //if (GuideModel.Instance.bReachCondition(5))
            //{
            //    GuideModel.Instance.SetFinish(5);
            //    OnXxAutoBtn();
            //    MainUICtrl.Instance.OpenGuidePanel(m_Btn_Back.transform, OnClickBackBtn);
            //}
        }
        protected override void Start()
        {
            base.Start();
            Transform roleNode = transform.Find("RawImage/role");
            float scaleValue = gameObject.GetComponent<RectTransform>().sizeDelta.y / 1334;
            float newScaleValue = scaleValue > 1 ? 1 : 0.7f;
            if (roleNode)
            {
                roleNode.GetComponent<RectTransform>().localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                playRoleAni(false);
            }

            // if (m_Btn_Rank)
            // {
            //     m_Btn_Rank.GetComponent<RectTransform>().localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
            //     m_Btn_Rank.GetComponent<RectTransform>().localPosition = new Vector3(-293, 434 - (scaleValue > 1 ? 0 : 43), 0);
            // }
            //
            // if (m_Btn_Task)
            // {
            //     m_Btn_Task.GetComponent<RectTransform>().localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
            //     m_Btn_Task.GetComponent<RectTransform>().localPosition = new Vector3(-293, 602 - (scaleValue > 1 ? 0 : 100), 0);
            // }
            //
            // if (m_Btn_Tour)
            // {
            //     m_Btn_Tour.GetComponent<RectTransform>().localScale = new Vector3(newScaleValue, newScaleValue, newScaleValue);
            //     m_Btn_Tour.GetComponent<RectTransform>().localPosition = new Vector3(293, 434 - (scaleValue > 1 ? 0 : 43), 0);
            // }

            // if (m_Btn_Tips)
            // {
            //     m_Btn_Tips.GetComponent<RectTransform>().localPosition = new Vector3(293, 555 - (scaleValue > 1 ? 0 : 43), 0);
            // }

            var obj = CoreEntry.gResLoader.ClonePre("UI/UITemplate/Gold_EffectNew", m_Rect_Effect.transform, false, false);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = m_Rect_Effect.sizeDelta;
            rect.anchoredPosition3D = new Vector3(0, 0, 88);
            rect.localScale = Vector3.one;
            m_Gold_Effect = obj.GetComponent<GoldEffectNew>();
            for (int i = 0; i < 7; i++)
            {
                
                for (int j = 0; j < 3; j++)
                {
                    var img = Instantiate(m_Rect_copy);
                    img.gameObject.SetActive(true);
                  
                    img.SetParent(m_Rect_bgPanel);
                    img.localScale = new Vector3(1, 1, 1);
                    img.anchoredPosition = new Vector2((-307 + i % 2 * 115) + 115 * j * 2, 256 - i * 115); //new Vector3(-307f, 13.192f,0f); 

                }
            }

           

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            CoreEntry.gAudioMgr.PlayUIMusic(299);
            m_gameState = 0;
            itemList.Clear();
            FortyTwoGridTabList.Clear();
            roomType = 1;
            if (param != null)
                roomType = (int)param;
            SetRollBtnRorate();
            isZidong = false;
            m_Btn_XxAuto.interactable = isZidong;
            m_Btn_XxAuto.gameObject.SetActive(isZidong);
            m_Btn_Reload.interactable = !isZidong;
            m_Btn_Auto.interactable = !isZidong;
            m_Txt_Reward.text = "0";
            if (roomType == 0)
            {
                isDanji = true;
                roomType = 1;
                FortyTwoGridModel.Instance.JackpotNum = 4560000;
            }

            if (isDanji)
            {
                if (PlayerPrefs.HasKey("FortyTwoGridDanji"))
                {
                    danjiGold = PlayerPrefs.GetInt("FortyTwoGridDanji");
                }
                else
                {
                    danjiGold = 5000000;
                    PlayerPrefs.SetInt("FortyTwoGridDanji", (int)danjiGold);
                }

                m_Txt_Gold.text = ToolUtil.AbbreviateNumber(danjiGold);
                nowGold = danjiGold;
            }
            else
            {
                m_Txt_Gold.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.Golds);
                nowGold = MainUIModel.Instance.Golds;//MainUIModel.Instance.palyerData.m_i8Golds;
            }

            RegisterListener();
            switch (roomType)
            {
                case 1:
                    m_chipArr = new int[5] { 3000, 5000, 9000, 21000, 50000 };
                    break;
                case 2:
                    m_chipArr = new int[5] { 70000, 150000, 300000, 600000, 900000 };
                    break;
                case 3:
                    m_chipArr = new int[5] { 1200000, 1500000, 2000000, 3000000, 6000000 };
                    break;
                default:
                    break;
            }
            InitGame();
            m_Txt_JackPot.text = FortyTwoGridModel.Instance.JackpotNum + "";
            ReloadUI();
            if (GuideModel.Instance.bReachCondition(11)&& !PlayerPrefs.HasKey(MainUIModel.Instance.palyerData.m_i8roleID + "===>-=-" + 12)) 
            {
                MainUICtrl.Instance.OpenGuidePanel(m_Btn_Reload.transform, OnReloadBtn, 12);
            
            }
            RefreshRedDot();

        }


        private void RefreshRedDot()
        {
            //任务红点
            var taskCount = 0;
            if (MainUIModel.Instance.taskInfo != null)
            {
                foreach (var item in MainUIModel.Instance.taskInfo.taskInfos)
                {
                    taskCount += item.Value.FindAll(x => x.total > x.taskTarget && !x.IsCollect).Count;
                }
                m_Dragon_taskRedDot.gameObject.SetActive(taskCount > 0);
                if(taskCount > 0)
                {
                    if (GuideModel.Instance.bReachCondition(4))
                    {
                        GuideModel.Instance.SetFinish(4);
                        MainUICtrl.Instance.OpenGuidePanel(m_Btn_Task.transform, OnTaskBtn);
                    }          
                }         
            }

        

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_gameState = 0;
            CoreEntry.gAudioMgr.StopMusic(299);
            for (int i = m_Mask2D_Panel.transform.childCount - 1; i >= 0; i--)
            {
                var obj = m_Mask2D_Panel.transform.GetChild(0);

                DestroyImmediate(obj.gameObject);
            }

            UnRegisterListener();
        }
        protected override void Update()
        {
            base.Update();
        }
        public void ReloadGame()
        {
            /*            m_gameState = 0;
                        for (int i = m_Mask2D_Panel.transform.childCount - 1; i >= 0; i--)
                        {
                            var obj = m_Mask2D_Panel.transform.GetChild(0);

                            DestroyImmediate(obj.gameObject);
                        }
                        itemList.Clear();
                        FortyTwoGridTabList.Clear();
                        isZidong = false;
                        m_Btn_Reload.interactable = !isZidong;
                        m_Txt_Reward.text = "0.00";

                        InitGame();
                        ReloadUI();*/
            if (isDanji)
            {
                m_Txt_Gold.text = ToolUtil.AbbreviateNumber(danjiGold);
            }
            else
            {
                m_Txt_Gold.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.Golds);
            }

            ReloadUI();

            if (m_gameState == 1 || m_gameState == 2)
            {
                for (int i = 0; i < FortyTwoGridTabList.Count; i++)
                {
                    var pos1 = FortyTwoGridTabList[i].obj.anchoredPosition3D;
                    var pos4 = new Vector3(pos1.x, 0f, pos1.z);
                    FortyTwoGridTabList[i].obj.localPosition = pos4;

                }
            }

            if (isZidong || m_gameState == 2)
            {
                m_gameState = 0;
                OnReloadBtn();
            }
            else
            {
                setBtnState(true);
                SetRollBtnRorate(false);
            }
        }
        public void ReloadUI()
        {
            //m_Txt_JackPot.text = ToolUtil.ShowF2Num(FortyTwoGridModel.Instance.JackpotNum);

            m_Txt_JackPot.text = FortyTwoGridModel.Instance.JackpotNum + "";
        }
        private void UpdateGold(long gold)
        {
            m_Txt_Gold.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.Golds);
            nowGold = MainUIModel.Instance.Golds;
        }


        public void InitGame()
        {
        
            m_Btn_Back.gameObject.SetActive(!isDanji);
       

            m_selecChipIndex = 0;
            m_Btn_jian.interactable = false;
            m_Btn_Jia.interactable = true;

            m_Txt_Couma.text = ToolUtil.ShowF2Num2(m_chipArr[m_selecChipIndex]) + "";
            FortyTwoGridModel.Instance.SetInitData();
            // 初始化列表  new Vector3(-307f, 13.192f,0f);
            var itemY = -3.3f;
            for (int i = 0; i < 6; i++)
            {
                var obj = ToolUtil.ClonePrefab(m_Rect_Item.gameObject, m_Mask2D_Panel.transform, "item");
                obj.gameObject.SetActive(true);
                obj.anchoredPosition3D = new Vector3(-308 + 123 * i, itemY, 0);
                //obj.anchoredPosition3D = new Vector3(-307 + 122 * i, itemY, 0);
                for (int j = 0; j < 7; j++)
                {
                    var icon = obj.gameObject.transform.GetChild(j).gameObject.GetComponent("Image") as Image;
                    var data = FortyTwoGridModel.Instance.gameData[0].list[j * 6 + i];
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(data.id);
                    icon.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + config.Pic);
                    icon.name = config.Pic;
                    FortyTwoGridTabItemInfo info = new FortyTwoGridTabItemInfo();
                    info.id = j * 6 + i;
                    info.oldId = j * 6 + i;
                    info.type = i;
                    info.line = j;
                    info.icon = icon;
                    info.isReplace = true;
                    itemList[j * 6 + i] = info;
                }
                FortyTwoGridTabInfo info1 = new FortyTwoGridTabInfo();
                info1.id = i;
                info1.obj = obj;
                FortyTwoGridTabList.Add(info1);
            }

        }
        #region 事件绑定
        public void RegisterListener()
        {
            Message.AddListener(MessageName.GAME_ZEUS_START, reciverMoveUI);
            Message.AddListener<GameObject, string>(MessageName.ANIEnd, FlyAniEnd);
            Message.AddListener(MessageName.GAME_ZEUS_RELOADUI, ReloadUI);

            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Reload.onClick.AddListener(OnReloadBtn);
            m_Btn_Back.onClick.AddListener(OnClickBackBtn);
            m_Btn_Jia.onClick.AddListener(OnClickJiaBtn);
            m_Btn_jian.onClick.AddListener(OnClickJianBtn);
          //  m_Tog_Zidong.onValueChanged.AddListener(OnChangeZidong);
            m_Btn_Tips.onClick.AddListener(OnClickRuleBtn);
            m_Btn_Shop.onClick.AddListener(OnShop);
      
         

            m_Btn_Task.onClick.AddListener(OnTaskBtn);
            m_Btn_Rank.onClick.AddListener(OnRankBtn);
            m_Btn_Tour.onClick.AddListener(OnTourBtn);
    
            m_Btn_Auto.onClick.AddListener(OnAutoBtn);
  
            m_Btn_XxAuto.onClick.AddListener(OnXxAutoBtn);

            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            m_Btn_Back.interactable = true;

            Message.AddListener(MessageName.REFRESH_MAINUI_PANEL, RefreshRedDot);
        }
        public void UnRegisterListener()
        {
            Message.RemoveListener(MessageName.GAME_ZEUS_RELOADUI, ReloadUI);
            Message.RemoveListener(MessageName.GAME_ZEUS_START, reciverMoveUI);
            Message.RemoveListener<GameObject, string>(MessageName.ANIEnd, FlyAniEnd);

            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            m_Btn_Tips.onClick.RemoveListener(OnClickRuleBtn);

            m_Btn_Reload.onClick.RemoveListener(OnReloadBtn);
            m_Btn_Back.onClick.RemoveListener(OnClickBackBtn);
            m_Btn_Jia.onClick.RemoveListener(OnClickJiaBtn);
            m_Btn_jian.onClick.RemoveListener(OnClickJianBtn);
           // m_Tog_Zidong.onValueChanged.RemoveListener(OnChangeZidong);
            
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);

            m_Btn_Task.onClick.RemoveListener(OnTaskBtn);
            m_Btn_Rank.onClick.RemoveListener(OnRankBtn);
            m_Btn_Tour.onClick.RemoveListener(OnTourBtn);

            m_Btn_Auto.onClick.RemoveListener(OnAutoBtn);
            m_Btn_XxAuto.onClick.RemoveListener(OnXxAutoBtn);
            m_Btn_Shop.onClick.RemoveListener(OnShop);
            Message.RemoveListener(MessageName.REFRESH_MAINUI_PANEL, RefreshRedDot);
        }
        #endregion

        #region 播放
        private void playRoleAni(bool needChange = true)
        {
            Transform roleNode = transform.Find("RawImage/role");
            if (roleNode)
            {
                RectTransform roleRect = roleNode.GetComponent<RectTransform>();
                int changeValue = needChange ? -1 : 1;
                roleRect.localPosition = new Vector3(roleRect.localPosition.x * changeValue, roleRect.localPosition.y, roleRect.localPosition.z);
                roleRect.localScale = new Vector3(roleRect.localScale.x * changeValue, roleRect.localScale.y, roleRect.localScale.z);
                ToolUtil.PlayAnimation(roleNode.GetChild(0), "animation", false, () =>
                {
                    playRoleAni();
                });
            }
        }
 

        /// <summary>
        /// 开始动画
        /// </summary>
        private void StartAni()
        {
            m_gameState = 1;
            setBtnState(false);
            SetRollBtnRorate(true);
            StartCoroutine(playMoveUI());
        }
        /// <summary>
        /// 播放item进入动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator playMoveUI()
        {
            CoreEntry.gAudioMgr.PlayUISound(298);
            m_Txt_Reward.text = "0";
            var num = nowGold - (long)(m_chipArr[m_selecChipIndex]);
            m_Txt_Gold.text = ToolUtil.AbbreviateNumber(num);
            for (int i = 0; i < itemList.Count; i++)
            {
                var icon = itemList[i].icon;
                var chengObj = icon.transform.Find("Rect_cheng");
                if (chengObj)
                {
                    recoderAni(6, chengObj.gameObject);
                }
            }
            for (int i = 0; i < FortyTwoGridTabList.Count; i++)
            {
                var pos1 = FortyTwoGridTabList[i].obj.anchoredPosition3D;
                var pos2 = new Vector3(pos1.x, -900f, pos1.z);
                var seq = DOTween.Sequence();
                FortyTwoGridTabList[i].obj.MoveUIFromTo(pos1, pos2, 0.15f);
                yield return new WaitForSecondsRealtime(0.02f);
            }
            yield return new WaitForSecondsRealtime(0.25f);
            m_gameState = 2;
        }

        IEnumerator playReciverMoveUI()
        {

            yield return new WaitWhile(() => m_gameState != 2);

            m_gameState = 3;
            m_Txt_Gold.text = ToolUtil.AbbreviateNumber(nowGold - FortyTwoGridModel.Instance.nAllBet);
            nowGold = FortyTwoGridModel.Instance.Gold;
            var list = new List<FortyTwoGridTabItemInfo>();
            if (FortyTwoGridModel.Instance.gameData.Count > 0)
            {
        
                for (int i = 0; i < itemList.Count; i++)
                {
                    var icon = itemList[i].icon;
                    var chengObj = icon.transform.Find("Rect_cheng");
                    if (chengObj)
                    {
                        recoderAni(6, chengObj.gameObject);
                    }
                    var data = FortyTwoGridModel.Instance.gameData[0].list[itemList[i].id];
                    itemList[i].isSHow = data.isShow;
                    itemList[i].isReplace = true;
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(data.id);
                    icon.name = config.Pic;
                    icon.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + config.Pic);
                    if (config.Type == 2)
                    {
                        var cheng = getItemAni(6, icon.transform);
                        var text = cheng.GetComponent<Image>();
                        text.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + "x" + config.Beishu);
                        cheng.SetActive(true);
                        cheng.name = "Rect_cheng";
                        text.SetNativeSize();
                        cheng.transform.localScale = new Vector3(1f, 1f, 1f);
                        cheng.transform.localPosition = Vector3.zero;
                    
                       // list.Add(itemList[i]);
                    }
                }
   
            }
            for (int i = 0; i < FortyTwoGridTabList.Count; i++)
            {
                var pos1 = FortyTwoGridTabList[i].obj.anchoredPosition3D;
                var pos2 = new Vector3(pos1.x, 900f, pos1.z);
                var pos3 = new Vector3(pos1.x, -18f, pos1.z);
                var pos4 = new Vector3(pos1.x, -3.3f, pos1.z);
                var seq = DOTween.Sequence();
                FortyTwoGridTabList[i].obj.anchoredPosition3D = pos2;
                if (i == FortyTwoGridTabList.Count - 1)
                {
                    seq.Append(FortyTwoGridTabList[i].obj.DOAnchorPos3D(pos3, 0.2f)).
                        Append(FortyTwoGridTabList[i].obj.DOAnchorPos3D(pos4, 0.1f)).
                        AppendCallback(() =>
                        {
                             CoreEntry.gAudioMgr.PlayUISound(295);
                        });
                }
                else
                {
                    seq.Append(FortyTwoGridTabList[i].obj.DOAnchorPos3D(pos3, 0.2f)).
                    Append(FortyTwoGridTabList[i].obj.DOAnchorPos3D(pos4, 0.1f));
                }
                yield return new WaitForSecondsRealtime(0.03f);
            }
            yield return new WaitForSecondsRealtime(0.1f);
            if (FortyTwoGridModel.Instance.gameData.Count > 1)
            {
                StartCoroutine(ShowNext());
            }
            else
            {
          
             
                endListAni();
            }
        }

        private void reciverMoveUI()
        {
            StartCoroutine(playReciverMoveUI());
        }
        /// <summary>
        /// 播放闪电动画
        /// </summary>
        /// <param name="item"></param>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        private void playShanDian(GameObject item, GameObject item1, GameObject item2)
        {
            CoreEntry.gAudioMgr.PlayUISound(4);
            CommonTools.PlayArmatureAni(item.transform, "zs_sd01_shandian", 1, () =>
            {
                CommonTools.PlayArmatureAni(item.transform, "zs_sd01_baodian", 1, () =>
                {
                    recoderAni(4, item);

                });
                CommonTools.PlayArmatureAni(item1.transform, "dz1", 1, () =>
                {
                    item2.SetActive(true);
                    var text = item1.transform.Find("Rect_cheng");
                    if (text)
                    {
                        recoderAni(6, text.gameObject);
                    }
                    recoderAni(1, item1);
                }, 1.2f);

            }, 0.8f);
        }

        /// <summary>
        /// 播放item动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowNext()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            if (FortyTwoGridModel.Instance.gameData.Count > 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (!ClearItemArr.ContainsKey(i))
                    {
                        ClearItemArr[i] = new List<GameObject>();
                    }
                    ClearItemArr[i].Clear();
                }
                var dic = new Dictionary<int, List<FortyTwoGridTabItemInfo>>();
                var listObj = new List<GameObject>();
                var count = 0;
                var isshowend = false;
                var isshowleishen = false;
                for (int i = 0; i < itemList.Count; i++)
                {
                    var data = FortyTwoGridModel.Instance.gameData[0].list[itemList[i].id];
                    if (!data.isShow)
                        continue;
                    var icon = itemList[i].icon;
                    itemList[i].isSHow = FortyTwoGridModel.Instance.gameData[0].list[i].isShow;
                   // icon.gameObject.SetActive(false);
                    var objTRS = icon.gameObject.GetComponent<RectTransform>();
                    listObj.Add(icon.gameObject);
                    if (FortyTwoGridModel.Instance.gameData[0].list[i].id == 10)
                    {
                        isshowleishen = true;
                    }
                    else
                    {
                        isshowend = true;
                    }
                    var item1 = getItemAni(2, icon.gameObject.transform.parent);
                    item1.SetActive(true);
                    item1.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    item1.transform.localPosition = objTRS.anchoredPosition3D;
                    item1.transform.SetAsFirstSibling();

                    ClearItemArr[2].Add(item1);
                    count++;
                  
                 
                    if (!dic.ContainsKey(data.id))
                    {
                        dic[data.id] = new List<FortyTwoGridTabItemInfo>();
                    }
                    dic[data.id].Add(itemList[i]);
                }
                //if (isshowleishen)
                //{
                //    CoreEntry.gAudioMgr.PlayUISound(5);
                //}

                if (isshowend)
                {
                    CoreEntry.gAudioMgr.PlayUISound(300);
                }
                for (int i = 0; i < itemList.Count; i++)
                {
                    var data = FortyTwoGridModel.Instance.gameData[0].list[itemList[i].id];
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(data.id);
                    if (config.Type == 2 && itemList[i].isReplace)
                    {
                        var icon = itemList[i].icon;
                        var rect = icon.transform.GetComponent<RectTransform>();

       /*                 var item1 = getItemAni(1, rect.parent, icon.name);
                        item1.SetActive(true);
                        item1.transform.SetAsLastSibling();
                        item1.transform.localPosition = rect.anchoredPosition3D;
                        CommonTools.SetArmatureName(item1.transform, icon.name);*/
/*
                        var item = getItemAni(4, rect.parent);
                     
                        item.transform.localScale = new Vector3(100, 100, 100);
                        item.SetActive(true);
                        item.transform.SetAsLastSibling();
                        item.transform.localPosition = rect.anchoredPosition3D;*/
/*
                        var cheng = getItemAni(6, item1.transform);
                        var text = cheng.GetComponent<Text>();
                        text.text = config.Beishu + "x";
                        cheng.name = "Rect_cheng";
                        cheng.SetActive(true);
                        cheng.transform.localScale = new Vector3(0.65f / item1.transform.localScale.x, 0.65f / item1.transform.localScale.y, 1f);
                        cheng.transform.localPosition = Vector3.zero;
                        playShanDian(item, item1, icon.gameObject);*/
                    }
                }

                var dic1 = new List<int>();
                var dic2 = new List<FortyTwoGridTabTipsInfo>();
                foreach (var item in dic)
                {
                    var posList = FindPos(item.Value);
                    if (posList.Count > 0)
                    {
                        for (int i = 0; i < posList.Count; i++)
                        {
                            if (!dic1.Contains(posList[i].line))
                            {
                                dic1.Add(posList[i].line);
                                FortyTwoGridTabTipsInfo tips = new FortyTwoGridTabTipsInfo();
                                tips.count = item.Value.Count;
                                tips.icon = posList[i].icon;
                                tips.id = posList[i].id;
                                dic2.Add(tips);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < dic2.Count; i++)
                {

                    var id = FortyTwoGridModel.Instance.gameData[0].list[dic2[i].id].id;
                    var data = FortyTwoGridModel.Instance.FindConfigByInterval(id, dic2[i].count);
                    long temp = (long)m_chipArr[m_selecChipIndex] / 100;
                    var num = (temp * (long)data.multiple  * (long)data.times ).ToString("F0", new CultureInfo("en"));
                    if (id == 11)
                    {
                        num = ToolUtil.ShowF2Num(FortyTwoGridModel.Instance.gameData[0].n64RSPowerGold);
                    }
                    var item1 = getItemAni(5, m_Mask2D_Panel.transform);//dic2[i].icon.transform.parent);
                    item1.SetActive(true);
                    item1.transform.SetAsLastSibling();
                    var text = item1.transform.GetChild(0).GetComponent<Text>();
                    var ani = text.transform.GetComponent<Animation>();
                    var rect = dic2[i].icon.transform.GetComponent<Transform>();
                    item1.transform.position = dic2[i].icon.transform.position;
                    item1.transform.localScale = Vector3.one;
                    text.text = "+" + num;
                    ani.Play();
                }
                yield return new WaitForSecondsRealtime(0.5f);
               
                foreach (var itemInfo in ClearItemArr)
                {
                    if (itemInfo.Key == 1 || itemInfo.Key == 2)
                    {
                        for (int k = 0; k < itemInfo.Value.Count; k++)
                        {
                            recoderAni(itemInfo.Key, itemInfo.Value[k]);
                        }
                        ClearItemArr[itemInfo.Key].Clear();
                        CoreEntry.gAudioMgr.PlayUISound(297);
                    }

                }

                for (int i = 0; i < listObj.Count; i++)
                {
                    listObj[i].SetActive(false);
                    var item2 = getItemAni(3, listObj[i].transform.parent);
                    var fram = item2.GetComponent<SpriteAnimation>();
                    var objTRS1 = listObj[i].transform;
                    item2.SetActive(true);
                    item2.transform.localPosition = objTRS1.localPosition;
                    ClearItemArr[3].Add(item2);
                    fram.SetSprite(0);
                    fram.Play(()=> {
                        item2.SetActive(false);
                    });
         
                }
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(startMove());
            }
        }

        public void playFirst()
        {

        }
        /// <summary>
        /// ani动画结束回调
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="str"></param>
        public void FlyAniEnd(GameObject obj, string str)
        {
            if (obj.name == "text")
            {
                recoderAni(5, obj.transform.parent.gameObject);
            }
        }

        /// <summary>
        /// 寻找展示Tips坐标
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<FortyTwoGridTabItemInfo> FindPos(List<FortyTwoGridTabItemInfo> list)
        {
            var pos = new int[30] { 15, 16, 9, 10, 21, 22, 3, 4, 27, 28, 14, 17, 8, 11, 20, 23, 2, 5, 26, 29, 13, 18, 7, 12, 19, 24, 1, 6, 25, 30 };
            var list1 = new List<FortyTwoGridTabItemInfo>();
            for (int i = 0; i < pos.Length; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].id == pos[i])
                    {
                        list1.Add(list[j]);
                        break;
                    }
                }
                if (list1.Count >= 4)
                {
                    break;
                }
            }
            return list1;
        }

        /// <summary>
        /// 播放替换item动画
        /// </summary>
        public IEnumerator startMove()
        {
            // ToolUtil.RollText(FortyTwoGridModel.Instance.gameData[0].n64TotalGold, FortyTwoGridModel.Instance.gameData[0].n64TotalGold * FortyTwoGridModel.Instance.gameData[0].nTotalDouble, m_Txt_Reward);
            m_Txt_Reward.text = FortyTwoGridModel.Instance.gameData[0].n64TotalGold + "";
            if (FortyTwoGridModel.Instance.gameData[0].n64RSPowerGold > 0)
            {
                var isShow = true;
                m_Gold_Effect.setData(4, FortyTwoGridModel.Instance.gameData[0].n64RSPowerGold, () => { isShow = false; }, isZidong, null, null, 100);

                yield return new WaitWhile(() => isShow);
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.3f);
            }


            foreach (var itemInfo in ClearItemArr)
            {
                for (int i = 0; i < itemInfo.Value.Count; i++)
                {
                    recoderAni(itemInfo.Key, itemInfo.Value[i]);
                }
                itemInfo.Value.Clear();
            }
            var list1 = new Dictionary<int, List<FortyTwoGridTabItemInfo>>();
            var list2 = new Dictionary<int, List<FortyTwoGridTabItemInfo>>();
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i].isSHow)
                {
                    if (!list1.ContainsKey(itemList[i].type))
                    {
                        list1[itemList[i].type] = new List<FortyTwoGridTabItemInfo>();
                    }
                    list1[itemList[i].type].Add(itemList[i]);
                }

                if (!list2.ContainsKey(itemList[i].type))
                {
                    list2[itemList[i].type] = new List<FortyTwoGridTabItemInfo>();
                }
                list2[itemList[i].type].Add(itemList[i]);
            }



            for (int i = 0; i < list2.Count; i++)
            {
                list2[i].Sort((a, b) =>
                {
                    var x = a.line;
                    var y = a.isSHow ? 0 : 100;

                    var x1 = b.line;
                    var y1 = b.isSHow ? 0 : 100;
                    return (x + y) - (x1 + y1);
                });
            }

            FortyTwoGridModel.Instance.gameData.RemoveAt(0);

            itemList.Clear();
            var count = 0;
            var textList = new List<FortyTwoGridTabItemInfo>();
            var isPlaySd = false;
            foreach (var item in list2)
            {

                for (int i = 0; i < item.Value.Count; i++)
                {

                    var data = list2[item.Key][i];
                    var line = data.line;
                    list2[item.Key][i].line = i;
                    list2[item.Key][i].id = i * 6 + item.Key;

                    var data1 = FortyTwoGridModel.Instance.gameData[0].list[list2[item.Key][i].id];
                    var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(data1.id);
                    list2[item.Key][i].icon.gameObject.SetActive(true);
                    list2[item.Key][i].isReplace = false;
                    if (list2[item.Key][i].icon.name != config.Pic)
                    {
                        list2[item.Key][i].isReplace = true;
                        var chengObj = list2[item.Key][i].icon.transform.Find("Rect_cheng");
                        if (chengObj)
                        {
                            recoderAni(6, chengObj.gameObject);
                        }
                        list2[item.Key][i].icon.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + config.Pic);
                        list2[item.Key][i].icon.name = config.Pic;
                        if (config.Type == 2)
                        {

                            var cheng = getItemAni(6, list2[item.Key][i].icon.transform);
                            var text = cheng.GetComponent<Image>();
                            text.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + "x" + config.Beishu);
                            cheng.SetActive(true);
                            cheng.name = "Rect_cheng";
                            text.SetNativeSize();
                            cheng.transform.localScale = new Vector3(1f, 1f, 1f);
                            cheng.transform.localPosition = Vector3.zero;
                            textList.Add(list2[item.Key][i]);
                            isPlaySd = true;

                        }
                    }

                    var rect = list2[item.Key][i].icon.transform.GetComponent<RectTransform>();
                    var pos1 = rect.anchoredPosition3D;
                    var pos2 = new Vector3(pos1.x, itemHeight - line * lineHeight, pos1.z);
                    if (list2[item.Key][i].isSHow)
                    {
                        pos2 = new Vector3(pos1.x, itemHeight - (i - 5) * lineHeight, pos1.z);
                    }
                    var pos3 = new Vector3(pos1.x, itemHeight - i * lineHeight - 20, pos1.z);
                    var pos4 = new Vector3(pos1.x, itemHeight - i * lineHeight, pos1.z);
                    if (data.isSHow)
                    {
                        count++;
                        var seq1 = DOTween.Sequence();
                        rect.anchoredPosition3D = pos2;
                        seq1.AppendInterval(0.1f).
                            Append(rect.DOAnchorPos3D(pos3, 0.2f)).
                            Append(rect.DOAnchorPos3D(pos4, 0.2f)).
                            AppendCallback(() =>
                            {
                                count--;
                                if (count > 0)
                                {
                                    return;
                                }
                                if (FortyTwoGridModel.Instance.gameData.Count > 1)
                                {
                                    StartCoroutine(ShowNext());
                                }
                                else
                                {
                                    StartCoroutine(ShowWinAni());


                                }
                            });
                    }
                    else
                    {
                        if (line != i)
                        {
                            var seq = DOTween.Sequence();
                            rect.anchoredPosition3D = pos1;
                            seq.Append(rect.DOAnchorPos3D(pos3, 0.2f)).
                                Append(rect.DOAnchorPos3D(pos4, 0.2f));
                        }

                    }
                    itemList[list2[item.Key][i].id] = list2[item.Key][i];
                    itemList[list2[item.Key][i].id].isSHow = false;
                }
            }
        }

        IEnumerator ShowWinAni()
        {
            var isShow = false;
            var count = 0;
            var data1 = FortyTwoGridModel.Instance.gameData[0];
            for (int i = 0; i < itemList.Count; i++)
            {
                var data = FortyTwoGridModel.Instance.gameData[0].list[itemList[i].id];
                var config = ConfigCtrl.Instance.Tables.TbFortyTwoGrid_Config.Get(data.id);
                if (config.Type == 2)
                {
                    isShow = true;
                    m_Rect_Ani.gameObject.SetActive(true);
                    itemList[i].icon.gameObject.SetActive(false);
                    var chengObj = itemList[i].icon.transform.Find("Rect_cheng");
                    if (chengObj)
                    {
                        recoderAni(6, chengObj.gameObject);
                    }
                    var rect = itemList[i].icon.transform.GetComponent<RectTransform>();
                    var item = Instantiate(itemList[i].icon.gameObject);
                    item.transform.SetParent(m_Rect_Ani.transform);
                    item.name = itemList[i].icon.name;
                    item.SetActive(true);
                    item.transform.SetAsLastSibling();
                    item.transform.position = rect.position;
                    item.transform.localScale = new Vector3(1, 1, 1);


                    var cheng = getItemAni(6, item.transform);
                    var text = cheng.GetComponent<Image>();
                    text.sprite = AtlasSpriteManager.Instance.GetSprite("FortyTwoGrid_icon:" + "x" + config.Beishu);
                    cheng.SetActive(true);
                    cheng.name = "Rect_cheng";
                    text.SetNativeSize();
                    cheng.transform.localScale = new Vector3(1f / item.transform.localScale.x, 1f / item.transform.localScale.y, 1f);
                    cheng.transform.localPosition = Vector3.zero;

                    itemList[i].icon.gameObject.SetActive(true);
                    
                    // ArmatureAni(item.gameObject, itemList[i].icon.gameObject, cheng);
                    count++;

                    var seq = DOTween.Sequence();
                    seq.Append(text.transform.DOScale(new Vector3(1f / item.transform.localScale.x, 1f / item.transform.localScale.y, 1f), 0.5f)).
                        Append(text.transform.DOMove(m_Txt_Reward.transform.position, 1f)).
                        AppendCallback(() =>
                        {
                            recoderAni(6, cheng);
                            Destroy(item);
                           
                            m_Rect_Ani.gameObject.SetActive(false);
                            text.gameObject.SetActive(false);
                            count--;
                            if (count <= 0)
                            {
                                ToolUtil.RollText(data1.n64TotalGold, data1.n64TotalGold * data1.nTotalDouble, m_Txt_Reward);
                            }
                        });
                }
            }
            if (isShow)
            {
                CoreEntry.gAudioMgr.PlayUISound(296);
                yield return new WaitForSecondsRealtime(2f);
            }
            var nTotalDouble = data1.nTotalDouble == 0 ? 1 : data1.nTotalDouble;
            var beishu = data1.n64TotalGold * nTotalDouble / FortyTwoGridModel.Instance.nAllBet;
            if (beishu >= 12)
            {
                m_Gold_Effect.setData(3, data1.n64TotalGold * nTotalDouble, endListAni, isZidong,null,null,100);

            }
            else if (beishu >= 4)
            {
                m_Gold_Effect.setData(2, data1.n64TotalGold * nTotalDouble, endListAni, isZidong, null, null, 100);
            }
            else if (beishu >= 2)
            {
                m_Gold_Effect.setData(1, data1.n64TotalGold * nTotalDouble, endListAni, isZidong, null, null, 100);
            }
            else
            {
                endListAni();
            }

            yield return null;
        }
        
        private void ArmatureAni(GameObject obj, GameObject obj1, GameObject obj2)
        {
            obj1.SetActive(true);
            CommonTools.PlayArmatureAni(obj.transform, "dz2", 1, () => {

                recoderAni(6, obj2);
                recoderAni(1, obj);
            });
        }
        private void endListAni()
        {
            m_Txt_Gold.text = ToolUtil.AbbreviateNumber(FortyTwoGridModel.Instance.Gold);
            FortyTwoGridModel.Instance.gameData.Clear();
            StartCoroutine(WaitEndAni());

        }
        private IEnumerator WaitEndAni()
        {
            if (isZidong)
            {
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.7f);
            }

            setBtnState(true);
            SetRollBtnRorate(false);
            m_gameState = 0;
            if (MainUIModel.Instance.palyerData.m_i8Diamonds >= 30)
            {
                if (GuideModel.Instance.bReachCondition(9) && GuideModel.Instance.bReachCondition(6))
                {
                    if (MainPanelMgr.Instance.IsShow("TaskPanel"))
                    {
                        MainPanelMgr.Instance.Close("TaskPanel");
                    }
                    GuideModel.Instance.SetFinish(9);
                    OnXxAutoBtn();
                    MainUICtrl.Instance.OpenGuidePanel(m_Btn_Back.transform, OnClickBackBtn);
                    yield break;
                }               
            }

            if (isZidong)
            {
                OnReloadBtn();
            }


            yield break;
        }

        #region 对象池
        /// <summary>
        /// 克隆对象池
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject cloneAni(int type, string name = "")
        {
            GameObject obj = null;
            switch (type)
            {
                case 1:
                   /* obj = Instantiate(m_Dragon_Icon.gameObject);
                    obj.name = name;*/
                    break;
                case 2:
                    obj = Instantiate(m_Img_Bg.gameObject);
                    obj.name = m_Img_Bg.name;
                    break;
                case 3:
                    obj = Instantiate(m_Img_shan.gameObject);
                    obj.name = m_Img_shan.name;
                    break;
  
                case 5:
                    obj = Instantiate(m_Rect_Tips.gameObject);
                    obj.name = m_Rect_Tips.name;
                    break;
                case 6:
                    obj = Instantiate(m_Rect_cheng.gameObject);
                    obj.name = m_Rect_cheng.name;
                    break;

                default:
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 回收对象池
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public void recoderAni(int type, GameObject obj)
        {
            CommonTools.removeArmatureCallback(obj.transform);
            obj.SetActive(false);
            obj.transform.SetParent(m_Rect_Pool.transform);

            ItemPool[type].Add(obj);

        }


        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject getItemAni(int type, Transform parent, string name = "")
        {
            GameObject item = null;

            if (!ItemPool.ContainsKey(type))
            {
                ItemPool[type] = new List<GameObject>();
            }
            if (ItemPool[type].Count <= 0)
            {
                item = cloneAni(type, name);
                item.transform.SetParent(parent, true);
                if (type == 1)
                {
                    item.transform.localScale = new Vector3(95, 95, 95);
                }
                else if ( type == 3)
                {
                    item.transform.localScale = new Vector3(1, 1, 1);
                }
                return item;
            }
            else
            {
                item = ItemPool[type][0];
                item.transform.SetParent(parent);
                ItemPool[type].RemoveAt(0);
                return item;
            }
        }
        #endregion
        /// <summary>
        /// 设置按钮状态
        /// </summary>
        /// <param name="isShow"></param>
        private void setBtnState(bool isShow)
        {

            m_Btn_Jia.interactable = isShow && m_selecChipIndex < m_chipArr.Length - 1;
            m_Btn_jian.interactable = isShow && m_selecChipIndex > 0;
            m_Btn_Reload.interactable = isShow && !isZidong;
            m_Btn_Back.interactable = isShow;

        }

        #endregion

        #region 事件响应

        private void OnClickJianBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            m_selecChipIndex--;
            m_Btn_Jia.interactable = true;
            if (m_selecChipIndex <= 0)
            {
                m_Btn_jian.interactable = false;
            }

            m_Txt_Couma.text = ToolUtil.ShowF2Num2(m_chipArr[m_selecChipIndex]) + "";
        }

        private void OnClickJiaBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);

            if ((m_selecChipIndex +1) >= 2)
            {
                if (MainUIModel.Instance.palyerData.m_i4Viplev < 1)
                {
                    ToolUtil.FloattingText("想要解锁需要充值哦", transform);
                    return;
                }
            }

            m_selecChipIndex++;
            m_Btn_jian.interactable = true;
            if (m_selecChipIndex >= m_chipArr.Length - 1)
            {
                m_Btn_Jia.interactable = false;
            }

            m_Txt_Couma.text = ToolUtil.ShowF2Num2(m_chipArr[m_selecChipIndex]) + "";


        }
        private void OnClickBackBtn()
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Back, 1f));
            CoreEntry.gAudioMgr.PlayUISound(1);
            MainUICtrl.Instance.SendLevelGameRoom();
        }
        
        private void OnReloadBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            SetRollBtnRorate(false);
            if (m_gameState != 0) return;

            if (nowGold >= (long)(m_chipArr[m_selecChipIndex]))
            {
                StartCoroutine(betSend());
            }
            else
            {
                var num = m_selecChipIndex;
                var isshow = false;
                while (num >= 0 && !isshow)
                {
                    num--;
                    if (num >= 0 && nowGold >= (long)(m_chipArr[num]))
                    {
                        m_selecChipIndex = num;
                        isshow = true;
                    }
                }

                if (isshow)
                {
                    m_Txt_Couma.text = ToolUtil.ShowF2Num2(m_chipArr[m_selecChipIndex]) + "";
                    StartCoroutine(betSend());
                    return;
                }

                if (isZidong)
                {
                    isZidong = false;
                    m_Btn_Auto.interactable = true;
                }

                if (isDanji)
                    return;
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
                //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool isFirst1);
                //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool isFirst2);
                var isBuyedFirst = isState;// && isFirst1 && isFirst2;
                if (isBuyedFirst)
                {
                    SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
                    tips.SetTipsPanel("温馨提示", "我感到抱歉！ 您的余额不足，请充值", "充值", delegate
                    {
                        MainUICtrl.Instance.OpenShopPanel();
                    });
                }
                else
                {
                    MainUIModel.Instance.bOpenAlmsPanel = true;
                    MainUICtrl.Instance.OpenFirstChargePanel();
                }

            }
            if (GuideModel.Instance.bReachCondition(11) && !GuideModel.Instance.bReachCondition(12))
                GuideModel.Instance.SetFinish(12);
        }

        public void SetRollBtnRorate(bool bFast = false, bool bPause = false)
        {
            m_Img_roll.transform.DOKill();
            if (!bPause)
            {

                if (bFast)
                {
                  
                    m_Img_roll.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

                }
                else
                {
            
                    m_Img_roll.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 5f, RotateMode.LocalAxisAdd).SetDelay(0.55f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

                }
            }

            //else
            //{
            //    m_Img_roll.transform.localEulerAngles = Vector3.zero;
            //}

            //m_Trans_Normal.gameObject.SetActive(!bPause);
            //m_Trans_Grey.gameObject.SetActive(bPause);
        }



        private void OnClickGoldBtn()
        {
            MainUICtrl.Instance.OpenShopPanel();
            CoreEntry.gAudioMgr.PlayUISound(1);
        }

        private void OnTaskBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTaskPanel();
        }

        private void OnRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenRankPanel();
        }

        private void OnTourBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTournamentPanel();
        }
        private void OnAutoBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            isZidong = true;
            OnReloadBtn();
            m_Btn_Auto.interactable = false;
            m_Btn_XxAuto.interactable = true;
            m_Btn_XxAuto.gameObject.SetActive(isZidong);
        }


        private void OnXxAutoBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            isZidong = false;
            m_Btn_Auto.interactable = true;
            m_Btn_XxAuto.interactable = false;
            m_Btn_XxAuto.gameObject.SetActive(isZidong);
        }
        private void OnMaxBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            if (MainUIModel.Instance.palyerData.m_i4Viplev < 1)
            {
                ToolUtil.FloattingText("想要解锁需要充值哦", transform);
                return;
            }
            m_selecChipIndex = m_chipArr.Length - 1;
            m_Txt_Couma.text = ToolUtil.ShowF2Num2(m_chipArr[m_selecChipIndex]) + "";
            m_Btn_Jia.interactable = false;
            m_Btn_jian.interactable = true;
        }

        
        private void OnClickRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("RoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < FortyTwoGridModel.Instance.awardList.Count; i++)
            {
                if (FortyTwoGridModel.Instance.awardList[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(FortyTwoGridModel.Instance.awardList[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        private void OnClickRuleBtn()
        {
            MainPanelMgr.Instance.ShowDialog("FortyTwoGridHelpPanel");
            CoreEntry.gAudioMgr.PlayUISound(1);
        }

        public void OnShop() 
        {
            MainUICtrl.Instance.OpenShopPanel();
            CoreEntry.gAudioMgr.PlayUISound(1);
        }
        IEnumerator betSend()
        {
            if (m_gameState != 0)
            {
                yield break;
            }

            if (!isDanji)
            {
                FortyTwoGridCtrl.Instance.GameBetSend(m_chipArr[m_selecChipIndex]);

                StartAni();
            }
            else
            {
                StartAni();
                FortyTwoGridModel.Instance.getServerData(danjiGold, m_chipArr[m_selecChipIndex]);
                danjiGold = FortyTwoGridModel.Instance.Gold;
                PlayerPrefs.SetInt("FortyTwoGridDanji", (int)danjiGold);
                reciverMoveUI();
            }

            /*          yield return new WaitForSecondsRealtime(2f);
                      if (m_gameState == 1 || )
                      {
                          setBtnState(true);
                      }*/
        }

        #endregion
    }
}
