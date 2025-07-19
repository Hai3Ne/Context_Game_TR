using UnityEngine;
using System.Collections;

public class EffectShake : MonoBehaviour 
{
	Vector3 offset_;
	Vector3 orginPosition;
	public float delay = 0f;
	int current_shake_frames = 0;
	int kShakFrames = 50;
	// Use this for initialization
	void Start () {
		orginPosition = Camera.main.transform.position;
		current_shake_frames = 0;
	}

	public bool shake_screen = false;
	// Update is called once per frame
	void Update () 
	{
		delay -= Time.deltaTime;
		if (delay > 0) {
			return;
		}

		if (!shake_screen) {
			orginPosition = Camera.main.transform.position;
			return;
		}

		if (current_shake_frames + 1 == kShakFrames) {
			Camera.main.transform.position = orginPosition;
			current_shake_frames = 0;
			shake_screen = false;
			return;
		}
		offset_.x = Random.Range (0, 100) % 2 == 0 ? (1f + Random.Range (0f, 2f)) : (-1f + Random.Range (-2f, 0f));
		offset_.y = Random.Range (0, 100) % 2 == 1 ? (1f + Random.Range (0f, 2f)) : (-1f + Random.Range (-2f, 0f));
		offset_.z = (1f + Random.Range (0f, 2f));
		Camera.main.transform.position = orginPosition + offset_;
		++current_shake_frames;
	}

	void OnDestroy()
	{
		Camera.main.transform.position = orginPosition;
	}
}
