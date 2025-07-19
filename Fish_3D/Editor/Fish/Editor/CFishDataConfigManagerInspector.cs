#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(CFishDataConfigManager))]

public class CFishDataConfigManagerInspector : Editor
{
    private CFishDataConfigManager                                          m_TargetData;

    public override void OnInspectorGUI()
    {
        m_TargetData = target as CFishDataConfigManager;

        if (GUILayout.Button("保存数据")) 
        {
            m_TargetData.SaveData();
        }
		if (Application.isEditor) 
		{
			if (CPathLinearRender.CurrentSelectedObj != null && CPathLinearRender.CurrentSelectedObj.IsAutoVisOnSelect)
				CPathLinearRender.CurrentSelectedObj.gameObject.SetActive(false);
			
			if (CPathGroupEditor.currentSelected != null)
				CPathGroupEditor.currentSelected.SetPathsVisble (false);

			if (COpeningPatternElementRender.mElemRender != null)
				COpeningPatternElementRender.mElemRender.ClearFish ();

			if (CGroupDataEditor.CurrentEditor != null)
				CGroupDataEditor.CurrentEditor.ClearFish ();
			
			CPathLinearRender.CurrentSelectedObj = null;
			CPathGroupEditor.currentSelected = null;
			COpeningPatternElementRender.mElemRender = null;
			CGroupDataEditor.CurrentEditor = null;

		}
    }
}
#endif