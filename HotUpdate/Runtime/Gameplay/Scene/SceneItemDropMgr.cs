using System;
using UnityEngine;
using System.Collections.Generic;

public class SceneItemDropMgr : ISceneMgr
{
	public void Init()
	{
			
	}

	public void Update (float delta)
	{
		for (int i = 0; i < m_fishcards.Count;) {
			if (!m_fishcards [i].Update (delta)) {
				m_fishcards [i].Destroy ();
				Utility.ListRemoveAt (m_fishcards, i);
			} else
				++i;
		}
	}

	public void Shutdown()
	{
		for (int i = 0; i < m_fishcards.Count; i++) {
			m_fishcards [i].Destroy ();
		}
		m_fishcards.Clear ();
	}

	List<DropItem> m_fishcards = new List<DropItem>();  //飞行的卡片
    public void MoveDropItem(Vector3 fromUIPos, ItemsVo itemvo, uint itemCnt)
	{
		if (itemvo.ItemType == (byte)EnumItemType.EngeryLCR)
			return;
        //Vector3 endPos = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.SeatTransform.TransformPoint(0,80,0);//飞向炮台
        Vector3 endPos = SceneLogic.Instance.LogicUI.GetItemIconWorldPos(itemvo.CfgID);//飞向道具栏
        if (endPos == Vector3.zero) {
			var lcr = SceneLogic.Instance.PlayerMgr.MySelf.Launcher;
			endPos = lcr.SeatTransform.TransformPoint(lcr.CanonBaseLocalPos.x, 80, 0);//飞向炮台
        }

        ResManager.LoadAsyn<GameObject>(ResPath.NewUIPath + "SceneItemDropUI", (ab_data, obj) => {
            GameObject goCard = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
            goCard.AddComponent<ResCount>().ab_info = ab_data;
            goCard.transform.localPosition = Vector3.zero;
            DropItem dropItem = new DropItem(goCard, fromUIPos, endPos, itemvo, itemCnt);
            dropItem.Start();
            m_fishcards.Add(dropItem);
        }, GameEnum.Fish_3D);
	}
}

class DropItem
{
	private GameObject mPanelGo, mItemGo;

	private CLineMove m_path;
	float m_fShowDelay = 0f;
	UILabel goldLabel;
	Vector3 startWorldPos, endWorldpos;
	bool isruning = false;
	ItemsVo mItemVo;
	GameObject effGo;
	public DropItem(GameObject goCard, Vector3 fromPos, Vector3 vEnd, ItemsVo itemvo, uint itemCtn)
	{
		uint itemCount = itemCtn;
		m_fShowDelay = 0f;
		mItemVo = itemvo;
		mPanelGo = goCard;
		startWorldPos = fromPos;
		endWorldpos = vEnd;
		mItemGo = goCard.transform.GetChild(0).gameObject;
		UISprite itemIcon = mItemGo.GetComponentInChildren<UISprite> ();
		itemIcon.spriteName = itemvo.ItemIcon;
		goldLabel = mPanelGo.GetComponentInChildren<UILabel> ();
		goldLabel.text = string.Format("{0} x{1}", StringTable.GetString(mItemVo.ItemName), itemCount);
		effGo = goCard.transform.GetChild (2).gameObject;
		isruning = false;

	}

//	float duration = 1f;
	bool isFinished = false;
	public void Start()
	{
		isFinished = false;
		float height = 100f;
		mPanelGo.transform.position = startWorldPos;
		mItemGo.transform.localScale = Vector3.zero;
		goldLabel.transform.localScale = Vector3.zero;

		iTween.ScaleTo (goldLabel.gameObject, iTween.Hash("scale", Vector3.one, "time", 1.5f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo (mItemGo, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveFrom (mItemGo, iTween.Hash("position", Vector3.up*height, "time",1f, "islocal",true, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo(mItemGo, iTween.Hash("position", endWorldpos, "time",1f, "islocal",false, "easetype", iTween.EaseType.easeInBack,"delay", 1.5f));	
		iTween.FadeTo(goldLabel.gameObject, iTween.Hash("alpha",0, "time", 0.5f, "delay", 1.5f));
		iTween.FadeTo(mItemGo, iTween.Hash("alpha",0f, "time", 0.5f, "oncomplete", "OnCallback", "delay", 2.5f));
		ItweenSetColorImp.Add (mItemGo);
		ItweenSetColorImp.Add (goldLabel.gameObject);

//		duration = 3f;
		isruning = true;
		MonMsgCallback.Add_OnCallback (mItemGo, delegate() {
			isFinished = true;
		});

	}

	public bool Update(float delta)
	{
		if (!isruning)
			return true;
		
		m_fShowDelay += delta;
		if (isFinished) {
            GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEffGetGold, SceneLogic.Instance.LogicUI.BattleUI);
            obj.transform.localPosition = SceneLogic.Instance.LogicUI.BattleUI.InverseTransformPoint(this.endWorldpos);
            //GameUtils.SetPSRenderQueue(obj, 1, 3333);
            GameObject.Destroy(obj, 3f);
			isruning = false;
			return false;
		}
		effGo.transform.position = mItemGo.transform.position;
		return true;
	}

	public void Destroy()
	{
		if (mPanelGo != null)
		{
			iTween[] its = mPanelGo.GetComponentsInChildren<iTween> ();
			for (int i = 0; i < its.Length; i++) {
				 UnityEngine.Object.Destroy (its [i]);
			}
			GameObject.Destroy(mPanelGo);
			mPanelGo = null;
		}
	}
}