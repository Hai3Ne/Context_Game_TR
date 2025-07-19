using UnityEngine;
using System.Collections.Generic;

public class FishRootRef : MonoBehaviour {
    public ushort fishID;
    public uint fishCFGID;
    public ushort groupIdx = 0xFFFF;
    public uint pathid;

#if UNITY_EDITOR
    public bool isInView;
    public bool isInView_FUll;
    public bool isCenterInView;
    void Update() {
        if (SceneLogic.Instance != null && SceneLogic.Instance.FishMgr != null) {
            var f = SceneLogic.Instance.FishMgr.FindFishByID(fishID);
            if (f != null) {
                isInView = f.IsInView();
                isInView_FUll = f.IsInView(true);
                isCenterInView = f.IsInView_Center();
            }
        }
    }
#endif
}

public class Fish:IGlobalEffect,ISortZObj
{
	public static bool ENABLE_HIDE_OUTVIEW = false;
    public static Vector3 FishInitPos = new Vector3(ConstValue.START_POS, 0, 0);
	public static bool IsBossFirstShowing = false; // boss 亮相

    ushort          m_FishID;
    uint            m_FishCfgId;
    GameObject      m_Model;
    Animator        m_Anim;
	AnimationEventListener mAnimListener;
    Transform       m_ModelTransform;
    Vector3         m_Offset;
    PathController  m_PathCtrl;
	bool            m_DelayDestroy;            //延迟销毁
    float           m_Scaling;
    Quaternion      m_OrgRot;
    Vector3         m_WorldOffset;
    List<IFishOpt>  m_OptList;
    Vector3         m_Position;
    byte            m_CatchSeat = 0xFF;
    bool            m_bUpdateScreen;
    bool            m_bUpdateViewPos;
	bool            m_bUpdateBSize;

    Vector3         m_ScrPos;
  	Vector3         m_ViewPos;
    uint          m_nRewardDrop = 0;
    float           m_bgsoundDelay=0;
	bool isQuadMeshRender = false;
	FishVo mFishVo;
	float[] mClipLength;
    private float mAddScale = 1;//追加缩放显示
    public float mFristCatchTime;//首次捕获时间

    public FishShapeContent mFishShapeContent;//当前路径的父节点，组合形状使用
    public Transform mFishShapeParent;//形状父节点

	public Dictionary<int, GameObjRef> mCacheEffObjRefMap = new Dictionary<int, GameObjRef>();
    //zzm
    float           m_PostLaunchDuration = 0.0f;
    bool            m_IsPostLaunch = false;

    public void SetFishShape(Transform parent, FishShapeContent shape_content) {//设置形状队列
        this.mFishShapeContent = shape_content;
        this.mFishShapeParent = parent;
        this.mFishShapeContent.AddFish(this);
    }
    public void RemoveFishShape() {//从形状队列中移除
        if (this.mFishShapeContent != null) {
            this.mFishShapeContent.RemoveFish(this);
            this.mFishShapeContent = null;
            this.mFishShapeParent = null;
        }
    }
    public void SetPostLaunch(float duration) 
    {  
		if (duration >= 0.001f) {
			m_PostLaunchDuration = duration;
			m_IsPostLaunch = true;
		} else {
			m_IsPostLaunch = false;
		}
    }

	public static bool CheckIsKingFish(FishVo fishVo)
	{
		return (EnumFishType)fishVo.Type == EnumFishType.FishKing;
	}

	public static bool CheckIsBoss(FishVo fishVo)
	{
		return (EnumFishType)fishVo.Type == EnumFishType.Boss;
	}

	Dictionary<PathEventType,float>  MakePathEventTime(FishAnimtorStatusVo statvo)
	{
		Dictionary<PathEventType,float> dict = new Dictionary<PathEventType, float> ();
		dict [PathEventType.LAUGH] = statvo.Laugh;
		dict [PathEventType.ACTTACK] = statvo.Attack;
		dict [PathEventType.BEATTACK] = statvo.BeAttack;
		return dict;
	}

	public float GetClipLength(FishClipType clipType, int subClipIdx = 0)
	{
		int i = (int)clipType;
		if (i >= 0 && i < mClipLength.Length)
			return mClipLength[i];
		return 0;
	}

	float mDefSwinClip = 0f;
	float mActionSpeed;
    Renderer[] mRenders;
    List<Material> mModelRenderMaters = new List<Material>();//模型材质，不包含粒子
    List<string> mModeShaderNames = new List<string>();
	public void Init(ushort id, uint fishCfgID, float scl, float time, float actionSpeed, float speed, PathLinearInterpolator interp, byte defSwinClip = 0)
    {
        m_CatchSeat     = 0xff;
        m_DelayDestroy         = false;
        m_Scaling       = scl;
        mAddScale = 1;
        m_FishID        = id;
		m_FishCfgId      = fishCfgID;
		mDefSwinClip = (float)defSwinClip;

		mFishVo = FishConfig.Instance.FishConf.TryGet (fishCfgID);
		FishAnimtorStatusVo statvo = FishConfig.Instance.fishAnimatorConf.TryGet (mFishVo.SourceID);

		mClipLength = new float[]{statvo.Swim, statvo.Dead, statvo.Laugh, statvo.BeAttack, statvo.Attack, statvo.Dizzy};	
		mIsBossfish = CheckIsBoss (mFishVo);//
		mIsFishKing = CheckIsKingFish(mFishVo);
        if (interp != null) {
            m_PathCtrl = new PathController();
            m_PathCtrl.ResetController(interp, speed, actionSpeed, time, MakePathEventTime(statvo));
        }

		GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet (mFishVo.SourceID);
		m_Model = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);
		FishRootRef re =	m_Model.AddComponent<FishRootRef> ();
		re.fishID = id;
		re.fishCFGID = m_FishCfgId;
        if (interp != null) {
            if (interp.groupID != 0xFFFF) {
                re.groupIdx = interp.groupID;
            }
            re.pathid = interp.pathUDID;
        }

		GameUtils.SetGOLayer (m_Model, LayerMask.NameToLayer ("FishLayer"));
	    m_ModelTransform = m_Model.GetComponent<Transform>();
        this.Position = FishInitPos;
        m_Anim = m_Model.GetComponent<Animator>();
		if (m_Anim == null)
			m_Anim = m_ModelTransform.gameObject.GetComponentInChildren<Animator>();
        m_OrgRot = m_ModelTransform.localRotation;
        m_Model.name = m_FishID.ToString();
        SetScaling(scl);
		ModelMeshRenderRef renderRef = m_ModelTransform.GetComponent<ModelMeshRenderRef> ();

		if (renderRef != null) {
			mRenders = renderRef.skilledrenders;
			isQuadMeshRender = renderRef.isQuadMeshRender;
		} else {
			mRenders = m_ModelTransform.GetChild (0).GetComponentsInChildren<SkinnedMeshRenderer> ();
		}
        mModelRenderMaters.Clear();
        mModeShaderNames.Clear();
        for (int i = 0; i < mRenders.Length; i++) {
            mModelRenderMaters.AddRange(mRenders[i].materials);
        }
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModeShaderNames.Add(mModelRenderMaters[i].shader.name);
        }
		mActionSpeed =actionSpeed;
		ResetActionSpeed ();
//		if (!actionUnite && m_Anim.HasState (0, FishAnimatorStatusMgr.YouYongHashName))
//			m_Anim.Set_Integer (FishAnimatorStatusMgr.STATUS_SUBCLIP, (int)defSwinClip);
		ResetAnim(m_Anim, mDefSwinClip);
		mAnimListener = m_Anim.GetComponent<AnimationEventListener> ();
		if (mAnimListener != null)
			mAnimListener.OnAnimatorEvent += HandleBossEventHandle;
		
        if (IsBossFish)
        {
            m_bgsoundDelay = 2;
			if (SceneLogic.Instance.EffectMgr != null) {
				List<FishBossVo> fbossVos = FishConfig.Instance.FishBossConf.FindAll (x => x.CfgID == vo.CfgID);
				mMaxLifeNum = (byte)fbossVos.Count;
				SceneLogic.Instance.FishMgr.activedBoss = this;

                for (int i = 0; i < fbossVos.Count; i++) {
                    if (fbossVos[i].ID == this.BossLifeIndex) {
                        if (fbossVos[i].IsCritPoint) {
                            BaoJiDianManager.ShowBaoJiDian(this, fbossVos[i]);
                        }
                        break;
                    }
                }
                
			}
        }

		InitFishCollider ();
		if (m_IsPostLaunch)
			m_Model.SetActive (false);		
		m_bUpdateBSize = true;
		//int val = m_FishID - clientFishIDStartID;
		//setRenderQueue (1000 + val);
		//SetMatIntVal ("_RefVal", val);
    }
    public void SetFishPath(FishPath path_info,float speed, float actionSpeed, float time) {
        if (m_PathCtrl == null) {
            m_PathCtrl = new PathController();
        }
        m_PathCtrl.ResetController(path_info, speed, actionSpeed, time);
    }
    //public void Init(ushort id, uint fishCfgID, float scl, float time, float actionSpeed, float speed, FishPath path_info) {
    //    if (clientFishIDStartID == -1)
    //        clientFishIDStartID = id;
    //    m_CatchSeat = 0xff;
    //    m_DelayDestroy = false;
    //    m_Scaling = scl;
    //    mAddScale = 1;
    //    m_FishID = id;
    //    m_FishCfgId = fishCfgID;
    //    mDefSwinClip = 0;

    //    mFishVo = FishConfig.Instance.FishConf.TryGet(fishCfgID);
    //    FishAnimtorStatusVo statvo = FishConfig.Instance.fishAnimatorConf.TryGet(mFishVo.SourceID);

    //    mClipLength = new float[] { statvo.Swim, statvo.Dead, statvo.Laugh, statvo.BeAttack, statvo.Attack, statvo.Dizzy };
    //    mIsBossfish = CheckIsBoss(mFishVo);//
    //    mIsFishKing = CheckIsKingFish(mFishVo);
    //    m_PathCtrl = new PathController();
    //    m_PathCtrl.ResetController(path_info, speed, actionSpeed, time, MakePathEventTime(statvo));

    //    GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet(mFishVo.SourceID);
    //    m_Model = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);
    //    FishRootRef re = m_Model.AddComponent<FishRootRef>();
    //    re.initTime = time;
    //    re.fishID = id;
    //    re.fishCFGID = m_FishCfgId;

    //    GameUtils.SetGOLayer(m_Model, LayerMask.NameToLayer("FishLayer"));
    //    m_ModelTransform = m_Model.GetComponent<Transform>();
    //    this.Position = FishInitPos;
    //    m_Anim = m_Model.GetComponent<Animator>();
    //    if (m_Anim == null)
    //        m_Anim = m_ModelTransform.gameObject.GetComponentInChildren<Animator>();
    //    m_OrgRot = m_ModelTransform.localRotation;
    //    m_Model.name = m_FishID.ToString();
    //    SetScaling(scl);
    //    ModelMeshRenderRef renderRef = m_ModelTransform.GetComponent<ModelMeshRenderRef>();

    //    if (renderRef != null) {
    //        mRenders = renderRef.skilledrenders;
    //        isQuadMeshRender = renderRef.isQuadMeshRender;
    //    } else {
    //        mRenders = m_ModelTransform.GetChild(0).GetComponentsInChildren<SkinnedMeshRenderer>();
    //    }
    //    mModelRenderMaters.Clear();
    //    mModeShaderNames.Clear();
    //    for (int i = 0; i < mRenders.Length; i++) {
    //        mModelRenderMaters.AddRange(mRenders[i].materials);
    //    }
    //    for (int i = 0; i < mModelRenderMaters.Count; i++) {
    //        mModeShaderNames.Add(mModelRenderMaters[i].shader.name);
    //    }
    //    mActionSpeed = actionSpeed;
    //    ResetActionSpeed();
    //    ResetAnim(m_Anim, mDefSwinClip);
    //    mAnimListener = m_Anim.GetComponent<AnimationEventListener>();
    //    if (mAnimListener != null)
    //        mAnimListener.OnAnimatorEvent += HandleBossEventHandle;

    //    if (IsBossFish) {
    //        m_bgsoundDelay = 2;
    //        if (SceneLogic.Instance.EffectMgr != null) {
    //            List<FishBossVo> fbossVos = FishConfig.Instance.FishBossConf.FindAll(x => x.CfgID == vo.CfgID);
    //            mMaxLifeNum = (byte)fbossVos.Count;
    //            SceneLogic.Instance.FishMgr.activedBoss = this;

    //            for (int i = 0; i < fbossVos.Count; i++) {
    //                if (fbossVos[i].ID == this.BossLifeIndex) {
    //                    if (fbossVos[i].IsCritPoint) {
    //                        BaoJiDianManager.ShowBaoJiDian(this, fbossVos[i]);
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    InitFishCollider();
    //    if (m_IsPostLaunch)
    //        m_Model.SetActive(false);
    //    m_bUpdateBSize = true;
    //}
    public void ResetCfgID(uint cfg_id) {//重设鱼的cfgID   目前应用于潘多拉
        this.m_FishCfgId = cfg_id;
        this.mFishVo = FishConfig.Instance.FishConf.TryGet(this.m_FishCfgId);

        FishAnimtorStatusVo statvo = FishConfig.Instance.fishAnimatorConf.TryGet(mFishVo.SourceID);
        mClipLength = new float[] { statvo.Swim, statvo.Dead, statvo.Laugh, statvo.BeAttack, statvo.Attack, statvo.Dizzy };
        mIsBossfish = CheckIsBoss(mFishVo);//
        mIsFishKing = CheckIsKingFish(mFishVo);

        GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet(mFishVo.SourceID);
        FishRootRef root_ref = m_Model.GetComponent<FishRootRef>();
        m_Model = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);
        FishRootRef re = m_Model.AddComponent<FishRootRef>();
        re.fishID = root_ref.fishID;
        re.fishCFGID = m_FishCfgId;
        re.groupIdx = root_ref.groupIdx;
        re.pathid = root_ref.pathid;
        //GameObject.Destroy(root_ref.gameObject);//删除之前模型

        GameUtils.SetGOLayer(m_Model, LayerMask.NameToLayer("FishLayer"));
        m_ModelTransform = m_Model.GetComponent<Transform>();
        this.Position = this.m_Position;
        m_Anim = m_Model.GetComponent<Animator>();
        if (m_Anim == null)
            m_Anim = m_ModelTransform.gameObject.GetComponentInChildren<Animator>();
        m_OrgRot = m_ModelTransform.localRotation;
        m_Model.name = m_FishID.ToString();
        this.m_Scaling = this.mFishVo.CallScale;
        SetScaling(this.m_Scaling);
        ModelMeshRenderRef renderRef = m_ModelTransform.GetComponent<ModelMeshRenderRef>();
        if (renderRef != null) {
            mRenders = renderRef.skilledrenders;
            isQuadMeshRender = renderRef.isQuadMeshRender;
        } else {
            mRenders = m_ModelTransform.GetChild(0).GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        mModelRenderMaters.Clear();
        mModeShaderNames.Clear();
        for (int i = 0; i < mRenders.Length; i++) {
            mModelRenderMaters.AddRange(mRenders[i].materials);
        }
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModeShaderNames.Add(mModelRenderMaters[i].shader.name);
        }
        ResetActionSpeed();
        ResetAnim(m_Anim, mDefSwinClip);
        mAnimListener = m_Anim.GetComponent<AnimationEventListener>();
        if (mAnimListener != null)
            mAnimListener.OnAnimatorEvent += HandleBossEventHandle;

        if (IsBossFish) {
            m_bgsoundDelay = 2;
            if (SceneLogic.Instance.EffectMgr != null) {
                List<FishBossVo> fbossVos = FishConfig.Instance.FishBossConf.FindAll(x => x.CfgID == vo.CfgID);
                mMaxLifeNum = (byte)fbossVos.Count;
                SceneLogic.Instance.FishMgr.activedBoss = this;

                for (int i = 0; i < fbossVos.Count; i++) {
                    if (fbossVos[i].ID == this.BossLifeIndex) {
                        if (fbossVos[i].IsCritPoint) {
                            BaoJiDianManager.ShowBaoJiDian(this, fbossVos[i]);
                        }
                        break;
                    }
                }

            }
        }

        InitFishCollider();
        if (m_IsPostLaunch)
            m_Model.SetActive(false);
        m_bUpdateBSize = true;

    }

    private float mWaitTime;
    public void SetWait(float time) {//设置操作等待时间  目前用于潘多拉变身鱼的时间
        this.mWaitTime = UnityEngine.Time.realtimeSinceStartup + time;
    }
    public float GetWait() {//获取剩余等待时间
        if (this.mWaitTime > UnityEngine.Time.realtimeSinceStartup) {
            return this.mWaitTime - UnityEngine.Time.realtimeSinceStartup;
        } else {
            return 0;
        }
    }

	public static void ResetAnim(Animator anim, float defSwinClip = 0)
	{
		foreach (var clip in FishAnimatorStatusMgr.ActionHashList) {
            anim.Set_Bool(clip, false);
		}

		anim.Set_Float(FishAnimatorStatusMgr.STATUS_SUBCLIP, defSwinClip);
        anim.Set_Bool(FishAnimatorStatusMgr.YouYongHashName, true);
        anim.Play(FishAnimatorStatusMgr.YouYongHashName, 0, Utility.Range(0.0f, 1.0f));
	}

    public GameObject Model {//鱼的模型
        get { return m_Model; }
    }

	bool mIsBossfish = false, mIsFishKing = false;
	public bool IsBossFish
	{
		get {return mIsBossfish;}
	}

	public bool IsKing
	{
		get { return mIsFishKing;}
	}


	byte mMaxLifeNum = 1;
	public byte MaxLifeNum
	{
		get {return mMaxLifeNum;}
	}

    private bool is_box = false;//是否进行碰撞检查
    Vector3[] mBSizeList = new Vector3[6];//鱼的碰撞区域
    public Vector3[] BSize2 {
        get {
            if (m_bUpdateBSize == false) {
                if (mFishCollider == null || m_Position.x == ConstValue.START_POS || m_Position.z < Utility.MainCam.nearClipPlane + 10f) {
                    is_box = false;
                } else {
                    Matrix4x4 cornMatrix = mFishCollider.transform.localToWorldMatrix;
                    for (byte i = 0; i < 8; i++) {
                        box_w_pos[i] = cornMatrix.MultiplyPoint(corns[i]);
                    }
                    GameUtils.PoList2Ver2List(box_w_pos, mBSizeList);

                    float minX = float.MaxValue;
                    float maxX = float.MinValue;
                    float min_y = float.MaxValue;
                    float max_y = float.MinValue;
                    foreach (var item in mBSizeList) {
                        if (minX > item.x) {
                            minX = item.x;
                        }
                        if (maxX < item.x) {
                            maxX = item.x;
                        }
                        if (min_y > item.y) {
                            min_y = item.y;
                        }
                        if (max_y < item.y) {
                            max_y = item.y;
                        }
                    }

                    if (minX > Resolution.ScreenWidth + 10 || maxX < -10 || min_y > Resolution.ScreenHeight + 10 || max_y < -10) {
                        is_box = false;
                    } else {
                        is_box = true;
                    }
                    //if ((minX >= -10f && minX <= Resolution.ScreenWidth + 10f) || (maxX >= -10f && maxX < Resolution.ScreenWidth + 10f)) {
                    //    //GameUtils.polish2Vector2List(screenCorns, mBSizeList);
                    //    GameUtils.PoList2Ver2List(box_w_pos, mBSizeList);
                    //}
                    //else {
                    //    mBSizeList.Clear();
                    //}
                }
                m_bUpdateBSize = true;
            }
            if (is_box) {
                return mBSizeList;
            } else {
                return null;
            }
        }
    }


	public bool IsInView_Center(){
		Matrix4x4 mvp = vp_mat * m_ModelTransform.localToWorldMatrix;
		Vector3 p = mvp.MultiplyPoint (Vector3.zero);
		const float side = 1f;
		return (p.y > -side && p.y < side && p.x > -side && p.x < side);
	}

	public bool IsInView(bool isFull = false)
	{
		if (Position == FishInitPos || m_Model.activeSelf == false)
			return false;
		Vector4 bound = CalBounds (m_ModelTransform.localToWorldMatrix);
		if (isFull) {
			const float side = 1.1f;
			return (bound.y > -side && bound.w < side && bound.z > -side && bound.x < side);
		} else {
			const float side = 1.1f;
			bool xxIn = (bound.y > -side && bound.y < side) || (bound.w > -side && bound.w < side);
			bool yyIn = (bound.x > -side && bound.x < side) || (bound.z > -side && bound.z < side);;
			return (xxIn && yyIn);
		}
	}

	bool _IsInView(Matrix4x4 ltwmat)
	{
		const float side = 1.2f;
		Vector4 b = CalBounds (ltwmat);
		float l = b.y;
		float r = b.z;
		return  (l > -side && l < side) || (r > -side && r < side);// || (l <= -1.2f && r >= 1.2f);
	}

	Vector4 CalBounds(Matrix4x4 ltwmat)
	{
		Matrix4x4 mvp = vp_mat * ltwmat;
		float l = float.MaxValue, r = float.MinValue;
		float b = float.MaxValue, t = float.MinValue;
		for (byte i = 0; i < corns.Length; i++) 
		{
			Vector3 p = mvp.MultiplyPoint (corns [i]);
			if (p.x < l) {
				l = p.x;
			} else if(p.x > r) {
				r = p.x;
			}

			if (p.y < b) {
				b = p.y;
			} else if (p.y > t) {
				t = p.y;
			}
		}
		return new Vector4 (t,l,b,r);
	}

	Vector3[] corns = new Vector3[8], worldCorns = new Vector3[8], box_w_pos = new Vector3[8];
	BoxCollider mFishCollider = null;
	BoxCollider[] bodyPartsBoxColliders;
	Matrix4x4 localToWorldMat, vp_mat;
	Vector3[][] bodyCorns = new Vector3[5][];
	void InitFishCollider()
	{
		vp_mat = Utility.MainCam.projectionMatrix * Utility.MainCam.worldToCameraMatrix;

		var BossPart_ColliderName = "FishBoxcollider".ToLower();
		int cornCount = 8;
		mFishCollider = m_Model.GetComponent<BoxCollider> ();
		if (mFishCollider == null) {
			BoxCollider[] css = m_Model.GetComponentsInChildren<BoxCollider> ();
			mFishCollider = System.Array.Find (css, x => x.name.ToLower() != BossPart_ColliderName);
		}

		if (mFishCollider != null) {
            Vector3 center = mFishCollider.center;
            Vector3 size = mFishCollider.size * 0.5f;
			for (byte i = 0; i < corns.Length; i++) {
                corns[i] = center + Vector3.Scale(ConstValue.BoxCorns[i], size);
			}
		}

		if (IsBossFish) {
			BoxCollider[] css = m_ModelTransform.GetComponentsInChildren<BoxCollider> ();
			Transform bodyColliderTrans = System.Array.Find (css, x => x.name.ToLower() == BossPart_ColliderName).transform;
			GameUtils.SetGOLayer(bodyColliderTrans.gameObject, LayerMask.NameToLayer("FishPartLayer"));
			bodyPartsBoxColliders = bodyColliderTrans.GetComponents<BoxCollider> ();
			for (int i = 0; i < bodyPartsBoxColliders.Length; i++) {
				bodyCorns [i] = new Vector3[cornCount];
				for (int j = 0; j < cornCount; j++) {
					bodyCorns [i][j] = bodyPartsBoxColliders [i].center + Vector3.Scale (ConstValue.BoxCorns [j], bodyPartsBoxColliders [i].size * 0.5f);
				}
			}
		}
		localToWorldMat = m_ModelTransform.localToWorldMatrix;
	}

	public BoxCollider Collider {get { return mFishCollider;}}
	public bool GetBodyPartScreenPos(short bodyIndex, out Vector3 outpos)
	{
		
		if (bodyPartsBoxColliders != null && bodyIndex >= 0 && bodyIndex < bodyPartsBoxColliders.Length) 
		{
			Vector3 wp = bodyPartsBoxColliders [bodyIndex].transform.TransformPoint (bodyPartsBoxColliders [bodyIndex].center);
			wp = Utility.MainCam.WorldToScreenPoint (wp);
			outpos = wp;
			return true;
		}
		outpos = ConstValue.UNVALIDE_SCREENPOS;
		return false;
	}
    public Transform GetBodyPartTrans(short bodyIndex) {//获取位置索引
        if (bodyPartsBoxColliders != null && bodyIndex >= 0 && bodyIndex < bodyPartsBoxColliders.Length) {
            return bodyPartsBoxColliders[bodyIndex].transform;
        } else {
            return this.m_ModelTransform;
        }
    }
    public BoxCollider RandomGetLockPos() {//随机获取锁定点
        if (bodyPartsBoxColliders != null && bodyPartsBoxColliders.Length > 0) {
            return bodyPartsBoxColliders[Random.Range(0, bodyPartsBoxColliders.Length)];
        } else {
            return this.mFishCollider;
        }
    }

	public static Vector3[] GetWorldCorners(BoxCollider bc)
	{
		float zx = bc.size.x * 0.5f;
		float zy = bc.size.y * 0.5f;
		float zf = bc.size.z * 0.5f;
		Vector3[] offset = new Vector3[] {
			new Vector3(-zx, -zy, zf),
			new Vector3(-zx, zy, zf),
			new Vector3(zx, zy, zf),
			new Vector3(zx, -zy, zf),

			new Vector3(-zx, -zy, -zf),
			new Vector3(-zx, zy, -zf),
			new Vector3(zx, zy, -zf),
			new Vector3(zx, -zy, -zf),
		};

		Vector3 center = bc.center;
		Vector3[] pl = new Vector3[8];
		for (int i = 0; i < pl.Length; i++) {
			pl [i] = center + offset [i];
			pl [i] = bc.transform.TransformPoint (pl [i]);
		}
		return pl;
	}

	public Vector3[] GetWorldCorners()
	{
		for (int i = 0; i < worldCorns.Length; i++) 
		{
			worldCorns [i] = mFishCollider.transform.TransformPoint(corns [i]);
		}
		return worldCorns;
	}

    public void SetDropReward(uint nReward)
    {
        m_nRewardDrop = nReward;
    }
    public uint  GetDropReward()
    {
        return m_nRewardDrop;
    }

    public void StopAnimEvents()
    {
		if (Controller.PathEvtType != PathEventType.NONE) 
		{
			FishClipType clip = FishAnimatorStatusMgr.PathEvent2FishClip(Controller.PathEvtType);
			if (clip != FishClipType.CLIP_MAX) {
				m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList[(byte)clip], false);
			}
			Controller.ClearAllEvent();
		}
    }

    public void SetCatched(byte seat)
    {
        m_CatchSeat = seat;
    }
    public bool Catched
    {
        get { return m_CatchSeat != 0xff; }
    }
    public Vector3 WorldOffset
    {
        get { return m_WorldOffset; }
        set { m_WorldOffset = value; }
    }
    public Quaternion OrgRot
    {
        get { return m_OrgRot; }
        set { m_OrgRot = value; }
    }


	bool mIsDizzy = false;
	public bool IsDizzy
	{
		set { mIsDizzy = value;}
		get { return mIsDizzy; }	
	}

	byte mBossLifeIdx = 0;
	public byte BossLifeIndex
	{
		get { return mBossLifeIdx; }
		set { mBossLifeIdx = value; }
	}

	bool mIsBoosLifeOver = false;
	public bool IsBosslifeOver {
		set { mIsBoosLifeOver = value;}
		get{ return mIsBoosLifeOver;}
	}

    public Animator Anim
    {
        get
        {
            return m_Anim;
        }
    }
    public float ActionSpeed
    {
        get
        {
            return m_Anim.speed;
        }
    }

	public void ResetActionSpeed(){
		m_Anim.speed = mActionSpeed;
		mActionAlterqueue.Clear ();
	}

	List<ActionSpeedAlter> mActionAlterqueue = new List<ActionSpeedAlter>();
	public bool AddActionSpeed(ActionSpeedAlter actionAlt){
		if (mActionAlterqueue.Contains (actionAlt))
			return false;
		mActionAlterqueue.Add (actionAlt);
		float minSpeed = 10f;
		for (int i = 0; i < mActionAlterqueue.Count; i++) {
			minSpeed = Mathf.Min(minSpeed, mActionAlterqueue [i].actionSpeed);
		}
		m_Anim.speed = minSpeed;
		return true;
	}

	public bool RemoveActionSpeed(ActionSpeedAlter actionAlt){
		if (!mActionAlterqueue.Contains (actionAlt))
			return false;
		mActionAlterqueue.Remove (actionAlt);
		if (mActionAlterqueue.Count == 0) {
			m_Anim.speed = 1f;
			return true;
		}
		float minSpeed = 10f;
		for (int i = 0; i < mActionAlterqueue.Count; i++) {
			minSpeed = Mathf.Min(minSpeed, mActionAlterqueue [i].actionSpeed);
		}
		m_Anim.speed = minSpeed;
		return true;
	}

	int _RefVal = 0;
	int renderQueue = 2000;
    
	public Shader shader
	{
		get 
		{
            if (mModelRenderMaters == null || mModelRenderMaters.Count == 0)
				return null;
			return mModelRenderMaters[0].shader;
		}
		set 
		{
			if (value == null || mModelRenderMaters == null)
				return;
            for (int i = 0; i < mModelRenderMaters.Count; i++) {
                mModelRenderMaters[i].shader = value;
            }
			this.SetMatIntVal ("_RefVal", _RefVal);
			this.SetRenderQueue (renderQueue);
		}
	}

	public void SetMatFloatVal(string nameID, float value)
	{
		if (mModelRenderMaters == null)
            return;
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModelRenderMaters[i].SetFloat(nameID, value);
        }
	}

	public void SetMatIntVal(string nameID, int refVal)
	{
		if (mModelRenderMaters == null)
			return;
		if (nameID == "_RefVal")
            _RefVal = refVal;
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModelRenderMaters[i].SetInt(nameID, refVal);
        }
		UpdateRender ();
	}

	public void SetRenderQueue(int queue)
	{
		if (mModelRenderMaters == null)
			return;
        renderQueue = queue;
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModelRenderMaters[i].renderQueue = queue;
        }
		UpdateRender ();
	}


	public void ResetColor(){
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
			mModelRenderMaters [i].shader = Shader.Find (mModeShaderNames[i]);
		}
	}

	public void SetColor(Color col)
	{
		if (mModelRenderMaters == null)
			return;
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
			SceneEffectMgr.CalColorBlendShader (mModelRenderMaters [i], mModeShaderNames[i], col);
		}
		UpdateRender ();
	}

	public Texture GetTexture(string properities)
	{
        if (mModelRenderMaters == null || mModelRenderMaters.Count == 0)
			return null;
		return mModelRenderMaters[0].GetTexture (properities);
	}

	public void SetTexture(string properities, Texture tex)
	{
		if (mModelRenderMaters == null)
            return;
        for (int i = 0; i < mModelRenderMaters.Count; i++) {
            mModelRenderMaters[i].SetTexture(properities, tex);
        }
		UpdateRender ();
	}

	void UpdateRender(){
        return;
        //for (int i = 0; i < mModelRenderMaters.Count; i++) {
        //    mRenders[i].material = mModelRenderMaters[i];
        //}
	}

    public Transform Transform
    {
        get
        {
            return m_ModelTransform;
        }
    }
    public bool HasOpt
    {
        get
        {
            return m_OptList != null && m_OptList.Count > 0;
        }
    }

	public bool CheckFishOpt(FishOptType optType)
	{
		if (HasOpt) {
			return m_OptList.Exists (x => x.OptType == optType);
		}
		return false;
	}

    public void AddOpt(IFishOpt opt)
    {
        if (m_OptList == null)
            m_OptList = new List<IFishOpt>();
		if (!isGoAway) {
			m_OptList.Add (opt);
		}
    }

	bool OptUpdate(float delta)
	{
		if (m_OptList == null) {			
			return true;
		}

		bool bRemove;
		for (int i = 0; i < m_OptList.Count; )
		{
			IFishOpt opt = m_OptList[i];
			FishOptState state = opt.CheckDelay(delta);
			if (state == FishOptState.FOS_DELAY)
			{
				++i;
				continue;
			}
			if (opt.isDestroy) {
				if (state == FishOptState.FOS_UPDATE)
					opt.Update(100f,this, out bRemove);
				Utility.ListRemoveAt(m_OptList, i);
				continue;
			}
			if (state == FishOptState.FOS_FIRST) {
				opt.Init (this);
				if (opt.isDestroy) {
					Utility.ListRemoveAt(m_OptList, i);
					continue;
				}
			}

			bool bRet = opt.Update(delta, this, out bRemove);
			if (bRet == false)
				return false;

			if (bRemove)
			{
				Utility.ListRemoveAt(m_OptList, i);
			}
			else
				++i;
		}
		return true;
	}

	public void RemoveOpt(IFishOpt opt)
	{
		if (opt != null)
			opt.isDestroy = true;
	}

	public bool ClearOpt()
	{
		if (m_OptList == null)
			return true;
		for (int i = 0; i < m_OptList.Count; ++i)
		{
			m_OptList [i].isDestroy = true;
		}
		return true;
	}

    public bool AddElapsedTime(float delta)
    {
        m_PathCtrl.AddElapsedTime(delta);
        if(Time > 1.0f)
        {
            m_DelayDestroy = true;
            m_PathCtrl.Time = 0;
        }
        return true;
    }
    public bool IsDelay
    {
        get { return m_DelayDestroy; }
        set { m_DelayDestroy = value; }
    }
	public FishVo vo
	{
		get { return mFishVo;}
	}
    public uint FishCfgID
    {
        get
        {
            return m_FishCfgId;
        }
    }
    public ushort FishID
    {
        get
        {
            return m_FishID;
        }
    }
    public float Time
    {
        get
        {
            return m_PathCtrl.Time;
        }
        set
        {
            m_PathCtrl.Time = value;
        }
    }

	bool mIsShowSelf = false;
	public bool IsShowSelf {
		get 
		{ 
			return mIsShowSelf;
		}

		set 
		{ 
			mIsShowSelf = value;
		}
	}

	public float ShowTimeTick = 0f;

    public Quaternion Rotation
    {
        get
        {
            return m_ModelTransform.localRotation;
        }
        set
        {
            m_ModelTransform.localRotation = value;
        }
    }
    public float Speed
    {
        get
        {
            return m_PathCtrl.CurrentSpeed;
        }
        
    }

    public Vector3 Position
    {
        get
        {
            return m_Position;
        }
        set
        {
			m_ModelTransform.position = m_Position = value;
        }
    }

    public PathController Controller
    {
        get { return m_PathCtrl; }
        set { m_PathCtrl = value; }
    }
    public void SetOffset(Vector3 offset)
    {
        m_Offset = offset;
    }
    void SetScaling(float scl)
    {
        m_ModelTransform.localScale *= scl;
        if (m_FishCfgId == 0)
        {
            ParticleSystem[] ps = m_Model.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < ps.Length; ++i)
            {
                if (ps[i].main.scalingMode != ParticleSystemScalingMode.Hierarchy) {
                    ps[i].startSize *= scl;
                    ps[i].Clear();
                }
            }
        }
    }
    public void SetAddScale(float add_scale) {//设置变更缩放
        this.SetScaling(add_scale / this.mAddScale);
        this.mAddScale = add_scale;
    }
    public float Scaling
    {
        get
        {
            return m_Scaling * mAddScale;
        }
    }

    public void Hide() {
        m_Model.SetActive(false);
    }

	PathEventType oldPathEventType = PathEventType.NONE;
    public bool Update(float delta)
    {
		oldPathEventType = m_PathCtrl.PathEvtType;
		if (IsShowSelf) {
			if (ShowTimeTick > 0f)
				ShowTimeTick -= delta;
		}

		if (isGoAway) 
		{
			ShowTimeTick = 0f;
			if (OptUpdate(delta) == false)
				return false;
			if (m_PathCtrl.Time >= 1.0f)
				return false;
			
			if (m_PathCtrl.Update(delta)) {
				UpdatePosAndRot();
			}
			UpdatePathEvents (delta);
			return true;
		}

        if (m_IsPostLaunch && m_PostLaunchDuration > 0f) {
            if (m_PostLaunchDuration <= delta) {
                delta -= m_PostLaunchDuration;
                m_PostLaunchDuration = 0;
                m_IsPostLaunch = false;
                m_Model.SetActive(true);
            } else {
                m_PostLaunchDuration -= delta;
                if (!IsShowSelf)
                    m_Model.SetActive(false);
                return true;
            }
        }

        if (IsBossFish && m_bgsoundDelay>0)
        {
            m_bgsoundDelay -= delta;
            if(m_bgsoundDelay<0)
            {   
				if (Application.isPlaying) {
                    SceneLogic.Instance.PlayBossBGM(this.FishID);
				}
	        }
        }

		if (IsBossFish && Fish.IsBossFirstShowing)
			return true;
		
		if (OptUpdate (delta) == false) {
			ShowTimeTick = 0f;
			return false;
		}

		if(m_DelayDestroy)
		{
			Time += delta;
			return Time < 1f;
		}

        bool bRet = m_PathCtrl.Update(delta);
        if (m_PathCtrl.Time < 0.0f)
            return true;
		
        if (m_PathCtrl.Time >= 1.0f)
        {
            if (bRet) {
                UpdatePosAndRot();
            }
			ShowTimeTick = 0f;
            //自然死亡，延迟删除
            if (this.FishID == ConstValue.WorldBossID) {//全服宝箱路径走完时，播放离场动画
                this.LeaveScene();
            } else {
                m_DelayDestroy = true;
                Time = 0;
            }
            return true;
        }

        if (bRet)
        {
			UpdatePosAndRot();
        }
		UpdatePathEvents (delta);
        return true;
    }

    public void LeaveScene() {//BOSS离场
        this.Anim.Set_Bool(FishAnimatorStatusMgr.DeadHashName, true);
        this.Anim.Set_Trigger(FishAnimatorStatusMgr.ANYSTATE_TRIGGER);
        m_DelayDestroy = true;
        Time = 0f;
        BaoJiDianManager.HideBaoJiDian();
    }

	bool isUpdatePosAndRot = false;
	public bool IsUpdatePosAndRot{ get { return isUpdatePosAndRot;}}
	void UpdatePosAndRot()
	{
		m_bUpdateScreen = false;
		m_bUpdateViewPos = false;
		m_bUpdateBSize = false;
        Quaternion newRot;
        if (this.mFishShapeContent == null) {
            Vector3 pos = m_PathCtrl.ToFishPos(m_Offset) + WorldOffset;
            pos.x *= Resolution.AdaptAspect;
            newRot = m_PathCtrl.ToFishRot();

            if (ENABLE_HIDE_OUTVIEW) {
                GameUtils.NormalizeQuaternion(ref newRot);
                localToWorldMat.SetTRS(pos, newRot * m_OrgRot, m_ModelTransform.localScale);
                if (!_IsInView(localToWorldMat)) {
                    if (!_IsInView(m_ModelTransform.localToWorldMatrix)) {
                        isUpdatePosAndRot = false;
                        return;
                    }
                }
            }
            isUpdatePosAndRot = true;
            this.Position = pos;
        } else {
            Vector3 pos = m_PathCtrl.ToFishPos(Vector3.zero) + WorldOffset;
            pos.x *= Resolution.AdaptAspect;
            newRot = m_PathCtrl.ToFishRot();

            if (ENABLE_HIDE_OUTVIEW) {
                GameUtils.NormalizeQuaternion(ref newRot);
                localToWorldMat.SetTRS(pos, newRot, m_ModelTransform.localScale);
                if (!_IsInView(localToWorldMat)) {
                    if (!_IsInView(m_ModelTransform.localToWorldMatrix)) {
                        isUpdatePosAndRot = false;
                        return;
                    }
                }
            }
            isUpdatePosAndRot = true;
            this.mFishShapeContent.transform.position = pos + this.mFishShapeContent.mOffset;
            this.mFishShapeContent.transform.rotation = newRot;
            this.Position = this.mFishShapeParent.TransformPoint(this.m_Offset);
        }


        if (this.m_PathCtrl.mPathInfo != null) {
            if ((newRot * Vector3.forward).z < 0) {
                newRot = newRot * (m_OrgRot * Quaternion.AngleAxis(-90, Vector3.right));
            } else {
                newRot = newRot * (m_OrgRot * Quaternion.AngleAxis(90, Vector3.right));
            }
        } else {
            newRot = newRot * m_OrgRot;
        }
       if (isQuadMeshRender) {
           m_ModelTransform.rotation = Quaternion.Euler(Vector3.forward * newRot.eulerAngles.z);
       } else {
           m_ModelTransform.rotation = newRot;
       }
        
//		localToWorldMat = m_ModelTransform.localToWorldMatrix;
	}


	void HandleBossEventHandle(string evet){
		if (evet == "HitOn") {
			int animIdx = (int)m_Anim.Get_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP);
			uint akteffId = 0;
			uint shake_eff = 0u;
			BossEffectVo bossEffvo = FishConfig.Instance.BossEffectConf.TryGet (FishCfgID);
			if (animIdx == 0) {
				akteffId = bossEffvo.Atk0Eff;
				shake_eff = bossEffvo.ShakeEff0;
			} else if (animIdx == 1) {
				akteffId = bossEffvo.Atk1Eff;
				shake_eff = bossEffvo.ShakeEff1;
			} else {
				akteffId = bossEffvo.Atk2Eff;
				shake_eff = bossEffvo.ShakeEff2;
			}

			if (bossEffvo.BulletIDs.Length > 0 && bossEffvo.BulletIDs [animIdx] > 0) {
				GameObject prefab = FishResManager.Instance.BulletObjMap.TryGet (bossEffvo.BulletIDs [animIdx]);
				var animStatInfo = m_Anim.GetCurrentAnimatorStateInfo (0);
				HeroFirePosition[] poslist = m_ModelTransform.GetComponentsInChildren<HeroFirePosition> ();
				float time = Mathf.Max (0f, 1f - animStatInfo.normalizedTime) * animStatInfo.length;//(Transform.position.z - 120f) * 0.0015f;
				if (poslist != null && poslist.Length > 0) {
					for (int i = 0; i < poslist.Length; i++) {
						this.LaunchBullet (0.3f * i, prefab, poslist [i].transform.position, time, akteffId, shake_eff);
					}
				} else {
					this.LaunchBullet (0f, prefab, Transform.position, time, akteffId, shake_eff);
				}
            }
            else
            {
                ResManager.LoadAsyn<GameObject>(string.Format(ResPath.NewBossEffect, akteffId), (ab_data, prefab) =>
                {
                    GameObject obj = GameUtils.CreateGo(prefab);
                    obj.AddComponent<ResCount>().ab_info = ab_data;
                    this.OnBossBulletCosion(obj, akteffId, prefab.transform.position, shake_eff);
                }, GameEnum.Fish_3D);
			}
		}
        else if (evet == "AtkEnd")
        {
		}
        else if (evet == "Laugh")
        {
			BossEffectVo bossEffvo = FishConfig.Instance.BossEffectConf.TryGet (FishCfgID);
            if (bossEffvo.LaughEff > 0) {
                ResManager.LoadAsyn<GameObject>(string.Format(ResPath.NewBossEffect, bossEffvo.LaughEff), (ab_data, prefab) => {
                    GameObject laughEffGo = GameUtils.CreateGo(prefab);
                    laughEffGo.AddComponent<ResCount>().ab_info = ab_data;
                    if (prefab.transform.localPosition != Vector3.zero) {
                        laughEffGo.transform.position = prefab.transform.position;
                    } else {
                        laughEffGo.transform.SetParent(Transform);
                        laughEffGo.transform.localPosition = Vector3.zero;
                    }
                    AutoDestroy.Begin(laughEffGo, GameUtils.CalPSLife(laughEffGo));
                    GameUtils.PlayPS(laughEffGo);
                }, GameEnum.Fish_3D);
			}

		}
	}

    //BOSS发射子弹
    private void LaunchBullet(float delay_time, GameObject prefab, Vector3 pos, float time, uint hit_eff_id, uint shake_eff) {
        MonoDelegate.Instance.Coroutine_Delay(delay_time, () => {
            Vector3 target_pos = new Vector3(Utility.Range(-10f, 10f), Utility.Range(-10f, 10f), 120f);
            GameObject bulletGo = GameUtils.CreateGo(prefab);
            bulletGo.transform.position = pos;
            iTween.MoveTo(bulletGo, iTween.Hash("time", time, "position", target_pos, "islocal", false));
            MonoDelegate.Instance.Coroutine_Delay(time, delegate {
                GameObject.Destroy(bulletGo);
                ResManager.LoadAsyn<GameObject>(string.Format(ResPath.NewBossEffect, hit_eff_id), (ab_data, prefabEff) => {
                    GameObject obj = GameUtils.CreateGo(prefabEff);
                    obj.AddComponent<ResCount>().ab_info = ab_data;
                    OnBossBulletCosion(obj, hit_eff_id, target_pos, shake_eff);
                }, GameEnum.Fish_3D);
            });
        });
    }
    private void OnBossBulletCosion(GameObject effObj, uint akteffId, Vector3 pos, uint shake_eff) {
        effObj.transform.position = pos;
        AutoDestroy.Begin(effObj);

        if (shake_eff > 0) {
            SkillEffectData effargs = new SkillEffectData();
            effargs.clientSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
            EffectVo effvo = FishConfig.Instance.EffectConf.TryGet(shake_eff);
            SceneLogic.Instance.SkillMgr.SkillApplyEffect(effvo, effargs);
        }
    }

    void OnBossBulletCosion(uint akteffId) {
        ResManager.LoadAsyn<GameObject>(string.Format(ResPath.NewBossEffect, akteffId), (ab_data, prefabEff) => {
            GameObject effObj = GameUtils.CreateGo(prefabEff);
            effObj.AddComponent<ResCount>().ab_info = ab_data;
            effObj.transform.position = prefabEff.transform.position;
            AutoDestroy.Begin(effObj);
        }, GameEnum.Fish_3D);
	}

	float curStatusSubClip = 0f;
	void UpdatePathEvents(float delta)
	{
		if(m_PathCtrl.PathEvtType != PathEventType.NONE)
		{
			if (m_PathCtrl.PathEvtType == PathEventType.ANIMATIONS) {
				UpdateBossAnimations (m_PathCtrl.TimeID,delta);
				return;
			}
			m_Anim.Set_Bool (FishAnimatorStatusMgr.YouYongHashName,true);
			m_Anim.Set_Bool (FishAnimatorStatusMgr.IdleHashName,false);
			if (oldPathEventType != m_PathCtrl.PathEvtType) {
				curStatusSubClip = m_Anim.Get_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP);
				FishClipType clipType = FishAnimatorStatusMgr.PathEvent2FishClip (m_PathCtrl.PathEvtType);
				m_Anim.Set_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP, (float)m_PathCtrl.SubAnimID);
				if (clipType != FishClipType.CLIP_MAX)
					m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList [(byte)clipType], true);
			}
		}
		else if(oldPathEventType != PathEventType.NONE)
		{
			bossEventList = null;
			m_Anim.Set_Bool (FishAnimatorStatusMgr.YouYongHashName,true);
			m_Anim.Set_Bool (FishAnimatorStatusMgr.IdleHashName,false);
			if(mCurrentBossClip != FishClipType.CLIP_MAX)
				m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList[(byte)mCurrentBossClip], false);
			
			FishClipType clipType = FishAnimatorStatusMgr.PathEvent2FishClip (oldPathEventType);
			m_Anim.Set_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP, curStatusSubClip);
			if (clipType != FishClipType.CLIP_MAX)
				m_Anim.Set_Bool(FishAnimatorStatusMgr.ActionHashList[(byte)clipType], false);
		}
	}

    public FishClipType CurrentBossClip {
        get {
            return this.mCurrentBossClip;
        }
    }

	BossPathEventVo[] bossEventList = null;
	int mBossEventIdex = 0;
	int mBossClipTimes = 0;
	FishClipType mCurrentBossClip = FishClipType.CLIP_MAX;
	void UpdateBossAnimations (ushort animsqID, float delta)
	{
		
		if (bossEventList == null) {
			curStatusSubClip = m_Anim.GetFloat (FishAnimatorStatusMgr.STATUS_SUBCLIP);
			BossPathEventVo[] voList = FishConfig.Instance.mBossPathEventConf.TryGet ((uint)animsqID);
			bossEventList = voList;
			mBossEventIdex = 0;
			m_Anim.Set_Bool (FishAnimatorStatusMgr.YouYongHashName,false);
			m_Anim.Set_Bool (FishAnimatorStatusMgr.IdleHashName,true);

		}

        //英雄播放动画时，矫正角度
        Vector3 rotate = this.Transform.localEulerAngles;
        rotate.x = Mathf.Floor(rotate.x / 90 + 0.5f) * 90;
        rotate.y = Mathf.Floor(rotate.y / 90 + 0.5f) * 90;
        rotate.z = Mathf.Floor(rotate.z / 90 + 0.5f) * 90;
        this.Transform.localEulerAngles = rotate;

		if (mBossEventIdex < bossEventList.Length) {
			BossPathEventVo vo = bossEventList [mBossEventIdex];
			FishClipType clipType = FishAnimatorStatusMgr.PathEvent2FishClip ((PathEventType)vo.EventType);

			if (clipType == mCurrentBossClip) {
				AnimatorStateInfo staInfo = m_Anim.GetCurrentAnimatorStateInfo (0);
				if (staInfo.shortNameHash != FishAnimatorStatusMgr.ActionHashList [(byte)mCurrentBossClip])
					return;
				if (vo.EventType == (byte)PathEventType.STAY) {
					mBossClipTimes += Mathf.FloorToInt(delta*1000);
					if (mBossClipTimes+0.2f >= (int)vo.EventTimes) {
						//m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList[(byte)mCurrentBossClip], false);
						mBossEventIdex++;
					}
				} else {
					int tm = Mathf.FloorToInt (staInfo.normalizedTime+0.1f);
					if (tm >= (int)vo.EventTimes) {
						m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList[(byte)mCurrentBossClip], false);
						mBossEventIdex++;
					}
				}
			}else if (clipType != FishClipType.CLIP_MAX) {
				mBossClipTimes = 0;
				m_Anim.Set_Float (FishAnimatorStatusMgr.STATUS_SUBCLIP, vo.SubClipId);
				m_Anim.Set_Bool (FishAnimatorStatusMgr.ActionHashList [(byte)clipType], true);
				mCurrentBossClip = clipType;
				GlobalAudioMgr.Instance.PlayAudioEff (vo.Audio);
			}
		}
	}


    public Vector3 ScreenPos
    {
        get
        {
            if (m_bUpdateScreen == false)
            {
                m_bUpdateScreen = true;
				m_ScrPos = Utility.MainCam.WorldToScreenPoint(m_Position);
                m_ScrPos.z = 0;
            }
            return m_ScrPos;
        }
    }

    public Vector3 ViewPos
    {
        get
        {
            if (m_bUpdateViewPos == false)
            {
                m_bUpdateViewPos = true;
                m_ViewPos = Camera.main.ScreenToViewportPoint(ScreenPos);
                m_ViewPos.z = 0;
            }
            return m_ViewPos;
        }
    }

	bool isGoAway = false;
	public void GoAway()
	{
		ClearEffectObjs ();
		ClearOpt ();
		Controller.ClearAllEvent();
		isGoAway = true;
	}

	void ClearEffectObjs()
	{
		foreach (var pair in mCacheEffObjRefMap) {
			GameObject.Destroy (pair.Value.mTrans);
		}
		mCacheEffObjRefMap.Clear ();
	}

	public void DestroyAway()
	{
		if (mAnimListener != null)
		{
			mAnimListener.OnAnimatorEvent -= HandleBossEventHandle;
		}
		mModelRenderMaters = null;
		IsDelay = true;
		GameObject.Destroy(m_Model);
		m_Model = null;
		m_ModelTransform = null;
		m_Anim = null;
		m_PathCtrl = null;
		if(IsBossFish)
		{

			if (SceneLogic.Instance.FishMgr != null)
			{
                //GlobalAudioMgr.Instance.PlayBGMusic(SceneLogic.Instance.BackgroundIdx); //正常离场不切换背景音乐
                if (SceneLogic.Instance.FishMgr.activedBoss != null && SceneLogic.Instance.FishMgr.activedBoss.FishID == this.FishID) {
                    SceneLogic.Instance.FishMgr.activedBoss = null;
                }
			}

		}
	}

    public void Destroy()
    {
		if (mAnimListener != null)
		{
			mAnimListener.OnAnimatorEvent -= HandleBossEventHandle;
		}

        if(IsBossFish)
        {
        	if (SceneLogic.Instance.FishMgr != null)
			{
                if (IsBosslifeOver || this.FishID == ConstValue.WorldBossID) {
                    SceneLogic.Instance.PlayBGM(SceneLogic.Instance.BackgroundIdx ,false);
                    SceneLogic.Instance.LogicUI.BossLifeUI.Hide();//BOSS死亡或者逃跑时，隐藏血条
                }
                if (SceneLogic.Instance.FishMgr.activedBoss != null && SceneLogic.Instance.FishMgr.activedBoss.FishID == this.FishID) {
                    SceneLogic.Instance.FishMgr.activedBoss = null;
                }
			}
        }

        this.RemoveFishShape();
        m_Model.SetActive(false);
        GameObject.Destroy(m_Model);
        m_Model = null;
        m_ModelTransform = null;
        m_Anim = null;
        m_PathCtrl = null;
		mModelRenderMaters = null;
    }
}
