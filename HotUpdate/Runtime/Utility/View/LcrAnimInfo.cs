using UnityEngine;
using System.Collections;

public class LcrAnimInfo : MonoBehaviour 
{
	[Header("子弹打出时间点")]
	public float fireTime = 0f;
	public Transform FireTrans, IdleTrans;
    public Transform FireIdleTrans;//炮口常驻特效点
	public UILabel lrLevelLabel;


}
