using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UI3D模型管理
/// </summary>
public class UI3DModelManager : MonoBehaviour {
    public static List<UIPanel> mPanelList = new List<UIPanel>();

    public void Awake() {
        mPanelList.Add(this.GetComponent<UIPanel>());
        UI3DModelManager.SortPanel();
    }

    public void OnDestroy() {
        mPanelList.Remove(this.GetComponent<UIPanel>());
        UI3DModelManager.SortPanel();
    }



    public static void AddUIPanel(GameObject obj) {
        if (obj.GetComponent<UIPanel>() == null) {
            Debug.LogError("找不到panel");
            return;
        }
        UI3DModelManager mgr = obj.GetComponent<UI3DModelManager>();
        if (mgr == null) {
            mgr = obj.AddComponent<UI3DModelManager>();
        }
    }

    public static void SortPanel() {//开始排列
        int count = mPanelList.Count;
        UIPanel t;
        for (int i = 0; i < count; i++) {
            for (int j = i+1; j < count; j++) {
                if (mPanelList[i].sortingOrder == mPanelList[j].sortingOrder) {
                    if (mPanelList[i].depth == mPanelList[j].depth) {
                        if (mPanelList[i].transform.GetSiblingIndex() > mPanelList[j].transform.GetSiblingIndex()) {
                            t = mPanelList[i];
                            mPanelList[i] = mPanelList[j];
                            mPanelList[j] = t;
                        }
                    }else if (mPanelList[i].depth > mPanelList[j].depth) {
                        t = mPanelList[i];
                        mPanelList[i] = mPanelList[j];
                        mPanelList[j] = t;
                    }
                } else {
                    if (mPanelList[i].sortingOrder > mPanelList[j].sortingOrder) {
                        t = mPanelList[i];
                        mPanelList[i] = mPanelList[j];
                        mPanelList[j] = t;
                    }
                }
            }

            Vector3 pos = mPanelList[i].transform.localPosition;
            pos.z = -1000 * i;
            mPanelList[i].transform.localPosition = pos;
        }
    }
}
