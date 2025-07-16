#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(CPathGroupEditor))]

public class CPathGroupEditorInspector : Editor 
{
    private CPathGroupEditor                                m_TargetData;
    public override void OnInspectorGUI()
    {
        m_TargetData = target as CPathGroupEditor;

        //if (!m_TargetData.IsRefresh) return;

        EditorGUILayout.BeginVertical();
        if (m_TargetData.m_Data != null)
        {
            SerializedProperty pathlinearInterpolatorArraySP = serializedObject.FindProperty("m_Data");
            EditorGUILayout.PropertyField(pathlinearInterpolatorArraySP, true);
            /*
            for (int i = 0; i < m_TargetData.m_Data.Length; ++i) 
            {
                SerializedProperty piSP = serializedObject.FindProperty("m_Data[" + i + "]");
                EditorGUILayout.PropertyField(piSP, true);
            }*/
        }
        serializedObject.ApplyModifiedProperties();
		if (CPathGroupEditor.currentSelected != m_TargetData) {

			m_TargetData.SetPathsVisble (true);
			if (CPathGroupEditor.currentSelected != null)
				CPathGroupEditor.currentSelected.SetPathsVisble (false);
		}
		CPathGroupEditor.currentSelected = m_TargetData;

        if (GUILayout.Button("为该路径组添加一条新路径")) 
        {
            CFishPathEditor mainEditor = m_TargetData.GetComponentInParent<CFishPathEditor>();

            if (null != mainEditor) 
            {
                CPathLinearRender newPathLinearRender = mainEditor.CreateNewPathForGroup(m_TargetData.transform, m_TargetData.transform.childCount);
                PathLinearInterpolator[] tmp = new PathLinearInterpolator[m_TargetData.m_Data.Length + 1];
                Array.Copy(m_TargetData.m_Data, 0, tmp, 0, m_TargetData.m_Data.Length);
				tmp[tmp.Length - 1] = newPathLinearRender.PathData;
                m_TargetData.m_Data = tmp;
                m_TargetData.IsRefresh = true;
            }
        }

        EditorGUILayout.EndVertical();

        m_TargetData.IsRefresh = false;
    }
}
#endif