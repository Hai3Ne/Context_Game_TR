/**
* @file     : #FileName#
* @brief    : 
* @details  : 
* @author   : #Author#
* @date     : #Date#
*/
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class InCludeShadersTool : EditorWindow
{
    [MenuItem("Tools/Always Included Shaders", false, 1)]
    public static void Init()
    {
        GetWindow<InCludeShadersTool>("Shaders Tool");
    }

    void OnGUI()
    {

        if (GUILayout.Button("Add", GUILayout.Height(50)))
        {
            Object[] selectedShaders = Selection.GetFiltered(typeof(Shader), SelectionMode.DeepAssets);
            if (selectedShaders.Length == 0)
            {
                return;
            }

            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name.Equals("m_AlwaysIncludedShaders"))
                {
                    for (int i = 0; i < selectedShaders.Length; i++)
                    {
                        bool isIncluded = false;
                        for (int j = 0; j < it.arraySize; j++)
                        {
                            SerializedProperty element = it.GetArrayElementAtIndex(j);
                            if (element.objectReferenceValue.name.Equals(selectedShaders[i].name))
                            {
                                isIncluded = true;

                                break;
                            }
                        }

                        if (!isIncluded)
                        {
                            int cnt = it.arraySize;
                            it.InsertArrayElementAtIndex(cnt);
                            SerializedProperty data = it.GetArrayElementAtIndex(cnt);
                            data.objectReferenceValue = selectedShaders[i];
                        }
                    }

                    graphicsSettings.ApplyModifiedProperties();
                }
            }

            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Remove", GUILayout.Height(50)))
        {
            Object[] selectedShaders = Selection.GetFiltered(typeof(Shader), SelectionMode.DeepAssets);
            if (selectedShaders.Length == 0)
            {
                return;
            }

            SerializedObject graphicsSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]);
            SerializedProperty it = graphicsSettings.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name.Equals("m_AlwaysIncludedShaders"))
                {
                    List<Object> includeShaders = new List<Object>();
                    for (int i = 0; i < it.arraySize; i++)
                    {
                        includeShaders.Add(it.GetArrayElementAtIndex(i).objectReferenceValue);
                    }
                    it.ClearArray();

                    for (int i = 0; i < selectedShaders.Length; i++)
                    {
                        for (int j = 0; j < includeShaders.Count; j++)
                        {
                            if (includeShaders[j].name.Equals(selectedShaders[i].name))
                            {
                                includeShaders.RemoveAt(j);

                                break;
                            }
                        }
                    }

                    for (int i = 0; i < includeShaders.Count; i++)
                    {
                        it.InsertArrayElementAtIndex(i);
                        SerializedProperty data = it.GetArrayElementAtIndex(i);
                        data.objectReferenceValue = includeShaders[i];
                    }

                    graphicsSettings.ApplyModifiedProperties();
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
