using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
public class GuideTipsEditor : MonoBehaviour {

	Dictionary<byte, List<GuideStepData>> stepDatasDicts = null;
	public List<GuideTipData> mGuideTipDataList;
	UI_GuideTips mGuideTips;
	public int currentTipIndex = 0;
	// Use this for initialization
	void Start () {
		if (mGuideTips == null)
			mGuideTips = FindObjectOfType<UI_GuideTips> ();
		mGuideTips.isAutoHide = false;
		mGuideTips.gameObject.SetActive (true);
	}

	public void Init(){
		OpenData ();

	}
	// Update is called once per frame
	void Update () {
		if (mGuideTips == null)
			return;
		string keyStr = string.Format ("GuideTips{0}", GuideStepData.TipStartId+currentTipIndex);
		mGuideTips.Init(StringTable.GetString (keyStr), Vector3.zero, Vector3.zero);
		mGuideTipDataList [currentTipIndex].textMaxSize.x = Mathf.Max (300f, mGuideTipDataList [currentTipIndex].textMaxSize.x);
		mGuideTipDataList [currentTipIndex].textMaxSize.y = Mathf.Max (80f, mGuideTipDataList [currentTipIndex].textMaxSize.y);
		mGuideTips.arrowDirect = mGuideTipDataList [currentTipIndex].ArrowDir;
		mGuideTips.range = mGuideTipDataList [currentTipIndex].ArrowRange;
		mGuideTips.dialogText.width = (int)mGuideTipDataList [currentTipIndex].textMaxSize.x;
		mGuideTips.dialogText.height = (int)mGuideTipDataList [currentTipIndex].textMaxSize.y;
		mGuideTips.layout ();
	}


	[ContextMenu("Open Data")]
	void OpenData(){
		List<GuideStepData> stepDataList;
		if (File.Exists(GuideStepEditor.savepath))
			stepDatasDicts = ConfigTables.ParseGuideConfig (File.ReadAllBytes(GuideStepEditor.savepath));
		if (!stepDatasDicts.ContainsKey (GuideStepData.EventTipByte)) {
			stepDatasDicts[GuideStepData.EventTipByte] = new List<GuideStepData> ();				
		}
		stepDataList = stepDatasDicts [GuideStepData.EventTipByte];
		mGuideTipDataList = new List<GuideTipData> ();
		for (int i = 0; i < stepDataList.Count; i++) {
			var tipData = new GuideTipData ();
			tipData.ArrowDir = stepDataList [i].ArrowDir;
			tipData.ArrowRange = stepDataList [i].ArrowRange;
			tipData.textMaxSize = stepDataList [i].textMaxSize;
			mGuideTipDataList.Add (tipData);
		}	
	}

	[ContextMenu("Save Data")]
	void SaveData(){
		stepDatasDicts = ConfigTables.ParseGuideConfig (File.ReadAllBytes(GuideStepEditor.savepath));
		stepDatasDicts [GuideStepData.EventTipByte] = Convert2StepDataList ();
		GuideStepEditor.SaveData (stepDatasDicts);
		LogMgr.Log ("保存成功.");
	}

	List<GuideStepData> Convert2StepDataList() 
	{
		List<GuideStepData> mDataList = new List<GuideStepData> ();
		for (int i = 0; i < mGuideTipDataList.Count; i++) {
			var stepData = new GuideStepData ();
			stepData.ArrowDir = mGuideTipDataList [i].ArrowDir;
			stepData.ArrowRange = mGuideTipDataList [i].ArrowRange;
			stepData.textMaxSize = mGuideTipDataList [i].textMaxSize;
			mDataList.Add (stepData);
		}
		return mDataList;
	}
}
#endif