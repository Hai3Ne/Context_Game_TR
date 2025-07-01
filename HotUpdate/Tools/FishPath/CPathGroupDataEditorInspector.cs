using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(CPathGroupDataEditor))]

public class CPathGroupDataEditorInspector : Editor 
{
	
    CPathGroupDataEditor                                                m_TargetData;

    public override void OnInspectorGUI() 
    {
        m_TargetData = target as CPathGroupDataEditor;
		var isInverse = m_TargetData.isInversePath;
		ushort oldIdx = m_TargetData.ParadePathGroupData.PathGroupIndex;
        EditorGUILayout.BeginVertical();
		SerializedProperty pathGroupIndexSP = serializedObject.FindProperty("ParadePathGroupData.PathGroupIndex");
		SerializedProperty speedSP = serializedObject.FindProperty("ParadePathGroupData.Speed");
		SerializedProperty fishCfgIDSP = serializedObject.FindProperty("ParadePathGroupData.FishCfgID");
		SerializedProperty fishScalingSP = serializedObject.FindProperty("ParadePathGroupData.FishScaling");
		SerializedProperty actionSpeedSP = serializedObject.FindProperty("ParadePathGroupData.ActionSpeed");
		SerializedProperty actionUniteSP = serializedObject.FindProperty("ParadePathGroupData.ActionUnite");
		SerializedProperty pathInverseSp = serializedObject.FindProperty("isInversePath");

        EditorGUILayout.PropertyField(pathGroupIndexSP);
        EditorGUILayout.PropertyField(speedSP);
        EditorGUILayout.PropertyField(fishCfgIDSP);
        EditorGUILayout.PropertyField(fishScalingSP);
        EditorGUILayout.PropertyField(actionSpeedSP);
        EditorGUILayout.PropertyField(actionUniteSP);
		EditorGUILayout.PropertyField(pathInverseSp);

        SerializedProperty postLaunchSP = serializedObject.FindProperty("m_PostLaunch");
        EditorGUILayout.PropertyField(postLaunchSP);
    
        serializedObject.ApplyModifiedProperties();

		if (m_TargetData.isInversePath != isInverse) {
			m_TargetData.GeneratePath ();
		}
		if (Application.isPlaying) //非编辑器模式(运行时)才使用以下功能
        {
            if (GUILayout.Button("全部出动"))
            {
                m_TargetData.LaunchAllFish();
            }

            if (GUILayout.Button("清空所有鱼"))
            {
                CFishPathEditor.ClearAllFish();
            }
        }
        EditorGUILayout.EndVertical();
    }
}

#endif