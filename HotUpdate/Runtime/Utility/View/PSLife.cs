using UnityEngine;
using System.Collections;

public class PSLife : MonoBehaviour {
	public float duration = 1f;

	System.Action cb = null;
	ParticleSystem[] pslist;

	void Start(){
		if (pslist == null)
			pslist = GetComponentsInChildren<ParticleSystem> ();


	}

    void Update() {
        for (int i = 0; i < pslist.Length; i++) {
            if (pslist[i].isStopped == false && pslist[i].main.loop == false) {
                return;
            }
        }

        cb.TryCall();
        cb = null;
        GameObject.Destroy(this.gameObject);
	}

	public static void Begin(GameObject go, System.Action cb){
		var psl = go.GetComponent<PSLife> ();
		if (psl == null)
			psl = go.AddComponent<PSLife> ();
		psl.cb = cb;
	}
}
