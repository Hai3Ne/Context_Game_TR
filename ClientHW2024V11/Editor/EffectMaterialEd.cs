
// by artsyli

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class EffectMaterialEd : MaterialEditor 
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		if (!isVisible)
			return;
		
		Material targetMat = target as Material;
		string[] keyWords = targetMat.shaderKeywords;
		
		bool maskON = keyWords.Contains ("MASK_ON");
		EditorGUI.BeginChangeCheck();
		maskON = EditorGUILayout.Toggle ("Use Mask Texture", maskON);
		if (EditorGUI.EndChangeCheck())
		{
			var keywords = new List<string> { maskON ? "MASK_ON" : "MASK_OFF"};
			targetMat.shaderKeywords = keywords.ToArray ();
			EditorUtility.SetDirty (targetMat);
		}
	}
}

