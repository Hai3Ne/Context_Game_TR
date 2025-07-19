using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShenHua_ZhuanPan_Item : UIItem {
    public UISprite mSprIcon;
    public UISprite mSprIconSelect;
    public GameObject mObjIcon;
    public GameObject mObjSelect;

    public SHEnumOption mOption;
    public bool mIsSelect;
    public void InitData(SHEnumOption option) {
        this.mOption = option;
        this.mSprIcon.spriteName = SHGameConfig.OptionIcons[(int)option];
        this.mSprIcon.MakePixelPerfect();
        this.mSprIconSelect.spriteName = SHGameConfig.SelectIcons[(int)option];
        this.mSprIconSelect.MakePixelPerfect();
        this.SetSelect(false,false);
    }
    public void SetSelect(bool is_select,bool is_anim = false) {
        this.mIsSelect = is_select;
        this.is_anim = is_anim;
        this.mObjIcon.SetActive(is_select == false);
        this.mObjSelect.SetActive(is_select);
        _time = 0;
    }

    private bool is_anim;
    private float _time;
    public void Update() {
        if (is_anim) {
            _time += Time.deltaTime;
            if (_time > 0.55f) {
                this.SetSelect(this.mIsSelect == false, true);
            }
        }
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_spr_icon":
                this.mSprIcon = tf.GetComponent<UISprite>();
                this.mObjIcon = tf.gameObject;
                break;
            case "item_spr_icon_select":
                this.mSprIconSelect = tf.GetComponent<UISprite>();
                this.mObjSelect = tf.gameObject;
                break;
        }
    }
}
