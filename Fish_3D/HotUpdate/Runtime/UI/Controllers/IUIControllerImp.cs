using System;
using UnityEngine;

public class IUIControllerImp : IUIController
{
	protected bool mIsActive = false;
	protected object mData = null;
	protected GameObject uiRefGo = null;
	protected EnumPanelType mPanelType = EnumPanelType.CenterPopUI;
	public virtual void Init (object data)
	{
		mData = data;
	}

    public virtual EnumPanelPRI PRI {
		get { return EnumPanelPRI.Low;}
	}

	public virtual void Show ()
	{
		mIsActive = true;
        if (LogMgr.ShowLog) {
            LogMgr.Log(this.ToString() + " UI Show.");
        }
	}

	protected bool TweenShow(System.Action cb = null){
		if (uiRefGo == null)
			return false;
		Transform t0 = uiRefGo.transform.Find ("bg");
		Transform t1 = uiRefGo.transform.Find ("scale");
		if (t0 != null && t1 != null) {
			float dura = GameParams.Instance.panelTweenTime;
            t1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            var ts = TweenScale.Begin(t1.gameObject, dura, Vector3.one);
            ts.animationCurve = GameParams.Instance.uiPanelshowCure;
            ts.SetOnFinished(delegate {
                cb.TryCall();
            });
			t0.GetComponent<UISprite> ().alpha = 0f;
			TweenAlpha.Begin (t0.gameObject, dura, 1f);
			return true;
		}
		return false;
	}

	public virtual EnumPanelType PanelType{ get { return mPanelType; }}
	public virtual bool CanShow { get{ return true;}}
	public virtual bool IsActive{ get{ return mIsActive;} }

	public virtual void Close ()
	{
        if (LogMgr.ShowLog) {
            LogMgr.Log(this.ToString() + " UI Close.");
        }
		mIsActive = false;
		if (uiRefGo != null) {
			WndManager.Instance.Pop(uiRefGo);
		}
		uiRefGo = null;
		mOnCloseEvent.TryCall (this);
	}

	Action<IUIController> mOnCloseEvent;
	public event Action<IUIController> OnCloseEvent 
	{
		add { mOnCloseEvent += value; }
		remove { mOnCloseEvent -= value; }
	}

	public object BindData{ get; set;}
}