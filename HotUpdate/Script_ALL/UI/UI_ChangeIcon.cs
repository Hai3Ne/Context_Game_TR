using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChangeIcon : UILayer {
    public UIScrollView mScrollView;
    public UIGrid mGridIcon;
    public GameObject mItemIcon;
    public UISprite mSprSelectTick;
    public UITexture mTexturePlayer;

    private ushort mInitFace;
    private ushort mSelectFace;
    
    public void InitData(){
        this.mSelectFace = HallHandle.FaceID;
        this.mInitFace = this.mSelectFace;
	
        this.SetPlayerIcon(this.mSelectFace);
	
        GameObject select_icon = null;
        GameObject obj;
        for (int i = 0; i < 30; i++) {
            obj = UnityEngine.GameObject.Instantiate(this.mItemIcon);
            obj.transform.SetParent(this.mGridIcon.transform);
            obj.SetActive(true);
            obj.transform.localScale = Vector3.one;

            ushort face = (ushort)i;
		    UITexture texture_player = obj.transform.GetComponent<UITexture>();
            texture_player.uvRect = GameUtils.FaceUVRect(face);
		
            UIEventListener.Get(obj).onClick =  ( button )=>{
                this.SelectPlayerIcon(face,button);
            };
		
            if(face == this.mSelectFace){
                select_icon = obj;
            }
        }
        this.mGridIcon.Reposition();
        if(select_icon != null){
            this.SelectPlayerIcon(this.mSelectFace,select_icon);
        }
    }

    public void SetPlayerIcon(uint face){
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(face);
    }
    public void SelectPlayerIcon(ushort face, GameObject button) {
        this.mSelectFace = face;
        this.SetPlayerIcon(face);
        this.mSprSelectTick.transform.position = button.transform.position;
    }
    
    public override void OnNodeLoad() { }
    public override void OnEnter() {
        this.mScrollView.ResetPosition();
    }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_ok"://
                if (this.mInitFace != this.mSelectFace) {
                    HallHandle.ChangePlayerHeadIcon(this.mSelectFace);
                }
                this.Close();
                break;
            case "btn_cancel":
                this.Close();
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) { 
        switch(name){
        case "scrollview_info" ://
            this.mScrollView = tf.GetComponent<UIScrollView>();
                break;
        case "grid_icon" ://-- 
            this.mGridIcon = tf.GetComponent<UIGrid>();
                break;
        case "btn_player_icon" ://
            this.mItemIcon = tf.gameObject;
            this.mItemIcon.SetActive(false);
                break;
        case "spr_select_tick" ://
            this.mSprSelectTick = tf.GetComponent<UISprite>();
                break;
        case "texture_player"://--当前选中头像
            this.mTexturePlayer = tf.GetComponent<UITexture>();
                break;
        }
    }
}
