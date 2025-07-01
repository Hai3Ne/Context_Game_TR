#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(COpeningPatternElementRender))]
public class COpeningPatternElementRenderInspector : Editor 
{
	COpeningPatternElementRender mTarget;


	public override void OnInspectorGUI()
	{
		
		bool hasChanged = false;
		mTarget = target as COpeningPatternElementRender;
		mTarget.GetPoslist (mTarget.m_OpeningParadeData.mFishParade);
		EditorGUILayout.BeginVertical();

		float[] delaysplitStr = mTarget.delaysplitStr;
		int groupLen = mTarget.m_OpeningParadeData.mFishParade.GroupDataArray.Length;

		float[] tmp = new float[groupLen];
		if (delaysplitStr.Length != groupLen) {
			Array.Copy (delaysplitStr, 0, tmp, 0, Mathf.Min (tmp.Length, delaysplitStr.Length));
			mTarget.delaysplitStr = tmp;
			delaysplitStr = tmp;
		}

		for (int i = 0; i < groupLen; i++) 
		{
			EditorGUILayout.BeginHorizontal ();
			delaysplitStr[i] = EditorGUILayout.FloatField(delaysplitStr[i]);
			if (GUILayout.Button("["+i + "] Arrange")) 
			{
				float delaysplit = delaysplitStr [i];
				float fval = delaysplit;
				for (int j = 0; j < mTarget.m_OpeningParadeData.mFishParade.GroupDataArray[i].DelayList.Length; j++)
				{
					mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [i].DelayList [j] = fval;
					fval += delaysplit;
				}
			}
			EditorGUILayout.EndHorizontal ();
		}

		if (Application.isPlaying) {
			if (GUILayout.Button ("Test")) 
			{
				int id = int.Parse(mTarget.name.Replace ("Parade", ""));
				mTarget.GetComponentInParent<COpeningPatternRender> ().TestLaunchFishes(id);
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Clear fish")) {
                CFishPathEditor.ClearAllFish();
            }
		}

		if (Application.isEditor) {
			if (mTarget != COpeningPatternElementRender.mElemRender) {
				mTarget.UpdateSceneObjStatus (mTarget.m_OpeningParadeData.mFishParade);
				mTarget.CheckDataOK (mTarget.m_OpeningParadeData.mFishParade);
				if (COpeningPatternElementRender.mElemRender != null)
					COpeningPatternElementRender.mElemRender.ClearFish ();
			}
			COpeningPatternElementRender.mElemRender = mTarget;
		}
		for (int i = 0; i < mTarget.m_OpeningParadeData.mFishParade.GroupDataArray.Length; i++) 
		{
			ushort nnnn = (ushort)mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [i].PosList.Length;
			mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [i].FishNum = nnnn;
			float[] oldDels = mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [i].DelayList;
			float[] dels = new float[nnnn];
			Array.Copy (oldDels, 0, dels, 0, Mathf.Min(oldDels.Length, nnnn));
			mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [i].DelayList = dels;
		}
		SerializedProperty groupDataArraySP = serializedObject.FindProperty("m_OpeningParadeData");
		EditorGUILayout.PropertyField(groupDataArraySP, true);

		hasChanged = serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndVertical ();

		if (hasChanged)
		{
			mTarget.UpdateSceneObjStatus (mTarget.m_OpeningParadeData.mFishParade);
		}

		ShowFishGraphic ();
	}

	void ShowFishGraphic()
	{
		if (Application.isEditor) {

			bool isfirst = false;
			if (mTarget.mFishGraphic == null) {
				mTarget.mFishGraphic = FishGraphicFactory.Create ();
				mTarget.mFishGraphic.GraphicData = mTarget.m_OpeningParadeData.mFishParade.FishGraphicData;
			}
			
			if (mTarget.mFishGraphic != null) 
			{
				GUILayout.Space (14);
				GUILayout.Label ("————————————————————");

				EditorGUILayout.BeginVertical();

				EditorGUILayout.BeginHorizontal ();
				mTarget.mFishGraphic.IsActive = EditorGUILayout.Toggle (mTarget.mFishGraphic.IsActive);
				GUILayout.Space (8);
				GUILayout.Label ("GraphicType:");
				mTarget.mFishGraphic.graphictype = (EnuGraphicType)EditorGUILayout.EnumPopup (mTarget.mFishGraphic.graphictype);
				EditorGUILayout.EndHorizontal ();
				if (mTarget.mFishGraphic.IsActive) {

					GroupData groupData = mTarget.m_OpeningParadeData.mFishParade.GroupDataArray [0];						
					
					mTarget.mFishGraphic.OrgOffset = EditorGUILayout.Vector3Field ("OrgOffset", mTarget.mFishGraphic.OrgOffset);
					mTarget.mFishGraphic.OrgOffsetTwo = EditorGUILayout.Vector3Field ("OrgOffsetTwo", mTarget.mFishGraphic.OrgOffsetTwo);
					mTarget.mFishGraphic.FishNum = EditorGUILayout.IntField ("fishnum", mTarget.mFishGraphic.FishNum);
					GUILayout.Space (4);

					mTarget.mFishGraphic.Value0 = EditorGUILayout.IntField ("Value0", mTarget.mFishGraphic.Value0);
					mTarget.mFishGraphic.Value1 = EditorGUILayout.IntField ("Value1", mTarget.mFishGraphic.Value1);
					mTarget.mFishGraphic.Value2 = EditorGUILayout.IntField ("Value2", mTarget.mFishGraphic.Value2);

					Vector3[] paths = mTarget.mFishGraphic.GenerateGraphic ();
					if (paths != null && paths.Length > 0) 
					{
						int n = paths.Length;
						groupData.FishNum = (ushort)n;
						groupData.PosList = new Vector3[n];
						for (int i = 0; i < groupData.PosList.Length; i++) {
							groupData.PosList [i] = paths [i];
						}
						float[] newdl = new float[n];
						int nn = Mathf.Min (groupData.DelayList.Length, n);
						Array.Copy (groupData.DelayList, 0, newdl, 0, nn);
						groupData.DelayList = newdl;
						mTarget.UpdateSceneObjStatus (mTarget.m_OpeningParadeData.mFishParade);
					}
				}
				EditorGUILayout.EndVertical ();
			}
		}
	}
}
#endif