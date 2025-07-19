using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class RollMsgData
{
	public RollMsgData(string str){
		data = str;
	}
	public string data;
}

public class ScrollMsgItem
{
	public GameObject msgGO;
	float duration = 1f;
	float time = 0f, delay = 0f;
	public UILabel msgLabel;
	public UIPanel maskPanel;
	bool misActive = false;

	bool needRollingShow = false;
	float mShowMaxwidth = 0f;
	float rollRunningTime = 0f, moveSpeed = 30f;
	Vector3 fromPos, targetPos;
	float rollingDuration = 0f;
	public void Init(string msg, float time, float delay = 0f)
	{
		maskPanel = msgGO.transform.GetChild (0).GetComponent<UIPanel> ();
		maskPanel.depth = msgGO.GetComponentInParent<UIPanel> ().depth + 1;
		msgLabel = maskPanel.transform.GetChild (0).GetComponent<UILabel> ();
		this.duration = time;
		this.delay = delay;
		this.time = 0f;
		msgLabel.text = msg;
		mShowMaxwidth = (int)maskPanel.finalClipRegion.z;
		msgLabel.width = (int)calMsgLength(msg);
		msgGO.SetActive (false);
		needRollingShow = (msgLabel.width > mShowMaxwidth);
		if (needRollingShow) {
			rollingDuration = (msgLabel.width - mShowMaxwidth) / FishConfig.Instance.GameSettingConf.RollingMessageSpeed;
			rollRunningTime = 0f;
			fromPos = Vector3.right * msgLabel.width * 0.5f;
			targetPos = Vector3.left * ((msgLabel.width - mShowMaxwidth) * 0.5f + 30f);
			msgLabel.transform.localPosition = fromPos;

		} else {
			msgLabel.transform.localPosition = Vector3.zero;
		}
	}

	static UILabel freelyLabel = null;
	float calMsgLength(string msg){
		if (msg.Length <= 45) {
			return mShowMaxwidth-10;
		}

		if (freelyLabel == null) {
			var freelyLabelGo = new GameObject ("freelyLabel");
			freelyLabelGo.hideFlags = HideFlags.HideAndDontSave;
            freelyLabelGo.layer = msgGO.layer;
			freelyLabelGo.transform.SetParent (msgGO.transform);
			freelyLabelGo.transform.position = Vector3.one * 1000f;
			freelyLabel = freelyLabelGo.AddComponent<UILabel> ();
			freelyLabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			freelyLabel.bitmapFont = msgLabel.bitmapFont;
			freelyLabel.trueTypeFont = msgLabel.trueTypeFont;
			freelyLabel.fontSize = 30;
		}
		freelyLabel.text = msg;
		return freelyLabel.width+10f;
	}

	public void SetTargetPos(Vector3 targetpos, Vector3 sc) {
        TweenPosition.Begin(msgGO, 0.5f, targetpos, false);
        TweenScale.Begin(msgGO, 0.5f, sc);
	}

	public bool IsActive
	{
		get { return misActive;}
		set { misActive = value;}
	}

	float staytime = 0f;
	public bool Update(float delta)
	{
		if (msgGO == null)
			return false;
		
		time += delta;
		if (time < delay) {
			return true;
		}
		float percent = 1f;
		if (needRollingShow) {
			rollRunningTime += delta;
			percent = rollRunningTime / rollingDuration;
			msgLabel.transform.localPosition = Vector3.Lerp (fromPos, targetPos, percent);
		}
		msgGO.SetActive (true);
		if (time < duration)
		{
			staytime = 0f;
			return true;
		}
		if (needRollingShow) {
			if (percent >= 0.9990f) {
				staytime += delta;
				if (staytime > 2f)
					return false;
			}
			return true;
		} else {
			return false;
		}
	}
}

public class ScrollingMessageUIController : IUIControllerImp
{
	const int MaxMsgItem = 2;
	const float RollMSG_SHOW_Interval = 5f;
	float startYPos = 370f;
	Queue<RollMsgData> msgDataList = new Queue<RollMsgData>();

	GameObject RollMsgItemPrefab = null;
	IEnumerator runMsgCoroutine = null;
	bool isRuning = false;
	public ScrollingMessageUIController ()
	{
		runMsgCoroutine = UpdateMssage ();

	}

	public override bool CanShow { get{ return false;}}

	bool isloadingUI = false;
	public void PushRollMsg(string data)
	{
		if (!string.IsNullOrEmpty(data)) {
			msgDataList.Enqueue (new RollMsgData (data.ToString()));
		}
		if (!isloadingUI) {
			isloadingUI = true;	
			//WndManager.LoadUIGameObject("RollMessageUI", delegate(GameObject obj) {
			//		RollMsgItemPrefab = obj;
			//	});

            GameObject prefabs = Resources.Load<GameObject>("RollMessageUI");
            RollMsgItemPrefab = prefabs;
        }

		if (!isRuning) {
			activeItems.Clear ();
			not_use_items.Clear ();
			MonoDelegate.Instance.StartCoroutine (runMsgCoroutine);
		}

	}

	public override void Close ()
	{
		MonoDelegate.Instance.StopCoroutine (runMsgCoroutine);
		isRuning = false;
		base.Close ();
		for (int i = 0; i < activeItems.Count; i++) {
			if (activeItems [i].msgGO != null) {
				GameObject.Destroy(activeItems[i].msgGO);
			}
		}

		for (int i = 0; i < not_use_items.Count; i++) {
			if (not_use_items [i].msgGO != null) {
				GameObject.Destroy(not_use_items[i].msgGO);
			}
		}

		activeItems.Clear ();
		not_use_items.Clear ();
	}

	float delayInterval = 0.50f;
	float ItemHeight = 54f;
	List<ScrollMsgItem> activeItems = new List<ScrollMsgItem>();
	List<ScrollMsgItem> not_use_items = new List<ScrollMsgItem>();

	IEnumerator UpdateMssage() {       
        isRuning = true;
        for (int i = 0; i < MaxMsgItem; i++) {
            not_use_items.Add(new ScrollMsgItem());
        }


        while (RollMsgItemPrefab == null) {
            yield return null;
        }

        float next_show_time = 0;
		while (true) {
			float delta = Time.deltaTime;
            for (int i = activeItems.Count - 1; i >= 0; i--) {
                if (activeItems[i].Update(delta) == false) {
                    activeItems[i].IsActive = false;
                    activeItems[i].msgGO.SetActive(false);
                    not_use_items.Add(activeItems[i]);
                    activeItems.RemoveAt(i);
                }
            }

            if (not_use_items.Count > 0) {
                if (msgDataList.Count > 0 && Time.realtimeSinceStartup > next_show_time) {
                    RollMsgData msgdata = msgDataList.Dequeue();
                    ScrollMsgItem item = not_use_items[0];
                    SetMsgData(item, msgdata, 0);
                    activeItems.Insert(0,item);
                    not_use_items.RemoveAt(0);

                    next_show_time = Time.realtimeSinceStartup + delayInterval;

                    int activeCnt = activeItems.Count;
                    for (int i = 0; i < activeCnt; i++) {
                        float ypos = startYPos + i * ItemHeight;
                        activeItems[i].SetTargetPos(Vector3.up * ypos, Vector3.one * (1f - i * 0.2f));
                        activeItems[i].IsActive = true;
                    }
                }
            }
			yield return null;
		}
	}
	
	void SetMsgData(ScrollMsgItem msgItem, RollMsgData data, float delay)
	{
		if (msgItem.msgGO == null)
            msgItem.msgGO = GameUtils.CreateGo(RollMsgItemPrefab, SceneObjMgr.Instance.UIContainerTransform);
        msgItem.msgGO.transform.localPosition = Vector3.up * startYPos;
        msgItem.msgGO.transform.localScale = Vector3.one*0.5f;

		msgItem.Init (data.data, RollMSG_SHOW_Interval, delay);
	}
}
