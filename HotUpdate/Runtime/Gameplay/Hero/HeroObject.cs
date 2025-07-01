using UnityEngine;
using System.Collections;

public enum AvaterAnimStatus
{
	idle = 0, run ,atk, hit, die
}  

public class BaseHeroObject3D:ISortZObj
{
	protected Vector3                                         m_Position;             //当前对象位置
	protected Transform                                       m_ModelTransform;       //当前对象变换
	protected GameObject                                      m_Model;                //对象GameObject引用
	protected AnimationEventListener                          m_AnimListener;               //帧动画脚本对象
	protected Animator                                        m_Animator; 
	Transform lcrPosTrans;

	HeroFirePosition[] mFirePositionList = null;
	public Transform Transform { get { return m_ModelTransform; } }
	public Vector3 Position
	{
		get 
		{ 
			if (m_ModelTransform != null)
				return m_ModelTransform.position; 
			return Vector3.zero;
		}
	}

	byte[] mHeroFireSlots = new byte[0];
	public int UseFireSlotIdx()
	{
		int i = -1;
		for (i = 0; i < mHeroFireSlots.Length; i++) {
			if (mHeroFireSlots [i] == 0) {
				mHeroFireSlots [i] = 1;
				break;
			}
		}
		if (i>=mHeroFireSlots.Length-1)
			ClearFireSlots ();
		return i;
	}

	public void ClearFireSlots()
	{
		for (int i = 0; i < mHeroFireSlots.Length; i++)
			mHeroFireSlots [i] = 0;
	}
	
	public bool IsVisible
	{
		get { return m_Model.activeSelf;}
		set { m_Model.SetActive (value);}
	}

	Renderer[] mRenders = null;
	public void SetRenderQueue(int queue){
		if (mRenders == null)
			return;
		
	#if UNITY_EDITOR
		foreach (var render in mRenders)
			System.Array.ForEach(render.materials, x=>x.renderQueue = queue);
	#else
		foreach (var render in mRenders)
			System.Array.ForEach(render.materials, x=>x.renderQueue = queue);
	#endif
	}

	public void SetMatIntVal(string nameID, int val)
	{		
		if (mRenders == null)
		return;
		#if UNITY_EDITOR
		foreach (var render in mRenders)
			System.Array.ForEach(render.materials, x=>x.SetInt (nameID, val));
		#else
		foreach (var render in mRenders)
			System.Array.ForEach(render.materials, x=>x.SetInt (nameID, val));
		#endif
		
	}


	public void UpdatePosition(Vector3 newpos)
	{
		m_ModelTransform.position = newpos;
		#if UNITY_EDITOR
		newpos = HeroUtilty.WorldPosToUIPos(newpos);
		newpos.z = 110f;
		newpos = HeroUtilty.UIPosToWorldPos(newpos);
		rangeCircleGo.transform.position = newpos;
		#endif
	}

	public void Move(Vector3 dir, float speed, float deltaTime)
	{
		UpdatePosition(m_ModelTransform.position + dir * speed * deltaTime);	
	}

	public Vector3 LcrPos
	{
		get
		{
			if (lcrPosTrans == null)
				return Position; 
			return lcrPosTrans.position;
		}
	}

	public float AnimSpeed {
		get { return m_Animator.speed;}
	}

	public GameObject rangeCircleGo;
    public void InitHeroResource(GameObject prefab) 
	{
        //GameObject heroPrefab = FishResManager.Instance.HeroObjMap.TryGet (heroCfgID);
        m_Model = prefab;
		m_ModelTransform = m_Model.transform;
		#if UNITY_EDITOR
		rangeCircleGo = GameObject.Instantiate <GameObject>(MainEntrace.Instance.circleObj);
		#endif
		m_Animator = m_Model.GetComponent<Animator>();
		if (m_Animator == null)
			m_Animator = m_Model.GetComponentInChildren<Animator>();
		m_AnimListener = m_Animator.GetComponent<AnimationEventListener>();
		AnimInfoRef animinfo = m_Animator.GetComponent<AnimInfoRef> ();
		if (animinfo != null) {
			lcrPosTrans = animinfo.fireTrans;
		}
		SwitchAnimStatus (AvaterAnimStatus.idle);
		mRenders = m_ModelTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
		HeroFirePosition[] poslist = m_ModelTransform.GetComponentsInChildren<HeroFirePosition> ();
		mHeroFireSlots = new byte[poslist.Length];
		System.Array.ForEach (mHeroFireSlots, x => x = 0);
		mFirePositionList = poslist;
	}

	public HeroFirePosition[] FirePositionList
	{
		get { return mFirePositionList;}
	}

	public void RegisterAnimEndEvent(System.Action<string> cb) 
	{
		m_AnimListener.OnAnimatorEvent += cb;
	}
	public void UnRegisterAnimEndEvent(System.Action<string> cb) 
	{
		m_AnimListener.OnAnimatorEvent -= cb;
	}

	public void ChangeAnimState(byte anim, byte subcip = 0, float animSpeed = 1f)
	{
		SwitchAnimStatus ((AvaterAnimStatus)anim, subcip,animSpeed);
	}

	public bool IsInStats(string statusName)
	{
		return m_Animator.GetCurrentAnimatorStateInfo (0).IsName (statusName);
	}

	public AnimatorStateInfo GetCurrentAnimatorStatus()
	{
		return m_Animator.GetCurrentAnimatorStateInfo (0);
	}

	public bool IsOverStats(string statusName)
	{
		var staInfo = m_Animator.GetCurrentAnimatorStateInfo (0);
        //var nextStaInfo = m_Animator.GetNextAnimatorStateInfo (0);
		if (staInfo.IsName (statusName)) {
			return (staInfo.normalizedTime > 0.95f);
		}
		return false;
	}

	protected void SwitchAnimStatus(AvaterAnimStatus tarStatus, byte subclip = 0, float speed = 1f)
	{
		m_Animator.speed = speed;
		switch (tarStatus) {
		case AvaterAnimStatus.idle:
			m_Animator.SetBool ("run", false);
			break;
		case AvaterAnimStatus.run:
			m_Animator.SetFloat ("SubClip", 1);
			m_Animator.SetBool ("run", true);
			break;
		case AvaterAnimStatus.atk:
			m_Animator.SetBool ("run", false);
			m_Animator.SetFloat ("SubClip", subclip);
			m_Animator.SetTrigger ("atk");
			break;
		case AvaterAnimStatus.die:
			m_Animator.SetBool ("run", false);
			m_Animator.SetTrigger ("die");
			break;
		}
	}

	public void Destroy()
	{
		GameObject.Destroy (m_ModelTransform.gameObject);
#if UNITY_EDITOR
		GameObject.Destroy (rangeCircleGo);
		rangeCircleGo = null;
#endif
		mRenders = null;
	}
}


public class HeroObject3D : BaseHeroObject3D
{
    public void SetUIPosition(Vector3 pos) 
    {
		UpdatePosition( HeroUtilty.UIPosToWorldPos(pos));

    }

    public Vector3 GetTargetPosition(Fish fish)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(fish.ScreenPos.x, fish.ScreenPos.y, ConstValue.NEAR_Z + 10f));
    }

    public Vector3 GetBulletStartPosition()
    {
        return new Vector3(m_ModelTransform.position.x, m_ModelTransform.position.y + 5, ConstValue.NEAR_Z + 10f);
    }

    public void InitHeroPositionInfo(byte clientSeat, ref Vector3 originalPosition) 
    {
        ScenePlayer scenePlayer = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
        if (null != scenePlayer) {
            Vector3 posWorld = HeroUtilty.CalHeroInitWorldPos(clientSeat, ConstValue.HERO_Z);
            Vector4 rect = HeroUtilty.CalArea(clientSeat, ConstValue.HERO_Z);
            originalPosition = new Vector3();
            originalPosition.x = Mathf.Lerp(rect.x, rect.z, 0.5f);
            originalPosition.y = Mathf.Lerp(rect.y, rect.w, 0.5f);
            originalPosition.z = ConstValue.HERO_Z;
            UpdatePosition(posWorld);
        }
    }

	public void HeroIdle() 
	{
		SwitchAnimStatus (AvaterAnimStatus.idle);
	}

	public void HeroAttack(int atkIdx, float animSpeed = 1f)
    {
		SwitchAnimStatus (AvaterAnimStatus.atk, (byte)atkIdx, animSpeed);
    }

	public bool HeroMove(int speed, ushort RotSpeed, Vector3 moveDir, float deltaTime, ref bool isAnimDirty) 
    {
		Vector3 rotDir = new Vector3(moveDir.x, 0f, moveDir.z);
		m_ModelTransform.rotation = Quaternion.Euler (Vector3.up * m_ModelTransform.rotation.eulerAngles.y);
		Quaternion tarRot = m_ModelTransform.rotation * Quaternion.FromToRotation(m_ModelTransform.forward, rotDir.normalized);
		m_ModelTransform.rotation = Quaternion.Lerp(m_ModelTransform.rotation, tarRot, deltaTime*RotSpeed);
        if (isAnimDirty)
        {
			SwitchAnimStatus (AvaterAnimStatus.run);
            isAnimDirty = false;
        }
		if (Mathf.Abs (m_ModelTransform.rotation.eulerAngles.y - tarRot.eulerAngles.y) > 10f)
			return false;
		Move (moveDir, speed, deltaTime);
		return true;
    }

	public bool FaceToAtkTargt(Vector3 direct, ushort RotSpeed, float deltaTime)
	{
		direct.y = 0f;
		direct = direct.normalized;
		m_ModelTransform.rotation = Quaternion.Euler (Vector3.up * m_ModelTransform.rotation.eulerAngles.y);
		Quaternion tarRot = m_ModelTransform.rotation * Quaternion.FromToRotation (m_ModelTransform.forward, direct);
		m_ModelTransform.rotation = Quaternion.Lerp (m_ModelTransform.rotation, tarRot, deltaTime*RotSpeed);
		if (Mathf.Abs (m_ModelTransform.rotation.eulerAngles.y - tarRot.eulerAngles.y) > 10f)
			return false;
		m_ModelTransform.rotation = Quaternion.Euler (Vector3.up * tarRot.eulerAngles.y);
		return true;
	}
}




/// <summary>
/// ================================================== 其他玩家 3d 模型对象 ==============================
/// </summary>

public class HeroClientObject3D : BaseHeroObject3D 
{
	public void InitHeroPositionInfo(byte clientSeat) 
	{
		ScenePlayer scenePlayer = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
		if (null != scenePlayer)
		{
			Vector3 posWorld = HeroUtilty.CalHeroInitWorldPos (clientSeat, ConstValue.HERO_Z);
            //Vector4 rect = HeroUtilty.CalArea (clientSeat, ConstValue.HERO_Z);
//			originalPosition = new Vector3 ();
//			originalPosition.x = Mathf.Lerp (rect.x, rect.z, 0.5f);
//			originalPosition.y = Mathf.Lerp (rect.y, rect.w, 0.5f);
//			originalPosition.z = ConstValue.HERO_Z;
			m_ModelTransform.position = posWorld;
		}
	}

	public bool FaceToAtkTargt(Vector3 direct, ushort RotSpeed, float deltaTime)
	{
		float angle = SceneHeroMgr.CalculateDegreeToFloatValue (direct);
		Quaternion tarRot = Quaternion.Euler (Vector3.up * angle);
		m_ModelTransform.rotation = Quaternion.RotateTowards(m_ModelTransform.rotation, tarRot, deltaTime*RotSpeed*10f);
		if (Mathf.Abs (m_ModelTransform.rotation.eulerAngles.y - tarRot.eulerAngles.y) > 10f)
			return false;
		m_ModelTransform.rotation = tarRot;
		return true;
	}
}