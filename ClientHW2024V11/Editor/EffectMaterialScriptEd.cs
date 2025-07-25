
// by artsyli
// 2014.4.22

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EffectMaterialScript))]
public class EffectMaterialScriptEd : Editor
{
	private bool bConnect = false;
	private void CallbackFunction()
	{
		EffectMaterialScript script = target as EffectMaterialScript;
		if(script != null)
		{
			UnityEditor.EditorUtility.SetDirty(script);
			script.ForceUpdate();
		}
	}

	void OnEnable()
	{
		if(!bConnect)
		{
			EffectMaterialScript script = target as EffectMaterialScript;
			script.EditorEnable();
			EditorApplication.update += CallbackFunction;
			bConnect = true;
		}
	}
	void OnDisable()
	{
		if(bConnect)
		{
			EffectMaterialScript script = target as EffectMaterialScript;
			script.EditorDisable();
			EditorApplication.update -= CallbackFunction;
			bConnect = false;
		}
	}
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		
		EffectMaterialScript script = target as EffectMaterialScript;
		GUI.changed = false;

		EffectMaterialScript.BlendMode blend = (EffectMaterialScript.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", script.blend);
		EffectMaterialScript.LayerMode layer = (EffectMaterialScript.LayerMode)EditorGUILayout.EnumPopup("Render Layer", script.layer);

		bool savematfile = EditorGUILayout.Toggle("Create Material File", script.savematfile);

		Color clr = EditorGUILayout.ColorField("Main Color", script.color);
		float brightness = EditorGUILayout.FloatField("Brightness", script.brightness);
		Texture mainTex = EditorGUILayout.ObjectField("Main Texture", script.mainTex, typeof(Texture), true) as Texture;
		float uoffset = EditorGUILayout.FloatField("U Offset", script.uoffset);
		float voffset = EditorGUILayout.FloatField("V Offset", script.voffset);
		float uscale = EditorGUILayout.FloatField("U Scale", script.uscale);
		float vscale = EditorGUILayout.FloatField("V Scale", script.vscale);
		float uvrotate = EditorGUILayout.FloatField("Rotate", script.uvrotate);
		int tilerow = EditorGUILayout.IntField("Tile Row", script.tileRow);
		int tilecol = EditorGUILayout.IntField("Tile Col", script.tileCol);

		EffectMaterialScript.SeqType seqType = (EffectMaterialScript.SeqType)EditorGUILayout.EnumPopup("Seq Type", script.seqType);
		float seqValue = EditorGUILayout.FloatField("Time or Sequence", script.seqValue);

		float delay = EditorGUILayout.FloatField("Delay", script.delay);
		bool once = EditorGUILayout.Toggle("Once", script.once);

		Texture maskTex = EditorGUILayout.ObjectField("Mask Texture", script.maskTex, typeof(Texture), true) as Texture;

		float maskuoffset = EditorGUILayout.FloatField("Mask U Offset", script.maskuoffset);
		float maskvoffset = EditorGUILayout.FloatField("Mask V Offset", script.maskvoffset);
		float maskuscale = EditorGUILayout.FloatField("Mask U Scale", script.maskuscale);
		float maskvscale = EditorGUILayout.FloatField("Mask V Scale", script.maskvscale);
		float maskuvrotate = EditorGUILayout.FloatField("Mask Rotate", script.maskuvrotate);
		
		if (GUI.changed)
		{
			UnityEditor.Undo.RecordObject(script, "Effect Script Change");

			bool bChangeShader = false;
			if(script.blend != blend)
			{
				bChangeShader = true;
			}
			bool bChangeMode = false;
			if(script.savematfile != savematfile)
			{
				bChangeMode = true;
			}

			script.delay = delay;
			script.once = once;

			script.blend = blend;
			script.layer = layer;
		
			script.savematfile = savematfile;

			script.color = clr;
			script.brightness = brightness;
			script.mainTex = mainTex;
			script.uoffset = uoffset;
			script.voffset = voffset;
			script.uscale = uscale;
			script.vscale = vscale;
			script.uvrotate = uvrotate;

			script.seqType = seqType;
			script.tileRow = tilerow;
			script.tileCol = tilecol;
			script.seqValue = seqValue;

			script.maskTex = maskTex;
			script.maskuoffset = maskuoffset;
			script.maskvoffset = maskvoffset;
			script.maskuscale = maskuscale;
			script.maskvscale = maskvscale;
			script.maskuvrotate = maskuvrotate;

			UnityEditor.EditorUtility.SetDirty(script);
			script.EditorChange(bChangeShader, bChangeMode);
		}
	}
}

// 编辑多对象的材质脚本
/*
[CanEditMultipleObjects]
[CustomEditor(typeof(EffectMaterialScript))]
public class EffectMaterialScriptEd : Editor
{
	private SerializedProperty color;
	private SerializedProperty mainTex;
	private SerializedProperty uoffset;
	private SerializedProperty voffset;
	private SerializedProperty uscale;
	private SerializedProperty vscale;
	private SerializedProperty uvrotate;

	private EffectMaterialScript script;
	
	public void OnEnable()
	{
		color = serializedObject.FindProperty("color");
		mainTex = serializedObject.FindProperty("mainTex");
		uoffset = serializedObject.FindProperty("uoffset");
		voffset = serializedObject.FindProperty("voffset");
		uscale = serializedObject.FindProperty("uscale");
		vscale = serializedObject.FindProperty("vscale");
		uvrotate = serializedObject.FindProperty("uvrotate");
		
		script = target as EffectMaterialScript;
	}
	
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		EditorGUILayout.PropertyField(color, new GUIContent("Main Color"));
		EditorGUILayout.PropertyField(mainTex, new GUIContent("Main Texture"));
		EditorGUILayout.PropertyField(uoffset, new GUIContent("U Offset"));
		EditorGUILayout.PropertyField(voffset, new GUIContent("V Offset"));
		EditorGUILayout.PropertyField(uscale, new GUIContent("U Scale"));
		EditorGUILayout.PropertyField(vscale, new GUIContent("V Scale"));
		EditorGUILayout.PropertyField(uvrotate, new GUIContent("UV Rotate"));

		EditorGUILayout
		
		serializedObject.ApplyModifiedProperties();
		script.EditorChange();
	}
}
*/