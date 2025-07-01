using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
public class COpeningPatternElementRender : BaseFishParadeEditor 
{
	public static COpeningPatternElementRender mElemRender;
	public OpeningParadeData m_OpeningParadeData;
	public bool fishPosChanged = false;
	public float[] delaysplitStr = new float[0];
	private List<GameObject>                                m_PathList = new List<GameObject>();

	public bool                                             IsEditorMode = true;

	// Use this for initialization
	void Start () {
	}

	[NonSerialized]
	public List<GameObject> cacheTestFishObjs = new List<GameObject>();
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