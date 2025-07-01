#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CalculateTheGraph  : IFishGraphic
{
	public bool IsActive { get; set;}
	public EnuGraphicType graphictype {get;set;}
	public int FishNum {get;set;}
	public Vector3 OrgOffset {get;set;}
	public Vector3 OrgOffsetTwo{ get; set;}

	public int Value0 {get;set;}
	public int Value1 {get;set;}
	public int Value2 {get;set;}
//	public byte[] GraphicData{ set; get;}
	List<Vector3> TrianglePosList = new List<Vector3>();
	List<Vector3> RectanglePosList = new List<Vector3>();
	List<Vector3> OvallePosList = new List<Vector3>();

	public Vector3[] GenerateGraphic ()
	{
		Vector3[] paths = null;
		switch (graphictype) 
		{
			case EnuGraphicType.Triangle:
				paths = CalTriangle ();
				break;

			case EnuGraphicType.Rectangle:
				paths = CalRect ();
				break;

			case EnuGraphicType.Ellipse:
				paths = CalEllipse ();
				break;
		}
		return paths;
	}
	byte[] buffer = null;
	public byte[] GraphicData{ 
		set
		{
			if (value == null)
				return;
			buffer = value;
			using (MemoryStream ms = new MemoryStream (buffer)) {
				BinaryReader br = new BinaryReader (ms);
				graphictype = (EnuGraphicType)br.ReadByte ();
				OrgOffset = Utility.ReadVec3 (br);
				OrgOffsetTwo = Utility.ReadVec3 (br);
				FishNum = br.ReadInt32 ();
				Value0 = br.ReadInt32 ();
				Value1 = br.ReadInt32 ();
				Value2 = br.ReadInt32 ();
			}
		}

		get{
			using (MemoryStream ms = new MemoryStream ()) 
			{
				BinaryWriter wr = new BinaryWriter (ms);

				wr.Write ((byte)graphictype);
				Utility.WriteVec3 (wr, OrgOffset);
				Utility.WriteVec3 (wr, OrgOffsetTwo);
				wr.Write (FishNum);
				wr.Write (Value0);
				wr.Write (Value1);
				wr.Write (Value2);

				buffer = ms.GetBuffer ();
				byte[] newBuff = new byte[ms.Length];
				Array.Copy (buffer, newBuff, newBuff.Length);
				buffer = newBuff;
			}
			return buffer;
		}}


	Vector3[] CalTriangle ()
	{
		TrianglePosList.Clear ();
		Vector3[] paths = null;
		int fishNum = FishNum;
		Vector3 posA = OrgOffset;
		Vector3 posB = OrgOffsetTwo;
		int angleA = Value0;
		int angleB = Value1;

		if (fishNum < 3 || angleA == 0 || angleB == 0 || (angleA + angleB)>=180 ||angleB < 10) {
			return null;
		}

		Vector3 posC = new Vector3 (posA.x, posA.y, posA.z);
		Vector3 posDir = posB - posA;

		float disAB = posDir.magnitude;

		float angleMaA = Mathf.Tan(Mathf.Deg2Rad * angleA);
		float angleMaB = Mathf.Tan(Mathf.Deg2Rad * angleB);
		float angleMaAB = angleMaA + angleMaB;

		if (angleA < 90f && angleB < 90f) {
			posC.y = (disAB*angleMaA*angleMaB)/(angleMaA+angleMaB) + posA.y;
			posC.x = (((disAB * angleMaA * angleMaB) / (angleMaA + angleMaB)) / angleMaA)+ posA.x;
		} else {

			if (angleB < 90f) {
				float l = disAB * Mathf.Abs (angleMaA) * angleMaB;
				float h = l / ((Mathf.Abs (angleMaA)) - angleMaB);
				float len = h / (Mathf.Abs (angleMaA));//B点角度小
				posC.x = posA.x - len;
				posC.y = h + posA.y;
			} else {
				float l = disAB * Mathf.Abs (angleMaB) * angleMaA;
				float h = l / ((Mathf.Abs (angleMaB)) - angleMaA);
				float len = h / (Mathf.Abs (angleMaB));//A点角度小
				posC.x = posB.x + len;
				posC.y = posB.y + h;
			}

		}

		float disAC = Vector3.Distance (posA, posC);
		float disBC = Vector3.Distance (posB, posC);

		int fishNumber = fishNum - 3;
		float sum = disAB + disAC + disBC;
		float equaAB = disAB / sum;
		float equaAC = disAC / sum;
		float equaBC = disBC / sum;

		int ABNum = (int)Math.Round ((double)(fishNumber * equaAB));
		int ACNum = (int)Math.Round ((double)(fishNumber * equaAC));
		int BCNum = (int)Math.Round ((double)(fishNumber * equaBC));
		TrianglePosList.Add(posA);
		TrianglePosList.Add(posB);
		TrianglePosList.Add(posC);

		float radianA = Mathf.Deg2Rad * angleA;
		float radianB = Mathf.Deg2Rad * angleB;

		float disNumCosA = Mathf.Cos (radianA);
		float disNumSinA = Mathf.Sin (radianA);
		float disNumCosB = Mathf.Cos (radianB);
		float disNumSinB = Mathf.Sin (radianB);
		if (ABNum >=1 ) {
			float disABNum = (float)(disAB / (ABNum + 1));
			for (int i = 1; i <= ABNum; i++) 
			{
				Vector3 vec = new Vector3 (posA.x + i * disABNum, posA.y, posA.z);
				TrianglePosList.Add(vec);
			}
		}
		if (ACNum >=1 ) {
			float disACNum = (float)(disAC / (ACNum + 1));
			for (int i = 1; i <= ACNum; i++) 
			{
				Vector3 vec = new Vector3 (posA.x + (i * disACNum * disNumCosA), posA.y + (i * disACNum * disNumSinA), posA.z);
				TrianglePosList.Add(vec);
			}
		}
		if (BCNum >= 1) {
			float disBCNum = (float)(disBC / (BCNum + 1));
			for (int i = 1; i <= BCNum; i++) 
			{
				Vector3 vec = new Vector3 (posB.x - (i * disBCNum * disNumCosB), posB.y + (i * disBCNum * disNumSinB), posB.z);
				TrianglePosList.Add(vec);
			}
		}
		paths = new Vector3[TrianglePosList.Count];
		if (TrianglePosList.Count > 0) {
			for (int i = 0; i < TrianglePosList.Count; i++) {
				paths [i] = TrianglePosList [i];
			}
		}

		return paths;
	}

	Vector3[] CalRect()
	{
		RectanglePosList.Clear ();
		Vector3[] paths = null;
		int fishNum = FishNum;
		Vector3 centerPos = OrgOffset;
		int lSide = Value0;
		int wSide = Value1;

		if (fishNum < 4 || lSide == 0 || wSide == 0 || wSide < 10) {
			return null;
		}
		int totalLength = (int)(lSide + wSide) * 2;
		if (totalLength == 0) {
			return null;
		}

		float lSNum = ((float)lSide / (float)totalLength);
		float wSNum = ((float)wSide / (float)totalLength);

		double lSNumber = (double)(lSNum * (fishNum - 4));
		double wSNumber = (double)(wSNum * (fishNum - 4));

		int lSideequalNum = (int)Math.Round (lSNumber) + 1;//长边上均分多少段
		int wSideequalNum = (int)Math.Round(wSNumber) + 1;//短边上均分多少段
		float LSideLen = lSide / lSideequalNum;
		float wSideLen = wSide / wSideequalNum;
		Vector3 posB = new Vector3 (centerPos.x + lSide, centerPos.y, centerPos.z);
		Vector3 posC = new Vector3 (posB.x, posB.y + wSide, posB.z);
		Vector3 posD = new Vector3 (centerPos.x, centerPos.y + wSide, centerPos.z);
		RectanglePosList.Add (centerPos);
		RectanglePosList.Add (posB);
		RectanglePosList.Add (posC);
		RectanglePosList.Add (posD);
		if (lSideequalNum >= 1) {
			for (int i = 1; i < lSideequalNum; i++) {
				Vector3 vecAB = new Vector3 (centerPos.x + i * LSideLen, centerPos.y, centerPos.z);
				Vector3 vecDC = new Vector3 (posD.x + i * LSideLen, posD.y, posD.z);
				RectanglePosList.Add (vecAB);
				RectanglePosList.Add (vecDC);
			}
		}
		if (wSideequalNum >= 1) {
			for (int i = 1; i < wSideequalNum; i++) {
				Vector3 vecAD = new Vector3 (centerPos.x, centerPos.y + i * wSideLen, centerPos.z);
				Vector3 vecBC = new Vector3 (posB.x, posB.y + i * wSideLen, posB.z);
				RectanglePosList.Add (vecAD);
				RectanglePosList.Add (vecBC);
			}
		}
		paths = new Vector3[RectanglePosList.Count];
		if (RectanglePosList.Count > 0) {
			for (int i = 0; i < RectanglePosList.Count; i++) {
				paths [i] = RectanglePosList [i];
			}
		}

		return paths;
	}


	Vector3[] CalEllipse()
	{
		OvallePosList.Clear ();
		Vector3[] paths = null;
		int fishNum = FishNum;
		Vector3 centerPos = OrgOffset;
		int lAxis = Value0;
		int wAxis = Value1;

		if (fishNum < 4 || lAxis == 0 || wAxis == 0 || wAxis < 10) {
			return null;
		}
		if (lAxis != wAxis) {
			int num = fishNum / 2;//循环次数
			float startLAxis = centerPos.x - lAxis;
			float trmp = ((lAxis * 2) / num);//递增数
			double AB = (Math.Pow (lAxis, 2)) * (Math.Pow (wAxis, 2));
			double Apow = Math.Pow (lAxis, 2);
			for (int i = 0; i < num; i++) {
				double B = (Math.Pow (wAxis, 2)) * (Math.Pow ((startLAxis - centerPos.x), 2));
				double Y = Math.Pow (((AB - B) / Apow), 0.5) + centerPos.y;
				float yy = centerPos.y - ((float)Y - centerPos.y);

				if (i == 0) {
					Vector3 vec = new Vector3 (centerPos.x - lAxis, (float)Y, centerPos.z);
					OvallePosList.Add (vec);
					Vector3 vecc = new Vector3 (centerPos.x + lAxis, yy, centerPos.z);
					OvallePosList.Add (vecc);
				} else {
					Vector3 vec = new Vector3 (startLAxis, (float)Y, centerPos.z);
					OvallePosList.Add (vec);
					Vector3 vecc = new Vector3 (startLAxis, yy, centerPos.z);
					OvallePosList.Add (vecc);
				}
				startLAxis = startLAxis + trmp;

			}
			paths = new Vector3[OvallePosList.Count];
			if (OvallePosList.Count > 0) {
				for (int i = 0; i < OvallePosList.Count; i++) {
					paths [i] = OvallePosList [i];
				}
			}
		} else {
			int num = fishNum;
			float angleEmp = (float)360 / num;
			for (int i = 0; i < num; i++){
				Vector3 vec = new Vector3 (centerPos.x + lAxis * Mathf.Cos (Mathf.Deg2Rad * (angleEmp*i)),centerPos.y + lAxis * Mathf.Sin (Mathf.Deg2Rad * (angleEmp*i)),centerPos.z);
				OvallePosList.Add (vec);
			}
			paths = new Vector3[OvallePosList.Count];
			if (OvallePosList.Count > 0) {
				for (int i = 0; i < OvallePosList.Count; i++) {
					paths [i] = OvallePosList [i];
				}
			}
		}
		return paths;
	}
}
#endif