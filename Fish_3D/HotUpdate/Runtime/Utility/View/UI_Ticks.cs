using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UI_Ticks : MonoBehaviour {
    private class TickInfo {
        public string tick;
        public int gold;//金币提示
        public SpriteAlignment align;
    }
    private static Dictionary<GameObject, TickInfo> dic_tick = new Dictionary<GameObject, TickInfo>();
    private static UI_Ticks ui_tick;
    public static UI_Ticks Instance {
        get {
            if (ui_tick == null) {
                GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mUITick, SceneObjMgr.Instance.UIPanelTransform);
                ui_tick = obj.AddComponent<UI_Ticks>();
            }
            return ui_tick;
        }
    }
    public static void AddTickListener(GameObject obj, string tick, SpriteAlignment align,int gold) {
        if (dic_tick.ContainsKey(obj)) {
            dic_tick[obj].tick = tick;
            dic_tick[obj].align = align;
            dic_tick[obj].gold = gold;
        } else {
            dic_tick.Add(obj, new TickInfo {
                tick = tick,
                align = align,
                gold = gold,
            });

#if UNITY_STANDALONE 
            UIEventListener.Get(obj).onHover = on_hover;
#else
            UIEventListener.Get(obj).onPress = (_o,state)=>{
                if (state) {
                    TimeManager.AddDelayEvent(1201, 1, () => {
                        on_hover(_o, state);
                    });
                } else {
                    on_hover(_o, state);
                    TimeManager.RemoveDelayEvent(1201);
                }
            };
#endif
        }
    }
    public static void AddTickListener(GameObject obj, UIEventListener.BoolDelegate call) {
#if UNITY_STANDALONE
        UIEventListener.Get(obj).onHover = call;
#else
        UIEventListener.Get(obj).onPress = (_o, state) => {
            if (state) {
                TimeManager.AddDelayEvent(1201, 1, () => {
                    call(_o, state);
                });
            } else {
                call(_o, state);
                TimeManager.RemoveDelayEvent(1201);
            }
        };
#endif
    }

    private static void on_hover(GameObject go, bool state) {
        if (state) {
            Instance.ShowTick(go, dic_tick[go]);
        } else {
            Instance.Hide();
        }
    }
    //炮台技能显示
    public static void ShowLuncherSkill(GameObject obj, bool state) {
        if (state) {
            EnergyPool ep = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.EnergyPoolLogic;
            if (ep != null) {
                if (SceneLogic.Instance.PlayerMgr.MyClientSeat == 0) {
                    Instance.ShowLauncherSkill(obj, ep, SpriteAlignment.BottomLeft);
                } else {
                    Instance.ShowLauncherSkill(obj, ep, SpriteAlignment.BottomRight);
                }
            } else {
                Instance.Hide();
            }
        } else {
            Instance.Hide();
        }
    }

    private UISprite mSprFrame;
    private UISprite mSprArrow;
    private UILabel mLbInfo;
    private UILabel mLbGold;
    private UISprite mSprGold;

    private void Awake() {
        this.mSprFrame = this.transform.Find("spr_frame").GetComponent<UISprite>();
        this.mSprArrow = this.transform.Find("spr_frame/spr_arrow").GetComponent<UISprite>();
        this.mLbInfo = this.transform.Find("lb_info").GetComponent<UILabel>();
        this.mLbGold = this.transform.Find("lb_gold").GetComponent<UILabel>();
        this.mSprGold = this.transform.Find("lb_gold/spr_gold").GetComponent<UISprite>();
    }

    private void Hide() {
        this.gameObject.SetActive(false);
    }

    private GameObject cur_obj;//当前显示对象
    public void Update() {
        if (cur_obj == null || cur_obj.activeSelf == false) {
            this.Hide();
        }
    }


    private void ShowTick(GameObject obj,TickInfo tick) {
        this.gameObject.SetActive(true);

        this.mLbInfo.text = tick.tick;
        int width = this.mLbInfo.width;
        int height = this.mLbInfo.height;

        if (tick.gold > 0) {
            this.mLbGold.gameObject.SetActive(true);
            this.mLbGold.text = string.Format("单价:{0}", tick.gold);
            width = Mathf.Max(width, this.mLbGold.width + 40);
            this.mLbInfo.transform.localPosition = new Vector3(0, 6);
            this.mLbGold.transform.localPosition = new Vector3(width * 0.5f - 22, -height * 0.5f - 10);
            this.mSprGold.spriteName = "gold";//金币图标
            height += 10;
        } else {
            this.mLbGold.gameObject.SetActive(false);
            this.mLbInfo.transform.localPosition = new Vector3(0, 0);
        }

        this.ResetPosition(obj, tick.align, width, height);
    }
    private void ShowLauncherSkill(GameObject obj, EnergyPool ep, SpriteAlignment align) {
        SkillVo skill = ep.SkillVo;

        this.gameObject.SetActive(true);

        this.mLbInfo.text = StringTable.GetString(skill.Desc);
        int width = this.mLbInfo.width;
        int height = this.mLbInfo.height;

        if (ep.EnergyMax > 0) {
            this.mLbGold.gameObject.SetActive(true);
            this.mLbGold.text = string.Format("消耗能量:{0}", ep.EnergyMax);
            width = Mathf.Max(width, this.mLbGold.width+40);
            this.mLbInfo.transform.localPosition = new Vector3(0, 6);
            this.mLbGold.transform.localPosition = new Vector3(width * 0.5f - 22, -height * 0.5f - 10);
            this.mSprGold.spriteName = "powerball";//能量图标
            height += 10;
        } else {
            this.mLbGold.gameObject.SetActive(false);
            this.mLbInfo.transform.localPosition = new Vector3(0, 0);
        }

        this.ResetPosition(obj, align, width, height);
    }

    private void ResetPosition(GameObject obj, SpriteAlignment align, int width, int height) {
        cur_obj = obj;
        BoxCollider box = obj.GetComponent<BoxCollider>();
        this.transform.position = obj.transform.position;

        Vector3 pos = this.transform.localPosition;

		float hh0 = 23f, hh1 = 28f;
        float arrow_off = width * 0.5f - 38;//箭头便宜位置
        switch (align) {
            case SpriteAlignment.LeftCenter://左侧
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, -90);
                this.mSprFrame.SetDimensions(height + 50, width + 40);
                this.mSprArrow.transform.localPosition = new Vector3(0, -width * 0.5f - hh0);
                pos.x += width * 0.5f + box.size.x * 0.5f + 44;
                break;
            case SpriteAlignment.RightCenter://右侧
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 90);
                this.mSprFrame.SetDimensions(height + 50, width + 40);
                this.mSprArrow.transform.localPosition = new Vector3(0, -width * 0.5f - hh0);
                pos.x -= width * 0.5f + box.size.x * 0.5f + 44;
                break;
            case SpriteAlignment.TopLeft://左上
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 180);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(arrow_off, -height * 0.5f - hh1);
                pos.x += arrow_off;
                pos.y -= box.size.y * 0.5f + 60;
                break;
            case SpriteAlignment.TopRight://右上
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 180);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(-arrow_off, -height * 0.5f - hh1);
                pos.x -= arrow_off;
                pos.y -= box.size.y * 0.5f + 60;
                break;
            case SpriteAlignment.TopCenter://顶部
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 180);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(0, -height * 0.5f - hh1);
                pos.y -= box.size.y * 0.5f + 60;
                break;
            case SpriteAlignment.BottomLeft://左下
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(-arrow_off, -height * 0.5f - hh1);
                pos.x += arrow_off;
                pos.y += box.size.y * 0.5f + 60;
                break;
            case SpriteAlignment.BottomRight://右下
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(arrow_off, -height * 0.5f - hh1);
                pos.x -= arrow_off;
                pos.y += box.size.y * 0.5f + 60;
                break;
            case SpriteAlignment.BottomCenter://底部
            default:
                this.mSprFrame.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.mSprFrame.SetDimensions(width + 40, height + 50);
                this.mSprArrow.transform.localPosition = new Vector3(0, -height * 0.5f - hh1);
                pos.y += box.size.y * 0.5f + 60;
                break;
        }

        this.transform.localPosition = pos;
    }
}
