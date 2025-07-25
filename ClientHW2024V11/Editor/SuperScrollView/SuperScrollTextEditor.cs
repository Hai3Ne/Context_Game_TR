using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuperScrollText))]
public class SuperScrollTextEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
       
        SuperScrollText _example = target as SuperScrollText;
        if (GUILayout.Button("生成"))
        {
            _example.startInit();
        }
       

        if (GUILayout.Button("重置位置"))
        {
            _example.ResetList();
        }

        _example.count = EditorGUILayout.IntSlider("生成数量", _example.count, 0, 100);
    }

}
