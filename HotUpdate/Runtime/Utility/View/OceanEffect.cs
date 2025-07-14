using UnityEngine;
using System.Collections;

public class OceanEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Camera> ().depth = -1;
		RenderSettings.fog = true;
		RenderSettings.fogColor = new Color (1f,1f,1f,0f);
		RenderSettings.fogDensity = 0.001f;
		RenderSettings.fogMode = FogMode.Exponential;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		RenderSettings.fog = false;
	}

	void OnEnable(){
		RenderSettings.fog = true;
	}
	void OnDisable(){
		RenderSettings.fog = false;
	}
}
