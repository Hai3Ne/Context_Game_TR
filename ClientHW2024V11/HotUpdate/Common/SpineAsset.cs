using Spine.Unity;
using UnityEngine;

namespace HotUpdate
{
    public class SpineAsset : MonoBehaviour
    {
        void Start()
        {
#if UNITY_EDITOR && UNITY_ANDROID
            SkeletonGraphic skeleton = transform.GetComponent<SkeletonGraphic>();
            if (skeleton == null)
            {
                return;
            }
            
            skeleton.material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/HotUpdate/Spine/Runtime/spine-unity/Materials/" + skeleton.material.name + ".mat"); 
#endif
        }
    }

}
