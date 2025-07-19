using System;
using UnityEngine;

public class EnergyPool
{
	UILabel engeryLabel;
	UISlider engeryProgress;
    UISprite mSprPro;
	UISprite mSkillIcon;

    private Launcher mLauncher;
    public void Init(Launcher lcr,UISprite skilIconSp, UISlider progress, UILabel label)
	{
        this.mLauncher = lcr;
		mSkillIcon = skilIconSp;
        this.mSprPro = progress.foregroundWidget as UISprite;
		engeryProgress = progress;
		engeryLabel = label;
		m_CurEnergy = 0;
		m_CurCDTime = 0;
	}

	long         m_EnergyMax;                //能量值
	long          m_CurEnergy;                //当前能量值

	float       m_CDTime;              //大招CD
	float       m_CurCDTime;

	bool        m_bFull;
	bool        m_bLaserCDState;            //是否为CD状态  

	SkillVo mSkillVo = null;
	public void InitEnergy(uint skillID, long initEngery, uint lcrMutil)
	{
		m_bFull = false;
		m_bLaserCDState = false;
		m_CurEnergy = initEngery;
		mSkillVo = FishConfig.Instance.SkillConf.TryGet (skillID);
		m_EnergyMax = (long)(mSkillVo.WorthFactor * lcrMutil);

		mSkillIcon.spriteName = mSkillVo.Icon;
		m_CDTime 	= mSkillVo.CD;
		engeryProgress.value = 0;
        if (m_EnergyMax > 0) {
            engeryLabel.text = "0";
        } else {//炮台能量为0则用CD代替进度条
            engeryLabel.text = "∞";//无限模式下道具只论CD
            m_bLaserCDState = true;
        }
        RefreshEngeryProgress();
	}
	public bool OnLcrMultiChange(uint lcrMutil)
	{
		m_EnergyMax = (long)(mSkillVo.WorthFactor * lcrMutil);
		return RefreshEngeryProgress ();
	}

    public SkillVo SkillVo {
        get {
            return mSkillVo;
        }
    }
    public long EnergyMax {
		get {
            return m_EnergyMax;
		}
	}
	public long CurEnergy {
		get {
			return m_CurEnergy;
		}
	}
    
	public bool UpdateEnergyPool(long energy)
	{
        m_CurEnergy = energy;
        RoleItemModel.Instance.Notifiy(SysEventType.EngeryChange);
		return RefreshEngeryProgress ();
	}

	bool RefreshEngeryProgress()
	{
        if (m_EnergyMax > 0) {
            bool old = m_bFull;
            long cnt = (long)(m_CurEnergy * 1.0f / m_EnergyMax);
            double m_FillAmount = (double)(m_CurEnergy - (cnt * m_EnergyMax)) / (double)m_EnergyMax;
            m_bFull = cnt >= 1;
            engeryProgress.value = (float)m_FillAmount;
            engeryLabel.text = cnt.ToString();
            return !old && m_bFull;
        }else{
            return false;
        }
	}

	public void ConsumeSkEngery()
	{
		if (m_CurEnergy >= m_EnergyMax) 
		{
			m_CurEnergy = Math.Max(0, m_CurEnergy - m_EnergyMax);
            RoleItemModel.Instance.Notifiy(SysEventType.EngeryChange);
		}
		RefreshEngeryProgress ();
	}

	public void PlayCD(float delta)
	{
		if (!m_bLaserCDState)
			return;
		m_CurCDTime += delta;
        if (m_CurCDTime >= m_CDTime) {
            //m_CurCDTime = 0;
            m_bLaserCDState = false;
            if (m_EnergyMax == 0) {
                this.mSkillIcon.IsGray = false;
                this.mSprPro.IsGray = false;
                engeryProgress.value = 1;
                this.mLauncher.UpdateEneryFullStatusEffect();
            }
        } else if (m_EnergyMax == 0) {
            this.mSkillIcon.IsGray = true;
            this.mSprPro.IsGray = true;
            engeryProgress.value = m_CurCDTime / m_CDTime;
        }
	}
	public bool IsCDState
	{
        set {
            if (value) {
                this.m_CurCDTime = 0;
            }
            m_bLaserCDState = value;
        }
		get { return m_bLaserCDState; }
	}

	public float LaserCDTime
	{
		get { return m_CDTime; }
	}

	public bool IsEngeryFull
	{
        get {
            if (m_EnergyMax == 0) {
                return this.m_bLaserCDState == false;
            } else {
                return m_bFull;
            }
        }
	}
}