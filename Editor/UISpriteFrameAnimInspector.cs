using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(UISpriteFrameAnim))]

public class UISpriteFrameAnimInspector : Editor 
{
    UISpriteFrameAnim                           m_Data;

    private string                              m_ChangeAnimName = "";//改变的动画名称
    private bool                                m_IsRefreshData = true;

	// Use this for initialization
    void OnEnable() 
    {
        m_Data = target as UISpriteFrameAnim;
    }
	
    public override void OnInspectorGUI ()
    {
        if (m_IsRefreshData)
        {
            m_Data.ClearSpriteData();
            m_Data.ReadSpriteData();
            m_IsRefreshData = false;
        }
        //Debug.Log("eeeeeeeessssss");

        EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("当前动画:", GUILayout.Width(100));
                EditorGUILayout.LabelField(m_Data.RunningAnimName, GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("总帧数:", GUILayout.Width(100));
                EditorGUILayout.LabelField(m_Data.RunningFrameNumber.ToString(), GUILayout.Width(30));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
                if (m_Data.Data.AnimList.Count > 0) 
                {
                    EditorGUILayout.LabelField("                          动画信息");
                } else 
                {
                    EditorGUILayout.LabelField("                          没有动画信息");
                }
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        for (int i = 0; i < m_Data.Data.AnimList.Count; ++i) 
        {
            FrameAnimData fad = m_Data.Data.AnimList[i];
            int frameNumber = fad.Number;
            string frameName = fad.Name;
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("动画名称:" + frameName, GUILayout.Width(120));
                //fad.Name = EditorGUILayout.TextField(frameName, GUILayout.Width(50));
                EditorGUILayout.LabelField("帧数:" + frameNumber, GUILayout.Width(60));
                //fad.Number = int.Parse( EditorGUILayout.TextField(frameNumber.ToString(), GUILayout.Width(20)) );
                /*if (GUILayout.Button("删除该动画")) 
                {
                    m_Data.Data.AnimList.RemoveAt(i);
                }*/
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("                               动 画 事 件");
            EditorGUILayout.EndVertical();

            for (int j = 0; j < fad.EventList.Count; ++j)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("事件名称:", GUILayout.Width(60));
                    //fad.EventList[j].MessageName = EditorGUILayout.TextField(fad.EventList[j].MessageName, GUILayout.Width(50));
                    EditorGUILayout.LabelField(fad.EventList[j].MessageName, GUILayout.Width(80));
                    EditorGUILayout.LabelField("通知帧数:", GUILayout.Width(60));
                    //fad.EventList[j].FrameIndex = int.Parse(EditorGUILayout.TextField(fad.EventList[j].FrameIndex.ToString(), GUILayout.Width(20)));
                    EditorGUILayout.LabelField(fad.EventList[j].FrameIndex.ToString(), GUILayout.Width(20));
                    /*
                    if (GUILayout.Button("删除该动画事件", GUILayout.Width(100))) 
                    {
                        fad.EventList.RemoveAt(j);
                    }*/
                EditorGUILayout.EndHorizontal();
            }
            /*
            EditorGUILayout.BeginVertical();
                if (GUILayout.Button("添加动画事件"))
                {
                    FrameAnimEvent fae = new FrameAnimEvent();
                    fae.FrameIndex = 0;
                    fae.MessageName = "OnFrameAnimAttack";
                    fad.EventList.Add(fae);
                    serializedObject.ApplyModifiedProperties();

                }
                
                EditorGUILayout.LabelField("");
            EditorGUILayout.EndVertical();*/
        }

        EditorGUILayout.BeginVertical();
            /*
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("添加动画"))
            {
                FrameAnimData fad = new FrameAnimData();
                fad.Number = 0;
                fad.Name = "";
                m_Data.Data.AnimList.Add(fad);
            }*/
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("改变当前动画(请输入帧动画名称):", GUILayout.Width(160));
                m_ChangeAnimName = EditorGUILayout.TextField(m_ChangeAnimName, GUILayout.Width(80));
                if (GUILayout.Button("播放该动画")) { m_Data.ChangeToAnim(m_ChangeAnimName); }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("播放下一个动画")) { m_Data.ChangeToNextAnim(); }
            if (GUILayout.Button("播放上一个动画")) { m_Data.ChangeToPreAnim(); }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("动画播放速度(正常10,范围1~100) ", GUILayout.Width(240));
            float slideValue = m_Data.AnimSpeed * 10;
            float animSpeed = EditorGUILayout.Slider("Alpha", slideValue, 1f, 100f);
            m_Data.AnimSpeed = animSpeed / 10f;
            EditorGUILayout.EndVertical();
            
            SerializedProperty testSP = serializedObject.FindProperty("m_AnimEventList");
            EditorGUILayout.PropertyField(testSP, true);
            serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
    }
    
    [MenuItem("Assets/创建帧动画预件")]
    public static void CreateFrameAnimationPrefab() 
    {
        var selected = Selection.activeObject;
        string assetDir = "";
        string fileName = "";
        string prefabName = "";
        string prefabPath = "";
        if (selected != null) 
        {
            assetDir = AssetDatabase.GetAssetPath(selected.GetInstanceID());
            int removeIndex = assetDir.IndexOf(selected.name);
            assetDir = assetDir.Remove(removeIndex);
            fileName = selected.name;
            prefabPath = assetDir + fileName + "_GO.prefab";
            Debug.Log("fileName:" + fileName);
            Debug.Log("dir:" + assetDir);
            Debug.Log("prefabName:" + prefabName);
            Debug.Log("prefabPath:" + prefabPath);
           
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            GameObject GO = new GameObject(prefabName);
            UISprite sprite = GO.AddComponent<UISprite>();
            GO.AddComponent<UISpriteFrameAnim>();

            GameObject atlasGO = AssetDatabase.LoadAssetAtPath(assetDir + fileName + ".prefab", typeof(GameObject)) as GameObject;
            if (null != atlasGO)
            {
                UIAtlas atlas = atlasGO.GetComponent<UIAtlas>();
                if (null != atlas) 
                {
                    sprite.atlas = atlas;
                }
                sprite.width = 512;
                sprite.height = 512;
            }
            GO.layer = LayerMask.NameToLayer("UI");

            PrefabUtility.ReplacePrefab(GO, prefab);
            DestroyImmediate(GO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }
    }
}
