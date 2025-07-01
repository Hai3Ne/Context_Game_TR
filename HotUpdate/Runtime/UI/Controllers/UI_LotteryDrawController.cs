using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_LotteryDrawController : IUIControllerImp {
    public LotteryEff lotteryEff;
    public Transform XTZP;
    public GameObject btnClose;
    public GameObject btnDraw1;
    public GameObject btnDraw5;
    public UILabel txtJiang;
    public UILabel draw1CostLabel;
    public UILabel draw5CostLabel;
    public UILabel draw1TimesLabel;
    public UILabel draw5TimesLabel;
    public UILabel countdown;

    public List<Item_LotteryDraw> itemList = new List<Item_LotteryDraw>();
    public List<Item_LotteryDraw> selectedItems = new List<Item_LotteryDraw>();
    public int circleNum;
    public int comboCircleNum;
    public int rotDuration;
    public uint comboDrawTimes;
    public List<Item_LotteryDraw> tmpSelects = new List<Item_LotteryDraw>();
    public List<float> rotarray = new List<float>();

    public uint LotteryTickItemID;
    public long timerId = -1;
    public uint[] awardIDlist;
    public uint LotteryTick;
    public bool isDrawing;
    public uint LotteryCountDown;//奖品倒计时
    public float LotteryTickTimedown;
    public class ItemData {
        public uint itemCfgId;
        public uint count;
        public uint awardId;
    }

    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }

    public void InitData() {
        this.selectedItems.Clear();
	    this.circleNum = 8;
	    this.comboCircleNum = 10;
	    this.rotDuration = 6;
        
        LotteryVo tRoom = FishConfig.Instance.mLotteryConf.TryGet(RoleInfoModel.Instance.RoomCfgID);
        this.LotteryTickItemID = tRoom.LotteryTicketID;
	    int comboCost = Mathf.CeilToInt(tRoom.ComboNum * tRoom.ComboDiscount);

	    this.draw1CostLabel.text = tRoom.TicketNum.ToString();
	    this.draw5CostLabel.text = comboCost.ToString();

	    this.draw5TimesLabel.text = tRoom.ComboNum + "次";
        this.comboDrawTimes = tRoom.ComboNum;
        MtaManager.AddLotteruEnter();

        this.UpdateJiangNumber(null);
        this.ReqLotteryAward();
    }
    public void ShowAwardDialog() {
        List<ItemData> dataList = this.ConvertAwardItems(this.awardIDlist);

        UI_GetAwardController.ParamInfo pi = new UI_GetAwardController.ParamInfo();
        pi.tipInfos = StringTable.GetString("Tip_18");

        pi.db_item_list = new List<KeyValuePair<ItemsVo, uint>>();
        for (int i = 0; i < dataList.Count; i++) {
            pi.db_item_list.Add(new KeyValuePair<ItemsVo, uint>(FishConfig.Instance.Itemconf.TryGet(dataList[i].itemCfgId), (uint)dataList[i].count));
        }
        pi.onCloseCb = () => {
            this.isDrawing = false;
            this.SetDrawBtnStatus(true, false);
        };

        WndManager.Instance.ShowUI(EnumUI.UI_GetAward, pi, true);
        GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.GetCheer, false, false, 1);
    }

    public List<ItemData> ConvertAwardItems(uint[] awardIDlist) {
        List<ItemData> datalist = new List<ItemData>();
        if (awardIDlist != null) {
            for (int i = 0; i < awardIDlist.Length; i++) {
                AwardVo vo = FishConfig.Instance.AwardConf.TryGet(awardIDlist[i]);
                if (vo != null) {
                    datalist.Add(new ItemData {
                        itemCfgId = vo.ItemID[0],
                        count = vo.ItemCount[0],
                        awardId = vo.AwardID,
                    });
                }
            }
        }
        return datalist;
    }
    public void UpdateJiangNumber(object obj) {
        int cnt = RoleItemModel.Instance.getItemCount(Item_Battle_Lottery.item_lottery.LotteryTickItemID);
        this.UpdateJiang(cnt);
    }
    public void InitUIComponets(){
	    Transform cont = uiRefGo.transform.GetChild(1);
        Transform itms = cont.Find("Fenge");
        //Transform choose = cont.Find("Effect_choose");
	    this.lotteryEff = uiRefGo.gameObject.GetComponent<LotteryEff>();
        this.XTZP = cont.Find("XTZP");
        this.btnClose = cont.Find("btn_close").gameObject;
        this.btnDraw1 = cont.Find("btn_CJ1").gameObject;
        this.btnDraw5 = cont.Find("btn_CJ5").gameObject;
        this.txtJiang = cont.Find("Txt_JiangNum").GetComponent<UILabel>();
        this.draw1CostLabel = this.btnDraw1.transform.Find("drawLabel").GetComponent<UILabel>();
        this.draw5CostLabel = this.btnDraw5.transform.Find("drawLabel").GetComponent<UILabel>();
        this.draw1TimesLabel = this.btnDraw1.transform.Find("drawTimes").GetComponent<UILabel>();
        this.draw5TimesLabel = this.btnDraw5.transform.Find("drawTimes").GetComponent<UILabel>();
        this.countdown = cont.Find("countdown").GetComponent<UILabel>();
	    
	    this.lotteryEff.StopPS();
        this.itemList.Clear();
        Item_LotteryDraw item;
        for (int i = 1; i <= 8; i++){
            item = itms.Find("item_item_" + i).gameObject.AddComponent<Item_LotteryDraw>();
            item.InitData();
            this.itemList.Add(item);
	    }

	    UIEventListener.Get(this.btnClose).onClick = ( button )=>{
            this.Close();
		};

	    UIEventListener.Get(this.btnDraw1).onClick = ( button )=>{
            this.OnDraw(1u);
	    };

	    UIEventListener.Get(this.btnDraw5).onClick = ( button )=>{
            this.OnDraw(this.comboDrawTimes);
	    };
    }

    public bool StartBatchDraw(uint[] arr_award) {//连抽奖励
        this.lotteryEff.PlayPS();
        this.UpdateSelected(false,null);
        this.selectedItems.Clear();
        tmpSelects.Clear();
        this.rotarray.Clear();

        int cnt = this.itemList.Count;
        Item_LotteryDraw item;
        for (int j = 0; j < arr_award.Length; j++) {
            item = this.GetRandomAwardItem(arr_award[j]);
            if (item != null) {
                float rot = this.CalTargetRot(item.mSprItemIcon.gameObject);
                tmpSelects.Add(item);
                rotarray.Add(rot);
            } else {
                LogMgr.LogError("error award id : " + arr_award[j]);
            }
        }
        if (rotarray.Count == 0) {
            return false;
        }
        this.comboCircleNum = rotarray.Count;
        LogMgr.Log("rot Numbers=" + rotarray.Count.ToString());

        this.BatchDrawRun();
        return true;
    }
    public void SetDrawBtnStatus(bool enabled, bool iscombo ){
	    this.btnDraw1.GetComponent<BoxCollider>().enabled = enabled;
	    this.btnDraw5.GetComponent<BoxCollider>().enabled = enabled;
	    this.btnClose.GetComponent<BoxCollider>().enabled = enabled;
	
	    if(enabled == false){
            GameUtils.SetGray(this.btnDraw1, iscombo == false);
            GameUtils.SetGray(this.btnDraw5, iscombo == true);
	    }else{
            GameUtils.SetGray(this.btnDraw1, false);
            GameUtils.SetGray(this.btnDraw5, false);
	    }
    }
    
    public void BatchDrawRun(){
        List<float> rot_list = new List<float>();
        for (int i = 0; i < this.rotarray.Count; i++){
            rot_list.Add((i*2 + 3) * 360 - this.rotarray[i]);
        }
        int _cur_index = -1;
        Item_LotteryDraw select_item = null;
        AnimRotate.Begin(this.XTZP.gameObject, 700, (step, is_finish) => {
            this.selectedItems.Add(this.tmpSelects[step]);
            this.UpdateSelected(true, this.tmpSelects[step]);
            if (select_item != null) {
                select_item.SetShowTick(false);
            }
            this.tmpSelects[step].SetShowTick(true);

            if (is_finish) {
                this.lotteryEff.StopPS();
                this.OnRotateFinish();
            }
        }, rot_list, (rotate) => {
            rotate = Mathf.Abs(rotate);
            if (Mathf.FloorToInt(rotate / 45) != _cur_index) {
                _cur_index = Mathf.FloorToInt(rotate / 45);
                GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.LotteryAni, false, false, 1);//每转过一个物品，播放一次
                if (select_item != null) {
                    select_item.SetShowTick(false);
                }
                select_item = this.itemList[_cur_index % this.itemList.Count];
                select_item.SetShowTick(true);
            }
        });
    }
    public Item_LotteryDraw GetRandomAwardItem(uint award_id) {//根据awardID获取对应奖励
        List<Item_LotteryDraw> list = new List<Item_LotteryDraw>();
        for (int i = 0; i < this.itemList.Count; i++) {
            if (this.itemList[i].data.awardId == award_id) {
                list.Add(this.itemList[i]);
            }
        }
        if (list.Count > 0) {
            return list[Random.Range(0,list.Count)];
        } else {
            return null;
        }
    }
    
    public bool StartDraw(uint awardId ){//单抽奖励
	    this.lotteryEff.PlayPS();
	    this.UpdateSelected(false,null);
	    this.selectedItems.Clear();

        Item_LotteryDraw item = this.GetRandomAwardItem(awardId);
        if (item == null) {
            return false;
        }

        this.selectedItems.Add(item);
        float rot = this.CalTargetRot(item.mSprItemIcon.gameObject);

	    rot = rot - 360 * this.circleNum;

        TweenRotationExt trot = TweenRotationExt.Begin(this.XTZP.gameObject, this.rotDuration, new Vector3(0, 0, rot));
        trot.animationCurve = GameParams.Instance.lotteryDrawCure;
        int _cur_index = -1;
        Item_LotteryDraw select_item = null;
        trot.OnUpdateValue = (pos) => {
            float rotate = Mathf.Abs(pos.z);
            if (Mathf.FloorToInt(rotate / 45) != _cur_index) {
                _cur_index = Mathf.FloorToInt(rotate / 45);
                GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.LotteryAni, false, false, 1);//每转过一个物品，播放一次
                if (select_item != null) {
                    select_item.SetShowTick(false);
                }
                select_item = this.itemList[_cur_index % this.itemList.Count];
                select_item.SetShowTick(true);
            }
        };
        trot.SetOnFinished(() => {
            this.UpdateSelected(true, item);
            this.lotteryEff.StopPS();
            this.OnRotateFinish();
            if (select_item != null) {
                select_item.SetShowTick(false);
            }
        });
	    return true;
    }
    
    public void UpdateSelected(bool show, Item_LotteryDraw item) {
        if (show) {
            int num = 0;
            for (int i = 0; i < this.selectedItems.Count; i++) {
                if (this.selectedItems[i] == item) {
                    num++;
                }
            }
            if (item.mObjChoose.activeSelf == false) {
                item.mObjChoose.SetActive(true);
            }
            if (num > 1) {
                item.mLbChooseNum.text = num.ToString();
            } else {
                item.mLbChooseNum.text = string.Empty;
            }
            this.PlayAwardAnim(item);
        } else {
            for (int i = 0; i < this.selectedItems.Count; i++) {
                this.selectedItems[i].mObjChoose.SetActive(false);
            }
        }
    }
    
    public void PlayAwardAnim(Item_LotteryDraw cell ){
        GameObject clone = this.CreateCloneItem(cell);
	    AnimBounce.Begin(clone, ()=>{
		    TweenPosition tp = TweenPosition.Begin(clone, 0.2f, Vector3.zero);
		    tp.SetOnFinished(()=>{
                GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEffGetGold, clone.transform.parent);
                obj.transform.localPosition = Vector3.zero;
                GameUtils.SetPSRenderQueue(obj, 5, 0);
                GameObject.Destroy(obj, 3f);
                GameObject.Destroy(clone);
            });
        }, 100, 0.15f, 3);
    }
    public GameObject CreateCloneItem(Item_LotteryDraw cell) {
	    GameObject clone = GameUtils.CreateGo(cell.mSprItemIcon.gameObject);
        clone.transform.SetParent(cell.mSprItemIcon.transform.parent);
	    clone.transform.localScale = cell.mSprItemIcon.transform.localScale;
	    clone.transform.localPosition = cell.mSprItemIcon.transform.localPosition;
	    clone.name = "Clone"+cell.mSprItemIcon.name;
	    clone.transform.GetChild(1).gameObject.SetActive(false);
	    GameObject kuang = clone.transform.GetChild(0).gameObject;
	    if (kuang.activeSelf) {
		    int d = cell.mSprItemIcon.transform.GetChild(0).GetComponent<UISprite>().depth;
            clone.GetComponent<UISprite>().depth = d + 1;
		    kuang.GetComponent<UISprite>().depth = d + 2;
	    }
	    return clone;
    }

    public float CalTargetRot(GameObject item) {
        Transform t = item.transform;
        Transform pt = t.parent.parent;
        Vector3 p = t.localPosition + pt.localPosition;
        Vector3 sp = p - this.XTZP.localPosition;
        float angle = Vector3.Angle(Vector3.up, sp);
        if (sp.x > 0) {
            angle = -angle + 360;
        }
        return angle;
    }

    public void UpdateJiang(int jiangnum) {
        this.txtJiang.text = jiangnum.ToString();
    }
    public void UpdateTimedown(string str) {
        this.countdown.text = str;
    }
    public void RefreshAwardItems(List<ItemData> ItemDatas) {
        for (int i = 0; i < this.itemList.Count; i++) {
            this.itemList[i].SetItem(ItemDatas[i]);
        }
    }
    public void OnRotateFinish() {
        RoleItemModel.Instance.Notifiy ("ItemInfoChange",null);
        RoleItemModel.Instance.Notifiy ("Msg_GoldNumChange",null);
        this.ShowAwardDialog();
    }
    public void OnDraw(uint num) {
        if(this.isDrawing ){
          LogMgr.Log("抽奖中.. 未结束");
          return;
        }
        LogMgr.Log("抽 "+num+" 次"+this.LotteryTick);
        this.ReqLottery(num,this.LotteryTick);
    }

    public void OnBrocast(NetCmdType eventType, NetCmdPack param) {
        switch (eventType) {
            case NetCmdType.SUB_S_LOTTERY_AWARD:
                SC_GR_LotteryAward lottery_award = param.ToObj<SC_GR_LotteryAward>();
                var list = this.ConvertAwardItems(lottery_award.ArrAwardID);
                this.LotteryTick = lottery_award.LotteryTick;
                this.LotteryCountDown = lottery_award.LotteryCountDown;
                this.RefreshAwardItems(list);

                TimeManager.ClearIntervalID(this.timerId);
                this.LotteryTickTimedown = UnityEngine.Time.realtimeSinceStartup + this.LotteryCountDown;
                this.timerId = TimeManager.StartTimerInterval(() => {
                    if (UnityEngine.Time.realtimeSinceStartup < this.LotteryTickTimedown) {
                        float timespan = this.LotteryTickTimedown - UnityEngine.Time.realtimeSinceStartup;
                        this.UpdateTimedown(GameUtils.ToTimeStr(timespan));
                    } else {
                        TimeManager.ClearIntervalID(this.timerId);
                        this.ReqLotteryAward();
                    }
                });
                break;
            case NetCmdType.SUB_S_LOTTERY:
                SC_GR_Lottery lottery = param.ToObj<SC_GR_Lottery>();
                this.awardIDlist = lottery.ArrAwardID;
                bool ret = false;
                bool iscombo = false;
                if (this.awardIDlist.Length == 1) {
                    iscombo = false;
                    ret = this.StartDraw(this.awardIDlist[0]);
                } else {
                    iscombo = true;
                    ret = this.StartBatchDraw(this.awardIDlist);
                }
                this.isDrawing = ret;
                this.SetDrawBtnStatus(this.isDrawing == false, iscombo);
                this.UpdateJiangNumber(null);

                //抽奖信息统计
                int cnt = RoleItemModel.Instance.getItemCount(LotteryTickItemID);
                MtaManager.AddLotteryUserEvent(this.awardIDlist.Length, cnt);
                break;
        }
        //ushort cnt = RoleItemModel.Instance.getItemCount(LotteryTickItemID);
        //this.UpdateJiang(cnt)
    }
    public void ReqLottery(uint drawTimes,uint awardTick ){
        CS_GR_Lottery regReq = new CS_GR_Lottery();
	    regReq.SetCmdType(NetCmdType.SUB_C_LOTTERY);
        regReq.TicketTime = drawTimes;
        regReq.LotteryTick = awardTick;
        NetServices.Instance.Send<CS_GR_Lottery>(regReq);
    }
    public void ReqLotteryAward(){
        CS_GR_LotteryAward regReq = new CS_GR_LotteryAward();
	    regReq.SetCmdType(NetCmdType.SUB_C_LOTTERY_AWARD);
        NetServices.Instance.Send<CS_GR_LotteryAward>(regReq);
    }
    public override void Init(object data) {
        base.Init(data);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_LOTTERY_AWARD, this.OnBrocast);
        NetEventManager.RegisterEvent(NetCmdType.SUB_S_LOTTERY, this.OnBrocast);
        RoleItemModel.Instance.RegisterGlobalMsg(SysEventType.ItemInfoChange, this.UpdateJiangNumber, false);
    }
    public override void Show() {
        WndManager.LoadUIGameObject("LotteryDrawUI", SceneObjMgr.Instance.UIPanelTransform,
            delegate(GameObject obj) {
                uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
                TweenShow();

                this.InitUIComponets();
                this.InitData();
            }
        );
        base.Show();
    }
    public override void Close() {
        TimeManager.ClearIntervalID(this.timerId);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_LOTTERY_AWARD, this.OnBrocast);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_S_LOTTERY, this.OnBrocast);
        RoleItemModel.Instance.UnRegisterGlobalMsg(SysEventType.ItemInfoChange, this.UpdateJiangNumber);
        base.Close();
    }
}
