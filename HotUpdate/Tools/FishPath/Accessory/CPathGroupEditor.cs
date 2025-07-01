using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
/*
 路径组配置编辑器类
 */

public class CPathGroupEditor : MonoBehaviour 
{
	public const string FishParadeDataFile = "./Assets/Tools/FishPath/FishPath.byte";
	public static CPathGroupEditor currentSelected = null;
    public PathLinearInterpolator[]                                 m_Data;
    public bool                                                     IsRefresh = true;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

    public void SetData(PathLinearInterpolator[] data) 
    {
        m_Data = new PathLinearInterpolator[data.Length];

        for (int i = 0; i < data.Length; ++i) 
        {
            m_Data[i] = data[i];
        }
    }

	public void SetPathsVisble(bool vis)
	{
		CFishPathEditor.SettSinglePathOther ();
		Transform mt = this.transform;
		for (int i = 0; i < mt.childCount; i++) 
		{
			Transform t = mt.GetChild (i);

			CPathLinearRender pR = t.GetComponent<CPathLinearRender> ();
			if (vis) {
				pR.RefreshData ();
				pR.IsAutoVisOnSelect = false;
				pR.gameObject.SetActive (true);

			} else {
				pR.IsAutoVisOnSelect = true;
				pR.gameObject.SetActive (false);
			}
		}
	}
}
#endif