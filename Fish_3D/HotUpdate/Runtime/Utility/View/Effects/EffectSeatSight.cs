using UnityEngine;
using System.Collections;

public class EffectSeatSight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Renderer[] renders = this.GetComponentsInChildren<Renderer>();
        //int sortingLayerID = 5;
		foreach (var r in renders) {
			r.sortingLayerName = "Top";
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
