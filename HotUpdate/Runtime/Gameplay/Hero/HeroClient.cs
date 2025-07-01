using UnityEngine;
using System.Collections;

public class HeroClient : IGlobalEffect
{
	private uint                                              m_HeroCfgID;            //cfgID
    
	private HeroClientObject3D                                  m_HeroClientObject;     //其他玩家英雄实例对象
    //HeroClientObject 
    private Vector3                                             m_Position;             //当前对象位置
    private Transform                                           m_ModelTransform;       //当前对象变换
    private GameObject                                          m_Model;                //对象GameObject引用

    private UISpriteFrameAnim                                   m_Sprite;
    //
	private bool                                                m_IsSpriteDirty;        //精灵动画状态更新
	public HeroVo mHeroVo;
	uint mHeroCfgID;
    byte mClientSeat = 0;
    private Vector3 m_init_pos;     //英雄出生位置


	int m_MoveSpeed;             //移动速度
	Vector3 m_MoveDir;               //运动方向
	Vector3 m_NewPostion,m_NewTargetPos;
	ushort m_RotSpeed = 1;
	ushort m_fishID = 0;
	byte m_AnimID, m_Subclip;

	float heroLeaveEffLength = -1f;
	bool isPlayingLeaveEff = false;
    public void Init(HeroVo vo, byte clientSeat,GameObject obj) 
    {
        m_HeroCfgID = vo.CfgID;
		mClientSeat = clientSeat;
		m_HeroClientObject = new HeroClientObject3D();
        mHeroVo = vo;
        m_HeroClientObject.InitHeroResource(obj);
		m_RotSpeed = mHeroVo.RotSpeed;
		InitHeroPositionInfo(clientSeat);
		if (heroLeaveEffLength < 0)
			heroLeaveEffLength = GameUtils.CalPSLife (FishResManager.Instance.HeroLeaveEff);
		isPlayingLeaveEff = false;
    }

	private void InitHeroPositionInfo(byte clientSeat) 
	{
        m_HeroClientObject.InitHeroPositionInfo(clientSeat);
        m_init_pos = HeroUtilty.CalHeroInitWorldPos(clientSeat, ConstValue.HERO_Z);
	}

	public void InitHeroData(ushort anim, Vector3 pos, Vector3 dir)
	{
		m_HeroClientObject.ChangeAnimState ((byte)anim);
		m_HeroClientObject.UpdatePosition (pos);
		if (anim == (ushort)AvaterAnimStatus.atk) {
			dir = (dir - pos).normalized;
		}
		m_HeroClientObject.FaceToAtkTargt (dir, mHeroVo.RotSpeed, 1);
	}

	public uint HeroCfgID	{get {return m_HeroCfgID;}}

	bool changeAnimState = false;
	bool isDing = false;
	float mAnimSpeed = 1f;
    public void UpdateHeroState(HeroSyncData pack) 
    {
		if (m_HeroClientObject == null)
			return;
        //LogMgr.LogError ("HeroClient  Anim:"+((AvaterAnimStatus)pack.Anim)+"startpos="+pack.startPos+" tar:"+pack.targetPos+" "+pack.Speed);
		m_MoveDir = Vector3.zero;
        m_MoveSpeed = pack.Speed;
		m_AnimID = pack.Anim;
		m_Subclip = pack.subclip;
		m_fishID = pack.FishID;
		m_NewPostion = HeroUtilty.WorldPositionInverse(mClientSeat, pack.startPos);
		m_NewTargetPos = HeroUtilty.WorldPositionInverse(mClientSeat, pack.targetPos);
		if (pack.targetPos != Vector3.zero) {
			m_MoveDir = (m_NewTargetPos - m_NewPostion).normalized;
		}
		mAnimSpeed = AvaterAnimStatus.atk == (AvaterAnimStatus)m_AnimID ? HeroUtilty.HeroAttackAnimSpeed : 1f;
		m_HeroClientObject.UpdatePosition (m_NewPostion);
		if (m_MoveDir == Vector3.zero || m_HeroClientObject.FaceToAtkTargt (m_MoveDir, m_RotSpeed, Time.deltaTime)) {
			changeAnimState = false;
			m_HeroClientObject.ChangeAnimState (m_AnimID, m_Subclip, mAnimSpeed);
		} else {
			changeAnimState = true;
		}
    }

    private bool is_move_init = false;//是否移动到初始点
    public bool Update(float delta)
    {
		if (m_HeroClientObject == null)
			return true;
		if (isDing) {
            if (is_move_init == false) {
                m_MoveDir = (m_init_pos - m_HeroClientObject.Position).normalized;
                if (m_HeroClientObject.FaceToAtkTargt(m_MoveDir, m_RotSpeed, delta)) {
                    if (Vector3.SqrMagnitude(m_init_pos - m_HeroClientObject.Position) < mHeroVo.MoveSpeed * delta * mHeroVo.MoveSpeed * delta) {
                        is_move_init = true;
                        m_HeroClientObject.ChangeAnimState((byte)AvaterAnimStatus.die);
                    }
                    m_HeroClientObject.Move(m_MoveDir, mHeroVo.MoveSpeed, delta);
                }
            } else {
                var staInfo = m_HeroClientObject.GetCurrentAnimatorStatus();
                if (staInfo.IsName("leave")) {
                    float len = staInfo.length * (1f - staInfo.normalizedTime);
                    if (isPlayingLeaveEff == false && len < heroLeaveEffLength) {
                        isPlayingLeaveEff = true;
                        GameObject effInst = GameUtils.CreateGo(FishResManager.Instance.HeroLeaveEff);
                        effInst.transform.position = m_HeroClientObject.Transform.position;
                        effInst.transform.localScale = m_HeroClientObject.Transform.localScale;
                        AutoDestroy.Begin(effInst);
                    }
                }
                if (m_HeroClientObject.IsOverStats("leave")) {
                    return false;
                }
            }
			return true;
		}

		if (m_AnimID == (byte)AvaterAnimStatus.run) {
			var tarFish = SceneLogic.Instance.FishMgr.FindFishByID (m_fishID);
			Vector3 targetPos = m_NewTargetPos;
			if (tarFish != null)
				targetPos = tarFish.Position;
			m_MoveDir = (targetPos - m_HeroClientObject.Position).normalized;
			if (m_HeroClientObject.FaceToAtkTargt (m_MoveDir, m_RotSpeed, delta))
				m_HeroClientObject.Move (m_MoveDir, m_MoveSpeed, delta);
		}

		if (changeAnimState) {
			if (m_AnimID == (byte)AvaterAnimStatus.atk) {
				if (m_HeroClientObject.FaceToAtkTargt (m_MoveDir, m_RotSpeed, delta)) {
					m_HeroClientObject.ChangeAnimState (m_AnimID, m_Subclip, mAnimSpeed);
					changeAnimState = false;
				}
			} else {
				m_HeroClientObject.ChangeAnimState (m_AnimID, m_Subclip, mAnimSpeed);
				changeAnimState = false;
			}
		}
        return true;
    }

    public Vector3 Position
    {
        get
        {
            return m_HeroClientObject.Position;
        }
    }

	public HeroClientObject3D HeroObj
	{
		get {return m_HeroClientObject;}
	}

	public void LeaveScene()
	{
		isDing = true;
	}

	public void Dispose()
	{
		if (m_HeroClientObject != null) 
		{
			m_HeroClientObject.Destroy ();
			m_HeroClientObject = null;
		}
	}
}
