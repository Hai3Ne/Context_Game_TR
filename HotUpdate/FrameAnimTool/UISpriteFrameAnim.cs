using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public delegate void OnFrameAnimEventCallbackDelegate(string frameName, int frameNumber);

[System.Serializable]
public class SpriteData 
{
    public List<FrameAnimData> AnimList = new List<FrameAnimData>();
}

[System.Serializable]
public class FrameAnimData 
{
    public string Name = "";
    public int Number = 0;
    public List<FrameAnimEvent> EventList = new List<FrameAnimEvent>();
}

[System.Serializable]
public class FrameAnimEvent 
{
    //public event OnFrameAnimEventCallbackDelegate OnFrameAnimEventCallback;
    public string AnimName;                     //所属动画名称
    public string MessageName;                  //事件名称
    public int FrameIndex;                      //播放帧
}

public delegate void OnSpriteFrameCallbackDelegate();


public class UISpriteFrameAnim : MonoBehaviour 
{
    //当前运行帧数据信息
    public string                                       CharacterID = "";
    public string                                       RunningAnimName = ""; //帧名称
    public int                                          RunningFrameNumber = 0; //当前帧动画的帧数

    //帧动画相关数据成员
    private UISprite                                    m_Sprite; //精灵类
    private int                                         m_SpriteIndex = 0; //UISprite中的sprite下标
    private float                                       m_RefreshInterval = 0.1f; //刷新间隔
    private float                                       m_TimeCounter = 0; //计时器
    private int                                         m_AnimIndex = 0; //动画下标，表示运行的是动画列表中第几个动画

    private float                                        m_Speed = 1;

    public SpriteData Data = new SpriteData(); //持久化数据

    [SerializeField]
    public List<FrameAnimEvent>                         m_AnimEventList = new List<FrameAnimEvent>();

    private event OnSpriteFrameCallbackDelegate         m_OnAttackEndCallback;
    private event OnSpriteFrameCallbackDelegate         m_OnAttackLaunchCallback;

	// Use this for initialization
	void Awake () 
    {
        m_Sprite = GetComponent<UISprite>();
        ClearSpriteData();
        ReadSpriteData();
	}

    void Start() 
    {
        if (!string.IsNullOrEmpty(RunningAnimName)) 
        {
            ChangeToAnim(RunningAnimName);
        }
    }

    #region editor method
    public void ReadSpriteData() 
    {
        //读取动画,动画数据根据附加的UISprite里的帧图集数据获得
        UISprite sprite = GetComponent<UISprite>();

        if (sprite == null) return;
        
        List<UISpriteData> spriteList = sprite.atlas.spriteList;
        
        foreach (UISpriteData sd in spriteList) 
        {
            string[] splitNames = sd.name.Split('_');
            
            if (splitNames != null && splitNames.Length == 2) 
            {
                bool isExit = false;
 
                //检查该动画是否已经存在
                foreach (FrameAnimData fad in Data.AnimList) 
                {
                    if (fad.Name.Equals(splitNames[0])) 
                    {
                        isExit = true;
                        fad.Number += 1;
                        break;
                    }
                }

                if (!isExit)
                {
                    FrameAnimData fad = new FrameAnimData();
                    fad.Name = splitNames[0];
                    fad.Number = 1;
                    Data.AnimList.Add(fad);
                }
            }
        }

        //读取动画事件数据,动画事件数据序列化到prefab中
        foreach (FrameAnimEvent fae in m_AnimEventList) 
        {
            bool isExit = false;//该事件是否有对应动画
            FrameAnimData tmpFad = null;
            foreach (FrameAnimData fad in Data.AnimList) 
            {
                if (fad.Name.Equals(fae.AnimName))  //找到对应动画
                {
                    tmpFad = fad;
                    isExit = true;
                    break;
                }
            }

            if (isExit && null != tmpFad) //该事件有对应动画则添加进去 
            {
                tmpFad.EventList.Add(fae);
            }
        }
    }
    public void ClearSpriteData() 
    {
        if (null == Data) return;

        Data.AnimList.Clear();
    }
    #endregion

    #region public method
    public void SetSpriteSize(int w, int h) 
    {
        if (null == m_Sprite) m_Sprite = GetComponent<UISprite>();

        m_Sprite.width = w;
        m_Sprite.height = h;
    }

    public void SetRunningAnimName(string animName) 
    {
        RunningAnimName = animName;
    }

    public void ChangeToAnim(string animName) 
    {
        bool isFound = false;

        int tempAnimIndex = 0;
        foreach(FrameAnimData fad in Data.AnimList)
        {
            if (animName.Equals(fad.Name)) 
            {
                RunningAnimName = fad.Name;
                RunningFrameNumber = fad.Number;
                m_SpriteIndex = 0;

                m_AnimIndex = tempAnimIndex;
                isFound = true;
                break;
            }

            ++tempAnimIndex;
        }
    }

    public void ChangeToNextAnim() 
    {
        ++m_AnimIndex;

        if (m_AnimIndex >= Data.AnimList.Count || m_AnimIndex < 0) m_AnimIndex = 0;

        if (Data.AnimList.Count <= 0) return;

        RunningAnimName = Data.AnimList[m_AnimIndex].Name;
        RunningFrameNumber = Data.AnimList[m_AnimIndex].Number;
    }

    public void ChangeToPreAnim() 
    {
        --m_AnimIndex;

        if (m_AnimIndex >= Data.AnimList.Count || m_AnimIndex < 0) m_AnimIndex = Data.AnimList.Count - 1;

        if (Data.AnimList.Count <= 0) return;

        RunningAnimName = Data.AnimList[m_AnimIndex].Name;
        RunningFrameNumber = Data.AnimList[m_AnimIndex].Number;
    }

    public void ChangeChracter(string charName) 
    {
    
    }

    public void SetFlip(bool isFlip)
    {
        if (null == m_Sprite) return;

        if (isFlip)
            m_Sprite.flip = UIBasicSprite.Flip.Horizontally;
        else
            m_Sprite.flip = UIBasicSprite.Flip.Nothing;
    }
    #endregion

    // Update is called once per frame
	void FixedUpdate () 
    {
        if (string.IsNullOrEmpty(RunningAnimName))
        {
            if (Data.AnimList.Count > 0)
            {
                RunningAnimName = Data.AnimList[0].Name;
                RunningFrameNumber = Data.AnimList[0].Number;
            }
        }

        if (string.IsNullOrEmpty(RunningAnimName)) return;

        m_TimeCounter += Time.fixedDeltaTime;

        if (m_TimeCounter >= m_RefreshInterval * (1f / m_Speed) )
        {
            m_TimeCounter = 0;

            m_SpriteIndex++;
            if (m_SpriteIndex > RunningFrameNumber) m_SpriteIndex = 1;

            string sname = RunningAnimName + "_" + string.Format("{0}", m_SpriteIndex);
            //string sname = RunningAnimName + "_" + string.Format("{0:D2}", m_SpriteIndex);
            m_Sprite.spriteName = sname;

            BroadcastEvent(RunningAnimName, m_SpriteIndex);
        }
    }

    //name动画名称,index帧下标
    void BroadcastEvent(string name, int index) 
    {
        bool isFound = false;

        foreach (FrameAnimData fad in Data.AnimList) 
        {
            if (fad.Name.Equals(name)) 
            {
                foreach (FrameAnimEvent fae in fad.EventList) 
                {
                    if (index == fae.FrameIndex) 
                    {
                        SendMessageUpwards(fae.MessageName);

                        isFound = true;

                        break;
                    }
                }
            }

            if (isFound) break;
        }
    }

    #region set get
    public float AnimSpeed 
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }

    public event OnSpriteFrameCallbackDelegate OnAttackEndCallbackEvent
    {
        add { m_OnAttackEndCallback += value; }
        remove { m_OnAttackEndCallback -= value; }
    }

    public event OnSpriteFrameCallbackDelegate OnAttackLaunchCallbackEvent
    {
        add { m_OnAttackLaunchCallback += value; }
        remove { m_OnAttackLaunchCallback -= value; }
    }
    
    #endregion

    #region anim event
    void OnHit() { Debug.Log("OnHit"); }
    void OnSound() { Debug.Log("OnSound"); }
    void OnEnd() { Debug.Log("OnEnd"); }

    //近战攻击判定
    void OnAttackLaunchCallback() 
    {
        if (null != m_OnAttackLaunchCallback) m_OnAttackLaunchCallback();
    }
    
    void OnBulletLaunchCallback() { Debug.Log("OnBulletLaunchCallback"); }
    
    //攻击动画结束事件
    void OnAttackEndCallback() 
    { 
        if (null != m_OnAttackEndCallback) m_OnAttackEndCallback();
    }
    #endregion
}
