using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuperScrollText))]
public class SuperScrollTextEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
       
        SuperScrollText _example = target as SuperScrollText;
        if (GUILayout.Button("����"))
        {
            _example.startInit();
        }
       

        if (GUILayout.Button("����λ��"))
        {
            _example.ResetList();
        }

        _example.count = EditorGUILayout.IntSlider("��������", _example.count, 0, 100);
    }

}
