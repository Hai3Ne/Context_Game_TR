using System;
using UnityEngine;
using System.IO;

public class SimpleByteUtility
{

	public static Vector3 ReadVec3(BinaryReader br)
	{
		return new Vector3(_ReadSingle(br), _ReadSingle(br), _ReadSingle(br));
	}

	public static Vector4 ReadVec4(BinaryReader br)
	{
		return new Vector4(_ReadSingle(br),_ReadSingle(br),_ReadSingle(br),_ReadSingle(br));
	}

	public static Quaternion ReadQuaternion(BinaryReader br)
	{
		return new Quaternion (_ReadSingle (br, 3), _ReadSingle (br, 3), _ReadSingle (br, 3), _ReadSingle (br, 3));
	}

	public static Matrix4x4 ReadMatrix4x4(BinaryReader br)
	{
		Matrix4x4 mat = new Matrix4x4();
		mat.SetRow(0, ReadVec4(br));
		mat.SetRow(1, ReadVec4(br));
		mat.SetRow(2, ReadVec4(br));
		mat.SetRow(3, ReadVec4(br));
		return mat;
	}		

	public static void WriteVec3(BinaryWriter wr, Vector3 pos)
	{
		_WriteSingle (wr, pos.x);
		_WriteSingle (wr, pos.y);
		_WriteSingle (wr, pos.z);
	}

	public static void WriteVec4(BinaryWriter wr, Vector4 pos)
	{
		_WriteSingle (wr, pos.x);
		_WriteSingle (wr, pos.y);
		_WriteSingle (wr, pos.z);
		_WriteSingle (wr, pos.w);
	}

	public static void WriteQuaternion(BinaryWriter wr, Quaternion rot)
	{
		_WriteSingle (wr, rot.x, 3);
		_WriteSingle (wr, rot.y, 3);
		_WriteSingle (wr, rot.z, 3);
		_WriteSingle (wr, rot.w, 3);
	}

	public static void WriteMatrix4x4(BinaryWriter wr, Matrix4x4 mat)
	{
		WriteVec4 (wr, mat.GetRow (0));
		WriteVec4 (wr, mat.GetRow (1));
		WriteVec4 (wr, mat.GetRow (2));
		WriteVec4 (wr, mat.GetRow (3));
	}

	public static float ReadSingle(BinaryReader br){
		return _ReadSingle (br);
	}

	public static void WriteSingle(BinaryWriter bw, float val){
		byte precious = 1;
		float p = Mathf.Pow (10f, precious);
		int num = Mathf.RoundToInt (val * p);
		if (Mathf.Abs (num) > 0x7FFF)
			Debug.LogError ("WriteSingle16 fail");
		bw.Write ((short)num);
	}

	static float _ReadSingle(BinaryReader br, byte precious = 1){
		int s = (int)br.ReadInt16 ();
		float p = Mathf.Pow (10f, precious);
		return (s * 1.0f / p);
	}


	static void _WriteSingle(BinaryWriter bw, float val, byte precious = 1){
		float p = Mathf.Pow (10f, precious);
		int num = Mathf.RoundToInt (val * p);
		bw.Write ((short)num);
	}

}
