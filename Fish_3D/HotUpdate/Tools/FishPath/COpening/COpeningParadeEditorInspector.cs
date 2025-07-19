#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(COpeningParadeEditor))]

public class COpeningParadeEditorInspector : Editor 
{
	COpeningParadeEditor                                                m_TargetData;
	public override void OnInspectorGUI()
	{
		m_TargetData = target as COpeningParadeEditor;

		EditorGUILayout.BeginVertical ();
		if (Application.isEditor) 
		{
			int i = m_TargetData.transform.childCount;
			if (GUILayout.Button ("Add")) {
				GameObject go = new GameObject (CFishPathEditor.OPENING_FISH_PARADE_ITEM_NAME+""+i);
				COpeningPatternRender pattern = go.AddComponent<COpeningPatternRender> ();
				go.transform.SetParent (m_TargetData.transform);
				Selection.activeGameObject = go;
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Save")) {
				m_TargetData.saveData ();
				EditorUtility.DisplayDialog("保存成功","保存成功.", "好");
			}
		}
		EditorGUILayout.EndVertical ();
	}
}
#endif