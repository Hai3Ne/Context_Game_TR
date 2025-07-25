using UnityEngine;
using System.Collections;
using UnityEditor;
public class PrefabReplaceSpriteEditor : EditorWindow
{
    [MenuItem("Tools/图集精灵预设替换")]
    private static void showWindow()
    {
        EditorWindow editor = new PrefabReplaceSpriteEditor();
        editor.Show();
    }
    private void OnHierarchyChange()
    {
       
    }


}
