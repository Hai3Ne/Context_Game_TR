using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {
    public Vector3 RotateSpd;//角度旋转速度
    public float AngleSped = 3.6f;//每秒旋转角度

	public void Update () {
        if (this.AngleSped > 0) {
            this.transform.localEulerAngles += new Vector3(0, this.AngleSped * Time.deltaTime);
        } else {
            this.transform.Rotate(this.RotateSpd * Time.deltaTime);
        }
	}
}
