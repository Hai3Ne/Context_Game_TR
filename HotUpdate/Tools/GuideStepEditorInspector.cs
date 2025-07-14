using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

public enum EnumArrowDir{ UP, Down, Left, Right}
[CustomEditor(typeof(GuideStepEditor))]
public class GuideStepEditorInspector : Editor {

	GuideStepEditor m_TargetObject;
	public override void OnInspectorGUI()
	{
		m_TargetObject = target as GuideStepEditor;
		if (m_TargetObject.mStepList == null || m_TargetObject.mStepList.Count == 0)
			m_TargetObject.Init ();
		GuideStepData stepData = m_TargetObject.mCurStepData;

        EditorGUILayout.BeginVertical();
        SerializedProperty seatData = serializedObject.FindProperty("seat");
        EditorGUILayout.PropertyField(seatData, true);

        GuideEventType _pre_event = m_TargetObject.EventType;
        SerializedProperty EventTypeData = serializedObject.FindProperty("EventType");
        EditorGUILayout.PropertyField(EventTypeData, true);
        if (_pre_event != m_TargetObject.EventType) {
            stepData.EventType = m_TargetObject.EventType;
        }

		EditorGUILayout.LabelField ("MaxStep:"+m_TargetObject.mStepList.Count);
		EditorGUILayout.LabelField ("StepIdx:"+m_TargetObject.currentStepIdx);
		if (Application.isPlaying) {
			stepData.msgContent = StringTable.GetString (string.Format ("GuideTips{0}", m_TargetObject.currentStepIdx));
		}
		stepData.msgContent = EditorGUILayout.TextField ("Msg:", stepData.msgContent);
		stepData.dialogPosition = EditorGUILayout.Vector3Field ("DLgPosition:", stepData.dialogPosition);
		stepData.fingerPos = EditorGUILayout.Vector3Field ("FingerPos:", stepData.fingerPos);
		stepData.holeRect = EditorGUILayout.Vector4Field ("HoleRect:",stepData.holeRect);
		stepData.ArrowRange = EditorGUILayout.Slider (stepData.ArrowRange, 0f, 1f);
		stepData.ArrowDir = (int)(EnumArrowDir)EditorGUILayout.EnumPopup ((EnumArrowDir)stepData.ArrowDir);
		stepData.textMaxSize = EditorGUILayout.Vector2Field ("TextSize:", stepData.textMaxSize);
		stepData.IsShowNPC = EditorGUILayout.Toggle ("IsShowNPC:",stepData.IsShowNPC);
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Prev")) {
            m_TargetObject.SetStep(m_TargetObject.currentStepIdx - 1);
		}

		if (GUILayout.Button ("Next")) {
            m_TargetObject.SetStep(m_TargetObject.currentStepIdx + 1);
		}

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Insert")) {
            m_TargetObject.Insert();
		}

		if (m_TargetObject.mStepList.Count > 2) {
			if (GUILayout.Button ("Delete")) {
                m_TargetObject.Remove();
			}
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();

		if (serializedObject.ApplyModifiedProperties ()) {
			m_TargetObject.UpdateData ();
		}
	}
}

#endif