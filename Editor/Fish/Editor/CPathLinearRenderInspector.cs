#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(CPathLinearRender))]
public class CPathLinearRenderInspector : CPathLinearRenderPreview 
{
    CPathLinearRender                           m_TargetData;
    public override void OnInspectorGUI()
    {
        m_TargetData = target as CPathLinearRender;

		EditorGUILayout.BeginVertical();
		if (m_TargetData.PathData != null) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("PathID:", GUILayout.Width(80));
			string numStr = GUILayout.TextField (m_TargetData.currentPathID.ToString (), GUILayout.Width (100));
			uint num = 0;
			if (uint.TryParse(numStr, out num))
			{
				m_TargetData.currentPathID = num;
				if (m_TargetData.currentPathID != m_TargetData.PathData.pathUDID) 
				{
					PathType ptype = PathType.GropPath;
					if (m_TargetData.transform.parent.name == CFishPathEditor.FISH_PATH_NAME)
						ptype = PathType.SinglePath;
					else if (m_TargetData.transform.parent.name == CFishPathEditor.BOSS_PATH_NAME) {
						ptype = PathType.BossPath;
					}
						
					if (CFishPathEditor.only.CheckPathIDAvaible (m_TargetData.currentPathID, ptype, m_TargetData.PathData.pathUDID)) {
						uint oldPathUID = m_TargetData.PathData.pathUDID;
						m_TargetData.PathData.pathUDID = m_TargetData.currentPathID;
						CFishPathEditor.only.UpdateFishPathReferences (oldPathUID, m_TargetData.currentPathID);
						m_TargetData.gameObject.name = "path" + m_TargetData.currentPathID;
					} else {
						if (CPathLinearRender.CurrentSelectedObj != m_TargetData) {
							m_TargetData.currentPathID = m_TargetData.PathData.pathUDID;
						}
					}
				}
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space (10);
		}

		if (m_TargetData.BossPathData != null && m_TargetData.BossPathData.bossCfgID > 0)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("BossCfgID:", GUILayout.Width(80));
			m_TargetData.BossPathData.bossCfgID = uint.Parse(GUILayout.TextField(m_TargetData.BossPathData.bossCfgID.ToString(), GUILayout.Width(100)));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("PathDuraion:", GUILayout.Width(80));
			m_TargetData.BossPathData.duration = float.Parse(GUILayout.TextField(m_TargetData.BossPathData.duration.ToString(), GUILayout.Width(100)));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("DefSwinClip:", GUILayout.Width(80));
			m_TargetData.BossPathData.defaultSwinClip = (byte)EditorGUILayout.IntField(m_TargetData.BossPathData.defaultSwinClip, GUILayout.Width(100));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("延迟发射时间:", GUILayout.Width(80));
			m_TargetData.BossPathData.delay = float.Parse(GUILayout.TextField(m_TargetData.BossPathData.delay.ToString(), GUILayout.Width(100)));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator ();

		}


		if (Application.isPlaying)
		{
			if (GUILayout.Button("Launch Fish"))
    	    {
        	    m_TargetData.LaunchFish();
        	}
		}
        EditorGUILayout.EndVertical();

        GUILayout.Space(3f);
        if (m_TargetData.PathData != null) 
        {
			GUILayout.BeginVertical (); 

            GUILayout.BeginHorizontal();
			GUILayout.Label("采样最大长度:", GUILayout.Width(80));
			m_TargetData.PathData.m_SampleMaxDistance = float.Parse(GUILayout.TextField(m_TargetData.PathData.m_SampleMaxDistance.ToString(), GUILayout.Width(100)));
            GUILayout.EndHorizontal();

			if (Application.isEditor) 
			{    
				m_TargetData.IsDrawPath = GUILayout.Toggle (m_TargetData.IsDrawPath, " IsDrawPath");
				m_TargetData.IsAutoVisOnSelect = GUILayout.Toggle (m_TargetData.IsAutoVisOnSelect, " IsAutoVisOnSelect");
				m_TargetData.isAutoAdjustPointDirection = GUILayout.Toggle (m_TargetData.isAutoAdjustPointDirection, "IsAutoAdjustPointDir");

			}


			//========================================= PathEvent ==============================================
			GUILayout.Space(10);
			m_TargetData.PathData.m_HasPathEvent = GUILayout.Toggle(m_TargetData.PathData.m_HasPathEvent, " HasPathEvent");
            if (m_TargetData.PathData.m_HasPathEvent) {
                ShowEventNodeButton();
                List<NodeEvent> event_list = m_TargetData.PathData.mEventList;
                if (event_list.Count > 0) {
                    while (m_TargetData.NodeVisble.Count < event_list.Count) {
                        m_TargetData.NodeVisble.Add(false);
                    }
                    for (int i = 0; i < event_list.Count; i++) {
                        GUILayout.BeginHorizontal();
                        m_TargetData.NodeVisble[i] = EditorGUILayout.Toggle(m_TargetData.NodeVisble[i]);

                        float min = 0;
                        float max = 1;
                        if (i > 0) {
                            min = event_list[i - 1].StartStep;
                        }
                        if (i < event_list.Count - 1) {
                            max = event_list[i + 1].StartStep;
                        }

                        GUILayout.Label(i + "(" + min.ToString("0.00") + "~" + max.ToString("0.00") + ") ");

                        GUILayout.Label("EventType:");
                        PathEventType evtType = (PathEventType)event_list[i].EventType;
                        evtType = (PathEventType)EditorGUILayout.EnumPopup(evtType);
                        event_list[i].EventType = (byte)evtType;

                        if (evtType == PathEventType.NONE) {
                            GUILayout.Space(220);
                        } else if (evtType == PathEventType.ANIMATIONS) {
                            GUILayout.Label("AnimaID:");
                            event_list[i].Times = (ushort)EditorGUILayout.IntField(event_list[i].Times, GUILayout.Width(150f));
                        } else {
                            event_list[i].AutoTrigger = EditorGUILayout.Toggle(event_list[i].AutoTrigger, GUILayout.Width(8f));

                            GUILayout.Space(5f);
                            GUILayout.Label("EventAnim:");
                            event_list[i].SubAnimID = byte.Parse(GUILayout.TextField(event_list[i].SubAnimID.ToString(), GUILayout.Width(30f)));

                            GUILayout.Space(5f);
                            GUILayout.Label("Times:");
                            event_list[i].Times = ushort.Parse(GUILayout.TextField(event_list[i].Times.ToString(), GUILayout.Width(30f)));
                            if (evtType == PathEventType.STAY)
                                GUILayout.Label("秒");
                            else
                                GUILayout.Label(" ");
                        }
                        GUILayout.EndHorizontal();

                        if (m_TargetData.NodeVisble[i]) {
                            this.SetEventIndex(event_list, i);
                        }
                    }
                }
            }
			//========================================= PathEvent End ==============================================


                m_TargetData.PathData.mAnimCurve = EditorGUILayout.CurveField(m_TargetData.PathData.mAnimCurve);
                serializedObject.ApplyModifiedProperties();

                if (m_TargetData.lastSelectChild) {
                    m_TargetData.IsKeySamplePointsChanged = true;
                    m_TargetData.lastSelectChild = false;
                    m_TargetData.IsSelected = false;
                }
				if (!m_TargetData.IsSelected) {
					m_TargetData.childKeySampleFromParent ();
					m_TargetData.IsSelected = true;
				}

                m_TargetData.RefreshData();
            
			GUILayout.Space (10f);

			GUILayout.BeginHorizontal ();
			if (m_TargetData.HasKeySample && GUILayout.Button("Add Point"))
			{
				m_TargetData.AddKeyPoint ();
			}

			if (GUILayout.Button ("Select All")) 
			{
				GameObject[] selObjs = new GameObject[m_TargetData.transform.childCount];
				for (int i = 0; i < m_TargetData.transform.childCount; i++) {
					selObjs[i] = m_TargetData.transform.GetChild (i).gameObject;
				}
				Selection.objects = selObjs;
			}
			CPathLinearRender.OnlyShow(m_TargetData);
        }
    }

    void ShowEventNodeButton() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add EventNode")) {
            List<NodeEvent> event_list = m_TargetData.PathData.mEventList;
            float min = 0;
            if (event_list.Count > 0) {
                min = event_list[event_list.Count - 1].StartStep;
            }
            event_list.Add(new NodeEvent {
                StartStep = min,
                EventType = (byte)PathEventType.NONE,
                Times = 0,
            });
        }
        if (GUILayout.Button("Clear EventNodes")) {
            m_TargetData.PathData.mEventList.Clear();
        }
        GUILayout.EndHorizontal();
    }
    private void SetEventIndex(List<NodeEvent> event_list, int index) {
        NodeEvent range = event_list[index];
        float min = 0;
        float max = 1;
        if (index > 0) {
            min = event_list[index - 1].StartStep;
        }
        if (index < event_list.Count - 1) {
            max = event_list[index + 1].StartStep;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        range.StartStep = EditorGUILayout.Slider(range.StartStep, min, max);
        GUILayout.EndHorizontal();
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
}
#endif