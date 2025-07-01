using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_ExitGameController : IUIControllerImp {
    public UILabel mLbEnergy;//当前炮台能量
    public GameObject mItemSkill;
    public UIGrid mGridSkill;
    public GameObject mItemHero;
    public UIGrid mGridHero;
    public UILabel mLbTotalGold;//总价值
    public UILabel mLbSellGold;//出售获得总金额
    public GameObject mBtnSell;//出售
    public UISprite mSprSell;
    public UISprite mSprSellZheKou;//折扣背景图
    public UILabel mLbAutoTick;//道具自动使用提示
    public GameObject mBtnAutoLauncherOn;//自动释放炮台技能
    public GameObject mBtnAutoLauncherOff;
    public GameObject mBtnAutoItemOn;//自动道具技能
    public GameObject mBtnAutoItemOff;
    public GameObject mBtnAutoHeroOn;//自动召唤英雄
    public GameObject mBtnAutoHeroOff;
    public GameObject mBtnClose;
    public GameObject mBtnChangeTable;//快速换桌
    public GameObject mBtnExitGame;//退出
    public GameObject mBtnGo;//继续游戏

	private uint[] skillItemList = null;
    private List<SkillItemRender> itemUIList = new List<SkillItemRender>();
    private uint[] heroItemIDList = null;
    private List<HeroItemRender> heroListRenders = new List<HeroItemRender>();
    private TimeRoomVo mRoomVo;

    private void InitData() {
        this.mLbAutoTick.text = StringTable.GetString("Tip_10", string.Empty);

        TimeRoomVo roomVo = FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID());
        skillItemList = roomVo.Items;
        for (int i = 0; i < skillItemList.Length; i++) {
            SkillItemRender itemRender = GameUtils.CreateGo(this.mItemSkill, this.mGridSkill.transform).GetComponent<SkillItemRender>();
            itemUIList.Add(itemRender);
        }

        heroItemIDList = roomVo.Heroes;
        for (int i = 0; i < heroItemIDList.Length; i++) {
            HeroItemRender heroRender = GameUtils.CreateGo(this.mItemHero, this.mGridHero.transform).GetComponent<HeroItemRender>();
            heroListRenders.Add(heroRender);
        }

        this.UpdateData();
    }
    public void UpdateData() {
        this.mLbEnergy.text = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.EnergyPoolLogic.CurEnergy.ToString();
        long price = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.EnergyPoolLogic.CurEnergy;
        for (int i = 0; i < itemUIList.Count; i++) {
            if (i < skillItemList.Length) {
                ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet(skillItemList[i]);
                int count = RoleItemModel.Instance.getItemCount(skillItemList[i]);
                itemUIList[i].gameObject.SetActive(true);
                itemUIList[i].iconSprite.spriteName = itemVo.ItemIcon;
                itemUIList[i].frameSp.spriteName = ConstValue.ItemFrameSpList[itemVo.Star];
                itemUIList[i].countLabel.text = string.Format("x{0}", count);
                itemUIList[i].mLbName.text = string.Format(ConstValue.ItemLvColors[itemVo.Star], itemVo.Star);
                //itemUIList[i].mLbName.text = StringTable.GetString(itemVo.ItemName);
                price += itemVo.SalePrice * count;

                UI_Ticks.AddTickListener(itemUIList[i].gameObject, StringTable.GetString(itemVo.ItemDec), SpriteAlignment.BottomLeft, itemVo.SalePrice);
            } else {
                itemUIList[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < heroListRenders.Count; i++) {
            if (i < heroItemIDList.Length) {
                ItemsVo itemVo = FishConfig.Instance.Itemconf.TryGet(heroItemIDList[i]);
                int count = RoleItemModel.Instance.getItemCount(heroItemIDList[i]);
                heroListRenders[i].gameObject.SetActive(true);
                heroListRenders[i].iconObj.spriteName = itemVo.ItemIcon.ToString();
                heroListRenders[i].countLabel.text = string.Format("x{0}", count);
                heroListRenders[i].mLbName.text = string.Format(ConstValue.ItemLvColors[itemVo.Star], itemVo.Star);
                //heroListRenders[i].mLbName.text = StringTable.GetString(itemVo.ItemName);
                heroListRenders[i].frameSp.spriteName = ConstValue.HeroFrameSpList[itemVo.Star];
                price += itemVo.SalePrice * count;

                UI_Ticks.AddTickListener(heroListRenders[i].gameObject, StringTable.GetString(itemVo.ItemDec), SpriteAlignment.BottomLeft, itemVo.SalePrice);
            } else {
                heroListRenders[i].gameObject.SetActive(false);
            }
        }

        this.mLbTotalGold.text = price.ToString();

        //特殊处理  防止精度缺失
        int rate = Mathf.FloorToInt(FishConfig.Instance.GameSettingConf.QuickSell * 10000);
        price = rate * price / 10000;
        this.mLbSellGold.text = price.ToString();


        if (price > 0) {
            if (__is_gray) {
                __is_gray = false;
                this.mBtnSell.GetComponent<BoxCollider>().enabled = true;
                this.mSprSell.IsGray = false;
                this.mSprSellZheKou.IsGray = false;
            }
        } else {
            if (__is_gray == false) {
                __is_gray = true;
                this.mBtnSell.GetComponent<BoxCollider>().enabled = false;
                this.mSprSell.IsGray = true;
                this.mSprSellZheKou.IsGray = true;
            }
        }
    }

    private bool __is_gray = false;
    void HandleItemChange(object args) {
        UpdateData();
    }
    public override EnumPanelPRI PRI {
        get { return EnumPanelPRI.HIGHT; }
    }
    public override void Init(object data) {
        base.Init(data);
    }

    public override void Show() {
        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_NotItem) {//无道具模式
            WndManager.LoadUIGameObject("UI_ExitGame_NoItem", SceneObjMgr.Instance.UIPanelTransform,
                delegate(GameObject obj) {
                    uiRefGo = obj;
                    WndManager.Instance.Push(uiRefGo);
                    TweenShow();

                    GameUtils.Traversal(obj.transform, this.OnNodeAsset);
                    UIEventListener.Get(this.mBtnChangeTable).onClick = this.OnButtonClick;
                    if (this.mBtnClose != null) {
                        UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;
                    }
                    UIEventListener.Get(this.mBtnExitGame).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnGo).onClick = this.OnButtonClick;
                }
            );
        } else {
            WndManager.LoadUIGameObject("UI_ExitGame", SceneObjMgr.Instance.UIPanelTransform,
                delegate(GameObject obj) {
                    uiRefGo = obj;
                    WndManager.Instance.Push(uiRefGo);
                    TweenShow();

                    GameUtils.Traversal(obj.transform, this.OnNodeAsset);
                    UIEventListener.Get(this.mBtnAutoHeroOff).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnAutoHeroOn).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnAutoItemOff).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnAutoItemOn).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnAutoLauncherOff).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnAutoLauncherOn).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnChangeTable).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnExitGame).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnGo).onClick = this.OnButtonClick;
                    UIEventListener.Get(this.mBtnSell).onClick = this.OnButtonClick;

                    this.SetAutoLauncherOP(GameConfig.OP_AutoLauncher);
                    this.SetAutoItemOP(GameConfig.OP_AutoSkill);
                    this.SetAutoHeroOP(GameConfig.OP_AutoHero);

                    this.mItemSkill.SetActive(false);
                    this.mItemHero.SetActive(false);

                    __is_gray = false;
                    this.InitData();

                    RoleItemModel.Instance.RegisterGlobalMsg(SysEventType.ItemInfoChange, HandleItemChange);
                    RoleItemModel.Instance.RegisterGlobalMsg(SysEventType.EngeryChange, HandleItemChange);
                }
            );
        }
        base.Show();
    }
    public override void Close() {
        if (RoleInfoModel.Instance.GameMode == EnumGameMode.Mode_NotItem) {//无道具模式
        } else {
            RoleItemModel.Instance.UnRegisterGlobalMsg(SysEventType.ItemInfoChange, HandleItemChange);
            RoleItemModel.Instance.UnRegisterGlobalMsg(SysEventType.EngeryChange, HandleItemChange);
            itemUIList.Clear();
            heroListRenders.Clear();
        }
        base.Close();
    }

    private void SetAutoLauncherOP(bool open) {
        this.mBtnAutoLauncherOn.SetActive(open);
        this.mBtnAutoLauncherOff.SetActive(open == false);
        GameConfig.OP_AutoLauncher = open;
        GameConfig.SaveData();
    }
    private void SetAutoItemOP(bool open) {
        this.mBtnAutoItemOn.SetActive(open);
        this.mBtnAutoItemOff.SetActive(open == false);
        GameConfig.OP_AutoSkill = open;
        GameConfig.SaveData();
    }
    private void SetAutoHeroOP(bool open) {
        this.mBtnAutoHeroOn.SetActive(open);
        this.mBtnAutoHeroOff.SetActive(open == false);
        GameConfig.OP_AutoHero = open;
        GameConfig.SaveData();
    }

    private void QuickSell() {//快速出售       
        SystemMessageMgr.Instance.DialogShow("Tip_9", FishNetAPI.Instance.SendQuickSell, null);
        //FishNetAPI.Instance.SendQuickSell();
    }
    public void OnButtonClick(GameObject obj) {
        if (this.mBtnClose == obj) {//关闭按钮
            this.Close();
        } else if (this.mBtnAutoLauncherOn == obj) {//自动释放炮台技能
            this.SetAutoLauncherOP(false);
        } else if (this.mBtnAutoLauncherOff == obj) {
            this.SetAutoLauncherOP(true);
        } else if (this.mBtnAutoItemOn == obj) {//自动使用道具技能
            this.SetAutoItemOP(false);
        } else if (this.mBtnAutoItemOff == obj) {
            this.SetAutoItemOP(true);
        } else if (this.mBtnAutoHeroOn == obj) {//自动召唤英雄
            this.SetAutoHeroOP(false);
        } else if (this.mBtnAutoHeroOff == obj) {
            this.SetAutoHeroOP(true);
        } else if (this.mBtnChangeTable == obj) {//快速换桌       
            this.Close(); 
            FishNetAPI.Instance.ChangeTableSeat();
        } else if (this.mBtnExitGame == obj) {//退出房间
			this.Close();
			SceneLogic.Instance.BackToHall();
        } else if (this.mBtnGo == obj) {//继续游戏
            this.Close();
        }else if(this.mBtnSell == obj){//一键出售
            this.QuickSell();
        }
    }
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_energy":
                this.mLbEnergy = tf.GetComponent<UILabel>();
                break;
            case "item_skill":
                this.mItemSkill = tf.gameObject;
                break;
            case "grid_skill":
                this.mGridSkill = tf.GetComponent<UIGrid>();
                break;
            case "item_hero":
                this.mItemHero = tf.gameObject;
                break;
            case "grid_hero":
                this.mGridHero = tf.GetComponent<UIGrid>();
                break;
            case "lb_tutal_gold":
                this.mLbTotalGold = tf.GetComponent<UILabel>();
                break;
            case "lb_sell_gold":
                this.mLbSellGold = tf.GetComponent<UILabel>();
                break;
            case "spr_sell":
                this.mSprSellZheKou = tf.GetComponent<UISprite>();
                break;
            case "btn_sell":
                this.mSprSell = tf.GetComponent<UISprite>();
                this.mBtnSell = tf.gameObject;
                break;
            case "lb_auto_tick":
                this.mLbAutoTick = tf.GetComponent<UILabel>();
                break;
            case "btn_open_launcher":
                this.mBtnAutoLauncherOn = tf.gameObject;
                break;
            case "btn_close_launcher":
                this.mBtnAutoLauncherOff = tf.gameObject;
                break;
            case "btn_open_skill":
                this.mBtnAutoItemOn = tf.gameObject;
                break;
            case "btn_close_skill":
                this.mBtnAutoItemOff = tf.gameObject;
                break;
            case "btn_open_hero":
                this.mBtnAutoHeroOn = tf.gameObject;
                break;
            case "btn_close_hero":
                this.mBtnAutoHeroOff = tf.gameObject;
                break;
            case "btn_close":
                this.mBtnClose = tf.gameObject;
                break;
            case "btn_change_desk":
                this.mBtnChangeTable = tf.gameObject;
                break;
            case "btn_quit_game":
                this.mBtnExitGame = tf.gameObject;
                break;
            case "btn_go":
                this.mBtnGo = tf.gameObject;
                break;
        }
    }
}
