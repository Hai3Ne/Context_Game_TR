using UnityEngine;
using System.Collections;

public class UI_LauncherInfoController : IUIControllerImp {
    public UILabel mLbTitle;
    public UISprite mSprLauncher;//炮台图片
    public UISprite mSprSkill;//技能图标
    public UILabel mLbSkillName;//技能名称
    public UILabel mLbSkillInfo;//技能信息
    public UILabel mLbAtkSpd;//攻速
    public UILabel mLbEnergy;//能量积攒
    public UILabel mLbItem;//道具掉落
    public UILabel mLbHit;//命中系数
    public UILabel mLbLauncherInfo;//炮台简介
    public GameObject mBtnClose;

    private LauncherBookVo mVo;
    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }
    public void InitData() {
        this.mLbTitle.text = StringTable.GetString(this.mVo.PicName);
        this.mSprSkill.spriteName = this.mVo.SkillIcon;
        this.mLbSkillName.text = StringTable.GetString(this.mVo.SkillName);
        this.mLbSkillInfo.text = StringTable.GetString(this.mVo.SkillDes);
        this.mLbAtkSpd.text = string.Format("{0:0.0} 发/秒", 1 / this.mVo.AttackSpeed);
        this.mLbEnergy.text = string.Format("+{0:0.##}%", this.mVo.EnergySpeed / 100f);
        this.mLbItem.text = string.Format("+{0:#.##}%", this.mVo.DropRate / 100);
        this.mLbHit.text = string.Format("{0}%", this.mVo.HitRate);
        this.mLbLauncherInfo.text = StringTable.GetString(this.mVo.PicDes);

        this.mSprLauncher.spriteName = this.mVo.ResName.ToString();
        UISpriteData sd = this.mSprLauncher.GetAtlasSprite();
        int height = 350;
        int width = height * sd.width / sd.height;
        this.mSprLauncher.SetDimensions(width, height);
    }
    public override void Init(object data) {
        base.Init(data);
        this.mVo = data as LauncherBookVo;
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_LauncherInfo", SceneObjMgr.Instance.UIPanelTransform,
            delegate(GameObject obj) {
                uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);
				TweenShow();
                GameUtils.Traversal(obj.transform, this.OnNodeAsset);
                UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;
                this.InitData();
            }
        );
        base.Show();
    }
    public override void Close() {
        base.Close();
    }
    private void OnButtonClick(GameObject obj) {
        if (this.mBtnClose == obj) {
            this.Close();
        }
    }
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_title":
                this.mLbTitle = tf.GetComponent<UILabel>();
                break;
            case "spr_launcher":
                this.mSprLauncher = tf.GetComponent<UISprite>();
                break;
            case "spr_skill":
                this.mSprSkill = tf.GetComponent<UISprite>();
                break;
            case "lb_skill_name":
                this.mLbSkillName = tf.GetComponent<UILabel>();
                break;
            case "lb_skill_info":
                this.mLbSkillInfo = tf.GetComponent<UILabel>();
                break;
            case "lb_atkspd":
                this.mLbAtkSpd = tf.GetComponent<UILabel>();
                break;
            case "lb_energy":
                this.mLbEnergy = tf.GetComponent<UILabel>();
                break;
            case "lb_item":
                this.mLbItem = tf.GetComponent<UILabel>();
                break;
            case "lb_hit":
                this.mLbHit = tf.GetComponent<UILabel>();
                break;
            case "lb_launcher_info":
                this.mLbLauncherInfo = tf.GetComponent<UILabel>();
                break;
            case "btn_close":
                this.mBtnClose = tf.gameObject;
                break;
        }
    }
}
