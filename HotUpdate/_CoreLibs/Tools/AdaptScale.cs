using UnityEngine;
using System.Collections;

public class AdaptScale : MonoBehaviour {
    private Vector3 mInitScale = Vector3.zero;//初始缩放值

	void Start () {
        if (this.mInitScale == Vector3.zero) {
            this.mInitScale = this.transform.localScale;
            this.mInitScale.x *= Resolution.AdaptAspect;
        }
        this.transform.localScale = this.mInitScale;
	}
	
}
