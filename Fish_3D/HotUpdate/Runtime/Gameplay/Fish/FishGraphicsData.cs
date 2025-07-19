using UnityEngine;
using System;
using System.Collections.Generic;



//鱼阵阵型类
[Serializable]
public class FishArrayGraphics
{
    [Header("三角形阵型")]
    public TriangleFishArrayGraphics triangleFishArray;
    [Header("矩形阵型")]
    public RectangleFishArrayGraphics rectangleFishArray;
    [Header("椭圆形阵型")]
    public OvalFishArrayGraphics ovalFishArray;
}


//三角形阵型
[Serializable]
public class TriangleFishArrayGraphics
{
    public int fishNum;
    public int AngleNumA;
    public int AngleNumB;
    public Vector3 PositionA;
    public Vector3 PositionB;
}

//矩形阵型
[Serializable]
public class RectangleFishArrayGraphics
{
    public int fishNum;
    public Vector3 CenterOfMind;
	[Header("下边长")]
    public float LongSide;
	[Header("左边长")]
    public float WidthSide;
}

//椭圆形阵型
[Serializable]
public class OvalFishArrayGraphics
{
    public int fishNum;
    public Vector3 OvalCenterOfMind;
    public int LongAxis;
	public int ShortAxis;
}
