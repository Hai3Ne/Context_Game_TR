using UnityEngine;
using System.Collections;

public class IceThorn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public void OnEffect(Transform parentTrans){
		var trans = parentTrans.GetChild (1).transform;
		this.transform.position = trans.position;
		this.transform.rotation = trans.rotation;
		this.transform.localScale = trans.lossyScale;
	}
}
