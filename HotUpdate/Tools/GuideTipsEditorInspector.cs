using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(GuideTipsEditor))]
public class GuideTipsEditorInspector : Editor {

	GuideTipsEditor m_TargetObject;
	public override void OnInspectorGUI()
	{
		m_TargetObject = target as GuideTipsEditor;
		if (m_TargetObject.mGuideTipDataList == null || m_TargetObject.mGuideTipDataList.Count == 0)
			m_TargetObject.Init ();
		GuideTipData tipData = m_TargetObject.mGuideTipDataList [m_TargetObject.currentTipIndex];
		EditorGUILayout.BeginVertical ();

		EditorGUILayout.LabelField ("MaxStep:" + m_TargetObject.mGuideTipDataList.Count);
		EditorGUILayout.LabelField ("StepIdx:" + m_TargetObject.currentTipIndex);

		EditorGUILayout.Space ();
		tipData.ArrowRange = EditorGUILayout.Slider (tipData.ArrowRange, 0f, 1f);
		tipData.ArrowDir = (int)(EnumArrowDir)EditorGUILayout.EnumPopup ((EnumArrowDir)tipData.ArrowDir);
		tipData.textMaxSize = EditorGUILayout.Vector2Field ("TextSize:", tipData.textMaxSize);


		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Prev")) {
			m_TargetObject.currentTipIndex = Mathf.Max (m_TargetObject.currentTipIndex - 1, 0);
		}

		if (GUILayout.Button ("Next")) {
			m_TargetObject.currentTipIndex = Mathf.Min (m_TargetObject.currentTipIndex + 1, m_TargetObject.mGuideTipDataList.Count - 1);
		}

		EditorGUILayout.Space ();
		if (GUILayout.Button ("Insert")) {
			GuideTipData sdata = new GuideTipData ();
			m_TargetObject.mGuideTipDataList.Insert (m_TargetObject.currentTipIndex, sdata);
		}

		if (m_TargetObject.mGuideTipDataList.Count > 2) {
			if (GUILayout.Button ("Delete")) {
				int idx = m_TargetObject.currentTipIndex;
				if (m_TargetObject.currentTipIndex == m_TargetObject.mGuideTipDataList.Count - 1)
					m_TargetObject.currentTipIndex = m_TargetObject.currentTipIndex - 1;
				m_TargetObject.mGuideTipDataList.RemoveAt (idx);
			}
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.EndVertical ();
	}
}


#endif