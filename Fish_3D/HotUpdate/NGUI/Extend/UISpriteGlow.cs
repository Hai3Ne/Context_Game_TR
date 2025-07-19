using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UISprite))]
public class UISpriteGlow : MonoBehaviour {

	public Color glowColor;
	public float glowSize;
	Shader effShader;
	// Use this for initialization

	UISprite mSprite = null;
	UISprite sprite
	{
		get 
		{
			if (mSprite == null)
				mSprite = this.gameObject.GetComponent<UISprite> ();
			return mSprite;
		}
	}
	string lastShaderName;

	void Awake()
	{
		effShader = Shader.Find ("Unlit/Transparent Glow");
	}
	void Start () {
		lastShaderName = sprite.shader.name;
		SetEnabled(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetEnabled(bool enable)
	{
		if (enable) {
			lastShaderName = sprite.shader.name;
			sprite.shader = effShader;
			sprite.material.SetColor ("Glow", glowColor);
		} else {
			sprite.shader = Shader.Find (lastShaderName);
		}
	}
}