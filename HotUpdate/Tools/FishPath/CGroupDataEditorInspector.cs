#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(CGroupDataEditor))]

public class CGroupDataEditorInspector : Editor 
{
    private CGroupDataEditor                                        m_TargetData;
	string delaysplitStr = "0";
    public override void OnInspectorGUI()
    {
		m_TargetData = target as CGroupDataEditor;
		bool hasChanged = false;
		m_TargetData.GetPoslist (m_TargetData.m_FishParadeData);
        EditorGUILayout.BeginVertical();

        if (m_TargetData.m_FishParadeData != null) {
            if (!m_TargetData.IsEditorMode) {
                SerializedProperty pathIndexSP = serializedObject.FindProperty("m_PathIndex");
                EditorGUILayout.PropertyField(pathIndexSP);
                if (GUILayout.Button("刷新路径")) {
                    m_TargetData.RefreshPath();
                }
            }
        }
        if (!m_TargetData.IsEditorMode) //运行时才使用该功能
        {
            if (GUILayout.Button("发出该路径的鱼")) {
                if (null != m_TargetData.m_FishParadeData) {
                    int idx = 0;
                    foreach (GroupData gd in m_TargetData.m_FishParadeData.GroupDataArray) {
                        if (gd == null)
                            break;
                        uint pathIdx = m_TargetData.m_FishParadeData.PathList[idx];
                        PathLinearInterpolator pi = CFishPathEditor.only.FishData.m_PathInterpList.Find(x => x.pathUDID == pathIdx);

                        if (gd.FishShapeID > 0) {
                            GameObject fish_content = GameObject.Instantiate(FishResManager.Instance.FishShapeMap.TryGet(gd.FishShapeID));
                            FishShapeContent content = fish_content.AddComponent<FishShapeContent>();
                            content.SetOffSet(gd.ShapeOffset);
                            content.transform.localScale *= gd.ShapeScale;

                            MeshFilter[] mfs = fish_content.GetComponentsInChildren<MeshFilter>();
                            List<Vector3> pos_list;
                            foreach (var item in mfs) {
                                pos_list = GameUtils.CreateFishPos(item.sharedMesh, gd.Density);

                                for (int j = 0; j < pos_list.Count; ++j) {
                                    float time = 0;//GameUtils.GetPathTimeByDist(m_TargetData.m_FishParadeData.FrontPosition.x, pos_list[j].x, pi);
                                    Fish fish = new Fish();
                                    FishVo fishvo = CFishPathEditor.only.GetFishVo(gd.FishCfgID);
                                    float fspeed = fishvo.Speed * gd.SpeedScaling;
                                    float fscale = fishvo.Scale * gd.FishScaling;
                                    fish.Init(++CFishPathEditor.FishID, gd.FishCfgID, fscale, time, gd.ActionSpeed, fspeed, pi);
                                    fish.SetOffset(pos_list[j]);

                                    fish.SetPostLaunch(0);
                                    fish.SetFishShape(item.transform, content);
                                    SetFish(fish);
                                }
                            }
                        } else {
                            if (gd.FishNum > gd.PosList.Length) {
                                LogMgr.Log("错误的鱼群路径点:" + gd.FishNum + ", posnum:" + gd.PosList.Length);
                                return;
                            }
                            for (int i = 0; i < gd.FishNum; ++i) {
                                float time = 0;// GameUtils.GetPathTimeByDist(m_TargetData.m_FishParadeData.FrontPosition.x, gd.PosList[i].x, pi);
                                Fish fish = new Fish();
                                FishVo mFishVo = CFishPathEditor.only.GetFishVo(gd.FishCfgID);
                                float fscale = mFishVo.Scale * gd.FishScaling;
                                float fspeed = mFishVo.Speed * gd.SpeedScaling;

                                fish.Init(++CFishPathEditor.FishID, gd.FishCfgID, fscale, time, gd.ActionSpeed, fspeed, pi);
                                fish.SetOffset(new Vector3(gd.PosList[i].x, 1 * gd.PosList[i].y, gd.PosList[i].z));

                                fish.SetPostLaunch(gd.DelayList[i]);

                                SetFish(fish);
                            }
                        }
                    }
                }
            }

            if (GUILayout.Button("清空所有鱼")) {
                CFishPathEditor.ClearAllFish();
            }
        }
        

		if (m_TargetData.m_FishParadeData != null) {
			int groupLen = m_TargetData.m_FishParadeData.GroupDataArray.Length;
			m_TargetData.name = "FishParade" + m_TargetData.m_FishParadeData.FishParadeId;
			for (int i = 0; i < m_TargetData.m_FishParadeData.GroupDataArray.Length; i++) 
			{
				EditorGUILayout.BeginHorizontal ();
				delaysplitStr = GUILayout.TextField (delaysplitStr);
				if (GUILayout.Button("["+i + "] Arrange")) 
				{
					float delaysplit = 0f;
					if (float.TryParse (delaysplitStr, out delaysplit)) 
					{
						float fval = delaysplit;
						for (int j = 0; j < m_TargetData.m_FishParadeData.GroupDataArray[i].DelayList.Length; j++)
						{
							m_TargetData.m_FishParadeData.GroupDataArray [i].DelayList [j] = fval;
							fval += delaysplit;
						}
					}
				}
				EditorGUILayout.EndHorizontal ();
			}

			SerializedProperty groupDataArraySP = serializedObject.FindProperty("m_FishParadeData");
            EditorGUILayout.PropertyField(groupDataArraySP, true);


			hasChanged = serializedObject.ApplyModifiedProperties();
        }

		if (hasChanged)
		{
			for (int i = 0; i < m_TargetData.m_FishParadeData.GroupDataArray.Length; i++) 
			{
				ushort nnnn = (ushort)m_TargetData.m_FishParadeData.GroupDataArray [i].PosList.Length;
				m_TargetData.m_FishParadeData.GroupDataArray [i].FishNum = nnnn;
				float[] oldDels = m_TargetData.m_FishParadeData.GroupDataArray [i].DelayList;
				float[] dels = new float[nnnn];
				System.Array.Copy (oldDels, 0, dels, 0, Mathf.Min(oldDels.Length, nnnn));
				m_TargetData.m_FishParadeData.GroupDataArray [i].DelayList = dels;
			}
			m_TargetData.UpdateSceneObjStatus (m_TargetData.m_FishParadeData);
		}

		if (CGroupDataEditor.CurrentEditor != m_TargetData) 
		{
			m_TargetData.CheckDataOK (m_TargetData.m_FishParadeData);
			m_TargetData.UpdateSceneObjStatus(m_TargetData.m_FishParadeData);
			if (CGroupDataEditor.CurrentEditor != null)
				CGroupDataEditor.CurrentEditor.ClearFish ();
		}
		CGroupDataEditor.CurrentEditor = m_TargetData;

        EditorGUILayout.EndVertical();
    }

    private void SetFish(Fish fish)
    {
        if (null != fish)
        {
            CFishPathEditor fpe = m_TargetData.gameObject.GetComponentInParent<CFishPathEditor>();

            if (fpe)
            {
                fpe.AddFishToList(fish);
            }
        }
    }

	void ShowFishGraphic()
	{
		if (Application.isEditor) {

			if (m_TargetData.mFishGraphic == null)
				m_TargetData.mFishGraphic = FishGraphicFactory.Create ();

			if (m_TargetData.mFishGraphic != null) {
				GUILayout.Space (14);
				GroupData groupData = m_TargetData.m_FishParadeData.GroupDataArray [0];
				GUILayout.Label ("———————————————————————————————————————————————————————————————————————————————————");
				Vector3 orgOffset = Vector3.zero;
				int fishnum = -1;
				int value0 = 0, value1 = 0, value2 = 0;

				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal ();
				m_TargetData.mFishGraphic.IsActive = EditorGUILayout.Toggle (m_TargetData.mFishGraphic.IsActive);
				GUILayout.Space (8);
				GUILayout.Label ("GraphicType:");
				m_TargetData.mFishGraphic.graphictype = (EnuGraphicType)EditorGUILayout.EnumPopup (m_TargetData.mFishGraphic.graphictype);
				EditorGUILayout.EndHorizontal ();
				if (m_TargetData.mFishGraphic.IsActive) {
					m_TargetData.mFishGraphic.OrgOffset = EditorGUILayout.Vector3Field ("OrgOffset", m_TargetData.mFishGraphic.OrgOffset);
					m_TargetData.mFishGraphic.FishNum = EditorGUILayout.IntField ("fishnum", m_TargetData.mFishGraphic.FishNum);
					GUILayout.Space (4);

					m_TargetData.mFishGraphic.Value0 = EditorGUILayout.IntField ("Value0", m_TargetData.mFishGraphic.Value0);
					m_TargetData.mFishGraphic.Value1 = EditorGUILayout.IntField ("Value1", m_TargetData.mFishGraphic.Value1);
					m_TargetData.mFishGraphic.Value2 = EditorGUILayout.IntField ("Value2", m_TargetData.mFishGraphic.Value2);
					if (fishnum > 0) {
						Vector3[] paths = m_TargetData.mFishGraphic.GenerateGraphic ();
						if (paths != null && paths.Length > 0) {
							groupData.FishNum = (ushort)fishnum;
							groupData.PosList = new Vector3[fishnum];
							for (int i = 0; i < groupData.PosList.Length; i++)
								groupData.PosList [i] = paths [i];
							float[] newdl = new float[fishnum];
							System.Array.Copy (groupData.DelayList, 0, newdl, 0, groupData.DelayList.Length);
							groupData.DelayList = newdl;
						}
					}
				}
				EditorGUILayout.EndVertical ();
			}
		}
	}
}
#endif