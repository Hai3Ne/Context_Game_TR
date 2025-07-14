using UnityEngine;
using System.Collections;

public class AutoLoopPs : MonoBehaviour {

	float totalSecs = 0f, startTime = 0f;
	void Start () {
		totalSecs = GameUtils.CalPSLife (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		startTime += Time.deltaTime;
		if (startTime >= totalSecs) {
			GameUtils.PlayPS (this.gameObject);
			startTime = 0f;
		}
	}
}
