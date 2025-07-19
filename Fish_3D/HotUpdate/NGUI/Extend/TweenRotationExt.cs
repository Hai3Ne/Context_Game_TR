using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween Rotation")]
public class TweenRotationExt : UITweener
{
	public Vector3 from;
	public Vector3 to;
	public bool quaternionLerp = false;

	Transform mTrans;
	public System.Action<Vector3> OnUpdateValue;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	[System.Obsolete("Use 'value' instead")]
	public Quaternion rotation { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Quaternion value { get { return cachedTransform.localRotation; } set { cachedTransform.localRotation = value; } }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
		float rxx = Mathf.Lerp(from.x, to.x, factor);
		float ryy = Mathf.Lerp(from.y, to.y, factor);
		float rzz = Mathf.Lerp(from.z, to.z, factor);
		Vector3 euler = new Vector3 (rxx % 360f, ryy % 360f, rzz % 360f);
		value = Quaternion.Euler (euler);
		OnUpdateValue.TryCall (new Vector3(rxx, ryy, rzz));
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenRotationExt Begin (GameObject go, float duration, Vector3 rot)
	{
		TweenRotationExt comp = UITweener.Begin<TweenRotationExt>(go, duration);
		comp.from = comp.value.eulerAngles;
		comp.to = rot;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}


	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value.eulerAngles; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value.eulerAngles; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = Quaternion.Euler(from); }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = Quaternion.Euler(to); }
}
