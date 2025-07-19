using UnityEngine;
using System.Collections;

/// <summary>
/// 帮助界面相关节点引用
/// </summary>
public class UI_TuJianRef : MonoBehaviour {
    public GameObject mObjBtnClose;//关闭按钮
    public GameObject[] mMenus = new GameObject[3];//菜单按钮
    public GameObject[] mMenuSelects = new GameObject[3];//菜单选中状态
    public GameObject mItemFishPuTong;//普通鱼
    public GameObject mItemFishTeShu;//特殊鱼
    public GameObject mItemLauncher;//炮台
    public UIScrollView mScrollViewInfo;
    public UIGrid mGrid;
}
