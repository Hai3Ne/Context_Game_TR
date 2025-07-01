#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(COpeningPatternRender))]
public class COpeningPatternRenderInspector : Editor 
{
	const string ITEM_NAME = "Parade";
    //int selectIndex = 0;
    COpeningPatternRender m_TargetData;

	public override void OnInspectorGUI()
	{
		m_TargetData = target as COpeningPatternRender;
		EditorGUILayout.BeginVertical ();
		if (Application.isEditor) {
			int i = m_TargetData.transform.childCount;
			if (GUILayout.Button ("Add")) {
				GameObject go = new GameObject (ITEM_NAME + "" + i);
				COpeningPatternElementRender patternElem = go.AddComponent<COpeningPatternElementRender> ();
				go.transform.SetParent (m_TargetData.transform);
				patternElem.m_OpeningParadeData = new OpeningParadeData ();
				patternElem.m_OpeningParadeData.mFishParade = new FishParadeData ();
				patternElem.m_OpeningParadeData.mFishParade.GroupDataArray = new GroupData[0];
				patternElem.m_OpeningParadeData.mFishParade.PathList = new uint[0];


//				CFishArrayGraphicsRender paradeElem = go.AddComponent<CFishArrayGraphicsRender>();
//				paradeElem.m_FishArrayGraphics = new FishArrayGraphics();
//				paradeElem.m_FishArrayGraphics.triangleFishArray = new TriangleFishArrayGraphics();
//				paradeElem.m_FishArrayGraphics.rectangleFishArray = new RectangleFishArrayGraphics();
//				paradeElem.m_FishArrayGraphics.ovalFishArray = new OvalFishArrayGraphics();
				Selection.activeGameObject = patternElem.gameObject;
			}


			EditorGUILayout.Space ();

			if (GUILayout.Button ("Refresh")) {
				
			}

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (Application.isPlaying)
			{
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Test")) 
				{
					m_TargetData.TestLaunchFishes ();
				}
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Clear fish")) {
					CFishPathEditor.ClearAllFish ();
				}
				EditorGUILayout.EndHorizontal ();
			}
		}

		EditorGUILayout.EndVertical ();
	}
}
#endif