using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
/*
 * 鱼群路径组配置编辑器
 * (GroupDataList中FishPathGroupData不为空时,
 *  根据FishPathGroupData中的PathGroupIndex读取路径组)
 */

public class CPathGroupDataEditor : MonoBehaviour
{
	public FishPathGroupData                                        ParadePathGroupData;

    private List<CPathLinearRender>                                 m_PathLinearRenderList = new List<CPathLinearRender>();

    public bool                                                     IsEditorMode = true;

    //临时
    public float                                                    m_PostLaunch = 3.0f;

	public bool isInversePath=false;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	public void GeneratePath()
    {
		PathLinearInterpolator[] piList = FishPathSetting.GetPathGroup(ParadePathGroupData.PathGroupIndex, isInversePath);
        if (piList != null)
        {
			while (m_PathLinearRenderList.Count > 0) {
				GameObject.Destroy (m_PathLinearRenderList [0].gameObject);
				m_PathLinearRenderList.RemoveAt (0);
			}

            m_PathLinearRenderList.Clear();
            for (int i = 0; i < piList.Length; ++i)
            {
                GameObject pathGO = new GameObject("Path" + i);
                CPathLinearRender pathLinearRender = pathGO.AddComponent<CPathLinearRender>();
                pathLinearRender.PathData = piList[i];
				pathLinearRender.FishPathGroupData = ParadePathGroupData;
                pathGO.transform.SetParent(transform);
                pathGO.transform.position = Vector3.zero;
                m_PathLinearRenderList.Add(pathLinearRender);
				pathGO.gameObject.SetActive (false);
            }
        }
    }

	public void LaunchAllFish() 
    {
        foreach (CPathLinearRender pathLinearRender in m_PathLinearRenderList) 
        {
			pathLinearRender.LaunchFish(ParadePathGroupData);
        }
    }

    public void RefreshPathData() 
    {
        foreach (CPathLinearRender pathLinearRender in m_PathLinearRenderList)
        {
			pathLinearRender.FishPathGroupData = ParadePathGroupData;

            pathLinearRender.m_PostLaunch = m_PostLaunch;
        }
    }
}
#endif