using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UITexture))]
public class UITextureClipHole : MonoBehaviour
{
	UITexture mTexture;
	public Vector4 clipRect = new Vector4(0f,0f,1f,1f);
	Vector4 mNowClipRect;
	public bool mIsDirty = false;
	void Start () {
		mTexture = GetComponent<UITexture> ();
		mTexture.shader = Shader.Find ("Unlit/Transparent ClipHole");

		Camera uiCam = mTexture.root.GetComponentInChildren<Camera> ();
		float w, h;
		h = mTexture.root.activeHeight;
		w = uiCam.aspect * h;
		mTexture.SetRect (0, 0, w, h);
		this.transform.localPosition = Vector3.zero;
		mIsDirty = true;

	}

	// Update is called once per frame
	void Update () {
		if (mTexture.drawCall == null)
			return;
		if (clipRect != mNowClipRect || mIsDirty || mTexture.drawCall.isDirty)
		{
			mNowClipRect = clipRect;
			mNowClipRect.z = Mathf.Max (mNowClipRect.x, mNowClipRect.z);
			mNowClipRect.w = Mathf.Max (mNowClipRect.y, mNowClipRect.w);
			mTexture.drawCall.dynamicMaterial.SetVector ("_ClipRange0", mNowClipRect);
			mIsDirty = false;
		}
	}

}


