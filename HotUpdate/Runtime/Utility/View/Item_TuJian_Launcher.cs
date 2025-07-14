using UnityEngine;
using System.Collections;

public class Item_TuJian_Launcher : MonoBehaviour {
    public UILabel mLbName;//名称
    public UISprite mSprLauncher;//图标
    public UISprite mSprSkill;//技能图标
    public UILabel mLbSkillName;//技能名称
    public UILabel mLbSkillInfo;//技能信息
    public UILabel mLbAtkSpd;//攻速
    public UILabel mLbEnergy;//能量积攒
    public UILabel mLbItem;//道具掉落
    public UILabel mLbHit;//命中系数

    public void Awake() {
        GameUtils.Traversal(this.transform, this.OnNodeAsset);
        //UIEventListener.Get(this.transform.Find("item_btn_frame").gameObject).onClick = this.OnButtonClick;
    }

    private LauncherBookVo mVo;
    public void InitData(LauncherBookVo vo) {
        this.mVo = vo;

        this.mLbName.text = StringTable.GetString(this.mVo.PicName);
        this.mSprLauncher.spriteName = this.mVo.ResName.ToString();
        UISpriteData sd = this.mSprLauncher.GetAtlasSprite();
        int height = 200;
        int width = height * sd.width / sd.height;
        this.mSprLauncher.SetDimensions(width, height);

        this.mSprSkill.spriteName = this.mVo.SkillIcon;
        this.mLbSkillName.text = StringTable.GetString(this.mVo.SkillName);
        this.mLbSkillInfo.text = StringTable.GetString(this.mVo.SkillDes);
        this.mLbAtkSpd.text = string.Format("{0:0.0} 发/秒", 1 / this.mVo.AttackSpeed);
        this.mLbEnergy.text = string.Format("+{0:0.##}%", this.mVo.EnergySpeed / 100);
        this.mLbItem.text = string.Format("+{0:#.##}%", this.mVo.DropRate / 100);
        this.mLbHit.text = string.Format("{0}%", this.mVo.HitRate);
    }
    //public void OnButtonClick(GameObject obj) {
    //    //打开炮台详情界面
    //    WndManager.Instance.ShowUI(EnumUI.UI_LauncherInfo, this.mVo);
    //}
    private void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_spr_launcher":
                this.mSprLauncher = tf.GetComponent<UISprite>();
                break;
            case "spr_skill":
                this.mSprSkill = tf.GetComponent<UISprite>();
                break;
            case "lb_skill_name":
                this.mLbSkillName = tf.GetComponent<UILabel>();
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
            case "lb_skill_info":
                this.mLbSkillInfo = tf.GetComponent<UILabel>();
                break;
        }
    }
}
