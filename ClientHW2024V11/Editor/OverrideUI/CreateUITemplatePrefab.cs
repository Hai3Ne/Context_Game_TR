using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class CreateUITemplatePrefab
{
    [MenuItem("GameObject/UI/U_UIPanel", false, 0)]
    static void CreateUIPanelObj(MenuCommand menuCommand)
    {
        GameObject panel = SaveObject(menuCommand, "UIPanel");
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GameObject/UI/U_Button - TextMeshPro", false, 20)]
    static void CreateUISuperButton(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "UIButton");
    }

    [MenuItem("GameObject/UI/U_Text - TextMeshPro", false, 20)]
    static void CreateUIU_Text(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "UIText");
    }

    [MenuItem("GameObject/UI/U_InputField - TextMeshPro", false, 22)]
    static void CreateUIInputField(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "UIInputField");
    }

    [MenuItem("GameObject/UI/U_ScrollView/HListScroll View", false, 22)]
    static void CreateHListScroll(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "ScrollView/HListScroll View");
    }
    [MenuItem("GameObject/UI/U_ScrollView/HGridScroll View", false, 23)]
    static void CreateHGridScroll(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "ScrollView/HGridScroll View");
    }

    [MenuItem("GameObject/UI/U_ScrollView/VListScroll View", false, 24)]
    static void CreateVListScroll(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "ScrollView/VListScroll View");
    }



    [MenuItem("GameObject/UI/U_ScrollView/VGridScroll View", false, 25)]
    static void CreateVGridScroll(MenuCommand menuCommand)
    {
        SaveObject(menuCommand, "ScrollView/VGridScroll View");
    }


    static GameObject SaveObject(MenuCommand menuCommand, string prefabName, string objName = "")
    {

        var path = "Assets/AssetsRaw/UITemplate/" + prefabName + ".prefab";
        GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(path);
        if (prefab)
        {
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (!string.IsNullOrEmpty(objName))
            {
                inst.gameObject.name = objName;
            }
            if (Selection.activeTransform)
            {
                inst.transform.SetParent(Selection.activeTransform);

            }
            GameObjectUtility.SetParentAndAlign(inst, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(inst, $"Create {inst.name}__" + inst.name);
            inst.transform.SetAsLastSibling();
            PrefabUtility.UnpackPrefabInstance(inst, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return inst;
        }
        return null;
    }
}
