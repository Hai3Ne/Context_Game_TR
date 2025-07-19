using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.NetworkInformation;
public static class Utility
{
    
    public static float InvShortMax             = 1.0f / short.MaxValue;
    public static Vector3 NearLeftTopPoint      = new Vector3();                   ///LeftTop  RightBottom 两个点
    public static Vector3 NearRightBottomPoint = new Vector3();
    public static Vector3 FarLeftTopPoint = new Vector3();
    public static Vector3 FarRightBottomPoint = new Vector3();
    public static Vector3 NearLeftTopFlagPoint = new Vector3();
    public static Vector3 NearRightBottomFlagPoint = new Vector3();
    public static void GlobalInit()
    {
        NearLeftTopPoint = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, ConstValue.NEAR_Z));
        NearRightBottomPoint = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, ConstValue.NEAR_Z));
        //Vector3 pt0 = new Vector3(-1, 1, -1.0f);
        //Vector3 pt1 = new Vector3(1, -1, -1.0f);
        Vector3 pt2 = new Vector3(-1, 1, 1.0f);
        Vector3 pt3 = new Vector3(1, -1, 1.0f);
        Matrix4x4 mat = Camera.main.projectionMatrix.inverse;
        //NearLeftTopPoint = mat.MultiplyPoint(pt0);
        //NearRightBottomPoint = mat.MultiplyPoint(pt1);
        FarLeftTopPoint = mat.MultiplyPoint(pt2);
        FarRightBottomPoint = mat.MultiplyPoint(pt3);


        //NearLeftTopPoint.z = -NearLeftTopPoint.z;
        //NearRightBottomPoint.z = -NearRightBottomPoint.z;
        FarLeftTopPoint.z = -FarLeftTopPoint.z;
        FarRightBottomPoint.z = -FarRightBottomPoint.z;


        //Debug.LogError("NearLeftTopPoint:" + NearLeftTopPoint);
        //Debug.LogError("NearRightBottomPoint:" + NearRightBottomPoint);
		/*
        NearLeftTopFlagPoint.x = NearLeftTopPoint.x + 2.0f;
        NearLeftTopFlagPoint.y = NearLeftTopPoint.y - 0.6f;
        NearLeftTopFlagPoint.z = NearLeftTopPoint.z;

        NearRightBottomFlagPoint.x = NearRightBottomPoint.x - 2.0f;
        NearRightBottomFlagPoint.y = NearRightBottomPoint.y + 0.6f;
        NearRightBottomFlagPoint.z = NearRightBottomPoint.z;
        */
		MainCam = Camera.main;
    }

	public static Camera MainCam;
    public static Texture2D ScaleTexture(Texture2D source, float scaling)
    {
        int targetWidth = (int)(source.width * scaling);
        int targetHeight = (int)(source.height * scaling);
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        //float incX = (1.0f / (float)targetWidth);
        //float incY = (1.0f / (float)targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    public static bool TryCall<T>(this Action<T> callback, T data)
    {
        if (callback != null)
        {
            callback(data);
            return true;
        }
        return false;
    }

    public static bool TryCall<T, U>(this Action<T, U> callback, T data, U udata)
    {
        if (callback != null)
        {
            callback(data, udata);
            return true;
        }
        return false;
    }

    public static bool TryCall(this Action callback)
    {
        if (callback != null)
        {
            callback();
            return true;
        }
        return false;
    }

    public static bool TryAdd<T>(this List<T> dict, T Key)
    {
        if (dict == null)
        {
            Debug.LogWarning("list 为 null.");
            return false;
        }
        if (Key == null)
        {
            Debug.LogWarning("Key 为 null.");
            return false;
        }
        if (dict.Contains(Key))
        {
            return false;
        }
        dict.Add(Key);
        return true;
    }

    public static bool TryAdd<T, U>(this Dictionary<T, U> dict, T Key, U Value)
    {
        if (dict.ContainsKey(Key))
        {
            dict[Key] = Value;
            return false;
        }
        dict.Add(Key, Value);
        return true;
    }


    public static Rect GetUIScreenRect(UIWidget tex)
    {
        Vector3 pos1 = UICamera.currentCamera.WorldToScreenPoint(tex.worldCorners[0]);
        Vector3 pos2 = UICamera.currentCamera.WorldToScreenPoint(tex.worldCorners[2]);
        return new Rect(pos1.x, pos1.y, (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y));
    }
    public static Rect GetUIScreenRect(UIPanel tex)
    {
        Vector3 pos1 = UICamera.currentCamera.WorldToScreenPoint(tex.worldCorners[0]);
        Vector3 pos2 = UICamera.currentCamera.WorldToScreenPoint(tex.worldCorners[2]);
        return new Rect(pos1.x, pos1.y, (int)(pos2.x - pos1.x), (int)(pos2.y - pos1.y));
    }
    
    public static float ByteToFloat(byte t)
    {
        return t * ConstValue.MIN_REDUCTION;
    }
    public static byte FloatToByte(float f)
    {
        return (byte)(f / ConstValue.MIN_REDUCTION);
    }
    public static void ReductionToFloat(byte scl, byte d1, byte d2, byte d3, out float fscl, float[] duration)
    {
        fscl = scl * ConstValue.INV255;
        duration[0] = d1 * ConstValue.MIN_REDUCTION;
        duration[1] = d2 * ConstValue.MIN_REDUCTION;
        duration[2] = d3 * ConstValue.MIN_REDUCTION;
    }
    public static float TickToFloat(uint tick)
    {
        return tick * 0.001f;
    }



	public static long SecToLongTicks(float secs)
	{
		return (long)(secs * 10000000);
	}

	public static uint Sec2MiliSecond(float secs)
	{
		return (uint)(secs * 1000f);
	}

    public static int GetHash(string str)
    {
        //return str.GetHashCode();
        return UnityEngine.Animator.StringToHash(str);
    }
    //计算字符串字节中字符个数。
    public static int GetByteCharCount(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i += 2)
        {
            if (bytes[i] == 0 && bytes[i + 1] == 0)
                return i;
        }
        return bytes.Length;
    }
    public static Vector3 MulAdd(ref Vector3 p1, ref Vector3 p2, ref Vector3 p3)
    {
        return new Vector3(
            p1.x * p2.x + p3.x,
            p1.y * p2.y + p3.y,
            p1.z * p2.z + p3.z);
    }

    public static Vector3 RandVec3()
    {
        return new Vector3(
            UnityEngine.Random.Range(-1.0f, 1.0f), 
            UnityEngine.Random.Range(-1.0f, 1.0f), 
            UnityEngine.Random.Range(-1.0f, 1.0f));
    }
    public static Vector3 RandVec3XY()
    {
        return new Vector3(
            UnityEngine.Random.Range(-1.0f, 1.0f),
            UnityEngine.Random.Range(-1.0f, 1.0f),
            0);
    }
    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static float RandFloat()
    {
        return UnityEngine.Random.Range(-1.0f, 1.0f);
    }
    public static float RandPosFloat()
    {
        return UnityEngine.Random.Range(0.0f, 1.0f);
    }
    public static Vector3 MulVec(Vector3 p1, Vector3 p2)
    {
        return new Vector3(
            p1.x * p2.x,
            p1.y * p2.y ,
            p1.z * p2.z);
    }
    public static Quaternion MulAdd(ref Quaternion p1, ref Quaternion p2, ref Quaternion p3)
    {
        return new Quaternion(
            p1.x * p2.x + p3.x,
            p1.y * p2.y + p3.y,
            p1.z * p2.z + p3.z,
            p1.w * p2.w + p3.w);
    }
    public static short ToShort(float f)
    {
        return (short)(f + 0.5f);
    }
    public static short FloatToShort(float f)
    {
        return (short)(f / 90.0f * short.MaxValue);
    }
    public static float ShortToFloat(short s)
	{
        return (float)s * ConstValue.InvShortMaxValue * 90;
    }


	public static float ShortToFloat180(short v)
	{
		return (float)v * (1.0f / short.MaxValue) * 180;
	}

	public static short FloatToShort180(float v)
	{
		return (short)(v / 180.0f * short.MaxValue);
	}


    public static Vector3 DivVec3(ref Vector3 v1, ref Vector3 v2)
    {
        return new Vector3(
            v1.x / v2.x,
            v1.y / v2.y,
            v1.z / v2.z);
    }
    public static Quaternion DivQ(ref Quaternion q1, ref Quaternion q2)
    {
        return new Quaternion(
            q1.x / q2.x,
            q1.y / q2.y,
            q1.z / q2.z,
            q1.w / q2.w);
    }
    public static Quaternion SubQ(ref Quaternion q1, ref Quaternion q2)
    {
        return new Quaternion(
            q1.x - q2.x,
            q1.y - q2.y,
            q1.z - q2.z,
            q1.w - q2.w);
    }
    public static Quaternion MinQ(ref Quaternion q1, ref Quaternion q2)
    {
        return new Quaternion(
            Math.Min(q1.x, q2.x),
            Math.Min(q1.y, q2.y),
            Math.Min(q1.z, q2.z),
            Math.Min(q1.w, q2.w)
            );
    }
    public static Quaternion MaxQ(ref Quaternion q1, ref Quaternion q2)
    {
        return new Quaternion(
            Math.Max(q1.x, q2.x),
            Math.Max(q1.y, q2.y),
            Math.Max(q1.z, q2.z),
            Math.Max(q1.w, q2.w)
            );
    }
    public static Vector3 MaxVec3(ref Vector3 v1, ref Vector3 v2)
    {
        return new Vector3(
            Math.Max(v1.x, v2.x),
            Math.Max(v1.y, v2.y),
            Math.Max(v1.z, v2.z)
            );
    }
    public static Vector3 MinVec3(ref Vector3 v1, ref Vector3 v2)
    {
        return new Vector3(
            Math.Min(v1.x, v2.x),
            Math.Min(v1.y, v2.y),
            Math.Min(v1.z, v2.z)
            );
    }
    public static void ListRemoveAt<T>(List<T> list, int idx)
    {
        if(idx != list.Count - 1)
        {
            list[idx] = list[list.Count - 1];
        }
        list.RemoveAt(list.Count - 1);
    }
    public static int ComputePathSampleCount(float dist)
    {
        //int count = (int)(dist * GlobalSetting.SampleFactor);
        //count = Mathf.Max(count, 50);
        //return count;
        return 1;
    }
    public static float LerpFloat(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    public static Vector3 ReadVec3(BinaryReader br)
    {
        return new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
    }
    public static Vector4 ReadVec4(BinaryReader br)
    {
        return new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
    }
    public static Quaternion ReadQuaternion(BinaryReader br)
    {
        return new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
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
		wr.Write (pos.x);
		wr.Write (pos.y);
		wr.Write (pos.z);
	}

	public static void WriteVec4(BinaryWriter wr, Vector4 pos)
	{
		wr.Write (pos.x);
		wr.Write (pos.y);
		wr.Write (pos.z);
		wr.Write (pos.w);
	}

	public static void WriteQuaternion(BinaryWriter wr, Quaternion rot)
	{
		wr.Write (rot.x);
		wr.Write (rot.y);
		wr.Write (rot.z);
		wr.Write (rot.w);
	}

	public static void WriteMatrix4x4(BinaryWriter wr, Matrix4x4 mat)
	{
		WriteVec4 (wr, mat.GetRow (0));
		WriteVec4 (wr, mat.GetRow (1));
		WriteVec4 (wr, mat.GetRow (2));
		WriteVec4 (wr, mat.GetRow (3));
	}


    public static uint VersionToUint(string ver)
    {
        int r1 = ver.IndexOf('_') + 1;
        int r2 = ver.IndexOf('_', r1) + 1;
        int r3 = ver.IndexOf('_', r2) + 1;
        string n1 = ver.Substring(0, r1 - 1);
        string n2 = ver.Substring(r1, r2 - r1 - 1);
        string n3 = ver.Substring(r2, r3 - r2 - 1);
        string n4 = ver.Substring(r3, ver.Length - r3);

        uint v1 = uint.Parse(n1);
        uint v2 = uint.Parse(n2);
        uint v3 = uint.Parse(n3);
        uint v4 = uint.Parse(n4);

        return (v1 << 24) | (v2 << 16) | (v3 << 8) | v4;
    }
    public static byte[] CopyArray(byte[] a, ushort offset, int length)
    {
        byte[] data = new byte[length];
        System.Array.Copy(a, offset, data, 0, length);
        return data;
    }
    public static string IPToString(uint ip)
    {
        return string.Format("{0}.{1}.{2}.{3}", (byte)(ip >> 0), (byte)(ip >> 8), (byte)(ip >> 16), (byte)(ip >> 24));
    }
    public static uint StringToIP(string str)
    {
        int k1 = str.IndexOf('.') + 1;
        int k2 = str.IndexOf('.', k1) + 1;
        int k3 = str.IndexOf('.', k2) + 1;
        uint a = uint.Parse(str.Substring(0, k1 - 1));
        uint b = uint.Parse(str.Substring(k1, k2 - k1 - 1));
        uint c = uint.Parse(str.Substring(k2, k3 - k2 - 1));
        uint d = uint.Parse(str.Substring(k3, str.Length - k3));
        return (a << 24) | (b << 16) | (c << 8) | d;
    }
   
    public static string VerToStr(uint ver)
    {
        return string.Format("{0}_{1}_{2}_{3}", ver >> 24, ver >> 16 & 0xff, ver >> 8 & 0xff, ver & 0xff);
    }
    public static string VerToDotStr(uint ver)
    {
        return string.Format("{0}.{1}.{2}", ver >> 16 & 0xff, ver >> 8 & 0xff, ver & 0xff);
    }

	public static T TryAt<T>(this List<T> list, int index)
	{
		if (list != null && index < list.Count)
			return list [index];
		return default(T);
	}
    public static bool Contains(this uint[] arr, uint t) {
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == t) {
                return true;
            }
        }
        return false;
    }
    public static bool Contains(this byte[] arr, byte t) {
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == t) {
                return true;
            }
        }
        return false;
    }

	static char [] hexChars = "0123456789ABCDEF".ToCharArray ();
	public static string UrlEncode(string str) 
	{
		return UrlEncode(str, Encoding.UTF8);
	}

	public static string UrlEncode (string s, Encoding Enc) 
	{
		if (s == null)
			return null;

		if (s == String.Empty)
			return String.Empty;

		bool needEncode = false;
		int len = s.Length;
		for (int i = 0; i < len; i++) {
			char c = s [i];
			if ((c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z')) {
				if (NotEncoded (c))
					continue;

				needEncode = true;
				break;
			}
		}

		if (!needEncode)
			return s;

		byte [] bytes = new byte[Enc.GetMaxByteCount(s.Length)];
		int realLen = Enc.GetBytes (s, 0, s.Length, bytes, 0);
		return Encoding.ASCII.GetString (UrlEncodeToBytes (bytes, 0, realLen));
	}

	internal static bool NotEncoded (char c)
	{
		return (c == '!' || c == '(' || c == ')' || c == '*' || c == '-' || c == '.' || c == '_'
			#if !NET_4_0
			|| c == '\''
			#endif
		);
	}

	internal static byte[] UrlEncodeToBytes (byte[] bytes, int offset, int count)
	{
		if (bytes == null)
			throw new ArgumentNullException ("bytes");

		int blen = bytes.Length;
		if (blen == 0)
			return new byte [0];

		if (offset < 0 || offset >= blen)
			throw new ArgumentOutOfRangeException("offset");

		if (count < 0 || count > blen - offset)
			throw new ArgumentOutOfRangeException("count");

		MemoryStream result = new MemoryStream (count);
		int end = offset + count;
		for (int i = offset; i < end; i++)
			UrlEncodeChar ((char)bytes [i], result, false);

		return result.ToArray();
	}

	internal static void UrlEncodeChar (char c, Stream result, bool isUnicode) {
		if (c > 255) {
			//FIXME: what happens when there is an internal error?
			//if (!isUnicode)
			//	throw new ArgumentOutOfRangeException ("c", c, "c must be less than 256");
			int idx;
			int i = (int) c;

			result.WriteByte ((byte)'%');
			result.WriteByte ((byte)'u');
			idx = i >> 12;
			result.WriteByte ((byte)hexChars [idx]);
			idx = (i >> 8) & 0x0F;
			result.WriteByte ((byte)hexChars [idx]);
			idx = (i >> 4) & 0x0F;
			result.WriteByte ((byte)hexChars [idx]);
			idx = i & 0x0F;
			result.WriteByte ((byte)hexChars [idx]);
			return;
		}

		if (c > ' ' && NotEncoded (c)) {
			result.WriteByte ((byte)c);
			return;
		}
		if (c==' ') {
			result.WriteByte ((byte)'+');
			return;
		}
		if (	(c < '0') ||
			(c < 'A' && c > '9') ||
			(c > 'Z' && c < 'a') ||
			(c > 'z')) {
			if (isUnicode && c > 127) {
				result.WriteByte ((byte)'%');
				result.WriteByte ((byte)'u');
				result.WriteByte ((byte)'0');
				result.WriteByte ((byte)'0');
			}
			else
				result.WriteByte ((byte)'%');

			int idx = ((int) c) >> 4;
			result.WriteByte ((byte)hexChars [idx]);
			idx = ((int) c) & 0x0F;
			result.WriteByte ((byte)hexChars [idx]);
		}
		else
			result.WriteByte ((byte)c);
	}


	public static T FindValue<K, T>(this Dictionary<K,T> dict, Predicate<T> predicate)
	{
		Dictionary<K,T>.Enumerator emt = dict.GetEnumerator ();
		while (emt.MoveNext ()) {
			if (predicate (emt.Current.Value))
				return emt.Current.Value;
		}
		return default(T);
	}

	public static List<T> FindValues<K, T>(this Dictionary<K,T> dict, Predicate<T> predicate)
	{
		List<T> ls = new List<T> ();
		Dictionary<K,T>.Enumerator emt = dict.GetEnumerator ();
		while (emt.MoveNext ()) {
			if (predicate (emt.Current.Value))
				ls.Add(emt.Current.Value);
		}
		return ls;
	}

	// [2,8,3,7,4]
	public static int Random_Weight(int[] weights)
	{
		int sum = 0;
		for (int i = 0; i < weights.Length; i++) {
			sum += weights [i];
		}
		int index = -1;
		int rval = UnityEngine.Random.Range (0, sum);
		for (int i = 0; i < weights.Length; i++) {
			if (weights [i] <= 0)
				continue;
			if (rval <= weights [i]) {
				index = i;
				break;
			}
			rval -= weights [i];
		}
		return index;
	}

	public static uint MakeServerLCRID(uint lcrCfgID, byte lcrLevel, bool isvalid)
	{
		if (isvalid) {
			return (1u<<31) | ((uint)lcrLevel<<24) | lcrCfgID;
		} else {
			return (0u<<31) | ((uint)lcrLevel<<24) | lcrCfgID;
		}
	}

	public static void UnMakeServerLCRTID(uint serverLauncherType, out uint clientLauncherType, out uint level,  out bool valid)
	{
		clientLauncherType = serverLauncherType & 0xFFFFFF;
		level = (serverLauncherType >> 24) & 0x7f;
		valid = (serverLauncherType >> 31) != 0;
	}

	public static string GetObjName(Transform trans)
	{
		if (trans == null)
			return null;
		if (trans.parent == null)
			return "/"+trans.name;
		else
			return GetObjName(trans.parent) + "/" + trans.name;
		
	}

	public static void SafeClose(this System.Net.Sockets.Socket socket)  
	{  
		if (socket == null)  
			return;  

		if (!socket.Connected)  
			return;  

		try  
		{  
			socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);  
		}  
		catch  
		{  
		}  

		try  
		{  
			socket.Close();  
		}  
		catch  
		{  
		}  
	}

	public static Vector3 ConvertWorldPos(Vector3 wp, Camera fromCam, Camera tarCam, float z = -1f){
		Vector3 screenPos = fromCam.WorldToScreenPoint (wp);
		screenPos.z = z;
		return  tarCam.ScreenToWorldPoint (screenPos);		
	}
}
