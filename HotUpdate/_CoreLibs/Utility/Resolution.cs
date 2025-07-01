using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
public enum ResolutionLevel
{
    RESOLUTION_HIGH,
    RESOLUTION_MID,
    RESOLUTION_LOW,
    RESOLUTION_ORIGNAL,
}


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)] 
public struct WinRect 
{ 
	public int left;
	public int  top;
	public int  right;
	public int  bottom;
} 

public class Resolution
{
    public const float DEFAULT_ASPECT = 1.777777777f;
    static int      m_nScreenWidth;
    static int      m_nScreenHeight;

	static int mReallWidth, mRealHeight;
    static float    m_nOrgAspect;
    static bool     m_bIsHD;
    private static float _adapt_aspect;
    private static float _view_aspect;//视图范围比例  超出16x9范围60%

    public static int ScreenWidth
    {
		get { return m_nScreenWidth; }
    }

    public static int ScreenHeight
	{
		get { return m_nScreenHeight; }
    }
    public static float AdaptAspect {//分辨率与16/9的x轴差别比例
        get {
            return _adapt_aspect;
        }
    }
    public static float ViewAdaptAspect {//分辨率与16/9的x轴显示差别比例
        get {
            return _view_aspect;
        }
    }
    public static bool IsHD
    {
        get
        {
            return m_bIsHD;
        }
    } 
    
    public static void GlobalInit()
	{
		#if UNITY_EDITOR
		m_nScreenWidth = Screen.width;
		m_nScreenHeight = Screen.height;
		#else
		m_nScreenWidth = Screen.currentResolution.width;
		m_nScreenHeight = Screen.currentResolution.height;
		#endif

		m_nOrgAspect = (float)m_nScreenWidth / m_nScreenHeight;
		if (m_nOrgAspect >= DEFAULT_ASPECT) {
			mRealHeight = m_nScreenHeight;
			mReallWidth = (int)(mRealHeight * DEFAULT_ASPECT);
		} else {
			mReallWidth = m_nScreenWidth;
			mRealHeight = (int)(mReallWidth / DEFAULT_ASPECT);
		}
        _adapt_aspect = Camera.main.aspect / DEFAULT_ASPECT;
        _view_aspect = Mathf.Min(_adapt_aspect, 2.008f / DEFAULT_ASPECT);
        //Camera.main.aspect = DEFAULT_ASPECT;
	}
}
