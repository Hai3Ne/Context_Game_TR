using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LightingStyle{
	StrokeParallel,
	StrokeSeries,
}

public class EffectLighting : MonoBehaviour {


	class LineData
	{
		public GameObject capHead;
		public LineRenderer mLineR;
		public float delay = 0f, life;
		public Vector3 startPos, targetPos;
		public float runningTime;
		public LineData(LineRenderer lineR, GameObject lineCapEnd, Vector3 startPos, Vector3 targetPos, float life, float delay = 0){
			this.capHead = lineCapEnd;
			this.mLineR = lineR;
			this.startPos = startPos;
			this.targetPos = targetPos;
			this.delay = delay;
			this.life = life;
			runningTime = 0f;
		}

		public void Destroy(){
			GameObject.Destroy (capHead);
			GameObject.Destroy (mLineR.gameObject);
		}
	}

	public LightingStyle style;
	[SerializeField]
	private LineRenderer mLinearR = null;
	[SerializeField]
	private GameObject lightEndCap = null;
	[SerializeField]
	private float duration=10f;

	public Vector3 fromPos;
	public Vector3[] targetList;
	float timeToggle = 0f;
	float oneLineDuration = 0.1f;
	float totalDuration = 1f;
	int lightingIdx = 0;
	bool mIsOver= false;
	List <LineData> mLightingLines = new List<LineData>();

	public float CalTotalDuration(int fishCnt){
		if (style == LightingStyle.StrokeSeries)
			return  duration * fishCnt;
		else
			return duration;
	}

	void Start () {
		timeToggle = 0f;
		for (int i = 0; i < targetList.Length; i++) {
			targetList [i].z = ConstValue.NEAR_Z + 0.1f;
			targetList [i] = Camera.main.ScreenToWorldPoint (targetList[i]);
		}
		mIsOver = false;
		if (fromPos.z < 100f)
			fromPos.z = ConstValue.NEAR_Z + 0.1f;
		fromPos = Camera.main.ScreenToWorldPoint (fromPos);
		currentLineData = null;
		InitLineTrans ();
	}
	
	// Update is called once per frame
	LineData currentLineData;
    private float mWaitTime = 0;
    public void SetWaitTime(float time) {
        this.mWaitTime = time;
    }
	void Update () {
		if (mIsOver)
			return;
        if (this.mWaitTime > 0) {
            this.mWaitTime -= Time.deltaTime;
            return;
        }

		float delta = Time.deltaTime;
		timeToggle += delta;
		if (currentLineData != null) {
			currentLineData.runningTime += delta;
			float percent = currentLineData.runningTime / oneLineDuration;
			Vector3 p = Vector3.Lerp (currentLineData.startPos, currentLineData.targetPos, percent);
			currentLineData.mLineR.SetPosition (1, p);
			currentLineData.capHead.transform.position = p;
		}
		if (timeToggle <= totalDuration) {
			if (style == LightingStyle.StrokeSeries) {
				if (lightingIdx < mLightingLines.Count) {
					if (timeToggle > lightingIdx * oneLineDuration) {						
						currentLineData = mLightingLines [lightingIdx];
						currentLineData.mLineR.gameObject.SetActive (true);
						currentLineData.capHead.SetActive (true);
						currentLineData.runningTime = delta;
						lightingIdx++;
					}
				} else {
					currentLineData = null;
				}
			}
		} else {
			foreach (var pdata in mLightingLines)
				pdata.Destroy ();
			mIsOver = true;
			mLightingLines.Clear ();
		}

	}

	public bool isOver
	{
		get { return mIsOver;}
	}


	void InitLineTrans()
	{
		mLightingLines.Clear ();
		if (targetList.Length == 0)
			return;
		totalDuration = duration;
		lightEndCap.transform.position = fromPos;
		SetLine (mLinearR, targetList[0], fromPos);

		for (int i = 1; i < targetList.Length; i++) 
		{
			SetLine (GameUtils.CreateGo (mLinearR.gameObject, this.transform).GetComponent<LineRenderer>(), targetList[i], targetList[i-1]);
		}
		if (style == LightingStyle.StrokeSeries) {
			totalDuration = duration * mLightingLines.Count;
			oneLineDuration = duration;/// / mLightingLines.Count;
			lightingIdx = 0;
		}
	}

	void SetLine(LineRenderer pLinear, Vector3 targetPos, Vector3 startPos, float delayv = -1f)
	{
		if (style == LightingStyle.StrokeParallel) {
			pLinear.SetPosition (0, fromPos);
			pLinear.SetPosition (1, targetPos);
			GameObject llineCap = GameUtils.CreateGo (lightEndCap, this.transform);
			llineCap.transform.position = targetPos;
			mLightingLines.Add (new LineData (pLinear, llineCap, startPos, targetPos, 1f, delayv));
		} else {			
			pLinear.SetPosition (0, startPos);
			pLinear.SetPosition (1, startPos);
			GameObject llineCap = GameUtils.CreateGo (lightEndCap, this.transform);
			llineCap.transform.position = startPos;
			pLinear.gameObject.SetActive (false);
			llineCap.gameObject.SetActive (false);
			mLightingLines.Add (new LineData (pLinear, llineCap, startPos, targetPos, 1f, delayv));

		}
	}
}