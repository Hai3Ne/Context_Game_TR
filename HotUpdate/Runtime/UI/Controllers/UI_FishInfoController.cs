using UnityEngine;
using System.Collections;

public class UI_FishInfoController : IUIControllerImp {
    public UILabel mLbTitle;//标题
    public Transform mFishContent;
    public UILabel mLbVal;//分值
    public UILabel mLbSpd;//速度
    public UILabel mLbCatch;//捕获难度
    public UILabel mLbFishInfo;//鱼的详情
    public GameObject mBtnClose;

    private FishBookVo mVo;
    private GameObject mObjFish;
    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }
    private void SetSpeed(uint spd) {
        switch (spd) {
            case 1://1.较快
                this.mLbSpd.text = "较快";
                break;
            case 2://2.块
                this.mLbSpd.text = "快";
                break;
            case 3://3.一般
                this.mLbSpd.text = "一般";
                break;
            case 4://4.慢
                this.mLbSpd.text = "慢";
                break;
            case 5://5.较慢
                this.mLbSpd.text = "较慢";
                break;
            default:
                this.mLbSpd.text = this.mVo.Speed.ToString();
                break;
        }
    }
    private void SetDifficulty(uint difficulty) {
        switch (difficulty) {
            case 1:
                this.mLbCatch.text = "★";
                break;
            case 2:
                this.mLbCatch.text = "★★";
                break;
            case 3:
                this.mLbCatch.text = "★★★";
                break;
            case 4:
                this.mLbCatch.text = "★★★★";
                break;
            case 5:
                this.mLbCatch.text = "★★★★★";
                break;
            default:
                this.mLbCatch.text = string.Empty;
                break;
        }
    }
    public void InitData() {
        this.mLbTitle.text = StringTable.GetString(this.mVo.PicName);
        if (this.mVo.Multiple == 0) {
            this.mLbVal.text = "随机倍率";
        } else {
            this.mLbVal.text = this.mVo.Multiple.ToString();
        }
        this.SetSpeed(this.mVo.Speed);
        this.SetDifficulty(this.mVo.Difficulty);
        this.mLbFishInfo.text = StringTable.GetString(this.mVo.PicDes);

        GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet(this.mVo.ModelName);
        this.mObjFish = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);

        ModelMeshRenderRef renderRef = mObjFish.GetComponent<ModelMeshRenderRef>();
        this.mObjFish.SetActive(true);
        GameUtils.SetGOLayer(this.mObjFish, this.mFishContent.gameObject.layer);
        this.mObjFish.transform.SetParent(this.mFishContent);

        var BossPart_ColliderName = "FishBoxcollider".ToLower();
        BoxCollider mFishCollider = this.mObjFish.GetComponent<BoxCollider>();
        if (mFishCollider == null) {
            BoxCollider[] css = this.mObjFish.GetComponentsInChildren<BoxCollider>(true);
            mFishCollider = System.Array.Find(css, x => x.name.ToLower() != BossPart_ColliderName);
        }
        float s = 1;
        if (mFishCollider != null) {
            s = Mathf.Max(s, mFishCollider.size.x, mFishCollider.size.y, mFishCollider.size.z);
            s = 400 / s;
            Transform tf = mFishCollider.transform;
            if (this.mObjFish.transform != tf) {
                s /= mFishCollider.transform.localScale.x;
                tf = tf.parent;
            }
        }
        s *= this.mVo.Scale;
        if (this.mVo.Face != 0) {
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * this.mVo.Face);
        }
        this.mObjFish.transform.localScale = Vector3.one;
        this.mObjFish.transform.localPosition = Vector3.zero;
        this.mFishContent.localScale = new Vector3(s, s, s);

        this.mObjFish.SetActive(true);

        Animator anim = this.mObjFish.GetComponentInChildren<Animator>();
        Fish.ResetAnim(anim);
        if (this.mVo.AnimSpeed != 1) {
            anim.speed *= this.mVo.AnimSpeed;
        }
        if (renderRef == null || renderRef.skilledrenders.Length > 0) {
            ParticleSystem[] pss = this.mObjFish.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < pss.Length; i++) {
                if (pss[i].main.scalingMode != ParticleSystemScalingMode.Hierarchy) {
                    pss[i].Stop();
                }
            }
        }
        Renderer[] renderers = this.mObjFish.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].sortingOrder += 4;
        }

    }
    public override void Init(object data) {
        base.Init(data);
        this.mVo = data as FishBookVo;
    }
    public override void Show() {
        WndManager.LoadUIGameObject("UI_FishInfo", SceneObjMgr.Instance.UIPanelTransform,
            delegate(GameObject obj) {
                uiRefGo = obj;
                WndManager.Instance.Push(uiRefGo);

                GameUtils.Traversal(obj.transform, this.OnNodeAsset);
                UIEventListener.Get(this.mBtnClose).onClick = this.OnButtonClick;
                this.InitData();
                UI3DModelManager.AddUIPanel(uiRefGo);
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
            case "fish_content":
                this.mFishContent = tf;
                break;
            case "lb_val":
                this.mLbVal = tf.GetComponent<UILabel>();
                break;
            case "lb_spd":
                this.mLbSpd = tf.GetComponent<UILabel>();
                break;
            case "lb_catch":
                this.mLbCatch = tf.GetComponent<UILabel>();
                break;
            case "lb_fish_info":
                this.mLbFishInfo = tf.GetComponent<UILabel>();
                break;
            case "btn_close":
                this.mBtnClose = tf.gameObject;
                break;
        }
    }
}
