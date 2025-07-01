#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CFishPathEditor))]
public class CFishPathEditorInspector : Editor 
{
    CFishPathEditor                                                 m_TargetObject;

    public override void OnInspectorGUI()
    {
        m_TargetObject = target as CFishPathEditor;

        EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Config Path:");
		m_TargetObject.ConfigPath = EditorGUILayout.TextField(m_TargetObject.ConfigPath, GUILayout.Width(400));
		EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("ParadePath:");
        //m_TargetObject.FishParadeDataFile = EditorGUILayout.TextField(m_TargetObject.FishParadeDataFile);
        //EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("BossPathsFile:");
		m_TargetObject.BossPathsFile = EditorGUILayout.TextField(m_TargetObject.BossPathsFile);
		EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OpeningParadeFile:");
		m_TargetObject.OpeningParadeFile = EditorGUILayout.TextField(m_TargetObject.OpeningParadeFile, GUILayout.Width(400));
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		SerializedProperty testPathData = serializedObject.FindProperty("testPathData");
		EditorGUILayout.PropertyField(testPathData, true);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		SerializedProperty sphereGo = serializedObject.FindProperty("sphereGo");
		EditorGUILayout.PropertyField(sphereGo, true);
		EditorGUILayout.EndHorizontal();

        SerializedProperty isRefreshEditorModeSP = serializedObject.FindProperty("IsRefreshEditorMode");
        EditorGUILayout.PropertyField(isRefreshEditorModeSP, true);
        serializedObject.ApplyModifiedProperties();

		if (GUILayout.Button ("添加单条路径")) {
			Transform singleTrans = CFishPathEditor.only.transform.GetChild (0).GetChild (0);
			int pathidx = singleTrans.childCount;
			CFishPathEditor.only.CreateNewPathForGroup (singleTrans, pathidx);
		}

        EditorGUILayout.EndVertical();
		if (Application.isEditor && m_TargetObject.transform.childCount == 0) {
			m_TargetObject.IsRefreshEditorMode = true;
			m_TargetObject.EditorPathMode ();
		}

        if (m_TargetObject.IsRefreshEditorMode)
        {
            m_TargetObject.EditorPathMode();

            m_TargetObject.IsRefreshEditorMode = false;
        }
    }
}
#endif