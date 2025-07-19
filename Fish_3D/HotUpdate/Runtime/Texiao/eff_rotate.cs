using UnityEngine;
using System.Collections;

public class eff_rotate : MonoBehaviour {
    public float rotate_x;
    public float rotate_y;
    public float rotate_z;

    private Transform mTf;

	void Start () {
        this.mTf = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        this.mTf.Rotate(this.rotate_x * Time.deltaTime, this.rotate_y * Time.deltaTime, this.rotate_z * Time.deltaTime);
	}
}
