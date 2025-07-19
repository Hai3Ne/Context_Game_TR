#if UNITY_EDITOR
using System;
using UnityEngine;
public enum EnuGraphicType
{
	Rectangle, // 矩形
	Triangle, // 三角形
	Ellipse,// 椭圆

}

public interface IFishGraphic
{
	bool IsActive { get; set;}
	byte[] GraphicData{ set; get;}
	EnuGraphicType graphictype {get;set;}
	int FishNum {get;set;}
	Vector3 OrgOffset {get;set;}
	Vector3 OrgOffsetTwo{ get; set;}

	int Value0 {get;set;}
	int Value1 {get;set;}
	int Value2 {get;set;}


	Vector3[] GenerateGraphic ();
}

public static class FishGraphicFactory
{
	public static IFishGraphic Create()
	{
		return new CalculateTheGraph();
	}
}
#endif