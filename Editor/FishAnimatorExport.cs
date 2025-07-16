using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using System.IO;

public class FishAnimatorExport
{
	class HeroHitOnVo{
		public float subClip;
		public float length;
		public float[] hitOnTime;
	}

	class FishAnimtorStatusVo
	{
		public float Swim, Idle, Dead, Laugh, Attack, BeAttack = 0f,Dizzy;	
	}
	static Dictionary<uint, FishAnimtorStatusVo> fishAnimatorDict = new Dictionary<uint, FishAnimtorStatusVo>();
	public static void exportAnim()
	{
		uint fcfgid = 0;
		string fpath = Application.dataPath + "/Arts/GameRes/Prefabs/Models";
		fishAnimatorDict.Clear ();
		string[] fs = Directory.GetFiles(fpath);
		foreach (var f in fs) 
		{
			string fn = f.Substring (f.LastIndexOf ("\\") + 1);
			if (fn.EndsWith (".meta"))
				continue;
			if (!fn.EndsWith (".prefab"))
				continue;
			
			if (fn.StartsWith ("Fish")) 
			{
				fn = fn.Substring (0, fn.LastIndexOf ("."));
				string rr = fn.Substring (4);
				if (uint.TryParse (rr, out fcfgid)) {
					string resId = string.Format ("Assets/Arts/GameRes/Prefabs/Models/Fish{0}.prefab", fcfgid);
					GameObject fgo = AssetDatabase.LoadAssetAtPath<GameObject> (resId);
                    if (fgo == null) {
                        Debug.LogError("找不到模型  path:" + resId);
                    }
					HandleFish (fgo, fcfgid);
				}
			}
		}

		using (MemoryStream ms = new MemoryStream ()) 
		{
			BinaryWriter bw = new BinaryWriter (ms);
			bw.Write (fishAnimatorDict.Count);
			foreach (var pair in fishAnimatorDict) {
				bw.Write (pair.Key);
				bw.Write (pair.Value.Swim);
				bw.Write (pair.Value.Idle);
				bw.Write (pair.Value.Dead);
				bw.Write (pair.Value.Laugh);
				bw.Write (pair.Value.Attack);
				bw.Write (pair.Value.BeAttack);
				bw.Write (pair.Value.Dizzy);
			}
			byte[] buffer = ms.GetBuffer ();
			File.WriteAllBytes (Application.dataPath + "/Arts/GameRes/Config/Bytes/FishAnimtor.byte", buffer);
			//EditorUtility.DisplayDialog ("Succfull","到处动画信息成功~","OK");
		}
	}

	static void HandleFish(GameObject fishGo, uint fishID)
	{
		Animator mAnimator = fishGo.GetComponent<Animator>();
        if (mAnimator == null) {
            mAnimator = fishGo.GetComponentInChildren<Animator>(true);
        }
		AnimatorController ac = mAnimator.runtimeAnimatorController as AnimatorController;
		if (ac == null)
			return;
		

		Debug.Log ("FishID:" + fishID+" mAnimator "+mAnimator.name);
		FishAnimtorStatusVo stateInfo = new FishAnimtorStatusVo ();
		foreach (var sta in ac.layers[0].stateMachine.states) 
		{
		//	Debug.Log (sta.state.name);
		//	Debug.Log (sta.state.name+" : "+sta.state.motion.averageDuration);
			switch (sta.state.name.ToLower()) {
			case "swim":
				stateInfo.Swim = sta.state.motion.averageDuration;
				break;
			case "idle":
				stateInfo.Idle = sta.state.motion.averageDuration;
				break;
			case "dead":
				stateInfo.Dead = sta.state.motion.averageDuration;
				break;
			case "laugh":
				stateInfo.Laugh = sta.state.motion.averageDuration;
				break;
			case "attack":
				stateInfo.Attack = sta.state.motion.averageDuration;
				break;
			case "beattack":
				stateInfo.BeAttack = sta.state.motion.averageDuration;
				break;
			case "dizzy":
				stateInfo.Dizzy = sta.state.motion.averageDuration;
				break;
			}
		}
		fishAnimatorDict [fishID] = stateInfo;
	}

	static Dictionary<uint, HeroHitOnVo[]> heroAnimatorDict = new Dictionary<uint, HeroHitOnVo[]>();
	public static void exportHeroAnim()
	{
		heroAnimatorDict.Clear ();
		uint fcfgid = 0;
		string fpath = Application.dataPath + "/Arts/GameRes/Prefabs/Heroes";
		string[] fs = Directory.GetFiles(fpath);
		int total = Mathf.FloorToInt(fs.Length*0.5f);
		int idx = 0;
		foreach (var f in fs) 
		{
			string fn = f.Substring (f.LastIndexOf ("\\") + 1);
			if (fn.EndsWith (".meta"))
				continue;
			if (!fn.EndsWith (".prefab"))
				continue;

			if (fn.StartsWith ("Hero")) 
			{
				fn = fn.Substring (0, fn.LastIndexOf ("."));
				string rr = fn.Substring (4);
				if (uint.TryParse (rr, out fcfgid)) {
					string resId = string.Format ("Assets/Arts/GameRes/Prefabs/Heroes/Hero{0}.prefab", fcfgid);
					GameObject fgo = AssetDatabase.LoadAssetAtPath<GameObject> (resId);
					HandleHero (fgo, fcfgid);
				}
				float per = idx *1.0f/ total;
				EditorUtility.DisplayProgressBar ("导出Hero Motions", "导出中", 0.9f*per);
			}
		}

		using (MemoryStream ms = new MemoryStream ()) 
		{
			BinaryWriter bw = new BinaryWriter (ms);
			bw.Write (heroAnimatorDict.Count);
			foreach (var pair in heroAnimatorDict) {
				bw.Write (pair.Key);
				bw.Write ((byte)pair.Value.Length);
				for (int i = 0; i < pair.Value.Length; i++) {
					bw.Write ((byte)i);
					bw.Write(pair.Value[i].length);
					bw.Write ((byte)pair.Value [i].hitOnTime.Length);
					for (int j = 0; j < pair.Value[i].hitOnTime.Length; j++){
						bw.Write(pair.Value[i].hitOnTime[j]);
					}
				}
			}
			EditorUtility.DisplayProgressBar ("导出Hero Motions", "导出中", 1f);
			byte[] buffer = ms.GetBuffer ();
			byte[] data = new byte[ms.Length];
			System.Array.Copy (buffer, 0, data, 0, ms.Length);
			File.WriteAllBytes (Application.dataPath + "/Arts/GameRes/Config/Bytes/HeroAnimator.byte", data);
			EditorUtility.ClearProgressBar ();
		}

	}


	static void HandleHero(GameObject heroGo, uint heroCfgID){
		Animator mAnimator = heroGo.transform.GetChild(0).GetComponent<Animator>();
		AnimatorController ac = mAnimator.runtimeAnimatorController as AnimatorController;
		if (ac == null)
			return;
		LogMgr.Log ("HEROID" + heroCfgID);
		BlendTree btree = System.Array.Find(ac.layers [0].stateMachine.states,x=>x.state.name=="Attack").state.motion as BlendTree;
		int subclip = 0;
		HeroHitOnVo[] hitOnArray = new HeroHitOnVo[btree.children.Length];
		for (subclip = 0; subclip < btree.children.Length; subclip++) {
			AnimationClip clip = btree.children[subclip].motion as AnimationClip;
			var evts = AnimationUtility.GetAnimationEvents (clip);
			List<float> hitOnTimes = new List<float> ();
			foreach (var ev in evts) {
				if (ev.functionName == "OnAnimEvent" && ev.stringParameter == "HitOn") {
					LogMgr.Log (subclip+" ["+clip.name +"] "+ev.time+"  "+clip.length);
					hitOnTimes.Add (ev.time);
				}
			}
			HeroHitOnVo hitOnvo = new HeroHitOnVo ();
			hitOnvo.subClip = subclip;
			hitOnvo.length = clip.length;
			hitOnvo.hitOnTime = hitOnTimes.ToArray();
			hitOnArray [subclip] = hitOnvo;
		}
		if (hitOnArray.Length > 0)
			heroAnimatorDict.Add (heroCfgID, hitOnArray);
	}
}
