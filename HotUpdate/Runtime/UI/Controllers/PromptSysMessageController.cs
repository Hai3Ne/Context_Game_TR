using System;
using UnityEngine;

public class PromptMsgData{
	public string content;
	public byte showBtns;

	public static PromptMsgData GenPromptWithOkay(string msg){
		PromptMsgData data = new PromptMsgData ();
		data.content = msg;
		data.showBtns = 2;
		return data;
	}
}

public class PromptSysMessageController : IUIControllerImp
{
	public PromptSysMessageController () { }

    public override EnumPanelType PanelType {
        get {
            return EnumPanelType.FloatUI;
        }
    }

	public Action onConfirmCb, onCancelCb;
	GameObject mViewer = null;
	bool isInitingUI = false;
	UILabel msgLabel;
	GameObject btnOk,btnCancel;
	UISprite bgSprite;
	float btnOK_x, btnCancel_x;
	float btnHeight = 1f;
	byte BtnShowByteMask = 0xFF;
	public override void Init (object data)
	{
		base.Init (data);
		if (data is PromptMsgData) {
			PromptMsgData msgData = data as PromptMsgData;
			BtnShowByteMask = msgData.showBtns;
			mData = msgData.content;
		}
	}

	public override void Show ()
	{
		if (mViewer == null && !isInitingUI) {
			isInitingUI = true;
             GameObject prefabs = Resources.Load<GameObject>("PanelPrompt");
            mViewer = GameObject.Instantiate(prefabs, SceneObjMgr.Instance.UIContainerTransform) as GameObject;
            mViewer.transform.localPosition = Vector3.zero;
            mViewer.transform.localScale = Vector3.one;
            mViewer.transform.localRotation = Quaternion.identity;
            //WndManager.LoadUIGameObject ("PanelPrompt", SceneObjMgr.Instance.UIContainerTransform,
                //delegate(GameObject obj) {
                    //mViewer = obj;
                    //mViewer.transform.localPosition = Vector3.zero;
					isInitingUI = false;

                    GameUtils.Traversal(mViewer.transform, this.OnNodeAsset);
					btnHeight = btnOk.GetComponent<UISprite> ().height;
					btnOK_x = btnOk.transform.localPosition.x;
					btnCancel_x = btnCancel.transform.localPosition.x;
					RegisterEvents ();
                    Show();
                    //setPanelSortOrder (mSortOrder);
                //});
		} else {
			base.Show ();
			if (mViewer != null) {
				msgLabel.text = (string)(mData);
				LayoutUI ();
			}
        }
        this.mViewer.GetComponent<UIPanel>().depth = UI.GetDepth() + 99999;
	}

	void LayoutUI (){
        //float halfLableH = msgLabel.height * 0.5f;
		float totalH = msgLabel.height + 334f;
        bgSprite.height = (int)totalH;
		bgSprite.transform.localPosition = Vector3.zero;
        float btnY = 100 - totalH * 0.5f;
		//Vector3 startPos = new Vector3(0f, 140 - totalH * 0.5f, 0f);
        //msgLabel.transform.localPosition = startPos + Vector3.down * (halfLableH+25f);
        //float btnY = startPos.y - (msgLabel.height + 88 + btnHeight * 0.5f + 20f);

		if (BtnShowByteMask == 0x2){
			btnCancel.gameObject.SetActive (false);
			btnOk.transform.localPosition = new Vector3 (0f, btnY, 0f);	
		}else if (BtnShowByteMask == 0x1) {
			btnOk.gameObject.SetActive (false);
			btnCancel.transform.localPosition = new Vector3 (0f, btnY, 0f);
		} else {
			btnOk.gameObject.SetActive (true);
			btnCancel.gameObject.SetActive (true);
			btnOk.transform.localPosition = new Vector3 (btnOK_x, btnY, 0f);
			btnCancel.transform.localPosition = new Vector3 (btnCancel_x, btnY, 0f);
		}
	}

	int mSortOrder = -1;
	public void setPanelSortOrder(int sortOrder)
	{
		if (sortOrder == -1)
			return;
		
		if (mViewer == null)
			mSortOrder = sortOrder;
		else
			mViewer.GetComponent<UIPanel> ().sortingOrder = sortOrder;
	}

	public override void Close ()
	{
		if (mViewer != null) {
			GameObject.Destroy (mViewer.gameObject);
			mViewer = null;
        }
        BtnShowByteMask = 0xFF;
        this.onConfirmCb = null;
        this.onCancelCb = null;
        base.Close();
	}

	void RegisterEvents()
	{
		UIEventListener.Get (btnOk).onClick = HandleBtnClick;
		UIEventListener.Get (btnCancel).onClick = HandleBtnClick;
	}

	void HandleBtnClick(GameObject go)
	{
		if (go == btnOk) {
			onConfirmCb.TryCall ();
		} else if (go == btnCancel) {
			onCancelCb.TryCall ();
		}
		Close ();
	}

    public void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "lb_info":
                this.msgLabel = tf.GetComponent<UILabel>();
                break;
            case "btn_ok":
                this.btnOk = tf.gameObject;
                break;
            case "btn_cancel":
                this.btnCancel = tf.gameObject;
                break;
            case "spr_background":
                this.bgSprite = tf.GetComponent<UISprite>();
                break;
        }
    }
}
