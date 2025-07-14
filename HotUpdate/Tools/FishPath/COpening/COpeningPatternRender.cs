using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
public class COpeningPatternRender : MonoBehaviour
{
	public OpeningParadeData[] Data;

	int testIndex = 0;
	public void TestLaunchFishes(int testId = -1)
	{
		if (testId >= 0) {
			lcrParade (Data [testId].mFishParade);
			return;
		}
		testIndex = 0;
		stage = 1;
	}

	int stage = -1;
	float time = 0f;
	void Update()
	{
		if (stage == 0) 
		{
			if (testIndex >= Data.Length) {
				stage = -1;
				return;
			}
			if (time >= Data [testIndex].delay) {
				time = 0;
				stage = 1;
			} else {
				time += Time.deltaTime;
			}

		}
		else if (stage == 1) 
		{
			lcrParade (Data [testIndex].mFishParade);
			testIndex++;
			stage = 0;
		}
	}

	void lcrParade(FishParadeData parade)
	{
		for (int i = 0; i < parade.PathList.Length; i++) 
		{
			uint pathid = parade.PathList [i];
			PathLinearInterpolator pi = CFishPathEditor.only.FishData.m_PathInterpList.Find (x => x.pathUDID == pathid);
            GroupData gd = parade.GroupDataArray[i];
            if (gd.FishShapeID > 0) {
                GameObject fish_content = GameObject.Instantiate(FishResManager.Instance.FishShapeMap.TryGet(gd.FishShapeID));
                FishShapeContent content = fish_content.AddComponent<FishShapeContent>();
                content.SetOffSet(gd.ShapeOffset);
                content.transform.localScale *= gd.ShapeScale;

                MeshFilter[] mfs = fish_content.GetComponentsInChildren<MeshFilter>();
                List<Vector3> pos_list;
                foreach (var item in mfs) {
                    pos_list = GameUtils.CreateFishPos(item.sharedMesh, gd.Density);

                    for (int j = 0; j < pos_list.Count; ++j) {
                        float time = 0;// GameUtils.GetPathTimeByDist(parade.FrontPosition.x, pos_list[j].x, pi);
                        Fish fish = new Fish();
                        FishVo fishvo = CFishPathEditor.only.GetFishVo(gd.FishCfgID);
                        float fspeed = fishvo.Speed * gd.SpeedScaling;
                        float fscale = fishvo.Scale * gd.FishScaling;
                        fish.Init(++CFishPathEditor.FishID, gd.FishCfgID, fscale, time, gd.ActionSpeed, fspeed, pi);
                        fish.SetOffset(pos_list[j]);

                        fish.SetPostLaunch(0);
                        fish.SetFishShape(item.transform, content);

                        SetFish(fish);
                    }
                }
            } else {
                if (gd.FishNum > gd.PosList.Length) {
                    LogMgr.Log("错误的鱼群路径点:" + gd.FishNum + ", posnum:" + gd.PosList.Length);
                    return;
                }
                for (int j = 0; j < gd.FishNum; ++j) {
                    float time = 0;// GameUtils.GetPathTimeByDist(parade.FrontPosition.x, gd.PosList[j].x, pi);
                    Fish fish = new Fish();
                    FishVo fishvo = CFishPathEditor.only.GetFishVo(gd.FishCfgID);
                    float fspeed = fishvo.Speed * gd.SpeedScaling;
                    float fscale = fishvo.Scale * gd.FishScaling;
                    fish.Init(++CFishPathEditor.FishID, gd.FishCfgID, fscale, time, gd.ActionSpeed, fspeed, pi);
                    fish.SetOffset(new Vector3(gd.PosList[i].x, 1 * gd.PosList[j].y, gd.PosList[j].z));

                    fish.SetPostLaunch(gd.DelayList[j]);
                    SetFish(fish);
                }
            }
		}
	}

	private void SetFish(Fish fish)
	{
		if (null != fish)
		{
			if (CFishPathEditor.only)
			{
				CFishPathEditor.only.AddFishToList(fish);
			}
		}
	}
}
#endif