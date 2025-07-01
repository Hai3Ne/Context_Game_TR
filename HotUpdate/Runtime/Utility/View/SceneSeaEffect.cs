using UnityEngine;
using System.Collections;

public class SceneSeaEffect : MonoBehaviour {
    public static SceneSeaEffect instance;
	public Material waterMat;
	public Shader waterShader;

    void Start() {
        CheckResource();
#if UNITY_EDITOR
        if (this.waterMat != null) {
            this.waterMat = new Material(this.waterMat);
        }
#endif
    }
    public void Awake() {
        instance = this;
    }

    public void OnDestroy() {
        instance = null;
    }

	
	protected void CheckResource()
	{
		bool isSupported = CheckSupport ();
		if (isSupported == false) {
			NotSupport ();
		}
	}

	bool CheckSupport() {
		if (SystemInfo.supportsImageEffects == false)
		{
			LogMgr.LogWarning ("This platform does not support image effects or render textures.");	
			return false;
		}
		return true;
	}

	void NotSupport()
	{
		enabled = false;
	}

	public Material CheckShaderAndCreateMaterial(Shader shader, Material material){
		if (shader == null)
			return null;
		if (shader.isSupported && material && material.shader == shader)
			return material;
		else {
			material = new Material (shader);
			material.hideFlags = HideFlags.DontSave;
			if (material)
				return material;
			return null;
		}
	}
    [System.NonSerialized]
    public bool mIsAnimHide;//是否开始消失
    [System.NonSerialized]
    public bool mIsAnimShow;//是否开始显示
    private float mStartTime;//结束时间

    public void StartHide() {//开始消失
        if (this.mIsAnimHide == false) {
            this.mIsAnimHide = true;
            this.mStartTime = Time.time;
        }
    }
    public void StartPalyAnim() {
        if (this.mIsAnimHide) {
            return;
        }
        this.mStartTime = Time.time;
        this.mIsAnimShow = true;
    }

    private const string AnimColorVal = "_AnimColorVal";
    private const string AnimRange = "_AnimRange";
	void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
        Material mat = waterMat;// CheckShaderAndCreateMaterial(waterShader, waterMat);
        if (this.mIsAnimHide) {//由白到黑
            float _t = (Time.time - this.mStartTime);
            if (_t < 1) {
                mat.SetFloat(AnimColorVal, Mathf.Min(mat.GetFloat(AnimColorVal), 1-_t));
                mat.SetFloat(AnimRange, Mathf.Max(mat.GetFloat(AnimRange), Mathf.Lerp(1, 200, _t)));
            } else {
                mat.SetFloat(AnimColorVal, 0);
                mat.SetFloat(AnimRange, 1);
                this.mIsAnimHide = false;
                this.StartPalyAnim();
            }
        }else if (this.mIsAnimShow) {//由黑到白
            float _t = Time.time- this.mStartTime;
            if (_t < 0.5f) {
                mat.SetFloat(AnimColorVal, 0);
                mat.SetFloat(AnimRange, 400);
            }else if (_t < 2.5f) {
                _t = (_t-0.5f)*0.5f;
                mat.SetFloat(AnimColorVal, _t);
                mat.SetFloat(AnimRange, Mathf.Lerp(400, 1, _t));
            } else {
                mat.SetFloat(AnimColorVal, 1);
                mat.SetFloat(AnimRange, 1);
                this.mIsAnimShow = false;
            }
        }
		if (mat) {
			Graphics.Blit (src, dest, mat);
		} else {
			Graphics.Blit (src, dest);
		}
	}
}
