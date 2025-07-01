using System;
using UnityEngine;
using System.IO;

public static class ByteUtility
{
	public static Vector3 ReadVec3(BinaryReader br)
	{
		byte[] head = br.ReadBytes (3);
		return new Vector3(br._ReadSingle(head[0]), br._ReadSingle(head[1]), br._ReadSingle(head[2]));
	}

	public static Vector4 ReadVec4(BinaryReader br)
	{
		byte[] head = br.ReadBytes (4);
		return new Vector4(br._ReadSingle(head[0]), br._ReadSingle(head[1]), br._ReadSingle(head[2]), br._ReadSingle(head[3]));
	}

	public static Quaternion ReadQuaternion(BinaryReader br)
	{
		byte[] head = br.ReadBytes (4);
		return new Quaternion(br._ReadSingle(head[0], 3), br._ReadSingle(head[1], 3), br._ReadSingle(head[2], 3), br._ReadSingle(head[3], 3));
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

	static float _ReadSingle(this BinaryReader br, byte occopyBit = 1, byte precious = 1){
		int s = 0;
		float p = Mathf.Pow (10f, precious);
		if (occopyBit == 0)
			s = (int)br.ReadSByte ();
		else if (occopyBit == 1)
			s = (short)br.ReadInt16 ();
		else
			s = br.ReadInt32 ();
		return (s * 1.0f / p);
	}

	public static float ReadSingle(BinaryReader br){
		byte precious = 1;
		short s = br.ReadInt16();
		float p = Mathf.Pow (10f, precious);
		return (s * 1.0f / p);
	}

	public static void WriteVec3(BinaryWriter wr, Vector3 pos)
	{
		byte[] head = GetVectorHead (pos);
		wr.Write (head, 0, head.Length);
		wr._WriteSingle (pos.x);
		wr._WriteSingle (pos.y);
		wr._WriteSingle (pos.z);
	}

	public static void WriteVec4(BinaryWriter wr, Vector4 pos)
	{
		byte[] head = GetVectorHead (pos);
		wr.Write (head, 0, head.Length);
		wr._WriteSingle (pos.x);
		wr._WriteSingle (pos.y);
		wr._WriteSingle (pos.z);
		wr._WriteSingle (pos.w);
	}

	public static void WriteQuaternion(BinaryWriter wr, Quaternion rot)
	{
		byte[] head = GetVectorHead (rot);
		wr.Write (head, 0, head.Length);
		wr._WriteSingle (rot.x, 3);
		wr._WriteSingle (rot.y, 3);
		wr._WriteSingle (rot.z, 3);
		wr._WriteSingle (rot.w, 3);
	}

	public static void WriteMatrix4x4(BinaryWriter wr, Matrix4x4 mat)
	{
		WriteVec4 (wr, mat.GetRow (0));
		WriteVec4 (wr, mat.GetRow (1));
		WriteVec4 (wr, mat.GetRow (2));
		WriteVec4 (wr, mat.GetRow (3));
	}

	static void _WriteSingle(this BinaryWriter bw, float val, byte precious = 1){
		float p = Mathf.Pow (10f, precious);
		int num = Mathf.RoundToInt (val * p);
		if (Mathf.Abs (num) <= 0x7F) {
			bw.Write((sbyte)num);
		} else if (Mathf.Abs (num) <= 0x7FFF) {
			bw.Write ((short)num);
		} else {
			Debug.LogError ("WriteSingle fail");
			bw.Write (num);
		}
	}

	public static void WriteSingle(BinaryWriter bw, float val){
		byte precious = 1;
		float p = Mathf.Pow (10f, precious);
		int num = Mathf.RoundToInt (val * p);
		if (Mathf.Abs (num) > 0x7FFF)
			Debug.LogError ("WriteSingle16 fail");
		bw.Write ((short)num);
	}

	static byte[] GetVectorHead(Vector4 v){
		byte[] head = new byte[4];
		head [0] = Mathf.Abs (Mathf.RoundToInt(v.x * 10f)) > 0x7F ? (byte)1 : (byte)0;
		head [1] = Mathf.Abs (Mathf.RoundToInt(v.y * 10f)) > 0x7F ? (byte)1 : (byte)0;
		head [2] = Mathf.Abs (Mathf.RoundToInt(v.z * 10f)) > 0x7F ? (byte)1 : (byte)0;
		head [3] = Mathf.Abs (Mathf.RoundToInt(v.w * 10f)) > 0x7F ? (byte)1 : (byte)0;
		return head;
	}

	static byte[] GetVectorHead(Vector3 v){
		byte[] head = new byte[3];
		head [0] = Mathf.Abs (Mathf.RoundToInt(v.x * 10f)) > 0x7F ? (byte)1 : (byte)0;
		head [1] = Mathf.Abs (Mathf.RoundToInt(v.y * 10f)) > 0x7F ? (byte)1 : (byte)0;
		head [2] = Mathf.Abs (Mathf.RoundToInt(v.z * 10f)) > 0x7F ? (byte)1 : (byte)0;
		return head;
	}
	static byte[] GetVectorHead(Quaternion v){
		byte[] head = new byte[4];
		head [0] = Mathf.Abs (Mathf.RoundToInt(v.x * 1000f)) > 0x7F ? (byte)1 : (byte)0;
		head [1] = Mathf.Abs (Mathf.RoundToInt(v.y * 1000f)) > 0x7F ? (byte)1 : (byte)0;
		head [2] = Mathf.Abs (Mathf.RoundToInt(v.z * 1000f)) > 0x7F ? (byte)1 : (byte)0;
		head [3] = Mathf.Abs (Mathf.RoundToInt(v.w * 1000f)) > 0x7F ? (byte)1 : (byte)0;
		return head;
	}
}
