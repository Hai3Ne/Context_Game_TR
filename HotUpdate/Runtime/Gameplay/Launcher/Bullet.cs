using UnityEngine;
using System.Collections.Generic;
public class Bullet:IGlobalEffect
{
    public const float     LOCK_FISH_RADIUS = 5.0f;
    public static float    LIFE_TIME = 10f;
    uint            m_LcrCfgID;
	uint			m_LcrLevel;

    byte            m_ClientSeat;
    uint            m_RateValue;
    float           m_Time;
	ushort          m_ID;
    int            m_ReboundCount;
    float           m_Angle;
	Vector3         m_Pos, bulletStartPos;
    Vector3         m_Dir;
    GameObject      m_Object;
    Transform       m_Trans;
    GameObject      m_BulletParEft;
    Transform       m_ParEftTrans;
    ushort          m_LockFishID = 0;
	byte m_fishPartID = 0;
    Vector3         m_OrgDir;
    float           m_OrgAngle;
    public byte mHandle;//代替机器人处理对应碰撞逻辑

    public float AddCatchRange;//是否增加的范围
    public float mCatchRange;//子弹捕获范围
    public int mCatchCount;//子弹捕获个数

	float mDelayTime = 0;//子弹延迟时间
	short mAngleOffset = 0;
	LauncherVo mlaunchVo;
	uint mHitOnEffectID;
    private List<Transform> mChildList = new List<Transform>();
	public void Init(byte clientSeat, ushort id, Launcher launcher, uint rateValue, Vector3 startPos, Vector3 dir, int reboundCount, ushort lockFishID, byte fishPartID, float delayMSecs = 0)
    {
		mlaunchVo = launcher.Vo;
        this.AddCatchRange = launcher.AddCatchRange;
        this.mCatchRange = launcher.CatchRange;
        this.mCatchCount = launcher.CatchCount;
        if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
            m_Object = SceneLogic.Instance.BulletMgr.CreateBulletObj(clientSeat, mlaunchVo.BulletEffIDSelf, true, true);
            mHitOnEffectID = mlaunchVo.HitOnEffIDSelf;
        } else {
            m_Object = SceneLogic.Instance.BulletMgr.CreateBulletObj(clientSeat, mlaunchVo.BulletEffID, true, true);
            mHitOnEffectID = mlaunchVo.HitOnEffID;
        }
        m_Trans         = m_Object.transform;
        m_ID            = id;
        m_ClientSeat    = clientSeat;
		m_RateValue     = rateValue;
		m_LcrCfgID  = mlaunchVo.LrCfgID;
		m_LcrLevel = mlaunchVo.Level;
        m_Pos           = startPos;
		m_OrgDir = m_Dir = dir;//恢复原始的方向
        m_BulletParEft = SceneLogic.Instance.BulletMgr.CreateBulletTrailObj(launcher.Vo.TrailEffID, startPos, Quaternion.identity);
		m_ParEftTrans   = m_BulletParEft != null ? m_BulletParEft.transform : null;
        m_ReboundCount  = reboundCount;
        m_LockFishID    = lockFishID;
		m_fishPartID	= fishPartID;
        mDelayTime = delayMSecs;
		m_Trans.position = Vector3.zero;

        TimeRoomVo roomVo = SceneLogic.Instance.RoomVo;
        float y_scale = rateValue * 0.3f / roomVo.RoomMultiple + 0.7f;
        //根据炮台倍率修改子弹缩放值
        //Vector3 scale = m_Trans.transform.localScale;
        //m_Trans.transform.localScale = new Vector3(scale.x, scale.y * y_scale, scale.z);
        mChildList.Clear();
        Transform _tf;
        for (int i = 0; i < m_Trans.childCount; i++) {
            _tf = m_Trans.GetChild(i);
            if (_tf.name.StartsWith("__") == false) {
                mChildList.Add(_tf);
                Vector3 scale = _tf.localScale;
                _tf.localScale = scale * y_scale;// new Vector3(scale.x, scale.y * y_scale, scale.z);
            }
        }
    }
	public void SetBranchOffsetAngle(short offsetAng){
		mAngleOffset = offsetAng;
	}
    
	public EffectVo[] ExtraBuff { get { return mExtraBuff; } }
	BulletBufferData[] bufferDataList = null;
	EffectVo[] mExtraBuff = null;
	public void SetBuff(BulletBufferData[] buffer){
		bufferDataList = buffer;
		if (bufferDataList != null && bufferDataList.Length > 0) {
			List<EffectVo> efflist = new List<EffectVo> ();
			for (int i = 0; i < bufferDataList.Length; i++) 
			{
				for (int j = 0; j < bufferDataList [i].effVo.Length; j++)
				{
					if (BulletBufferData.IsApplyAtonce (bufferDataList [i].effVo [j]))
						continue;
					efflist.Add (bufferDataList [i].effVo[j]);
				}
				if (bufferDataList [i].mBbufferVo.HitAnim > 0)
					mHitOnEffectID = bufferDataList [i].mBbufferVo.HitAnim;
			}
			mExtraBuff = efflist.ToArray ();
		}
	}

    public List<Transform> ChildList {
        get {
            return this.mChildList;
        }
    }
	public uint HitOnEffectID
	{
		get { return mHitOnEffectID;}
	}

	Vector3 m_ScreenPos = Vector3.zero;
	public Vector3 ScreenPos
	{
		get { return m_ScreenPos;}
	}
    
	public ushort ID
    {
        get { return m_ID; }
    }
    //public uint ComboID
    //{
    //    get { return m_ComboID; }
    //    set { m_ComboID = value; }
    //}
    public void InitAngle(float angle)
    {
        if (m_Object != null)
        {
            m_OrgAngle = angle;
            if (ClientSeat > 1)
                angle = 180 + angle;
            m_Angle = angle;
            m_Trans.localEulerAngles = new Vector3(0, 0, angle);
        }
    }
    
    public GameObject Object
    {
        get { return m_Object; }
        set { m_Object = value; }
    }
    public Vector3 Position
    {
        get
        {
            return m_Pos;
        }
    }

    public uint RateValue
    {
        get
        {
            return m_RateValue;
        }
    }
    public float Time
    {
        get
        {
            return m_Time;
        }
    }
    public uint LcrCfgID    { get { return m_LcrCfgID; }}

	public uint LcrLevel	{ get { return m_LcrLevel; }}

	public LauncherVo LcrVo { get { return mlaunchVo; }}

	public uint LcrMegerID  { get { return (LcrLevel << 24) | LcrCfgID;}}
    public byte ClientSeat
    {
        get
        {
            return m_ClientSeat;
        }
    }

    public void Destroy()
    {
        if (m_Object != null)
        {
            GameObject.Destroy(m_Object);
        }
		if (m_BulletParEft != null) {
			GlobalEffectData efc = new GlobalEffectData (m_BulletParEft, 0, 2.0f);
			GlobalEffectMgr.Instance.AddEffect (efc);
		}
    }

    void CheckBoundary()
    {
        //裁剪掉屏幕外的子弹
        if (m_ReboundCount == 0)
            return;
        if (m_Pos.x > Utility.NearRightBottomPoint.x || m_Pos.x < Utility.NearLeftTopPoint.x ||
            m_Pos.y > Utility.NearLeftTopPoint.y || m_Pos.y < Utility.NearRightBottomPoint.y)
        {
            if (m_Pos.x > Utility.NearRightBottomPoint.x || m_Pos.x < Utility.NearLeftTopPoint.x)
            {
                if (m_Pos.x > Utility.NearRightBottomPoint.x)
                    m_Pos.x = Utility.NearRightBottomPoint.x;
                else
                    m_Pos.x = Utility.NearLeftTopPoint.x;
                m_Dir.x = -m_Dir.x;
                m_Angle = -m_Angle;
                m_Trans.localEulerAngles = new Vector3(0, 0, m_Angle);
            }
            if (m_Pos.y > Utility.NearLeftTopPoint.y || m_Pos.y < Utility.NearRightBottomPoint.y)
            {
                if (m_Pos.y > Utility.NearLeftTopPoint.y)
                    m_Pos.y = Utility.NearLeftTopPoint.y;
                else
                    m_Pos.y = Utility.NearRightBottomPoint.y;
                m_Dir.y = -m_Dir.y;
                m_Angle = 180 - m_Angle;
                m_Trans.localEulerAngles = new Vector3(0, 0, m_Angle);
            }
            m_Time = 0;
            --m_ReboundCount;
        }
    }

    public ushort LockedFishID
    {
        get
        { 
            return m_LockFishID;
        }
    }

	bool mIsCollion = false;
	public bool IsCollion{
		get { return mIsCollion;}
	}

	float dirLerpTime = 0f, flyDistance = 0f, dirLerpDistance = -1f;
    private Vector3 mLockFishPos;//锁定鱼的坐标
	float curLockDistance = 0f, lastLockDistance = float.MaxValue;
    bool UpdateLockFish()
    {
		Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(m_LockFishID);
		if (fish == null || fish.IsDelay || fish.Catched)
            return false;
		if (fish.IsBossFish && m_fishPartID != 0xFF) {
            fish.GetBodyPartScreenPos(m_fishPartID, out this.mLockFishPos);
		} else {
            this.mLockFishPos = fish.ScreenPos;
		}
        this.mLockFishPos.z = ConstValue.NEAR_Z + 0.1f;
        this.mLockFishPos = Camera.main.ScreenToWorldPoint(this.mLockFishPos);
        m_Dir = this.mLockFishPos - m_Pos;
		curLockDistance = m_Dir.magnitude;
		if (mIsCollion == false && lastLockDistance < curLockDistance) {
			mIsCollion = true;
		}
		if (mAngleOffset != 0) {
			if (dirLerpDistance < 0f) {
				dirLerpDistance = curLockDistance;
			}
			if (dirLerpTime < 1f) {
				flyDistance += (m_Pos - bulletStartPos).magnitude;
				dirLerpTime = flyDistance / dirLerpDistance;
			}

			m_Dir = m_Dir.normalized;
			m_Dir = Vector3.Lerp (m_OrgDir, m_Dir, dirLerpTime);
		} else {
			m_Dir = m_Dir.normalized;
		}
		if (curLockDistance > LOCK_FISH_RADIUS)
        {
            Vector2 UpDir = new Vector2(0, ClientSeat > 1 ? -1 : 1);
            float dot = Vector2.Dot(UpDir, m_Dir);
            m_Angle = Mathf.Acos(Mathf.Clamp(dot, 0.0f, 1.0f)) * Mathf.Rad2Deg;
            if (m_Dir.x >= 0)
                m_Angle = -m_Angle;
            if (ClientSeat > 1)
                m_Angle = 180 - m_Angle;
            m_Trans.localEulerAngles = new Vector3(0, 0, m_Angle);
        }
		lastLockDistance = curLockDistance;
        return true;
    }
    
	public void ClearLockFishID()
    {
        m_Dir = m_OrgDir;
        InitAngle(m_OrgAngle);
        m_LockFishID = 0;
    }

    private void SetBulletPos(Vector3 pos) {//设置子弹真实位置
        m_Trans.position = pos;
        if (m_ParEftTrans != null) {
            m_ParEftTrans.position = pos;
        }
    }
    public bool Update(float delta)
    {
        if (mDelayTime != 0) {
            if (mDelayTime > delta) {
                mDelayTime -= delta;
                return true;
            } else {
                delta -= mDelayTime;
                mDelayTime = 0;
            }
        }

		if (m_Trans.position == Vector3.zero) {
			ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (m_ClientSeat);
			if (sp != null) {
				short lcrAng = (short)(sp.Launcher.LcrAngle + mAngleOffset);
				lcrAng = (short)Mathf.Clamp((int)lcrAng,  (int)Utility.FloatToShort(ConstValue.LaunchRotRangeMin), (int)Utility.FloatToShort(ConstValue.LaunchRotRangeMax));
				float angle = Utility.ShortToFloat (lcrAng);
				if (mAngleOffset == 0) {
					m_Pos = SceneLogic.Instance.FModel.GetBulletPos (m_ClientSeat, sp.Launcher.LcrAngle);
				} else {
					m_Pos = SceneLogic.Instance.FModel.GetBulletPos (m_ClientSeat, lcrAng);
				}
				m_Dir = SceneLogic.Instance.FModel.GetBulletDir (m_ClientSeat, lcrAng);
				m_OrgDir = m_Dir;
				InitAngle( angle );
			}

            this.SetBulletPos(this.m_Pos);
			bulletStartPos = m_Pos;
            //return true;
		}
		if(m_LockFishID != 0)
        {
            if (!UpdateLockFish())
            {
                ClearLockFishID();
            }
        }
        m_Time += delta;
        if (m_Time >= LIFE_TIME)//1.5f保证所有子弹出屏幕外
            return false;
	
        Vector3 move_pos = m_Dir * delta * mlaunchVo.Speed * new Vector3(m_Dir.x * Resolution.AdaptAspect, m_Dir.y).magnitude;//宽屏子弹x轴速度处理
        m_Pos += move_pos;
		var newDir = (this.mLockFishPos - m_Pos).normalized;
		if (m_LockFishID != 0 && Vector3.Dot (m_Dir, newDir) < 0f) {
			m_Pos = this.mLockFishPos;
        }
        this.SetBulletPos(this.m_Pos);
		mlaunchVo.Size = new Vector2 (1f, 1f);
		m_ScreenPos = Utility.MainCam.WorldToScreenPoint (m_Pos);
		m_ScreenPos.z = 0f;

        CheckBoundary();
        return true;
    }
    //*/

    public bool CollionLock() {
        Fish lockedFish = SceneLogic.Instance.FishMgr.FindFishByID(this.LockedFishID);
        if (lockedFish == null) {
            return false;
        }
        if (this.IsCollion) {
            return true;
        }

        if (this.mLockFishPos == this.m_Pos) {
            return true;
        }
        return false;
    }
}
