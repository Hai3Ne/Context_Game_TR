#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(SPoint))]
public class SpointInspector : UnityEditor.Editor
{
	SPoint m_Target;

	public override void OnInspectorGUI()
	{
		m_Target = target as SPoint;
		if (m_Target.transform.parent == null)
			return;
		CPathLinearRender cpathR = m_Target.transform.parent.GetComponent<CPathLinearRender> ();
		bool isDeleted = false;
		if (cpathR != null) 
		{
			EditorGUILayout.BeginVertical ();
			SerializedProperty valSp = serializedObject.FindProperty ("val");
			EditorGUILayout.PropertyField (valSp, true);

			SerializedProperty NowPositionSp = serializedObject.FindProperty ("NowPosition");
			EditorGUILayout.PropertyField (NowPositionSp, true);

			SerializedProperty csp = serializedObject.FindProperty ("color");
			EditorGUILayout.PropertyField (csp, true);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			float angle = m_Target.forwardRot;
			m_Target.forwardRot = EditorGUILayout.Slider (m_Target.forwardRot, -1000f, 1000f);

			if (m_Target.transform.parent != null) {
				CPathLinearRender pathR = m_Target.transform.parent.GetComponent<CPathLinearRender> ();
				if (pathR != null) 
				{
					UnityEditor.Undo.RecordObject (m_Target, m_Target.transform.name);
					if (m_Target.forwardRot != angle)
						pathR.OnSPointRotationUpdate();
					
					if (pathR.isAutoAdjustPointDirection) {
						Vector3 RDir = m_Target.transform.InverseTransformVector (m_Target.transform.right);
						m_Target.transform.localRotation = m_Target.cachedRot * Quaternion.AngleAxis (m_Target.forwardRot, RDir);
					}
				}
			}

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("添加")) {
				cpathR.AddKeyPoint (m_Target.transform.GetSiblingIndex()+1);
			}

			EditorGUILayout.Space ();

			if (GUILayout.Button ("删除")) 
			{
				cpathR.RemoveKeyPoint (m_Target.transform.GetSiblingIndex());
				isDeleted = true;
			}

//			
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical ();
		} else {
			
			SerializedProperty csp = serializedObject.FindProperty ("color");
			EditorGUILayout.PropertyField (csp, true);

			SerializedProperty radius = serializedObject.FindProperty ("radius");
			EditorGUILayout.PropertyField (radius, true);

		}
		if (!isDeleted)
			serializedObject.ApplyModifiedProperties();
	}

}
#endif