using UnityEngine;
using System.Collections;

public class InitPS : MonoBehaviour {

	public string[] prefabIDs;
	public Transform[] containers;
	// Use this for initialization
	void Start () {
		for (int i = 0; i < prefabIDs.Length; i++)
        {
            GameObject go = ResManager.LoadAndCreate(GameEnum.Fish_3D, prefabIDs[i], containers[i]);
            GameUtils.ResumeShader(go);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
