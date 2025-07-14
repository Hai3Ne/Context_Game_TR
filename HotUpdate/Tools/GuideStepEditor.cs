using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

public class GuideStepEditor : MonoBehaviour {
	public enum seatbyte
	{
		Seat0, Seat1
	}

    [EnumLabel("动作类型")]
    public GuideEventType EventType;
	public int currentStepIdx = 0;
    public seatbyte seat;
    public bool drawHole = false;
	public List<GuideStepData> mStepList;

    public GuideStepData mCurStepData;
	byte nSeat = 0;
	Dictionary<byte, List<GuideStepData>> stepDatasDicts = new Dictionary<byte, List<GuideStepData>>();
	// Use this for initialization
	public static string savepath{
		get {
			return Application.dataPath + "/Arts/GameRes/Config/Bytes/Guide.byte";
		}
	}
	public GuideUIRef mviewer;

    public void Awake() {
        UICamera.ChkGameObjCanClick = FilterClick;
        this.mviewer = FindObjectOfType<GuideUIRef>();
        OpenData();
        this.SetStep(0);
    }

	void OnDetroy(){
		UICamera.ChkGameObjCanClick = null;
	}

	bool FilterClick(GameObject go){
        return Input.GetKey(KeyCode.LeftControl) == false;
	}

	public void UpdateData()
	{
		mStepList = stepDatasDicts [nSeat];
        this.SetStep(0);
	}

	public void Init(){
        OpenData();
        this.SetStep(0);
	}

    public void SetStep(int step) {
        currentStepIdx = Mathf.Clamp(step, 0, this.mStepList.Count - 1);
        this.mCurStepData = this.mStepList[currentStepIdx];
        this.EventType = this.mCurStepData.EventType;
    }
    public void Insert() {//插入新步骤
        GuideStepData sdata = new GuideStepData();
        this.mStepList.Insert(this.currentStepIdx, sdata);
        this.SetStep(this.currentStepIdx);
    }
    public void Remove() {//移除当前步骤
        this.mStepList.RemoveAt(this.currentStepIdx);
        this.SetStep(this.currentStepIdx);
    }

    private Vector3 mDrawStartPos;
    private bool mIsTouch;
	void Update () {
		if (mviewer == null)
			return;
		if (nSeat != (byte)seat) {
			nSeat = (byte)seat;
			mStepList = stepDatasDicts [nSeat];
            this.SetStep(0);
		}
		if (Input.GetMouseButton (1)) {
			UnityEditor.Selection.activeGameObject = this.gameObject;
		}

        if (Input.GetKey(KeyCode.LeftControl)) {
            if (Input.GetKeyUp(KeyCode.LeftControl)) {
                this.mIsTouch = false;
            }
            if (Input.GetMouseButtonDown(0)) {
                this.mIsTouch = true;
                this.mDrawStartPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0)) {
                this.mIsTouch = false;
            }

            if (this.mIsTouch) {
                Vector2 pos1 = this.mDrawStartPos;
                Vector2 pos2 = Input.mousePosition;
                pos1.x /= Screen.width;
                pos1.y /= Screen.height;
                pos2.x /= Screen.width;
                pos2.y /= Screen.height;
                this.mCurStepData.holeRect.x = Mathf.Min(pos1.x, pos2.x);
                this.mCurStepData.holeRect.y = 1 - Mathf.Max(pos1.y, pos2.y);
                this.mCurStepData.holeRect.z = Mathf.Max(pos1.x, pos2.x);
                this.mCurStepData.holeRect.w = 1 - Mathf.Min(pos1.y, pos2.y);
            }
        }

		if (this.mCurStepData != null){
			if (this.mCurStepData.textMaxSize != Vector2.zero) {
                mviewer.dialogText.width = (int)Mathf.Max(this.mCurStepData.textMaxSize.x, 400f);
                mviewer.dialogText.height = (int)Mathf.Max(this.mCurStepData.textMaxSize.y, 90f);
			}
            Vector4 v4 = this.mCurStepData.holeRect;
			mviewer.ShowMsg (StringTable.GetString (string.Format("GuideTips{0}", currentStepIdx)));
            mviewer.setFingerPos(this.mCurStepData.fingerPos);
            mviewer.npc.SetActive(this.mCurStepData.IsShowNPC);
            mviewer.titleLabel.gameObject.SetActive(this.mCurStepData.IsShowNPC);
			mviewer.setHoleRect(v4);
            mviewer.setDialogPosition(this.mCurStepData.dialogPosition);
            mviewer.arrowDirect = this.mCurStepData.ArrowDir;
            mviewer.range = this.mCurStepData.ArrowRange;
		}
	}

	public int MaxStep = 1;
	[ContextMenu("Open Data")]
	void OpenData(){
		if (File.Exists(savepath))
			stepDatasDicts = ConfigTables.ParseGuideConfig (File.ReadAllBytes(savepath));
		mStepList = stepDatasDicts [(byte)nSeat];
		MaxStep = mStepList.Count;

        //foreach (var mGuideStepList in stepDatasDicts.Values) {
        //    if (mGuideStepList.Count < 26) {
        //        continue;
        //    }
        //    mGuideStepList[5].EventType = GuideEventType.ShowLCTList;
        //    mGuideStepList[6].EventType = GuideEventType.HideLCTList;
        //    mGuideStepList[7].EventType = GuideEventType.ShowRateList;
        //    mGuideStepList[8].EventType = GuideEventType.HideRateList;
        //    mGuideStepList[11].EventType = GuideEventType.ShowHeroList;
        //    mGuideStepList[12].EventType = GuideEventType.HideHeroList;
        //    mGuideStepList[13].EventType = GuideEventType.ShowQuickBuy;

        //    mGuideStepList[15].EventType = GuideEventType.HideQuickBuy;


        //    mGuideStepList[16].EventType = GuideEventType.ShowExit;
        //    mGuideStepList[18].EventType = GuideEventType.HideExit;

        //    mGuideStepList[20].EventType = GuideEventType.ShowSetting;
        //    mGuideStepList[21].EventType = GuideEventType.HideSetting;

        //    mGuideStepList[22].EventType = GuideEventType.ShowTuJian_TeShu;
        //    mGuideStepList[23].EventType = GuideEventType.ShowTuJian_PuTong;
        //    mGuideStepList[24].EventType = GuideEventType.ShowTuJian_PaoTai;
        //    mGuideStepList[25].EventType = GuideEventType.HideTuJian;

        //    mGuideStepList[26].EventType = GuideEventType.HideMenuList;
        //}
	}

	[ContextMenu("Save Data")]
	void _SaveData(){
		SaveData (stepDatasDicts);
		mStepList = stepDatasDicts [(byte)nSeat];
		LogMgr.Log ("保存成功.");
	}

	public static void SaveData(Dictionary<byte, List<GuideStepData>> guideDict){
		using (FileStream ms = new FileStream (savepath, FileMode.Create)) {
			BinaryWriter bw = new BinaryWriter (ms);
			byte s = 0;
			byte[] ss = new byte[guideDict.Keys.Count];
			guideDict.Keys.CopyTo (ss, 0);
			int cnt = guideDict.Count;
			List<GuideStepData> mstpList;
			bw.Write ((byte)cnt);
			while (s < cnt) {
				mstpList = guideDict [ss[s]];
				bw.Write (ss[s]);
				bw.Write (mstpList.Count);
				for (int i = 0; i < mstpList.Count; i++) {
                    bw.Write((int)mstpList[i].EventType);
					bw.Write (mstpList [i].textMaxSize.x);
					bw.Write (mstpList [i].textMaxSize.y);

					bw.Write (mstpList [i].dialogPosition.x);
					bw.Write (mstpList [i].dialogPosition.y);
					bw.Write (mstpList [i].dialogPosition.z);

					bw.Write (mstpList [i].fingerPos.x);
					bw.Write (mstpList [i].fingerPos.y);
					bw.Write (mstpList [i].fingerPos.z);

					Vector4 v4 = mstpList [i].holeRect;
					bw.Write (v4.x);
					bw.Write (v4.y);
					bw.Write (v4.z);
					bw.Write (v4.w);

					bw.Write (i.ToString());
					bw.Write (mstpList [i].ArrowDir);
					bw.Write (mstpList [i].ArrowRange);
					bw.Write (mstpList [i].IsShowNPC);
				}
				s++;
			}
			bw.Flush ();
		}
	}
}

#endif