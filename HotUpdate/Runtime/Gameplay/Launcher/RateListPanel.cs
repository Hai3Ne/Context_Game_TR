using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RateListPanel
{
	public Action<uint> OnSelected;
	List<RateItemToggleUI> mItemViewList = new List<RateItemToggleUI>();
	UISprite rateBgSp;
	GameObject conTrans;
	GameObject itemPrefab;

	uint[] RateValues = new uint[0];
	uint selectedRateValue = 0;
	float cellHeight = 60f;//倍率高度


    public void Init(Transform con) {
        this.conTrans = con.gameObject;
        rateBgSp = this.conTrans.transform.GetChild(0).GetComponent<UISprite>();
        this.itemPrefab = this.conTrans.transform.GetChild(1).gameObject;
        this.itemPrefab.SetActive(false);
        conTrans.SetActive(false);

        //FishGameMode.AddListTap3Dscene(delegate {
        //    if (conTrans != null && conTrans.activeSelf) {
        //        Hide();
        //        return true;
        //    }
        //    return false;
        //});

        UpdateData();
    }

	void UpdateData()
	{
		RateValues = SceneLogic.Instance.FModel.GetAvaibleRateValuList ();
		if (RateValues.Length <= 1) {
			return;
		}
		if (currentSelectItem != null)
			currentSelectItem.toggleValue = false;
		
		RateItemToggleUI cellItem = null;
		int i = 0;
		Vector3 itemStartPos = new Vector3 (0f, -150f,0f);
		Vector3 itemPos = itemStartPos;
		for(i = 0; i < RateValues.Length; i++)
		{
			if (i < mItemViewList.Count)
				cellItem = mItemViewList [i];
			else {
                cellItem = GameUtils.CreateGo(itemPrefab, this.conTrans.transform).GetComponent<RateItemToggleUI>();
				UIEventListener.Get(cellItem.gameObject).onClick = HandleClickRateItem;
				UIEventListener.Get(cellItem.gameObject).onHover = HandleHoverRateItem;
				mItemViewList.Add (cellItem);
			}
			cellItem.transform.localPosition = itemPos;//new Vector3(0, cellHeight * (i - 4.5f));
			cellItem.valueLabel.text = string.Format ("x{0}", RateValues[i]);
			mItemViewList [i].toggleValue = RateValues[i] == selectedRateValue;
			if (mItemViewList [i].toggleValue)
				currentSelectItem = mItemViewList [i];
			itemPos += Vector3.up * cellHeight; 
		}

		while (i < mItemViewList.Count) {
			GameObject.Destroy (mItemViewList[i].gameObject);
			i++;
		}

		float itemTotalHeight = (itemPos.y + cellHeight * 0.5f) - itemStartPos.y;
		rateBgSp.height = (int)(itemTotalHeight + 6f);
	}

	public void Show(uint RateValue)
	{
		selectedRateValue = RateValue;
		UpdateData ();
		conTrans.SetActive (true);
	}

	public void Hide()
	{
		conTrans.SetActive (false);
	}

	public bool isActive
	{
        get { return conTrans != null && conTrans.activeSelf; }
	}

	void HandleClickRateItem(GameObject go)
	{
		RateItemToggleUI toggleui = go.GetComponent<RateItemToggleUI> ();
		int idx = mItemViewList.IndexOf(toggleui);
		OnSelected.TryCall (RateValues [idx]);
		Hide ();

	}

	RateItemToggleUI currentSelectItem = null;
	void HandleHoverRateItem(GameObject go, bool isHover)
	{
		if (isHover) {
			if (currentSelectItem != null)
				currentSelectItem.toggleValue = false;
			currentSelectItem = go.GetComponent<RateItemToggleUI> ();
			currentSelectItem.toggleValue = true;
		} else {
			if (currentSelectItem != null)
				currentSelectItem.toggleValue = false;
		}

	}
}
