using UnityEngine;
using System.Collections;

/// <summary>
/// 获取奖励节点引用
/// </summary>
public class UI_GetAwardRef : MonoBehaviour {
    public GameObject mBtnBG;//背景按钮
    public GameObject mObjItemList;
	public UIGrid mGridItem,mGridItem2;
    public Item_GetAward[] mItemAwards = new Item_GetAward[5];
	public Item_GetAward[] mItemAwards2 = new Item_GetAward[5];
    public UILabel mLbTick;

	public UIWidget container;
	int childListCnt = 0,childListCnt2 = 0;
	void Awake(){
		childListCnt = 0;
		childListCnt2 = 0;
	}

	public void UpdateSize(){
		int maxWidth = 0, maxHeight = 100, n;
		if (childListCnt != mGridItem.GetChildList ().Count) {
			n = Mathf.Max (4, mGridItem.GetChildList ().Count);
			maxWidth = Mathf.Max ((int)(mGridItem.cellWidth * n), maxWidth);
			childListCnt = mGridItem.GetChildList ().Count;
		}

		if (mGridItem2.gameObject.activeSelf && childListCnt2 != mGridItem2.GetChildList ().Count) {
			n = Mathf.Max (4, mGridItem2.GetChildList ().Count);
			maxWidth = Mathf.Max ((int)(mGridItem2.cellWidth * n), maxWidth);
			childListCnt2 = mGridItem2.GetChildList ().Count;
		}
		if (mGridItem2.gameObject.activeSelf)
			maxHeight = (int)(mGridItem2.cellHeight * 2);
		else
			maxHeight = (int)mGridItem2.cellHeight;
		
		if (maxWidth > 0 && maxWidth != container.width)
			container.width = maxWidth;
		if (maxHeight > 0 && maxHeight != container.height) {
			container.height = maxHeight;
			var pg = NGUITools.FindInParents<UIGrid> (mGridItem.transform.parent.gameObject);
			if (pg != null) {
				pg.repositionNow = true;
				pg.Reposition ();
			}
		}
	}

	[ContextMenu("fsfe")]
	void updateBg(){
		UpdateSize ();

	}
}
