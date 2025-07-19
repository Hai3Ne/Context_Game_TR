using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class AtkTarget {

	public ushort LockedFishID = 0;
	public Vector3 Position;

	Vector3 mScreenPos;

	Fish fish;
	public Vector3 RealPosition { get { return (fish != null) ? fish.Position : Position;}}
	public Vector3 ScreenPos {get {return (fish != null) ? fish.ScreenPos : mScreenPos;}}
	public AtkTarget(Fish pFish)// p0, Vector3 p1, Vector2 scrren2d, ushort lkfID)
	{
		fish = pFish;
		Position = pFish.Position;
		mScreenPos = pFish.ScreenPos;
		LockedFishID = pFish.FishID;
	}
}
/*
 英雄进场首先从固定位置移动到范围中心点,然后进入寻敌流程
 */
public class Hero : IGlobalEffect
{
	const int AttackDistanceFactor = 1; //攻击距离系数,和配置表攻击距离数据匹配
    private uint m_HeroCfgID;
	private HeroObject3D m_HeroObject;
	private Dictionary<int,int> m_AttackNumber = new Dictionary<int, int>();         //可攻击次数,消耗完则英雄消失
	uint mLifeTicks, mLastTicks;
	int mCurrLifeTicks;
    private bool m_IsBeginLifeCount;     //开始计时存在时间
    private Vector3 m_init_pos;     //英雄出生位置

    private Vector3                                                                 m_OriginalPosition;     //原点,以此为原点作范围内移动
    private ushort                                                                  m_MoveSpeed;            //移动速度
	private ushort                                                                  m_RotSpeed;            //转向速度
    private float                                                                   m_CheckFishInterval;    //间隔一定时间检查鱼是否在活动范围内,而不是每一帧
    private float                                                                   m_CheckFishTimeCount;   //计时器
    private bool                                                                    m_IsSpriteDirty;        //精灵动画状态更新
    private bool                                                                    m_IsHeroStateUpdate;    //英雄状态更新

	private AtkTarget                                                                    m_CurTarget = null;
	private List<AtkTarget>			mTargetList = new List<AtkTarget>();
	float mAtkRange, mAtkRangeSqrt;

    enum HeroActionCommand
    {
        HeroActionCommandNone = 0,
        HeroActionCommandEnterScene,        //英雄进场状态
        HeroActionCommandIdle,              //英雄待机状态
        HeroActionCommandMoveTowardEnemy,   //英雄向敌人移动状态
        HeroActionCommandAttack,            //英雄攻击目标
        HeroActionMoveToInit,           //英雄离开场景前，需要回到初始出生点
		HeroActionCommandLeave,
    };
    
    private HeroActionCommand m_HeroActionCommand = HeroActionCommand.HeroActionCommandNone;
    
	byte mClientSeat;
	public HeroVo mHeroVo;

	static float heroLeaveEffLength = -1f;
	bool isPlayingLeaveEff = false;
	int[] heroActionWeights = new int[0];


	HashSet<sbyte> mHitOnEventStatus = new HashSet<sbyte>();
	HeroHitOnInfo mCurrentHeroHitInfo;
	bool isTriggerAttack = false;
	float animTime = 0f;

	public void Init(HeroVo vo, byte clientSeat,GameObject obj) 
    {
		mClientSeat = clientSeat;
        mHeroVo = vo;
        m_HeroCfgID = vo.CfgID;
        m_HeroObject = new HeroObject3D();
        m_HeroObject.InitHeroResource(obj);
        InitHeroPositionInfo(clientSeat);
		StartEnterScene();
		if (heroLeaveEffLength < 0)
			heroLeaveEffLength = GameUtils.CalPSLife (FishResManager.Instance.HeroLeaveEff);
		mCurrentHeroHitInfo = FishConfig.Instance.heroAttackOnConf.TryGet(mHeroVo.SourceID);
		mHitOnEventStatus.Clear ();
    }

    public void InitHeroData(int[] atkTimes, uint startTicks, ushort anim, Vector3 pos, Vector3 dir) {
        if (pos == Vector3.zero) {//如果坐标是原点，则表名之前英雄数据未存储，强制设置成出生点
            pos = this.m_init_pos;
        }
		for (int i = 0; i < m_AttackNumber.Count; i++) {
			m_AttackNumber [i] = atkTimes [i];
		}

		mLastTicks =UTool.GetTickCount ();
        uint tickSpan = mLastTicks - startTicks;//Utility.GetTickCount ();
        mCurrLifeTicks = (int)mLifeTicks - (int)tickSpan;
		m_HeroObject.UpdatePosition (pos);
		if (anim == (ushort)AvaterAnimStatus.atk){
			dir = (dir - pos).normalized;
		}
		m_HeroObject.FaceToAtkTargt (dir, m_RotSpeed, 1f);
	}

	public uint HeroCfgID	{get {return m_HeroCfgID;}}

	private void InitHeroPositionInfo(byte clientSeat) 
	{
		m_CheckFishInterval = 0.1f;
		m_CheckFishTimeCount = 0;

		//配置信息
		m_AttackNumber = new Dictionary<int, int>();
		heroActionWeights = new int[mHeroVo.ActionList.Length];
		for (int i = 0; i < mHeroVo.ActionList.Length; i++) {
			HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet(mHeroVo.ActionList [i]);
			m_AttackNumber [i] = actVo.AttTime;
			heroActionWeights [i] = actVo.AniTime;
		}

		m_MoveSpeed = mHeroVo.MoveSpeed;
		m_RotSpeed = mHeroVo.RotSpeed;
        mLifeTicks = mHeroVo.Duration + 1000; //客户端CD加1秒防止时间不同步
		mCurrLifeTicks = (int)mLifeTicks;
		mLastTicks =UTool.GetTickCount ();

        m_HeroObject.InitHeroPositionInfo(clientSeat, ref m_OriginalPosition);
        this.m_init_pos = HeroUtilty.CalHeroInitWorldPos (clientSeat, ConstValue.HERO_Z);
	}

	public void LeaveScene()
	{
		EnterAction(HeroActionCommand.HeroActionMoveToInit);
	}

	public void Dispose()
	{
		if (m_HeroObject != null) 
		{
			m_HeroObject.Destroy ();
		}
		m_HeroActionCommand = HeroActionCommand.HeroActionCommandNone;
		m_HeroObject = null;
			
	}

	public uint TotalLifeTicks
	{
		get { return mLifeTicks; }
	}

	public int CurrLifeTicks
	{
		get { return mCurrLifeTicks;}
        set { mCurrLifeTicks = value; }
	}

	public bool Update (float delta) 
    {
		uint tick =UTool.GetTickCount ();
		uint span = tick >= mLastTicks ? tick - mLastTicks : 0;
		mCurrLifeTicks -= (int)span;
		mLastTicks = tick;

		CheckFishPool();

		return ActionUpdate(delta);
	}
    private void StartEnterScene() 
    {
        this.EnterAction(HeroActionCommand.HeroActionCommandEnterScene);
    }

	bool IsRemoteAtk()
	{
		return (currHeroActionVo != null && currHeroActionVo.BulletCfgID > 0);
	}

    //检查范围内的鱼,确定攻击对象
    private void CheckFishPool() 
    {
        if (this.m_HeroActionCommand != HeroActionCommand.HeroActionCommandIdle) {
            return;
        }

        //每隔一定时间检测一次而不是每一帧
        if (m_CheckFishTimeCount < m_CheckFishInterval)
        {
            m_CheckFishTimeCount += Time.deltaTime;
            return;
        }
		currentActionIndex = Utility.Random_Weight(heroActionWeights);
		if (currentActionIndex < 0) {
			return;
		}

		uint pAttackDistance = currHeroActionVo.AttDistance;
		mAtkRange = AttackDistanceFactor * pAttackDistance;
		mAtkRangeSqrt = Mathf.Pow (mAtkRange, 2f);
		#if UNITY_EDITOR
		m_HeroObject.rangeCircleGo.transform.localScale = Vector3.one * (mAtkRange * 2f);
		#endif


		bool isRemote = IsRemoteAtk ();
		m_CheckFishTimeCount = 0;
        ushort lockfishID = SceneLogic.Instance.PlayerMgr.LockedFishID;
        if (lockfishID != 0)
        {
			mTargetList.Clear ();
            Fish lockfish = SceneLogic.Instance.FishMgr.FindFishByID(lockfishID);
            if (lockfish != null)
            {
				Vector3 lockfishLocalPos = HeroUtilty.Screen2UIPos(lockfish.ScreenPos);
				if (IsWithinBoundary(lockfishLocalPos))//炮台锁定的鱼在英雄攻击范围内
                {
					m_CurTarget = new AtkTarget(lockfish);
					mTargetList.Add (m_CurTarget);
					if (isRemote) {
                        this.EnterAction(HeroActionCommand.HeroActionCommandAttack);
                    } else {
                        this.EnterAction(HeroActionCommand.HeroActionCommandMoveTowardEnemy);
					}
                    return;
                }
            }
        }

      
        bool isCurrentFishStillIn = false;//当前攻击的鱼是否还在范围内
		uint maxQuality = 0;
		Stack<Fish> pStack = new Stack<Fish> ();
        foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
            if (IsWithinBoundary(HeroUtilty.Screen2UIPos(fish.ScreenPos)))  //在范围内
            {
                selectTargetFishes(fish, pStack, ref maxQuality);
            }
        }

		if (isCurrentFishStillIn) 
		{
			for (int i = 0; i < mTargetList.Count;) {
				if (!IsWithinBoundary (HeroUtilty.Screen2UIPos (mTargetList [i].ScreenPos))) {
					Utility.ListRemoveAt (mTargetList, i);
				} else
					i++;
			}
			//ChangeRenderColor(m_CurTarget, Color.red);
            if (isRemote) {
                this.EnterAction(HeroActionCommand.HeroActionCommandAttack);
            } else {
                this.EnterAction(HeroActionCommand.HeroActionCommandMoveTowardEnemy);
            }
			if (!mTargetList.Contains (m_CurTarget)) {
				mTargetList.Add (m_CurTarget);
			}
		}
		else
		{
			mTargetList.Clear ();
			Fish targetFish = null;
			if (pStack.Count > 0) {
				Vector3 heroSrcPos = HeroUtilty.WorldPosToUIPos (m_HeroObject.Position);
				heroSrcPos.z = 0f;
				targetFish = HeroUtilty.SelectMinDisTarget (pStack, heroSrcPos);
				Vector3 heroDir = targetFish.ScreenPos - heroSrcPos;
				HeroUtilty.SelectMinDisTargets (pStack, mTargetList, heroSrcPos, heroDir);

			}

            if (targetFish != null)
            {
				m_CurTarget = new AtkTarget(targetFish);
				if (!mTargetList.Contains(m_CurTarget))
					mTargetList.Add(m_CurTarget);

                if (isRemote) {
                    this.EnterAction(HeroActionCommand.HeroActionCommandAttack);
                } else {
                    this.EnterAction(HeroActionCommand.HeroActionCommandMoveTowardEnemy);
                }
            }
			else 
			{
				mTargetList.Clear ();
				m_CurTarget = null;
				if (m_HeroActionCommand != HeroActionCommand.HeroActionCommandIdle)
				{
                    this.EnterAction(HeroActionCommand.HeroActionCommandIdle);
				}
            }
        }
    }


	void selectTargetFishes(Fish fish, Stack<Fish> pStack, ref uint maxQuality)
	{
		FishVo mFishVo = FishConfig.Instance.FishConf.TryGet(fish.FishCfgID);
		//范围内选择品质最高的鱼
		if (mFishVo.Quality > maxQuality) {
			maxQuality = mFishVo.Quality;
			pStack.Push (fish);
		} else if (mFishVo.Quality == maxQuality) {
			pStack.Push (fish);
		}
	}

    private void EnterAction(HeroActionCommand action) {
        if (m_HeroActionCommand == HeroActionCommand.HeroActionCommandLeave)
            return;
        if (m_HeroActionCommand == HeroActionCommand.HeroActionMoveToInit && action != HeroActionCommand.HeroActionCommandLeave)
            return;
        m_HeroActionCommand = action;
        m_IsSpriteDirty = true;
        m_IsHeroStateUpdate = true;
//		LogMgr.LogError ("EnterAction..."+m_HeroActionCommand);
    }

    //更新行动
	private bool ActionUpdate(float delta) 
    {
		if (m_HeroActionCommand == HeroActionCommand.HeroActionCommandNone)
			return true;

        switch (m_HeroActionCommand) {
            case HeroActionCommand.HeroActionCommandEnterScene:
                if (m_HeroObject.IsInStats("Idle")) {
                    this.EnterAction(HeroActionCommand.HeroActionCommandIdle);
                }
                break;

            case HeroActionCommand.HeroActionCommandIdle:
                this.HeroIdleHandle(delta);
                break;

            case HeroActionCommand.HeroActionCommandMoveTowardEnemy:
                HeroMoveTowardEnemyHandle();
                break;
            case HeroActionCommand.HeroActionCommandAttack:				
                HeroAttackHandle(delta);
                break;
            case HeroActionCommand.HeroActionMoveToInit:				
                this.MoveToOriginal();
                break;
            case HeroActionCommand.HeroActionCommandLeave:
                var staInfo = m_HeroObject.GetCurrentAnimatorStatus();
                if (staInfo.IsName("leave")) {
                    float len = staInfo.length * (1f - staInfo.normalizedTime);
                    if (isPlayingLeaveEff == false && len < heroLeaveEffLength) {
                        isPlayingLeaveEff = true;
                        GameObject effInst = GameUtils.CreateGo(FishResManager.Instance.HeroLeaveEff);
                        effInst.transform.position = m_HeroObject.Transform.position;
                        effInst.transform.localScale = m_HeroObject.Transform.localScale;
                        AutoDestroy.Begin(effInst);
                    }
                }
                if (m_HeroObject.IsOverStats("leave")) {
                    return false;
                }
                break;

            default:
                break;
        }
		return true;
    }


	int currentActionIndex = 0;
    #region hero command handle   
    private float idle_time = 0;//空闲时间
    private Vector3 idle_move_target;
    private bool is_wait_move = true;//是否等待空闲行走
    private void HeroIdleHandle(float delta) {
        if (m_IsSpriteDirty) {
            m_HeroObject.HeroIdle();
            m_IsSpriteDirty = false;

            is_wait_move = true;
            idle_time = 0;
            idle_move_target = this.m_init_pos + new Vector3(Utility.Range(-130f, 130f), 0);
        }
        if (m_IsHeroStateUpdate) {
            SendSyncHeroQuest(Vector3.zero, 0, (byte)AvaterAnimStatus.idle, 0);
            m_IsHeroStateUpdate = false;
        }
        idle_time += delta;
        if (idle_time > 3) {//空闲一定时间后，英雄做自由运动
            ushort move_spd = m_MoveSpeed;
            move_spd /= 3;

            if (Vector3.SqrMagnitude(idle_move_target - m_HeroObject.Position) < move_spd * Time.deltaTime * move_spd * Time.deltaTime) {//判断当前帧是否能到达目标点
                //到达目的地后，等待一段时间，继续行走
                m_IsSpriteDirty = true;
            }
            Vector3 worldDirection = (idle_move_target - m_HeroObject.Position).normalized;
            if (is_wait_move || m_IsHeroStateUpdate || Vector3.Dot(lastWorldDirection, worldDirection) < 0.8f) {
                SendSyncHeroQuest(idle_move_target, move_spd, (byte)AvaterAnimStatus.run, 0, 0);
                m_IsHeroStateUpdate = false;
                isRotatingDir = true;
            }

            if (m_HeroObject.HeroMove(move_spd, m_RotSpeed, worldDirection, Time.deltaTime, ref is_wait_move)) {
                if (isRotatingDir == true) {
                    SendSyncHeroQuest(Vector3.zero, move_spd, (byte)AvaterAnimStatus.run, 0, 0);
                    isRotatingDir = true;
                }
            }

            lastWorldDirection = worldDirection;
        }
    }
	private void HeroAttackHandle(float delta)
	{
		AttackMotionHiton(delta);
		if (m_HeroActionCommand != HeroActionCommand.HeroActionCommandAttack)
			return;
		if (m_IsSpriteDirty == false) {
			return;
		}

        if (m_IsHeroStateUpdate)
        {
			SendSyncHeroQuest(m_CurTarget.RealPosition, 0, (byte)AvaterAnimStatus.atk, (byte)currentActionIndex);
            m_IsHeroStateUpdate = false;
        }

		Vector3 direct = (m_CurTarget.Position - m_HeroObject.Position).normalized;
		if (m_HeroObject.FaceToAtkTargt (direct, m_RotSpeed, delta)) {
			HeroAttack ();
		}
    }

    private void MoveToOriginal() {//移动到原点
        //判断当前帧是否能到达目标点
        if (Vector3.SqrMagnitude(m_init_pos - m_HeroObject.Position) < m_MoveSpeed * Time.deltaTime * m_MoveSpeed * Time.deltaTime) {
            this.EnterAction(HeroActionCommand.HeroActionCommandLeave);
            m_HeroObject.ChangeAnimState((byte)AvaterAnimStatus.die);
        }
        Vector3 worldDirection = (m_init_pos - m_HeroObject.Position).normalized;
        if (m_IsHeroStateUpdate || Vector3.Dot(lastWorldDirection, worldDirection) < 0.8f) {
            m_IsHeroStateUpdate = false;
            isRotatingDir = true;
        }

        if (m_HeroObject.HeroMove(m_MoveSpeed, m_RotSpeed, worldDirection, Time.deltaTime, ref m_IsSpriteDirty)) {
            if (isRotatingDir == true) {
                isRotatingDir = true;
            }
        }

        lastWorldDirection = worldDirection;
    }

	Vector3 lastWorldDirection = Vector3.zero;
	bool isRotatingDir = false;
    private void HeroMoveTowardEnemyHandle() 
    {
        if (m_CurTarget != null)
        {
			if (!IsWithinBoundary(HeroUtilty.Screen2UIPos(m_CurTarget.ScreenPos))) 
            {
//                ChangeRenderColor(m_CurTarget, Color.white);
                m_CurTarget = null;
                this.EnterAction(HeroActionCommand.HeroActionCommandIdle);
                return;
            }

			//dis.z = 0f;
			Vector3 worldDirection = (m_CurTarget.RealPosition - m_HeroObject.Position).normalized;
			if (CheckInAtkRange())
            {
				if (m_IsHeroStateUpdate || Vector3.Dot(lastWorldDirection, worldDirection)<0.8f)
                {
					SendSyncHeroQuest(m_CurTarget.RealPosition, m_MoveSpeed, (byte)AvaterAnimStatus.run, 0, m_CurTarget.LockedFishID);
                    m_IsHeroStateUpdate = false;
					isRotatingDir = true;
                }

				if (m_HeroObject.HeroMove (m_MoveSpeed, m_RotSpeed, worldDirection, Time.deltaTime, ref m_IsSpriteDirty)) {
					if (isRotatingDir == true) {
						SendSyncHeroQuest (Vector3.zero, m_MoveSpeed, (byte)AvaterAnimStatus.run, 0, m_CurTarget.LockedFishID);
						isRotatingDir = true;
					}
				}

				lastWorldDirection = worldDirection;
            }
			else
            {
                this.EnterAction(HeroActionCommand.HeroActionCommandAttack);
            }
        }
        else {
            this.EnterAction(HeroActionCommand.HeroActionCommandIdle);
		}
    }

	bool CheckInAtkRange()
	{
		Vector3 uiVec = HeroUtilty.WorldPosToUIPos (m_CurTarget.RealPosition) - HeroUtilty.WorldPosToUIPos(m_HeroObject.Position);
		uiVec.z = 0f;
		return uiVec.sqrMagnitude >= mAtkRangeSqrt;
	}
	/*
    const float ZERO_DISTANCE = 5f;
    private void HeroEnterSceneHandle() 
    {

		Vector3 heroUIPos = HeroUtilty.WorldPosToUIPos(m_HeroObject.GetPosition ());
        Vector3 areaSrcCenter = m_OriginalPosition;
		if (Vector3.Distance(heroUIPos, m_OriginalPosition) > ZERO_DISTANCE)
        {
			Vector3 dir = (areaSrcCenter - heroUIPos).normalized;
            
            if (m_IsHeroStateUpdate)
            {
				SendSyncHeroQuest(dir, m_MoveSpeed, m_RotSpeed, (byte)AvaterAnimStatus.run);
                m_IsHeroStateUpdate = false;
            }

            HeroMove(dir);
        }
		else //结束,进入idle状态
        {
			m_HeroObject.SetUIPosition(m_OriginalPosition);
            m_HeroActionCommand = HeroActionCommand.HeroActionCommandIdle;
            m_IsSpriteDirty = true;
            m_IsHeroStateUpdate = true;
        }
    }

    private void HeroIdleHandle() 
    {
       
    }
	//*/
    #endregion

    #region hero action


    private void HeroAttack() 
    {
        if (m_IsSpriteDirty) 
        {
			float speed = HeroUtilty.HeroAttackAnimSpeed;
			m_HeroObject.HeroAttack(currentActionIndex,speed);
            m_IsSpriteDirty = false;
			isTriggerAttack = true;
			mHitOnEventStatus.Clear ();
			animTime = 0f;
			heroActionWeights [currentActionIndex] =  Mathf.Max(0, heroActionWeights [currentActionIndex]  - 1);
//			LogMgr.LogError (currentActionIndex+"@@@@@@@@@@@@@@@@@ "+heroActionWeights[0]+","+heroActionWeights[1]+","+heroActionWeights[2]);

        }
    }
    #endregion

	void AttackMotionHiton(float delta){
		if (isTriggerAttack == false) {
			animTime = 0f;
			return;
		}

		float atime = 0f;
		sbyte n = (sbyte)mCurrentHeroHitInfo.hitClips [currentActionIndex].hitTimes.Length;
		animTime += delta;
        int hit_count = mHitOnEventStatus.Count;
		for (sbyte i = 0; i < n; i++) {
			atime = mCurrentHeroHitInfo.hitClips [currentActionIndex].hitTimes [i];
			atime = atime / m_HeroObject.AnimSpeed;
			if (mHitOnEventStatus.Contains(i) == false && animTime >= atime) {
				mHitOnEventStatus.Add (i);
                OnAttackHitOn();
			}
        }
        if (hit_count == 0 && mHitOnEventStatus.Count > 0) {//第一次触发hiton播放音效
            GlobalAudioMgr.Instance.PlayAudioEff(currHeroActionVo.AudioID);
        }

		atime = mCurrentHeroHitInfo.hitClips [currentActionIndex].length;
		atime = atime / m_HeroObject.AnimSpeed;
		if (animTime >= atime) {
			OnAttackEndCallback ();
			mHitOnEventStatus.Clear ();
		}
	}

    #region sprite fram animtion event callback
    //攻击动画结束事件
    void OnAttackEndCallback() {
		isTriggerAttack = false;
        this.EnterAction(HeroActionCommand.HeroActionCommandIdle);
    }

    //攻击动画攻击判定事件
	void OnAttackHitOn() 
    {
        if (m_CurTarget != null) {
			if (currHeroActionVo.BulletCfgID > 0)
            {
				SendHeroLaunchBullet ();
            }
			else 
            {
				SendHeroMeleeAttack();                
            }
//			LogMgr.LogError ("atkNums:"+m_AttackNumber[0]+","+m_AttackNumber[1]+","+m_AttackNumber[2]+"    "+heroActionWeights[0]+","+heroActionWeights[1]+","+heroActionWeights[2]);
        }
    }
    #endregion


	HeroActionVo currHeroActionVo
	{
		get 
		{
			uint atkActionId = mHeroVo.ActionList [currentActionIndex];
			HeroActionVo actVo = FishConfig.Instance.HeroActionConf.TryGet (atkActionId);
			return actVo;
		}
	}

    private void SendHeroMeleeAttack() 
    {
		ushort[] fishIDList = SceneLogic.Instance.CollisonMgr.CollisonRangeFishes(m_CurTarget, currHeroActionVo.Range);
        Vector3 worldPos = m_CurTarget.RealPosition;
        SceneLogic.Instance.CollisonMgr.DestroyCollider(worldPos, 0, currHeroActionVo.Range, currHeroActionVo.HitEffID, 0, currHeroActionVo.HitEffDelay,null);
		if (m_AttackNumber[currentActionIndex] > 0) {//攻击次数不足，则不触发攻击
			FishNetAPI.Instance.SendHeroCollion(0, mHeroVo.CfgID, currHeroActionVo.ActionCfgID, fishIDList);
			ConsumeAtkNumber(currentActionIndex);
		}
    }

    private bool SendHeroLaunchBullet() 
    {
		bool succ = false;
		ushort LockedFishID = m_CurTarget.LockedFishID;
		Vector3 targetPos = m_CurTarget.Position;
		Vector3 startPos = m_HeroObject.LcrPos;
		uint actionID = mHeroVo.ActionList [currentActionIndex];
		HeroFirePosition[] poslist = m_HeroObject.FirePositionList;
		if (poslist != null && poslist.Length > 0) {
			m_HeroObject.ClearFireSlots ();
			int j = 0;
			for (int i = 0; i < poslist.Length; i++) {
				j = i % mTargetList.Count;
				if (i >= poslist.Length)
					break;
				if (m_AttackNumber [currentActionIndex] > 0) {
					targetPos = mTargetList [j].Position;
					LockedFishID = mTargetList [j].LockedFishID;
                    if (SceneLogic.Instance.BulletMgr.LaunchHeroBullet(HeroCfgID, LockedFishID, actionID, targetPos)) {
                        ConsumeAtkNumber(currentActionIndex);
                        succ = true;
                    }
				}
			}
		} else {
			if (m_AttackNumber [currentActionIndex] > 0) {
				Vector3 bulletDir = (targetPos - startPos).normalized;
				Vector3 direct = targetPos - startPos;
				float floatAngle = SceneHeroMgr.CalculateDegreeToFloatValue (direct);
				m_HeroObject.Transform.localEulerAngles = new Vector3 (0, floatAngle, 0);
                if (SceneLogic.Instance.BulletMgr.LaunchHeroBullet(HeroCfgID, LockedFishID, actionID, bulletDir)) {
                    ConsumeAtkNumber(currentActionIndex);
                    succ = true;
                }
			}
		}

		if (succ == false) {
			LogMgr.LogWarning("succ");
		}
		return succ;
    }

	void ConsumeAtkNumber(int actionIdx)
	{
		m_AttackNumber [actionIdx] -= 1;
	}

	Vector4 ScrOffset = new Vector4 (10f, -10f, -10, 10f);
    //检查传入的位置是否超出英雄活动范围,pos必须为本地坐标
    public bool IsWithinBoundary(Vector3 pos) 
    {
		bool isRemote = IsRemoteAtk ();
		Vector4 rect = isRemote ? (HeroUtilty.ScreenUIRect+ScrOffset) : HeroUtilty.CalArea (mClientSeat, m_HeroObject.Position.z);
        bool ret = false;

		if (pos.x > (rect.x) &&
			pos.x < (rect.z) &&
			pos.y > (rect.w) &&
			pos.y < (rect.y))
        {
            ret = true;
        }
        return ret;
    }


	void SendSyncHeroQuest(Vector3 targetPos, ushort moveSpeed, byte anim, byte subclip, ushort fishID = 0) 
    {
		FishNetAPI.Instance.SendHeroSyncLaunch(mHeroVo.CfgID, targetPos, moveSpeed, m_HeroObject.Position, anim, subclip,fishID);
    }

	public HeroObject3D HeroObj 
    {
        get 
        {
            return m_HeroObject;
        }
    }

    //IGlobalEffect interface
    public Vector3 Position
    {
        get
        {
            return m_HeroObject.Position;
        }
    }
}
