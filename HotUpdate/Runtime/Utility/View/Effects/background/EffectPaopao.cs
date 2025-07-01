using UnityEngine;
using System.Collections;

public class pselem:MonoBehaviour
{
	
}

public class EffectPaopao : MonoBehaviour {

	public static int startidx = 10;
	public ParticleSystem[] psList;
	public Material sharedMaterial;
	public pselem[] psEList;
	// Use this for initialization
	void Awake(){
		for (int i = 0; i < psList.Length; i++) {
			psList [i].gameObject.AddComponent<pselem> ();
		}
	}

	IEnumerator Start () {
		yield return 0;
		yield return 0;
		psEList = GetComponentsInChildren<pselem> ();
		Renderer render = null;
		for (int i = 0; i < psEList.Length; i++) {			
			render = psEList [i].GetComponent<Renderer> ();
			render.material = null;
			render.sharedMaterial = null;
		}
		yield return 0;
		for (int i = 0; i < psEList.Length; i++) {
			render = psEList [i].GetComponent<Renderer> ();
			render.sharedMaterials = new Material[]{ sharedMaterial};
			render.sharedMaterial = sharedMaterial;
			render.sortingOrder = startidx + i;
		}
	}
}
