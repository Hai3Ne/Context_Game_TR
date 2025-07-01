using System;
using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;

public class GunBarrel
{
    private const string AttackAnimName = "attack";//攻击动画名称
    private static int StandAnimName = Animator.StringToHash("stand");//待机动画名称
    private static int AttackParameters = Animator.StringToHash("attack");//攻击动画参数

    Animator m_SkAniatom;
    GameObject          m_standyEffGo = null;
    GameObject          m_standyFireEffGo = null;//炮口常驻特效
	Transform           m_BaseTrans, m_FireTrans,m_IdleTrans;
    private Transform mParent;//炮台父节点
    private Transform mFireIdleTrans;//炮口常驻特效父节点
	UILabel             m_BulletConsumeLabel;              //子弹基础消耗
	Vector2             m_GunPivot = Vector2.zero;        //炮管中心轴点的位置
	float               m_AnimTimes;
	float               m_AnimInterval;                 //动画播放频率
	bool                m_IsPlayAnim;            //
	uint                m_BulletConsume;        //炮的发出子弹的基础伤害
	byte                m_Seat;
	LauncherVo 			mLauncherVo;
    private float mInitRotate_z;//初始角度

	public GunBarrel()
	{
		m_AnimTimes = 0.0f;
		m_AnimInterval = 0.2f;
		m_IsPlayAnim = false;
	}

	float hitFiretime = 0f;
	const float lcrFPS = 0.0333f;
	public void Init( GameObject gunObj,LauncherVo vo)
	{
		mLauncherVo = vo;
		m_BaseTrans = gunObj.transform;
        m_SkAniatom = gunObj.GetComponent<Animator>();
        this.mParent = this.m_BaseTrans.parent;

		LcrAnimInfo lcrAnim = gunObj.GetComponent<LcrAnimInfo> ();
		m_FireTrans = lcrAnim.FireTrans;
		m_IdleTrans = lcrAnim.IdleTrans;
        mFireIdleTrans = lcrAnim.FireIdleTrans;
		m_BulletConsumeLabel = lcrAnim.lrLevelLabel;
        hitFiretime = lcrAnim.fireTime* lcrFPS;

        m_BulletConsumeLabel.text = vo.Level.ToString();
        
		SetHalo (false);
	}

	public void SetHalo(bool isEnable)
	{
        //if (isEnable) {
        //    mMeshRender.sharedMaterial.SetInt ("_IsBend", 1);
        //} else {
        //    mMeshRender.sharedMaterial.SetInt ("_IsBend", 0);
        //}
	}

    private float GetClipLength(Animator animator, string clip) {
        AnimationClip[] ac = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < ac.Length; i++) {
            if (ac[i].name == clip) {
                return ac[i].length;
            }
        }
        return 0f;
    }
	public void Reset(byte seat, float interval)
	{
        m_Seat = seat;
        if (seat > 1) {
            this.mInitRotate_z = 180;
        } else {
            this.mInitRotate_z = 0;
        }
		m_IsPlayAnim = false;
        m_AnimInterval = Mathf.Max(this.GetClipLength(this.m_SkAniatom, AttackAnimName), interval);
	}

	public bool IsLauncher {
		get { return m_IsPlayAnim;}
	}
	public void UpdatePivot()
	{
		if (m_BaseTrans == null)
			return;
		Vector3 pos =   SceneObjMgr.Instance.UICamera.WorldToScreenPoint(m_BaseTrans.position);
		m_GunPivot.x = pos.x;
		m_GunPivot.y = pos.y;
	}

	public float GunFireTick
	{
        get { return hitFiretime; }
	}

	public Action onFireFrame;
	public void Update(float deltaTime)
	{
		m_AnimTimes += deltaTime;
		if (m_IsPlayAnim && m_AnimTimes >= m_AnimInterval)
			StopAnimation();
        if (!m_IsPlayAnim) {
            StandbyAnimation(deltaTime);
            PlayStandFire();
        }
		UpdatePivot();
	}

	void StandbyAnimation(float deltaTime)
	{
        if (m_standyEffGo == null && mLauncherVo.IdleEffID > 0)
		{
			GameObject prefab = FishResManager.Instance.LauncherIdleEff [mLauncherVo.IdleEffID];
			m_standyEffGo = GameObject.Instantiate(prefab) as GameObject;
			m_standyEffGo.transform.SetParent (m_IdleTrans);
			m_standyEffGo.transform.localScale = prefab.transform.localScale;
			m_standyEffGo.transform.localPosition = Vector3.zero;
            m_standyEffGo.transform.localRotation = Quaternion.identity;

		}
		if (m_standyEffGo != null)
			m_standyEffGo.SetActive (true);
	}
    private void PlayStandFire() {
		if (m_standyFireEffGo == null)
		{
            GameObject prefab = FishResManager.Instance.LauncherFireEff[mLauncherVo.IdleFireEffID];
			m_standyFireEffGo = GameObject.Instantiate(prefab) as GameObject;
            m_standyFireEffGo.transform.SetParent(mFireIdleTrans);
			m_standyFireEffGo.transform.localScale = prefab.transform.localScale;
			m_standyFireEffGo.transform.localPosition = Vector3.zero;
            m_standyFireEffGo.transform.localRotation = Quaternion.identity;

        }
        if (m_standyFireEffGo != null && m_standyFireEffGo.activeSelf  == false) {
            m_standyFireEffGo.SetActive(true);
        }
    }

	public void UpdateAngle(float angle) {
        this.mParent.localEulerAngles = new Vector3(0, 0, angle + this.mInitRotate_z);
	}

    public float GetAngle() {
        return this.mParent.localEulerAngles.z - this.mInitRotate_z;
    }

	public void PlayAnimation(float delaySecs)
	{
        //delaySecs = GunFireTick - delaySecs;
		m_IsPlayAnim = true;
		m_AnimTimes = 0;
		if (m_standyEffGo != null)
			m_standyEffGo.SetActive(true);
        if (m_standyFireEffGo != null) {
            m_standyFireEffGo.SetActive(true);
        }
        this.m_SkAniatom.Update(10);//强制跳过10秒，防止动画切换
        this.m_SkAniatom.SetTrigger(AttackParameters);

        //Debug.LogError("delaySecs:" + delaySecs);
        //this.m_SkAniatom.Update(-delaySecs);
        onFireFrame.TryCall();
	}

	void StopAnimation()
	{
        //停止动画 
        m_IsPlayAnim = false;
        this.m_SkAniatom.Update(10);//直接更新10秒跳过动画
	}

	public Transform BaseTrans
	{
		get { return m_BaseTrans; }
	}

	public Transform FireTrans
	{
		get { return m_FireTrans; }
	}

	public Vector2 GunPivot
	{
		get { return m_GunPivot; }
		set { m_GunPivot = value; }
	}

	public uint BulletConsume
	{
		get { return m_BulletConsume; }
		set { m_BulletConsume = value; }
	}
	public void ShutDown()
	{
		if (m_BaseTrans != null)
			GameObject.Destroy(m_BaseTrans.gameObject);
		if (m_standyEffGo != null)
			GameObject.Destroy(m_standyEffGo.gameObject);
		m_standyEffGo = null;
        if (m_standyFireEffGo != null) {
            GameObject.Destroy(m_standyFireEffGo);
            m_standyFireEffGo = null;
        }
		m_BaseTrans = null;
	}
}
