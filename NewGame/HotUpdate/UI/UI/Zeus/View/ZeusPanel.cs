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
    public partial class ZeusPanel : PanelBase
    {
        private Dictionary<int, List<GameObject>> ItemPool = new Dictionary<int, List<GameObject>>();

        private int m_gameState = 0;
        private int[] m_chipArr = new int[5];
        private int m_selecChipIndex = 0;
        private Dictionary<int, ZeusTabItemInfo> itemList = new Dictionary<int, ZeusTabItemInfo>();
        private List<ZeusTabInfo> ZeusTabList = new List<ZeusTabInfo>();
        private Dictionary<int, List<GameObject>> ClearItemArr = new Dictionary<int, List<GameObject>>();
        private long nowGold = 0;
        private int roomType = 0;
        private GoldEffectNew m_Gold_Effect;
        public CommonTop commonTop;
        private bool isDanji = false;
        private long danjiGold = 0;
        private int rollTimes = 0;
        protected override void Awake()
        {
            base.Awake();
            
            GetBindComponents(gameObject);
            ZeusModel.Instance.InitZeusConfig();

            GameObject common = CommonTools.AddSubChild(m_Trans_CommonTop.gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonTop");
            commonTop = common.GetComponent<CommonTop>();
            commonTop.OnRank = OnClickRankBtn;
            commonTop.OnHelp = OnClickRuleBtn;
            //commonTop.OnBetChangeCallBack = OnBetChangeCallBack;
            //commonTop.OnClickEnd = SlotCpt.endSpin;
            commonTop.beginSpin = OnChangeZidong;
            //commonTop.OnPointerUp = onPointerUp;
            commonTop.OnTolgAuto = null;// OnChangeZidong;
            //commonTop.ClickCondition = null;
            commonTop.OneBtnCloseAutoSpinCallBack = OnCloseAutoSpin;

        }
        protected override void Start()
        {
            base.Start();


            var obj = CoreEntry.gResLoader.ClonePre("UI/UITemplate/Gold_EffectNew", m_Rect_Effect.transform,false,false);
            var rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = m_Rect_Effect.sizeDelta;
            rect.anchoredPosition3D = new Vector3(0, 0, 88);
            rect.localScale = Vector3.one;
            m_Gold_Effect = obj.GetComponent<GoldEffectNew>();


            commonTop.SetDanJi(isDanji, true);
            rollTimes = 0;
            commonTop.SetSlotSpinNum(rollTimes);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            CoreEntry.gAudioMgr.PlayUIMusic(12);
            m_gameState = 0;
            itemList.Clear();
            ZeusTabList.Clear();
            roomType = (int)param;
            
            commonTop.GetAutoToggle().isOn = true;
            commonTop.GetBeginBtn().interactable = true;// !commonTop.GetTolAuto();
            m_Txt_Reward.text = "0.00";
            commonTop.UpDateScore(0);
            if(roomType == 0)
            {
                isDanji = true;
                roomType = 1;
                ZeusModel.Instance.JackpotNum = 4560000;
            }
          
            if (isDanji)
            {
                if (PlayerPrefs.HasKey("zeusDanji"))
                {
                    danjiGold = PlayerPrefs.GetInt("zeusDanji");
                }
                else
                {
                    danjiGold = 5000000;
                    PlayerPrefs.SetInt("zeusDanji", (int)danjiGold);
                }
                
                commonTop.UpdateGold(danjiGold);
                nowGold = danjiGold;
            }
            else
            {
                commonTop.UpdateGold(MainUIModel.Instance.Golds);
                nowGold = MainUIModel.Instance.Golds;//MainUIModel.Instance.palyerData.m_i8Golds;
            }
    
            RegisterListener();
            switch (roomType)
            {
                case 1:
                    m_chipArr = new int[5] { 6000, 12000, 18000, 24000, 30000 };
                    break;
                case 2:
                    m_chipArr = new int[5] { 60000, 120000, 180000, 240000, 300000 };
                    break;
                case 3:
                    m_chipArr = new int[5] { 600000, 1200000, 1800000, 2400000, 3000000 };
                    break;
                default:
                    break;
            }
            InitGame();
            m_Roll_JackPot.SetNum(ZeusModel.Instance.JackpotNum);
            ReloadUI();
            StartCoroutine(playJackpot());
            UpdateJackpot();

            if (Screen.height / Screen.width >= 2)
                m_Dragon_Lan.transform.localPosition = new Vector3(0,434,0);
            else
                m_Dragon_Lan.transform.localPosition = new Vector3(-800, 434, 0);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            m_gameState = 0;
            CoreEntry.gAudioMgr.StopMusic(12);
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
            if (isDanji)
            {
                commonTop.UpdateGold(danjiGold);
            }
            else
            {
                commonTop.UpdateGold(MainUIModel.Instance.Golds);
            }
            
            ReloadUI();

            if (m_gameState == 1 || m_gameState == 2)
            {
                for (int i = 0; i < ZeusTabList.Count; i++)
                {
                    var pos1 = ZeusTabList[i].obj.anchoredPosition3D;
                    var pos4 = new Vector3(pos1.x,0f, pos1.z);
                    ZeusTabList[i].obj.localPosition = pos4;

                }
            }

            if (rollTimes > 0 || m_gameState == 2)
            {
                m_gameState = 0;
                rollTimes--;
                commonTop.SetSlotSpinNum(rollTimes);
                OnReloadBtn();
            }
            else
            {
                commonTop.SetRollBtnRorate(false);
                setBtnState(true);
            }
        }
        public void ReloadUI()
        {
            m_Txt_JackPot.text = ToolUtil.ShowF2Num(ZeusModel.Instance.JackpotNum);
            
            m_Roll_JackPot.RollNum(ZeusModel.Instance.JackpotNum,false,true);
        }
        private void UpdateGold(long gold)
        {
            commonTop.UpdateGold(MainUIModel.Instance.Golds);
            nowGold = MainUIModel.Instance.Golds;
        }
        private void UpdateJackpot()
        {
            if (isDanji)
                return;
            if (ZeusModel.Instance.awardList.Count > 0)
            {
                commonTop.SetTopRank(CommonTools.BytesToString(ZeusModel.Instance.awardList[0].szName), ZeusModel.Instance.awardList[0].n64Gold, ZeusModel.Instance.awardList[0].nIconID);
            }
        }

        public void InitGame()
        {
            m_selecChipIndex = 0;
            commonTop.GetBtn_Min().interactable = false;
            commonTop.GetBtnAdd().interactable = true;
            commonTop.UpDate_ChipsValue(m_chipArr[m_selecChipIndex]);
            ZeusModel.Instance.SetInitData();
            // 初始化列表
            var itemY = 0f;
            for (int i = 0; i < 6; i++)
            {
                var obj = ToolUtil.ClonePrefab(m_Rect_Item.gameObject, m_Mask2D_Panel.transform, "item");
                obj.gameObject.SetActive(true);
                obj.anchoredPosition3D = new Vector3(-314 + 125 * i, itemY, 0);
                for (int j = 0; j < 5; j++)
                {
                    var icon = obj.gameObject.transform.GetChild(j).gameObject.GetComponent("Image") as Image;
                    var data = ZeusModel.Instance.gameData[0].list[j * 6 + i];
                    var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data.id);
                    icon.sprite = AtlasSpriteManager.Instance.GetSprite("Zeus:" + config.Pic);
                    icon.name = config.Pic;
                    ZeusTabItemInfo info = new ZeusTabItemInfo();
                    info.id = j * 6 + i;
                    info.oldId = j * 6 + i;
                    info.type = i;
                    info.line = j;
                    info.icon = icon;
                    info.isReplace = true;
                    itemList[j * 6 + i] = info;
                }
                ZeusTabInfo info1 = new ZeusTabInfo();
                info1.id = i;
                info1.obj = obj;
                ZeusTabList.Add(info1);
            }
           
        }
        #region 事件绑定
        public void RegisterListener()
        {
            Message.AddListener(MessageName.GAME_ZEUS_START, reciverMoveUI);
            Message.AddListener<GameObject, string>(MessageName.ANIEnd, FlyAniEnd);
            Message.AddListener(MessageName.GAME_ZEUS_RELOADUI, ReloadUI);
            Message.AddListener(MessageName.GAME_ZEUS_UPDATEJACKPOT, UpdateJackpot);
            Message.AddListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.AddListener(MessageName.GE_COMMONTOP_MINSBTN, OnClickJianBtn);
            Message.AddListener(MessageName.GE_COMMONTOP_ADDBTN, OnClickJiaBtn);
            Message.AddListener(MessageName.GE_COMMONTOP_MAXBTN, OnClickMaxBtn);
            Message.AddListener(MessageName.GE_COMMONTOP_ROLLBTN, OnReloadBtn);
            commonTop.GetLeaveBtn().interactable = true;
        }
        public void UnRegisterListener()
        {
            Message.RemoveListener(MessageName.GAME_ZEUS_RELOADUI, ReloadUI);
            Message.RemoveListener(MessageName.GAME_ZEUS_START, reciverMoveUI);
            Message.RemoveListener<GameObject, string>(MessageName.ANIEnd, FlyAniEnd);
            Message.RemoveListener(MessageName.GAME_ZEUS_UPDATEJACKPOT, UpdateJackpot);
            Message.RemoveListener<long>(MessageName.UPDATE_GOLD, UpdateGold);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            Message.RemoveListener(MessageName.GE_COMMONTOP_MINSBTN, OnClickJianBtn);
            Message.RemoveListener(MessageName.GE_COMMONTOP_ADDBTN, OnClickJiaBtn);
            Message.RemoveListener(MessageName.GE_COMMONTOP_MAXBTN, OnClickMaxBtn);
            Message.RemoveListener(MessageName.GE_COMMONTOP_ROLLBTN, OnReloadBtn);
        }
        #endregion

        #region 播放
        private IEnumerator playJackpot()
        {
            yield return new WaitForEndOfFrame();
            CommonTools.PlayArmatureAni(m_Dragon_Jackpot.transform, "Sprite", 1, null, 0.7f);

            yield return new WaitForSecondsRealtime(20f);
            StartCoroutine(playJackpot());
        }
        /// <summary>
        /// 龙骨回调
        /// </summary>
        private void ArmatureAniBack()
        {
            CommonTools.SetArmatureName(m_Dragon_Lan.transform, "zs_lan");
            CommonTools.PlayArmatureAni(m_Dragon_Lan.transform, "start", 1, ArmatureAniBack);
            CommonTools.PlayArmatureAni(m_Dragon_Lan.transform, "idle", 0);
        }
        private void playSDArmatureAni()
        {
            CoreEntry.gAudioMgr.PlayUISound(11);
            CommonTools.removeArmatureCallback(m_Dragon_Lan.transform);
            CommonTools.SetArmatureName(m_Dragon_Lan.transform, "zs_huang");
            CommonTools.PlayArmatureAni(m_Dragon_Lan.transform, "start", 1, () =>
            {
                CommonTools.PlayArmatureAni(m_Dragon_Lan.transform, "idle", 1, ArmatureAniBack);
            });
        }
        /// <summary>
        /// 开始动画
        /// </summary>
        private void StartAni()
        {
            m_gameState = 1;
            setBtnState(false);
            StartCoroutine(playMoveUI());
        }
        /// <summary>
        /// 播放item进入动画
        /// </summary>
        /// <returns></returns>
        private IEnumerator playMoveUI()
        {
            CoreEntry.gAudioMgr.PlayUISound(9);
            m_Txt_Reward.text = "0.00";
            commonTop.UpDateScore(0);
            var num = nowGold - (long)(m_chipArr[m_selecChipIndex]);
            commonTop.UpdateGold(num);
            
            for (int i = 0; i < ZeusTabList.Count; i++)
            {
                var pos1 = ZeusTabList[i].obj.anchoredPosition3D;
                var pos2 = new Vector3(pos1.x, -600f, pos1.z);
                var seq = DOTween.Sequence();
                ZeusTabList[i].obj.MoveUIFromTo(pos1, pos2, 0.15f);
                yield return new WaitForSecondsRealtime(0.02f);
            }
            yield return new WaitForSecondsRealtime(0.25f);
            m_gameState = 2;
        }

        IEnumerator playReciverMoveUI()
        {

            yield return new WaitWhile(() => m_gameState != 2);
            
            m_gameState = 3;
            commonTop.UpdateGold(nowGold - ZeusModel.Instance.nAllBet);
            nowGold = ZeusModel.Instance.Gold;
            var list = new List<ZeusTabItemInfo>();
            if (ZeusModel.Instance.gameData.Count > 0)
            {
                var isPlaySd = false;
                for (int i = 0; i < itemList.Count; i++)
                {
                    var icon = itemList[i].icon;
                    var chengObj = icon.transform.Find("Rect_cheng");
                    if (chengObj)
                    {
                        recoderAni(6, chengObj.gameObject);
                    }
                    var data = ZeusModel.Instance.gameData[0].list[itemList[i].id];
                    itemList[i].isSHow = data.isShow;
                    itemList[i].isReplace = true;
                    var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data.id);
                    icon.name = config.Pic;
                    icon.sprite = AtlasSpriteManager.Instance.GetSprite("Zeus:" + config.Pic);
                    if (config.Type == 2)
                    {
                        var cheng = getItemAni(6, icon.transform);
                        var text = cheng.GetComponent<Text>();
                        text.text = config.Beishu + "x";
                        cheng.SetActive(true);
                        cheng.name = "Rect_cheng";
                        cheng.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                        cheng.transform.localPosition = Vector3.zero;
                        isPlaySd = true;
                        list.Add(itemList[i]);
                    }
                }
                if (isPlaySd)
                {
                    playSDArmatureAni();
                }
            }
            for (int i = 0; i < ZeusTabList.Count; i++)
            {
                var pos1 = ZeusTabList[i].obj.anchoredPosition3D;
                var pos2 = new Vector3(pos1.x, 600f, pos1.z);
                var pos3 = new Vector3(pos1.x, -10f, pos1.z);
                var pos4 = new Vector3(pos1.x, 0f, pos1.z);
                var seq = DOTween.Sequence();
                ZeusTabList[i].obj.anchoredPosition3D = pos2;
                if (i == ZeusTabList.Count - 1)
                {
                    seq.Append(ZeusTabList[i].obj.DOAnchorPos3D(pos3, 0.2f)).
                        Append(ZeusTabList[i].obj.DOAnchorPos3D(pos4, 0.1f)).
                        AppendCallback(() =>
                        {
                            CoreEntry.gAudioMgr.PlayUISound(7);
                        });
                }
                else
                {
                    seq.Append(ZeusTabList[i].obj.DOAnchorPos3D(pos3, 0.2f)).
                    Append(ZeusTabList[i].obj.DOAnchorPos3D(pos4, 0.1f));
                }
                yield return new WaitForSecondsRealtime(0.03f);
            }
            yield return new WaitForSecondsRealtime(0.1f);
            if (ZeusModel.Instance.gameData.Count > 1)
            {
                StartCoroutine(ShowNext());
            }
            else
            {
                //闪电
                if (list.Count > 0)
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var icon = list[i].icon;
                        var data = ZeusModel.Instance.gameData[0].list[list[i].id];
                        var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data.id);
                        var rect = icon.transform.GetComponent<RectTransform>();
                        var item1 = getItemAni(1, rect.parent, icon.name);
                        item1.SetActive(true);
                        item1.transform.SetAsLastSibling();
                        item1.transform.localPosition = rect.anchoredPosition3D;
                        CommonTools.SetArmatureName(item1.transform, icon.name);
                        var item = getItemAni(4, rect.parent);
                        icon.gameObject.SetActive(false);
                        item.transform.localScale = new Vector3(100, 100, 100);
                        item.SetActive(true);
                        item.transform.SetAsLastSibling();
                        item.transform.localPosition = rect.anchoredPosition3D;
                        var cheng = getItemAni(6, item1.transform);
                        var text = cheng.GetComponent<Text>();
                        text.text = config.Beishu + "x";
                        cheng.name = "Rect_cheng";
                        cheng.SetActive(true);
                        cheng.transform.localScale = new Vector3(0.65f / item1.transform.localScale.x, 0.65f / item1.transform.localScale.y, 1f);
                        cheng.transform.localPosition = Vector3.zero;

                        playShanDian(item, item1, icon.gameObject);
                    }
                    yield return new WaitForSecondsRealtime(2f);
                }
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
                CommonTools.PlayArmatureAni(item.transform, "zs_sd01_baodian",1,()=>
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
            if (ZeusModel.Instance.gameData.Count > 0)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (!ClearItemArr.ContainsKey(i))
                    {
                        ClearItemArr[i] = new List<GameObject>();
                    }
                    ClearItemArr[i].Clear();
                }
                var dic = new Dictionary<int, List<ZeusTabItemInfo>>();
                var count = 0;
                var isshowend = false;
                var isshowleishen = false;
                for (int i = 0; i < itemList.Count; i++)
                {
                    var data = ZeusModel.Instance.gameData[0].list[itemList[i].id];
                    if (!data.isShow)
                        continue;
                    var icon = itemList[i].icon;
                    itemList[i].isSHow = ZeusModel.Instance.gameData[0].list[i].isShow;
                    icon.gameObject.SetActive(false);
                    var objTRS = icon.gameObject.GetComponent<RectTransform>();
                    var item = getItemAni(1, icon.gameObject.transform.parent, icon.name);
                    item.SetActive(true);
                    item.transform.SetAsLastSibling();
                    item.transform.localPosition = objTRS.anchoredPosition3D;
                    CommonTools.SetArmatureName(item.transform, icon.name);
                    if (ZeusModel.Instance.gameData[0].list[i].id == 10)
                    {
                        isshowleishen = true;
                    }
                    else
                    {
                        isshowend = true;
                    }
                    var item1 = getItemAni(2, icon.gameObject.transform.parent);
                    item1.SetActive(true);
                    item1.transform.localPosition = objTRS.anchoredPosition3D;
                    item1.transform.SetAsFirstSibling();
                    ClearItemArr[1].Add(item);
                    ClearItemArr[2].Add(item1);
                    count++;
                    CommonTools.PlayArmatureAni(item.transform, "newAnimation", 1, () =>
                    {
                        count--;
                        if (count <= 0)
                        {
                            foreach (var itemInfo in ClearItemArr)
                            {
                                if (itemInfo.Key == 1 || itemInfo.Key == 2)
                                {
                                    for (int k = 0; k < itemInfo.Value.Count; k++)
                                    {
                                        if (itemInfo.Key == 1)
                                        {
                                            var item2 = getItemAni(3, itemInfo.Value[k].transform.parent);
                                            var objTRS1 = itemInfo.Value[k].gameObject.transform;
                                            item2.SetActive(true);
                                            item2.transform.localPosition = objTRS1.localPosition;
                                            ClearItemArr[3].Add(item2);
                                            if (k == 0)
                                            {
                                                CommonTools.PlayArmatureAni(item2.transform, "zs_tbxg2", 1, () =>
                                                {
                                                    StartCoroutine(startMove());
                                                }, 2.5f);
                                            }
                                            else
                                            {
                                                CommonTools.PlayArmatureAni(item2.transform, "zs_tbxg2", 1, null, 2.5f);
                                            }
                                        }
                                        recoderAni(itemInfo.Key, itemInfo.Value[k]);
                                    }
                                    ClearItemArr[itemInfo.Key].Clear();
                                    CoreEntry.gAudioMgr.PlayUISound(2);
                                }

                            }
                        }
                    });
                    CommonTools.PlayArmatureAni(item1.transform, "zs_tbxg1", 1);
                    if (!dic.ContainsKey(data.id))
                    {
                        dic[data.id] = new List<ZeusTabItemInfo>();
                    }
                    dic[data.id].Add(itemList[i]);
                }
                if (isshowleishen)
                {
                    CoreEntry.gAudioMgr.PlayUISound(5);
                }

                if (isshowend)
                {
                    CoreEntry.gAudioMgr.PlayUISound(3);
                }
                for (int i = 0; i < itemList.Count; i++)
                {
                    var data = ZeusModel.Instance.gameData[0].list[itemList[i].id];
                    var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data.id);
                    if (config.Type == 2 && itemList[i].isReplace)
                    {
                        var icon = itemList[i].icon;
                        var rect = icon.transform.GetComponent<RectTransform>();

                        var item1 = getItemAni(1, rect.parent, icon.name);
                        item1.SetActive(true);
                        item1.transform.SetAsLastSibling();
                        item1.transform.localPosition = rect.anchoredPosition3D;
                        CommonTools.SetArmatureName(item1.transform, icon.name);

                        var item = getItemAni(4, rect.parent);
                        icon.gameObject.SetActive(false);
                        item.transform.localScale = new Vector3(100, 100, 100);
                        item.SetActive(true);
                        item.transform.SetAsLastSibling();
                        item.transform.localPosition = rect.anchoredPosition3D;

                        var cheng = getItemAni(6, item1.transform);
                        var text = cheng.GetComponent<Text>();
                        text.text = config.Beishu + "x";
                        cheng.name = "Rect_cheng";
                        cheng.SetActive(true);
                        cheng.transform.localScale = new Vector3(0.65f / item1.transform.localScale.x, 0.65f / item1.transform.localScale.y, 1f);
                        cheng.transform.localPosition = Vector3.zero;
                        playShanDian(item, item1, icon.gameObject);
                    }
                }

                var dic1 = new List<int>();
                var dic2 = new List<ZeusTabTipsInfo>();
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
                                ZeusTabTipsInfo tips = new ZeusTabTipsInfo();
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
                    var id = ZeusModel.Instance.gameData[0].list[dic2[i].id].id;
                    var data = ZeusModel.Instance.FindConfigByInterval(id, dic2[i].count);
                    var num = (data.multiple * data.times * m_chipArr[m_selecChipIndex] / ToolUtil.GetGoldRadio()).ToString(MainUIModel.Instance.bNormalGame? "F2":"F0", new CultureInfo("en"));
                    if (id == 11)
                    {
                        num = ToolUtil.ShowF2Num(ZeusModel.Instance.gameData[0].n64RSPowerGold);
                    }
                    var item1 = getItemAni(5, m_Mask2D_Panel.transform);//dic2[i].icon.transform.parent);
                    item1.SetActive(true);
                    item1.transform.SetAsLastSibling();
                    var text = item1.transform.GetChild(0).GetComponent<Text>();
                    var ani = text.transform.GetComponent<Animation>();
                    // var rect = dic2[i].icon.transform.GetComponent<>();
                    item1.transform.position = dic2[i].icon.transform.position;
                    item1.transform.localScale = MainUIModel.Instance.bNormalGame ? Vector3.one : new Vector3(0.85f,1,0);
                    text.text = "R$:+" + num;
                    ani.Play();
                }
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
        public List<ZeusTabItemInfo> FindPos(List<ZeusTabItemInfo> list)
        {
            var pos = new int[30] { 15, 16, 9, 10, 21, 22, 3, 4, 27, 28, 14, 17, 8, 11, 20, 23, 2, 5, 26, 29, 13, 18, 7, 12, 19, 24, 1, 6, 25, 30 };
            var list1 = new List<ZeusTabItemInfo>();
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

            m_Txt_Reward.text = ToolUtil.ShowF2Num(ZeusModel.Instance.gameData[0].n64TotalGold);
            commonTop.UpDateScore(ZeusModel.Instance.gameData[0].n64TotalGold);
            if (ZeusModel.Instance.gameData[0].n64RSPowerGold > 0)
            {
                var isShow = true;
                m_Gold_Effect.setData(4, ZeusModel.Instance.gameData[0].n64RSPowerGold, () => {
                    isShow = false;
                    commonTop.UpDateScore(ZeusModel.Instance.gameData[0].n64TotalGold);
                }, rollTimes > 0);

                yield return new WaitWhile(() => isShow);
            }
            else
            {
                commonTop.UpDateScore(ZeusModel.Instance.gameData[0].n64TotalGold);
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
            var list1 = new Dictionary<int, List<ZeusTabItemInfo>>();
            var list2 = new Dictionary<int, List<ZeusTabItemInfo>>();
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i].isSHow)
                {
                    if (!list1.ContainsKey(itemList[i].type))
                    {
                        list1[itemList[i].type] = new List<ZeusTabItemInfo>();
                    }
                    list1[itemList[i].type].Add(itemList[i]);
                }

                if (!list2.ContainsKey(itemList[i].type))
                {
                    list2[itemList[i].type] = new List<ZeusTabItemInfo>();
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

            ZeusModel.Instance.gameData.RemoveAt(0);

            itemList.Clear();
            var count = 0;
            var textList = new List<ZeusTabItemInfo>();
            var isPlaySd = false;
            foreach (var item in list2)
            {

                for (int i = 0; i < item.Value.Count; i++)
                {

                    var data = list2[item.Key][i];
                    var line = data.line;
                    list2[item.Key][i].line = i;
                    list2[item.Key][i].id = i * 6 + item.Key;

                    var data1 = ZeusModel.Instance.gameData[0].list[list2[item.Key][i].id];
                    var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data1.id);
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
                        list2[item.Key][i].icon.sprite = AtlasSpriteManager.Instance.GetSprite("Zeus:" + config.Pic);
                        list2[item.Key][i].icon.name = config.Pic;
                        if (config.Type == 2)
                        {

                            var cheng = getItemAni(6, list2[item.Key][i].icon.transform);
                            var text = cheng.GetComponent<Text>();
                            text.text = config.Beishu + "x";
                            cheng.SetActive(true);
                            cheng.name = "Rect_cheng";
                            cheng.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                            cheng.transform.localPosition = Vector3.zero;
                            textList.Add(list2[item.Key][i]);
                            isPlaySd = true;

                        }
                    }

                    var rect = list2[item.Key][i].icon.transform.GetComponent<RectTransform>();
                    var pos1 = rect.anchoredPosition3D;
                    float temp = 111;//元素之间间隔
                    var pos2 = new Vector3(pos1.x, temp*2 - line * temp, pos1.z);
                    if (list2[item.Key][i].isSHow)
                    {
                        pos2 = new Vector3(pos1.x, temp*2 - (i - 5) * temp, pos1.z);
                    }
                    var pos3 = new Vector3(pos1.x, temp*2 - i * temp - 15, pos1.z);
                    var pos4 = new Vector3(pos1.x, temp * 2 - i * temp, pos1.z);
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
                                if (ZeusModel.Instance.gameData.Count > 1)
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

            if (isPlaySd)
            {
                playSDArmatureAni();
            }
        }

        IEnumerator ShowWinAni()
        {
            var isShow = false;
            var count = 0;
            var data1 = ZeusModel.Instance.gameData[0];
            for (int i = 0; i < itemList.Count; i++)
            {
                var data = ZeusModel.Instance.gameData[0].list[itemList[i].id];
                var config = ConfigCtrl.Instance.Tables.TbZeus_Config.Get(data.id);
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
                    var item = getItemAni(1, m_Rect_Ani.transform, itemList[i].icon.name);

                    item.SetActive(true);
                    item.transform.SetAsLastSibling();
                    item.transform.position = rect.position;


                    CommonTools.SetArmatureName(item.transform, itemList[i].icon.name);
                    var cheng = getItemAni(6, item.transform);
                    var text = cheng.GetComponent<Text>();
                    text.text = config.Beishu + "x";
                    cheng.name = "Rect_cheng";
                    cheng.SetActive(true);
                    cheng.transform.localScale = new Vector3(0.65f / item.transform.localScale.x, 0.65f / item.transform.localScale.y, 1f);
                    cheng.transform.localPosition = Vector3.zero;

                    ArmatureAni(item.gameObject, itemList[i].icon.gameObject, cheng);
                    count++;

                    var seq = DOTween.Sequence();
                    seq.Append(text.transform.DOScale(new Vector3(1f / item.transform.localScale.x, 1f / item.transform.localScale.y, 1f), 0.5f)).
                        Append(text.transform.DOMove(m_Rect_Img.position, 1f)).
                        AppendCallback(() =>
                        {
                            m_Rect_Ani.gameObject.SetActive(false);
                            text.gameObject.SetActive(false);
                            count--;
                            if (count <= 0)
                            {
                                ToolUtil.RollText(data1.n64TotalGold, data1.n64TotalGold * data1.nTotalDouble, commonTop.GetScoreText());
                            }
                        });
                }
            }
            if (isShow)
            {
                CoreEntry.gAudioMgr.PlayUISound(8);
                yield return new WaitForSecondsRealtime(2f);
            }
            var nTotalDouble = data1.nTotalDouble == 0 ? 1 : data1.nTotalDouble;
            var beishu = data1.n64TotalGold * nTotalDouble / ZeusModel.Instance.nAllBet;
            if (beishu >= 12)
            {
                m_Gold_Effect.setData(3, data1.n64TotalGold * nTotalDouble, endListAni,rollTimes > 0);

            }
            else if (beishu >= 4)
            {
                m_Gold_Effect.setData(2, data1.n64TotalGold * nTotalDouble, endListAni,rollTimes > 0);
            }
            else if (beishu >= 2)
            {
                m_Gold_Effect.setData(1, data1.n64TotalGold * nTotalDouble, endListAni, rollTimes > 0);
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
            commonTop.UpdateGold(ZeusModel.Instance.Gold);
            ZeusModel.Instance.gameData.Clear();
            StartCoroutine(WaitEndAni());

        }
        private IEnumerator WaitEndAni()
        {
            if (rollTimes > 0)
            {
                yield return new WaitForSecondsRealtime(1f);
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.7f);
            }

            setBtnState(true);
            commonTop.SetRollBtnRorate(false);
            m_gameState = 0;
            if (rollTimes >0)
            {
                rollTimes--;
                commonTop.SetSlotSpinNum(rollTimes);
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
                    obj = Instantiate(m_Dragon_Icon.gameObject);
                    obj.name = name;
                    break;
                case 2:
                    obj = Instantiate(m_Dragon_tbxg1.gameObject);
                    obj.name = m_Dragon_tbxg1.name;
                    break;
                case 3:
                    obj = Instantiate(m_Dragon_tbxg2.gameObject);
                    obj.name = m_Dragon_tbxg2.name;
                    break;
                case 4:
                    obj = Instantiate(m_Dragon_Sd.gameObject);
                    obj.name = m_Dragon_Sd.name;
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
                    item.transform.localScale = new Vector3(100, 100, 1);
                }
                else if (type == 2 || type == 3)
                {
                    item.transform.localScale = new Vector3(82, 100, 1);
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
            commonTop.GetBtnAdd().interactable = isShow && m_selecChipIndex < m_chipArr.Length - 1;
            commonTop.GetBtn_Min().interactable = isShow && m_selecChipIndex > 0;
            commonTop.GetBeginBtn().interactable = isShow;// && !commonTop.GetTolAuto();
            commonTop.GetAutoToggle().interactable = isShow;
            commonTop.GetAutoToggle().transform.GetChild(2).gameObject.SetActive(!isShow);
            commonTop.GetLeaveBtn().interactable = isShow;
            commonTop.GetBtn_Max().interactable = isShow;
        }

        #endregion

        #region 事件响应
        private void OnChangeZidong(int num ,bool isOn)
        {
            rollTimes = num;
            CoreEntry.gAudioMgr.PlayUISound(1);
            //commonTop.GetBeginBtn().interactable = !commonTop.GetTolAuto();           
            if (rollTimes >0)
            {
                rollTimes--;
                commonTop.SetSlotSpinNum(rollTimes);
                OnReloadBtn();
            }
        }
        private void OnClickJianBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            m_selecChipIndex--;
            commonTop.GetBtnAdd().interactable = true;
            if (m_selecChipIndex <= 0)
            {
                commonTop.GetBtn_Min().interactable = false;
            }
            commonTop.UpDate_ChipsValue(m_chipArr[m_selecChipIndex]);
        }

        private void OnClickJiaBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            m_selecChipIndex++;
            commonTop.GetBtn_Min().interactable = true;
            if (m_selecChipIndex >= m_chipArr.Length - 1)
            {
                commonTop.GetBtnAdd().interactable = false;
            }
            commonTop.UpDate_ChipsValue(m_chipArr[m_selecChipIndex]);
        }

        private void OnClickMaxBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            m_selecChipIndex = m_chipArr.Length - 1;
            commonTop.GetBtn_Min().interactable = true;
            if (m_selecChipIndex >= m_chipArr.Length - 1)
            {
                commonTop.GetBtnAdd().interactable = false;
            }
            commonTop.UpDate_ChipsValue(m_chipArr[m_selecChipIndex]);
        }

        private void OnCloseAutoSpin()
        {
            rollTimes = 0;
            commonTop.SetSlotSpinNum(rollTimes);
        }

        private void OnReloadBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            if (m_gameState != 0) return;
            commonTop.SetRollBtnRorate(true);
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
                    commonTop.UpDate_ChipsValue(m_chipArr[m_selecChipIndex]);
                    StartCoroutine(betSend());
                    return;
                }

                if (commonTop.GetTolAuto())
                {
                    commonTop.GetAutoToggle().isOn = false;
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
                    MainUICtrl.Instance.OpenFirstChargePanel();
            }            
        }

        private void OnClickRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
            UIRoomRank rankPanel = MainPanelMgr.Instance.ShowDialog("UIRoomRank") as UIRoomRank;

            List<RankItem> rankList = new List<RankItem>();
            for (int i = 0; i < ZeusModel.Instance.awardList.Count; i++)
            {
                if (ZeusModel.Instance.awardList[i].n64Gold <= 0)
                    continue;
                RankItem item = UIRoomRank.ConvertToRankItem(ZeusModel.Instance.awardList[i]);
                rankList.Add(item);
            }
            rankPanel.InitData(rankList);
        }

        private void OnClickRuleBtn()
        {
            MainPanelMgr.Instance.ShowDialog("ZeuHelpPanel");
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
                ZeusCtrl.Instance.GameBetSend(m_chipArr[m_selecChipIndex]);

                StartAni();
            }
            else
            {
                StartAni();
                ZeusModel.Instance.getServerData(danjiGold,m_chipArr[m_selecChipIndex]);
                danjiGold = ZeusModel.Instance.Gold;
                PlayerPrefs.SetInt("zeusDanji", (int)danjiGold);
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
