using UnityEngine;
using System.Collections.Generic;
using Kubility;

public class SceneObjMgr:SingleTon<SceneObjMgr>
{
    GameObject   m_UIRoot;
	Camera       m_UICamera,m_BgCamera, m_MainCamera, m_TopEffCamera;
    GameObject   m_UIRootPanel;
    Transform    m_UIPanelTransform;
    Transform    m_UIContainerTransform;

    public Transform UIContainerTransform
    {
        get { return m_UIContainerTransform; }
    }

	public GameObject UIRoot
    {
        get { return m_UIRoot; }
    }

    public void GlobalInit()
    {
        GetSceneObj();
    }
       
	public Camera BgCam
	{
		get {return m_BgCamera;}
	}

	public Camera MainCam
    {
        get {return m_MainCamera;}
    }

	public Camera SceneTopCamera
	{
		get {return m_TopEffCamera;}
	}


    void GetSceneObj()
    {
        InitUIRoot();
    }

	public MeshRenderer mBossBGMeshR;
    void InitUIRoot()
    {
		SceneRef viewRef = MainEntrace.Instance.GetComponent<SceneRef> ();

		m_MainCamera = viewRef.MainCam;
		m_BgCamera = viewRef.BgCam;
        m_UIRoot = GameObject.Find("SceneUIRoot");//viewRef.UIRootObj;
		m_TopEffCamera = viewRef.TopCam;
		mBossBGMeshR = viewRef.bossBackgroundRender;

		m_UICamera = m_UIRoot.transform.GetChild(0).GetComponent<Camera>();
		m_UIRootPanel = m_UIRoot.transform.GetChild(1).gameObject;
        m_UIPanelTransform = m_UIRootPanel.transform;
        m_UIContainerTransform = m_UIRootPanel.transform.GetChild(0);

        //viewRef.TopCam.aspect = m_UICamera.aspect = Camera.main.aspect;
        //viewRef.TopCam.rect   = m_UICamera.rect = Camera.main.rect;
        
        //UIRoot uroot = GameObject.FindObjectOfType<UIRoot> ();
		//uroot.manualWidth  = Resolution.ScreenWidth;
		//uroot.manualHeight = Resolution.ScreenHeight;

        this.SaveToCameraParam();
    }

	public void UpdateBossBackground(FishVo bossvo)
	{
		string path = ResPath.NewBossSceneBG + bossvo.BirthBG;
//        Kubility.KAssetBundleManger.Instance.ResourceLoad<Texture>(path, delegate(SmallAbStruct data)
//        {
//            Texture tex = (Texture)data.MainObject;
//#if UNITY_EDITOR
//            mBossBGMeshR.material.SetTexture("_MainTex", tex);
//#else
//            mBossBGMeshR.sharedMaterial.SetTexture("_MainTex", tex);
//#endif
//        });

        Texture tex = ResManager.LoadAsset<Texture>(GameEnum.Fish_3D,path);
#if UNITY_EDITOR
        mBossBGMeshR.material.SetTexture("_MainTex", tex);
#else
        mBossBGMeshR.sharedMaterial.SetTexture("_MainTex", tex);
#endif
    }

    public Transform UIPanelTransform
    {
        get { return m_UIPanelTransform; }
        set { m_UIPanelTransform = value; }
    }
    public Camera UICamera
    {
        get { return m_UICamera; }
        set { m_UICamera = value; }
    }
    public Vector2 MinUIPos {
        get {
            return new Vector2(-960, -540);
        }
    }
    public Vector2 MaxUIPos {
        get {
            return new Vector2(960, 540);
        }
    }
    
	bool mScreenMasked = false;

    int _top_cullingmask;
    CameraClearFlags _top_cflags;
    public void SaveToCameraParam() {
        _top_cullingmask = SceneObjMgr.Instance.SceneTopCamera.cullingMask;
        _top_cflags = SceneObjMgr.Instance.SceneTopCamera.clearFlags;
    }

    public void ResetTopCameraParam() {
        SceneObjMgr.Instance.SceneTopCamera.transform.localPosition = Vector3.zero;
        SceneObjMgr.Instance.SceneTopCamera.cullingMask = _top_cullingmask;
        SceneObjMgr.Instance.SceneTopCamera.clearFlags = _top_cflags;
    }
}
