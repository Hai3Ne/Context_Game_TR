using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR

public class COpeningParadeEditor : MonoBehaviour {

	public List<OpeningParadeData[]> mData;
	public bool IsEditorMode = true;

	public void LuachOpeningParade(int idx)
	{
		
	}

	public void RefreshDatas()
	{
		Transform MTrans = this.transform;
		while(MTrans.childCount>0)
		{
			GameObject.DestroyImmediate(MTrans.GetChild (0).gameObject);
		}

		for (int i = 0; i < mData.Count; i++) {
			GameObject itemGo = new GameObject (CFishPathEditor.OPENING_FISH_PARADE_ITEM_NAME+""+i);
			COpeningPatternRender itemPattern = itemGo.AddComponent<COpeningPatternRender> ();
			itemGo.transform.SetParent (MTrans);
			itemGo.transform.transform.localPosition = Vector3.zero;
			itemPattern.Data = mData [i];
			for (int j = 0; j < itemPattern.Data.Length; j++) {
				GameObject itemParade = new GameObject ("Parade"+j);
				COpeningPatternElementRender gropEditor = itemParade.AddComponent<COpeningPatternElementRender> ();
				gropEditor.m_OpeningParadeData = itemPattern.Data [j];
				gropEditor.transform.SetParent (itemPattern.transform);
				itemParade.transform.localPosition = Vector3.zero;
			}
		}
	}


	public void saveData()
	{
		List<OpeningParadeData[]> datas = new List<OpeningParadeData[]> ();
		for (int i = 0; i < this.transform.childCount; i++) {
			COpeningPatternRender patterR = this.transform.GetChild (i).GetComponent<COpeningPatternRender>();

			List<OpeningParadeData> parades = new List<OpeningParadeData> ();
			for (int j = 0; j < patterR.transform.childCount; j++) {
				COpeningPatternElementRender elem = patterR.transform.GetChild (j).GetComponent<COpeningPatternElementRender> ();
				elem.GetPoslist (elem.m_OpeningParadeData.mFishParade);
				parades.Add (elem.m_OpeningParadeData);
			}
			if (parades.Count > 0) {
                //parades.Sort (delegate(OpeningParadeData x, OpeningParadeData y) {
                //    return x.delay.CompareTo(y.delay);
                //});
				datas.Add (parades.ToArray ());
			}
		}

		mData = datas;
		byte[] buff = FishPathConfParser.Serialize_OpeningParades (mData);
		File.WriteAllBytes (Application.dataPath + CFishPathEditor.only.ConfigPath + CFishPathEditor.only.OpeningParadeFile, buff);
	}
}
#endif