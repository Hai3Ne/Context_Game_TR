using UnityEngine;
using System.Collections;

public class AnimBossComingTip : MonoBehaviour {
    public UILabel mLbName;
    public UILabel mLbVal;
    public Transform mFishParent;

    private FishVo mVo;
    private GameObject mObjFish;

    public void Awake() {
        WndManager.Instance.SetPanelSort(this.gameObject);
        UI3DModelManager.AddUIPanel(this.gameObject);
    }

    public void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "fishname":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "beilv":
                this.mLbVal = tf.GetComponent<UILabel>();
                break;
            case "fishmodel":
                this.mFishParent = tf;
                break;
        }
    }

    public void SetFishVo(FishVo vo) {
        GameUtils.Traversal(this.transform, this.OnNodeAsset);
        this.mVo = vo;
        this.mLbName.text = StringTable.GetString(this.mVo.Name);
        if (this.mVo.SourceID == ConstValue.PandoraID) {//潘多拉不显示倍率
            this.mLbVal.text = string.Empty;
            this.mLbName.transform.localPosition -= new Vector3(this.mLbName.width >> 1, 0);
        } else {
            if (this.mVo.CfgID == ConstValue.HaiDaoBoxID) {//海盗宝箱倍率特殊处理
                this.mLbVal.text = string.Format("{0}倍", ScenePirateBoxMgr.GetPirateBoxMul(ConstValue.PirateBoxID, (ushort)this.mVo.Multiple));
            } else {
                this.mLbVal.text = string.Format("{0}倍", this.mVo.Multiple);
            }
        }

        GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet(this.mVo.SourceID);
        this.mObjFish = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);

        ModelMeshRenderRef renderRef = mObjFish.GetComponent<ModelMeshRenderRef>();
        this.mObjFish.SetActive(true);
        GameUtils.SetGOLayer(this.mObjFish, this.mFishParent.gameObject.layer);
        this.mObjFish.transform.SetParent(this.mFishParent);


        var BossPart_ColliderName = "FishBoxcollider".ToLower();
        BoxCollider mFishCollider = this.mObjFish.GetComponent<BoxCollider>();
        if (mFishCollider == null) {
            BoxCollider[] css = this.mObjFish.GetComponentsInChildren<BoxCollider>(true);
            for (int i = 0; i < css.Length; i++) {
                css[i].enabled = false;
            }
            mFishCollider = System.Array.Find(css, x => x.name.ToLower() != BossPart_ColliderName);
        } else {
            mFishCollider.enabled = false;
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
        if (this.mVo.SourceID == ConstValue.WorldBossModelID) {//全服宝箱锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 180f);
        } else if (this.mVo.SourceID == ConstValue.HaiDaoBoxID) {//海盗宝箱锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
            s = s * 0.5f;
        } else if (this.mVo.SourceID == ConstValue.FootFish) {//足球锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
            s = s * 0.5f;
        } else if (this.mVo.SourceID == ConstValue.PandoraID) {//潘多拉锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
            s = s * 0.4f;
        } else {
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
        }
        this.mObjFish.transform.localScale = Vector3.one;
        this.mObjFish.transform.localPosition = Vector3.zero;
        this.mFishParent.localScale = new Vector3(s, s, s);

        this.mObjFish.SetActive(true);

        Animator anim = this.mObjFish.GetComponentInChildren<Animator>();
        Fish.ResetAnim(anim);
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
            renderers[i].sortingOrder += 6;
        }
    }
}
