using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 鱼群组配置编辑器
 * (GroupDataList中PathGroupData为空时
 * 读取鱼群组GroupDataArray)
 */
#if UNITY_EDITOR
public class CGroupDataEditor : BaseFishParadeEditor 
{
	public static CGroupDataEditor CurrentEditor;
	public FishParadeData m_FishParadeData;

    public ushort                                           m_PathIndex; //路径配置id 对应FishPathConfData的m_PathInterpList

    private List<GameObject>                                m_PathList = new List<GameObject>();

    public bool                                             IsEditorMode = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

    public void RefreshPath() 
    {
        foreach (GameObject go in m_PathList) 
        {
            DestroyImmediate(go);
        }
        m_PathList.Clear();
    }
}
#endif