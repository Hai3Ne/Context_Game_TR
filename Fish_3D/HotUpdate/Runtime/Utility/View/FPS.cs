using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour {

	public int AccountInterval = 10;
	// Use this for initialization
	void Start () {
		lastTicks = GlobalTimer.Ticks; 
		interval = AccountInterval;
	}

	int fishCnt = 0;
	uint fpsval = 0;
	GUIStyle fontStyle;
	void OnGUI()
	{
		if (SceneLogic.Instance != null && SceneLogic.Instance.FishMgr != null)
            fishCnt = SceneLogic.Instance.FishMgr.DicFish.Count;
		if (fontStyle == null) {
			fontStyle = new GUIStyle ();
			fontStyle.normal.textColor = Color.red;
			fontStyle.fontSize = 30;
		}

		float ww = 200f;
		float hh = 80f;
		string fps = string.Format("FPS:{0}",fpsval);
		GUI.BeginGroup (new Rect (Screen.width - ww, 60f, ww, hh));
		GUI.Label (new Rect (1f, 1f, ww, 30f), fps, fontStyle);
		if (fishCnt > 0)
			GUI.Label (new Rect (1f, 31f, ww, 30f), string.Format("Fish:{0}",fishCnt), fontStyle);
		GUI.EndGroup ();

	}

	uint fpcount = 0;
	long lastTicks = 0;
	const long SEC = 10000000;
	const long SEC2 = 5000000;
	float interval = 0;
	void Update()
	{
		long nowT = GlobalTimer.m_Watch.ElapsedTicks; 
		long t = nowT - lastTicks;
		lastTicks = nowT;
		if (interval++ < AccountInterval)
			return;
		interval = 0;
		fpsval = (uint)Mathf.CeilToInt(1f/(t * 1.0f / SEC));
	
		/*if (t >= SEC2)
		{
			t -= SEC;
			lastTicks = nowT;
			fpsval = fpcount*2;
			fpcount = 0;//(t * 1.0f)/ SEC;
		}
		fpcount++;*/
	}

	void OnDrawGizmos()
	{
		float z = Camera.main.transform.position.z;
		Gizmos.DrawFrustum (Camera.main.transform.position, Camera.main.fieldOfView, Camera.main.nearClipPlane+z, Camera.main.farClipPlane+z, Camera.main.aspect);
	}

	public Material Mat;
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (Mat != null)
			Graphics.Blit(src, dst, Mat);
	}

	[ContextMenu("testt")]
	void dsf()
	{
		Vector3 a = new Vector3 (-1f, 1, -1f);
		Vector3 b = new Vector3 (30f, 40, 50f);
		Debug.Log(Vector3.Scale(a,b*0.5f));
	}
}
