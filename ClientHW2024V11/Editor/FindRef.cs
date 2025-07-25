using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MissingReferencesFinder : MonoBehaviour
{

    private static void FindReferences(Object[] objects, System.Type type)
    {
        foreach (var oo in objects)
        {
            GameObject go = oo as GameObject;
            var components = go.GetComponents<Component>();

            foreach (var c in components)
            {
                if (c!=null  && c.GetType() == type)
                {
                    Debug.LogError("Box2D obj: " + FullPath(go), go);
                    BoxCollider box = c as BoxCollider;
                    box.isTrigger = true; 
                    //Selection.activeGameObject = go; 
                   
                    continue;
                }

                //SerializedObject so = new SerializedObject(c);
                //var sp = so.GetIterator();

                //while (sp.NextVisible(true))
                //{
                //    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                //    {
                //        if (sp.objectReferenceValue == null
                //            && sp.objectReferenceInstanceIDValue != 0)
                //        {
                //            ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                //        }
                //    }
                //}
            }
        }
    }

    private static void FindMissingReferences(string context, Object[] objects)
    {
        foreach (var oo in objects)
        {
            GameObject go = oo as GameObject; 
            var components = go.GetComponents < MonoBehaviour>();

            foreach (var c in components)
            {
                if (!c)
                {
                    Debug.LogError("Missing Component in GO: " + FullPath(go), go);
                    continue;
                }

                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null
                            && sp.objectReferenceInstanceIDValue != 0)
                        {
                            ShowError(context, go, c.GetType().Name, ObjectNames.NicifyVariableName(sp.name));
                        }
                    }
                }
            }
        }
    }

    private static Object[] GetSceneObjects()
    {
        return Resources.FindObjectsOfTypeAll(typeof(GameObject))
            .Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
                   && go.hideFlags == HideFlags.None).ToArray();
    }

    private const string err = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

    private static void ShowError(string context, GameObject go, string c, string property)
    {
        Debug.LogError(string.Format(err, FullPath(go), c, property, context), go);
    }

    private static string FullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
                : FullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}