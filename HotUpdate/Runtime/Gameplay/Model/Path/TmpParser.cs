using System;
using UnityEngine;
using System.IO;
using System.Text;
public class TmpParser
{
  const uint PathCRC = 323242;
  static int m_NodeCount = 0;
  public static FishPathConfData ParseData(byte[] bytes)
  {
		return null;
    /*
    FishPathConfData confData = new FishPathConfData ();
    using(MemoryStream ms = new MemoryStream(bytes))
    {
      BinaryReader br = new BinaryReader(ms);
      uint m_PathCRC = br.ReadUInt32();
      ushort pathCount = br.ReadUInt16();
      ushort pathGroupCount = br.ReadUInt16();
      ushort fishGroupCount = br.ReadUInt16();
      ushort fishDataCount = br.ReadUInt16();

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

      for (ushort i = 0; i < fishDataCount; ++i)
      {
        ResFishData fd = new ResFishData();
        fd.Size = Utility.ReadVec3(br);
        Utility.ReadQuaternion(br);
        for (int j = 0; j < (int)FishClipType.CLIP_MAX; ++j)
          fd.ClipLength[j] = br.ReadSingle();
        confData.m_FishList.Add(fd);
      }
      for (ushort i = 0; i < fishGroupCount; ++i)
      {
        GroupDataList gdl = new GroupDataList();
        confData.m_FishGroupList.Add(gdl);
        bool bFishPathGroup = br.ReadBoolean();
        if (bFishPathGroup)
        {
          gdl.PathGroupData = new FishPathGroupData();
          gdl.PathGroupData.PathGroupIndex = br.ReadUInt16();
          gdl.PathGroupData.Speed = br.ReadSingle();
		  gdl.PathGroupData.FishCfgID = br.ReadByte();
          gdl.PathGroupData.FishScaling = br.ReadSingle();
          gdl.PathGroupData.ActionSpeed = br.ReadSingle();
          gdl.PathGroupData.ActionUnite = br.ReadBoolean();
          //重复次数和间隔
          br.ReadUInt16();
          br.ReadUInt16();
        }
        else
        {
          gdl.FrontPosition = Utility.ReadVec3(br);
          ushort subPathCount = br.ReadUInt16();
          gdl.PathList = new ushort[subPathCount];
          for (ushort j = 0; j < subPathCount; ++j)
          {
            gdl.PathList[j] = br.ReadUInt16();
          }
          ushort groupDataCount = br.ReadUInt16();
          gdl.GroupDataArray = new GroupData[groupDataCount];
          for (ushort j = 0; j < groupDataCount; ++j)
          {
            GroupData gd = new GroupData();
			gd.FishCfgID = br.ReadByte ();//br.ReadUInt32();
            gd.FishNum = br.ReadUInt16();
            gd.FishScaling = br.ReadSingle();
            gd.SpeedScaling = br.ReadSingle();
            gd.ActionSpeed = br.ReadSingle();
            gd.ActionUnite = br.ReadBoolean();
            gd.PosList = new Vector3[gd.FishNum];
            for (int n = 0; n < gd.FishNum; ++n)
              gd.PosList[n] = Utility.ReadVec3(br);
            gdl.GroupDataArray[j] = gd;

          }
        }// end if (pd.FishGroupByPathGroup)
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

  static PathLinearInterpolator ReadPathData(BinaryReader br)
  {
    PathLinearInterpolator pi = new PathLinearInterpolator();
    pi.m_WorldMatrix        = Utility.ReadMatrix4x4(br);
    pi.m_WorldRotation      = Utility.ReadQuaternion(br);

    pi.m_MaxDistance        = br.ReadSingle();
    pi.m_SampleMaxDistance  = br.ReadSingle();
    pi.m_HasPathEvent       = br.ReadBoolean();
    int sampleCount         = br.ReadInt32();
    int nodeCount           = br.ReadInt32();
		int keySampleCnt = 0;//br.ReadInt32 ();
    pi.m_NodeList           = new LinearNodeData[nodeCount];
    pi.m_SplineDataList     = new SplineSampleData[sampleCount];
    pi.keySamplePoints   =new Vector3[keySampleCnt];
    for (int i = 0; i < nodeCount; ++i)
    {
      LinearNodeData node = new LinearNodeData();
      node.EventType = br.ReadByte();
      node.Times = br.ReadUInt16();
      pi.m_NodeList[i] = node;
    }
    m_NodeCount += sampleCount;
    for (int j = 0; j < sampleCount; ++j)
    {
      SplineSampleData node = new SplineSampleData();
      node.pos    = Utility.ReadVec3(br);
      node.rot = Utility.ReadQuaternion(br);
      node.timeScaling = br.ReadSingle();
      node.nodeIdx = br.ReadInt16();
      pi.m_SplineDataList[j] = node;
    }
    for (int j = 0; j < keySampleCnt; ++j) {
      pi.keySamplePoints [j] = Utility.ReadVec3 (br);
    }
    return pi;
  }

  static PathLinearInterpolator[] ReadPathGroupData(BinaryReader br,int pathCount)
  {
    PathLinearInterpolator[] piList = new PathLinearInterpolator[pathCount];
    for (int n = 0; n < pathCount; ++n)
    {
      piList[n] = ReadPathData(br);
    }
    return piList;
  }

  static void WritePathData(BinaryWriter wr, PathLinearInterpolator pi)
  {
    Utility.WriteMatrix4x4 (wr, pi.m_WorldMatrix);
    Utility.WriteQuaternion (wr, pi.m_WorldRotation);

    wr.Write (pi.m_MaxDistance);
    wr.Write (pi.m_SampleMaxDistance);
    wr.Write (pi.m_HasPathEvent);
    int sampleCount = pi.m_SplineDataList.Length;
    int nodeCount = pi.m_NodeList.Length;
    int keySamepCount = pi.keySamplePoints != null ? pi.keySamplePoints.Length : 0;
    wr.Write (sampleCount);
    wr.Write (nodeCount);
    wr.Write (keySamepCount);

    for (int i = 0; i < nodeCount; ++i)
    {
      wr.Write((byte)pi.m_NodeList[i].EventType);
      wr.Write ((ushort)pi.m_NodeList [i].Times);
    }

    for (int j = 0; j < sampleCount; ++j)
    {
      Utility.WriteVec3 (wr, pi.m_SplineDataList [j].pos);
      Utility.WriteQuaternion (wr, pi.m_SplineDataList [j].rot);
      wr.Write (pi.m_SplineDataList [j].timeScaling);
      wr.Write (pi.m_SplineDataList [j].nodeIdx);
    }

    for (int j = 0; j < keySamepCount; ++j) {
      Utility.WriteVec3 (wr, pi.keySamplePoints [j]);
    }

  }

  static void WritePathGroupData(BinaryWriter wr, PathLinearInterpolator[] pathGroup)
  {
    for (int n = 0; n < pathGroup.Length; ++n)
    {
      WritePathData(wr, pathGroup[n]);
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
      wr.Write((ushort)fishData.m_FishGroupList.Count);
      wr.Write((ushort)fishData.m_FishList.Count);

      for (int i = 0; i < fishData.m_PathInterpList.Count; ++i) {
        WritePathData(wr, fishData.m_PathInterpList [i]);
        WritePathData(wr, fishData.m_PathInterpListInv [i]);
      }

      for (int i = 0; i < fishData.m_PathGroupList.Count; ++i) 
      {
        wr.Write((short)fishData.m_PathGroupList[i].Length);
        WritePathGroupData (wr, fishData.m_PathGroupList [i]);
        WritePathGroupData (wr, fishData.m_PathGroupListInv [i]);
      }

      for (int i = 0; i < fishData.m_FishList.Count; ++i) 
      {
        Utility.WriteVec3(wr, fishData.m_FishList [i].Size);
        Utility.WriteQuaternion (wr, Quaternion.identity);
        for (int j = 0; j < (int)FishClipType.CLIP_MAX; ++j)
          wr.Write (fishData.m_FishList [i].ClipLength [j]);
      }

      for (int i = 0; i < fishData.m_FishGroupList.Count; ++i) {
        GroupDataList gdl = fishData.m_FishGroupList [i];
        bool isGroup = gdl.PathGroupData != null;
        wr.Write (isGroup);
        if (isGroup) {
          wr.Write((ushort)gdl.PathGroupData.PathGroupIndex);
          wr.Write (gdl.PathGroupData.Speed);
          wr.Write(gdl.PathGroupData.FishCfgID);
          wr.Write(gdl.PathGroupData.FishScaling);
          wr.Write (gdl.PathGroupData.ActionSpeed);
          wr.Write (gdl.PathGroupData.ActionUnite);
          wr.Write ((ushort)0);
          wr.Write ((ushort)0);
        } else {
          Utility.WriteVec3 (wr, gdl.FrontPosition);
          wr.Write ((ushort)gdl.PathList.Length);
          for (int k = 0; k < gdl.PathList.Length; k++)
          {
            wr.Write((ushort)gdl.PathList [k]);
          }
          wr.Write ((ushort)gdl.GroupDataArray.Length);
          for (int k = 0; k < gdl.GroupDataArray.Length; k++) {
            wr.Write(gdl.GroupDataArray [k].FishCfgID);
            wr.Write ((ushort)gdl.GroupDataArray [k].FishNum);
            wr.Write (gdl.GroupDataArray [k].FishScaling);
            wr.Write (gdl.GroupDataArray [k].SpeedScaling);
            wr.Write (gdl.GroupDataArray [k].ActionSpeed);
            wr.Write (gdl.GroupDataArray [k].ActionUnite);
            for (int j = 0; j < gdl.GroupDataArray [k].FishNum; j++) {
              Utility.WriteVec3 (wr, gdl.GroupDataArray [k].PosList [j]);
            }
          }
        }
      }
      WritePathData (wr, fishData.m_BoLang);
      WritePathData (wr, fishData.m_DouDongPath);
      wr.Write((short)fishData.m_LongJuanFeng.Length);
      WritePathGroupData (wr, fishData.m_LongJuanFeng);
      wr.Write (ConstValue.FILE_END_MAGIC);

      buffer = ms.GetBuffer ();
    }

    return buffer;
  }

  public static byte[] Serialize2(FishPathConfData fishData)
  {
    byte[] buffer = null;
    using (MemoryStream ms = new MemoryStream ()) {
      BinaryWriter wr = new BinaryWriter (ms);
      wr.Write (PathCRC);
      wr.Write((ushort)fishData.m_PathInterpList.Count);
      wr.Write((ushort)fishData.m_PathGroupList.Count);
      wr.Write((ushort)fishData.m_PathGroupDataList.Count);
      wr.Write((ushort)fishData.m_FishParadeDataList.Count);
      wr.Write((ushort)fishData.m_FishList.Count);

      for (int i = 0; i < fishData.m_PathInterpList.Count; ++i) {
        WritePathData(wr, fishData.m_PathInterpList [i]);
        WritePathData(wr, fishData.m_PathInterpListInv [i]);
      }

      for (int i = 0; i < fishData.m_PathGroupList.Count; ++i) 
      {
        wr.Write((short)fishData.m_PathGroupList[i].Length);
        WritePathGroupData (wr, fishData.m_PathGroupList [i]);
        WritePathGroupData (wr, fishData.m_PathGroupListInv [i]);
      }

      for (int i = 0; i < fishData.m_FishList.Count; ++i) 
      {
        Utility.WriteVec3(wr, fishData.m_FishList [i].Size);
        Utility.WriteQuaternion (wr, Quaternion.identity);
        for (int j = 0; j < (int)FishClipType.CLIP_MAX; ++j)
          wr.Write (fishData.m_FishList [i].ClipLength [j]);
      }

      for (int i = 0; i < fishData.m_PathGroupDataList.Count; ++i) {
        FishPathGroupData pgd = fishData.m_PathGroupDataList [i];
        wr.Write((ushort)pgd.PathGroupIndex);
        wr.Write (pgd.Speed);
        wr.Write (pgd.FishCfgID);
        wr.Write (pgd.FishScaling);
        wr.Write (pgd.ActionSpeed);
        wr.Write (pgd.ActionUnite);
        wr.Write ((ushort)0);
        wr.Write ((ushort)0);
      }

      for (int i = 0; i < fishData.m_FishParadeDataList.Count; ++i) {
        FishParadeData paradata = fishData.m_FishParadeDataList [i];
        Utility.WriteVec3 (wr, paradata.FrontPosition);
        for (int k = 0; k < paradata.GroupDataArray.Length; k++) 
        {
          wr.Write (paradata.GroupDataArray [k].FishCfgID);
          wr.Write ((ushort)paradata.GroupDataArray [k].FishNum);
          wr.Write (paradata.GroupDataArray [k].FishScaling);
          wr.Write (paradata.GroupDataArray [k].SpeedScaling);
          wr.Write (paradata.GroupDataArray [k].ActionSpeed);
          wr.Write (paradata.GroupDataArray [k].ActionUnite);
          for (int j = 0; j < paradata.GroupDataArray [k].FishNum; j++) {
            Utility.WriteVec3 (wr, paradata.GroupDataArray [k].PosList [j]);
          }

          float[] delays = paradata.GroupDataArray [k].PostLaunchList;
          for (int j = 0; j < paradata.GroupDataArray [k].FishNum; j++) {
            
            if (delays != null && j < delays.Length) {
              wr.Write (delays [j]);
            } else
              wr.Write (0f);
          }
        }
      }

      WritePathData (wr, fishData.m_BoLang);
      WritePathData (wr, fishData.m_DouDongPath);
      wr.Write((short)fishData.m_LongJuanFeng.Length);
      WritePathGroupData (wr, fishData.m_LongJuanFeng);
      wr.Write (ConstValue.FILE_END_MAGIC);

      buffer = ms.GetBuffer ();
    }

    return buffer;
     //*/
  }
 
}