using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

using BinaryReadTools =  SimpleByteUtility;
using BinaryWriteTools =  SimpleByteUtility;

public class FishPathConfParser
{
	const uint PathCRC = 323242;
	static int m_NodeCount = 0;
    #region 简化版数据操作
    //简化版解析数据

	public const int PathSplitNum = 10;

	public static byte[] MergerPathParts(List<byte[]> partList){
		partList.Sort(delegate(byte[] A, byte[] B) {
			return A[0].CompareTo(B[0]);
		});
		byte[] fishpahtbytes = null;
		using(MemoryStream ms = new MemoryStream()){
			BinaryWriter bw = new BinaryWriter(ms);
			for (int idx = 0; idx < partList.Count; idx++){
				bw.Write(partList[idx], 1, partList[idx].Length-1);
			}
			fishpahtbytes = ms.GetBuffer ();
			byte[] tmp = new byte[ms.Length];
			Array.Copy (fishpahtbytes, tmp, ms.Length);
			fishpahtbytes = tmp;
		}
		return fishpahtbytes;
	}

	public static List<byte[]> SplitPath2Parts(byte[] data){
		List<byte[]> partList = new List<byte[]> ();
		int perNum = data.Length / PathSplitNum;
		int lastRemind = data.Length - perNum * PathSplitNum;
		int pos = 0;
		for (int i = 0; i < PathSplitNum; i++) {
			int partLen = perNum;
			if (i == PathSplitNum - 1)
				partLen += lastRemind;			
			byte[] partByte = new byte[partLen+1];
			partByte [0] = (byte)i;
			Array.Copy (data, pos, partByte, 1, partLen);
			pos += partLen;
			partList.Add (partByte);
		}
		return partList;
	}

    public static FishPathConfData ParseDataSimple(byte[] bytes) {
        FishPathConfData confData = new FishPathConfData();
        using (MemoryStream ms = new MemoryStream(bytes)) {
            BinaryReader br = new BinaryReader(ms);
            br.ReadUInt32();//uint m_PathCRC = br.ReadUInt32();
            ushort pathCount = br.ReadUInt16();
            ushort pathGroupCount = br.ReadUInt16();
            ushort lcrFishPathGroupCount = br.ReadUInt16();
            ushort lcrFishParaderCount = br.ReadUInt16();

            // 单条路径
            for (ushort i = 0; i < pathCount; ++i) {
                PathLinearInterpolator pi = ReadPathData(br, true);
                PathLinearInterpolator piInv = ReadPathData(br, true);
                confData.m_PathInterpList.Add(pi);
                confData.m_PathInterpListInv.Add(piInv);
            }

            // 多条路径
            for (ushort i = 0; i < pathGroupCount; ++i) {
                short subPathCount = br.ReadInt16();
                PathLinearInterpolator[] pi = ReadPathGroupData(br, subPathCount, true);
                PathLinearInterpolator[] piInv = ReadPathGroupData(br, subPathCount, true);
                confData.m_PathGroupList.Add(pi);
                confData.m_PathGroupListInv.Add(piInv);
            }

            // 鱼群
            for (ushort i = 0; i < lcrFishPathGroupCount; ++i) {
                FishPathGroupData mpathGroupElem = new FishPathGroupData();
                mpathGroupElem.PathGroupIndex = br.ReadUInt16();
				mpathGroupElem.Speed = BinaryReadTools.ReadSingle(br);
                mpathGroupElem.FishCfgID = br.ReadUInt32();
				mpathGroupElem.FishScaling = BinaryReadTools.ReadSingle(br);
				mpathGroupElem.ActionSpeed = BinaryReadTools.ReadSingle(br);

                confData.m_PathParadeDataList.Add(mpathGroupElem);
            }

            // 渔阵
            for (ushort i = 0; i < lcrFishParaderCount; i++) {
                FishParadeData mParadeElem = ReadFishParadeData(br, true);
                confData.m_FishParadeDataList.Add(mParadeElem);
            }
            confData.m_BoLang = ReadPathData(br, true);

            confData.m_BoLang.SetWorldPosition(Vector3.zero);

            uint endmagic = br.ReadUInt32();
            if (endmagic != ConstValue.FILE_END_MAGIC) {
                LogMgr.Log("路径文件结束不正确.");
            }
        }
        return confData;
    }
    //简化版导出数据
    public static byte[] SerializeSimple(FishPathConfData fishData) {
        byte[] buffer = null;
        using (MemoryStream ms = new MemoryStream()) {
            BinaryWriter wr = new BinaryWriter(ms);
            wr.Write(PathCRC);
            wr.Write((ushort)fishData.m_PathInterpList.Count);
            wr.Write((ushort)fishData.m_PathGroupList.Count);
            wr.Write((ushort)fishData.m_PathParadeDataList.Count);
            wr.Write((ushort)fishData.m_FishParadeDataList.Count);

            // 单条路径
            for (int i = 0; i < fishData.m_PathInterpList.Count; ++i) {
                WritePathData(wr, fishData.m_PathInterpList[i], true);
                WritePathData(wr, fishData.m_PathInterpListInv[i], true);
            }

            // 多条路径
            for (int i = 0; i < fishData.m_PathGroupList.Count; ++i) {
                wr.Write((short)fishData.m_PathGroupList[i].Length);
                WritePathGroupData(wr, fishData.m_PathGroupList[i], true);
                WritePathGroupData(wr, fishData.m_PathGroupListInv[i], true);
            }

            // 鱼群
            for (int i = 0; i < fishData.m_PathParadeDataList.Count; ++i) {
                FishPathGroupData pgd = fishData.m_PathParadeDataList[i];
                wr.Write((ushort)pgd.PathGroupIndex);
				BinaryWriteTools.WriteSingle(wr,pgd.Speed);
                wr.Write(pgd.FishCfgID);
				BinaryWriteTools.WriteSingle(wr,pgd.FishScaling);
				BinaryWriteTools.WriteSingle(wr,pgd.ActionSpeed);
            }

            // 渔阵
            for (int i = 0; i < fishData.m_FishParadeDataList.Count; ++i) {
                FishParadeData paradata = fishData.m_FishParadeDataList[i];
                WriteFishParadeData(wr, paradata, true);
            }

            WritePathData(wr, fishData.m_BoLang, true);
            wr.Write(ConstValue.FILE_END_MAGIC);

            buffer = ms.GetBuffer();
            byte[] newBuff = new byte[ms.Length];
            Array.Copy(buffer, newBuff, newBuff.Length);
            buffer = newBuff;
        }
        return buffer;
    }
    #endregion


    public static FishPathConfData ParseData(byte[] bytes)
	{
		FishPathConfData confData = new FishPathConfData ();
		using(MemoryStream ms = new MemoryStream(bytes))
		{
			BinaryReader br = new BinaryReader(ms);
			uint m_PathCRC 					= br.ReadUInt32();
			ushort pathCount 				= br.ReadUInt16();
			ushort pathGroupCount 			= br.ReadUInt16();
			ushort lcrFishPathGroupCount 	= br.ReadUInt16();
			ushort lcrFishParaderCount 		= br.ReadUInt16();

			for (ushort i = 0; i < pathCount; ++i)
			{
				PathLinearInterpolator pi = ReadPathData(br);
				PathLinearInterpolator piInv = ReadPathData(br);
				confData.m_PathInterpList.Add(pi);
				confData.m_PathInterpListInv.Add(piInv);
			}

			for (ushort i = 0; i < pathGroupCount; ++i)
			{
				short subPathCount = br.ReadInt16();
				PathLinearInterpolator[] pi = ReadPathGroupData(br, subPathCount);
				PathLinearInterpolator[] piInv = ReadPathGroupData(br, subPathCount);
				confData.m_PathGroupList.Add(pi);
				confData.m_PathGroupListInv.Add(piInv);
			}

			for (ushort i = 0; i < lcrFishPathGroupCount; ++i) {

				FishPathGroupData mpathGroupElem = new FishPathGroupData ();
				mpathGroupElem.PathGroupIndex = br.ReadUInt16 ();
				mpathGroupElem.Speed = BinaryReadTools.ReadSingle (br);
				mpathGroupElem.FishCfgID = br.ReadUInt32 ();
				mpathGroupElem.FishScaling = BinaryReadTools.ReadSingle (br);
				mpathGroupElem.ActionSpeed = BinaryReadTools.ReadSingle (br);
				mpathGroupElem.ActionUnite = br.ReadBoolean ();
				//重复次数和间隔
				br.ReadUInt16 ();
				br.ReadUInt16 ();

				confData.m_PathParadeDataList.Add (mpathGroupElem);
			}

			for (ushort i = 0; i < lcrFishParaderCount; i++) 
			{
				FishParadeData mParadeElem = ReadFishParadeData (br);
				confData.m_FishParadeDataList.Add (mParadeElem);
			}

			confData.m_BoLang = ReadPathData(br);
			confData.m_DouDongPath = ReadPathData(br);
			short subPathCount2 = br.ReadInt16();
			confData.m_LongJuanFeng = ReadPathGroupData(br, subPathCount2);

			confData.m_BoLang.SetWorldPosition(Vector3.zero);
			confData.m_DouDongPath.SetWorldPosition(Vector3.zero);

			for (int i = 0; i < confData.m_LongJuanFeng.Length; ++i)
				confData.m_LongJuanFeng[i].SetWorldPosition(Vector3.zero);

			uint endmagic = br.ReadUInt32();
			if (endmagic != ConstValue.FILE_END_MAGIC)
			{
				LogMgr.Log("路径文件结束不正确.");
			}
		}
		return confData;
	}

	static PathLinearInterpolator ReadPathData(BinaryReader br,bool is_simple = false)
	{
		PathLinearInterpolator pi = new PathLinearInterpolator();
		pi.pathUDID = br.ReadUInt32 ();
		pi.m_WorldMatrix        = BinaryReadTools.ReadMatrix4x4(br);
		pi.m_WorldRotation      = BinaryReadTools.ReadQuaternion(br);

		pi.m_SampleMaxDistance = br.ReadSingle ();
		pi.m_HasPathEvent       = br.ReadBoolean();
		int sampleCount         = (int)br.ReadUInt16();
		int nodeCount           = (int)br.ReadUInt16();
        ushort keySampleCnt;
        if (is_simple == false) {//简易版数据过滤
            keySampleCnt = br.ReadUInt16();
        } else {
            keySampleCnt = 0;
        }
        ushort kfcount = br.ReadUInt16();

        pi.mEventList.Clear();
		pi.m_SplineDataList     = new SplineSampleData[sampleCount];
        pi.keySamplePoints = new Vector3[keySampleCnt];
        pi.keyPointsAngles = new float[keySampleCnt];
        pi.mAnimCurve = new AnimationCurve();
		for (int i = 0; i < nodeCount; ++i) {
            NodeEvent node = new NodeEvent();
            node.StartStep = br.ReadSingle();
			node.EventType = br.ReadByte();
			node.SubAnimID = br.ReadByte ();
			node.Times = br.ReadUInt16();
            node.AutoTrigger = br.ReadBoolean();
            pi.mEventList.Add(node);
		}
		m_NodeCount += sampleCount;
		for (int j = 0; j < sampleCount; ++j)
		{
			SplineSampleData node = new SplineSampleData();
			node.pos    = BinaryReadTools.ReadVec3(br);
			node.rot 	= BinaryReadTools.ReadQuaternion(br);
            //node.timeScaling = BinaryReadTools.ReadSingle(br);
            //node.nodeIdx = br.ReadInt16();
			pi.m_SplineDataList[j] = node;
		}
        for (int j = 0; j < keySampleCnt; ++j) {
			pi.keySamplePoints[j] = BinaryReadTools.ReadVec3(br);
			pi.keyPointsAngles[j] = BinaryReadTools.ReadSingle(br);
        }
        for (int j = 0; j < kfcount; ++j) {
            Keyframe frame = new Keyframe();
            frame.time = br.ReadSingle();
            frame.value = Math.Max(0.01f, br.ReadSingle());
            //frame.inTangent = br.ReadSingle();
            //frame.outTangent = br.ReadSingle();
            //frame.tangentMode = br.ReadInt32 ();
            pi.mAnimCurve.AddKey(frame);
		}

		return pi;
	}

	static PathLinearInterpolator[] ReadPathGroupData(BinaryReader br,int pathCount,bool is_simple = false)
	{
		PathLinearInterpolator[] piList = new PathLinearInterpolator[pathCount];
		for (int n = 0; n < pathCount; ++n)
		{
            piList[n] = ReadPathData(br, is_simple);
		}
		return piList;
	}

    static FishParadeData ReadFishParadeData(BinaryReader br, bool is_simple = false)
	{
		FishParadeData mParadeElem = new FishParadeData ();
		mParadeElem.FrontPosition = BinaryReadTools.ReadVec3(br);

		mParadeElem.FishParadeId = br.ReadUInt32();

		ushort subPathCount = br.ReadUInt16();
		mParadeElem.PathList = new uint[subPathCount];
		for (ushort j = 0; j < subPathCount; ++j)
		{
			mParadeElem.PathList[j] = br.ReadUInt32();
		}

		ushort groupDataCount = br.ReadUInt16();
		mParadeElem.GroupDataArray = new GroupData[groupDataCount];
		for (ushort j = 0; j < groupDataCount; ++j)
		{
			GroupData gd = new GroupData();
			gd.FishCfgID = br.ReadUInt32();
			gd.FishNum = br.ReadUInt16();
			gd.FishScaling = BinaryReadTools.ReadSingle(br);
			gd.SpeedScaling = BinaryReadTools.ReadSingle(br);
			gd.ActionSpeed = BinaryReadTools.ReadSingle(br);
            if (is_simple == false) {//简易版数据过滤
                gd.ActionUnite = br.ReadBoolean();
            }

            gd.FishShapeID = br.ReadUInt32();
            if (is_simple == false || gd.FishShapeID > 0) {//简易版数据过滤
                gd.Density = br.ReadUInt32();
                if (gd.Density == 0) {
                    gd.Density = 1;
                }
				gd.ShapeOffset = BinaryReadTools.ReadVec3(br);
				gd.ShapeScale = BinaryReadTools.ReadSingle(br);
                if (gd.ShapeScale == 0) {
                    gd.ShapeScale = 1;
                }
            }

			gd.PosList = new Vector3[gd.FishNum];
			gd.DelayList = new float[gd.FishNum];
            if (is_simple == false || gd.FishShapeID == 0) {//简易版数据过滤
                for (int n = 0; n < gd.FishNum; ++n)
					gd.PosList[n] = BinaryReadTools.ReadVec3(br);

                for (int n = 0; n < gd.FishNum; ++n)
					gd.DelayList[n] = BinaryReadTools.ReadSingle(br);
            }

			mParadeElem.GroupDataArray[j] = gd;
		}
        if (is_simple == false) {//简易版数据过滤
            mParadeElem.FishGraphicData = null;
            int cnt = br.ReadInt32();
            if (cnt > 0) {
                mParadeElem.FishGraphicData = br.ReadBytes(cnt);
            }
        }
		return mParadeElem;
	}

	static void WriteFishParadeData(BinaryWriter wr, FishParadeData paradata, bool is_simple = false)
	{
		BinaryWriteTools.WriteVec3 (wr, paradata.FrontPosition);

		wr.Write (paradata.FishParadeId);

		wr.Write ((ushort)paradata.PathList.Length);
		for (int k = 0; k < paradata.PathList.Length; k++) {
			wr.Write (paradata.PathList[k]);
		}
		wr.Write ((ushort)paradata.GroupDataArray.Length);
		for (int k = 0; k < paradata.GroupDataArray.Length; k++) 
		{
            wr.Write(paradata.GroupDataArray[k].FishCfgID);
            wr.Write((ushort)paradata.GroupDataArray[k].FishNum);
			BinaryWriteTools.WriteSingle (wr,paradata.GroupDataArray [k].FishScaling);
			BinaryWriteTools.WriteSingle (wr,paradata.GroupDataArray [k].SpeedScaling);
			BinaryWriteTools.WriteSingle (wr,paradata.GroupDataArray [k].ActionSpeed);
            if (is_simple == false) {//简易版数据过滤
                wr.Write(paradata.GroupDataArray[k].ActionUnite);
            }

            wr.Write(paradata.GroupDataArray[k].FishShapeID);
            if (is_simple == false || paradata.GroupDataArray[k].FishShapeID > 0) {//简易版数据过滤
                wr.Write(paradata.GroupDataArray[k].Density);
				BinaryWriteTools.WriteVec3(wr, paradata.GroupDataArray[k].ShapeOffset);
				BinaryWriteTools.WriteSingle(wr,paradata.GroupDataArray[k].ShapeScale);
            }

            if (is_simple == false || paradata.GroupDataArray[k].FishShapeID == 0) {//简易版数据过滤
                for (int j = 0; j < paradata.GroupDataArray[k].FishNum; j++) {
					BinaryWriteTools.WriteVec3(wr, paradata.GroupDataArray[k].PosList[j]);
                }

                float[] delays = paradata.GroupDataArray[k].DelayList;
                for (int j = 0; j < paradata.GroupDataArray[k].FishNum; j++) {

                    if (delays != null && j < delays.Length) {
						BinaryWriteTools.WriteSingle(wr,delays[j]);
                    } else
						BinaryWriteTools.WriteSingle(wr,0f);
                }
            }
		}

        if (is_simple == false) {//简易版数据过滤
            if (paradata.FishGraphicData != null) {
                wr.Write(paradata.FishGraphicData.Length);
                wr.Write(paradata.FishGraphicData, 0, paradata.FishGraphicData.Length);
            } else {
                wr.Write((int)0);
            }
        }
	}

    static void WritePathData(BinaryWriter wr, PathLinearInterpolator pi, bool is_simple = false)
	{
		wr.Write (pi.pathUDID);
		BinaryWriteTools.WriteMatrix4x4 (wr, pi.m_WorldMatrix);
		BinaryWriteTools.WriteQuaternion (wr, pi.m_WorldRotation);

		wr.Write(pi.m_SampleMaxDistance);
		wr.Write (pi.m_HasPathEvent);
        int sampleCount = pi.m_SplineDataList.Length;
        int nodeCount = pi.mEventList.Count;
		wr.Write((ushort)sampleCount);
		wr.Write((ushort)nodeCount);

        ushort keySamepCount;
        if (is_simple == false) {//简易版数据过滤
            keySamepCount = pi.keySamplePoints != null ? (ushort)pi.keySamplePoints.Length : (ushort)0;
            wr.Write(keySamepCount);
        } else {
            keySamepCount = 0;
        }
        ushort keyfcount = pi.mAnimCurve != null ? (ushort)pi.mAnimCurve.length : (ushort)0;
        wr.Write(keyfcount);

        for (int i = 0; i < nodeCount; ++i) {
            wr.Write(pi.mEventList[i].StartStep);
            wr.Write((byte)pi.mEventList[i].EventType);
            wr.Write(pi.mEventList[i].SubAnimID);
            wr.Write((ushort)pi.mEventList[i].Times);
            wr.Write(pi.mEventList[i].AutoTrigger);
        }

		for (int j = 0; j < sampleCount; ++j)
		{
			BinaryWriteTools.WriteVec3 (wr, pi.m_SplineDataList [j].pos);
			BinaryWriteTools.WriteQuaternion (wr, pi.m_SplineDataList [j].rot);
		}

		for (int j = 0; j < keySamepCount; ++j) {
			BinaryWriteTools.WriteVec3 (wr, pi.keySamplePoints [j]);
			if (pi.keyPointsAngles != null &&  j < pi.keyPointsAngles.Length)
				BinaryWriteTools.WriteSingle (wr,pi.keyPointsAngles [j]);
			else
				BinaryWriteTools.WriteSingle (wr,0f);
		}

        for (int j = 0; j < keyfcount; ++j) {
            wr.Write(pi.mAnimCurve[j].time);
            wr.Write(pi.mAnimCurve[j].value);
            //wr.Write(pi.mAnimCurve[j].inTangent);
            //wr.Write(pi.mAnimCurve[j].outTangent);
            //wr.Write(pi.mAnimCurve[j].tangentMode);
        }
	}

    static void WritePathGroupData(BinaryWriter wr, PathLinearInterpolator[] pathGroup, bool is_simple = false)
	{
		for (int n = 0; n < pathGroup.Length; ++n)
		{
            WritePathData(wr, pathGroup[n], is_simple);
		}
	}

	public static byte[] Serialize(FishPathConfData fishData)
	{
		byte[] buffer = null;
		using (MemoryStream ms = new MemoryStream ()) {
			BinaryWriter wr = new BinaryWriter (ms);
			wr.Write (PathCRC);
			wr.Write((ushort)fishData.m_PathInterpList.Count);
			wr.Write((ushort)fishData.m_PathGroupList.Count);
			wr.Write((ushort)fishData.m_PathParadeDataList.Count);
			wr.Write((ushort)fishData.m_FishParadeDataList.Count);

			// 单条路径
			for (int i = 0; i < fishData.m_PathInterpList.Count; ++i) {
				WritePathData(wr, fishData.m_PathInterpList [i]);
				WritePathData(wr, fishData.m_PathInterpListInv [i]);
			}

			// 多条路径
			for (int i = 0; i < fishData.m_PathGroupList.Count; ++i) 
			{
				wr.Write((short)fishData.m_PathGroupList[i].Length);
				WritePathGroupData (wr, fishData.m_PathGroupList [i]);
				WritePathGroupData (wr, fishData.m_PathGroupListInv [i]);
			}

			// 鱼群
			for (int i = 0; i < fishData.m_PathParadeDataList.Count; ++i) {
				FishPathGroupData pgd = fishData.m_PathParadeDataList [i];
				wr.Write((ushort)pgd.PathGroupIndex);
				BinaryWriteTools.WriteSingle (wr,pgd.Speed);
				wr.Write (pgd.FishCfgID);
				BinaryWriteTools.WriteSingle (wr,pgd.FishScaling);
				BinaryWriteTools.WriteSingle (wr,pgd.ActionSpeed);
				wr.Write (pgd.ActionUnite);
				wr.Write ((ushort)0);
				wr.Write ((ushort)0);
			}

			// 渔阵
			for (int i = 0; i < fishData.m_FishParadeDataList.Count; ++i) {
				FishParadeData paradata = fishData.m_FishParadeDataList [i];
				WriteFishParadeData (wr, paradata);
			}

			WritePathData (wr, fishData.m_BoLang);
			WritePathData (wr, fishData.m_DouDongPath);
			wr.Write((short)fishData.m_LongJuanFeng.Length);
			WritePathGroupData (wr, fishData.m_LongJuanFeng);
			wr.Write (ConstValue.FILE_END_MAGIC);

			buffer = ms.GetBuffer ();
			byte[] newBuff = new byte[ms.Length];
			Array.Copy (buffer, newBuff, newBuff.Length);
			buffer = newBuff;
		}

		return buffer;
	}

	public static byte[] Serialize_OpeningParades(List<OpeningParadeData[]> Data)
	{
		byte[] buff = null;
		using (MemoryStream ms = new MemoryStream ()) 
		{
			BinaryWriter br = new BinaryWriter (ms);
			br.Write ((ushort)Data.Count);
			for (int i = 0; i < Data.Count; i++) {
				OpeningParadeData[] pdata = Data [i];
				br.Write ((ushort)pdata.Length);
				for (int j = 0; j < pdata.Length; j++) 
				{
					br.Write(pdata [j].delay);
					WriteFishParadeData (br, pdata [j].mFishParade);
				}
			}
			br.Write (ConstValue.FILE_END_MAGIC);
			buff = ms.GetBuffer();
			byte[] newbuff = new byte[ms.Length];
			Array.Copy (buff, newbuff, newbuff.Length);
			buff = newbuff;
		}
		return buff;
	}

	public static List<OpeningParadeData[]> UnSerialize_OpeningParades(byte[] buffer)
	{
		List<OpeningParadeData[]> datalist = new List<OpeningParadeData[]> ();
		using (MemoryStream ms = new MemoryStream (buffer)) {
			BinaryReader br = new BinaryReader (ms);
			ushort count = br.ReadUInt16 ();
			for (ushort i = 0; i < count; i++) {
				ushort len = br.ReadUInt16 ();
				OpeningParadeData[] openingArray = new OpeningParadeData[len];
				for (ushort j = 0; j < len; j++) {
					openingArray [j] = new OpeningParadeData ();
					openingArray[j].delay = br.ReadSingle();
					openingArray [j].mFishParade = ReadFishParadeData (br);
				}
				datalist.Add (openingArray);
			}
		}
		return datalist;
	}

	public static byte[] Serialize_BossPath(Dictionary<uint, List<BossPathLinearInterpolator>> BossPathMap)
	{
		byte[] buff = null;
		using (MemoryStream ms = new MemoryStream ()) {
			BinaryWriter br = new BinaryWriter (ms);
			br.Write ((ushort)BossPathMap.Count);
			foreach (var pair in BossPathMap) 
			{
				BossPathLinearInterpolator[] BossPath = pair.Value.ToArray();
				ushort pathCnt = (ushort)BossPath.Length;
				uint bossCfgID = pair.Key;
				br.Write (bossCfgID);
				br.Write (pathCnt);
				for (int i = 0; i < pathCnt; i++) {
					br.Write(BossPath [i].duration);
					br.Write(BossPath [i].delay);
					br.Write(BossPath [i].defaultSwinClip);
					WritePathData (br, BossPath [i].mPath);
					WritePathData (br, BossPath [i].mPathInv);
				}
			}
			br.Write (ConstValue.FILE_END_MAGIC);
			buff = ms.GetBuffer();

			byte[] newbuff = new byte[ms.Length];
			Array.Copy (buff, newbuff, newbuff.Length);
			buff = newbuff;
		}
		return buff;
	}

	public static Dictionary<uint, BossPathLinearInterpolator[]> UnSerialize_BossPath(byte[] buffer)
	{
		Dictionary<uint, BossPathLinearInterpolator[]> bossPathMap = new Dictionary<uint, BossPathLinearInterpolator[]> ();
		using (MemoryStream ms = new MemoryStream (buffer)) {
			BinaryReader br = new BinaryReader (ms);
			ushort bossCnt = br.ReadUInt16 ();
			for (ushort i = 0; i < bossCnt; i++) 
			{
				uint bossCfgID = br.ReadUInt32 ();
				ushort pathcnt = br.ReadUInt16 ();
				BossPathLinearInterpolator[] bossPathList = new BossPathLinearInterpolator[pathcnt];
				for (int j = 0; j < pathcnt; j++) 
				{
					float duration = br.ReadSingle();	
					float delay =  br.ReadSingle();	
					byte defSwinClip = br.ReadByte ();
					PathLinearInterpolator path = ReadPathData (br);
					PathLinearInterpolator pathInv = ReadPathData (br);
					bossPathList [j] = new BossPathLinearInterpolator ();
					bossPathList [j].bossCfgID = bossCfgID;
					bossPathList [j].duration = duration;
					bossPathList [j].delay = delay;
					bossPathList [j].defaultSwinClip = defSwinClip;
					bossPathList [j].mPath = path;
					bossPathList [j].mPathInv = pathInv;
				}
				bossPathMap.Add (bossCfgID, bossPathList);
			}
		}
		return bossPathMap;
	}

}